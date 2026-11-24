using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using static FrameworkOnCore.Tests.RewriteHarness;

namespace FrameworkOnCore.Tests;

/// <summary>
/// What the converter writes for Windows' path separators (WindowsPathAnalyzer, SourceEdits), run: a path made with '\' in
/// every way code makes one (joined, formatted, interpolated, built, combined, replaced into, trimmed, kept in a local, a
/// field or a constant, chosen by a condition, in a lambda or a local function; C# and Visual Basic), the method as written
/// and as rewritten both compiled and called. Held:
/// <list type="bullet">
/// <item>the rewritten source compiles, and rewritten again it is the same (nothing left to rewrite);</item>
/// <item>on Windows the rewritten method gives what the original gives (the rewrite changes nothing there);</item>
/// <item>on Linux it gives the original's path with Linux's separators, no '\' left in it: the path a file is found by;</item>
/// <item>a '\' that is no path's (a regular expression, an escape, DOMAIN\user, a JavaScript string) is not rewritten, and
/// the method gives what it gave.</item>
/// </list>
/// </summary>
public class RewriteBehaviourSweepTests
{
    const string Head = "using System; using System.IO; using System.Linq; using System.Text; using System.Collections.Generic;\npublic static class C\n{\n";

    // Each: members of a static class with string F(string rootPath, string fileName), the path of a file under a folder.
    public static readonly (string Name, string Members)[] Paths =
    {
        ("joined", "public static string F(string rootPath, string fileName) => rootPath + \"\\\\bin\\\\\" + fileName;"),
        ("joined, a verbatim string", "public static string F(string rootPath, string fileName) => rootPath + @\"\\bin\\\" + fileName;"),
        ("joined with a char", "public static string F(string rootPath, string fileName) => rootPath + '\\\\' + fileName;"),
        ("joined, the separator first", "public static string F(string rootPath, string fileName) => rootPath + \"\\\\\" + \"bin\" + \"\\\\\" + fileName;"),
        ("string.Format", "public static string F(string rootPath, string fileName) => string.Format(\"{0}\\\\bin\\\\{1}\", rootPath, fileName);"),
        ("an interpolation", "public static string F(string rootPath, string fileName) => $\"{rootPath}\\\\bin\\\\{fileName}\";"),
        ("a verbatim interpolation", "public static string F(string rootPath, string fileName) => $@\"{rootPath}\\bin\\{fileName}\";"),
        ("Path.Combine with a folder of two", "public static string F(string rootPath, string fileName) => Path.Combine(rootPath, \"bin\\\\sub\", fileName);"),
        ("Path.Combine with a verbatim folder", "public static string F(string rootPath, string fileName) => Path.Combine(rootPath, @\"bin\\sub\", fileName);"),
        ("string.Concat", "public static string F(string rootPath, string fileName) => string.Concat(rootPath, \"\\\\bin\\\\\", fileName);"),
        ("string.Join", "public static string F(string rootPath, string fileName) { var filePath = string.Join(\"\\\\\", new[] { rootPath, \"bin\", fileName }); return filePath; }"),
        ("a StringBuilder", "public static string F(string rootPath, string fileName) { var filePath = new StringBuilder(rootPath); filePath.Append(\"\\\\bin\\\\\").Append(fileName); return filePath.ToString(); }"),
        ("kept in a local", "public static string F(string rootPath, string fileName) { var folderPath = \"bin\\\\sub\"; return Path.Combine(rootPath, folderPath, fileName); }"),
        ("kept in a local, joined later", "public static string F(string rootPath, string fileName) { string filePath = rootPath; filePath += \"\\\\bin\\\\\"; filePath += fileName; return filePath; }"),
        ("a static readonly field", "static readonly string BinFolder = \"\\\\bin\\\\\"; public static string F(string rootPath, string fileName) => rootPath + BinFolder + fileName;"),
        ("a constant", "const string BinFolder = \"\\\\bin\\\\\"; public static string F(string rootPath, string fileName) => rootPath + BinFolder + fileName;"),
        ("a constant char", "const char DirSeparator = '\\\\'; public static string F(string rootPath, string fileName) => rootPath + DirSeparator + \"bin\" + DirSeparator + fileName;"),
        ("a property", "static string BinFolder => \"\\\\bin\\\\\"; public static string F(string rootPath, string fileName) => rootPath + BinFolder + fileName;"),
        ("chosen by a condition", "public static string F(string rootPath, string fileName) => rootPath + (fileName.Length > 3 ? \"\\\\bin\\\\\" : \"\\\\obj\\\\\") + fileName;"),
        ("chosen by a switch", "public static string F(string rootPath, string fileName) { string folderPath; switch (fileName.Length) { case 0: folderPath = \"\\\\none\\\\\"; break; default: folderPath = \"\\\\bin\\\\\"; break; } return rootPath + folderPath + fileName; }"),
        ("or a default", "public static string F(string rootPath, string fileName) { string folderPath = null; return rootPath + (folderPath ?? \"\\\\bin\\\\\") + fileName; }"),
        ("in a lambda", "public static string F(string rootPath, string fileName) { Func<string, string> filePath = f => rootPath + \"\\\\bin\\\\\" + f; return filePath(fileName); }"),
        ("in a local function", "public static string F(string rootPath, string fileName) { string FilePath(string f) => rootPath + \"\\\\bin\\\\\" + f; return FilePath(fileName); }"),
        ("in a query", "public static string F(string rootPath, string fileName) => new[] { fileName }.Select(f => rootPath + \"\\\\bin\\\\\" + f).First();"),
        ("through a method of the class", "static string MapPath(string relativePath) => \"{root}\" + relativePath; public static string F(string rootPath, string fileName) => MapPath(\"\\\\bin\\\\\" + fileName).Replace(\"{root}\", rootPath);"),
        ("slashes made backslashes", "public static string F(string rootPath, string fileName) { var filePath = rootPath + \"/bin/\" + fileName; return filePath.Replace('/', '\\\\'); }"),
        ("slashes made backslashes, as strings", "public static string F(string rootPath, string fileName) { var filePath = rootPath + \"/bin/\" + fileName; return filePath.Replace(\"/\", \"\\\\\"); }"),
        ("trimmed, then joined", "public static string F(string rootPath, string fileName) => rootPath.TrimEnd('\\\\') + \"\\\\bin\\\\\" + fileName;"),
        ("ends with the separator, or it is added", "public static string F(string rootPath, string fileName) { var folderPath = rootPath + \"\\\\bin\"; if (!folderPath.EndsWith(\"\\\\\")) folderPath += \"\\\\\"; return folderPath + fileName; }"),
        ("the folder of a path", "public static string F(string rootPath, string fileName) => Path.GetDirectoryName(rootPath + \"\\\\bin\\\\sub\\\\x.txt\") + \"\\\\\" + fileName;"),
        ("split and joined", "public static string F(string rootPath, string fileName) { var filePath = rootPath + \"\\\\bin\\\\\" + fileName; var parts = filePath.Split('\\\\'); return string.Join(\"\\\\\", parts); }"),
        ("the last separator's place", "public static string F(string rootPath, string fileName) { var filePath = rootPath + \"\\\\bin\\\\\" + fileName; var at = filePath.LastIndexOf('\\\\'); return filePath.Substring(0, at) + \"\\\\\" + filePath.Substring(at + 1); }"),
        ("an array of folders", "public static string F(string rootPath, string fileName) { var folderPaths = new[] { \"bin\\\\sub\", \"obj\\\\sub\" }; return Path.Combine(rootPath, folderPaths[0], fileName); }"),
        ("a dictionary of folders", "public static string F(string rootPath, string fileName) { var folderPaths = new Dictionary<string, string> { [\"bin\"] = \"bin\\\\sub\" }; return Path.Combine(rootPath, folderPaths[\"bin\"], fileName); }"),
        ("given to a file API", "public static string F(string rootPath, string fileName) => new FileInfo(rootPath + \"\\\\bin\\\\\" + fileName).FullName;"),
        ("given to a file API by a named argument", "public static string F(string rootPath, string fileName) => Path.GetFullPath(path: rootPath + \"\\\\bin\\\\\" + fileName);"),
        ("a parameter's default", "static string Join(string rootPath, string fileName, string folderPath = \"\\\\bin\\\\\") => rootPath + folderPath + fileName; public static string F(string rootPath, string fileName) => Join(rootPath, fileName);"),
        ("a raw string", "public static string F(string rootPath, string fileName) => rootPath + \"\"\"\\bin\\\"\"\" + fileName;"),
    };

    // A '\' that is no path's: the method's result is to be as it was, the source as it was.
    public static readonly (string Name, string Members)[] NotPaths =
    {
        ("a regular expression", "public static string F(string first, string text) => System.Text.RegularExpressions.Regex.Replace(text, \"\\\\d+\\\\.\", \"#\") + \"\\\\w\";"),
        ("a JavaScript string", "public static string F(string first, string text) => \"alert('\" + text.Replace(\"'\", \"\\\\'\").Replace(\"\\n\", \"\\\\n\") + \"');\";"),
        ("a domain's user", "public static string F(string first, string text) { var userName = \"DOMAIN\\\\\" + text; return userName; }"),
        ("a JSON text", "public static string F(string first, string text) => \"{\\\"name\\\":\\\"a\\\\\\\\b\\\"}\" + text.Length;"),
        ("an LDAP filter", "public static string F(string first, string text) => \"(cn=\" + text.Replace(\"*\", \"\\\\2a\").Replace(\"(\", \"\\\\28\") + \")\";"),
        ("a registry key", "public static string F(string first, string text) { var registryKey = \"SOFTWARE\\\\Vendor\\\\\" + text; return registryKey; }"),
        ("a format's escape", "public static string F(string first, string text) => DateTime.MinValue.ToString(\"yyyy\\\\-MM\\\\-dd\") + text.Length;"),
        ("a CSV's quote", "public static string F(string first, string text) => \"\\\"\" + text.Replace(\"\\\"\", \"\\\\\\\"\") + \"\\\"\";"),
        ("a case label", "public static string F(string first, string text) { var n = 0; foreach (var c in text) switch (c) { case '\\\\': n++; break; } return n.ToString(); }"),
        ("both separators handled already", "public static string F(string first, string text) { var joined = first + \"/\" + text; return joined.Replace('\\\\', '/').TrimEnd('/', '\\\\'); }"),
    };

    public static IEnumerable<object[]> PathCases => Paths.Select(p => new object[] { p.Name });
    public static IEnumerable<object[]> NotPathCases => NotPaths.Select(p => new object[] { p.Name });

    static Func<string, string, string> Load(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        Assert.True(result.Success, string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)) + "\n" + compilation.SyntaxTrees.First());
        stream.Position = 0;
        var assembly = new AssemblyLoadContext("sweep", isCollectible: true).LoadFromStream(stream);
        var method = assembly.GetTypes().Single(t => t.Name == "C").GetMethod("F", BindingFlags.Public | BindingFlags.Static)!;
        return (rootPath, fileName) => (string)method.Invoke(null, new object[] { rootPath, fileName })!;
    }

    static Func<string, string, string> CSharpMethod(string source) => Load(CSharpCompilation.Create("Sweep" + Guid.NewGuid().ToString("N"),
        new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest)) }, Framework, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)));

    static Func<string, string, string> VisualBasicMethod(string source) => Load(VisualBasicCompilation.Create("Sweep" + Guid.NewGuid().ToString("N"),
        new[] { VisualBasicSyntaxTree.ParseText(source) }, Framework, new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary)));

    // The folder and the file the methods are called with: this system's.
    static (string Root, string File) Given => OperatingSystem.IsWindows() ? (@"C:\app\site", "page.aspx") : ("/app/site", "page.aspx");

    // The file's path under the folder, as the original makes it on Windows: \bin\page.aspx, but for these.
    static string Under(string name) =>
        name is "joined with a char" or "a char" ? @"\page.aspx"
        : name is "Path.Combine with a folder of two" or "Path.Combine with a verbatim folder" or "the folder of a path" or "an array of folders" or "a dictionary of folders" or "Path.Combine" ? @"\bin\sub\page.aspx"
        : @"\bin\page.aspx";

    static void Hold(string language, string name, string source, string rewritten, string again, Func<string, Func<string, string, string>> compile)
    {
        Assert.True(source != rewritten, $"{name}: nothing was rewritten\n{source}");
        Assert.True(again == rewritten, $"{name}: rewritten again it is another\n{rewritten}\n--\n{again}");
        var (root, file) = Given;
        // ("kept in a local": C#'s keeps a folder of two, Visual Basic's the path joined)
        var under = language == "CS" && name == "kept in a local" ? @"\bin\sub\page.aspx" : Under(name);
        var ported = compile(rewritten)(root, file);
        if (OperatingSystem.IsWindows())
        {
            // The original's path on Windows, and the rewritten's the same.
            var original = compile(source)(root, file);
            Assert.True(original == root + under, $"{name}: the original gives {original}, not {root + under}");
            Assert.True(original == ported, $"{name}: on Windows the original gives {original}, the rewritten {ported}\n{rewritten}");
        }
        // The same file on Linux: its path with Linux's separators.
        else Assert.True(ported == root + under.Replace('\\', '/'), $"{name}: the rewritten gives {ported}, not {root + under.Replace('\\', '/')}\n{rewritten}");
    }

    [Theory]
    [MemberData(nameof(PathCases))]
    public void A_path_made_in_csharp_is_this_systems_once_rewritten(string name)
    {
        var source = Head + Paths.Single(p => p.Name == name).Members + "\n}\n";
        var rewritten = CSharp(source);
        Hold("CS", name, source, rewritten, CSharp(rewritten), CSharpMethod);
    }

    [Theory]
    [MemberData(nameof(NotPathCases))]
    public void What_is_no_path_in_csharp_is_left(string name)
    {
        var source = Head + NotPaths.Single(p => p.Name == name).Members + "\n}\n";
        var rewritten = CSharp(source);
        Assert.True(source == rewritten, $"{name}: rewritten\n{rewritten}");
    }

    // The same in Visual Basic, its forms.
    const string VisualBasicHead = "Imports System\nImports System.IO\nImports System.Linq\nImports System.Text\nImports System.Collections.Generic\nPublic Module C\n";

    public static readonly (string Name, string Members)[] VisualBasicPaths =
    {
        ("joined with &", "Public Function F(rootPath As String, fileName As String) As String\n  Return rootPath & \"\\bin\\\" & fileName\nEnd Function"),
        ("joined with +", "Public Function F(rootPath As String, fileName As String) As String\n  Return rootPath + \"\\bin\\\" + fileName\nEnd Function"),
        ("String.Format", "Public Function F(rootPath As String, fileName As String) As String\n  Return String.Format(\"{0}\\bin\\{1}\", rootPath, fileName)\nEnd Function"),
        ("an interpolation", "Public Function F(rootPath As String, fileName As String) As String\n  Return $\"{rootPath}\\bin\\{fileName}\"\nEnd Function"),
        ("Path.Combine", "Public Function F(rootPath As String, fileName As String) As String\n  Return Path.Combine(rootPath, \"bin\\sub\", fileName)\nEnd Function"),
        ("kept in a local", "Public Function F(rootPath As String, fileName As String) As String\n  Dim filePath As String = rootPath\n  filePath &= \"\\bin\\\"\n  filePath &= fileName\n  Return filePath\nEnd Function"),
        ("a constant", "Const BinFolder As String = \"\\bin\\\"\nPublic Function F(rootPath As String, fileName As String) As String\n  Return rootPath & BinFolder & fileName\nEnd Function"),
        ("a char", "Public Function F(rootPath As String, fileName As String) As String\n  Return rootPath & \"\\\"c & fileName\nEnd Function"),
        ("chosen by If", "Public Function F(rootPath As String, fileName As String) As String\n  Return rootPath & If(fileName.Length > 3, \"\\bin\\\", \"\\obj\\\") & fileName\nEnd Function"),
        ("slashes made backslashes", "Public Function F(rootPath As String, fileName As String) As String\n  Dim filePath = rootPath & \"/bin/\" & fileName\n  Return filePath.Replace(\"/\", \"\\\")\nEnd Function"),
        ("trimmed, then joined", "Public Function F(rootPath As String, fileName As String) As String\n  Return rootPath.TrimEnd(\"\\\"c) & \"\\bin\\\" & fileName\nEnd Function"),
        ("in a lambda", "Public Function F(rootPath As String, fileName As String) As String\n  Dim filePath As Func(Of String, String) = Function(n) rootPath & \"\\bin\\\" & n\n  Return filePath(fileName)\nEnd Function"),
        ("a StringBuilder", "Public Function F(rootPath As String, fileName As String) As String\n  Dim filePath As New StringBuilder(rootPath)\n  filePath.Append(\"\\bin\\\").Append(fileName)\n  Return filePath.ToString()\nEnd Function"),
        ("given to a file API", "Public Function F(rootPath As String, fileName As String) As String\n  Return New FileInfo(rootPath & \"\\bin\\\" & fileName).FullName\nEnd Function"),
    };

    public static IEnumerable<object[]> VisualBasicPathCases => VisualBasicPaths.Select(p => new object[] { p.Name });

    [Theory]
    [MemberData(nameof(VisualBasicPathCases))]
    public void A_path_made_in_visual_basic_is_this_systems_once_rewritten(string name)
    {
        var source = VisualBasicHead + VisualBasicPaths.Single(p => p.Name == name).Members + "\nEnd Module\n";
        var rewritten = VisualBasic(source);
        Hold("VB", name, source, rewritten, VisualBasic(rewritten), VisualBasicMethod);
    }
}
