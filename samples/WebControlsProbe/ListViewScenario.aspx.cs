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
    /// The ListView's and the DataPager's scenario layer (scenarios/ListView.json, DataPager.json): a list view bound by the
    /// page with its pager (paging, sorting, selecting, editing, inserting, deleting in its handlers), one bound by an
    /// ObjectDataSource with its pager (all of it automatic), a grouped one, an empty one, one without view state, a hidden
    /// one, a dynamic one with a dynamic pager; and a log of what the server saw.
    /// </summary>
    public partial class ListViewScenario : Page
    {
        readonly List<string> entries = new List<string>();
        ListView dynamicList;
        DataPager dynamicPager;

        string ManualSort
        {
            get { return (string)ViewState["sort"] ?? ""; }
            set { ViewState["sort"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dynamicList = new ListView { ID = "lvDynamic", ItemTemplate = new ProbeTemplate("dynamic") };
            dynamicList.PagePropertiesChanging += (sender, args) =>
            {
                entries.Add("lvDynamic PagePropertiesChanging: StartRowIndex=" + args.StartRowIndex + " MaximumRows=" + args.MaximumRows);
                dynamicPager.SetPageProperties(args.StartRowIndex, args.MaximumRows, false);
                BindDynamic();
            };
            dynamicPager = new DataPager { ID = "dpDynamic", PagedControlID = "lvDynamic", PageSize = 2 };
            dynamicPager.Fields.Add(new NumericPagerField());
            Dynamic.Controls.Add(dynamicList);
            Dynamic.Controls.Add(dynamicPager);
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
            lvGroups.DataSource = ProbeData.Many();
            lvGroups.DataBind();
            lvEmpty.DataSource = new List<ProbeData.Product>();
            lvEmpty.DataBind();
            foreach (var list in new[] { lvNoViewState, lvHidden })
            {
                list.DataSource = ProbeData.Many().Take(2).ToList();
                list.DataBind();
            }
            BindDynamic();
        }

        void BindManual()
        {
            lvManual.DataSource = ProductStore.Sorted(ManualSort).ToList();
            lvManual.DataBind();
        }

        void BindDynamic()
        {
            dynamicList.DataSource = ProbeData.Many().Take(5).ToList();
            dynamicList.DataBind();
        }

        protected void ManualPaging(object sender, PagePropertiesChangingEventArgs e)
        {
            entries.Add("PagePropertiesChanging: StartRowIndex=" + e.StartRowIndex + " MaximumRows=" + e.MaximumRows);
            dpManual.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            BindManual();
        }

        protected void ManualSorting(object sender, ListViewSortEventArgs e)
        {
            var descending = ManualSort == e.SortExpression;
            entries.Add("Sorting: SortExpression=" + e.SortExpression + " SortDirection=" + e.SortDirection + " -> " + (descending ? "descending" : "ascending"));
            ManualSort = descending ? e.SortExpression + " DESC" : e.SortExpression;
            BindManual();
        }

        protected void ManualSelecting(object sender, ListViewSelectEventArgs e)
        {
            entries.Add("SelectedIndexChanging: NewSelectedIndex=" + e.NewSelectedIndex);
            lvManual.SelectedIndex = e.NewSelectedIndex;
            BindManual();
        }

        protected void SelectionChanged(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            entries.Add("SelectedIndexChanged " + list.ID + ": SelectedIndex=" + list.SelectedIndex + " SelectedValue=" + list.SelectedValue);
        }

        protected void ManualEditing(object sender, ListViewEditEventArgs e)
        {
            entries.Add("ItemEditing: NewEditIndex=" + e.NewEditIndex);
            lvManual.EditIndex = e.NewEditIndex;
            BindManual();
        }

        protected void ManualCanceling(object sender, ListViewCancelEventArgs e)
        {
            entries.Add("ItemCanceling: ItemIndex=" + e.ItemIndex + " CancelMode=" + e.CancelMode);
            lvManual.EditIndex = -1;
            BindManual();
        }

        protected void ManualUpdating(object sender, ListViewUpdateEventArgs e)
        {
            entries.Add("ItemUpdating: ItemIndex=" + e.ItemIndex + " Keys=" + Values(e.Keys) + " OldValues=" + Values(e.OldValues) + " NewValues=" + Values(e.NewValues));
            ProductStore.Items.First(p => p.Code == (string)e.Keys["Code"]).Name = (string)e.NewValues["Name"];
            lvManual.EditIndex = -1;
            BindManual();
        }

        protected void ManualInserting(object sender, ListViewInsertEventArgs e)
        {
            entries.Add("ItemInserting: Values=" + Values(e.Values));
            ProductStore.Items.Add(new ProbeData.Product { Code = (string)e.Values["Code"], Name = (string)e.Values["Name"] });
            BindManual();
        }

        protected void ManualDeleting(object sender, ListViewDeleteEventArgs e)
        {
            entries.Add("ItemDeleting: ItemIndex=" + e.ItemIndex + " Keys=" + Values(e.Keys) + " Values=" + Values(e.Values));
            ProductStore.Items.RemoveAll(p => p.Code == (string)e.Keys["Code"]);
            BindManual();
        }

        protected void ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            entries.Add("ItemCommand " + ((Control)sender).ID + ": " + e.CommandName + " " + e.CommandArgument + " item " + (e.Item == null ? "-" : e.Item.ItemType.ToString()));
        }

        protected void OdsUpdating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            entries.Add("ObjectDataSource Updating: " + Values(e.InputParameters));
        }

        protected void AutoSorting(object sender, ListViewSortEventArgs e)
        {
            entries.Add("lvAuto Sorting: SortExpression=" + e.SortExpression + " SortDirection=" + e.SortDirection);
        }

        protected void AutoUpdated(object sender, ListViewUpdatedEventArgs e)
        {
            entries.Add("ItemUpdated: AffectedRows=" + e.AffectedRows + " NewValues=" + Values(e.NewValues));
        }

        protected void AutoDeleted(object sender, ListViewDeletedEventArgs e)
        {
            entries.Add("ItemDeleted: AffectedRows=" + e.AffectedRows + " Keys=" + Values(e.Keys));
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            lvHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        static string Values(IDictionary values) =>
            "{" + string.Join(", ", values.Keys.Cast<object>().OrderBy(k => k.ToString(), StringComparer.Ordinal).Select(k => k + "=" + Convert.ToString(values[k], CultureInfo.InvariantCulture))) + "}";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            foreach (var list in new[] { lvManual, lvAuto, lvGroups, lvEmpty, lvNoViewState, lvHidden, dynamicList })
                entries.Add("List " + list.ID + ": Items=" + list.Items.Count + " EditIndex=" + list.EditIndex + " SelectedIndex=" + list.SelectedIndex +
                            " SortExpression=\"" + list.SortExpression + "\" SortDirection=" + list.SortDirection + (list.Visible ? "" : " (hidden)"));
            foreach (var pager in new[] { dpManual, dpAuto, dynamicPager })
                entries.Add("Pager " + pager.ID + ": StartRowIndex=" + pager.StartRowIndex + " MaximumRows=" + pager.MaximumRows + " TotalRowCount=" + pager.TotalRowCount);
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
            Response.Write("<!DOCTYPE html><html><head><title>ListView scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
