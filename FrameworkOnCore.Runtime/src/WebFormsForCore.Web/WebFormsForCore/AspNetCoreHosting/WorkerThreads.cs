#if NETCOREAPP

using System.Collections.Concurrent;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace System.Web.Hosting
{
	/// <summary>
	/// The threads System.Web runs its requests on (AspNetCoreWorkerRequest.Process), as IIS's worker process has its own:
	/// threads of their own, not the thread pool's (a request blocks its thread: on Kestrel's I/O too, which the thread pool
	/// completes; the pool's threads all blocked, nothing would complete it), kept and used again. A request was a thread
	/// started for it (Task.Factory.StartNew, LongRunning): starting and ending a thread, its thread statics made anew, its
	/// handles finalized, were a third of the CPU a small page took. A request never waits for another: an idle thread takes
	/// it, or a new one does; a thread idle for a minute ends, the processors' count of them stay. The idle threads wait on
	/// one semaphore (it spins a little before it sleeps): a thread of its own woken by an event of its own each, tried,
	/// answered fewer requests (its wake-ups slower).
	/// </summary>
	internal static class WorkerThreads
	{
		static readonly ConcurrentQueue<WorkItem> queue = new ConcurrentQueue<WorkItem>();
		static readonly SemaphoreSlim signal = new SemaphoreSlim(0);
		static readonly TimeSpan idleTimeout = TimeSpan.FromMinutes(1);
		static readonly int kept = Environment.ProcessorCount;
		static int idle;      // threads waiting for work, less the ones a request has taken (TryTakeIdle) and not yet woken
		static int threads;

		sealed class WorkItem
		{
			public ExecutionContext Context;
			public Action Work;
			public TaskCompletionSource<bool> Done;
		}

		/// <summary>Runs the work on one of the threads, in the caller's execution context (its AsyncLocal values).</summary>
		public static Task Run(Action work)
		{
			var item = new WorkItem
			{
				Context = ExecutionContext.Capture(),
				Work = work,
				Done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously),
			};
			if (TryTakeIdle())
			{
				queue.Enqueue(item);
				signal.Release();
			}
			else
			{
				Interlocked.Increment(ref threads);
				new Thread(() => Loop(item)) { IsBackground = true, Name = "ASP.NET Worker" }.Start();
			}
			return item.Done.Task;
		}

		static bool TryTakeIdle()
		{
			while (true)
			{
				var count = Volatile.Read(ref idle);
				if (count <= 0) return false;
				if (Interlocked.CompareExchange(ref idle, count - 1, count) == count) return true;
			}
		}

		static void Loop(WorkItem first)
		{
			Execute(first);
			while (true)
			{
				Interlocked.Increment(ref idle);
				if (!signal.Wait(idleTimeout))
				{
					// Idle for a while: this thread ends, unless only the kept ones are left; it is taken back from the idle
					// count, unless a request has just counted on it (its signal is coming: waited for).
					if (Volatile.Read(ref threads) > kept && TryTakeIdle())
					{
						Interlocked.Decrement(ref threads);
						return;
					}
					if (TryTakeIdle()) continue;
					signal.Wait();
				}
				WorkItem item;
				while (!queue.TryDequeue(out item)) Thread.Yield();
				Execute(item);
			}
		}

		static void Execute(WorkItem item)
		{
			try
			{
				if (item.Context != null) ExecutionContext.Run(item.Context, state => ((WorkItem)state).Work(), item);
				else item.Work();
				item.Done.SetResult(true);
			}
			catch (Exception e)
			{
				item.Done.SetException(e);
			}
			finally
			{
				// What a request left on the thread: its illogical call context (CallContext.SetData), as a thread of the
				// thread pool's starts each work item without it.
				CallContext.ClearIllogical();
			}
		}
	}
}

#endif
