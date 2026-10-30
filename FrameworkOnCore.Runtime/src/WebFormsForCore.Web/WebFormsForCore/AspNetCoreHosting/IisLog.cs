#if NETCOREAPP

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
	/// <summary>IIS's W3C log fields (its logExtFileFlags), in the order IIS writes them.</summary>
	[Flags]
	internal enum IisLogFields
	{
		None = 0,
		Date = 1 << 0,
		Time = 1 << 1,
		SiteName = 1 << 2,
		ComputerName = 1 << 3,
		ServerIP = 1 << 4,
		Method = 1 << 5,
		UriStem = 1 << 6,
		UriQuery = 1 << 7,
		ServerPort = 1 << 8,
		UserName = 1 << 9,
		ClientIP = 1 << 10,
		ProtocolVersion = 1 << 11,
		UserAgent = 1 << 12,
		Cookie = 1 << 13,
		Referer = 1 << 14,
		Host = 1 << 15,
		HttpStatus = 1 << 16,
		HttpSubStatus = 1 << 17,
		Win32Status = 1 << 18,
		BytesSent = 1 << 19,
		BytesRecv = 1 << 20,
		TimeTaken = 1 << 21,

		// IIS 10's default (a site's logFile without logExtFileFlags).
		Default = Date | Time | ServerIP | Method | UriStem | UriQuery | ServerPort | UserName | ClientIP | UserAgent | Referer |
			HttpStatus | HttpSubStatus | Win32Status | BytesSent | BytesRecv | TimeTaken,
	}

	/// <summary>
	/// What IIS logs of a request that ASP.NET Core does not know: the substatus (404.3, 404.7, 413.1...), the Win32 status
	/// (2: a file not found...), and whether IIS logs it at all (http.sys's own refusals go to its HTTPERR log, not to the
	/// site's). Set where the host answers as IIS would (IisStaticFile, the request filtering, System.Web's SubStatusCode).
	/// </summary>
	internal sealed class IisLogStatus
	{
		public int SubStatus;
		public int Win32Status;
		public bool NotLogged;
		// URL Rewrite's logRewrittenUrl: the rewritten URL logged (cs-uri-stem, cs-uri-query), not the one asked for.
		public string LoggedStem;
		public string LoggedQuery;
		// The status IIS logs where it sent another (httpErrors' Redirect: the error's, not the 302).
		public int LoggedStatus;

		internal static void Set(Core.HttpContext context, int subStatus, int win32Status = 0)
		{
			if (context == null) return;
			var status = context.Features.Get<IisLogStatus>();
			if (status == null) context.Features.Set(status = new IisLogStatus());
			status.SubStatus = subStatus;
			status.Win32Status = win32Status;
		}

		internal static void SetLoggedUrl(Core.HttpContext context, string stem, string query)
		{
			if (context == null) return;
			var status = context.Features.Get<IisLogStatus>();
			if (status == null) context.Features.Set(status = new IisLogStatus());
			status.LoggedStem = stem;
			status.LoggedQuery = query;
		}

		internal static void SetNotLogged(Core.HttpContext context)
		{
			if (context == null) return;
			var status = context.Features.Get<IisLogStatus>();
			if (status == null) context.Features.Set(status = new IisLogStatus());
			status.NotLogged = true;
		}
	}

	internal enum IisLogPeriod { Hourly, Daily, Monthly }

	internal sealed class IisLogOptions
	{
		public string Directory;
		public IisLogFields Fields = IisLogFields.Default;
		public IisLogPeriod Period = IisLogPeriod.Daily;
		public bool LocalTimeRollover;
		public string SiteName = "W3SVC1";
		public string Software = "Microsoft Internet Information Services 10.0";
	}

	/// <summary>
	/// IIS's W3C log files (u_exYYMMDD.log: IIS's names, header, fields and their order, its escaping), written as http.sys
	/// writes them: lines kept in memory and written together, not a write (and a flush) for each line as ASP.NET Core's
	/// W3C logging does (which kept up with about 10,000 requests a second). A line is queued when its response has been
	/// sent (time-taken: until then); when the writer falls behind (65,536 lines waiting), the requests wait for it (the
	/// connection's next request), none is dropped. The file is held open (others may read, rename or delete it), a header
	/// block written each time it is opened (IIS too: a process's first line, a new file). UTF-8 without a BOM, CRLF.
	/// </summary>
	internal sealed class IisLogWriter : IAsyncDisposable
	{
		const int Capacity = 65536;

		readonly IisLogOptions options;
		readonly Channel<(DateTime When, string Line)> channel = Channel.CreateBounded<(DateTime, string)>(
			new BoundedChannelOptions(Capacity) { FullMode = BoundedChannelFullMode.Wait, SingleReader = true });
		readonly Task loop;
		readonly string fieldsDirective;
		StreamWriter writer;
		string currentFile;

		internal IisLogOptions Options => options;

		public IisLogWriter(IisLogOptions options)
		{
			this.options = options;
			fieldsDirective = "#Fields: " + string.Join(" ", Order.Where(f => (options.Fields & f.Field) != 0).Select(f => f.Name));
			loop = Task.Run(Loop);
			// The lines still queued written when the process ends (an application restart ends the worker process).
			AppDomain.CurrentDomain.ProcessExit += (sender, e) => DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(5));
		}

		internal static readonly (IisLogFields Field, string Name)[] Order =
		{
			(IisLogFields.Date, "date"), (IisLogFields.Time, "time"), (IisLogFields.SiteName, "s-sitename"),
			(IisLogFields.ComputerName, "s-computername"), (IisLogFields.ServerIP, "s-ip"), (IisLogFields.Method, "cs-method"),
			(IisLogFields.UriStem, "cs-uri-stem"), (IisLogFields.UriQuery, "cs-uri-query"), (IisLogFields.ServerPort, "s-port"),
			(IisLogFields.UserName, "cs-username"), (IisLogFields.ClientIP, "c-ip"), (IisLogFields.ProtocolVersion, "cs-version"),
			(IisLogFields.UserAgent, "cs(User-Agent)"), (IisLogFields.Cookie, "cs(Cookie)"), (IisLogFields.Referer, "cs(Referer)"),
			(IisLogFields.Host, "cs-host"), (IisLogFields.HttpStatus, "sc-status"), (IisLogFields.HttpSubStatus, "sc-substatus"),
			(IisLogFields.Win32Status, "sc-win32-status"), (IisLogFields.BytesSent, "sc-bytes"), (IisLogFields.BytesRecv, "cs-bytes"),
			(IisLogFields.TimeTaken, "time-taken"),
		};

		/// <summary>IIS's logExtFileFlags ("Date, Time, ClientIP, ..."); null for a name it does not have.</summary>
		internal static IisLogFields? ParseFields(string flags)
		{
			var fields = IisLogFields.None;
			foreach (var name in flags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			{
				IisLogFields field;
				if (!Enum.TryParse(name, true, out field) || field == IisLogFields.Default) return null;
				fields |= field;
			}
			return fields;
		}

		/// <summary>Queues the line; waits while the writer is behind.</summary>
		internal ValueTask WriteAsync(DateTime when, string line) => channel.Writer.WriteAsync((when, line));

		async Task Loop()
		{
			var reader = channel.Reader;
			try
			{
				while (await reader.WaitToReadAsync().ConfigureAwait(false))
				{
					while (reader.TryRead(out var item))
					{
						try
						{
							Open(item.When);
							writer.Write(item.Line);
							writer.Write("\r\n");
						}
						catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
						{
							Close();
							Console.Error.WriteLine("W3C log: " + e.Message);
						}
					}
					try
					{
						writer?.Flush();
					}
					catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
					{
						Close();
						Console.Error.WriteLine("W3C log: " + e.Message);
					}
				}
			}
			finally
			{
				Close();
			}
		}

		void Open(DateTime when)
		{
			var name = FileName(when);
			if (writer != null && name == currentFile) return;
			// A line of an earlier period, come after the next period's first (its request ended before the boundary, its
			// line queued after another's that ended after it): written to the file being written, as http.sys writes its
			// lines in the order they come. The last file was opened again for it (a header block in its middle), and again
			// and again while the lines of the two periods came mixed. (The names of a period sort as their times.)
			if (writer != null && string.CompareOrdinal(name, currentFile) < 0) return;
			Close();
			System.IO.Directory.CreateDirectory(options.Directory);
			var stream = new FileStream(Path.Combine(options.Directory, name), FileMode.Append, FileAccess.Write,
				FileShare.ReadWrite | FileShare.Delete, 4096);
			writer = new StreamWriter(stream, new UTF8Encoding(false), 64 * 1024);
			currentFile = name;
			// IIS's header, each time the file is opened (its first line in this process, a new file).
			writer.Write("#Software: " + options.Software + "\r\n");
			writer.Write("#Version: 1.0\r\n");
			writer.Write("#Date: " + when.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + "\r\n");
			writer.Write(fieldsDirective + "\r\n");
		}

		void Close()
		{
			try
			{
				writer?.Dispose();
			}
			catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
			{
			}
			writer = null;
			currentFile = null;
		}

		// IIS's names: u_exYYMMDD.log a day, u_exYYMMDDHH.log an hour, u_exYYMM.log a month; the UTC date unless
		// localTimeRollover (the lines' dates and times are UTC either way).
		string FileName(DateTime utc)
		{
			var date = options.LocalTimeRollover ? utc.ToLocalTime() : utc;
			var format = options.Period switch
			{
				IisLogPeriod.Hourly => "yyMMddHH",
				IisLogPeriod.Monthly => "yyMM",
				_ => "yyMMdd",
			};
			return "u_ex" + date.ToString(format, System.Globalization.CultureInfo.InvariantCulture) + ".log";
		}

		int disposed;

		public async ValueTask DisposeAsync()
		{
			if (Interlocked.Exchange(ref disposed, 1) != 0) return;
			channel.Writer.TryComplete();
			await loop.ConfigureAwait(false);
		}

		/// <summary>
		/// A field's value as IIS writes it: '-' for none, '+' for a space or a nonprintable character (W3C Logging: "the
		/// HTTP Server API replaces it with a plus sign"; a tab observed on IIS 10). Other characters as they are (UTF-8).
		/// </summary>
		internal static string Escape(string value)
		{
			if (string.IsNullOrEmpty(value)) return "-";
			foreach (var c in value)
				if (c <= ' ' || c == '\x7f') return string.Create(value.Length, value, (span, v) =>
				{
					for (int i = 0; i < span.Length; i++) span[i] = v[i] <= ' ' || v[i] == '\x7f' ? '+' : v[i];
				});
			return value;
		}
	}

	/// <summary>
	/// The middleware of IIS's W3C log (IisLogWriter): a request's line once its response has been sent, with what IIS
	/// logs of it (IisLogStatus). sc-bytes: the response's status line, headers and body as sent (the body counted); cs-bytes:
	/// the request's line, headers and body (its Content-Length): Kestrel does not tell the bytes it read and wrote, both are
	/// counted from what the request and the response hold.
	/// </summary>
	internal sealed class IisLogMiddleware
	{
		readonly Core.RequestDelegate next;
		readonly IisLogWriter writer;
		readonly IisLogFields fields;
		readonly string computerName = Environment.MachineName;

		public IisLogMiddleware(Core.RequestDelegate next, IisLogWriter writer)
		{
			this.next = next;
			this.writer = writer;
			fields = writer.Options.Fields;
		}

		sealed class Entry
		{
			public Core.HttpContext Context;
			public long Start;
			public string Stem;
			public string Query;
			public CountingStream Body;
			public IisLogMiddleware Middleware;
		}

		public Task Invoke(Core.HttpContext context)
		{
			var entry = new Entry
			{
				Context = context,
				Start = Stopwatch.GetTimestamp(),
				// The URL as the client asked for it (IIS logs it, not a default document's or a rewrite's).
				Stem = context.Request.PathBase.Value + context.Request.Path.Value,
				Query = context.Request.QueryString.Value,
				Middleware = this,
			};
			if ((fields & IisLogFields.BytesSent) != 0)
			{
				entry.Body = new CountingStream(context.Response.Body);
				context.Response.Body = entry.Body;
			}
			context.Response.OnCompleted(static state => ((Entry)state).Middleware.LogAsync((Entry)state), entry);
			return next(context);
		}

		Task LogAsync(Entry entry)
		{
			var context = entry.Context;
			var status = context.Features.Get<IisLogStatus>();
			if (status != null && status.NotLogged) return Task.CompletedTask;
			var now = DateTime.UtcNow;
			var request = context.Request;
			var response = context.Response;
			var connection = context.Connection;
			var line = new StringBuilder(256);
			void Add(string value)
			{
				if (line.Length > 0) line.Append(' ');
				line.Append(value);
			}
			foreach (var (field, _) in IisLogWriter.Order)
			{
				if ((fields & field) == 0) continue;
				switch (field)
				{
					case IisLogFields.Date: Add(now.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)); break;
					case IisLogFields.Time: Add(now.ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)); break;
					case IisLogFields.SiteName: Add(IisLogWriter.Escape(writer.Options.SiteName)); break;
					case IisLogFields.ComputerName: Add(IisLogWriter.Escape(computerName)); break;
					case IisLogFields.ServerIP: Add(IisLogWriter.Escape(Address(connection.LocalIpAddress))); break;
					case IisLogFields.Method: Add(IisLogWriter.Escape(request.Method)); break;
					case IisLogFields.UriStem: Add(IisLogWriter.Escape(status?.LoggedStem ?? entry.Stem)); break;
					case IisLogFields.UriQuery:
						var query = status?.LoggedStem != null ? status.LoggedQuery : entry.Query;
						Add(IisLogWriter.Escape(string.IsNullOrEmpty(query) ? null : query.Substring(1)));
						break;
					case IisLogFields.ServerPort: Add(connection.LocalPort.ToString(System.Globalization.CultureInfo.InvariantCulture)); break;
					case IisLogFields.UserName: Add(IisLogWriter.Escape(context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : null)); break;
					case IisLogFields.ClientIP: Add(IisLogWriter.Escape(Address(connection.RemoteIpAddress))); break;
					case IisLogFields.ProtocolVersion: Add(IisLogWriter.Escape(request.Protocol)); break;
					case IisLogFields.UserAgent: Add(IisLogWriter.Escape(request.Headers.UserAgent.ToString())); break;
					case IisLogFields.Cookie: Add(IisLogWriter.Escape(request.Headers.Cookie.ToString())); break;
					case IisLogFields.Referer: Add(IisLogWriter.Escape(request.Headers.Referer.ToString())); break;
					case IisLogFields.Host: Add(IisLogWriter.Escape(request.Headers.Host.ToString())); break;
					case IisLogFields.HttpStatus: Add((status?.LoggedStatus > 0 ? status.LoggedStatus : response.StatusCode).ToString(System.Globalization.CultureInfo.InvariantCulture)); break;
					case IisLogFields.HttpSubStatus: Add((status?.SubStatus ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture)); break;
					case IisLogFields.Win32Status: Add((status?.Win32Status ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture)); break;
					case IisLogFields.BytesSent: Add(BytesSent(response, entry.Body).ToString(System.Globalization.CultureInfo.InvariantCulture)); break;
					case IisLogFields.BytesRecv: Add(BytesReceived(context).ToString(System.Globalization.CultureInfo.InvariantCulture)); break;
					case IisLogFields.TimeTaken: Add(((long)Stopwatch.GetElapsedTime(entry.Start).TotalMilliseconds).ToString(System.Globalization.CultureInfo.InvariantCulture)); break;
				}
			}
			return writer.WriteAsync(now, line.ToString()).AsTask();
		}

		static string Address(System.Net.IPAddress address) =>
			address == null ? null : (address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address).ToString();

		static long BytesSent(Core.HttpResponse response, CountingStream body)
		{
			long length = "HTTP/1.1 ".Length + 3 + 1 + ReasonPhrases.GetReasonPhrase(response.StatusCode).Length + 2 + 2;
			foreach (var header in response.Headers)
				foreach (var value in header.Value)
					length += header.Key.Length + 2 + (value?.Length ?? 0) + 2;
			return length + (body?.Count ?? response.ContentLength ?? 0);
		}

		static long BytesReceived(Core.HttpContext context)
		{
			var request = context.Request;
			// The request line as it came (its URL still encoded: Kestrel's raw target).
			var target = context.Features.Get<Core.Features.IHttpRequestFeature>()?.RawTarget ??
				(request.PathBase.Value + request.Path.Value + request.QueryString.Value);
			long length = request.Method.Length + 1 + target.Length + 1 + request.Protocol.Length + 2 + 2;
			foreach (var header in request.Headers)
				foreach (var value in header.Value)
					length += header.Key.Length + 2 + (value?.Length ?? 0) + 2;
			return length + (request.ContentLength ?? 0);
		}

		/// <summary>The response's body, its bytes counted (sc-bytes).</summary>
		internal sealed class CountingStream : Stream
		{
			readonly Stream inner;
			long count;

			public CountingStream(Stream inner) { this.inner = inner; }

			public long Count => Interlocked.Read(ref count);

			public override bool CanRead => false;
			public override bool CanSeek => false;
			public override bool CanWrite => true;
			public override long Length => throw new NotSupportedException();
			public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
			public override void Flush() => inner.Flush();
			public override Task FlushAsync(CancellationToken cancellationToken) => inner.FlushAsync(cancellationToken);
			public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
			public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
			public override void SetLength(long value) => throw new NotSupportedException();

			public override void Write(byte[] buffer, int offset, int count)
			{
				inner.Write(buffer, offset, count);
				Interlocked.Add(ref this.count, count);
			}

			public override void Write(ReadOnlySpan<byte> buffer)
			{
				inner.Write(buffer);
				Interlocked.Add(ref count, buffer.Length);
			}

			public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				await inner.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
				Interlocked.Add(ref this.count, count);
			}

			public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
			{
				await inner.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
				Interlocked.Add(ref count, buffer.Length);
			}

			// The response's own stream is Kestrel's: closing this one is not to close it (the response is completed by the host).
			protected override void Dispose(bool disposing) { }
		}
	}
}

#endif
