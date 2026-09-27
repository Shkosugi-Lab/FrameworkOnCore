using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>
/// A repository without a build script of its own (no Cake): its solution, built as Visual Studio builds
/// it, which is how its authors build it. The web project's folder is then the deployed site (post-build
/// events copy other projects' output into it: mojoPortal's features).
/// - Visual Studio (Build Tools) 2022's MSBuild: old-style projects with PackageReference need its NuGet
///   targets, which the .NET SDK does not have. Reported when it is not installed.
/// - The .NET Framework reference assemblies of every version the projects target, from NuGet
///   (Microsoft.NETFramework.ReferenceAssemblies.*), in one root in the cache: a solution mixes them.
/// - nuget.exe for packages.config, in the cache.
/// - The newest C# compiler (the .NET SDK's): sources are written for the one their authors had, and a
///   recent codebase uses C# 14 (mojoPortal 3.1.6: the field keyword), which Visual Studio 2022's has not.
/// - The projects the solution's configuration builds, one by one in the order their project references
///   and the solution's own dependencies give, with BuildingInsideVisualStudio: a project reference then
///   only names the other's output (from the command line, MSBuild builds it again: MSB4006, a cycle in
///   mojoPortal). Those that fail are built once more (DNN's ModulePipeline copies another target's output).
/// - A source file listed twice in a project compiled once, as Visual Studio does (its project system holds a file
///   once; the Visual Basic compiler on the command line defines its types twice: openIMIS's Resource1.designer.vb,
///   BC30179).
/// - The repository's own setup steps after it (--original-step project;target: N2 links its management
///   pages into the templates site).
/// </summary>
public sealed partial class OriginalBuild
{
    static readonly string[] frameworkVersions = ["net45", "net451", "net452", "net46", "net461", "net462", "net47", "net471", "net472", "net48", "net481"];

    bool? RunSolution(string root, string webProject, IReadOnlyList<(string Project, string Target)> steps, string configuration = "Release")
    {
        if (!OperatingSystem.IsWindows())
        {
            report.Add(Report.Kind.Error, "original build", "a .NET Framework solution is built on Windows (Visual Studio's MSBuild): build it there, or give the deployed site (--site)");
            return null;
        }
        var solution = FindSolution(root, webProject);
        if (solution == null)
        {
            report.Add(Report.Kind.Error, "original build", $"no build found: no Cake build, and no solution with {Path.GetFileName(webProject)}; give the deployed site (--site)");
            return null;
        }
        var msbuild = FindMSBuild();
        if (msbuild == null)
        {
            report.Add(Report.Kind.Error, "original build", "Visual Studio (Build Tools) 2022's MSBuild is not installed (old-style projects with PackageReference need its NuGet targets): winget install Microsoft.VisualStudio.2022.BuildTools --override \"--quiet --add Microsoft.VisualStudio.Workload.WebBuildTools --add Microsoft.VisualStudio.Workload.MSBuildTools --includeRecommended\"");
            return false;
        }
        report.Add(Report.Kind.Project, "original build", $"solution: {Path.GetRelativePath(root, solution)} ({configuration}), MSBuild {msbuild}");
        var referenceRoot = EnsureReferenceAssemblies();
        var nuget = EnsureNuGet();
        var solutionDirectory = Path.GetDirectoryName(solution)!;

        foreach (var config in Directory.EnumerateFiles(root, "packages.config", SearchOption.AllDirectories).Where(f => !Regex.IsMatch(f, @"[\\/](node_modules|bin|obj)[\\/]", RegexOptions.IgnoreCase)))
            RunProcess(nuget, $"restore \"{config}\" -PackagesDirectory \"{Path.Combine(solutionDirectory, "packages")}\" -NonInteractive", root, quiet: true);

        var sdk = CaptureProcess("dotnet", "--list-sdks", root).Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).Last();
        var sdkMatch = Regex.Match(sdk, @"^(\S+) \[(.*)\]$");
        var compiler = Path.Combine(sdkMatch.Groups[2].Value, sdkMatch.Groups[1].Value, "Roslyn", "bincore");

        var afterCommon = WriteAfterCommonTargets();
        var order = SolutionOrder(solution, configuration);
        report.Add(Report.Kind.Project, "original build", $"{order.Count} project(s) in the solution configuration, built one by one");
        string Arguments(string project) =>
            $"\"{project}\" /restore /p:Configuration={configuration} /p:Platform=AnyCPU \"/p:SolutionDir={solutionDirectory}\\\\\" " +
            $"\"/p:TargetFrameworkRootPath={referenceRoot}\\\\\" \"/p:CscToolPath={compiler}\" /p:CscToolExe=csc.exe " +
            $"\"/p:CustomAfterMicrosoftCommonTargets={afterCommon}\" /p:BuildingInsideVisualStudio=true /p:ShouldUnsetParentConfigurationAndPlatform=false /m:1 /v:m /nologo";
        var failed = order.Where(p => RunProcess(msbuild, Arguments(p), root, quiet: true) != 0).ToList();
        if (failed.Count > 0) failed = failed.Where(p => RunProcess(msbuild, Arguments(p), root, quiet: true) != 0).ToList();
        foreach (var project in failed) report.Add(Report.Kind.Error, "original build", $"{Path.GetRelativePath(root, project)} did not build (see {log})");

        foreach (var (project, target) in steps)
        {
            report.Add(Report.Kind.Project, "original build", $"setup step: {project} /t:{target}");
            if (RunProcess(msbuild, $"\"{Path.Combine(root, project)}\" /t:{target} /p:Configuration={configuration} \"/p:TargetFrameworkRootPath={referenceRoot}\\\\\" \"/p:CscToolPath={compiler}\" /p:CscToolExe=csc.exe /m:1 /v:m /nologo", root) != 0)
                failed.Add(project);
        }
        return failed.Count == 0;
    }

    // The solution that builds the web project; of several, the one with the most projects (the whole build).
    static string? FindSolution(string root, string webProject) =>
        Directory.EnumerateFiles(root, "*.sln", SearchOption.AllDirectories)
            .Where(f => !Regex.IsMatch(f, @"[\\/](node_modules|packages|bin|obj)[\\/]", RegexOptions.IgnoreCase))
            .Select(f => (File: f, Projects: SolutionProjects(f)))
            .Where(s => s.Projects.Any(p => p.Path.Equals(webProject, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(s => s.Projects.Count).Select(s => s.File).FirstOrDefault();

    static List<(string Name, string Path, string Guid)> SolutionProjects(string solution)
    {
        var directory = Path.GetDirectoryName(solution)!;
        return Regex.Matches(File.ReadAllText(solution), @"Project\(""\{[^}]+\}""\)\s*=\s*""([^""]*)"",\s*""([^""]+\.(?:cs|vb)proj)"",\s*""(\{[^}]+\})""")
            .Select(m => (m.Groups[1].Value, Path.GetFullPath(Path.Combine(directory, m.Groups[2].Value)), m.Groups[3].Value.ToUpperInvariant())).ToList();
    }

    // The projects the configuration builds, in dependency order (project references, the solution's
    // ProjectDependencies: a project may use another's output by a reference to its DLL - DNN's log4net).
    static List<string> SolutionOrder(string solution, string configuration)
    {
        var text = File.ReadAllText(solution);
        var projects = SolutionProjects(solution);
        var platforms = Regex.Matches(text, $@"(?m)^\s*{Regex.Escape(configuration)}\|([^=]+?)\s*=").Select(m => m.Groups[1].Value).Distinct().ToList();
        var platform = new[] { "Any CPU", "Mixed Platforms" }.Concat(platforms).FirstOrDefault(p => platforms.Contains(p)) ?? "Any CPU";
        var built = projects.Where(p => text.Contains($"{p.Guid}.{configuration}|{platform}.Build.0", StringComparison.OrdinalIgnoreCase)).ToList();

        var dependencies = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (Match block in Regex.Matches(text, @"(?s)Project\(""\{[^}]+\}""\)\s*=\s*""[^""]*"",\s*""[^""]+"",\s*""(\{[^}]+\})""(.*?)EndProject\b"))
        {
            var section = Regex.Match(block.Groups[2].Value, @"(?s)ProjectSection\(ProjectDependencies\)(.*?)EndProjectSection");
            if (section.Success) dependencies[block.Groups[1].Value] = Regex.Matches(section.Groups[1].Value, @"(\{[^}]+\})\s*=").Select(m => m.Groups[1].Value).ToList();
        }

        var order = new List<string>();
        var visiting = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        void Visit((string Name, string Path, string Guid) project)
        {
            if (order.Contains(project.Path, StringComparer.OrdinalIgnoreCase) || !visiting.Add(project.Path)) return;
            try
            {
                foreach (var reference in XDocument.Load(project.Path).Descendants().Where(e => e.Name.LocalName == "ProjectReference"))
                {
                    var path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(project.Path)!, (string?)reference.Attribute("Include") ?? ""));
                    var dependency = built.FirstOrDefault(b => b.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
                    if (dependency.Path != null) Visit(dependency);
                }
            }
            catch (System.Xml.XmlException) { }
            foreach (var guid in dependencies.GetValueOrDefault(project.Guid) ?? new List<string>())
            {
                var dependency = built.FirstOrDefault(b => b.Guid.Equals(guid, StringComparison.OrdinalIgnoreCase));
                if (dependency.Path != null) Visit(dependency);
            }
            order.Add(project.Path);
        }
        foreach (var project in built) Visit(project);
        return order;
    }

    // Imported after Microsoft.Common.targets in every project: the Compile items made unique before the compiler runs.
    static string WriteAfterCommonTargets()
    {
        var file = Path.Combine(toolsCache, "original-build.targets");
        Directory.CreateDirectory(toolsCache);
        File.WriteAllText(file, """
            <Project>
              <Target Name="FrameworkOnCoreUniqueCompile" BeforeTargets="CoreCompile">
                <RemoveDuplicates Inputs="@(Compile)">
                  <Output TaskParameter="Filtered" ItemName="_FrameworkOnCoreCompile" />
                </RemoveDuplicates>
                <ItemGroup>
                  <Compile Remove="@(Compile)" />
                  <Compile Include="@(_FrameworkOnCoreCompile)" />
                </ItemGroup>
              </Target>
            </Project>
            """);
        return file;
    }

    static string? FindMSBuild()
    {
        var vswhere = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio", "Installer", "vswhere.exe");
        if (!File.Exists(vswhere)) return null;
        var found = CaptureProcess(vswhere, @"-latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe", Directory.GetCurrentDirectory())
            .Split('\n').Select(l => l.Trim()).FirstOrDefault(l => l.Length > 0);
        return found != null && File.Exists(found) ? found : null;
    }

    // Every .NET Framework version's reference assemblies (NuGet's Microsoft.NETFramework.ReferenceAssemblies)
    // in one root: <cache>\reference-assemblies (with .NETFramework\v4.x under it).
    string EnsureReferenceAssemblies()
    {
        var root = Path.Combine(toolsCache, "reference-assemblies");
        using var http = new HttpClient();
        foreach (var version in frameworkVersions)
        {
            var marker = Path.Combine(root, $".{version}");
            if (File.Exists(marker)) continue;
            var id = $"microsoft.netframework.referenceassemblies.{version}";
            var package = http.GetByteArrayAsync($"https://api.nuget.org/v3-flatcontainer/{id}/1.0.3/{id}.1.0.3.nupkg").GetAwaiter().GetResult();
            using var archive = new ZipArchive(new MemoryStream(package));
            foreach (var entry in archive.Entries.Where(e => e.FullName.StartsWith("build/.NETFramework/", StringComparison.OrdinalIgnoreCase) && e.Name.Length > 0))
            {
                var target = Path.Combine(root, entry.FullName.Substring("build/".Length).Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(target)) continue;
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                entry.ExtractToFile(target);
            }
            File.WriteAllText(marker, "");
        }
        return root;
    }

    string EnsureNuGet()
    {
        var nuget = Path.Combine(toolsCache, "nuget.exe");
        if (!File.Exists(nuget))
        {
            Directory.CreateDirectory(toolsCache);
            using var http = new HttpClient();
            File.WriteAllBytes(nuget, http.GetByteArrayAsync("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe").GetAwaiter().GetResult());
        }
        return nuget;
    }
}
