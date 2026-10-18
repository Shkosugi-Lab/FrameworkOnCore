using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BclProbe
{
    /// <summary>
    /// Doubles, floats and dates as the controls show them (AutoGenerateColumns, BoundField with and without a format,
    /// Eval, Bind into a TextBox, a DropDownList's text and value, a Label from ViewState), from objects and from a
    /// DataTable; on a postback the TextBoxes' values are parsed back and the ViewState's values written again.
    /// </summary>
    public partial class Bound : Page
    {
        public class Row
        {
            public string Name { get; set; }
            public double Ratio { get; set; }
            public decimal Amount { get; set; }
            public DateTime When { get; set; }
            public float Weight { get; set; }
            public double? Missing { get; set; }
            public bool Flag { get; set; }
        }

        static List<Row> Rows() => new List<Row>
        {
            new Row { Name = "sum", Ratio = 0.1 + 0.2, Amount = 1.10m, When = new DateTime(2026, 3, 4, 13, 5, 9), Weight = 0.1f + 0.2f, Flag = true },
            new Row { Name = "third", Ratio = 1.0 / 3, Amount = -0.004m, When = new DateTime(2026, 3, 4), Weight = 1f / 3, Missing = 2.5 },
            new Row { Name = "neg zero", Ratio = -0.0, Amount = 0m, When = DateTime.MinValue, Weight = -0.0f },
            new Row { Name = "tiny neg", Ratio = -0.0001, Amount = 1234567.891m, When = new DateTime(2026, 12, 31, 23, 59, 59, 999), Weight = 16777217 },
            new Row { Name = "big", Ratio = 1e21, Amount = decimal.MaxValue, When = new DateTime(2026, 1, 1), Weight = float.MaxValue, Missing = double.NaN },
        };

        readonly List<string> entries = new List<string>();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                var table = new DataTable();
                table.Columns.Add("Double", typeof(double));
                table.Columns.Add("Single", typeof(float));
                table.Columns.Add("Decimal", typeof(decimal));
                table.Columns.Add("Date", typeof(DateTime));
                table.Columns.Add("Computed", typeof(double), "Double * 3");
                table.Rows.Add(0.1 + 0.2, 0.1f + 0.2f, 1.10m, new DateTime(2026, 3, 4, 13, 5, 9));
                table.Rows.Add(1.0 / 3, 1f / 3, -0.5m, new DateTime(2026, 3, 4));
                table.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
                gv.DataSource = table;
                gv.DataBind();
                gvFormatted.DataSource = Rows();
                gvFormatted.DataBind();
                ddl.DataSource = Rows();
                ddl.DataTextField = "Ratio";
                ddl.DataValueField = "Weight";
                ddl.DataTextFormatString = "{0}";
                ddl.DataBind();
                ViewState["d"] = 0.1 + 0.2;
                ViewState["f"] = 1f / 3;
                ViewState["m"] = 1.10m;
                ViewState["t"] = new DateTime(2026, 3, 4, 13, 5, 9, DateTimeKind.Local);
            }
        }

        protected void Post(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvFormatted.Rows)
            {
                var text = ((TextBox)row.FindControl("txtRatio")).Text;
                double value;
                var ok = double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
                entries.Add("row " + row.RowIndex + " '" + text + "' -> " + ok + " " + value + " same=" + (ok && value == Rows()[row.RowIndex].Ratio));
            }
            entries.Add("selected " + ddl.SelectedItem.Text + " / " + ddl.SelectedValue);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            lblState.Text = "ViewState: " + ViewState["d"] + " " + ViewState["f"] + " " + ViewState["m"] + " " + ViewState["t"]
                + " " + ((DateTime)ViewState["t"]).Kind;
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
