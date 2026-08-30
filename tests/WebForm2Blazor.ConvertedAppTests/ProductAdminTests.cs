using Bunit;
using Xunit;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// Verifies the conversion result of the complex sample (master page / user control /
/// data binding / validation) at the component level.
/// </summary>
public class ProductAdminTests : WebFormsTestContext
{
    public ProductAdminTests()
        : base(new Dictionary<string, string?> { ["AppSettings:RecentItemCount"] = "3" })
    {
    }

    [Fact]
    public void ユーザーコントロールは親から渡されたプロパティと集計を表示する()
    {
        // Folder-preserving output: the source lives in Controls/ProductSummary.ascx,
        // so the component lands in Components/Controls/Controls (folder mirrored)
        var cut = RenderComponent<ProductAdminBlazor.Components.Controls.Controls.ProductSummary>(
            parameters => parameters.Add(control => control.Title, "在庫サマリー"));

        Assert.Contains("在庫サマリー", cut.Markup);
        Assert.Contains("登録商品数:", cut.Markup);
    }

    [Fact]
    public void 一覧ページはGridViewとRepeaterをデータバインドする()
    {
        var cut = RenderComponent<ProductAdminBlazor.Components.Pages.Default>();

        // DataBind runs in Page_Load (OnAfterRender)
        Assert.NotEmpty(cut.FindAll("table.grid tbody tr"));
        Assert.Equal(3, cut.FindAll("ul.recent li").Count);

        // BoundField's DataFormatString is applied
        Assert.Contains(" 円", cut.Find("table.grid tbody tr:first-child td:last-child").TextContent);
    }

    [Fact]
    public void 編集ページは未入力で保存するとPageIsValidがfalseになりエラーを表示する()
    {
        var cut = RenderComponent<ProductAdminBlazor.Components.Pages.Edit>();

        cut.Find("#btnSave").Click();

        Assert.Contains("商品名は必須です。", cut.Find("#vsMain").TextContent);
        Assert.Contains("価格は必須です。", cut.Find("#vsMain").TextContent);
    }

    [Fact]
    public void Repeaterの削除コマンドがItemCommandにバブリングして行が減る()
    {
        var cut = RenderComponent<ProductAdminBlazor.Components.Pages.Default>();

        var rowsBefore = cut.FindAll("table.grid tbody tr").Count;
        var recentBefore = cut.FindAll("ul.recent li").Count;
        Assert.True(recentBefore > 0);

        // Record the deletion target (the first recent item) and restore it after the test
        var firstRecentText = cut.Find("ul.recent li").TextContent;

        // CommandName="Delete" + CommandArgument='<%# Eval("Id") %>' on the LinkButton
        // inside the template reaches the Repeater's ItemCommand
        cut.FindAll("ul.recent li a").First(anchor => anchor.TextContent == "削除").Click();

        Assert.Equal(rowsBefore - 1, cut.FindAll("table.grid tbody tr").Count);

        // Other tests (catalog paging etc.) depend on the product count, so restore state
        RestoreDeletedProduct(firstRecentText);
    }

    private static void RestoreDeletedProduct(string recentItemText)
    {
        var all = ProductAdmin.Models.ProductRepository.Instance.GetAll();
        if (recentItemText.Contains("デスクライト") && all.All(p => p.Name != "デスクライト"))
        {
            ProductAdmin.Models.ProductRepository.Instance.Save(
                new ProductAdmin.Models.Product { Name = "デスクライト", Category = "家電", Price = 4980m, InStock = true });
        }
        else if (all.Count < 4)
        {
            ProductAdmin.Models.ProductRepository.Instance.Save(
                new ProductAdmin.Models.Product { Name = "補充品", Category = "文具", Price = 500m, InStock = true });
        }
    }

    [Fact]
    public void カタログ_GridViewのページングとソートが動作する()
    {
        var cut = RenderComponent<ProductAdminBlazor.Components.Pages.Catalog>();

        // Ordinal ascending by name, PageSize=3 -> the first item on page 1 is デスクライト
        Assert.Equal("デスクライト", cut.Find("table.grid > tbody > tr > td").TextContent.Trim());

        // Pager (WebForms-compatible nested table): current page is a span, others are links
        Assert.Equal("1", cut.Find("table.grid td[colspan] span").TextContent);
        cut.FindAll("table.grid td[colspan] a").First(a => a.TextContent == "2").Click();
        Assert.Contains("電子辞書", cut.Find("table.grid > tbody > tr > td").TextContent);

        // Sort by price (ascending) -> the page also resets to 1
        cut.FindAll("th a").First(a => a.TextContent == "価格").Click();
        Assert.Equal("ボールペン", cut.Find("table.grid > tbody > tr > td").TextContent.Trim());

        // Clicking again sorts descending
        cut.FindAll("th a").First(a => a.TextContent == "価格").Click();
        Assert.Equal("電子辞書", cut.Find("table.grid > tbody > tr > td").TextContent.Trim());
    }

    [Fact]
    public void カタログ_ListViewとFormViewが描画される()
    {
        var cut = RenderComponent<ProductAdminBlazor.Components.Pages.Catalog>();

        // ListView: the ItemTemplate expands into the LayoutTemplate's ul + itemPlaceholder
        Assert.Equal(4, cut.FindAll("ul.price-list li").Count);
        Assert.Contains("ボールペン: 150 円", cut.Find("ul.price-list").TextContent);

        // FormView: the highest-priced product (電子辞書) is shown
        Assert.Contains("電子辞書", cut.Find(".featured").TextContent);
        Assert.Contains("18,800 円", cut.Find(".featured").TextContent);
    }

    [Fact]
    public void 編集ページのRangeValidatorが範囲外の価格を検出する()
    {
        var cut = RenderComponent<ProductAdminBlazor.Components.Pages.Edit>();

        cut.Find("#txtName").Input("テスト商品");
        cut.Find("#txtPrice").Input("0");
        cut.Find("#btnSave").Click();

        Assert.Contains("価格は 1〜999999 の範囲で入力してください。", cut.Find("#vsMain").TextContent);

        // The required check passes (the field is not empty), so no required message
        Assert.DoesNotContain("価格は必須です。", cut.Find("#vsMain").TextContent);
    }

    [Fact]
    public void WebControl共通プロパティとAttributes_Styleが描画される()
    {
        var cut = RenderComponent<ProductAdminBlazor.Components.Pages.Edit>();

        // Markup attributes: Width / ToolTip / ForeColor (effective from the initial render)
        Assert.Contains("width:240px", cut.Find("#txtName").GetAttribute("style"));
        Assert.Contains("width:120px", cut.Find("#txtPrice").GetAttribute("style"));
        Assert.Equal("税込価格を入力してください", cut.Find("#txtPrice").GetAttribute("title"));
        Assert.Equal("入力内容を保存します", cut.Find("#btnSave").GetAttribute("title"));
        Assert.Equal("1", cut.Find("#txtName").GetAttribute("tabindex"));

        // Attributes / Style from code-behind (set in Page_Load)
        Assert.Equal("商品名を入力", cut.Find("#txtName").GetAttribute("placeholder"));
        Assert.Contains("max-width:480px", cut.Find("#pnlForm").GetAttribute("style"));

        // The validator's ForeColor / Font-Bold (rendered when validation fails)
        cut.Find("#btnSave").Click();
        var validatorStyle = cut.Find("#rfvName").GetAttribute("style");
        Assert.Contains("color:Red", validatorStyle);
        Assert.Contains("font-weight:bold", validatorStyle);
    }

    [Fact]
    public void 編集ページはCausesValidationがfalseのボタンでは検証しない()
    {
        var cut = RenderComponent<ProductAdminBlazor.Components.Pages.Edit>();

        // Cancel has CausesValidation="false", so it raises no validation errors
        cut.Find("#btnCancel").Click();

        Assert.Empty(cut.FindAll("#vsMain"));
    }
}
