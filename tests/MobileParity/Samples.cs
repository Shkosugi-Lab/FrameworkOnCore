using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mobile;
using System.Web.UI;
using System.Web.UI.MobileControls;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.MobileParity
{
    /// <summary>
    /// The values the API cases call with. A sample mobile page (SamplePage: two forms, every kind of control, a style
    /// sheet, a device specific choice), fresh for each member: an instance of a type is the first object of that type
    /// found in it (its controls and properties, breadth first), else one made by the type's constructor with sample
    /// arguments. A device's capabilities (Capabilities: an HTML 3.2 browser's, as a fixed list). Arguments by type, a few
    /// by parameter name (indexes, IDs of the page's controls). The same values on .NET Framework and on the port, so what
    /// differs is the implementation. No request: outside a web application, as the members are called here, what needs
    /// one fails the same way on both.
    /// </summary>
    public sealed class Samples : ApiSamples
    {
        readonly List<IDisposable> made = new List<IDisposable>();
        MobilePage page;

        public override void Dispose()
        {
            for (var i = made.Count - 1; i >= 0; i--) { try { made[i].Dispose(); } catch { } }
        }

        T Keep<T>(T value)
        {
            if (value is IDisposable disposable) made.Add(disposable);
            return value;
        }

        public MobilePage Page => page ?? (page = SamplePage());

        /// <summary>The page the members are called on: every kind of control, with the IDs the name samples say.</summary>
        public static MobilePage SamplePage()
        {
            var page = new MobilePage { ID = "page1" };
            var styles = new StyleSheet { ID = "styles" };
            Add(page, styles, () => styles["title"] = new Style { Name = "title", Font = { Bold = BooleanOption.True }, ForeColor = Color.Navy });
            var main = new Form { ID = "Main", Title = "Mobile probe" };
            Add(page, main);
            Add(main, new Label { ID = "lblTitle", Text = "Title", StyleReference = "title" });
            Add(main, new TextView { ID = "txtView", Text = "The <b>mobile</b> controls." });
            Add(main, new TextBox { ID = "txtName", Text = "Taro", MaxLength = 20, Size = 10 });
            Add(main, new RequiredFieldValidator { ID = "reqName", ControlToValidate = "txtName", ErrorMessage = "Enter a name." });
            Add(main, new CompareValidator { ID = "cmpName", ControlToValidate = "txtName", ValueToCompare = "Taro", ErrorMessage = "Not Taro." });
            Add(main, new RangeValidator { ID = "rngName", ControlToValidate = "txtName", MinimumValue = "A", MaximumValue = "Z", ErrorMessage = "Out of range." });
            Add(main, new RegularExpressionValidator { ID = "rexName", ControlToValidate = "txtName", ValidationExpression = "[A-Z][a-z]+", ErrorMessage = "Not a name." });
            Add(main, new CustomValidator { ID = "cusName", ControlToValidate = "txtName", ErrorMessage = "Custom." });
            Add(main, new ValidationSummary { ID = "summary", FormToValidate = "Main", HeaderText = "Errors" });
            var size = new SelectionList { ID = "selSize", SelectType = ListSelectType.DropDown };
            Add(main, size, () => { size.Items.Add(new MobileListItem("Small", "S")); size.Items.Add(new MobileListItem("Medium", "M") { Selected = true }); size.Items.Add(new MobileListItem("Large", "L")); });
            Add(main, new Command { ID = "cmdGo", Text = "Go", CommandName = "go", CommandArgument = "1" });
            var colors = new List { ID = "lstColors", ItemsAsLinks = false };
            Add(main, colors, () => { colors.Items.Add(new MobileListItem("Red", "#f00")); colors.Items.Add(new MobileListItem("Green", "#0f0")); });
            Add(main, new Link { ID = "lnkSecond", NavigateUrl = "#Second", Text = "Second" });
            Add(main, new PhoneCall { ID = "call", PhoneNumber = "0123456789", Text = "Call", AlternateFormat = "{0} {1}" });
            Add(main, new System.Web.UI.MobileControls.Image { ID = "img", ImageUrl = "logo.gif", AlternateText = "Logo", NavigateUrl = "#Second" });
            Add(main, new System.Web.UI.MobileControls.Calendar { ID = "cal", SelectedDate = new DateTime(2020, 1, 2), VisibleDate = new DateTime(2020, 1, 1) });
            Add(main, new AdRotator { ID = "ads", AdvertisementFile = "ads.xml", KeywordFilter = "k" });
            var panel = new Panel { ID = "pnl" };
            Add(main, panel, () => panel.Controls.Add(new Label { ID = "lblInPanel", Text = "In a panel" }));
            var choices = new DeviceSpecific { ID = "specific" };
            Add(panel, choices, () => choices.Choices.Add(new DeviceSpecificChoice { Filter = "isHTML32", Argument = "html" }));
            var second = new Form { ID = "Second", Title = "Products" };
            Add(page, second);
            var products = new ObjectList { ID = "objProducts", LabelField = "Name", AutoGenerateFields = true };
            Add(second, products, () =>
            {
                products.Fields.Add(new ObjectListField { Name = "Name", DataField = "Name", Title = "Name" });
                products.Fields.Add(new ObjectListField { Name = "Price", DataField = "Price", Title = "Price", DataFormatString = "{0:N2}" });
                products.Commands.Add(new ObjectListCommand("buy", "Buy"));
                products.DataSource = Products();
                products.DataBind();
            });
            Add(second, new Link { ID = "lnkMain", NavigateUrl = "#Main", Text = "Back" });
            return page;
        }

        // Each part separately: one that fails (as outside a request a control may) leaves the rest of the page.
        static void Add(Control parent, Control child, Action fill = null)
        {
            try { parent.Controls.Add(child); } catch (Exception) { }
            try { fill?.Invoke(); } catch (Exception) { }
        }

        public sealed class Product
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
        }

        static List<Product> Products() => new List<Product> { new Product { Name = "Pen", Price = 1.25m }, new Product { Name = "Bag", Price = 24.99m } };

        /// <summary>An HTML 3.2 browser's capabilities, as .NET Framework's browser files give a desktop browser's (the ones the mobile controls read).</summary>
        public static MobileCapabilities Capabilities()
        {
            var capabilities = new MobileCapabilities();
            capabilities.Capabilities = new Hashtable(StringComparer.OrdinalIgnoreCase)
            {
                ["browser"] = "IE", ["type"] = "IE6", ["version"] = "6.0", ["majorversion"] = "6", ["minorversion"] = ".0",
                ["isMobileDevice"] = "false", ["preferredRenderingType"] = "html32", ["preferredRenderingMime"] = "text/html",
                ["preferredImageMime"] = "image/gif", ["maximumRenderedPageSize"] = "300000", ["screenCharactersWidth"] = "80",
                ["screenCharactersHeight"] = "40", ["screenPixelsWidth"] = "640", ["screenPixelsHeight"] = "480",
                ["screenBitDepth"] = "8", ["isColor"] = "true", ["javascript"] = "true", ["ecmascriptversion"] = "1.2",
                ["tables"] = "true", ["frames"] = "true", ["cookies"] = "true", ["supportsCss"] = "true",
                ["requiresUniqueFilePathSuffix"] = "false", ["requiresAttributeColonSubstitution"] = "false",
                ["canInitiateVoiceCall"] = "false", ["canSendMail"] = "true", ["numberOfSoftkeys"] = "0",
                ["defaultSubmitButtonLimit"] = "1", ["maximumSoftkeyLabelLength"] = "5", ["mobileDeviceManufacturer"] = "Unknown",
                ["mobileDeviceModel"] = "Unknown", ["inputType"] = "keyboard", ["supportsBold"] = "true", ["supportsItalic"] = "true",
                ["supportsFontSize"] = "true", ["supportsFontName"] = "true", ["supportsFontColor"] = "true", ["supportsBodyColor"] = "true",
                ["supportsDivAlign"] = "true", ["supportsDivNoWrap"] = "true", ["supportsImageSubmit"] = "true", ["supportsSelectMultiple"] = "true",
            };
            return capabilities;
        }

        public static readonly Color Color1 = Color.FromArgb(255, 51, 102, 204);

        public override object Argument(ParameterInfo parameter, MemberInfo member)
        {
            var type = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
            var name = (parameter.Name ?? "").ToLowerInvariant();
            if (parameter.IsOut) return type.IsValueType ? Activator.CreateInstance(type) : null;
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(float) || type == typeof(double) || type == typeof(byte) || type == typeof(decimal))
                return Convert.ChangeType(Number(name), type, CultureInfo.InvariantCulture);
            // A pointer: none (a delegate's constructor made from another one runs it: an access violation).
            if (type == typeof(IntPtr)) return IntPtr.Zero;
            if (type == typeof(bool)) return true;
            if (type == typeof(char)) return ',';
            if (type == typeof(string)) return Text(name);
            if (type == typeof(object)) return name.Contains("source") || name.Contains("data") ? (object)Products() : "Abc 123";
            if (type == typeof(DateTime)) return new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Unspecified);
            if (type == typeof(Color)) return Color1;
            if (type == typeof(Type)) return typeof(Label);
            if (type == typeof(CultureInfo)) return CultureInfo.InvariantCulture;
            if (type == typeof(TextWriter)) return Keep(new StringWriter(CultureInfo.InvariantCulture));
            if (type == typeof(HtmlTextWriter)) return Keep(new HtmlTextWriter(new StringWriter(CultureInfo.InvariantCulture)));
            if (type == typeof(EventArgs)) return EventArgs.Empty;
            if (type == typeof(MobileCapabilities) || type == typeof(HttpBrowserCapabilities)) return Capabilities();
            if (type == typeof(NameValueCollection)) return new NameValueCollection { { "__EVENTTARGET", "cmdGo" }, { "txtName", "Hanako" } };
            if (type == typeof(IDictionary) || type == typeof(Hashtable)) return new Hashtable { { "a", "1" } };
            if (type == typeof(IEnumerable) || type == typeof(ICollection) || type == typeof(IList)) return Products();
            if (type == typeof(Control)) return Page.FindControl("lblTitle") ?? Keep(new Label { ID = "lblLoose", Text = "Loose" });
            if (type.IsEnum) return EnumValue(type);
            if (type.IsArray) return Array(type.GetElementType());
            if (typeof(Delegate).IsAssignableFrom(type)) return Callback(type);
            if (MobileDescribe.IsMobileType(type)) return New(type) ?? Instance(type);
            return Instance(type);
        }

        static object Number(string name)
        {
            if (name.Contains("index") || name == "i") return 0;
            if (name.Contains("count") || name.Contains("size") || name.Contains("length")) return 2;
            return 1;
        }

        /// <summary>An ID of the page's controls where the parameter names one; a format, a filter; else text.</summary>
        static string Text(string name)
        {
            if (name == "id" || name.Contains("control") || name.Contains("target")) return "lblTitle";
            if (name.Contains("form")) return "Second";
            if (name.Contains("format")) return "{0}";
            if (name.Contains("filter")) return "isHTML32";
            if (name.Contains("url") || name.Contains("path")) return "~/Default.aspx";
            if (name.Contains("field") || name.Contains("member")) return "Name";
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

        /// <summary>A fresh object of the type (a control to add, an item to insert): by its constructor, or null.</summary>
        object New(Type type)
        {
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
            if (type == typeof(MobilePage)) return Page;
            if (type == typeof(MobileCapabilities)) return Capabilities();
            if (!MobileDescribe.IsMobileType(type)) return type.IsAbstract || type.IsInterface ? null : New(type) ?? NonPublic(type);
            return Find(type) ?? New(type) ?? Concrete(type) ?? NonPublic(type);
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
                // Not the designer's (a control's adapter outside a request on .NET Framework, Visual Studio's, not in the port):
                // the same objects on both.
                if (IsOfType(value, type) && !MobileDescribe.IsDesignerType(value.GetType())) return value;
                if (depth >= 5) continue;
                if (value is Control control && control.HasControls())
                    foreach (Control child in control.Controls) queue.Enqueue((child, depth + 1));
                if (!MobileDescribe.IsMobileType(value.GetType())) continue;
                if (value is IEnumerable sequence && !(value is Control))
                {
                    try { foreach (var item in sequence.Cast<object>().Take(4)) queue.Enqueue((item, depth + 1)); } catch (Exception) { }
                }
                foreach (var property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => MobileDescribe.IsMobileType(x.DeclaringType)).OrderBy(x => x.Name, StringComparer.Ordinal))
                {
                    if (property.GetIndexParameters().Length > 0 || property.PropertyType.IsValueType || property.PropertyType == typeof(string)) continue;
                    try { queue.Enqueue((property.GetValue(value, null), depth + 1)); } catch (TargetInvocationException) { }
                }
            }
            return null;
        }

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
            foreach (var derived in type.Assembly.GetExportedTypes().Where(t => type.IsAssignableFrom(t) && !t.IsAbstract && !MobileDescribe.IsDesignerType(t)).OrderBy(t => t.FullName, StringComparer.Ordinal))
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
