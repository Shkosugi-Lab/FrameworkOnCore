using System;
using System.Text;
using System.Web.UI;

namespace ShiftJisProbe
{
    // 送信された日本語(requestEncoding shift_jis)を読み、応答(responseEncoding shift_jis)に書く。
    public partial class Form : Page
    {
        protected void Sent(object sender, EventArgs e)
        {
            var name = txtName.Text;
            var text = new StringBuilder();
            text.AppendLine("こんにちは、" + name + " さん(" + ddlKind.SelectedValue + ")");
            text.AppendLine("文字数: " + name.Length + "、Shift_JIS のバイト数: " + Encoding.GetEncoding(932).GetByteCount(name));
            text.AppendLine("生の値: " + Request.Form["txtName"]);
            Out.Text = Server.HtmlEncode(text.ToString());
        }
    }
}
