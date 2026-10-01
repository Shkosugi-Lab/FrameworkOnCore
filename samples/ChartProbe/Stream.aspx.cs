using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;

namespace ChartProbe
{
    /// <summary>A chart made in code and written as the response (a PNG), as reports' and dashboards' image pages do.</summary>
    public partial class Stream : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var chart = new Chart { Width = 320, Height = 200 };
            chart.ChartAreas.Add(new ChartArea("Area"));
            var series = chart.Series.Add("Balance");
            series.ChartType = SeriesChartType.Bar;
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "N2";
            series.Points.DataBindXY(new[] { "a", "b", "c" }, new[] { 3.5, -1.25, 2.125 });
            Response.Clear();
            Response.ContentType = "image/png";
            using (var buffer = new MemoryStream())
            {
                chart.SaveImage(buffer, ChartImageFormat.Png);
                buffer.WriteTo(Response.OutputStream);
            }
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}