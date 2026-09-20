using System.Text;
using System.Text.RegularExpressions;
using WebForm2Blazor.Converter.Convert;
using WebForm2Blazor.Converter.Mapping;
using WebForm2Blazor.Converter.Parsing;
using WebForm2Blazor.Converter.Project;

namespace WebForm2Blazor.Converter.Emit;

/// <summary>
/// Resolves an unmapped control tag to the ported class LegacyRenderHost can render, or
/// returns null and says why not.
/// </summary>
public delegate string? LegacyControlLookup(string prefix, string name, out string? reason);

/// <summary>
/// A control field generated into the code-behind.
///
/// <paramref name="LegacyHost"/> marks the case where the rendered component is a
/// LegacyRenderHost wrapper rather than the control itself, so the field has to reach
/// through it - see CodeBehindRewriter.EmitControlField.
/// </summary>
/// <param name="Instantiated">
/// The field is given an instance instead of waiting for an @ref. Used where the markup
/// element is deliberately gone but the code-behind still holds the control - an
/// UpdatePanel, whose wrapper Blazor does not need and whose Update() calls still have to
/// run rather than dereference null.
/// </param>
public sealed record ControlField(
    string Type, string Name, bool LegacyHost = false, bool Instantiated = false);

/// <summary>Prefix that makes a compat type name unambiguous against the page namespace.</summary>
internal static class CompatNames
{
    public const string QualifiedPrefix = "global::WebForm2Blazor.Components.";
}

/// <summary>
/// Control information for smoke-scenario auto-generation.
/// The converter knows every control's ID, kind, and whether it has events from the
/// syntax tree, so it can emit test scenarios deterministically without relying on AI.
/// </summary>
public sealed record SmokeControl(
    string Id,
    string Type,
    bool AssertPresence,
    bool Click,
    bool Change,
    bool Fill);

public sealed class EmitContext
{
    public required ConversionReport Report { get; init; }
    public required string SourceName { get; init; }

    /// <summary>"uc:ProductSummary" -> the referenced user control (from Register directives / Web.config).</summary>
    public Dictionary<string, UserControlRef> UserControlTags { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Hook to intercept page/master-specific elements (asp:Content etc.).</summary>
    public Func<ElementNode, MarkupEmitter, string?>? SpecialElementHandler { get; set; }

    public List<ControlField> Fields { get; } = [];

    public List<SmokeControl> SmokeControls { get; } = [];

    /// <summary>Namespaces of the user controls actually emitted (become @using lines / code-behind usings).</summary>
    public HashSet<string> UsedControlNamespaces { get; } = new(StringComparer.Ordinal);

    /// <summary>Namespace for generated placeholder components (null disables stub generation).</summary>
    public string? StubNamespace { get; init; }

    /// <summary>
    /// Resolves (prefix, name) of an unmapped control to the full type name of a ported
    /// legacy control class, when its source is available (assembly/namespace tag
    /// registrations + the base-class registry). Enables LegacyRenderHost conversion.
    /// </summary>
    /// <summary>
    /// (prefix, name, out reason) -&gt; ported class name, or null with a reason saying which
    /// of the several different failures this was.
    /// </summary>
    public LegacyControlLookup? LegacyControlResolver { get; set; }

    /// <summary>
    /// (prefix, name) -&gt; the ported or compat type the tag names, whether or not it can
    /// render on its own. Used for the entries of a collection element (&lt;Items&gt;,
    /// &lt;Columns&gt;), which the parent control renders itself.
    /// </summary>
    public Func<string, string, string?>? AnyTypeResolver { get; set; }

    /// <summary>Stub component name per unmapped control tag encountered in this file.</summary>
    public Dictionary<string, string> StubComponents { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// "resourceKey.PropertyName" -> value, from this file's App_LocalResources .resx.
    /// Drives WebForms implicit localization (meta:resourcekey).
    /// </summary>
    public Dictionary<string, string> LocalResources { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Server-control IDs declared anywhere in this file, collected BEFORE emitting.
    ///
    /// Populated up front rather than from <see cref="Fields"/> because Fields grows in
    /// document order during the emit walk, and markup regularly refers to a control
    /// declared further down (a &lt;script&gt; block at the top of the page is the usual
    /// case). Reading a half-built list would make the rewrite depend on where in the file
    /// the reference happens to sit.
    /// </summary>
    public HashSet<string> DeclaredControlIds { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Bodies of &lt;script runat="server"&gt; blocks, in document order. These are
    /// code-behind written inline; the converter appends them to the generated partial
    /// class rather than discarding them.
    /// </summary>
    public List<string> ServerScriptBlocks { get; } = [];
}

/// <summary>Converts the ASPX syntax tree into Razor markup.</summary>
public sealed partial class MarkupEmitter(EmitContext context)
{
    private int _dataBindingTemplateDepth;

    /// <summary>
    /// Nesting depth of templates that instantiate their content inside the owning
    /// control (data-bound item templates and the like). Controls there are duplicated
    /// per row and never become page fields in WebForms either, so field generation and
    /// @ref are limited to controls outside them.
    /// &lt;ContentTemplate&gt; is deliberately NOT counted: an UpdatePanel / TabPanel
    /// renders it once, and WebForms does generate designer fields for its controls
    /// (code-behind reaches txtName inside a TabPanel directly).
    /// </summary>
    private int _templateDepth;

    /// <summary>Templates whose content still produces page fields (rendered once, not per row).</summary>
    private static readonly HashSet<string> FieldTransparentTemplates = new(StringComparer.OrdinalIgnoreCase)
    {
        "ContentTemplate",
    };

    /// <summary>
    /// ItemPlaceholderID of the ListView being processed (default "itemPlaceholder").
    /// Inside a LayoutTemplate, the element with this ID is replaced by @ItemsPlaceholder.
    /// </summary>
    private string? _layoutPlaceholderId;

    /// <summary>
    /// GroupPlaceholderID of the ListView being processed, when it has a GroupTemplate
    /// (default "groupPlaceholder"). Null when the ListView does not group.
    /// </summary>
    private string? _listViewGroupPlaceholderId;

    /// <summary>Property types of the user control currently being emitted (null otherwise).</summary>
    private IReadOnlyDictionary<string, string>? _userControlPropertyTypes;

    /// <summary>
    /// The ItemType of the data-bound control being processed (WebForms 4.5 model
    /// binding). Typed Item references in its templates are cast against this type.
    /// </summary>
    private string? _currentItemType;

    /// <summary>The component whose children are being emitted (for template naming).</summary>
    private string? _currentComponent;

    public EmitContext Context => context;

    public string EmitNodes(IEnumerable<AspxNode> nodes)
    {
        var list = nodes as IReadOnlyList<AspxNode> ?? nodes.ToList();
        var builder = new StringBuilder();
        for (var index = 0; index < list.Count; index++)
        {
            // The classic WebForms idiom "<% if (cond) { %> markup <% } %>" spans sibling
            // code blocks; pair them here and emit a structured Razor @if
            if (list[index] is ExpressionNode { Kind: ExpressionKind.Code } code
                && StartsBlock(code.Code)
                && TryEmitCodeConstruct(list, ref index, builder))
            {
                continue;
            }
            builder.Append(EmitNode(list[index]));
        }
        return builder.ToString();
    }

    /// <summary>
    /// Keywords whose block form Razor accepts directly after '@'. Anything else opening
    /// a brace (an object initializer, a bare block) is left to the residual path.
    /// </summary>
    private static readonly string[] BlockKeywords = ["if", "foreach", "for", "while", "switch"];

    /// <summary>True when a code block begins one of the supported statements.</summary>
    private static bool StartsBlock(string code)
    {
        var trimmed = code.TrimStart();
        return BlockKeywords.Any(keyword =>
            trimmed.StartsWith(keyword, StringComparison.Ordinal)
            && trimmed.Length > keyword.Length
            && !char.IsLetterOrDigit(trimmed[keyword.Length])
            && trimmed[keyword.Length] != '_');
    }

    /// <summary>
    /// Emits an inline construct that spans sibling code blocks
    /// ("&lt;% if (c) { %&gt; markup &lt;% } %&gt;", the same for foreach / for / while, and
    /// any nesting of those) as one Razor code region.
    ///
    /// Pairing is done on BRACE DEPTH, not on one-statement-per-block: real corpora put
    /// several statements in a single block ("&lt;% if (a) { if (b) { %&gt;"), which a
    /// statement-shaped matcher mis-reads as a single if and then emits unbalanced braces.
    ///
    /// Returns false (having emitted nothing) when the construct cannot be paired or its
    /// markup cannot be made tag-balanced; the caller then falls back to per-node emission.
    /// </summary>
    private bool TryEmitCodeConstruct(IReadOnlyList<AspxNode> list, ref int index, StringBuilder builder)
    {
        if (!TryFindConstructEnd(list, index, out var end))
        {
            return false;
        }

        var region = new StringBuilder();
        var markup = new StringBuilder();
        var neutralized = false;

        for (var scan = index; scan <= end; scan++)
        {
            if (list[scan] is not ExpressionNode { Kind: ExpressionKind.Code } fragment)
            {
                markup.Append(EmitNode(list[scan]));
                continue;
            }

            // A cleanly opened nested construct stays INSIDE the surrounding markup run:
            // a wrapper opened before it and closed after it then still balances, and only
            // genuinely unbalanced markup has to be neutralized
            if (scan > index && IsCleanBlockOpener(fragment.Code)
                && TryFindConstructEnd(list, scan, out var nestedEnd) && nestedEnd < end)
            {
                var nested = new StringBuilder();
                var nestedIndex = scan;
                if (TryEmitCodeConstruct(list, ref nestedIndex, nested))
                {
                    markup.Append(nested);
                    scan = nestedIndex;
                    continue;
                }
            }

            if (!AppendMarkupRun(region, markup, ref neutralized))
            {
                Residual(ResidualKind.Structure,
                    $"インラインコード <% {Truncate(FirstLine(((ExpressionNode)list[index]).Code))} %> は"
                    + "タグ境界を跨ぐ部分を均衡させられないため Razor 構文に変換できません。");
                return false;
            }
            region.Append(' ').Append(fragment.Code.Trim()).Append(' ');
        }

        if (!AppendMarkupRun(region, markup, ref neutralized))
        {
            Residual(ResidualKind.Structure,
                $"インラインコード <% {Truncate(FirstLine(((ExpressionNode)list[index]).Code))} %> は"
                + "タグ境界を跨ぐ部分を均衡させられないため Razor 構文に変換できません。");
            return false;
        }

        // "@" must be followed immediately by the keyword (RZ1003)
        builder.Append('@').Append(region.ToString().TrimStart());

        context.Report.Info(context.SourceName,
            $"インラインコード <% {Truncate(FirstLine(((ExpressionNode)list[index]).Code))} %> を Razor 構文に変換しました"
            + (neutralized ? "(タグ境界を跨ぐ部分は不均衡タグを raw 出力に退避)" : string.Empty) + "。");
        index = end;
        return true;
    }

    /// <summary>
    /// Wraps the pending markup in &lt;text&gt; and appends it. Razor needs each markup run
    /// to be tag-balanced on its own, so unmatched plain tags are written as raw markup -
    /// the rendered bytes stay identical while the rest remains ordinary Razor.
    /// </summary>
    private static bool AppendMarkupRun(StringBuilder region, StringBuilder markup, ref bool neutralized)
    {
        if (markup.Length == 0)
        {
            return true;
        }

        var inner = markup.ToString();
        markup.Clear();

        if (!TagBalance.IsBalanced(inner))
        {
            if (!TagBalance.HasUnbalancedPlainTags(inner))
            {
                return false;
            }
            inner = TagBalance.Neutralize(inner);
            neutralized = true;
        }

        region.Append("<text>").Append(inner).Append("</text>");
        return true;
    }

    /// <summary>
    /// Finds the sibling whose code block brings the construct's brace depth back to zero.
    /// Fails when it never balances, or when the closing block carries trailing code -
    /// that code would land outside the Razor block and be rendered as text.
    /// </summary>
    private static bool TryFindConstructEnd(IReadOnlyList<AspxNode> list, int start, out int end)
    {
        end = -1;
        var depth = 0;

        for (var scan = start; scan < list.Count; scan++)
        {
            if (list[scan] is not ExpressionNode { Kind: ExpressionKind.Code } fragment)
            {
                continue;
            }

            depth = ScanBraces(fragment.Code, depth, out var closeIndex);
            if (depth < 0)
            {
                return false;
            }
            if (depth > 0)
            {
                continue;
            }

            if (closeIndex >= 0 && fragment.Code[closeIndex..].Trim().Length > 0)
            {
                return false;
            }
            end = scan;
            return true;
        }

        return false;
    }

    /// <summary>
    /// True when the block is exactly one statement header ("foreach (x in y) {"), so it
    /// can be converted on its own. Deliberately rejects blocks holding several statements.
    /// </summary>
    private static bool IsCleanBlockOpener(string code)
    {
        var trimmed = code.Trim();
        if (!StartsBlock(trimmed))
        {
            return false;
        }

        var open = trimmed.IndexOf('(');
        if (open < 0)
        {
            return false;
        }

        var depth = 0;
        for (var i = open; i < trimmed.Length; i++)
        {
            if (trimmed[i] == '(')
            {
                depth++;
            }
            else if (trimmed[i] == ')' && --depth == 0)
            {
                return trimmed[(i + 1)..].Trim() == "{";
            }
        }
        return false;
    }

    /// <summary>
    /// Net brace depth of a C# fragment, ignoring braces in string / char literals and in
    /// line comments. <paramref name="closeIndex"/> receives the position just past the
    /// brace that first brings the depth to zero (-1 when that never happens).
    /// </summary>
    private static int ScanBraces(string code, int startDepth, out int closeIndex)
    {
        var depth = startDepth;
        closeIndex = -1;

        for (var i = 0; i < code.Length; i++)
        {
            var current = code[i];

            if (current is '"' or '\'')
            {
                for (i++; i < code.Length && code[i] != current; i++)
                {
                    if (code[i] == '\\')
                    {
                        i++;
                    }
                }
                continue;
            }

            if (current == '/' && i + 1 < code.Length && code[i + 1] == '/')
            {
                while (i < code.Length && code[i] != '\n')
                {
                    i++;
                }
                continue;
            }

            if (current == '{')
            {
                depth++;
            }
            else if (current == '}')
            {
                depth--;
                if (depth == 0 && closeIndex < 0)
                {
                    closeIndex = i + 1;
                }
            }
        }

        return depth;
    }

    /// <summary>
    /// WebForms implicit localization: meta:resourcekey="X" makes the control take its
    /// properties from the "X.Text" / "X.ToolTip" entries of the page's App_LocalResources
    /// file, OVERRIDING what the markup says.
    ///
    /// Resolved here rather than at runtime: the .resx belongs to this one page, so the
    /// values are known while converting, and the output carries plain literals.
    /// Culture-specific variants are a separate migration (reported as a residual).
    /// </summary>
    private void ApplyImplicitLocalization(ElementNode element, List<KeyValuePair<string, string>> attributes)
    {
        if (context.LocalResources.Count == 0)
        {
            return;
        }

        var resourceKey = attributes.FirstOrDefault(pair =>
            pair.Key.Equals("resourcekey", StringComparison.OrdinalIgnoreCase)
            || pair.Key.Equals("meta:resourcekey", StringComparison.OrdinalIgnoreCase)).Value;
        if (string.IsNullOrEmpty(resourceKey))
        {
            return;
        }

        var applied = 0;
        foreach (var (property, value) in context.LocalResources)
        {
            var separator = property.LastIndexOf('.');
            if (separator <= 0)
            {
                continue;
            }

            // Two spellings occur in the wild. The plain form names the control and the
            // .resx supplies the property ("lblNoRecords" -> "lblNoRecords.Text"); the
            // qualified form (DNN) already names the property, so the key matches the
            // .resx entry whole ("valEndDate2.ErrorMessage").
            var matchesPlain = property[..separator].Equals(resourceKey, StringComparison.OrdinalIgnoreCase);
            var matchesQualified = property.Equals(resourceKey, StringComparison.OrdinalIgnoreCase);
            if (!matchesPlain && !matchesQualified)
            {
                continue;
            }

            var propertyName = property[(separator + 1)..];
            attributes.RemoveAll(pair => pair.Key.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            attributes.Add(new KeyValuePair<string, string>(propertyName, value));
            applied++;
        }

        if (applied > 0)
        {
            // The key itself is consumed: it has been folded into literal attributes, so
            // leaving it would surface as an unmapped attribute for work already done.
            attributes.RemoveAll(pair =>
                pair.Key.Equals("resourcekey", StringComparison.OrdinalIgnoreCase)
                || pair.Key.Equals("meta:resourcekey", StringComparison.OrdinalIgnoreCase));

            context.Report.Info(context.SourceName,
                $"<{element.QualifiedName}> の meta:resourcekey=\"{resourceKey}\" を "
                + $"App_LocalResources から解決しました(プロパティ {applied} 件)。");
        }
    }

    private static string FirstLine(string code)
        => Regex.Replace(code.Trim(), @"\s+", " ");

    /// <summary>
    /// True when a code block is self-contained C# statements rather than a fragment of
    /// tag-crossing control flow: braces balance, it does not open or close a block, and
    /// it ends in a way a statement does.
    /// </summary>
    private static bool IsStatementBlock(string code)
    {
        var trimmed = code.Trim();
        if (trimmed.Length == 0 || trimmed.StartsWith('}') || trimmed.EndsWith('{'))
        {
            return false;
        }

        if (ScanBraces(trimmed, 0, out _) != 0)
        {
            return false;
        }

        // "=" / ":" / "#" forms are expressions, handled by their own kinds; what is left
        // has to look like statements, so require the usual terminator
        return trimmed.EndsWith(';') || trimmed.EndsWith('}');
    }

    /// <summary>
    /// Source line of the construct being emitted. Residuals raised anywhere below pick
    /// it up, so every one of them points at a place in the original file.
    /// </summary>
    private int _currentLine;

    /// <summary>How many plain &lt;script&gt; elements the emitter is currently inside.</summary>
    private int _scriptDepth;

    private static int CountOccurrences(string text, string needle)
    {
        var count = 0;
        for (var index = text.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
             index >= 0;
             index = text.IndexOf(needle, index + needle.Length, StringComparison.OrdinalIgnoreCase))
        {
            count++;
        }
        return count;
    }

    private string EmitNode(AspxNode node)
    {
        var previousLine = _currentLine;
        if (node.Line > 0)
        {
            _currentLine = node.Line;
        }
        try
        {
            switch (node)
            {
                case TextNode text:
                    // A plain <script> is carried through as text, so the only way to know
                    // a control sits inside client script is to track the tag here
                    var emitted = EscapeRazorText(text.Text);
                    _scriptDepth += CountOccurrences(text.Text, "<script")
                                    - CountOccurrences(text.Text, "</script");
                    _scriptDepth = Math.Max(0, _scriptDepth);
                    return emitted;
                case ExpressionNode expression:
                    return EmitExpression(expression);
                case ElementNode element:
                    return EmitElement(element);
                default:
                    return string.Empty;
            }
        }
        finally
        {
            _currentLine = previousLine;
        }
    }

    /// <summary>Records a residual at the construct currently being emitted.</summary>
    private void Residual(ResidualKind kind, string message,
        ResidualDisposition disposition = ResidualDisposition.Convertible)
        => context.Report.Residual(context.SourceName, kind, message, _currentLine, disposition);

    private string EmitElement(ElementNode element)
    {
        // itemPlaceholder inside a ListView LayoutTemplate -> where the items expand
        if (_layoutPlaceholderId is not null
            && element.Id is not null
            && element.Id.Equals(_layoutPlaceholderId, StringComparison.OrdinalIgnoreCase))
        {
            return "@ItemsPlaceholder";
        }

        var handled = context.SpecialElementHandler?.Invoke(element, this);
        if (handled is not null)
        {
            return handled;
        }

        // Razor parses <script> content as raw text, so a component written there is not
        // recognized and its @ref is read as C# - the file then fails to compile. WebForms
        // used such controls to inject generated script; that is manual-migration work, and
        // a visible residual is worth more than output that does not build.
        if (_scriptDepth > 0 && !string.IsNullOrEmpty(element.Prefix))
        {
            Residual(ResidualKind.InlineCode,
                $"<{element.QualifiedName}> がクライアント側 <script> の内部にあります。"
                + "Razor はスクリプト内のコンポーネントを解釈できないため除去しました(スクリプト生成は手動移行が必要)。");
            return $"@* TODO(W2B): <script> 内のサーバーコントロール <{element.QualifiedName}> *@";
        }

        if (string.IsNullOrEmpty(element.Prefix))
        {
            if (ControlMappings.DataBoundTemplates.Contains(element.Name))
            {
                return EmitTemplate(element, dataBound: true);
            }
            if (ControlMappings.PlainTemplates.Contains(element.Name))
            {
                return EmitTemplate(element, dataBound: false);
            }

            // A template the built-in lists do not name. The parser now hands these over
            // (a third-party control declares its own slots), so the emitter has to place
            // them - left to fall through, "<usernametemplate>" became an
            // HtmlGenericControl inside PasswordRecovery and Razor rejected the whole
            // component (RZ9996).
            //
            // Not data-bound: a slot that instantiates per item is one of the names in
            // DataBoundTemplates, which was checked first. What is left renders once.
            if (element.Name.EndsWith("Template", StringComparison.OrdinalIgnoreCase))
            {
                return EmitTemplate(element, dataBound: false);
            }
            if (ControlMappings.StyleChildElements.Contains(element.Name))
            {
                // Already flattened into attributes by the parent control's EmitComponent
                return string.Empty;
            }

            // A server-side script block: its content is C# code, not markup. Emitting
            // the children would leak the code into the page as text, so drop it with a
            // residual instead.
            if (element.Name.Equals("script", StringComparison.OrdinalIgnoreCase))
            {
                // The block IS the code-behind, just written inside the markup. The
                // generated partial class is the place it belongs, so carry the text
                // across instead of dropping it. Only at the top level: a script block
                // nested in a data-bound template would be captured once per row.
                var code = string.Concat(element.Children.OfType<TextNode>().Select(text => text.Text));
                if (_templateDepth == 0 && !string.IsNullOrWhiteSpace(code))
                {
                    context.ServerScriptBlocks.Add(code);
                    context.Report.Info(context.SourceName,
                        "<script runat=\"server\"> ブロックの内容をコードビハインドへ移しました。");
                    return "@* W2B: server-side script block moved to the code-behind *@";
                }

                Residual(ResidualKind.InlineCode,
                    "<script runat=\"server\"> ブロックは自動変換できません。コードビハインドへの移動が必要です。");
                return "@* TODO(W2B): server-side script block removed *@";
            }

            // Structural elements (<form>, <head>, ...) are handled by App.razor / the
            // layout in Blazor, so only their content survives
            if (StructuralHtmlElements.Contains(element.Name))
            {
                context.Report.Info(context.SourceName,
                    $"<{element.Name} runat=\"server\"> は Blazor では不要なため中身だけを出力しました。");
                return EmitNodes(element.Children);
            }

            // Every other runat="server" HTML element (div, tr, a, input, ...) is the
            // HtmlControls family - the tag must survive, and code-behind keeps
            // toggling it (Visible / InnerHtml / Attributes)
            return EmitHtmlGenericControl(element);
        }

        // --- AJAX plumbing (asp: and AjaxControlToolkit): Blazor always renders diffs,
        //     so script managers and partial-update wrappers are unnecessary ---
        if (element.Name.Equals("ScriptManager", StringComparison.OrdinalIgnoreCase)
            || element.Name.Equals("ScriptManagerProxy", StringComparison.OrdinalIgnoreCase)
            || element.Name.Equals("ToolkitScriptManager", StringComparison.OrdinalIgnoreCase)
            || element.Name.Equals("UpdateProgress", StringComparison.OrdinalIgnoreCase)
            // Friendly URLs mobile/desktop switcher: meaningless in Blazor
            || element.Name.Equals("ViewSwitcher", StringComparison.OrdinalIgnoreCase))
        {
            context.Report.Info(context.SourceName,
                $"<{element.QualifiedName}> は Blazor では不要なため除去しました。");
            return string.Empty;
        }

        if (element.Name.Equals("UpdatePanel", StringComparison.OrdinalIgnoreCase))
        {
            context.Report.Info(context.SourceName,
                $"<{element.QualifiedName}> を展開しました(Blazor は常に差分描画のため部分更新の仕掛けは不要)。");

            // The wrapper goes, the FIELD stays. Code-behind keeps the panel and calls
            // Update() on it after changing something, and dropping both left every one of
            // those calls as "the name does not exist in the current context" - 22 in
            // mojoPortal. The field is a compat UpdatePanel whose Update() does nothing,
            // which is what re-rendering the whole tree already does for it.
            //
            // No @ref: there is no element to bind one to. The field is assigned an
            // instance so the calls run rather than dereferencing null.
            if (element.Id is { Length: > 0 } panelId && _templateDepth == 0)
            {
                context.Fields.Add(new ControlField(
                    "global::WebForm2Blazor.Components.UpdatePanel", panelId, Instantiated: true));
                context.DeclaredControlIds.Add(panelId);
            }

            var contentTemplate = element.Children.OfType<ElementNode>()
                .FirstOrDefault(child => child.Name.Equals("ContentTemplate", StringComparison.OrdinalIgnoreCase));
            if (contentTemplate is not null)
            {
                return EmitNodes(contentTemplate.Children);
            }
            return EmitNodes(element.Children.Where(child =>
                child is not ElementNode e2 || !e2.Name.Equals("Triggers", StringComparison.OrdinalIgnoreCase)));
        }

        if (context.UserControlTags.TryGetValue(element.QualifiedName, out var userControl))
        {
            context.UsedControlNamespaces.Add(userControl.Namespace);
            // Fully qualified component tag: same-named user controls in different
            // folders (frontend/admin NumericTextBox etc.) would otherwise be ambiguous
            // once both namespaces are imported
            _userControlPropertyTypes = userControl.PropertyTypes;
            var emitted = EmitComponent(element, $"{userControl.Namespace}.{userControl.ComponentName}",
                mapping: null, createsField: true);
            _userControlPropertyTypes = null;
            return emitted;
        }

        var controlMapping = ControlMappings.Find(element.Prefix, element.Name);
        if (controlMapping is null)
        {
            // A render-based custom control whose ported source is available runs
            // unchanged under LegacyRenderHost. Plain markup children render between the
            // legacy begin/end tags; controls with template children keep the stub path
            // A <HeaderTemplate> / <FooterTemplate> does NOT stop a control being hosted.
            // They are static markup rendered around the control's own output - no data
            // item, no per-row instantiation - so Blazor can render them directly. That is
            // not true of an <ItemTemplate> (needs the data item) or of <Items>/<Columns>
            // (child control declarations the legacy control builds itself), which still
            // take the stub path. n2's Zone is nine of these and its templates are
            // "<div class=\"list\">" and "</div>".
            // The test is on the SHAPE of the name, not on the three hand-written lists.
            //
            // Those lists hold the templates of the BUILT-IN controls. A third-party
            // control declares its own - mojoPortal's SiteMapPath has NodeTemplate,
            // RootNodeTemplate and CurrentNodeTemplate - and none of them appear in any of
            // the lists, so this answered "no templates here", the control was hosted, and
            // its template children were emitted as ordinary content at data-binding depth
            // zero. Every "<%# Eval(...) %>" inside them was then reported as a data-bound
            // expression OUTSIDE a template and dropped: the breadcrumb rendered links
            // with no href and no text, which is worse than not rendering it.
            //
            // The residual said "outside a template" and it was inside one. That is the
            // part that made this hard to find, and it is why the check reads the name
            // rather than a list something has to be added to.
            var hasTemplateChildren = element.Children.OfType<ElementNode>().Any(child =>
                string.IsNullOrEmpty(child.Prefix)
                && !IsHostableTemplate(child)
                && !IsHostableCollection(child)
                && (child.Name.EndsWith("Template", StringComparison.OrdinalIgnoreCase)
                    || ControlMappings.DataBoundTemplates.Contains(child.Name)
                    || ControlMappings.PlainTemplates.Contains(child.Name)
                    || ControlMappings.StyleChildElements.Contains(child.Name)));
            string? legacyReason = null;
            var legacyTypeName = context.LegacyControlResolver?.Invoke(
                element.Prefix, element.Name, out legacyReason);
            if (!hasTemplateChildren && legacyTypeName is not null)
            {
                return EmitLegacyRenderHost(element, legacyTypeName);
            }

            // Which of several different situations this is, in the report rather than in
            // the reader's head. A tag prefix nobody registered, a type outside the ported
            // tree and a base chain this tool will not render each need a different answer.
            var why = hasTemplateChildren && legacyTypeName is not null
                ? $"{legacyTypeName} は移植済みですが、テンプレート子要素を持つため LegacyRenderHost では描画できません"
                : legacyReason;

            // Two different situations share this failure. A standard <asp:*> control has
            // a rendering WebForms itself defines - Login, Calendar, Wizard - and the only
            // thing missing is the compat component: that is the converter's backlog, not
            // anyone's decision. A vendored control whose source is not in the input tree
            // is different: its behaviour lives in a binary the tool cannot read, so the
            // replacement has to come from the user via --control-map.
            var isStandardControl = element.Prefix.Equals("asp", StringComparison.OrdinalIgnoreCase);
            Residual(ResidualKind.UnmappedControl,
                isStandardControl
                    ? $"<{element.QualifiedName}> は標準コントロールですが互換コンポーネントが未実装です。"
                    : $"<{element.QualifiedName}> は未対応コントロールです"
                      + (why is null ? string.Empty : $"({why})")
                      + "。移植ソースが無い場合は --control-map で置き換え先の指定が必要です。",
                isStandardControl ? ResidualDisposition.Backlog : ResidualDisposition.NeedsInput);

            // Emit a generated placeholder component so the output still compiles and the
            // missing control is visible on the page (the residual above stays on record)
            if (context.StubNamespace is not null)
            {
                var stubName = "Stub_" + SanitizeIdentifier(
                    string.IsNullOrEmpty(element.Prefix) ? element.Name : $"{element.Prefix}_{element.Name}");
                context.StubComponents[element.QualifiedName] = stubName;
                context.UsedControlNamespaces.Add(context.StubNamespace);

                // Said out loud, because dropping the children also drops whatever the
                // children would have been reported for. The control above is already a
                // residual; this names what went with it.
                if (element.Children.Count > 0)
                {
                    context.Report.Info(context.SourceName,
                        $"<{element.QualifiedName}> の子マークアップ(テンプレート等)は出力していません"
                        + "(プレースホルダは何も描画しないため)。置き換え先を決めると復活します。");
                }

                return EmitComponent(WithoutChildren(element), stubName, mapping: null, createsField: true);
            }

            return $"@* TODO(W2B): unmapped control <{element.QualifiedName}> *@"
                   + EmitNodes(element.Children);
        }

        return EmitComponent(element, controlMapping.Component, controlMapping, controlMapping.CreatesField);
    }

    /// <summary>runat="server" HTML elements whose tag is dropped (layout territory).</summary>
    private static readonly HashSet<string> StructuralHtmlElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "form", "head", "body", "html", "title",
    };

    /// <summary>
    /// Emits an HtmlGenericControl for a runat="server" HTML element: the tag survives,
    /// ID becomes a code-behind field (the WebForms designer declared HtmlGenericControl /
    /// HtmlTableRow / ... the same way), and the remaining attributes pass through.
    /// </summary>
    /// <summary>
    /// The HtmlControl WebForms creates for a runat="server" &lt;input&gt;, decided by its type
    /// attribute exactly as the WebForms parser decided it.
    ///
    /// Every one of them used to come out as HtmlGenericControl, which has none of the
    /// members that make the specific control useful: 281 of YAF's build errors were
    /// "HtmlGenericControl has no definition for PostedFile" from
    /// &lt;input type="file" runat="server"&gt;. The tag survives either way; what differs is
    /// whether the code-behind can still talk to it.
    /// </summary>
    private static string? HtmlInputControlFor(ElementNode element)
    {
        if (!element.Name.Equals("input", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return element.Attributes.GetValueOrDefault("type")?.ToLowerInvariant() switch
        {
            "file" => "HtmlInputFile",
            _ => null,
        };
    }

    private string EmitHtmlGenericControl(ElementNode element)
    {
        // A typed HtmlControl renders its own tag, so it does not take TagName.
        if (HtmlInputControlFor(element) is { } inputControl)
        {
            return EmitComponent(element, inputControl, mapping: null, createsField: true);
        }

        var id = element.Id;
        var attributes = new List<string> { $"TagName=\"{element.Name.ToLowerInvariant()}\"" };
        if (id is not null)
        {
            attributes.Add($"ID=\"{id}\"");
        }

        var passthrough = new List<KeyValuePair<string, string>>();
        foreach (var (name, value) in element.Attributes)
        {
            if (name.Equals("runat", StringComparison.OrdinalIgnoreCase)
                || name.Equals("ID", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            if (name.Equals("visible", StringComparison.OrdinalIgnoreCase))
            {
                if (TryConvertDataBindValue(value, out var visibleCode))
                {
                    attributes.Add($"Visible=\"@(global::System.Convert.ToBoolean({visibleCode}))\"");
                }
                // An expression builder is handled for every other attribute (further down)
                // but was not for this one, so Visible="<%$ HasValue: LogoUrl %>" became a
                // residual even with the prefix in --expression-map. It is the same
                // conversion; only the attribute differs.
                else if (ExpressionBuilders.TryParseValue(value, out var visiblePrefix, out var visibleValue)
                         && ExpressionBuilders.TryConvert(visiblePrefix, visibleValue, out var visibleExpression))
                {
                    attributes.Add($"Visible=\"@(global::System.Convert.ToBoolean({visibleExpression}))\"");
                    context.Report.Info(context.SourceName,
                        $"<{element.Name} runat=\"server\"> の Visible=\"<%$ {visiblePrefix}:{visibleValue} %>\" を変換しました。");
                }
                else if (value.Contains("<%", StringComparison.Ordinal))
                {
                    Residual(ResidualKind.DataBinding,
                        $"<{element.Name} runat=\"server\"> の Visible=\"{Truncate(value)}\" は変換できません。属性を除去しました。");
                }
                else
                {
                    attributes.Add($"Visible=\"{value.ToLowerInvariant()}\"");
                }
                continue;
            }
            if (name.StartsWith("onserver", StringComparison.OrdinalIgnoreCase))
            {
                // onserverclick is the HtmlAnchor / HtmlButton click: the compat control
                // raises it from the DOM click, so the handler name ports as-is.
                if (name.Equals("onserverclick", StringComparison.OrdinalIgnoreCase))
                {
                    attributes.Add($"OnServerClick=\"@(e => {value}(this, e))\"");
                    context.Report.Info(context.SourceName,
                        $"<{element.Name} runat=\"server\"> の {name}=\"{value}\" を OnServerClick に変換しました。");
                    continue;
                }

                Residual(ResidualKind.CodeBehind,
                    $"<{element.Name} runat=\"server\"> のサーバーイベント {name} は未対応です。");
                continue;
            }
            passthrough.Add(new KeyValuePair<string, string>(name, value));
        }

        if (BuildPassthroughAttribute(passthrough, element.QualifiedName) is { } passthroughAttribute)
        {
            attributes.Add(passthroughAttribute);
        }

        if (id is not null && _templateDepth == 0)
        {
            attributes.Add($"@ref=\"{id}\"");
            context.Fields.Add(new ControlField("HtmlGenericControl", id));
        }

        context.Report.ConvertedControls++;
        context.Report.Info(context.SourceName,
            $"<{element.Name} runat=\"server\"> を HtmlGenericControl として変換しました(タグ保持)。");

        attributes = DedupeAttributes(attributes);
        return element.SelfClosing || element.Children.Count == 0
            ? $"<HtmlGenericControl {string.Join(" ", attributes)} />"
            : $"<HtmlGenericControl {string.Join(" ", attributes)}>{EmitNodes(element.Children)}</HtmlGenericControl>";
    }

    /// <summary>
    /// WebForms tolerates the same attribute written twice (and the style-child element
    /// form plus the dashed attribute form can map onto one parameter); Razor rejects
    /// duplicate parameters, so the last occurrence wins - the WebForms parser behavior.
    /// </summary>
    private static List<string> DedupeAttributes(List<string> attributes)
    {
        var indexByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var result = new List<string>();
        foreach (var attribute in attributes)
        {
            var equalsIndex = attribute.IndexOf('=');
            var name = equalsIndex < 0 ? attribute : attribute[..equalsIndex];
            if (indexByName.TryGetValue(name, out var existingIndex))
            {
                result[existingIndex] = attribute;
            }
            else
            {
                indexByName[name] = result.Count;
                result.Add(attribute);
            }
        }
        return result;
    }

    /// <summary>
    /// Emits a LegacyRenderHost that instantiates the ported legacy control class and
    /// injects its Render(HtmlTextWriter) output. Markup attributes travel as property
    /// values (the host applies them by reflection with type conversion).
    /// </summary>
    /// <summary>
    /// Templates a LegacyRenderHost can render itself: static markup around the control's
    /// own output, with no data item behind it.
    /// </summary>
    private static readonly Dictionary<string, string> LegacyHostableTemplates =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["HeaderTemplate"] = "HeaderContent",
            ["FooterTemplate"] = "FooterContent",
        };

    /// <summary>
    /// Template elements whose content is handed to the legacy control as an ITemplate.
    /// A separator only makes sense placed BETWEEN items, which only the control can do -
    /// there is no "before" or "after" the host could use.
    /// </summary>
    private static readonly HashSet<string> LegacyTemplateProperties =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "HeaderTemplate", "FooterTemplate", "SeparatorTemplate", "ItemSeparatorTemplate",
            "EmptyDataTemplate", "EmptyItemTemplate",
        };

    /// <summary>
    /// Whether a template child can travel to the legacy control as fixed markup.
    ///
    /// Only static content qualifies. A template holding a data-bound expression or a
    /// server control has to be instantiated per item with the item in scope, and a string
    /// cannot carry that - those still take the stub path.
    /// </summary>
    private static bool IsHostableTemplate(ElementNode child)
        => (LegacyHostableTemplates.ContainsKey(child.Name)
            || LegacyTemplateProperties.Contains(child.Name)
            // Any other "...Template" slot a third-party control declares, provided the
            // content is static. n2's ControlPanel has six of its own
            // (DragDropFooterTemplate, HiddenTemplate, ...) and they are plain divs; the
            // control decides which one to show, so handing it the markup keeps that
            // decision where it was. Listing only the built-in names sent the whole
            // control to the stub instead, which rendered none of them.
            //
            // The parser now surfaces these as elements at all - before, they came
            // through as text and the host emitted every template's markup at once,
            // unconditionally, which is not what any of them meant.
            || child.Name.EndsWith("Template", StringComparison.OrdinalIgnoreCase))
           && IsStaticMarkup(child);

    /// <summary>
    /// Static template content as the HTML it was in the .aspx.
    ///
    /// Deliberately NOT EmitNodes: that produces Razor, where "@" is escaped as "@@" and
    /// text is emitted for a Razor parser. This string ends up inside a C# literal and is
    /// written to the response verbatim, so it has to be the original markup.
    /// </summary>
    private static string EmitStaticMarkup(IEnumerable<AspxNode> nodes)
    {
        var builder = new StringBuilder();
        foreach (var node in nodes)
        {
            switch (node)
            {
                case TextNode text:
                    builder.Append(text.Text);
                    break;
                case ElementNode element:
                    builder.Append('<').Append(element.Name);
                    foreach (var attribute in element.Attributes)
                    {
                        builder.Append(' ').Append(attribute.Key)
                            .Append("=\"").Append(attribute.Value).Append('"');
                    }
                    if (element.SelfClosing)
                    {
                        builder.Append(" />");
                        break;
                    }
                    builder.Append('>')
                        .Append(EmitStaticMarkup(element.Children))
                        .Append("</").Append(element.Name).Append('>');
                    break;
            }
        }
        return builder.ToString();
    }

    /// <summary>
    /// Markup as a C# string literal that Razor will not read as markup.
    ///
    /// A verbatim literal is not enough: Razor scans an attribute's C# expression for tag
    /// starts, so @"&lt;div id=""x""&gt;" came back as RZ9980 "Unclosed tag 'div'". Escaping
    /// "&lt;" as < is the same answer TagBalance.Neutralize already uses, and the
    /// compiler produces the identical string.
    /// </summary>
    private static string Quote(string value)
        => "\"" + value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("<", "\\u003c", StringComparison.Ordinal)
            + "\"";

    /// <summary>
    /// Collection elements: their children are not rendered by the page, they are entries
    /// the parent control keeps in a list and renders itself.
    /// </summary>
    private static readonly HashSet<string> LegacyCollectionElements =
        new(StringComparer.OrdinalIgnoreCase) { "Items", "Columns", "Fields" };

    /// <summary>
    /// Whether a collection element's entries can all be built at run time: every child
    /// has to name a type that exists, since a collection missing half its entries would
    /// render a control that is quietly wrong - worse than the stub, which is visibly
    /// absent.
    /// </summary>
    /// <summary>
    /// Pulls the ported-column entries out of a &lt;Columns&gt; child and returns their
    /// declarations. They are REMOVED from the tree so the stub path does not also report
    /// them - a column carried this way is converted, not missing.
    /// </summary>
    private List<string> TakeLegacyColumns(ElementNode element)
    {
        var declarations = new List<string>();
        var columns = element.Children.OfType<ElementNode>().FirstOrDefault(child =>
            string.IsNullOrEmpty(child.Prefix)
            && child.Name.Equals("Columns", StringComparison.OrdinalIgnoreCase));
        if (columns is null)
        {
            return declarations;
        }

        foreach (var entry in columns.Children.OfType<ElementNode>().ToList())
        {
            if (ControlMappings.Find(entry.Prefix, entry.Name) is not null
                || context.AnyTypeResolver?.Invoke(entry.Prefix, entry.Name) is not { } entryType)
            {
                continue;
            }

            var values = entry.Attributes
                .Where(pair => !pair.Key.Equals("runat", StringComparison.OrdinalIgnoreCase))
                .Select(pair => $"[{Quote(pair.Key)}] = {Quote(pair.Value)}");
            declarations.Add(
                $"new global::WebForm2Blazor.Components.LegacyChild({Quote("Columns")}, "
                + $"{Quote(entryType)}, new global::System.Collections.Generic.Dictionary<string, string>("
                + "global::System.StringComparer.OrdinalIgnoreCase) { "
                + string.Join(", ", values) + " })");

            context.Report.Info(context.SourceName,
                $"<{entry.QualifiedName}> を移植済みの列 {entryType} として <Columns> に組み込みました"
                + "(WebForms の列プロトコルで、列自身がセルを描画します)。");
            columns.Children.Remove(entry);
        }

        return declarations;
    }

    private bool IsHostableCollection(ElementNode child)
        => LegacyCollectionElements.Contains(child.Name)
           && child.Children.OfType<ElementNode>().Any()
           && child.Children.OfType<ElementNode>().All(entry =>
               context.AnyTypeResolver?.Invoke(entry.Prefix, entry.Name) is not null);

    private static bool IsStaticMarkup(ElementNode element)
        => element.Descendants().All(descendant =>
               string.IsNullOrEmpty(descendant.Prefix)
               && !descendant.Attributes.ContainsKey("runat"))
           && !element.Children.OfType<ExpressionNode>().Any()
           && !element.Descendants().SelectMany(descendant => descendant.Children)
               .OfType<ExpressionNode>().Any();

    private string EmitLegacyRenderHost(ElementNode element, string legacyTypeName)
    {
        var id = element.Id;
        var properties = element.Attributes
            .Where(pair => !pair.Key.Equals("runat", StringComparison.OrdinalIgnoreCase)
                           && !pair.Key.Equals("ID", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var attributes = new List<string>();
        if (id is not null)
        {
            attributes.Add($"ID=\"{id}\"");
        }
        attributes.Add($"TypeName=\"{legacyTypeName}\"");
        if (BuildExpressionDictionary(properties, element.QualifiedName, "PropertyValues",
                stringifyExpressions: false, entryKind: "プロパティ") is { } propertyValues)
        {
            attributes.Add(propertyValues);
        }

        if (id is not null && _templateDepth == 0)
        {
            // The field the code-behind uses is the LEGACY CONTROL, not this host.
            // @ref can only capture the component, so it captures a separate host field
            // and the named field reaches through it to ControlInstance. Binding @ref to
            // the named field directly is what BlogEngine's "recaptcha.UserUniqueIdentifier"
            // and "pager1.Posts" hit: both resolved against LegacyRenderHost and threw.
            attributes.Add($"@ref=\"__{id}_host\"");

            // The ported control's own type, not dynamic: the type IS known here, and a
            // dynamic field spreads into every expression it touches - an extension method
            // called with a dynamic argument is CS1973, which cost YAF 44 errors.
            //
            // A CONVERTED control is re-namespaced (YAF.Web.Controls.Form ->
            // yaf.Components.Pages.Web.Controls.Form), and these fields are generated after
            // that rewrite has run, so the original name survives here and one of them -
            // YAF's ForumPageBase.form1 - does not resolve. Dropping the global:: prefix so
            // the rewrite could reach it was measured and did not help, for the same
            // reason: the pass had already finished. One error against 447 is the trade
            // taken for now; the fix is to re-namespace the field types where they are
            // emitted rather than before.
            context.Fields.Add(new ControlField("global::" + legacyTypeName, id, LegacyHost: true));
        }

        context.Report.Info(context.SourceName,
            $"<{element.QualifiedName}> を LegacyRenderHost({legacyTypeName})に変換しました(移植ソースの Render を実行して描画。表示専用)。");
        context.Report.ConvertedControls++;

        if (element.SelfClosing || element.Children.Count == 0)
        {
            return $"<LegacyRenderHost {string.Join(" ", attributes)} />";
        }

        // Static templates go to the control itself as ITemplate values - it knows where
        // each belongs, which is the only way a <SeparatorTemplate> lands between items.
        // <HeaderTemplate> / <FooterTemplate> ALSO become named fragments, as a fallback
        // for a control that declares no such property: those two do have a meaningful
        // "before" and "after". LegacyRenderHost suppresses the fragment when the control
        // accepted the template, so nothing renders twice.
        var templates = new StringBuilder();
        var templateMarkup = new List<string>();
        var collectionEntries = new List<string>();
        var rest = new List<AspxNode>();
        foreach (var child in element.Children)
        {
            // <Items> / <Columns>: each entry becomes a type name plus its attributes, and
            // LegacyRenderHost adds the built objects to the control's own collection -
            // which is what the WebForms parser did.
            if (child is ElementNode collection && string.IsNullOrEmpty(collection.Prefix)
                && IsHostableCollection(collection))
            {
                foreach (var entry in collection.Children.OfType<ElementNode>())
                {
                    var entryType = context.AnyTypeResolver!.Invoke(entry.Prefix, entry.Name)!;
                    var values = entry.Attributes
                        .Where(pair => !pair.Key.Equals("runat", StringComparison.OrdinalIgnoreCase))
                        .Select(pair => $"[{Quote(pair.Key)}] = {Quote(pair.Value)}");
                    collectionEntries.Add(
                        $"new global::WebForm2Blazor.Components.LegacyChild({Quote(collection.Name)}, "
                        + $"{Quote(entryType)}, new global::System.Collections.Generic.Dictionary<string, string>("
                        + "global::System.StringComparer.OrdinalIgnoreCase) { "
                        + string.Join(", ", values) + " })");
                }
                continue;
            }

            if (child is not ElementNode template || !string.IsNullOrEmpty(template.Prefix)
                || !IsHostableTemplate(template))
            {
                rest.Add(child);
                continue;
            }

            // Everything hostable EXCEPT the two that travel as render fragments: those
            // are placed around the control's output by the host itself, and sending them
            // as markup too would draw them twice.
            if (!LegacyHostableTemplates.ContainsKey(template.Name))
            {
                templateMarkup.Add(
                    $"[\"{template.Name}\"] = {Quote(EmitStaticMarkup(template.Children))}");
            }

            if (LegacyHostableTemplates.TryGetValue(template.Name, out var parameterName))
            {
                // A header and a footer are balanced TOGETHER, not separately: n2's Zone
                // opens "<div class="list">" in the header and closes it in the footer.
                // Razor parses each fragment on its own, so each half has to be
                // neutralized into raw output - the DOM the browser builds is the same.
                templates.Append($"<{parameterName}>{TagBalance.Neutralize(EmitNodes(template.Children))}</{parameterName}>");
            }
        }

        if (collectionEntries.Count > 0)
        {
            attributes.Add("CollectionChildren=\"@(new global::WebForm2Blazor.Components.LegacyChild[] { "
                + string.Join(", ", collectionEntries) + " })\"");
        }

        if (templateMarkup.Count > 0)
        {
            attributes.Add("TemplateMarkup=\"@(new global::System.Collections.Generic.Dictionary<string, string>("
                + "global::System.StringComparer.OrdinalIgnoreCase) { "
                + string.Join(", ", templateMarkup) + " })\"");
        }

        return $"<LegacyRenderHost {string.Join(" ", attributes)}>{templates}{EmitNodes(rest)}</LegacyRenderHost>";
    }

    private string EmitComponent(ElementNode element, string component, ControlMapping? mapping, bool createsField)
    {
        var attributes = new List<string>();
        var passthrough = new List<KeyValuePair<string, string>>();
        string? id = null;

        // Under a ListView, enable the itemPlaceholder substitution in LayoutTemplate
        var previousPlaceholderId = _layoutPlaceholderId;
        var previousGroupPlaceholderId = _listViewGroupPlaceholderId;
        if (component == "ListView")
        {
            var placeholderId = element.Attributes.GetValueOrDefault("ItemPlaceholderID");
            _layoutPlaceholderId = string.IsNullOrEmpty(placeholderId) ? "itemPlaceholder" : placeholderId;

            var groupPlaceholderId = element.Attributes.GetValueOrDefault("GroupPlaceholderID");
            _listViewGroupPlaceholderId = FindTemplate(element, "GroupTemplate") is null
                ? null
                : string.IsNullOrEmpty(groupPlaceholderId) ? "groupPlaceholder" : groupPlaceholderId;
        }

        // The component the children are being placed INTO, so a template child can ask
        // it for the canonical spelling of its own parameter (see EmitTemplate).
        var previousComponent = _currentComponent;
        _currentComponent = component;

        // Model binding (4.5): typed Item references in the templates cast to this type
        var previousItemType = _currentItemType;
        var declaredItemType = element.Attributes.GetValueOrDefault("ItemType");
        if (!string.IsNullOrEmpty(declaredItemType))
        {
            _currentItemType = declaredItemType;
        }

        // Flatten style child elements such as <HeaderStyle CssClass=... /> into
        // "HeaderStyle-CssClass"-style attributes (same conversion path as the attribute form)
        var effectiveAttributes = new List<KeyValuePair<string, string>>(element.Attributes);
        foreach (var styleChild in element.Children.OfType<ElementNode>()
                     .Where(child => string.IsNullOrEmpty(child.Prefix)
                                     && ControlMappings.StyleChildElements.Contains(child.Name)))
        {
            foreach (var (styleAttr, styleValue) in styleChild.Attributes)
            {
                if (!styleAttr.Equals("runat", StringComparison.OrdinalIgnoreCase))
                {
                    effectiveAttributes.Add(new KeyValuePair<string, string>(
                        $"{styleChild.Name}-{styleAttr}", styleValue));
                }
            }
        }

        ApplyImplicitLocalization(element, effectiveAttributes);

        foreach (var (name, value) in effectiveAttributes)
        {
            // ItemPlaceholderID / GroupPlaceholderID are consumed at conversion time
            // (not runtime parameters)
            if (name.Equals("ItemPlaceholderID", StringComparison.OrdinalIgnoreCase)
                || name.Equals("GroupPlaceholderID", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (ControlMappings.KnownNoOpAttributes.Contains(name))
            {
                if (!name.Equals("runat", StringComparison.OrdinalIgnoreCase))
                {
                    context.Report.Info(context.SourceName,
                        $"<{element.QualifiedName}> の属性 {name} は Blazor では不要なため除去しました。");
                }
                continue;
            }

            if (name.Equals("ID", StringComparison.OrdinalIgnoreCase))
            {
                id = value;
            }

            // User controls (mapping == null) expose public properties directly as
            // [Parameter], so pass attribute names through unconverted
            var parameterName = mapping is null ? name : mapping.Attributes.GetValueOrDefault(name);

            // Expression builders: Text="<%$ AppSettings:Key %>" etc. Convert to a C#
            // expression, or drop with a residual - never leak the raw text into the page
            if (ExpressionBuilders.TryParseValue(value, out var builderPrefix, out var builderValue))
            {
                if (parameterName is not null
                    && ExpressionBuilders.TryConvert(builderPrefix, builderValue, out var builderExpression))
                {
                    attributes.Add(RazorExpressionAttribute(parameterName, builderExpression));
                    context.Report.Info(context.SourceName,
                        $"式ビルダー {name}=\"<%$ {builderPrefix}:{builderValue} %>\" を変換しました。");
                    continue;
                }

                // Distinguish the two failure modes: an unknown prefix needs an
                // --expression-map entry, but a KNOWN prefix on an unmapped attribute is a
                // mapping-table gap - reporting both as "unsupported builder" hid that
                Residual(ResidualKind.InlineCode, parameterName is null
                    ? $"<{element.QualifiedName}> の属性 {name} がマッピング未定義のため、"
                      + $"式ビルダー <%$ {builderPrefix}:{Truncate(builderValue)} %> ごと除去しました。"
                    : $"<{element.QualifiedName}> の式ビルダー {name}=\"<%$ {builderPrefix}:{Truncate(builderValue)} %>\" は未対応です"
                      + "(--expression-map で変換式を指定できます)。属性を除去しました。");
                continue;
            }

            if (parameterName is null)
            {
                // WebForms IAttributeAccessor semantics: an attribute matching no server
                // property renders verbatim (expando). WebForms decides by property
                // existence - the loaded catalog reproduces that exactly; the lowercase
                // heuristic ("Style" = the Style collection's markup form) is the
                // fallback when the catalog does not know the control.
                var isServerProperty = PropertyKnowledge.IsServerProperty(element.Name, name);

                // Case-INSENSITIVE: markup writes style="..." far more often than
                // Style="...", and an ordinal match let the lowercase form fall through to
                // "unmapped" and be dropped - inline CSS silently disappearing from the page
                var isStyle = name.Equals("Style", StringComparison.OrdinalIgnoreCase);
                var isExpando = !isStyle
                                && (isServerProperty == false
                                    || (isServerProperty is null && IsExpandoAttribute(name)));

                if (mapping is { CreatesField: true } && (isExpando || isStyle))
                {
                    passthrough.Add(new KeyValuePair<string, string>(isStyle ? "style" : name, value));
                    context.Report.Info(context.SourceName,
                        $"<{element.QualifiedName}> の属性 {name} は HTML 属性としてそのまま出力します(WebForms の expando 属性)。");
                    continue;
                }

                Residual(ResidualKind.UnmappedAttribute,
                    $"<{element.QualifiedName}> の属性 {name}=\"{value}\" はマッピング未定義のため除去しました。");
                continue;
            }

            // The declared parameter type decides how a markup value must be rendered:
            // Razor parses the value of a NON-string parameter as a C# expression.
            var parameterType = ResolveParameterType(component, parameterName);

            // Data-binding expression in an attribute value: CommandArgument='<%# Eval("Id") %>' etc.
            if (value.StartsWith("<%#", StringComparison.Ordinal) && value.EndsWith("%>", StringComparison.Ordinal))
            {
                var convertedBinding = parameterType is null
                    ? EmitAttributeDataBinding(value[3..^2].Trim())
                    : EmitTypedAttributeDataBinding(value[3..^2].Trim(), parameterType);
                if (convertedBinding.Length > 0)
                {
                    attributes.Add($"{parameterName}=\"{convertedBinding}\"");
                }
                // Unconvertible (already a residual): omit the attribute entirely -
                // an empty value would not compile against int/bool parameters
                continue;
            }

            if (parameterType is not null)
            {
                // An empty value on a non-string parameter (Selected="") carries no
                // information: WebForms leaves the property at its default, and so does
                // omitting the attribute. Not a residual - nothing was lost.
                if (string.IsNullOrWhiteSpace(value))
                {
                    context.Report.Info(context.SourceName,
                        $"<{element.QualifiedName}> の属性 {name} は値が空のため既定値のままにしました。");
                    continue;
                }

                var typedLiteral = RenderTypedLiteral(value, parameterType);
                if (typedLiteral is null)
                {
                    Residual(ResidualKind.UnmappedAttribute,
                        $"<{element.QualifiedName}> の属性 {name}=\"{Truncate(value)}\" は "
                        + $"{parameterType.FullTypeName} 型のパラメータに変換できないため除去しました。");
                    continue;
                }
                attributes.Add($"{parameterName}=\"{typedLiteral}\"");
                continue;
            }

            // Values containing @ (e.g. an email regex) would be read by Razor as the start
            // of an expression, so emit them as a C# verbatim string literal expression.
            // Unit-style sizes ("50px") on user-control components need the same treatment:
            // a non-string parameter type makes Razor parse the raw value as C#
            // (the compat Unit converts implicitly from the string).
            if (value.Contains('@') || (mapping is null && UnitValueRegex().IsMatch(value)))
            {
                var verbatim = value.Replace("\"", "\"\"");
                attributes.Add($"{parameterName}=\"@(@\"{verbatim}\")\"");
                continue;
            }

            attributes.Add($"{parameterName}=\"{EscapeAttributeValue(NormalizeValue(parameterName, value))}\"");
        }

        // In WebForms, controls inside a template are duplicated per row and therefore
        // do not become page fields (they are not in designer.cs either)
        if (id is not null && createsField && _templateDepth == 0)
        {
            // Connect via @ref so code-behind can write txtName.Text just like WebForms.
            // Stub placeholders get a dynamic field: the original code-behind accesses
            // the real control's members (editor.Value etc.), which a generated stub
            // cannot declare - dynamic keeps that code compiling (inert at runtime).
            attributes.Add($"@ref=\"{id}\"");

            // Fully qualified, because a PAGE can be named after a control. BlogEngine has
            // Login.aspx, whose class is Login in the page namespace, and a bare "Login"
            // field there resolves to the page - not the compat control - so every member
            // the code-behind touches is missing. The enclosing namespace is checked before
            // any using, so only global:: settles it.
            var declaredType = component.StartsWith("Stub_", StringComparison.Ordinal)
                ? "dynamic"
                : CodeBehindRewriter.DeclaresCompatType(component)
                    ? CompatNames.QualifiedPrefix + component
                    : component;
            context.Fields.Add(new ControlField(declaredType, id));
        }

        if (id is not null && mapping is not null && _templateDepth == 0)
        {
            context.SmokeControls.Add(new SmokeControl(
                id,
                component,
                mapping.AssertPresence,
                Click: element.Attributes.ContainsKey("OnClick"),
                Change: element.Attributes.ContainsKey("OnSelectedIndexChanged")
                        || element.Attributes.ContainsKey("OnCheckedChanged")
                        || element.Attributes.ContainsKey("OnTextChanged"),
                Fill: component == "TextBox"));
        }

        // Constant parameters the mapping always emits (e.g. DataGrid's legacy rendering flag)
        if (mapping is not null)
        {
            foreach (var (fixedName, fixedValue) in mapping.FixedParameters)
            {
                if (!attributes.Any(attribute => attribute.StartsWith(fixedName + "=", StringComparison.Ordinal)))
                {
                    attributes.Add($"{fixedName}=\"{fixedValue}\"");
                }
            }
        }

        if (BuildPassthroughAttribute(passthrough, element.QualifiedName) is { } passthroughAttribute)
        {
            attributes.Add(passthroughAttribute);
        }

        // A <Columns> entry with no control mapping but a ported type is a WebForms COLUMN
        // (DNN's textcolumn, imagecommandcolumn). It is not a component and cannot go in
        // the ColumnsContent fragment, so it travels as a declaration and the grid builds
        // it - and then the ported column renders its own cells, through the same protocol
        // WebForms used.
        if (TakeLegacyColumns(element) is { Count: > 0 } legacyColumns)
        {
            attributes.Add("LegacyColumns=\"@(new global::WebForm2Blazor.Components.LegacyChild[] { "
                + string.Join(", ", legacyColumns) + " })\"");
        }

        context.Report.ConvertedControls++;

        attributes = DedupeAttributes(attributes);

        // Same collision as the field type above, on the TAG. n2 has Login.ascx, whose
        // component is Login in the application's own namespace, and <Login> inside that
        // very file resolves to itself. Razor takes a fully qualified tag, and the
        // qualification only goes on where a converted control really does share the name.
        var tag = CodeBehindRewriter.DeclaresCompatType(component) && SharesNameWithAConvertedControl(component)
            ? "WebForm2Blazor.Components." + component
            : component;

        var openTag = attributes.Count == 0 ? tag : $"{tag} {string.Join(" ", attributes)}";
        string result;
        if (element.SelfClosing || element.Children.Count == 0)
        {
            result = $"<{openTag} />";
        }
        else
        {
            result = $"<{openTag}>{EmitChildren(element)}</{tag}>";
        }

        _layoutPlaceholderId = previousPlaceholderId;
        _listViewGroupPlaceholderId = previousGroupPlaceholderId;
        _currentItemType = previousItemType;
        _currentComponent = previousComponent;
        return result;
    }

    /// <summary>
    /// Restructures the common WebForms pattern of "open a tag in HeaderTemplate and
    /// close it in FooterTemplate" into a single wrapper template.
    ///
    /// Razor requires matching tags, so a dangling open tag cannot be emitted as-is.
    /// Emitting raw HTML via MarkupString is not an option either: the browser parses
    /// each fragment individually and auto-closes the tag, so the items would not nest.
    /// </summary>
    private string EmitChildren(ElementNode element)
    {
        var header = FindTemplate(element, "HeaderTemplate");
        var footer = FindTemplate(element, "FooterTemplate");

        if (header is null || footer is null)
        {
            return EmitNodes(element.Children);
        }

        // Balance is judged on the raw text portions only: server controls and
        // expressions inside the templates always emit balanced markup, so any imbalance
        // comes from the raw HTML text - exactly the split-wrapper idiom targeted here.
        // (Deciding before emitting matters: emission has side effects such as field and
        // residual registration.)
        var headerText = RawText(header).Trim();
        var footerText = RawText(footer).Trim();

        var isSplitWrapper = !TagBalance.IsBalanced(headerText)
                             && !TagBalance.IsBalanced(footerText)
                             && TagBalance.IsBalanced(headerText + footerText);

        if (!isSplitWrapper)
        {
            return EmitNodes(element.Children);
        }

        // Controls inside the merged wrapper live in a template in WebForms terms:
        // no page fields / @ref for them
        _templateDepth++;
        var headerInner = EmitNodes(header.Children).Trim();
        var footerInner = EmitNodes(footer.Children).Trim();
        _templateDepth--;

        context.Report.Info(context.SourceName,
            $"HeaderTemplate/FooterTemplate が「{Truncate(headerInner)} … {Truncate(footerInner)}」でタグを分割していたため、"
            + "1 つの WrapperTemplate に組み替えました。");

        var builder = new StringBuilder();
        foreach (var child in element.Children)
        {
            if (ReferenceEquals(child, header))
            {
                builder.Append("<WrapperTemplate Context=\"items\">");
                builder.Append(headerInner);
                builder.Append("@items");
                builder.Append(footerInner);
                builder.Append("</WrapperTemplate>");
                continue;
            }
            if (ReferenceEquals(child, footer))
            {
                continue;
            }
            builder.Append(EmitNode(child));
        }
        return builder.ToString();
    }

    private static ElementNode? FindTemplate(ElementNode element, string templateName)
        => element.Children.OfType<ElementNode>()
            .FirstOrDefault(child => string.IsNullOrEmpty(child.Prefix)
                                     && child.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase));

    private static string RawText(ElementNode element)
        => string.Concat(element.Children.OfType<TextNode>().Select(child => child.Text));

    private string EmitTemplate(ElementNode element, bool dataBound)
    {
        var isLayout = element.Name.Equals("LayoutTemplate", StringComparison.OrdinalIgnoreCase);
        var isGroup = element.Name.Equals("GroupTemplate", StringComparison.OrdinalIgnoreCase);

        // With grouping, the LayoutTemplate's placeholder is the GROUP placeholder;
        // the item placeholder then lives inside the GroupTemplate
        var previousPlaceholderId = _layoutPlaceholderId;
        if (isLayout && _listViewGroupPlaceholderId is not null)
        {
            _layoutPlaceholderId = _listViewGroupPlaceholderId;
        }

        var fieldTransparent = FieldTransparentTemplates.Contains(element.Name);
        if (!fieldTransparent)
        {
            _templateDepth++;
        }
        if (dataBound)
        {
            _dataBindingTemplateDepth++;
        }
        // Nested data-bound templates need distinct context names (Razor rejects
        // shadowing); WebForms Container always binds to the innermost template
        var containerName = ContainerName;
        var inner = EmitNodes(element.Children);
        if (dataBound)
        {
            _dataBindingTemplateDepth--;
        }
        if (!fieldTransparent)
        {
            _templateDepth--;
        }

        // Placeholder written as plain HTML (no runat: <tr id="groupPlaceholder"></tr>).
        // In WebForms an HtmlTable parses its rows into server controls, so ListView
        // still finds it; here the emitted text is substituted directly.
        if ((isLayout || isGroup) && _layoutPlaceholderId is not null)
        {
            inner = Regex.Replace(inner,
                $@"<(\w+)[^>]*\bid\s*=\s*[""']{Regex.Escape(_layoutPlaceholderId)}[""'][^>]*>\s*</\1\s*>",
                "@ItemsPlaceholder", RegexOptions.IgnoreCase);
        }
        _layoutPlaceholderId = previousPlaceholderId;

        // Unbalanced plain HTML (stray close tags, unclosed <b> - tolerated by WebForms,
        // rejected by Razor) is neutralized into raw MarkupString output
        if (TagBalance.HasUnbalancedPlainTags(inner))
        {
            inner = TagBalance.Neutralize(inner);
            context.Report.Info(context.SourceName,
                $"<{element.Name}> 内の不整合な HTML タグを raw 出力に退避しました(WebForms は許容するが Razor は構文エラーのため)。");
        }

        // Context is only legal on a RenderFragment<T>, and it is only NEEDED when the
        // template body actually names the value - which, for a layout, means the item
        // placeholder was found and substituted above. Login also has a LayoutTemplate,
        // a plain RenderFragment, and emitting Context there is RZ9997.
        var contextAttribute = dataBound
            ? $" Context=\"{containerName}\""
            : (isLayout || isGroup) && inner.Contains("@ItemsPlaceholder", StringComparison.Ordinal)
                ? " Context=\"ItemsPlaceholder\""
                : string.Empty;
        var tagName = TemplateParameterNameFor(element.Name);
        return $"<{tagName}{contextAttribute}>{inner}</{tagName}>";
    }

    /// <summary>
    /// The parameter name to emit for a template element, asking the COMPONENT first.
    ///
    /// The mapping table answers for the built-in slots and stores the WebForms spelling,
    /// so a lower-case "&lt;itemtemplate&gt;" comes back as ItemTemplate. It cannot answer
    /// for a slot it does not list - mojoPortal writes "&lt;usernametemplate&gt;" on a
    /// PasswordRecovery, and emitting that spelling is a parameter the component does not
    /// have, which Razor rejects as unrecognized child content (RZ9996).
    ///
    /// The component itself does know: its [Parameter] names are read off the compat
    /// assembly. Asking it is the same rule the enum and template spellings already
    /// follow - the artifact, not a second list that can disagree with it.
    /// </summary>
    private string TemplateParameterNameFor(string writtenName)
    {
        var mapped = ControlMappings.TemplateParameterName(writtenName);
        if (!string.Equals(mapped, writtenName, StringComparison.Ordinal))
        {
            return mapped;
        }

        return _currentComponent is not null
               && ComponentParameterTypes.ParameterNameOf(_currentComponent, writtenName) is { } declared
            ? declared
            : mapped;
    }

    /// <summary>
    /// Whether a converted control of the application's own would answer to this name -
    /// either one this file registers, or THIS file itself. n2's Login.ascx becomes a
    /// component called Login in the application's namespace, and a bare &lt;Login&gt;
    /// inside it resolves to itself rather than to the compat control.
    /// </summary>
    private bool SharesNameWithAConvertedControl(string component)
        => context.UserControlTags.Values.Any(reference =>
               string.Equals(reference.ComponentName, component, StringComparison.Ordinal))
           || string.Equals(
               Path.GetFileNameWithoutExtension(context.SourceName.AsSpan()).ToString(),
               component,
               StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Context parameter name for the CURRENT data-bound template depth. Depth 1 keeps
    /// the WebForms-familiar "Container"; nested templates get numbered names.
    /// </summary>
    private string ContainerName
        => _dataBindingTemplateDepth <= 1 ? "Container" : $"Container{_dataBindingTemplateDepth}";

    /// <summary>Renames user-written Container references to the current depth's context name.</summary>
    private string RewriteContainerReferences(string code)
        => _dataBindingTemplateDepth <= 1 ? code : Regex.Replace(code, @"\bContainer\b", ContainerName);

    /// <summary>
    /// Records every server-control ID in the document into
    /// <see cref="EmitContext.DeclaredControlIds"/>. Run once over the whole parse tree
    /// before emitting, so a reference can be resolved regardless of whether the control
    /// is declared above or below it.
    /// </summary>
    public static void CollectDeclaredControlIds(IEnumerable<AspxNode> nodes, EmitContext context)
    {
        foreach (var element in nodes.OfType<ElementNode>())
        {
            foreach (var node in new[] { element }.Concat(element.Descendants()))
            {
                // Only runat="server" elements get an ID the framework rewrites; a plain
                // HTML id is already the DOM id and is never reached through ClientID.
                if (node.Id is { Length: > 0 } id
                    && node.Attributes.TryGetValue("runat", out var runat)
                    && runat.Equals("server", StringComparison.OrdinalIgnoreCase))
                {
                    context.DeclaredControlIds.Add(id);
                }
            }
        }
    }

    /// <summary>
    /// A control declared in this file used as "&lt;id&gt;.ClientID" - the WebForms idiom
    /// for "the DOM id this control will render with", overwhelmingly inside
    /// &lt;label for=...&gt;.
    ///
    /// The lookbehind keeps it to a bare identifier: "Foo.Bar.ClientID" is a property path
    /// through something else and must be left alone.
    /// </summary>
    private static readonly Regex ClientIdReferenceRegex =
        new(@"(?<![.\w])(?<id>[A-Za-z_]\w*)\s*\.\s*ClientID\b", RegexOptions.Compiled);

    /// <summary>
    /// Rewrites "txtName.ClientID" to "ClientIdOf(\"txtName\")".
    ///
    /// The control is an @ref field in the converted output, and Blazor assigns those only
    /// AFTER the first render - so the original expression is null-dereferenced on the way
    /// in and the page answers 500. BlogEngine's /contact and /post failed exactly here.
    ///
    /// The rewrite is limited to IDs declared in THIS file. An identifier the file does not
    /// declare may be an inherited code-behind member or an unrelated local, and there the
    /// original expression is the correct one.
    /// </summary>
    private string RewriteClientIdReferences(string code)
    {
        if (context.DeclaredControlIds.Count == 0 || !code.Contains("ClientID", StringComparison.Ordinal))
        {
            return code;
        }

        return ClientIdReferenceRegex.Replace(code, match =>
        {
            var id = match.Groups["id"].Value;
            if (!context.DeclaredControlIds.Contains(id))
            {
                return match.Value;
            }

            context.Report.Info(context.SourceName,
                $"{id}.ClientID を ClientIdOf(\"{id}\") に変換しました"
                + "(@ref は初回描画後にしか代入されないため、描画中は null になります)。");
            return $"ClientIdOf(\"{id}\")";
        });
    }

    private string EmitExpression(ExpressionNode expression)
    {
        switch (expression.Kind)
        {
            case ExpressionKind.Encoded:
                return $"@({RewriteClientIdReferences(expression.Code)})";

            case ExpressionKind.Render:
                // <%= %> renders WITHOUT HTML encoding in WebForms; MarkupString keeps
                // that semantic exactly (<%: %> is the encoded form and maps to @())
                context.Report.Info(context.SourceName,
                    $"<%= {Truncate(expression.Code)} %> を MarkupString 出力に変換しました(WebForms と同じ非エンコード描画)。");
                // Convert.ToString handles value types too (?. would not compile on int etc.)
                return $"@((global::Microsoft.AspNetCore.Components.MarkupString)global::System.Convert.ToString({RewriteClientIdReferences(expression.Code)}))";

            case ExpressionKind.DataBind:
                return EmitDataBinding(RewriteClientIdReferences(expression.Code));

            default:
                // A text-position expression builder ("<% $Prefix:Value %>")
                if (ExpressionBuilders.TryParseCode(expression.Code, out var builderPrefix, out var builderValue))
                {
                    if (ExpressionBuilders.TryConvert(builderPrefix, builderValue, out var builderExpression))
                    {
                        context.Report.Info(context.SourceName,
                            $"式ビルダー <%$ {builderPrefix}:{builderValue} %> を変換しました。");
                        return $"@({builderExpression})";
                    }
                    Residual(ResidualKind.InlineCode,
                        $"式ビルダー <%$ {builderPrefix}:{Truncate(builderValue)} %> は未対応です(--expression-map で変換式を指定できます)。");
                    return $"@* TODO(W2B): expression builder <%$ {expression.Code} %> *@";
                }

                // A brace-balanced statement block is not control flow spanning tags - it
                // is ordinary code, usually declaring the locals the rest of the markup
                // reads (<% var title = Server.HtmlEncode(Post.Title); %>). Razor's @{ }
                // scopes them the same way, and dropping the block instead made those
                // names silently disappear.
                if (IsStatementBlock(expression.Code))
                {
                    context.Report.Info(context.SourceName,
                        $"インラインコードブロック <% {Truncate(expression.Code)} %> を @{{ }} に変換しました。");
                    return $"@{{{expression.Code}}}";
                }

                Residual(ResidualKind.InlineCode,
                    $"インラインコードブロック <% {Truncate(expression.Code)} %> は自動変換できません。");
                return $"@* TODO(W2B): inline code <% {expression.Code} %> *@";
        }
    }

    /// <summary>
    /// Converts a data-binding expression in an attribute value into a Razor expression.
    /// The value is passed to a string parameter, so it is stringified with ?.ToString()
    /// (Razor correctly parses double quotes inside a @() expression as C#).
    /// </summary>
    /// <summary>
    /// A &lt;%# %&gt; outside any template is the Page.DataBind() idiom. When the
    /// expression does not touch the row context (no Container / Eval / Bind), it is a
    /// plain one-shot expression and converts safely to @().
    /// </summary>
    private static bool IsContainerFree(string code)
        => !ContainerReferenceRegex().IsMatch(code);

    /// <summary>
    /// Builds the PassthroughAttributes dictionary-literal attribute. A value that is a
    /// data-binding expression is converted to C#; any other server expression is dropped
    /// with a residual - raw &lt;% %&gt; text must never leak into the rendered page.
    /// </summary>
    private string? BuildPassthroughAttribute(
        List<KeyValuePair<string, string>> passthrough, string qualifiedName)
        => BuildExpressionDictionary(passthrough, qualifiedName, "PassthroughAttributes",
            stringifyExpressions: true, entryKind: "expando 属性");

    /// <summary>
    /// Rewrites the inside of a &lt;%# ... %&gt; attribute value into a C# expression valid
    /// at the emission point (Container-based inside templates). Returns false when the
    /// value is not a single data-binding expression or cannot be converted.
    /// </summary>
    private bool TryConvertDataBindValue(string value, out string code)
    {
        code = string.Empty;
        var trimmed = value.Trim();
        if (!trimmed.StartsWith("<%#", StringComparison.Ordinal)
            || !trimmed.EndsWith("%>", StringComparison.Ordinal)
            || trimmed.IndexOf("%>", StringComparison.Ordinal) != trimmed.Length - 2)
        {
            return false;
        }

        var inner = trimmed[3..^2].Trim();
        if (inner.StartsWith(':'))
        {
            inner = inner[1..].Trim();
        }
        inner = RewriteTypedItemReferences(inner);
        if (BindCallRegex().IsMatch(inner))
        {
            return false;
        }
        if (_dataBindingTemplateDepth > 0)
        {
            inner = RewriteContainerReferences(inner);
            inner = EvalCallRegex().Replace(inner, $"Eval({ContainerName}, ");
        }
        else if (!IsContainerFree(inner))
        {
            return false;
        }

        code = inner;
        return true;
    }

    /// <summary>
    /// Builds a Dictionary&lt;string, object&gt; literal attribute (PassthroughAttributes /
    /// PropertyValues). Data-binding expression values become C# expressions; any other
    /// server expression is dropped with a residual - raw &lt;% %&gt; text must never
    /// leak into the rendered page.
    /// </summary>
    private string? BuildExpressionDictionary(List<KeyValuePair<string, string>> entries,
        string qualifiedName, string parameterName, bool stringifyExpressions, string entryKind)
    {
        var pairs = new List<string>();
        foreach (var (key, value) in entries)
        {
            var escapedKey = key.Replace("\"", "\"\"");

            // Expression builders (<%$ Prefix:Value %>) reach here too - a legacy control's
            // properties and expando attributes carry localization values just like mapped
            // parameters do, and they must go through the same conversion
            if (ExpressionBuilders.TryParseValue(value, out var builderPrefix, out var builderValue)
                && ExpressionBuilders.TryConvert(builderPrefix, builderValue, out var builderExpression))
            {
                pairs.Add(stringifyExpressions
                    ? $"[@\"{escapedKey}\"] = global::System.Convert.ToString({builderExpression})"
                    : $"[@\"{escapedKey}\"] = ({builderExpression})");
                context.Report.Info(context.SourceName,
                    $"<{qualifiedName}> の {entryKind} {key} の式ビルダーを変換しました。");
                continue;
            }

            if (TryConvertDataBindValue(value, out var code))
            {
                pairs.Add(stringifyExpressions
                    ? $"[@\"{escapedKey}\"] = global::System.Convert.ToString({code})"
                    : $"[@\"{escapedKey}\"] = ({code})");
                context.Report.Info(context.SourceName,
                    $"<{qualifiedName}> の {entryKind} {key} のデータバインド式を変換しました。");
            }
            else if (value.Contains("<%", StringComparison.Ordinal))
            {
                Residual(ResidualKind.DataBinding,
                    $"<{qualifiedName}> の {entryKind} {key}=\"{Truncate(value)}\" はサーバー式を含み変換できません。除去しました。");
            }
            else
            {
                // Verbatim strings keep quote escaping simple
                pairs.Add($"[@\"{escapedKey}\"] = @\"{value.Replace("\"", "\"\"")}\"");
            }
        }

        return pairs.Count == 0
            ? null
            : $"{parameterName}=\"@(new global::System.Collections.Generic.Dictionary<string, object> {{ {string.Join(", ", pairs)} }})\"";
    }

    /// <summary>
    /// The declared type of a component parameter, when the value must be rendered as
    /// something other than a plain string. Returns null for string parameters and for
    /// everything the existing handling already covers (delegates, RenderFragment, ...),
    /// so those paths stay exactly as they were.
    /// </summary>
    private ParameterTypeInfo? ResolveParameterType(string component, string parameterName)
    {
        // A user control's own code-behind is the authority for its properties; the
        // compatibility components are read by reflection
        var fromSource = _userControlPropertyTypes?.GetValueOrDefault(parameterName) is not null;
        var declared = fromSource
            ? ComponentParameterTypes.FromSourceTypeName(_userControlPropertyTypes![parameterName])
            // A user control also inherits the compatibility base's parameters (Visible etc.)
            : ComponentParameterTypes.Find(
                _userControlPropertyTypes is null ? component : "WebFormsUserControl", parameterName);

        if (declared is null || declared.Kind == ParameterKind.String)
        {
            return null;
        }

        // "Other" on a compatibility component means a delegate / RenderFragment, whose
        // existing handling (method group, template) must stay untouched. On a user
        // control it is an ordinary business type (Product etc.) that must not be
        // stringified - only its data-binding path needs the typed treatment.
        return declared.Kind == ParameterKind.Other && !fromSource ? null : declared;
    }

    /// <summary>
    /// Renders a markup literal for a non-string component parameter. Returns null when
    /// the value cannot be expressed (the attribute is then dropped with a residual).
    /// </summary>
    private static string? RenderTypedLiteral(string value, ParameterTypeInfo parameterType)
    {
        var normalized = value.Trim();
        switch (parameterType.Kind)
        {
            case ParameterKind.Bool:
                return normalized.Equals("true", StringComparison.OrdinalIgnoreCase) ? "true"
                    : normalized.Equals("false", StringComparison.OrdinalIgnoreCase) ? "false"
                    : null;

            case ParameterKind.Numeric:
                return long.TryParse(normalized, out _) || double.TryParse(normalized, out _)
                    ? normalized
                    : null;

            case ParameterKind.Decimal:
                return decimal.TryParse(normalized, out _) ? normalized + "m" : null;

            // Unit-typed sizes ("50px") convert implicitly from the string
            case ParameterKind.Unit:
                return $"@(@\"{normalized.Replace("\"", "\"\"")}\")";

            // WebForms writes a comma-separated list for a string[] property
            // (DataKeyNames="Id,Name"); an empty value means an empty array, not null.
            case ParameterKind.StringArray:
            {
                var items = normalized
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => $"@\"{item.Trim().Replace("\"", "\"\"")}\"");
                return $"@(new string[] {{ {string.Join(", ", items)} }})";
            }

            // WebForms writes the bare member name (RepeatDirection="Horizontal"), and
            // .aspx matches it without regard to case - so the ENUM's spelling is what
            // gets emitted, not the markup's.
            case ParameterKind.Enum:
                return System.Text.RegularExpressions.Regex.IsMatch(normalized, @"^[A-Za-z_]\w*$")
                       && parameterType.MemberFor(normalized) is { } member
                    ? $"@({parameterType.FullTypeName}.{member})"
                    : null;

            // A business-typed property (Product etc.) cannot take a markup literal
            default:
                return null;
        }
    }

    /// <summary>Data binding assigned to a non-string parameter: no string conversion.</summary>
    private string EmitTypedAttributeDataBinding(string code, ParameterTypeInfo parameterType)
    {
        if (code.StartsWith(':'))
        {
            code = code[1..].Trim();
        }
        code = RewriteTypedItemReferences(code);
        if (_dataBindingTemplateDepth == 0 && !IsContainerFree(code))
        {
            Residual(ResidualKind.DataBinding,
                $"属性のデータバインド式 <%# {Truncate(code)} %> がテンプレート外にあります。");
            return string.Empty;
        }

        var rewritten = RewriteBindingExpression(code);
        // Eval returns object; a typed parameter needs a conversion. Convert.ToX handles
        // the DB-ish values WebForms bound this way (boxed ints, "True", DBNull).
        return parameterType.Kind switch
        {
            ParameterKind.Bool => $"@(global::System.Convert.ToBoolean({rewritten}))",
            ParameterKind.Numeric or ParameterKind.Decimal =>
                $"@(({parameterType.FullTypeName})global::System.Convert.ChangeType({rewritten}, typeof({parameterType.FullTypeName})))",
            ParameterKind.Unit => $"@(global::System.Convert.ToString({rewritten}))",
            _ => $"@(({parameterType.FullTypeName})({rewritten}))",
        };
    }

    private string EmitAttributeDataBinding(string code)
    {
        // <%#: expr %> is the HTML-encoded data-binding form (WebForms 4.5)
        if (code.StartsWith(':'))
        {
            code = code[1..].Trim();
        }
        code = RewriteTypedItemReferences(code);
        if (_dataBindingTemplateDepth == 0)
        {
            if (IsContainerFree(code))
            {
                context.Report.Info(context.SourceName,
                    $"テンプレート外のデータバインド式 <%# {Truncate(code)} %> を @() に変換しました(Page.DataBind イディオム)。");
                // Convert.ToString handles value types too (?. would not compile on int etc.)
                return $"@(global::System.Convert.ToString({code}))";
            }
            Residual(ResidualKind.DataBinding,
                $"属性のデータバインド式 <%# {Truncate(code)} %> がテンプレート外にあります。");
            return string.Empty;
        }

        var rewritten = RewriteBindingExpression(code);
        return $"@(global::System.Convert.ToString({rewritten}))";
    }

    /// <summary>
    /// The whole data-binding rewrite, in one place: container renaming, Eval's implicit
    /// container, then Bind() degraded to Eval().
    ///
    /// One method because there are THREE emit sites - text position, a string attribute
    /// and a typed attribute - and only the first one degraded Bind(). The other two
    /// passed "Bind(...)" through verbatim into the generated .razor, where nothing
    /// declares it: 13 of mojoPortal's "the name Bind does not exist in the current
    /// context", in the converter's own output. An attribute is where Bind() is USUALLY
    /// written, so the one path that had the rule was the one that needed it least.
    ///
    /// The ORDER is load-bearing and is why this is not three copies. Eval's rewrite has
    /// to run BEFORE Bind's, because Bind's produces an "Eval(Container, " that the Eval
    /// rewrite would then match again - "Eval(Container, Container, \"X\")", which is 13
    /// CS1503 where 13 CS0103 used to be. Copying the two lines to the other two sites is
    /// exactly how that was introduced.
    /// </summary>
    private string RewriteBindingExpression(string code)
    {
        var rewritten = RewriteContainerReferences(code);
        rewritten = EvalCallRegex().Replace(rewritten, $"Eval({ContainerName}, ");
        return DegradeBindToEval(rewritten, code);
    }

    /// <inheritdoc cref="RewriteBindingExpression"/>
    private string DegradeBindToEval(string rewritten, string original)
    {
        if (!BindCallRegex().IsMatch(rewritten))
        {
            return rewritten;
        }

        // Backlog, not Convertible.
        //
        // The DISPLAY side is already right - Eval reads the same value Bind would. What
        // is not carried is the write-back: WebForms used Bind's metadata to fill
        // e.NewValues on update, and a TemplateField here extracts nothing.
        //
        // That is a converter/compat capability, not something a model can fix by
        // rewriting this .razor - the correct .razor is the one already emitted. Left as
        // Convertible it filled the AI layer with 13 tasks whose only honest answer was
        // "no change", which is what made that layer's acceptance rate meaningless the
        // last time (see the note on retiring ManualMigration).
        //
        // An application that reads edited values through FindControl on the cell - the
        // idiom the compat GridView documents, and what mojoPortal does in every one of
        // these 13 - loses nothing at all.
        Residual(ResidualKind.DataBinding,
            $"双方向バインド Bind() は片方向の Eval() として出力しました: {Truncate(original)}"
            + "(表示は同じ値です。書き戻し e.NewValues は運んでいません — "
            + "編集値をセルの FindControl で読むコードは影響を受けません)。",
            ResidualDisposition.Backlog);
        return BindCallRegex().Replace(rewritten, $"Eval({ContainerName}, ");
    }

    /// <summary>
    /// WebForms 4.5 typed model-binding references: Item.X (and one-way-degraded
    /// BindItem.X) become a cast against the declaring control's ItemType.
    /// </summary>
    private string RewriteTypedItemReferences(string code)
    {
        if (_currentItemType is null || !TypedItemRegex().IsMatch(code))
        {
            return code;
        }
        var cast = $"((global::{_currentItemType})Container.DataItem)";
        return TypedItemRegex().Replace(code, cast);
    }

    private string EmitDataBinding(string code)
    {
        code = RewriteTypedItemReferences(code);
        if (_dataBindingTemplateDepth == 0)
        {
            if (IsContainerFree(code))
            {
                context.Report.Info(context.SourceName,
                    $"テンプレート外のデータバインド式 <%# {Truncate(code)} %> を @() に変換しました(Page.DataBind イディオム)。");
                return $"@({code})";
            }
            Residual(ResidualKind.DataBinding,
                $"データバインド式 <%# {Truncate(code)} %> がテンプレート外にあります。");
            return $"@* TODO(W2B): <%# {code} %> *@";
        }

        var rewritten = RewriteBindingExpression(code);
        return $"@({rewritten})";
    }

    /// <summary>
    /// The element with its children dropped, for the generated-stub path.
    ///
    /// A stub renders nothing but an HTML comment - its own documentation says the child
    /// markup "is intentionally not rendered" - so emitting that markup produced code
    /// nobody runs, and that code did not compile. A data-bound template carries
    /// Context="Container" and a body full of Container.DataItem, which needs a
    /// RenderFragment&lt;T&gt; parameter the stub does not have: 9 of mojoPortal's "the
    /// name Container does not exist in the current context", in the converter's own
    /// generated .razor.
    ///
    /// Declaring the slots on the stub instead was tried and is worse. Blazor takes every
    /// child element as ChildContent only while a component declares NO named
    /// RenderFragment; the first one switches it to parameter matching, and then any child
    /// that is not a parameter is RZ9996. The set cannot be written down in advance -
    /// n2's Repeater stub is handed a WrapperTemplate (this converter's own split-wrapper,
    /// synthesised after the children are read) and an EmptyTemplate (a name n2 invented,
    /// which the parser does not even see as an element). That attempt took n2 from 11
    /// build errors to 13.
    ///
    /// Nothing is hidden by dropping it: the control is already an UnmappedControl
    /// residual naming the tag, and the stub's comment names it again in the page source.
    /// </summary>
    private static ElementNode WithoutChildren(ElementNode element)
        => new()
        {
            Prefix = element.Prefix,
            Name = element.Name,
            Attributes = element.Attributes,
            SelfClosing = true,
            Line = element.Line,
        };

    /// <summary>
    /// True for markup attributes that are expando (plain HTML) attributes in WebForms:
    /// entirely lowercase names (autocomplete, class, data-*) plus "Style".
    /// </summary>
    private static bool IsExpandoAttribute(string name)
        => name.Equals("Style", StringComparison.Ordinal)
           || !name.Any(char.IsUpper);

    private static string SanitizeIdentifier(string value)
    {
        var builder = new StringBuilder();
        foreach (var c in value)
        {
            builder.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');
        }
        var result = builder.ToString();
        return result.Length > 0 && char.IsDigit(result[0]) ? "_" + result : result;
    }

    /// <summary>
    /// In Razor, @ starts an expression, so escape @ in plain text. Closing tags of void
    /// elements (&lt;/link&gt; etc. - tolerated by WebForms/browsers, a Razor error) are
    /// removed; the DOM is identical.
    /// </summary>
    private static string EscapeRazorText(string text)
        => VoidCloseTagRegex().Replace(text.Replace("@", "@@"), string.Empty);

    [GeneratedRegex(@"</(?:area|base|br|col|embed|hr|img|input|link|meta|param|source|track|wbr)\s*>",
        RegexOptions.IgnoreCase)]
    private static partial Regex VoidCloseTagRegex();

    [GeneratedRegex(@"^\d+(?:\.\d+)?(?:px|%|pt|em)$", RegexOptions.IgnoreCase)]
    private static partial Regex UnitValueRegex();

    private static string EscapeAttributeValue(string value) => value.Replace("\"", "&quot;");

    /// <summary>
    /// An attribute whose value is a C# expression, quoted so the expression survives.
    ///
    /// A string literal inside the expression is the problem: Attr="@("x")" ends the
    /// attribute at the second quote, and escaping it to &amp;quot; would change the C#
    /// rather than the markup. Razor accepts single quotes around an attribute, so the
    /// quote that does not occur in the expression is used. n2cms writes
    /// ZoneName="&lt;%$ Code:"AutoZone2" %&gt;", which is exactly this case.
    /// </summary>
    private static string RazorExpressionAttribute(string parameterName, string expression)
        => expression.Contains('"', StringComparison.Ordinal)
           && !expression.Contains('\'', StringComparison.Ordinal)
            ? $"{parameterName}='@({expression})'"
            : $"{parameterName}=\"@({expression})\"";

    /// <summary>
    /// Normalizes values of bool-typed parameters.
    /// WebForms markup commonly uses PascalCase like Font-Bold="True", but Razor parses
    /// bool parameter attribute values as C# expressions, so they must be true/false.
    /// </summary>
    private static readonly HashSet<string> BoolParameters = new(StringComparer.OrdinalIgnoreCase)
    {
        "CausesValidation", "Visible", "Enabled", "ReadOnly", "Checked",
        "AutoGenerateColumns", "Selected", "FontBold", "FontItalic", "FontUnderline",
        "AllowPaging", "AllowSorting", "ValidateEmptyText",
        "ShowHeader", "ShowSummary", "ShowMessageBox", "Wrap", "AppendDataBoundItems", "EnablePaging",
        "HeaderStyleFontBold", "RowStyleFontBold", "AlternatingRowStyleFontBold",
    };

    private static string NormalizeValue(string parameterName, string value)
        => BoolParameters.Contains(parameterName)
           && (value.Equals("true", StringComparison.OrdinalIgnoreCase)
               || value.Equals("false", StringComparison.OrdinalIgnoreCase))
            ? value.ToLowerInvariant()
            : value;

    private static string Truncate(string value)
    {
        var single = value.Replace("\r", " ").Replace("\n", " ").Trim();
        return single.Length <= 60 ? single : single[..60] + "…";
    }

    // DataBinder.Eval(Container.DataItem, "X") - the long-form idiom common in DNN-era
    // code - is left untouched: Container resolves to the row item, so the compat
    // DataBinder.Eval(object, string) works as written. Only the shorthand needs the
    // Container injection.
    //
    // "this.Eval(...)" IS the shorthand. The lookbehind used to reject any preceding dot,
    // which rejected it too - and YAF writes it that way throughout, so 203 of its build
    // errors were "no overload for Eval takes 1 argument". An explicit this is a style
    // choice, not a different call.
    [GeneratedRegex(@"(?<![\w.])(?:this\.)?Eval\s*\(")]
    private static partial Regex EvalCallRegex();

    // The trailing "." used to be part of the pattern, so only "Item.Something" was
    // rewritten and a BARE "Item" was left as an identifier that nothing declares.
    // ItemType="System.String" makes the item a string, and a string is used whole far more
    // often than it is dereferenced - YAF's OpenAuthProviders writes CommandArgument="<%#:
    // Item %>" three times in one tag. The "." moves into the replacement instead.
    [GeneratedRegex(@"(?<![\w.])(?:Item|BindItem)(?![\w])")]
    private static partial Regex TypedItemRegex();

    [GeneratedRegex(@"\bContainer\b|\bEval\s*\(|\bBind\s*\(")]
    private static partial Regex ContainerReferenceRegex();

    [GeneratedRegex(@"\bBind\s*\(")]
    private static partial Regex BindCallRegex();
}
