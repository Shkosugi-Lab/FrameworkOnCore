using System.Collections.Concurrent;
using System.Reflection;
using System.Resources;

namespace WebForm2Blazor.Components;

/// <summary>
/// WebForms App_GlobalResources equivalent (&lt;%$ Resources: ClassKey, Key %&gt;).
///
/// The converter copies the .resx files into the generated project, where the SDK embeds
/// them automatically. Their manifest names depend on the project's root namespace and
/// folder layout, so instead of requiring configuration the lookup scans the loaded
/// assemblies for a resource set whose name ends with the class key.
/// </summary>
public static class GlobalResources
{
    private static readonly ConcurrentDictionary<string, ResourceManager?> ManagersByClassKey = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Resolves a global resource string. An unresolved key returns the key itself rather
    /// than an empty string: a missing translation should be visible on the page, not
    /// silently blank (WebForms failed at compile time, which has no equivalent here).
    /// </summary>
    public static string GetString(string classKey, string resourceKey)
    {
        if (string.IsNullOrEmpty(resourceKey))
        {
            return string.Empty;
        }

        var manager = ManagersByClassKey.GetOrAdd(classKey ?? string.Empty, CreateManager);
        if (manager is null)
        {
            return resourceKey;
        }

        try
        {
            return manager.GetString(resourceKey) ?? resourceKey;
        }
        catch (MissingManifestResourceException)
        {
            return resourceKey;
        }
    }

    /// <summary>WebForms HttpContext.GetGlobalResourceObject equivalent.</summary>
    public static object GetObject(string classKey, string resourceKey)
        => GetString(classKey, resourceKey);

    private static ResourceManager? CreateManager(string classKey)
    {
        if (string.IsNullOrEmpty(classKey))
        {
            return null;
        }

        var suffix = "." + classKey + ".resources";
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
            {
                continue;
            }

            string[] names;
            try
            {
                names = assembly.GetManifestResourceNames();
            }
            catch (NotSupportedException)
            {
                continue;
            }

            var match = names.FirstOrDefault(name =>
                name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
            if (match is not null)
            {
                // ResourceManager wants the base name, i.e. the manifest name without ".resources"
                return new ResourceManager(match[..^".resources".Length], assembly);
            }
        }
        return null;
    }
}
