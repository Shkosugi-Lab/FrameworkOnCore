namespace WebForm2Blazor.Components;

/// <summary>WebForms DataKey equivalent (the key values of a single row).</summary>
public sealed class DataKey(Dictionary<string, object> values)
{
    /// <summary>The first key's value (the canonical access when DataKeyNames has one entry).</summary>
    public object Value => values.Count > 0 ? values.Values.First() : null;

    public object this[string name] => values.TryGetValue(name, out var value) ? value : null;

    public object this[int index] => index >= 0 && index < values.Count ? values.Values.ElementAt(index) : null;

    /// <summary>All key name/value pairs (used to build GridViewDeleteEventArgs.Keys).</summary>
    internal IReadOnlyDictionary<string, object> All => values;
}

/// <summary>WebForms DataKeyCollection equivalent (aligned with the rows of the current page).</summary>
public sealed class DataKeyCollection(List<DataKey> keys)
{
    public int Count => keys.Count;

    public DataKey this[int index] => index >= 0 && index < keys.Count ? keys[index] : null;
}

/// <summary>
/// The DataList / DataGrid form of DataKeys. Those controls name a single key column
/// (DataKeyField), so their indexer yields the raw value rather than a DataKey -
/// dl.DataKeys[e.Item.ItemIndex] is used directly as an id. Kept separate from
/// <see cref="DataKeyCollection"/> so each control keeps its own WebForms shape.
/// </summary>
public sealed class DataKeyValueCollection(List<object> values)
{
    public int Count => values.Count;

    public object this[int index] => index >= 0 && index < values.Count ? values[index] : null;
}
