using System.Globalization;
using FrameworkOnCore.Converter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

namespace FrameworkOnCore.Tests;

/// <summary>
/// BuildFixer (FixFile) on the kinds of errors a conversion's build has besides a type .NET has not: a member a type has not
/// (CS0117, CS1061, BC30456), a call with the arguments of an overload .NET has not (CS1501, CS1729, BC30516, BC30057), a name
/// not there (CS0103, BC30451), a conversion .NET does not make (CS0029), each in a method's body, a property's, a
/// constructor's, a field's initializer; and two places with errors at once (BuildFixerSweepTests' places, in pairs). Round
/// after round until the source compiles: the fix where the error is (the member's body stubbed, the initializer
/// removed), the file not excluded, the code around kept, nothing thrown.
/// </summary>
public sealed class BuildFixerKindsTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-kinds").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { }
    }

    static List<BuildFixer.BuildError> Errors(string file, Compilation compilation) =>
        compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).Select(d =>
        {
            var start = d.Location.GetLineSpan().StartLinePosition;
            return new BuildFixer.BuildError(file, start.Line + 1, start.Character + 1, d.Id, d.GetMessage(CultureInfo.GetCultureInfo("en")), null);
        }).ToList();

    static Compilation CSharp(string source) => CSharpCompilation.Create("App", [CSharpSyntaxTree.ParseText(source)],
        RewriteHarness.Framework, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    static Compilation VisualBasic(string source) => VisualBasicCompilation.Create("App", [VisualBasicSyntaxTree.ParseText(source)],
        RewriteHarness.Framework, new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    (string Text, List<BuildFixer.BuildError> Left, List<string> Codes) Sweep(string name, string source, Func<string, Compilation> compile)
    {
        var file = Path.Combine(root, name);
        File.WriteAllText(file, source);
        var fixer = new BuildFixer(new Report(), [], root, RewriteHarness.Rules);
        var codes = Errors(file, compile(source)).Select(e => e.Code).Distinct().ToList();
        for (var round = 0; round < 8; round++)
        {
            var errors = Errors(file, compile(File.ReadAllText(file)));
            if (errors.Count == 0) return (File.ReadAllText(file), errors, codes);
            if (!fixer.FixFile(file, errors)) return (File.ReadAllText(file), errors, codes);
        }
        return (File.ReadAllText(file), Errors(file, compile(File.ReadAllText(file))), codes);
    }

    static readonly (string Kind, string Expression)[] CSharpKinds =
    {
        ("a static member a type has not", "Math.Missing(1)"),
        ("an instance member a type has not", "\"a\".Missing()"),
        ("an overload .NET has not", "Math.Abs(1, 2, 3)"),
        ("a constructor .NET has not", "new System.Text.StringBuilder(1, 2, 3, 4, 5)"),
        ("a name not there", "MissingThing.Run()"),
        ("a type not there", "new Missing.Thing()"),
    };

    static readonly (string Place, Func<string, string> Member)[] CSharpPlaces =
    {
        ("a method's body", e => $"public object Make() {{ var x = {e}; return x; }}"),
        ("a property's body", e => $"public object Value {{ get {{ return {e}; }} }}"),
        ("a constructor's body", e => $"public Widget() {{ object x = {e}; }}"),
        ("a field's initializer", e => $"static object field = {e};"),
        ("a lambda in a method", e => $"public Func<object> Later() {{ return () => {e}; }}"),
    };

    public static IEnumerable<object[]> CSharpCases() =>
        from k in CSharpKinds from p in CSharpPlaces select new object[] { k.Kind, p.Place };

    [Theory]
    [MemberData(nameof(CSharpCases))]
    public void Csharp_errors_of_each_kind_are_fixed_where_they_are(string kind, string place)
    {
        var expression = CSharpKinds.Single(k => k.Kind == kind).Expression;
        var member = CSharpPlaces.Single(p => p.Place == place).Member(expression);
        var source = $$"""
            using System;

            namespace App
            {
                public class Widget
                {
                    {{member}}

                    public string Keep() => "kept";
                }

                public class Other { public string KeepToo() => "kept too"; }
            }
            """;
        var (text, left, codes) = Sweep("Widget.cs", source, CSharp);
        Assert.NotEmpty(codes);
        Assert.True(left.Count == 0, $"{kind} in {place} ({string.Join(",", codes)}): still {string.Join("; ", left.Select(e => e.Code + " " + e.Message))}\n{text}");
        Assert.DoesNotContain("Excluded by FrameworkOnCore", text);
        Assert.Contains("\"kept\"", text);
        Assert.Contains("\"kept too\"", text);
    }

    static readonly (string Kind, string Expression)[] VisualBasicKinds =
    {
        ("a static member a type has not", "Math.Missing(1)"),
        ("an instance member a type has not", "\"a\".Missing()"),
        ("an overload .NET has not", "Math.Abs(1, 2, 3)"),
        ("a constructor .NET has not", "New System.Text.StringBuilder(1, 2, 3, 4, 5)"),
        ("a name not there", "MissingThing.Run()"),
        ("a type not there", "New Missing.Thing()"),
    };

    static readonly (string Place, Func<string, string> Member)[] VisualBasicPlaces =
    {
        ("a method's body", e => $"Public Function Make() As Object\n        Dim x = {e}\n        Return x\n    End Function"),
        ("a property's body", e => $"Public ReadOnly Property Value As Object\n        Get\n            Return {e}\n        End Get\n    End Property"),
        ("a constructor's body", e => $"Public Sub New()\n        Dim x As Object = {e}\n    End Sub"),
        ("a field's initializer", e => $"Private Shared field As Object = {e}"),
        ("a lambda in a method", e => $"Public Function Later() As Func(Of Object)\n        Return Function() {e}\n    End Function"),
    };

    public static IEnumerable<object[]> VisualBasicCases() =>
        from k in VisualBasicKinds from p in VisualBasicPlaces select new object[] { k.Kind, p.Place };

    [Theory]
    [MemberData(nameof(VisualBasicCases))]
    public void Visual_basic_errors_of_each_kind_are_fixed_where_they_are(string kind, string place)
    {
        var expression = VisualBasicKinds.Single(k => k.Kind == kind).Expression;
        var member = VisualBasicPlaces.Single(p => p.Place == place).Member(expression);
        var source = $$"""
            Option Strict On
            Imports System

            Public Class Widget
                {{member}}

                Public Function Keep() As String
                    Return "kept"
                End Function
            End Class

            Public Class Other
                Public Function KeepToo() As String
                    Return "kept too"
                End Function
            End Class
            """;
        var (text, left, codes) = Sweep("Widget.vb", source, VisualBasic);
        Assert.NotEmpty(codes);
        Assert.True(left.Count == 0, $"{kind} in {place} ({string.Join(",", codes)}): still {string.Join("; ", left.Select(e => e.Code + " " + e.Message))}\n{text}");
        Assert.DoesNotContain("Excluded by FrameworkOnCore", text);
        Assert.Contains("\"kept\"", text);
        Assert.Contains("\"kept too\"", text);
    }

    // ---- two places with errors at once: each place with the three after it (BuildFixerSweepTests' places, wrapped around)

    public static IEnumerable<object[]> CSharpPairs()
    {
        var places = BuildFixerSweepTests.CSharpPlaces;
        for (var i = 0; i < places.Length; i++)
            for (var step = 1; step <= 3; step++)
            {
                var (a, b) = (places[i], places[(i + step) % places.Length]);
                // One class's bases, one constraint: two of them do not make one source.
                if (a.Bases.Length > 0 && b.Bases.Length > 0) continue;
                yield return new object[] { a.Name, b.Name };
            }
    }

    [Theory]
    [MemberData(nameof(CSharpPairs))]
    public void Csharp_two_places_at_once(string first, string second)
    {
        var (a, b) = (BuildFixerSweepTests.CSharpPlaces.Single(p => p.Name == first), BuildFixerSweepTests.CSharpPlaces.Single(p => p.Name == second));
        var bases = a.Bases.Length > 0 ? a.Bases : b.Bases;
        var generic = bases.StartsWith("<", StringComparison.Ordinal);
        // using directives before the assembly's attributes (C#'s order: CS1529 otherwise, the source's own error).
        var heads = new[] { a.Usings, b.Usings }.Distinct().Where(h => h.Length > 0).OrderBy(h => h.StartsWith("using", StringComparison.Ordinal) ? 0 : 1);
        var source = $$"""
            using System;
            {{string.Join("\n", heads)}}

            namespace App
            {
                {{a.Before}}{{b.Before}}
                public class Widget{{(generic ? bases : " " + bases)}}
                {
                    {{a.Members}}
                    {{b.Members.Replace("Make(", "Make2(").Replace("Use(", "Use2(").Replace("Property", "Property2").Replace(" field", " field2").Replace("Take(", "Take2(")}}

                    public string Keep() => "kept";
                }

                {{a.After}}
                {{b.After.Replace("Callback", "Callback2").Replace("Extensions", "Extensions2")}}

                public class Other { public string KeepToo() => "kept too"; }
            }
            """;
        var (text, left, _) = Sweep("Widget.cs", source, CSharp);
        Assert.True(left.Count == 0, $"{first} + {second}: still {string.Join("; ", left.Select(e => e.Code + " " + e.Message))}\n{text}");
        Assert.DoesNotContain("Excluded by FrameworkOnCore", text);
        Assert.Contains("\"kept too\"", text);
    }
}
