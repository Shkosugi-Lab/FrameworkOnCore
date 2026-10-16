using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace WebControlsProbe
{
    /// <summary>
    /// The web parts' scenario layer (scenarios/WebPartManager.json and the zones' and parts'): a user (taro, set for the
    /// request: personalization is a user's) and the page's parts in zones; the display modes (design, catalog, edit,
    /// connect); a part added from the catalog, a part's title edited, its connections shown; personalization in memory
    /// (ProbePersonalization.cs; ?reset=1 forgets it); a zone made in code; a log of the web part manager's events.
    /// </summary>
    public partial class WebPartsScenario : Page
    {
        readonly List<string> entries = new List<string>();

        protected string BoundText => "bound";

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            // Personalization is per user: an authenticated one (as forms authentication would set).
            Context.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity("taro", "Forms"), new string[0]);
            if (Request.QueryString["reset"] == "1" && !IsPostBack) ProbePersonalizationProvider.Reset();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var zone = new WebPartZone { ID = "wpzDynamic", HeaderText = "Dynamic zone", WebPartVerbRenderMode = WebPartVerbRenderMode.TitleBar, ZoneTemplate = new ProbeControlsTemplate(() => new Control[] { new Label { ID = "lblDynamicPart", Text = "A dynamic part" } }) };
            Dynamic.Controls.Add(zone);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack + " User=" + User.Identity.Name);
            if (IsPostBack) return;
            DataBind();
        }

        void To(WebPartDisplayMode mode)
        {
            try { wpmPage.DisplayMode = mode; }
            catch (Exception error) { entries.Add("DisplayMode " + mode.Name + ": " + error.GetType().FullName); }
        }

        protected void ToBrowse(object sender, EventArgs e) => To(WebPartManager.BrowseDisplayMode);

        protected void ToDesign(object sender, EventArgs e) => To(WebPartManager.DesignDisplayMode);

        protected void ToCatalog(object sender, EventArgs e) => To(WebPartManager.CatalogDisplayMode);

        protected void EditFirst(object sender, EventArgs e)
        {
            To(WebPartManager.EditDisplayMode);
            wpmPage.BeginWebPartEditing(wpmPage.WebParts[0]);
        }

        protected void ConnectFirst(object sender, EventArgs e)
        {
            To(WebPartManager.ConnectDisplayMode);
            wpmPage.BeginWebPartConnecting(wpmPage.WebParts[0]);
        }

        protected void DisplayModeChanged(object sender, WebPartDisplayModeEventArgs e) => entries.Add("DisplayModeChanged " + e.OldDisplayMode.Name + " -> " + wpmPage.DisplayMode.Name);

        protected void WebPartAdded(object sender, WebPartEventArgs e) => entries.Add("WebPartAdded " + e.WebPart.Title + " in " + e.WebPart.Zone.ID);

        protected void WebPartClosed(object sender, WebPartEventArgs e) => entries.Add("WebPartClosed " + e.WebPart.Title);

        protected void WebPartMoved(object sender, WebPartEventArgs e) => entries.Add("WebPartMoved " + e.WebPart.Title);

        protected void ShowHidden(object sender, EventArgs e)
        {
            wpzHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            lblMode.Text = wpmPage.DisplayMode.Name;
            entries.Add("State DisplayMode=" + wpmPage.DisplayMode.Name + " Scope=" + wpmPage.Personalization.Scope + " IsModifiable=" + wpmPage.Personalization.IsModifiable);
            entries.Add("State parts: " + string.Join(", ", wpmPage.WebParts.Cast<WebPart>().Select(p => p.Title + " (" + (p.Zone == null ? "-" : p.Zone.ID) + (p.IsClosed ? ", closed" : "") + ")")));
            entries.Add("State supported modes: " + string.Join(", ", wpmPage.SupportedDisplayModes.Cast<WebPartDisplayMode>().Select(m => m.Name + (m.IsEnabled(wpmPage) ? "" : " (disabled)"))));
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>Web parts scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
