using System.Text;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Tests;

/// <summary>
/// A process' output read whichever encoding it writes in (a build's tools on Japanese Windows: UTF-8 from .NET's tools,
/// node and docker; Shift_JIS from nuget.exe, MSBuild.exe, cmd and appcmd): UTF-8's Japanese was shown garbled ("繧、繝ウ")
/// in the original's build log.
/// </summary>
public sealed class ProcessOutputTests
{
    static readonly Encoding ShiftJis = ShiftJisEncoding();

    static Encoding ShiftJisEncoding()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(932);
    }

    static List<string> Lines(byte[] bytes)
    {
        var lines = new List<string>();
        ProcessOutput.ReadLines(new MemoryStream(bytes), lines.Add).Wait();
        return lines;
    }

    [Fact]
    public void Utf8_is_read_as_utf8() =>
        Assert.Equal(["フォールバック パッケージ フォルダー:", "  C:\\x"], Lines(Encoding.UTF8.GetBytes("フォールバック パッケージ フォルダー:\r\n  C:\\x\r\n")));

    [Fact]
    public void Ascii_is_read_as_it_is() =>
        Assert.Equal(["BuildNpmPackages", "", "done"], Lines(Encoding.ASCII.GetBytes("BuildNpmPackages\n\ndone")));

    [Fact]
    public void Lines_ending_with_a_carriage_return_alone_are_lines() =>
        Assert.Equal(["10%", "50%", "100%"], Lines(Encoding.ASCII.GetBytes("10%\r50%\r100%\r\n")));

    [SkippableFact]
    public void The_consoles_code_page_is_read_as_it_where_it_is_not_utf8()
    {
        Skip.IfNot(OperatingSystem.IsWindows() && System.Globalization.CultureInfo.CurrentCulture.TextInfo.OEMCodePage == 932, "Japanese Windows' OEM code page");
        var bytes = ShiftJis.GetBytes("復元に成功しました\r\n").Concat(Encoding.UTF8.GetBytes("ビルドに成功しました\r\n")).ToArray();
        Assert.Equal(["復元に成功しました", "ビルドに成功しました"], Lines(bytes));
    }

    [Fact]
    public void A_line_split_between_reads_is_whole()
    {
        var bytes = Encoding.UTF8.GetBytes(new string('あ', 10000) + "\nend\n");  // more than one read (16 KB)
        Assert.Equal([new string('あ', 10000), "end"], Lines(bytes));
    }
}
