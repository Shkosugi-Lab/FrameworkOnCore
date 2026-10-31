using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace NavigationProbe
{
    /// <summary>The request's event log (HttpContext.Items): every page of a Transfer / Execute chain writes to it.</summary>
    public static class Log
    {
        public static List<string> Of(HttpContext context)
        {
            var log = context.Items["navlog"] as List<string>;
            if (log == null)
            {
                log = new List<string>();
                context.Items["navlog"] = log;
            }
            return log;
        }

        public static void Add(string entry)
        {
            var context = HttpContext.Current;
            if (context != null) Of(context).Add(entry);
        }

        /// <summary>A value kept for the next request (the session): what code ran after a redirect / transfer.</summary>
        public static void Side(string entry)
        {
            var context = HttpContext.Current;
            if (context == null || context.Session == null) return;
            var old = context.Session["side"] as string;
            context.Session["side"] = old == null ? entry : old + " | " + entry;
        }
    }

    /// <summary>
    /// The pages' base: each lifecycle event logged as "Name.Event"; At(stage) lets a page act there (R/Life.aspx redirects
    /// in the stage its query names).
    /// </summary>
    public class ProbePage : Page
    {
        protected string Short { get { return Path.GetFileNameWithoutExtension(AppRelativeVirtualPath ?? "?"); } }

        protected void L(string entry) { Log.Add(Short + "." + entry); }

        protected virtual void At(string stage) { }

        protected override void OnPreInit(EventArgs e) { L("PreInit"); At("PreInit"); base.OnPreInit(e); }
        protected override void OnInit(EventArgs e) { L("Init"); At("Init"); base.OnInit(e); }
        protected override void OnInitComplete(EventArgs e) { L("InitComplete"); At("InitComplete"); base.OnInitComplete(e); }
        protected override void OnPreLoad(EventArgs e) { L("PreLoad"); At("PreLoad"); base.OnPreLoad(e); }
        protected override void OnLoad(EventArgs e) { L("Load"); At("Load"); base.OnLoad(e); }
        protected override void OnLoadComplete(EventArgs e) { L("LoadComplete"); At("LoadComplete"); base.OnLoadComplete(e); }
        protected override void OnPreRender(EventArgs e) { L("PreRender"); At("PreRender"); base.OnPreRender(e); }
        protected override void OnPreRenderComplete(EventArgs e) { L("PreRenderComplete"); At("PreRenderComplete"); base.OnPreRenderComplete(e); }
        protected override void OnSaveStateComplete(EventArgs e) { L("SaveStateComplete"); At("SaveStateComplete"); base.OnSaveStateComplete(e); }
        protected override void Render(HtmlTextWriter writer) { L("Render"); At("Render"); base.Render(writer); }
        protected override void OnUnload(EventArgs e) { L("Unload"); At("Unload"); base.OnUnload(e); }

        /// <summary>A control event, logged with the control's id.</summary>
        protected void Event(object sender, string name)
        {
            var control = sender as Control;
            L(name + "(" + (control != null ? control.ID : "?") + ")");
        }
    }

    /// <summary>
    /// What a page knows about where it is, rendered where the control is (the log so far: up to the page's Render):
    /// the page, the URL's parts, the postback, the previous page, the posted fields and the query, the session's side
    /// effects of the previous request (shown once).
    /// </summary>
    public class Info : Control
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var context = Context;
            var request = context.Request;
            var page = Page;
            var text = new StringBuilder();
            text.Append("page: ").Append(page != null ? page.AppRelativeVirtualPath : "(none)").Append('\n');
            text.Append("url: path=").Append(request.Path).Append(" raw=").Append(request.RawUrl)
                .Append(" exec=").Append(request.AppRelativeCurrentExecutionFilePath)
                .Append(" file=").Append(request.FilePath)
                .Append(" pathinfo=").Append(request.PathInfo).Append('\n');
            if (page != null)
            {
                var previous = page.PreviousPage;
                text.Append("post: method=").Append(request.HttpMethod)
                    .Append(" postback=").Append(page.IsPostBack)
                    .Append(" cross=").Append(page.IsCrossPagePostBack)
                    .Append(" async=").Append(IsAsyncPostBack(page))
                    .Append(" prev=").Append(previous == null ? "(none)" : previous.AppRelativeVirtualPath + "(cross=" + previous.IsCrossPagePostBack + ")")
                    .Append('\n');
            }
            text.Append("form: ").Append(Pairs(request.Form)).Append('\n');
            text.Append("query: ").Append(Pairs(request.QueryString)).Append('\n');
            text.Append("user: ").Append(context.User != null && context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "(anonymous)").Append('\n');
            if (context.Session != null && context.Session["side"] != null)
            {
                text.Append("side: ").Append(context.Session["side"]).Append('\n');
                context.Session.Remove("side");
            }
            text.Append("log: ").Append(string.Join(" ", Log.Of(context)));
            writer.Write("<pre id=\"" + (ID ?? "info") + "\">");
            writer.Write(HttpUtility.HtmlEncode(text.ToString()));
            writer.Write("</pre>");
        }

        private static bool IsAsyncPostBack(Page page)
        {
            var manager = System.Web.UI.ScriptManager.GetCurrent(page);
            return manager != null && manager.IsInAsyncPostBack;
        }

        /// <summary>The fields (the page's own hidden ones left out), "name=value" in the order sent; repeated names joined by '|'.</summary>
        public static string Pairs(System.Collections.Specialized.NameValueCollection values)
        {
            var parts = new List<string>();
            foreach (var key in values.AllKeys)
            {
                if (key != null && key.StartsWith("__", StringComparison.Ordinal)) continue;
                var all = values.GetValues(key);
                parts.Add((key ?? "(null)") + "=" + (all == null ? "" : string.Join("|", all)));
            }
            return string.Join(" & ", parts);
        }
    }
}
