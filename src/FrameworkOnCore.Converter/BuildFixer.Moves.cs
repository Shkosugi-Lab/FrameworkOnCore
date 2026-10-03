using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FrameworkOnCore.Converter;

/// <summary>
/// Namespaces and types a package moved (rules/packages.json moveGroups, a group per choice: Entity Framework 4, .NET
/// Framework's own System.Data.Entity, is EF6 on .NET, its core types in System.Data.Entity.Core; WCF's service side is
/// CoreWCF's). Where the build does not find them
/// (CS0234: System.Data.Objects; CS0246: EntityState, in System.Data before), the sources name them where they are now: a
/// using directive or a qualified name rewritten, a using alias for a type. A round that finds any does only that: the
/// errors that follow from them (in the file and in others: the context that did not compile) are the next build's, not
/// stubbed now.
/// </summary>
public sealed partial class BuildFixer
{
    static readonly Regex missingInNamespace = new(@"The type or namespace name '(?<name>[^']+)' does not exist in the namespace '(?<namespace>[^']+)'", RegexOptions.Compiled);
    static readonly Regex missingType = new(@"The type or namespace name '(?<name>[\w]+)' could not be found|The name '(?<name>[\w]+)' does not exist in the current context", RegexOptions.Compiled);
    // A type both packages have, the one the sources find and the moved one another moved type takes (WCF's client has
    // System.ServiceModel.ConcurrencyMode, CoreWCF's ServiceBehavior takes CoreWCF.ConcurrencyMode): CS0266, CS0029, CS1503.
    static readonly Regex otherType = new(@"[Cc]annot (implicitly )?convert (type |from )'(?<from>[\w\.]+)' to '(?<to>[\w\.]+)'", RegexOptions.Compiled);

    bool IsMove(BuildError error)
    {
        if (error.Code == "CS0234" && missingInNamespace.Match(error.Message) is { Success: true } m)
            return MovedNamespace(m.Groups["namespace"].Value + "." + m.Groups["name"].Value) != null;
        // A moved type as a type (CS0246) or in an expression (CS0103: EntityState.Detached), where the file imports the
        // namespace it was in (the rule's old namespace: a local of that name is not it).
        if (error.Code is "CS0266" or "CS0029" or "CS1503")
            return OtherTypeMove(error) != null;
        return error.Code is "CS0246" or "CS0103" && missingType.Match(error.Message) is { Success: true } t &&
               TypeMove(error.File!, t.Groups["name"].Value) != null;
    }

    // The simple name of a type the sources find in an old namespace where its moved twin is wanted: the name, or null.
    string? OtherTypeMove(BuildError error)
    {
        if (otherType.Match(error.Message) is not { Success: true } m) return null;
        string from = m.Groups["from"].Value, to = m.Groups["to"].Value;
        var name = from[(from.LastIndexOf('.') + 1)..];
        if (name != to[(to.LastIndexOf('.') + 1)..]) return null;
        var group = TypeMove(error.File!, name);
        return group != null && group.TypeMoves[name] == to && group.TypeMoveOrigins.Contains(from[..Math.Max(0, from.LastIndexOf('.'))]) ? name : null;
    }

    // The group that moved a type the file names unqualified: one whose old namespaces the file imports.
    MoveGroup? TypeMove(string file, string name) =>
        rules.MoveGroups.FirstOrDefault(g => g.TypeMoves.ContainsKey(name) && ImportsOldNamespace(file, g));

    readonly Dictionary<string, string> sources = new(StringComparer.OrdinalIgnoreCase);

    // Whether the file imports a namespace the group's moved types were in before (System.Data for System.Data.Entity.EntityState).
    bool ImportsOldNamespace(string file, MoveGroup group)
    {
        if (!sources.TryGetValue(file, out var text)) sources[file] = text = File.Exists(file) ? File.ReadAllText(file) : "";
        return group.TypeMoveOrigins.Any(origin => Regex.IsMatch(text, $@"^\s*using\s+{Regex.Escape(origin)}\s*;", RegexOptions.Multiline));
    }

    // The namespace a name is in now, by the longest moved namespace it starts with: (old, new, the group's note), or null.
    (string From, string To, string Note)? MovedNamespace(string name) =>
        rules.MoveGroups.SelectMany(g => g.NamespaceMoves.Select(m => (m.Key, m.Value, g.Note)))
            .Where(m => name == m.Key || name.StartsWith(m.Key + ".", StringComparison.Ordinal))
            .OrderByDescending(m => m.Key.Length).Select(m => ((string, string, string)?)(m.Key, m.Value, m.Note)).FirstOrDefault();

    // The moves in a C# file: the names rewritten, the aliases added. True when the file changed.
    bool FixMoves(string file, List<BuildError> errors)
    {
        var original = File.ReadAllText(file);
        var text = SourceText.From(original);
        var root = (CompilationUnitSyntax)SourceLanguage.CSharp.Parse(text, file, SymbolsFor(file)).GetRoot();
        var renames = new Dictionary<SyntaxNode, (string Name, string Note)>();
        var aliases = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var error in errors.Where(IsMove).DistinctBy(e => (e.Line, e.Column, e.Code)))
        {
            if (error.Line < 1 || error.Line > text.Lines.Count) continue;
            if (error.Code is "CS0266" or "CS0029" or "CS1503")
            {
                var other = OtherTypeMove(error)!;
                aliases.Add($"{other} = global::{TypeMove(file, other)!.TypeMoves[other]}");
                continue;
            }
            if (error.Code is "CS0246" or "CS0103")
            {
                var name = missingType.Match(error.Message).Groups["name"].Value;
                aliases.Add($"{name} = global::{TypeMove(file, name)!.TypeMoves[name]}");
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
            renames[outermost] = (global + move.To + written.Substring(move.From.Length), move.Note);
        }
        if (renames.Count == 0 && aliases.Count == 0) return false;

        var newRoot = root.ReplaceNodes(renames.Keys, (o, _) =>
            (o is NameSyntax ? SyntaxFactory.ParseName(renames[o].Name) : (SyntaxNode)SyntaxFactory.ParseExpression(renames[o].Name)).WithTriviaFrom(o));
        foreach (var (node, (name, note)) in renames)
            report.Add(Report.Kind.Stub, $"{Relative(file)}:{text.Lines.GetLineFromPosition(node.SpanStart).LineNumber + 1}", $"{node} -> {name} ({note})");
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
