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
		// ResourceType: IIS's resourceType (File, Directory, Either; Unspecified, the default: whatever is there). Kind: Managed
		// (a type of the managed pipeline's), or IIS's own: AspNet (ASP.NET's -Integrated-4.0 entries, the types the host
		// gives System.Web), Extensionless (ExtensionlessUrlHandler-Integrated-4.0, "*."), Options, Trace, Static
		// (StaticFile), Native (native modules: not run here), NativeTyped (native modules named with a type: IIS 10 answers
		// 200 with no body). RequireAccess: its requireAccess (Script, IIS's default), held to handlers' accessPolicy.
		internal sealed record Handler(string Name, string Path, string Verb, string Type, string ResourceType = "Unspecified",
			string Kind = "Managed", string RequireAccess = "Script", string PreCondition = null, string Modules = null);

		// applicationHost.config's handlers (IIS 10, ASP.NET 4.8, integrated mode), in its order: ASP.NET's (the types System.Web
		// takes here), the extensionless URLs', the verbs', the static files' last. A web.config's are before them.
		static readonly Handler[] IisHandlers =
		{
			new Handler("ASP.NET's -Integrated-4.0 handlers", "*", "GET,HEAD,POST,DEBUG", null, "Unspecified", "AspNet", "Script"),
			new Handler("ExtensionlessUrlHandler-Integrated-4.0", "*.", "GET,HEAD,POST,DEBUG", null, "Unspecified", "Extensionless", "Script"),
			new Handler("TRACEVerbHandler", "*", "TRACE", null, "Unspecified", "Trace", "None"),
			new Handler("OPTIONSVerbHandler", "*", "OPTIONS", null, "Unspecified", "Options", "None"),
			new Handler("StaticFile", "*", "*", null, "Either", "Static", "Read"),
		};

		// A preCondition the host's pipeline meets: integrated mode, 64 bits, .NET 4 (an application pool's of ASP.NET 4.8 on
		// IIS 10; IIS 10 leaves out a handler of classicMode, bitness32 or runtimeVersionv2.0: its file served as static).
		static bool Meets(string preCondition) =>
			string.IsNullOrEmpty(preCondition) || !preCondition.Split(',').Select(c => c.Trim()).Any(c =>
				c.Equals("classicMode", StringComparison.OrdinalIgnoreCase) || c.Equals("bitness32", StringComparison.OrdinalIgnoreCase) ||
				c.Equals("runtimeVersionv2.0", StringComparison.OrdinalIgnoreCase) || c.Equals("runtimeVersionv1.1", StringComparison.OrdinalIgnoreCase));

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
			if (file == null || !File.Exists(file)) return (new List<XElement>(), new List<Handler>(IisHandlers), false);
			var written = File.GetLastWriteTimeUtc(file);
			lock (gate)
			{
				if (cache.Modules != null && cache.Written == written) return (cache.Modules, cache.Handlers, cache.RunAllManagedModules);
				var modules = new List<XElement>();
				var handlers = new List<Handler>(IisHandlers);
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

		// A system.webServer section's handlers over the ones before it: its adds before them, in its order (IIS's handlers
		// collection: a web.config's entries come first; IIS 10, a folder's "*" before the root's "*.probe" answers it), its
		// clear and remove of the ones before it (applicationHost.config's own: IisHandlers).
		static void ApplyHandlers(List<Handler> handlers, XElement section)
		{
			var added = new List<Handler>();
			foreach (var entry in section.Elements().Where(e => e.Name.LocalName == "handlers").Elements())
			{
				var name = (string)entry.Attribute("name") ?? "";
				switch (entry.Name.LocalName)
				{
					case "clear": handlers.Clear(); added.Clear(); break;
					case "remove":
						handlers.RemoveAll(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
						added.RemoveAll(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
						break;
					case "add":
						var type = (string)entry.Attribute("type");
						var path = (string)entry.Attribute("path");
						if (string.IsNullOrEmpty(path)) break;
						var modules = ((string)entry.Attribute("modules") ?? "ManagedPipelineHandler").Trim();
						var managed = modules.Equals("ManagedPipelineHandler", StringComparison.OrdinalIgnoreCase);
						// IIS's TransferRequestHandler (extensionless URLs to the managed pipeline, an application's own entry:
						// verb "*" for its Web API's verbs): the extensionless URLs' handler.
						var kind = !string.IsNullOrEmpty(type) && type.StartsWith("System.Web.Handlers.TransferRequestHandler", StringComparison.Ordinal) ? "Extensionless"
							: !string.IsNullOrEmpty(type) ? (managed ? "Managed" : "NativeTyped")
							: modules.IndexOf("StaticFileModule", StringComparison.OrdinalIgnoreCase) >= 0 ? "Static"
							: modules.IndexOf("ProtocolSupportModule", StringComparison.OrdinalIgnoreCase) >= 0 ? ((string)entry.Attribute("verb") ?? "").Trim().Equals("TRACE", StringComparison.OrdinalIgnoreCase) ? "Trace" : "Options"
							: "Native";
						handlers.RemoveAll(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
						added.RemoveAll(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
						added.Add(new Handler(name, path, (string)entry.Attribute("verb") ?? "*", kind is "Managed" ? type : null,
							(string)entry.Attribute("resourceType") ?? "Unspecified", kind, (string)entry.Attribute("requireAccess") ?? "Script",
							(string)entry.Attribute("preCondition"), modules));
						break;
				}
			}
			handlers.InsertRange(0, added);
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
		internal static string FindHandlerType(string verb, VirtualPath path) => FindHandler(verb, path.VirtualPathString)?.Type;

		/// <summary>
		/// Whether IIS's own OPTIONSVerbHandler answers OPTIONS for the URL: no managed handler of the application's takes the
		/// verb (theirs come before IIS's: a Web API's ExtensionlessUrlHandler with verb "*"), and the application's web.config
		/// does not remove it (a Web API's &lt;remove name="OPTIONSVerbHandler" /&gt;, a &lt;clear /&gt;).
		/// </summary>
		internal static bool AnswersOptions(string virtualPath) => AnswersVerb("OPTIONS", virtualPath);

		/// <summary>
		/// Whether IIS's own TRACEVerbHandler answers TRACE for the URL (its protocol support module's 501.0: IIS 10 answers
		/// TRACE of a page, a handler, a static file and of nothing so, after its request filtering and URL Rewrite): no managed
		/// handler of the application's takes the verb, and its web.config does not remove the handler.
		/// </summary>
		internal static bool AnswersTrace(string virtualPath) => AnswersVerb("TRACE", virtualPath);

		// The verb's entry is the one IIS maps the URL to (a managed handler's first; the folder's web.config's remove of it:
		// the next one's, the static files' 405.0 for OPTIONS, IIS 10).
		static bool AnswersVerb(string verb, string virtualPath) =>
			Resolve(verb, virtualPath, null)?.Kind == (verb == "OPTIONS" ? "Options" : "Trace");

		/// <summary>
		/// Whether a managed handler of system.webServer/handlers answers the URL where no file or folder is (nopCommerce's
		/// pricelist.csv: its PricelistHandler): IIS gives the request to it - the managed pipeline - when its resourceType is
		/// Unspecified (the default); File, Directory or Either: IIS's 404 (learn.microsoft.com/iis/configuration/
		/// system.webserver/handlers/add: resourceType).
		/// </summary>
		internal static bool HandlesWithoutResource(string verb, string virtualPath) =>
			FindHandler(verb, virtualPath) is { } handler && string.Equals(handler.ResourceType.Trim(), "Unspecified", StringComparison.OrdinalIgnoreCase);

		// The managed handler of system.webServer/handlers that answers the request (System.Web's mapping otherwise): the first
		// entry for it in IIS's order, if it is a managed one.
		static Handler FindHandler(string verb, string virtualPath)
		{
			var handler = Resolve(verb, virtualPath, null);
			return handler?.Kind == "Managed" ? handler : null;
		}

		/// <summary>
		/// The entry of the handlers in effect for the URL (the application's, its folders', IIS's own: IisHandlers) that IIS maps
		/// the request to: the first whose path and verb take it and whose preCondition the host meets; null for none (IIS's
		/// 404.4). aspNet: whether System.Web takes the URL's type (the -Integrated-4.0 entries'); null: none of them answers.
		/// </summary>
		internal static Handler Resolve(string verb, string virtualPath, Func<string, bool> aspNet)
		{
			var handlers = HandlersFor(virtualPath);
			var fileName = virtualPath.Substring(virtualPath.LastIndexOf('/') + 1);
			foreach (var handler in handlers)
			{
				if (!Meets(handler.PreCondition)) continue;
				if (handler.Kind == "AspNet")
				{
					if (aspNet == null || !aspNet(virtualPath)) continue;
				}
				// "*.": a name without a type (a folder's URL too).
				else if (handler.Path == "*.")
				{
					if (fileName.IndexOf('.') >= 0) continue;
				}
				else if (!PathMatches(handler, virtualPath, fileName)) continue;
				if (handler.Verb != "*" && !handler.Verb.Split(',').Any(v => v.Trim().Equals(verb, StringComparison.OrdinalIgnoreCase))) continue;
				return handler;
			}
			return null;
		}

		/// <summary>Whether a handler of the application's, of a type's path ("*.txt", not "*"), takes the name.</summary>
		internal static bool HasManagedHandler(string virtualPath)
		{
			var fileName = virtualPath.Substring(virtualPath.LastIndexOf('/') + 1);
			return HandlersFor(virtualPath).Any(h => h.Kind is "Managed" or "NativeTyped" && h.Path.IndexOf('.') >= 0 && Meets(h.PreCondition) && PathMatches(h, virtualPath, fileName));
		}

		/// <summary>Whether the folder's handlers have IIS's entry of the name (StaticFile: not removed or cleared).</summary>
		internal static bool HasIisHandler(string name, string virtualPath) =>
			HandlersFor(virtualPath).Any(h => h.Kind != "Managed" && h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

		/// <summary>
		/// handlers' accessPolicy for the URL (the nearest one's: the original server's, web.config's; applicationHost.config's
		/// "Read, Script" without them) against a handler's requireAccess: 0 when it has the access, else IIS's 403 substatus
		/// (IIS 10: Read missing 403.2, Script or Execute 403.1, Write 403.3).
		/// </summary>
		internal static int AccessDenied(string requireAccess, string virtualPath)
		{
			var required = (requireAccess ?? "Script").Trim();
			if (required.Equals("None", StringComparison.OrdinalIgnoreCase)) return 0;
			var policy = "Read, Script";
			var root = IisConfiguration.Root;
			if (root != null)
				foreach (var section in IisConfiguration.For(root, virtualPath))
					if ((string)IisConfiguration.Child(section.WebServer, "handlers")?.Attribute("accessPolicy") is { } value) policy = value;
			var flags = new HashSet<string>(policy.Split(',').Select(f => f.Trim()), StringComparer.OrdinalIgnoreCase);
			switch (required.ToLowerInvariant())
			{
				case "read": return flags.Contains("Read") ? 0 : 2;
				case "write": return flags.Contains("Write") ? 0 : 3;
				case "execute": return flags.Contains("Execute") ? 0 : 1;
				default: return flags.Contains("Script") || flags.Contains("Execute") ? 0 : 1;
			}
		}

		static bool PathMatches(Handler handler, string virtualPath, string fileName)
		{
			// Without a '/', IIS matches the path against the request's file name. With one (seen on IIS 10, a folder's
			// web.config): from a '/' the whole URL path ("/T/H/Root.ashx"; "/H/Abs.ashx" does not answer /T/H/Abs.ashx);
			// else the end of the URL's path, whole segments ("Z/R.ashx" answers .../Z/R.ashx and .../X/Z/R.ashx, not
			// .../XZ/R.ashx; "Y/Redirect.ashx" of the application's answers /Y/Redirect.ashx); '*' within a segment
			// ("H/*.foo" answers /T/H/a.foo, not /T/H/Sub/a.foo). The host's choice: "~/" is the whole path too (IIS takes
			// '~' as a character: it never matches).
			return handler.Path.Contains('/')
				? Wildcard(handler.Path, segments: true).IsMatch(virtualPath)
				: Wildcard(handler.Path.TrimStart('~'), segments: false).IsMatch(fileName);
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
			// The rest of requestFiltering (IIS 10's answers to each recorded: Security/iis-golden/iis-settings.json).
			public bool AllowHighBitCharacters = true;         // false: a URL with a character beyond ASCII, 404.12
			public bool UnescapeQueryString = true;            // the query's sequences and rules checked on it unescaped too
			public bool RemoveServerHeader;
			public Dictionary<string, bool> Verbs = new Dictionary<string, bool>(StringComparer.Ordinal);   // 404.6
			public bool VerbsAllowUnlisted = true;
			public Dictionary<string, long> HeaderLimits = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);   // 431
			public HashSet<string> AlwaysAllowedUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			public HashSet<string> AlwaysAllowedQueryStrings = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			public List<string> DenyQueryStringSequences = new List<string>();   // 404.18
			public List<FilteringRule> Rules = new List<FilteringRule>();        // 404.19
		}

		internal sealed class FilteringRule
		{
			public string Name;
			public bool ScanUrl, ScanQueryString, ScanAllRaw;
			public List<string> ScanHeaders = new List<string>();
			public List<string> AppliesTo = new List<string>();
			public List<string> DenyStrings = new List<string>();
		}

		// A collection's add, remove and clear by its key attribute (as IIS merges a web.config's collection over its parent's).
		static void Collection(XElement parent, string name, string key, Action clear, Action<string, XElement> add, Action<string> remove)
		{
			foreach (var collection in IisConfiguration.Children(parent, name))
				foreach (var item in collection.Elements())
				{
					var value = ((string)item.Attribute(key))?.Trim();
					switch (item.Name.LocalName)
					{
						case "clear": clear(); break;
						case "remove" when !string.IsNullOrEmpty(value): remove(value); break;
						default:
							if (item.Name.LocalName != "clear" && item.Name.LocalName != "remove" && !string.IsNullOrEmpty(value)) add(value, item);
							break;
					}
				}
		}

		static bool Flag(XElement element, string attribute, bool current)
		{
			var value = (string)element.Attribute(attribute);
			return value == null ? current : string.Equals(value.Trim(), "true", StringComparison.OrdinalIgnoreCase);
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
								// (allowed left out: allowed, the schema's default - IIS 10 serves the type; it was denied)
								filtering.Extensions[extension] = !string.Equals((string)item.Attribute("allowed"), "false", StringComparison.OrdinalIgnoreCase);
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
			foreach (var element in webServers.SelectMany(w => IisConfiguration.Children(w, "security")).SelectMany(s => IisConfiguration.Children(s, "requestFiltering")))
			{
				filtering.AllowHighBitCharacters = Flag(element, "allowHighBitCharacters", filtering.AllowHighBitCharacters);
				filtering.UnescapeQueryString = Flag(element, "unescapeQueryString", filtering.UnescapeQueryString);
				filtering.RemoveServerHeader = Flag(element, "removeServerHeader", filtering.RemoveServerHeader);
				foreach (var verbs in IisConfiguration.Children(element, "verbs")) filtering.VerbsAllowUnlisted = Flag(verbs, "allowUnlisted", filtering.VerbsAllowUnlisted);
				Collection(element, "verbs", "verb", filtering.Verbs.Clear, (verb, item) => filtering.Verbs[verb] = Flag(item, "allowed", false), verb => filtering.Verbs.Remove(verb));
				foreach (var limits in IisConfiguration.Children(element, "requestLimits"))
					Collection(limits, "headerLimits", "header", filtering.HeaderLimits.Clear,
						(header, item) => filtering.HeaderLimits[header] = long.TryParse((string)item.Attribute("sizeLimit"), out var size) ? size : long.MaxValue,
						header => filtering.HeaderLimits.Remove(header));
				Collection(element, "alwaysAllowedUrls", "url", filtering.AlwaysAllowedUrls.Clear, (url, _) => filtering.AlwaysAllowedUrls.Add(url), url => filtering.AlwaysAllowedUrls.Remove(url));
				Collection(element, "alwaysAllowedQueryStrings", "queryString", filtering.AlwaysAllowedQueryStrings.Clear,
					(query, _) => filtering.AlwaysAllowedQueryStrings.Add(query), query => filtering.AlwaysAllowedQueryStrings.Remove(query));
				Collection(element, "denyQueryStringSequences", "sequence", filtering.DenyQueryStringSequences.Clear,
					(sequence, _) => { if (!filtering.DenyQueryStringSequences.Contains(sequence, StringComparer.OrdinalIgnoreCase)) filtering.DenyQueryStringSequences.Add(sequence); },
					sequence => filtering.DenyQueryStringSequences.RemoveAll(s => string.Equals(s, sequence, StringComparison.OrdinalIgnoreCase)));
				Collection(element, "filteringRules", "name", filtering.Rules.Clear, (name, item) =>
				{
					filtering.Rules.RemoveAll(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
					var rule = new FilteringRule
					{
						Name = name, ScanUrl = Flag(item, "scanUrl", false), ScanQueryString = Flag(item, "scanQueryString", false), ScanAllRaw = Flag(item, "scanAllRaw", false),
					};
					Collection(item, "scanHeaders", "requestHeader", rule.ScanHeaders.Clear, (header, _) => rule.ScanHeaders.Add(header), header => rule.ScanHeaders.Remove(header));
					Collection(item, "appliesTo", "fileExtension", rule.AppliesTo.Clear, (extension, _) => rule.AppliesTo.Add(extension), extension => rule.AppliesTo.Remove(extension));
					Collection(item, "denyStrings", "string", rule.DenyStrings.Clear, (text, _) => rule.DenyStrings.Add(text), text => rule.DenyStrings.Remove(text));
					filtering.Rules.Add(rule);
				}, name => filtering.Rules.RemoveAll(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase)));
			}
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
