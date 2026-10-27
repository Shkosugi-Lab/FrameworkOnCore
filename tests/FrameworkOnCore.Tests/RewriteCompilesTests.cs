using static FrameworkOnCore.Tests.RewriteHarness;

namespace FrameworkOnCore.Tests;

/// <summary>
/// What the converter writes still compiles, whatever form the code it rewrites has (an argument passed as an array, by
/// name, out; a member of an anonymous type or a tuple; ?.; an interpolation; a constant; a method group; an expression
/// tree; Visual Basic's module, As New, New With). A rewrite that does not compile is a method the build stubs: a feature
/// that silently stops working (mojoPortal's CombinePath, DNN's language settings).
/// </summary>
public class RewriteCompilesTests
{
    const string Head = "using System; using System.IO; using System.Linq; using System.Text; using System.Globalization;\n" +
                        "using System.Linq.Expressions; using System.Security.Principal;\npublic class C\n{\n";

    [Theory]
    // paths: params given an array or its elements, a named argument, joined, searched for, in an interpolation
    [InlineData("string F(string[] p) => Path.Combine(p);")]
    [InlineData("bool F(string a, string b) => File.Exists(Path.Combine(a, b, \"bin\\\\x\"));")]
    [InlineData("bool F(string p) => File.Exists(path: p);")]
    [InlineData("string F(string root) => AppDomain.CurrentDomain.BaseDirectory + \"bin\\\\\" + root;")]
    [InlineData("int F(string filePath) => filePath.IndexOf(\"\\\\bin\\\\\");")]
    [InlineData("string F(string folder) => $\"{folder}\\\\bin\\\\{AppDomain.CurrentDomain.BaseDirectory}\";")]
    [InlineData("string F(string filePath) => filePath.Replace('/', '\\\\');")]
    [InlineData("string[] F(string[] p) => p.Select(Path.GetFileName).ToArray();")]
    // constants: a path, a separator char, one used where only a constant can be
    [InlineData("const string ConfigFolder = \"\\\\Config\\\\\"; string F() => ConfigFolder;")]
    [InlineData("const char DirSeparator = '\\\\'; string F(string path) => path + DirSeparator;")]
    [InlineData("const string UploadFolder = \"Uploads\\\\\"; string F(string s) { switch (s) { case UploadFolder: return s; default: return \"\"; } }")]
    [InlineData("[System.ComponentModel.DefaultValue(Folder)] public string P { get; set; } const string Folder = \"App\\\\Data\";")]
    // platform members: in an anonymous type, a tuple, with ?., in an expression tree, in nameof
    [InlineData("object F(CultureInfo l) => new { l.NativeName, l.DisplayName, l.Name };")]
    [InlineData("string F(CultureInfo l) { var t = (l.NativeName, 1); return t.NativeName; }")]
    [InlineData("string F() => WindowsIdentity.GetCurrent()?.Name;")]
    [InlineData("string F() => AppDomain.CurrentDomain?.RelativeSearchPath;")]
    [InlineData("string F(CultureInfo c) => c?.NativeName;")]
    [InlineData("Expression<Func<string, bool>> F() => p => p.StartsWith(\"bin\\\\x\") && Encoding.Default.GetBytes(p).Length > 0;")]
    [InlineData("string F() => nameof(CultureInfo.NativeName);")]
    [InlineData("byte[] F(string s) => Encoding.Default.GetBytes(s);")]
    [InlineData("Uri F(string s) => new Uri(s, UriKind.Relative);")]
    [InlineData("bool F(string s) => Uri.TryCreate(s, UriKind.Absolute, out var u) && u != null;")]
    // doubles made text: ToString, concatenation, Convert, string.Format, string.Join, an anonymous type's member, nullable
    [InlineData("string F(double d) => d.ToString() + d;")]
    [InlineData("string F(double? d) => d.ToString() + d;")]
    [InlineData("string F(double d) => string.Format(\"{0}\", d) + Convert.ToString(d) + $\"{d}\";")]
    [InlineData("string F(double[] a) => string.Join(\",\", a);")]
    [InlineData("object F(double d) => new { d, Half = d / 2 };")]
    [InlineData("string F(double d) { var t = (d, 1); return t.d.ToString(); }")]
    [InlineData("string F(float f) => f.ToString(\"N2\", CultureInfo.InvariantCulture);")]
    public void A_csharp_rewrite_compiles(string members)
    {
        var source = Head + members + "\n}";
        var written = CSharp(source);
        var errors = CompileCSharp(written);
        Assert.True(errors.Count == 0, string.Join("\n", errors) + "\n----\n" + written);
    }

    [Theory]
    // a module's constants, a class's, one without As
    [InlineData("Public Module M\n  Public Const UploadFolder As String = \"Uploads\\\"\n  Private Const BinFolder = \"bin\\\"\n  Function F() As String\n    Return AppDomain.CurrentDomain.BaseDirectory & UploadFolder & BinFolder\n  End Function\nEnd Module")]
    [InlineData("Public Class C\n  Public Const ConfigFolder As String = \"\\Config\\\"\n  Function F() As String\n    Return ConfigFolder\n  End Function\nEnd Class")]
    // As New, New With, ?., a tuple, an interpolation
    [InlineData("Public Class C\n  Function F(s As String) As Uri\n    Dim u As New Uri(s, UriKind.Relative)\n    Return u\n  End Function\nEnd Class")]
    [InlineData("Public Class C\n  Function F(l As Globalization.CultureInfo) As Object\n    Return New With {l.NativeName, l.Name}\n  End Function\nEnd Class")]
    [InlineData("Public Class C\n  Function F() As String\n    Return Security.Principal.WindowsIdentity.GetCurrent()?.Name\n  End Function\nEnd Class")]
    [InlineData("Public Class C\n  Function F(folder As String) As String\n    Return $\"{folder}\\bin\\{AppDomain.CurrentDomain.BaseDirectory}\"\n  End Function\nEnd Class")]
    [InlineData("Public Class C\n  Function F(p As String) As Boolean\n    Return IO.File.Exists(IO.Path.Combine(p, \"bin\\x\"))\n  End Function\nEnd Class")]
    // doubles made text
    [InlineData("Public Class C\n  Function F(d As Double) As String\n    Return d.ToString() & d & CStr(d)\n  End Function\nEnd Class")]
    [InlineData("Public Class C\n  Function F(d As Double) As Object\n    Return New With {d, .Half = d / 2}\n  End Function\nEnd Class")]
    public void A_visual_basic_rewrite_compiles(string source)
    {
        source = "Imports System\nImports System.IO\nImports System.Linq\n" + source;
        var written = VisualBasic(source);
        var errors = CompileVisualBasic(written);
        Assert.True(errors.Count == 0, string.Join("\n", errors) + "\n----\n" + written);
    }
}
