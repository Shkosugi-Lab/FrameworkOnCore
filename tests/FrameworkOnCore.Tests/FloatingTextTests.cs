namespace FrameworkOnCore.Tests;

/// <summary>
/// FOC1008 and the floating-point rules: where the application makes text of a double or a float, the converter writes
/// FrameworkOnCore.FrameworkNumbers (as .NET Framework wrote it); Parse, TryParse and TimeSpan.From* of a double,
/// FrameworkParsing. C# and Visual Basic.
/// </summary>
public class FloatingTextTests
{
    const string N = "global::FrameworkOnCore.FrameworkNumbers.";

    static string CSharp(string body) =>
        RewriteHarness.CSharp("using System; using System.Text; using System.Linq; using System.Linq.Expressions;\npublic class C {\n" + body + "\n}");

    [Fact]
    public void ToString_of_a_double_and_a_float()
    {
        var written = CSharp("string F(double d, float f) => d.ToString() + f.ToString(\"N2\") + d.ToString(System.Globalization.CultureInfo.InvariantCulture);");
        Assert.Contains(N + "ToString(d)", written);
        Assert.Contains(N + "ToString(f, \"N2\")", written);
        Assert.Contains(N + "ToString(d, System.Globalization.CultureInfo.InvariantCulture)", written);
    }

    [Fact]
    public void Concatenation_and_compound_assignment()
    {
        var written = CSharp("string F(double d, double? n, object o) { var s = \"a\" + d + n; s += d; return s + o; }");
        Assert.Contains("\"a\" + " + N + "Text(d) + " + N + "Text(n)", written);
        Assert.Contains("s += " + N + "Text(d)", written);
        Assert.Contains("s + " + N + "Text(o)", written);
    }

    [Fact]
    public void An_interpolations_hole_is_parenthesized()
    {
        var written = CSharp("string F(double d) => $\"{d} {d,8:N2} {d.ToString(\"F1\")}\";");
        Assert.Contains("{(" + N + "Formattable(d))}", written);
        Assert.Contains("{(" + N + "Formattable(d)),8:N2}", written);
        Assert.Contains("{(" + N + "ToString(d, \"F1\"))}", written);
    }

    [Fact]
    public void Composite_formats_and_writers()
    {
        var written = CSharp("""
            string F(double d, float f, StringBuilder sb, System.IO.TextWriter w)
            {
                sb.Append(d).AppendFormat("{0:N1}", f);
                w.Write(d);
                w.WriteLine("{0} {1}", d, "x");
                return string.Format("{0:C} {1}", d, 1) + Convert.ToString(d) + string.Join(",", new[] { d });
            }
            """);
        Assert.Contains("sb.Append(" + N + "Text(d))", written);
        Assert.Contains("AppendFormat(\"{0:N1}\", " + N + "Formattable(f))", written);
        Assert.Contains("w.Write(" + N + "Text(d))", written);
        Assert.Contains("w.WriteLine(\"{0} {1}\", " + N + "Formattable(d), \"x\")", written);
        Assert.Contains("string.Format(\"{0:C} {1}\", " + N + "Formattable(d), 1)", written);
        Assert.Contains(N + "ConvertToString(d)", written);
        Assert.Contains("string.Join(\",\", " + N + "Texts(new[] { d }))", written);
    }

    [Fact]
    public void An_objects_ToString_a_DataRow_field()
    {
        var written = CSharp("string F(System.Data.DataRow row, object o) => row[\"Price\"].ToString() + Convert.ToString(o, System.Globalization.CultureInfo.CurrentCulture);");
        Assert.Contains(N + "ToString(row[\"Price\"])", written);
        Assert.Contains(N + "ConvertToString(o, System.Globalization.CultureInfo.CurrentCulture)", written);
    }

    [Fact]
    public void What_is_not_a_double_and_expression_trees_are_left()
    {
        const string source = """
            string F(int i, string s, decimal m) => i.ToString() + s + m + $"{i}" + string.Format("{0}", s);
            Expression<Func<double, string>> G() => d => d.ToString();
            string H() => base.ToString() + this.ToString();
            void Log(string format, params object[] args) { }
            void L(double d) => Log("{0}", d);
            """;
        Assert.DoesNotContain("FrameworkNumbers", CSharp(source));
    }

    [Fact]
    public void Visual_Basic()
    {
        var written = RewriteHarness.VisualBasic("""
            Imports System
            Public Class C
                Function Build(d As Double, f As Single, row As System.Data.DataRow) As String
                    Dim s As String = "a" & d & row("Price")
                    s = s & CStr(f) & d.ToString("N2") & $"{d:F1}" & String.Format("{0}", d)
                    Return s
                End Function
            End Class
            """);
        Assert.True(written.Contains("\"a\" & Global.FrameworkOnCore.FrameworkNumbers.Text(d) & Global.FrameworkOnCore.FrameworkNumbers.Text(row(\"Price\"))"), written);
        Assert.Contains("CStr(Global.FrameworkOnCore.FrameworkNumbers.Text(f))", written);
        Assert.Contains("Global.FrameworkOnCore.FrameworkNumbers.ToString(d, \"N2\")", written);
        Assert.Contains("{Global.FrameworkOnCore.FrameworkNumbers.Formattable(d):F1}", written);
        Assert.Contains("String.Format(\"{0}\", Global.FrameworkOnCore.FrameworkNumbers.Formattable(d))", written);
    }

    [Fact]
    public void Visual_Basic_runtime_functions()
    {
        var written = RewriteHarness.VisualBasic("""
            Imports Microsoft.VisualBasic
            Public Class C
                Function Build(d As Double, s As String) As String
                    Dim x As Double = CDbl(s)
                    Dim y As Double = s
                    Return Str(d) & Format(d, "Fixed") & FormatNumber(d, 2) & FormatCurrency(d, 0, , TriState.True) & FormatPercent(d) & Asc("a").ToString()
                End Function
            End Class
            """);
        Assert.True(written.Contains("CDbl(Global.FrameworkOnCore.VisualBasicText.ToDouble(s))"), written);
        Assert.Contains("Dim y As Double = Global.FrameworkOnCore.VisualBasicText.ToDouble(s)", written);
        Assert.Contains("Global.FrameworkOnCore.VisualBasicText.Str(d)", written);
        Assert.Contains("Global.FrameworkOnCore.VisualBasicText.Format(d, \"Fixed\")", written);
        Assert.Contains("Global.FrameworkOnCore.VisualBasicText.FormatNumber(d, 2)", written);
        Assert.Contains("Global.FrameworkOnCore.VisualBasicText.FormatCurrency(d, 0, , TriState.True)", written);
        Assert.Contains("Global.FrameworkOnCore.VisualBasicText.Asc(\"a\")", written);
    }

    [Fact]
    public void The_Visual_Basic_runtime_as_Framework()
    {
        // en-US as Windows has it (NLS: two decimals; this test process's ICU data has three).
        var us = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.GetCultureInfo("en-US").Clone();
        us.NumberFormat.NumberDecimalDigits = us.NumberFormat.CurrencyDecimalDigits = us.NumberFormat.PercentDecimalDigits = 2;
        var saved = System.Globalization.CultureInfo.CurrentCulture;
        System.Globalization.CultureInfo.CurrentCulture = us;
        try
        {
            Assert.Equal(" .3", VisualBasicText.Str(0.1 + 0.2));
            Assert.Equal("-.5", VisualBasicText.Str(-0.5));
            Assert.Equal("2.68", VisualBasicText.Format(2.675, "Fixed"));
            Assert.Equal("12.50%", VisualBasicText.Format(0.125, "Percent"));
            Assert.Equal("0.00", VisualBasicText.FormatNumber(-0.004, 2));
            Assert.Equal("$23", VisualBasicText.FormatCurrency(22.5, 0));
            Assert.Equal("3/4/2026", VisualBasicText.Format(new DateTime(2026, 3, 4), "Short Date"));
            Assert.Throws<OverflowException>(() => VisualBasicText.ToDouble("1e400"));
            Assert.Equal(16.0, VisualBasicText.ToDouble("&H10"));
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = saved;
        }
    }

    /// <summary>StrConv away from Windows (.NET throws there) against Windows' own (LCMapString), Japanese.</summary>
    [SkippableFact]
    public void StrConv_elsewhere_as_Windows()
    {
        Skip.IfNot(OperatingSystem.IsWindows());
        // The runtime's StrConv encodes in the locale's code page (932), as FrameworkOnCore's System.Web registers at start.
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        var samples = new List<string> { "abc ABC 123 !~\\", "ｶﾞｷﾞｸﾞｹﾞｺﾞ ﾊﾟﾋﾟﾌﾟﾍﾟﾎﾟ ｳﾞ ｶﾞﾟ ﾞ ｱﾞ", "ガギグゲゴ パピプペポ ヴ ヷ", "ひらがな ゝゞ カタカナ ヽヾ ー", "漢字とｶﾅと1２3" };
        samples.Add(new string(Enumerable.Range(0xFF61, 0x3F).Select(i => (char)i).ToArray()));
        samples.Add(new string(Enumerable.Range(0x3041, 0x56).Select(i => (char)i).ToArray()));
        samples.Add(new string(Enumerable.Range(0x30A1, 0x5A).Select(i => (char)i).ToArray()));
        samples.Add(new string(Enumerable.Range(0xFF01, 0x5E).Select(i => (char)i).ToArray()));
        var conversions = new[] { Microsoft.VisualBasic.VbStrConv.Wide, Microsoft.VisualBasic.VbStrConv.Narrow, Microsoft.VisualBasic.VbStrConv.Katakana,
            Microsoft.VisualBasic.VbStrConv.Hiragana, Microsoft.VisualBasic.VbStrConv.Wide | Microsoft.VisualBasic.VbStrConv.Katakana,
            Microsoft.VisualBasic.VbStrConv.Narrow | Microsoft.VisualBasic.VbStrConv.Katakana, Microsoft.VisualBasic.VbStrConv.Uppercase | Microsoft.VisualBasic.VbStrConv.Wide };
        var differences = new List<string>();
        foreach (var conversion in conversions)
            foreach (var s in samples)
            {
                var windows = Microsoft.VisualBasic.Strings.StrConv(s, conversion, 0x411);
                var ours = VisualBasicText.StrConvElsewhere(s, conversion, 0x411);
                if (windows != ours) differences.Add($"{conversion} [{s}]: windows [{windows}] ours [{ours}]");
            }
        Assert.True(differences.Count == 0, string.Join("\n", differences));
    }

    [Fact]
    public void Parsing_and_TimeSpan()
    {
        var written = CSharp("""
            double F(string s, out double d) => double.Parse(s) + (double.TryParse(s, out d) ? 1 : 0) + float.Parse(s, System.Globalization.CultureInfo.InvariantCulture)
                + TimeSpan.FromSeconds(0.5).TotalMilliseconds + TimeSpan.FromMilliseconds(1.5).Ticks + TimeSpan.FromSeconds(30).Ticks;
            """);
        Assert.Contains("global::FrameworkOnCore.FrameworkParsing.ParseDouble(s)", written);
        Assert.Contains("global::FrameworkOnCore.FrameworkParsing.TryParseDouble(s, out d)", written);
        Assert.Contains("global::FrameworkOnCore.FrameworkParsing.ParseSingle(s, System.Globalization.CultureInfo.InvariantCulture)", written);
        Assert.Contains("global::FrameworkOnCore.FrameworkParsing.FromSeconds(0.5)", written);
        Assert.Contains("global::FrameworkOnCore.FrameworkParsing.FromMilliseconds(1.5)", written);
        // An integer: .NET's TimeSpan.FromSeconds(long), exact as .NET Framework's.
        Assert.Contains("TimeSpan.FromSeconds(30)", written);
    }

    // A property of an object replaced by a call with it (platformReplacements: an instance property, "call").
    [Fact]
    public void Culture_names_and_parents()
    {
        var written = CSharp("""
            string F(System.Globalization.CultureInfo c) => c.DisplayName + System.Globalization.CultureInfo.CurrentUICulture.Parent.Name + $"{c.DisplayName}";
            """);
        Assert.Contains("global::FrameworkOnCore.FrameworkCultures.DisplayName(c)", written);
        Assert.Contains("global::FrameworkOnCore.FrameworkCultures.Parent(System.Globalization.CultureInfo.CurrentUICulture).Name", written);
        var vb = RewriteHarness.VisualBasic("""
            Public Class C
                Function F(c As System.Globalization.CultureInfo) As String
                    Return c.DisplayName & c.Parent.Name
                End Function
            End Class
            """);
        Assert.Contains("Global.FrameworkOnCore.FrameworkCultures.DisplayName(c)", vb);
        Assert.Contains("Global.FrameworkOnCore.FrameworkCultures.Parent(c).Name", vb);
        Assert.Equal("zh-CHS", FrameworkCultures.Parent(System.Globalization.CultureInfo.GetCultureInfo("zh-CN")).Name);
        Assert.Equal("zh-CHT", FrameworkCultures.Parent(System.Globalization.CultureInfo.GetCultureInfo("zh-HK")).Name);
        Assert.Equal("de", FrameworkCultures.Parent(System.Globalization.CultureInfo.GetCultureInfo("de-DE")).Name);
    }

    [Fact]
    public void Files_listed_as_on_Windows()
    {
        var written = CSharp("""
            string[] F(string d) => System.IO.Directory.GetFiles(d, "*.xls").Concat(new System.IO.DirectoryInfo(d).GetFiles().Select(f => f.Name)).Concat(System.IO.Path.GetInvalidFileNameChars().Select(c => c.ToString())).ToArray();
            """);
        // (the path from data on the platform: WindowsPath.Native, FOC1004)
        Assert.Contains("global::FrameworkOnCore.FrameworkFiles.GetFiles(global::FrameworkOnCore.WindowsPath.Native(d), \"*.xls\")", written);
        Assert.Contains("global::FrameworkOnCore.FrameworkFiles.GetFiles(new System.IO.DirectoryInfo(global::FrameworkOnCore.WindowsPath.Native(d)))", written);
        Assert.Contains("global::FrameworkOnCore.FrameworkFiles.GetInvalidFileNameChars()", written);
        var folder = Directory.CreateTempSubdirectory("foc-files").FullName;
        try
        {
            foreach (var name in new[] { "b.txt", "A.txt", "book.xls", "book.xlsx", ".hidden", "noext", "page.html" })
                File.WriteAllText(Path.Combine(folder, name), name);
            Assert.Equal(new[] { "book.xls", "book.xlsx" }, FrameworkFiles.GetFiles(folder, "*.xls").Select(Path.GetFileName));
            Assert.Equal(new[] { "page.html" }, FrameworkFiles.GetFiles(folder, "*.htm").Select(Path.GetFileName));
            Assert.Equal(new[] { ".hidden", "noext" }, FrameworkFiles.GetFiles(folder, "*.").Select(Path.GetFileName));
            Assert.Equal(new[] { ".hidden", "A.txt", "b.txt", "book.xls", "book.xlsx", "noext", "page.html" }, FrameworkFiles.GetFiles(folder).Select(Path.GetFileName));
            Assert.Equal(36, FrameworkFiles.GetInvalidPathChars().Length);
            Assert.Equal(41, FrameworkFiles.GetInvalidFileNameChars().Length);
            Assert.Equal("0.70000000000000007", FrameworkNumbers.XmlText(0.7000000000000001));
            Assert.Equal("-INF", FrameworkNumbers.XmlText(double.NegativeInfinity));
        }
        finally { Directory.Delete(folder, true); }
    }

    [Fact]
    public void The_framework_formats()
    {
        Assert.Equal("0.3", FrameworkNumbers.ToString(0.1 + 0.2, System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal("0", FrameworkNumbers.ToString(-0.0, System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal("2.68", FrameworkNumbers.ToString(2.675, "N2", System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal("0.00", FrameworkNumbers.ToString(-0.004, "F2", System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal("0.33333333333333331", FrameworkNumbers.ToString(1.0 / 3, "R", System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal("0.3333333", FrameworkNumbers.ToString(1f / 3, System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal("$23", string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:C0}", FrameworkNumbers.Formattable(22.5)));
        Assert.Throws<OverflowException>(() => FrameworkParsing.ParseDouble("1e400", System.Globalization.CultureInfo.InvariantCulture));
        Assert.False(FrameworkParsing.TryParseDouble("1e400", System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _));
        Assert.Equal(TimeSpan.FromMilliseconds(2), FrameworkParsing.FromMilliseconds(1.5));
        Assert.Equal(TimeSpan.Zero, FrameworkParsing.FromSeconds(0.00015));
    }
}
