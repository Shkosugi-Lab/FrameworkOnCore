namespace WebForm2Blazor.Converter.Parsing;

public abstract class AspxNode
{
    /// <summary>
    /// 1-based line in the source file. Residuals carry it so a reviewer (and the AI
    /// layer) can go straight to the spot instead of scanning the whole file.
    /// </summary>
    public int Line { get; init; }
}

/// <summary>Plain HTML / text other than server controls. Carried through verbatim.</summary>
public sealed class TextNode(string text) : AspxNode
{
    public string Text { get; } = text;
}

public enum ExpressionKind
{
    /// <summary>&lt;%= expr %&gt;</summary>
    Render,
    /// <summary>&lt;%: expr %&gt; (HTML encoded)</summary>
    Encoded,
    /// <summary>&lt;%# expr %&gt; (data-binding expression)</summary>
    DataBind,
    /// <summary>&lt;% code %&gt; (inline code block)</summary>
    Code,
}

public sealed class ExpressionNode(ExpressionKind kind, string code) : AspxNode
{
    public ExpressionKind Kind { get; } = kind;
    public string Code { get; } = code;
}

/// <summary>Where a construct sits in the source file.</summary>
public readonly record struct SourcePosition(int Line)
{
    public override string ToString() => $"{Line} 行目";
}

/// <summary>A directive such as &lt;%@ Page ... %&gt;.</summary>
public sealed class DirectiveNode(string name, Dictionary<string, string> attributes)
{
    public string Name { get; } = name;
    public Dictionary<string, string> Attributes { get; } = attributes;

    public string? Get(string key) => Attributes.GetValueOrDefault(key);
}

/// <summary>A server control, template element, or HTML element with runat="server".</summary>
public sealed class ElementNode : AspxNode
{
    public required string Prefix { get; init; }
    public required string Name { get; init; }
    public required Dictionary<string, string> Attributes { get; init; }
    public List<AspxNode> Children { get; } = [];
    public bool SelfClosing { get; init; }

    public string QualifiedName => string.IsNullOrEmpty(Prefix) ? Name : $"{Prefix}:{Name}";

    public string? Id => Attributes.GetValueOrDefault("ID");

    /// <summary>Enumerates descendants in pre-order.</summary>
    public IEnumerable<ElementNode> Descendants()
    {
        foreach (var child in Children.OfType<ElementNode>())
        {
            yield return child;
            foreach (var nested in child.Descendants())
            {
                yield return nested;
            }
        }
    }
}

public sealed class ParsedAspx
{
    public required List<DirectiveNode> Directives { get; init; }
    public required List<AspxNode> Nodes { get; init; }

    public DirectiveNode? MainDirective =>
        Directives.FirstOrDefault(d =>
            d.Name.Equals("Page", StringComparison.OrdinalIgnoreCase)
            || d.Name.Equals("Master", StringComparison.OrdinalIgnoreCase)
            || d.Name.Equals("Control", StringComparison.OrdinalIgnoreCase));

    public IEnumerable<ElementNode> RootElements => Nodes.OfType<ElementNode>();

    public IEnumerable<ElementNode> AllElements
    {
        get
        {
            foreach (var element in RootElements)
            {
                yield return element;
                foreach (var descendant in element.Descendants())
                {
                    yield return descendant;
                }
            }
        }
    }
}
