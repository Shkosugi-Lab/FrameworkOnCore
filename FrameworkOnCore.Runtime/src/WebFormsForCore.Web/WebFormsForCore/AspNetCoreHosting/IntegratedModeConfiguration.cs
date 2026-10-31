#if NETCOREAPP

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Xml.Linq;

namespace System.Web
{
	/// <summary>
	/// The IIS integrated pipeline's configuration of the application (web.config's system.webServer), in
	/// the classic pipeline this runtime has. An application written for integrated mode - IIS's default
	/// since IIS 7 - declares its modules and handlers there only (DNN: the URL rewriter that sets up every
	/// page); IIS reads it, and the classic pipeline reads system.web's httpModules and httpHandlers.
	/// Only the managed entries (with a type: the others are IIS's native modules and handlers).
	/// </summary>
	internal static class IntegratedModeConfiguration
	{
		internal sealed record Handler(string Name, string Path, string Verb, string Type);

		static readonly object gate = new object();
		static (DateTime Written, List<XElement> Modules, List<Handler> Handlers, bool RunAllManagedModules) cache;
		static long cacheCheckedUntil;

		// web.config's time looked at once a second at most, not on every request (a file system call each: on Linux, through
		// the case-insensitive file names' library too).
		const int CheckEveryMilliseconds = 1000;

		// The application's web.config (read again when it changes: DNN writes it at run time).
		static (List<XElement> Modules, List<Handler> Handlers, bool RunAllManagedModules) Read()
		{
			var cached = cache;
			if (cached.Modules != null && Environment.TickCount64 < Volatile.Read(ref cacheCheckedUntil)) return (cached.Modules, cached.Handlers, cached.RunAllManagedModules);
			Volatile.Write(ref cacheCheckedUntil, Environment.TickCount64 + CheckEveryMilliseconds);
			var file = HostingEnvironment.MapPath("~/web.config");
			if (file == null || !File.Exists(file)) return (new List<XElement>(), new List<Handler>(), false);
			var written = File.GetLastWriteTimeUtc(file);
			lock (gate)
			{
				if (cache.Modules != null && cache.Written == written) return (cache.Modules, cache.Handlers, cache.RunAllManagedModules);
				var modules = new List<XElement>();
				var handlers = new List<Handler>();
				var runAll = false;
				try
				{
					var root = XDocument.Load(file).Root;
					// system.webServer directly under configuration, or in a <location> for the application itself.
					var sections = root.Elements("system.webServer")
						.Concat(root.Elements("location")
							.Where(l => { var p = (string)l.Attribute("path"); return string.IsNullOrEmpty(p) || p == "."; })
							.Elements("system.webServer"));
					foreach (var section in sections)
					{
						modules.AddRange(section.Elements("modules").Elements());
						foreach (var element in section.Elements("modules"))
							if ((string)element.Attribute("runAllManagedModulesForAllRequests") is { } all) runAll = string.Equals(all.Trim(), "true", StringComparison.OrdinalIgnoreCase);
						ApplyHandlers(handlers, section);
					}
				}
				catch (Exception e) when (e is System.Xml.XmlException || e is IOException)
				{
				}
				cache = (written, modules, handlers, runAll);
				return (modules, handlers, runAll);
			}
		}

		// A system.webServer section's handlers over the ones before it.
		static void ApplyHandlers(List<Handler> handlers, XElement section)
		{
			foreach (var entry in section.Elements().Where(e => e.Name.LocalName == "handlers").Elements())
			{
				var name = (string)entry.Attribute("name") ?? "";
				switch (entry.Name.LocalName)
				{
					case "clear": handlers.Clear(); break;
					case "remove": handlers.RemoveAll(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); break;
					case "add":
						var type = (string)entry.Attribute("type");
						var path = (string)entry.Attribute("path");
						// IIS's TransferRequestHandler (extensionless URLs to the managed pipeline): here every
						// request reaches it already.
						if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(path) || type.StartsWith("System.Web.Handlers.TransferRequestHandler", StringComparison.Ordinal)) break;
						handlers.RemoveAll(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
						handlers.Add(new Handler(name, path, (string)entry.Attribute("verb") ?? "*", type));
						break;
				}
			}
		}

		// The handlers of a folder with handlers of its own (a <location path>'s, a folder's web.config's: IIS 10's handlers
		// section is delegated, overrideMode Allow, no allowDefinition), made from the application's ones it was made from.
		static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (List<Handler> From, List<Handler> Handlers)> handlersByFolder =
			new System.Collections.Concurrent.ConcurrentDictionary<string, (List<Handler>, List<Handler>)>(StringComparer.OrdinalIgnoreCase);

		// The handlers in effect for the URL: the application's (web.config's own section, its location for itself), then
		// the sections of the folders down to the URL's (IisConfiguration's order; not the original server's: its managed
		// handlers are system.web's here).
		static List<Handler> HandlersFor(string virtualPath)
		{
			var handlers = Read().Handlers;
			var root = IisConfiguration.Root;
			if (root == null) return handlers;
			var folder = IisConfiguration.Deepest(root, virtualPath);
			if (folder.Length == 0) return handlers;
			var key = root + "|" + folder;
			if (handlersByFolder.TryGetValue(key, out var made) && made.From == handlers) return made.Handlers;
			var list = new List<Handler>(handlers);
			foreach (var section in IisConfiguration.For(root, virtualPath))
				if (!section.Server && section.Base.Length > 0) ApplyHandlers(list, section.WebServer);
			handlersByFolder[key] = (handlers, list);
			return list;
		}

		/// <summary>
		/// Whether IIS runs the managed modules of preCondition "managedHandler" for the request (UrlRoutingModule-4.0's in
		/// applicationHost.config): only when its handler is a managed one, unless system.webServer/modules has
		/// runAllManagedModulesForAllRequests="true" (learn.microsoft.com/iis/configuration/system.webserver/modules).
		/// IIS's handlers: a file name without a '.' goes to ExtensionlessUrlHandler-Integrated-4.0 (path "*.",
		/// TransferRequestHandler: managed); one with a '.' to a managed handler of system.webServer/handlers, else
		/// system.web's mapping here (.aspx, .ashx, .asmx, .axd, the forbidden .cs, .config...: IIS's -Integrated-4.0
		/// entries), else IIS's StaticFile (here the catch-all "*": DefaultHttpHandler, HttpMethodNotAllowedHandler).
		/// Seen on IIS 10 (NavigationProbe): the catch-all route's /files/x.txt is IIS's 404, /files/a/b c the route's page.
		/// </summary>
		internal static bool RunsManagedHandlerModules(HttpContext context)
		{
			if (Read().RunAllManagedModules) return true;
			var path = context.Request.FilePathObject;
			var name = path.VirtualPathString;
			name = name.Substring(name.LastIndexOf('/') + 1);
			if (name.IndexOf('.') < 0) return true;
			var verb = context.Request.HttpMethod;
			if (FindHandlerType(verb, path) != null) return true;
			var mapping = RuntimeConfig.GetConfig(context).HttpHandlers.FindMapping(verb, path);
			if (mapping == null) return false;
			var type = mapping.Type ?? "";
			return !(type.StartsWith("System.Web.DefaultHttpHandler", StringComparison.Ordinal) ||
				type.StartsWith("System.Web.HttpMethodNotAllowedHandler", StringComparison.Ordinal) ||
				type.StartsWith("System.Web.StaticFileHandler", StringComparison.Ordinal));
		}

		/// <summary>The application's modules after its system.webServer/modules, in IIS's order.</summary>
		internal static HttpModuleCollection ApplyModules(HttpModuleCollection modules)
		{
			var entries = Read().Modules;
			if (entries.Count == 0) return modules;
			var list = Enumerable.Range(0, modules.Count).Select(i => (Name: modules.GetKey(i), Module: modules[i])).ToList();
			foreach (var entry in entries)
			{
				var name = (string)entry.Attribute("name") ?? "";
				switch (entry.Name.LocalName)
				{
					case "clear": list.Clear(); break;
					case "remove": list.RemoveAll(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); break;
					case "add":
						var type = (string)entry.Attribute("type");
						if (string.IsNullOrEmpty(type)) break;
						list.RemoveAll(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
						list.Add((name, new System.Web.Configuration.Common.ModulesEntry(name, type, "type", null).Create()));
						break;
				}
			}
			var result = new HttpModuleCollection();
			foreach (var (moduleName, module) in list) result.AddModule(moduleName, module);
			return result;
		}

		/// <summary>The application's handler for the request in system.webServer/handlers; null if none.</summary>
		internal static string FindHandlerType(string verb, VirtualPath path)
		{
			var virtualPath = path.VirtualPathString;
			var handlers = HandlersFor(virtualPath);
			if (handlers.Count == 0) return null;
			var fileName = virtualPath.Substring(virtualPath.LastIndexOf('/') + 1);
			foreach (var handler in handlers)
			{
				// Without a '/', IIS matches the path against the request's file name. With one (seen on IIS 10, a folder's
				// web.config): from a '/' the whole URL path ("/T/H/Root.ashx"; "/H/Abs.ashx" does not answer /T/H/Abs.ashx);
				// else the end of the URL's path, whole segments ("Z/R.ashx" answers .../Z/R.ashx and .../X/Z/R.ashx, not
				// .../XZ/R.ashx; "Y/Redirect.ashx" of the application's answers /Y/Redirect.ashx); '*' within a segment
				// ("H/*.foo" answers /T/H/a.foo, not /T/H/Sub/a.foo). The host's choice: "~/" is the whole path too (IIS takes
				// '~' as a character: it never matches).
				var matches = handler.Path.Contains('/')
					? Wildcard(handler.Path, segments: true).IsMatch(virtualPath)
					: Wildcard(handler.Path.TrimStart('~'), segments: false).IsMatch(fileName);
				if (!matches) continue;
				if (handler.Verb != "*" && !handler.Verb.Split(',').Any(v => v.Trim().Equals(verb, StringComparison.OrdinalIgnoreCase))) continue;
				return handler.Type;
			}
			return null;
		}

		static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Tuple<long, int, int>> limitsByFolder =
			new System.Collections.Concurrent.ConcurrentDictionary<string, Tuple<long, int, int>>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// IIS's request filtering's limits for the URL (system.webServer/security/requestFiltering/requestLimits):
		/// maxAllowedContentLength (30,000,000 bytes), maxUrl (4,096), maxQueryString (2,048), IIS's defaults; the original
		/// server's over them (IisProfile), then web.config's, its locations' and its folders' web.config files' down to the
		/// URL's folder (IisConfiguration: an upload folder's larger limit).
		/// </summary>
		internal static (long MaxAllowedContentLength, int MaxUrl, int MaxQueryString) RequestLimits(string path)
		{
			var root = IisConfiguration.Root;
			if (root == null) return (30000000L, 4096, 2048);
			var limits = limitsByFolder.GetOrAdd(IisConfiguration.Deepest(root, path), _ =>
			{
				var folded = Limits((30000000L, 4096, 2048), IisConfiguration.For(root, path).Select(s => s.WebServer));
				return Tuple.Create(folded.Item1, folded.Item2, folded.Item3);
			});
			return (limits.Item1, limits.Item2, limits.Item3);
		}

		static (long, int, int) Limits((long, int, int) limits, IEnumerable<XElement> webServers)
		{
			foreach (var element in webServers.SelectMany(w => IisConfiguration.Children(w, "security")).SelectMany(s => IisConfiguration.Children(s, "requestFiltering"))
				.SelectMany(r => IisConfiguration.Children(r, "requestLimits")))
			{
				long content; int url, query;
				if (long.TryParse((string)element.Attribute("maxAllowedContentLength"), out content)) limits.Item1 = content;
				if (int.TryParse((string)element.Attribute("maxUrl"), out url)) limits.Item2 = url;
				if (int.TryParse((string)element.Attribute("maxQueryString"), out query)) limits.Item3 = query;
			}
			return limits;
		}

		/// <summary>
		/// IIS's request filtering for a URL (system.webServer/security/requestFiltering): its hidden segments (404.8), its file
		/// extensions (allowed or not, 404.7; allowUnlisted false: only the allowed ones), its denied URL sequences (404.5) and
		/// allowDoubleEscaping (404.11).
		/// </summary>
		internal sealed class Filtering
		{
			public HashSet<string> HiddenSegments;
			public Dictionary<string, bool> Extensions;   // extension ("." and its name): allowed
			public bool AllowUnlisted = true;
			public bool AllowDoubleEscaping;
			public string[] DenyUrlSequences = Array.Empty<string>();
		}

		// applicationHost.config's (IIS 10 with ASP.NET 4.8): its hiddenSegments and the extensions it denies.
		internal static readonly string[] IisHiddenSegments =
			{ "web.config", "bin", "App_code", "App_GlobalResources", "App_LocalResources", "App_WebReferences", "App_Data", "App_Browsers" };

		internal static readonly string[] IisDeniedExtensions =
		{
			".asax", ".ascx", ".master", ".skin", ".browser", ".sitemap", ".config", ".cs", ".csproj", ".vb", ".vbproj", ".webinfo",
			".licx", ".resx", ".resources", ".mdb", ".vjsproj", ".java", ".jsl", ".ldb", ".dsdgm", ".ssdgm", ".lsad", ".ssmap", ".cd",
			".dsprototype", ".lsaprototype", ".sdm", ".sdmDocument", ".mdf", ".ldf", ".ad", ".dd", ".ldd", ".sd", ".adprototype",
			".lddprototype", ".exclude", ".refresh", ".compiled", ".msgx", ".vsdisco", ".rules",
		};

		static readonly Filtering iisFiltering = Fold(new Filtering
		{
			HiddenSegments = new HashSet<string>(IisHiddenSegments, StringComparer.OrdinalIgnoreCase),
			Extensions = IisDeniedExtensions.ToDictionary(e => e, e => false, StringComparer.OrdinalIgnoreCase),
		}, Enumerable.Empty<XElement>());

		static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Filtering> filteringByFolder =
			new System.Collections.Concurrent.ConcurrentDictionary<string, Filtering>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// The request filtering IIS applies to the URL's path (decoded, from '/'): applicationHost.config's, the original
		/// server's over it (IisProfile), then web.config's, its locations' and its folders' web.config files' down to the URL's
		/// folder (their add, remove and clear). Without a running application, applicationHost.config's.
		/// </summary>
		internal static Filtering RequestFiltering(string path)
		{
			string root;
			try { root = IisConfiguration.Root; }
			catch (Exception e) when (e is HttpException || e is ArgumentException || e is InvalidOperationException) { root = null; }
			if (root == null) return iisFiltering;
			return filteringByFolder.GetOrAdd(root + "|" + IisConfiguration.Deepest(root, path ?? "/"), _ =>
				Fold(new Filtering
				{
					HiddenSegments = new HashSet<string>(iisFiltering.HiddenSegments, StringComparer.OrdinalIgnoreCase),
					Extensions = new Dictionary<string, bool>(iisFiltering.Extensions, StringComparer.OrdinalIgnoreCase),
				}, IisConfiguration.For(root, path ?? "/").Select(s => s.WebServer)));
		}

		static Filtering Fold(Filtering filtering, IEnumerable<XElement> webServers)
		{
			var sequences = new List<string>(filtering.DenyUrlSequences);
			foreach (var element in webServers.SelectMany(w => IisConfiguration.Children(w, "security")).SelectMany(s => IisConfiguration.Children(s, "requestFiltering")))
			{
				var doubleEscaping = (string)element.Attribute("allowDoubleEscaping");
				if (doubleEscaping != null) filtering.AllowDoubleEscaping = string.Equals(doubleEscaping, "true", StringComparison.OrdinalIgnoreCase);
				foreach (var segments in IisConfiguration.Children(element, "hiddenSegments"))
					foreach (var item in segments.Elements())
					{
						var segment = ((string)item.Attribute("segment"))?.Trim();
						switch (item.Name.LocalName)
						{
							case "clear": filtering.HiddenSegments.Clear(); break;
							case "add" when !string.IsNullOrEmpty(segment): filtering.HiddenSegments.Add(segment); break;
							case "remove" when !string.IsNullOrEmpty(segment): filtering.HiddenSegments.Remove(segment); break;
						}
					}
				foreach (var extensions in IisConfiguration.Children(element, "fileExtensions"))
				{
					var unlisted = (string)extensions.Attribute("allowUnlisted");
					if (unlisted != null) filtering.AllowUnlisted = !string.Equals(unlisted, "false", StringComparison.OrdinalIgnoreCase);
					foreach (var item in extensions.Elements())
					{
						var extension = ((string)item.Attribute("fileExtension"))?.Trim();
						switch (item.Name.LocalName)
						{
							case "clear": filtering.Extensions.Clear(); break;
							case "add" when !string.IsNullOrEmpty(extension):
								filtering.Extensions[extension] = string.Equals((string)item.Attribute("allowed"), "true", StringComparison.OrdinalIgnoreCase);
								break;
							case "remove" when !string.IsNullOrEmpty(extension): filtering.Extensions.Remove(extension); break;
						}
					}
				}
				foreach (var deny in IisConfiguration.Children(element, "denyUrlSequences"))
					foreach (var item in deny.Elements())
					{
						var sequence = (string)item.Attribute("sequence");
						switch (item.Name.LocalName)
						{
							case "clear": sequences.Clear(); break;
							case "add" when !string.IsNullOrEmpty(sequence): sequences.Add(sequence); break;
							case "remove" when !string.IsNullOrEmpty(sequence): sequences.RemoveAll(s => string.Equals(s, sequence, StringComparison.OrdinalIgnoreCase)); break;
						}
					}
			}
			filtering.DenyUrlSequences = sequences.ToArray();
			return filtering;
		}

		// A handler's path as a regular expression, made once for each path (it was made in each request).
		static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Regex> wildcards = new System.Collections.Concurrent.ConcurrentDictionary<string, Regex>(StringComparer.Ordinal);

		// segments: a handler's path with a folder in it, matched against the URL's path: from its start ('/', "~/"), else
		// from any '/' to its end; '*' and '?' within a segment.
		static Regex Wildcard(string pattern, bool segments) => wildcards.GetOrAdd((segments ? "|" : "") + pattern, key =>
		{
			if (!segments)
				return new Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			var anchored = pattern.StartsWith("/", StringComparison.Ordinal) || pattern.StartsWith("~/", StringComparison.Ordinal);
			var rest = pattern.TrimStart('~').TrimStart('/');
			return new Regex((anchored ? "^/" : "(^|/)") + Regex.Escape(rest).Replace(@"\*", "[^/]*").Replace(@"\?", "[^/]") + "$",
				RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		});
	}
}

#endif
