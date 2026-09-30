using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace FrameworkOnCore.Converter;

/// <summary>
/// A place the build pointed at for a rewrite: an analyzer's diagnostic (FOC1001-1006: its line and column, and the
/// node's length and what it is at the end of its message, FrameworkOnCore.Analyzers' Located) or the compiler's
/// (CS9258, SYSLIB0007: a line and column only).
/// </summary>
public sealed record Pointed(string Code, int Line, int Column, int? Length, string? Tag, string Message)
{
    static readonly Regex tail = new(@" \[len=(?<len>\d+)(,(?<tag>\w+))?\]", RegexOptions.Compiled);

    public static Pointed From(string code, int line, int column, string message)
    {
        var m = tail.Match(message);
        return m.Success
            ? new Pointed(code, line, column, int.Parse(m.Groups["len"].Value), m.Groups["tag"].Success ? m.Groups["tag"].Value : null, message.Remove(m.Index, m.Length))
            : new Pointed(code, line, column, null, null, message);
    }
}

/// <summary>
/// The rewrites of a file the build pointed at, in one pass (nested ones included: a literal in an argument wrapped as a
/// whole), written in the file's language (SourceLanguage). What each one does:
/// - FOC1001: WindowsPath.Native("bin\\"); a separator character: Path.DirectorySeparatorChar.
/// - FOC1002: a constant path: the literal as FOC1001, the declaration not constant (where its names are not used as
///   constants: the caller says).
/// - FOC1003: WindowsPath.TrimStartRelative(x, separators) for x.TrimStart(separators) joined to a folder.
/// - FOC1004: WindowsPath.Native(argument).
/// - FOC1005: AsyncDelegate.BeginInvoke(d, new object[] { args }, callback, state); (T)AsyncDelegate.EndInvoke(result).
/// - FOC1006: rules/packages.json platformReplacements (by the rule's number).
/// - FOC1007: Enumerable.Contains(array, x) for array.Contains(x) in an expression tree (C# 14 bound it to a span method).
/// - CS9258 (C#): field -> @field. SYSLIB0007 (C#): the .NET Framework default algorithm.
/// </summary>
public sealed class SourceEdits(SourceLanguage language, Rules rules)
{
    public sealed record Done(int Line, string Kind, string Text, Report.Kind ReportKind);

    // The algorithm each Create() returned on .NET Framework (machine.config's CryptoConfig defaults).
    static readonly Dictionary<string, string> frameworkDefaultAlgorithm = new()
    {
        ["HashAlgorithm"] = "System.Security.Cryptography.SHA1.Create()",
        ["KeyedHashAlgorithm"] = "new System.Security.Cryptography.HMACSHA1()",
        ["HMAC"] = "new System.Security.Cryptography.HMACSHA1()",
        // Rijndael with its default 128-bit block: AES.
        ["SymmetricAlgorithm"] = "System.Security.Cryptography.Aes.Create()",
        ["AsymmetricAlgorithm"] = "System.Security.Cryptography.RSA.Create()",
    };

    /// <summary>
    /// The file's text rewritten, and what was done (for the report). usableConstant: whether a constant's names may
    /// stop being constants (not used where only a constant can be).
    /// </summary>
    public (string Text, List<Done> Done, List<Pointed> Left) Apply(SourceText text, string path, IEnumerable<string> symbols,
        IReadOnlyCollection<Pointed> pointed, Func<IReadOnlyList<string>, bool, bool> usableConstant)
    {
        var root = language.Parse(text, path, symbols).GetRoot();
        // A node's edits, in order: what changes its shape (a call replaced) before what wraps it (WindowsPath.Native):
        // Path.Combine(root, WindowsPath.Native(WindowsPath.TrimStartRelative(filename, '\\', '/'))).
        var edits = new Dictionary<SyntaxNode, SortedList<int, Func<SyntaxNode, SyntaxNode?>>>();
        var done = new List<Done>();
        var left = new List<Pointed>();
        int LineOf(SyntaxNode node) => text.Lines.GetLineFromPosition(node.SpanStart).LineNumber + 1;
        string Native(SyntaxNode inner) => language.Call(language.Global("FrameworkOnCore.WindowsPath.Native"), new[] { inner.WithoutTrivia().ToString() });

        const int Shape = 0, Wrap = 1, Declaration = 2;
        void Add(SyntaxNode node, Func<SyntaxNode, string?> write, int order = Shape) =>
            Put(node, inner => write(inner) is { } replacement ? language.Expression(replacement).WithTriviaFrom(inner) : null, order);

        // One edit of each order for a node (two diagnostics of the same kind: once).
        void Put(SyntaxNode node, Func<SyntaxNode, SyntaxNode?> edit, int order)
        {
            if (!edits.TryGetValue(node, out var list)) edits[node] = list = new SortedList<int, Func<SyntaxNode, SyntaxNode?>>();
            if (!list.ContainsKey(order)) list[order] = edit;
        }

        var data = new List<(SyntaxNode Node, string Reason)>();
        foreach (var p in pointed.OrderBy(p => p.Line).ThenBy(p => p.Column))
        {
            if (p.Line < 1 || p.Line > text.Lines.Count) continue;
            var start = text.Lines[p.Line - 1].Start + p.Column - 1;
            if (p.Length is { } length)
            {
                if (start + length > text.Length) continue;
                var node = root.FindNode(new TextSpan(start, length), getInnermostNodeForTie: true);
                if (node.Span != new TextSpan(start, length)) { left.Add(p); continue; }
                switch (p.Code)
                {
                    case "FOC1001":
                        var isChar = p.Tag == "char";
                        Add(node, inner => isChar ? language.Global("System.IO.Path.DirectorySeparatorChar") : Native(inner), Wrap);
                        done.Add(new Done(LineOf(node), p.Code, $"{node} -> {(isChar ? "Path.DirectorySeparatorChar" : "WindowsPath.Native(...)")} (FOC1001, {p.Message})", Report.Kind.Platform));
                        break;
                    case "FOC1004":
                        Add(node, Native, Wrap);
                        data.Add((node, p.Message));
                        break;
                    case "FOC1002":
                        var declaration = language.ConstantDeclaration(node, out var names, out var isField);
                        if (declaration == null || !usableConstant(names, isField)) { left.Add(p); break; }
                        Add(node, Native, Wrap);
                        if (edits.ContainsKey(declaration)) break;
                        Put(declaration, inner => language.NotConstant(inner), Declaration);
                        done.Add(new Done(LineOf(declaration), p.Code, $"const {string.Join(", ", names)} -> {(isField ? "static readonly" : "a variable")}, WindowsPath.Native(...) (FOC1002: a constant path, not used where a constant has to be)", Report.Kind.Platform));
                        break;
                    case "FOC1003":
                        if (!language.TrySplitCall(node, out _, out var trim, out _)) { left.Add(p); break; }
                        var helper = trim == "Trim" ? "TrimRelative" : "TrimStartRelative";
                        Put(node, inner => language.TrySplitCall(inner, out var receiver, out _, out _) && receiver != null
                            ? language.Retarget(inner, language.Global("FrameworkOnCore.WindowsPath." + helper), receiver)
                            : null, Shape);
                        done.Add(new Done(LineOf(node), p.Code, $"{node} -> WindowsPath.{helper} (FOC1003: an absolute path joined to a folder keeps its root, as a Windows path keeps its drive)", Report.Kind.Platform));
                        break;
                    case "FOC1005":
                        var returns = Regex.Match(p.Message, @"\.(?<method>BeginInvoke|EndInvoke) returns (?<type>.+)$");
                        // The user chose to leave them (async-delegates: none): the call throws, reported.
                        if (!rules.IsChosen("async-delegates:compat"))
                        {
                            done.Add(new Done(LineOf(node), p.Code, $"FOC1005: {p.Message} (not rewritten: async-delegates is none; it throws PlatformNotSupportedException)", Report.Kind.Unsupported));
                            break;
                        }
                        if (!returns.Success || returns.Groups["type"].Value == "ref/out")
                        {
                            done.Add(new Done(LineOf(node), p.Code, $"FOC1005: {p.Message} (a delegate with ref/out parameters: its EndInvoke gives them back; not rewritten)", Report.Kind.Unsupported));
                            break;
                        }
                        var begin = returns.Groups["method"].Value == "BeginInvoke";
                        var type = returns.Groups["type"].Value;
                        Add(node, inner =>
                        {
                            if (!language.TrySplitCall(inner, out var target, out _, out var arguments) || target == null) return null;
                            if (begin && arguments.Count >= 2)
                                return language.Call(language.Global("FrameworkOnCore.AsyncDelegate.BeginInvoke"),
                                    new[] { target, language.ObjectArray(arguments.Take(arguments.Count - 2)), arguments[^2], arguments[^1] });
                            if (!begin && arguments.Count == 1)
                            {
                                var end = language.Call(language.Global("FrameworkOnCore.AsyncDelegate.EndInvoke"), arguments);
                                return type == "void" ? end : language.Cast(type, end);
                            }
                            return null;
                        });
                        done.Add(new Done(LineOf(node), p.Code, $"{p.Message.Split(' ').FirstOrDefault(w => w.Contains("Invoke"))}(...) -> FrameworkOnCore.AsyncDelegate (FOC1005: .NET has no asynchronous delegate call; the thread pool runs it)", Report.Kind.Platform));
                        break;
                    case "FOC1007":
                        // C# 14 bound an array's Contains in an expression tree to MemoryExtensions' (a span): Enumerable's, as the
                        // C# of .NET Framework's time bound it, which the query provider translates.
                        if (p.Tag is not ("Contains" or "SequenceEqual") || !language.TrySplitCall(node, out var spanReceiver, out _, out _) || spanReceiver == null)
                        {
                            done.Add(new Done(LineOf(node), p.Code, $"FOC1007: {p.Message} (not rewritten: Enumerable has no {p.Tag} of the same shape)", Report.Kind.Unsupported));
                            break;
                        }
                        var enumerable = p.Tag;
                        Add(node, inner => language.TrySplitCall(inner, out var receiver, out _, out var arguments) && receiver != null
                            ? language.Call(language.Global("System.Linq.Enumerable." + enumerable), new[] { receiver }.Concat(arguments))
                            : null);
                        done.Add(new Done(LineOf(node), p.Code, $"{node.WithoutTrivia()} -> Enumerable.{enumerable}(...) (FOC1007: in an expression tree C# 14 binds an array's {enumerable} to MemoryExtensions' span method, which a query provider such as Entity Framework does not translate)", Report.Kind.Platform));
                        break;
                    case "FOC1006":
                        var instanceCall = p.Tag?.EndsWith("_instance", StringComparison.Ordinal) == true;
                        if (p.Tag is not { } tag || !tag.StartsWith("rule", StringComparison.Ordinal) || !int.TryParse(tag[4..^(instanceCall ? "_instance".Length : 0)], out var number) ||
                            number >= rules.PlatformReplacements.Count)
                        {
                            left.Add(p);
                            break;
                        }
                        var rule = rules.PlatformReplacements[number];
                        var replacement = language.Global(rule.Replacement);
                        // A call of an instance method: its receiver is the replacement's first argument (log.WriteEntry(m) ->
                        // EventLogs.WriteEntry(log, m)); one without a receiver written is left.
                        if (rule.Replace == "call" && instanceCall)
                        {
                            if (!language.TrySplitCall(node, out var instanceReceiver, out _, out _) || instanceReceiver == null) { left.Add(p); break; }
                            Put(node, inner => language.TrySplitCall(inner, out var receiver, out _, out _) && receiver != null ? language.Retarget(inner, replacement, receiver) : null, Shape);
                        }
                        else if (rule.Replace == "call") Put(node, inner => language.Retarget(inner, replacement), Shape);
                        else Add(node, _ => replacement);
                        done.Add(new Done(LineOf(node), p.Code, $"{rule.Member}{(rule.Then != null ? "." + rule.Then : "")} -> {rule.Replacement} ({rule.Note})", Report.Kind.Platform));
                        break;
                    default:
                        left.Add(p);
                        break;
                }
                continue;
            }

            // The compiler's: a line and column (C#).
            var token = root.FindToken(Math.Min(start, Math.Max(0, text.Length - 1)));
            switch (p.Code)
            {
                // (C# 14 parses it as its keyword, the field expression; before, a name)
                case "CS9258" when token.Text == "field" && token.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax fieldName:
                    if (edits.ContainsKey(fieldName)) break;
                    Put(fieldName, inner => Microsoft.CodeAnalysis.CSharp.SyntaxFactory.IdentifierName(
                        Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Identifier(inner.GetLeadingTrivia(), Microsoft.CodeAnalysis.CSharp.SyntaxKind.IdentifierToken, "@field", "field", inner.GetTrailingTrivia())), Shape);
                    done.Add(new Done(LineOf(fieldName), p.Code, "field -> @field (CS9258: C# 14's field keyword; the source meant its type's member named field)", Report.Kind.Stub));
                    break;
                case "SYSLIB0007":
                    var invocation = token.Parent?.AncestorsAndSelf().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax>()
                        .FirstOrDefault(i => i.ArgumentList.Arguments.Count == 0 && i.Expression is Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax { Name.Identifier.Text: "Create" });
                    if (invocation == null) break;
                    var algorithm = ((Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax)invocation.Expression).Expression.ToString().Split('.').Last();
                    if (!frameworkDefaultAlgorithm.TryGetValue(algorithm, out var created)) break;
                    Add(invocation, _ => created);
                    done.Add(new Done(LineOf(invocation), p.Code, $"{invocation} -> {created} (SYSLIB0007: the .NET Framework default algorithm; .NET throws)", Report.Kind.Stub));
                    break;
            }
        }

        // The arguments are many (every file API taking a path from data): one entry per file.
        if (data.Count > 0)
            done.Add(new Done(0, "FOC1004",
                $"{data.Count} path argument(s) from data -> WindowsPath.Native(...) (FOC1004: {string.Join(", ", data.Select(d => d.Reason.Replace("A path from data passed to a file API: Windows' separators in it are not Linux's ", "")).Distinct().OrderBy(r => r, StringComparer.Ordinal).Take(4))})",
                Report.Kind.Platform));

        if (edits.Count == 0) return (text.ToString(), done, left);
        var rewritten = root.ReplaceNodes(edits.Keys, (original, inner) =>
        {
            foreach (var edit in edits[original].Values) inner = edit(inner) ?? inner;
            return inner;
        });
        return (rewritten.ToFullString(), done, left);
    }
}
