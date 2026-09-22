using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace WebForm2Blazor.Components;

/// <summary>
/// System.Web.UI.HtmlControls equivalent (HtmlGenericControl / HtmlTableRow / ...).
/// The converter maps plain HTML elements carrying runat="server" (div, tr, a, ...)
/// onto this component so the tag itself survives and code-behind keeps working
/// (Visible toggling, InnerHtml, Attributes - the classic show/hide idiom).
/// form / head / body / html stay structural and are handled by the layout instead.
/// </summary>
public class HtmlGenericControl : WebFormsControlBase
{
    private string _innerHtml;
    private string _innerText;
    private string _href;
    private string _src;
    private string _alt;

    /// <summary>The original element name (div, tr, a, ...).</summary>
    // NOTE: a Blazor component must have exactly one applicable constructor - the
    // activator throws at render time otherwise. The WebForms form
    // (new HtmlGenericControl("div")) therefore cannot be offered here; dynamically
    // created controls are manual-migration territory anyway.

    [Parameter] public string TagName { get; set; } = "div";

    public HtmlGenericControl()
    {
    }

    /// <summary>
    /// WebForms HtmlGenericControl(string tag). Ported controls derive from it and pick
    /// their element in the constructor (n2cms's UrlSelector: ": base("div")").
    /// </summary>
    public HtmlGenericControl(string tag) => TagName = string.IsNullOrEmpty(tag) ? "span" : tag;

    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>WebForms InnerHtml equivalent; assigning replaces the child content (unencoded).</summary>
    public string InnerHtml
    {
        get => _innerHtml;
        set => SetAndRefresh(ref _innerHtml, value);
    }

    /// <summary>WebForms InnerText equivalent; assigning replaces the child content (encoded).</summary>
    public string InnerText
    {
        get => _innerText;
        set => SetAndRefresh(ref _innerText, value);
    }

    // The typed HtmlControls (HtmlAnchor / HtmlImage / HtmlInputImage) expose these as
    // first-class properties, and the classic idiom sets them from code-behind
    // (lnk.HRef = ResolveUrl(...)). They render as the matching HTML attribute, and
    // rendering is skipped when unset so a <div runat="server"> gains nothing.

    /// <summary>WebForms HtmlAnchor.HRef equivalent; rendered as href.</summary>
    [Parameter] public string HRef { get => _href; set => SetAndRefresh(ref _href, value); }

    /// <summary>WebForms HtmlImage.Src equivalent; rendered as src.</summary>
    [Parameter] public string Src { get => _src; set => SetAndRefresh(ref _src, value); }

    /// <summary>WebForms HtmlImage.Alt equivalent; rendered as alt.</summary>
    [Parameter] public string Alt { get => _alt; set => SetAndRefresh(ref _alt, value); }

    /// <summary>
    /// WebForms onserverclick equivalent (HtmlAnchor / HtmlButton). Wired only when a
    /// handler is attached, so a plain server-side div keeps rendering without an
    /// onclick attribute.
    /// </summary>
    [Parameter] public EventCallback<EventArgs> OnServerClick { get; set; }

    /// <summary>CLR event form of <see cref="OnServerClick"/>, for code-behind that subscribes in Page_Init.</summary>
    public event EventHandler ServerClick
    {
        add => _serverClick += value;
        remove => _serverClick -= value;
    }

    private EventHandler _serverClick;

    private bool HasClickHandler => OnServerClick.HasDelegate || _serverClick is not null;

    private async Task RaiseServerClickAsync()
    {
        _serverClick?.Invoke(this, EventArgs.Empty);
        if (OnServerClick.HasDelegate)
        {
            await OnServerClick.InvokeAsync(EventArgs.Empty);
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
        {
            return;
        }

        builder.OpenElement(0, TagName);
        builder.AddMultipleAttributes(1, ExtraAttributes!);
        var style = ComputedStyle;
        if (!string.IsNullOrEmpty(style))
        {
            builder.AddAttribute(2, "style", style);
        }
        if (!string.IsNullOrEmpty(ID))
        {
            builder.AddAttribute(3, "id", ClientID);
        }
        if (!string.IsNullOrEmpty(CssClass))
        {
            builder.AddAttribute(4, "class", CssClass);
        }
        if (!string.IsNullOrEmpty(_href))
        {
            builder.AddAttribute(8, "href", UrlMapper.ResolveUrl(_href));
        }
        if (!string.IsNullOrEmpty(_src))
        {
            builder.AddAttribute(9, "src", UrlMapper.ResolveUrl(_src));
        }
        if (_alt is not null)
        {
            builder.AddAttribute(10, "alt", _alt);
        }
        if (HasClickHandler)
        {
            builder.AddAttribute(11, "onclick",
                EventCallback.Factory.Create(this, RaiseServerClickAsync));
        }

        if (_innerHtml is not null)
        {
            builder.AddMarkupContent(5, _innerHtml);
        }
        else if (_innerText is not null)
        {
            builder.AddContent(6, _innerText);
        }
        else
        {
            builder.AddContent(7, ChildContent);
        }

        // Children added from code-behind. InnerHtml/InnerText REPLACE the content in
        // WebForms, so they win above; anything added to Controls renders after the
        // markup children, in the order it was added.
        RenderDynamicChildren(builder, 20);

        builder.CloseElement();
    }
}
