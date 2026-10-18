using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PageProbe
{
    /// <summary>
    /// ClientIDMode in every form (the page AutoID; Static, Predictable with ClientIDRowSuffix on a GridView and a
    /// ListView of two fields, Inherit, an AutoID control in a Predictable row), the IDs a label's for, a validator's
    /// script and the summary use; the page lists every control's ClientID and UniqueID as the server sees them.
    /// </summary>
    public partial class ClientIds : Page
    {
        public class Row
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            var rows = new List<Row> { new Row { Code = "A1", Name = "alpha" }, new Row { Code = "B-2", Name = "beta gamma" } };
            rptPredictable.DataSource = rows;
            gvRows.DataSource = rows;
            lvItems.DataSource = rows;
            DataBind();
        }

        protected void Skipped(object sender, EventArgs e) { Journal.Add("Skip"); }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var lines = new List<string>();
            Walk(Master, lines);
            litIds.Text = HttpUtility.HtmlEncode(string.Join(" | ", lines));
        }

        static void Walk(Control control, List<string> lines)
        {
            if (!string.IsNullOrEmpty(control.ID) && !(control is LiteralControl))
                lines.Add(control.ID + ":" + control.ClientIDMode + "=" + control.ClientID + "/" + control.UniqueID);
            foreach (Control child in control.Controls) Walk(child, lines);
        }
    }
}
