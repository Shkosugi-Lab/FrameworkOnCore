namespace FrameworkOnCore.Converter;

/// <summary>What becomes of a package a project lists (packages.config, PackageReference).</summary>
public enum PackageFate
{
    /// <summary>Referenced as it is.</summary>
    Kept,
    /// <summary>Its .NET answer instead (replacedPackages): another package, or the same one at another version.</summary>
    Replaced,
    /// <summary>Not referenced: in .NET's box (a System.* 4.x package), or .NET Framework build tooling (droppedPackages).</summary>
    Dropped,
    /// <summary>Not referenced: a shim gives its API (shimPackages).</summary>
    Shim,
    /// <summary>Not referenced: Microsoft's language package of a package a port replaces (languagePackages).</summary>
    LanguagePackage,
}

/// <summary>How a package a project references is referenced.</summary>
public enum PackageAssets
{
    /// <summary>As NuGet gives it.</summary>
    All,
    /// <summary>By its .NET Framework asset (frameworkAssets): restored, not compiled against; a Reference to the asset.</summary>
    FrameworkAsset,
    /// <summary>With nothing of it used (inertPackages): .NET Framework build tooling another package depends on.</summary>
    Inert,
}

/// <summary>The decision on a package a project lists.</summary>
/// <param name="Package">What the project references (Kept, Replaced); null when it references nothing of it.</param>
/// <param name="Companions">Added along with it (packageCompanions).</param>
/// <param name="Shim">Why a shim gives its API (Shim).</param>
/// <param name="RaisedNote">A replacement by the same package at a higher major version: what changed (PackageRaises).</param>
public sealed record PackageDecision(PackageFate Fate, Package? Package, IReadOnlyList<Package> Companions, string? Shim = null, string? RaisedNote = null);

/// <summary>The decision on a .NET Framework assembly a project references by name (no HintPath), web.config names for
/// page compilation, or a DLL references: its package and what .NET Framework's assembly also had (frameworkReferences,
/// frameworkCompanions); none when .NET has it in the box, or has no answer (NoAnswer).</summary>
public sealed record ReferenceDecision(Package? Package, IReadOnlyList<Package> Companions, bool NoAnswer);

/// <summary>
/// The package decisions of rules/packages.json, in one place: old-style projects (written as text) and SDK-style ones
/// (edited as XML), web.config's assemblies and the DLLs' references get the same; only how they are written differs.
/// </summary>
public static class PackageDecisions
{
    /// <summary>A package a project lists (packages.config, PackageReference); version "*" when it has none.</summary>
    public static PackageDecision Listed(this Rules rules, string id, string version)
    {
        if (rules.LanguagePackageOf(id) != null) return new(PackageFate.LanguagePackage, null, []);
        if (rules.ShimPackages.TryGetValue(id, out var shim)) return new(PackageFate.Shim, null, [], shim);
        if (rules.IsDropped(id, version)) return new(PackageFate.Dropped, null, []);
        var companions = rules.PackageCompanions.TryGetValue(id, out var listed) ? listed : [];
        if (!rules.ReplacedPackages.TryGetValue(id, out var replaced)) return new(PackageFate.Kept, new Package(id, version), companions);
        // Not from a version not given (a PackageReference without one): nothing is known to have been raised.
        var raised = replaced.Id.Equals(id, StringComparison.OrdinalIgnoreCase) && version != "*" ? rules.Raises.Note(id, version, replaced.Version) : null;
        return new(PackageFate.Replaced, replaced, companions, RaisedNote: raised);
    }

    /// <summary>A DLL checked into the repository (a HintPath outside NuGet's packages) of a package that has a .NET answer
    /// (nopCommerce's AjaxControlToolkit 4.1): that package, as for a package reference; null otherwise.</summary>
    public static PackageDecision? InRepository(this Rules rules, string assembly) =>
        rules.ReplacedPackages.TryGetValue(assembly, out var replaced)
            ? new(PackageFate.Replaced, replaced, rules.PackageCompanions.TryGetValue(assembly, out var companions) ? companions : [])
            : null;

    /// <summary>A .NET Framework assembly referenced by name.</summary>
    public static ReferenceDecision Reference(this Rules rules, string assembly) =>
        rules.FrameworkReferences.TryGetValue(assembly, out var package)
            ? new(package, rules.FrameworkCompanions.TryGetValue(assembly, out var companions) ? companions : [], false)
            : new(null, [], rules.NoAnswer.Contains(assembly));

    /// <summary>How a package a project references is referenced.</summary>
    public static PackageAssets AssetsOf(this Rules rules, string id) =>
        rules.FrameworkAssets.ContainsKey(id) ? PackageAssets.FrameworkAsset
        : rules.InertPackages.Contains(id) ? PackageAssets.Inert
        : PackageAssets.All;
}
