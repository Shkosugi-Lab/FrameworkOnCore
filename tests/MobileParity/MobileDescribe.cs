using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FrameworkOnCore.DrawingParity;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.MobileParity
{
    /// <summary>
    /// How the cases write an object of the mobile controls (a control, a style, a list item, an adapter...): its type and
    /// the values of its public properties of simple types (numbers, text, enums, colors), in name order, one line; an
    /// object of the API it holds by its type (and ID or name), a collection by its count. Only the properties the mobile
    /// types introduce: the ones they inherit or override from System.Web's controls (ClientID, Page, IdSeparator...) are
    /// System.Web's, which its own tests cover, and outside a web application (as here) FrameworkOnCore's System.Web has no
    /// configuration to answer them from (their own cases show it). A control's adapter outside a request is the designer's
    /// on .NET Framework (Visual Studio's, not in the port): written as such on both. Other values as the System.Drawing
    /// cases write them (Describe).
    /// </summary>
    public static class MobileDescribe
    {
        public static bool IsMobileType(Type type)
        {
            var ns = type.Namespace ?? "";
            return ns.StartsWith("System.Web.UI.MobileControls", StringComparison.Ordinal) || ns == "System.Web.Mobile"
                || ns.StartsWith("System.Web.UI.Design.MobileControls", StringComparison.Ordinal);
        }

        public static bool IsMobileObject(object value) => value != null && !(value is Enum) && IsMobileType(value.GetType());

        /// <summary>Visual Studio's designer (System.Web.UI.Design.MobileControls): not in the port.</summary>
        public static bool IsDesignerType(Type type) => (type.Namespace ?? "").StartsWith("System.Web.UI.Design.MobileControls", StringComparison.Ordinal);

        public static void Record(Probe p, string label, object value)
        {
            if (IsMobileObject(value)) p.Is(label, Text(value));
            else if (value is string text) p.Is(label, Unrandom(text));
            else Describe.Record(p, label, value, false);
        }

        /// <summary>
        /// Text with what the mobile controls make up at random (on .NET Framework as well, each run its own) written as its
        /// shape: a GUID (MultiPartWriter.NewUrl), the unique file path suffix (MobilePage.UniqueFilePathSuffix).
        /// </summary>
        public static string Unrandom(string text) =>
            Regex.Replace(Regex.Replace(text, @"[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}", "{guid}"), @"__ufps=\d{6}", "__ufps={6 digits}");

        /// <summary>An object of the API as one line.</summary>
        public static string Text(object value)
        {
            var type = value.GetType();
            if (value is IEnumerable sequence && !(value is string) && !HasOwnProperties(type))
            {
                var items = Items(sequence);
                return type.Name + " Count=" + (items == null ? "!" : items.Count.ToString(System.Globalization.CultureInfo.InvariantCulture)) +
                       (items == null ? "" : " [" + string.Join(", ", items.Take(16).Select(Short)) + "]");
            }
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetIndexParameters().Length == 0 && x.GetGetMethod() != null && IsMobileType(x.DeclaringType) && IsMobileType(x.GetGetMethod().GetBaseDefinition().DeclaringType))
                .OrderBy(x => x.Name, StringComparer.Ordinal);
            return type.Name + " {" + string.Join("; ", properties.Select(x => x.Name + "=" + PropertyText(x, value))) + "}";
        }

        // A collection that is also a control (a List's items are its own, a Form's controls System.Web's): its properties.
        static bool HasOwnProperties(Type type) =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(x => x.GetIndexParameters().Length == 0 && IsMobileType(x.DeclaringType) && x.Name != "Count" && x.Name != "IsReadOnly" && x.Name != "IsSynchronized" && x.Name != "SyncRoot");

        static System.Collections.Generic.List<object> Items(IEnumerable sequence)
        {
            try { return sequence.Cast<object>().ToList(); }
            catch (Exception) { return null; }
        }

        static string PropertyText(PropertyInfo property, object target)
        {
            object value;
            try { value = property.GetValue(target, null); }
            catch (TargetInvocationException e) { return "!" + (e.InnerException ?? e).GetType().Name; }
            if (property.Name == "Adapter" && value != null && (IsDesignerType(value.GetType()) || (value.GetType().Name == "EmptyControlAdapter" && InDesignMode(target))))
                return "(designer's)";
            return Value(value);
        }

        // Outside a request: the control's page (if any) in design mode.
        static bool InDesignMode(object target) =>
            !(target is System.Web.UI.Control control) || !(control.Page is System.Web.UI.MobileControls.MobilePage page) || page.DesignMode;

        /// <summary>A property's value inside an object's line: simple values in full, the rest by type (and name).</summary>
        static string Value(object value)
        {
            switch (value)
            {
                case null: return "null";
                case string s: return Probe.Format(Unrandom(s));
                case Enum e: return Probe.EnumName(e);
                case Color color: return Describe.Text(color);
                case float _:
                case double _:
                case decimal _:
                case bool _:
                case DateTime _:
                    return Probe.Format(value);
                default:
                    if (value.GetType().IsPrimitive) return Probe.Format(value);
                    return Short(value);
            }
        }

        /// <summary>An object of the API by its type and ID or name; a collection by its count; others by their type.</summary>
        static string Short(object value)
        {
            if (value == null) return "null";
            var type = value.GetType();
            if (!IsMobileObject(value)) return type.IsValueType || value is string ? Probe.Format(value) : type.Name;
            if (value is System.Web.UI.Control control) return type.Name + "#" + (control.ID ?? "");
            if (value is ICollection collection) return type.Name + "(" + collection.Count + ")";
            foreach (var key in new[] { "Name", "Text" })
            {
                var name = type.GetProperty(key, BindingFlags.Public | BindingFlags.Instance, null, typeof(string), Type.EmptyTypes, null);
                if (name == null) continue;
                try { return type.Name + ":" + (string)name.GetValue(value, null); }
                catch (TargetInvocationException e) { return type.Name + ":!" + (e.InnerException ?? e).GetType().Name; }
            }
            return type.Name;
        }
    }
}
