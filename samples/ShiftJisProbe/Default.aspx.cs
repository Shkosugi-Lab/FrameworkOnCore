using System;
using System.Configuration;
using System.IO;
using System.Resources;
using System.Text;
using System.Web.UI;

namespace ShiftJisProbe
{
    // 既定のページ: コードビハインドの日本語(コメント、文字列、識別子)。
    // 2 バイト目が 0x5C(\)の文字: ソ、表、能、十、予、申 -- UTF-8 として読むと、文字列の閉じ引用符を逃がす。
    public partial class _Default : Page
    {
        const string 見出し = "ソフト表示能力";

        protected void Page_Load(object sender, EventArgs e)
        {
            var text = new StringBuilder();
            text.AppendLine("定数: " + 見出し);
            text.AppendLine("文字数: " + 見出し.Length);
            text.AppendLine("エスケープ: \"表\" \\ 十\t予");
            text.AppendLine("appSettings: " + ConfigurationManager.AppSettings["SiteTitle"]);
            // 埋め込みリソース(Resources\Strings.resx: UTF-8、Visual Studio の既定)
            var resources = new ResourceManager("ShiftJisProbe.Resources.Strings", typeof(_Default).Assembly);
            text.AppendLine("リソース: " + resources.GetString("Caption"));
            // Encoding.Default: .NET Framework では ANSI コードページ(日本語の Windows では shift_jis)
            text.AppendLine("Encoding.Default: " + Encoding.Default.WebName + " " + Encoding.Default.GetByteCount("表示"));
            text.AppendLine("Response: " + Response.ContentEncoding.WebName + ", Request: " + Request.ContentEncoding.WebName);
            // Shift_JIS のファイルを書いて読む(App_Data\メモ.txt: Windows の区切り文字)
            var memo = Server.MapPath("~/App_Data") + "\\memo.txt";
            Directory.CreateDirectory(Server.MapPath("~/App_Data"));
            File.WriteAllText(memo, "メモ:表示能力\r\n二行目", Encoding.GetEncoding("shift_jis"));
            var bytes = File.ReadAllBytes(memo);
            text.AppendLine("ファイル: " + File.ReadAllText(memo, Encoding.GetEncoding(932)).Replace("\r\n", " / ") + " (" + bytes.Length + " bytes)");
            text.AppendLine("古い処理: " + 古い機能.使える());
            Out.Text = Server.HtmlEncode(text.ToString());
        }
    }
}
