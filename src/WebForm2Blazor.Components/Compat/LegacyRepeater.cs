namespace WebForm2Blazor.Components;

/// <summary>
/// The render-based base for a ported control that derived from System.Web.UI.WebControls
/// .Repeater.
///
/// Such a control used to map onto LegacyWebControl, which has the lifecycle and the render
/// virtuals and nothing that makes a repeater a repeater. The subclass lost the members it
/// is written against: n2cms's Repeater overrides CreateControlHierarchy and OnItemCreated,
/// then builds an EmptyTemplate row by hand with new RepeaterItem(0, ListItemType.Header)
/// and raises OnItemDataBound for it - five errors, every one a member Repeater has.
/// Same arrangement as LegacyListControl and LegacyCalendar: the members live on a base a
/// ported class can derive from, not on the Blazor component of the same name.
///
/// The repeater works as the original does for a control driven from CODE: DataBind
/// instantiates the templates into one row per data item (with header, footer and
/// separators), raises ItemCreated / ItemDataBound, and Render writes the rows and nothing
/// around them. What it is not is the markup-declared Repeater, which is the Blazor
/// component.
/// </summary>
public class LegacyRepeater : LegacyWebControl, INamingContainer
{
    public virtual object DataSource { get; set; }

    public virtual string DataSourceID { get; set; }

    public virtual string DataMember { get; set; }

    public virtual ITemplate HeaderTemplate { get; set; }

    public virtual ITemplate ItemTemplate { get; set; }

    public virtual ITemplate AlternatingItemTemplate { get; set; }

    public virtual ITemplate SeparatorTemplate { get; set; }

    public virtual ITemplate FooterTemplate { get; set; }

    /// <summary>The data rows (Item / AlternatingItem only, as in WebForms).</summary>
    public virtual List<RepeaterItem> Items { get; } = [];

    public event RepeaterItemEventHandler ItemCreated;

    public event RepeaterItemEventHandler ItemDataBound;

    /// <summary>WebForms Repeater.DataBind: rebuilds the rows from DataSource.</summary>
    public override void DataBind()
    {
        OnDataBinding(EventArgs.Empty);
        CreateControlHierarchy(useDataSource: true);
    }

    /// <summary>
    /// WebForms Repeater.CreateControlHierarchy - builds the rows. With useDataSource false
    /// the original rebuilt from ViewState; there is no row state to rebuild from here, so
    /// only the header and footer are produced in that case.
    /// </summary>
    protected virtual void CreateControlHierarchy(bool useDataSource)
    {
        Controls.Clear();
        Items.Clear();

        if (HeaderTemplate is not null)
        {
            CreateItem(-1, ListItemType.Header, useDataSource, null);
        }

        var index = 0;
        if (useDataSource && DataSource is System.Collections.IEnumerable rows and not string)
        {
            foreach (var row in rows)
            {
                if (index > 0 && SeparatorTemplate is not null)
                {
                    CreateItem(index - 1, ListItemType.Separator, useDataSource, null);
                }
                Items.Add(CreateItem(
                    index,
                    index % 2 == 0 ? ListItemType.Item : ListItemType.AlternatingItem,
                    useDataSource,
                    row));
                index++;
            }
        }

        if (FooterTemplate is not null)
        {
            CreateItem(-1, ListItemType.Footer, useDataSource, null);
        }
    }

    private RepeaterItem CreateItem(int itemIndex, ListItemType itemType, bool dataBind, object dataItem)
    {
        var item = new RepeaterItem(itemIndex, itemType) { DataItem = dataItem };
        var template = itemType switch
        {
            ListItemType.Header => HeaderTemplate,
            ListItemType.Footer => FooterTemplate,
            ListItemType.Separator => SeparatorTemplate,
            ListItemType.AlternatingItem => AlternatingItemTemplate ?? ItemTemplate,
            _ => ItemTemplate,
        };
        template?.InstantiateIn(item);
        Controls.Add(item);

        OnItemCreated(new RepeaterItemEventArgs(item));
        if (dataBind)
        {
            item.DataBind();
            OnItemDataBound(new RepeaterItemEventArgs(item));
        }
        return item;
    }

    protected virtual void OnItemCreated(RepeaterItemEventArgs e) => ItemCreated?.Invoke(this, e);

    protected virtual void OnItemDataBound(RepeaterItemEventArgs e) => ItemDataBound?.Invoke(this, e);

    /// <summary>A Repeater renders its rows and no element of its own.</summary>
    protected override void Render(HtmlTextWriter writer) => RenderChildren(writer);
}
