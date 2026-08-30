using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// Verifies the controls added for the nopCommerce coverage push (HiddenField / Image /
/// RadioButton / CheckBoxList / DataList / field-level styles / TabContainer).
/// Parity cannot see td/th cells (no IDs), so the field styles are asserted here.
/// </summary>
public class NewControlTests : WebFormsTestContext
{
    [Fact]
    public void 変換済みProbeページ_HiddenFieldとImageが描画される()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        Assert.Equal("hidden-value", cut.Find("#pHidden").GetAttribute("value"));
        Assert.Equal("hidden", cut.Find("#pHidden").GetAttribute("type"));

        // alt is omitted when AlternateText is unset (measured on the 4.8 runtime)
        Assert.Equal("/probe.png", cut.Find("#pImage").GetAttribute("src"));
        Assert.False(cut.Find("#pImage").HasAttribute("alt"));
        Assert.Equal("代替テキスト", cut.Find("#pImageAlt").GetAttribute("alt"));
    }

    [Fact]
    public void 変換済みProbeページ_単体RadioButtonのグループが排他になる()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        // Initial state: pRadioBtn2 has Checked="true" in markup
        Assert.False(cut.Find("#pRadioBtn1").HasAttribute("checked"));
        Assert.True(cut.Find("#pRadioBtn2").HasAttribute("checked"));
        Assert.Equal("pg", cut.Find("#pRadioBtn1").GetAttribute("name"));

        // Selecting the sibling clears the previously checked one (GroupName coordination)
        cut.Find("#pRadioBtn1").Change(true);
        Assert.True(cut.Find("#pRadioBtn1").HasAttribute("checked"));
        Assert.False(cut.Find("#pRadioBtn2").HasAttribute("checked"));
    }

    [Fact]
    public void 変換済みProbeページ_CheckBoxListの選択がItemsに反映される()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        Assert.Equal(3, cut.FindAll("#pCheckList input[type=checkbox]").Count);
        Assert.True(cut.Find("#pCheckList_1").HasAttribute("checked"));

        cut.Find("#pCheckList_0").Change(true);
        Assert.True(cut.Find("#pCheckList_0").HasAttribute("checked"));
        // The markup-selected item stays selected (multi-select)
        Assert.True(cut.Find("#pCheckList_1").HasAttribute("checked"));
    }

    [Fact]
    public void 変換済みProbeページ_DataListの縦横レイアウト()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        // Vertical (default): one cell per row; CellSpacing=0 renders border-collapse
        Assert.Equal(2, cut.FindAll("#pDataListV tr").Count);
        Assert.Contains("border-collapse:collapse", cut.Find("#pDataListV").GetAttribute("style"));
        Assert.Equal("0", cut.Find("#pDataListV").GetAttribute("cellspacing"));

        // Horizontal with RepeatColumns=2: one row with two cells
        Assert.Single(cut.FindAll("#pDataListH tr"));
        Assert.Equal(2, cut.FindAll("#pDataListH td").Count);
    }

    [Fact]
    public void 変換済みProbeページ_フィールド単位のItemStyleとHeaderStyleが描画される()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        // Measured 4.8 rendering: HorizontalAlign -> align attribute (lower case),
        // Width -> style="width:...px;"
        var headers = cut.FindAll("#pGridFieldStyles th");
        Assert.Equal("right", headers[0].GetAttribute("align"));
        Assert.Contains("width:80px", headers[1].GetAttribute("style"));

        var firstRowCells = cut.FindAll("#pGridFieldStyles tbody tr")[0].QuerySelectorAll("td");
        Assert.Equal("center", firstRowCells[0].GetAttribute("align"));
        Assert.Contains("width:120px", firstRowCells[0].GetAttribute("style"));
        Assert.Equal("right", firstRowCells[1].GetAttribute("align"));
    }

    [Fact]
    public void 変換済みProbeページ_VerticalAlignとPagerStyleと画像リンク()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        // DataList ItemStyle-VerticalAlign -> valign attribute (measured on 4.8)
        Assert.Equal("top", cut.FindAll("#pDataListVA td")[0].GetAttribute("valign"));

        // GridView PagerStyle-CssClass -> class on the pager tr
        Assert.NotNull(cut.Find("#pGridPager tr.pager-style"));

        // HyperLink ImageUrl -> img inside the anchor, alt = Text
        var image = cut.Find("#pHlImage img");
        Assert.Equal("/probe.png", image.GetAttribute("src"));
        Assert.Equal("画像リンク", image.GetAttribute("alt"));
    }

    [Fact]
    public void 変換済みProbeページ_CheckBoxList列と宣言的データソース()
    {
        var cut = RenderComponent<DefaultsProbeBlazor.Components.Pages.Default>();

        // RepeatColumns=2 with 3 items: 2 rows, trailing cell padded empty (measured)
        var rows = cut.FindAll("#pCheckCols tr");
        Assert.Equal(2, rows.Count);
        Assert.Equal(2, rows[1].QuerySelectorAll("td").Length);
        Assert.Equal("", rows[1].QuerySelectorAll("td")[1].TextContent.Trim());

        // ObjectDataSource + DataSourceID: auto-bound on first render
        Assert.Contains("表", cut.Find("#pGridOds tbody").TextContent);
        Assert.Contains("裏", cut.Find("#pGridOds tbody").TextContent);
    }

    [Fact]
    public void TabContainer_タブ切り替えで表示パネルが変わる()
    {
        var cut = Render(builder =>
        {
            builder.OpenComponent<WebForm2Blazor.Components.TabContainer>(0);
            builder.AddComponentParameter(1, "ID", (object)"tc");
            builder.AddComponentParameter(2, "ChildContent", (RenderFragment)(child =>
            {
                child.OpenComponent<WebForm2Blazor.Components.TabPanel>(0);
                child.AddComponentParameter(1, "ID", (object)"tab1");
                child.AddComponentParameter(2, "HeaderText", (object)"基本");
                child.AddComponentParameter(3, "ChildContent", (RenderFragment)(b => b.AddContent(0, "panel-one")));
                child.CloseComponent();
                child.OpenComponent<WebForm2Blazor.Components.TabPanel>(4);
                child.AddComponentParameter(5, "ID", (object)"tab2");
                child.AddComponentParameter(6, "HeaderText", (object)"詳細");
                child.AddComponentParameter(7, "ChildContent", (RenderFragment)(b => b.AddContent(0, "panel-two")));
                child.CloseComponent();
            }));
            builder.CloseComponent();
        });

        // Both headers render; the first tab is active, the second hidden
        Assert.Equal(2, cut.FindAll(".ajax__tab_header a").Count);
        Assert.DoesNotContain("display:none", cut.Find("#tab1").GetAttribute("style") ?? "");
        Assert.Contains("display:none", cut.Find("#tab2").GetAttribute("style"));

        // Clicking the second header switches the visible panel
        cut.FindAll(".ajax__tab_header a")[1].Click();
        Assert.Contains("display:none", cut.Find("#tab1").GetAttribute("style"));
        Assert.DoesNotContain("display:none", cut.Find("#tab2").GetAttribute("style") ?? "");
    }
}
