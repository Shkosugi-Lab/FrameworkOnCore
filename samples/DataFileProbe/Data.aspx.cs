using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DataFileProbe
{
    /// <summary>
    /// DataTable and DataView as applications sort, filter and compute with them: the table's Locale and CaseSensitive
    /// (the comparison is its culture's, with IgnoreKanaType and IgnoreWidth when not case-sensitive), Select, Find,
    /// RowFilter's LIKE and comparisons, Convert() and expressions over doubles (the text System.Data makes of them),
    /// Compute's aggregates, primary keys and Merge, the XML of a DataSet. Each case is a line.
    /// </summary>
    public partial class Data : ProbePage
    {
        static readonly string[] Names =
        {
            "a", "B", "ä", "Æ", "ae", "résumé", "resume", "RESUME", "Zebra", "zebra", "10", "9", "é", "é", "",
            "ß", "ss", "İ", "i", "I", "ı", "カ", "か", "ｶ", "が", "ガ", "ｶﾞ", "co-op", "coop", "co op", "Ａ", "a b",
        };

        static DataTable Table(string locale, bool caseSensitive)
        {
            var table = new DataTable("Items") { Locale = CultureInfo.GetCultureInfo(locale), CaseSensitive = caseSensitive };
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Amount", typeof(double));
            table.Columns.Add("Price", typeof(decimal));
            table.Columns.Add("When", typeof(DateTime));
            for (var i = 0; i < Names.Length; i++)
                table.Rows.Add(i, Names[i], i * 0.1, i * 1.25m, new DateTime(2026, 1, 1).AddDays(i * 7));
            table.Rows.Add(Names.Length, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
            return table;
        }

        static string Ids(DataRow[] rows) => string.Join(" ", rows.Select(r => r["Name"] is DBNull ? "(null)" : "[" + r["Name"] + "]"));
        static string Ids(DataView view) => string.Join(" ", view.Cast<DataRowView>().Select(r => r["Name"] is DBNull ? "(null)" : "[" + r["Name"] + "]"));

        protected override void Run()
        {
            Section("sort");
            foreach (var locale in new[] { "en-US", "ja-JP", "de-DE", "sv-SE", "tr-TR" })
                foreach (var caseSensitive in new[] { false, true })
                    Case($"Sort {locale} caseSensitive={caseSensitive}", () => Ids(new DataView(Table(locale, caseSensitive)) { Sort = "Name ASC" }));
            Case("Select Name DESC (invariant)", () => Ids(Table("", false).Select("", "Name DESC")));
            Case("Sort Amount DESC, Name", () => Ids(new DataView(Table("en-US", false)) { Sort = "Amount DESC, Name" }).Substring(0, 60));

            Section("filter");
            var t = Table("en-US", false);
            foreach (var filter in new[]
            {
                "Name = 'RESUME'", "Name = 'résumé'", "Name LIKE 'a*'", "Name LIKE '*e'", "Name LIKE '[a]*'", "Name < 'b'",
                "Name = 'ｶ'", "Name = 'が'", "Name = 'ss'", "Name = 'i'", "Name IS NULL", "Len(Name) = 2", "Name IN ('a', 'B', 'z')",
                "Amount > 1.5", "Amount = 0.3", "Amount * 3 = 0.9", "Convert(Amount, 'System.String') LIKE '0.3*'",
                "Convert(Amount, 'System.String') = '0.30000000000000004'", "Price > 10.5", "When > #2026-03-01#", "When = #01/08/2026#",
                "Substring(Name, 1, 1) = 'z'", "Trim(Name) = ''", "IsNull(Name, 'none') = 'none'", "Name + 'x' = 'ax'",
            })
                Case("RowFilter " + filter, () => Ids(new DataView(t) { RowFilter = filter, Sort = "Id" }));
            Case("RowFilter de-DE Name = 'ae'", () => Ids(new DataView(Table("de-DE", false)) { RowFilter = "Name = 'ae'", Sort = "Id" }));
            Case("RowFilter ja-JP Name = 'ｶﾞ'", () => Ids(new DataView(Table("ja-JP", false)) { RowFilter = "Name = 'ｶﾞ'", Sort = "Id" }));
            Case("RowFilter tr-TR Name = 'I'", () => Ids(new DataView(Table("tr-TR", false)) { RowFilter = "Name = 'I'", Sort = "Id" }));
            Case("Find resume", () => new DataView(t) { Sort = "Name" }.Find("RESUME"));
            Case("FindRows カ", () => new DataView(Table("ja-JP", false)) { Sort = "Name" }.FindRows("カ").Length);

            Section("expressions");
            var e = Table("en-US", false);
            e.Columns.Add("Text", typeof(string), "Convert(Amount, 'System.String')");
            e.Columns.Add("Triple", typeof(double), "Amount * 3");
            e.Columns.Add("Label", typeof(string), "Name + ':' + Amount");
            e.Columns.Add("Cost", typeof(decimal), "Price * 1.1");
            e.Columns.Add("Kind", typeof(string), "IIF(Amount > 1, 'big', 'small')");
            foreach (var i in new[] { 1, 3, 7, 12 })
                Case($"Row {i} Text Triple Label Cost Kind", () => $"{e.Rows[i]["Text"]} | {e.Rows[i]["Triple"]} | {e.Rows[i]["Label"]} | {e.Rows[i]["Cost"]} | {e.Rows[i]["Kind"]}");
            Case("Compute Sum(Amount)", () => e.Compute("Sum(Amount)", ""));
            Case("Compute Avg(Amount)", () => e.Compute("Avg(Amount)", ""));
            Case("Compute Avg(Id)", () => e.Compute("Avg(Id)", ""));
            Case("Compute StDev(Amount)", () => e.Compute("StDev(Amount)", ""));
            Case("Compute Var(Price)", () => e.Compute("Var(Price)", ""));
            Case("Compute Max(Name)", () => e.Compute("Max(Name)", ""));
            Case("Compute Min(Name) ja", () => Table("ja-JP", false).Compute("Min(Name)", "Len(Name) > 0"));
            Case("Compute Count(Name)", () => e.Compute("Count(Name)", "Amount < 1"));
            Case("Compute 1/3", () => new DataTable().Compute("1/3", ""));
            Case("Compute 0.1+0.2", () => new DataTable().Compute("0.1 + 0.2", ""));
            Case("Compute 10/4", () => new DataTable().Compute("10/4", ""));
            Case("Compute 7 % 3", () => new DataTable().Compute("7 % 3", ""));
            Case("Compute 'a' + 1", () => new DataTable().Compute("'a' + 1", ""));
            Case("Compute 1e20 * 1e20", () => new DataTable().Compute("1e20 * 1e20", ""));
            Case("Compute 2147483647 + 1", () => new DataTable().Compute("2147483647 + 1", ""));
            Case("Compute Convert('1.5', 'System.Double')", () => new DataTable().Compute("Convert('1.5', 'System.Double')", ""));
            Case("Compute Convert(1.5, 'System.Int32')", () => new DataTable().Compute("Convert(1.5, 'System.Int32')", ""));
            Case("Compute Convert(2.5, 'System.Int32')", () => new DataTable().Compute("Convert(2.5, 'System.Int32')", ""));
            Case("Compute bad", () => new DataTable().Compute("1 +", ""));

            Section("keys and merge");
            var keyed = new DataTable("K") { CaseSensitive = false };
            keyed.Columns.Add("Code", typeof(string));
            keyed.PrimaryKey = new[] { keyed.Columns[0] };
            keyed.Rows.Add("abc");
            Case("Add ABC to a case-insensitive key", () => { keyed.Rows.Add("ABC"); return keyed.Rows.Count; });
            Case("Rows.Find aBc", () => keyed.Rows.Find("aBc")?["Code"]);
            var other = keyed.Clone();
            other.CaseSensitive = true;
            other.Rows.Add("Abc");
            Case("Merge case-sensitive into insensitive", () => { var copy = keyed.Copy(); copy.Merge(other); return string.Join(",", copy.Rows.Cast<DataRow>().Select(r => r[0])); });
            Case("Unique constraint message type", () => { try { keyed.Rows.Add("abc"); return "added"; } catch (Exception ex) { return ex.GetType().Name; } });

            Section("xml");
            var set = new DataSet("Set");
            var x = Table("en-US", false);
            set.Tables.Add(x);
            var writer = new StringWriter();
            set.WriteXml(writer);
            var xml = writer.ToString();
            Case("WriteXml row 3", () => xml.Substring(xml.IndexOf("<Id>3</Id>", StringComparison.Ordinal), 200).Replace("\r", "").Replace("\n", "|"));
            var schema = new StringWriter();
            set.WriteXmlSchema(schema);
            Case("WriteXmlSchema locale", () => schema.ToString().Contains("msdata:Locale=\"en-US\""));
            Case("WriteXmlSchema length", () => schema.ToString().Length);
            var read = new DataSet();
            read.ReadXml(new StringReader(xml));
            Case("ReadXml Amount of row 3", () => read.Tables[0].Rows[3]["Amount"]);
            Case("ReadXml When of row 3", () => read.Tables[0].Rows[3]["When"]);
            Case("GetXml length", () => set.GetXml().Length);
        }
    }
}
