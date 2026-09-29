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
        var rebuilt = new HashSet<string>(Directory.EnumerateFiles(webBin).Select(f => Path.GetFileName(f)!), StringComparer.OrdinalIgnoreCase);

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
                    rebuilt.Add(name);
                    fromOthers++;
                }
            }
        }
        bool IsFromWebBuild(string name) => File.Exists(Path.Combine(webBin, name));

        RemoveFrameworkNatives(bin, webBin, report);
        ReplaceInPackages(output, bin, rebuilt, report);

        // The web.config rules, on the deployed web.config and the folders' (not bin's).
        foreach (var webConfig in Directory.EnumerateFiles(output, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = true })
                     .Where(c => !System.Text.RegularExpressions.Regex.IsMatch(Path.GetRelativePath(output, c), @"^bin[\\/]", System.Text.RegularExpressions.RegexOptions.IgnoreCase)))
            converter.TransformWebConfig(webConfig, webConfig, Path.GetDirectoryName(Path.GetRelativePath(output, webConfig)) is { Length: > 0 } folder ? $"site {folder}" : "site", null);

        if (cultureProfile != null)
        {
            Directory.CreateDirectory(Path.Combine(output, "App_Data"));
            File.Copy(cultureProfile, Path.Combine(output, "App_Data", "culture-profile.json"), overwrite: true);
        }
        report.Add(Report.Kind.Project, "site", $"assembled from the deployed site: {fromWeb} file(s) from the web project's build, {fromOthers} from the projects built on their own -> {output}");
    }

    /// <summary>
    /// The native libraries the .NET build brings in runtimes\&lt;rid&gt;\native (System.Data.SQLite's
    /// SQLite.Interop.dll), which the deployed site has in .NET Framework's layout (bin\x86, bin\x64: the
    /// package's build targets put them there): those are the old versions, and a library that looks there
    /// first would load one. Removed.
    /// </summary>
    static void RemoveFrameworkNatives(string bin, string webBin, Report report)
    {
        var runtimes = Path.Combine(webBin, "runtimes");
        if (!Directory.Exists(runtimes)) return;
        var natives = Directory.EnumerateFiles(runtimes, "*", SearchOption.AllDirectories)
            .Where(f => Path.GetFileName(Path.GetDirectoryName(f)!).Equals("native", StringComparison.OrdinalIgnoreCase))
            .Select(f => Path.GetFileName(f)!).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var removed = new List<string>();
        foreach (var file in Directory.EnumerateFiles(bin, "*", SearchOption.AllDirectories).ToList())
        {
            var relative = Path.GetRelativePath(bin, file);
            if (relative.StartsWith("runtimes" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                !natives.Contains(Path.GetFileName(file)) || File.Exists(Path.Combine(webBin, relative))) continue;
            File.Delete(file);
            removed.Add(relative);
        }
        if (removed.Count > 0)
            report.Add(Report.Kind.Project, "site", $"the deployed site's .NET Framework native libraries removed ({string.Join(", ", removed)}): the .NET build has them in runtimes");
    }

    /// <summary>
    /// The packages the site installs from itself (DNN: Install\Module\*.zip, Install\*\*.resources, the
    /// modules and providers its install wizard puts in bin) carry the .NET Framework builds of the
    /// assemblies: installed, they would replace the .NET ones in bin. A DLL in an archive of the site
    /// (a zip, by its content, whatever its extension) that the build replaced in bin is replaced there
    /// too.
    /// </summary>
    static void ReplaceInPackages(string site, string bin, IReadOnlySet<string> rebuilt, Report report)
    {
        var packages = 0;
        var replaced = 0;
        foreach (var file in Directory.EnumerateFiles(site, "*", SearchOption.AllDirectories))
        {
            if (file.StartsWith(bin + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || !IsZip(file)) continue;
            var count = 0;
            try
            {
                using var archive = System.IO.Compression.ZipFile.Open(file, System.IO.Compression.ZipArchiveMode.Update);
                foreach (var entry in archive.Entries.ToList())
                {
                    if (!entry.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || !rebuilt.Contains(entry.Name)) continue;
                    var name = entry.FullName;
                    entry.Delete();
                    System.IO.Compression.ZipFileExtensions.CreateEntryFromFile(archive, Path.Combine(bin, Path.GetFileName(name)), name);
                    count++;
                }
            }
            catch (InvalidDataException) { continue; }
            if (count == 0) continue;
            packages++;
            replaced += count;
            report.Add(Report.Kind.Stub, "site", $"{Path.GetRelativePath(site, file)}: {count} assembly(ies) replaced by their .NET builds");
        }
        if (packages > 0) report.Add(Report.Kind.Project, "site", $"packages in the site: {replaced} assembly(ies) in {packages} archive(s) replaced by their .NET builds (installed into bin, they would bring back the .NET Framework ones)");
    }

    static bool IsZip(string file)
    {
        Span<byte> head = stackalloc byte[4];
        using var stream = File.OpenRead(file);
        return stream.Read(head) == 4 && head[0] == (byte)'P' && head[1] == (byte)'K' && head[2] == 3 && head[3] == 4;
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
