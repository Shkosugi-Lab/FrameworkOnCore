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

		private AutoResetEvent CompletedEvent = new AutoResetEvent(false);

		private static readonly char[] IntToHex = new[]
			{
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
			};

		// IIS's hidden segments (request filtering, applicationHost.config): a URL with one of them as any of its segments is
		// answered 404 (404.8). They were written without their underscores ("/appdata"), so only bin was refused: App_Data
		// was served (BlogEngine's users.xml, its users and their password hashes).
		private static readonly string[] HiddenSegments = new[]
			{
				"bin",
				"App_code",
				"App_GlobalResources",
				"App_LocalResources",
				"App_WebReferences",
				"App_Data",
				"App_Browsers"
			};

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

		private bool specialCaseStaticFileHeaders;

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

		public override void EndOfRequest()
		{
#if DebugWF4C
			var path = Context.Request.Path;
			Debug.WriteLine($"EndOfRequestStart {path}");
#endif
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
				Context.Response.Body.Flush();
				if (finalFlush) Context.Response.Body.Close();
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
		public override string GetLocalAddress() => Context.Connection.LocalIpAddress.ToString();
		// As SERVER_NAME (GetServerName), the port the client asked for: Request.Url and absolute
		// redirects are built from them. The socket's port is not it behind port mapping (a container's
		// 8080 published as another) or a reverse proxy: the Host header's port, or the scheme's.
		public override int GetLocalPort() => Context.Request.Host.Port ?? (Context.Request.IsHttps ? 443 : 80);
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
			// As IIS's: the URL the client asked for, unaffected by what served it (the default document "/" is served by:
			// UseDefaultFiles or the host set Request.Path to /Default.aspx), its path decoded and its query string as it was
			// sent ("/Urls.aspx/a b?q=x%20y"). FriendlyUrls redirects a RawUrl ending in .aspx to the URL without it:
			// WingtipToys' "/" was a 301 to /Default where IIS answers 200.
			var target = Context.Features.Get<Core.Features.IHttpRequestFeature>()?.RawTarget;
			if (!string.IsNullOrEmpty(target) && target[0] != '/' && Uri.TryCreate(target, UriKind.Absolute, out var absolute))
				target = absolute.PathAndQuery;  // the absolute form (a proxy's request)
			if (string.IsNullOrEmpty(target) || target[0] != '/')
			{
				var query = GetQueryString();
				return string.IsNullOrEmpty(query) ? DecodedPath() : $"{DecodedPath()}?{query}";
			}
			var mark = target.IndexOf('?');
			return mark < 0 ? DecodeRawPath(target) : DecodeRawPath(target[..mark]) + target[mark..];
		}

		// A path as sent, decoded as IIS decodes it (every escape, '/' too).
		static string DecodeRawPath(string raw)
		{
			return raw.IndexOf('%') < 0 ? raw : Uri.UnescapeDataString(raw);
		}
		public override string GetRemoteAddress() => Context.Connection.RemoteIpAddress.ToString();
		public override int GetRemotePort() => Context.Connection.RemotePort;
		public override string GetServerName()
		{
			// As IIS's SERVER_NAME: the host the client asked for (Request.Url, absolute redirects).
			// The local address is what a client behind port mapping or a proxy cannot reach.
			var host = Context.Request.Host;
			if (host.HasValue && !string.IsNullOrEmpty(host.Host)) return host.Host;

			string localAddress = GetLocalAddress();
			if (localAddress.Equals("127.0.0.1") || localAddress.Equals("::1"))
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
				default:
					return null;
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
		// decoded once more is not the same ("%2520", or a '+', a space then). /Urls.aspx/a+b was served.
		bool IsDoubleEscaped()
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

		public override bool HeadersSent()
		{
			return headersSent;
		}

		public override bool IsClientConnected() => true;
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

			if (mappedPath.EndsWith("\\", StringComparison.Ordinal) &&
				!mappedPath.EndsWith(":\\", StringComparison.Ordinal))
			{
				mappedPath = mappedPath.Substring(0, mappedPath.Length - 1);
			}

			return mappedPath;
		}

		[AspNetHostingPermission(SecurityAction.Assert, Level = AspNetHostingPermissionLevel.Medium)]
		public async Task Process()
		{
			// read the request
			if (!TryParseRequest())
			{
				return;
			}

			if (!Host.RequireAuthentication || true)
			{
				// deny access to code, bin, etc.
				if (IsRequestForRestrictedDirectory())
				{
					Context.Response.StatusCode = 404;  // as IIS (404.8)
					Context.Response.CompleteAsync();
					return;
				}

				PrepareResponse();

				// Hand the processing over to HttpRuntime
				// Run processing in separate ASP.NET Worker Thread
				//var completed = Completed.Task;
				await Task.Factory.StartNew(() =>
					{
						Thread.CurrentThread.Name = $"ASP.NET WorkerThread {Context.Request.Path}";
#if DebugWF4C
						Debug.WriteLine($"Start ProcessRequest {Context.Request.Path}");
#endif
						HttpRuntime.ProcessRequest(this);
#if DebugWF4C
						Debug.WriteLine($"End ProcessRequest {Context.Request.Path}");
#endif
					},
					TaskCreationOptions.LongRunning);

#if DebugWF4C
				Debug.WriteLine($"Waiting {Context.Request.Path} to finish");
#endif
				await Completed.Task;
#if DebugWF4C
				Debug.WriteLine($"Request {Context.Request.Path} finished");
#endif
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

				bytesRead = reader.Read(buffer, 0, size);
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
				case HeaderAcceptRanges:
					// FIX: #14359
					if (value != "bytes")
					{
						// use this header to detect when we're processing a static file
						break;
					}
					specialCaseStaticFileHeaders = true;
					return;

				case HeaderExpires:
				case HeaderLastModified:
					// FIX: #14359
					if (!specialCaseStaticFileHeaders)
					{
						// NOTE: Ignore these for static files. These are generated
						//       by the StaticFileHandler, but they shouldn't be.
						break;
					}
					return;


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
				using (var noSyncContext = new SafeAsync())
				{
					Context.Response.Body.Write(data, 0, length);
				}
			}
		}

		public override void SendStatus(int statusCode, string statusDescription)
		{
			// TODO description
			Context.Response.StatusCode = statusCode;
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

		// Any segment of the URL, as IIS matches them (/App_Data/x, /a/bin/x), without regard to case.
		internal static bool IsHidden(string path) =>
			path.Split('/', StringSplitOptions.RemoveEmptyEntries)
				.Any(segment => HiddenSegments.Contains(segment, StringComparer.OrdinalIgnoreCase));

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
						str = string.Join(';', (IEnumerable<string?>)strValues);
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
            
            // append AspFilterSessionId
            var path = Context.Request.Path.Value ?? string.Empty;
            // Optimize for the common case where there is no cookie
            if (path.IndexOf('(') != -1)
            {
                int endPos = path.LastIndexOf(")/", StringComparison.Ordinal);
                int startPos = (endPos > 2 ? path.LastIndexOf("/(", endPos - 1, endPos, StringComparison.Ordinal) : -1);
                if (startPos < 0) // pattern not found: common case, exit immediately
                    return;

                if (IsValidHeader(path, startPos + 2, endPos))
                {
                    var sessionHeader = path.Substring(startPos + 2, endPos - startPos - 2);
                    headers.Add("AspFilterSessionId");
                    headers.Add(sessionHeader);
                }
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

        // Make sure sub-string if of the pattern: A(XXXX)N(XXXXX)P(XXXXX) and so on.
        static private bool IsValidHeader(string path, int startPos, int endPos)
        {
            if (endPos - startPos < 3) // Minimum len is "X()"
                return false;

            while (startPos <= endPos - 3) { // Each iteration deals with one "A(XXXX)" pattern

                if (path[startPos] < 'A' || path[startPos] > 'Z') // Make sure pattern starts with a capital letter
                    return false;

                if (path[startPos + 1] != '(') // Make sure next char is '('
                    return false;

                startPos += 2;
                bool found = false;
                for (; startPos < endPos; startPos++) { // Find the ending ')'

                    if (path[startPos] == ')') { // found it!
                        startPos++; // Set position for the next pattern
                        found = true;
                        break; // Break out of this for-loop.
                    }

                    if (path[startPos] == '/') { // Can't contain path separaters
                        return false;
                    }
                }
                if (!found)  {
                    return false; // Ending ')' not found!
                }
            }

            if (startPos < endPos) // All chars consumed?
                return false;

            return true;
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
			specialCaseStaticFileHeaders = false;
		}

		private void SendResponseFromFileStream(Stream f, long offset, long length)
		{
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
		private bool TryParseRequest()
		{
			Reset();

			ParseRequest();

			if (IsDoubleEscaped())
			{
				Context.Response.StatusCode = 404;  // as IIS (404.11)
				Context.Response.CompleteAsync();
				return false;
			}

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
				Context.Response.StatusCode = 404;
				Context.Response.CompleteAsync();
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