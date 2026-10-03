// A converted application's WCF services in its ASP.NET Core host, before Web Forms (a site whose every request goes to
// Web Forms has its .svc requests served here first): its .svc files and web.config's activations served by CoreWCF
// (ServicePlan), their faults made CoreWCF's (FaultBridge). A .svc request no service answers is not found (404), as the
// .svc file is no page to send.
//
//   builder.Services.AddWebFormsServiceModel();
//   ...
//   app.UseWebFormsServiceModel();
//   app.UseWebForms(...);

using System;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text.RegularExpressions;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebFormsServiceModelExtensions
    {
        private static readonly Regex s_svcPath = new Regex(@"\.svc(/|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static IServiceCollection AddWebFormsServiceModel(this IServiceCollection services)
        {
            services.AddServiceModelServices();
            services.AddServiceModelMetadata();
            services.AddServiceModelWebServices(options => { });
            return services;
        }

        public static IApplicationBuilder UseWebFormsServiceModel(this IApplicationBuilder app, string siteRoot = null)
        {
            siteRoot ??= app.ApplicationServices.GetService<IWebHostEnvironment>()?.ContentRootPath ?? Environment.CurrentDirectory;
            ServicePlan plan = ServicePlan.Build(siteRoot, message => Console.WriteLine($"WebFormsForCore: WCF: {message}"));
            if (plan.Services.Any(s => s.Endpoints.Count > 0))
            {
                // enableWebScript's script proxies (Service.svc/js, /jsdebug), before CoreWCF's endpoint at that address takes them.
                var proxies = plan.Services.SelectMany(s => s.Endpoints).Where(e => e.WebScript).ToList();
                if (proxies.Count > 0)
                {
                    var started = DateTime.UtcNow;
                    started = new DateTime(started.Year, started.Month, started.Day, started.Hour, started.Minute, started.Second, DateTimeKind.Utc);
                    app.Use(async (context, next) =>
                    {
                        if (HttpMethods.IsGet(context.Request.Method) && WebScriptProxy(proxies, context.Request.Path.Value ?? "") is { } found)
                        {
                            string path = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{found.Endpoint.Address}";
                            context.Response.ContentType = "application/x-javascript";
                            context.Response.Headers["Cache-Control"] = "public";
                            context.Response.Headers["Last-Modified"] = started.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                            context.Response.Headers["Expires"] = started.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                            await context.Response.WriteAsync(WCFServiceClientProxyGenerator.GetClientProxyScript(found.Endpoint.Contract, path, found.Debug));
                            return;
                        }
                        await next();
                    });
                }
                app.UseServiceModel(builder =>
                {
                    foreach (ServicePlan.Service service in plan.Services.Where(s => s.Endpoints.Count > 0))
                    {
                        builder.AddService(service.Type, options =>
                        {
                            options.DebugBehavior.IncludeExceptionDetailInFaults = service.IncludeExceptionDetailInFaults;
                            options.DebugBehavior.HttpHelpPageEnabled = service.HttpHelpPageEnabled;
                        });
                        foreach (ServicePlan.Endpoint endpoint in service.Endpoints)
                        {
                            var address = new Uri(endpoint.Address, UriKind.Relative);
                            if (endpoint.WebScript)
                            {
                                builder.AddServiceEndpoint(service.Type, endpoint.Contract, endpoint.Binding, address, null,
                                    serviceEndpoint => serviceEndpoint.EndpointBehaviors.Add(new WebScriptEnablingBehavior(app.ApplicationServices)));
                            }
                            else if (endpoint.Web)
                            {
                                builder.AddServiceWebEndpoint(service.Type, endpoint.Contract, (CoreWCF.WebHttpBinding)endpoint.Binding, address, null, behavior =>
                                {
                                    if (endpoint.WebHttp != null)
                                    {
                                        ApplyWebHttp(behavior, endpoint.WebHttp);
                                    }
                                });
                            }
                            else
                            {
                                builder.AddServiceEndpoint(service.Type, endpoint.Contract, endpoint.Binding, address, null);
                            }
                            Console.WriteLine($"WebFormsForCore: WCF: {endpoint.Address} {service.Type.FullName} ({endpoint.Contract.FullName}, {endpoint.Binding.GetType().Name})");
                        }
                    }
                    builder.ConfigureAllServiceHostBase(host => host.Description.Behaviors.Add(new FaultBridge()));
                });
                // WCF published each service's metadata as its serviceMetadata behavior allowed; CoreWCF's is the services'.
                ServiceMetadataBehavior metadata = app.ApplicationServices.GetRequiredService<ServiceMetadataBehavior>();
                metadata.HttpGetEnabled = plan.HttpGetEnabled;
                metadata.HttpsGetEnabled = plan.HttpsGetEnabled;
            }
            // A .svc no service answered (its type not found, its binding not served): not found, rather than the file.
            app.Use(async (context, next) =>
            {
                if (s_svcPath.IsMatch(context.Request.Path.Value ?? ""))
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }
                await next();
            });
            return app;
        }

        // The enableWebScript endpoint whose script proxy a path asks for (its address and /js, or /jsdebug: the debug one).
        private static (ServicePlan.Endpoint Endpoint, bool Debug)? WebScriptProxy(System.Collections.Generic.List<ServicePlan.Endpoint> endpoints, string path)
        {
            foreach (ServicePlan.Endpoint endpoint in endpoints)
            {
                string address = endpoint.Address.TrimEnd('/');
                if (string.Equals(path, address + "/js", StringComparison.OrdinalIgnoreCase))
                {
                    return (endpoint, false);
                }
                if (string.Equals(path, address + "/jsdebug", StringComparison.OrdinalIgnoreCase))
                {
                    return (endpoint, true);
                }
            }
            return null;
        }

        private static void ApplyWebHttp(WebHttpBehavior behavior, System.Xml.Linq.XElement configuration)
        {
            foreach (System.Xml.Linq.XAttribute attribute in configuration.Attributes())
            {
                System.Reflection.PropertyInfo property = typeof(WebHttpBehavior).GetProperty(attribute.Name.LocalName,
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                if (property == null || !property.CanWrite)
                {
                    Console.WriteLine($"WebFormsForCore: WCF: webHttp {attribute.Name.LocalName} is not applied");
                    continue;
                }
                Type type = property.PropertyType;
                property.SetValue(behavior, type.IsEnum ? Enum.Parse(type, attribute.Value, ignoreCase: true) : Convert.ChangeType(attribute.Value, type, System.Globalization.CultureInfo.InvariantCulture));
            }
        }
    }
}
