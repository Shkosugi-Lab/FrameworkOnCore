using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FrameworkOnCore.Converter;

/// <summary>
/// Namespaces and types a package moved (rules/packages.json namespaceMoves, typeMoves: Entity Framework 4, .NET Framework's
/// own System.Data.Entity, is EF6 on .NET, its core types in System.Data.Entity.Core). Where the build does not find them
/// (CS0234: System.Data.Objects; CS0246: EntityState, in System.Data before), the sources name them where they are now: a
/// using directive or a qualified name rewritten, a using alias for a type. A round that finds any does only that: the
/// errors that follow from them (in the file and in others: the context that did not compile) are the next build's, not
/// stubbed now.
/// </summary>
public sealed partial class BuildFixer
{
    static readonly Regex missingInNamespace = new(@"The type or namespace name '(?<name>[^']+)' does not exist in the namespace '(?<namespace>[^']+)'", RegexOptions.Compiled);
    static readonly Regex missingType = new(@"The type or namespace name '(?<name>[\w]+)' could not be found|The name '(?<name>[\w]+)' does not exist in the current context", RegexOptions.Compiled);

    bool IsMove(BuildError error)
    {
        if (error.Code == "CS0234" && missingInNamespace.Match(error.Message) is { Success: true } m)
            return MovedNamespace(m.Groups["namespace"].Value + "." + m.Groups["name"].Value) != null;
        // A moved type as a type (CS0246) or in an expression (CS0103: EntityState.Detached), where the file imports the
        // namespace it was in (the rule's old namespace: a local of that name is not it).
        return error.Code is "CS0246" or "CS0103" && missingType.Match(error.Message) is { Success: true } t &&
               rules.TypeMoves.TryGetValue(t.Groups["name"].Value, out var moved) && ImportsOldNamespace(error.File!, moved);
    }

    readonly Dictionary<string, string> sources = new(StringComparer.OrdinalIgnoreCase);

    // Whether the file imports the namespace the moved type was in before (System.Data for System.Data.Entity.EntityState).
    bool ImportsOldNamespace(string file, string _)
    {
        if (!sources.TryGetValue(file, out var text)) sources[file] = text = File.Exists(file) ? File.ReadAllText(file) : "";
        return rules.TypeMoveOrigins.Any(origin => Regex.IsMatch(text, $@"^\s*using\s+{Regex.Escape(origin)}\s*;", RegexOptions.Multiline));
    }

    // The namespace a name is in now, by the longest moved namespace it starts with: (old, new), or null.
    (string From, string To)? MovedNamespace(string name) =>
        rules.NamespaceMoves.Where(m => name == m.Key || name.StartsWith(m.Key + ".", StringComparison.Ordinal))
            .OrderByDescending(m => m.Key.Length).Select(m => ((string, string)?)(m.Key, m.Value)).FirstOrDefault();

    // The moves in a C# file: the names rewritten, the aliases added. True when the file changed.
    bool FixMoves(string file, List<BuildError> errors)
    {
        var original = File.ReadAllText(file);
        var text = SourceText.From(original);
        var root = (CompilationUnitSyntax)SourceLanguage.CSharp.Parse(text, file, SymbolsFor(file)).GetRoot();
        var renames = new Dictionary<SyntaxNode, string>();
        var aliases = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var error in errors.Where(IsMove).DistinctBy(e => (e.Line, e.Column, e.Code)))
        {
            if (error.Line < 1 || error.Line > text.Lines.Count) continue;
            if (error.Code is "CS0246" or "CS0103")
            {
                var name = missingType.Match(error.Message).Groups["name"].Value;
                aliases.Add($"{name} = global::{rules.TypeMoves[name]}");
                continue;
            }
            var position = Math.Min(text.Lines[error.Line - 1].Start + error.Column - 1, Math.Max(0, text.Length - 1));
            // The whole qualified name (System.Data.Objects.DataClasses.EntityObject), in a using directive or in code.
            var node = root.FindToken(position).Parent;
            var outermost = node?.AncestorsAndSelf().TakeWhile(n => n is QualifiedNameSyntax or MemberAccessExpressionSyntax or AliasQualifiedNameSyntax or IdentifierNameSyntax)
                .LastOrDefault();
            if (outermost == null) continue;
            var written = outermost.ToString().Replace("global::", "");
            if (MovedNamespace(written) is not { } move) continue;
            var global = outermost.ToString().StartsWith("global::", StringComparison.Ordinal) ? "global::" : "";
            renames[outermost] = global + move.To + written.Substring(move.From.Length);
        }
        if (renames.Count == 0 && aliases.Count == 0) return false;

        var newRoot = root.ReplaceNodes(renames.Keys, (o, _) =>
            (o is NameSyntax ? SyntaxFactory.ParseName(renames[o]) : (SyntaxNode)SyntaxFactory.ParseExpression(renames[o])).WithTriviaFrom(o));
        foreach (var (node, name) in renames)
            report.Add(Report.Kind.Stub, $"{Relative(file)}:{text.Lines.GetLineFromPosition(node.SpanStart).LineNumber + 1}", $"{node} -> {name} ({rules.NamespaceMovesNote.Split(':')[0]})");
        foreach (var alias in aliases)
        {
            newRoot = ((CompilationUnitSyntax)newRoot).AddUsings(SyntaxFactory.ParseCompilationUnit($"using {alias};\n").Usings[0]);
            report.Add(Report.Kind.Stub, Relative(file), $"using {alias} added (a type the package moved)");
        }
        var result = newRoot.ToFullString();
        if (result == original) return false;
        File.WriteAllText(file, result, new UTF8Encoding(false));
        sources.Remove(file);
        pointed.Remove(file);
        return true;
    }
}
