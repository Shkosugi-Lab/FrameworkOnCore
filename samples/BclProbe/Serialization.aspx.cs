using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BclProbe
{
    public enum Status { Draft, Published = 5 }

    [Serializable]
    [DataContract(Name = "Order", Namespace = "urn:probe")]
    public class Order
    {
        [DataMember(Order = 1)] public int Id { get; set; }
        [DataMember(Order = 2)] public string Customer { get; set; }
        [DataMember(Order = 3)] public double Total { get; set; }
        [DataMember(Order = 4)] public decimal Tax { get; set; }
        [DataMember(Order = 5)] public DateTime Placed { get; set; }
        [DataMember(Order = 6)] public DateTime? Shipped { get; set; }
        [DataMember(Order = 7)] public Status Status { get; set; }
        [DataMember(Order = 8)] public List<string> Tags { get; set; }
        [DataMember(Order = 9)] public Dictionary<string, int> Counts { get; set; }
        [DataMember(Order = 10)] public float Weight { get; set; }
        [DataMember(Order = 11)] public Guid Key { get; set; }
        [DataMember(Order = 12)] public TimeSpan Wait { get; set; }
        [DataMember(Order = 13)] public byte[] Blob { get; set; }
        [DataMember(Order = 14)] public char Grade { get; set; }
        [DataMember(Order = 15)] public DateTimeOffset At { get; set; }
        [DataMember(Order = 16)] public Uri Link { get; set; }
    }

    /// <summary>
    /// What the serializers write (the application stores it, sends it to a browser or to another system):
    /// JavaScriptSerializer (ASP.NET AJAX), DataContractJsonSerializer, XmlSerializer, DataContractSerializer,
    /// DataSet.WriteXml and GetXml, XmlDocument and XDocument as text, XmlWriter's settings; doubles, dates of every
    /// kind, escaping of HTML and non-ASCII characters, and reading back.
    /// </summary>
    public partial class Serialization : ProbePage
    {
        static Order Sample() => new Order
        {
            Id = 7,
            Customer = "O'Brien <b>&\"日本\" \u2028 \U0001F600",
            Total = 0.1 + 0.2,
            Tax = 1.10m,
            Placed = new DateTime(2026, 3, 4, 13, 5, 9, 789, DateTimeKind.Utc),
            Shipped = new DateTime(2026, 3, 5, 9, 0, 0, DateTimeKind.Unspecified),
            Status = Status.Published,
            Tags = new List<string> { "a", "b/c", "</script>" },
            Counts = new Dictionary<string, int> { { "x", 1 }, { "y z", 2 } },
            Weight = 1f / 3,
            Key = new Guid("0f8fad5b-d9cb-469f-a165-70867728950e"),
            Wait = TimeSpan.FromMinutes(90.5),
            Blob = new byte[] { 0, 1, 254, 255 },
            Grade = 'A',
            At = new DateTimeOffset(2026, 3, 4, 22, 5, 9, TimeSpan.FromHours(9)),
            Link = new Uri("http://example.com/a b?x=日本"),
        };

        protected override void Run()
        {
            var order = Sample();
            Section("JavaScriptSerializer");
            var js = new JavaScriptSerializer();
            Case("order", () => js.Serialize(order));
            Case("doubles", () => js.Serialize(new object[] { 0.1, 0.1 + 0.2, 1.0 / 3, 1e21, 1e-7, -0.0, 100.0, float.MaxValue, 1.5f, double.MaxValue }));
            Case("NaN", () => js.Serialize(double.NaN));
            Case("decimal", () => js.Serialize(new[] { 1.10m, 0m, -0.5m, decimal.MaxValue }));
            Case("dates", () => js.Serialize(new object[] { new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Local), new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Unspecified), DateTime.MinValue, new DateTime(1960, 1, 1, 0, 0, 0, DateTimeKind.Utc) }));
            Case("anonymous", () => js.Serialize(new { a = 1, b = (string)null, c = new[] { 1, 2 }, d = new Dictionary<string, object> { { "k", true } } }));
            Case("escaping", () => js.Serialize("<>&'\"\\/\u0001\u007f\u0085\u2028\u2029é😀"));
            Case("char", () => js.Serialize(new[] { 'a', '\0', '"' }));
            Case("enum", () => js.Serialize(new[] { Status.Draft, Status.Published, (Status)3 }));
            Case("Deserialize order", () => { var o = js.Deserialize<Order>(js.Serialize(order)); return o.Customer + " " + o.Total + " " + o.Placed.ToString("o") + " " + o.Shipped.Value.ToString("o") + " " + o.Weight + " " + o.Wait; });
            Case("DeserializeObject", () => Describe(js.DeserializeObject("{\"a\":1,\"b\":1.5,\"c\":12345678901234,\"d\":[1,\"x\",null],\"e\":\"\\/Date(1772629509789)\\/\",\"f\":1e400}")));
            Case("Deserialize number", () => Describe(js.DeserializeObject("[0.1, 1E2, -0, 79228162514264337593543950336, 1.7976931348623157E+309]")));
            Case("Deserialize bad", () => js.DeserializeObject("{a:"));
            Case("Deserialize date local", () => js.Deserialize<DateTime>("\"\\/Date(1772629509789)\\/\"").ToString("o"));
            Case("Deserialize single quote", () => Describe(js.DeserializeObject("{'a':'b'}")));
            Case("MaxJsonLength", () => new JavaScriptSerializer { MaxJsonLength = 10 }.Serialize(new string('x', 20)));
            Case("RecursionLimit", () => new JavaScriptSerializer { RecursionLimit = 2 }.Serialize(new[] { new[] { new[] { 1 } } }));
            Case("ConvertToType", () => js.ConvertToType<Dictionary<string, int>>(new Dictionary<string, object> { { "a", 1 } })["a"]);

            Section("DataContractJsonSerializer");
            Case("order", () => DcJson(order));
            Case("doubles", () => DcJson(new[] { 0.1, 0.1 + 0.2, 1.0 / 3, 1e21, -0.0, double.NaN, double.PositiveInfinity }));
            Case("floats", () => DcJson(new[] { 0.1f, 1f / 3, float.MaxValue }));
            Case("dates", () => DcJson(new[] { new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Unspecified) }));
            Case("escaping", () => DcJson("<>&'\"\\/\u0001\u007f\u2028é😀"));
            Case("read back", () => { var s = new DataContractJsonSerializer(typeof(Order)); var o = (Order)s.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(DcJson(order)))); return o.Total + " " + o.Customer + " " + o.Weight; });

            Section("XmlSerializer");
            Case("order", () => XmlSer(new XmlOrder(order)));
            Case("doubles", () => XmlSer(new[] { 0.1, 0.1 + 0.2, 1.0 / 3, 1e21, -0.0, double.NaN, double.NegativeInfinity }));
            Case("floats", () => XmlSer(new[] { 0.1f, 1f / 3 }));
            Case("decimal", () => XmlSer(new[] { 1.10m, -0.0m }));
            Case("dates", () => XmlSer(new[] { new DateTime(2026, 3, 4, 13, 5, 9, 789, DateTimeKind.Utc), new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Unspecified), DateTime.MinValue }));
            Case("control char", () => XmlSer("a\u0001b"));
            Case("read back", () => { var s = new XmlSerializer(typeof(XmlOrder)); var o = (XmlOrder)s.Deserialize(new StringReader(XmlSer(new XmlOrder(order)))); return o.Total + " " + o.Customer + " " + o.Placed.ToString("o"); });

            Section("DataContractSerializer");
            Case("order", () => { var s = new DataContractSerializer(typeof(Order)); var w = new StringWriter(); using (var x = XmlWriter.Create(w)) s.WriteObject(x, order); return w.ToString(); });

            Section("DataSet");
            var ds = new DataSet("Shop");
            var t = ds.Tables.Add("Item");
            t.Columns.Add("Id", typeof(int));
            t.Columns.Add("Price", typeof(double));
            t.Columns.Add("Cost", typeof(decimal));
            t.Columns.Add("When", typeof(DateTime));
            t.Columns.Add("Name", typeof(string));
            t.Rows.Add(1, 0.1 + 0.2, 1.10m, new DateTime(2026, 3, 4, 13, 5, 9, DateTimeKind.Unspecified), "a<b>&");
            t.Rows.Add(2, 1.0 / 3, -0.5m, new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Utc), null);
            Case("GetXml", () => ds.GetXml());
            Case("WriteXml schema", () => { var w = new StringWriter(); ds.WriteXml(w, XmlWriteMode.WriteSchema); return w.ToString(); });
            Case("GetXmlSchema", () => ds.GetXmlSchema());
            Case("row ToString", () => t.Rows[0]["Price"].ToString() + " " + t.Rows[0]["When"]);
            Case("Compute avg", () => t.Compute("Avg(Price)", "").ToString());
            Case("Select sort", () => string.Join(",", t.Select("", "Price DESC").Select(r => r["Id"])));
            Case("ReadXml", () => { var d = new DataSet(); d.ReadXml(new StringReader(ds.GetXml())); return d.Tables[0].Rows[0]["Price"] + " " + d.Tables[0].Columns["Price"].DataType.Name; });

            Section("XML as text");
            var doc = new XmlDocument();
            doc.LoadXml("<root a='1'  b=\"2\"><x>text &amp; more</x><y/><z><![CDATA[c]]></z><!-- c --></root>");
            Case("OuterXml", () => doc.OuterXml);
            Case("Save indent", () => { var w = new StringWriter(); doc.Save(w); return w.ToString(); });
            Case("XmlWriter default", () => { var w = new StringWriter(); using (var x = XmlWriter.Create(w)) { x.WriteStartElement("a"); x.WriteAttributeString("v", "1\n2\t\"<"); x.WriteString("é\u0085"); x.WriteEndElement(); } return w.ToString(); });
            Case("XmlWriter bytes", () => { var m = new MemoryStream(); using (var x = XmlWriter.Create(m)) { x.WriteElementString("a", "b"); } return BitConverter.ToString(m.ToArray().Take(8).ToArray()); });
            Case("XmlTextWriter", () => { var w = new StringWriter(); var x = new XmlTextWriter(w) { Formatting = Formatting.Indented }; x.WriteStartElement("a"); x.WriteElementString("b", "c"); x.WriteEndElement(); x.Flush(); return w.ToString(); });
            Case("XDocument", () => XDocument.Parse("<r><a x='1'/><b>t</b></r>").ToString());
            Case("XDocument declaration", () => { var d = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("r", 0.1 + 0.2, new XAttribute("d", 1.0 / 3))); var w = new StringWriter(); d.Save(w); return w.ToString(); });
            Case("XElement double", () => new XElement("v", 0.1 + 0.2).ToString() + (string)new XAttribute("a", 1f / 3) + " " + (double)new XElement("x", "0.30000000000000004"));
            Case("XElement date", () => new XElement("d", new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Local)).Value.Length);
            Case("XmlConvert double", () => XmlConvert.ToString(0.1 + 0.2) + " " + XmlConvert.ToString(1f / 3) + " " + XmlConvert.ToString(-0.0) + " " + XmlConvert.ToString(double.PositiveInfinity));
            Case("XmlConvert date", () => XmlConvert.ToString(new DateTime(2026, 3, 4, 13, 5, 9, DateTimeKind.Utc), XmlDateTimeSerializationMode.RoundtripKind));
            Case("XmlConvert parse", () => XmlConvert.ToDouble(" 1E2 ") + " " + XmlConvert.ToDouble("INF"));
            Case("bad xml", () => { new XmlDocument().LoadXml("<a><b></a>"); return 0; });
            Case("XPath", () => doc.SelectNodes("//*[@a or self::y]").Count);
            Case("XPath number", () => doc.CreateNavigator().Evaluate("1 div 3").ToString());
            Case("XPath string", () => doc.CreateNavigator().Evaluate("string(0.1 + 0.2)"));
            Case("XSLT", () => { var x = new System.Xml.Xsl.XslCompiledTransform(); x.Load(XmlReader.Create(new StringReader("<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'><xsl:output method='html'/><xsl:template match='/'><p><xsl:value-of select='1 div 3'/> <xsl:value-of select='format-number(1234.5, \"#,##0.00\")'/></p><br/></xsl:template></xsl:stylesheet>"))); var w = new StringWriter(); x.Transform(doc, null, w); return w.ToString(); });

            Section("HttpUtility");
            const string text = "a b&c=d/é日本?#%+<>\"'\u00a0\u2028😀";
            Case("UrlEncode", () => HttpUtility.UrlEncode(text));
            Case("UrlPathEncode", () => HttpUtility.UrlPathEncode("/a b/é?x=y z"));
            Case("UrlEncodeUnicode", () => HttpUtility.UrlEncodeUnicode(text));
            Case("UrlDecode", () => HttpUtility.UrlDecode("a+b%20c%e6%97%a5%u65e5%zz%"));
            Case("UrlDecode bad utf8", () => Show(HttpUtility.UrlDecode("%C0%AF%E6%97")));
            Case("HtmlEncode", () => HttpUtility.HtmlEncode(text));
            Case("HtmlAttributeEncode", () => HttpUtility.HtmlAttributeEncode(text));
            Case("HtmlDecode", () => HttpUtility.HtmlDecode("&lt;&amp;&#39;&#x1F600;&nbsp;&eacute;&notanentity;&#99999999;&"));
            Case("JavaScriptStringEncode", () => HttpUtility.JavaScriptStringEncode(text, true));
            Case("ParseQueryString", () => { var q = HttpUtility.ParseQueryString("a=1&a=2&b&=c&d=%e6%97%a5&e=x+y"); return string.Join("|", q.AllKeys.Select(k => (k ?? "(null)") + "=" + q[k])) + " / " + q.ToString(); });
            Case("UrlEncode bytes", () => HttpUtility.UrlEncode(new byte[] { 0, 32, 255 }));
            Case("Uri.EscapeDataString", () => Uri.EscapeDataString(text));
            Case("Uri.EscapeUriString", () => Uri.EscapeUriString("http://x/a b/é?q=a b#f g"));
            Case("WebUtility.UrlEncode", () => WebUtilityEncode(text));
            Case("Uri ToString", () => new Uri("http://example.com/a%20b/%E6%97%A5?q=%41").ToString());
            Case("Uri AbsoluteUri", () => new Uri("http://example.com/a b/日本?q=x y").AbsoluteUri);
            Case("Uri dots", () => new Uri("http://example.com/a/./b/../c%2F..%2Fd").AbsolutePath);
            Case("Uri trailing dot", () => new Uri("http://example.com/a./b...").AbsolutePath);
            Case("Uri host", () => new Uri("http://ＥＸＡＭＰＬＥ.com:80/").Host);
            Case("Uri idn", () => new Uri("http://日本.jp/").Host + " " + new Uri("http://日本.jp/").IdnHost);
            Case("Uri combine", () => new Uri(new Uri("http://x/a/b/"), "../c?d#e").ToString());
            Case("Uri file", () => new Uri("file:///C:/a b/c").LocalPath);
            Case("Uri unc", () => new Uri(@"\\server\share\a").ToString());
            Case("Uri userinfo", () => new Uri("http://u:p@x/").UserInfo);
            Case("Uri MakeRelative", () => new Uri("http://x/a/b/c").MakeRelativeUri(new Uri("http://x/a/d e")).ToString());
        }

        static string WebUtilityEncode(string s) => System.Net.WebUtility.UrlEncode(s);

        static string DcJson(object value)
        {
            var s = new DataContractJsonSerializer(value.GetType());
            var m = new MemoryStream();
            s.WriteObject(m, value);
            return Encoding.UTF8.GetString(m.ToArray());
        }

        static string XmlSer(object value)
        {
            var s = new XmlSerializer(value.GetType());
            var w = new StringWriter();
            s.Serialize(w, value);
            return w.ToString();
        }

        static string Describe(object value)
        {
            if (value == null) return "null";
            var d = value as IDictionary<string, object>;
            if (d != null) return "{" + string.Join(",", d.Select(p => p.Key + ":" + Describe(p.Value))) + "}";
            var a = value as object[];
            if (a != null) return "[" + string.Join(",", a.Select(Describe)) + "]";
            var l = value as System.Collections.ArrayList;
            if (l != null) return "[" + string.Join(",", l.Cast<object>().Select(Describe)) + "]";
            var dt = value as DateTime?;
            if (dt != null) return value.GetType().Name + ":" + dt.Value.ToString("o");
            return value.GetType().Name + ":" + Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    public class XmlOrder
    {
        public XmlOrder() { }
        public XmlOrder(Order o)
        {
            Id = o.Id; Customer = o.Customer; Total = o.Total; Tax = o.Tax; Placed = o.Placed; Shipped = o.Shipped;
            Status = o.Status; Tags = o.Tags; Weight = o.Weight; Key = o.Key; Blob = o.Blob; Grade = o.Grade;
        }
        [XmlAttribute] public int Id { get; set; }
        public string Customer { get; set; }
        public double Total { get; set; }
        public decimal Tax { get; set; }
        public DateTime Placed { get; set; }
        public DateTime? Shipped { get; set; }
        public Status Status { get; set; }
        public List<string> Tags { get; set; }
        public float Weight { get; set; }
        public Guid Key { get; set; }
        public byte[] Blob { get; set; }
        public char Grade { get; set; }
    }
}
