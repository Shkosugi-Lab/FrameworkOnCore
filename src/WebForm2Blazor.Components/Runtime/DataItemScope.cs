using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>
/// The data item being bound, for code that asks for it without being handed it -
/// WebForms' Page.GetDataItem(), which TemplateControl.Eval(expression) reads.
///
/// The idiom is a code-behind helper called from a template:
///
///     &lt;%# GetClass() %&gt;
///     protected string GetClass() => (bool)Eval("IsNew") ? "new" : "existing";
///
/// WebForms kept a stack of the containers being data-bound, and Eval with no container
/// read the top of it. Here the equivalent moment is a data-bound control rendering a row's
/// template: RenderTreeBuilder runs a RenderFragment inline, so every expression in the
/// template - including the call into the helper - is evaluated between the push and the
/// pop below. Nested templates stack, as they did in WebForms.
///
/// Per thread, because a render is synchronous on one thread and circuits render on
/// different ones.
/// </summary>
public static class DataItemScope
{
    [ThreadStatic]
    private static Stack<object> _items;

    /// <summary>The innermost data item being rendered, or null outside any template.</summary>
    public static object Current => _items is { Count: > 0 } items ? items.Peek() : null;

    /// <summary>
    /// A fragment that renders <paramref name="template"/> with a data item current: the
    /// row's, when given a RepeaterItem (the data-bound lists), or the item itself (FormView,
    /// DetailsView, which hold theirs directly).
    /// </summary>
    public static RenderFragment Render(object containerOrItem, RenderFragment template)
    {
        if (template is null)
        {
            return null;
        }

        return builder =>
        {
            (_items ??= new Stack<object>()).Push(containerOrItem is RepeaterItem row ? row.DataItem : containerOrItem);
            try
            {
                template(builder);
            }
            finally
            {
                _items.Pop();
            }
        };
    }

    /// <summary>
    /// WebForms TemplateControl.Eval(expression): DataBinder.Eval against the data item
    /// being bound. Outside a template there is none, and the original threw there too.
    /// </summary>
    public static object Eval(string expression)
        => DataBinder.Eval(
            Current ?? throw new InvalidOperationException(
                "Eval() を使えるのは、データバインドされたコントロールのテンプレートの中だけです。"),
            expression);

    public static string Eval(string expression, string format)
        => DataBinder.Eval(
            Current ?? throw new InvalidOperationException(
                "Eval() を使えるのは、データバインドされたコントロールのテンプレートの中だけです。"),
            expression,
            format);
}
