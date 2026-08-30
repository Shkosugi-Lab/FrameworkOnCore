using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// Verifies the conversion result of the complex sample (OrderAdmin).
/// Covers TemplateField + RowCommand, paging, the Page_Init/PreRender lifecycle,
/// UpdatePanel expansion, and all validator kinds.
/// </summary>
public class OrderAdminTests : WebFormsTestContext
{
    [Fact]
    public void 受注一覧_ライフサイクルとページングが動作する()
    {
        // The count depends on the execution order of other tests (order entry),
        // so compute it dynamically
        var total = OrderAdmin.Models.OrderRepository.Instance.GetAll().Count;
        var page1 = Math.Min(4, total);
        var page2 = Math.Min(4, total - 4);

        var cut = RenderComponent<OrderAdminBlazor.Components.Pages.Default>();

        // Page_Init (ViewState init) -> Page_Load (bind) -> Page_PreRender (summary)
        Assert.Contains($"表示中 {page1} 件", cut.Find("#lblSummary").TextContent);

        // Go to page 2 via the pager -> Page_PreRender re-runs after the event
        cut.FindAll("table.grid td[colspan] a").First(a => a.TextContent == "2").Click();
        Assert.Contains($"表示中 {page2} 件", cut.Find("#lblSummary").TextContent);
    }

    [Fact]
    public void 受注一覧_RadioButtonListの絞り込みとPreRenderの再実行()
    {
        var accepted = OrderAdmin.Models.OrderRepository.Instance.Find("受付").Count;
        var expected = Math.Min(4, accepted);

        var cut = RenderComponent<OrderAdminBlazor.Components.Pages.Default>();

        // Filter to 受付 via the RadioButtonList (WebForms-compatible table layout)
        cut.Find("#rblStatus_1").Change(true);

        // Page_PreRender re-runs after the event and refreshes the summary
        Assert.Contains($"表示中 {expected} 件", cut.Find("#lblSummary").TextContent);
        Assert.DoesNotContain("出荷済", cut.Find("table.grid tbody").TextContent);
    }

    [Fact]
    public void 受注一覧_TemplateFieldのRowCommandがサイドバーを更新する()
    {
        // The sidebar renders into the layout's SectionOutlet, so render the outlet
        // explicitly alongside the page
        var cut = Render(builder =>
        {
            builder.OpenComponent<OrderAdminBlazor.Components.Pages.Default>(0);
            builder.CloseComponent();
            builder.OpenComponent<Microsoft.AspNetCore.Components.Sections.SectionOutlet>(1);
            builder.AddComponentParameter(2, "SectionName", (object)"SidePanel");
            builder.CloseComponent();
        });

        // The LinkButton inside the TemplateField (CommandName="Pick") reaches the
        // GridView's RowCommand
        cut.FindAll("table.grid a").First(a => a.TextContent == "選択").Click();

        // The ContentPlaceHolder is a naming container, so the DOM id carries its ID.
        // A section is rendered away from where it is written, so unlike the @Body route
        // it carries the prefix explicitly and therefore shows up here without a layout.
        Assert.Contains("田中商事 / ボールペン", cut.Find("#SidePanel_lblPicked").TextContent);

        // Also saved into Session
        var session = Services.GetRequiredService<WebForm2Blazor.Components.WebFormsSession>();
        Assert.Equal("田中商事 / ボールペン", session["LastPicked"]);
    }

    [Fact]
    public void 受注詳細_QueryStringと出荷ボタン()
    {
        // Include the QueryString in the NavigationManager's initial URL
        var navigation = Services.GetRequiredService<Bunit.TestDoubles.FakeNavigationManager>();
        navigation.NavigateTo("http://localhost/Detail?id=3");

        var cut = RenderComponent<OrderAdminBlazor.Components.Pages.Detail>();

        Assert.Equal("鈴木工業", cut.Find("#lblCustomer").TextContent);
        Assert.Equal("受付", cut.Find("#lblStatus").TextContent);

        // Ship button (OnClientClick passes through via bUnit's Loose JSInterop)
        cut.Find("#btnShip").Click();
        Assert.Equal("出荷済", cut.Find("#lblStatus").TextContent);

        // Now shipped, the button is Visible=false -> not rendered
        Assert.Empty(cut.FindAll("#btnShip"));

        // Restore state for subsequent tests
        OrderAdmin.Models.OrderRepository.Instance.UpdateStatus(3, "受付");
    }

    [Fact]
    public void 新規受注_全種バリデータが機能する()
    {
        var cut = RenderComponent<OrderAdminBlazor.Components.Pages.Entry>();

        // Trigger insufficient stock (Custom) + email format (RegularExpression) +
        // mismatch (Compare) at the same time
        cut.Find("#txtCustomer").Input("テスト商会");
        cut.Find("#txtQuantity").Input("50");
        cut.Find("#ddlProduct").Change("電子辞書");
        cut.Find("#txtEmail").Input("abc");
        cut.Find("#txtEmailConfirm").Input("def");
        cut.Find("#btnSubmit").Click();

        var summary = cut.Find("#vsEntry").TextContent;
        Assert.Contains("在庫が不足しています。", summary);
        Assert.Contains("メールアドレスの形式が正しくありません。", summary);
        Assert.Contains("メールアドレスが一致しません。", summary);
        Assert.DoesNotContain("数量は 1〜100", summary); // Range passes

        // After fixing, the order is registered and the app redirects to the list
        var before = OrderAdmin.Models.OrderRepository.Instance.GetAll().Count;
        cut.Find("#txtQuantity").Input("5");
        cut.Find("#txtEmail").Input("test@example.com");
        cut.Find("#txtEmailConfirm").Input("test@example.com");
        cut.Find("#btnSubmit").Click();

        Assert.Equal(before + 1, OrderAdmin.Models.OrderRepository.Instance.GetAll().Count);
    }
}
