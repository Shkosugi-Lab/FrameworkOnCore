using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace LocaleProbe
{
    /// <summary>
    /// A form in the culture ?c= names (InitializeCulture): the Calendar in the culture's calendar (Buddhist, Hijri,
    /// Persian), the validators' client data (decimal and group characters, date order, the century), values typed
    /// as the culture writes them parsed on the server, bound values formatted, the month names. Dates pinned.
    /// </summary>
    public partial class Form : Page
    {
        public class Row
        {
            public DateTime When { get; set; }
            public decimal Amount { get; set; }
            public double Ratio { get; set; }
        }

        readonly List<string> lines = new List<string>();

        protected override void InitializeCulture()
        {
            var name = Request.QueryString["c"] ?? "en-US";
            Culture = name;
            UICulture = name;
            base.InitializeCulture();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cal.TodaysDate = new DateTime(2026, 3, 18);
            if (!IsPostBack)
            {
                cal.VisibleDate = new DateTime(2026, 3, 1);
                var format = CultureInfo.CurrentCulture.DateTimeFormat;
                ddlMonth.DataSource = format.MonthNames.Where(m => m.Length > 0).ToList();
                ddlMonth.DataBind();
                gv.DataSource = new[]
                {
                    new Row { When = new DateTime(2026, 3, 18, 9, 5, 0), Amount = 1234.5m, Ratio = 0.125 },
                    new Row { When = new DateTime(1999, 12, 31, 23, 59, 59), Amount = -0.5m, Ratio = -1 },
                };
                gv.DataBind();
            }
            var c = CultureInfo.CurrentCulture;
            lines.Add("Culture = " + c.Name + " calendar=" + c.Calendar.GetType().Name + " twoDigitYearMax=" + c.Calendar.TwoDigitYearMax);
            lines.Add("ShortDatePattern = " + c.DateTimeFormat.ShortDatePattern + " first=" + c.DateTimeFormat.FirstDayOfWeek);
            lines.Add("Decimal = " + c.NumberFormat.NumberDecimalSeparator + " group=" + c.NumberFormat.NumberGroupSeparator + " currency=" + c.NumberFormat.CurrencySymbol + " " + c.NumberFormat.CurrencyDecimalSeparator);
        }

        protected void DayChosen(object sender, EventArgs e) => lines.Add("Chosen = " + cal.SelectedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + " shown " + cal.SelectedDate.ToString("D"));

        protected void Submitted(object sender, EventArgs e)
        {
            lines.Add("IsValid = " + IsValid + " amount=" + valAmount.IsValid + " range=" + valRange.IsValid + " date=" + valDate.IsValid + " count=" + valCount.IsValid);
            lines.Add("Amount = " + Parse(() => decimal.Parse(txtAmount.Text, NumberStyles.Currency).ToString(CultureInfo.InvariantCulture)));
            lines.Add("Date = " + Parse(() => DateTime.Parse(txtDate.Text).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)));
            lines.Add("Count = " + Parse(() => int.Parse(txtCount.Text).ToString(CultureInfo.InvariantCulture)));
            lines.Add("Month = " + ddlMonth.SelectedValue + " #" + ddlMonth.SelectedIndex);
        }

        static string Parse(Func<string> parse)
        {
            try { return parse(); }
            catch (Exception ex) { return "!" + ex.GetType().Name; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Out.Text = HttpUtility.HtmlEncode(string.Join("\n", lines));
        }
    }
}