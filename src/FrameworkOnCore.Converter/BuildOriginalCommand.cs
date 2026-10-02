using System.Text;

namespace FrameworkOnCore.Converter;

/// <summary>
/// FrameworkOnCore.Converter build-original &lt;project&gt; --out &lt;dir&gt; [--root &lt;dir&gt;] [--configuration &lt;name&gt;]
/// [--original-step &lt;project;target&gt;] [--target &lt;name&gt;]: the original application built as its repository builds it
/// (OriginalBuild, as --build-original does), in a copy (&lt;out&gt;), nothing converted; the deployed site it made is the
/// last line, "site: &lt;path&gt;" (to run it as it is on .NET Framework: Studio's test run). ORIGINAL-BUILD.md beside the copy.
/// </summary>
public static class BuildOriginalCommand
{
    /// <summary>build-original-advice &lt;project&gt; [--root &lt;dir&gt;]: whether its site is made by building it, and why (OriginalBuildAdvice).</summary>
    internal static int Advise(string[] args)
    {
        string? project = null, rootDirectory = null;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--root") rootDirectory = args[++i];
            else project = args[i];
        }
        if (project == null)
        {
            Console.Error.WriteLine("usage: FrameworkOnCore.Converter build-original-advice <web project .csproj|.vbproj> [--root <dir>]");
            return 2;
        }
        project = Path.GetFullPath(project);
        var advice = OriginalBuildAdvice.Of(project, Path.GetFullPath(rootDirectory ?? Paths.FindRoot(project)));
        Console.WriteLine(advice.Needed ? "--build-original: needed" : "--build-original: not needed (the web project's folder is the site)");
        foreach (var reason in advice.Reasons) Console.WriteLine($"  {reason}");
        return 0;
    }

    internal static int Run(string[] args)
    {
        string? project = null, outDirectory = null, rootDirectory = null, configuration = null, target = null;
        var steps = new List<(string Project, string Target)>();
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--out": outDirectory = args[++i]; break;
                case "--root": rootDirectory = args[++i]; break;
                case "--configuration": configuration = args[++i]; break;
                case "--target": target = args[++i]; break;
                case "--original-step":
                    var step = args[++i].Split(';', 2);
                    steps.Add((step[0], step.Length > 1 ? step[1] : "Build"));
                    break;
                default: project = args[i]; break;
            }
        }
        if (project == null || outDirectory == null)
        {
            Console.Error.WriteLine("usage: FrameworkOnCore.Converter build-original <web project .csproj|.vbproj> --out <dir> [--root <dir>] [--configuration <name>] [--target <name>] [--original-step <project;target>]");
            return 2;
        }
        project = Path.GetFullPath(project);
        var root = Path.GetFullPath(rootDirectory ?? Paths.FindRoot(project));
        var work = Path.GetFullPath(outDirectory).TrimEnd('\\', '/');
        var log = work + ".build.log";
        File.Delete(log);
        var report = new Report();
        var site = new OriginalBuild(report, log, configuration).Run(root, work, project, target, steps);
        File.WriteAllText(work + ".ORIGINAL-BUILD.md", report.ToMarkdown($"FrameworkOnCore: {Path.GetFileName(project)} (original build)"), new UTF8Encoding(false));
        if (site == null)
        {
            Console.WriteLine($"original build FAILED; log: {log}");
            return 1;
        }
        Console.WriteLine($"site: {site}");
        return 0;
    }
}
