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
    private static string CssName(HtmlTextWriterStyle key)
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
/// System.Web.UI.WebControls.WebControl on ported legacy custom controls
/// (render-based controls run under LegacyRenderHost).
/// </summary>
public abstract class LegacyWebControl : IWebFormsControl, IDisposable
{
    /// <inheritdoc cref="WebFormsControlBase.Dispose"/>
    public virtual void Dispose() => GC.SuppressFinalize(this);

    public virtual string ID { get; set; }

    public virtual string ClientID => ID;

    /// <summary>
    /// WebForms Control.UniqueID equivalent. Virtual because ported controls override it -
    /// mojoPortal's AdRotator returns a stable id so its client script can find itself.
    /// </summary>
    public virtual string UniqueID => ID;

    /// <summary>
    /// WebForms WebControl.Font equivalent. The Font-* markup attributes land on the
    /// component parameters; this is the object form ported code assigns through.
    /// </summary>
    public virtual FontInfo Font { get; set; } = new();

    public virtual string CssClass { get; set; }

    /// <summary>
    /// WebForms WebControl.ApplyStyle / MergeStyle.
    ///
    /// Declared on the CLASS as well as on <see cref="IWebFormsControl"/>, because a
    /// default interface member is not callable through the class - the same lesson the
    /// lifecycle methods already recorded here. Ported code holds a concrete control
    /// (mojoPortal's breadcrumb holds a SiteMapNodeItem) and calls it directly.
    /// </summary>
    public virtual void ApplyStyle(Style style)
    {
        if (!string.IsNullOrEmpty(style?.CssClass))
        {
            CssClass = style.CssClass;
        }
    }

    /// <inheritdoc cref="ApplyStyle"/>
    public virtual void MergeStyle(Style style)
    {
        if (string.IsNullOrEmpty(CssClass) && !string.IsNullOrEmpty(style?.CssClass))
        {
            CssClass = style.CssClass;
        }
    }

    public virtual bool Visible { get; set; } = true;

    public virtual bool Enabled { get; set; } = true;

    public virtual string ToolTip { get; set; }

    /// <summary>WebForms WebControl.TabIndex equivalent. Rendered by the control's own Render override.</summary>
    public virtual short TabIndex { get; set; }

    protected bool DesignMode => false;

    /// <summary>WebForms Control.Attributes equivalent.</summary>
    public AttributeCollection Attributes { get; } = new(() => { });

    /// <summary>WebForms Control.ViewState equivalent (per-instance; no persistence).</summary>
    protected StateBag ViewState { get; } = new();

    /// <summary>
    /// WebForms Control.Events: the delegate store a control uses to declare an event
    /// without a field per event ("add { this.Events.AddHandler(ClickKey, value); }").
    /// YAF's ThemeButton writes its Click that way. Without it the property is missing and
    /// the event does not compile at all.
    /// </summary>
    protected EventHandlerList Events { get; } = new();

    /// <summary>WebForms Control.Controls equivalent (children added programmatically).</summary>
    public virtual ControlCollection Controls { get; } = [];

    /// <summary>
    /// WebForms Control.Page / .Context / .Master equivalents. A legacy control reaches
    /// its host through these; Master is always null (Blazor layouts are not addressable
    /// at runtime).
    ///
    /// Page used to be a hard null, and a ported control that touches it - which is
    /// ordinary WebForms code - threw a NullReferenceException the moment it rendered.
    /// BlogEngine's PostCalendar calls Page.ClientScript in OnLoad and Page.IsPostBack in
    /// OnPreRender and Render, so the whole control came out as "[render error]". The
    /// hosting LegacyRenderHost knows the page and sets this before the lifecycle runs.
    /// It is still null when nothing hosts the control (a unit test, a control created in
    /// code and never placed), which is the honest answer there.
    /// </summary>
    public Page Page { get; internal set; }

    public HttpContext Context => HttpContext.Current;

    public object Master => null;

    /// <summary>WebForms Control.ResolveUrl equivalent.</summary>
    public string ResolveUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    public string ResolveClientUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    /// <summary>
    /// WebForms Control.FindControl equivalent (searches the Controls collection).
    ///
    /// IWebFormsControl, like every other FindControl here. It used to return Control, and
    /// a ported override - whose "Control" return type the converter rewrites to the
    /// interface, because a child can be either a component or a plain control - could not
    /// match it (CS0508). Returning the interface also stops the search silently dropping
    /// a child that is a Blazor component rather than a Control.
    /// </summary>
    public virtual IWebFormsControl FindControl(string id)
        => Controls.FirstOrDefault(child => string.Equals(child.ID, id, StringComparison.Ordinal));

    // ---------------------------------------------------------------------------------
    // Postback and view-state extension points.
    //
    // The lifecycle hooks (OnInit / OnLoad / OnPreRender / CreateChildControls) are
    // declared further down; these are the rest of what a ported custom control overrides.
    // Without something to override, none of those files compile, and the resulting CS0115
    // storm points at the control's own source rather than at the missing base.
    //
    // Declared, not driven: nothing here raises them, because these controls render through
    // LegacyRenderHost rather than taking part in the Blazor lifecycle. An override that is
    // never called stays visible in the source; a control that silently skipped its own
    // state handling would not be.
    // ---------------------------------------------------------------------------------

    /// <summary>WebForms Control.ChildControlsCreated equivalent.</summary>
    protected bool ChildControlsCreated { get; set; }

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

    /// <summary>
    /// WebForms WebControl.AddAttributesToRender / RenderAttributes equivalents. Ported
    /// controls override these to put their own attributes on the element, and without
    /// them every such override is CS0115 against a base that renders but offers no hook
    /// to add attributes.
    ///
    /// RenderBeginTag calls AddAttributesToRender, so an override reaches the output the
    /// same way it did on 4.8.
    /// </summary>
    protected virtual void AddAttributesToRender(HtmlTextWriter writer)
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
    /// WebForms Control.LoadViewState / SaveViewState equivalents. ViewState here is a
    /// per-instance bag with no round trip, so a saved state is never handed back.
    /// </summary>
    protected virtual void LoadViewState(object savedState)
    {
    }

    protected virtual object SaveViewState() => null;

    /// <summary>
    /// WebForms control state (LoadControlState / SaveControlState). Separate from view
    /// state in the original because a control could not opt out of it; here neither
    /// round-trips, since a Blazor circuit keeps the component itself alive on the server.
    /// Declared because ported composite controls override them - DNN's DnnFormEditor and
    /// DnnFormItemBase do.
    /// </summary>
    protected virtual void LoadControlState(object savedState)
    {
    }

    protected virtual object SaveControlState() => null;

    /// <summary>
    /// WebForms Control.EnableViewState / ViewStateMode equivalents. Nothing is serialised
    /// here (see above), so the value is recorded and read back. Virtual because ported
    /// controls override it to force it off.
    /// </summary>
    public virtual bool EnableViewState { get; set; } = true;

    /// <inheritdoc cref="EnableViewState"/>
    public virtual ViewStateMode ViewStateMode { get; set; } = ViewStateMode.Inherit;

    /// <summary>
    /// WebForms Control.SkinID / EnableTheming equivalents.
    ///
    /// Themes and skins were a Web.config + App_Themes mechanism that the conversion does
    /// not carry, so both are state a control reads back and nothing applies. Declaring
    /// them is what matters: markup and code-behind set them on every themed control, and
    /// the residual for the theme records what was not carried.
    /// </summary>
    /// <summary>
    /// WebForms WebControl.AccessKey. On the interface as well, so code that takes a
    /// control of either family can set it.
    /// </summary>
    public virtual string AccessKey { get; set; } = string.Empty;

    public virtual string SkinID { get; set; } = string.Empty;

    /// <inheritdoc cref="SkinID"/>
    public virtual bool EnableTheming { get; set; } = true;

    /// <summary>
    /// WebForms WebControl.Width / Height equivalents. The compat COMPONENTS render these
    /// as CSS; a legacy control renders itself, so here they are values its own Render
    /// reads. Virtual because ported controls override them to compute a size.
    /// </summary>
    public virtual Unit Width { get; set; }

    /// <inheritdoc cref="Width"/>
    public virtual Unit Height { get; set; }

    /// <summary>
    /// WebForms IStateManager.IsTrackingViewState equivalent. Always false: there is no
    /// view state to start tracking here, and ported code guards its TrackViewState calls
    /// with it ("if (IsTrackingViewState) ((IStateManager)style).TrackViewState();"), so
    /// false is the answer that skips work that would do nothing.
    /// </summary>
    protected bool IsTrackingViewState => false;

    protected virtual void TrackViewState()
    {
    }

    /// <summary>WebForms Control.OnBubbleEvent equivalent.</summary>
    protected virtual bool OnBubbleEvent(object source, EventArgs args) => false;

    /// <summary>WebForms Control.RaiseBubbleEvent equivalent.</summary>
    protected void RaiseBubbleEvent(object source, EventArgs args) => OnBubbleEvent(source, args);

    /// <summary>The element the default rendering wraps (WebControl defaults to span).</summary>
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

    protected virtual string TagName => _tagName ?? "span";

    /// <summary>
    /// WebForms WebControl.ControlStyleCreated / Initialized.
    ///
    /// False. Both answer "has this been through the step that would have created it",
    /// and neither step exists here: the compat layer has no lazily-created style object
    /// and no Init phase that flips a flag. Ported controls guard work with them
    /// ("if (ControlStyleCreated) ..."), and false is the branch that skips work which
    /// would do nothing.
    /// </summary>
    protected bool ControlStyleCreated => false;

    /// <inheritdoc cref="ControlStyleCreated"/>
    protected bool Initialized => false;

    /// <summary>
    /// WebForms DataBoundControl.OnDataPropertyChanged equivalent: "a property that
    /// decides what I show has changed, re-bind".
    ///
    /// Inert. Re-binding is what Blazor's render loop does when the property is assigned,
    /// so the notification has nowhere to go that it has not already been.
    /// </summary>
    protected virtual void OnDataPropertyChanged()
    {
    }

    public virtual void RenderControl(HtmlTextWriter writer)
    {
        if (Visible)
        {
            Render(writer);
        }
    }

    /// <summary>
    /// The standard WebControl protocol: begin tag + contents + end tag.
    /// LegacyRenderHost uses the begin/end halves separately to wrap Blazor-rendered
    /// child content for controls that carry children in markup.
    /// </summary>
    protected virtual void Render(HtmlTextWriter writer)
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
    protected virtual void RenderChildren(HtmlTextWriter writer)
    {
        foreach (var child in Controls)
        {
            child?.RenderControl(writer);
        }
    }

    protected virtual void RenderContents(HtmlTextWriter writer)
    {
    }

    // WebForms Control lifecycle virtuals: ported controls override these to build
    // state before Render. LegacyRenderHost drives them via RunLifecycle.
    // The lifecycle EVENTS beside the virtuals. WebForms exposes both, and ported code
    // uses whichever fits: a control overrides OnLoad, while the page holding it writes
    // "themeButton.Load += ...". Raised from the virtuals so a subscriber and an override
    // see the same moment - declaring them inert would have been the easy half and the
    // wrong half, because the handler is where the control gets its text.
    public event EventHandler Init;

    /// <inheritdoc cref="Init"/>
    public event EventHandler Load;

    /// <inheritdoc cref="Init"/>
    public event EventHandler PreRender;

    protected virtual void OnInit(EventArgs e) => Init?.Invoke(this, e);

    protected virtual void OnLoad(EventArgs e) => Load?.Invoke(this, e);

    protected virtual void OnPreRender(EventArgs e) => PreRender?.Invoke(this, e);

    protected virtual void OnUnload(EventArgs e)
    {
    }

    // Declared on the CLASS, not only on IWebFormsControl. A default interface member is
    // reachable through the interface and not through a class that implements it, so
    // putting these on the interface alone left every ported control that derives from
    // this base unable to call them - YAF's ThemeButton could not call Focus(), which is
    // the exact thing the interface default was added for.

    /// <summary>
    /// WebForms Control.Focus(). Focus is a client concern in Blazor
    /// (ElementReference.FocusAsync), so a ported call is accepted and does nothing.
    /// </summary>
    public void Focus()
    {
    }

    /// <summary>WebForms Control.HasControls(): whether anything was added to Controls.</summary>
    public bool HasControls() => Controls.Count > 0;

    /// <summary>
    /// WebForms Control.Parent. Same reason as Focus() above: it was an interface default
    /// and therefore invisible from the class. Defaults to the naming container, which is
    /// the parent for every control the compat layer actually places, and is settable for
    /// the ones ported code builds by hand.
    /// </summary>
    public IWebFormsControl Parent
    {
        get => _parent ?? NamingContainer;
        set => _parent = value;
    }

    private IWebFormsControl _parent;

    /// <summary>
    /// WebForms Control.Site - the designer's hook. There is no designer, and code reads
    /// it to ask "am I in the designer?", where null means no.
    /// </summary>
    public ISite Site => null;

    /// <summary>
    /// WebForms Control.Unload. Nothing raises it here (a Blazor component is disposed),
    /// so a subscription is accepted and never fires.
    /// </summary>
    public event EventHandler Unload
    {
        add { }
        remove { }
    }

    protected virtual void OnDataBinding(EventArgs e) => DataBinding?.Invoke(this, e);

    /// <summary>
    /// WebForms Control.DataBinding. A template subscribes to it and fills the control in
    /// the handler - that is how a WebForms column gets a value into a cell, so it has to
    /// be raised and not just declared.
    /// </summary>
    public event EventHandler DataBinding;

    /// <summary>
    /// WebForms Control.NamingContainer: the control that owns this one's ID space. A
    /// template's binding handler reaches the row's data through it
    /// ("(DataGridItem)sender.NamingContainer").
    /// </summary>
    public IWebFormsControl NamingContainer { get; set; }

    /// <summary>
    /// Button / LinkButton / ImageButton raised this, and a ported control that derives
    /// from one of them overrides it - YAF.NET's CollapseButton toggles a panel here.
    /// Those bases are Blazor components in the compat layer, which a plain ported class
    /// cannot derive from, so it lands on LegacyWebControl and the override needs
    /// something to bind to.
    ///
    /// Raised by <see cref="RaisePostBackEvent"/>, which is how WebForms reached it.
    /// </summary>
    protected virtual void OnClick(EventArgs e) => Click?.Invoke(this, e);

    /// <summary>Button-style click event, for code that subscribes rather than overrides.</summary>
    public event EventHandler Click;

    protected virtual void CreateChildControls()
    {
    }

    /// <summary>CompositeDataBoundControl overload (GridView-derived controls).</summary>
    protected virtual int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding) => 0;

    protected void EnsureChildControls() => CreateChildControls();

    public virtual void DataBind() => OnDataBinding(EventArgs.Empty);

    /// <summary>Runs Init -> Load -> PreRender before rendering (WebForms order).</summary>
    internal void RunLifecycle()
    {
        OnInit(EventArgs.Empty);
        OnLoad(EventArgs.Empty);
        OnPreRender(EventArgs.Empty);
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

/// <summary>System.Web.UI.HtmlControls.HtmlTableRow equivalent (declaration surface).</summary>
public class HtmlTableRow : LegacyWebControl
{
    protected override string TagName => "tr";

    public List<HtmlTableCell> Cells { get; } = [];
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

/// <summary>System.Web.UI.WebControls.BaseValidator equivalent (declaration surface).</summary>
public class BaseValidator : LegacyWebControl
{
    public string ErrorMessage { get; set; }

    public string ControlToValidate { get; set; }

    public string ValidationGroup { get; set; }

    public bool IsValid { get; set; } = true;

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
/// The third time the same shape has turned up: LegacyListControl had to carry Items,
/// LegacyButton had to carry Text, and a TextBox base has to carry Text too. Mapping all
/// three to LegacyWebControl gave them the lifecycle and the render virtuals and nothing
/// that says what the control IS - so mojoPortal's CodeEditor (a TextBox that renders a
/// syntax-highlighting editor) and its jDatePicker lost the property they exist to set.
/// </summary>
public abstract class LegacyTextBox : LegacyWebControl
{
    /// <summary>
    /// virtual, as TextBox.Text is. A ported editor overrides it to read and write its own
    /// backing store - mojoPortal's CKEditorControl does exactly that - and a non-virtual
    /// property here is CS0506, which fails the DECLARATION pass and takes the whole
    /// measurement down with it.
    /// </summary>
    public virtual string Text { get; set; } = string.Empty;

    public virtual TextBoxMode TextMode { get; set; } = TextBoxMode.SingleLine;

    public int Columns { get; set; }

    public int Rows { get; set; }

    public int MaxLength { get; set; }

    public bool ReadOnly { get; set; }

    public bool AutoPostBack { get; set; }

    public bool CausesValidation { get; set; }

    public string ValidationGroup { get; set; } = string.Empty;

    public bool Wrap { get; set; } = true;

    /// <summary>
    /// WebForms TextBox.TextChanged. Declared and never raised: a postback is what fired
    /// it, and a render-hosted legacy control has no Blazor-side event wiring - the
    /// control's own residual records that.
    /// </summary>
    public event EventHandler TextChanged;

    protected virtual void OnTextChanged(EventArgs e) => TextChanged?.Invoke(this, e);

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
