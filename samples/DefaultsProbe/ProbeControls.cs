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

    /// <summary>
    /// TextBox 継承で保護メソッド OnTextChanged を上書きするカスタムコントロール。
    /// 互換層ではマークアップの引数が OnTextChanged のため、保護メソッドは TextChangedHandler
    /// という名前になっている。変換器が上書きと base 呼び出しを改名することの適合検証用。
    /// </summary>
    public class TrimmingTextBox : TextBox
    {
        protected override void OnTextChanged(System.EventArgs e)
        {
            this.Text = this.Text.Trim();
            base.OnTextChanged(e);
        }

        public void RaiseTextChanged()
        {
            OnTextChanged(System.EventArgs.Empty);
        }
    }

    /// <summary>アプリ内の基底を 1 段挟んだ場合(基底の連鎖を辿って改名できること)の検証用。</summary>
    public class UpperTrimmingTextBox : TrimmingTextBox
    {
        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            this.Text = this.Text.ToUpperInvariant();
        }
    }
}
