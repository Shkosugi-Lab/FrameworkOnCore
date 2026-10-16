using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The DetailsView's scenario layer (scenarios/DetailsView.json): a details view bound by the page (paging, modes,
    /// updating, inserting, deleting in its handlers), one bound by an ObjectDataSource (all of it automatic), one paging by
    /// callbacks, an empty one, one without view state, a hidden one, a dynamic one; and a log of what the server saw.
    /// </summary>
    public partial class DetailsViewScenario : Page
    {
        readonly List<string> entries = new List<string>();
        DetailsView dynamicDetails;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dynamicDetails = new DetailsView { ID = "dvDynamic" };
            Dynamic.Controls.Add(dynamicDetails);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            if (!IsCallback) entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (IsPostBack) return;
            ProductStore.Reset();
            BindManual();
            dvEmpty.DataSource = new List<ProbeData.Product>();
            dvEmpty.DataBind();
            foreach (var details in new[] { dvNoViewState, dvHidden, dynamicDetails })
            {
                details.DataSource = ProbeData.Many().Take(2).Select(p => new { p.Name, p.Code }).ToList();
                details.DataBind();
            }
        }

        void BindManual()
        {
            dvManual.DataSource = ProductStore.Items.ToList();
            dvManual.DataBind();
        }

        protected void ManualPaging(object sender, DetailsViewPageEventArgs e)
        {
            entries.Add("PageIndexChanging: NewPageIndex=" + e.NewPageIndex);
            dvManual.PageIndex = e.NewPageIndex;
            BindManual();
        }

        protected void ManualModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            entries.Add("ModeChanging: NewMode=" + e.NewMode + " CancelingEdit=" + e.CancelingEdit);
            dvManual.ChangeMode(e.NewMode);
            BindManual();
        }

        protected void ManualUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            entries.Add("ItemUpdating: Keys=" + Values(e.Keys) + " OldValues=" + Values(e.OldValues) + " NewValues=" + Values(e.NewValues));
            var item = ProductStore.Items.First(p => p.Code == (string)e.Keys["Code"]);
            item.Name = (string)e.NewValues["Name"];
            item.Price = decimal.Parse((string)e.NewValues["Price"], NumberStyles.Number, CultureInfo.CurrentCulture);
            dvManual.ChangeMode(DetailsViewMode.ReadOnly);
            BindManual();
        }

        protected void ManualInserting(object sender, DetailsViewInsertEventArgs e)
        {
            entries.Add("ItemInserting: Values=" + Values(e.Values));
            ProductStore.Items.Add(new ProbeData.Product { Code = (string)e.Values["Code"], Name = (string)e.Values["Name"], Price = decimal.Parse((string)e.Values["Price"], NumberStyles.Number, CultureInfo.CurrentCulture) });
            dvManual.ChangeMode(DetailsViewMode.ReadOnly);
            BindManual();
        }

        protected void ManualDeleting(object sender, DetailsViewDeleteEventArgs e)
        {
            entries.Add("ItemDeleting: Keys=" + Values(e.Keys) + " Values=" + Values(e.Values));
            ProductStore.Items.RemoveAll(p => p.Code == (string)e.Keys["Code"]);
            BindManual();
        }

        protected void ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            entries.Add("ItemCommand " + ((Control)sender).ID + ": " + e.CommandName + " " + e.CommandArgument);
        }

        protected void OdsUpdating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            entries.Add("ObjectDataSource Updating: " + Values(e.InputParameters));
        }

        protected void OdsInserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            entries.Add("ObjectDataSource Inserting: " + Values(e.InputParameters));
        }

        protected void AutoUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            entries.Add("ItemUpdated: AffectedRows=" + e.AffectedRows + " Keys=" + Values(e.Keys) + " NewValues=" + Values(e.NewValues));
        }

        protected void AutoInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            entries.Add("ItemInserted: AffectedRows=" + e.AffectedRows + " Values=" + Values(e.Values));
        }

        protected void AutoDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            entries.Add("ItemDeleted: AffectedRows=" + e.AffectedRows + " Keys=" + Values(e.Keys));
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            dvHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        static string Values(IDictionary values) =>
            "{" + string.Join(", ", values.Keys.Cast<object>().OrderBy(k => k.ToString(), StringComparer.Ordinal).Select(k => k + "=" + Convert.ToString(values[k], CultureInfo.InvariantCulture))) + "}";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            foreach (var details in new[] { dvManual, dvAuto, dvCallback, dvEmpty, dvNoViewState, dvHidden, dynamicDetails })
                entries.Add("Details " + details.ID + ": PageIndex=" + details.PageIndex + " PageCount=" + details.PageCount + " CurrentMode=" + details.CurrentMode +
                            " Rows=" + details.Rows.Count + (details.Visible ? "" : " (hidden)"));
            entries.Add("Store: " + ProductStore.Describe());
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>DetailsView scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
