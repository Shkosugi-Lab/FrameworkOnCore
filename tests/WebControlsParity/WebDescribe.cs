using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using FrameworkOnCore.DrawingParity;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.WebControlsParity
{
    /// <summary>
    /// How the API cases write a value of System.Web: a control as its type and ID, and (its state, after a call) the
    /// values of its public properties of simple types (text, numbers, enums, units, colors), in name order, one line;
    /// a style likewise; a view state (StateBag) as its keys, values and dirtiness; attributes as their keys and values;
    /// a writer as the text written into it (a rendering, outside a request). Other values as the System.Drawing cases
    /// write them (Describe).
    /// </summary>
    public static class WebDescribe
    {
        /// <summary>A description written as it is (not quoted as a string value is).</summary>
        public sealed class Raw
        {
            public Raw(string text) => Text = text;
            public string Text { get; }
        }

        public static bool IsWebType(Type type) => type.Assembly == typeof(Control).Assembly || type.Assembly.GetName().Name == "System.Web.Extensions";

        public static void Record(Probe p, string label, object value)
        {
            switch (value)
            {
                case Control control: p.Is(label, new Raw(State(control))); return;
                case Style style: p.Is(label, new Raw(Properties(style))); return;
                case StateBag state: p.Is(label, new Raw(Text(state))); return;
                case System.Web.UI.AttributeCollection attributes: p.Is(label, new Raw(Text(attributes))); return;
                case HtmlTextWriter writer: p.Text(label, writer.InnerWriter.ToString()); return;
                case StringWriter writer: p.Text(label, writer.ToString()); return;
                case Unit unit: p.Is(label, new Raw("Unit " + unit.ToString(CultureInfo.InvariantCulture))); return;
                case FontUnit unit: p.Is(label, new Raw("FontUnit " + unit.ToString(CultureInfo.InvariantCulture))); return;
                case ControlCollection controls: p.Is(label, new Raw("ControlCollection [" + string.Join(", ", controls.Cast<Control>().Select(Short)) + "]")); return;
            }
            if (value != null && IsWebType(value.GetType()) && !(value is Enum) && !value.GetType().IsValueType && !(value is IEnumerable))
            {
                p.Is(label, new Raw(Properties(value)));
                return;
            }
            Describe.Record(p, label, value, false);
        }

        /// <summary>A control's state: its type and ID, its simple properties.</summary>
        public static string State(Control control) => Properties(control);

        static string Short(Control control) => control == null ? "null" : control.GetType().Name + "#" + (control.ID ?? "");

        static readonly string[] skipped =
        {
            // Objects, written by their own cases (the page, the parent, the site); the designer's.
            "Page", "Parent", "NamingContainer", "BindingContainer", "DataItemContainer", "DataKeysContainer", "Site", "TemplateControl",
            "AppRelativeTemplateSourceDirectory", "TemplateSourceDirectory",
        };

        /// <summary>An object of System.Web's public simple properties, in name order.</summary>
        public static string Properties(object value)
        {
            var type = value.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetIndexParameters().Length == 0 && x.GetGetMethod() != null && IsWebType(x.DeclaringType) && !skipped.Contains(x.Name) && Simple(x.PropertyType))
                .OrderBy(x => x.Name, StringComparer.Ordinal);
            var head = type.Name + (value is Control control ? "#" + (control.ID ?? "") : "");
            return head + " {" + string.Join("; ", properties.Select(x => x.Name + "=" + PropertyText(x, value))) + "}";
        }

        static bool Simple(Type type) =>
            type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(Unit) || type == typeof(FontUnit) || type == typeof(Color) || type == typeof(DateTime) || type == typeof(decimal);

        static string PropertyText(PropertyInfo property, object target)
        {
            object value;
            try { value = property.GetValue(target, null); }
            catch (TargetInvocationException e) { return "!" + (e.InnerException ?? e).GetType().Name; }
            return Value(value);
        }

        public static string Value(object value)
        {
            switch (value)
            {
                case null: return "null";
                case string s: return Probe.Format(s);
                case Enum e: return Probe.EnumName(e);
                case Color color: return Describe.Text(color);
                case Unit unit: return unit.ToString(CultureInfo.InvariantCulture);
                case FontUnit unit: return unit.ToString(CultureInfo.InvariantCulture);
                case Control control: return Short(control);
                default: return value.GetType().IsPrimitive || value is decimal || value is DateTime ? Probe.Format(value) : value.GetType().Name;
            }
        }

        static string Text(StateBag state)
        {
            var keys = state.Keys.Cast<string>().OrderBy(k => k, StringComparer.Ordinal);
            return "StateBag [" + string.Join(", ", keys.Select(k => k + "=" + Value(state[k]) + (state.IsItemDirty(k) ? "*" : ""))) + "]";
        }

        static string Text(System.Web.UI.AttributeCollection attributes)
        {
            var keys = attributes.Keys.Cast<string>().OrderBy(k => k, StringComparer.Ordinal);
            return "AttributeCollection [" + string.Join(", ", keys.Select(k => k + "=" + Probe.Format(attributes[k]))) + "]" +
                   (attributes.CssStyle.Count > 0 ? " style " + Probe.Format(attributes.CssStyle.Value) : "");
        }
    }
}
