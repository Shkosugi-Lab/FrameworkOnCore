using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Converter;

/// <summary>
/// The encodings of the sources, as the original build read them (SourceEncoding): a file without a byte order mark that
/// is not UTF-8 is in the project's CodePage, else in the ANSI code page of the machine it was built on (Shift_JIS on a
/// Japanese Windows), which Roslyn's compilers fall back to there. The .NET 10 build on Windows falls back to it too; on
/// Linux there is none (Shift_JIS read as UTF-8: CS1056, CS1010 where a character's second byte is a backslash). The
/// converted project names it (CodePage, csc /codepage), the files kept as they are. And the server's: its pages,
/// App_Code, read by FrameworkOnCore's System.Web as ASP.NET read them (fileEncoding, else the ANSI code page), which it
/// is told (the runtime option FrameworkOnCore.AnsiCodePage).
/// </summary>
public sealed partial class ProjectConverter
{
    /// <summary>The runtime's option of the ANSI code page of the original server (FrameworkOnCore's System.Web; the
    /// compatibility assembly's Encoding.Default).</summary>
    public const string AnsiCodePageOption = "FrameworkOnCore.AnsiCodePage";

    // A project's sources' code page where it does not say: the machine's.
    static int? SourceEncodingFallback => SourceEncoding.MachineCodePage;

    static int? ParseCodePage(string? value) => int.TryParse(value?.Trim(), out var page) && page > 0 ? page : null;

    /// <summary>
    /// The project's sources read as its build read them: the CodePage to write into the converted project (null: none
    /// needed, or the project has its own). The fewer files changed of two ways: CodePage written, and the UTF-8 files
    /// without a byte order mark that have characters beyond ASCII given one (CodePage is for every file without one; the
    /// original's compiler read them as UTF-8 first) - a project in Shift_JIS; or, where those are more than the files in
    /// the code page, these made UTF-8 with a byte order mark, the text the original build read (a few files of another
    /// code page among UTF-8 ones: YAF's, nopCommerce's).
    /// </summary>
    int? SourcesCodePage(string name, IEnumerable<string> sources, int? projectCodePage)
    {
        var fallback = projectCodePage ?? SourceEncoding.MachineCodePage;
        List<string> inCodePage = new(), notUtf8 = new(), utf8 = new();
        foreach (var file in sources.Where(File.Exists).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var read = SourceFile.Read(file, fallback);
            if (read.Kind == SourceEncodingKind.CodePage) inCodePage.Add(file);
            else if (read.Kind == SourceEncodingKind.NotUtf8) notUtf8.Add(file);
            else if (read.Kind == SourceEncodingKind.Utf8 && read.HasNonAscii) utf8.Add(file);
        }
        if (notUtf8.Count > 0)
            report.Add(Report.Kind.Error, name, $"{notUtf8.Count} source file(s) without a byte order mark are not UTF-8, and the code page they were written in is not known (this machine has no ANSI code page): convert with --code-page (932: Shift_JIS); compiled as UTF-8, their characters beyond ASCII are lost ({Files(notUtf8)})");
        if (inCodePage.Count == 0) return null;
        var described = SourceEncoding.Describe(fallback!.Value);
        if (projectCodePage != null)
        {
            report.Add(Report.Kind.Project, name, $"{inCodePage.Count} source file(s) in code page {described}, the project's CodePage ({Files(inCodePage)})");
            return null;
        }
        if (utf8.Count > inCodePage.Count)
        {
            foreach (var file in inCodePage)
            {
                var target = TargetOf(file);
                if (!File.Exists(target) || SourceFile.Read(target, fallback) is not { Kind: SourceEncodingKind.CodePage } read) continue;
                File.WriteAllBytes(target, [0xEF, 0xBB, 0xBF, .. new System.Text.UTF8Encoding(false).GetBytes(read.Text)]);
            }
            report.Add(Report.Kind.Project, name, $"{inCodePage.Count} source file(s) in code page {described} without a byte order mark, which the original build read in the ANSI code page of the machine it was built on ({Files(inCodePage)}), " +
                $"among {utf8.Count} UTF-8 one(s) without one: made UTF-8 with a byte order mark, the text the original build read (CodePage {fallback} would change more files)");
            return null;
        }
        report.Add(Report.Kind.Project, name, $"{inCodePage.Count} source file(s) in code page {described} without a byte order mark, which the original build read in the ANSI code page of the machine it was built on ({Files(inCodePage)}): " +
            $"CodePage {fallback} written, so that the compiler reads them so on Linux too (there is none there); the files kept as they are");
        foreach (var file in utf8)
        {
            var target = TargetOf(file);
            if (!File.Exists(target)) continue;
            var bytes = File.ReadAllBytes(target);
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF) continue;
            File.WriteAllBytes(target, [0xEF, 0xBB, 0xBF, .. bytes]);
            report.Add(Report.Kind.Project, $"{name} {Path.GetRelativePath(sourceRoot, file)}", $"UTF-8 without a byte order mark, with code page {fallback}'s files: given the mark, as CodePage is for every file without one (its text as the original build read it)");
        }
        return fallback;
    }

    /// <summary>
    /// The site's pages, user controls, handlers, App_Code and code files (CodeFile) - what the server reads and compiles,
    /// not the project's sources - in the machine's code page: the code page the runtime takes as the original server's
    /// ANSI code page (ASP.NET's fileEncoding when web.config has none; Encoding.Default), null when none is.
    /// </summary>
    int? ServerCodePage(string name, string directory, IEnumerable<string> compiled)
    {
        if (SourceEncoding.MachineCodePage is not { } machine) return null;
        var built = compiled.Select(Path.GetFullPath).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var pages = PageFiles(directory).Concat(Directory.EnumerateFiles(directory, "*.skin", SearchOption.AllDirectories))
            .Where(f => !built.Contains(Path.GetFullPath(f)))
            .Where(f => SourceFile.Read(f, machine).Kind == SourceEncodingKind.CodePage).ToList();
        if (pages.Count == 0) return null;
        report.Add(Report.Kind.Project, name, $"{pages.Count} file(s) the server reads and compiles in code page {SourceEncoding.Describe(machine)} without a byte order mark ({Files(pages)}): " +
            $"read so by FrameworkOnCore's System.Web too, as ASP.NET read them where web.config has no fileEncoding ({AnsiCodePageOption} {machine})");
        return machine;
    }

    string Files(List<string> files) =>
        string.Join(", ", files.Take(6).Select(f => Path.GetRelativePath(sourceRoot, f).Replace('\\', '/'))) + (files.Count > 6 ? $", ... ({files.Count})" : "");
}
