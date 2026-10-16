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
    /// The GridView's scenario layer (scenarios/GridView.json): a grid bound by the page (DataSource; paging, sorting,
    /// selecting, editing, deleting in its handlers), one bound by an ObjectDataSource (all of it automatic), one paging
    /// and sorting by callbacks, an empty one, one without view state, a hidden one, a dynamic one; and a log of what the
    /// server saw (the request's number, kept in view state: a callback is no postback; each event with its arguments,
    /// each grid's page, sort, selection and edit row, the store's items).
    /// </summary>
    public partial class GridViewScenario : Page
    {
        readonly List<string> entries = new List<string>();
        GridView dynamicGrid;

        string ManualSort
        {
            get { return (string)ViewState["sort"] ?? ""; }
            set { ViewState["sort"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dynamicGrid = new GridView { ID = "gvDynamic", AutoGenerateColumns = true };
            Dynamic.Controls.Add(dynamicGrid);
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
            gvEmpty.DataSource = new List<ProbeData.Product>();
            gvEmpty.DataBind();
            foreach (var grid in new[] { gvNoViewState, gvHidden, dynamicGrid })
            {
                grid.DataSource = ProbeData.Many().Take(2).Select(p => new { p.Name, p.Code }).ToList();
                grid.DataBind();
            }
        }

        void BindManual()
        {
            gvManual.DataSource = ProductStore.Sorted(ManualSort).ToList();
            gvManual.DataBind();
        }

        protected void ManualPaging(object sender, GridViewPageEventArgs e)
        {
            entries.Add("PageIndexChanging: NewPageIndex=" + e.NewPageIndex);
            gvManual.PageIndex = e.NewPageIndex;
            BindManual();
        }

        protected void ManualSorting(object sender, GridViewSortEventArgs e)
        {
            // The page keeps the direction: with a DataSource the grid's own SortDirection stays Ascending.
            var descending = ManualSort == e.SortExpression;
            entries.Add("Sorting: SortExpression=" + e.SortExpression + " SortDirection=" + e.SortDirection + " -> " + (descending ? "descending" : "ascending"));
            ManualSort = descending ? e.SortExpression + " DESC" : e.SortExpression;
            BindManual();
        }

        protected void ManualEditing(object sender, GridViewEditEventArgs e)
        {
            entries.Add("RowEditing: NewEditIndex=" + e.NewEditIndex);
            gvManual.EditIndex = e.NewEditIndex;
            BindManual();
        }

        protected void ManualCanceling(object sender, GridViewCancelEditEventArgs e)
        {
            entries.Add("RowCancelingEdit: RowIndex=" + e.RowIndex);
            gvManual.EditIndex = -1;
            BindManual();
        }

        protected void ManualUpdating(object sender, GridViewUpdateEventArgs e)
        {
            entries.Add("RowUpdating: RowIndex=" + e.RowIndex + " Keys=" + Values(e.Keys) + " OldValues=" + Values(e.OldValues) + " NewValues=" + Values(e.NewValues));
            var item = ProductStore.Items.First(p => p.Code == (string)e.Keys["Code"]);
            item.Name = (string)e.NewValues["Name"];
            item.Price = decimal.Parse((string)e.NewValues["Price"], NumberStyles.Number, CultureInfo.CurrentCulture);
            gvManual.EditIndex = -1;
            BindManual();
        }

        protected void ManualDeleting(object sender, GridViewDeleteEventArgs e)
        {
            entries.Add("RowDeleting: RowIndex=" + e.RowIndex + " Keys=" + Values(e.Keys) + " Values=" + Values(e.Values));
            ProductStore.Items.RemoveAll(p => p.Code == (string)e.Keys["Code"]);
            BindManual();
        }

        protected void RowCommand(object sender, GridViewCommandEventArgs e)
        {
            entries.Add("RowCommand " + ((Control)sender).ID + ": " + e.CommandName + " " + e.CommandArgument + " (from " + ((Control)e.CommandSource).GetType().Name + ")");
        }

        protected void SelectionChanged(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            entries.Add("SelectedIndexChanged " + grid.ID + ": SelectedIndex=" + grid.SelectedIndex + " SelectedValue=" + grid.SelectedValue);
        }

        protected void OdsUpdating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            entries.Add("ObjectDataSource Updating: " + Values(e.InputParameters));
        }

        protected void OdsDeleting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            entries.Add("ObjectDataSource Deleting: " + Values(e.InputParameters));
        }

        protected void AutoUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            entries.Add("RowUpdated: AffectedRows=" + e.AffectedRows + " Keys=" + Values(e.Keys) + " NewValues=" + Values(e.NewValues));
        }

        protected void AutoDeleted(object sender, GridViewDeletedEventArgs e)
        {
            entries.Add("RowDeleted: AffectedRows=" + e.AffectedRows + " Keys=" + Values(e.Keys));
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            gvHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        static string Values(IDictionary values) =>
            "{" + string.Join(", ", values.Keys.Cast<object>().OrderBy(k => k.ToString(), StringComparer.Ordinal).Select(k => k + "=" + Convert.ToString(values[k], CultureInfo.InvariantCulture))) + "}";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            foreach (var grid in new[] { gvManual, gvAuto, gvCallback, gvEmpty, gvNoViewState, gvHidden, dynamicGrid })
                entries.Add("Grid " + grid.ID + ": PageIndex=" + grid.PageIndex + " PageCount=" + grid.PageCount + " EditIndex=" + grid.EditIndex + " SelectedIndex=" + grid.SelectedIndex +
                            " SortExpression=\"" + grid.SortExpression + "\" SortDirection=" + grid.SortDirection + " Rows=" + grid.Rows.Count + (grid.Visible ? "" : " (hidden)"));
            entries.Add("Manual sort: \"" + ManualSort + "\"");
            entries.Add("Store: " + ProductStore.Describe());
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>GridView scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
