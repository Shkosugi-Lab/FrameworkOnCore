#if NETCOREAPP

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Text;
using System.Diagnostics;
using System.Web;
using System.Web.Hosting;
using Microsoft.Win32.SafeHandles;
using Microsoft.Extensions.Primitives;
using System.Security.Principal;
using Microsoft.AspNetCore;
using Core = Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace System.Web.Hosting
{
	public class AspNetCoreWorkerRequest : SimpleWorkerRequest, IAspNetCoreWorkerRequest
	{
		private const int MaxChunkLength = 64 * 1024;

		private const int MaxHeaderBytes = 32 * 1024;

		private static readonly char[] BadPathChars = new[] { '%', '>', '<', ':', '\\' };


		private TaskCompletionSource<bool> Completed = new TaskCompletionSource<bool>();


		private static readonly char[] IntToHex = new[]
			{
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
			};

		// IIS's hidden segments (request filtering: applicationHost.config's, web.config's hiddenSegments over them,
		// IntegratedModeConfiguration.RequestFiltering): a URL with one of them as any of its segments is answered 404 (404.8).
		// They were written without their underscores ("/appdata"), so only bin was refused: App_Data was served
		// (BlogEngine's users.xml, its users and their password hashes); web.config was not one (/web.config/x 404.0), and an
		// application's own (web.config's <add segment>) were not refused.

		public AspNetCoreHost Host { get; private set; }

		private string allRawHeaders;

		private byte[] body;

		private int bodyLength;

		private int contentLength;

		// security permission to Assert remoting calls to connection
		private int endHeadersOffset;

		private string filePath;

		private byte[] headerBytes;

		private List<ByteString> headerByteStrings;

		private bool headersSent;

		// parsed request data

		private bool isClientScriptPath;

		private string[] knownRequestHeaders;

		private string path;

		private string pathInfo;

		private string pathTranslated;


		private string queryString;
		private byte[] queryStringBytes;

		private List<byte[]> responseBodyBytes;

		private StringBuilder responseHeadersBuilder;

		private int responseStatus;

		private int startHeadersOffset;

		private string[][] unknownRequestHeaders;

		private string url;

		private string verb;

		public Core.HttpContext Context { get; private set; }
		public AspNetCoreWorkerRequest(AspNetCoreHost host, Core.HttpContext context)
			: base(string.Empty, string.Empty, null)
		{
			this.Host = host;
			Context = context;
		}

		public override void CloseConnection()
		{
			Context.Connection.RequestClose();
		}

		// HttpRuntime's end of send notification (its EndOfSendCallback: Request.Dispose, Response.Dispose), as IIS's worker
		// request calls it once the response is sent: the response's buffers back to their pool (HttpResponseBufferElement:
		// ArrayPool's; never returned, each request's buffers were new 32 KB arrays), the response's cache dependencies
		// disposed, the request's posted files' temporary file. The response is written to Kestrel's pipe as it is sent
		// (SendResponseFromMemory: copied), so the buffers are free at the end of the request.
		private EndOfSendNotification endOfSend;
		private object endOfSendData;

		// The response captured to be kept as IIS's kernel cache keeps a static file's (StaticResponseCache): made once
		// IisStaticFile answers the whole file (the response is sent after the handler: buffered), not for the other
		// requests (a page's response copied for nothing was a tenth of a small page's rate on Linux).
		internal StaticResponseCache.Capture ResponseCapture;
		// The application's files' state when the request came (FileCache.Generation): a response made from files changed
		// since is not kept.
		internal long FilesGeneration;
		// System.Web's word that the response is not for the kernel cache (HttpResponse.DisableKernelCache).
		internal bool KernelCacheDisabled;

		internal override void DisableKernelCache()
		{
			KernelCacheDisabled = true;
			ResponseCapture?.Abandon();
		}

		// Response.TrySkipIisCustomErrors (ASP.NET's own error pages set it): IIS's httpErrors leaves the answer then (IisHttpErrors).
		internal override bool TrySkipIisCustomErrors
		{
			get => Context.Features.Get<IisErrorFlags>()?.TrySkipIisCustomErrors == true;
			set
			{
				var flags = Context.Features.Get<IisErrorFlags>();
				if (flags == null) Context.Features.Set(flags = new IisErrorFlags());
				flags.TrySkipIisCustomErrors = value;
			}
		}

		public override void SetEndOfSendNotification(EndOfSendNotification callback, object extraData)
		{
			endOfSendData = extraData;
			endOfSend = callback;
		}

		// What is disposed once the response is sent (EndOfRequest): a static file's open file its response is sent from
		// (IisStaticFile, HttpResponse.WriteFileStream).
		private List<IDisposable> disposeAtEnd;

		internal void DisposeAtEndOfRequest(IDisposable disposable)
		{
			lock (this) (disposeAtEnd ??= new List<IDisposable>()).Add(disposable);
		}

		private void DisposeAtEnd()
		{
			List<IDisposable> list;
			lock (this)
			{
				list = disposeAtEnd;
				disposeAtEnd = null;
			}
			if (list == null) return;
			foreach (var disposable in list)
			{
				try { disposable.Dispose(); }
				catch (Exception) { }
			}
		}

		private void NotifyEndOfSend()
		{
			var callback = Interlocked.Exchange(ref endOfSend, null);
			if (callback == null) return;
			// The request's user as System.Web authenticated it (forms authentication...), for what logs the request after
			// it (the W3C log's cs-username: IIS logs the user the integrated pipeline gave it).
			try
			{
				// The substatus the application set (Response.SubStatusCode), as IIS logs it.
				var subStatus = (endOfSendData as HttpContext)?.Response?.SubStatusCodeInternal ?? 0;
				if (subStatus != 0) IisLogStatus.Set(Context, subStatus, Context.Features.Get<IisLogStatus>()?.Win32Status ?? 0);
				var user = (endOfSendData as HttpContext)?.User?.Identity;
				if (user != null && user.IsAuthenticated && !string.IsNullOrEmpty(user.Name))
					Context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Principal.GenericIdentity(user.Name, user.AuthenticationType ?? ""));
			}
			catch (Exception)
			{
			}
			try
			{
				callback(this, endOfSendData);
			}
			catch (Exception e)
			{
				System.Web.Management.WebBaseEvent.RaiseRuntimeError(e, this);
			}
		}

		public override void EndOfRequest()
		{
#if DebugWF4C
			var path = Context.Request.Path;
			Debug.WriteLine($"EndOfRequestStart {path}");
#endif
			NotifyEndOfSend();
			DisposeAtEnd();
			try
			{
				if (Context != null && Context.Response != null)
				{
					var task = Context.Response.CompleteAsync();
					if (task != null)
					{
						task.ContinueWith(t =>
							{
#if DebugWF4C
								Debug.WriteLine($"EndOfRequest {path}");
#endif
								if (t.Exception != null) Completed.SetException(t.Exception);
								else Completed.SetResult(true);
							},
							CancellationToken.None,
							TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
							TaskScheduler.Default);
					}
					else
					{
#if DebugWF4C
						Debug.WriteLine($"EndOfRequest Fail three {path}");
#endif
						Completed.SetResult(true);
					}
				} else
				{
#if DebugWF4C
					Debug.WriteLine($"EndOfRequest Fail {path}");
#endif
					Completed.SetResult(true);
				}
			}
			catch (Exception ex)
			{
#if DebugWF4C
				Debug.WriteLine($"EndOfRequest Fail two {path}");
#endif
				//Completed.SetException(ex);
				Completed.SetResult(true);
			}
		}

		public override void FlushResponse(bool finalFlush)
		{
			using (var noSyncContext = new SafeAsync())
			{
				// The final one is the end of the request's (ASP.NET Core sends what is left): not a flush the application
				// asked for, after which an answer goes out as it comes. IIS's features in front (IisCompression) see the
				// whole answer then: compressed with its length, as IIS's.
				if (!finalFlush) Context.Response.Body.Flush();
				else Context.Response.Body.Close();
			}
		}

		public override string GetAppPath() => Host.VirtualPath;
		public override string GetAppPathTranslated() => Host.PhysicalPath;
		public override string GetFilePath()
		{
			ParseRequest();
			return filePath;
		}
		public override string GetFilePathTranslated()
		{
			ParseRequest();
			return pathTranslated;
		}
		public override string GetHttpVerbName() => Context.Request.Method;
		public override string GetHttpVersion() => Context.Request.Protocol;
		public override string GetKnownRequestHeader(int index) => knownRequestHeaders[index];
		public override string GetLocalAddress() => Address(Context.Connection.LocalIpAddress);
		// As SERVER_NAME (GetServerName), the port the client asked for: Request.Url and absolute
		// redirects are built from them. The socket's port is not it behind port mapping (a container's
		// 8080 published as another) or a reverse proxy: the Host header's port, or the scheme's.
		// Without a Host header (HTTP/1.0), the port the request came to, as IIS's SERVER_PORT (it was the scheme's).
		public override int GetLocalPort() => Context.Request.Host.Port ??
			(!Context.Request.Host.HasValue && Context.Connection.LocalPort > 0 ? Context.Connection.LocalPort : Context.Request.IsHttps ? 443 : 80);
		// HTTPS as the client used it (a proxy that ends TLS says so in X-Forwarded-Proto, which ASP.NET
		// Core applies with ASPNETCORE_FORWARDEDHEADERS_ENABLED=true). It was always http.
		public override bool IsSecure() => Context.Request.IsHttps;
		public override string GetPathInfo() => pathInfo;
		public override byte[] GetPreloadedEntityBody() => null;
		public override string GetQueryString() {
			var str = Context.Request.QueryString.ToString();
			if (str.StartsWith("?")) return str.Substring(1);
			else return str;
		}
		public override byte[] GetQueryStringRawBytes() => Encoding.ASCII.GetBytes(GetQueryString());
		public override string GetRawUrl()
		{
			// The path as http.sys gives it to IIS (HttpSysUrl: "/sub/../a.aspx" is /a.aspx, "/a%2fb" /a/b).
			// As IIS's: the URL the client asked for, unaffected by what served it (the default document "/" is served by:
			// UseDefaultFiles or the host set Request.Path to /Default.aspx), its path decoded and its query string as it was
			// sent ("/Urls.aspx/a b?q=x%20y"). FriendlyUrls redirects a RawUrl ending in .aspx to the URL without it:
			// WingtipToys' "/" was a 301 to /Default where IIS answers 200.
			var target = RawTarget(Context);
			var mark = target.IndexOf('?');
			var rawPath = mark < 0 ? target : target[..mark];
			var path = HttpSysUrl.Canonical(rawPath, out var canonical) == 0 && canonical != null ? canonical : DecodeRawPath(rawPath);
			return mark < 0 ? path : path + target[mark..];
		}

		/// <summary>The URL as the request line has it (escaped as sent; IIS's UNENCODED_URL): its path and query string.</summary>
		internal static string RawTarget(Core.HttpContext context)
		{
			var target = context.Features.Get<Core.Features.IHttpRequestFeature>()?.RawTarget;
			if (!string.IsNullOrEmpty(target) && target[0] != '/' && Uri.TryCreate(target, UriKind.Absolute, out var absolute))
				target = absolute.PathAndQuery;  // the absolute form (a proxy's request)
			if (string.IsNullOrEmpty(target) || target[0] != '/')
			{
				var request = context.Request;
				var path = (request.PathBase + request.Path).ToUriComponent();
				return (path.Length == 0 ? "/" : path) + request.QueryString.Value;
			}
			return target;
		}

		/// <summary>
		/// The server variables IIS has that ASP.NET's list for other hosts (HttpRequest.FillInServerVariablesCollection) does
		/// not: UNENCODED_URL, the ones URL Rewrite's rules set (IisUrlRewrite: serverVariables, IIS_WasUrlRewritten).
		/// </summary>
		internal IEnumerable<KeyValuePair<string, string>> ExtraServerVariables()
		{
			yield return new KeyValuePair<string, string>("UNENCODED_URL", RawTarget(Context));
			var set = Context.Features.Get<IisServerVariables>();
			if (set != null)
				foreach (var variable in set.Values) yield return variable;
		}

		// A path as sent, decoded as IIS decodes it (every escape, '/' too).
		static string DecodeRawPath(string raw)
		{
			return raw.IndexOf('%') < 0 ? raw : Uri.UnescapeDataString(raw);
		}
		public override string GetRemoteAddress() => Address(Context.Connection.RemoteIpAddress);

		// An IPv4 client as IIS writes it (REMOTE_ADDR, UserHostAddress: "172.19.0.3"), not as Kestrel's dual-mode socket has it
		// ("::ffff:172.19.0.3"): what applications compare addresses with or log.
		static string Address(System.Net.IPAddress address) =>
			address == null ? null : (address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address).ToString();
		public override int GetRemotePort() => Context.Connection.RemotePort;
		public override string GetServerName()
		{
			// As IIS's SERVER_NAME: the host the client asked for (Request.Url, absolute redirects).
			// The local address is what a client behind port mapping or a proxy cannot reach.
			var host = Context.Request.Host;
			if (host.HasValue && !string.IsNullOrEmpty(host.Host)) return host.Host;

			// Without one (HTTP/1.0), the address the request came to, as IIS gives it ("127.0.0.1", observed on IIS 10: it was
			// "localhost"); an IPv6 loopback "localhost" (a name a URL can have).
			string localAddress = GetLocalAddress();
			if (localAddress == null || localAddress.Equals("::1"))
			{
				return "localhost";
			}
			return localAddress;
		}

		// The server variables HttpRequest.ServerVariables asks the worker request for (the others it makes itself), as IIS
		// answers them. The names were without their underscores ("ALLRAW", "SERVERPROTOCOL", "LOGONUSER", "AUTHTYPE"), so
		// every one was "": HTTPS was "" where IIS says "off" (an application testing HTTPS != "off" took every request as
		// secure), SERVER_PROTOCOL and REMOTE_PORT were "".
		public override string GetServerVariable(string name)
		{
			switch (name)
			{
				case null:
					return string.Empty;
				case "ALL_RAW":
					return string.Concat(Context.Request.Headers.Select(header => $"{header.Key}: {header.Value}\r\n"));
				case "SERVER_PROTOCOL":
					return GetHttpVersion();
				// Anonymous, as IIS's anonymous authentication (Kestrel has no Windows authentication): empty. With the
				// process's user here, the Windows authentication module (authentication mode Windows, the default) made
				// every request authenticated as it (User.Identity.Name, Request.IsAuthenticated).
				case "LOGON_USER":
				case "AUTH_TYPE":
					return string.Empty;
				case "HTTPS":
					return IsSecure() ? "on" : "off";
				case "REMOTE_PORT":
					return GetRemotePort().ToString(CultureInfo.InvariantCulture);
				case "GATEWAY_INTERFACE":
					return "CGI/1.1";
				case "SERVER_SOFTWARE":
					return "Kestrel";
				// IIS's site and application, as IIS names its first site's (no metabase here).
				case "INSTANCE_ID":
					return "1";
				case "INSTANCE_META_PATH":
					return "/LM/W3SVC/1";
				case "APPL_MD_PATH":
					return "/LM/W3SVC/1/ROOT" + (Host.VirtualPath == "/" ? "" : Host.VirtualPath.TrimEnd('/'));
				// Without TLS (or its client certificate) IIS gives these empty.
				case "AUTH_PASSWORD":
				case "CERT_COOKIE":
				case "CERT_FLAGS":
				case "CERT_ISSUER":
				case "CERT_KEYSIZE":
				case "CERT_SECRETKEYSIZE":
				case "CERT_SERIALNUMBER":
				case "CERT_SERVER_ISSUER":
				case "CERT_SERVER_SUBJECT":
				case "CERT_SUBJECT":
				case "HTTPS_KEYSIZE":
				case "HTTPS_SECRETKEYSIZE":
				case "HTTPS_SERVER_ISSUER":
				case "HTTPS_SERVER_SUBJECT":
					return string.Empty;
				case "UNENCODED_URL":
					return RawTarget(Context);
				default:
					string value = null;
					Context.Features.Get<IisServerVariables>()?.Values.TryGetValue(name, out value);
					return value;
			}
		}

		public override string GetUnknownRequestHeader(string name)
		{
			int n = unknownRequestHeaders.Length;

			for (int i = 0; i < n; i++)
			{
				if (string.Compare(name, unknownRequestHeaders[i][0], StringComparison.OrdinalIgnoreCase) == 0)
				{
					return unknownRequestHeaders[i][1];
				}
			}

			return null;
		}

		public override string[][] GetUnknownRequestHeaders()
		{
			return unknownRequestHeaders;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		// Implementation of HttpWorkerRequest

		// Decoded, as IIS's (Request.Path).
		public override string GetUriPath() => DecodedPath();

		// The request's path decoded: the application's base and the path in it ("/" when both are empty). As IIS decodes
		// it: an escaped '/' too (Kestrel leaves "%2F" in Request.Path, which was a bad path, 400, where IIS answers
		// /Urls.aspx/a%2Fb with the path info "/a/b").
		string DecodedPath()
		{
			var request = Context.Request;
			var decoded = (request.PathBase.Value ?? "") + (request.Path.Value ?? "");
			if (decoded.IndexOf('%') >= 0) decoded = decoded.Replace("%2F", "/").Replace("%2f", "/");
			return decoded.Length == 0 ? "/" : decoded;
		}

		// IIS's request filtering refuses (404.11) a URL whose path is escaped twice (allowDoubleEscaping false): one that
		// decoded once more is not the same ("%2520", or a '+', a space then). /Urls.aspx/a+b was served. web.config's
		// allowDoubleEscaping="true" lets them through (it was refused all the same).
		internal static bool IsDoubleEscaped(string path)
		{
			try { return Uri.UnescapeDataString(path.Replace('+', ' ')) != path; }
			catch (UriFormatException) { return true; }
		}

		public override IntPtr GetUserToken()
		{
			return Host.GetProcessToken();
		}

		public string GetProcessUser()
		{
			return Host.GetProcessUser();
		}

		public override string GetAppPoolID()
		{
			return Host.PhysicalPath.GetHashCode().ToString("X");
		}

		// As IIS's: true once the response has started (its headers gone to the client).
		public override bool HeadersSent()
		{
			return headersSent || Context.Response.HasStarted;
		}

		// As IIS's: false once the client has gone (the request aborted).
		public override bool IsClientConnected() => !Context.RequestAborted.IsCancellationRequested;
		public override bool IsEntireEntityBodyIsPreloaded() => false;

		// A virtual path that is a file of the application.
		bool IsFile(string virtualPath)
		{
			try { return File.Exists(MapPath(virtualPath)); }
			catch (Exception e) when (e is ArgumentException or IOException or NotSupportedException or HttpException) { return false; }
		}

		public override string MapPath(string path)
		{
			string mappedPath;
			bool isClientScriptPath;

			if (string.IsNullOrEmpty(path) || path.Equals("/"))
			{
				// asking for the site root
				mappedPath = Host.VirtualPath == "/" ? Host.PhysicalPath : Environment.SystemDirectory;
			}
			else if (Host.IsVirtualPathAppPath(path))
			{
				// application path
				mappedPath = Host.PhysicalPath;
			}
			else if (Host.IsVirtualPathInApp(path, out isClientScriptPath))
			{
				if (isClientScriptPath)
				{
					mappedPath = Path.Combine(Host.PhysicalClientScriptPath,
								 path.Substring(Host.NormalizedClientScriptPath.Length));
				}
				else
				{
					// inside app but not the app path itself
					mappedPath = Path.Combine(Host.PhysicalPath, path.Substring(Host.NormalizedVirtualPath.Length));
				}
			}
			else
			{
				// outside of app -- make relative to app path
				if (path.StartsWith("/", StringComparison.Ordinal))
				{
					mappedPath = Path.Combine(Host.PhysicalPath, path.Substring(1));
				}
				else
				{
					mappedPath = Path.Combine(Host.PhysicalPath, path);
				}
			}

			mappedPath = mappedPath.Replace('/', Path.DirectorySeparatorChar);
			// URLs are case-insensitive on IIS (/Default.aspx for default.aspx).
			mappedPath = WebFormsForCore.PhysicalPathCasing.Resolve(mappedPath);

			// Without a trailing separator but a root's ("C:\", Linux's "/"), on Linux as on Windows.
			if (mappedPath.Length > 1 && mappedPath[mappedPath.Length - 1] == Path.DirectorySeparatorChar &&
				!mappedPath.EndsWith(":\\", StringComparison.Ordinal))
			{
				mappedPath = mappedPath.Substring(0, mappedPath.Length - 1);
			}

			return mappedPath;
		}

		[AspNetHostingPermission(SecurityAction.Assert, Level = AspNetHostingPermissionLevel.Medium)]
		public async Task Process()
		{
			try
			{
				await ProcessCore();
			}
			finally
			{
				// (a request that failed before its end, EndOfRequest: what it holds open)
				DisposeAtEnd();
			}
		}

		async Task ProcessCore()
		{
			// read the request
			if (!TryParseRequest())
			{
				if (refusal != 0) await RefuseAsync(refusal);
				return;
			}

			if (!Host.RequireAuthentication || true)
			{
				// deny access to code, bin, etc.
				if (IsRequestForRestrictedDirectory())
				{
					IisLogStatus.Set(Context, 8);
					await RefuseAsync(404);  // as IIS (404.8)
					return;
				}

				PrepareResponse();
				FilesGeneration = FileCache.Generation;

				// Hand the processing over to HttpRuntime, on one of System.Web's worker threads (a thread of its own for the
				// request, used again: WorkerThreads).
				await WorkerThreads.Run(() =>
					{
#if DebugWF4C
						Debug.WriteLine($"Start ProcessRequest {Context.Request.Path}");
#endif
						HttpRuntime.ProcessRequest(this);
#if DebugWF4C
						Debug.WriteLine($"End ProcessRequest {Context.Request.Path}");
#endif
					});

#if DebugWF4C
				Debug.WriteLine($"Waiting {Context.Request.Path} to finish");
#endif
				await Completed.Task;
#if DebugWF4C
				Debug.WriteLine($"Request {Context.Request.Path} finished");
#endif
				StaticResponseCache.Store(Context, ResponseCapture);
			}
		}


		public override int ReadEntityBody(byte[] buffer, int size) => ReadEntityBody(buffer, 0, size);
		public override int ReadEntityBody(byte[] buffer, int offset, int size)
		{
			if (offset < 0) throw new ArgumentOutOfRangeException("offset must be grater than zero.");

			int bytesRead = 0;

			using (var noSyncContext = new SafeAsync())
			{
				var reader = Context.Request.Body;

				// At the offset (HttpBufferlessInputStream.Read passes its own). A body Kestrel refuses while it is read (a chunk
				// that is not one, beyond the content length IIS allows: http.sys's 400, IIS's 413) is an HttpException of its
				// status (it was the application's error, 500, with Kestrel's exception).
				try { bytesRead = reader.Read(buffer, offset, size); }
				catch (Microsoft.AspNetCore.Http.BadHttpRequestException e) { throw new HttpException(e.StatusCode, e.Message, e); }
			}
			return bytesRead;
		}

		public override void SendCalculatedContentLength(int contentLength)
		{
			if (!headersSent)
			{
				Context.Response.ContentLength = contentLength;
			}
		}

		public override void SendKnownResponseHeader(int index, string value)
		{
			if (headersSent)
			{
				return;
			}

			switch (index)
			{
				case HeaderServer:
				case HeaderDate:
				case HeaderConnection:
					// ignore these
					return;
				// Accept-Ranges, Last-Modified and Expires of a static file: sent, as IIS's static file module sends them
				// (IisStaticFile; StaticFileHandler's own Expires a day ahead is not written any more).


				// FIX: #12506
				case HeaderContentType:

					string contentType = null;

					if (value == "application/octet-stream")
					{
						// application/octet-stream is default for unknown so lets
						// take a shot at determining the type.
						// don't do this for other content-types as you are going to
						// end up sending text/plain for endpoints that are handled by
						// asp.net such as .aspx, .asmx, .axd, etc etc
						//contentType = CommonExtensions.GetContentType(pathTranslated);
					}
					value = contentType ?? value;
					Context.Response.ContentType = value;
					return;
			}

			SendUnknownResponseHeader(GetKnownResponseHeaderName(index), value);
		}

		public override void SendResponseFromFile(string filename, long offset, long length)
		{
			if (length == 0)
			{
				return;
			}

			FileStream f = null;
			try
			{
				f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
				SendResponseFromFileStream(f, offset, length);
			}
			finally
			{
				if (f != null)
				{
					f.Close();
				}
			}
		}

		public override void SendResponseFromFile(IntPtr handle, long offset, long length)
		{
			if (length == 0)
			{
				return;
			}

			using (var sfh = new SafeFileHandle(handle, false))
			{
				using (var f = new FileStream(sfh, FileAccess.Read))
				{
					SendResponseFromFileStream(f, offset, length);
				}
			}
		}

		public override void SendResponseFromMemory(byte[] data, int length)
		{
			if (length > 0)
			{
				ResponseCapture?.Append(data, 0, length);
				using (var noSyncContext = new SafeAsync())
				{
					Context.Response.Body.Write(data, 0, length);
				}
			}
		}

		public override void SendStatus(int statusCode, string statusDescription)
		{
			// A description with a line's end or another control character (Response.StatusDescription from the request: a header
			// injected into the response, which Kestrel wrote as it was given) is not sent: IIS closes the connection.
			if (statusDescription != null && statusDescription.Any(c => c < 0x20 && c != '\t' || c == 0x7F)) statusDescription = null;
			Context.Response.StatusCode = statusCode;
			// The description in the status line, as IIS sends it (HttpResponse.StatusDescription); only one that is not
			// the status's usual text (Kestrel's): a later change of the status (IIS's error pages) is not left with it.
			var feature = Context.Features.Get<Core.Features.IHttpResponseFeature>();
			if (feature != null && !Context.Response.HasStarted)
				feature.ReasonPhrase = string.IsNullOrEmpty(statusDescription) ||
					string.Equals(statusDescription, Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(statusCode), StringComparison.Ordinal)
					? null : statusDescription;
		}

		public override void SendUnknownResponseHeader(string name, string value)
		{
			if (headersSent)
				return;

			if (Context.Response.Headers.ContainsKey(name)) {
				var header = Context.Response.Headers[name];
				Context.Response.Headers[name] = StringValues.Concat(header, value);
			} else
			{
				Context.Response.Headers[name] = value;
			}
		}

		private bool IsBadPath()
		{
			ParseRequest();

			if (path.IndexOfAny(BadPathChars) >= 0)
			{
				return true;
			}

			if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(path, "..", CompareOptions.Ordinal) >= 0)
			{
				return true;
			}

			if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(path, "//", CompareOptions.Ordinal) >= 0)
			{
				return true;
			}

			return false;
		}

		private bool IsRequestForRestrictedDirectory() => IsHidden(path);

		// Any segment of the URL, as IIS matches them (/App_Data/x, /a/bin/x), without regard to case; the hidden segments of
		// the URL's folder (configurationPath: the path with the application's base).
		internal static bool IsHidden(string path, string configurationPath = null)
		{
			var hidden = IntegratedModeConfiguration.RequestFiltering(configurationPath ?? path).HiddenSegments;
			return path.Split('/', StringSplitOptions.RemoveEmptyEntries).Any(hidden.Contains);
		}

		// IIS7WorkerRequest.SetRequestHeader's: a request's header set, appended to (a header sent more than once) or removed (null)
		// on the ASP.NET Core request, and read again as the worker request's headers.
		internal void SetRequestHeader(string name, string value, bool replace)
		{
			var headers = Context.Request.Headers;
			if (value == null)
				headers.Remove(name);
			else if (replace)
				headers[name] = value;
			else
				headers[name] = StringValues.Concat(headers[name], value);
			allRawHeaders = null;
			ParseHeaders();
		}

		private void ParseHeaders()
		{
			knownRequestHeaders = new string[RequestHeaderMaximum];

			// construct unknown headers as array list of name1,value1,...
			var headers = new List<string>();


			foreach (var header in Context.Request.Headers)
			{
				string name = header.Key;
				var strValues = header.Value;
				string str;
				switch (strValues.Count)
				{
					case 0:
						str = (string)null;
						break;
					case 1:
						str = strValues[0];
						break;
					default:
						// A header sent more than once, as http.sys gives it (RFC 9110 5.3: ", "; observed on IIS 10); the
						// cookies with "; " (RFC 9113 8.2.3: HTTP/2's cookie crumbs; System.Web splits cookies at ';' only).
						str = string.Join(name.Equals("Cookie", StringComparison.OrdinalIgnoreCase) ? "; " : ", ", (IEnumerable<string?>)strValues);
						break;
				}

				// remember
				int knownIndex = GetKnownRequestHeaderIndex(name);
				if (knownIndex >= 0)
				{
					knownRequestHeaders[knownIndex] = str;
				}
				else
				{
					headers.Add(name);
					headers.Add(str);
				}
			} 
            
            // aspnet_filter's AspFilterSessionId: the URL's cookieless values (FilterCookielessValues)
            FilterCookielessValues();
            if (cookielessValues != null)
            {
                headers.Add(System.Web.Security.CookielessHelperClass.COOKIELESS_SESSION_FILTER_HEADER);
                headers.Add(cookielessValues);
            }

            // copy to array unknown headers
			int n = headers.Count / 2;
			unknownRequestHeaders = new string[n][];
			int j = 0;

			for (int i = 0; i < n; i++)
			{
				unknownRequestHeaders[i] = new string[2];
				unknownRequestHeaders[i][0] = headers[j++];
				unknownRequestHeaders[i][1] = headers[j++];
			}
		}

		private void ParsePostedContent()
		{
			contentLength = 0;
			bodyLength = 0;

			string contentLengthValue = knownRequestHeaders[HeaderContentLength];
			if (contentLengthValue != null)
			{
				try
				{
					contentLength = Int32.Parse(contentLengthValue, CultureInfo.InvariantCulture);
				}
				// ReSharper disable EmptyGeneralCatchClause
				catch
				// ReSharper restore EmptyGeneralCatchClause
				{
				}
			}

			/*
			if (headerBytes.Length > endHeadersOffset)
			{
				bodyLength = headerBytes.Length - endHeadersOffset;

				if (bodyLength > contentLength)
				{
					bodyLength = contentLength; // don't read more than the content-length
				}

				if (bodyLength > 0)
				{
					body = new byte[bodyLength];
					Buffer.BlockCopy(headerBytes, endHeadersOffset, body, 0, bodyLength);
					//connection.LogRequestBody(body);
				}
			} */
		}

		bool requestParsed = false;
		private void ParseRequest()
		{
			if (!requestParsed)
			{
				// Decoded, as IIS gives it (its cooked URL): a PathString in a string is its escaped form (ToUriComponent),
				// and an escaped "%20" was a bad path (400) where IIS serves "/Product/Fast Car".
				path = DecodedPath();

				// The file is the first segment IIS maps to a handler by its extension (*.aspx ...) or that is a file; the
				// rest is the path info: /Page.aspx/x/y is /Page.aspx and /x/y. It was split at the last '/' after the last
				// '.', /Page.aspx/x and /y, a 404.
				filePath = path;
				pathInfo = String.Empty;
				for (var slash = path.IndexOf('/', 1); slash > 0; slash = path.IndexOf('/', slash + 1))
				{
					var candidate = path[..slash];
					var segment = candidate[(candidate.LastIndexOf('/') + 1)..];
					if (segment.IndexOf('.') < 0) continue;
					if (Host.HandleExtensions.Any(extension => segment.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) || IsFile(candidate))
					{
						filePath = candidate;
						pathInfo = path[slash..];
						break;
					}
				}

				pathTranslated = MapPath(filePath);

				requestParsed = true;
			}
		}

		private void PrepareResponse()
		{
			Context.Response.Headers.Clear();
			//Context.Response.Cookies.Clear();
		}

		private void Reset()
		{
			headerBytes = null;
			startHeadersOffset = 0;
			endHeadersOffset = 0;
			headerByteStrings = null;

			isClientScriptPath = false;

			verb = null;
			url = null;

			path = null;
			filePath = null;
			pathInfo = null;
			pathTranslated = null;
			queryString = null;
			queryStringBytes = null;

			contentLength = 0;
			bodyLength = 0;
			body = null;

			allRawHeaders = null;
			unknownRequestHeaders = null;
			knownRequestHeaders = null;
		}

		private void SendResponseFromFileStream(Stream f, long offset, long length)
		{
			ResponseCapture?.Abandon();     // a file sent from the file (a large one): not kept
			long fileSize = f.Length;

			if (length == -1)
			{
				length = fileSize - offset;
			}

			if (length == 0 || offset < 0 || length > fileSize - offset)
			{
				return;
			}

			if (offset > 0)
			{
				f.Seek(offset, SeekOrigin.Begin);
			}

			using (var noSyncContext = new SafeAsync())
			{
				if (length <= MaxChunkLength)
				{
					var fileBytes = ArrayPool<byte>.Shared.Rent((int)length);
					try
					{
						int bytesRead = f.Read(fileBytes, 0, (int)length);
						//SendResponseFromMemory(fileBytes, bytesRead);
						Context.Response.Body.Write(fileBytes, 0, bytesRead);
					}
					finally
					{
						ArrayPool<byte>.Shared.Return(fileBytes);
					}
				}
				else
				{
					var chunk = ArrayPool<byte>.Shared.Rent(MaxChunkLength);
					try
					{
						var bytesRemaining = (int)length;

						while (bytesRemaining > 0)
						{
							int bytesToRead = (bytesRemaining < MaxChunkLength) ? bytesRemaining : MaxChunkLength;
							int bytesRead = f.Read(chunk, 0, bytesToRead);

							//SendResponseFromMemory(chunk, bytesRead);
							Context.Response.Body.Write(chunk, 0, bytesRead);
							bytesRemaining -= bytesRead;

							// flush to release keep memory
							if ((bytesRemaining > 0) && (bytesRead > 0))
							{
								//FlushResponse(false);
								Context.Response.Body.Flush();
							}
						}
					}
					finally
					{
						ArrayPool<byte>.Shared.Return(chunk);
					}
				}
			}
		}

		private void SkipAllPostedContent()
		{
			if ((contentLength > 0) && (bodyLength < contentLength))
			{
				byte[] buffer = new byte[1024];
				for (int i = contentLength - bodyLength; i > 0; i -= buffer.Length)
				{
					var nread = Context.Request.Body.Read(buffer, 0, Math.Min(i, buffer.Length));
					if ((buffer == null) || (nread == 0))
					{
						return;
					}
				}
			}
		}

		/// <summary>
		/// TODO: defer response until request is written
		/// </summary>
		/// <returns></returns>
		// A request refused before it reaches the application (its status: RefuseAsync answers it).
		private int refusal;

		// The answer of IIS (its request filtering: 404.8, 404.11, 404.14, 404.15, 413.1) or of http.sys (400) to a request it
		// refuses before ASP.NET sees it: an HTML page, not the application's error.
		private async Task RefuseAsync(int status)
		{
			Context.Response.StatusCode = status;
			// http.sys's 400 its own page; IIS's request filtering's (404.x, 413.1) the status only: their page is httpErrors'
			// (IisHttpErrors).
			string page = null;
			switch (status)
			{
				case 400:
					Context.Response.ContentType = "text/html; charset=us-ascii";
					page = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\"\"http://www.w3.org/TR/html4/strict.dtd\">\r\n<HTML><HEAD><TITLE>Bad Request</TITLE>\r\n" +
						"<META HTTP-EQUIV=\"Content-Type\" Content=\"text/html; charset=us-ascii\"></HEAD>\r\n<BODY><h2>Bad Request - Invalid URL</h2>\r\n" +
						"<hr><p>HTTP Error 400. The request URL is invalid.</p>\r\n</BODY></HTML>\r\n";
					break;
				case 413:
					// The status line as IIS writes it (Kestrel's is RFC 9110's "Payload Too Large").
					var feature = Context.Features.Get<Core.Features.IHttpResponseFeature>();
					if (feature != null) feature.ReasonPhrase = "Request Entity Too Large";
					break;
			}
			if (page != null) await Core.HttpResponseWritingExtensions.WriteAsync(Context.Response, page);
			await Context.Response.CompleteAsync();
			// A body too large: read to its end (beyond Kestrel's limit), as IIS takes it, so that the client, still sending,
			// gets the answer and not a connection closed under it. Not a body the client waits to be asked for (Expect:
			// 100-continue): IIS answers 413 without asking for it (reading it made Kestrel send 100 Continue first).
			if (status == 413 && !string.Equals(Context.Request.Headers.Expect.ToString(), "100-continue", StringComparison.OrdinalIgnoreCase))
			{
				var bodySize = Context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpMaxRequestBodySizeFeature>();
				if (bodySize != null && !bodySize.IsReadOnly) bodySize.MaxRequestBodySize = null;
				try
				{
					await Context.Request.Body.CopyToAsync(Stream.Null);
				}
				catch (Exception e) when (e is IOException || e is Microsoft.AspNetCore.Http.BadHttpRequestException || e is OperationCanceledException)
				{
				}
			}
		}

		// IIS's aspnet_filter (integrated mode too; observed on IIS 10: /(S(abc))/echo.aspx): a URL's cookieless values, the
		// "/(...)/" segment CookielessHelperClass takes (FindCookielessValues: .NET Framework 4.8's servicing updates' first one),
		// leave the URL before anything sees it (Path, FilePath, RawUrl, UNENCODED_URL, HTTP_URL are /echo.aspx) and are given
		// as the AspFilterSessionId header, which System.Web reads them from. The segment was left in the path: with the
		// servicing updates System.Web no longer takes it out of FilePath when aspnet_filter gave the header.
		private string cookielessValues;
		private bool cookielessFiltered;

		private void FilterCookielessValues()
		{
			if (cookielessFiltered) return;
			cookielessFiltered = true;
			var requestPath = Context.Request.Path.Value ?? string.Empty;
			// the common case: no cookieless values
			if (requestPath.IndexOf('(') < 0) return;
			if (!System.Web.Security.CookielessHelperClass.FindCookielessValues(requestPath, out var start, out var end)
				|| !System.Web.Security.CookielessHelperClass.IsValidCookielessValues(requestPath, start + 2, end))
				return;
			cookielessValues = requestPath.Substring(start + 2, end - start - 2);
			var segment = requestPath.Substring(start, end + 1 - start);
			Context.Request.Path = new Core.PathString(requestPath.Remove(start, segment.Length));
			// The URL as sent too (its path's; the values are letters, digits, '-' and '_': not escaped).
			var feature = Context.Features.Get<Core.Features.IHttpRequestFeature>();
			var target = feature?.RawTarget;
			if (!string.IsNullOrEmpty(target))
			{
				int query = target.IndexOf('?');
				int at = target.IndexOf(segment, StringComparison.Ordinal);
				if (at >= 0 && (query < 0 || at < query)) feature.RawTarget = target.Remove(at, segment.Length);
			}
		}

		private bool TryParseRequest()
		{
			refusal = 0;
			Reset();

			FilterCookielessValues();

			ParseRequest();

			if (!IntegratedModeConfiguration.RequestFiltering(path).AllowDoubleEscaping && IsDoubleEscaped(path))
			{
				IisLogStatus.Set(Context, 11);
				refusal = 404;  // as IIS (404.11)
				return false;
			}

			// http.sys refuses a path segment longer than 260 characters (UrlSegmentMaxLength) before IIS sees the request.
			var requestPath = Context.Request.Path.Value ?? "";
			if (requestPath.Split('/').Any(segment => segment.Length > 260))
			{
				// http.sys's: in its HTTPERR log, not in the site's.
				IisLogStatus.SetNotLogged(Context);
				refusal = 400;
				return false;
			}

			// IIS's request filtering's limits (system.webServer/security/requestFiltering/requestLimits): the content's
			// length (413.1), the URL's (404.14), the query string's (404.15). Kestrel's own limit on the body is IIS's: a
			// larger one in web.config is taken (an application that takes larger uploads raises it there).
			var limits = IntegratedModeConfiguration.RequestLimits((Context.Request.PathBase.Value ?? "") + requestPath);
			if (Context.Request.ContentLength > limits.MaxAllowedContentLength)
			{
				IisLogStatus.Set(Context, 1);
				refusal = 413;
				return false;
			}
			var queryLength = Math.Max(0, (Context.Request.QueryString.Value ?? "").Length - 1);
			if (requestPath.Length > limits.MaxUrl || queryLength > limits.MaxQueryString)
			{
				IisLogStatus.Set(Context, requestPath.Length > limits.MaxUrl ? 14 : 15);
				refusal = 404;
				return false;
			}
			var bodySize = Context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpMaxRequestBodySizeFeature>();
			if (bodySize != null && !bodySize.IsReadOnly) bodySize.MaxRequestBodySize = limits.MaxAllowedContentLength;

			// Check for bad path
			if (IsBadPath())
			{
				Context.Response.StatusCode = 400;
				Context.Response.CompleteAsync();
				return false;
			}

			// Check if the path is not well formed or is not for the current app
			if (!Host.IsVirtualPathInApp(path, out isClientScriptPath))
			{
				refusal = 404;
				return false;
			}

			ParseHeaders();

			ParsePostedContent();

			return true;
		}

		/*private static string UrlEncodeRedirect(string path)
		{
			// this method mimics the logic in HttpResponse.Redirect (which relies on internal methods)

			// count non-ascii characters
			byte[] bytes = Encoding.UTF8.GetBytes(path);
			int count = bytes.Length;
			int countNonAscii = 0;
			for (int i = 0; i < count; i++)
			{
				if ((bytes[i] & 0x80) != 0)
				{
					countNonAscii++;
				}
			}

			// encode all non-ascii characters using UTF-8 %XX
			if (countNonAscii > 0)
			{
				// expand not 'safe' characters into %XX, spaces to +s
				var expandedBytes = new byte[count + countNonAscii * 2];
				int pos = 0;
				for (int i = 0; i < count; i++)
				{
					byte b = bytes[i];

					if ((b & 0x80) == 0)
					{
						expandedBytes[pos++] = b;
					}
					else
					{
						expandedBytes[pos++] = (byte)'%';
						expandedBytes[pos++] = (byte)IntToHex[(b >> 4) & 0xf];
						expandedBytes[pos++] = (byte)IntToHex[b & 0xf];
					}
				}

				path = Encoding.ASCII.GetString(expandedBytes);
			}

			// encode spaces into %20
			if (path.IndexOf(' ') >= 0)
			{
				path = path.Replace(" ", "%20");
			}

			return path;
		}*/

		#region Nested type: ByteParser

		internal class ByteParser
		{
			private readonly byte[] bytes;

			private int pos;

			public ByteParser(byte[] bytes)
			{
				bytes = bytes;
				pos = 0;
			}

			public int CurrentOffset
			{
				get { return pos; }
			}

			public ByteString ReadLine()
			{
				ByteString line = null;

				for (int i = pos; i < bytes.Length; i++)
				{
					if (bytes[i] == (byte)'\n')
					{
						int len = i - pos;
						if (len > 0 && bytes[i - 1] == (byte)'\r')
						{
							len--;
						}

						line = new ByteString(bytes, pos, len);
						pos = i + 1;
						return line;
					}
				}

				if (pos < bytes.Length)
				{
					line = new ByteString(bytes, pos, bytes.Length - pos);
				}

				pos = bytes.Length;
				return line;
			}
		}

		#endregion

		#region Nested type: ByteString

		internal class ByteString
		{
			private readonly byte[] bytes;

			private readonly int length;

			private readonly int offset;

			public ByteString(byte[] bytes, int offset, int length)
			{
				bytes = bytes;
				offset = offset;
				length = length;
			}

			public byte[] Bytes
			{
				get { return bytes; }
			}

			public bool IsEmpty
			{
				get { return (bytes == null || length == 0); }
			}

			public byte this[int index]
			{
				get { return bytes[offset + index]; }
			}

			public int Length
			{
				get { return length; }
			}

			public int Offset
			{
				get { return offset; }
			}

			public byte[] GetBytes()
			{
				var bytes = new byte[length];
				if (length > 0) Buffer.BlockCopy(bytes, offset, bytes, 0, length);
				return bytes;
			}

			public string GetString(Encoding enc)
			{
				if (IsEmpty) return string.Empty;
				return enc.GetString(bytes, offset, length);
			}

			public string GetString()
			{
				return GetString(Encoding.UTF8);
			}

			public int IndexOf(char ch)
			{
				return IndexOf(ch, 0);
			}

			public int IndexOf(char ch, int offset)
			{
				for (int i = offset; i < length; i++)
				{
					if (this[i] == (byte)ch) return i;
				}
				return -1;
			}

			public ByteString[] Split(char sep)
			{
				var list = new List<ByteString>();

				int pos = 0;
				while (pos < length)
				{
					int i = IndexOf(sep, pos);
					if (i < 0)
					{
						break;
					}

					list.Add(Substring(pos, i - pos));
					pos = i + 1;

					while (this[pos] == (byte)sep && pos < length)
					{
						pos++;
					}
				}

				if (pos < length)
					list.Add(Substring(pos));

				return list.ToArray();
			}

			public ByteString Substring(int offset, int len)
			{
				return new ByteString(bytes, offset + offset, len);
			}

			public ByteString Substring(int offset)
			{
				return Substring(offset, length - offset);
			}
		}

		#endregion
	}
}
#endif