#if NETCOREAPP

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// IIS's features in front of System.Web, in IIS's order, from web.config's system.webServer and the original server's
	/// settings (iis-profile.config): the response headers (customHeaders, redirectHeaders: every response's, the ones
	/// answered by what follows too), compression (the answers as they finally are), the error pages (httpErrors: every
	/// answer of 400 or more, the ones of what follows),
	/// URL Rewrite (UrlRewrite:Mode), HTTP redirection (httpRedirect: the rewritten URL's).
	/// After the W3C log (whose line has the URL asked for, as IIS's), before UseWebForms.
	/// HttpErrors:Mode in the configuration: Iis (the default, System.Web.Hosting.IisHttpErrors: IIS's pages, the
	/// application's error answers replaced as IIS replaces them), AspNetCore (ASP.NET Core's status code pages: a text
	/// for an error without a body; the application's answers left), Off (an error without a body stays empty).
	/// HttpErrors:LocalAddresses: the clients taken as this machine for errorMode DetailedLocalOnly ("*", addresses,
	/// networks), for a test whose client is in front of a container; none by default (the loopback and the server's own
	/// address only, as IIS).
	/// Compression:Mode: Iis (the default, System.Web.Hosting.IisCompression: IIS's static and dynamic compression as the
	/// original server has them; without its settings, Windows' default, static only), AspNetCore (ASP.NET Core's response
	/// compression: every answer of its types, brotli first; not over HTTPS unless its options say), Off (none: a proxy in
	/// front compresses).
	/// </summary>
	public static class WebFormsIisExtensions
	{
		public static IApplicationBuilder UseWebFormsIis(this IApplicationBuilder app)
		{
			var environment = app.ApplicationServices.GetService<IHostEnvironment>();
			if (environment == null) return app;
			var contentRoot = environment.ContentRootPath;
			// The URL as http.sys gives it to IIS (HttpSysUrl: decoded, its separators folded, its dot segments resolved), for
			// every feature after it; what http.sys refuses is answered here.
			app.Use(async (context, next) =>
			{
				if (await System.Web.Hosting.HttpSysUrl.Canonicalize(context)) await next(context);
			});
			app.UseMiddleware<System.Web.Hosting.IisHttpProtocol.Middleware>(new System.Web.Hosting.IisHttpProtocol(contentRoot));
			var configuration = app.ApplicationServices.GetService<IConfiguration>();
			var compression = configuration?["Compression:Mode"];
			if (string.IsNullOrEmpty(compression) || string.Equals(compression, "Iis", StringComparison.OrdinalIgnoreCase))
			{
				var iis = new System.Web.Hosting.IisCompression(contentRoot);
				if (iis.Any) app.UseMiddleware<System.Web.Hosting.IisCompression.Middleware>(iis);
			}
			else if (string.Equals(compression, "AspNetCore", StringComparison.OrdinalIgnoreCase))
				app.UseMiddleware<Microsoft.AspNetCore.ResponseCompression.ResponseCompressionMiddleware>(
					new Microsoft.AspNetCore.ResponseCompression.ResponseCompressionProvider(app.ApplicationServices,
						Microsoft.Extensions.Options.Options.Create(new Microsoft.AspNetCore.ResponseCompression.ResponseCompressionOptions())));
			else if (!string.Equals(compression, "Off", StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("Compression:Mode " + compression + ": Iis, AspNetCore or Off");
			var mode = configuration?["HttpErrors:Mode"];
			if (string.IsNullOrEmpty(mode) || string.Equals(mode, "Iis", StringComparison.OrdinalIgnoreCase))
				app.UseMiddleware<System.Web.Hosting.IisHttpErrors.Middleware>(new System.Web.Hosting.IisHttpErrors(contentRoot,
					app.ApplicationServices.GetService<IConfiguration>()?["HttpErrors:LocalAddresses"]));
			else if (string.Equals(mode, "AspNetCore", StringComparison.OrdinalIgnoreCase))
				app.UseStatusCodePages();
			else if (!string.Equals(mode, "Off", StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("HttpErrors:Mode " + mode + ": Iis, AspNetCore or Off");
			app.UseWebFormsUrlRewrite();
			var redirect = System.Web.Hosting.IisHttpRedirect.Load(contentRoot);
			if (redirect != null) app.UseMiddleware<System.Web.Hosting.IisHttpRedirect.Middleware>(redirect);
			return app;
		}
	}
}

#endif
