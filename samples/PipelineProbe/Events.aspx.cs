using System;
using System.Web;
using System.Web.UI;

namespace PipelineProbe
{
    /// <summary>
    /// The page's events among the application's, in order (a postback: the control events between Load and LoadComplete),
    /// up to Render; and those of the last request to this page, after its handler (Unload, the application's end).
    /// </summary>
    public partial class Events : Page
    {
        void Note(string name) => Probe.Log(Context, "Page." + name);

        protected override void OnPreInit(EventArgs e) { Note("PreInit"); base.OnPreInit(e); }
        protected override void OnInit(EventArgs e) { Note("Init"); base.OnInit(e); }
        protected override void OnInitComplete(EventArgs e) { Note("InitComplete"); base.OnInitComplete(e); }
        protected override void OnPreLoad(EventArgs e) { Note("PreLoad"); base.OnPreLoad(e); }
        protected override void OnLoad(EventArgs e) { Note("Load(IsPostBack=" + IsPostBack + ")"); base.OnLoad(e); }
        protected override void OnLoadComplete(EventArgs e) { Note("LoadComplete"); base.OnLoadComplete(e); }
        protected override void OnPreRender(EventArgs e) { Note("PreRender"); base.OnPreRender(e); }
        protected override void OnPreRenderComplete(EventArgs e) { Note("PreRenderComplete"); base.OnPreRenderComplete(e); }
        protected override void OnSaveStateComplete(EventArgs e) { Note("SaveStateComplete"); base.OnSaveStateComplete(e); }
        protected override void OnUnload(EventArgs e) { Note("Unload"); base.OnUnload(e); }

        protected void ValueChanged(object sender, EventArgs e) => Note("txtValue.TextChanged");

        protected void PostClick(object sender, EventArgs e) => Note("btnPost.Click");

        protected override void Render(HtmlTextWriter writer)
        {
            Note("Render");
            var previous = Probe.Previous(Request.Path);
            Log.Text = HttpUtility.HtmlEncode(
                "Application_Start ran: " + Global.Started +
                "\nThis request: " + string.Join(", ", Probe.Events(Context)) +
                "\nThe last request to this page, after Render: " + string.Join(", ", AfterRender(previous)));
            base.Render(writer);
        }

        /// <summary>What a request's list had after the page's Render.</summary>
        static string[] AfterRender(System.Collections.Generic.List<string> events)
        {
            var i = events.IndexOf("Page.Render");
            return i < 0 ? new string[0] : events.GetRange(i + 1, events.Count - i - 1).ToArray();
        }
    }
}
