using System.Text;
using System.Text.RegularExpressions;
using FrameworkOnCore.Converter;

// FrameworkOnCore: converts a .NET Framework web application (Web Forms, C#) to .NET 10 on
// WebFormsForCore, to run on Linux. See LINUX-CONVERTER-DESIGN.md.
//
//   FrameworkOnCore.Converter <web project .csproj> --out <dir> [--root <dir>] [--runtime <dir>]
//                             [--culture-profile <file>] [--no-build]
//
// --root     the repository to copy (default: the topmost folder above the project with a .sln)
// --runtime  where the WebFormsForCore fork's feed (_feed) and the shims (shims) are
//            (default: experiments/wf4c above the current folder)
// --culture-profile  the original server's culture data (capture-culture.ps1), placed in App_Data
// --site     the deployed site (the original build's web folder, or the site's folder on the IIS
//            server): what the application is made of. Its assemblies built from the repository are
//            rebuilt for .NET 10 and the site is assembled in <out>\site (run bin\<web>.dll there).
// --build-original [target]  get the deployed site by building the repository with its own build
//            (a Cake build: Cake Frosting or a .cake script; its default target, or the one given)
//            in a copy (<out>.original); the site it deploys is then --site. Without one, the solution
//            with the web project, as Visual Studio builds it (Windows, Visual Studio's MSBuild).
// --original-step <project;target>  a setup step of the repository after its build (repeatable).
// --deploy   how it is deployed on Linux: container (Dockerfile), linux (deploy/linux/install.sh, systemd),
//            both (the default) or none. See DeployWriter.

string? project = null, outDirectory = null, rootDirectory = null, runtimeDirectory = null, cultureProfile = null, site = null, originalTarget = null;
var build = true;
var buildOriginal = false;
var deployKinds = "both";
var originalSteps = new List<(string Project, string Target)>();
for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--out": outDirectory = args[++i]; break;
        case "--root": rootDirectory = args[++i]; break;
        case "--runtime": runtimeDirectory = args[++i]; break;
        case "--culture-profile": cultureProfile = args[++i]; break;
        case "--site": site = Path.GetFullPath(args[++i]); break;
        case "--no-build": build = false; break;
        case "--deploy": deployKinds = args[++i]; break;
        case "--original-step":
            var step = args[++i].Split(';', 2);
            originalSteps.Add((step[0], step.Length > 1 ? step[1] : "Build"));
            break;
        case "--build-original":
            buildOriginal = true;
            if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal) && !args[i + 1].EndsWith("proj", StringComparison.OrdinalIgnoreCase)) originalTarget = args[++i];
            break;
        default: project = args[i]; break;
    }
}
if (project == null || outDirectory == null)
{
    Console.Error.WriteLine("usage: FrameworkOnCore.Converter <web project .csproj> --out <dir> [--root <dir>] [--runtime <dir>] [--culture-profile <file>] [--site <dir> | --build-original [target]] [--deploy container|linux|both|none] [--no-build]");
    return 2;
}

project = Path.GetFullPath(project);
var sourceRoot = Path.GetFullPath(rootDirectory ?? Paths.FindRoot(project));
var outRoot = Path.GetFullPath(outDirectory);
runtimeDirectory = Path.GetFullPath(runtimeDirectory ?? FindRuntime() ?? throw new InvalidOperationException("--runtime: experiments/wf4c not found"));
var runtime = new RuntimeLayout(
    Path.Combine(runtimeDirectory, "_feed"),
    Directory.Exists(Path.Combine(runtimeDirectory, "shims"))
        ? Directory.GetFiles(Path.Combine(runtimeDirectory, "shims"), "*.csproj", SearchOption.AllDirectories)
            .Where(f => !Regex.IsMatch(f, @"[\\/](bin|obj)[\\/]")).ToList()
        : new List<string>());

var report = new Report();
var rules = Rules.Load(Path.Combine(AppContext.BaseDirectory, "rules", "packages.json"));
if (buildOriginal)
{
    var originalWork = outRoot.TrimEnd('\\', '/') + ".original";
    var originalLog = originalWork + ".build.log";
    File.Delete(originalLog);
    site = new OriginalBuild(report, originalLog).Run(sourceRoot, originalWork, project, originalTarget, originalSteps);
    if (site == null)
    {
        File.WriteAllText(Path.Combine(Directory.CreateDirectory(outRoot).FullName, "CONVERSION-REPORT.md"), report.ToMarkdown($"FrameworkOnCore: {Path.GetFileName(project)}"), new UTF8Encoding(false));
        Console.WriteLine($"original build FAILED; log: {originalLog}");
        return 1;
    }
}
Console.WriteLine($"copying {sourceRoot} -> {outRoot}");
Paths.CopyTree(sourceRoot, outRoot);

var converter = new ProjectConverter(rules, report, new Conditions("Debug", "AnyCPU", report), sourceRoot, outRoot, runtime);
if (site != null) converter.DeployedBin = Path.Combine(site, "bin");
var web = converter.Convert(project, isWeb: true);
WriteHost(web);

// With the deployed site: the assemblies in its bin built from the repository's projects are part of
// the application, referenced by the web project or not (mojoPortal's features reference the web
// project; its build scripts put them in the site).
var others = new List<ConvertedProject>();
if (site != null)
{
    foreach (var (assembly, member) in SiteAssembler.BuiltFromSource(site, converter))
    {
        if (!member.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
        {
            report.Add(Report.Kind.Unsupported, assembly, $"in the deployed site, built by {Path.GetFileName(member)}: not converted (only C# projects so far); the .NET Framework assembly is left");
            continue;
        }
        if (converter.Converted.Any(c => c.SourcePath.Equals(member, StringComparison.OrdinalIgnoreCase))) continue;
        converter.CopyLocalProjects.Add(member);
        others.Add(converter.Convert(member, isWeb: false));
        report.Add(Report.Kind.Project, assembly, $"in the deployed site, built by {Path.GetFileName(member)} which the web project does not reference: converted and built on its own");
    }
}

// Built as one solution: the web project and every converted project.
var buildTarget = Path.Combine(outRoot, "FrameworkOnCore.slnx");
File.WriteAllText(buildTarget,
    "<Solution>\n" + string.Concat(converter.Converted.Select(c => $"  <Project Path=\"{System.Security.SecurityElement.Escape(Path.GetRelativePath(outRoot, c.TargetPath).Replace('\\', '/'))}\" />\n")) + "</Solution>\n",
    new UTF8Encoding(false));

var succeeded = true;
if (build) succeeded = new BuildFixer(report, converter.Converted, outRoot, rules).Run(buildTarget);
if (build && succeeded && site != null)
{
    // The ones built on their own, as they are after the build (the web project's are in its bin).
    var built = others.Select(o => converter.Converted.First(c => c.SourcePath == o.SourcePath)).ToList();
    SiteAssembler.Assemble(site, Path.Combine(outRoot, "site"), web, built, converter, report, cultureProfile);
}
// How it is deployed on Linux: the assembled site, or the web project's folder (built in place).
if (build && succeeded)
{
    var deployedSite = site != null ? Path.Combine(outRoot, "site") : Path.GetDirectoryName(web.TargetPath)!;
    new DeployWriter(report, outRoot, runtimeDirectory).Write(deployKinds, deployedSite, web, cultureProfile);
}

var reportPath = Path.Combine(outRoot, "CONVERSION-REPORT.md");
File.WriteAllText(reportPath, report.ToMarkdown($"FrameworkOnCore: {Path.GetFileName(project)}"), new UTF8Encoding(false));
Console.WriteLine($"{(build ? (succeeded ? "build succeeded" : "build FAILED") : "not built")}; web project: {web.TargetPath}; report: {reportPath}");
return succeeded ? 0 : 1;

// Program.cs of the web project, and the culture profile. An application that routes
// (System.Web.Routing, FriendlyUrls) serves extensionless URLs, which reach Web Forms only when
// every request is handed to it; it also answers "/" itself, so IIS's default document is not
// emulated.
void WriteHost(ConvertedProject web)
{
    var directory = Path.GetDirectoryName(web.TargetPath)!;
    using var stream = typeof(Report).Assembly.GetManifestResourceStream("ProgramTemplate.txt")!;
    var program = new StreamReader(stream).ReadToEnd();
    var projectText = File.ReadAllText(web.TargetPath);
    var routes = projectText.Contains("Microsoft.AspNet.FriendlyUrls", StringComparison.OrdinalIgnoreCase) ||
                 Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
                     .Any(f => Regex.IsMatch(File.ReadAllText(f), @"RouteTable\.Routes|RouteCollection"));
    if (routes)
    {
        program = program.Replace("app.UseDefaultFiles(defaults);", "// Routed application: Web Forms answers \"/\".")
            .Replace("options => options.UseAspNetCoreSessionProvider()", "options => options.HandleAllRequestsWithWebForms().UseAspNetCoreSessionProvider()");
        report.Add(Report.Kind.Project, web.Name, "routes: every request goes to Web Forms");
    }
    File.WriteAllText(Path.Combine(directory, "Program.cs"), program, new UTF8Encoding(false));

    if (cultureProfile != null)
    {
        Directory.CreateDirectory(Path.Combine(directory, "App_Data"));
        File.Copy(cultureProfile, Path.Combine(directory, "App_Data", "culture-profile.json"), overwrite: true);
        report.Add(Report.Kind.Project, web.Name, "culture profile placed in App_Data (Windows: UseNls; Linux: ICU data built from it)");
    }
}

static string? FindRuntime()
{
    for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory()); directory != null; directory = directory.Parent)
    {
        var candidate = Path.Combine(directory.FullName, "experiments", "wf4c");
        if (Directory.Exists(Path.Combine(candidate, "_feed"))) return candidate;
    }
    return null;
}
