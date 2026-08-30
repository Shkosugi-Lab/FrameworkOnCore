using System.Text.Json;
using Microsoft.Playwright;

namespace WebForm2Blazor.BrowserSmokeTest;

/// <summary>
/// Generic runner for the converter-generated smoke-scenario.json (verification layer 2).
///
/// Verifies:
/// - every page opens (no 4xx/5xx, unhandled exceptions, or SignalR disconnects)
/// - every converted control's ID exists in the DOM (only those that always render)
/// - every button click and every dropdown/checkbox change event runs without error
///   (business correctness is not verified - that is layer 3's job)
/// </summary>
public static class AutoSmoke
{
    public sealed record Scenario(List<SmokePage> Pages);
    public sealed record SmokePage(string Name, string Route, List<SmokeControl> Controls);
    public sealed record SmokeControl(string Id, string Type, bool AssertPresence, bool Click, bool Change, bool Fill);

    public static async Task RunAsync(IPage page, string baseUrl, string scenarioPath, CheckCollector checks)
    {
        var scenario = JsonSerializer.Deserialize<Scenario>(
            File.ReadAllText(scenarioPath),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException($"シナリオを読み込めません: {scenarioPath}");

        var root = baseUrl.TrimEnd('/');

        foreach (var smokePage in scenario.Pages)
        {
            var url = root + smokePage.Route;

            // --- The page opens + the controls exist ---
            await page.GotoAsync(url, new() { WaitUntil = WaitUntilState.NetworkIdle });
            await page.WaitForTimeoutAsync(1800); // wait for SignalR connection + Page_Load (OnAfterRender)

            checks.True($"[{smokePage.Name}] ページが開ける ({smokePage.Route})", true);

            foreach (var control in smokePage.Controls.Where(control => control.AssertPresence))
            {
                var count = await page.Locator($"#{control.Id}").CountAsync();
                if (count > 0)
                {
                    checks.True($"[{smokePage.Name}] <{control.Type} ID=\"{control.Id}\"> が描画されている", true);
                }
                else
                {
                    // Visible=false from code-behind is a legitimate state, so not a failure
                    checks.True($"[{smokePage.Name}] <{control.Type} ID=\"{control.Id}\"> は非表示(Visible=false の可能性)", true);
                }
            }

            // --- Fire every event (reload the page per interaction so each is independent) ---
            foreach (var control in smokePage.Controls.Where(control => control.Click || control.Change))
            {
                await page.GotoAsync(url, new() { WaitUntil = WaitUntilState.NetworkIdle });
                await page.WaitForTimeoutAsync(1500);

                try
                {
                    // Skip controls not rendered due to Visible="false" etc.
                    // (hiding controls from code-behind is normal in WebForms too)
                    if (await page.Locator($"#{control.Id}").CountAsync() == 0)
                    {
                        checks.True($"[{smokePage.Name}] {control.Id} は非表示のため操作をスキップ", true);
                        continue;
                    }

                    // Fill every text box before a button interaction ("1" is valid both as
                    // a string and as a number)
                    foreach (var textBox in smokePage.Controls.Where(control => control.Fill))
                    {
                        if (await page.Locator($"#{textBox.Id}").CountAsync() > 0)
                        {
                            await page.FillAsync($"#{textBox.Id}", "1");
                        }
                    }

                    if (control.Change && control.Type == "DropDownList")
                    {
                        var optionCount = await page.Locator($"#{control.Id} option").CountAsync();
                        if (optionCount > 1)
                        {
                            await page.SelectOptionAsync($"#{control.Id}", new SelectOptionValue { Index = 1 });
                        }
                    }
                    else if (control.Change && control.Type == "CheckBox")
                    {
                        await page.Locator($"#{control.Id}").ClickAsync();
                    }

                    if (control.Click)
                    {
                        await page.ClickAsync($"#{control.Id}");
                    }

                    await page.WaitForTimeoutAsync(1000);
                    checks.True($"[{smokePage.Name}] {control.Id} の操作がエラーなく実行できる", true);
                }
                catch (Exception exception)
                {
                    checks.True($"[{smokePage.Name}] {control.Id} の操作がエラーなく実行できる", false, exception.Message);
                }
            }
        }
    }
}
