using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FrameworkOnCore.Analyzers;

/// <summary>
/// What the analyzers tell the converter, through the build's output: the diagnostic's line and column, and at the end
/// of its message the node's length and, for some, what it is ("[len=12]", "[len=4,char]"). The converter finds the
/// node by its span, in C# and in Visual Basic alike.
/// </summary>
public static class Located
{
    public static Diagnostic Create(DiagnosticDescriptor descriptor, SyntaxNode node, string reason, string? tag = null) =>
        Diagnostic.Create(descriptor, node.GetLocation(), reason + Tail(node, tag));

    public static Diagnostic Create(DiagnosticDescriptor descriptor, SyntaxNode node, string reason, string second, string? tag) =>
        Diagnostic.Create(descriptor, node.GetLocation(), reason, second + Tail(node, tag));

    static string Tail(SyntaxNode node, string? tag) => $" [len={node.Span.Length}{(tag != null ? "," + tag : "")}]";

    /// <summary>A list of assembly names the converter passes as an MSBuild property (CompilerVisibleProperty).</summary>
    public static HashSet<string> Names(AnalyzerConfigOptions options, string property)
    {
        options.TryGetValue("build_property." + property, out var names);
        return new HashSet<string>((names ?? "").Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => n.Trim()), StringComparer.OrdinalIgnoreCase);
    }
}
