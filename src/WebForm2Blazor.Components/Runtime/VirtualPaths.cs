namespace WebForm2Blazor.Components;

/// <summary>
/// Turns a WebForms virtual path ("~/Custom/Widgets/Search/widget.cshtml") into a path on
/// disk.
///
/// WebForms had ONE root: the application directory was both what the browser saw and what
/// MapPath resolved against. An ASP.NET Core app has two - wwwroot is browser-visible, the
/// content root holds everything else - and the converter puts the original's static
/// content under wwwroot, because that is the half the original root corresponded to.
///
/// So a virtual path is resolved by looking, in order, at:
///
///   1. wwwroot          - where the converted static content went
///   2. the content root - where App_Data and anything else generated lives
///   3. AppContext.BaseDirectory - where the build copies content to
///
/// and the first one that EXISTS wins. Probing rather than picking: the same application
/// reads its widget templates (now under wwwroot) and its data store (App_Data, not under
/// wwwroot) through the same call, so no single root answers both.
///
/// When nothing exists the wwwroot path is returned, because a caller that is about to
/// CREATE a file is asking where it should go, and that is the browser-visible root the
/// original would have written into.
/// </summary>
public static class VirtualPaths
{
    /// <summary>Set once at startup by AddWebFormsCompat; null outside a hosted app.</summary>
    internal static string ContentRoot { get; set; }

    public static string Resolve(string virtualPath)
    {
        var relative = (virtualPath ?? string.Empty)
            .TrimStart('~')
            .TrimStart('/', '\\')
            .Replace('/', Path.DirectorySeparatorChar);

        var contentRoot = ContentRoot ?? Directory.GetCurrentDirectory();
        var candidates = new[]
        {
            Path.Combine(contentRoot, "wwwroot", relative),
            Path.Combine(contentRoot, relative),
            Path.Combine(AppContext.BaseDirectory, "wwwroot", relative),
            Path.Combine(AppContext.BaseDirectory, relative),
        };

        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate) || Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        return candidates[0];
    }
}
