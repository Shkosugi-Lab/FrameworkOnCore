using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The DataGrid's scenario layer (scenarios/DataGrid.json): a data grid bound by the page (paging, sorting, selecting,
    /// editing, updating, deleting in its command handlers, its data keys), one bound by an ObjectDataSource, an empty one,
    /// one without view state, a hidden one, a dynamic one; and a log of what the server saw.
    /// </summary>
    public partial class DataGridScenario : Page
    {
        readonly List<string> entries = new List<string>();
        DataGrid dynamicGrid;

        string ManualSort
        {
            get { return (string)ViewState["sort"] ?? ""; }
            set { ViewState["sort"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dynamicGrid = new DataGrid { ID = "dgDynamic" };
            Dynamic.Controls.Add(dynamicGrid);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (IsPostBack) return;
            ProductStore.Reset();
            BindManual();
            dgEmpty.DataSource = new List<ProbeData.Product>();
            dgEmpty.DataBind();
            foreach (var grid in new[] { dgNoViewState, dgHidden, dynamicGrid })
            {
                grid.DataSource = ProbeData.Many().Take(2).Select(p => new { p.Name, p.Code }).ToList();
                grid.DataBind();
            }
        }

        void BindManual()
        {
            dgManual.DataSource = ProductStore.Sorted(ManualSort).ToList();
            dgManual.DataBind();
        }

        protected void PageChanged(object source, DataGridPageChangedEventArgs e)
        {
            entries.Add("PageIndexChanged: NewPageIndex=" + e.NewPageIndex);
            dgManual.CurrentPageIndex = e.NewPageIndex;
            BindManual();
        }

        protected void SortCommand(object source, DataGridSortCommandEventArgs e)
        {
            var descending = ManualSort == e.SortExpression;
            entries.Add("SortCommand: SortExpression=" + e.SortExpression + " -> " + (descending ? "descending" : "ascending"));
            ManualSort = descending ? e.SortExpression + " DESC" : e.SortExpression;
            BindManual();
        }

        protected void EditCommand(object source, DataGridCommandEventArgs e)
        {
            entries.Add("EditCommand: item " + e.Item.ItemIndex + " key " + dgManual.DataKeys[e.Item.ItemIndex]);
            dgManual.EditItemIndex = e.Item.ItemIndex;
            BindManual();
        }

        protected void UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            var name = ((TextBox)e.Item.FindControl("txtDgName")).Text;
            var key = (string)dgManual.DataKeys[e.Item.ItemIndex];
            entries.Add("UpdateCommand: item " + e.Item.ItemIndex + " key " + key + " name \"" + name + "\"");
            ProductStore.Items.First(p => p.Code == key).Name = name;
            dgManual.EditItemIndex = -1;
            BindManual();
        }

        protected void CancelCommand(object source, DataGridCommandEventArgs e)
        {
            entries.Add("CancelCommand: item " + e.Item.ItemIndex);
            dgManual.EditItemIndex = -1;
            BindManual();
        }

        protected void DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            var key = (string)dgManual.DataKeys[e.Item.ItemIndex];
            entries.Add("DeleteCommand: item " + e.Item.ItemIndex + " key " + key);
            ProductStore.Items.RemoveAll(p => p.Code == key);
            BindManual();
        }

        protected void ItemCommand(object source, DataGridCommandEventArgs e)
        {
            entries.Add("ItemCommand: " + e.CommandName + " " + e.CommandArgument + " item " + (e.Item == null ? "-" : e.Item.ItemIndex + " (" + e.Item.ItemType + ")"));
        }

        protected void SelectionChanged(object sender, EventArgs e)
        {
            entries.Add("SelectedIndexChanged: SelectedIndex=" + dgManual.SelectedIndex);
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            dgHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            foreach (var grid in new[] { dgManual, dgAuto, dgEmpty, dgNoViewState, dgHidden, dynamicGrid })
                entries.Add("DataGrid " + grid.ID + ": CurrentPageIndex=" + grid.CurrentPageIndex + " PageCount=" + grid.PageCount + " EditItemIndex=" + grid.EditItemIndex +
                            " SelectedIndex=" + grid.SelectedIndex + " Items=" + grid.Items.Count + (grid.Visible ? "" : " (hidden)"));
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
            Response.Write("<!DOCTYPE html><html><head><title>DataGrid scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
