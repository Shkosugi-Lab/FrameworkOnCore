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
