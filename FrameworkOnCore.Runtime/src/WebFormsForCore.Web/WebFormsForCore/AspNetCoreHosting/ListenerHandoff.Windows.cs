#if NETCOREAPP

using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace System.Web.Hosting
{
	public static partial class ListenerHandoff
	{
		// The pipe's messages: the worker asks for a connection (Ask), the supervisor sends one (its endpoint's number, the
		// length and the bytes of its WSAPROTOCOL_INFOW, duplicated for the worker's process), the worker says it has made
		// its socket of it (Taken): only then the supervisor closes its own. A connection not taken (the worker stopping, or
		// ended) is the next worker's.
		const byte Ask = (byte)'A';
		const byte Taken = (byte)'K';
		// sizeof(WSAPROTOCOL_INFOW)
		const int ProtocolInfoSize = 628;

		/// <summary>
		/// Windows, the process that runs the workers: the connections of the application's endpoints, accepted here (a thread
		/// an endpoint, synchronous accepts: no completion port) and kept in order until a worker takes them, one at a time,
		/// through the named pipe of its generation. A worker that ends leaves the connections not yet taken to the next.
		/// </summary>
		internal sealed class WindowsBroker : IDisposable
		{
			readonly List<(string Key, Socket Socket)> listeners;
			readonly LinkedList<(int Index, Socket Socket)> waiting = new();
			readonly SemaphoreSlim available = new(0);
			readonly object gate = new();
			NamedPipeServerStream pipe;
			CancellationTokenSource generation;
			Thread serving;
			volatile bool disposed;

			internal WindowsBroker(List<(string Key, Socket Socket)> listeners)
			{
				this.listeners = listeners;
				for (var i = 0; i < listeners.Count; i++)
				{
					var index = i;
					new Thread(() => AcceptLoop(index)) { IsBackground = true, Name = "WebFormsForCore accept " + listeners[i].Key }.Start();
				}
			}

			/// <summary>The connections waiting for a worker (tests).</summary>
			internal int Waiting { get { lock (waiting) return waiting.Count; } }

			void AcceptLoop(int index)
			{
				var listener = listeners[index].Socket;
				while (!disposed)
				{
					Socket accepted;
					try { accepted = listener.Accept(); }
					catch (Exception e) when (e is SocketException or ObjectDisposedException or InvalidOperationException)
					{
						if (disposed || e is not SocketException) return;
						continue; // a connection reset before it was accepted
					}
					Enqueue(index, accepted, front: false);
				}
			}

			void Enqueue(int index, Socket socket, bool front)
			{
				lock (waiting)
				{
					if (front) waiting.AddFirst((index, socket));
					else waiting.AddLast((index, socket));
				}
				available.Release();
			}

			/// <summary>The pipe of the next worker (its name for the worker's environment).</summary>
			internal string NextPipe()
			{
				EndGeneration();
				var name = "WebFormsForCore-" + Environment.ProcessId + "-" + Guid.NewGuid().ToString("N");
				lock (gate)
				{
					pipe = new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
					generation = new CancellationTokenSource();
				}
				return name;
			}

			/// <summary>The worker (its process) of the pipe made last takes the connections.</summary>
			internal void Serve(int processId)
			{
				lock (gate)
				{
					if (pipe == null) return;
					var (stream, token) = (pipe, generation.Token);
					serving = new Thread(() => Dispatch(stream, processId, token)) { IsBackground = true, Name = "WebFormsForCore handoff" };
					serving.Start();
				}
			}

			void Dispatch(NamedPipeServerStream stream, int processId, CancellationToken token)
			{
				try { stream.WaitForConnectionAsync(token).GetAwaiter().GetResult(); }
				catch (Exception e) when (e is IOException or ObjectDisposedException or OperationCanceledException) { return; }
				var info = new byte[ProtocolInfoSize];
				var header = new byte[8];
				while (!token.IsCancellationRequested)
				{
					int request;
					try { request = stream.ReadByte(); }
					catch (Exception e) when (e is IOException or ObjectDisposedException) { return; }
					if (request != Ask) return; // the worker closed the pipe (stopping, ended)

					(int Index, Socket Socket) next;
					try
					{
						available.Wait(token);
						lock (waiting)
						{
							next = waiting.First.Value;
							waiting.RemoveFirst();
						}
					}
					catch (OperationCanceledException) { return; }

					try
					{
						if (WSADuplicateSocketW(next.Socket.SafeHandle, (uint)processId, info) != 0)
							throw new SocketException(Marshal.GetLastPInvokeError());
						BitConverter.TryWriteBytes(header.AsSpan(0, 4), next.Index);
						BitConverter.TryWriteBytes(header.AsSpan(4, 4), info.Length);
						stream.Write(header, 0, header.Length);
						stream.Write(info, 0, info.Length);
						stream.Flush();
						if (stream.ReadByte() != Taken) throw new IOException("the worker did not take the connection");
					}
					catch (Exception e) when (e is IOException or SocketException or ObjectDisposedException or InvalidOperationException)
					{
						// Not the worker's: the next worker's, first.
						Enqueue(next.Index, next.Socket, front: true);
						return;
					}
					// The worker's socket is the connection's now; this process's handle of it is closed.
					next.Socket.Dispose();
				}
			}

			/// <summary>The worker of the pipe ended (or the next one starts): the connections not taken wait.</summary>
			internal void EndGeneration()
			{
				Thread thread;
				lock (gate)
				{
					generation?.Cancel();
					pipe?.Dispose();
					pipe = null;
					generation = null;
					thread = serving;
					serving = null;
				}
				thread?.Join(5000);
			}

			public void Dispose()
			{
				disposed = true;
				EndGeneration();
				lock (waiting)
				{
					foreach (var (_, socket) in waiting) socket.Dispose();
					waiting.Clear();
				}
			}
		}

		/// <summary>
		/// Windows, the worker: Kestrel's transport. The endpoints given (ListenerHandoff's) take the connections the supervisor
		/// accepted, through its pipe; the others are bound by the socket transport, as without this. One receiver asks for the
		/// connections, one at a time, and queues each for its endpoint's listener (Kestrel's accept); StopAll ends it (the
		/// application restarting: the next connections are the next worker's), the connections taken already stay Kestrel's.
		/// </summary>
		internal sealed class WindowsListenerFactory : IConnectionListenerFactory, IConnectionListenerFactorySelector
		{
			static WindowsListenerFactory current;

			readonly SocketTransportFactory sockets;
			readonly SocketConnectionContextFactory contexts;
			readonly Dictionary<int, Channel<Socket>> channels = new();
			readonly object gate = new();
			NamedPipeClientStream pipe;
			Thread receiver;
			bool stopping;

			public WindowsListenerFactory(IOptions<SocketTransportOptions> options, ILoggerFactory loggers)
			{
				sockets = new SocketTransportFactory(options, loggers);
				contexts = new SocketConnectionContextFactory(new SocketConnectionFactoryOptions(), loggers.CreateLogger("WebFormsForCore.ListenerHandoff"));
				// A queue an endpoint given, from the start: a connection may come before Kestrel has bound its endpoint.
				foreach (var index in windowsEndpoints.Values) channels[index] = Channel.CreateUnbounded<Socket>();
				current = this;
			}

			public bool CanBind(EndPoint endpoint) => true;

			public async ValueTask<IConnectionListener> BindAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
			{
				if (endpoint is IPEndPoint ip && windowsEndpoints.TryGetValue(Key(ip), out var index))
				{
					StartReceiving();
					return new Listener(this, ip, channels[index]);
				}
				return await sockets.BindAsync(endpoint, cancellationToken).ConfigureAwait(false);
			}

			void StartReceiving()
			{
				lock (gate)
				{
					if (receiver != null || stopping) return;
					receiver = new Thread(Receive) { IsBackground = true, Name = "WebFormsForCore handoff" };
					receiver.Start();
				}
			}

			void Receive()
			{
				try
				{
					var stream = new NamedPipeClientStream(".", windowsPipe, PipeDirection.InOut, PipeOptions.None);
					lock (gate)
					{
						if (stopping) { stream.Dispose(); return; }
						pipe = stream;
					}
					stream.Connect(30_000);
					var header = new byte[8];
					while (true)
					{
						stream.WriteByte(Ask);
						stream.Flush();
						stream.ReadExactly(header);
						var index = BitConverter.ToInt32(header, 0);
						var info = new byte[BitConverter.ToInt32(header, 4)];
						stream.ReadExactly(info);
						lock (gate)
						{
							// Stopping: not taken (no answer), the supervisor gives it to the next worker.
							if (stopping) return;
							var socket = new Socket(new SocketInformation { ProtocolInformation = info, Options = SocketInformationOptions.Connected });
							try
							{
								stream.WriteByte(Taken);
								stream.Flush();
							}
							catch (Exception e) when (e is IOException or ObjectDisposedException)
							{
								socket.Dispose(); // the supervisor's handle stays: the next worker's
								return;
							}
							if (!channels.TryGetValue(index, out var channel) || !channel.Writer.TryWrite(socket)) socket.Dispose();
						}
					}
				}
				catch (Exception e) when (e is IOException or ObjectDisposedException or EndOfStreamException or SocketException or TimeoutException or InvalidOperationException)
				{
					// The pipe closed: the supervisor ended, or StopAll.
				}
				finally
				{
					lock (gate) foreach (var channel in channels.Values) channel.Writer.TryComplete();
				}
			}

			/// <summary>No more connections from the supervisor: the receiver ends (its question not answered: the connection
			/// it would have got stays the supervisor's), the listeners give Kestrel the ones taken already, then none.</summary>
			internal void Stop()
			{
				lock (gate)
				{
					stopping = true;
					pipe?.Dispose();
					foreach (var channel in channels.Values) channel.Writer.TryComplete();
				}
			}

			internal static void StopAll() => current?.Stop();

			sealed class Listener(WindowsListenerFactory factory, IPEndPoint endpoint, Channel<Socket> channel) : IConnectionListener
			{
				public EndPoint EndPoint => endpoint;

				public async ValueTask<ConnectionContext> AcceptAsync(CancellationToken cancellationToken = default)
				{
					try
					{
						while (await channel.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
						{
							if (channel.Reader.TryRead(out var socket))
							{
								// As Kestrel's socket transport takes a connection (NoDelay, its default).
								socket.NoDelay = true;
								return factory.contexts.Create(socket);
							}
						}
					}
					catch (OperationCanceledException) { }
					return null; // unbound
				}

				public ValueTask UnbindAsync(CancellationToken cancellationToken = default)
				{
					// The host is stopping: no more connections for this worker.
					factory.Stop();
					return default;
				}

				public ValueTask DisposeAsync() => default;
			}
		}

		[DllImport("ws2_32.dll", SetLastError = true)]
		static extern int WSADuplicateSocketW(SafeSocketHandle socket, uint processId, [Out] byte[] protocolInfo);
	}
}

#endif
