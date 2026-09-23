using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using WebForm2Blazor.Components;
using Xunit;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// The compatibility layer used to have two control hierarchies - Razor components
/// (WebFormsControlBase) and render-based controls (LegacyWebControl) - and a control of
/// one could not stand where the other was expected. WebForms has one.
///
/// These pin the shape that broke n2cms: an editor built IN CODE, returned as a
/// ListControl, and added to a render-based container. It has to compile, render inside
/// the container's element, and post its value back - the last of which a render-based
/// stand-in can never do.
/// </summary>
public class UnifiedControlHierarchyTests : WebFormsTestContext
{
    /// <summary>A ported render-based container: a fieldset around what its code adds.</summary>
    private sealed class EditorPanel : LegacyWebControl
    {
        protected override string TagName => "fieldset";

        public void BuildInto(RenderTreeBuilder builder) => BuildRenderTree(builder);
    }

    /// <summary>n2cms's EditableDropDownAttribute.CreateEditor shape.</summary>
    private static ListControl CreateEditor()
    {
        var list = new DropDownList { ID = "ddlTheme" };
        list.Items.Add(new ListItem("Blue", "blue"));
        list.Items.Add(new ListItem("Red", "red"));
        return list;
    }

    /// <summary>A page-side host that builds the tree in code, as the editing UI does.</summary>
    private sealed class CodeBuiltEditorHost : WebFormsControlBase
    {
        public ListControl Editor { get; private set; }

        public int SelectionChanges { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var panel = new EditorPanel { ID = "pnlEditor" };
            Editor = CreateEditor();
            ((DropDownList)Editor).SelectedIndexChanged += (_, _) => SelectionChanges++;
            panel.Controls.Add(Editor);
            Controls.Add(panel);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
            => builder.AddContent(0, DynamicChildren);
    }

    [Fact]
    public void コードで作ったDropDownListが描画型の親の中に描画される()
    {
        var cut = RenderComponent<CodeBuiltEditorHost>();

        // Inside the render-based container's element, not beside it. Markup cut around a
        // component would put the select AFTER an empty, auto-closed fieldset.
        var select = cut.Find("fieldset#pnlEditor > select#ddlTheme");
        Assert.Equal(2, select.QuerySelectorAll("option").Length);
    }

    [Fact]
    public void コードで作ったDropDownListの選択がサーバーに戻る()
    {
        var cut = RenderComponent<CodeBuiltEditorHost>();

        cut.Find("#ddlTheme").Change("red");

        // The value comes back to the object the code built and still holds, and the event
        // it subscribed to fires - which a render-based stand-in could never do.
        Assert.Equal("red", cut.Instance.Editor.SelectedValue);
        Assert.Equal(1, cut.Instance.Editor.SelectedIndex);
        Assert.Equal(1, cut.Instance.SelectionChanges);
    }

    [Fact]
    public void 文字列への描画にはコンポーネントの印が漏れない()
    {
        // Ported code that renders a control into a string for its own use (n2cms's
        // ControlPanel does) must get plain text: the component marker exists only for a
        // writer Blazor is filling.
        var panel = new EditorPanel { ID = "pnl" };
        panel.Controls.Add(CreateEditor());

        var text = new System.IO.StringWriter();
        panel.RenderControl(new HtmlTextWriter(text));

        Assert.DoesNotContain('\u0001', text.ToString());
        Assert.StartsWith("<fieldset", text.ToString());
    }

    [Fact]
    public void 描画型の親は子のコンポーネントを要素フレームの内側に置く()
    {
        // bUnit joins every frame's HTML into one string before parsing it, so it cannot
        // tell "<fieldset>" + component + "</fieldset>" as three markup frames (which a
        // browser renders as an EMPTY fieldset followed by the select) from real nesting.
        // The frames can: the fieldset has to be an element frame, and the component frame
        // has to fall inside its subtree.
        var panel = new EditorPanel { ID = "pnl" };
        panel.Controls.Add(CreateEditor());

        var builder = new RenderTreeBuilder();
        panel.BuildInto(builder);
        var frames = builder.GetFrames();

        var fieldsetIndex = -1;
        var componentIndex = -1;
        for (var i = 0; i < frames.Count; i++)
        {
            var frame = frames.Array[i];
            if (frame.FrameType == Microsoft.AspNetCore.Components.RenderTree.RenderTreeFrameType.Element
                && frame.ElementName == "fieldset")
            {
                fieldsetIndex = i;
            }
            if (frame.FrameType == Microsoft.AspNetCore.Components.RenderTree.RenderTreeFrameType.Component
                && frame.ComponentType == typeof(DropDownList))
            {
                componentIndex = i;
            }
        }

        Assert.True(fieldsetIndex >= 0, "fieldset はマークアップ文字列ではなく要素フレームであること");
        Assert.True(componentIndex > fieldsetIndex, "DropDownList はコンポーネントフレームであること");
        Assert.True(
            componentIndex < fieldsetIndex + frames.Array[fieldsetIndex].ElementSubtreeLength,
            "DropDownList は fieldset の部分木の内側にあること");
    }
}