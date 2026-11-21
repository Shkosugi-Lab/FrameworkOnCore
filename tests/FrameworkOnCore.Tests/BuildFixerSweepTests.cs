using System.Globalization;
using FrameworkOnCore.Converter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

namespace FrameworkOnCore.Tests;

/// <summary>
/// BuildFixer (FixFile) on a type .NET has not in every place a type can be, in C# and in Visual Basic, alone and with
/// others in the same list or line: round after round as the build does (the compiler's errors, the fixes, again), until
/// the source compiles. The fixes stay where the errors are: the file is not excluded for an error a member's removal or
/// a body's stub answers, the code around them is kept, and the fixer neither throws nor goes round without end.
/// </summary>
public sealed class BuildFixerSweepTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-sweep").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { }
    }

    // The places (name, the source's text in the class or around it), C#: "Missing.*" is the type .NET has not.
    public static readonly (string Name, string Usings, string Before, string Bases, string Members, string After)[] CSharpPlaces =
    {
        ("attribute", "", "[Missing.A]", "", "", ""),
        ("attributes, all missing", "", "[Missing.A, Missing.B]", "", "", ""),
        ("attributes, one missing", "", "[Missing.A, Obsolete]", "", "", ""),
        ("attribute lists, all missing", "", "[Missing.A][Missing.B]", "", "", ""),
        ("base", "", "", ": Missing.Base", "", ""),
        ("bases, all missing", "", "", ": Missing.Base, Missing.IOne", "", ""),
        ("bases, one missing", "", "", ": Exception, Missing.IOne", "", ""),
        ("generic constraint", "", "", "<T> where T : Missing.Entity", "", ""),
        ("field", "", "", "", "Missing.Thing field;", ""),
        ("field initializer", "", "", "", "object field = new Missing.Thing();", ""),
        ("fields, two initializers", "", "", "", "object a = new Missing.Thing(), b = new Missing.Other();", ""),
        ("property", "", "", "", "public Missing.Thing Property { get; set; }", ""),
        ("property initializer", "", "", "", "public object Property { get; } = new Missing.Thing();", ""),
        ("expression-bodied property", "", "", "", "public object Property => new Missing.Thing();", ""),
        ("return type", "", "", "", "Missing.Thing Make() => null;", ""),
        ("parameter", "", "", "", "void Take(Missing.Thing thing) { }", ""),
        ("parameter attribute", "", "", "", "void Take([Missing.A] int x) { }", ""),
        ("local", "", "", "", "void Use() { Missing.Thing thing = null; }", ""),
        ("four on one line", "", "", "", "void Use() { var d = new System.Collections.Generic.Dictionary<Missing.Key, Missing.Value>(); System.Collections.Generic.Dictionary<Missing.Key, Missing.Value> e = d; }", ""),
        ("cast", "", "", "", "object Cast(object o) => (Missing.Thing)o;", ""),
        ("typeof", "", "", "", "Type type = typeof(Missing.Thing);", ""),
        ("catch", "", "", "", "void Catch() { try { } catch (Missing.Failure) { } }", ""),
        ("event", "", "", "", "event Missing.Handler Happened;", ""),
        ("lambda field", "", "", "", "Func<object> make = () => new Missing.Thing();", ""),
        ("method constraint", "", "", "", "void Make<T>() where T : Missing.Entity { }", ""),
        ("missing member", "", "", "", "int count = string.Missing;", ""),
        ("override of a missing member", "", "", "", "public override string Missing() => \"\";", ""),
        ("constructor initializer", "", "", ": Exception", "public Widget() : base(new Missing.Thing().ToString()) { }", ""),
        ("nested type's base", "", "", "", "class Nested : Missing.Base { }", ""),
        ("explicit implementation", "", "", ": Missing.IOne", "void Missing.IOne.Run() { }", ""),
        ("using", "using Missing;", "", "", "", ""),
        ("using alias", "using Alias = Missing.Thing;", "", "", "", ""),
        ("assembly attribute", "[assembly: Missing.A]", "", "", "", ""),
        ("delegate", "", "", "", "", "delegate void Callback(Missing.Thing thing);"),
        ("extension method", "", "", "", "", "static class Extensions { public static void Run(this Missing.Thing thing) { } }"),
        ("everywhere", "using Missing;", "[Missing.A, Missing.B]", ": Missing.Base, Missing.IOne",
            "[Missing.C, Obsolete] Missing.Thing Make([Missing.D] Missing.Thing x) { Missing.Other y = null; return x; }", ""),
    };

    public static readonly (string Name, string Imports, string Before, string Inherits, string Members, string After)[] VisualBasicPlaces =
    {
        ("attribute", "", "<Missing.A>", "", "", ""),
        ("attributes, all missing", "", "<Missing.A, Missing.B>", "", "", ""),
        ("attributes, one missing", "", "<Missing.A, Obsolete>", "", "", ""),
        ("attribute lists, all missing", "", "<Missing.A><Missing.B>", "", "", ""),
        ("base", "", "", "Inherits Missing.Base", "", ""),
        ("base and interface missing", "", "", "Inherits Missing.Base\n    Implements Missing.IOne", "", ""),
        ("interfaces, all missing", "", "", "Implements Missing.IOne, Missing.ITwo", "", ""),
        ("interfaces, one missing", "", "", "Implements IDisposable, Missing.IOne", "Public Sub Dispose() Implements IDisposable.Dispose\n    End Sub", ""),
        ("generic constraint", "", "", "(Of T As Missing.Entity)", "", ""),
        ("field", "", "", "", "Private field As Missing.Thing", ""),
        ("field initializer", "", "", "", "Private field As Object = New Missing.Thing()", ""),
        ("fields, two initializers", "", "", "", "Private a As Object = New Missing.Thing(), b As Object = New Missing.Other()", ""),
        ("property", "", "", "", "Public Property Thing As Missing.Thing", ""),
        ("property initializer", "", "", "", "Public Property Thing As Object = New Missing.Thing()", ""),
        ("return type", "", "", "", "Function Make() As Missing.Thing\n        Return Nothing\n    End Function", ""),
        ("parameter", "", "", "", "Sub Take(thing As Missing.Thing)\n    End Sub", ""),
        ("parameter attribute", "", "", "", "Sub Take(<Missing.A> x As Integer)\n    End Sub", ""),
        ("local", "", "", "", "Sub Use()\n        Dim thing As Missing.Thing = Nothing\n    End Sub", ""),
        ("four on one line", "", "", "", "Sub Use()\n        Dim d As New System.Collections.Generic.Dictionary(Of Missing.Key, Missing.Value)(), e As System.Collections.Generic.Dictionary(Of Missing.Key, Missing.Value) = d\n    End Sub", ""),
        ("cast", "", "", "", "Function Cast(o As Object) As Object\n        Return CType(o, Missing.Thing)\n    End Function", ""),
        ("GetType", "", "", "", "Private type As Type = GetType(Missing.Thing)", ""),
        ("catch", "", "", "", "Sub CatchIt()\n        Try\n        Catch ex As Missing.Failure\n        End Try\n    End Sub", ""),
        ("event", "", "", "", "Public Event Happened As Missing.Handler", ""),
        ("lambda field", "", "", "", "Private make As Func(Of Object) = Function() New Missing.Thing()", ""),
        ("method constraint", "", "", "", "Sub Make(Of T As Missing.Entity)()\n    End Sub", ""),
        ("missing member", "", "", "", "Private count As Integer = String.Missing", ""),
        ("override of a missing member", "", "", "", "Public Overrides Function Missing() As String\n        Return \"\"\n    End Function", ""),
        ("constructor's base call", "", "", "Inherits Exception", "Sub New()\n        MyBase.New(New Missing.Thing().ToString())\n    End Sub", ""),
        ("nested type's base", "", "", "", "Class Nested\n        Inherits Missing.Base\n    End Class", ""),
        ("Implements clause", "", "", "Implements Missing.IOne", "Sub Run() Implements Missing.IOne.Run\n    End Sub", ""),
        ("WithEvents and Handles", "", "", "", "Private WithEvents source As Missing.Source\n    Sub OnIt() Handles source.Happened\n    End Sub", ""),
        ("imports", "Imports Missing", "", "", "", ""),
        ("imports alias", "Imports Alias = Missing.Thing", "", "", "", ""),
        ("assembly attribute", "<Assembly: Missing.A>", "", "", "", ""),
        ("delegate", "", "", "", "", "Delegate Sub Callback(thing As Missing.Thing)"),
        ("extension method", "", "", "", "", "Module Extensions\n    <System.Runtime.CompilerServices.Extension>\n    Sub Run(thing As Missing.Thing)\n    End Sub\nEnd Module"),
        ("everywhere", "Imports Missing", "<Missing.A, Missing.B>", "Inherits Missing.Base\n    Implements Missing.IOne",
            "<Missing.C, Obsolete>\n    Function Make(<Missing.D> x As Missing.Thing) As Missing.Thing\n        Dim y As Missing.Other = Nothing\n        Return x\n    End Function", ""),
    };

    public static IEnumerable<object[]> CSharpCases => CSharpPlaces.Select(p => new object[] { p.Name });
    public static IEnumerable<object[]> VisualBasicCases => VisualBasicPlaces.Select(p => new object[] { p.Name });

    static string CSharpSource(string name)
    {
        var p = CSharpPlaces.Single(c => c.Name == name);
        var generic = p.Bases.StartsWith("<", StringComparison.Ordinal);
        return $$"""
            using System;
            {{p.Usings}}

            namespace App
            {
                {{p.Before}}
                public class Widget{{(generic ? p.Bases : " " + p.Bases)}}
                {
                    {{p.Members}}

                    public string Keep() => "kept";
                }

                {{p.After}}

                public class Other
                {
                    public string KeepToo() => "kept too";
                }
            }
            """;
    }

    static string VisualBasicSource(string name)
    {
        var p = VisualBasicPlaces.Single(c => c.Name == name);
        var generic = p.Inherits.StartsWith("(", StringComparison.Ordinal);
        return $$"""
            Imports System
            {{p.Imports}}

            {{p.Before}}
            Public Class Widget{{(generic ? p.Inherits : "")}}
                {{(generic ? "" : p.Inherits)}}

                {{p.Members}}

                Public Function Keep() As String
                    Return "kept"
                End Function
            End Class

            {{p.After}}

            Public Class Other
                Public Function KeepToo() As String
                    Return "kept too"
                End Function
            End Class
            """;
    }

    static List<BuildFixer.BuildError> Errors(string file, Compilation compilation) =>
        compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).Select(d =>
        {
            var start = d.Location.GetLineSpan().StartLinePosition;
            return new BuildFixer.BuildError(file, start.Line + 1, start.Character + 1, d.Id, d.GetMessage(CultureInfo.GetCultureInfo("en")), null);
        }).ToList();

    // The rounds of the build: its errors, the fixes, again; the source as it ends and how many rounds it took.
    (string Text, int Rounds, List<BuildFixer.BuildError> Left) Sweep(string fileName, string source, Func<string, Compilation> compile)
    {
        var file = Path.Combine(root, fileName);
        File.WriteAllText(file, source);
        var fixer = new BuildFixer(new Report(), [], root, RewriteHarness.Rules);
        for (var round = 1; round <= 8; round++)
        {
            var errors = Errors(file, compile(File.ReadAllText(file)));
            if (errors.Count == 0) return (File.ReadAllText(file), round - 1, errors);
            if (!fixer.FixFile(file, errors)) return (File.ReadAllText(file), round, errors);
        }
        return (File.ReadAllText(file), 9, Errors(file, compile(File.ReadAllText(file))));
    }

    static Compilation CSharp(string source) => CSharpCompilation.Create("App", [CSharpSyntaxTree.ParseText(source)],
        RewriteHarness.Framework, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    static Compilation VisualBasic(string source) => VisualBasicCompilation.Create("App", [VisualBasicSyntaxTree.ParseText(source)],
        RewriteHarness.Framework, new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    [Theory]
    [MemberData(nameof(CSharpCases))]
    public void Csharp_missing_types_are_fixed_where_they_are(string place)
    {
        var (text, rounds, left) = Sweep("Widget.cs", CSharpSource(place), CSharp);
        Assert.True(left.Count == 0, $"{place}: still failing after {rounds} rounds: {string.Join("; ", left.Select(e => e.Code + " " + e.Message))}\n{text}");
        Assert.DoesNotContain("Excluded by FrameworkOnCore", text);
        Assert.Contains("\"kept too\"", text);
        Assert.Contains("\"kept\"", text);
    }

    [Theory]
    [MemberData(nameof(VisualBasicCases))]
    public void Visual_basic_missing_types_are_fixed_where_they_are(string place)
    {
        var (text, rounds, left) = Sweep("Widget.vb", VisualBasicSource(place), VisualBasic);
        Assert.True(left.Count == 0, $"{place}: still failing after {rounds} rounds: {string.Join("; ", left.Select(e => e.Code + " " + e.Message))}\n{text}");
        Assert.DoesNotContain("Excluded by FrameworkOnCore", text);
        Assert.Contains("\"kept too\"", text);
        Assert.Contains("\"kept\"", text);
    }
}
