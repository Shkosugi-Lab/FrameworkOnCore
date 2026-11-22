// ASP.NET Core middleware

#if NETCOREAPP

using Microsoft.AspNetCore.Builder;
using Core = Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;
using System.Runtime.Loader;

namespace Microsoft.AspNetCore.Builder
{
	public class WebFormsMiddleware
	{
		const bool UseSeparateAssemblyLoadContext = false;

		public string VirtualPath { get; set; }
		public string PhysicalPath { get; set; }
		public string AppId { get; set; }
		public bool HandleAllRequestsWithWebForms = false;
		public bool UseClassicMode = true;
		public IEnumerable<string> HandleExtensions = null;
		public IEnumerable<string> AddHandleExtensions = Array.Empty<string>();
		public IEnumerable<string> DefaultDocuments = null;
		public IEnumerable<string> AddDefaultDocuments = Array.Empty<string>();


		private readonly Core.RequestDelegate next;

		public readonly ApplicationManager ApplicationManager = new ApplicationManager();

		AspNetCoreHost host = null;
		public AspNetCoreHost Host
		{
			get
			{
				if (host == null)
				{
					//AppId = ApplicationManager.CreateApplicationId(VirtualPath, PhysicalPath);
					//host = ApplicationManager.CreateInstanceInNewWorkerLoadContext(typeof(AspNetCoreHost), AppId, System.Web.VirtualPath.Create(VirtualPath), PhysicalPath, UseSeparateAssemblyLoadContext) as AspNetCoreHost;
					host = CreateWorkerLoadContextWithHost(VirtualPath, PhysicalPath, typeof(AspNetCoreHost)) as AspNetCoreHost;
					if (HandleExtensions != null) host.HandleExtensions = HandleExtensions.ToArray();
					if (AddHandleExtensions.Any()) host.HandleExtensions = host.HandleExtensions.Concat(AddHandleExtensions).ToArray();
					if (DefaultDocuments != null) host.DefaultDocuments = DefaultDocuments.ToArray();
					if (AddDefaultDocuments.Any()) host.DefaultDocuments = host.DefaultDocuments.Concat(AddDefaultDocuments).ToArray();
					host.HandleAllRequestsWithWebForms = HandleAllRequestsWithWebForms;
					host.UseClassicMode = UseClassicMode;
				}
				return host;
			}
		}

		private object CreateWorkerLoadContextWithHost(string virtualPath, string physicalPath, Type hostType)
		{
			// create BuildManagerHost in the worker app domain
			//ApplicationManager appManager = ApplicationManager.GetApplicationManager();
			Type buildManagerHostType = typeof(HttpRuntime).Assembly.GetType("System.Web.Compilation.BuildManagerHost");
			AppId = ApplicationManager.CreateApplicationId(VirtualPath, PhysicalPath);
			var vpath = System.Web.VirtualPath.Create(virtualPath);
			IRegisteredObject buildManagerHost = ApplicationManager.CreateInstanceInNewWorkerLoadContext(buildManagerHostType, AppId, vpath, physicalPath, false) as IRegisteredObject;

			// call BuildManagerHost.RegisterAssembly to make Host type loadable in the worker app domain
			buildManagerHostType.InvokeMember("RegisterAssembly",
											  BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
											  null,
											  buildManagerHost,
											  new object[] { hostType.Assembly.FullName, hostType.Assembly.Location });
			// create Host in the worker app domain
			return ApplicationManager.CreateObject(AppId, hostType, virtualPath, physicalPath, false);
		}

		public void RestartApplication()
		{
			AssemblyLoadContext context;
			lock (ApplicationManager)
			{
				context = ApplicationManager.GetAssemblyLoadContext(AppId);
				host = null;
				AppId = Guid.NewGuid().ToString();
			}
			if (context != AssemblyLoadContext.Default) context.Unload();
		}

		public WebFormsMiddleware(Core.RequestDelegate next, Action<WebFormsOptions> optionsBuilder, WebFormsPipeline pipeline)
			: this(next, optionsBuilder) => pipeline.Middleware = this;

		public WebFormsMiddleware(Core.RequestDelegate next, Action<WebFormsOptions> optionsBuilder)
		{
			this.next = next;
			var path = System.Web.Hosting.AppBinDirectory.PhysicalPath;
			if (path.EndsWith(Path.DirectorySeparatorChar.ToString())) path = path.Substring(0, path.Length - 1);
			// In a worker (WebFormsProcess), bin is a copy: the application's root is given.
			PhysicalPath = WebFormsProcess.AppPath ?? Path.GetDirectoryName(path);
			//PhysicalPath = Path.GetDirectoryName(Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath));
			VirtualPath = "/";

			optionsBuilder?.Invoke(new WebFormsOptions(this));

			Host.Configure(VirtualPath, PhysicalPath);
		}

		public void AllowSynchronousIO(Core.HttpContext context)
		{
			var syncIoFeature = context.Features.Get<Core.Features.IHttpBodyControlFeature>();
			if (syncIoFeature != null)
			{
				syncIoFeature.AllowSynchronousIO = true;
			}
		}

		public async Task Invoke(Core.HttpContext context)
		{
			// The URL as http.sys gives it (HttpSysUrl), when IIS's features in front have not made it so already: the
			// restart's 307 below has a Location of the path ("//evil.example/x" was sent as it was: another host's).
			if (!await HttpSysUrl.Canonicalize(context)) return;
			// The application is restarting (web.config changed ...): on .NET Framework the requests from
			// now on go to its new AppDomain; this one is ending (ProcessRestart), so they are to come
			// again once it is started.
			if (HostingEnvironment.ShutdownInitiated)
			{
				// The listening sockets stay (ListenerHandoff): the request again, on a new connection, waits in their queue
				// for the next worker, and is answered as IIS answers it from the next AppDomain (307 keeps a POST's method
				// and body). This connection, kept alive by the client, is this worker's, which is ending.
				if (ListenerHandoff.Active)
				{
					// (no more connections taken: they wait for the next worker)
					ListenerHandoff.StopAccepting();
					context.Response.StatusCode = 307;
					context.Response.Headers["Location"] = context.Request.PathBase.Add(context.Request.Path).ToUriComponent() + context.Request.QueryString.ToUriComponent();
					context.Response.Headers["Connection"] = "close";
					return;
				}
				context.Response.StatusCode = 503;
				context.Response.Headers["Retry-After"] = "1";
				context.Response.Headers["Connection"] = "close";
				return;
			}
			// A static file's response kept as IIS's kernel cache keeps it: answered before System.Web, as IIS answers it
			// before its pipeline (StaticResponseCache).
			if (await StaticResponseCache.TryServe(context)) { IisDecision.Set(context, IisDecision.StaticFile); return; }
			// IIS's request filtering, on the URL as it is now: the rewritten one (it ran on the URL as sent before URL Rewrite:
			// UseWebFormsIis, RequestFiltering).
			if (RequestFiltering(context)) { await AfterFiltering(context); return; }
			// OPTIONS that no handler of the application's takes (IIS's OPTIONSVerbHandler, after the managed handlers of
			// GET, HEAD, POST and DEBUG, before StaticFile): 200 with the verbs IIS's protocol support module names, no body
			// (IIS 10: OPTIONS of a page, a static file, a folder; a CORS preflight). The page's handler answered 405, a static
			// file's 404, a page under forms authentication its redirect.
			// Before the static files' answers (a static file's path info is not theirs to answer for OPTIONS: IIS 10's OPTIONS
			// "/y.secret/x" is 200); not the asterisk form ("OPTIONS *": IIS's 500.50).
			if (Core.HttpMethods.IsOptions(context.Request.Method) && (context.Request.Path.Value ?? "").StartsWith('/') && IntegratedModeConfiguration.AnswersOptions((context.Request.PathBase.Value ?? "") + (context.Request.Path.Value ?? "")))
			{
				context.Response.StatusCode = 200;
				context.Response.Headers["Allow"] = "OPTIONS, TRACE, GET, HEAD, POST";
				context.Response.Headers["Public"] = "OPTIONS, TRACE, GET, HEAD, POST";
				IisDecision.Set(context, IisDecision.ProtocolSupport);
				context.Response.ContentLength = 0;
				return;
			}
			// TRACE that no handler of the application's takes (IIS's TRACEVerbHandler: its protocol support module's 501.0, of a
			// page, a handler, a static file, of nothing; after its request filtering - TRACE of App_Data is 404.8 - and URL
			// Rewrite - of a rule's URL its 301). ASP.NET's handlers answered 405.
			if (Core.HttpMethods.IsTrace(context.Request.Method) && (context.Request.Path.Value ?? "").StartsWith('/') && IntegratedModeConfiguration.AnswersTrace((context.Request.PathBase.Value ?? "") + (context.Request.Path.Value ?? "")))
			{
				System.Web.Hosting.IisLogStatus.Set(context, 0);
				context.Response.StatusCode = 501;
				IisDecision.Set(context, IisDecision.ProtocolSupport);
				return;
			}
			// The handler's stage (after URL Rewrite and httpRedirect: IIS 10's "/go/y.secret%20" is the rule's 301, not a 404).
			// A path ending in whitespace (" ", U+00A0, U+3000...): System.Web's VirtualPath trims it, so "/y.secret%20" was the
			// file y.secret to it (served on Linux, where the name with its space is no file: request filtering saw "y.secret "),
			// "/default.aspx%E3%80%80" the page, "/secure/x.aspx%E3%80%80" the page under another URL. IIS answers 404 (404.0).
			var last = (context.Request.Path.Value ?? "").Length > 0 ? context.Request.Path.Value[^1] : 'x';
			if (char.IsWhiteSpace(last))
			{
				System.Web.Hosting.IisLogStatus.Set(context, 0, 2);
				context.Response.StatusCode = 404;
				IisDecision.Set(context, IisDecision.StaticFile);
				return;
			}
			// A static file's name with path info after it ("/x.cs/x", "/global.asax/x": no handler's type, IisUrl.Split):
			// IIS's static file handler finds no file of the whole path, 404.0 (ASP.NET's handler of the file's type answered,
			// 403).
			if (host.IsStaticFileWithPathInfo(context))
			{
				System.Web.Hosting.IisLogStatus.Set(context, 0, 2);
				context.Response.StatusCode = 404;
				IisDecision.Set(context, IisDecision.StaticFile);
				return;
			}
			if (IsLegacyRequest(context))
			{
				// System.Web answers it: its error answers are IIS's httpErrors' to replace (IisHttpErrors).
				var flags = context.Features.Get<IisErrorFlags>();
				if (flags == null) context.Features.Set(flags = new IisErrorFlags());
				flags.SystemWeb = true;
				AllowSynchronousIO(context);
				IisDecision.Set(context, IisDecision.Managed);

				await host.ProcessRequest(context);
			} else {
				// (no file, no handler: the static file handler's 404 on IIS)
				IisDecision.Set(context, IisDecision.StaticFile);
				await next.Invoke(context);
			}
		}

		public virtual bool IsLegacyRequest(Core.HttpContext context) => host.IsLegacyRequest(context);

		/// <summary>
		/// IIS's request filtering (RequestFilteringModule) for the URL as the request has it now: true when it refused it
		/// (the status set: its page is httpErrors'). IIS runs it on the URL as sent, before URL Rewrite, and again on the
		/// URL a rule rewrote it to (observed on IIS 10: "/App_Data/from" rewritten to "/a.txt" is 404.8, "/to-data"
		/// rewritten to "/App_Data/users.xml" is 404.8): UseWebFormsIis runs it before URL Rewrite, Invoke after. It ran
		/// after only: a protected URL a rule rewrote was served.
		/// </summary>
		internal static Task AfterFiltering(Core.HttpContext context) =>
			context.Response.StatusCode == 413 ? AspNetCoreWorkerRequest.TooLargeAsync(context) : Task.CompletedTask;

		internal bool RequestFiltering(Core.HttpContext context)
		{
			// (A folder's URL without its '/': IIS's courtesy redirect comes after the managed pipeline has run for it, from its
			// static file handler: IisStaticFile, as System.Web answers the request.)
			// What IIS's request filtering refuses (404.7: web.config, the sources, the databases' files): its 404 page,
			// not the empty answer of the end of ASP.NET Core's pipeline.
			// IIS: web.config and the hidden segments (bin, App_Data, App_Code...: also where nothing is) 404.8, the other types
			// its denied file extensions 404.7. The status only: its page is httpErrors' (IisHttpErrors).
			// IIS's request filtering's order (IIS 10, iis-settings.json): the URL's and the query's lengths (404.14, 404.15:
			// before double escaping), the verbs (404.6), double escaping (404.11), characters beyond ASCII (404.12), the denied
			// URL sequences (404.5), the hidden segments and the denied file extensions, the query's sequences (404.18), the
			// headers' sizes (431), the filtering rules (404.19). A URL of alwaysAllowedUrls skips the URL's own checks, a query
			// of alwaysAllowedQueryStrings the query's. Each refusal closes the connection (IIS's Connection: close).
			var requestPath = (context.Request.PathBase.Value ?? "") + (context.Request.Path.Value ?? "");
			var filtering = IntegratedModeConfiguration.RequestFiltering(requestPath);
			var rawQuery = (context.Request.QueryString.Value ?? "").TrimStart('?');
			bool Refuse(int status, int subStatus)
			{
				System.Web.Hosting.IisLogStatus.Set(context, subStatus);
				context.Response.StatusCode = status;
				context.Response.Headers["Connection"] = "close";
				IisDecision.Set(context, IisDecision.RequestFiltering);
				return true;
			}
			// requestLimits' maxUrl (404.14) and maxQueryString (404.15), for every URL (the worker request checked them for
			// System.Web's requests only: a long URL of a file not there was 404.0).
			var limits = IntegratedModeConfiguration.RequestLimits(requestPath);
			if (requestPath.Length > limits.MaxUrl) return Refuse(404, 14);
			if (rawQuery.Length > limits.MaxQueryString) return Refuse(404, 15);
			var urlAllowed = filtering.AlwaysAllowedUrls.Contains(requestPath);
			if (!urlAllowed)
			{
				if (!filtering.AllowDoubleEscaping && AspNetCoreWorkerRequest.IsDoubleEscaped(requestPath)) return Refuse(404, 11);
				if (!filtering.AllowHighBitCharacters && requestPath.Any(c => c > 0x7F)) return Refuse(404, 12);
				if (filtering.DenyUrlSequences.Any(sequence => requestPath.IndexOf(sequence, StringComparison.OrdinalIgnoreCase) >= 0)) return Refuse(404, 5);
				var hidden = AspNetCoreWorkerRequest.IsHidden(context.Request.Path.Value ?? "", requestPath);
				if (hidden || host.IsProhibited(context))
				{
					var name = System.IO.Path.GetFileName(context.Request.Path.Value ?? "");
					return Refuse(404, hidden || string.Equals(name, "web.config", StringComparison.OrdinalIgnoreCase) ? 8 : 7);
				}
			}
			// (after the hidden segments: IIS 10's GET /App_Data/a.txt with GET denied is 404.8)
			var method = context.Request.Method;
			if (filtering.Verbs.TryGetValue(method, out var verbAllowed) ? !verbAllowed : !filtering.VerbsAllowUnlisted) return Refuse(404, 6);
			// The query as sent and, with unescapeQueryString (the default), unescaped (IIS 10: "q=%62ad" is refused by "bad").
			string unescapedQuery = null;
			if (filtering.UnescapeQueryString && (filtering.DenyQueryStringSequences.Count > 0 || filtering.Rules.Count > 0))
			{
				try { unescapedQuery = Uri.UnescapeDataString(rawQuery.Replace('+', ' ')); }
				catch (UriFormatException) { unescapedQuery = rawQuery; }
			}
			bool QueryHas(string text) => rawQuery.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
				(unescapedQuery != null && unescapedQuery.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
			// (an allowed query as sent or unescaped: IIS 10's "q=bad" allows "q=%62ad")
			var queryAllowed = filtering.AlwaysAllowedQueryStrings.Contains(rawQuery) || (unescapedQuery != null && filtering.AlwaysAllowedQueryStrings.Contains(unescapedQuery));
			if (!queryAllowed && filtering.DenyQueryStringSequences.Any(QueryHas)) return Refuse(404, 18);
			foreach (var limit in filtering.HeaderLimits)
				if (context.Request.Headers.TryGetValue(limit.Key, out var values) && values.ToString().Length > limit.Value) return Refuse(431, 0);
			if (filtering.Rules.Count > 0)
			{
				var ruleName = requestPath[(requestPath.LastIndexOf('/') + 1)..];
				var dot = ruleName.LastIndexOf('.');
				var extension = dot < 0 ? "." : ruleName[dot..];
				foreach (var rule in filtering.Rules)
				{
					if (rule.AppliesTo.Count > 0 && !rule.AppliesTo.Contains(extension, StringComparer.OrdinalIgnoreCase)) continue;
					foreach (var text in rule.DenyStrings)
					{
						var found = (rule.ScanUrl || rule.ScanAllRaw) && requestPath.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
							(rule.ScanQueryString || rule.ScanAllRaw) && QueryHas(text) ||
							rule.ScanHeaders.Any(h => context.Request.Headers.TryGetValue(h, out var header) && header.ToString().IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0) ||
							rule.ScanAllRaw && context.Request.Headers.Any(h => h.Value.ToString().IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
						if (found) return Refuse(404, 19);
					}
				}
			}
			// removeServerHeader: no Server header on the answer.
			if (filtering.RemoveServerHeader)
				context.Response.OnStarting(() => { context.Response.Headers.Remove("Server"); return Task.CompletedTask; });
			// requestLimits' maxAllowedContentLength (413.1), after the URL's checks (IIS 10: 300 KB posted to App_Data is
			// 404.8) and before URL Rewrite (posted to a rule's URL it is 413.1, not the rule's redirect). The worker request
			// checked it, for System.Web's requests, after the rules: the redirect was answered.
			if (context.Request.ContentLength > limits.MaxAllowedContentLength) return Refuse(413, 1);
			// A last segment ending in '.' or a space ("/a.txt.", "/old/x.", "/sub%20") of a URL ASP.NET's handlers take (no
			// extension: IIS's "*." handler, which a name ending in '.' matches; "page.aspx " has the type ".aspx ", no
			// handler's: IIS 10's "/go/page.aspx%20" is the rule's 301): ASP.NET refuses it as its
			// managed pipeline starts (a suspicious physical path: Windows takes the name without them), before URL Rewrite
			// and httpRedirect (IIS 10: "/go/a.txt." and "/go/sub%20" are ASP.NET's 404, not the rule's redirect). A static
			// file's name is not ASP.NET's ("/go/y.secret%20" is the rule's 301; Invoke refuses one after). Nor OPTIONS IIS's
			// own handler answers. It was refused after them.
			var path = context.Request.Path.Value ?? "";
			var lastName = path[(path.LastIndexOf('/') + 1)..];
			if (lastName.Length > 0 && (lastName[^1] == '.' || char.IsWhiteSpace(lastName[^1])) &&
				!(Core.HttpMethods.IsOptions(context.Request.Method) && IntegratedModeConfiguration.AnswersOptions((context.Request.PathBase.Value ?? "") + path)))
			{
				var trimmed = lastName.TrimEnd();
				if (lastName[^1] == '.' || trimmed.IndexOf('.') < 0)
				{
					System.Web.Hosting.IisLogStatus.Set(context, 0, 2);
					context.Response.StatusCode = 404;
					IisDecision.Set(context, IisDecision.Managed);
					return true;
				}
			}
			return false;
		}
	}

	/// <summary>
	/// The application's pipeline's Web Forms (its WebFormsMiddleware), for the IIS features in front of it (UseWebFormsIis):
	/// they run before it is made, it is given to them once it is.
	/// </summary>
	public sealed class WebFormsPipeline
	{
		const string Key = "FrameworkOnCore.WebFormsPipeline";
		public WebFormsMiddleware Middleware { get; internal set; }

		public static WebFormsPipeline Of(IApplicationBuilder builder)
		{
			if (builder.Properties.TryGetValue(Key, out var value) && value is WebFormsPipeline pipeline) return pipeline;
			builder.Properties[Key] = pipeline = new WebFormsPipeline();
			return pipeline;
		}
	}

	public class WebFormsOptions
	{
		WebFormsMiddleware Instance;

		public WebFormsOptions(WebFormsMiddleware instance) { Instance = instance; }
		public WebFormsOptions HandleExtensions(params string[] extensions) { Instance.HandleExtensions = extensions; return this; }
		public WebFormsOptions AddHandleExtensions(params string[] extensions) { Instance.AddHandleExtensions = Instance.AddHandleExtensions.Concat(extensions).ToArray(); return this; }
		public WebFormsOptions DefaultDocuments(params string[] docs) { Instance.DefaultDocuments = docs; return this; }
		public WebFormsOptions AddDefaultDocuments(params string[] docs) { Instance.AddDefaultDocuments = Instance.AddDefaultDocuments.Concat(docs).ToArray(); return this; }
		public WebFormsOptions VirtualPath(string path) {
			if (!path.StartsWith('/')) path = "/" + path;
			Instance.VirtualPath = path;
			return this;
		}
		public WebFormsOptions PhysicalPath(string path) { Instance.PhysicalPath = path; return this; }
		public WebFormsOptions HandleAllRequestsWithWebForms() { Instance.HandleAllRequestsWithWebForms = true; return this; }

        public WebFormsOptions UseClassicMode() { Instance.UseClassicMode = true; return this; }
        public WebFormsOptions UseIntegratedMode() { Instance.UseClassicMode = false; return this; }
        public WebFormsOptions UseAspNetCoreSessionProvider()
        {
            // Equivalent to a web.config entry of:
            // <sessionState mode="Custom" customProvider="AspNetCoreSession">
            //     <providers>
            //         <add name="AspNetCoreSession" type="System.Web.SessionState.AspNetCoreSessionProvider, System.Web" />
            //     </providers>
            // </sessionState>
            SessionStateModule.ProviderOverride = () =>
            {
                var provider = new AspNetCoreSessionProvider();
                provider.Initialize("AspNetCoreSession", new NameValueCollection());
                return provider;
            };

            return this;
        }
    }

    public static class WebFormsMiddlewareExtensions
	{
		public static IApplicationBuilder UseWebForms(this IApplicationBuilder builder, Action<WebFormsOptions> optionsBuilder = null)
		{
			AssemblyLoaderNetCore.Init();
			// The code pages .NET Framework had (iso-8859-15, shift_jis, windows-1252 ...): .NET has only the
			// Unicode ones and Latin-1 until they are registered. web.config names them (<globalization
			// fileEncoding>, requestEncoding, responseEncoding) and pages use them (Encoding.GetEncoding).
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			if (optionsBuilder == null) optionsBuilder = options => { };
			// A response header's value beyond ASCII (Response.AppendHeader("Content-Disposition", "attachment; filename=" +
			// a Japanese name), a cookie's) is sent, as IIS sends it, where Kestrel threw (InvalidOperationException: the
			// application's 500): UTF-8 where the host chose no encoding of its own for the header (IIS writes the system's code
			// page).
			var kestrel = builder.ApplicationServices?.GetService(typeof(Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>))
				as Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>;
			if (kestrel != null)
			{
				var chosen = kestrel.Value.ResponseHeaderEncodingSelector;
				kestrel.Value.ResponseHeaderEncodingSelector = name => chosen?.Invoke(name) ?? System.Text.Encoding.UTF8;
			}
			// An application restart ends the process (ProcessRestart): through the host, which lets the
			// pending requests complete.
			var lifetime = builder.ApplicationServices?.GetService(typeof(Microsoft.Extensions.Hosting.IHostApplicationLifetime)) as Microsoft.Extensions.Hosting.IHostApplicationLifetime;
			if (lifetime != null)
			{
				ProcessRestart.StopHost = lifetime.StopApplication;
				ProcessRestart.HostStopping = () => lifetime.ApplicationStopping.IsCancellationRequested;
			}
			return builder.UseMiddleware<WebFormsMiddleware>(optionsBuilder, WebFormsPipeline.Of(builder));
        }
        public static IApplicationBuilder UseAspNetCoreSessionProvider(this IApplicationBuilder builder)
        {
            AssemblyLoaderNetCore.Init();

            // Equivalent to a web.config entry of:
            // <sessionState mode="Custom" customProvider="AspNetCoreSession">
            //     <providers>
            //         <add name="AspNetCoreSession" type="System.Web.SessionState.AspNetCoreSessionProvider, System.Web" />
            //     </providers>
            // </sessionState>
            SessionStateModule.ProviderOverride = () =>
            {
                var provider = new AspNetCoreSessionProvider();
                provider.Initialize("AspNetCoreSession", new NameValueCollection());
                return provider;
            };

			return builder;
        }

    }
}

#endif