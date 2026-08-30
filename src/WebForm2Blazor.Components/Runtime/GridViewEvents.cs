namespace WebForm2Blazor.Components;

/// <summary>WebForms SortDirection equivalent.</summary>
public enum SortDirection
{
    Ascending,
    Descending,
}

/// <summary>WebForms GridViewPageEventArgs equivalent.</summary>
public sealed class GridViewPageEventArgs(int newPageIndex) : EventArgs
{
    public int NewPageIndex { get; set; } = newPageIndex;
    public bool Cancel { get; set; }
}

public delegate void GridViewPageEventHandler(object sender, GridViewPageEventArgs e);

/// <summary>WebForms GridViewSortEventArgs equivalent.</summary>
public sealed class GridViewSortEventArgs(string sortExpression, SortDirection sortDirection) : EventArgs
{
    public string SortExpression { get; set; } = sortExpression;
    public SortDirection SortDirection { get; set; } = sortDirection;
    public bool Cancel { get; set; }
}

public delegate void GridViewSortEventHandler(object sender, GridViewSortEventArgs e);
