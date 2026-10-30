using System.Globalization;
using FrameworkOnCore.Converter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

namespace FrameworkOnCore.Tests;

/// <summary>
/// What BuildFixer does with the compiler's errors in a file (FixFile: the same in C# and Visual Basic, each language's
/// decisions in its syntax), with the namespaces and types a package moved (FixMoves), and with the places the analyzers
/// point at (Rewrite) - on the errors Roslyn reports for the source, as the build would.
/// </summary>
public sealed class BuildFixerFixTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-fix").FullName;
    readonly Report report = new();

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { }
    }

    BuildFixer Fixer() => new(report, [], root, RewriteHarness.Rules);

    string WriteSource(string name, string text)
    {
        var path = Path.Combine(root, name);
        File.WriteAllText(path, text);
        return path;
    }

    // The compiler's errors, as the build reports them (English, line and column from 1).
    static List<BuildFixer.BuildError> Errors(string file, Compilation compilation) =>
        compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).Select(d =>
        {
            var start = d.Location.GetLineSpan().StartLinePosition;
            return new BuildFixer.BuildError(file, start.Line + 1, start.Character + 1, d.Id, d.GetMessage(CultureInfo.GetCultureInfo("en")), null);
        }).ToList();

    static Compilation VisualBasic(string source) => VisualBasicCompilation.Create("App", [VisualBasicSyntaxTree.ParseText(source)],
        RewriteHarness.Framework, new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    static Compilation CSharp(string source) => CSharpCompilation.Create("App", [CSharpSyntaxTree.ParseText(source)],
        RewriteHarness.Framework, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    // ------------------------------------------------------------------------------------------
    // Visual Basic

    const string VisualBasicWidget = """
        Imports System

        Public Class Widget
            Inherits System.Data.Services.Client.DataServiceContext

            Private ReadOnly canvas As Object = New Missing.Canvas()

            Public Sub Draw()
                Dim brush As New Missing.Brush()
                Console.WriteLine("draw")
            End Sub

            Public Overrides Function Size() As Integer
                Return 1
            End Function

            Public Function Name() As String
                Return "widget"
            End Function
        End Class
        """;

    [Fact]
    public void Visual_basic_errors_are_fixed_where_they_are()
    {
        var file = WriteSource("Widget.vb", VisualBasicWidget);
        var fixer = Fixer();
        Assert.True(fixer.FixFile(file, Errors(file, VisualBasic(VisualBasicWidget))));

        var written = File.ReadAllText(file);
        Assert.DoesNotContain("Inherits", written);                                                    // the base .NET has not
        Assert.Matches(@"Private ReadOnly canvas As Object\s*\r?\n", written);                         // its initializer
        Assert.Contains("Throw New Global.System.PlatformNotSupportedException(", written);            // Draw's body
        Assert.DoesNotContain("Dim brush", written);
        Assert.Contains("Public Function Size() As Integer", written);                                 // Overrides removed
        Assert.Contains("Return \"widget\"", written);                                                 // untouched
        Assert.Empty(VisualBasic(written).GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        // The type whose base was removed, as for C#: its members' errors are .NET's, not the conversion's (StubCauses).
        Assert.Contains("Widget", fixer.Causes.TypesWithoutBase);
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Stub && e.Text.StartsWith("'Overrides' removed", StringComparison.Ordinal));
    }

    [Fact] // BC36908: an extension method .NET added (EncodingExtensions.GetBytes) where .NET Framework bound the call late
    public void A_visual_basic_call_dotnet_made_ambiguous_is_late_bound_again()
    {
        const string source = """
            Public Module Bytes
                Public Function Of(encoding As Object, text As String) As Object
                    Return encoding.GetBytes(text)
                End Function
            End Module
            """;
        var file = WriteSource("Bytes.vb", source);
        var error = new BuildFixer.BuildError(file, 3, source.Split('\n')[2].IndexOf("GetBytes", StringComparison.Ordinal) + 1, "BC36908",
            "Late-bound extension methods are not supported.", null);
        Assert.True(Fixer().FixFile(file, [error]));
        Assert.Contains("Return CObj(encoding).GetBytes(text)", File.ReadAllText(file));
    }

    [Fact]
    public void A_visual_basic_file_that_cannot_be_fixed_is_excluded_with_its_comment()
    {
        var file = WriteSource("Broken.vb", "Public Class Broken\nEnd Class\n");
        Assert.True(Fixer().FixFile(file, [new BuildFixer.BuildError(file, 99, 1, "BC30002", "Type 'X' is not defined.", null)]));
        Assert.StartsWith("' Excluded by FrameworkOnCore", File.ReadAllText(file));
    }

    // ------------------------------------------------------------------------------------------
    // C#: the same skeleton, the same decisions.

    const string CSharpWidget = """
        using System;
        using Missing.Things;

        namespace App
        {
            public class Widget : System.Data.Services.Client.DataServiceContext
            {
                readonly object canvas = new Missing.Canvas();

                public void Draw()
                {
                    var brush = new Missing.Brush();
                    Console.WriteLine("draw");
                }

                public override int Size() => 1;

                public string Name() => "widget";
            }
        }
        """;

    [Fact]
    public void Csharp_errors_are_fixed_where_they_are()
    {
        var file = WriteSource("Widget.cs", CSharpWidget);
        var fixer = Fixer();
        Assert.True(fixer.FixFile(file, Errors(file, CSharp(CSharpWidget))));

        var written = File.ReadAllText(file);
        Assert.DoesNotContain("Missing.Things", written);
        Assert.DoesNotContain("DataServiceContext", written);
        Assert.Matches(@"readonly object canvas\s*;", written);
        Assert.Contains("throw new global::System.PlatformNotSupportedException(", written);
        // The override's error comes once the base is gone (the next build's): the next round.
        Assert.True(fixer.FixFile(file, Errors(file, CSharp(written))));
        written = File.ReadAllText(file);
        Assert.Contains("public int Size() => 1;", written);
        Assert.Contains("public string Name() => \"widget\";", written);
        Assert.Empty(CSharp(written).GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        Assert.Contains("Widget", fixer.Causes.TypesWithoutBase);
    }

    [Fact] // System.Range: the application's type, as on .NET Framework
    public void A_name_dotnet_made_ambiguous_gets_an_alias()
    {
        const string source = "using System;\nusing App.Models;\nnamespace App.Models { public class Range { } }\nnamespace App { class Use { Range range; } }\n";
        var file = WriteSource("Use.cs", source);
        Assert.True(Fixer().FixFile(file, Errors(file, CSharp(source))));
        var written = File.ReadAllText(file);
        Assert.Contains("using Range = global::App.Models.Range;", written);
        Assert.Empty(CSharp(written).GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
    }

    // ------------------------------------------------------------------------------------------
    // FixMoves: Entity Framework 4's namespaces and types, where EF6 has them.

    [Fact]
    public void Moved_namespaces_and_types_are_named_where_they_are_now()
    {
        const string source = "using System.Data;\nusing System.Data.Objects;\nnamespace App { class Use { EntityState state; } }\n";
        var file = WriteSource("Moved.cs", source);
        var fixer = Fixer();
        Assert.True(fixer.FixMoves(file, Errors(file, CSharp(source))));
        var written = File.ReadAllText(file);
        Assert.Contains("using System.Data.Entity.Core.Objects;", written);
        Assert.Contains("using EntityState = global::System.Data.Entity.EntityState;", written);
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Stub && e.Text.StartsWith("System.Data.Objects -> System.Data.Entity.Core.Objects", StringComparison.Ordinal));
        // Nothing moved: nothing done.
        Assert.False(fixer.FixMoves(file, [new BuildFixer.BuildError(file, 3, 1, "CS0246", "The type or namespace name 'Unknown' could not be found", null)]));
    }

    // ------------------------------------------------------------------------------------------
    // Rewrite: the places the analyzers pointed at, rewritten in the file as SourceEdits does.

    [Fact]
    public void The_places_pointed_at_are_rewritten_in_the_file()
    {
        const string source = "using System; using System.IO;\npublic class C\n{\nstring Bin() => AppDomain.CurrentDomain.BaseDirectory + \"bin\\\\\";\n}";
        var file = WriteSource("Paths.cs", source);
        var fixer = Fixer();
        foreach (var place in RewriteHarness.PointedAtCSharp(source)) fixer.Point(file, place);
        Assert.True(fixer.Rewrite());
        Assert.Equal(RewriteHarness.CSharp(source), File.ReadAllText(file));
        Assert.Contains("global::FrameworkOnCore.WindowsPath.Native(\"bin\\\\\")", File.ReadAllText(file));
        // Once: the places are cleared.
        Assert.False(fixer.Rewrite());
    }
}
