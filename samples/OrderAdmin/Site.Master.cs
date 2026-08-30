using System;
using System.Configuration;
using System.Web;
using System.Web.UI;

namespace OrderAdmin
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            litSiteTitle.Text = ConfigurationManager.AppSettings["SiteTitle"];
            lblFooter.Text = "© 2026 OrderAdmin";
        }
    }
}
