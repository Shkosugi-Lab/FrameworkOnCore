#if NETCOREAPP

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
	/// <summary>
	/// IIS's response headers (system.webServer/httpProtocol, IisConfiguration's sections for the URL): customHeaders on
	/// every response (the pages', the static files', the errors', the redirects', 304), redirectHeaders on the 3xx ones
	/// (304 too), as IIS sends them: added to what the application sends (a header both send is sent twice), the
	/// collections inherited (clear, remove: an inherited entry, not the application's header). Without the original
	/// server's settings (IisProfile), IIS's defaults: X-Powered-By: ASP.NET.
	/// </summary>
	internal sealed class IisHttpProtocol
	{
		sealed class Headers
		{
			public (string Name, string Value)[] Custom;
			public (string Name, string Value)[] Redirect;
		}

		readonly string contentRoot;
		readonly bool defaults;
		readonly ConcurrentDictionary<string, Headers> byFolder = new ConcurrentDictionary<string, Headers>(StringComparer.OrdinalIgnoreCase);

		internal IisHttpProtocol(string contentRoot)
		{
			this.contentRoot = contentRoot;
			defaults = IisProfile.Of(contentRoot) == null;
		}

		Headers For(string path) => byFolder.GetOrAdd(IisConfiguration.Deepest(contentRoot, path), _ => Make(path));

		Headers Make(string path)
		{
			// applicationHost.config's own: <customHeaders><clear /><add name="X-Powered-By" value="ASP.NET" /></customHeaders>.
			var custom = defaults ? new List<(string, string)> { ("X-Powered-By", "ASP.NET") } : new List<(string, string)>();
			var redirect = new List<(string, string)>();
			foreach (var section in IisConfiguration.For(contentRoot, path))
			{
				var protocol = IisConfiguration.Child(section.WebServer, "httpProtocol");
				if (protocol == null) continue;
				Apply(IisConfiguration.Child(protocol, "customHeaders"), custom);
				Apply(IisConfiguration.Child(protocol, "redirectHeaders"), redirect);
			}
			return new Headers { Custom = custom.ToArray(), Redirect = redirect.ToArray() };
		}

		static void Apply(XElement collection, List<(string Name, string Value)> headers)
		{
			if (collection == null) return;
			foreach (var item in collection.Elements())
			{
				var name = (string)item.Attribute("name");
				switch (item.Name.LocalName)
				{
					case "clear": headers.Clear(); break;
					case "remove": headers.RemoveAll(h => string.Equals(h.Name, name, StringComparison.OrdinalIgnoreCase)); break;
					case "add":
						if (string.IsNullOrEmpty(name)) break;
						headers.RemoveAll(h => string.Equals(h.Name, name, StringComparison.OrdinalIgnoreCase));
						headers.Add((name, (string)item.Attribute("value") ?? ""));
						break;
				}
			}
		}

		// When the response starts: what the application sent is there (the headers of the URL it served: a rewritten one's).
		Task AddHeaders(Core.HttpContext context)
		{
			var request = context.Request;
			var headers = For((request.PathBase.Value ?? "") + (request.Path.Value ?? "/"));
			var response = context.Response;
			var redirect = response.StatusCode >= 300 && response.StatusCode < 400;
			if (headers.Custom.Length == 0 && (!redirect || headers.Redirect.Length == 0)) return Task.CompletedTask;
			// Not kept with a static file's response (StaticResponseCache): added again when it is answered.
			foreach (var (name, value) in headers.Custom) { Core.HeaderDictionaryExtensions.Append(response.Headers, name, value); IisAddedHeaders.Add(context, name); }
			if (redirect)
				foreach (var (name, value) in headers.Redirect) { Core.HeaderDictionaryExtensions.Append(response.Headers, name, value); IisAddedHeaders.Add(context, name); }
			return Task.CompletedTask;
		}

		internal sealed class Middleware
		{
			readonly Core.RequestDelegate next;
			readonly IisHttpProtocol protocol;

			public Middleware(Core.RequestDelegate next, IisHttpProtocol protocol)
			{
				this.next = next;
				this.protocol = protocol;
			}

			public Task Invoke(Core.HttpContext context)
			{
				context.Response.OnStarting(static state =>
				{
					var (protocol, context) = ((IisHttpProtocol, Core.HttpContext))state;
					return protocol.AddHeaders(context);
				}, (protocol, context));
				return next(context);
			}
		}
	}

	/// <summary>
	/// The headers IIS's features added to the response (IisHttpProtocol's customHeaders and redirectHeaders, IisCompression's
	/// Content-Encoding and Vary): not the application's (StaticResponseCache does not keep them).
	/// </summary>
	internal sealed class IisAddedHeaders
	{
		internal readonly List<string> Names = new List<string>();

		internal static void Add(Core.HttpContext context, string name)
		{
			var added = context.Features.Get<IisAddedHeaders>();
			if (added == null) context.Features.Set(added = new IisAddedHeaders());
			added.Names.Add(name);
		}

		internal bool Has(string name) => Names.Contains(name, StringComparer.OrdinalIgnoreCase);
	}

	/// <summary>
	/// IIS's HTTP redirection (system.webServer/httpRedirect, IisConfiguration's sections for the URL; after URL Rewrite: the
	/// rewritten URL's), as IIS answers (its documentation, the IIS 6.0 SDK's HttpRedirect for the variables, the module's
	/// author's blog for the folder; checked on IIS 10, samples/HeadersProbe): the attributes inherited one by one (a
	/// folder's enabled="false" over its parent's destination). The destination with the rest of the URL after the deepest
	/// folder that has an httpRedirect of its own (whichever of its attributes it sets; the destination's trailing '/' left
	/// out; the query string not kept), or exactly (exactDestination: $S the rest, $Q the query string with its '?', $P
	/// without it, $V the path); childOnly: the folder's own URLs only (not its sub-folders'); wildcards (add
	/// wildcard="*.php" destination="..."), the first that matches: the rest of the URL from its '/' matched, a '*' any
	/// characters ('/' too, as few as can be), $0-$9 what each matched; Found 302 (the default), Permanent 301,
	/// Temporary 307, PermRedirect 308, with IIS's reasons ("302 Redirect", "307 Moved Temporarily", "308 Redirected
	/// Permanently"); an absolute Location ('/'), a relative one as it is, IIS's "Document Moved" body.
	/// </summary>
	internal sealed class IisHttpRedirect
	{
		sealed class Settings
		{
			public bool Enabled;
			public string Destination = "";
			public string Base = "";   // the deepest folder with an httpRedirect of its own: the rest of the URL is after it
			public bool Exact;
			public bool ChildOnly;
			public int Status = 302;
			public List<(string Wildcard, string Destination, Regex Pattern)> Wildcards = new List<(string, string, Regex)>();
		}

		readonly string contentRoot;
		readonly ConcurrentDictionary<string, Settings> byFolder = new ConcurrentDictionary<string, Settings>(StringComparer.OrdinalIgnoreCase);

		IisHttpRedirect(string contentRoot) { this.contentRoot = contentRoot; }

		/// <summary>The application's HTTP redirection; null when no section turns it on.</summary>
		internal static IisHttpRedirect Load(string contentRoot)
		{
			var on = IisConfiguration.Sections(contentRoot).Any(s =>
				string.Equals((string)IisConfiguration.Child(s.WebServer, "httpRedirect")?.Attribute("enabled"), "true", StringComparison.OrdinalIgnoreCase));
			return on ? new IisHttpRedirect(contentRoot) : null;
		}

		Settings For(string path) => byFolder.GetOrAdd(IisConfiguration.Deepest(contentRoot, path), _ => Make(path));

		Settings Make(string path)
		{
			var settings = new Settings();
			foreach (var section in IisConfiguration.For(contentRoot, path))
			{
				var redirect = IisConfiguration.Child(section.WebServer, "httpRedirect");
				if (redirect == null) continue;
				settings.Base = section.Base;
				bool flag;
				if (bool.TryParse((string)redirect.Attribute("enabled"), out flag)) settings.Enabled = flag;
				var destination = (string)redirect.Attribute("destination");
				if (destination != null) settings.Destination = destination;
				if (bool.TryParse((string)redirect.Attribute("exactDestination"), out flag)) settings.Exact = flag;
				if (bool.TryParse((string)redirect.Attribute("childOnly"), out flag)) settings.ChildOnly = flag;
				switch (((string)redirect.Attribute("httpResponseStatus"))?.ToLowerInvariant())
				{
					case "permanent": settings.Status = 301; break;
					case "found": settings.Status = 302; break;
					case "temporary": settings.Status = 307; break;
					case "permredirect": settings.Status = 308; break;
				}
				foreach (var item in redirect.Elements())
				{
					var wildcard = (string)item.Attribute("wildcard");
					switch (item.Name.LocalName)
					{
						case "clear": settings.Wildcards.Clear(); break;
						case "remove": settings.Wildcards.RemoveAll(w => string.Equals(w.Wildcard, wildcard, StringComparison.OrdinalIgnoreCase)); break;
						case "add":
							if (string.IsNullOrEmpty(wildcard)) break;
							var pattern = new Regex("^" + Regex.Escape(wildcard).Replace(@"\*", "(.*?)") + "$",
								RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);
							settings.Wildcards.Add((wildcard, (string)item.Attribute("destination") ?? "", pattern));
							break;
					}
				}
			}
			return settings;
		}

		static string Rest(string path, string folder) =>
			folder.Length > 0 && IisConfiguration.Covers(folder, path) ? path.Substring(folder.Length) : path;

		/// <summary>The redirect's Location for the request; null when it is not redirected.</summary>
		string Location(Core.HttpContext context)
		{
			var request = context.Request;
			var path = (request.PathBase.Value ?? "") + (request.Path.Value ?? "");
			if (path.Length == 0) path = "/";
			var settings = For(path);
			if (!settings.Enabled) return null;
			var query = request.QueryString.HasValue ? request.QueryString.Value : "";
			var remainder = Rest(path, settings.Base);
			foreach (var (_, destination, pattern) in settings.Wildcards)
			{
				var match = pattern.Match(remainder);
				if (match.Success) return Target(destination, match, remainder, path, query, settings.Exact);
			}
			if (string.IsNullOrEmpty(settings.Destination)) return null;
			// childOnly: the folder's own files (and the folder), not what is in its sub-folders.
			if (settings.ChildOnly && remainder.LastIndexOf('/') > 0) return null;
			return Target(settings.Destination, null, remainder, path, query, settings.Exact);
		}

		static string Target(string destination, Match wildcard, string rest, string path, string query, bool exact)
		{
			var target = destination;
			// $0-$9: what the wildcard's stars matched.
			if (wildcard != null)
				target = Regex.Replace(target, @"\$([0-9])", m =>
				{
					var group = m.Groups[1].Value[0] - '0' + 1;
					return group < wildcard.Groups.Count ? wildcard.Groups[group].Value : "";
				});
			target = target.Replace("$S", rest).Replace("$Q", query).Replace("$P", query.Length > 0 ? query.Substring(1) : "").Replace("$V", path);
			if (!exact) target = target.TrimEnd('/') + rest;
			return target.Length == 0 ? "/" : target;
		}

		internal sealed class Middleware
		{
			readonly Core.RequestDelegate next;
			readonly IisHttpRedirect redirect;

			public Middleware(Core.RequestDelegate next, IisHttpRedirect redirect)
			{
				this.next = next;
				this.redirect = redirect;
			}

			public async Task Invoke(Core.HttpContext context)
			{
				var location = redirect.Location(context);
				if (location == null)
				{
					await next(context);
					return;
				}
				var status = redirect.For((context.Request.PathBase.Value ?? "") + (context.Request.Path.Value ?? "/")).Status;
				var body = IisUrlRewrite.Redirect(context, status, location.Replace(" ", "%20"));
				if (!Core.HttpMethods.IsHead(context.Request.Method)) await context.Response.Body.WriteAsync(body, 0, body.Length);
			}
		}
	}
}

#endif
