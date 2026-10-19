using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LegacyRenderProbe
{
    /// <summary>
    /// The controls as ASP.NET 3.5 rendered them (web.config: controlRenderingCompatibilityVersion 3.5, xhtmlConformance
    /// Legacy): Menu and TreeView (tables and their scripts), Calendar, Panel with a grouping text and a default button,
    /// validators, disabled controls (3.5 wrote disabled spans and classes differently), list controls, GridView with
    /// paging, sorting and an image ButtonField, DetailsView, Wizard with its side bar, Table, ImageButton. The events
    /// the clicks raise are written at the bottom. The calendar's dates are pinned.
    /// </summary>
    public partial class Controls : Page
    {
        public class Row
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Active { get; set; }
        }

        readonly List<string> entries = new List<string>();

        static List<Row> Rows() => new List<Row>
        {
            new Row { Id = 1, Name = "delta", Active = true },
            new Row { Id = 2, Name = "Alpha", Active = false },
            new Row { Id = 3, Name = "charlie <c>", Active = true },
            new Row { Id = 4, Name = "Bravo & co", Active = false },
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            cal.TodaysDate = new DateTime(2026, 3, 18);
            if (!IsPostBack)
            {
                cal.VisibleDate = new DateTime(2026, 3, 1);
                Bind(null);
            }
            entries.Add("Load IsPostBack=" + IsPostBack);
        }

        void Bind(string sort)
        {
            var rows = Rows().AsEnumerable();
            if (sort != null) rows = rows.OrderBy(r => r.Name, StringComparer.Ordinal);
            gv.DataSource = rows.ToList();
            gv.DataBind();
            dv.DataSource = Rows();
            dv.DataBind();
        }

        protected void MenuClicked(object sender, MenuEventArgs e) => entries.Add("Menu " + e.Item.Value + " path=" + e.Item.ValuePath);
        protected void TreeSelected(object sender, EventArgs e) => entries.Add("Tree selected " + tree.SelectedNode.ValuePath);
        protected void TreeExpanded(object sender, TreeNodeEventArgs e) => entries.Add("Tree expanded " + e.Node.ValuePath);
        protected void CalendarChanged(object sender, EventArgs e) => entries.Add("Calendar " + string.Join(",", cal.SelectedDates.Cast<DateTime>().Select(d => d.ToString("yyyy-MM-dd"))));
        protected void Submitted(object sender, EventArgs e) => entries.Add("Submit IsValid=" + IsValid + " name=" + txtName.Text + " checks=" + string.Join(",", cbl.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value)) + " radio=" + rbl.SelectedValue);
        protected void GridPaging(object sender, GridViewPageEventArgs e) { gv.PageIndex = e.NewPageIndex; Bind(null); entries.Add("Grid page " + e.NewPageIndex); }
        protected void GridSorting(object sender, GridViewSortEventArgs e) { Bind(e.SortExpression); entries.Add("Grid sort " + e.SortExpression); }
        protected void DetailsPaging(object sender, DetailsViewPageEventArgs e) { dv.PageIndex = e.NewPageIndex; dv.DataSource = Rows(); dv.DataBind(); entries.Add("Details page " + e.NewPageIndex); }
        protected void WizardFinished(object sender, WizardNavigationEventArgs e) => entries.Add("Wizard finished from " + e.CurrentStepIndex + " text=" + txtStep.Text);
        protected void ImageClicked(object sender, ImageClickEventArgs e) => entries.Add("Image clicked");
        protected void LinkClicked(object sender, EventArgs e) => entries.Add("Link clicked");

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
