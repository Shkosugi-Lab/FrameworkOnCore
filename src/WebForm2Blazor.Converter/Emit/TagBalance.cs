using System.Text.RegularExpressions;
using WebForm2Blazor.Components;

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
        // Void in HTML4 and still parsed that way. They arrive via HtmlTextWriterTag
        // below, and without these they would be read as forever-unclosed opens.
        "basefont", "bgsound", "frame", "isindex",
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
    /// HTML5 container elements that <see cref="HtmlTextWriterTag"/> predates. The enum is
    /// the WebForms-era element list; it stops before HTML5, so these are added by hand -
    /// but they are the ONLY hand-written part, and the part that cannot grow silently
    /// wrong: a missing one is a tag this scan skips, and the enum covers everything
    /// WebForms itself could render.
    /// </summary>
    private static readonly string[] Html5ContainerElements =
    {
        "abbr", "article", "aside", "audio", "bdi", "canvas", "data", "datalist",
        "details", "dialog", "figcaption", "figure", "footer", "header", "hgroup",
        "main", "mark", "meter", "output", "picture", "progress", "summary",
        "template", "time", "video",
    };

    /// <summary>
    /// Plain HTML tag names eligible for neutralization. Case-SENSITIVE lowercase:
    /// PascalCase component tags (&lt;Label&gt;) and C# generics inside @() expressions
    /// (Dictionary&lt;string, object&gt;) must never match.
    ///
    /// This WAS a hand-written list of 57 names, and a name missing from it was a tag
    /// the balance scan silently skipped - an unclosed &lt;textarea&gt; or &lt;blockquote&gt;
    /// inside a template read as balanced, emitted as-is, and failed in Razor with the
    /// error pointing somewhere else. The list happened to be right for elements the
    /// corpora use, but only by having been extended each time one was found missing.
    ///
    /// So ask the artifact instead. <see cref="HtmlTextWriterTag"/> is WebForms' own
    /// enumeration of the elements it can render; every element reachable from a
    /// WebForms control is in it by construction. The old list was missing 35 of them
    /// (textarea, blockquote's neighbours del/ins/q/samp/kbd/var, colgroup, iframe,
    /// object, script, style, title, head, html, ...).
    /// </summary>
    private static readonly HashSet<string> NeutralizableElements = BuildNeutralizableElements();

    private static HashSet<string> BuildNeutralizableElements()
    {
        var elements = new HashSet<string>(StringComparer.Ordinal);

        foreach (var name in Enum.GetNames<HtmlTextWriterTag>())
        {
            elements.Add(name.ToLowerInvariant());
        }

        elements.UnionWith(Html5ContainerElements);

        // "Unknown" is the enum's no-tag sentinel, not an element. The void elements
        // have no close tag, so an unmatched one is not evidence of anything.
        elements.Remove(nameof(HtmlTextWriterTag.Unknown).ToLowerInvariant());
        elements.ExceptWith(VoidElements);

        return elements;
    }

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

    /// <summary>
    /// One tag. The attribute list consumes QUOTED values whole, so a "&gt;" - or a whole
    /// tag - inside an attribute value does not end the match.
    ///
    /// It used to be "\s[^&gt;]*?", which stopped at the first "&gt;" wherever it was. DNN's
    /// InstallWizard has
    ///
    ///     &lt;asp:Label Text="&lt;a class=&amp;quot;videoLink&amp;quot; href=&amp;quot;...&amp;quot;&gt;Check this&lt;/a&gt;" /&gt;
    ///
    /// so the Label's match ended at the "&gt;" of the &lt;a&gt; INSIDE its Text. The opening &lt;a&gt;
    /// was therefore swallowed and only the closing &lt;/a&gt; was seen - a stray close tag
    /// that Neutralize dutifully rewrote into a Razor expression, inside an attribute,
    /// which is RZ9986 ("Component attributes do not support complex content"). Three DNN
    /// pages failed to compile because of a repair applied where nothing was broken.
    ///
    /// A tag whose quoting is genuinely unbalanced ('&lt;a title="x&gt;') no longer matches at
    /// all, which is the honest answer: its extent cannot be determined.
    /// </summary>
    [GeneratedRegex("""<(?<close>/)?(?<name>[A-Za-z][A-Za-z0-9-]*)(?:\s(?:"[^"]*"|'[^']*'|[^>"'])*?)?(?<self>/)?>""")]
    private static partial Regex TagRegex();
}
