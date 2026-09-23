using Bunit;
using WebForm2Blazor.Components;
using Xunit;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// Shapes n2cms's template project (the application itself) is written in, which the
/// compatibility layer used to reject at compile time.
/// </summary>
public class N2TemplatesShapeTests : WebFormsTestContext
{
    private sealed record Option(string Title, int ID);

    [Fact]
    public void ListControl型の変数でデータバインドと選択ができる()
    {
        // n2's SingleSelectControl: a DropDownList, ListBox or RadioButtonList held as one
        // ListControl and bound through it.
        ListControl list = new RadioButtonList();
        list.DataTextField = "Title";
        list.DataValueField = "ID";
        list.DataSource = new[] { new Option("甲", 1), new Option("乙", 2) };
        list.DataBind();

        list.SelectedValue = "2";

        Assert.Equal(2, list.Items.Count);
        Assert.Equal("乙", list.SelectedItem?.Text);
    }

    [Fact]
    public void RepeatLayoutは列挙型で代入できる()
    {
        var list = new RadioButtonList { RepeatLayout = RepeatLayout.Flow };

        Assert.Equal(RepeatLayout.Flow, list.RepeatLayout);
    }

    [Fact]
    public void CalendarはSelectedDatesの日をすべて選択状態で描く()
    {
        var cut = RenderComponent<Calendar>(parameters => parameters
            .Add(c => c.VisibleDate, new DateTime(2024, 5, 1))
            .Add(c => c.SelectedDayStyleCssClass, "sel"));

        // n2's calendar teaser marks each day that has an event.
        cut.InvokeAsync(() =>
        {
            cut.Instance.SelectedDates.Add(new DateTime(2024, 5, 3, 13, 0, 0));
            cut.Instance.SelectedDates.Add(new DateTime(2024, 5, 20));
        });

        Assert.Equal(["3", "20"], cut.FindAll("td.sel").Select(td => td.TextContent.Trim()));
        Assert.Equal(new DateTime(2024, 5, 3), cut.Instance.SelectedDate);

        // Assigning SelectedDate replaces the selection, as in WebForms.
        cut.InvokeAsync(() => cut.Instance.SelectedDate = new DateTime(2024, 5, 7));
        Assert.Equal(["7"], cut.FindAll("td.sel").Select(td => td.TextContent.Trim()));
    }

    [Fact]
    public void ログイン系のイベントはWebFormsのデリゲート型で購読できる()
    {
        // n2's Login part: LoginBox.Authenticate += new AuthenticateEventHandler(...), and
        // its Register part the same with LoginCancelEventHandler.
        var login = new Login();
        var wizard = new CreateUserWizard();
        var authenticated = false;

        login.Authenticate += new AuthenticateEventHandler((_, e) => authenticated = true);
        wizard.CreatingUser += new LoginCancelEventHandler((_, e) => e.Cancel = true);

        Assert.False(authenticated);
    }

    [Fact]
    public void Page_Cultureの代入で現在のカルチャーが変わり表示名が返る()
    {
        var original = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            var page = new DetachedPage();

            page.Culture = "ja-JP";

            Assert.Equal("ja-JP", System.Globalization.CultureInfo.CurrentCulture.Name);
            Assert.Equal(System.Globalization.CultureInfo.GetCultureInfo("ja-JP").DisplayName, page.Culture);
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = original;
        }
    }
}
