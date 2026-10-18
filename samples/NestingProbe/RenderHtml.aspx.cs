using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NestingProbe
{
    /// <summary>
    /// The HTML of a panel of controls, verbatim (parity-scenario.json): the page renders pnlLive into a string
    /// during Render and shows it encoded in a &lt;pre&gt;, so the comparison sees the markup itself - attribute
    /// order, the styles the controls expand, __doPostBack targets, auto-numbered IDs. The first request renders
    /// what DataBind built; the postback renders what ViewState rebuilt, which must be the same. The Calendar's
    /// dates are pinned (TodaysDate): nothing in the dump depends on the clock.
    /// </summary>
    public partial class RenderHtml : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            calS.TodaysDate = new DateTime(2026, 1, 15);
            if (!IsPostBack)
            {
                calS.VisibleDate = new DateTime(2026, 1, 1);
                calS.SelectedDate = new DateTime(2026, 1, 20);
                Bind(null);
            }
        }

        void Bind(string sort)
        {
            var categories = Store.Of(Session).Categories.AsEnumerable();
            if (sort == "Name") categories = categories.OrderBy(c => c.Name, StringComparer.Ordinal);
            gvS.DataSource = categories.ToList();
            gvS.DataBind();
            dlS.DataSource = Store.ProductsOf(Session, 4);
            dlS.DataBind();
            blS.DataSource = Store.ProductsOf(Session, 2).Select(p => p.Name).ToList();
            blS.DataBind();
        }

        protected void RowBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var inner = (GridView)e.Row.FindControl("gvSP");
            inner.DataSource = Store.ProductsOf(Session, (int)gvS.DataKeys[e.Row.RowIndex].Value);
            inner.DataBind();
        }

        protected void Sorted(object sender, GridViewSortEventArgs e)
        {
            Bind(e.SortExpression);
        }

        protected void BulletClicked(object sender, BulletedListEventArgs e)
        {
            // The click only proves the __doPostBack target in the dumped HTML works; nothing to change.
        }

        protected void Re(object sender, EventArgs e)
        {
        }

        const string Marker = "\u0001dump\u0001";

        // Rendered once: a second rendering of a validator registers its expando attributes again (an exception
        // for a browser that gets client script). The panel is cut out of the page's own output.
        protected override void Render(HtmlTextWriter writer)
        {
            HtmlOut.Text = Marker;
            var builder = new StringBuilder();
            using (var inner = new HtmlTextWriter(new StringWriter(builder)))
            {
                base.Render(inner);
            }
            var html = builder.ToString();
            var start = html.IndexOf("<div id=\"pnlLive\"", StringComparison.Ordinal);
            var end = html.IndexOf("<!--end-live-->", StringComparison.Ordinal);
            var panel = start >= 0 && end > start ? html.Substring(start, end - start) : "(panel not found)";
            writer.Write(html.Replace(Marker, HttpUtility.HtmlEncode(panel)));
        }
    }
}
