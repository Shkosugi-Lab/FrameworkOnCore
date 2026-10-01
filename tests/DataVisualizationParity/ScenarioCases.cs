using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using FrameworkOnCore.DrawingParity;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DataVisualizationParity
{
    /// <summary>
    /// What applications do with the Chart control, end to end (the API cases call one member at a time, with sample
    /// arguments): every chart type drawn, nopCommerce's report pie, the label and legend keywords, data binding (a table,
    /// a cross table, arrays), the financial and statistical formulas, sorting, filtering and grouping, saving and loading
    /// a chart's settings (XML), the image map's HTML, 3D, annotations, the image formats. Pictures are written as the
    /// System.Drawing cases write them (Describe: exact on Windows, within a tolerance on Linux); text measured is font
    /// dependent.
    /// </summary>
    public static class ScenarioCases
    {
        static readonly double[] Values = { 3, 5, 2, 7, 4, 6, 8, 5 };

        static Chart NewChart(int width = 320, int height = 240)
        {
            var chart = new Chart { Width = width, Height = height, AntiAliasing = AntiAliasingStyles.All };
            chart.ChartAreas.Add(new ChartArea("Default"));
            chart.Legends.Add(new Legend("Default"));
            return chart;
        }

        /// <summary>The chart drawn (PNG, read back): its size and pixels.</summary>
        static void Drawn(Probe p, string label, Chart chart, bool fontDependent = true)
        {
            using (var stream = new MemoryStream())
            {
                chart.SaveImage(stream, ChartImageFormat.Png);
                // FOC_DATAVIZ_PICTURES: the pictures written to that folder too, to look at a difference.
                var folder = Environment.GetEnvironmentVariable("FOC_DATAVIZ_PICTURES");
                if (!string.IsNullOrEmpty(folder)) File.WriteAllBytes(Path.Combine(folder, string.Concat(label.Select(c => char.IsLetterOrDigit(c) ? c : '_')) + ".png"), stream.ToArray());
                stream.Position = 0;
                using (var picture = new Bitmap(stream)) Describe.Record(p, label, picture, fontDependent);
            }
        }

        static Series Filled(Series series, int yValues)
        {
            var random = new Random(7);   // the same values on both runtimes (a seeded Random's sequence is the same)
            for (var i = 0; i < 8; i++)
            {
                var y = new double[yValues];
                for (var j = 0; j < yValues; j++) y[j] = Values[(i + j) % Values.Length] + j * 1.5;
                if (yValues == 4) y = new[] { Values[i] + 4, Values[i] - 1, Values[i] + 1, Values[i] + 2 };   // high, low, open, close
                series.Points.AddXY(i + 1, y.Cast<object>().ToArray());
            }
            GC.KeepAlive(random);
            return series;
        }

        /// <summary>Every chart type, drawn from the same values (as many Y values as the type takes).</summary>
        [Case]
        public static void Chart_types(Probe p)
        {
            foreach (SeriesChartType type in Enum.GetValues(typeof(SeriesChartType)))
            {
                using (var chart = NewChart(160, 120))
                {
                    var series = new Series("S") { ChartType = type, ChartArea = "Default", Legend = "Default" };
                    var yValues = type == SeriesChartType.Stock || type == SeriesChartType.Candlestick ? 4
                        : type == SeriesChartType.Bubble || type == SeriesChartType.RangeBar || type == SeriesChartType.RangeColumn || type == SeriesChartType.Range || type == SeriesChartType.SplineRange || type == SeriesChartType.ErrorBar ? 2
                        : type == SeriesChartType.BoxPlot ? 6 : 1;
                    chart.Series.Add(Filled(series, yValues));
                    try { Drawn(p, type.ToString(), chart); }
                    catch (Exception e) { p.Is(type.ToString(), "-> " + Probe.Exception(e)); }
                }
            }
        }

        /// <summary>nopCommerce's reports (SalesReport, CustomerReports): a pie of points added in code, labelled #PERCENT{P1}.</summary>
        [Case]
        public static void Report_pie(Probe p)
        {
            using (var chart = new Chart { Width = 600, EnableViewState = true })
            {
                var area = new ChartArea("Default") { BorderColor = Color.FromArgb(64, 64, 64, 64), BackSecondaryColor = Color.Transparent, BackColor = Color.Transparent, ShadowColor = Color.Transparent, BackGradientStyle = GradientStyle.TopBottom };
                area.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
                area.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
                chart.ChartAreas.Add(area);
                chart.Legends.Add(new Legend("Default") { TitleFont = new Font("Microsoft Sans Serif", 8, FontStyle.Bold), BackColor = Color.Transparent, IsEquallySpacedItems = true, Font = new Font("Trebuchet MS", 8, FontStyle.Bold), IsTextAutoFit = false, Alignment = StringAlignment.Center });
                chart.Series.Add(new Series("Orders") { ChartArea = "Default", ChartType = SeriesChartType.Pie, Label = "#PERCENT{P1}", Legend = "Default" });
                foreach (var (name, total) in new[] { ("Laptop", 1250.5), ("Camera", 420.0), ("Book", 35.9) })
                {
                    var point = new DataPoint();
                    point.LegendText = name;
                    point.ToolTip = point.LegendText;
                    point.YValues = new[] { total };
                    chart.Series[0].Points.Add(point);
                }
                chart.DataBind();
                p.Is("points", chart.Series[0].Points.Select(x => x.LegendText + " " + Probe.Format(x.YValues[0])));
                Drawn(p, "pie", chart);
                p.Is("image map", chart.GetHtmlImageMap("map"));
            }
        }

        /// <summary>The keywords of labels, legend texts and tooltips (#VALY, #PERCENT, #TOTAL...), with formats, as the chart writes them in its image map.</summary>
        [Case]
        public static void Keywords(Probe p)
        {
            foreach (var culture in new[] { "en-US", "ja-JP", "de-DE" })
            {
                var saved = CultureInfo.CurrentCulture;
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                try
                {
                    using (var chart = NewChart())
                    {
                        var series = new Series("S") { ChartArea = "Default", Legend = "Default", ChartType = SeriesChartType.Column };
                        series.Points.AddXY("a", 1234.5);
                        series.Points.AddXY("b", 2.25);
                        series.Points.AddXY("c", 0.125);
                        foreach (var keyword in new[] { "#VALY", "#VALY{C}", "#VALY{N3}", "#VALX", "#AXISLABEL", "#PERCENT", "#PERCENT{P2}", "#TOTAL", "#AVG{0.00}", "#MAX", "#MIN", "#FIRST", "#LAST", "#INDEX", "#SERIESNAME", "#LEGENDTEXT", "#LABEL" })
                        {
                            series.ToolTip = keyword;
                            chart.Series.Clear();
                            chart.Series.Add(series);
                            Drawn(p, culture + " " + keyword + " drawn", chart);
                            p.Is(culture + " " + keyword, Titles(chart.GetHtmlImageMap("m")));
                        }
                    }
                }
                finally { CultureInfo.CurrentCulture = saved; }
            }
        }

        static IEnumerable<string> Titles(string html) =>
            System.Text.RegularExpressions.Regex.Matches(html, "title=\"([^\"]*)\"").Cast<System.Text.RegularExpressions.Match>().Select(m => m.Groups[1].Value);

        static DataTable Sales()
        {
            var table = new DataTable("Sales") { Locale = CultureInfo.InvariantCulture };
            table.Columns.Add("Month", typeof(string));
            table.Columns.Add("Region", typeof(string));
            table.Columns.Add("Amount", typeof(double));
            table.Columns.Add("Units", typeof(int));
            foreach (var (month, region, amount, units) in new[] { ("Jan", "North", 120.5, 3), ("Jan", "South", 80.0, 2), ("Feb", "North", 150.25, 4), ("Feb", "South", 95.75, 3), ("Mar", "North", 90.0, 2), ("Mar", "South", 130.0, 5) })
                table.Rows.Add(month, region, amount, units);
            return table;
        }

        static IEnumerable<string> Points(Chart chart) =>
            chart.Series.Select(s => s.Name + ": " + string.Join(" ", s.Points.Select(x => (x.AxisLabel.Length > 0 ? x.AxisLabel : Probe.Format(x.XValue)) + "=" + Probe.Format(x.YValues))));

        [Case]
        public static void Data_binding(Probe p)
        {
            using (var chart = NewChart())
            {
                chart.DataBindTable(Sales().DefaultView, "Month");
                p.Is("DataBindTable", Points(chart));
            }
            using (var chart = NewChart())
            {
                chart.DataBindCrossTable(Sales().DefaultView, "Region", "Month", "Amount", "Label=Units");
                p.Is("DataBindCrossTable", Points(chart));
                p.Is("DataBindCrossTable labels", chart.Series.SelectMany(s => s.Points.Select(x => x.Label)));
            }
            using (var chart = NewChart())
            {
                chart.DataSource = Sales();
                chart.Series.Add(new Series("A") { XValueMember = "Month", YValueMembers = "Amount" });
                chart.DataBind();
                p.Is("DataSource and members", Points(chart));
            }
            using (var chart = NewChart())
            {
                var series = new Series("S");
                series.Points.DataBindXY(new[] { "x", "y", "z" }, new[] { 1.5, 2.5, 3.5 });
                series.Points.DataBindY(new[] { 4.0, 5.0 }, new[] { 6.0, 7.0 });
                chart.Series.Add(series);
                p.Is("DataBindXY then DataBindY", Points(chart));
                var bound = new Series("B");
                bound.Points.DataBind(Sales().DefaultView, "Month", "Amount,Units", "Tooltip=Region");
                chart.Series.Add(bound);
                p.Is("Points.DataBind", Points(chart));
                p.Is("Points.DataBind tooltips", bound.Points.Select(x => x.ToolTip));
            }
        }

        static Chart Prices()
        {
            var chart = NewChart();
            var series = new Series("Price") { ChartType = SeriesChartType.Candlestick, YValuesPerPoint = 4 };
            double[] closes = { 10, 11, 10.5, 12, 13.5, 13, 14, 12.5, 13, 15, 16, 15.5, 17, 16.5, 18, 19, 18.5, 20, 21, 20.5 };
            for (var i = 0; i < closes.Length; i++)
                series.Points.AddXY(new DateTime(2020, 1, 1).AddDays(i), closes[i] + 1, closes[i] - 1, closes[i] - 0.25, closes[i]);
            chart.Series.Add(series);
            var volume = new Series("Volume");
            for (var i = 0; i < closes.Length; i++) volume.Points.AddXY(new DateTime(2020, 1, 1).AddDays(i), 100 + (i * 37) % 50);
            chart.Series.Add(volume);
            return chart;
        }

        /// <summary>Every financial formula, over the same prices: the values it writes.</summary>
        [Case]
        public static void Financial_formulas(Probe p)
        {
            foreach (FinancialFormula formula in Enum.GetValues(typeof(FinancialFormula)))
            {
                using (var chart = Prices())
                {
                    try
                    {
                        var input = formula == FinancialFormula.MedianPrice || formula == FinancialFormula.TypicalPrice || formula == FinancialFormula.WeightedClose
                            || formula == FinancialFormula.AverageTrueRange || formula == FinancialFormula.CommodityChannelIndex || formula == FinancialFormula.StochasticIndicator
                            || formula == FinancialFormula.WilliamsR || formula == FinancialFormula.MassIndex || formula == FinancialFormula.ChaikinOscillator
                            || formula == FinancialFormula.AccumulationDistribution || formula == FinancialFormula.EaseOfMovement || formula == FinancialFormula.MoneyFlow
                            || formula == FinancialFormula.VolatilityChaikins
                            ? "Price:Y,Price:Y2,Price:Y4" + (formula == FinancialFormula.AccumulationDistribution || formula == FinancialFormula.ChaikinOscillator || formula == FinancialFormula.EaseOfMovement || formula == FinancialFormula.MoneyFlow ? ",Volume" : "")
                            : formula == FinancialFormula.OnBalanceVolume || formula == FinancialFormula.NegativeVolumeIndex || formula == FinancialFormula.PositiveVolumeIndex || formula == FinancialFormula.PriceVolumeTrend
                                ? "Price:Y4,Volume" : "Price:Y4";
                        var output = formula == FinancialFormula.BollingerBands || formula == FinancialFormula.Envelopes || formula == FinancialFormula.StochasticIndicator || formula == FinancialFormula.MovingAverageConvergenceDivergence
                            ? "Out:Y,Out:Y2" : "Out";
                        chart.DataManipulator.FinancialFormula(formula, "3", input, output);
                        p.Is(formula.ToString(), chart.Series["Out"].Points.Select(x => Probe.Format(x.XValue) + "=" + Probe.Format(x.YValues)));
                    }
                    catch (Exception e) { p.Is(formula.ToString(), "-> " + Probe.Exception(e)); }
                }
            }
        }

        [Case]
        public static void Statistics(Probe p)
        {
            using (var chart = NewChart())
            {
                var a = new Series("A");
                var b = new Series("B");
                a.Points.DataBindY(new[] { 2.0, 4.5, 3.25, 6.0, 5.5, 7.0, 4.0 });
                b.Points.DataBindY(new[] { 3.0, 4.0, 5.5, 5.0, 6.5, 8.0, 7.5 });
                chart.Series.Add(a);
                chart.Series.Add(b);
                var s = chart.DataManipulator.Statistics;
                p.Try("Mean", () => s.Mean("A"));
                p.Try("Median", () => s.Median("A"));
                p.Try("Variance", () => s.Variance("A", true));
                p.Try("Covariance", () => s.Covariance("A", "B"));
                p.Try("Correlation", () => s.Correlation("A", "B"));
                p.Try("BetaFunction", () => s.BetaFunction(2.5, 1.5));
                p.Try("GammaFunction", () => s.GammaFunction(4.5));
                p.Try("NormalDistribution", () => s.NormalDistribution(0.75));
                p.Try("InverseNormalDistribution", () => s.InverseNormalDistribution(0.8));
                p.Try("TDistribution", () => s.TDistribution(1.5, 10, true));
                p.Try("InverseTDistribution", () => s.InverseTDistribution(0.1, 10));
                p.Try("FDistribution", () => s.FDistribution(1.5, 3, 10));
                p.Try("InverseFDistribution", () => s.InverseFDistribution(0.1, 3, 10));
                foreach (var (name, result) in new (string, Func<object>)[]
                {
                    ("TTestEqualVariances", () => s.TTestEqualVariances(0.5, 0.05, "A", "B")),
                    ("TTestUnequalVariances", () => s.TTestUnequalVariances(0.5, 0.05, "A", "B")),
                    ("TTestPaired", () => s.TTestPaired(0.5, 0.05, "A", "B")),
                    ("ZTest", () => s.ZTest(0.5, 1.2, 1.4, 0.05, "A", "B")),
                    ("FTest", () => s.FTest(0.05, "A", "B")),
                    ("Anova", () => s.Anova(0.05, "A,B")),
                })
                    p.Try(name, () => ChartDescribe.Text(result()));
            }
        }

        [Case]
        public static void Sort_filter_group(Probe p)
        {
            using (var chart = NewChart())
            {
                var series = new Series("S");
                foreach (var (label, y) in new[] { ("d", 4.0), ("a", 9.0), ("c", 1.0), ("b", 9.0), ("e", 5.5), ("f", 2.0) }) series.Points.AddXY(label, y);
                chart.Series.Add(series);
                chart.DataManipulator.Sort(PointSortOrder.Descending, "S");
                p.Is("Sort descending", Points(chart));
                chart.DataManipulator.Sort(PointSortOrder.Ascending, "AxisLabel", "S");
                p.Is("Sort by label", Points(chart));
                chart.DataManipulator.FilterTopN(3, "S", "S:Y", "Y");
                p.Is("FilterTopN", Points(chart));
                chart.DataManipulator.Filter(CompareMethod.LessThan, 5, "S");
                p.Is("Filter less than 5", Points(chart));
            }
            using (var chart = NewChart())
            {
                var series = new Series("S");
                foreach (var (label, y) in new[] { ("a", 4.0), ("bb", 9.0), ("c", 1.0), ("dd", 9.0) }) series.Points.AddXY(label, y);
                chart.Series.Add(series);
                var filter = new LongLabels();
                chart.DataManipulator.Filter(filter, "S", "Filtered");
                p.Is("Filter by an IDataPointFilter", Points(chart));
                p.Is("the filter was asked", filter.Asked);
                chart.DataManipulator.FilterMatchedPoints = false;
                chart.DataManipulator.Filter(filter, "S");
                p.Is("Filter keeping the matched points", Points(chart));
            }
            using (var chart = NewChart())
            {
                var series = new Series("S");
                for (var i = 0; i < 12; i++) series.Points.AddXY(new DateTime(2020, 1, 1).AddDays(i * 5), Values[i % Values.Length]);
                chart.Series.Add(series);
                chart.DataManipulator.Group("AVE", 1, IntervalType.Months, "S", "G");
                p.Is("Group average by month", Points(chart));
                chart.DataManipulator.Group("MAX,Y:SUM", 2, IntervalType.Weeks, "S", "W");
                p.Is("Group by two weeks", Points(chart));
                chart.DataManipulator.InsertEmptyPoints(1, IntervalType.Days, "S");
                p.Is("InsertEmptyPoints", series.Points.Count + " points, empty " + series.Points.Count(x => x.IsEmpty));
                p.Is("ExportSeriesValues", chart.DataManipulator.ExportSeriesValues("G").Tables.Cast<DataTable>().Select(t => t.TableName + " " + string.Join(";", t.Rows.Cast<DataRow>().Select(r => string.Join(",", r.ItemArray.Select(Probe.Format)))))) ;
            }
        }

        /// <summary>The application's filter (DataManipulator.Filter's IDataPointFilter): the points labelled with more than one letter.</summary>
        sealed class LongLabels : IDataPointFilter
        {
            public List<string> Asked { get; } = new List<string>();

            public bool FilterDataPoint(DataPoint point, Series series, int pointIndex)
            {
                Asked.Add(series.Name + "[" + pointIndex + "] " + point.AxisLabel);
                return point.AxisLabel.Length > 1;
            }
        }

        /// <summary>A chart's settings saved (XML) and loaded back into a new chart.</summary>
        [Case]
        public static void Serialization(Probe p)
        {
            string xml;
            using (var chart = Samples.SampleChart())
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                chart.Serializer.Content = SerializationContents.All;
                chart.Serializer.Save(writer);
                xml = writer.ToString();
                p.Text("saved", xml);
            }
            using (var loaded = new Chart())
            using (var reader = new StringReader(xml))
            {
                loaded.Serializer.Load(reader);
                p.Is("loaded", Points(loaded));
                p.Is("loaded areas", loaded.ChartAreas.Select(a => a.Name));
                using (var writer = new StringWriter(CultureInfo.InvariantCulture))
                {
                    loaded.Serializer.Save(writer);
                    p.Is("saved again equal", writer.ToString() == xml);
                }
            }
            using (var chart = Samples.SampleChart())
            using (var stream = new MemoryStream())
            {
                chart.Serializer.Format = System.Web.UI.DataVisualization.Charting.SerializationFormat.Binary;
                chart.Serializer.Save(stream);
                p.Is("binary", "bytes[" + stream.Length + "]");
            }
        }

        [Case]
        public static void Image_map(Probe p)
        {
            using (var chart = Samples.SampleChart())
            {
                chart.Series["S1"].ToolTip = "#VALX: #VALY";
                chart.Series["S1"].Url = "detail.aspx?x=#VALX";
                chart.Series["S1"].MapAreaAttributes = "onclick=\"go(#INDEX)\"";
                chart.Legends[0].CustomItems[0].ToolTip = "custom item";
                chart.Titles[0].ToolTip = "the title";
                p.Is("areas", chart.GetHtmlImageMap("map"));
            }
        }

        [Case]
        public static void Three_d_and_annotations(Probe p)
        {
            using (var chart = Samples.SampleChart())
            {
                chart.ChartAreas[0].Area3DStyle.Enable3D = true;
                chart.ChartAreas[0].Area3DStyle.Inclination = 30;
                chart.ChartAreas[0].Area3DStyle.Rotation = 20;
                Drawn(p, "3D", chart);
            }
            using (var chart = Samples.SampleChart())
            {
                chart.Annotations.Add(new ArrowAnnotation { X = 20, Y = 20, Width = 20, Height = 10 });
                chart.Annotations.Add(new RectangleAnnotation { X = 50, Y = 50, Width = 30, Height = 20, Text = "box" });
                chart.Annotations.Add(new CalloutAnnotation { AnchorDataPoint = chart.Series["S1"].Points[3], Text = "peak" });
                chart.Annotations.Add(new HorizontalLineAnnotation { AxisX = chart.ChartAreas[0].AxisX, AxisY = chart.ChartAreas[0].AxisY, AnchorY = 4, IsInfinitive = true, LineColor = Color.Red });
                Drawn(p, "annotations", chart);
                p.Is("callout", ChartDescribe.Text(chart.Annotations[chart.Annotations.Count - 2]));
            }
        }

        /// <summary>
        /// The numbers the chart writes (axis labels, keywords), as its FormatNumber event sees them: steps of 0.1 (the
        /// shortest round-trip text on .NET: 0.30000000000000004), an axis from negative zero, midpoints (0.125, 2.5),
        /// large and small values, the standard and custom formats.
        /// </summary>
        [Case]
        public static void Number_formats(Probe p)
        {
            foreach (var culture in new[] { "en-US", "ja-JP", "de-DE" })
            {
                var saved = CultureInfo.CurrentCulture;
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                try
                {
                    using (var chart = NewChart())
                    {
                        var written = new List<string>();
                        chart.FormatNumber += (sender, e) => written.Add(e.ElementType + " " + Probe.Format(e.Value) + " \"" + e.Format + "\" -> " + e.LocalizedValue);
                        var area = chart.ChartAreas[0];
                        area.AxisY.Minimum = -0.0;
                        area.AxisY.Maximum = 0.6;
                        area.AxisY.Interval = 0.1;
                        area.AxisX.LabelStyle.Format = "N2";
                        var series = new Series("S") { ChartArea = "Default", ChartType = SeriesChartType.Column, ToolTip = "#VALY{C}|#VALY{F0}|#VALY{G}|#VALY{E2}|#VALY{#,##0.0}|#VALY{P1}|#VALX{0.000}" };
                        foreach (var (x, y) in new[] { (0.125, 0.125), (2.5, 2.5), (1e-30, 0.3), (1e20, 0.1 + 0.2), (-0.0, 1.0 / 3) }) series.Points.AddXY(x, y);
                        chart.Series.Add(series);
                        Drawn(p, culture + " drawn", chart);
                        p.Is(culture + " FormatNumber", written.Distinct());
                        p.Is(culture + " tooltips", Titles(chart.GetHtmlImageMap("m")));
                    }
                }
                finally { CultureInfo.CurrentCulture = saved; }
            }
        }

        /// <summary>Every image format the chart writes: the format read back, and its size.</summary>
        [Case]
        public static void Image_formats(Probe p)
        {
            foreach (ChartImageFormat format in Enum.GetValues(typeof(ChartImageFormat)))
            {
                using (var chart = Samples.SampleChart())
                using (var stream = new MemoryStream())
                {
                    try
                    {
                        chart.SaveImage(stream, format);
                        stream.Position = 0;
                        if (format == ChartImageFormat.Emf || format == ChartImageFormat.EmfDual || format == ChartImageFormat.EmfPlus)
                            p.Is(format.ToString(), "bytes " + (stream.Length > 0));
                        else
                            using (var image = Image.FromStream(stream)) p.Is(format.ToString(), Describe.Image(image));
                    }
                    catch (Exception e) { p.Is(format.ToString(), "-> " + Probe.Exception(e)); }
                }
            }
        }
    }
}
