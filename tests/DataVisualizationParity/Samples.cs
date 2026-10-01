using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DataVisualizationParity
{
    /// <summary>
    /// The values the API cases call with. A sample chart (SampleChart: two series with points, a chart area, a legend, a
    /// title, an annotation, strip lines, custom labels, an image), fresh for each member: an instance of a type is the
    /// first element of that type found in it (its properties, breadth first), else one made by the type's constructor
    /// with sample arguments. Arguments by type, a few by parameter name (indexes, names of the chart's elements). The
    /// same values on .NET Framework and on the port, so what differs is the implementation.
    /// </summary>
    public sealed class Samples : ApiSamples
    {
        readonly List<IDisposable> made = new List<IDisposable>();
        readonly List<string> files = new List<string>();
        Chart chart;

        public override void Dispose()
        {
            for (var i = made.Count - 1; i >= 0; i--) { try { made[i].Dispose(); } catch { } }
            foreach (var file in files) { try { File.Delete(file); } catch { } }
        }

        T Keep<T>(T value)
        {
            if (value is IDisposable disposable) made.Add(disposable);
            return value;
        }

        public Chart Chart => chart ?? (chart = Keep(SampleChart()));

        /// <summary>The chart the members are called on: every kind of element, named as the name samples say.</summary>
        public static Chart SampleChart()
        {
            var chart = new Chart { ID = "chart1", Width = 300, Height = 200 };
            var area = new ChartArea("Default");
            chart.ChartAreas.Add(area);
            chart.Legends.Add(new Legend("Default"));
            chart.Titles.Add(new Title("Sales", Docking.Top) { Name = "Title1" });
            var columns = new Series("S1") { ChartType = SeriesChartType.Column, ChartArea = "Default", Legend = "Default" };
            foreach (var (x, y) in new[] { (1.0, 3.0), (2.0, 5.0), (3.0, 2.0), (4.0, 7.0), (5.0, 4.0) }) columns.Points.AddXY(x, y);
            columns.Points[1].Label = "#VALY";
            columns.Points[2].IsEmpty = true;
            chart.Series.Add(columns);
            var lines = new Series("S2") { ChartType = SeriesChartType.Line, ChartArea = "Default", Legend = "Default" };
            lines.Points.DataBindY(new[] { 2.0, 4.0, 3.0, 6.0, 5.0 });
            chart.Series.Add(lines);
            area.AxisX.StripLines.Add(new StripLine { IntervalOffset = 1, StripWidth = 0.5, BackColor = Color.LightGray });
            area.AxisX.CustomLabels.Add(0.5, 1.5, "one");
            chart.Legends[0].CustomItems.Add(Color.Red, "custom");
            chart.Annotations.Add(new TextAnnotation { Name = "note", Text = "Note", X = 10, Y = 10 });
            chart.Images.Add(new NamedImage("img", new Bitmap(4, 4)));
            return chart;
        }

        public static readonly Color Color1 = Color.FromArgb(255, 51, 102, 204);

        string Template()
        {
            var path = Path.Combine(Path.GetTempPath(), "foc-dataviz-template-" + Guid.NewGuid().ToString("N") + ".xml");
            using (var sample = SampleChart()) sample.SaveXml(path);
            files.Add(path);
            return path;
        }

        public override object Argument(ParameterInfo parameter, MemberInfo member)
        {
            var type = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
            var name = (parameter.Name ?? "").ToLowerInvariant();
            var owner = member.DeclaringType;
            if (parameter.IsOut) return type.IsValueType ? Activator.CreateInstance(type) : null;
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(float) || type == typeof(double) || type == typeof(byte) || type == typeof(decimal))
                return Convert.ChangeType(Number(name), type, CultureInfo.InvariantCulture);
            if (type == typeof(bool)) return true;
            if (type == typeof(char)) return ',';
            // A file of their own, made first (the sample chart as XML): a relative name ("Abc 123") was a file in the
            // working directory, that SaveXml wrote and LoadTemplate read, of an earlier run or none.
            if (type == typeof(string) && owner == typeof(Chart) && (member.Name == "SaveXml" || member.Name == "LoadTemplate")) return Template();
            if (type == typeof(string)) return Text(name, owner);
            if (type == typeof(object)) return name.Contains("source") || name.Contains("data") ? (object)new[] { 1.0, 2.0, 3.0 } : 2.0;
            if (type == typeof(DateTime)) return new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Unspecified);
            if (type == typeof(Color)) return Color1;
            if (type == typeof(Font)) return Keep(new Font("Arial", 9f));
            if (type == typeof(Point)) return new Point(4, 5);
            if (type == typeof(PointF)) return new PointF(4.5f, 5f);
            if (type == typeof(Size)) return new Size(20, 12);
            if (type == typeof(SizeF)) return new SizeF(20.5f, 12f);
            if (type == typeof(Rectangle)) return new Rectangle(4, 5, 20, 12);
            if (type == typeof(RectangleF)) return new RectangleF(4.5f, 5f, 20f, 12.5f);
            if (type == typeof(Type)) return typeof(Series);
            if (type == typeof(CultureInfo)) return CultureInfo.InvariantCulture;
            if (type == typeof(Image)) return Keep(new Bitmap(4, 4));
            if (type == typeof(Graphics)) { var bitmap = Keep(new Bitmap(32, 32)); return Keep(Graphics.FromImage(bitmap)); }
            if (type == typeof(Stream)) return Keep(new MemoryStream());
            if (type == typeof(TextWriter)) return Keep(new StringWriter(CultureInfo.InvariantCulture));
            if (type == typeof(TextReader)) return Keep(new StringReader("<Chart />"));
            if (type == typeof(HtmlTextWriter)) return Keep(new HtmlTextWriter(new StringWriter(CultureInfo.InvariantCulture)));
            if (type == typeof(EventArgs)) return EventArgs.Empty;
            if (type == typeof(IEnumerable) || type == typeof(ICollection) || type == typeof(IList)) return new[] { 1.0, 2.0, 3.0 };
            if (type == typeof(DataTable)) return Table();
            if (type.IsEnum) return EnumValue(type);
            if (type.IsArray) return Array(type.GetElementType());
            if (typeof(Delegate).IsAssignableFrom(type)) return Callback(type);
            if (IsChartType(type)) return New(type) ?? Instance(type);
            return Instance(type);
        }

        static object Number(string name)
        {
            if (name.Contains("index") || name == "i") return 0;
            if (name.Contains("count") || name.Contains("period")) return 2;
            if (name.Contains("width")) return 20;
            if (name.Contains("height")) return 12;
            if (name.Contains("angle")) return 30;
            if (name == "x" || name.StartsWith("left", StringComparison.Ordinal) || name.Contains("from") || name.Contains("min")) return 1;
            if (name == "y" || name.StartsWith("top", StringComparison.Ordinal)) return 2;
            if (name.Contains("to") || name.Contains("max")) return 4;
            return 3;
        }

        /// <summary>A name the chart has, where the parameter names one of its elements; a format, a keyword; else text.</summary>
        static string Text(string name, Type owner)
        {
            var ownerName = owner?.Name ?? "";
            if (name.Contains("series") || name.Contains("input") || name.Contains("output")) return "S1";
            if (name.Contains("area")) return "Default";
            if (name.Contains("legend")) return "Default";
            if (name == "name")
            {
                if (ownerName.Contains("Series")) return "S1";
                if (ownerName.Contains("ChartArea") || ownerName.Contains("Legend")) return "Default";
                if (ownerName.Contains("Title")) return "Title1";
                if (ownerName.Contains("Annotation")) return "note";
                if (ownerName.Contains("Image")) return "img";
            }
            if (name.Contains("format")) return "N2";
            if (name.Contains("field") || name.Contains("member")) return "Y";
            if (name.Contains("file") || name.Contains("path")) return Path.Combine(Path.GetTempPath(), "foc-dataviz-sample.png");
            if (name.Contains("parameter")) return "2";
            return "Abc 123";
        }

        static object EnumValue(Type type)
        {
            var values = Enum.GetValues(type);
            return values.GetValue(values.Length > 1 ? 1 : 0);
        }

        object Array(Type element)
        {
            if (element == typeof(double)) return new[] { 1.0, 2.0, 3.0 };
            if (element == typeof(float)) return new[] { 1f, 2f, 3f };
            if (element == typeof(int)) return new[] { 1, 2, 3 };
            if (element == typeof(string)) return new[] { "a", "b" };
            if (element == typeof(object)) return new object[] { 1.0, 2.0, 3.0 };
            if (element == typeof(PointF)) return new[] { new PointF(1, 1), new PointF(5, 3), new PointF(3, 6) };
            if (element == typeof(Point)) return new[] { new Point(1, 1), new Point(5, 3), new Point(3, 6) };
            if (element == typeof(Color)) return new[] { Color1, Color.Green };
            if (element == typeof(byte)) return new byte[] { 0, 1, 1 };
            var array = System.Array.CreateInstance(element, 1);
            array.SetValue(Argument(new SampleParameter(element), typeof(Samples)), 0);
            return array;
        }

        static object Callback(Type type)
        {
            var invoke = type.GetMethod("Invoke");
            var parameters = invoke.GetParameters().Select(x => System.Linq.Expressions.Expression.Parameter(x.ParameterType, x.Name)).ToArray();
            var body = invoke.ReturnType == typeof(void)
                ? (System.Linq.Expressions.Expression)System.Linq.Expressions.Expression.Empty()
                : System.Linq.Expressions.Expression.Default(invoke.ReturnType);
            return System.Linq.Expressions.Expression.Lambda(type, body, parameters).Compile();
        }

        static DataTable Table()
        {
            var table = new DataTable("Sales") { Locale = CultureInfo.InvariantCulture };
            table.Columns.Add("X", typeof(string));
            table.Columns.Add("Y", typeof(double));
            table.Columns.Add("Region", typeof(string));
            table.Rows.Add("a", 3.0, "north");
            table.Rows.Add("b", 5.0, "south");
            table.Rows.Add("c", 2.0, "north");
            return table;
        }

        static bool IsChartType(Type type) => (type.Namespace ?? "").StartsWith("System.Web.UI.DataVisualization", StringComparison.Ordinal);

        /// <summary>A fresh element of the type (a series to add, a point to insert): by its constructor, or null.</summary>
        object New(Type type)
        {
            // Constructors taking elements whose constructors take elements...: a few levels.
            if (type.IsAbstract || type.IsInterface || making > 3) return null;
            making++;
            try
            {
                foreach (var constructor in type.GetConstructors().OrderBy(c => c.GetParameters().Length))
                {
                    if (constructor.GetParameters().Any(x => x.ParameterType == type)) continue;
                    try { return Keep(constructor.Invoke(constructor.GetParameters().Select(x => Argument(x, constructor)).ToArray())); }
                    catch (TargetInvocationException) { }
                }
                return null;
            }
            finally { making--; }
        }

        int making;

        public override object Instance(Type type)
        {
            if (type == typeof(Chart)) return Chart;
            if (!IsChartType(type)) return type.IsAbstract || type.IsInterface ? null : New(type) ?? NonPublic(type);
            return Find(type) ?? New(type) ?? Concrete(type) ?? NonPublic(type);
        }

        /// <summary>The first element of the type in the sample chart, breadth first through its properties and collections.</summary>
        object Find(Type type)
        {
            var seen = new HashSet<object>(ReferenceComparer.Instance);
            var queue = new Queue<(object Value, int Depth)>();
            queue.Enqueue((Chart, 0));
            while (queue.Count > 0)
            {
                var (value, depth) = queue.Dequeue();
                if (value == null || !seen.Add(value)) continue;
                if (IsOfType(value, type)) return value;
                if (depth >= 4 || !IsChartType(value.GetType())) continue;
                if (value is IEnumerable sequence)
                    foreach (var item in sequence.Cast<object>().Take(4)) queue.Enqueue((item, depth + 1));
                foreach (var property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.Name, StringComparer.Ordinal))
                {
                    if (property.GetIndexParameters().Length > 0 || property.PropertyType.IsValueType || property.PropertyType == typeof(string)) continue;
                    try { queue.Enqueue((property.GetValue(value, null), depth + 1)); } catch (TargetInvocationException) { }
                }
            }
            return null;
        }

        /// <summary>Whether the value is of the type, or (a generic type's definition) of a type closing it.</summary>
        static bool IsOfType(object value, Type type)
        {
            if (!type.IsGenericTypeDefinition) return type.IsInstanceOfType(value);
            for (var t = value.GetType(); t != null; t = t.BaseType)
                if (t.IsGenericType && t.GetGenericTypeDefinition() == type) return true;
            return false;
        }

        /// <summary>An abstract type's first concrete type of the assembly, found or made.</summary>
        object Concrete(Type type)
        {
            if (!type.IsAbstract) return null;
            foreach (var derived in type.Assembly.GetExportedTypes().Where(t => type.IsAssignableFrom(t) && !t.IsAbstract).OrderBy(t => t.Name, StringComparer.Ordinal))
            {
                var instance = Find(derived) ?? New(derived);
                if (instance != null) return instance;
            }
            return null;
        }

        /// <summary>A type whose constructors are internal (event arguments): its first one, with sample arguments.</summary>
        object NonPublic(Type type)
        {
            if (type.IsAbstract || type.IsInterface) return null;
            foreach (var constructor in type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(c => c.GetParameters().Length))
            {
                try { return Keep(constructor.Invoke(constructor.GetParameters().Select(x => Argument(x, constructor)).ToArray())); }
                catch (TargetInvocationException) { }
                catch (ArgumentException) { }
            }
            return null;
        }

        sealed class ReferenceComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceComparer Instance = new ReferenceComparer();
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }

        /// <summary>A parameter of a type (an array's element, made as an argument would be).</summary>
        sealed class SampleParameter : ParameterInfo
        {
            public SampleParameter(Type type) { ClassImpl = type; NameImpl = "item"; }
        }
    }
}
