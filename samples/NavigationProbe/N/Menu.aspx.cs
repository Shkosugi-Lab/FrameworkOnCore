using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.N
{
    public class MenuPage : NavigationProbe.ProbePage
    {
        protected System.Web.UI.WebControls.Menu menuItems;
        protected TreeView tvItems;
        protected Literal selection;

        protected void MenuClick(object sender, MenuEventArgs e) { L("MenuItemClick(" + e.Item.ValuePath + ")"); }
        protected void NodeChanged(object sender, EventArgs e) { L("SelectedNodeChanged(" + tvItems.SelectedNode.ValuePath + ")"); }
        protected void NodeExpanded(object sender, TreeNodeEventArgs e) { L("TreeNodeExpanded(" + e.Node.ValuePath + ")"); }
        protected void NodeCollapsed(object sender, TreeNodeEventArgs e) { L("TreeNodeCollapsed(" + e.Node.ValuePath + ")"); }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            selection.Text = "menu selected=" + (menuItems.SelectedValue ?? "") + "; tree selected=" + (tvItems.SelectedNode == null ? "(none)" : tvItems.SelectedNode.ValuePath);
        }
    }
}
