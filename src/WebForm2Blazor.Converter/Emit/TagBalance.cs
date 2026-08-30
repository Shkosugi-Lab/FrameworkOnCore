using System.Text.RegularExpressions;

namespace WebForm2Blazor.Converter.Emit;

/// <summary>
/// Determines whether an HTML fragment has matching tags.
/// Razor rejects unmatched tags as syntax errors, so this is used to decide whether
/// WebForms HeaderTemplate / FooterTemplate content can be emitted as-is.
/// </summary>
public static partial class TagBalance
{
    private static readonly HashSet<string> VoidElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "area", "base", "br", "col", "embed", "hr", "img", "input",
        "link", "meta", "param", "source", "track", "wbr",
    };

    public static bool IsBalanced(string html)
    {
        var stack = new Stack<string>();

        foreach (Match match in TagRegex().Matches(html))
        {
            var name = match.Groups["name"].Value;
            if (VoidElements.Contains(name))
            {
                continue;
            }

            var isClosing = match.Groups["close"].Success;
            var isSelfClosing = match.Groups["self"].Success;

            if (isSelfClosing)
            {
                continue;
            }

            if (isClosing)
            {
                if (stack.Count == 0 || !stack.Pop().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            else
            {
                stack.Push(name);
            }
        }

        return stack.Count == 0;
    }

    /// <summary>
    /// Plain HTML tag names eligible for neutralization. A closed, case-SENSITIVE
    /// lowercase set: PascalCase component tags (&lt;Label&gt;) and C# generics inside
    /// @() expressions (Dictionary&lt;string, object&gt;) must never match.
    /// </summary>
    private static readonly HashSet<string> NeutralizableElements = new(StringComparer.Ordinal)
    {
        "a", "abbr", "article", "aside", "b", "big", "blockquote", "body", "button",
        "caption", "center", "cite", "code", "dd", "div", "dl", "dt", "em", "fieldset",
        "font", "footer", "form", "h1", "h2", "h3", "h4", "h5", "h6", "header", "hgroup",
        "i", "label", "legend", "li", "main", "nav", "ol", "option", "p", "pre", "s",
        "section", "select", "small", "span", "strike", "strong", "sub", "sup", "table",
        "tbody", "td", "tfoot", "th", "thead", "tr", "u", "ul",
    };

    /// <summary>
    /// True when the fragment contains unbalanced PLAIN lowercase HTML tags (the set
    /// Neutralize can repair). Component tags and generics are ignored by design.
    /// </summary>
    public static bool HasUnbalancedPlainTags(string html)
        => FindUnmatchedPlainTags(html).Count > 0;

    /// <summary>
    /// Rewrites unmatched plain HTML tags (a stray &lt;/p&gt;, an unclosed &lt;b&gt; - WebForms
    /// tolerated sloppy markup that Razor rejects) into raw MarkupString output. The DOM
    /// the browser builds is identical; Razor just no longer parses the tag structurally.
    /// </summary>
    public static string Neutralize(string html)
    {
        var unmatched = FindUnmatchedPlainTags(html);
        if (unmatched.Count == 0)
        {
            return html;
        }

        var result = html;
        foreach (var (index, length) in unmatched.OrderByDescending(span => span.Index))
        {
            // "<" is escaped as < so an enclosing template's balance scan does not
            // re-match the tag inside this C# string literal (double neutralization)
            var tag = result.Substring(index, length)
                .Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("<", "\\u003c");
            result = result[..index]
                + $"@((global::Microsoft.AspNetCore.Components.MarkupString)\"{tag}\")"
                + result[(index + length)..];
        }
        return result;
    }

    private static List<(int Index, int Length)> FindUnmatchedPlainTags(string html)
    {
        var opens = new List<(string Name, int Index, int Length)>();
        var unmatched = new List<(int Index, int Length)>();

        foreach (Match match in TagRegex().Matches(html))
        {
            var name = match.Groups["name"].Value;
            if (VoidElements.Contains(name) || match.Groups["self"].Success
                || !NeutralizableElements.Contains(name))
            {
                continue;
            }

            if (match.Groups["close"].Success)
            {
                var openIndex = opens.FindLastIndex(open =>
                    open.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (openIndex < 0)
                {
                    unmatched.Add((match.Index, match.Length));
                }
                else
                {
                    // Improperly interleaved opens above the match are unmatched
                    for (var k = opens.Count - 1; k > openIndex; k--)
                    {
                        unmatched.Add((opens[k].Index, opens[k].Length));
                    }
                    opens.RemoveRange(openIndex, opens.Count - openIndex);
                }
            }
            else
            {
                opens.Add((name, match.Index, match.Length));
            }
        }

        unmatched.AddRange(opens.Select(open => (open.Index, open.Length)));
        return unmatched;
    }

    [GeneratedRegex(@"<(?<close>/)?(?<name>[A-Za-z][A-Za-z0-9-]*)(?:\s[^>]*?)?(?<self>/)?>")]
    private static partial Regex TagRegex();
}
