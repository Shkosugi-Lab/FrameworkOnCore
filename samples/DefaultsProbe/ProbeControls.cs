using System.Web.UI;
using System.Web.UI.WebControls;

namespace DefaultsProbe
{
    /// <summary>
    /// Render(HtmlTextWriter) 型のカスタムコントロール(YAF の LocalizedLabel 等と同型)。
    /// LegacyRenderHost 機構の適合検証用。
    /// </summary>
    public class FancyBadge : WebControl
    {
        public string Label { get; set; }

        public int Count { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "badge");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(this.Label);
            writer.Write(" (");
            writer.Write(this.Count);
            writer.Write(")");
            writer.RenderEndTag();
        }
    }

    /// <summary>
    /// Panel 継承+子要素を持つカスタムコントロール(mojoPortal の FormGroupPanel 等と同型)。
    /// LegacyRenderHost の Begin/ChildContent/End 分割描画の適合検証用。
    /// </summary>
    public class FancyPanel : Panel
    {
    }

    /// <summary>
    /// HyperLink 継承のカスタムコントロール(mojoPortal の mojoHelpLink 等と同型)。
    /// 表示系基底シム(LegacyHyperLink)の適合検証用。
    /// </summary>
    public class FancyLink : HyperLink
    {
    }
}
