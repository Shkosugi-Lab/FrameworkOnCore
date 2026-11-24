using System.Text;

namespace FrameworkOnCore.Converter;

/// <summary>
/// FrameworkOnCore.Converter build-original &lt;project&gt; --out &lt;dir&gt; [--root &lt;dir&gt;] [--configuration &lt;name&gt;] [--msbuild &lt;which&gt;]
/// [--original-step &lt;project;target&gt;] [--target &lt;name&gt;]: the original application built as its repository builds it
/// (OriginalBuild, as --build-original does), in a copy (&lt;out&gt;-&lt;time&gt;, a new one each time), nothing converted; the deployed site it made is the
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
            if (args[i] == "--root") rootDirectory = CommandLine.Value(args, ref i);
            else CommandLine.Positional(ref project, args[i], "project");
        }
        if (project == null)
        {
            Console.Error.WriteLine("usage: FrameworkOnCore.Converter build-original-advice <web project .csproj|.vbproj> [--root <dir>]");
            return 2;
        }
        project = CommandLine.Project(project);
        var advice = OriginalBuildAdvice.Of(project, rootDirectory != null ? CommandLine.Folder("--root", rootDirectory) : Paths.FindRoot(project));
        Console.WriteLine(advice.Needed ? "--build-original: needed" : "--build-original: not needed (the web project's folder is the site)");
        foreach (var reason in advice.Reasons) Console.WriteLine($"  {reason}");
        return 0;
    }

    internal static int Run(string[] args)
    {
        string? project = null, outDirectory = null, rootDirectory = null, configuration = null, target = null, msbuild = null;
        var steps = new List<(string Project, string Target)>();
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--out": outDirectory = CommandLine.Value(args, ref i); break;
                case "--root": rootDirectory = CommandLine.Value(args, ref i); break;
                case "--configuration": configuration = CommandLine.Value(args, ref i); break;
                case "--target": target = CommandLine.Value(args, ref i); break;
                case "--msbuild": msbuild = CommandLine.Value(args, ref i); break;
                case "--original-step":
                    var step = CommandLine.Value(args, ref i).Split(';', 2);
                    steps.Add((step[0], step.Length > 1 ? step[1] : "Build"));
                    break;
                default: CommandLine.Positional(ref project, args[i], "project"); break;
            }
        }
        if (project == null || outDirectory == null)
        {
            Console.Error.WriteLine("usage: FrameworkOnCore.Converter build-original <web project .csproj|.vbproj> --out <dir> [--root <dir>] [--configuration <name>] [--target <name>] [--original-step <project;target>] [--msbuild visualstudio|buildtools|<folder>]");
            return 2;
        }
        project = CommandLine.Project(project);
        var root = rootDirectory != null ? CommandLine.Folder("--root", rootDirectory) : Paths.FindRoot(project);
        if (File.Exists(outDirectory)) throw new UsageException($"--out {outDirectory}: a file, not a folder");
        var work = Path.GetFullPath(outDirectory).TrimEnd('\\', '/');
        var log = work + ".build.log";
        Directory.CreateDirectory(Path.GetDirectoryName(work)!);  // beside --out, whose folder may not be there yet
        File.Delete(log);
        var report = new Report();
        var site = new OriginalBuild(report, log, configuration, msbuild).Run(root, work, project, target, steps);
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
