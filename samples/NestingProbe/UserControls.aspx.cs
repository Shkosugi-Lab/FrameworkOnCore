using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NestingProbe
{
    /// <summary>
    /// User controls made in code (parity-scenario.json): the page recreates as many ChildPanels as its ViewState
    /// counted, during Load - their text boxes get the posted values through the second postback-data pass, and
    /// their ViewState catches up as they are added. "Add child" adds one more in the same request. Commands bubble
    /// from the grandchild through both containers to the page's OnBubbleEvent. The static ChildPanel has
    /// EnableViewState=false: its grandchild's click count does not survive the next postback.
    /// </summary>
    public partial class UserControls : Page
    {
        readonly List<string> entries = new List<string>();

        int Count
        {
            get { return (int?)ViewState["n"] ?? 1; }
            set { ViewState["n"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            entries.Add("Load IsPostBack=" + IsPostBack + " recreating " + Count);
            for (var i = 0; i < Count; i++) AddChild(i);
        }

        void AddChild(int index)
        {
            var child = (ChildPanel)LoadControl("ChildPanel.ascx");
            child.ID = "child" + index;
            child.Title = "Child " + index;
            ph.Controls.Add(child);
        }

        protected void Add(object sender, EventArgs e)
        {
            Count = Count + 1;
            AddChild(Count - 1);
            entries.Add("Add -> " + Count);
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            var command = args as CommandEventArgs;
            if (command != null)
            {
                entries.Add("Bubble '" + command.CommandName + "' arg='" + command.CommandArgument
                    + "' from " + ((Control)source).UniqueID);
            }
            return true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            foreach (var child in ph.Controls.OfType<ChildPanel>())
            {
                entries.Add(child.ID + " entered='" + child.Entered + "'");
            }
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
