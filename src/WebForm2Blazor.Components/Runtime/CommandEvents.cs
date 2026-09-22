namespace WebForm2Blazor.Components;

/// <summary>WebForms System.Web.UI.WebControls.CommandEventArgs equivalent.</summary>
public class CommandEventArgs(string commandName, object commandArgument) : EventArgs
{
    public string CommandName { get; } = commandName;

    /// <summary>object-typed as in WebForms (handlers typically read it via .ToString()).</summary>
    public object CommandArgument { get; } = commandArgument;
}

public delegate void CommandEventHandler(object sender, CommandEventArgs e);

/// <summary>WebForms RepeaterCommandEventArgs equivalent.</summary>
public class RepeaterCommandEventArgs(RepeaterItem item, object commandSource, CommandEventArgs originalArgs)
    : CommandEventArgs(originalArgs.CommandName, originalArgs.CommandArgument)
{
    /// <summary>The row that raised the command. Item.DataItem exposes the row data.</summary>
    public RepeaterItem Item { get; } = item;

    /// <summary>The control that raised the command (LinkButton etc.).</summary>
    public object CommandSource { get; } = commandSource;
}

public delegate void RepeaterCommandEventHandler(object source, RepeaterCommandEventArgs e);

/// <summary>
/// WebForms event bubbling (OnBubbleEvent) equivalent sink.
/// Repeater / GridView wrap their template content with this interface so commands raised
/// by Button / LinkButton inside a template reach the parent control.
/// </summary>
public interface ICommandSink
{
    void RaiseCommand(object commandSource, CommandEventArgs args);
}

/// <summary>Bubbles commands into the Repeater's ItemCommand.</summary>
public sealed class RepeaterCommandContext(Repeater owner, RepeaterItem item) : ICommandSink
{
    public RepeaterItem Item => item;

    public void RaiseCommand(object commandSource, CommandEventArgs args)
        => owner.RaiseItemCommand(commandSource, item, args);
}

/// <summary>WebForms System.Web.UI.ImageClickEventArgs equivalent.</summary>
public class ImageClickEventArgs(int x, int y) : EventArgs
{
    public int X { get; } = x;
    public int Y { get; } = y;
}

public delegate void ImageClickEventHandler(object sender, ImageClickEventArgs e);

/// <summary>WebForms DataListCommandEventArgs equivalent.</summary>
public class DataListCommandEventArgs(RepeaterItem item, object commandSource, CommandEventArgs originalArgs)
    : CommandEventArgs(originalArgs.CommandName, originalArgs.CommandArgument)
{
    /// <summary>The item that raised the command. Item.DataItem exposes the row data.</summary>
    public RepeaterItem Item { get; } = item;

    public object CommandSource { get; } = commandSource;
}

public delegate void DataListCommandEventHandler(object source, DataListCommandEventArgs e);

/// <summary>Bubbles commands into the DataList's ItemCommand.</summary>
public sealed class DataListCommandContext(DataList owner, RepeaterItem item) : ICommandSink
{
    public RepeaterItem Item => item;

    public void RaiseCommand(object commandSource, CommandEventArgs args)
        => owner.RaiseItemCommand(commandSource, item, args);
}

/// <summary>WebForms GridViewCommandEventArgs equivalent (RowCommand).</summary>
public class GridViewCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
    : CommandEventArgs(originalArgs.CommandName, originalArgs.CommandArgument)
{
    public object CommandSource { get; } = commandSource;
}

public delegate void GridViewCommandEventHandler(object sender, GridViewCommandEventArgs e);

/// <summary>WebForms ListViewCommandEventArgs equivalent (ItemCommand).</summary>
public sealed class ListViewCommandEventArgs(RepeaterItem item, object commandSource, CommandEventArgs originalArgs)
    : CommandEventArgs(originalArgs.CommandName, originalArgs.CommandArgument)
{
    /// <summary>The item that raised the command. Item.DataItem exposes the row data.</summary>
    public RepeaterItem Item { get; } = item;

    public object CommandSource { get; } = commandSource;
}

public delegate void ListViewCommandEventHandler(object source, ListViewCommandEventArgs e);

/// <summary>Bubbles commands into the ListView's ItemCommand (and its Delete handling).</summary>
public sealed class ListViewCommandContext(ListView owner, RepeaterItem item) : ICommandSink
{
    public RepeaterItem Item => item;

    public void RaiseCommand(object commandSource, CommandEventArgs args)
        => owner.RaiseItemCommand(commandSource, item, args);
}
