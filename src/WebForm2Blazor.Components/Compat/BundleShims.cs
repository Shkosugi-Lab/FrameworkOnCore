using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>
/// System.Web.Optimization.Scripts / Styles equivalents - the ASP.NET bundling helpers a
/// master page calls from markup:
///
///     &lt;%: Scripts.Render("~/bundles/modernizr") %&gt;
///
/// EMIT NOTHING, deliberately. Bundling is a different framework and is already reported
/// as out of scope three times over (BundleConfig.cs, Global.asax.cs, and the skipped
/// NuGet references), so the bundle DEFINITIONS never came across - there is no
/// "~/bundles/modernizr" in the converted app to point a &lt;script&gt; at. Emitting the tag
/// anyway would produce a 404 on every page.
///
/// What this changes is only that the call compiles. Without it the markup - which the
/// converter does carry over - fails with CS0103 and takes the whole application's build
/// with it, which is how WingtipToys' single remaining build error arose.
///
/// Migrating the bundles themselves means replacing them with the ASP.NET Core equivalent
/// (a bundler at build time, or plain static files); that is the same "port it separately"
/// answer the report already gives for BundleConfig.
/// </summary>
public static class Scripts
{
    /// <summary>WebForms Scripts.Render equivalent. Renders nothing (see the class remarks).</summary>
    public static MarkupString Render(params string[] paths)
    {
        _ = paths;
        return default;
    }

    /// <summary>WebForms Scripts.RenderFormat equivalent. Renders nothing.</summary>
    public static MarkupString RenderFormat(string tagFormat, params string[] paths)
    {
        _ = tagFormat;
        _ = paths;
        return default;
    }

    /// <summary>
    /// WebForms Scripts.Url equivalent. Returns the virtual path unchanged rather than a
    /// bundle URL: a page that builds a link from it still points at something, and the
    /// path it names is the one the source asked for.
    /// </summary>
    public static MarkupString Url(string virtualPath)
        => (MarkupString)UrlMapper.ResolveUrl(virtualPath ?? string.Empty);
}

/// <summary>System.Web.Optimization.Styles equivalent. See <see cref="Scripts"/>.</summary>
public static class Styles
{
    /// <summary>WebForms Styles.Render equivalent. Renders nothing (see Scripts).</summary>
    public static MarkupString Render(params string[] paths)
    {
        _ = paths;
        return default;
    }

    /// <summary>WebForms Styles.RenderFormat equivalent. Renders nothing.</summary>
    public static MarkupString RenderFormat(string tagFormat, params string[] paths)
    {
        _ = tagFormat;
        _ = paths;
        return default;
    }

    /// <inheritdoc cref="Scripts.Url"/>
    public static MarkupString Url(string virtualPath)
        => (MarkupString)UrlMapper.ResolveUrl(virtualPath ?? string.Empty);
}
