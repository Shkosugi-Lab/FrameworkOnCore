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

        // DataTable / DataSet bind through IListSource (rows become DataRowView),
        // exactly as WebForms resolves them - a DataTable is NOT IEnumerable itself
        if (dataSource is System.ComponentModel.IListSource listSource)
        {
            return listSource.GetList().Cast<object>().ToList();
        }

        if (dataSource is System.Collections.IEnumerable enumerable)
        {
            return enumerable.Cast<object>().ToList();
        }

        return [dataSource];
    }
}
