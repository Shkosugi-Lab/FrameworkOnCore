using Bunit;
using Xunit;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// Verifies the frequently-used property implementations (DataKeys / row styles /
/// GroupingText / AppendDataBoundItems / Display / TextAlign / Wrap etc.).
/// DOM-level conformance is covered by the DefaultsProbe parity; here the API surface
/// (behavior as seen from code-behind) is verified.
/// </summary>
public class FrequentPropertyTests : WebFormsTestContext
{
    [Fact]
    public async Task GridViewのDataKeysが主キー値を返す()
    {
        var cut = RenderComponent<WebForm2Blazor.Components.GridView>(parameters => parameters
            .Add(grid => grid.DataKeyNames, ["Name"]));

        await cut.InvokeAsync(() =>
        {
            cut.Instance.DataSource = new[]
            {
                new { Name = "甲", Price = 100 },
                new { Name = "乙", Price = 200 },
            };
            cut.Instance.DataBind();
        });

        Assert.Equal(2, cut.Instance.DataKeys.Count);
        Assert.Equal("乙", cut.Instance.DataKeys[1].Value?.ToString());
        Assert.Equal("甲", cut.Instance.DataKeys[0]["Name"]?.ToString());
    }

    [Fact]
    public void 変換済みProbeページ_行スタイルとキャプションが描画される()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        Assert.Equal("スタイル付きグリッド", cut.Find("#pGridStyled caption").TextContent);
        Assert.Equal("head-style", cut.Find("#pGridStyled thead tr").GetAttribute("class"));

        var bodyRows = cut.FindAll("#pGridStyled tbody tr");
        Assert.Equal("row-style", bodyRows[0].GetAttribute("class"));
        Assert.Contains("background-color:#EEEEEE", bodyRows[0].GetAttribute("style"));
        Assert.Equal("alt-style", bodyRows[1].GetAttribute("class"));
    }

    [Fact]
    public void 変換済みProbeページ_GroupingTextとAssociatedControlID()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        // Panel GroupingText: fieldset + legend inside the outer div (which carries the ID)
        Assert.Equal("グループ見出し", cut.Find("#pFieldset fieldset legend").TextContent);

        // Label AssociatedControlID -> <label for>
        var label = cut.Find("#pLabelFor");
        Assert.Equal("label", label.TagName.ToLowerInvariant());
        Assert.Equal("pTextBox", label.GetAttribute("for"));
    }

    [Fact]
    public void 変換済みProbeページ_AppendDataBoundItemsとWrapとTextAlign()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        // AppendDataBoundItems: 1 declarative + 2 bound = 3 options
        Assert.Equal(3, cut.FindAll("#pAppend option").Count);
        Assert.Contains("(選択してください)", cut.Find("#pAppend").TextContent);

        // Wrap="false" -> wrap="off"
        Assert.Equal("off", cut.Find("#pNoWrap").GetAttribute("wrap"));

        // TextAlign="Left" -> the label precedes the input
        var container = cut.Find("#pCheckLeft").ParentElement!;
        Assert.Equal("label", container.Children[0].TagName.ToLowerInvariant());
    }

    [Fact]
    public void 変換済みProbeページ_ValidatorのDisplayとSummaryのHeaderText()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        // Display default (Static): rendered hidden even while valid
        Assert.Contains("visibility:hidden", cut.Find("#pRequired").GetAttribute("style"));

        // Display="Dynamic": not rendered while valid
        Assert.Empty(cut.FindAll("#pRequiredDyn"));

        // Run validation -> both appear, and the List-mode summary gets its HeaderText
        cut.Find("#btnValidate").Click();
        Assert.DoesNotContain("visibility:hidden", cut.Find("#pRequired").GetAttribute("style") ?? "");
        Assert.NotEmpty(cut.FindAll("#pRequiredDyn"));
        Assert.Contains("入力エラー:", cut.Find("#pSummaryList").TextContent);
        Assert.Contains("Dynamic 表示の必須エラー", cut.Find("#pSummaryList").TextContent);
    }
}
