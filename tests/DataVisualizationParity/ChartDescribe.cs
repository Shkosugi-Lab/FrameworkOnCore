using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Reflection;
using FrameworkOnCore.DrawingParity;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DataVisualizationParity
{
    /// <summary>
    /// How the cases write a chart's element (a series, an axis, a legend...): its type and the values of its public
    /// properties of simple types (numbers, text, enums, colors, points and sizes), in name order, one line; an element it
    /// holds by its type and name, a collection by its count. Only the properties the chart's types declare: the ones
    /// they inherit from System.Web's controls (ClientID, Page...) are System.Web's, which its own tests cover, and
    /// outside a web application (as here) FrameworkOnCore's System.Web has no configuration to answer them from; nor the Chart's
    /// BuildNumber, its assembly's version (Api.Chart.BuildNumber shows it). Fonts are
    /// left out of that line: the fonts installed decide
    /// them (Linux has other ones); a member returning a font is written by Describe, marked font dependent. Other values
    /// (System.Drawing's, images) are written as the System.Drawing cases write them (Describe).
    /// </summary>
    public static class ChartDescribe
    {
        public static bool IsChartObject(object value) => value != null && !(value is Enum) && IsChartType(value.GetType());

        static bool IsChartType(Type type) => (type.Namespace ?? "").StartsWith("System.Web.UI.DataVisualization", StringComparison.Ordinal);

        public static void Record(Probe p, string label, object value, bool fontDependent)
        {
            if (IsChartObject(value)) p.Is((fontDependent ? Describe.FontDependent : "") + label, Text(value));
            else Describe.Record(p, label, value, fontDependent);
        }

        /// <summary>What the installed fonts decide: fonts, text measured.</summary>
        public static bool DependsOnFonts(MemberInfo member)
        {
            var type = member is PropertyInfo property ? property.PropertyType : member is MethodInfo method ? method.ReturnType : null;
            if (type == typeof(Font) || type == typeof(FontFamily)) return true;
            if (member.Name.Contains("Measure")) return true;
            var parameters = member is MethodBase methodBase ? methodBase.GetParameters() : member is PropertyInfo indexer ? indexer.GetIndexParameters() : new ParameterInfo[0];
            return parameters.Any(x => x.ParameterType == typeof(Font) || x.ParameterType == typeof(FontFamily));
        }

        /// <summary>A chart's element as one line.</summary>
        public static string Text(object value)
        {
            var type = value.GetType();
            if (value is IEnumerable sequence && !(value is string))
            {
                var items = sequence.Cast<object>().ToList();
                return type.Name + " Count=" + items.Count + " [" + string.Join(", ", items.Take(16).Select(Short)) + "]";
            }
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetIndexParameters().Length == 0 && x.GetGetMethod() != null && x.PropertyType != typeof(Font) && x.PropertyType != typeof(FontFamily) && IsChartType(x.DeclaringType) && x.Name != "BuildNumber")
                .OrderBy(x => x.Name, StringComparer.Ordinal);
            return type.Name + " {" + string.Join("; ", properties.Select(x => x.Name + "=" + PropertyText(x, value)).Where(x => x != null)) + "}";
        }

        static string PropertyText(PropertyInfo property, object target)
        {
            object value;
            try { value = property.GetValue(target, null); }
            catch (TargetInvocationException e) { return "!" + (e.InnerException ?? e).GetType().Name; }
            return Value(value);
        }

        /// <summary>A property's value inside an element's line: simple values in full, the rest by type (and name).</summary>
        static string Value(object value)
        {
            switch (value)
            {
                case null: return "null";
                case string s: return Probe.Format(s);
                case Enum e: return Probe.EnumName(e);
                case Color color: return Describe.Text(color);
                case float _:
                case double _:
                case decimal _:
                case bool _:
                case DateTime _:
                case Point _:
                case PointF _:
                case Size _:
                case SizeF _:
                case Rectangle _:
                case RectangleF _:
                    return Probe.Format(value);
                case double[] numbers: return Probe.Format(numbers);
                default:
                    if (value.GetType().IsPrimitive) return Probe.Format(value);
                    return Short(value);
            }
        }

        /// <summary>An element named: its type and name; a collection: its count; others: their type.</summary>
        static string Short(object value)
        {
            if (value == null) return "null";
            var type = value.GetType();
            if (!IsChartObject(value)) return type.IsValueType || value is string ? Probe.Format(value) : type.Name;
            if (value is IEnumerable sequence) return type.Name + "(" + sequence.Cast<object>().Count() + ")";
            var name = type.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance, null, typeof(string), Type.EmptyTypes, null);
            if (name == null) return type.Name;
            try { return type.Name + ":" + (string)name.GetValue(value, null); }
            catch (TargetInvocationException e) { return type.Name + ":!" + (e.InnerException ?? e).GetType().Name; }
        }
    }
}
