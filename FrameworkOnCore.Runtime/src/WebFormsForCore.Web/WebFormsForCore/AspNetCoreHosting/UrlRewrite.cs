#if NETCOREAPP

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// IIS's URL Rewrite module (system.webServer/rewrite of web.config and of the original server: IisProfile), in one of
	/// two ways (UrlRewrite:Mode in the configuration):
	/// <list type="bullet">
	/// <item>Iis (the default): the rules run as IIS runs them (System.Web.Hosting.IisUrlRewrite): its inheritance, its
	/// server variables, its patterns, its answers.</item>
	/// <item>AspNetCore: ASP.NET Core's rewriter (Microsoft.AspNetCore.Rewrite, AddIISUrlRewrite), which an application
	/// rewritten for ASP.NET Core keeps. The root's web.config only; what it does otherwise than IIS: the query string
	/// appended before the rule's, a relative Location (absolute, as IIS's, if a browser would take it for another host's:
	/// "//host", "/\host"), IsDirectory never true, {REQUEST_URI} without the query string, no
	/// X-Original-URL; a rule it cannot read (subStatusCode, Wildcard, server variables it does not have:
	/// HTTP_X_FORWARDED_PROTO, URL, SERVER_PORT...) stops the application's start.</item>
	/// <item>Off: the rules are not run.</item>
	/// </list>
	/// After the W3C log (whose line has the URL asked for, as IIS's), before System.Web.
	/// </summary>
	public static class WebFormsUrlRewriteExtensions
	{
		public static IApplicationBuilder UseWebFormsUrlRewrite(this IApplicationBuilder app)
		{
			var services = app.ApplicationServices;
			var configuration = services.GetService<IConfiguration>();
			var environment = services.GetService<IHostEnvironment>();
			if (environment == null) return app;
			var mode = configuration?["UrlRewrite:Mode"];
			if (string.Equals(mode, "Off", StringComparison.OrdinalIgnoreCase)) return app;
			if (string.Equals(mode, "AspNetCore", StringComparison.OrdinalIgnoreCase)) return UseAspNetCoreRewriter(app, environment.ContentRootPath);
			if (!string.IsNullOrEmpty(mode) && !string.Equals(mode, "Iis", StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("UrlRewrite:Mode " + mode + ": Iis, AspNetCore or Off");
			var rewrite = System.Web.Hosting.IisUrlRewrite.Load(environment.ContentRootPath);
			return rewrite == null ? app : app.UseMiddleware<System.Web.Hosting.IisUrlRewrite.Middleware>(rewrite);
		}

		static IApplicationBuilder UseAspNetCoreRewriter(IApplicationBuilder app, string contentRoot)
		{
			var rules = Rules(contentRoot);
			if (rules == null) return app;
			var options = new RewriteOptions
			{
				// {REQUEST_FILENAME}'s IsFile and IsDirectory: the application's folder (the rewriter's default is wwwroot).
				StaticFileProvider = new PhysicalFileProvider(contentRoot),
			};
			try
			{
				using (var reader = new StringReader(rules)) options.AddIISUrlRewrite(reader);
			}
			catch (Exception e) when (e is NotSupportedException || e is FormatException || e is InvalidOperationException)
			{
				throw new InvalidOperationException("web.config's URL Rewrite rules: ASP.NET Core's rewriter (UrlRewrite:Mode AspNetCore) does not read them: " +
					e.Message + ". UrlRewrite:Mode Iis runs them as IIS does.", e);
			}
			// A redirect of the rewriter's own (the request not passed on): its Location as the rule made it, "//evil.example" too
			// (a rule's "/{C:1}" of a query string "/evil.example") or "/\evil.example", which a browser takes for another host's
			// (an open redirect). Such a one made absolute with the request's host, as Iis mode (IIS) sends every redirect's
			// (http://host//evil.example); a path that is one on this host stays relative (the summary).
			var passedOn = new object();
			app.Use(async (context, next) =>
			{
				await next(context);
				var response = context.Response;
				if (context.Items.ContainsKey(passedOn) || response.HasStarted) return;
				var location = response.Headers.Location.ToString();
				if (location.Length > 0 && System.Web.Hosting.IisUrlRewrite.IsSchemeRelative(location))
					response.Headers.Location = System.Web.Hosting.IisUrlRewrite.Absolute(context, System.Web.Hosting.IisUrlRewrite.WithoutLeadingControls(location));
			});
			app.UseRewriter(options);
			return app.Use((context, next) =>
			{
				context.Items[passedOn] = true;
				return next(context);
			});
		}

		/// <summary>web.config's rewrite section (its root's and its &lt;location&gt; for the root), as a document of its own; null without rules.</summary>
		static string Rules(string contentRoot)
		{
			var file = Path.Combine(contentRoot, "web.config");
			if (!File.Exists(file)) file = Directory.EnumerateFiles(contentRoot, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
			if (file == null) return null;
			XElement root;
			try { root = XDocument.Load(file).Root; }
			catch (Exception e) when (e is System.Xml.XmlException || e is IOException) { return null; }
			var rewrite = root.Elements().Where(e => e.Name.LocalName == "system.webServer")
				.Concat(root.Elements().Where(e => e.Name.LocalName == "location")
					.Where(l => { var p = (string)l.Attribute("path"); return string.IsNullOrEmpty(p) || p == "."; })
					.Elements().Where(e => e.Name.LocalName == "system.webServer"))
				.Elements().FirstOrDefault(e => e.Name.LocalName == "rewrite");
			if (rewrite == null) return null;
			// The rewriter reads <rewrite> in a document's root's system.webServer, without a namespace.
			foreach (var element in rewrite.DescendantsAndSelf()) element.Name = element.Name.LocalName;
			return new XElement("configuration", new XElement("system.webServer", rewrite)).ToString();
		}
	}
}

#endif
