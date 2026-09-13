using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>Replacing a file's using list without moving what sits above it.</summary>
internal static class SyntaxUsings
{
    /// <summary>
    /// Gives the file this using list, keeping the header trivia at the top of the file.
    ///
    /// WithUsings alone puts the new directives ahead of everything, because in a file with
    /// no usings the header - #pragma, #define, #if, the licence comment - is leading
    /// trivia on the first token of the namespace. A using in front of #define is
    /// CS1032 ("cannot define/undefine preprocessor symbols after first token"), which is
    /// what Lucene.Net's NullableAttributes.cs hit.
    /// </summary>
    public static CompilationUnitSyntax Replace(
        CompilationUnitSyntax unit,
        IEnumerable<UsingDirectiveSyntax> usings)
    {
        var header = unit.GetLeadingTrivia();
        return unit
            .WithLeadingTrivia(SyntaxTriviaList.Empty)
            .WithUsings(SyntaxFactory.List(usings))
            .WithLeadingTrivia(header);
    }
}
