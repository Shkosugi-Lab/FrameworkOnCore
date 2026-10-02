using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>
/// Whether an application's site is made by building it (--build-original), read from its repository's files: a build
/// script that assembles it (Cake: DNN), other projects whose build writes into the web project's folder (their output
/// path: nopCommerce 3.90's plugins; a copy after their build: mojoPortal's features), the web project's own build
/// making files of the site (a web.config transform: openIMIS; a copy after it), no web.config in its folder (made by
/// the build), a build file of the repository's own (n2cms' build\n2.proj). Without one of them the web project's
/// folder is the site, as it is for an ordinary web application project (BlogEngine, WingtipToys).
/// </summary>
public static partial class OriginalBuildAdvice
{
    public sealed record Advice(bool Needed, IReadOnlyList<string> Reasons);

    public static Advice Of(string webProject, string root)
    {
        webProject = Path.GetFullPath(webProject);
        root = Path.GetFullPath(root);
        var web = Path.GetDirectoryName(webProject)!;
        var reasons = new List<string>();
        string Relative(string path) => Path.GetRelativePath(root, path).Replace('\\', '/');

        var projects = Files(root, "*.csproj").Concat(Files(root, "*.vbproj")).ToList();

        // A Cake build (Cake Frosting's project, a .cake script): the converter runs it.
        if (projects.FirstOrDefault(p => Regex.IsMatch(Text(p), @"Include=""Cake\.Frosting""", RegexOptions.IgnoreCase)) is { } frosting)
            reasons.Add($"Cake のビルド({Relative(frosting)})がある");
        else if (Directory.EnumerateFiles(root, "*.cake").FirstOrDefault() is { } script)
            reasons.Add($"Cake のビルドスクリプト({Relative(script)})がある");

        // Other projects that build into the web project's folder. Not those it references (the conversion converts them
        // and builds them into its bin: YAF's libraries), nor other web projects beside it (YAF-MySql.csproj, the same site
        // for another database).
        var referenced = References(webProject);
        foreach (var project in projects.Where(p => !p.Equals(webProject, StringComparison.OrdinalIgnoreCase) && !referenced.Contains(p)))
        {
            if (Path.GetDirectoryName(project)!.Equals(web, StringComparison.OrdinalIgnoreCase) && IsWebProject(Text(project))) continue;
            var text = WithoutComments(Text(project));
            var directory = Path.GetDirectoryName(project)!;
            foreach (Match output in OutputPath().Matches(text))
            {
                if (Inside(Resolve(output.Groups["v"].Value, directory, root), web))
                {
                    reasons.Add($"{Relative(project)} の出力先が Web プロジェクトのフォルダーの中({output.Groups["v"].Value.Trim()})");
                    break;
                }
            }
            if (CopiesInto(text, directory, root, web) is { } copy)
                reasons.Add($"{Relative(project)} のビルド後の処理が Web プロジェクトのフォルダーへコピーする({copy})");
        }

        // The web project's own build making files of the site.
        var own = WithoutComments(Text(webProject));
        if (Regex.IsMatch(own, @"<TransformXml\b", RegexOptions.IgnoreCase))
            reasons.Add("Web プロジェクトのビルドが web.config を変換して作る(TransformXml)");
        if (CopiesInto(own, web, root, web) is { } ownCopy)
            reasons.Add($"Web プロジェクトのビルド後の処理がファイルをコピーする({ownCopy})");
        if (!Directory.EnumerateFiles(web, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).Any())
            reasons.Add("Web プロジェクトのフォルダーに web.config が無い(ビルドで作られる)");

        // A build file of the repository's own, at its root or in its build folder (when no Cake build was found).
        if (!reasons.Any(r => r.StartsWith("Cake", StringComparison.Ordinal)))
        {
            var folders = new[] { root, Path.Combine(root, "build"), Path.Combine(root, "Build") }.Where(Directory.Exists).Distinct(StringComparer.OrdinalIgnoreCase);
            var script = folders.SelectMany(f => Directory.EnumerateFiles(f))
                .FirstOrDefault(f => Path.GetExtension(f).ToLowerInvariant() is ".proj" or ".build" ||
                                     Path.GetFileName(f).ToLowerInvariant() is "build.ps1" or "build.cmd" or "build.bat" or "build.sh" or "psakefile.ps1");
            if (script != null) reasons.Add($"リポジトリ独自のビルドファイル({Relative(script)})がある");
        }
        return new Advice(reasons.Count > 0, reasons);
    }

    // The projects a project references, directly or through the ones it references.
    static HashSet<string> References(string project)
    {
        var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var pending = new Stack<string>([project]);
        while (pending.Count > 0)
        {
            var current = pending.Pop();
            var directory = Path.GetDirectoryName(current)!;
            foreach (Match reference in ProjectReference().Matches(WithoutComments(Text(current))))
            {
                string path;
                try { path = Path.GetFullPath(Path.Combine(directory, reference.Groups["v"].Value.Replace('\\', Path.DirectorySeparatorChar))); }
                catch (ArgumentException) { continue; }
                if (File.Exists(path) && found.Add(path)) pending.Push(path);
            }
        }
        return found;
    }

    // A web application project (its project type, or the web SDK).
    static bool IsWebProject(string text) =>
        text.Contains("349c5851-65df-11da-9384-00065b846f21", StringComparison.OrdinalIgnoreCase) || text.Contains("Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase);

    // A copy after a build (PostBuildEvent, a Copy or Exec task) to a path in the web project's folder: the path.
    static string? CopiesInto(string text, string directory, string root, string web)
    {
        foreach (Match step in BuildStep().Matches(text))
        {
            var body = step.Value;
            if (!Regex.IsMatch(body, @"xcopy|robocopy|\bcopy\b|<Copy\b|DestinationFolder", RegexOptions.IgnoreCase)) continue;
            foreach (Match path in StepPath().Matches(body))
            {
                var value = path.Value.Trim('"', '\'');
                if (Inside(Resolve(value, directory, root), web)) return value;
            }
        }
        return null;
    }

    static string? Resolve(string value, string directory, string root)
    {
        value = value.Trim();
        value = Regex.Replace(value, @"\$\(SolutionDir\)", root.TrimEnd('\\', '/') + "\\", RegexOptions.IgnoreCase);
        value = Regex.Replace(value, @"\$\((ProjectDir|MSBuildProjectDirectory|MSBuildThisFileDirectory)\)\\?", directory.TrimEnd('\\', '/') + "\\", RegexOptions.IgnoreCase);
        if (value.Contains("$(") || value.Length == 0) return null;  // another property: not known without the build
        try { return Path.GetFullPath(Path.Combine(directory, value.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar))); }
        catch (ArgumentException) { return null; }
        catch (NotSupportedException) { return null; }
    }

    static bool Inside(string? path, string folder) =>
        path != null && (path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar)
            .StartsWith(folder.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

    static IEnumerable<string> Files(string root, string pattern) =>
        Directory.EnumerateFiles(root, pattern, new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true })
            .Where(f => !Regex.IsMatch(f, @"[\\/](node_modules|packages|bin|obj|\.git)[\\/]", RegexOptions.IgnoreCase));

    static string Text(string file)
    {
        try { return File.ReadAllText(file); }
        catch (IOException) { return ""; }
    }

    static string WithoutComments(string text) => Regex.Replace(text, "<!--.*?-->", "", RegexOptions.Singleline);

    [GeneratedRegex(@"<ProjectReference\b[^>]*Include=""(?<v>[^""]+)""", RegexOptions.IgnoreCase)]
    private static partial Regex ProjectReference();

    [GeneratedRegex(@"<(OutputPath|OutDir)\b[^>]*>(?<v>[^<]+)</", RegexOptions.IgnoreCase)]
    private static partial Regex OutputPath();

    [GeneratedRegex(@"<PostBuildEvent>.*?</PostBuildEvent>|<Target\b[^>]*Name=""(AfterBuild|[^""]*Copy[^""]*)""[^>]*>.*?</Target>|<Target\b[^>]*AfterTargets=""Build""[^>]*>.*?</Target>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex BuildStep();

    [GeneratedRegex(@"""[^""]*(\$\((SolutionDir|ProjectDir|MSBuildProjectDirectory|MSBuildThisFileDirectory)\)|\.\.[\\/])[^""]*""|(\$\((SolutionDir|ProjectDir)\)|\.\.[\\/])[^\s""<>]*", RegexOptions.IgnoreCase)]
    private static partial Regex StepPath();
}
