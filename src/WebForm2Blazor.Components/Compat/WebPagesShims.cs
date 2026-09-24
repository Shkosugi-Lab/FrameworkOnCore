using System.Diagnostics;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;


namespace System.Web.WebPages
{
    /// <summary>
    /// ASP.NET Web Pages' WebPage: the base of a .cshtml template an application renders
    /// itself (BlogEngine's RazorHelpers.ParseRazor: BuildManager.GetCompiledType, then
    /// ExecutePageHierarchy into a StringWriter).
    ///
    /// The template is compiled at BUILD time by the Razor SDK as an ASP.NET Core view whose
    /// base is this class (the converter writes the "@inherits" into a _ViewImports.cshtml
    /// beside it). What the original compiled on first request, this compiles with the
    /// application - the same template, found by the same virtual path
    /// (<see cref="WebForm2Blazor.Components.BuildManager.GetCompiledType"/>).
    ///
    /// Only the Web Pages surface the templates in the corpora use is here: Context,
    /// Request, Response, Server, IsPost, Href, VirtualPath and the dynamic Model. A layout
    /// (Layout = ...) and RenderPage are not: a template that uses them fails to COMPILE,
    /// which the build gate reports, rather than rendering something else.
    /// </summary>
    public abstract class WebPage : WebPageRenderingBase
    {
        /// <summary>The virtual path the page was loaded from ("~/Custom/Widgets/Search/widget.cshtml").</summary>
        public virtual string VirtualPath { get; set; }

        /// <summary>What <see cref="ExecutePageHierarchy"/> was handed.</summary>
        public WebPageContext PageContext { get; private set; }

        /// <summary>
        /// Web Pages' Context is the HttpContextBase of the request (Request.QueryString,
        /// Request.Form, Server...), not ASP.NET Core's HttpContext - hidden here so the
        /// template's "Context.Request.QueryString[...]" means what it meant.
        /// </summary>
        public new WebForm2Blazor.Components.HttpContextBase Context
            => PageContext?.HttpContext ?? new WebForm2Blazor.Components.HttpContextWrapper(WebForm2Blazor.Components.HttpContext.Current);

        public WebForm2Blazor.Components.HttpRequestShim Request => Context.Request;

        public WebForm2Blazor.Components.HttpResponseShim Response => Context.Response;

        public WebForm2Blazor.Components.ServerUtilityShim Server => Context.Server;

        public bool IsPost => string.Equals(Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase);

        /// <summary>WebPages Href: an application-relative path made absolute ("~/a" -> "/a").</summary>
        public string Href(string path, params object[] pathParts)
        {
            var resolved = WebForm2Blazor.Components.VirtualPathUtility.ToAbsolute(path ?? "~/");
            return pathParts is { Length: > 0 }
                ? resolved.TrimEnd('/') + "/" + string.Join("/", pathParts.Select(part => Uri.EscapeDataString(Convert.ToString(part) ?? string.Empty)))
                : resolved;
        }

        /// <summary>
        /// Renders the page into <paramref name="writer"/> with <see cref="WebPageContext.Model"/>
        /// as its dynamic Model. <paramref name="startPage"/> is accepted for the original
        /// signature; a start page (_PageStart) is not run.
        ///
        /// Synchronous, as the original was: its callers build a string for a Literal.
        /// </summary>
        public void ExecutePageHierarchy(WebPageContext pageContext, TextWriter writer, WebPageRenderingBase startPage)
        {
            PageContext = pageContext ?? throw new ArgumentNullException(nameof(pageContext));
            WebPageRenderer.Render(this, pageContext.Model, writer);
        }

        /// <inheritdoc cref="ExecutePageHierarchy(WebPageContext, TextWriter, WebPageRenderingBase)"/>
        public void ExecutePageHierarchy(WebPageContext pageContext, TextWriter writer)
            => ExecutePageHierarchy(pageContext, writer, null);
    }

    /// <summary>
    /// Web Pages' WebPageRenderingBase, the base WebPage derives from and the type
    /// ExecutePageHierarchy takes its start page as. Here it is where the page becomes an
    /// ASP.NET Core view.
    /// </summary>
    public abstract class WebPageRenderingBase : RazorPage<dynamic>
    {
    }

    /// <summary>Web Pages' WebPageContext: the request, the page and the model it renders.</summary>
    public class WebPageContext
    {
        public WebPageContext(WebForm2Blazor.Components.HttpContextBase context, WebPage page, object model)
        {
            HttpContext = context;
            Page = page;
            Model = model;
        }

        public WebForm2Blazor.Components.HttpContextBase HttpContext { get; }

        public WebPage Page { get; }

        public object Model { get; }
    }

    /// <summary>
    /// Renders a <see cref="WebPage"/> as the ASP.NET Core view it was compiled into.
    /// Needs the view services (<see cref="WebPagesServiceCollectionExtensions.AddWebPagesTemplates"/>).
    /// </summary>
    internal static class WebPageRenderer
    {
        public static void Render(WebPage page, object model, TextWriter writer)
        {
            var root = WebForm2Blazor.Components.HttpContext.Services
                       ?? throw new InvalidOperationException(
                           "Web Pages テンプレートの描画にはサービスが要ります(UseWebFormsSession の後で呼んでください)。");

            // During an interactive Blazor render there is no live request; a scope stands
            // in for it, because the view helpers the template injects are scoped.
            var live = root.GetService<IHttpContextAccessor>()?.HttpContext;
            using var scope = live is null ? root.CreateScope() : null;
            var httpContext = live ?? new DefaultHttpContext { RequestServices = scope!.ServiceProvider };
            var services = httpContext.RequestServices;

            var actionContext = new ActionContext(
                httpContext, httpContext.GetRouteData() ?? new RouteData(), new ActionDescriptor());
            var viewData = new ViewDataDictionary<object>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model,
            };
            var tempData = new TempDataDictionary(httpContext, NullTempDataProvider.Instance);

            var view = new RazorView(
                services.GetRequiredService<IRazorViewEngine>(),
                services.GetRequiredService<IRazorPageActivator>(),
                Array.Empty<IRazorPage>(),
                page,
                HtmlEncoder.Default,
                services.GetService<DiagnosticListener>() ?? new DiagnosticListener("WebForm2Blazor.WebPages"));

            var viewContext = new ViewContext(actionContext, view, viewData, tempData, writer, new HtmlHelperOptions());

            // Off the caller's synchronization context. The caller is usually a Blazor
            // render, on the renderer's dispatcher; a template that awaits something truly
            // asynchronous would post its continuation back to that dispatcher while it is
            // blocked here. The corpora's templates complete synchronously - this is a guard,
            // not a fix for something measured.
            Task.Run(() => view.RenderAsync(viewContext)).GetAwaiter().GetResult();
        }

        /// <summary>A template has no TempData to keep; nothing is loaded or saved.</summary>
        private sealed class NullTempDataProvider : ITempDataProvider
        {
            public static readonly NullTempDataProvider Instance = new();

            public IDictionary<string, object> LoadTempData(Microsoft.AspNetCore.Http.HttpContext context)
                => new Dictionary<string, object>();

            public void SaveTempData(Microsoft.AspNetCore.Http.HttpContext context, IDictionary<string, object> values)
            {
            }
        }
    }

    public static class WebPagesServiceCollectionExtensions
    {
        /// <summary>
        /// The view services a Web Pages template renders with. Called from the Program.cs
        /// the converter generates when it compiled templates.
        /// </summary>
        public static IServiceCollection AddWebPagesTemplates(this IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddTransient<Html.HtmlHelper>();
            return services;
        }
    }
}

namespace System.Web.WebPages.Html
{
    /// <summary>
    /// Web Pages' HtmlHelper - the "Html" of a template (Html.Raw), and the type an
    /// application's own helpers extend (BlogEngine's RazorHelpers: Html.PageBody(),
    /// Html.RenderWidgetZone(...)). Injected into the template in place of the MVC helper,
    /// so those extensions bind as they did.
    /// </summary>
    public class HtmlHelper
    {
        public IHtmlContent Raw(string value) => new HtmlString(value);

        public IHtmlContent Raw(object value) => new HtmlString(Convert.ToString(value));

        public string Encode(string value) => HtmlEncoder.Default.Encode(value ?? string.Empty);

        public string Encode(object value) => Encode(Convert.ToString(value));

        public string AttributeEncode(string value) => HtmlEncoder.Default.Encode(value ?? string.Empty);

        public string AttributeEncode(object value) => AttributeEncode(Convert.ToString(value));
    }
}
