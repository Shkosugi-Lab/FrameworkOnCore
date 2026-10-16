using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The Repeater's scenario layer (scenarios/Repeater.json): a repeater with every template whose items post (a text box)
    /// and command (a link, a button), nested repeaters, one bound by an ObjectDataSource, an empty one, one without view
    /// state, a hidden one, a dynamic one; and a log of what the server saw (the request's number, each ItemCreated and
    /// ItemDataBound counted, each ItemCommand with its item, every repeater's items and their text boxes' values).
    /// </summary>
    public partial class RepeaterScenario : Page
    {
        readonly List<string> entries = new List<string>();
        Repeater dynamicRepeater;
        int created;
        int bound;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dynamicRepeater = new Repeater { ID = "rptDynamic", ItemTemplate = new ProbeTemplate("dynamic") };
            Dynamic.Controls.Add(dynamicRepeater);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack + " (items created from view state: " + created + ")");
            if (IsPostBack) return;
            ProductStore.Reset();
            BindItems();
            rptOuter.DataSource = new Dictionary<string, string[]> { { "Fruit", new[] { "Apple", "Banana" } }, { "Tool", new[] { "Hammer" } } };
            rptOuter.DataBind();
            rptEmpty.DataSource = new List<ProbeData.Product>();
            rptEmpty.DataBind();
            foreach (var repeater in new[] { rptNoViewState, rptHidden, dynamicRepeater })
            {
                repeater.DataSource = ProbeData.Many().Take(2).ToList();
                repeater.DataBind();
            }
        }

        void BindItems()
        {
            rptItems.DataSource = ProductStore.Items.Take(4).ToList();
            rptItems.DataBind();
            entries.Add("rptItems bound: ItemCreated " + created + ", ItemDataBound " + bound);
        }

        protected void ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            created++;
        }

        protected void ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            bound++;
        }

        protected void OuterItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            var inner = (Repeater)e.Item.FindControl("rptInner");
            inner.DataSource = ((KeyValuePair<string, string[]>)e.Item.DataItem).Value;
            inner.DataBind();
        }

        protected void ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            entries.Add("ItemCommand " + ((Control)source).ID + " item " + e.Item.ItemIndex + " (" + e.Item.ItemType + "): " + e.CommandName + " " + e.CommandArgument +
                        " from " + ((Control)e.CommandSource).ID);
            if (e.CommandName == "remove")
            {
                ProductStore.Items.RemoveAll(p => p.Code == (string)e.CommandArgument);
                BindItems();
            }
        }

        protected void Rebind(object sender, EventArgs e)
        {
            entries.Add("Click btnRebind");
            BindItems();
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            rptHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            foreach (var repeater in new[] { rptItems, rptOuter, rptAuto, rptEmpty, rptNoViewState, rptHidden, dynamicRepeater })
                entries.Add("Repeater " + repeater.ID + ": Items=" + repeater.Items.Count + (repeater.Visible ? "" : " (hidden)"));
            foreach (RepeaterItem item in rptItems.Items)
                entries.Add("Item " + item.ItemIndex + " " + item.ItemType + ": " + ((Label)item.FindControl("lblName")).Text + " qty \"" + ((TextBox)item.FindControl("txtQty")).Text + "\"");
            entries.Add("Store: " + ProductStore.Describe());
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>Repeater scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
