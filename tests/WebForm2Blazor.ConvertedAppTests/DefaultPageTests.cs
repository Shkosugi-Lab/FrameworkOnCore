using Bunit;
using HelloBlazor.Components.Pages;
using Xunit;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// Verifies that the converted Default page behaves the same as the WebForms version.
/// (The minimal form of the behavioral parity test against the pre-conversion app.)
/// </summary>
public class DefaultPageTests : WebFormsTestContext
{
    [Fact]
    public void 初期表示_タイトルとコントロールが描画される()
    {
        var cut = RenderComponent<Default>();

        Assert.Contains("あいさつアプリ", cut.Markup);
        Assert.NotNull(cut.Find("#txtName"));
        Assert.NotNull(cut.Find("#btnGreet"));
        Assert.NotNull(cut.Find("#lblResult"));
    }

    [Fact]
    public void 名前を入力してボタンを押すと挨拶が表示される()
    {
        var cut = RenderComponent<Default>();

        cut.Find("#txtName").Input("太郎");
        cut.Find("#btnGreet").Click();

        Assert.Equal("こんにちは、太郎 さん！", cut.Find("#lblResult").TextContent);
    }

    [Fact]
    public void 入力を変えて再度押すと挨拶が更新される()
    {
        var cut = RenderComponent<Default>();

        cut.Find("#txtName").Input("太郎");
        cut.Find("#btnGreet").Click();
        cut.Find("#txtName").Input("花子");
        cut.Find("#btnGreet").Click();

        Assert.Equal("こんにちは、花子 さん！", cut.Find("#lblResult").TextContent);
    }
}
