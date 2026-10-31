#nullable enable
using System.Linq;
using Microsoft.CodeAnalysis;
using CS = Microsoft.CodeAnalysis.CSharp;
using CSS = Microsoft.CodeAnalysis.CSharp.Syntax;
using VB = Microsoft.CodeAnalysis.VisualBasic;
using VBS = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace FrameworkOnCore.Analyzers;

/// <summary>
/// FOC1009's rewrite (FrameworkOnCore.Analyzers' EndInCatchAnalyzer): a catch that can catch Response.End's
/// ThreadAbortException, its code in try / finally with System.Web.Util.ResponseEnd.RaiseAgain() in the finally - the abort
/// raised again at the end of the catch, as .NET Framework raised it (whatever leaves the catch: its end, a return, a throw).
/// The converter's (SourceEdits) and FrameworkOnCore's System.Web's for what the server compiles (both link this file;
/// not in the analyzers' assembly: Visual Basic). The catch's own lines as they were; C#: catch (...) { try { ... } finally { ...RaiseAgain(); } },
/// Visual Basic: the Catch's statements between Try and Finally.
/// </summary>
static class EndInCatchRewrite
{
    public const string RaiseAgain = "System.Web.Util.ResponseEnd.RaiseAgain";

    /// <summary>The catch rewritten (C#: a CatchClauseSyntax, Visual Basic: a CatchBlockSyntax); null if it is neither, or done already.</summary>
    public static SyntaxNode? Rewrite(SyntaxNode node)
    {
        switch (node)
        {
            case CSS.CatchClauseSyntax clause:
            {
                if (clause.Block.ToString().Contains(RaiseAgain)) return null;
                var block = clause.Block;
                var text = "try { } catch { try " + block.WithoutTrivia().ToFullString() + " finally { global::" + RaiseAgain + "(); } }";
                if (CS.SyntaxFactory.ParseStatement(text) is not CSS.TryStatementSyntax parsed || parsed.Catches.Count != 1 || parsed.ContainsDiagnostics) return null;
                return clause.WithBlock(parsed.Catches[0].Block.WithTriviaFrom(block));
            }
            case VBS.CatchBlockSyntax catchBlock:
            {
                if (catchBlock.ToString().Contains(RaiseAgain)) return null;
                var statement = catchBlock.CatchStatement.ToFullString();
                // The file's line ends (the Catch line's).
                var nl = statement.EndsWith("\n") && !statement.EndsWith("\r\n") ? "\n" : "\r\n";
                if (!statement.EndsWith("\n")) statement += nl;
                var leading = catchBlock.CatchStatement.GetLeadingTrivia().ToFullString();
                var indent = leading.Substring(leading.LastIndexOf('\n') + 1);
                if (indent.Trim().Length > 0) indent = "";
                var body = string.Concat(catchBlock.Statements.Select(s => s.ToFullString()));
                if (body.Length > 0 && !body.EndsWith("\n")) body += nl;
                var text = "Try" + nl + statement +
                    indent + "    Try" + nl + body +
                    indent + "    Finally" + nl +
                    indent + "        Global." + RaiseAgain + "()" + nl +
                    indent + "    End Try" + nl +
                    "End Try";
                if (VB.SyntaxFactory.ParseExecutableStatement(text) is not VBS.TryBlockSyntax parsed || parsed.CatchBlocks.Count != 1 || parsed.ContainsDiagnostics) return null;
                return parsed.CatchBlocks[0];
            }
            default:
                return null;
        }
    }
}
