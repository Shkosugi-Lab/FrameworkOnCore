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

    public override void Write(string value) => inner.Write(value);
    public override void Write(char value) => inner.Write(value);
    public override void Write(object value) => inner.Write(value);
    public override void Write(string format, params object[] args) => inner.Write(format, args);
    public override void WriteLine() => inner.WriteLine();
    public override void WriteLine(string value) => inner.WriteLine(value);
    public override void WriteLine(string format, params object[] args) => inner.WriteLine(format, args);
    public override void WriteLine(char value) => inner.WriteLine(value);
    public override void WriteLine(object value) => inner.WriteLine(value);

    public void WriteBeginTag(string tagName) => inner.Write('<' + tagName);
    public void WriteFullBeginTag(string tagName) => inner.Write('<' + tagName + '>');
    public void WriteEndTag(string tagName) => inner.Write("</" + tagName + '>');

    // virtual: ported writers (URL-rewriting form writers etc.) override it
    public virtual void WriteAttribute(string name, string value, bool encode = false)
        => inner.Write($" {name}=\"{(encode ? System.Net.WebUtility.HtmlEncode(value) : value)}\"");

    public void WriteEncodedText(string text) => inner.Write(System.Net.WebUtility.HtmlEncode(text));

    public void AddAttribute(HtmlTextWriterAttribute key, string value)
        => AddAttribute(key.ToString().ToLowerInvariant(), value);

    public void AddAttribute(string name, string value)
        => _pendingAttributes.Add(new KeyValuePair<string, string>(name, value));

    public void AddStyleAttribute(HtmlTextWriterStyle key, string value)
        => AddStyleAttribute(CssName(key), value);

    public void AddStyleAttribute(string name, string value)
        => _pendingStyles.Add(new KeyValuePair<string, string>(name, value));

    public void RenderBeginTag(HtmlTextWriterTag tag) => RenderBeginTag(tag.ToString().ToLowerInvariant());

    public void RenderBeginTag(string tagName)
    {
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

    /// <summary>WebForms Control.Controls equivalent (children added programmatically).</summary>
    public virtual ControlCollection Controls { get; } = [];

    /// <summary>
    /// WebForms Control.Page / .Context / .Master equivalents. A legacy control reaches
    /// its host through these; Page is null unless one is in scope, and Master is always
    /// null (Blazor layouts are not addressable at runtime).
    /// </summary>
    public Page Page => null;

    public HttpContext Context => HttpContext.Current;

    public object Master => null;

    /// <summary>WebForms Control.ResolveUrl equivalent.</summary>
    public string ResolveUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    public string ResolveClientUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    /// <summary>WebForms Control.FindControl equivalent (searches the Controls collection).</summary>
    public virtual Control FindControl(string id)
        => Controls.FirstOrDefault(child => string.Equals(child.ID, id, StringComparison.Ordinal)) as Control;

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

    /// <summary>WebForms IPostBackEventHandler.RaisePostBackEvent equivalent.</summary>
    public virtual void RaisePostBackEvent(string eventArgument)
    {
    }

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

    protected virtual void TrackViewState()
    {
    }

    /// <summary>WebForms Control.OnBubbleEvent equivalent.</summary>
    protected virtual bool OnBubbleEvent(object source, EventArgs args) => false;

    /// <summary>WebForms Control.RaiseBubbleEvent equivalent.</summary>
    protected void RaiseBubbleEvent(object source, EventArgs args) => OnBubbleEvent(source, args);

    /// <summary>The element the default rendering wraps (WebControl defaults to span).</summary>
    protected virtual string TagName => "span";

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

    protected virtual void RenderContents(HtmlTextWriter writer)
    {
    }

    // WebForms Control lifecycle virtuals: ported controls override these to build
    // state before Render. LegacyRenderHost drives them via RunLifecycle.
    protected virtual void OnInit(EventArgs e)
    {
    }

    protected virtual void OnLoad(EventArgs e)
    {
    }

    protected virtual void OnPreRender(EventArgs e)
    {
    }

    protected virtual void OnUnload(EventArgs e)
    {
    }

    protected virtual void OnDataBinding(EventArgs e)
    {
    }

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
