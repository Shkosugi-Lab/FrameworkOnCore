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
		static (DateTime Written, List<XElement> Modules, List<Handler> Handlers) cache;
		static long cacheCheckedUntil;

		// web.config's time looked at once a second at most, not on every request (a file system call each: on Linux, through
		// the case-insensitive file names' library too).
		const int CheckEveryMilliseconds = 1000;

		// The application's web.config (read again when it changes: DNN writes it at run time).
		static (List<XElement> Modules, List<Handler> Handlers) Read()
		{
			var cached = cache;
			if (cached.Modules != null && Environment.TickCount64 < Volatile.Read(ref cacheCheckedUntil)) return (cached.Modules, cached.Handlers);
			Volatile.Write(ref cacheCheckedUntil, Environment.TickCount64 + CheckEveryMilliseconds);
			var file = HostingEnvironment.MapPath("~/web.config");
			if (file == null || !File.Exists(file)) return (new List<XElement>(), new List<Handler>());
			var written = File.GetLastWriteTimeUtc(file);
			lock (gate)
			{
				if (cache.Modules != null && cache.Written == written) return (cache.Modules, cache.Handlers);
				var modules = new List<XElement>();
				var handlers = new List<Handler>();
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
						ApplyHandlers(handlers, section);
					}
				}
				catch (Exception e) when (e is System.Xml.XmlException || e is IOException)
				{
				}
				cache = (written, modules, handlers);
				return (modules, handlers);
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
				// Without a '/', IIS matches the path against the request's file name; with one, the whole path.
				var subject = handler.Path.Contains('/') ? virtualPath : fileName;
				if (!Wildcard(handler.Path.TrimStart('~')).IsMatch(subject)) continue;
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

		// A handler's path as a regular expression, made once for each path (it was made in each request).
		static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Regex> wildcards = new System.Collections.Concurrent.ConcurrentDictionary<string, Regex>(StringComparer.Ordinal);

		static Regex Wildcard(string pattern) => wildcards.GetOrAdd(pattern, pattern =>
			new Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
	}
}

#endif
