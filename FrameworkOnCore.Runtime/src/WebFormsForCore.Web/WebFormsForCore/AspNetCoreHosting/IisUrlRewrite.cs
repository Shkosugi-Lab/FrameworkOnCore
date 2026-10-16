#if NETCOREAPP

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
	/// <summary>
	/// IIS's URL Rewrite module (2.1) as IIS runs it (UrlRewrite:Mode Iis, the default): system.webServer/rewrite of the
	/// original server (IisProfile: its globalRules, its rules, the site's) and of the application's web.config files
	/// (each folder's, &lt;location&gt;s, rules and rewriteMaps from configSource files), inherited as IIS inherits them
	/// (the parent's rules first; clear, remove). A distributed rule matches the URL's path relative to the folder of its
	/// web.config (decoded, without its leading '/'); its relative URLs are that folder's. The rules after a rewrite see the
	/// rewritten URL. ECMAScript, Wildcard and ExactMatch patterns; conditions (MatchAll, MatchAny, trackAllCaptures,
	/// IsFile, IsDirectory); {R:n}, {C:n}, server variables (IIS's names, HTTP_* the request's headers), ToLower,
	/// UrlEncode, UrlDecode, rewrite maps; serverVariables (HTTP_* set the request's headers, RESPONSE_* the response's,
	/// the others Request.ServerVariables'; allowedServerVariables from the profile: 500.50 otherwise). Actions: Rewrite
	/// (the query string after the rule's, X-Original-URL, logRewrittenUrl), Redirect (an absolute Location, IIS's
	/// "Document Moved" body), CustomResponse (IIS's error page, its substatus logged), AbortRequest (not logged), None.
	/// What it does not do: outboundRules (the responses' rewriting), providers, a rewrite to another server (ARR's
	/// reverse proxy: 502). Told on the console once, and in the conversion report.
	/// </summary>
	internal sealed class IisUrlRewrite
	{
		enum Syntax { ECMAScript, Wildcard, ExactMatch }
		enum MatchType { Pattern, IsFile, IsDirectory }
		enum ActionType { None, Rewrite, Redirect, CustomResponse, AbortRequest }

		static readonly string[] NoCaptures = new string[0];
		static readonly byte[] NoBody = new byte[0];

		/// <summary>A pattern as IIS matches it: its captures (0: the whole match), null when it does not match.</summary>
		sealed class Pattern
		{
			readonly Syntax syntax;
			readonly string text;
			readonly bool ignoreCase;
			readonly Regex regex;

			public Pattern(Syntax syntax, string text, bool ignoreCase)
			{
				this.syntax = syntax;
				this.text = text ?? "";
				this.ignoreCase = ignoreCase;
				var timeout = TimeSpan.FromSeconds(2);
				if (syntax == Syntax.Wildcard)
					regex = new Regex("^" + Regex.Escape(this.text).Replace(@"\*", "(.*)") + "$",
						RegexOptions.Singleline | RegexOptions.CultureInvariant | (ignoreCase ? RegexOptions.IgnoreCase : 0), timeout);
				else if (syntax == Syntax.ECMAScript)
				{
					// IIS's regular expressions are ECMAScript's; one .NET's ECMAScript option does not take (named groups,
					// lookbehinds) as .NET's.
					try { regex = new Regex(this.text, RegexOptions.ECMAScript | (ignoreCase ? RegexOptions.IgnoreCase : 0), timeout); }
					catch (ArgumentException) { regex = new Regex(this.text, RegexOptions.CultureInvariant | (ignoreCase ? RegexOptions.IgnoreCase : 0), timeout); }
				}
			}

			public string[] Match(string input)
			{
				if (syntax == Syntax.ExactMatch)
					return string.Equals(input, text, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ? new[] { input } : null;
				Match match;
				try { match = regex.Match(input); }
				catch (RegexMatchTimeoutException) { return null; }
				if (!match.Success) return null;
				var captures = new string[match.Groups.Count];
				for (int i = 0; i < captures.Length; i++) captures[i] = match.Groups[i].Success ? match.Groups[i].Value : "";
				return captures;
			}
		}

		sealed class Condition
		{
			public string Input;
			public MatchType Type;
			public Pattern Pattern;
			public bool Negate;
		}

		sealed class Rule
		{
			public string Name;
			public string Base;   // the folder of its configuration ("" the site's root, "/admin")
			public Pattern Match;
			public bool Negate;
			public List<Condition> Conditions = new List<Condition>();
			public bool MatchAny;
			public bool TrackAllCaptures;
			public List<(string Name, string Value, bool Replace)> ServerVariables = new List<(string, string, bool)>();
			public ActionType Action;
			public string Url;
			public bool AppendQueryString = true;
			public bool LogRewrittenUrl;
			public int RedirectStatus = 301;
			public int StatusCode;
			public int SubStatusCode;
			public string StatusReason;
			public bool StopProcessing;
		}

		sealed class Map
		{
			public Dictionary<string, string> Values;
			public string DefaultValue = "";
		}

		readonly string contentRoot;
		readonly string siteId;
		readonly List<Rule> globalRules = new List<Rule>();
		// Each folder that has rules of its own (the longest first) and the rules in effect there.
		readonly List<(string Base, List<Rule> Rules)> folders = new List<(string, List<Rule>)>();
		readonly Dictionary<string, Map> maps = new Dictionary<string, Map>(StringComparer.OrdinalIgnoreCase);
		HashSet<string> allowedServerVariables;   // null: any (no profile to say)
		readonly List<string> unsupported = new List<string>();

		IisUrlRewrite(string contentRoot)
		{
			this.contentRoot = contentRoot;
			siteId = IisProfile.SiteId(IisProfile.Of(contentRoot)) ?? "1";
		}

		/// <summary>The rules of the application in the folder (its profile's, its web.config files'); null without any.</summary>
		internal static IisUrlRewrite Load(string contentRoot)
		{
			var rewrite = new IisUrlRewrite(contentRoot);
			rewrite.Read();
			if (rewrite.globalRules.Count == 0 && rewrite.folders.All(f => f.Rules.Count == 0)) return null;
			foreach (var what in rewrite.unsupported.Distinct())
				Console.Error.WriteLine("URL Rewrite: " + what + " (not done)");
			return rewrite;
		}

		static IEnumerable<XElement> Children(XElement element, string name) =>
			element == null ? Enumerable.Empty<XElement>() : element.Elements().Where(e => e.Name.LocalName == name);

		static string Attribute(XElement element, string name) => (string)element?.Attribute(name);

		static bool Flag(XElement element, string name, bool otherwise)
		{
			bool value;
			return bool.TryParse(Attribute(element, name), out value) ? value : otherwise;
		}

		static bool Covers(string folder, string path) => IisConfiguration.Covers(folder, path);

		void Read()
		{
			// The rewrite sections in IIS's order (IisConfiguration: the server's, then the web.config files from the root down).
			var ordered = IisConfiguration.Sections(contentRoot)
				.SelectMany(s => Children(s.WebServer, "rewrite").Select(r => (s.Base, Rewrite: r, s.FileFolder, s.Server))).ToList();
			foreach (var section in ordered)
			{
				if (section.Server)
				{
					// applicationHost.config's only: the global rules, the variables rules may set.
					foreach (var global in Children(section.Rewrite, "globalRules")) ReadRules(global, "", globalRules, null);
					ReadAllowed(section.Rewrite);
				}
				foreach (var mapsElement in Children(section.Rewrite, "rewriteMaps")) ReadMaps(External(mapsElement, section.FileFolder));
				if (Children(section.Rewrite, "outboundRules").Any(o => Children(o, "rule").Any())) unsupported.Add("outboundRules (the responses' rewriting)");
				if (Children(section.Rewrite, "providers").Any(p => Children(p, "provider").Any())) unsupported.Add("providers");
			}
			foreach (var folder in ordered.Select(s => s.Base).Distinct(StringComparer.OrdinalIgnoreCase).OrderByDescending(b => b.Length))
			{
				var rules = new List<Rule>();
				foreach (var section in ordered.Where(s => Covers(s.Base, folder)))
					foreach (var rulesElement in Children(section.Rewrite, "rules"))
						ReadRules(External(rulesElement, section.FileFolder), section.Base, rules, section.FileFolder);
				folders.Add((folder, rules));
			}
		}

		// A section's configSource: the file next to the web.config, its root the section.
		XElement External(XElement element, string fileFolder)
		{
			var source = Attribute(element, "configSource");
			if (string.IsNullOrEmpty(source) || fileFolder == null) return element;
			try { return XDocument.Load(Path.Combine(fileFolder, source)).Root; }
			catch (Exception e) when (e is System.Xml.XmlException || e is IOException || e is UnauthorizedAccessException)
			{
				unsupported.Add("configSource " + source + ": " + e.Message);
				return element;
			}
		}

		void ReadAllowed(XElement section)
		{
			foreach (var allowed in Children(section, "allowedServerVariables"))
				foreach (var item in allowed.Elements())
				{
					if (allowedServerVariables == null) allowedServerVariables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					var name = Attribute(item, "name");
					switch (item.Name.LocalName)
					{
						case "clear": allowedServerVariables.Clear(); break;
						case "add": if (name != null) allowedServerVariables.Add(name); break;
						case "remove": if (name != null) allowedServerVariables.Remove(name); break;
					}
				}
		}

		void ReadMaps(XElement element)
		{
			foreach (var item in element.Elements())
			{
				var name = Attribute(item, "name");
				if (item.Name.LocalName == "clear") { maps.Clear(); continue; }
				if (item.Name.LocalName == "remove") { if (name != null) maps.Remove(name); continue; }
				if (item.Name.LocalName != "rewriteMap" || name == null) continue;
				var map = new Map
				{
					Values = new Dictionary<string, string>(Flag(item, "ignoreCase", true) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal),
					DefaultValue = Attribute(item, "defaultValue") ?? "",
				};
				foreach (var add in Children(item, "add"))
				{
					var key = Attribute(add, "key");
					if (key != null) map.Values[key] = Attribute(add, "value") ?? "";
				}
				maps[name] = map;
			}
		}

		void ReadRules(XElement element, string folder, List<Rule> rules, string fileFolder)
		{
			foreach (var item in element.Elements())
			{
				var name = Attribute(item, "name");
				switch (item.Name.LocalName)
				{
					case "clear": rules.Clear(); continue;
					case "remove": rules.RemoveAll(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase)); continue;
					case "rule": break;
					default: continue;
				}
				if (!Flag(item, "enabled", true)) continue;
				try { rules.Add(ReadRule(item, folder)); }
				catch (ArgumentException e) { unsupported.Add("the rule " + name + ": " + e.Message); }
			}
		}

		Rule ReadRule(XElement item, string folder)
		{
			Syntax syntax;
			if (!Enum.TryParse(Attribute(item, "patternSyntax"), true, out syntax)) syntax = Syntax.ECMAScript;
			var match = Children(item, "match").FirstOrDefault();
			var rule = new Rule
			{
				Name = Attribute(item, "name"),
				Base = folder,
				Match = new Pattern(syntax, Attribute(match, "url") ?? "", Flag(match, "ignoreCase", true)),
				Negate = Flag(match, "negate", false),
				StopProcessing = Flag(item, "stopProcessing", false),
			};
			var conditions = Children(item, "conditions").FirstOrDefault();
			if (conditions != null)
			{
				rule.MatchAny = string.Equals(Attribute(conditions, "logicalGrouping"), "MatchAny", StringComparison.OrdinalIgnoreCase);
				rule.TrackAllCaptures = Flag(conditions, "trackAllCaptures", false);
				foreach (var add in Children(conditions, "add"))
				{
					MatchType type;
					if (!Enum.TryParse(Attribute(add, "matchType"), true, out type)) type = MatchType.Pattern;
					rule.Conditions.Add(new Condition
					{
						Input = Attribute(add, "input") ?? "",
						Type = type,
						Pattern = type == MatchType.Pattern ? new Pattern(syntax, Attribute(add, "pattern") ?? "", Flag(add, "ignoreCase", true)) : null,
						Negate = Flag(add, "negate", false),
					});
				}
			}
			foreach (var set in Children(item, "serverVariables").SelectMany(v => Children(v, "set")))
				if (Attribute(set, "name") != null) rule.ServerVariables.Add((Attribute(set, "name"), Attribute(set, "value") ?? "", Flag(set, "replace", true)));
			var action = Children(item, "action").FirstOrDefault();
			ActionType type2;
			if (!Enum.TryParse(Attribute(action, "type"), true, out type2)) type2 = ActionType.None;
			rule.Action = type2;
			rule.Url = Attribute(action, "url") ?? "";
			rule.AppendQueryString = Flag(action, "appendQueryString", true);
			rule.LogRewrittenUrl = Flag(action, "logRewrittenUrl", false);
			switch ((Attribute(action, "redirectType") ?? "Permanent").ToLowerInvariant())
			{
				case "found": rule.RedirectStatus = 302; break;
				case "seeother": rule.RedirectStatus = 303; break;
				case "temporary": rule.RedirectStatus = 307; break;
				default: rule.RedirectStatus = 301; break;
			}
			int number;
			rule.StatusCode = int.TryParse(Attribute(action, "statusCode"), NumberStyles.Integer, CultureInfo.InvariantCulture, out number) ? number : 200;
			rule.SubStatusCode = int.TryParse(Attribute(action, "subStatusCode"), NumberStyles.Integer, CultureInfo.InvariantCulture, out number) ? number : 0;
			rule.StatusReason = Attribute(action, "statusReason");
			if (rule.Action == ActionType.Rewrite && IsAbsolute(rule.Url)) unsupported.Add("the rule " + rule.Name + ": a rewrite to another server (ARR's reverse proxy)");
			return rule;
		}

		static bool IsAbsolute(string url) =>
			url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

		/// <summary>The request's state while the rules run.</summary>
		sealed class State
		{
			public Core.HttpContext Context;
			public string Path;    // decoded, from '/'
			public string Query;   // as sent, without '?'
			public string RawTarget;
			public bool Rewritten;
			public bool LogRewritten;
			public Dictionary<string, string> Variables;
			public byte[] Body;   // the answer's body, when the rules answered
		}

		/// <summary>Runs the rules: true when they answered the request (a redirect, a response of their own, an abort).</summary>
		internal async Task<bool> RunAsync(Core.HttpContext context)
		{
			var body = Run(context);
			if (body == null) return false;
			if (body.Length > 0 && !Core.HttpMethods.IsHead(context.Request.Method)) await context.Response.Body.WriteAsync(body, 0, body.Length);
			return true;
		}

		// The answer's body when the rules answered (empty: none), null when the request goes on.
		byte[] Run(Core.HttpContext context)
		{
			var request = context.Request;
			var state = new State
			{
				Context = context,
				Path = (request.PathBase.Value ?? "") + (request.Path.Value ?? ""),
				Query = request.QueryString.HasValue ? request.QueryString.Value.Substring(1) : "",
				RawTarget = AspNetCoreWorkerRequest.RawTarget(context),
			};
			if (state.Path.Length == 0) state.Path = "/";
			if (globalRules.Count > 0)
			{
				if (Run(globalRules, state)) return state.Body ?? NoBody;
				// IIS runs the global rules before it takes the URL its log has: a global rewrite's URL logged.
				if (state.Rewritten) state.LogRewritten = true;
			}
			foreach (var folder in folders)
				if (Covers(folder.Base, state.Path))
				{
					if (Run(folder.Rules, state)) return state.Body ?? NoBody;
					break;
				}
			if (state.Rewritten)
			{
				// As IIS's: the URL asked for in X-Original-URL (HTTP_X_ORIGINAL_URL), the application served the rewritten one.
				request.Headers["X-Original-URL"] = state.RawTarget;
				var pathBase = request.PathBase.Value ?? "";
				request.Path = new Core.PathString(pathBase.Length > 0 && Covers(pathBase, state.Path) ? state.Path.Substring(pathBase.Length) : state.Path);
				request.QueryString = state.Query.Length == 0 ? Core.QueryString.Empty : new Core.QueryString("?" + state.Query);
				(state.Variables ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))["IIS_WasUrlRewritten"] = "1";
				if (state.LogRewritten)
					IisLogStatus.SetLoggedUrl(context, state.Path, state.Query.Length == 0 ? null : "?" + state.Query);
			}
			if (state.Variables != null) context.Features.Set(new IisServerVariables(state.Variables));
			return null;
		}

		bool Run(List<Rule> rules, State state)
		{
			foreach (var rule in rules)
			{
				var input = state.Path;
				if (rule.Base.Length > 0 && Covers(rule.Base, input)) input = input.Substring(rule.Base.Length);
				input = input.TrimStart('/');
				var r = rule.Match.Match(input);
				if (rule.Negate) r = r == null ? NoCaptures : null;
				if (r == null) continue;
				var c = Conditions(rule, r, state);
				if (c == null) continue;
				foreach (var (name, value, replace) in rule.ServerVariables)
				{
					if (allowedServerVariables != null && !allowedServerVariables.Contains(name))
					{
						// IIS: 500.50, a server variable the server does not let rules set.
						state.Body = Respond(state.Context, 500, 50, null);
						return true;
					}
					SetVariable(state, name, Expand(value, r, c, state), replace);
				}
				switch (rule.Action)
				{
					case ActionType.Rewrite:
					{
						var url = Expand(rule.Url, r, c, state);
						if (IsAbsolute(url))
						{
							state.Body = Respond(state.Context, 502, 3, null);   // ARR's: no reverse proxy here
							return true;
						}
						var (path, query) = Target(url, rule, state);
						state.Path = Uri.UnescapeDataString(path);
						state.Query = query;
						state.Rewritten = true;
						if (rule.LogRewrittenUrl) state.LogRewritten = true;
						break;
					}
					case ActionType.Redirect:
					{
						var url = Expand(rule.Url, r, c, state);
						string location;
						if (IsAbsolute(url))
						{
							var mark = url.IndexOf('?');
							var given = mark < 0 ? null : url.Substring(mark + 1);
							var query = Append(given, rule, state);
							location = (mark < 0 ? url : url.Substring(0, mark)) + (query.Length > 0 ? "?" + query : "");
						}
						else
						{
							var (path, query) = Target(url, rule, state);
							location = path + (query.Length > 0 ? "?" + query : "");
						}
						state.Body = Redirect(state.Context, rule.RedirectStatus, location.Replace(" ", "%20"));
						return true;
					}
					case ActionType.CustomResponse:
						state.Body = Respond(state.Context, rule.StatusCode, rule.SubStatusCode, rule.StatusReason);
						return true;
					case ActionType.AbortRequest:
						IisLogStatus.SetNotLogged(state.Context);   // IIS writes no line for it
						state.Context.Abort();
						return true;
				}
				if (rule.StopProcessing) break;
			}
			return false;
		}

		// The rule's URL in the site: its path (a relative one in the rule's folder) and its query string, the request's
		// after the rule's (appendQueryString).
		static (string Path, string Query) Target(string url, Rule rule, State state)
		{
			var mark = url.IndexOf('?');
			var path = mark < 0 ? url : url.Substring(0, mark);
			var given = mark < 0 ? null : url.Substring(mark + 1);
			if (!path.StartsWith("/", StringComparison.Ordinal)) path = rule.Base + "/" + path;
			return (path, Append(given, rule, state));
		}

		static string Append(string given, Rule rule, State state)
		{
			if (!rule.AppendQueryString || state.Query.Length == 0) return given ?? "";
			return string.IsNullOrEmpty(given) ? state.Query : given + "&" + state.Query;
		}

		string[] Conditions(Rule rule, string[] r, State state)
		{
			var last = NoCaptures;
			List<string> all = rule.TrackAllCaptures ? new List<string>() : null;
			foreach (var condition in rule.Conditions)
			{
				var value = Expand(condition.Input, r, all != null ? all.ToArray() : last, state);
				string[] captures = null;
				bool matched;
				switch (condition.Type)
				{
					case MatchType.IsFile: matched = FileCache.FileExists(Trimmed(value)); break;
					case MatchType.IsDirectory: matched = FileCache.DirectoryExists(Trimmed(value)); break;
					default: captures = condition.Pattern.Match(value); matched = captures != null; break;
				}
				if (condition.Negate) { matched = !matched; captures = null; }
				if (matched && captures != null)
				{
					if (all != null) all.AddRange(all.Count == 0 ? captures : captures.Skip(1));
					else last = captures;
				}
				if (rule.MatchAny && matched) return all != null ? all.ToArray() : last;
				if (!rule.MatchAny && !matched) return null;
			}
			if (rule.MatchAny && rule.Conditions.Count > 0) return null;
			return all != null ? all.ToArray() : last;
		}

		static string Trimmed(string path) =>
			path.Length > 1 && (path.EndsWith("\\", StringComparison.Ordinal) || path.EndsWith("/", StringComparison.Ordinal)) ? path.Substring(0, path.Length - 1) : path;

		// {R:n}, {C:n}, {VARIABLE}, {Function:...}, {Map:...}, nested.
		string Expand(string text, string[] r, string[] c, State state)
		{
			if (string.IsNullOrEmpty(text) || text.IndexOf('{') < 0) return text ?? "";
			var result = new StringBuilder(text.Length + 32);
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != '{') { result.Append(text[i]); continue; }
				int depth = 0, end = -1;
				for (int j = i; j < text.Length; j++)
				{
					if (text[j] == '{') depth++;
					else if (text[j] == '}' && --depth == 0) { end = j; break; }
				}
				if (end < 0) { result.Append(text, i, text.Length - i); break; }
				result.Append(Evaluate(text.Substring(i + 1, end - i - 1), r, c, state));
				i = end;
			}
			return result.ToString();
		}

		string Evaluate(string inner, string[] r, string[] c, State state)
		{
			var colon = inner.IndexOf(':');
			if (colon < 0) return Variable(inner.Trim(), state);
			var name = inner.Substring(0, colon);
			var argument = inner.Substring(colon + 1);
			int index;
			if ((name == "R" || name == "C") && int.TryParse(argument, NumberStyles.None, CultureInfo.InvariantCulture, out index))
			{
				var captures = name == "R" ? r : c;
				return index < captures.Length ? captures[index] : "";
			}
			var value = Expand(argument, r, c, state);
			switch (name.ToLowerInvariant())
			{
				case "tolower": return value.ToLowerInvariant();
				case "urlencode": return Uri.EscapeDataString(value);
				case "urldecode": return Uri.UnescapeDataString(value);
			}
			Map map;
			if (maps.TryGetValue(name, out map)) return map.Values.TryGetValue(value, out var mapped) ? mapped : map.DefaultValue;
			return "";   // a provider's (not here)
		}

		// A server variable as IIS gives the rules it: the URL being rewritten's, the request's headers (HTTP_*).
		string Variable(string name, State state)
		{
			string set;
			if (state.Variables != null && state.Variables.TryGetValue(name, out set)) return set;
			var context = state.Context;
			var request = context.Request;
			var connection = context.Connection;
			switch (name.ToUpperInvariant())
			{
				case "URL":
				case "PATH_INFO":
				case "SCRIPT_NAME":
					return state.Path;
				case "QUERY_STRING":
					return state.Query;
				case "REQUEST_URI":
				case "HTTP_URL":
					return state.Rewritten ? state.Path + (state.Query.Length > 0 ? "?" + state.Query : "") : state.RawTarget;
				case "UNENCODED_URL":
					return state.RawTarget;
				case "CACHE_URL":
					return request.Scheme + "://" + request.Host.Host + ":" + Port(request).ToString(CultureInfo.InvariantCulture) +
						(state.Rewritten ? state.Path + (state.Query.Length > 0 ? "?" + state.Query : "") : state.RawTarget);
				case "REQUEST_FILENAME":
				case "SCRIPT_FILENAME":
				case "PATH_TRANSLATED":
					return Physical(state.Path);
				case "APPL_PHYSICAL_PATH":
					return contentRoot.EndsWith(Path.DirectorySeparatorChar) ? contentRoot : contentRoot + Path.DirectorySeparatorChar;
				case "APPL_MD_PATH":
					return "/LM/W3SVC/" + siteId + "/ROOT";
				case "INSTANCE_ID":
					return siteId;
				case "HTTPS":
					return request.IsHttps ? "on" : "off";
				case "SERVER_PORT":
					return Port(request).ToString(CultureInfo.InvariantCulture);
				case "SERVER_PORT_SECURE":
					return request.IsHttps ? "1" : "0";
				case "SERVER_NAME":
					return request.Host.Host;
				case "SERVER_PROTOCOL":
					return request.Protocol;
				case "REQUEST_METHOD":
					return request.Method;
				case "REMOTE_ADDR":
				case "REMOTE_HOST":
					return Address(connection.RemoteIpAddress);
				case "REMOTE_PORT":
					return connection.RemotePort.ToString(CultureInfo.InvariantCulture);
				case "LOCAL_ADDR":
					return Address(connection.LocalIpAddress);
				case "CONTENT_TYPE":
					return request.ContentType ?? "";
				case "CONTENT_LENGTH":
					return request.ContentLength?.ToString(CultureInfo.InvariantCulture) ?? "0";
			}
			if (name.StartsWith("HTTP_", StringComparison.OrdinalIgnoreCase))
				return request.Headers[name.Substring(5).Replace('_', '-')].ToString();
			return "";
		}

		static int Port(Core.HttpRequest request) => request.Host.Port ?? (request.IsHttps ? 443 : 80);

		static string Address(System.Net.IPAddress address) =>
			address == null ? "" : (address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address).ToString();

		// The URL's file or folder (a folder's with its separator, as IIS's).
		string Physical(string path)
		{
			var relative = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
			return Path.Combine(contentRoot, relative);
		}

		static void SetVariable(State state, string name, string value, bool replace)
		{
			var context = state.Context;
			if (name.StartsWith("HTTP_", StringComparison.OrdinalIgnoreCase))
			{
				var header = name.Substring(5).Replace('_', '-');
				if (replace || !context.Request.Headers.ContainsKey(header)) context.Request.Headers[header] = value;
				return;
			}
			if (name.StartsWith("RESPONSE_", StringComparison.OrdinalIgnoreCase))
			{
				// When the response starts: System.Web sends its headers anew.
				var header = name.Substring(9).Replace('_', '-');
				context.Response.OnStarting(() =>
				{
					if (replace || !context.Response.Headers.ContainsKey(header)) context.Response.Headers[header] = value;
					return Task.CompletedTask;
				});
				return;
			}
			state.Variables ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (replace || !state.Variables.ContainsKey(name)) state.Variables[name] = value;
		}

		/// <summary>
		/// IIS's redirect: an absolute Location, its "Document Moved" body (its courtesy redirects', URL Rewrite's); the body
		/// returned, for the caller to write.
		/// </summary>
		internal static byte[] Redirect(Core.HttpContext context, int status, string location)
		{
			var request = context.Request;
			if (location.StartsWith("/", StringComparison.Ordinal) && !location.StartsWith("//", StringComparison.Ordinal))
				location = request.Scheme + "://" + (request.Host.HasValue ? request.Host.Value : Address(context.Connection.LocalIpAddress)) + location;
			var response = context.Response;
			response.StatusCode = status;
			// The status line as IIS writes its redirects' (URL Rewrite's, HTTP redirection's).
			var reason = status switch { 302 => "Redirect", 307 => "Moved Temporarily", 308 => "Redirected Permanently", _ => null };
			var feature = context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpResponseFeature>();
			if (reason != null && feature != null) feature.ReasonPhrase = reason;
			response.Headers.Location = location;
			response.ContentType = "text/html; charset=UTF-8";
			// In the server's language, as IIS writes it (the system's: a Japanese Windows' IIS writes Japanese); a line feed after
			// the head, not CR LF (IIS's bytes).
			var href = location.Replace("\"", "%22");
			var body = Encoding.UTF8.GetBytes(CultureInfo.InstalledUICulture.TwoLetterISOLanguageName == "ja"
				? "<head><title>ドキュメント移動</title></head>\n<body><h1>オブジェクトは移動しました</h1>このドキュメントは<a HREF=\"" + href + "\">ここ</a>で見つかる可能性があります</body>"
				: "<head><title>Document Moved</title></head>\n<body><h1>Object Moved</h1>This document may be found <a HREF=\"" + href + "\">here</a></body>");
			response.ContentLength = body.Length;
			return body;
		}

		// A response of the rules' own: the status, IIS's page for it, its substatus in the log.
		static byte[] Respond(Core.HttpContext context, int status, int subStatus, string reason)
		{
			var response = context.Response;
			response.StatusCode = status;
			// IIS's 500.50: a server variable the server does not let rules set.
			if (string.IsNullOrEmpty(reason) && status == 500 && subStatus == 50) reason = "URL Rewrite Module Error.";
			if (!string.IsNullOrEmpty(reason))
			{
				var feature = context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpResponseFeature>();
				if (feature != null) feature.ReasonPhrase = reason;
			}
			// The status only: its page is IIS's httpErrors' (IisHttpErrors).
			IisLogStatus.Set(context, subStatus);
			return NoBody;
		}

		/// <summary>The middleware: the rules, then the rest of the pipeline unless they answered.</summary>
		internal sealed class Middleware
		{
			readonly Core.RequestDelegate next;
			readonly IisUrlRewrite rewrite;

			public Middleware(Core.RequestDelegate next, IisUrlRewrite rewrite)
			{
				this.next = next;
				this.rewrite = rewrite;
			}

			public async Task Invoke(Core.HttpContext context)
			{
				if (!await rewrite.RunAsync(context)) await next(context);
			}
		}
	}

	/// <summary>The server variables URL Rewrite's rules set (serverVariables), which Request.ServerVariables has.</summary>
	internal sealed class IisServerVariables
	{
		internal readonly Dictionary<string, string> Values;
		internal IisServerVariables(Dictionary<string, string> values) { Values = values; }
	}
}

#endif
