using System;
using System.Web.UI;

namespace ChartProbe
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
            Sales.Series["Sales"].Points.DataBindXY(months, new[] { 120.5, 98.25, 143, 170.125, 155, 0 });
            Sales.Series["Orders"].Points.DataBindXY(months, new[] { 12, 9, 15, 17, 16, 2 });
            Saved.Series["Share"].Points.DataBindXY(new[] { "Web", "Store", "Phone" }, new[] { 55, 30, 15 });
        }
    }
}