using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using WebForm2Blazor.Components;
using Xunit;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// A WebForms event is reached three ways - a handler the markup names, one code attaches,
/// and a subclass overriding the protected raise method - and all three run together.
/// The compat components keep them apart: the [Parameter] OnX, the CLR event X and the
/// virtual XHandler (see WebFormsControlBase, "Event naming").
/// </summary>
public class EventNamingTests : WebFormsTestContext
{
    /// <summary>A page whose button is wired from markup AND from code, as WebForms allows.</summary>
    private sealed class BothWaysPage : ComponentBase
    {
        public Button SaveButton { get; private set; }

        public List<string> Log { get; } = [];

        public void Refresh() => InvokeAsync(StateHasChanged);

        private void Save_Click(object sender, EventArgs e) => Log.Add("markup");

        private void WriteAuditLog(object sender, EventArgs e) => Log.Add("code");

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                SaveButton.Click += WriteAuditLog;
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<Button>(0);
            builder.AddAttribute(1, nameof(Button.ID), "btnSave");
            builder.AddAttribute(2, nameof(Button.OnClick), (EventHandler)Save_Click);
            builder.AddComponentReferenceCapture(3, component => SaveButton = (Button)component);
            builder.CloseComponent();
        }
    }

    [Fact]
    public void マークアップとコードの両方でつないだハンドラが再描画の後も両方動く()
    {
        var cut = RenderComponent<BothWaysPage>();

        // A parent render re-assigns the markup's parameter. When code's handler was added
        // onto that same delegate, this is where it disappeared.
        cut.Instance.Refresh();
        cut.Find("#btnSave").Click();

        Assert.Equal(["markup", "code"], cut.Instance.Log);
    }

    /// <summary>A ported button that logs before the click - WebForms' OnClick override, renamed.</summary>
    private sealed class ConfirmButton : Button
    {
        public List<string> Log { get; } = [];

        protected override void ClickHandler(EventArgs e)
        {
            Log.Add("override");
            base.ClickHandler(e);
        }
    }

    [Fact]
    public void ClickHandlerを上書きしたサブクラスで上書きと両方のハンドラが動く()
    {
        ConfirmButton button = null;
        var cut = RenderComponent<ConfirmButton>(parameters => parameters
            .Add(b => b.ID, "btnConfirm")
            .Add(b => b.OnClick, (EventHandler)((_, _) => button.Log.Add("markup"))));
        button = cut.Instance;
        button.Click += (_, _) => button.Log.Add("code");

        cut.Find("#btnConfirm").Click();

        Assert.Equal(["override", "markup", "code"], button.Log);
    }

    [Fact]
    public void TextBoxのマークアップのOnTextChangedが確定時に呼ばれる()
    {
        var calls = 0;
        var cut = RenderComponent<TextBox>(parameters => parameters
            .Add(t => t.ID, "txtName")
            .Add(t => t.OnTextChanged, (EventHandler)((_, _) => calls++)));

        cut.Find("#txtName").Change("山田");

        Assert.Equal(1, calls);
        Assert.Equal("山田", cut.Instance.Text);
    }

    [Fact]
    public async Task GridViewはコードでつないだPageIndexChangingをハンドラとして扱う()
    {
        var cut = RenderComponent<GridView>(parameters => parameters
            .Add(g => g.ID, "gvPaged")
            .Add(g => g.AllowPaging, true)
            .Add(g => g.PageSize, 1));

        int? requested = null;
        await cut.InvokeAsync(() =>
        {
            cut.Instance.PageIndexChanging += (_, e) => requested = e.NewPageIndex;
            cut.Instance.DataSource = new[] { new { Name = "甲" }, new { Name = "乙" } };
            cut.Instance.DataBind();
        });

        cut.FindAll("a").First(link => link.TextContent == "2").Click();

        // The handler got the request. Paging itself is the handler's job (it sets
        // PageIndex and re-binds), so the grid must not have moved on its own.
        Assert.Equal(1, requested);
        Assert.Equal(0, cut.Instance.PageIndex);
    }
}
