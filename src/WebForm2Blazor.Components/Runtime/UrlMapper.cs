namespace WebForm2Blazor.Components;

/// <summary>
/// Maps WebForms physical-path URLs to Blazor routes.
/// The converter generates @page routes with the same rule (path minus extension), so
/// Response.Redirect("~/Edit.aspx?id=3") and NavigateUrl="Default.aspx" resolve correctly
/// without touching the original code.
/// </summary>
public static class UrlMapper
{
    private static readonly string[] PageExtensions = [".aspx", ".ascx", ".ashx", ".asmx"];

    public static string ResolveUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return url;
        }

        // Absolute URLs, protocol-relative URLs and anchors pass through untouched
        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("//", StringComparison.Ordinal)
            || url.StartsWith("#", StringComparison.Ordinal)
            || url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        var resolved = url;

        // "~/Foo.aspx" -> "/Foo.aspx" (app-root relative)
        if (resolved.StartsWith("~/", StringComparison.Ordinal))
        {
            resolved = resolved[1..];
        }
        else if (!resolved.StartsWith("/", StringComparison.Ordinal))
        {
            resolved = "/" + resolved;
        }

        // Separate the query/fragment before stripping the extension from the path part
        var separatorIndex = resolved.IndexOfAny(['?', '#']);
        var path = separatorIndex >= 0 ? resolved[..separatorIndex] : resolved;
        var suffix = separatorIndex >= 0 ? resolved[separatorIndex..] : string.Empty;

        foreach (var extension in PageExtensions)
        {
            if (path.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                path = path[..^extension.Length];
                break;
            }
        }

        return path + suffix;
    }
}
