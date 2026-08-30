using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DefaultsProbe
{
    public partial class Default : System.Web.UI.Page
    {
        public class Item
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
        }

        // Static so the RowDeleting mutation survives postbacks (reset by app restart)
        private static readonly List<Item> _evtItems = new List<Item>
        {
            new Item { Name = "イ", Price = 10m },
            new Item { Name = "ロ", Price = 20m },
            new Item { Name = "ハ", Price = 30m },
        };

        // Referenced by the inline <% if %> block in markup
        protected bool ShowExtraSection
        {
            get { return true; }
        }

        // Referenced by <%= %> in markup (renders unencoded in WebForms)
        protected string RawHtmlFragment
        {
            get { return "<b>太字断片</b>"; }
        }

        // Drives the tag-crossing <% if %> in markup. False on purpose: that is the
        // branch where a wrong conversion is visible (the wrapper divs must NOT render)
        protected bool ShowWrapper
        {
            get { return false; }
        }

        // Model binding target (SelectMethod="GetModelItems")
        public List<ProbeItem> GetModelItems()
        {
            return new ProbeData().GetItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var items = new List<Item>
                {
                    new Item { Name = "甲", Price = 100m },
                    new Item { Name = "乙", Price = 2500m },
                };

                pGridAuto.DataSource = items;
                pGridAuto.DataBind();

                pGridColumns.DataSource = items;
                pGridColumns.DataBind();

                pFormView.DataSource = items;
                pFormView.DataBind();

                pGridStyled.DataSource = items;
                pGridStyled.DataBind();

                pGridFieldStyles.DataSource = items;
                pGridFieldStyles.DataBind();

                pDataListV.DataSource = items;
                pDataListV.DataBind();

                pDataListH.DataSource = items;
                pDataListH.DataBind();

                pAppend.DataSource = items;
                pAppend.DataBind();

                pDataListEvt.DataSource = items;
                pDataListEvt.DataBind();

                pDataListVA.DataSource = items;
                pDataListVA.DataBind();

                pDataGrid.DataSource = items;
                pDataGrid.DataBind();

                pDetails.DataSource = items;
                pDetails.DataBind();

                var table = new System.Data.DataTable();
                table.Columns.Add("項目");
                table.Columns.Add("値", typeof(int));
                table.Rows.Add("あ", 10);
                table.Rows.Add("い", 20);
                pGridDt.DataSource = table;
                pGridDt.DataBind();

                pGridPager.DataSource = new List<Item>
                {
                    new Item { Name = "頁一", Price = 1m },
                    new Item { Name = "頁二", Price = 2m },
                    new Item { Name = "頁三", Price = 3m },
                };
                pGridPager.DataBind();

                BindEvtGrid();
            }
        }

        private void BindEvtGrid()
        {
            pGridEvt.DataSource = _evtItems;
            pGridEvt.DataBind();
        }

        protected void pDataListEvt_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as Item;
                var hl = e.Item.FindControl("hlItem") as HyperLink;
                if (hl != null)
                {
                    hl.Text = item.Name + "特";
                    hl.NavigateUrl = "~/Default.aspx?item=" + item.Name;
                }
            }
        }

        protected void pGridEvt_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lbl = e.Row.FindControl("lblEvt") as Label;
                if (lbl != null)
                {
                    lbl.Text = "行" + e.Row.RowIndex.ToString();
                }
            }
        }

        protected void pGridEvt_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string name = (string)pGridEvt.DataKeys[e.RowIndex]["Name"];
            _evtItems.RemoveAll(x => x.Name == name);
            BindEvtGrid();
        }
    }
}
