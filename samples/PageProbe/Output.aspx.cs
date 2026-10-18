using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace PageProbe
{
    /// <summary>
    /// What reaches the browser when the application writes around the page (?mode=): a response filter that changes
    /// the whole output (upper case, a stamp at the end), text written in Load (before the page's markup), a plain text
    /// answer, Clear after writing, Flush in the middle, the head's title and meta made in code.
    /// </summary>
    public partial class Output : Page
    {
        sealed class UpperFilter : Stream
        {
            readonly Stream inner;
            readonly Encoding encoding;
            public UpperFilter(Stream inner, Encoding encoding) { this.inner = inner; this.encoding = encoding; }
            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override void Flush() => inner.Flush();
            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count)
            {
                var text = encoding.GetString(buffer, offset, count).Replace("label text", "LABEL TEXT (filtered)").Replace("</body>", "<p id=\"stamp\">filtered</p></body>");
                var bytes = encoding.GetBytes(text);
                inner.Write(bytes, 0, bytes.Length);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (Request.QueryString["mode"])
            {
                case "filter":
                    Response.Filter = new UpperFilter(Response.Filter, Response.ContentEncoding);
                    break;
                case "early":
                    Response.Write("<p id=\"early\">written in Load &amp; before the page</p>");
                    break;
                case "plain":
                    Response.ContentType = "text/plain";
                    Response.Write("plain text <b>not markup</b>\nsecond line");
                    Response.End();
                    break;
                case "clear":
                    Response.Write("<p id=\"cleared\">this is cleared</p>");
                    Response.Clear();
                    Response.Write("<p id=\"kept\">written after Clear</p>");
                    break;
                case "flush":
                    Response.Write("<p id=\"flushed\">flushed first</p>");
                    Response.Flush();
                    break;
                case "head":
                    Title = "Title <made> in code & more";
                    MetaDescription = "description \"quoted\"";
                    MetaKeywords = "a, b";
                    Header.Controls.Add(new LiteralControl("<meta name=\"added\" content=\"by code\" />"));
                    Header.Controls.Add(new System.Web.UI.HtmlControls.HtmlLink { Href = "~/probe.css" });
                    break;
            }
        }
    }
}
