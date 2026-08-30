using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForm2Blazor.PropertyCatalog
{
    /// <summary>
    /// Generates the WebForms control property catalog.
    ///
    /// Instantiates each control on the real .NET Framework 4.8 runtime and reads the
    /// "runtime default value" of every public property via reflection, writing the
    /// result to JSON. The values come from the real machine, not from documentation
    /// or memory, so assumed-default accidents cannot happen.
    ///
    /// The output feeds the converter's --coverage mode (the unsupported-property audit).
    /// </summary>
    internal static class Program
    {
        private static readonly Type[] SupportedControls =
        {
            typeof(Label), typeof(Literal), typeof(TextBox), typeof(Button), typeof(LinkButton),
            typeof(HyperLink), typeof(CheckBox), typeof(Panel), typeof(PlaceHolder),
            typeof(DropDownList), typeof(ListItem), typeof(RadioButtonList),
            typeof(GridView), typeof(BoundField), typeof(TemplateField),
            typeof(Repeater), typeof(ListView), typeof(FormView),
            typeof(RequiredFieldValidator), typeof(RangeValidator), typeof(CompareValidator),
            typeof(RegularExpressionValidator), typeof(CustomValidator), typeof(ValidationSummary),
            typeof(HiddenField), typeof(System.Web.UI.WebControls.Image), typeof(RadioButton), typeof(DataList),
            typeof(FileUpload), typeof(CheckBoxList),
        };

        private static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("使い方: WebForm2Blazor.PropertyCatalog <出力先.json>");
                return 1;
            }

            var json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("  \"runtime\": " + Quote(Environment.Version.ToString()) + ",");
            json.AppendLine("  \"controls\": {");

            for (var typeIndex = 0; typeIndex < SupportedControls.Length; typeIndex++)
            {
                var type = SupportedControls[typeIndex];
                object instance;
                try
                {
                    instance = Activator.CreateInstance(type);
                }
                catch (Exception)
                {
                    instance = null;
                }

                json.Append("    " + Quote(type.Name) + ": [");

                var properties = type
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetIndexParameters().Length == 0)
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .ToList();

                for (var i = 0; i < properties.Count; i++)
                {
                    var property = properties[i];
                    json.Append(i == 0 ? "\n" : ",\n");
                    json.Append("      { \"name\": " + Quote(property.Name)
                        + ", \"type\": " + Quote(property.PropertyType.Name)
                        + ", \"settable\": " + (property.CanWrite && property.GetSetMethod() != null ? "true" : "false")
                        + ", \"default\": " + Quote(ReadDefault(property, instance)) + " }");
                }

                json.AppendLine("\n    ]" + (typeIndex < SupportedControls.Length - 1 ? "," : ""));
            }

            json.AppendLine("  }");
            json.AppendLine("}");

            File.WriteAllText(args[0], json.ToString(), new UTF8Encoding(false));
            Console.WriteLine("カタログ生成: " + args[0] + "(コントロール " + SupportedControls.Length + " 種)");
            return 0;
        }

        private static string ReadDefault(PropertyInfo property, object instance)
        {
            if (instance == null)
            {
                return "(生成不可)";
            }

            object value;
            try
            {
                value = property.GetValue(instance);
            }
            catch (Exception)
            {
                return "(取得不可)";
            }

            return Format(value);
        }

        private static string Format(object value)
        {
            if (value == null)
            {
                return "null";
            }
            if (value is string s)
            {
                return "\"" + s + "\"";
            }
            if (value is bool || value is Enum)
            {
                return value.ToString();
            }
            if (value is IFormattable formattable)
            {
                return formattable.ToString(null, CultureInfo.InvariantCulture);
            }
            if (value is Color color)
            {
                return color.IsEmpty ? "Color.Empty" : color.ToString();
            }
            if (value is Unit unit)
            {
                return unit.IsEmpty ? "Unit.Empty" : unit.ToString();
            }

            // Composite types such as FontInfo / collections: record the type name only
            return "(複合型: " + value.GetType().Name + ")";
        }

        private static string Quote(string value)
        {
            var sb = new StringBuilder("\"");
            foreach (var c in value)
            {
                if (c == '"' || c == '\\')
                {
                    sb.Append('\\');
                    sb.Append(c);
                }
                else if (c < ' ')
                {
                    sb.Append("\\u").Append(((int)c).ToString("x4"));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.Append('"').ToString();
        }
    }
}
