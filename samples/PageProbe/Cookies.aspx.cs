using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace PageProbe
{
    /// <summary>
    /// Cookies as the application sets them (?set=1: values with spaces, Japanese, '=' and ';' encoded or not, a
    /// multi-value cookie, HttpOnly, a path, an expired one) and reads them back on the next request: the server's
    /// Request.Cookies (names, values, Values), the raw Cookie header, and what the browser's script sees.
    /// </summary>
    public partial class Cookies : Page
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var lines = new List<string>();
            if (Request.QueryString["set"] == "1")
            {
                Response.Cookies.Add(new HttpCookie("plain", "simple"));
                Response.Cookies.Add(new HttpCookie("spaced", "a b c"));
                Response.Cookies.Add(new HttpCookie("encoded", HttpUtility.UrlEncode("日本語 a=b;c")));
                var multi = new HttpCookie("multi");
                multi.Values["first"] = "1";
                multi.Values["second"] = "two words";
                multi.Values["third"] = "x&y";
                Response.Cookies.Add(multi);
                Response.Cookies.Add(new HttpCookie("secret", "only server") { HttpOnly = true });
                Response.Cookies.Add(new HttpCookie("scoped", "under /Sub") { Path = "/Sub" });
                Response.Cookies.Add(new HttpCookie("gone", "x") { Expires = new DateTime(2000, 1, 1) });
                Response.Cookies["indexer"].Value = "made by the indexer";
                lines.Add("set " + Response.Cookies.Count + " cookie(s): " + string.Join(",", Response.Cookies.AllKeys));
            }
            foreach (var name in Request.Cookies.AllKeys.Where(k => k != "ASP.NET_SessionId").OrderBy(k => k, StringComparer.Ordinal))
            {
                var cookie = Request.Cookies[name];
                lines.Add(name + " = [" + cookie.Value + "] HasKeys=" + cookie.HasKeys
                    + (cookie.HasKeys ? " values: " + string.Join(", ", cookie.Values.AllKeys.Select(k => k + "=" + cookie.Values[k])) : "")
                    + (name == "encoded" ? " decoded [" + HttpUtility.UrlDecode(cookie.Value) + "]" : ""));
            }
            var header = (Request.Headers["Cookie"] ?? "").Split(';').Select(p => p.Trim()).Where(p => p.Length > 0 && !p.StartsWith("ASP.NET_SessionId")).OrderBy(p => p, StringComparer.Ordinal);
            lines.Add("header: " + string.Join("; ", header));
            writer.Write("<!DOCTYPE html><html><head><title>Cookies</title></head><body><pre id=\"server\">" + HttpUtility.HtmlEncode(string.Join("\n", lines)) + "</pre>");
            writer.Write("<pre id=\"script\"></pre><script>document.getElementById('script').textContent = document.cookie.split('; ').filter(function (c) { return c && c.indexOf('ASP.NET_SessionId') != 0; }).sort().join('\\n');</script>");
            writer.Write("<a id=\"again\" href=\"Cookies.aspx\">again</a></body></html>");
        }
    }
}
