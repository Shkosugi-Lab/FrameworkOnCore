using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The ASP.NET AJAX controls' scenario layer (scenarios/ScriptManager.json, ScriptManagerProxy.json, UpdatePanel.json,
    /// UpdateProgress.json, Timer.json): partial rendering (a panel's own button, a trigger outside, a postback trigger
    /// inside, a conditional panel left as it was), the progress shown while a slow request runs, a timer's ticks, a
    /// history point and the browser's back button (the script manager's Navigate), a proxy's script, a panel made in code;
    /// and a log, in a panel updated on every request (the request's number, kept in view state; partial or full; the
    /// events in the order raised).
    /// </summary>
    public partial class AjaxScenario : Page
    {
        readonly List<string> entries = new List<string>();
        Label dynamicLabel;

        protected string BoundText => "bound";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // A panel made in code, with a button that posts it asynchronously.
            var panel = new UpdatePanel { ID = "upDynamic", UpdateMode = UpdatePanelUpdateMode.Conditional };
            dynamicLabel = new Label { ID = "lblDynamic" };
            var button = new Button { ID = "btnDynamic", Text = "Post the dynamic panel" };
            button.Click += (sender, args) => { entries.Add("Click btnDynamic"); dynamicLabel.Text = "dynamic, posted"; };
            panel.ContentTemplateContainer.Controls.Add(dynamicLabel);
            panel.ContentTemplateContainer.Controls.Add(button);
            Dynamic.Controls.Add(panel);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack + " IsInAsyncPostBack=" + smPage.IsInAsyncPostBack +
                        (smPage.IsInAsyncPostBack ? " AsyncPostBackSourceElementID=" + smPage.AsyncPostBackSourceElementID : ""));
            lblMainRequest.Text = number.ToString();
            lblTriggeredRequest.Text = number.ToString();
            if (IsPostBack) return;
            lblInHidden.Text = "set while hidden";
            dynamicLabel.Text = "dynamic";
            DataBind();
        }

        protected void PanelClicked(object sender, EventArgs e) => entries.Add("Click " + ((Control)sender).ID);

        protected void SlowClicked(object sender, EventArgs e)
        {
            entries.Add("Click btnSlow");
            System.Threading.Thread.Sleep(2500);
        }

        protected void OutsideClicked(object sender, EventArgs e)
        {
            entries.Add("Click btnOutside");
            upTriggered.Update();
        }

        protected void HistoryClicked(object sender, EventArgs e)
        {
            entries.Add("Click btnHistory");
            smPage.AddHistoryPoint("step", "1", "History point 1");
        }

        protected void Navigated(object sender, HistoryEventArgs e) =>
            entries.Add("Navigate step=" + (e.State["step"] ?? "(none)") + " IsNavigating=" + smPage.IsNavigating);

        protected void StartTimer(object sender, EventArgs e)
        {
            entries.Add("Click btnStartTimer");
            tmrMain.Enabled = true;
        }

        protected void Ticked(object sender, EventArgs e)
        {
            var ticks = int.Parse(lblTicks.Text) + 1;
            lblTicks.Text = ticks.ToString();
            entries.Add("Tick " + ticks);
            // Once: the timer stops itself.
            tmrMain.Enabled = false;
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            upHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            entries.Add("State upMain UpdateMode=" + upMain.UpdateMode + " IsInPartialRendering=" + upMain.IsInPartialRendering);
            entries.Add("State upTriggered IsInPartialRendering=" + upTriggered.IsInPartialRendering);
            entries.Add("State txtInPanel Text=\"" + txtInPanel.Text + "\"");
            entries.Add("State lblBound Text=\"" + lblBound.Text + "\"");
            entries.Add("State lblInHidden Text=\"" + lblInHidden.Text + "\"" + (upHidden.Visible ? "" : " (hidden)"));
            entries.Add("State tmrMain Enabled=" + tmrMain.Enabled + " Interval=" + tmrMain.Interval);
            entries.Add("State lblDynamic Text=\"" + dynamicLabel.Text + "\"");
            entries.Add("State smPage EnablePartialRendering=" + smPage.EnablePartialRendering + " SupportsPartialRendering=" + smPage.SupportsPartialRendering + " EnableHistory=" + smPage.EnableHistory);
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
