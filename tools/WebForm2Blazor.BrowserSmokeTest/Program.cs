using Microsoft.Playwright;
using WebForm2Blazor.BrowserSmokeTest;

// Real-browser verification of the converted app.
// bUnit can only verify component logic - it cannot detect a failure to serve
// blazor.web.js or a failed SignalR circuit connection (= the page renders but does
// not respond), so this drives the installed Edge headless and verifies actual
// interaction.
//
// Usage:
//   auto-generated scenario (layer 2): dotnet run -- --url <URL> --auto <smoke-scenario.json>
//   hand-written scenario:             dotnet run -- --url <URL> --scenario hello|product

var url = "http://localhost:5080/";
string? scenario = null;
string? autoScenarioPath = null;

for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--url": url = args[++i]; break;
        case "--scenario": scenario = args[++i]; break;
        case "--auto": autoScenarioPath = args[++i]; break;
        default:
            Console.Error.WriteLine($"不明な引数: {args[i]}");
            return 1;
    }
}

if (scenario is null && autoScenarioPath is null)
{
    scenario = "hello";
}

var checks = new CheckCollector();

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = true });
var page = await browser.NewPageAsync();

// Capture resource 4xx/5xx with the URL (the console's "Failed to load resource"
// carries no URL and cannot be told apart from a favicon 404)
page.Response += (_, response) =>
{
    if (response.Status >= 400 && !response.Url.Contains("favicon"))
    {
        checks.Fail($"HTTP {response.Status}: {response.Url}");
    }
};
page.PageError += (_, error) => checks.Fail($"ページエラー: {error}");
page.RequestFailed += (_, request) =>
{
    // /_blazor/disconnect is the disconnect notice sent on page navigation; being
    // aborted by the navigation is normal behavior. Do not treat it as an error.
    if (!request.Url.Contains("/_blazor/disconnect"))
    {
        checks.Fail($"リクエスト失敗: {request.Url} {request.Failure}");
    }
};
page.Console += (_, message) =>
{
    // "Failed to load resource" carries no URL and cannot be told apart from favicon.
    // Resource failures are caught with URLs by the Response handler above, so ignore here.
    if (message.Type == "error" && !message.Text.StartsWith("Failed to load resource", StringComparison.Ordinal))
    {
        checks.Fail($"ブラウザコンソールエラー: {message.Text}");
    }
};

// Always answer confirm() etc. with "OK" (the default auto-dismiss would make
// buttons with OnClientClick="return confirm(...)" unverifiable)
page.Dialog += (_, dialog) => dialog.AcceptAsync();

Console.WriteLine($"接続先: {url}  シナリオ: {autoScenarioPath ?? scenario}");

try
{
    if (autoScenarioPath is not null)
    {
        await AutoSmoke.RunAsync(page, url, autoScenarioPath, checks);
    }
    else if (scenario == "product")
    {
        await RunProductAdminScenario(page, url, checks);
    }
    else
    {
        await RunHelloScenario(page, url, checks);
    }
}
catch (Exception exception)
{
    checks.Fail($"シナリオ実行中に例外: {exception.Message}");
}

Console.WriteLine();
Console.WriteLine(checks.Report());
return checks.HasFailures ? 1 : 0;

static async Task RunHelloScenario(IPage page, string url, CheckCollector checks)
{
    await page.GotoAsync(url, new() { WaitUntil = WaitUntilState.NetworkIdle });
    await page.Locator("#btnGreet").WaitForAsync(new() { Timeout = 15000 });
    await page.WaitForTimeoutAsync(1500); // wait for the SignalR circuit to connect

    await page.FillAsync("#txtName", "太郎");
    await page.ClickAsync("#btnGreet");
    await page.WaitForTimeoutAsync(1000);

    checks.Equal("クリック後の挨拶", await page.InnerTextAsync("#lblResult"), "こんにちは、太郎 さん！");
}

static async Task RunProductAdminScenario(IPage page, string url, CheckCollector checks)
{
    var baseUrl = url.TrimEnd('/');

    // --- List page ---
    await page.GotoAsync(baseUrl + "/", new() { WaitUntil = WaitUntilState.NetworkIdle });

    // Page_Load runs after the first render, so wait for the data to appear
    await page.Locator("table.grid tbody tr").First.WaitForAsync(new() { Timeout = 20000 });

    // Master page (layout) applied?
    checks.Equal("マスターページのヘッダー(Web.config の SiteTitle 由来)",
        await page.InnerTextAsync("header.site-header h1"), "商品管理システム");
    checks.Contains("マスターページのフッター",
        await page.InnerTextAsync("footer.site-footer"), "Shkosugi Lab");

    // User control (.ascx)
    checks.Contains("ユーザーコントロールのタイトル(親から渡したプロパティ)",
        await page.InnerTextAsync("div.summary"), "在庫サマリー");
    checks.Contains("ユーザーコントロールの集計",
        await page.InnerTextAsync("div.summary"), "登録商品数: 4");
    checks.Contains("ユーザーコントロールの在庫数",
        await page.InnerTextAsync("div.summary"), "在庫あり: 3");

    // GridView (BoundField + DataFormatString)
    checks.Equal("GridView の行数", (await page.Locator("table.grid tbody tr").CountAsync()).ToString(), "4");
    checks.Equal("GridView のヘッダー",
        await page.InnerTextAsync("table.grid thead tr"), "商品名\tカテゴリ\t価格");
    checks.Equal("価格昇順で先頭はボールペン",
        await page.InnerTextAsync("table.grid tbody tr:first-child td:first-child"), "ボールペン");
    checks.Equal("DataFormatString が効いている",
        await page.InnerTextAsync("table.grid tbody tr:first-child td:last-child"), "150 円");
    checks.Contains("件数ラベル", await page.InnerTextAsync("#lblCount"), "4 件を表示中");

    // Repeater (HeaderTemplate/ItemTemplate/FooterTemplate)
    checks.Equal("Repeater の件数(Web.config の RecentItemCount=3)",
        (await page.Locator("ul.recent li").CountAsync()).ToString(), "3");

    // Filter via DropDownList (AutoPostBack -> Blazor event)
    await page.SelectOptionAsync("#ddlCategory", "文具");
    await page.WaitForTimeoutAsync(800);
    checks.Equal("カテゴリ「文具」で絞り込み",
        (await page.Locator("table.grid tbody tr").CountAsync()).ToString(), "2");

    await page.SelectOptionAsync("#ddlCategory", "");
    await page.WaitForTimeoutAsync(800);

    // Filter via CheckBox
    await page.CheckAsync("#chkInStockOnly");
    await page.WaitForTimeoutAsync(800);
    checks.Equal("在庫ありのみで絞り込み",
        (await page.Locator("table.grid tbody tr").CountAsync()).ToString(), "3");

    await page.UncheckAsync("#chkInStockOnly");
    await page.WaitForTimeoutAsync(800);

    // Sort button (ViewState read/write)
    await page.ClickAsync("#btnSort");
    await page.WaitForTimeoutAsync(800);
    checks.Equal("ViewState による降順切り替えで先頭が電子辞書",
        await page.InnerTextAsync("table.grid tbody tr:first-child td:first-child"), "電子辞書");

    await page.ClickAsync("#btnSort");
    await page.WaitForTimeoutAsync(800);
    checks.Equal("もう一度押すと昇順に戻る(ViewState が保持されている)",
        await page.InnerTextAsync("table.grid tbody tr:first-child td:first-child"), "ボールペン");

    // --- Edit page (does the HyperLink's ~/Edit.aspx resolve?) ---
    await page.ClickAsync("#lnkNew");
    await page.WaitForURLAsync("**/Edit", new() { Timeout = 15000 });
    await page.Locator("#btnSave").WaitForAsync(new() { Timeout = 15000 });
    await page.WaitForTimeoutAsync(1000);

    checks.Contains("QueryString なしのときは登録モード", await page.InnerTextAsync("h2"), "商品登録");
    checks.Equal("DropDownList の DataBind による選択肢",
        (await page.Locator("#ddlCategory option").CountAsync()).ToString(), "2");

    // Is <asp:Content ContentPlaceHolderID="head"> placed into <head>?
    var headStyles = await page.Locator("head style").CountAsync();
    checks.True("head 用 Content が HeadContent として <head> に出力されている", headStyles > 0);

    // Validation (RequiredFieldValidator + ValidationSummary + Page.IsValid)
    await page.ClickAsync("#btnSave");
    await page.WaitForTimeoutAsync(800);
    checks.Contains("未入力で保存するとエラー一覧が出る",
        await page.InnerTextAsync("#vsMain"), "商品名は必須です。");
    checks.Equal("バリデータの印が出る", await page.InnerTextAsync("#rfvName"), "*");
    checks.True("Page.IsValid が false なので遷移しない", page.Url.Contains("/Edit"));

    // Save -> Response.Redirect("Default.aspx")
    await page.FillAsync("#txtName", "電卓");
    await page.FillAsync("#txtPrice", "2480");
    await page.SelectOptionAsync("#ddlCategory", "家電");
    await page.CheckAsync("#chkInStock");
    await page.ClickAsync("#btnSave");

    await page.WaitForURLAsync("**/Default", new() { Timeout = 15000 });
    await page.Locator("table.grid tbody tr").First.WaitForAsync(new() { Timeout = 20000 });
    await page.WaitForTimeoutAsync(800);

    checks.Equal("保存後に一覧へリダイレクトされ 5 件になる",
        (await page.Locator("table.grid tbody tr").CountAsync()).ToString(), "5");
    checks.Contains("保存した商品が一覧にある",
        await page.InnerTextAsync("table.grid tbody"), "電卓");
}
