using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.WebControlsParity
{
    /// <summary>
    /// The values the API cases call with, fresh for each member. The component (Component.Make: TextBox txtName, "Taro")
    /// in a sample page (Page > HtmlForm form1 > Label lblTitle, the component; event validation off), no request: an instance of a type the
    /// component is (TextBox, WebControl, Control, IPostBackDataHandler...) is the component; of another type, the first
    /// object of that type in the page (its controls and their properties, breadth first), else one made by the type's
    /// constructor with sample arguments. Arguments by type, a few by parameter name (an ID names a control of the page;
    /// posted data names the component). The same values on .NET Framework and on the port, so what differs is the
    /// implementation.
    /// </summary>
    public sealed class Samples : ApiSamples
    {
        readonly Component component;
        readonly List<IDisposable> made = new List<IDisposable>();
        Page page;
        Control instance;

        public Samples(Component component) => this.component = component;

        public override void Dispose()
        {
            for (var i = made.Count - 1; i >= 0; i--) { try { made[i].Dispose(); } catch { } }
        }

        T Keep<T>(T value)
        {
            if (value is IDisposable disposable) made.Add(disposable);
            return value;
        }

        public Page Page
        {
            get
            {
                if (page != null) return page;
                // Event validation needs a request (the rendering's registrations, the posted values' check): off, so
                // that rendering and posted data run here; in a request (the rendering and scenario layers) it is on.
                page = new Page { EnableEventValidation = false };
                instance = component.Make();
                // A form or a head is the page's own (a page has one of each).
                var form = instance as HtmlForm ?? new HtmlForm { ID = "form1" };
                if (instance is HtmlHead) Add(page, instance);
                Add(page, form);
                // Beside the component, not in it: a member given a control (SetOwnerControl) given its own child loops.
                Add(instance == form ? (Control)page : form, new Label { ID = "lblTitle", Text = "Title" });
                if (instance != form && !(instance is HtmlHead)) Add(form, component.Wrap == null ? instance : component.Wrap(instance));
                if (component.Siblings != null)
                    foreach (var sibling in component.Siblings()) Add(form, sibling);
                return page;
            }
        }

        public Control Instance()
        {
            if (instance == null) { var _ = Page; }
            return instance;
        }

        // Each part separately: one that fails (as outside a request a control may) leaves the rest of the page.
        static void Add(Control parent, Control child)
        {
            try { parent.Controls.Add(child); } catch (Exception) { }
        }

        public static readonly Color Color1 = Color.FromArgb(255, 51, 102, 204);

        public override object Argument(ParameterInfo parameter, MemberInfo member)
        {
            var type = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
            var name = (parameter.Name ?? "").ToLowerInvariant();
            if (parameter.IsOut) return type.IsValueType ? Activator.CreateInstance(type) : null;
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(float) || type == typeof(double) || type == typeof(byte) || type == typeof(decimal))
                return Convert.ChangeType(Number(name), type, CultureInfo.InvariantCulture);
            if (type == typeof(IntPtr)) return IntPtr.Zero;
            if (type == typeof(bool)) return true;
            if (type == typeof(char)) return ',';
            if (type == typeof(string)) return name == "postdatakey" || name == "uniqueid" ? Instance().UniqueID ?? Instance().ID : Text(name);
            if (type == typeof(object)) return name.Contains("source") || name.Contains("data") ? (object)new[] { "Pen", "Bag" } : "Abc 123";
            if (type == typeof(DateTime)) return new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Unspecified);
            if (type == typeof(Color)) return Color1;
            if (type == typeof(Unit)) return Unit.Pixel(120);
            if (type == typeof(FontUnit)) return FontUnit.Point(12);
            if (type == typeof(Type)) return typeof(Label);
            if (type == typeof(CultureInfo) || type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            if (type == typeof(TextWriter)) return Keep(new StringWriter(CultureInfo.InvariantCulture));
            if (type == typeof(HtmlTextWriter)) return Keep(new HtmlTextWriter(new StringWriter(CultureInfo.InvariantCulture)));
            if (type == typeof(EventArgs)) return EventArgs.Empty;
            // Posted data names the component: a text box's text, a list's item value (Large).
            if (type == typeof(NameValueCollection)) return new NameValueCollection { { Instance().UniqueID ?? Instance().ID, Instance() is ListControl ? "L" : "Hanako" }, { "__EVENTTARGET", "" } };
            if (type == typeof(IDictionary) || type == typeof(Hashtable)) return new Hashtable { { "a", "1" } };
            if (type == typeof(IEnumerable) || type == typeof(ICollection) || type == typeof(IList)) return new[] { "Pen", "Bag" };
            if (type == typeof(Control)) return name.Contains("child") ? Keep(new Label { ID = "lblAdded", Text = "Added" }) : Page.FindControl("lblTitle");
            if (type == typeof(Style)) return new Style { ForeColor = Color1, BorderWidth = Unit.Pixel(2), CssClass = "merged" };
            if (type.IsEnum) return EnumValue(type);
            if (type.IsArray) return Array(type.GetElementType());
            if (typeof(Delegate).IsAssignableFrom(type)) return Callback(type);
            if (type.IsAssignableFrom(component.Type)) return Instance();
            return Instance(type);
        }

        static object Number(string name)
        {
            if (name.Contains("index") || name == "i") return 0;
            if (name.Contains("count") || name.Contains("size") || name.Contains("length")) return 2;
            return 1;
        }

        /// <summary>An ID of the page's controls where the parameter names one (posted data: the component's); else text.</summary>
        static string Text(string name)
        {
            if (name == "id" || name.Contains("control") || name.Contains("target")) return "lblTitle";
            if (name.Contains("format")) return "{0}";
            if (name.Contains("url") || name.Contains("path")) return "~/Default.aspx";
            if (name.Contains("field") || name.Contains("member")) return "Name";
            if (name.Contains("key") || name == "name" || name == "attribute") return "data-probe";
            if (name.Contains("group")) return "grp1";
            return "Abc 123";
        }

        static object EnumValue(Type type)
        {
            var values = Enum.GetValues(type);
            return values.GetValue(values.Length > 1 ? 1 : 0);
        }

        object Array(Type element)
        {
            if (element == typeof(string)) return new[] { "a", "b" };
            if (element == typeof(int)) return new[] { 1, 2 };
            if (element == typeof(object)) return new object[] { "a", 1 };
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

        int making;

        /// <summary>A fresh object of the type: by its constructor, or null.</summary>
        object New(Type type)
        {
            if (type.IsAbstract || type.IsInterface || type.ContainsGenericParameters || making > 3) return null;
            making++;
            try
            {
                foreach (var constructor in type.GetConstructors().OrderBy(c => c.GetParameters().Length))
                {
                    if (constructor.GetParameters().Any(x => x.ParameterType == type)) continue;
                    try { return Keep(constructor.Invoke(constructor.GetParameters().Select(x => Argument(x, constructor)).ToArray())); }
                    catch (TargetInvocationException) { }
                    catch (ArgumentException) { }
                }
                return null;
            }
            finally { making--; }
        }

        public override object Instance(Type type)
        {
            if (type.IsAssignableFrom(component.Type)) return Instance();
            if (type == typeof(Page)) return Page;
            return Find(type) ?? New(type);
        }

        /// <summary>The first object of the type in the sample page, breadth first through its controls and properties.</summary>
        object Find(Type type)
        {
            var seen = new HashSet<object>(ReferenceComparer.Instance);
            var queue = new Queue<(object Value, int Depth)>();
            queue.Enqueue((Page, 0));
            while (queue.Count > 0)
            {
                var (value, depth) = queue.Dequeue();
                if (value == null || !seen.Add(value)) continue;
                if (type.IsInstanceOfType(value)) return value;
                if (depth >= 3) continue;
                if (value is Control control && control.HasControls())
                    foreach (Control child in control.Controls) queue.Enqueue((child, depth + 1));
                if (!(value is WebControl)) continue;
                foreach (var property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.Name, StringComparer.Ordinal))
                {
                    if (property.GetIndexParameters().Length > 0 || property.PropertyType.IsValueType || property.PropertyType == typeof(string)) continue;
                    if (property.Name == "Page" || property.Name == "Parent" || property.Name == "NamingContainer" || property.Name == "BindingContainer") continue;
                    try { queue.Enqueue((property.GetValue(value, null), depth + 1)); } catch (TargetInvocationException) { }
                }
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
