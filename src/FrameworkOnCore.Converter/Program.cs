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

string? project = null, outDirectory = null, rootDirectory = null, runtimeDirectory = null, cultureProfile = null;
var build = true;
for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--out": outDirectory = args[++i]; break;
        case "--root": rootDirectory = args[++i]; break;
        case "--runtime": runtimeDirectory = args[++i]; break;
        case "--culture-profile": cultureProfile = args[++i]; break;
        case "--no-build": build = false; break;
        default: project = args[i]; break;
    }
}
if (project == null || outDirectory == null)
{
    Console.Error.WriteLine("usage: FrameworkOnCore.Converter <web project .csproj> --out <dir> [--root <dir>] [--runtime <dir>] [--culture-profile <file>] [--no-build]");
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
Console.WriteLine($"copying {sourceRoot} -> {outRoot}");
Paths.CopyTree(sourceRoot, outRoot);

var converter = new ProjectConverter(rules, report, new Conditions("Debug", "AnyCPU", report), sourceRoot, outRoot, runtime);
var web = converter.Convert(project, isWeb: true);
WriteHost(web);

var succeeded = true;
if (build) succeeded = new BuildFixer(report, converter.Converted, outRoot).Run(web.TargetPath);

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
