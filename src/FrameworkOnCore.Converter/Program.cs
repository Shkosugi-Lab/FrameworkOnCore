using System.Text;
using System.Text.RegularExpressions;
using FrameworkOnCore.Converter;

// FrameworkOnCore: converts a .NET Framework web application (Web Forms, C#) to .NET 10 on
// WebFormsForCore, to run on Linux. See LINUX-CONVERTER-DESIGN.md. Its projects are C# and Visual Basic.
//
//   FrameworkOnCore.Converter <web project .csproj|.vbproj> --out <dir> [--root <dir>] [--runtime <dir>]
//                             [--culture-profile <file>] [--no-build]
//
// --root     the repository to copy (default: the topmost folder above the project with a .sln)
// --runtime  where the WebFormsForCore fork's feed (_feed) and the shims (shims) are
//            (default: experiments/wf4c above the current folder, or above the converter). The fork's packages are
//            fetched into _feed from the GitHub Release when they are not there (RuntimeSetup).
// --culture-profile  the original server's culture data (capture-culture.ps1), placed in App_Data
// --site     the deployed site (the original build's web folder, or the site's folder on the IIS
//            server): what the application is made of. Its assemblies built from the repository are
//            rebuilt for .NET 10 and the site is assembled in <out>\site (run bin\<web>.dll there).
// --build-original [target]  get the deployed site by building the repository with its own build
//            (a Cake build: Cake Frosting or a .cake script; its default target, or the one given)
//            in a copy (<out>.original); the site it deploys is then --site. Without one, the solution
//            with the web project, as Visual Studio builds it (Windows, Visual Studio's MSBuild).
// --configuration <name>  the configuration the site is built in: the projects' conditions are read for it
//            (Debug when not given) and a solution is built in it (Release when not given). openIMIS: DemoRelease,
//            the one whose web.config transform the repository has; its code under #If DEMO is that build's.
// --original-step <project;target>  a setup step of the repository after its build (repeatable).
// --deploy   how it is deployed on Linux: container (Dockerfile), linux (deploy/linux/install.sh, systemd),
//            both (the default) or none. See DeployWriter.
// --case-insensitive on|off  file names without regard to case in the deployment, as on Windows (on, the default: the
//            library casefs/libfoccase.so, which casefs/build.ps1 builds, preloaded by start.sh; off: Linux's).
// --choices <file>  what the user chose per component (foc-choices.json; analyze writes one with the defaults): the
//            rules of options not chosen are left out. Without it, the catalog's defaults (as always). --case-insensitive
//            given is over the file's file-name-case.

//
//   FrameworkOnCore.Converter analyze <project> --out <dir> [--root <dir>] [--configuration <name>] [--runtime <dir>]
//            the .NET Framework APIs the application uses, their counts, and what .NET 10 has of each (AnalyzeCommand):
//            api-analysis.json and API-ANALYSIS.md, nothing converted.

if (args.Length > 0 && args[0] == "analyze") return AnalyzeCommand.Run(args[1..], RuntimeSetup.Find);

string? project = null, outDirectory = null, rootDirectory = null, runtimeDirectory = null, cultureProfile = null, site = null, originalTarget = null, configuration = null;
var build = true;
var buildOriginal = false;
var deployKinds = "both";
bool? caseInsensitive = null;
string? choicesFile = null;
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
        case "--case-insensitive":
            var value = args[++i];
            if (value is not ("on" or "off")) { Console.Error.WriteLine($"--case-insensitive: on or off, not {value}"); return 2; }
            caseInsensitive = value == "on";
            break;
        case "--configuration": configuration = args[++i]; break;
        case "--choices": choicesFile = args[++i]; break;
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
    Console.Error.WriteLine("usage: FrameworkOnCore.Converter <web project .csproj|.vbproj> --out <dir> [--root <dir>] [--runtime <dir>] [--culture-profile <file>] [--configuration <name>] [--site <dir> | --build-original [target]] [--deploy container|linux|both|none] [--case-insensitive on|off] [--choices <file>] [--no-build]");
    return 2;
}

project = Path.GetFullPath(project);
var sourceRoot = Path.GetFullPath(rootDirectory ?? Paths.FindRoot(project));
var outRoot = Path.GetFullPath(outDirectory);
runtimeDirectory = Path.GetFullPath(runtimeDirectory ?? RuntimeSetup.Find() ?? throw new InvalidOperationException("--runtime: experiments/wf4c not found"));
var runtime = new RuntimeLayout(Path.Combine(runtimeDirectory, "_feed"), RuntimeSetup.ShimProjects(runtimeDirectory));

var report = new Report();
var rules = Rules.Load(Path.Combine(AppContext.BaseDirectory, "rules", "packages.json"));
RuntimeSetup.EnsureFeed(runtimeDirectory, rules.ForkVersion, Console.WriteLine);
// The user's choices: validated against the catalog, the rules narrowed to them; each one not the default reported.
var catalog = FrameworkOnCore.Analysis.Catalog.Default();
var choices = choicesFile != null ? FrameworkOnCore.Analysis.Choices.Load(choicesFile) : new FrameworkOnCore.Analysis.Choices();
if (choices.Validate(catalog) is { Count: > 0 } choiceErrors)
{
    foreach (var error in choiceErrors) Console.Error.WriteLine($"--choices {choicesFile}: {error}");
    return 2;
}
rules = rules.Choose(choices, catalog);
foreach (var (component, option) in choices.Components.Where(c => catalog.DefaultOf(c.Key).Id != c.Value))
    report.Add(Report.Kind.Project, "choices", $"{component}: {option} (default {catalog.DefaultOf(component).Id})");
foreach (var (api, option) in choices.Apis)
    report.Add(Report.Kind.Project, "choices", $"{api}: {option}");
if (caseInsensitive == null && choices.Settings.TryGetValue("file-name-case", out var nameCase) && nameCase != catalog.Settings.First(s => s.Id == "file-name-case").DefaultOption.Id)
    report.Add(Report.Kind.Project, "choices", $"file-name-case: {nameCase}");
caseInsensitive ??= choices.SettingOf(catalog, "file-name-case") == "insensitive";
if (buildOriginal)
{
    var originalWork = outRoot.TrimEnd('\\', '/') + ".original";
    var originalLog = originalWork + ".build.log";
    File.Delete(originalLog);
    site = new OriginalBuild(report, originalLog, configuration).Run(sourceRoot, originalWork, project, originalTarget, originalSteps);
    if (site == null)
    {
        File.WriteAllText(Path.Combine(Directory.CreateDirectory(outRoot).FullName, "CONVERSION-REPORT.md"), report.ToMarkdown($"FrameworkOnCore: {Path.GetFileName(project)}"), new UTF8Encoding(false));
        Console.WriteLine($"original build FAILED; log: {originalLog}");
        return 1;
    }
}
Console.WriteLine($"copying {sourceRoot} -> {outRoot}");
Paths.CopyTree(sourceRoot, outRoot);

var converter = new ProjectConverter(rules, report, new Conditions(configuration ?? "Debug", "AnyCPU", report), sourceRoot, outRoot, runtime);
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
        if (!ProjectConverter.IsConvertible(member))
        {
            report.Add(Report.Kind.Unsupported, assembly, $"in the deployed site, built by {Path.GetFileName(member)}: not converted (only C# and Visual Basic projects); the .NET Framework assembly is left");
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
// DLLs without source that refer to types where .NET 10 does not have them: retargeted into foc-retargeted, which
// the projects' build copies in their place (FocUseRetargetedAssemblies); built again to take them.
var preferred = AssemblyRetargeter.PreferredAssemblies(runtime.Feed, runtime.ShimProjects);
var ownAssemblies = converter.Converted.Select(c => c.AssemblyName ?? c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
if (build && succeeded)
{
    var retargetedFolder = Path.Combine(outRoot, ProjectConverter.RetargetedFolder);
    var webBin = Path.Combine(Path.GetDirectoryName(web.TargetPath)!, "bin");
    if (Directory.Exists(webBin) && AssemblyRetargeter.RetargetFolder(webBin, retargetedFolder, preferred, ownAssemblies, report).Any(r => r.Retargeted.Count > 0))
    {
        Console.WriteLine($"retargeted DLLs in {retargetedFolder}: building again");
        var (exit, output) = BuildFixer.Dotnet($"build \"{buildTarget}\" -nologo -v q");
        if (exit != 0)
        {
            report.Add(Report.Kind.Error, "build", "the build with the retargeted DLLs failed: " + string.Join(" / ", output.Split('\n').Where(l => l.Contains(" error ")).Take(5)));
            succeeded = false;
        }
    }
}
if (build && succeeded && site != null)
{
    // The ones built on their own, as they are after the build (the web project's are in its bin).
    var built = others.Select(o => converter.Converted.First(c => c.SourcePath == o.SourcePath)).ToList();
    SiteAssembler.Assemble(site, Path.Combine(outRoot, "site"), web, built, converter, report, cultureProfile);
    // The deployed site's .NET Framework DLLs are in it as they are: retargeted there, in place.
    AssemblyRetargeter.RetargetFolder(Path.Combine(outRoot, "site", "bin"), null, preferred, ownAssemblies, report);
}
// How it is deployed on Linux: the assembled site, or the web project's folder (built in place).
if (build && succeeded)
{
    var deployedSite = site != null ? Path.Combine(outRoot, "site") : Path.GetDirectoryName(web.TargetPath)!;
    new DeployWriter(report, outRoot, runtimeDirectory).Write(deployKinds, deployedSite, web, cultureProfile, caseInsensitive.Value);
}

var reportPath = Path.Combine(outRoot, "CONVERSION-REPORT.md");
File.WriteAllText(reportPath, report.ToMarkdown($"FrameworkOnCore: {Path.GetFileName(project)}"), new UTF8Encoding(false));
Console.WriteLine($"{(build ? (succeeded ? "build succeeded" : "build FAILED") : "not built")}; web project: {web.TargetPath}; report: {reportPath}");
return succeeded ? 0 : 1;

// Program.cs (Program.vb) of the web project, and the culture profile. An application that routes
// (System.Web.Routing, FriendlyUrls) serves extensionless URLs, which reach Web Forms only when
// every request is handed to it; it also answers "/" itself, so IIS's default document is not
// emulated.
// Why IIS's integrated pipeline gives every request to the application's managed modules (web.config's
// system.webServer/modules), or null: runAllManagedModulesForAllRequests="true", or a module added without the
// managedHandler precondition (IIS runs those for static files and URLs that are no file too).
static string? ModulesForAllRequests(string webDirectory)
{
    var config = Directory.EnumerateFiles(webDirectory, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
    if (config == null) return null;
    System.Xml.Linq.XElement? modules;
    // By local name: old web.configs have the .NET 2.0 configuration namespace.
    try { modules = System.Xml.Linq.XDocument.Load(config).Root?.Elements().FirstOrDefault(e => e.Name.LocalName == "system.webServer")?.Elements().FirstOrDefault(e => e.Name.LocalName == "modules"); }
    catch (System.Xml.XmlException) { return null; }
    if (modules == null) return null;
    if (string.Equals((string?)modules.Attribute("runAllManagedModulesForAllRequests"), "true", StringComparison.OrdinalIgnoreCase))
        return "web.config: runAllManagedModulesForAllRequests";
    var forAll = modules.Elements().Where(e => e.Name.LocalName == "add")
        .Where(a => a.Attribute("type") != null && !((string?)a.Attribute("preCondition") ?? "").Contains("managedHandler", StringComparison.OrdinalIgnoreCase))
        .Select(a => (string?)a.Attribute("name")).ToList();
    return forAll.Count > 0 ? $"web.config: modules without the managedHandler precondition ({string.Join(", ", forAll)})" : null;
}

void WriteHost(ConvertedProject web)
{
    var directory = Path.GetDirectoryName(web.TargetPath)!;
    var visualBasic = ProjectConverter.IsVisualBasic(web.TargetPath);
    using var stream = typeof(Report).Assembly.GetManifestResourceStream(visualBasic ? "ProgramTemplate.vb.txt" : "ProgramTemplate.txt")!;
    var program = new StreamReader(stream).ReadToEnd();
    var projectText = File.ReadAllText(web.TargetPath);
    // Routes registered by the application, in any of its projects (DNN: DotNetNuke.Web's ServicesRoutingManager;
    // its friendly URLs, /Login, /Terms, are rewritten by a module, for requests that reach it).
    var routes = projectText.Contains("Microsoft.AspNet.FriendlyUrls", StringComparison.OrdinalIgnoreCase) ||
                 converter.Converted.Select(c => Path.GetDirectoryName(c.TargetPath)!).Append(directory).Distinct(StringComparer.OrdinalIgnoreCase)
                     .SelectMany(d => Directory.EnumerateFiles(d, "*.*", SearchOption.AllDirectories)).Where(f => SourceLanguage.For(f) != null)
                     .Where(f => !Regex.IsMatch(f, @"[\\/](obj|bin)[\\/]"))
                     .Any(f => Regex.IsMatch(File.ReadAllText(f), @"RouteTable\.Routes|RouteCollection"));
    if (routes)
    {
        program = (visualBasic
                ? program.Replace("app.UseDefaultFiles(defaults)", "' Routed application: Web Forms answers \"/\".")
                : program.Replace("app.UseDefaultFiles(defaults);", "// Routed application: Web Forms answers \"/\"."))
            .Replace("options.UseAspNetCoreSessionProvider()", "options.HandleAllRequestsWithWebForms().UseAspNetCoreSessionProvider()");
        report.Add(Report.Kind.Project, web.Name, "routes: every request goes to Web Forms");
    }
    // Managed modules IIS runs for every request (runAllManagedModulesForAllRequests, or a module without the
    // managedHandler precondition): a URL rewriter among them answers URLs that are no file (BlogEngine's /post/...,
    // /archive). Every request goes to Web Forms, as IIS gives it to them; "/" is still the default document.
    else if (ModulesForAllRequests(directory) is { } why)
    {
        program = program.Replace("options.UseAspNetCoreSessionProvider()", "options.HandleAllRequestsWithWebForms().UseAspNetCoreSessionProvider()");
        report.Add(Report.Kind.Project, web.Name, $"every request goes to Web Forms: {why}");
    }
    File.WriteAllText(Path.Combine(directory, ProjectConverter.ProgramFile(web.TargetPath)), program, new UTF8Encoding(false));

    if (cultureProfile != null)
    {
        Directory.CreateDirectory(Path.Combine(directory, "App_Data"));
        File.Copy(cultureProfile, Path.Combine(directory, "App_Data", "culture-profile.json"), overwrite: true);
        report.Add(Report.Kind.Project, web.Name, "culture profile placed in App_Data (Windows: UseNls; Linux: ICU data built from it)");
    }
}

