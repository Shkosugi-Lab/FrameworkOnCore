using System;
using System.Web.UI;

namespace WebControlsProbe
{
    /// <summary>The master page of ContentScenario.aspx: its place holders, a label bound to its own data, a button of its own.</summary>
    public partial class Probe : MasterPage
    {
        protected string MasterText => "master bound";

        /// <summary>What the master saw, for the page's log.</summary>
        public string Clicked { get; private set; }

        protected void MasterClicked(object sender, EventArgs e)
        {
            Clicked = "Click btnMaster (in the master)";
        }
    }
}
