using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NestingProbe
{
    /// <summary>
    /// Templates three levels deep (parity-scenario.json): a Repeater of categories, a DataList of their products,
    /// a Repeater of tags with buttons. A command stops at the level that handles it (the Repeater and the DataList
    /// raise ItemCommand and swallow the bubble): the log shows which handlers ran. A GridView's rows post back on
    /// their own (an AutoPostBack DropDownList, a CheckBox, a TextBox whose TextChanged rides other postbacks); the
    /// row is found through NamingContainer. Two templates are made in code (an ITemplate class, LoadTemplate of an
    /// .ascx that compiles at run time): their items are recreated from ViewState on a postback that does not rebind,
    /// and the text boxes the ITemplate made keep what was posted.
    /// </summary>
    public partial class DeepTemplates : Page
    {
        readonly List<string> entries = new List<string>();

        sealed class DynTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                var literal = new Literal();
                literal.DataBinding += (sender, e) =>
                {
                    var l = (Literal)sender;
                    var item = (RepeaterItem)l.NamingContainer;
                    l.Text = "<span class=\"dyn\">dyn " + item.ItemIndex + ": " + HttpUtility.HtmlEncode((string)item.DataItem) + "</span> ";
                };
                container.Controls.Add(literal);
                container.Controls.Add(new TextBox { ID = "txtDyn" });
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Templates are set on every request, before postback data and ViewState: the items are recreated with them.
            rptDyn.ItemTemplate = new DynTemplate();
            rptLoaded.ItemTemplate = LoadTemplate("InnerTemplate.ascx");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            entries.Add("Load IsPostBack=" + IsPostBack);
            if (!IsPostBack)
            {
                rptA.DataSource = Store.Of(Session).Categories.Take(2).ToList();
                rptA.DataBind();
                gvAuto.DataSource = Store.ProductsOf(Session, 1);
                gvAuto.DataBind();
                rptDyn.DataSource = new[] { "one", "二", "three" };
                rptDyn.DataBind();
                rptLoaded.DataSource = Store.ProductsOf(Session, 2).Select(p => p.Name).ToList();
                rptLoaded.DataBind();
            }
        }

        protected void ABound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            var category = (Category)e.Item.DataItem;
            var inner = (DataList)e.Item.FindControl("dlB");
            inner.DataSource = Store.ProductsOf(Session, category.Id);
            inner.DataBind();
        }

        protected void BBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            var product = (Product)e.Item.DataItem;
            var inner = (Repeater)e.Item.FindControl("rptC");
            inner.DataSource = new[] { product.Id + ":a", product.Id + ":b" };
            inner.DataBind();
        }

        protected void ACommand(object source, RepeaterCommandEventArgs e)
            => entries.Add("A ItemCommand " + e.CommandName + " arg=" + e.CommandArgument + " item=" + e.Item.ItemIndex
                + " source=" + ((Control)e.CommandSource).UniqueID);

        protected void BCommand(object source, DataListCommandEventArgs e)
            => entries.Add("B ItemCommand " + e.CommandName + " arg=" + e.CommandArgument + " item=" + e.Item.ItemIndex
                + " source=" + ((Control)e.CommandSource).UniqueID);

        protected void CCommand(object source, RepeaterCommandEventArgs e)
            => entries.Add("C ItemCommand " + e.CommandName + " arg=" + e.CommandArgument + " item=" + e.Item.ItemIndex
                + " source=" + ((Control)e.CommandSource).UniqueID);

        protected void OptionChanged(object sender, EventArgs e)
        {
            var list = (DropDownList)sender;
            var row = (GridViewRow)list.NamingContainer;
            entries.Add("OptionChanged row=" + row.RowIndex + " key=" + gvAuto.DataKeys[row.RowIndex].Value + " value=" + list.SelectedValue);
        }

        protected void FlagChanged(object sender, EventArgs e)
        {
            var box = (CheckBox)sender;
            var row = (GridViewRow)box.NamingContainer;
            entries.Add("FlagChanged row=" + row.RowIndex + " checked=" + box.Checked);
        }

        protected void NoteChanged(object sender, EventArgs e)
        {
            var text = (TextBox)sender;
            var row = (GridViewRow)text.NamingContainer;
            entries.Add("NoteChanged row=" + row.RowIndex + " text=" + text.Text);
        }

        protected void DynPost(object sender, EventArgs e)
        {
            entries.Add("DynPost");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            entries.Add("rptDyn items=" + rptDyn.Items.Count + " texts: " + string.Join("|",
                rptDyn.Items.Cast<RepeaterItem>().Select(item => ((TextBox)item.FindControl("txtDyn")).Text)));
            entries.Add("rptLoaded items=" + rptLoaded.Items.Count);
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
