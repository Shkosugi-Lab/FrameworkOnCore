using Bunit;
using Microsoft.AspNetCore.Components;
using WebForm2Blazor.Components;
using Xunit;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// Shapes n2cms's management project is written in, each of which the conversion used to
/// break. The Probe* types are CONVERTED DefaultsProbe sources, so these run what the
/// converter produced, not a hand-written stand-in.
/// </summary>
public class N2ManagementShapeTests : WebFormsTestContext
{
    [Fact]
    public void 名前空間と同名のフィールドは互換層の型に書き換えられない()
    {
        // namespace DefaultsProbe.Wizard { ... Wizard.GetLocations() } where Wizard is a
        // field. It used to become WebForm2Blazor.Components.Wizard.GetLocations().
        Assert.Equal(3, new DefaultsProbe.Wizard.ProbeWizardPage().CountLocations());
    }

    [Fact]
    public void コードで代入したColorは色の文字列になる()
    {
        var first = new Label();
        var second = new Label();

        DefaultsProbe.ProbeColors.MarkError(first, second);

        // ColorTranslator.ToHtml, as WebForms rendered them: a named colour by name.
        Assert.Equal("Red", first.ForeColor);
        Assert.Equal("Red", second.ForeColor);
        Assert.Equal("#123456", first.BackColor);
    }

    [Fact]
    public void Image継承のクラスはImageUrlを持ち自己終了のimgを描く()
    {
        var image = new DefaultsProbe.ProbeImage { ID = "imgLogo", ImageUrl = "~/logo.png", AlternateText = "ロゴ" };

        var text = new System.IO.StringWriter();
        image.RenderControl(new HtmlTextWriter(text));

        Assert.Equal("~/logo.png", image.CurrentUrl);
        Assert.Equal("<img id=\"imgLogo\" src=\"/logo.png\" alt=\"ロゴ\" />", text.ToString());
    }

    [Fact]
    public void テンプレートから呼んだコードのEvalは描画中の行のデータを読む()
    {
        // n2's Languages.ascx: <%# GetClass() %>, and GetClass() calls Eval("IsNew") with no
        // container - WebForms' Page.GetDataItem().
        RenderFragment<RepeaterItem> template = item => builder =>
            builder.AddMarkupContent(0, $"<li>{DataItemScope.Eval("Name")}</li>");

        var cut = RenderComponent<Repeater>(parameters => parameters.Add(r => r.ItemTemplate, template));
        cut.InvokeAsync(() =>
        {
            cut.Instance.DataSource = new[] { new { Name = "甲" }, new { Name = "乙" } };
            cut.Instance.DataBind();
        });

        Assert.Equal(["甲", "乙"], cut.FindAll("li").Select(li => li.TextContent));
        Assert.Null(DataItemScope.Current);
    }

    [Fact]
    public void 行のParentは持ち主のコントロールを指す()
    {
        // n2's security page: (RepeaterItem)item.Parent.Parent climbs to the outer row.
        RepeaterItem captured = null;
        RenderFragment<RepeaterItem> template = item => builder =>
        {
            captured = item;
            builder.AddContent(0, "x");
        };

        var cut = RenderComponent<Repeater>(parameters => parameters.Add(r => r.ItemTemplate, template));
        cut.InvokeAsync(() =>
        {
            cut.Instance.DataSource = new[] { 1 };
            cut.Instance.DataBind();
        });

        Assert.Same(cut.Instance, captured.Parent);
    }
}
