#if NETCOREAPP

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
	/// <summary>
	/// IIS's error pages (system.webServer/httpErrors, IisConfiguration's sections for the URL; HttpErrors:Mode Iis, the
	/// default), as IIS 10 answers (its documentation; checked on IIS, samples/ErrorsProbe). An answer of 400 or more:
	/// <list type="bullet">
	/// <item>errorMode: Custom, the pages below; Detailed, IIS's detailed error page (here a simple one: the status, the
	/// URL, the file); DetailedLocalOnly (the default), detailed for a request from this machine, custom otherwise.</item>
	/// <item>existingResponse: Auto (the default) replaces the application's answer unless it asked IIS not to
	/// (Response.TrySkipIisCustomErrors: ASP.NET's own error pages ask it), and a detailed page replaces an answer without a
	/// body only; Replace replaces them all; PassThrough none.</item>
	/// <item>error (statusCode, subStatusCode, path, responseMode, inherited with clear and remove): File, the
	/// application's file with the status (IIS's own files, prefixLanguageFilePath's custerr: the host's simple pages,
	/// IIS's text); ExecuteURL, the application's page run in the request's place (200, its query string
	/// "404;http://host/url"); Redirect, a 302 to it. Without an entry: the status's short text. Without the original
	/// server's settings, IIS's: its pages for 401, 403, 404, 405, 406, 412, 431, 500, 501, 502.</item>
	/// </list>
	/// The application's headers are kept (Cache-Control, X-AspNet-Version), its body and type replaced. An answer whose
	/// headers went out before it could be looked at (a large one flushed) is left as it is, as IIS can only leave it.
	/// </summary>
	internal sealed class IisHttpErrors
	{
		enum ErrorMode { DetailedLocalOnly, Custom, Detailed }
		enum Existing { Auto, Replace, PassThrough }
		enum ResponseMode { File, ExecuteURL, Redirect }

		sealed class Entry
		{
			public int Status;
			public int SubStatus = -1;
			public string Path;
			public ResponseMode Mode;
			public bool Builtin;   // IIS's own file (prefixLanguageFilePath): the host's simple page
		}

		sealed class Settings
		{
			public ErrorMode ErrorMode = ErrorMode.DetailedLocalOnly;
			public Existing Existing = Existing.Auto;
			public ResponseMode DefaultMode = ResponseMode.File;
			public string DefaultPath;
			public List<Entry> Entries = new List<Entry>();
			public string Base = "";   // the folder whose file a relative File path is in
			public string FileFolder;
		}

		// IIS's own entries (applicationHost.config's httpErrors): its custerr pages.
		static readonly int[] IisEntries = { 401, 403, 404, 405, 406, 412, 431, 500, 501, 502 };

		readonly string contentRoot;
		readonly bool defaults;
		readonly ConcurrentDictionary<string, Settings> byFolder = new ConcurrentDictionary<string, Settings>(StringComparer.OrdinalIgnoreCase);
		// The clients taken as this machine (DetailedLocalOnly's local): HttpErrors:LocalAddresses ("*", addresses, networks:
		// a test's client in front of a container, as the golden data's recording was from IIS's machine). None by default.
		readonly bool everyClientLocal;
		readonly List<IPNetwork> localNetworks = new List<IPNetwork>();

		internal IisHttpErrors(string contentRoot, string localAddresses = null)
		{
			this.contentRoot = contentRoot;
			defaults = IisProfile.Of(contentRoot) == null;
			foreach (var item in (localAddresses ?? "").Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (item == "*") { everyClientLocal = true; continue; }
				IPNetwork network;
				IPAddress address;
				if (IPNetwork.TryParse(item, out network)) localNetworks.Add(network);
				else if (IPAddress.TryParse(item, out address)) localNetworks.Add(new IPNetwork(address, address.AddressFamily == Net.Sockets.AddressFamily.InterNetwork ? 32 : 128));
			}
		}

		Settings For(string path) => byFolder.GetOrAdd(IisConfiguration.Deepest(contentRoot, path), _ => Make(path));

		Settings Make(string path)
		{
			var settings = new Settings();
			if (defaults)
				foreach (var status in IisEntries) settings.Entries.Add(new Entry { Status = status, Builtin = true });
			foreach (var section in IisConfiguration.For(contentRoot, path))
			{
				var errors = IisConfiguration.Child(section.WebServer, "httpErrors");
				if (errors == null) continue;
				ErrorMode errorMode;
				if (Enum.TryParse((string)errors.Attribute("errorMode"), true, out errorMode)) settings.ErrorMode = errorMode;
				Existing existing;
				if (Enum.TryParse((string)errors.Attribute("existingResponse"), true, out existing)) settings.Existing = existing;
				ResponseMode mode;
				if (Enum.TryParse((string)errors.Attribute("defaultResponseMode"), true, out mode)) settings.DefaultMode = mode;
				if (errors.Attribute("defaultPath") != null) settings.DefaultPath = (string)errors.Attribute("defaultPath");
				foreach (var item in errors.Elements())
				{
					int status, sub;
					int.TryParse((string)item.Attribute("statusCode"), out status);
					if (!int.TryParse((string)item.Attribute("subStatusCode"), out sub)) sub = -1;
					switch (item.Name.LocalName)
					{
						case "clear": settings.Entries.Clear(); break;
						case "remove": settings.Entries.RemoveAll(e => e.Status == status && e.SubStatus == sub); break;
						case "error":
							settings.Entries.RemoveAll(e => e.Status == status && e.SubStatus == sub);
							if (!Enum.TryParse((string)item.Attribute("responseMode"), true, out mode)) mode = ResponseMode.File;
							settings.Entries.Add(new Entry
							{
								Status = status,
								SubStatus = sub,
								Path = (string)item.Attribute("path"),
								Mode = mode,
								Builtin = mode == ResponseMode.File && (item.Attribute("prefixLanguageFilePath") != null || section.Server),
							});
							settings.Base = section.Base;
							settings.FileFolder = section.FileFolder;
							break;
					}
				}
			}
			return settings;
		}

		/// <summary>Whether IIS puts its page in the answer's place (its status of 400 or more), from what the answer has so far.</summary>
		bool Replaces(Core.HttpContext context, bool hasBody)
		{
			var response = context.Response;
			if (response.StatusCode < 400) return false;
			var settings = For(Path(context));
			switch (settings.Existing)
			{
				case Existing.PassThrough: return false;
				case Existing.Replace: return true;
			}
			var flags = context.Features.Get<IisErrorFlags>();
			if (flags?.TrySkipIisCustomErrors == true) return false;
			// What answered other than System.Web (CoreWCF's services, ASP.NET Core's endpoints) with a body of its own: left, as
			// IIS's WCF asked IIS (TrySkipIisCustomErrors); the end of the pipeline's empty 404 is IIS's.
			if (hasBody && flags?.SystemWeb != true) return false;
			// A detailed page replaces an answer that has nothing of its own only.
			return !Detailed(context, settings) || !hasBody;
		}

		static string Path(Core.HttpContext context)
		{
			var path = (context.Request.PathBase.Value ?? "") + (context.Request.Path.Value ?? "");
			return path.Length == 0 ? "/" : path;
		}

		bool Detailed(Core.HttpContext context, Settings settings)
		{
			if (settings.ErrorMode == ErrorMode.Detailed) return true;
			if (settings.ErrorMode == ErrorMode.Custom) return false;
			// IIS's local request: from this machine (the loopback, or the server's own address).
			var connection = context.Connection;
			var remote = connection.RemoteIpAddress;
			if (remote == null) return false;
			if (remote.IsIPv4MappedToIPv6) remote = remote.MapToIPv4();
			return IPAddress.IsLoopback(remote) || remote.Equals(connection.LocalIpAddress) || everyClientLocal || localNetworks.Any(n => n.Contains(remote));
		}

		/// <summary>The answer's place taken by IIS's page (the response not started).</summary>
		async Task WriteErrorAsync(Core.HttpContext context, Core.RequestDelegate next)
		{
			var response = context.Response;
			var request = context.Request;
			var status = response.StatusCode;
			var subStatus = context.Features.Get<IisLogStatus>()?.SubStatus ?? 0;
			var settings = For(Path(context));
			response.Headers.ContentLength = null;
			response.Headers.Remove("Content-Encoding");
			if (Detailed(context, settings))
			{
				response.ContentType = "text/html; charset=utf-8";
				response.Headers.CacheControl = "private";
				await WriteAsync(response, DetailedPage(context, status, subStatus));
				return;
			}
			var entry = settings.Entries.FirstOrDefault(e => e.Status == status && e.SubStatus == subStatus) ??
				settings.Entries.FirstOrDefault(e => e.Status == status && e.SubStatus == -1);
			if (entry == null && !string.IsNullOrEmpty(settings.DefaultPath))
				entry = new Entry { Status = status, Path = settings.DefaultPath, Mode = settings.DefaultMode };
			response.ContentType = "text/html";
			if (entry == null || entry.Builtin || string.IsNullOrEmpty(entry.Path))
			{
				await WriteAsync(response, IisStaticFile.ErrorPage(status, Japanese(context)) ?? ShortText(context, status));
				return;
			}
			switch (entry.Mode)
			{
				case ResponseMode.Redirect:
				{
					// IIS logs the error's status, not the 302 it sends.
					var logStatus = context.Features.Get<IisLogStatus>();
					if (logStatus == null) context.Features.Set(logStatus = new IisLogStatus { SubStatus = subStatus });
					logStatus.LoggedStatus = status;
					var body = IisUrlRewrite.Redirect(context, 302, entry.Path);
					if (!Core.HttpMethods.IsHead(request.Method)) await response.Body.WriteAsync(body, 0, body.Length);
					return;
				}
				case ResponseMode.ExecuteURL:
				{
					// IIS runs the page in the request's place: 200 unless it says, its query string the status and the URL
					// that failed ("404;http://host/folder/page.aspx?k=v"), Request.RawUrl the one asked for.
					var failed = request.Scheme + "://" + request.Host.Value + AspNetCoreWorkerRequest.RawTarget(context);
					var url = entry.Path.StartsWith("/", StringComparison.Ordinal) ? entry.Path : settings.Base + "/" + entry.Path;
					var mark = url.IndexOf('?');
					request.PathBase = default;
					request.Path = new Core.PathString(mark < 0 ? url : url.Substring(0, mark));
					request.QueryString = new Core.QueryString("?" + status.ToString(global::System.Globalization.CultureInfo.InvariantCulture) + ";" + failed);
					response.StatusCode = 200;
					response.Headers.Remove("Content-Type");
					context.Features.Set(new IisErrorFlags { Executing = true });
					await next(context);
					return;
				}
				default:
				{
					var file = ErrorFile(entry.Path, settings);
					byte[] content = null;
					try { if (file != null && File.Exists(file)) content = await File.ReadAllBytesAsync(file); }
					catch (Exception e) when (e is IOException || e is UnauthorizedAccessException) { }
					if (content == null)
					{
						await WriteAsync(response, IisStaticFile.ErrorPage(status, Japanese(context)) ?? ShortText(context, status));
						return;
					}
					response.ContentLength = content.Length;
					if (!Core.HttpMethods.IsHead(request.Method)) await response.Body.WriteAsync(content, 0, content.Length);
					return;
				}
			}
		}

		// A File entry's file: relative to the site's root (IIS's allowAbsolutePathsWhenDelegated false); an absolute one
		// only in the application's folder.
		string ErrorFile(string path, Settings settings)
		{
			path = Environment.ExpandEnvironmentVariables(path).Replace('\\', System.IO.Path.DirectorySeparatorChar).Replace('/', System.IO.Path.DirectorySeparatorChar);
			var full = System.IO.Path.IsPathRooted(path) ? path : System.IO.Path.Combine(contentRoot, path.TrimStart(System.IO.Path.DirectorySeparatorChar));
			try { full = System.IO.Path.GetFullPath(full); }
			catch (Exception e) when (e is ArgumentException || e is NotSupportedException) { return null; }
			return full.StartsWith(contentRoot, StringComparison.OrdinalIgnoreCase) ? full : null;
		}

		static Task WriteAsync(Core.HttpResponse response, string page)
		{
			var bytes = Encoding.UTF8.GetBytes(page);
			response.ContentLength = bytes.Length;
			return Core.HttpMethods.IsHead(response.HttpContext.Request.Method) ? Task.CompletedTask : response.Body.WriteAsync(bytes, 0, bytes.Length);
		}

		// The language IIS writes its own texts in (observed on IIS 10 with en-US and ja-JP installed, a Japanese Windows): the
		// request's Accept-Language by its quality (q=0 left out; ties in the header's order), the first tag with a region
		// whose language IIS has (en-US, en-GB, en-AU: English; ja-JP: Japanese where the server's language is); a tag
		// without one ("en", "ja") matches nothing. None: the server's language (the system's). The same for its detailed page
		// and its custerr pages (prefixLanguageFilePath's en-US, ja-JP: observed with errorMode Custom; IIS's documentation
		// does not say how the language is chosen). FrameworkOnCore has English and Japanese.
		static bool Japanese(Core.HttpContext context)
		{
			var server = global::System.Globalization.CultureInfo.InstalledUICulture.TwoLetterISOLanguageName == "ja";
			var header = context?.Request.Headers.AcceptLanguage.ToString();
			if (string.IsNullOrEmpty(header)) return server;
			var tags = new List<(string Tag, double Quality, int Index)>();
			var parts = header.Split(',');
			for (int i = 0; i < parts.Length; i++)
			{
				var fields = parts[i].Split(';');
				var tag = fields[0].Trim();
				double quality = 1;
				foreach (var field in fields.Skip(1))
				{
					var f = field.Trim();
					if (f.StartsWith("q=", StringComparison.OrdinalIgnoreCase) &&
						!double.TryParse(f.Substring(2), global::System.Globalization.NumberStyles.Float, global::System.Globalization.CultureInfo.InvariantCulture, out quality))
						quality = 0;
				}
				if (quality > 0 && tag.Length > 0) tags.Add((tag, quality, i));
			}
			foreach (var (tag, _, _) in tags.OrderByDescending(t => t.Quality).ThenBy(t => t.Index))
			{
				var dash = tag.IndexOf('-');
				if (dash <= 0) continue;
				var language = tag.Substring(0, dash);
				if (language.Equals("en", StringComparison.OrdinalIgnoreCase)) return false;
				if (language.Equals("ja", StringComparison.OrdinalIgnoreCase) && server) return true;
			}
			return server;
		}

		// IIS's answer for a status it has no page for: its short description (in the server's language where it is known).
		static string ShortText(Core.HttpContext context, int status)
		{
			if (Japanese(context))
				switch (status)
				{
					case 400: return "要求が正しくありません";
					case 410: return "要求されたページは削除されました。";
					case 503: return "サービスを利用できません。";
				}
			switch (status)
			{
				case 410: return "The requested page has been removed.";
				case 413: return "Request Entity Too Large";
			}
			var reason = Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(status);
			return string.IsNullOrEmpty(reason) ? "Error" : reason;
		}

		// IIS's detailed error page, simply: its title, the status, the URL and the file.
		static string DetailedPage(Core.HttpContext context, int status, int subStatus)
		{
			var request = context.Request;
			// The reason the answer has (URL Rewrite's statusReason), else the status's.
			var reason = context.Features.Get<Core.Features.IHttpResponseFeature>()?.ReasonPhrase;
			if (string.IsNullOrEmpty(reason)) reason = Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(status);
			var code = status.ToString(global::System.Globalization.CultureInfo.InvariantCulture) + "." + subStatus.ToString(global::System.Globalization.CultureInfo.InvariantCulture);
			var url = request.Scheme + "://" + request.Host.Value + AspNetCoreWorkerRequest.RawTarget(context);
			string physical = null;
			try { physical = HostingEnvironment.MapPath(request.PathBase.Value + request.Path.Value); }
			catch (Exception e) when (e is HttpException || e is ArgumentException) { }
			return "<!DOCTYPE html><html><head><title>IIS 10.0 " + (Japanese(context) ? "エラーの詳細" : "Detailed Error") + " - " + code + " - " + reason + "</title></head><body>" +
				"<h3>HTTP " + (Japanese(context) ? "エラー " : "Error ") + code + " - " + reason + "</h3>" +
				"<table><tr><th>Requested URL</th><td>" + HttpUtility.HtmlEncode(url) + "</td></tr>" +
				(physical == null ? "" : "<tr><th>Physical Path</th><td>" + HttpUtility.HtmlEncode(physical) + "</td></tr>") +
				"</table></body></html>";
		}

		/// <summary>
		/// The response's body held until what it is is known: an answer of 400 or more IIS replaces is not sent (its body
		/// dropped, its headers kept for the page); any other goes on as it is written.
		/// </summary>
		sealed class Body : Stream, Core.Features.IHttpResponseBodyFeature
		{
			readonly IisHttpErrors errors;
			readonly Core.HttpContext context;
			internal readonly Core.Features.IHttpResponseBodyFeature Inner;
			PipeWriter writer;
			internal bool? Replace;
			internal bool CompletePending;

			public Body(IisHttpErrors errors, Core.HttpContext context, Core.Features.IHttpResponseBodyFeature inner)
			{
				this.errors = errors;
				this.context = context;
				Inner = inner;
			}

			bool Decide(bool hasBody) => Replace ??= errors.Replaces(context, hasBody);

			internal Task FlushWriterAsync() => writer == null ? Task.CompletedTask : writer.FlushAsync().AsTask();

			public Stream Stream => this;
			public PipeWriter Writer => writer ??= PipeWriter.Create(this, new StreamPipeWriterOptions(leaveOpen: true));
			public void DisableBuffering() => Inner.DisableBuffering();
			public Task StartAsync(CancellationToken cancellationToken = default)
			{
				// Started without a body yet: what it is is not known (an empty error page is IIS's to fill).
				if (Replace == null && context.Response.StatusCode >= 400) return Task.CompletedTask;
				return Decide(true) ? Task.CompletedTask : Inner.StartAsync(cancellationToken);
			}
			public Task SendFileAsync(string path, long offset, long? count, CancellationToken cancellationToken = default) =>
				Decide(true) ? Task.CompletedTask : Inner.SendFileAsync(path, offset, count, cancellationToken);
			public Task CompleteAsync()
			{
				if (Replace == false) return Inner.CompleteAsync();
				CompletePending = true;   // the middleware ends it, with IIS's page or as it was
				return Task.CompletedTask;
			}

			public override bool CanRead => false;
			public override bool CanSeek => false;
			public override bool CanWrite => true;
			public override long Length => throw new NotSupportedException();
			public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
			public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
			public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
			public override void SetLength(long value) => throw new NotSupportedException();
			public override void Flush()
			{
				if (Replace == false) Inner.Stream.Flush();
			}
			public override Task FlushAsync(CancellationToken cancellationToken) =>
				Replace == false ? Inner.Stream.FlushAsync(cancellationToken) : Task.CompletedTask;
			public override void Write(byte[] buffer, int offset, int count)
			{
				if (count == 0 || Decide(true)) return;
				Inner.Stream.Write(buffer, offset, count);
			}
			public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				if (count == 0 || Decide(true)) return Task.CompletedTask;
				return Inner.Stream.WriteAsync(buffer, offset, count, cancellationToken);
			}
			public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
			{
				if (buffer.Length == 0 || Decide(true)) return default;
				return Inner.Stream.WriteAsync(buffer, cancellationToken);
			}
		}

		internal sealed class Middleware
		{
			readonly Core.RequestDelegate next;
			readonly IisHttpErrors errors;

			public Middleware(Core.RequestDelegate next, IisHttpErrors errors)
			{
				this.next = next;
				this.errors = errors;
			}

			public async Task Invoke(Core.HttpContext context)
			{
				var inner = context.Features.Get<Core.Features.IHttpResponseBodyFeature>();
				var body = new Body(errors, context, inner);
				context.Features.Set<Core.Features.IHttpResponseBodyFeature>(body);
				try
				{
					await next(context);
					await body.FlushWriterAsync();
				}
				finally
				{
					context.Features.Set(inner);
				}
				if (context.Response.HasStarted) return;
				var replace = body.Replace ?? errors.Replaces(context, false);
				if (replace && context.Features.Get<IisErrorFlags>()?.Executing != true)
					await errors.WriteErrorAsync(context, next);
				if (body.CompletePending) await inner.CompleteAsync();
			}
		}
	}

	/// <summary>What the application told IIS of its answer (Response.TrySkipIisCustomErrors); an error page being run.</summary>
	internal sealed class IisErrorFlags
	{
		public bool TrySkipIisCustomErrors;
		public bool Executing;
		public bool SystemWeb;   // System.Web answered it
	}
}

#endif
