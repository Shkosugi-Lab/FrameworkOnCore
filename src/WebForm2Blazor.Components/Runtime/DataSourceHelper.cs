namespace WebForm2Blazor.Components;

/// <summary>A control that can hold ListItems (DropDownList etc.).</summary>
public interface IListItemContainer
{
    void AddItem(ListItem item);
}

/// <summary>A control that can hold column definitions (GridView etc.).</summary>
public interface IColumnContainer
{
    void AddColumn(DataControlField column);
}

internal static class DataSourceHelper
{
    /// <summary>Normalizes a WebForms DataSource (an anything-goes object) into a list of rows.</summary>
    public static List<object> Materialize(object dataSource)
    {
        if (dataSource is null)
        {
            return [];
        }

        if (dataSource is string singleString)
        {
            return [singleString];
        }

        // IEnumerable FIRST, IListSource second. Both branches exist for the same reason
        // (DataTable / DataSet bind through IListSource because a DataTable is NOT
        // IEnumerable itself), but the order matters for a type implementing both:
        // EF6's DbSet/DbQuery is IEnumerable (enumerating EXECUTES the query - what
        // WebForms model binding does with an IQueryable select method) and IListSource
        // whose GetList() THROWS by design ("Data binding directly to a store query is
        // not supported"). With IListSource first, WingtipToys' GetProducts crashed the
        // whole circuit on every page. DataTable / DataSet are unaffected either way -
        // they are not IEnumerable, so they still fall through to IListSource.
        if (dataSource is System.Collections.IEnumerable enumerable)
        {
            return enumerable.Cast<object>().ToList();
        }

        if (dataSource is System.ComponentModel.IListSource listSource)
        {
            return listSource.GetList().Cast<object>().ToList();
        }

        return [dataSource];
    }
}
