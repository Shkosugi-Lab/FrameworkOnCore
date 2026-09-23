using System.Text;

namespace WebForm2Blazor.Components;

/// <summary>System.Web.UI.HtmlTextWriterTag equivalent (the commonly used members).</summary>
public enum HtmlTextWriterTag
{
    Unknown, A, Acronym, Address, Area, B, Base, Basefont, Bdo, Bgsound, Big,
    Blockquote, Body, Br, Button, Caption, Center, Cite, Code, Col, Colgroup,
    Dd, Del, Dfn, Dir, Div, Dl, Dt, Em, Embed, Fieldset, Font, Form, Frame,
    Frameset, H1, H2, H3, H4, H5, H6, Head, Hr, Html, I, Iframe, Img, Input,
    Ins, Isindex, Kbd, Label, Legend, Li, Link, Map, Marquee, Menu, Meta, Nav,
    Nobr, Noframes, Noscript, Object, Ol, Option, P, Param, Pre, Q, Rt, Ruby,
    S, Samp, Script, Section, Select, Small, Span, Strike, Strong, Style, Sub,
    Sup, Table, Tbody, Td, Textarea, Tfoot, Th, Thead, Title, Tr, Tt, U, Ul,
    Var, Wbr, Xml,
}

/// <summary>System.Web.UI.HtmlTextWriterAttribute equivalent (the commonly used members).</summary>
public enum HtmlTextWriterAttribute
{
    Align, Alt, Border, Cellpadding, Cellspacing, Checked, Class, Cols, Colspan,
    Disabled, For, Height, Href, Id, Maxlength, Name, Onclick, Onchange, ReadOnly,
    Rel, Rows, Rowspan, Selected, Size, Src, Style, Tabindex, Target, Title, Type,
    Valign, Value, Width, Wrap,
}

/// <summary>System.Web.UI.HtmlTextWriterStyle equivalent (the commonly used members).</summary>
public enum HtmlTextWriterStyle
{
    BackgroundColor, BorderColor, BorderStyle, BorderWidth, Color, Display,
    FontFamily, FontSize, FontStyle, FontWeight, Height, Margin, Padding,
    TextAlign, TextDecoration, VerticalAlign, Visibility, Width,
    // The rest of System.Web.UI.HtmlTextWriterStyle. Appended rather than merged in, so the
    // members above keep their values; CssName turns each into its kebab-case property.
    BackgroundImage, BorderCollapse, ListStyleImage, ListStyleType, Cursor, Direction,
    Filter, FontVariant, Left, MarginBottom, MarginLeft, MarginRight, MarginTop, Overflow,
    OverflowX, OverflowY, PaddingBottom, PaddingLeft, PaddingRight, PaddingTop, Position,
    TextOverflow, Top, WhiteSpace, ZIndex,
}

/// <summary>
/// System.Web.UI.HtmlTextWriter equivalent, writing into a string buffer.
/// Lets legacy custom controls (Render(HtmlTextWriter) overrides) run unchanged;
/// LegacyRenderHost feeds the buffered markup into the Blazor render tree.
/// </summary>
public class HtmlTextWriter(TextWriter inner) : TextWriter
{
    public const char TagRightChar = '>';
    public const char TagLeftChar = '<';
    public const char SlashChar = '/';
    public const char SpaceChar = ' ';
    public const char EqualsChar = '=';
    public const char SemicolonChar = ';';
    public const char StyleEqualsChar = ':';
    public const char SingleQuoteChar = '\'';
    public const string EndTagLeftChars = "</";
    public const string SelfClosingTagEndWithSlash = "/>";
    public const string SelfClosingTagEnd = " />";

    /// <summary>
    /// Components met while rendering, when this writer is filling a render-based control
    /// that Blazor itself is rendering. Null otherwise - see <see cref="EmbedComponent"/>.
    /// </summary>
    internal List<Microsoft.AspNetCore.Components.IComponent> EmbeddedComponents { get; set; }

    /// <summary>Marker text for the n-th embedded component. Control characters cannot occur in rendered HTML.</summary>
    internal static string EmbeddedMarker(int index) => $"\u0001wfc{index}\u0001";

    /// <summary>
    /// Writes a marker in place of a component, when this writer can carry one (see
    /// <see cref="EmbeddedComponents"/>); otherwise writes nothing.
    /// </summary>
    internal void EmbedComponent(Microsoft.AspNetCore.Components.IComponent component)
    {
        if (EmbeddedComponents is null || component is null)
        {
            return;
        }
        Write(EmbeddedMarker(EmbeddedComponents.Count));
        EmbeddedComponents.Add(component);
    }
    public const string SelfClosingChars = " /";
    public const char DoubleQuoteChar = '"';
    public const string EqualsDoubleQuoteString = "=\"";

    private readonly List<KeyValuePair<string, string>> _pendingAttributes = [];
    private readonly List<KeyValuePair<string, string>> _pendingStyles = [];
    private readonly Stack<string> _openTags = new();

    /// <summary>
    /// WebForms HtmlTextWriter.InnerWriter equivalent. Settable like the original: a
    /// legacy Render override swaps the target writer to capture its own output.
    /// </summary>
    public TextWriter InnerWriter
    {
        get => inner;
        set => inner = value;
    }

    // System.Web.UI.HtmlTextWriter derives from TextWriter, and ported code relies on it:
    // a writer whose constructor takes a TextWriter is handed an HtmlTextWriter
    // (BlogEngine's RewriteFormHtmlTextWriter does exactly that), and `using` blocks
    // around a writer expect IDisposable. Deriving here reproduces both.
    public override Encoding Encoding => inner?.Encoding ?? Encoding.UTF8;

    private int _indent;
    private bool _tabsPending;

    /// <summary>
    /// System.Web.UI.HtmlTextWriter.Indent equivalent: how many tabs open each new line.
    ///
    /// This is real output, not a formatting preference - the original app's HTML contains
    /// those tabs, so a writer that accepted the property and ignored it would render
    /// something the parity gate is entitled to call a difference. mojoPortal's seven menu
    /// adapters drive it directly (writer.Indent++ / writer.Indent--, 154 sites).
    ///
    /// Negative values are clamped, as the original does: the adapters decrement on paths
    /// they did not always increment, and the original never wrote a negative number of
    /// tabs.
    /// </summary>
    public int Indent
    {
        get => _indent;
        set => _indent = value < 0 ? 0 : value;
    }

    /// <summary>
    /// Writes the pending tabs, if a WriteLine left any owing.
    ///
    /// The flag is cleared BEFORE writing, not after: the tabs go out through the same
    /// writer, and leaving it set would make a re-entrant write emit them a second time.
    /// That is also the order the original uses.
    /// </summary>
    private void OutputTabs()
    {
        if (!_tabsPending)
        {
            return;
        }
        _tabsPending = false;
        for (var i = 0; i < _indent; i++)
        {
            inner.Write('\t');
        }
    }

    public override void Write(string value)
    {
        OutputTabs();
        inner.Write(value);
    }

    public override void Write(char value)
    {
        OutputTabs();
        inner.Write(value);
    }

    public override void Write(object value)
    {
        OutputTabs();
        inner.Write(value);
    }

    public override void Write(string format, params object[] args)
    {
        OutputTabs();
        inner.Write(format, args);
    }

    // No OutputTabs: the original does not indent a line it is only ending, so a blank
    // line stays blank rather than becoming a line of tabs.
    public override void WriteLine()
    {
        inner.WriteLine();
        _tabsPending = true;
    }

    /// <summary>
    /// WebForms HtmlTextWriter.WriteLineNoTabs - a line written without the current indent.
    /// This writer does not indent, so it is WriteLine.
    /// </summary>
    public void WriteLineNoTabs(string value) => WriteLine(value);

    public override void WriteLine(string value)
    {
        OutputTabs();
        inner.WriteLine(value);
        _tabsPending = true;
    }

    public override void WriteLine(string format, params object[] args)
    {
        OutputTabs();
        inner.WriteLine(format, args);
        _tabsPending = true;
    }

    public override void WriteLine(char value)
    {
        OutputTabs();
        inner.WriteLine(value);
        _tabsPending = true;
    }

    public override void WriteLine(object value)
    {
        OutputTabs();
        inner.WriteLine(value);
        _tabsPending = true;
    }

    public void WriteBeginTag(string tagName)
    {
        OutputTabs();
        inner.Write('<' + tagName);
    }

    public void WriteFullBeginTag(string tagName)
    {
        OutputTabs();
        inner.Write('<' + tagName + '>');
    }

    public void WriteEndTag(string tagName)
    {
        OutputTabs();
        inner.Write("</" + tagName + '>');
    }

    // virtual: ported writers (URL-rewriting form writers etc.) override it
    public virtual void WriteAttribute(string name, string value, bool encode = false)
    {
        OutputTabs();
        inner.Write($" {name}=\"{(encode ? System.Net.WebUtility.HtmlEncode(value) : value)}\"");
    }

    /// <summary>
    /// WebForms HtmlTextWriter.WriteBreak: a line break element.
    ///
    /// "&lt;br /&gt;" - the XHTML form, which is what 4.8 writes, so the DOM matches.
    /// </summary>
    public void WriteBreak()
    {
        OutputTabs();
        inner.Write("<br />");
    }

    public void WriteEncodedText(string text)
    {
        OutputTabs();
        inner.Write(System.Net.WebUtility.HtmlEncode(text));
    }

    public void AddAttribute(HtmlTextWriterAttribute key, string value)
        => AddAttribute(key.ToString().ToLowerInvariant(), value);

    public void AddAttribute(string name, string value)
        => _pendingAttributes.Add(new KeyValuePair<string, string>(name, value));

    /// <summary>
    /// HtmlTextWriter.AddAttribute(..., bool fEncode). The flag is the whole point of the
    /// overload - the caller has a value it knows is not yet safe for an attribute - so it
    /// is honoured rather than accepted and ignored. mojoPortal's renderers call this 23
    /// times, every one of them on a URL or a caption that came out of the database.
    /// </summary>
    public void AddAttribute(HtmlTextWriterAttribute key, string value, bool fEncode)
        => AddAttribute(key.ToString().ToLowerInvariant(), value, fEncode);

    public void AddAttribute(string name, string value, bool fEncode)
        => AddAttribute(name, fEncode ? System.Net.WebUtility.HtmlEncode(value) : value);

    public void AddStyleAttribute(HtmlTextWriterStyle key, string value)
        => AddStyleAttribute(CssName(key), value);

    public void AddStyleAttribute(string name, string value)
        => _pendingStyles.Add(new KeyValuePair<string, string>(name, value));

    public virtual void RenderBeginTag(HtmlTextWriterTag tag) => RenderBeginTag(tag.ToString().ToLowerInvariant());

    /// <summary>
    /// virtual, as HtmlTextWriter.RenderBeginTag is. mojoPortal derives two writers from
    /// it (mojoHtmlTextWriter, mojoHtml32TextWriter) and overrides this one.
    /// </summary>
    public virtual void RenderBeginTag(string tagName)
    {
        OutputTabs();
        inner.Write('<' + tagName);
        foreach (var attribute in _pendingAttributes)
        {
            inner.Write($" {attribute.Key}=\"{attribute.Value}\"");
        }
        if (_pendingStyles.Count > 0)
        {
            var style = new StringBuilder();
            foreach (var declaration in _pendingStyles)
            {
                style.Append(declaration.Key).Append(':').Append(declaration.Value).Append(';');
            }
            inner.Write($" style=\"{style}\"");
        }
        _pendingAttributes.Clear();
        _pendingStyles.Clear();
        inner.Write('>');
        _openTags.Push(tagName);
    }

    public void RenderEndTag()
    {
        if (_openTags.Count > 0)
        {
            OutputTabs();
            inner.Write("</" + _openTags.Pop() + '>');
        }
    }

    public void BeginRender()
    {
    }

    public void EndRender()
    {
    }

    /// <summary>FontFamily -> font-family etc. (kebab-case CSS property names).</summary>
    internal static string CssName(HtmlTextWriterStyle key)
    {
        var name = key.ToString();
        var builder = new StringBuilder();
        for (var i = 0; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]) && i > 0)
            {
                builder.Append('-');
            }
            builder.Append(char.ToLowerInvariant(name[i]));
        }
        return builder.ToString();
    }
}

/// <summary>
/// Base class the converter substitutes for System.Web.UI.Control /
/// System.Web.UI.WebControls.WebControl on ported legacy custom controls - the ones that
/// render themselves through Render(HtmlTextWriter).
///
/// It derives from <see cref="WebFormsControlBase"/>, the base of the compat COMPONENTS.
/// It used to be a separate root, and the compatibility layer had two control hierarchies
/// that implemented WebForms' Control twice - 53 members declared in both. A control of one
/// family could not stand where the other was expected, which is not a distinction WebForms
/// ever had: n2cms returns "new DropDownList()" from a method declared to return ListControl,
/// and with the component DropDownList outside the ListControl family that line could not
/// compile, let alone work.
///
/// One hierarchy now, as in System.Web. What is left here is what a RENDER-BASED control
/// has and a Razor component does not: the Render(HtmlTextWriter) protocol and the members
/// ported controls override to take part in it.
///
/// A render-based control is rendered by Blazor through <see cref="BuildRenderTree"/>, which
/// runs that protocol into a writer. Where the control holds COMPONENTS among its children
/// (a DropDownList built in code and added to a render-based editor), those are put into
/// the output as real components rather than text, so they stay interactive.
/// </summary>
public abstract class LegacyWebControl : WebFormsControlBase
{
    /// <summary>
    /// WebForms WebControl.Font equivalent. The Font-* markup attributes land on the
    /// component parameters; this is the object form ported code assigns through.
    /// </summary>
    public virtual FontInfo Font { get; set; } = new();

    /// <summary>
    /// WebForms Control.ClearChildViewState / ClearChildState - discard the children's saved
    /// state before rebuilding them. A legacy control keeps no saved child state here (it
    /// re-renders from its fields), so there is nothing to discard.
    /// </summary>
    protected void ClearChildViewState()
    {
    }

    protected void ClearChildState()
    {
    }

    /// <summary>
    /// WebForms Control.Events: the delegate store a control uses to declare an event
    /// without a field per event ("add { this.Events.AddHandler(ClickKey, value); }").
    /// YAF's ThemeButton writes its Click that way. Without it the property is missing and
    /// the event does not compile at all.
    /// </summary>
    protected EventHandlerList Events { get; } = new();

    public HttpContext Context => HttpContext.Current;

    /// <summary>Always null: Blazor layouts are not addressable at run time.</summary>
    public object Master => null;

    // ---------------------------------------------------------------------------------
    // Postback and view-state extension points a ported custom control overrides. Declared,
    // not driven: there is no postback to raise them. An override that is never called
    // stays visible in the source; a control that silently skipped its own state handling
    // would not be.
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// WebForms IPostBackDataHandler.LoadPostData equivalent. Always false: there is no
    /// postback, so no control ever reports a changed value.
    /// </summary>
    public virtual bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        => false;

    /// <summary>WebForms IPostBackDataHandler.RaisePostDataChangedEvent equivalent.</summary>
    public virtual void RaisePostDataChangedEvent() => OnDataChanged(EventArgs.Empty);

    protected virtual void OnDataChanged(EventArgs e)
    {
    }

    /// <summary>
    /// WebForms IPostBackEventHandler.RaisePostBackEvent equivalent. Reaches OnClick,
    /// which is how WebForms got from a postback to a Button-derived control's handler.
    /// </summary>
    public virtual void RaisePostBackEvent(string eventArgument) => OnClick(EventArgs.Empty);

    /// <summary>WebForms WebControl.OnAttributesChanged equivalent.</summary>
    protected virtual void OnAttributesChanged()
    {
    }

    /// <summary>WebForms WebControl.RenderAttributes equivalent (pre-2.0 spelling).</summary>
    protected virtual void RenderAttributes(HtmlTextWriter writer) => AddAttributesToRender(writer);

    /// <summary>
    /// WebForms WebControl.TagKey equivalent. Rendering here goes through
    /// <see cref="TagName"/>; TagKey exists because ported controls override it to pick
    /// their element.
    /// </summary>
    protected virtual HtmlTextWriterTag TagKey => HtmlTextWriterTag.Span;

    /// <summary>
    /// WebForms control state (LoadControlState / SaveControlState). Here neither
    /// round-trips, since a Blazor circuit keeps the control itself alive on the server.
    /// Declared because ported composite controls override them - DNN's DnnFormEditor and
    /// DnnFormItemBase do.
    /// </summary>
    protected virtual void LoadControlState(object savedState)
    {
    }

    protected virtual object SaveControlState() => null;

    /// <summary>
    /// WebForms WebControl.EnableTheming. Themes were a Web.config + App_Themes mechanism the
    /// conversion does not carry, so this is state a control reads back and nothing applies.
    /// </summary>
    public virtual bool EnableTheming { get; set; } = true;

    /// <summary>WebForms Control.RaiseBubbleEvent equivalent.</summary>
    protected void RaiseBubbleEvent(object source, EventArgs args) => OnBubbleEvent(source, args);

    protected LegacyWebControl()
    {
    }

    /// <summary>
    /// WebForms WebControl(HtmlTextWriterTag) equivalent: the element the control renders
    /// as, chosen at construction.
    ///
    /// A ported control passes it through ("public DayNumberDiv(...) : base(Div)") and
    /// then never mentions the tag again, so the constructor has to exist for the subclass
    /// to compile - and it has to actually set the tag, or the control would render a span
    /// where the original rendered a div.
    /// </summary>
    protected LegacyWebControl(HtmlTextWriterTag tag) => _tagName = tag.ToString().ToLowerInvariant();

    private readonly string _tagName;

    /// <summary>The element the default rendering wraps (WebControl defaults to span).</summary>
    protected virtual string TagName => _tagName ?? "span";

    /// <summary>
    /// WebForms WebControl.ControlStyleCreated / Initialized. False: neither step exists
    /// here, and ported controls guard work with them - false skips work that would do
    /// nothing.
    /// </summary>
    protected bool ControlStyleCreated => false;

    /// <inheritdoc cref="ControlStyleCreated"/>
    protected bool Initialized => false;

    /// <summary>
    /// WebForms DataBoundControl.OnDataPropertyChanged equivalent. Inert: re-binding is what
    /// Blazor's render loop does when the property is assigned.
    /// </summary>
    protected virtual void OnDataPropertyChanged()
    {
    }

    /// <summary>
    /// True for a control drawn by Razor markup (DropDownList, TextBox) rather than by the
    /// Render(HtmlTextWriter) protocol. Stated by the control rather than inferred from which
    /// class declares BuildRenderTree: a render-based subclass can sit BELOW a Razor one
    /// (a ported FreeTextArea under TextBox) and the declaring class says nothing about it.
    /// </summary>
    protected virtual bool UsesRazorRendering
        => _usesRazorRendering ??= GetType().GetMethod(
                   nameof(BuildRenderTree),
                   System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
               ?.DeclaringType != typeof(LegacyWebControl);

    // The default asks the type: a Razor-generated class overrides BuildRenderTree. A class
    // that sits under a Razor one and renders itself says so explicitly (LegacyTextBox).
    private bool? _usesRazorRendering;

    private static readonly string[] RenderProtocolMethods =
    [
        nameof(Render), nameof(RenderContents), nameof(RenderChildren), nameof(RenderBeginTag),
        nameof(RenderEndTag), nameof(AddAttributesToRender), nameof(RenderControl),
    ];

    /// <summary>
    /// Whether a class below <paramref name="boundary"/> overrides any part of the render
    /// protocol. WebForms' own rule: a TextBox subclass that does not override Render IS
    /// rendered as a TextBox. Only one that takes over its rendering renders itself.
    /// </summary>
    protected bool OverridesRenderingBelow(Type boundary)
        => GetType()
            .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
                        | System.Reflection.BindingFlags.NonPublic)
            .Where(method => RenderProtocolMethods.Contains(method.Name))
            .Any(method => method.DeclaringType is { } declaring
                           && declaring != boundary
                           && boundary.IsAssignableFrom(declaring));

    public override void RenderControl(HtmlTextWriter writer)
    {
        if (!Visible)
        {
            return;
        }

        // A Razor-rendered control cannot be written as text; it takes part the way any
        // component does (see WebFormsControlBase.RenderControl).
        if (UsesRazorRendering)
        {
            base.RenderControl(writer);
            return;
        }

        RunLifecycle();
        Render(writer);
    }

    /// <summary>
    /// The standard WebControl protocol: begin tag + contents + end tag.
    /// LegacyRenderHost uses the begin/end halves separately to wrap Blazor-rendered
    /// child content for controls that carry children in markup.
    /// </summary>
    protected override void Render(HtmlTextWriter writer)
    {
        RenderBeginTag(writer);
        RenderContents(writer);
        RenderEndTag(writer);
    }

    public virtual void RenderBeginTag(HtmlTextWriter writer)
    {
        if (!string.IsNullOrEmpty(ID))
        {
            writer.AddAttribute("id", ClientID);
        }
        if (!string.IsNullOrEmpty(CssClass))
        {
            writer.AddAttribute("class", CssClass);
        }
        if (!string.IsNullOrEmpty(ToolTip))
        {
            writer.AddAttribute("title", ToolTip);
        }
        foreach (var pair in Attributes.Items)
        {
            writer.AddAttribute(pair.Key, pair.Value);
        }
        // Last, so a control's own attributes can override the ones above - the order
        // WebForms uses.
        AddAttributesToRender(writer);
        writer.RenderBeginTag(TagName);
    }

    public virtual void RenderEndTag(HtmlTextWriter writer) => writer.RenderEndTag();

    /// <summary>
    /// WebForms Control.RenderChildren equivalent. A container control overrides it to put
    /// markup around its children (n2's TreeNode, YAF's Form).
    /// </summary>
    protected override void RenderChildren(HtmlTextWriter writer)
    {
        foreach (var child in Controls)
        {
            child?.RenderControl(writer);
        }
    }

    /// <summary>
    /// WebForms WebControl.RenderContents: what goes between the begin and end tags,
    /// which by default is the control's children.
    ///
    /// It was empty once, so a control that builds its content in code rendered its own tag
    /// and nothing inside it. BlogEngine's WidgetZone is the shape: OnLoad adds a Literal
    /// per widget and Render writes "&lt;div class=widgetzone&gt;" + base.Render + "&lt;/div&gt;".
    /// </summary>
    protected override void RenderContents(HtmlTextWriter writer) => RenderChildren(writer);

    /// <summary>
    /// WebForms Control.Parent. Defaults to the naming container, which is the parent for
    /// every control the compat layer actually places, and is settable for the ones ported
    /// code builds by hand.
    /// </summary>
    public IWebFormsControl Parent
    {
        get => _parent ?? NamingContainer;
        set => _parent = value;
    }

    private IWebFormsControl _parent;

    /// <summary>
    /// Button / LinkButton / ImageButton raised this, and a ported control that derives
    /// from one of them overrides it - YAF.NET's CollapseButton toggles a panel here.
    /// Raised by <see cref="RaisePostBackEvent"/>, which is how WebForms reached it.
    /// </summary>
    protected virtual void OnClick(EventArgs e) => Click?.Invoke(this, e);

    /// <summary>Button-style click event, for code that subscribes rather than overrides.</summary>
    public event EventHandler Click;

    /// <summary>CompositeDataBoundControl overload (GridView-derived controls).</summary>
    protected virtual int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding) => 0;

    /// <summary>
    /// Init -> Load -> PreRender, once (see WebFormsControlBase.RunInitialLifecycle). The
    /// name LegacyRenderHost has always called it by.
    /// </summary>
    internal void RunLifecycle() => RunInitialLifecycle();

    /// <summary>
    /// How Blazor draws a render-based control: the Render protocol into a writer.
    ///
    /// Plain text when the output holds only markup, which is what it always was. When a
    /// COMPONENT sits among the children, the writer carries a marker in its place, and the
    /// markup is rebuilt as element frames with the real component at each marker - text
    /// alone cannot be split around a component, because a browser handed "&lt;div&gt;" on
    /// its own closes it on the spot.
    /// </summary>
    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        => BuildRenderTreeFromRender(builder);

    /// <summary>
    /// The render-protocol build itself, callable from a render-based subclass that sits
    /// under a Razor control and has to skip the Razor markup it would otherwise inherit.
    /// </summary>
    protected void BuildRenderTreeFromRender(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        var text = new System.IO.StringWriter();
        var writer = new HtmlTextWriter(text) { EmbeddedComponents = [] };
        // Render directly, not through RenderControl: RenderControl hands a Razor-flagged
        // control back to Blazor as an embedded component, and from here that would embed
        // this very control inside itself, forever.
        if (Visible)
        {
            RunLifecycle();
            Render(writer);
        }
        writer.Flush();

        var html = text.ToString();
        if (writer.EmbeddedComponents.Count == 0)
        {
            builder.AddMarkupContent(0, html);
            return;
        }

        var activator = RootServices?.GetService(typeof(PreparedComponentActivator)) as PreparedComponentActivator;
        MarkupFrames.Build(builder, html, writer.EmbeddedComponents, activator);
    }
}
/// <summary>
/// System.Web.UI.Control equivalent for code that declares variables / parameters of
/// type Control (the ported render pipeline works through LegacyWebControl).
/// </summary>
public class Control : LegacyWebControl
{
}

/// <summary>System.Web.UI.WebControls.WebControl equivalent as a usable type
/// (variable / parameter declarations; bases are rewritten to LegacyWebControl).</summary>
public class WebControl : LegacyWebControl
{
}

/// <summary>
/// System.Web.UI.HtmlControls.HtmlTableRow equivalent. Cells is its own list rather than a
/// view over Controls, so it is rendered explicitly - see <see cref="HtmlTable"/>.
/// </summary>
public class HtmlTableRow : LegacyWebControl
{
    protected override string TagName => "tr";

    public List<HtmlTableCell> Cells { get; } = [];

    protected override void RenderContents(HtmlTextWriter writer)
    {
        foreach (var cell in Cells)
        {
            cell?.RenderControl(writer);
        }
        base.RenderContents(writer);
    }
}

/// <summary>System.Web.UI.HtmlControls.HtmlTableCell equivalent (declaration surface).</summary>
public class HtmlTableCell : LegacyWebControl
{
    protected override string TagName => string.IsNullOrEmpty(CellTagName) ? "td" : CellTagName;

    public HtmlTableCell()
    {
    }

    /// <summary>WebForms allowed the tag name to be chosen at construction (td / th).</summary>
    public HtmlTableCell(string tagName) => CellTagName = tagName;

    /// <summary>Overrides <see cref="TagName"/> when the constructor supplied one.</summary>
    public string CellTagName { get; set; }

    public string InnerText { get; set; }

    public string InnerHtml { get; set; }

    public int ColSpan { get; set; }

    protected override void RenderContents(HtmlTextWriter writer)
    {
        if (InnerHtml is not null)
        {
            writer.Write(InnerHtml);
        }
        else if (InnerText is not null)
        {
            writer.WriteEncodedText(InnerText);
        }
    }
}

/// <summary>
/// System.Web.UI.WebControls.BaseValidator equivalent.
///
/// The root of BOTH kinds of validator: a ported one that renders itself (n2cms's
/// RequireEitherFieldValidator derives from it directly) and the compat layer's own Razor
/// validators, through ValidatorBase. Those used to be unrelated types, so n2cms's
/// "protected override BaseValidator CreateValidator() => new RangeValidator { ... }" could
/// not compile. The members are virtual so the Razor side can make them parameters.
/// </summary>
public class BaseValidator : LegacyWebControl, IValidator
{
    public virtual string ErrorMessage { get; set; }

    /// <summary>Static / Dynamic / None, as text - see <see cref="ValidatorDisplay"/>.</summary>
    public virtual string Display { get; set; } = ValidatorDisplay.Static;

    /// <summary>
    /// Accepted and inert: validation runs on the server, so there is no client script for
    /// this to enable. Ported validators read and set it while wiring themselves up.
    /// </summary>
    public virtual bool EnableClientScript { get; set; } = true;

    /// <summary>WebForms BaseValidator.GetControlValidationValue - the value of a named control.</summary>
    protected virtual string GetControlValidationValue(string name)
        => (FindControl(name) ?? Page?.FindControl(name)) is IValueControl control
            ? control.GetControlValue()
            : null;

    /// <summary>WebForms BaseValidator.GetControlRenderID - the client id of a named control.</summary>
    protected string GetControlRenderID(string name)
        => (FindControl(name) ?? Page?.FindControl(name))?.ClientID ?? name;

    /// <summary>
    /// WebForms BaseValidator.DetermineRenderUplevel - whether the browser can run client
    /// validation. There is none here, so never.
    /// </summary>
    protected virtual bool DetermineRenderUplevel() => false;

    /// <summary>WebForms BaseValidator.ControlPropertiesValid (the default accepts).</summary>
    protected virtual bool ControlPropertiesValid() => true;

    /// <summary>
    /// WebForms BaseValidator.CheckControlValidationProperty - throws when the named control
    /// cannot be validated. Here: when it cannot be found at all.
    /// </summary>
    protected void CheckControlValidationProperty(string name, string propertyName)
    {
        if ((FindControl(name) ?? Page?.FindControl(name)) is null)
        {
            throw new HttpException(
                $"検証対象のコントロール '{name}' が見つかりません({propertyName})。");
        }
    }

    public virtual string ControlToValidate { get; set; }

    public virtual string ValidationGroup { get; set; }

    public virtual bool IsValid { get; set; } = true;

    public virtual void Validate() => IsValid = EvaluateIsValid();

    protected virtual bool EvaluateIsValid() => true;
}

/// <summary>Substitute base for classes deriving System.Web.UI.WebControls.Panel (renders a div).</summary>
public abstract class LegacyPanel : LegacyWebControl
{
    protected override string TagName => "div";
}

/// <summary>Substitute base for classes deriving System.Web.UI.WebControls.Label.</summary>
public abstract class LegacyLabel : LegacyWebControl
{
    public string Text { get; set; }

    protected override void RenderContents(HtmlTextWriter writer) => writer.Write(Text ?? string.Empty);
}

/// <summary>Substitute base for classes deriving System.Web.UI.WebControls.Literal (no wrapper tag).</summary>
public abstract class LegacyLiteral : LegacyWebControl
{
    public string Text { get; set; }

    protected override void Render(HtmlTextWriter writer) => writer.Write(Text ?? string.Empty);
}

/// <summary>
/// Substitute base for classes deriving Button / LinkButton / ImageButton.
///
/// Those were mapped to LegacyWebControl, which carries the lifecycle and the render
/// virtuals and nothing else - so a ported button base lost Text, CommandName and the rest,
/// and every line of the subclass that touched them failed. Same arrangement as
/// LegacyListControl, and for the same reason: the base has to carry what the control IS,
/// not only what every control has.
///
/// The click surface is declared and never raised. A postback is what fired it, and a
/// render-hosted legacy control has no event wiring on the Blazor side; the residual for
/// the control records that separately.
/// </summary>
public abstract class LegacyButton : LegacyWebControl, IButtonControl
{
    public string Text { get; set; }

    public string CommandName { get; set; }

    public string CommandArgument { get; set; }

    public string PostBackUrl { get; set; }

    public string OnClientClick { get; set; }

    public bool CausesValidation { get; set; } = true;

    public string ValidationGroup { get; set; }

    public string ImageUrl { get; set; }

    public string AlternateText { get; set; }

    /// <summary>WebForms Button.Command, beside Click on LegacyWebControl.</summary>
    public event CommandEventHandler Command;

    protected virtual void OnCommand(CommandEventArgs e) => Command?.Invoke(this, e);

    protected override string TagName => "a";

    protected override void RenderContents(HtmlTextWriter writer)
        => writer.Write(Text ?? string.Empty);
}

/// <summary>
/// Substitute base for classes deriving TextBox.
///
/// It derives from the compat TextBox, so a ported subclass IS a TextBox: n2cms's
/// EditableFreeTextAreaAttribute.CreateEditor is declared to return TextBox and returns its
/// own FreeTextArea, which could not compile while this was a separate render-based root.
///
/// How it renders follows WebForms' rule. A subclass that does not override any part of the
/// render protocol renders AS A TEXTBOX - FreeTextArea only sets CssClass and registers its
/// editor script in OnPreRender, and on 4.8 it came out as the ordinary textarea. One that
/// takes over its rendering (mojoPortal's CodeEditor) renders itself through Render, with
/// the contents below as the default.
/// </summary>
public abstract class LegacyTextBox : TextBox
{
    protected override bool UsesRazorRendering => !OverridesRenderingBelow(typeof(LegacyTextBox));

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        if (UsesRazorRendering)
        {
            base.BuildRenderTree(builder);
            return;
        }
        BuildRenderTreeFromRender(builder);
    }

    protected override string TagName => "input";

    protected override void RenderContents(HtmlTextWriter writer)
        => writer.Write(Text ?? string.Empty);
}
/// <summary>
/// Substitute base for classes deriving FileUpload.
///
/// Same rule as <see cref="LegacyTextBox"/>: the base has to carry what the control is.
/// PostedFile is null and HasFile false, because a Blazor page uploads over the circuit
/// rather than as a multipart post - the same answer Request.Files gives, and for the
/// same reason.
/// </summary>
public abstract class LegacyFileUpload : LegacyWebControl
{
    public HttpPostedFile PostedFile => null;

    public bool HasFile => false;

    public string FileName => string.Empty;

    public byte[] FileBytes => [];

    public Stream FileContent => Stream.Null;

    protected override string TagName => "input";
}

/// <summary>Substitute base for classes deriving System.Web.UI.WebControls.HyperLink.</summary>
public abstract class LegacyHyperLink : LegacyWebControl
{
    public string Text { get; set; }
    public string NavigateUrl { get; set; }
    public string Target { get; set; }
    public string ImageUrl { get; set; }

    protected override string TagName => "a";

    public override void RenderBeginTag(HtmlTextWriter writer)
    {
        if (!string.IsNullOrEmpty(NavigateUrl))
        {
            writer.AddAttribute("href", UrlMapper.ResolveUrl(NavigateUrl));
        }
        if (!string.IsNullOrEmpty(Target))
        {
            writer.AddAttribute("target", Target);
        }
        base.RenderBeginTag(writer);
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
        if (!string.IsNullOrEmpty(ImageUrl))
        {
            writer.Write($"<img src=\"{UrlMapper.ResolveUrl(ImageUrl)}\"" +
                (string.IsNullOrEmpty(Text) ? " />" : $" alt=\"{Text}\" />"));
        }
        else
        {
            writer.Write(Text ?? string.Empty);
        }
    }
}

/// <summary>
/// System.Web.UI.WebControls.PlaceHolder equivalent: a container that renders ITS CHILDREN
/// AND NO TAG OF ITS OWN.
///
/// PlaceHolder derives from Control, not WebControl, and Control.Render just renders the
/// children. Mapping it onto <see cref="LegacyWebControl"/> gave it the WebControl
/// protocol instead - begin tag + contents + end tag - so every ported PlaceHolder grew a
/// &lt;span id="..."&gt; wrapper the original never had.
///
/// BlogEngine's WidgetZone is "class WidgetZone : PlaceHolder" and writes its own div:
///
///     writer.Write("&lt;div id=\"widgetzone_{0}\" class=\"widgetzone\"&gt;", zoneName);
///     base.Render(writer);
///     writer.Write("&lt;/div&gt;");
///
/// That base call is the whole issue. In the original it emits the widgets; here it was
/// emitting a span around them, which the comparison against the original caught as an
/// element the legacy app never rendered.
/// </summary>
public class LegacyPlaceHolder : LegacyWebControl
{
    protected override void Render(HtmlTextWriter writer) => RenderChildren(writer);
}
