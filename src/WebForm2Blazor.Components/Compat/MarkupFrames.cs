using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace WebForm2Blazor.Components;

/// <summary>
/// Turns the HTML a render-based control wrote into Blazor element frames, putting a real
/// component wherever the writer left a marker for one.
///
/// Needed only in that case, and used only in that case (LegacyWebControl.BuildRenderTree
/// emits plain markup otherwise). Markup cannot simply be cut around a component: each
/// markup frame is parsed by the browser on its own, and "&lt;div class=editor&gt;" parsed on
/// its own is a closed, empty div - the component would land beside the element that was
/// meant to contain it. Element frames keep the nesting.
///
/// The input is what HtmlTextWriter and a ported Render override produce: ordinary,
/// well-formed-enough HTML. Unmatched end tags are ignored and open elements are closed at
/// the end, which is what a browser would do with the same text.
/// </summary>
internal static class MarkupFrames
{
    private static readonly HashSet<string> VoidElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param",
        "source", "track", "wbr",
    };

    private static readonly HashSet<string> RawTextElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "script", "style", "textarea", "title",
    };

    public static void Build(
        RenderTreeBuilder builder, string html, IReadOnlyList<IComponent> components,
        PreparedComponentActivator activator)
    {
        var sequence = 0;
        var open = new Stack<string>();
        var index = 0;

        while (index < html.Length)
        {
            var lt = html.IndexOf('<', index);
            if (lt < 0)
            {
                EmitText(builder, html[index..], components, activator, ref sequence);
                break;
            }
            if (lt > index)
            {
                EmitText(builder, html[index..lt], components, activator, ref sequence);
            }

            // Comment / doctype / CDATA: passed through as markup, it holds no element.
            if (html.AsSpan(lt).StartsWith("<!--"))
            {
                var end = html.IndexOf("-->", lt + 4, StringComparison.Ordinal);
                end = end < 0 ? html.Length : end + 3;
                builder.AddMarkupContent(sequence++, html[lt..end]);
                index = end;
                continue;
            }
            if (lt + 1 < html.Length && html[lt + 1] is '!' or '?')
            {
                var end = html.IndexOf('>', lt);
                end = end < 0 ? html.Length : end + 1;
                builder.AddMarkupContent(sequence++, html[lt..end]);
                index = end;
                continue;
            }

            var close = FindTagEnd(html, lt);
            if (close < 0)
            {
                // A '<' that opens no tag is text.
                EmitText(builder, html[lt..], components, activator, ref sequence);
                break;
            }

            var tag = html[(lt + 1)..close];
            index = close + 1;

            if (tag.StartsWith('/'))
            {
                var name = tag[1..].Trim();
                if (open.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    // Close everything opened inside it too, as a browser does.
                    while (open.Count > 0)
                    {
                        var top = open.Pop();
                        builder.CloseElement();
                        if (string.Equals(top, name, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
                continue;
            }

            var selfClosing = tag.EndsWith('/');
            if (selfClosing)
            {
                tag = tag[..^1];
            }

            var (elementName, attributes) = ParseTag(tag);
            if (elementName.Length == 0)
            {
                continue;
            }

            builder.OpenElement(sequence++, elementName);
            foreach (var (name, value) in attributes)
            {
                builder.AddAttribute(sequence++, name, value);
            }

            if (selfClosing || VoidElements.Contains(elementName))
            {
                builder.CloseElement();
                continue;
            }

            if (RawTextElements.Contains(elementName))
            {
                var endTag = html.IndexOf("</" + elementName, index, StringComparison.OrdinalIgnoreCase);
                var contentEnd = endTag < 0 ? html.Length : endTag;
                if (contentEnd > index)
                {
                    builder.AddMarkupContent(sequence++, html[index..contentEnd]);
                }
                builder.CloseElement();
                var afterEnd = endTag < 0 ? html.Length : html.IndexOf('>', endTag);
                index = afterEnd < 0 ? html.Length : afterEnd + 1;
                continue;
            }

            open.Push(elementName);
        }

        while (open.Count > 0)
        {
            open.Pop();
            builder.CloseElement();
        }
    }

    /// <summary>The '&gt;' ending a tag, skipping any inside a quoted attribute value.</summary>
    private static int FindTagEnd(string html, int lt)
    {
        char quote = '\0';
        for (var position = lt + 1; position < html.Length; position++)
        {
            var character = html[position];
            if (quote != '\0')
            {
                if (character == quote)
                {
                    quote = '\0';
                }
            }
            else if (character is '"' or '\'')
            {
                quote = character;
            }
            else if (character == '>')
            {
                return position;
            }
            else if (character == '<')
            {
                return -1;
            }
        }
        return -1;
    }

    private static (string Name, List<(string Name, string Value)> Attributes) ParseTag(string tag)
    {
        var attributes = new List<(string, string)>();
        var position = 0;

        static bool IsNameChar(char c) => !char.IsWhiteSpace(c) && c is not ('=' or '"' or '\'' or '/' or '>');

        var nameStart = position;
        while (position < tag.Length && IsNameChar(tag[position]))
        {
            position++;
        }
        var elementName = tag[nameStart..position];

        while (position < tag.Length)
        {
            while (position < tag.Length && (char.IsWhiteSpace(tag[position]) || tag[position] == '/'))
            {
                position++;
            }
            if (position >= tag.Length)
            {
                break;
            }

            var attrStart = position;
            while (position < tag.Length && IsNameChar(tag[position]))
            {
                position++;
            }
            var attrName = tag[attrStart..position];
            if (attrName.Length == 0)
            {
                position++;
                continue;
            }

            while (position < tag.Length && char.IsWhiteSpace(tag[position]))
            {
                position++;
            }

            string value;
            if (position < tag.Length && tag[position] == '=')
            {
                position++;
                while (position < tag.Length && char.IsWhiteSpace(tag[position]))
                {
                    position++;
                }
                if (position < tag.Length && tag[position] is '"' or '\'')
                {
                    var quote = tag[position++];
                    var valueStart = position;
                    while (position < tag.Length && tag[position] != quote)
                    {
                        position++;
                    }
                    value = tag[valueStart..Math.Min(position, tag.Length)];
                    position++;
                }
                else
                {
                    var valueStart = position;
                    while (position < tag.Length && !char.IsWhiteSpace(tag[position]))
                    {
                        position++;
                    }
                    value = tag[valueStart..position];
                }
            }
            else
            {
                // A bare attribute ("disabled") is present with an empty value.
                value = string.Empty;
            }

            // The writer HTML-encoded the value; the frame takes the raw one and Blazor
            // encodes it again on output.
            attributes.Add((attrName, WebUtility.HtmlDecode(value)));
        }

        return (elementName, attributes);
    }

    /// <summary>Text between tags, with each component marker replaced by that component.</summary>
    private static void EmitText(
        RenderTreeBuilder builder, string text, IReadOnlyList<IComponent> components,
        PreparedComponentActivator activator, ref int sequence)
    {
        var position = 0;
        while (position < text.Length)
        {
            var marker = text.IndexOf('\u0001', position);
            if (marker < 0)
            {
                builder.AddMarkupContent(sequence++, text[position..]);
                return;
            }

            var markerEnd = text.IndexOf('\u0001', marker + 1);
            if (markerEnd < 0)
            {
                builder.AddMarkupContent(sequence++, text[position..]);
                return;
            }

            if (marker > position)
            {
                builder.AddMarkupContent(sequence++, text[position..marker]);
            }

            var token = text[(marker + 1)..markerEnd];
            if (token.StartsWith("wfc", StringComparison.Ordinal)
                && int.TryParse(token[3..], out var componentIndex)
                && componentIndex >= 0 && componentIndex < components.Count)
            {
                var component = components[componentIndex];
                activator?.Register(component);
                builder.OpenComponent(sequence++, component.GetType());
                // Keyed by the object so a re-render keeps the same instance in place.
                builder.SetKey(component);
                builder.CloseComponent();
            }

            position = markerEnd + 1;
        }
    }
}
