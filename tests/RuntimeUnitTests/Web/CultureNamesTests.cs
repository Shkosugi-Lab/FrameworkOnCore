using System.Globalization;
using System.Web.Util;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// FrameworkCultures.ThrowIfRefused: the culture names .NET Framework refuses are refused by System.Web's cultures (Accept-Language
/// with culture="auto", &lt;globalization&gt;, Page.Culture) though .NET with ICU makes a culture of them. The names: .NET Framework 4.8
/// on Windows 11 (Windows PowerShell 5.1, New-Object System.Globalization.CultureInfo(name), 2026-11-02); FrameworkOnCore.Tests'
/// CompatTests has the same for FrameworkOnCore.Compat.
/// </summary>
public class CultureNamesTests
{
    [Theory]
    [InlineData("Auto")] [InlineData("abcd")] [InlineData("abcde")] [InlineData("root")] [InlineData("a")] [InlineData("e1")]
    [InlineData("Auto-XX")] [InlineData("auto:ja")] [InlineData("en-Z")] [InlineData("en-ZZZ")] [InlineData("en-ZZZZZ")]
    [InlineData("en-1")] [InlineData("en-12")] [InlineData("en-1234")] [InlineData("en-1a")] [InlineData("en-US-123456789")]
    [InlineData("en-Latn-ZZZ")] [InlineData("en-Latn-12")] [InlineData("en-Latn-Latn")] [InlineData("x")] [InlineData("i")]
    [InlineData("en-x")] [InlineData("zh-yue")] [InlineData("en-")] [InlineData("-en")] [InlineData("en--US")]
    [InlineData("en_US_POSIX")] [InlineData("en.US")] [InlineData("en@x")]
    public void A_name_net_framework_refuses_throws(string name) =>
        Assert.Throws<CultureNotFoundException>(() => FrameworkCultures.ThrowIfRefused(name));

    [Theory]
    [InlineData("xx-YY")] [InlineData("en-ZZ")] [InlineData("zz")] [InlineData("xyz")] [InlineData("en-XA")] [InlineData("en-Latn-US")]
    [InlineData("en-419")] [InlineData("en-123")] [InlineData("en-ZZZZ")] [InlineData("en-US-posix")] [InlineData("en-US-abcde")]
    [InlineData("qaa-Qaaa-QM")] [InlineData("EN-us")] [InlineData("ja-JP-u-ca-japanese")] [InlineData("en-x-a")] [InlineData("x-a")]
    [InlineData("en-US-a")] [InlineData("ja-JP-mac")] [InlineData("en-ZZ-ZZ")] [InlineData("sgn-BE-FR")] [InlineData("de-DE_phoneb")]
    [InlineData("zh-CHS")] [InlineData("zh-CHT")] [InlineData("ja-JP")] [InlineData("")]
    public void A_name_net_framework_takes_is_taken(string name) => FrameworkCultures.ThrowIfRefused(name);

    [Fact] // Accept-Language "abcd, ja" with culture="auto": .NET Framework went on to ja (abcd refused); ICU made a culture "abcd"
    public void Accept_language_goes_on_past_a_name_net_framework_refuses()
    {
        Assert.Equal("ja-JP", CultureUtil.ExtractCultureImpl(["abcd", "ja;q=0.8"], requireSpecific: true, maxCount: 10).Name);
        Assert.Equal("ja", CultureUtil.ExtractCultureImpl(["en-ZZZ", "ja"], requireSpecific: false, maxCount: 10).Name);
        Assert.Throws<CultureNotFoundException>(() => CultureUtil.ExtractCultureImpl(["abcd"], requireSpecific: true, maxCount: 10));
    }
}
