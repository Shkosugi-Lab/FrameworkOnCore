using System;
using System.Web.UI;
using System.Text;
namespace RuntimeProbe
{
    public partial class Encodings : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // .NET Framework's Encoding.Default: the system's ANSI code page; code pages by name.
            Response.Write("default=" + Encoding.Default.WebName + "\n");
            Response.Write("shift_jis=" + BitConverter.ToString(Encoding.GetEncoding("shift_jis").GetBytes("日本語")) + "\n");
            Response.Write("euc-jp=" + BitConverter.ToString(Encoding.GetEncoding(51932).GetBytes("日本語")) + "\n");
        }
    }
}
