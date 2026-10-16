using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The navigation controls' scenario layer (scenarios/TreeView.json, Menu.json, SiteMapPath.json, SiteMapDataSource.json):
    /// the site map (Web.sitemap) as a path, a tree and a menu; a tree of static nodes posting back (EnableClientScript=false:
    /// expanding and selecting post back), its check boxes posted, a node populated on demand on the server and one by a
    /// client callback; trees and menus bound to XML; a menu's items clicked; ones made in code; a log of what the server saw.
    /// </summary>
    public partial class NavigationScenario : Page
    {
        readonly List<string> entries = new List<string>();
        TreeView dynamicTree;
        Menu dynamicMenu;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Made on every request, before the posted data is loaded.
            dynamicTree = new TreeView { ID = "tvDynamic", EnableClientScript = false, ShowCheckBoxes = TreeNodeTypes.Leaf };
            dynamicTree.SelectedNodeChanged += NodeSelected;
            dynamicTree.TreeNodeCheckChanged += NodeChecked;
            dynamicMenu = new Menu { ID = "mnuDynamic", StaticDisplayLevels = 2 };
            dynamicMenu.MenuItemClick += MenuClicked;
            var path = new SiteMapPath { ID = "smpDynamic", RenderCurrentNodeAsLink = true };
            var source = new SiteMapDataSource { ID = "smdsDynamic", ShowStartingNode = false };
            var sourced = new Menu { ID = "mnuDynamicSourced", DataSourceID = "smdsDynamic" };
            foreach (var control in new Control[] { dynamicTree, dynamicMenu, path, source, sourced }) Dynamic.Controls.Add(control);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack + " IsCallback=" + IsCallback);
            if (IsPostBack) return;
            // Nodes and items added in code: kept by view state.
            var node = new TreeNode("Dynamic node", "dn");
            node.ChildNodes.Add(new TreeNode("Dynamic leaf", "dl"));
            dynamicTree.Nodes.Add(node);
            var item = new MenuItem("Dynamic item", "di");
            item.ChildItems.Add(new MenuItem("Dynamic child", "dc"));
            dynamicMenu.Items.Add(item);
            tvHidden.Nodes.Add(new TreeNode("Added while hidden", "h"));
            mnuHidden.Items.Add(new MenuItem("Added while hidden", "h"));
        }

        protected void NodeSelected(object sender, EventArgs e)
        {
            var tree = (TreeView)sender;
            entries.Add("SelectedNodeChanged " + tree.ID + " " + tree.SelectedNode.ValuePath);
        }

        protected void NodeChecked(object sender, TreeNodeEventArgs e) => entries.Add("TreeNodeCheckChanged " + ((Control)sender).ID + " " + e.Node.ValuePath + " Checked=" + e.Node.Checked);

        protected void NodeExpanded(object sender, TreeNodeEventArgs e) => entries.Add("TreeNodeExpanded " + ((Control)sender).ID + " " + e.Node.ValuePath);

        protected void NodeCollapsed(object sender, TreeNodeEventArgs e) => entries.Add("TreeNodeCollapsed " + ((Control)sender).ID + " " + e.Node.ValuePath);

        protected void NodePopulate(object sender, TreeNodeEventArgs e)
        {
            // In a callback the log is not rendered: the nodes' text says what populated them.
            var how = IsCallback ? "callback" : "postback";
            e.Node.ChildNodes.Add(new TreeNode("Populated by " + how + " 1", "p1"));
            e.Node.ChildNodes.Add(new TreeNode("Populated by " + how + " 2", "p2"));
            entries.Add("TreeNodePopulate " + ((Control)sender).ID + " " + e.Node.ValuePath);
        }

        protected void NodeBound(object sender, TreeNodeEventArgs e) => entries.Add("TreeNodeDataBound " + e.Node.ValuePath);

        protected void MenuBound(object sender, MenuEventArgs e) => entries.Add("MenuItemDataBound " + e.Item.ValuePath);

        protected void MenuClicked(object sender, MenuEventArgs e) => entries.Add("MenuItemClick " + ((Control)sender).ID + " " + e.Item.ValuePath);

        protected void ShowHidden(object sender, EventArgs e)
        {
            tvHidden.Visible = true;
            mnuHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        static IEnumerable<TreeNode> All(TreeNodeCollection nodes) => nodes.Cast<TreeNode>().SelectMany(n => new[] { n }.Concat(All(n.ChildNodes)));

        static string Describe(TreeView tree) =>
            tree.ID + " Nodes=[" + string.Join(", ", All(tree.Nodes).Select(n => n.ValuePath + (n.Checked ? " checked" : "") + (n.Expanded == true ? " expanded" : "") + (n.Selected ? " selected" : ""))) + "]" + (tree.Visible ? "" : " (hidden)");

        static IEnumerable<MenuItem> All(MenuItemCollection items) => items.Cast<MenuItem>().SelectMany(i => new[] { i }.Concat(All(i.ChildItems)));

        static string Describe(Menu menu) =>
            menu.ID + " Items=[" + string.Join(", ", All(menu.Items).Select(i => i.ValuePath + (i.Selected ? " selected" : ""))) + "]" + (menu.Visible ? "" : " (hidden)");

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            foreach (var tree in new[] { tvSite, tvStatic, tvCallback, tvBound, tvHidden, dynamicTree }) entries.Add("State " + Describe(tree));
            foreach (var menu in new[] { mnuSite, mnuStatic, mnuBound, mnuTemplated, mnuHidden, dynamicMenu }) entries.Add("State " + Describe(menu));
            entries.Add("State current node: " + (SiteMap.CurrentNode == null ? "(none)" : SiteMap.CurrentNode.Title + " " + SiteMap.CurrentNode.Url));
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>Navigation scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p><p id=\"message\">" + HttpUtility.HtmlEncode(error.Message) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
