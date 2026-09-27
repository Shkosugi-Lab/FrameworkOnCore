namespace FrameworkOnCore.Converter;

/// <summary>
/// The application as it runs: the deployed site (the original build's web folder, or the folder the
/// site is deployed to on the IIS server) with its assemblies built from source replaced by their
/// .NET 10 builds. The deployed site is what the application is made of - the markup, configuration and
/// assemblies its build scripts put there (mojoPortal's features, DNN's providers, N2's management
/// pages) - however they did it.
/// </summary>
public static class SiteAssembler
{
    /// <summary>
    /// The deployed site's assemblies built by a C# project of the repository (by assembly name), which
    /// are rebuilt for .NET 10; the others (third-party .NET Framework DLLs) stay as they are.
    /// </summary>
    public static IReadOnlyList<(string Assembly, string Project)> BuiltFromSource(string site, ProjectConverter converter)
    {
        var bin = Path.Combine(site, "bin");
        if (!Directory.Exists(bin)) return Array.Empty<(string, string)>();
        return Directory.EnumerateFiles(bin, "*.dll")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Select(a => (Assembly: a, Project: converter.ProjectOfAssembly(a)))
            .Where(p => p.Project != null)
            .Select(p => (p.Assembly, p.Project!)).ToList();
    }

    public static void Assemble(string deployed, string output, ConvertedProject web, IEnumerable<ConvertedProject> others,
        ProjectConverter converter, Report report, string? cultureProfile)
    {
        if (Directory.Exists(output)) Directory.Delete(output, recursive: true);
        CopyAll(deployed, output, overwrite: true);
        var bin = Path.Combine(output, "bin");
        Directory.CreateDirectory(bin);

        // The web project's build: its assembly, the packages' .NET assemblies, the runtime files.
        var webBin = Path.Combine(Path.GetDirectoryName(web.TargetPath)!, "bin");
        var fromWeb = CopyAll(webBin, bin, overwrite: true);

        // The projects built on their own (not referenced by the web project): their assembly and,
        // where the web project's build did not bring one, their packages' assemblies.
        var fromOthers = 0;
        foreach (var project in others)
        {
            var assembly = (project.AssemblyName ?? project.Name) + ".dll";
            var built = Directory.EnumerateFiles(Path.Combine(Path.GetDirectoryName(project.TargetPath)!, "bin"), assembly, SearchOption.AllDirectories)
                .OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
            if (built == null) { report.Add(Report.Kind.Error, project.Name, $"{assembly} not built: the deployed site's .NET Framework one is left"); continue; }
            var directory = Path.GetDirectoryName(built)!;
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                var name = Path.GetFileName(file);
                var own = Path.GetFileNameWithoutExtension(name).Equals(project.AssemblyName ?? project.Name, StringComparison.OrdinalIgnoreCase);
                var target = Path.Combine(bin, name);
                if (own || !IsFromWebBuild(name))
                {
                    File.Copy(file, target, overwrite: true);
                    fromOthers++;
                }
            }
        }
        bool IsFromWebBuild(string name) => File.Exists(Path.Combine(webBin, name));

        // The web.config rules, on the deployed web.config.
        var webConfig = Directory.EnumerateFiles(output, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
        if (webConfig != null) converter.TransformWebConfig(webConfig, webConfig, "site", null);

        if (cultureProfile != null)
        {
            Directory.CreateDirectory(Path.Combine(output, "App_Data"));
            File.Copy(cultureProfile, Path.Combine(output, "App_Data", "culture-profile.json"), overwrite: true);
        }
        report.Add(Report.Kind.Project, "site", $"assembled from the deployed site: {fromWeb} file(s) from the web project's build, {fromOthers} from the projects built on their own -> {output}");
    }

    static int CopyAll(string from, string to, bool overwrite)
    {
        var count = 0;
        Directory.CreateDirectory(to);
        foreach (var file in Directory.EnumerateFiles(from))
        {
            File.Copy(file, Path.Combine(to, Path.GetFileName(file)), overwrite);
            count++;
        }
        foreach (var directory in Directory.EnumerateDirectories(from))
        {
            count += CopyAll(directory, Path.Combine(to, Path.GetFileName(directory)), overwrite);
        }
        return count;
    }
}
