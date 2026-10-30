using System.Text;
using System.Xml.Linq;
using FrameworkOnCore.Analysis;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Sources in Shift_JIS (code page 932) without a byte order mark, as a Japanese Windows' Visual Studio saved them, and the
/// others: read as the original build read them (a byte order mark's encoding; UTF-8 when valid; else the project's
/// CodePage or the machine's ANSI code page), written back in their encoding with their byte order mark as it was and the
/// bytes of the lines not rewritten as they were (SourceFile); the converted project told the code page (CodePage, the
/// runtime's FrameworkOnCore.AnsiCodePage); web.config in its declared encoding (XmlFiles).
/// </summary>
[Collection(nameof(SourceEncodingTests))]
public sealed class SourceEncodingTests : IDisposable
{
    static readonly Encoding ShiftJis;
    static SourceEncodingTests()
    {
        SourceEncoding.Register();
        ShiftJis = Encoding.GetEncoding(932);
    }

    readonly string root = Path.Combine(Path.GetTempPath(), "foc-encoding-" + Guid.NewGuid().ToString("N"));
    readonly int? machine = SourceEncoding.MachineCodePage;

    // The tests here take the machine's code page as a Japanese Windows' (restored after).
    public SourceEncodingTests() => SourceEncoding.MachineCodePage = 932;

    public void Dispose()
    {
        SourceEncoding.MachineCodePage = machine;
        try { Directory.Delete(root, recursive: true); } catch (IOException) { }
    }

    const string Code = "// 表示能力のソフト\r\nclass 表 { string s = \"ソ\\\\十\"; }\r\n// 末尾の行:申込表\r\n";

    static byte[] Bytes(params object[] parts) => parts.SelectMany(p => p switch
    {
        byte[] b => b,
        string s => ShiftJis.GetBytes(s),
        _ => throw new ArgumentException(),
    }).ToArray();

    [Fact]
    public void Shift_jis_without_a_byte_order_mark_is_read_in_the_code_page()
    {
        var bytes = ShiftJis.GetBytes(Code);
        var read = SourceFile.Decode(bytes, 932);
        Assert.Equal(SourceEncodingKind.CodePage, read.Kind);
        Assert.Equal(Code, read.Text);
        // As File.ReadAllText read it: U+FFFD, and the second byte of 表 (0x5C) a backslash that escapes the quote.
        Assert.Contains((char)0xFFFD, Encoding.UTF8.GetString(bytes));
        // The machine's code page where none is given.
        Assert.Equal(Code, SourceFile.Decode(bytes).Text);
    }

    [Fact]
    public void Without_a_code_page_it_is_read_as_the_compiler_reads_it_and_its_bytes_kept()
    {
        SourceEncoding.MachineCodePage = null;  // Linux: no ANSI code page
        var bytes = ShiftJis.GetBytes(Code);
        var read = SourceFile.Decode(bytes);
        Assert.Equal(SourceEncodingKind.NotUtf8, read.Kind);
        Assert.Contains((char)0xFFFD, read.Text);
        var rewritten = read.Text.Replace("class ", "sealed class ");
        var written = read.Encode(rewritten);
        // The comments' lines, not rewritten, as they were.
        Assert.Equal(ShiftJis.GetBytes("// 表示能力のソフト\r\n"), written.Take(ShiftJis.GetByteCount("// 表示能力のソフト\r\n")));
        Assert.EndsWith("// 末尾の行:申込表\r\n", ShiftJis.GetString(written));
    }

    [Fact]
    public void A_rewritten_shift_jis_file_stays_shift_jis_its_other_lines_byte_for_byte()
    {
        // 0xED40 (纊): NEC's selection of IBM's extensions, which 932 reads as U+7E8A and writes as IBM's 0xFA5C.
        var original = Bytes("// 先頭の行:", new byte[] { 0xED, 0x40 }, "\r\nclass A\r\n{\r\n    void M() { Old(); }\r\n}\r\n// 表示\r\n");
        var read = SourceFile.Decode(original, 932);
        Assert.Equal(SourceEncodingKind.CodePage, read.Kind);
        Assert.NotEqual(original, ShiftJis.GetBytes(read.Text));  // what writing it whole would do
        var rewritten = read.Text.Replace("    void M() { Old(); }\r\n", "    void M()\r\n    {\r\n        throw new PlatformNotSupportedException(\"使えない\");\r\n    }\r\n");
        var written = read.Encode(rewritten);
        Assert.Equal(original.Take(16), written.Take(16));  // the first line with 0xED40
        Assert.Equal(new byte[] { 0xED, 0x40 }, written.Skip(12).Take(2));
        Assert.Equal(rewritten, SourceFile.Decode(written, 932).Text);
        Assert.Throws<DecoderFallbackException>(() => new UTF8Encoding(false, true).GetString(written));
        Assert.True(IndexOf(written, ShiftJis.GetBytes("\"使えない\"")) > 0);
    }

    [Fact]
    public void A_file_replaced_whole_is_in_its_encoding()
    {
        var read = SourceFile.Decode(ShiftJis.GetBytes(Code), 932);
        var written = read.Encode("// Excluded by FrameworkOnCore: 表示\n");
        Assert.Equal(ShiftJis.GetBytes("// Excluded by FrameworkOnCore: 表示\n"), written);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Utf8_keeps_its_byte_order_mark_or_its_absence(bool bom)
    {
        var text = "// 表示\nclass A { }\n";
        var bytes = (bom ? new byte[] { 0xEF, 0xBB, 0xBF } : []).Concat(Encoding.UTF8.GetBytes(text)).ToArray();
        var read = SourceFile.Decode(bytes, 932);
        Assert.Equal(bom ? SourceEncodingKind.Bom : SourceEncodingKind.Utf8, read.Kind);
        Assert.Equal(text, read.Text);
        var written = read.Encode(text.Replace("class A", "class B"));
        Assert.Equal(bom, written.Take(3).SequenceEqual(new byte[] { 0xEF, 0xBB, 0xBF }));
        Assert.Equal(text.Replace("class A", "class B"), Encoding.UTF8.GetString(written, bom ? 3 : 0, written.Length - (bom ? 3 : 0)));
    }

    [Fact]
    public void Utf16_with_its_byte_order_mark_stays_utf16()
    {
        var text = "' 表示\r\nModule M\r\nEnd Module\r\n";
        var bytes = Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(text)).ToArray();
        var read = SourceFile.Decode(bytes, 932);
        Assert.Equal(SourceEncodingKind.Bom, read.Kind);
        var written = read.Encode(text.Replace("Module M", "Module N"));
        Assert.Equal(Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(text.Replace("Module M", "Module N"))), written);
    }

    [Fact]
    public void Lines_inserted_and_removed_keep_the_others_bytes()
    {
        var lines = Enumerable.Range(0, 300).Select(i => $"// 行 {i}:表示\r\n").ToArray();
        var original = ShiftJis.GetBytes(string.Concat(lines));
        var read = SourceFile.Decode(original, 932);
        var changed = lines.ToList();
        changed.RemoveRange(100, 3);
        changed.Insert(100, "// 新しい行\r\n");
        changed[250] = "// 変えた行\r\n";
        changed.Add("// 最後に足した行\r\n");
        var written = read.Encode(string.Concat(changed));
        Assert.Equal(ShiftJis.GetBytes(string.Concat(changed)), written);
    }

    // ------------------------------------------------------------------ the converter

    void Write(string relative, byte[] bytes)
    {
        var path = Path.Combine(root, "source", relative.Replace('\\', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, bytes);
    }

    string Output(string relative) => Path.Combine(root, "output", relative.Replace('\\', Path.DirectorySeparatorChar));

    static string WebProject(string compile, string properties = "") => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
          <PropertyGroup>
            <ProjectGuid>{"{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}"}</ProjectGuid>
            <ProjectTypeGuids>{"{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}"}</ProjectTypeGuids>
            <OutputType>Library</OutputType>
            <AssemblyName>Web</AssemblyName>
            <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
            {properties}
          </PropertyGroup>
          <ItemGroup>
        {compile}
          </ItemGroup>
          <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
        </Project>
        """;

    // web.config in Shift_JIS, with an assembly the charts' choice "none" leaves out (the file is written back).
    const string WebConfig = """
        <?xml version="1.0" encoding="shift_jis"?>
        <!-- 日本語の web.config:表示能力 -->
        <configuration>
          <appSettings><add key="Title" value="ソフト表示" /></appSettings>
          <system.web>
            <compilation targetFramework="4.8">
              <assemblies>
                <add assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" />
              </assemblies>
            </compilation>
            <globalization requestEncoding="shift_jis" responseEncoding="shift_jis" />
          </system.web>
        </configuration>
        """;

    (ProjectConverter Converter, Report Report) Converter(Rules? rules = null)
    {
        Paths.CopyTree(Path.Combine(root, "source"), Path.Combine(root, "output"));
        var report = new Report();
        return (new ProjectConverter(rules ?? RewriteHarness.Rules, report, new Conditions("Debug", "AnyCPU", report), Path.Combine(root, "source"),
            Path.Combine(root, "output"), new RuntimeLayout(Path.Combine(root, "feed"), Array.Empty<string>())), report);
    }

    static Rules WithoutCharts => RewriteHarness.Rules.Choose(System.Text.Json.JsonSerializer.Deserialize<Choices>(
        """{ "components": { "charts": "none" } }""", Choices.Json)!, Catalog.Default());

    [Fact]
    public void Shift_jis_sources_get_the_code_page_written_and_the_server_told_it()
    {
        Write(@"Web\Default.aspx.cs", ShiftJis.GetBytes(Code));
        Write(@"Web\Utf8.cs", Encoding.UTF8.GetBytes("// UTF-8 の行\nclass U { }\n"));
        Write(@"Web\Ascii.cs", Encoding.ASCII.GetBytes("class Plain { }\n"));
        Write(@"Web\Default.aspx", ShiftJis.GetBytes("<%@ Page Language=\"C#\" %>\r\n<p>表示能力</p>\r\n"));
        Write(@"Web\web.config", ShiftJis.GetBytes(WebConfig));
        Write(@"Web\Web.csproj", Encoding.UTF8.GetBytes(WebProject("""
                <Compile Include="Default.aspx.cs" />
                <Compile Include="Utf8.cs" />
                <Compile Include="Ascii.cs" />
            """)));
        var (converter, report) = Converter(WithoutCharts);
        var web = converter.Convert(Path.Combine(root, "source", "Web", "Web.csproj"), isWeb: true);

        Assert.Equal(932, web.CodePage);
        var project = XDocument.Load(Output(@"Web\Web.csproj"));
        Assert.Equal("932", project.Descendants("CodePage").Single().Value);
        Assert.Contains(project.Descendants("RuntimeHostConfigurationOption"), o => (string?)o.Attribute("Include") == ProjectConverter.AnsiCodePageOption && (string?)o.Attribute("Value") == "932");
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Project && e.Text.Contains("CodePage 932 written") && e.Text.Contains("Default.aspx.cs"));
        // The sources kept as they are; the UTF-8 one with Japanese given a byte order mark (CodePage is for every file without one).
        Assert.Equal(ShiftJis.GetBytes(Code), File.ReadAllBytes(Output(@"Web\Default.aspx.cs")));
        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }.Concat(Encoding.UTF8.GetBytes("// UTF-8 の行\nclass U { }\n")), File.ReadAllBytes(Output(@"Web\Utf8.cs")));
        Assert.Equal(Encoding.ASCII.GetBytes("class Plain { }\n"), File.ReadAllBytes(Output(@"Web\Ascii.cs")));

        // web.config written back (the charts' assembly left out): Shift_JIS as declared, no byte order mark, its Japanese kept.
        var config = File.ReadAllBytes(Output(@"Web\web.config"));
        Assert.False(config.Take(3).SequenceEqual(new byte[] { 0xEF, 0xBB, 0xBF }));
        var text = ShiftJis.GetString(config);
        Assert.DoesNotContain("DataVisualization", text);
        Assert.Contains("encoding=\"shift_jis\"", text);
        Assert.Contains("日本語の web.config:表示能力", text);
        Assert.Contains("value=\"ソフト表示\"", text);
        Assert.DoesNotContain(report.Entries, e => e.Kind == Report.Kind.Error);
    }

    [Fact] // YAF, nopCommerce: a file of another code page among many UTF-8 ones without a byte order mark
    public void A_few_code_page_files_among_more_utf8_ones_are_made_utf8_instead()
    {
        Write(@"Web\Old.cs", ShiftJis.GetBytes(Code));
        Write(@"Web\A.cs", Encoding.UTF8.GetBytes("// © A\nclass A { }\n"));
        Write(@"Web\B.cs", Encoding.UTF8.GetBytes("// © B\nclass B { }\n"));
        Write(@"Web\Default.aspx", Encoding.UTF8.GetBytes("<p>表示</p>"));
        Write(@"Web\Web.csproj", Encoding.UTF8.GetBytes(WebProject("""
                <Compile Include="Old.cs" />
                <Compile Include="A.cs" />
                <Compile Include="B.cs" />
            """)));
        var (converter, report) = Converter();
        converter.Convert(Path.Combine(root, "source", "Web", "Web.csproj"), isWeb: true);
        var project = File.ReadAllText(Output(@"Web\Web.csproj"));
        Assert.DoesNotContain("<CodePage>", project);
        // The server reads no file in the code page (Old.cs is compiled by the build).
        Assert.DoesNotContain(ProjectConverter.AnsiCodePageOption, project);
        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }.Concat(Encoding.UTF8.GetBytes(Code)), File.ReadAllBytes(Output(@"Web\Old.cs")));
        Assert.Equal(Encoding.UTF8.GetBytes("// © A\nclass A { }\n"), File.ReadAllBytes(Output(@"Web\A.cs")));
        Assert.Contains(report.Entries, e => e.Text.Contains("made UTF-8 with a byte order mark") && e.Text.Contains("Old.cs"));
    }

    [Fact]
    public void Utf8_and_ascii_sources_change_nothing()
    {
        Write(@"Web\Default.aspx.cs", new byte[] { 0xEF, 0xBB, 0xBF }.Concat(Encoding.UTF8.GetBytes(Code)).ToArray());
        Write(@"Web\Utf8.cs", Encoding.UTF8.GetBytes("// UTF-8 の行\nclass U { }\n"));
        Write(@"Web\Default.aspx", Encoding.UTF8.GetBytes("<p>表示</p>"));
        Write(@"Web\Web.csproj", Encoding.UTF8.GetBytes(WebProject("""
                <Compile Include="Default.aspx.cs" />
                <Compile Include="Utf8.cs" />
            """)));
        var (converter, _) = Converter();
        var web = converter.Convert(Path.Combine(root, "source", "Web", "Web.csproj"), isWeb: true);
        var project = File.ReadAllText(Output(@"Web\Web.csproj"));
        Assert.DoesNotContain("<CodePage>", project);
        Assert.DoesNotContain(ProjectConverter.AnsiCodePageOption, project);
        Assert.Equal(Encoding.UTF8.GetBytes("// UTF-8 の行\nclass U { }\n"), File.ReadAllBytes(Output(@"Web\Utf8.cs")));
    }

    [Fact]
    public void The_projects_own_code_page_is_kept()
    {
        var western = Encoding.GetEncoding(1252);
        Write(@"Web\Default.aspx.cs", western.GetBytes("// café, naïve\nclass C { string s = \"é\"; }\n"));
        Write(@"Web\Web.csproj", Encoding.UTF8.GetBytes(WebProject("""    <Compile Include="Default.aspx.cs" />""", "<CodePage>1252</CodePage>")));
        var (converter, report) = Converter();
        var web = converter.Convert(Path.Combine(root, "source", "Web", "Web.csproj"), isWeb: true);
        Assert.Equal(1252, web.CodePage);
        Assert.Equal("1252", XDocument.Load(Output(@"Web\Web.csproj")).Descendants("CodePage").Single().Value);
        Assert.Contains(report.Entries, e => e.Text.Contains("the project's CodePage"));
    }

    [Fact]
    public void Without_a_code_page_known_it_is_reported()
    {
        SourceEncoding.MachineCodePage = null;
        Write(@"Web\Default.aspx.cs", ShiftJis.GetBytes(Code));
        Write(@"Web\Web.csproj", Encoding.UTF8.GetBytes(WebProject("""    <Compile Include="Default.aspx.cs" />""")));
        var (converter, report) = Converter();
        converter.Convert(Path.Combine(root, "source", "Web", "Web.csproj"), isWeb: true);
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Error && e.Text.Contains("--code-page"));
        Assert.DoesNotContain("<CodePage>", File.ReadAllText(Output(@"Web\Web.csproj")));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void An_xml_file_keeps_its_byte_order_mark_or_its_absence(bool bom)
    {
        Directory.CreateDirectory(root);
        var file = Path.Combine(root, "web.config");
        var text = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<configuration><appSettings><add key=\"a\" value=\"表示\" /></appSettings></configuration>";
        File.WriteAllBytes(file, (bom ? new byte[] { 0xEF, 0xBB, 0xBF } : []).Concat(Encoding.UTF8.GetBytes(text)).ToArray());
        var document = XmlFiles.Load(file, LoadOptions.PreserveWhitespace, out var original);
        document.Root!.Element("appSettings")!.Add(new XElement("add", new XAttribute("key", "b")));
        XmlFiles.Save(document, file, SaveOptions.DisableFormatting, original);
        var written = File.ReadAllBytes(file);
        Assert.Equal(bom, written.Take(3).SequenceEqual(new byte[] { 0xEF, 0xBB, 0xBF }));
        Assert.Contains("value=\"表示\" /><add key=\"b\" />", Encoding.UTF8.GetString(written));
    }

    // ------------------------------------------------------------------ the build's fixes (dotnet build)

    [Fact]
    public void A_stubbed_shift_jis_file_is_written_back_in_shift_jis()
    {
        var directory = Path.Combine(root, "build");
        Directory.CreateDirectory(directory);
        var project = Path.Combine(directory, "App.csproj");
        File.WriteAllText(project, "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType><CodePage>932</CodePage></PropertyGroup></Project>");
        var program = Path.Combine(directory, "Program.cs");
        var source = "// 表示能力のソフト:ソ十\r\nclass Program\r\n{\r\n    static void Main() { System.Console.WriteLine(\"表示:\" + 表.ソ); }\r\n\r\n    // 呼ばれない\r\n    static bool 古い() { return System.EnterpriseServices.ContextUtil.IsInTransaction; }\r\n}\r\nstatic class 表 { public const string ソ = \"ソフト\\\\表\"; }\r\n";
        var original = Bytes("// ", new byte[] { 0xED, 0x40 }, "\r\n", source);
        File.WriteAllBytes(program, original);
        var report = new Report();
        var converted = new ConvertedProject("App", project, project, false, [], "App", CodePage: 932);
        Assert.True(new BuildFixer(report, [converted], directory, RewriteHarness.Rules).Run(project), string.Join("\n", report.Entries.Select(e => e.Subject + ": " + e.Text)));
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Stub && e.Subject.Contains("古い()"));

        var written = File.ReadAllBytes(program);
        Assert.Throws<DecoderFallbackException>(() => new UTF8Encoding(false, true).GetString(written));
        Assert.Equal(original.Take(5), written.Take(5));  // "// " and 0xED40 as they were
        var text = ShiftJis.GetString(written);
        Assert.Contains("static bool 古い()", text);
        Assert.Contains("PlatformNotSupportedException", text);
        Assert.Contains("static class 表 { public const string ソ = \"ソフト\\\\表\"; }", text);
        Assert.Contains("// 表示能力のソフト:ソ十", text);
    }

    [Fact]
    public void A_stubbed_visual_basic_file_in_shift_jis_is_written_back_in_shift_jis()
    {
        var directory = Path.Combine(root, "buildvb");
        Directory.CreateDirectory(directory);
        var project = Path.Combine(directory, "App.vbproj");
        File.WriteAllText(project, "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Library</OutputType><CodePage>932</CodePage></PropertyGroup></Project>");
        var program = Path.Combine(directory, "表示部品.vb");
        var source = "' 表示部品:日本語の名前\r\nPublic Module 表示部品\r\n    Sub Main()\r\n        System.Console.WriteLine(\"ソフト表示\")\r\n    End Sub\r\n\r\n    Function 古い() As Boolean\r\n        Return System.EnterpriseServices.ContextUtil.IsInTransaction\r\n    End Function\r\n\r\n    Public ReadOnly 末尾 As String = \"後ろの行(十分)\"\r\nEnd Module\r\n";
        File.WriteAllBytes(program, ShiftJis.GetBytes(source));
        var report = new Report();
        var converted = new ConvertedProject("App", project, project, false, [], "App", CodePage: 932);
        Assert.True(new BuildFixer(report, [converted], directory, RewriteHarness.Rules).Run(project), string.Join("\n", report.Entries.Select(e => e.Subject + ": " + e.Text)));
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Stub);

        var written = File.ReadAllBytes(program);
        Assert.Throws<DecoderFallbackException>(() => new UTF8Encoding(false, true).GetString(written));
        var text = ShiftJis.GetString(written);
        Assert.Contains("Throw New Global.System.PlatformNotSupportedException", text);
        Assert.Contains("Public ReadOnly 末尾 As String = \"後ろの行(十分)\"", text);
        Assert.StartsWith("' 表示部品:日本語の名前\r\nPublic Module 表示部品\r\n", text);
    }

    static int IndexOf(byte[] haystack, byte[] needle)
    {
        for (var i = 0; i + needle.Length <= haystack.Length; i++)
            if (haystack.AsSpan(i, needle.Length).SequenceEqual(needle)) return i;
        return -1;
    }
}
