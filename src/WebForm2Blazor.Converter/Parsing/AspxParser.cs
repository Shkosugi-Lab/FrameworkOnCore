using System.Text;

namespace WebForm2Blazor.Converter.Parsing;

/// <summary>
/// Parser for .aspx / .ascx / .master files.
///
/// Plain HTML is not parsed and is carried through as text; only server controls
/// (registered prefixes such as asp:, elements with runat="server", and template
/// elements) are built into a nested structure. ASPX is not guaranteed to be valid
/// HTML (e.g. an unclosed &lt;ul&gt; inside a HeaderTemplate), so this approach is
/// more robust than using an HTML parser.
/// </summary>
public static class AspxParser
{
    /// <summary>Known child/template elements treated as elements only when the parent is a server control.</summary>
    /// <summary>
    /// A prefix-less element named "...Template" inside a server control.
    ///
    /// <see cref="ChildElements"/> is the list of the BUILT-IN controls' slots. A
    /// third-party control declares its own - mojoPortal's SiteMapPath has NodeTemplate,
    /// RootNodeTemplate and CurrentNodeTemplate - and a name missing from that list was
    /// not parsed as an element at all. It came through as TEXT, so the tag appeared in
    /// the output while the emitter never knew it had entered a template: every
    /// "&lt;%# Eval(...) %&gt;" inside was reported as a data-bound expression OUTSIDE a
    /// template and dropped. The breadcrumb rendered links with no href and no text.
    ///
    /// The residual said "outside a template" while the expression was inside one, which
    /// is what made this hard to find - the report was describing the parser's belief.
    ///
    /// Matched on the NAME because that is what WebForms itself goes on: a template
    /// property is found by name on the control, and there is no registry of them. The
    /// enclosing element is already known to be a server control (the root is excluded
    /// below), so a plain HTML "&lt;template&gt;" outside one is unaffected.
    /// </summary>
    private static bool IsTemplateName(string name)
        => name.EndsWith("Template", StringComparison.OrdinalIgnoreCase);

    private static readonly HashSet<string> ChildElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "Columns", "Items", "Fields", "Triggers",
        "ItemTemplate", "AlternatingItemTemplate", "HeaderTemplate", "FooterTemplate",
        "SeparatorTemplate", "ItemSeparatorTemplate", "EditItemTemplate", "InsertItemTemplate",
        "SelectedItemTemplate", "EmptyDataTemplate", "EmptyItemTemplate",
        "ContentTemplate", "LayoutTemplate", "GroupTemplate", "PagerTemplate",
        // Style child elements (<HeaderStyle CssClass=... /> etc.)
        "HeaderStyle", "RowStyle", "AlternatingRowStyle", "FooterStyle",
        "PagerStyle", "SelectedRowStyle", "EditRowStyle", "EmptyDataRowStyle",
        "ItemStyle", "ControlStyle", "PagerSettings",
        // The DataGrid / DataList naming of the same slots
        "AlternatingItemStyle", "EditItemStyle", "SelectedItemStyle", "SeparatorStyle",
        "InsertItemStyle", "InsertRowStyle", "CommandRowStyle", "FieldHeaderStyle",
        "EmptyItemStyle", "GroupSeparatorStyle",
    };

    public static ParsedAspx Parse(string source, ISet<string> serverPrefixes)
    {
        var directives = new List<DirectiveNode>();
        var root = new ElementNode { Prefix = "", Name = "#root", Attributes = NewAttributes() };
        var stack = new Stack<ElementNode>();
        stack.Push(root);

        // Plain (text-carried) HTML tags with the same name as the enclosing server
        // element: <td runat="server"><table><tr><td>...</td> must not close the server
        // td on the first inner </td>. Counts same-name plain opens per open element.
        var plainSameNameDepth = new Dictionary<ElementNode, int>();

        var text = new StringBuilder();

        // Line lookup by index: the scanner walks characters, so the line is derived from
        // the offset rather than tracked as mutable state (which drifts on backtracking)
        var lineStarts = new List<int> { 0 };
        for (var scan = 0; scan < source.Length; scan++)
        {
            if (source[scan] == '\n')
            {
                lineStarts.Add(scan + 1);
            }
        }

        int LineAt(int index)
        {
            var low = 0;
            var high = lineStarts.Count - 1;
            while (low < high)
            {
                var middle = (low + high + 1) / 2;
                if (lineStarts[middle] <= index)
                {
                    low = middle;
                }
                else
                {
                    high = middle - 1;
                }
            }
            return low + 1;
        }

        var textStart = 0;

        void FlushText()
        {
            if (text.Length > 0)
            {
                stack.Peek().Children.Add(new TextNode(text.ToString()) { Line = LineAt(textStart) });
                text.Clear();
            }
        }

        var i = 0;
        while (i < source.Length)
        {
            if (source[i] != '<')
            {
                if (text.Length == 0)
                {
                    textStart = i;
                }
                text.Append(source[i]);
                i++;
                continue;
            }

            // <% ... %> - directive / expression / code block
            if (i + 1 < source.Length && source[i + 1] == '%')
            {
                // <%-- ... --%> server comment ends at "--%>", not at the first "%>"
                // (the comment body may itself contain <% ... %> blocks)
                if (i + 3 < source.Length && source[i + 2] == '-' && source[i + 3] == '-')
                {
                    var commentEnd = source.IndexOf("--%>", i + 4, StringComparison.Ordinal);
                    if (commentEnd >= 0)
                    {
                        FlushText();
                        i = commentEnd + 4;
                        continue;
                    }
                }

                var end = source.IndexOf("%>", i + 2, StringComparison.Ordinal);
                if (end < 0)
                {
                    text.Append(source[i]);
                    i++;
                    continue;
                }

                var inner = source[(i + 2)..end];
                FlushText();

                if (inner.StartsWith('@'))
                {
                    directives.Add(ParseDirective(inner[1..]));
                }
                else if (!inner.StartsWith("--", StringComparison.Ordinal))
                {
                    // Content starting with "--" is a server-side comment; emit nothing.
                    // The marker may be separated from "<%" by whitespace ("<% =Expr %>",
                    // real in DNN): read past it, or the block is taken for a code block
                    // and its output silently disappears into a TODO comment.
                    inner = inner.TrimStart();
                    if (inner.Length == 0)
                    {
                        i = end + 2;
                        continue;
                    }

                    var kind = inner[0] switch
                    {
                        '=' => ExpressionKind.Render,
                        ':' => ExpressionKind.Encoded,
                        '#' => ExpressionKind.DataBind,
                        _ => ExpressionKind.Code,
                    };
                    var code = kind == ExpressionKind.Code ? inner : inner[1..];
                    // <%#: expr %> is the HTML-encoded data-binding form (WebForms 4.5)
                    if (kind == ExpressionKind.DataBind && code.StartsWith(':'))
                    {
                        code = code[1..];
                    }
                    stack.Peek().Children.Add(new ExpressionNode(kind, code.Trim()) { Line = LineAt(i) });
                }

                i = end + 2;
                continue;
            }

            // Closing tag
            if (i + 1 < source.Length && source[i + 1] == '/')
            {
                var gt = source.IndexOf('>', i);
                if (gt < 0)
                {
                    text.Append(source[i]);
                    i++;
                    continue;
                }

                var (closePrefix, closeName) = SplitQualifiedName(source[(i + 2)..gt].Trim());
                var top = stack.Peek();
                if (stack.Count > 1
                    && top.Prefix.Equals(closePrefix, StringComparison.OrdinalIgnoreCase)
                    && top.Name.Equals(closeName, StringComparison.OrdinalIgnoreCase))
                {
                    // A pending plain open of the same name consumes this close as text
                    if (plainSameNameDepth.TryGetValue(top, out var depth) && depth > 0)
                    {
                        plainSameNameDepth[top] = depth - 1;
                        text.Append(source[i]);
                        i++;
                        continue;
                    }

                    FlushText();
                    plainSameNameDepth.Remove(top);
                    stack.Pop();
                    i = gt + 1;
                    continue;
                }

                // An unmatched close of a SERVER control can never be meaningful text - it
                // means the scanner lost the opening tag (DNN writes a control inside an
                // HTML tag's attribute list: <html <asp:literal .../></asp:literal>>).
                // Carrying it through would emit an unparsable tag into the Razor output.
                if (!string.IsNullOrEmpty(closePrefix) && serverPrefixes.Contains(closePrefix))
                {
                    i = gt + 1;
                    continue;
                }

                // A closing tag with no matching element (plain HTML) is treated as text
                text.Append(source[i]);
                i++;
                continue;
            }

            // Opening tag
            var tag = TryReadTag(source, i);
            if (tag is null)
            {
                text.Append(source[i]);
                i++;
                continue;
            }

            var isServerControl =
                serverPrefixes.Contains(tag.Prefix)
                || (string.IsNullOrEmpty(tag.Prefix)
                    && (ChildElements.Contains(tag.Name) || IsTemplateName(tag.Name))
                    && !ReferenceEquals(stack.Peek(), root))
                || tag.Attributes.TryGetValue("runat", out var runat)
                    && runat.Equals("server", StringComparison.OrdinalIgnoreCase);

            if (!isServerControl)
            {
                var enclosing = stack.Peek();
                if (!ReferenceEquals(enclosing, root)
                    && !tag.SelfClosing
                    && string.IsNullOrEmpty(tag.Prefix)
                    && string.IsNullOrEmpty(enclosing.Prefix)
                    && enclosing.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase))
                {
                    plainSameNameDepth[enclosing] =
                        plainSameNameDepth.GetValueOrDefault(enclosing) + 1;
                }
                text.Append(source[i]);
                i++;
                continue;
            }

            FlushText();
            var element = new ElementNode
            {
                Prefix = tag.Prefix,
                Name = tag.Name,
                Attributes = tag.Attributes,
                SelfClosing = tag.SelfClosing,
                Line = LineAt(i),
            };
            stack.Peek().Children.Add(element);
            if (!tag.SelfClosing)
            {
                stack.Push(element);
            }
            i = tag.End;
        }

        FlushText();

        return new ParsedAspx { Directives = directives, Nodes = root.Children };
    }

    /// <summary>
    /// Extracts only the directives before the main parse. Without knowing the tag
    /// prefixes declared by &lt;%@ Register %&gt;, the main parse cannot decide which
    /// elements to treat as server controls.
    /// </summary>
    public static List<DirectiveNode> PreScanDirectives(string source)
    {
        var directives = new List<DirectiveNode>();
        var i = 0;
        while (true)
        {
            var start = source.IndexOf("<%@", i, StringComparison.Ordinal);
            if (start < 0)
            {
                break;
            }
            var end = source.IndexOf("%>", start, StringComparison.Ordinal);
            if (end < 0)
            {
                break;
            }
            directives.Add(ParseDirective(source[(start + 3)..end]));
            i = end + 2;
        }
        return directives;
    }

    private static DirectiveNode ParseDirective(string inner)
    {
        inner = inner.Trim();
        var nameEnd = 0;
        while (nameEnd < inner.Length && !char.IsWhiteSpace(inner[nameEnd]))
        {
            nameEnd++;
        }
        var name = inner[..nameEnd];
        var attributes = ParseAttributes(inner[nameEnd..], out _);
        return new DirectiveNode(name, attributes);
    }

    private sealed record TagInfo(string Prefix, string Name, Dictionary<string, string> Attributes, bool SelfClosing, int End);

    private static TagInfo? TryReadTag(string source, int start)
    {
        var i = start + 1;
        if (i >= source.Length || (!char.IsLetter(source[i]) && source[i] != '_'))
        {
            return null;
        }

        var nameStart = i;
        while (i < source.Length
               && (char.IsLetterOrDigit(source[i]) || source[i] is ':' or '_' or '-' or '.'))
        {
            i++;
        }
        var (prefix, name) = SplitQualifiedName(source[nameStart..i]);

        var attributes = ParseAttributes(source[i..], out var consumed, out var terminator);
        if (terminator == TagTerminator.None)
        {
            // No '>' found = the tag is not closed; treat it as text
            return null;
        }

        return new TagInfo(prefix, name, attributes, terminator == TagTerminator.SelfClosing, i + consumed);
    }

    private enum TagTerminator
    {
        None,
        Close,
        SelfClosing,
    }

    private static Dictionary<string, string> ParseAttributes(string source, out int consumed)
        => ParseAttributes(source, out consumed, out _);

    /// <summary>Reads an attribute list and returns the character count consumed up to '&gt;' or '/&gt;'.</summary>
    private static Dictionary<string, string> ParseAttributes(string source, out int consumed, out TagTerminator terminator)
    {
        terminator = TagTerminator.None;
        var attributes = NewAttributes();
        var i = 0;

        while (i < source.Length)
        {
            while (i < source.Length && char.IsWhiteSpace(source[i]))
            {
                i++;
            }
            if (i >= source.Length)
            {
                break;
            }

            if (source[i] == '>')
            {
                i++;
                terminator = TagTerminator.Close;
                break;
            }
            if (source[i] == '/' && i + 1 < source.Length && source[i + 1] == '>')
            {
                i += 2;
                terminator = TagTerminator.SelfClosing;
                break;
            }
            if (source[i] == '%' && i + 1 < source.Length && source[i + 1] == '>')
            {
                // End of a directive
                i += 2;
                terminator = TagTerminator.Close;
                break;
            }

            var nameStart = i;
            while (i < source.Length
                   && !char.IsWhiteSpace(source[i])
                   && source[i] is not ('=' or '>' or '/'))
            {
                i++;
            }
            if (i == nameStart)
            {
                i++;
                continue;
            }
            var attributeName = source[nameStart..i];

            while (i < source.Length && char.IsWhiteSpace(source[i]))
            {
                i++;
            }

            var value = string.Empty;
            if (i < source.Length && source[i] == '=')
            {
                i++;
                while (i < source.Length && char.IsWhiteSpace(source[i]))
                {
                    i++;
                }

                if (i < source.Length && (source[i] == '"' || source[i] == '\''))
                {
                    var quote = source[i];
                    i++;
                    var valueStart = i;
                    while (i < source.Length && source[i] != quote)
                    {
                        i++;
                    }
                    value = source[valueStart..i];
                    if (i < source.Length)
                    {
                        i++;
                    }
                }
                else
                {
                    var valueStart = i;
                    while (i < source.Length
                           && !char.IsWhiteSpace(source[i])
                           && source[i] is not ('>' or '/'))
                    {
                        i++;
                    }
                    value = source[valueStart..i];
                }
            }

            attributes[attributeName] = value;
        }

        consumed = i;
        return attributes;
    }

    private static (string Prefix, string Name) SplitQualifiedName(string qualified)
    {
        var colon = qualified.IndexOf(':');
        return colon < 0
            ? (string.Empty, qualified)
            : (qualified[..colon], qualified[(colon + 1)..]);
    }

    private static Dictionary<string, string> NewAttributes() => new(StringComparer.OrdinalIgnoreCase);
}
