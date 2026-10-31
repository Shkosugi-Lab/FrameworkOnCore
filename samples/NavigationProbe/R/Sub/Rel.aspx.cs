using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.R.Sub
{
    public class Rel : NavigationProbe.ProbePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (Request.QueryString["c"])
            {
                case "parent": Response.Redirect("../Landing.aspx?from=parent"); break;
                case "file": Response.Redirect("Landing.aspx?from=file"); break;
                case "dot": Response.Redirect("./Landing.aspx?from=dot"); break;
                case "tilde": Response.Redirect("~/R/Landing.aspx?from=tilde"); break;
                case "query": Response.Redirect("?from=query-only"); break;
                case "frag": Response.Redirect("#frag"); break;
                case "query-frag": Response.Redirect("Landing.aspx?from=query-frag#section"); break;
                case "tilde-frag": Response.Redirect("~/R/Landing.aspx#top"); break;
                case "parent-parent": Response.Redirect("../../Default.aspx?from=parent-parent"); break;
            }
        }
    }
}
