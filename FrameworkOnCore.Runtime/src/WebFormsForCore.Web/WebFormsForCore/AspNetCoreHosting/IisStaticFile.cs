#if NETCOREAPP

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Util;
using System.Xml.Linq;

namespace System.Web
{
	/// <summary>
	/// A static file as IIS's static file module serves it (the integrated pipeline, which the applications were written
	/// for), where ASP.NET's StaticFileHandler served it in the classic one: its type from IIS's MIME map with
	/// system.webServer/staticContent's mimeMap (a type that is not there: 404, as IIS's 404.3), ETag, Last-Modified and
	/// Accept-Ranges, 304 for a request that has it already (If-None-Match, If-Modified-Since), the cache headers of
	/// staticContent's clientCache, no Cache-Control and no Expires otherwise, no X-AspNet-Version. A file that is not: 404,
	/// IIS's (not an error of the application's: no Application_Error, no customErrors page).
	/// </summary>
	internal static class IisStaticFile
	{
		/// <summary>Serves the file; false for what is not a file of its (a directory: StaticFileHandler's).</summary>
		internal static bool Serve(HttpContext context, string virtualPath, string physicalPath)
		{
			var response = context.Response;
			var request = context.Request;
			response.SuppressVersionHeader = true;
			// The path's state as FileCache keeps it (IIS's file cache): no file system call for a file served before.
			var file = FileCache.Get(physicalPath);
			if (file.IsDirectory && FileUtil.DirectoryExists(physicalPath))   // (a name FileUtil does not take as a folder's: not one)
			{
				// A folder without a default document: IIS's directory listing module answers 403.14 (its page, not the
				// application's customErrors), or its listing where system.webServer/directoryBrowse enables it for the folder.
				// IIS's own answers have no Cache-Control (System.Web's default, private), as a remote client gets them (IIS's
				// detailed error pages, the local client's, have it).
				response.SuppressDefaultCacheControlHeader = true;
				var showFlags = DirectoryBrowsing(virtualPath);
				if (showFlags != null)
				{
					response.ContentType = "text/html; charset=UTF-8";
					response.Write(Listing(context, virtualPath, physicalPath, showFlags));
					return true;
				}
				// The status only: its page is IIS's httpErrors' (IisHttpErrors).
				IisLogStatus.Set((context.WorkerRequest as AspNetCoreWorkerRequest)?.Context, 14);
				response.StatusCode = 403;
				return true;
			}
			if (!file.IsFile || (file.Attributes & FileAttributes.Hidden) != 0 || physicalPath.EndsWith(".", StringComparison.Ordinal))
			{
				NotFound(context, 0, 2);   // IIS: 404.0, Win32 2 (ERROR_FILE_NOT_FOUND)
				return true;
			}

			var (types, clientCache) = StaticContent(virtualPath);
			string contentType;
			if (!types.TryGetValue(Path.GetExtension(physicalPath), out contentType) && !types.TryGetValue(".*", out contentType))
			{
				// IIS's 404.3: a type it is not told.
				NotFound(context, 3, 50);  // IIS: 404.3, Win32 50 (ERROR_NOT_SUPPORTED)
				return true;
			}

			var utc = file.LastWriteTimeUtc;
			var lastModified = new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second, DateTimeKind.Utc);
			if (lastModified > DateTime.UtcNow) lastModified = DateTime.UtcNow.AddTicks(-(DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond));
			// IIS's form: the file time in hexadecimal and the configuration's change number.
			var etag = "\"" + file.LastWriteTimeUtc.ToFileTimeUtc().ToString("x", CultureInfo.InvariantCulture) + ":0\"";

			response.ContentType = contentType;
			response.SuppressDefaultCacheControlHeader = true;

			// A range (StaticFileHandler's: it writes ETag, Last-Modified and Accept-Ranges itself); the whole file if the
			// range is not one to answer.
			var range = request.Headers["Range"];
			if (range != null && range.StartsWith("bytes", StringComparison.OrdinalIgnoreCase) &&
				StaticFileHandler.ProcessRangeRequest(context, physicalPath, file.Length, range, etag, lastModified))
			{
				clientCache.Apply(response);
				return true;
			}

			response.AppendHeader("Accept-Ranges", "bytes");
			response.AppendHeader("Last-Modified", HttpUtilityInternal.FormatHttpDateTime(lastModified));
			if (clientCache.SetEtag) response.AppendHeader("ETag", etag);
			clientCache.Apply(response);

			if (NotModified(request, etag, lastModified))
			{
				response.StatusCode = 304;
				response.SuppressContent = true;
				return true;
			}
			// The whole file: what IIS's static compression looks at (IisCompression).
			var core = (context.WorkerRequest as AspNetCoreWorkerRequest)?.Context;
			core?.Features.Set(new IisStaticResponse { PhysicalPath = physicalPath, Length = file.Length, LastWriteTicks = file.LastWriteTimeUtc.Ticks });
			if (file.Length > 0)
			{
				// A small file's bytes from memory (FileCache: read once), a large one from the file.
				var content = FileCache.Content(physicalPath, file);
				if (content != null)
				{
					response.OutputStream.Write(content, 0, content.Length);
					// The whole file to an anonymous user: a response IIS's kernel cache would keep (StaticResponseCache).
					var worker = context.WorkerRequest as AspNetCoreWorkerRequest;
					if (worker != null && !worker.KernelCacheDisabled && context.User?.Identity?.IsAuthenticated != true)
						worker.ResponseCapture = StaticResponseCache.Start(worker.Context, worker.FilesGeneration);
				}
				else response.TransmitFile(physicalPath, 0, file.Length);
			}
			return true;
		}

		static bool NotModified(HttpRequest request, string etag, DateTime lastModified)
		{
			var ifNoneMatch = request.Headers["If-None-Match"];
			if (ifNoneMatch != null)
				return ifNoneMatch.Split(',').Select(t => t.Trim()).Any(t => t == "*" || t == etag || t == "W/" + etag);
			var ifModifiedSince = request.Headers["If-Modified-Since"];
			DateTime since;
			// A date later than the server's time is not one (RFC 9110 13.1.3: ignored), as IIS ignores it: the file is sent.
			return ifModifiedSince != null &&
				DateTime.TryParse(ifModifiedSince, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out since) &&
				since <= DateTime.UtcNow && lastModified <= since;
		}

		static void NotFound(HttpContext context, int subStatus, int win32Status)
		{
			var response = context.Response;
			// What IIS logs of it (the W3C log's sc-substatus, sc-win32-status).
			// The status only: its page is IIS's httpErrors' (IisHttpErrors); no Cache-Control (System.Web's private), as IIS's.
			IisLogStatus.Set((context.WorkerRequest as AspNetCoreWorkerRequest)?.Context, subStatus, win32Status);
			response.SuppressDefaultCacheControlHeader = true;
			response.StatusCode = 404;
		}


		static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> browsing =
			new System.Collections.Concurrent.ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		// system.webServer/directoryBrowse for the folder (the original server's, web.config's, a location's, the folder's own
		// web.config: IisConfiguration; each attribute the nearest that says): its showFlags when enabled, null when not
		// (IIS's default).
		static string DirectoryBrowsing(string virtualPath)
		{
			var root = IisConfiguration.Root;
			if (root == null) return null;
			var path = VirtualPathUtility.RemoveTrailingSlash(virtualPath) ?? "/";
			var flags = browsing.GetOrAdd(IisConfiguration.Deepest(root, path), _ =>
			{
				var enabled = false;
				var showFlags = "Date, Time, Size, Extension";
				foreach (var section in IisConfiguration.For(root, path))
				{
					var browse = IisConfiguration.Child(section.WebServer, "directoryBrowse");
					bool value;
					if (bool.TryParse((string)browse?.Attribute("enabled"), out value)) enabled = value;
					if (browse?.Attribute("showFlags") != null) showFlags = (string)browse.Attribute("showFlags");
				}
				return enabled ? showFlags : "";
			});
			return flags.Length == 0 ? null : flags;
		}

		/// <summary>
		/// IIS's directory listing (its DirectoryListingModule's page, as IIS 10 writes it): the host and the folder, a link to
		/// the parent, then each entry the file system has (not the hidden ones; web.config too) on a line: its date (the
		/// server's short date, 10 wide; LongDate: its long one), time (8 wide), size (12 wide, &lt;dir&gt; for a folder) as
		/// showFlags asks, then its link (its name without its extension unless Extension). The server's language and
		/// formats: the system's (CultureInfo.InstalledUICulture), as IIS's.
		/// </summary>
		static string Listing(HttpContext context, string virtualPath, string physicalPath, string showFlags)
		{
			var flags = new HashSet<string>(showFlags.Split(',').Select(f => f.Trim()), StringComparer.OrdinalIgnoreCase);
			var folder = virtualPath.EndsWith("/", StringComparison.Ordinal) ? virtualPath : virtualPath + "/";
			var host = HttpUtility.HtmlEncode(context.Request.Url.Host);
			var page = new System.Text.StringBuilder(1024);
			page.Append("<html><head><title>").Append(host).Append(" - ").Append(HttpUtility.HtmlEncode(folder)).Append("</title></head><body><H1>")
				.Append(host).Append(" - ").Append(HttpUtility.HtmlEncode(folder)).Append("</H1><hr>\r\n\r\n<pre>");
			if (folder != "/")
			{
				var parent = folder.Substring(0, folder.LastIndexOf('/', folder.Length - 2) + 1);
				var toParent = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName == "ja" ? "[親ディレクトリへ]" : "[To Parent Directory]";
				page.Append("<A HREF=\"").Append(HttpUtility.UrlPathEncode(parent)).Append("\">").Append(toParent).Append("</A><br><br>");
			}
			IEnumerable<FileSystemInfo> entries;
			try { entries = new DirectoryInfo(physicalPath).EnumerateFileSystemInfos().ToList(); }
			catch (Exception e) when (e is IOException || e is UnauthorizedAccessException) { entries = Enumerable.Empty<FileSystemInfo>(); }
			// The server's language and formats (IIS's: the system's), not the request's (web.config's globalization).
			var culture = CultureInfo.InstalledUICulture;
			foreach (var entry in entries.Where(e => (e.Attributes & FileAttributes.Hidden) == 0).OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase))
			{
				var directory = (entry.Attributes & FileAttributes.Directory) != 0;
				var written = entry.LastWriteTime;
				if (flags.Contains("LongDate")) page.Append(written.ToString("D", culture)).Append(' ');
				else if (flags.Contains("Date")) page.Append(written.ToString("d", culture).PadLeft(10)).Append(' ');
				if (flags.Contains("Time")) page.Append(written.ToString("t", culture).PadLeft(8)).Append(' ');
				if (flags.Contains("Size")) page.Append(directory ? "       &lt;dir&gt;" : ((FileInfo)entry).Length.ToString(CultureInfo.InvariantCulture).PadLeft(12)).Append(' ');
				var name = directory || flags.Contains("Extension") ? entry.Name : Path.GetFileNameWithoutExtension(entry.Name);
				page.Append("<A HREF=\"").Append(HttpUtility.UrlPathEncode(folder + entry.Name + (directory ? "/" : ""))).Append("\">")
					.Append(HttpUtility.HtmlEncode(name)).Append("</A><br>");
			}
			page.Append("</pre><hr></body></html>");
			return page.ToString();
		}


		/// <summary>
		/// IIS's page for an error status (its httpErrors' defaults for remote clients: %SystemDrive%\inetpub\custerr's
		/// pages, English); null for a status IIS has none for (its response sent as it is).
		/// </summary>
		internal static string ErrorPage(int status)
		{
			string title, text;
			switch (status)
			{
				case 401: title = "401 - Unauthorized: Access is denied due to invalid credentials."; text = "You do not have permission to view this directory or page using the credentials that you supplied."; break;
				case 403: title = "403 - Forbidden: Access is denied."; text = "You do not have permission to view this directory or page using the credentials that you supplied."; break;
				case 404: title = "404 - File or directory not found."; text = "The resource you are looking for might have been removed, had its name changed, or is temporarily unavailable."; break;
				case 405: title = "405 - HTTP verb used to access this page is not allowed."; text = "The page you are looking for cannot be displayed because an invalid method (HTTP verb) was used to attempt access."; break;
				case 406: title = "406 - Client browser does not accept the MIME type of the requested page."; text = "The page you are looking for cannot be opened by your browser because it has a file name extension that your browser does not accept."; break;
				case 412: title = "412 - Precondition set by the client failed when evaluated on the Web server."; text = "The request was not completed due to preconditions that are set in the request."; break;
				case 500: title = "500 - Internal server error."; text = "There is a problem with the resource you are looking for, and it cannot be displayed."; break;
				case 501: title = "501 - Header values specify a method that is not implemented."; text = "The page you are looking for cannot be displayed because a header value in the request does not match certain configuration settings on the Web server."; break;
				case 502: title = "502 - Web server received an invalid response while acting as a gateway or proxy server."; text = "There is a problem with the page you are looking for, and it cannot be displayed. When the Web server (while acting as a gateway or proxy) contacted the upstream content server, it received an invalid response from the content server."; break;
				default: return null;
			}
			// IIS's page's parts, not its style: the same text.
			return "<!DOCTYPE html><html><head><title>" + title + "</title></head><body><div id=\"header\"><h1>Server Error</h1></div>" +
				"<div id=\"content\"><div class=\"content-container\"><fieldset><h2>" + title + "</h2><h3>" + text + "</h3></fieldset></div></div></body></html>";
		}

		/// <summary>staticContent's clientCache: its attributes, each the nearest one's (as IIS inherits them).</summary>
		internal sealed class ClientCache
		{
			public string Mode = "NoControl";
			public TimeSpan MaxAge = TimeSpan.FromDays(1);
			public string HttpExpires;
			public string Custom;
			public bool SetEtag = true;

			public void Read(XElement element)
			{
				Mode = (string)element.Attribute("cacheControlMode") ?? Mode;
				var maxAge = (string)element.Attribute("cacheControlMaxAge");
				TimeSpan value;
				if (maxAge != null && TimeSpan.TryParse(maxAge, CultureInfo.InvariantCulture, out value)) MaxAge = value;
				HttpExpires = (string)element.Attribute("httpExpires") ?? HttpExpires;
				Custom = (string)element.Attribute("cacheControlCustom") ?? Custom;
				var setEtag = (string)element.Attribute("setEtag");
				if (setEtag != null) SetEtag = !setEtag.Equals("false", StringComparison.OrdinalIgnoreCase);
			}

			public void Apply(HttpResponse response)
			{
				string directive = null;
				switch (Mode)
				{
					case "DisableCache": directive = "no-cache"; break;
					case "UseMaxAge": directive = "max-age=" + ((long)MaxAge.TotalSeconds).ToString(CultureInfo.InvariantCulture); break;
					case "UseExpires": if (!string.IsNullOrEmpty(HttpExpires)) response.AppendHeader("Expires", HttpExpires); break;
				}
				// IIS writes the custom value first ("public,max-age=86400").
				var cacheControl = string.Join(",", new[] { Custom, directive }.Where(v => !string.IsNullOrEmpty(v)));
				if (cacheControl.Length > 0) response.AppendHeader("Cache-Control", cacheControl);
			}
		}

		static readonly object gate = new object();
		static readonly Dictionary<string, (DateTime Written, XElement Root)> documents = new Dictionary<string, (DateTime, XElement)>(StringComparer.OrdinalIgnoreCase);

		static XElement Document(string file)
		{
			var state = FileCache.Get(file);
			if (!state.IsFile) return null;
			var written = state.LastWriteTimeUtc;
			lock (gate)
			{
				(DateTime Written, XElement Root) entry;
				if (documents.TryGetValue(file, out entry) && entry.Written == written) return entry.Root;
				XElement root = null;
				try
				{
					root = XDocument.Load(file).Root;
				}
				catch (Exception e) when (e is System.Xml.XmlException || e is IOException)
				{
				}
				documents[file] = (written, root);
				return root;
			}
		}

		/// <summary>
		/// The types and clientCache for a file: IIS's MIME map, then each staticContent from the application's web.config
		/// down to the file's folder's (the root's, its locations' for the path, the folders' web.config files').
		/// </summary>
		internal static (Dictionary<string, string> Types, ClientCache ClientCache) StaticContent(string virtualPath)
		{
			// Made once for the path while the application's files do not change (FileCache), not for each request (a copy of
			// IIS's MIME map, the web.config files read down the path).
			var kept = FileCache.Memo("staticContent:" + virtualPath, () => Tuple.Create(MakeStaticContent(virtualPath)));
			return kept.Item1;
		}

		static (Dictionary<string, string> Types, ClientCache ClientCache) MakeStaticContent(string virtualPath)
		{
			// The original server's settings first (IisProfile: its MIME map is the whole map, applicationHost.config's), then the
			// application's web.config files down the path.
			var profile = IisProfile.Current;
			var serverMap = IisProfile.WebServer(profile).Elements("staticContent").Elements("mimeMap").Any();
			var types = serverMap ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>(IisTypes, StringComparer.OrdinalIgnoreCase);
			var clientCache = new ClientCache();
			var relative = VirtualPathUtility.ToAppRelative(virtualPath).TrimStart('~', '/');
			var folders = relative.Split('/');
			var folder = "";
			for (var depth = -1; depth < folders.Length; depth++)
			{
				// depth -1: the profile (shaped as a web.config of the site: system.webServer, locations by path).
				var root = depth < 0 ? profile : Document(HostingEnvironment.MapPath("~/" + folder + "web.config"));
				if (root != null)
				{
					var rest = relative.Substring(folder.Length);
					var sections = root.Elements("system.webServer").Elements("staticContent")
						.Concat(root.Elements("location")
							.Select(l => (Path: ((string)l.Attribute("path") ?? "").Trim('/'), Location: l))
							.Where(l => l.Path.Length == 0 || l.Path == "." || rest.Equals(l.Path, StringComparison.OrdinalIgnoreCase) ||
								rest.StartsWith(l.Path + "/", StringComparison.OrdinalIgnoreCase))
							.OrderBy(l => l.Path.Length)
							.SelectMany(l => l.Location.Elements("system.webServer").Elements("staticContent")));
					foreach (var section in sections)
					{
						foreach (var entry in section.Elements())
						{
							var extension = (string)entry.Attribute("fileExtension");
							switch (entry.Name.LocalName)
							{
								case "clear": types.Clear(); break;
								case "remove": if (extension != null) types.Remove(extension); break;
								case "mimeMap":
									var type = (string)entry.Attribute("mimeType");
									if (extension != null && type != null) types[extension] = type;
									break;
								case "clientCache": clientCache.Read(entry); break;
							}
						}
					}
				}
				if (depth >= 0 && depth < folders.Length - 1) folder += folders[depth] + "/";
			}
			return (types, clientCache);
		}

		// IIS 10's own MIME map (applicationHost.config's staticContent).
		static readonly Dictionary<string, string> IisTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{ ".323", "text/h323" },
			{ ".3g2", "video/3gpp2" },
			{ ".3gp2", "video/3gpp2" },
			{ ".3gp", "video/3gpp" },
			{ ".3gpp", "video/3gpp" },
			{ ".aaf", "application/octet-stream" },
			{ ".aac", "audio/aac" },
			{ ".aca", "application/octet-stream" },
			{ ".accdb", "application/msaccess" },
			{ ".accde", "application/msaccess" },
			{ ".accdt", "application/msaccess" },
			{ ".acx", "application/internet-property-stream" },
			{ ".adt", "audio/vnd.dlna.adts" },
			{ ".adts", "audio/vnd.dlna.adts" },
			{ ".afm", "application/octet-stream" },
			{ ".ai", "application/postscript" },
			{ ".aif", "audio/x-aiff" },
			{ ".aifc", "audio/aiff" },
			{ ".aiff", "audio/aiff" },
			{ ".appcache", "text/cache-manifest" },
			{ ".application", "application/x-ms-application" },
			{ ".appx", "application/vns.ms-appx" },
			{ ".appxbundle", "application/vnd.ms-appx.bundle" },
			{ ".art", "image/x-jg" },
			{ ".asd", "application/octet-stream" },
			{ ".asf", "video/x-ms-asf" },
			{ ".asi", "application/octet-stream" },
			{ ".asm", "text/plain" },
			{ ".asr", "video/x-ms-asf" },
			{ ".asx", "video/x-ms-asf" },
			{ ".atom", "application/atom+xml" },
			{ ".au", "audio/basic" },
			{ ".avi", "video/avi" },
			{ ".axs", "application/olescript" },
			{ ".bas", "text/plain" },
			{ ".bcpio", "application/x-bcpio" },
			{ ".bin", "application/octet-stream" },
			{ ".bmp", "image/bmp" },
			{ ".c", "text/plain" },
			{ ".cab", "application/vnd.ms-cab-compressed" },
			{ ".calx", "application/vnd.ms-office.calx" },
			{ ".cat", "application/vnd.ms-pki.seccat" },
			{ ".cdf", "application/x-cdf" },
			{ ".chm", "application/octet-stream" },
			{ ".class", "application/x-java-applet" },
			{ ".clp", "application/x-msclip" },
			{ ".cmx", "image/x-cmx" },
			{ ".cnf", "text/plain" },
			{ ".cod", "image/cis-cod" },
			{ ".cpio", "application/x-cpio" },
			{ ".cpp", "text/plain" },
			{ ".crd", "application/x-mscardfile" },
			{ ".crl", "application/pkix-crl" },
			{ ".crt", "application/x-x509-ca-cert" },
			{ ".csh", "application/x-csh" },
			{ ".css", "text/css" },
			{ ".csv", "application/octet-stream" },
			{ ".cur", "application/octet-stream" },
			{ ".dcr", "application/x-director" },
			{ ".deploy", "application/octet-stream" },
			{ ".der", "application/x-x509-ca-cert" },
			{ ".dib", "image/bmp" },
			{ ".dir", "application/x-director" },
			{ ".disco", "text/xml" },
			{ ".dll", "application/x-msdownload" },
			{ ".dll.config", "text/xml" },
			{ ".dlm", "text/dlm" },
			{ ".doc", "application/msword" },
			{ ".docm", "application/vnd.ms-word.document.macroEnabled.12" },
			{ ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
			{ ".dot", "application/msword" },
			{ ".dotm", "application/vnd.ms-word.template.macroEnabled.12" },
			{ ".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
			{ ".dsp", "application/octet-stream" },
			{ ".dtd", "text/xml" },
			{ ".dvi", "application/x-dvi" },
			{ ".dvr-ms", "video/x-ms-dvr" },
			{ ".dwf", "drawing/x-dwf" },
			{ ".dwp", "application/octet-stream" },
			{ ".dxr", "application/x-director" },
			{ ".eml", "message/rfc822" },
			{ ".emz", "application/octet-stream" },
			{ ".eot", "application/vnd.ms-fontobject" },
			{ ".eps", "application/postscript" },
			{ ".esd", "application/vnd.ms-cab-compressed" },
			{ ".etx", "text/x-setext" },
			{ ".evy", "application/envoy" },
			{ ".exe", "application/octet-stream" },
			{ ".exe.config", "text/xml" },
			{ ".fdf", "application/vnd.fdf" },
			{ ".fif", "application/fractals" },
			{ ".fla", "application/octet-stream" },
			{ ".flr", "x-world/x-vrml" },
			{ ".flv", "video/x-flv" },
			{ ".gif", "image/gif" },
			{ ".glb", "model/gltf-binary" },
			{ ".gtar", "application/x-gtar" },
			{ ".gz", "application/x-gzip" },
			{ ".h", "text/plain" },
			{ ".hdf", "application/x-hdf" },
			{ ".hdml", "text/x-hdml" },
			{ ".hhc", "application/x-oleobject" },
			{ ".hhk", "application/octet-stream" },
			{ ".hhp", "application/octet-stream" },
			{ ".hlp", "application/winhlp" },
			{ ".hqx", "application/mac-binhex40" },
			{ ".hta", "application/hta" },
			{ ".htc", "text/x-component" },
			{ ".htm", "text/html" },
			{ ".html", "text/html" },
			{ ".htt", "text/webviewhtml" },
			{ ".hxt", "text/html" },
			{ ".ico", "image/x-icon" },
			{ ".ics", "text/calendar" },
			{ ".ief", "image/ief" },
			{ ".iii", "application/x-iphone" },
			{ ".inf", "application/octet-stream" },
			{ ".ins", "application/x-internet-signup" },
			{ ".isp", "application/x-internet-signup" },
			{ ".IVF", "video/x-ivf" },
			{ ".jar", "application/java-archive" },
			{ ".java", "application/octet-stream" },
			{ ".jck", "application/liquidmotion" },
			{ ".jcz", "application/liquidmotion" },
			{ ".jfif", "image/pjpeg" },
			{ ".jpb", "application/octet-stream" },
			{ ".jpe", "image/jpeg" },
			{ ".jpeg", "image/jpeg" },
			{ ".jpg", "image/jpeg" },
			{ ".js", "application/javascript" },
			{ ".json", "application/json" },
			{ ".jsonld", "application/ld+json" },
			{ ".jsx", "text/jscript" },
			{ ".latex", "application/x-latex" },
			{ ".less", "text/css" },
			{ ".lit", "application/x-ms-reader" },
			{ ".lpk", "application/octet-stream" },
			{ ".lsf", "video/x-la-asf" },
			{ ".lsx", "video/x-la-asf" },
			{ ".lzh", "application/octet-stream" },
			{ ".m13", "application/x-msmediaview" },
			{ ".m14", "application/x-msmediaview" },
			{ ".m1v", "video/mpeg" },
			{ ".m2ts", "video/vnd.dlna.mpeg-tts" },
			{ ".m3u", "audio/x-mpegurl" },
			{ ".m4a", "audio/mp4" },
			{ ".m4v", "video/mp4" },
			{ ".man", "application/x-troff-man" },
			{ ".manifest", "application/x-ms-manifest" },
			{ ".map", "text/plain" },
			{ ".mdb", "application/x-msaccess" },
			{ ".mdp", "application/octet-stream" },
			{ ".me", "application/x-troff-me" },
			{ ".mht", "message/rfc822" },
			{ ".mhtml", "message/rfc822" },
			{ ".mid", "audio/mid" },
			{ ".midi", "audio/mid" },
			{ ".mix", "application/octet-stream" },
			{ ".mmf", "application/x-smaf" },
			{ ".mno", "text/xml" },
			{ ".mny", "application/x-msmoney" },
			{ ".mov", "video/quicktime" },
			{ ".movie", "video/x-sgi-movie" },
			{ ".mp2", "video/mpeg" },
			{ ".mp3", "audio/mpeg" },
			{ ".mp4", "video/mp4" },
			{ ".mp4v", "video/mp4" },
			{ ".mpa", "video/mpeg" },
			{ ".mpe", "video/mpeg" },
			{ ".mpeg", "video/mpeg" },
			{ ".mpg", "video/mpeg" },
			{ ".mpp", "application/vnd.ms-project" },
			{ ".mpv2", "video/mpeg" },
			{ ".ms", "application/x-troff-ms" },
			{ ".msi", "application/octet-stream" },
			{ ".msix", "application/msix" },
			{ ".msixbundle", "application/vnd.ms-appx.bundle" },
			{ ".mso", "application/octet-stream" },
			{ ".msu", "application/octet-stream" },
			{ ".mvb", "application/x-msmediaview" },
			{ ".mvc", "application/x-miva-compiled" },
			{ ".nc", "application/x-netcdf" },
			{ ".nsc", "video/x-ms-asf" },
			{ ".nws", "message/rfc822" },
			{ ".ocx", "application/octet-stream" },
			{ ".oda", "application/oda" },
			{ ".odc", "text/x-ms-odc" },
			{ ".ods", "application/oleobject" },
			{ ".oga", "audio/ogg" },
			{ ".ogg", "video/ogg" },
			{ ".ogv", "video/ogg" },
			{ ".one", "application/onenote" },
			{ ".onea", "application/onenote" },
			{ ".onetoc", "application/onenote" },
			{ ".onetoc2", "application/onenote" },
			{ ".onetmp", "application/onenote" },
			{ ".onepkg", "application/onenote" },
			{ ".osdx", "application/opensearchdescription+xml" },
			{ ".otf", "font/otf" },
			{ ".p10", "application/pkcs10" },
			{ ".p12", "application/x-pkcs12" },
			{ ".p7b", "application/x-pkcs7-certificates" },
			{ ".p7c", "application/pkcs7-mime" },
			{ ".p7m", "application/pkcs7-mime" },
			{ ".p7r", "application/x-pkcs7-certreqresp" },
			{ ".p7s", "application/pkcs7-signature" },
			{ ".pbm", "image/x-portable-bitmap" },
			{ ".pcx", "application/octet-stream" },
			{ ".pcz", "application/octet-stream" },
			{ ".pdf", "application/pdf" },
			{ ".pfb", "application/octet-stream" },
			{ ".pfm", "application/octet-stream" },
			{ ".pfx", "application/x-pkcs12" },
			{ ".pgm", "image/x-portable-graymap" },
			{ ".pko", "application/vnd.ms-pki.pko" },
			{ ".pma", "application/x-perfmon" },
			{ ".pmc", "application/x-perfmon" },
			{ ".pml", "application/x-perfmon" },
			{ ".pmr", "application/x-perfmon" },
			{ ".pmw", "application/x-perfmon" },
			{ ".png", "image/png" },
			{ ".pnm", "image/x-portable-anymap" },
			{ ".pnz", "image/png" },
			{ ".pot", "application/vnd.ms-powerpoint" },
			{ ".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12" },
			{ ".potx", "application/vnd.openxmlformats-officedocument.presentationml.template" },
			{ ".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12" },
			{ ".ppm", "image/x-portable-pixmap" },
			{ ".pps", "application/vnd.ms-powerpoint" },
			{ ".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12" },
			{ ".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
			{ ".ppt", "application/vnd.ms-powerpoint" },
			{ ".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12" },
			{ ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
			{ ".prf", "application/pics-rules" },
			{ ".prm", "application/octet-stream" },
			{ ".prx", "application/octet-stream" },
			{ ".ps", "application/postscript" },
			{ ".psd", "application/octet-stream" },
			{ ".psm", "application/octet-stream" },
			{ ".psp", "application/octet-stream" },
			{ ".pub", "application/x-mspublisher" },
			{ ".qt", "video/quicktime" },
			{ ".qtl", "application/x-quicktimeplayer" },
			{ ".qxd", "application/octet-stream" },
			{ ".ra", "audio/x-pn-realaudio" },
			{ ".ram", "audio/x-pn-realaudio" },
			{ ".rar", "application/octet-stream" },
			{ ".ras", "image/x-cmu-raster" },
			{ ".rf", "image/vnd.rn-realflash" },
			{ ".rgb", "image/x-rgb" },
			{ ".rm", "application/vnd.rn-realmedia" },
			{ ".rmi", "audio/mid" },
			{ ".roff", "application/x-troff" },
			{ ".rpm", "audio/x-pn-realaudio-plugin" },
			{ ".rtf", "application/rtf" },
			{ ".rtx", "text/richtext" },
			{ ".scd", "application/x-msschedule" },
			{ ".sct", "text/scriptlet" },
			{ ".sea", "application/octet-stream" },
			{ ".setpay", "application/set-payment-initiation" },
			{ ".setreg", "application/set-registration-initiation" },
			{ ".sgml", "text/sgml" },
			{ ".sh", "application/x-sh" },
			{ ".shar", "application/x-shar" },
			{ ".sit", "application/x-stuffit" },
			{ ".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12" },
			{ ".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide" },
			{ ".smd", "audio/x-smd" },
			{ ".smi", "application/octet-stream" },
			{ ".smx", "audio/x-smd" },
			{ ".smz", "audio/x-smd" },
			{ ".snd", "audio/basic" },
			{ ".snp", "application/octet-stream" },
			{ ".spc", "application/x-pkcs7-certificates" },
			{ ".spl", "application/futuresplash" },
			{ ".spx", "audio/ogg" },
			{ ".src", "application/x-wais-source" },
			{ ".ssm", "application/streamingmedia" },
			{ ".sst", "application/vnd.ms-pki.certstore" },
			{ ".stl", "application/vnd.ms-pki.stl" },
			{ ".sv4cpio", "application/x-sv4cpio" },
			{ ".sv4crc", "application/x-sv4crc" },
			{ ".svg", "image/svg+xml" },
			{ ".svgz", "image/svg+xml" },
			{ ".swf", "application/x-shockwave-flash" },
			{ ".t", "application/x-troff" },
			{ ".tar", "application/x-tar" },
			{ ".tcl", "application/x-tcl" },
			{ ".tex", "application/x-tex" },
			{ ".texi", "application/x-texinfo" },
			{ ".texinfo", "application/x-texinfo" },
			{ ".tgz", "application/x-compressed" },
			{ ".thmx", "application/vnd.ms-officetheme" },
			{ ".thn", "application/octet-stream" },
			{ ".tif", "image/tiff" },
			{ ".tiff", "image/tiff" },
			{ ".toc", "application/octet-stream" },
			{ ".tr", "application/x-troff" },
			{ ".trm", "application/x-msterminal" },
			{ ".ts", "video/vnd.dlna.mpeg-tts" },
			{ ".tsv", "text/tab-separated-values" },
			{ ".ttf", "application/octet-stream" },
			{ ".tts", "video/vnd.dlna.mpeg-tts" },
			{ ".txt", "text/plain" },
			{ ".u32", "application/octet-stream" },
			{ ".uls", "text/iuls" },
			{ ".ustar", "application/x-ustar" },
			{ ".vbs", "text/vbscript" },
			{ ".vcf", "text/x-vcard" },
			{ ".vcs", "text/plain" },
			{ ".vdx", "application/vnd.ms-visio.viewer" },
			{ ".vml", "text/xml" },
			{ ".vsd", "application/vnd.visio" },
			{ ".vss", "application/vnd.visio" },
			{ ".vst", "application/vnd.visio" },
			{ ".vsto", "application/x-ms-vsto" },
			{ ".vsw", "application/vnd.visio" },
			{ ".vsx", "application/vnd.visio" },
			{ ".vtx", "application/vnd.visio" },
			{ ".wasm", "application/wasm" },
			{ ".wav", "audio/wav" },
			{ ".wax", "audio/x-ms-wax" },
			{ ".wbmp", "image/vnd.wap.wbmp" },
			{ ".wcm", "application/vnd.ms-works" },
			{ ".wdb", "application/vnd.ms-works" },
			{ ".webm", "video/webm" },
			{ ".wim", "application/x-ms-wim" },
			{ ".wks", "application/vnd.ms-works" },
			{ ".wm", "video/x-ms-wm" },
			{ ".wma", "audio/x-ms-wma" },
			{ ".wmd", "application/x-ms-wmd" },
			{ ".wmf", "application/x-msmetafile" },
			{ ".wml", "text/vnd.wap.wml" },
			{ ".wmlc", "application/vnd.wap.wmlc" },
			{ ".wmls", "text/vnd.wap.wmlscript" },
			{ ".wmlsc", "application/vnd.wap.wmlscriptc" },
			{ ".wmp", "video/x-ms-wmp" },
			{ ".wmv", "video/x-ms-wmv" },
			{ ".wmx", "video/x-ms-wmx" },
			{ ".wmz", "application/x-ms-wmz" },
			{ ".woff", "font/x-woff" },
			{ ".woff2", "application/font-woff2" },
			{ ".wps", "application/vnd.ms-works" },
			{ ".wri", "application/x-mswrite" },
			{ ".wrl", "x-world/x-vrml" },
			{ ".wrz", "x-world/x-vrml" },
			{ ".wsdl", "text/xml" },
			{ ".wtv", "video/x-ms-wtv" },
			{ ".wvx", "video/x-ms-wvx" },
			{ ".x", "application/directx" },
			{ ".xaf", "x-world/x-vrml" },
			{ ".xaml", "application/xaml+xml" },
			{ ".xap", "application/x-silverlight-app" },
			{ ".xbap", "application/x-ms-xbap" },
			{ ".xbm", "image/x-xbitmap" },
			{ ".xdr", "text/plain" },
			{ ".xht", "application/xhtml+xml" },
			{ ".xhtml", "application/xhtml+xml" },
			{ ".xla", "application/vnd.ms-excel" },
			{ ".xlam", "application/vnd.ms-excel.addin.macroEnabled.12" },
			{ ".xlc", "application/vnd.ms-excel" },
			{ ".xlm", "application/vnd.ms-excel" },
			{ ".xls", "application/vnd.ms-excel" },
			{ ".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12" },
			{ ".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12" },
			{ ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
			{ ".xlt", "application/vnd.ms-excel" },
			{ ".xltm", "application/vnd.ms-excel.template.macroEnabled.12" },
			{ ".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template" },
			{ ".xlw", "application/vnd.ms-excel" },
			{ ".xml", "text/xml" },
			{ ".xof", "x-world/x-vrml" },
			{ ".xpm", "image/x-xpixmap" },
			{ ".xps", "application/vnd.ms-xpsdocument" },
			{ ".xsd", "text/xml" },
			{ ".xsf", "text/xml" },
			{ ".xsl", "text/xml" },
			{ ".xslt", "text/xml" },
			{ ".xsn", "application/octet-stream" },
			{ ".xtp", "application/octet-stream" },
			{ ".xwd", "image/x-xwindowdump" },
			{ ".z", "application/x-compress" },
			{ ".zip", "application/x-zip-compressed" }
		};
	}
}

#endif
