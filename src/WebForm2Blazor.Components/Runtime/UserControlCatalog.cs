using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>
/// Virtual path -> converted component type, populated by the generated app at startup
/// (UserControlCatalog.g.cs) and read by LoadControl.
///
/// WebForms compiled a .ascx on demand, so LoadControl("~/Custom/PostView.ascx") returned
/// a live control. Here the .ascx became a component at conversion time; the only thing
/// missing was the map from the path the code-behind still names to the type it became,
/// and only the converter knows both ends of that.
///
/// An unregistered path resolves to null, exactly as LoadControl did for a path that did
/// not exist - a control invented for an unknown path would render something the original
/// never had.
/// </summary>
public static class UserControlCatalog
{
    private static readonly Dictionary<string, Type> ByPath = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Registers one converted user control. Called by generated startup code.</summary>
    public static void Register(string virtualPath, Type componentType)
    {
        if (!string.IsNullOrEmpty(virtualPath) && componentType is not null)
        {
            ByPath[Normalize(virtualPath)] = componentType;
        }
    }

    /// <summary>The component a virtual path was converted into, or null when unknown.</summary>
    public static Type Resolve(string virtualPath)
        => string.IsNullOrEmpty(virtualPath)
            ? null
            : ByPath.GetValueOrDefault(Normalize(virtualPath));

    /// <summary>
    /// Creates the component for a virtual path, or null when the path is not one of the
    /// converted controls.
    /// </summary>
    public static IWebFormsControl Create(string virtualPath)
        => Resolve(virtualPath) is { } type && Activator.CreateInstance(type) is IWebFormsControl control
            ? control
            : null;

    /// <summary>
    /// Path spellings are normalised to one form. A code-behind writes "~/A/B.ascx",
    /// "/A/B.ascx" or "A/B.ascx" for the same file, and may build the string at runtime
    /// (BlogEngine assembles the theme path from settings), so matching has to be
    /// independent of the leading form and of separator and case.
    /// </summary>
    private static string Normalize(string virtualPath)
        => virtualPath.Replace('\\', '/').TrimStart('~').TrimStart('/');
}

/// <summary>
/// Virtual path -> converted LAYOUT type, populated by the generated app at startup
/// (MasterPageCatalog.g.cs) and read when a page assigns Page.MasterPageFile.
///
/// A .master is a layout here, and which one a page uses is usually written in its @Page
/// directive - so the converter binds it at conversion time and this map is not consulted.
/// It exists for the pages that choose at RUNTIME:
///
///     // BlogEngine.Core\Web\Controls\BlogBasePage.cs
///     MasterPageFile = $"~/Custom/Themes/{BlogSettings.Instance.Theme}/site.master";
///
/// There is nothing in the markup to bind, and the string is only known once settings are
/// read. MasterPageFile used to be an inert property - assigned, stored, never looked at -
/// so BlogEngine rendered with no theme at all: no header, no menu, no site title. It
/// built with zero errors while doing it, because nothing about it is a compile error.
/// </summary>
public static class MasterPageCatalog
{
    private static readonly Dictionary<string, Type> ByPath = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Registers one converted master page. Called by generated startup code.</summary>
    public static void Register(string virtualPath, Type layoutType)
    {
        if (!string.IsNullOrEmpty(virtualPath) && layoutType is not null)
        {
            ByPath[Normalize(virtualPath)] = layoutType;
        }
    }

    /// <summary>
    /// The layout a master's virtual path was converted into, or null when unknown.
    ///
    /// Null rather than a guess: an unknown path means the master was not converted, and
    /// rendering the page with SOME other theme would be a difference invented here rather
    /// than one carried over.
    /// </summary>
    public static Type Resolve(string virtualPath)
        => string.IsNullOrEmpty(virtualPath)
            ? null
            : ByPath.GetValueOrDefault(Normalize(virtualPath));

    public static int Count => ByPath.Count;

    private static string Normalize(string virtualPath)
        => virtualPath.Replace('\\', '/').TrimStart('~').TrimStart('/');
}
