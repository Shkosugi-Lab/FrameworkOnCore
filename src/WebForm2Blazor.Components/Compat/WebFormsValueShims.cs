using System.Collections;

namespace WebForm2Blazor.Components;

// WebForms types that ported LIBRARY code names and that the compatibility layer did not
// have. Found by building n2cms's N2.dll as its own project (--split-projects): merged into
// the application, that compilation always stopped at the declaration pass, so the method
// bodies naming these were never bound and none of them was ever reported.

/// <summary>
/// System.Web.UI.WebControls.ValidatorDisplay equivalent.
///
/// String constants, not an enum, because the compatibility validators take Display as a
/// STRING parameter ("Static" / "Dynamic" / "None") - that is what markup writes, and
/// ValidatorBase compares it as text. "rfv.Display = ValidatorDisplay.Dynamic" in ported
/// code then assigns exactly the value markup would have.
/// </summary>
public static class ValidatorDisplay
{
    public const string None = "None";
    public const string Static = "Static";
    public const string Dynamic = "Dynamic";
}

/// <summary>
/// System.Web.UI.WebControls.ValidationCompareOperator equivalent. String constants for the
/// same reason as <see cref="ValidatorDisplay"/>: CompareValidator.Operator is a string.
/// </summary>
public static class ValidationCompareOperator
{
    public const string Equal = "Equal";
    public const string NotEqual = "NotEqual";
    public const string GreaterThan = "GreaterThan";
    public const string GreaterThanEqual = "GreaterThanEqual";
    public const string LessThan = "LessThan";
    public const string LessThanEqual = "LessThanEqual";
    public const string DataTypeCheck = "DataTypeCheck";
}

/// <summary>
/// System.Web.UI.WebControls.ListSelectionMode equivalent. String constants: ListBox.SelectionMode
/// is a string parameter.
/// </summary>
public static class ListSelectionMode
{
    public const string Single = "Single";
    public const string Multiple = "Multiple";
}

/// <summary>System.Web.UI.OutputCacheLocation equivalent.</summary>
public enum OutputCacheLocation
{
    Any,
    Client,
    Downstream,
    Server,
    None,
    ServerAndClient,
}

/// <summary>
/// System.Web.UI.Triplet equivalent - three objects, the unit WebForms controls pack their
/// SaveViewState results into. A plain holder with no behaviour to lose.
/// </summary>
[Serializable]
public sealed class Triplet
{
    public Triplet()
    {
    }

    public Triplet(object x, object y)
    {
        First = x;
        Second = y;
    }

    public Triplet(object x, object y, object z) : this(x, y) => Third = z;

    public object First;
    public object Second;
    public object Third;
}

/// <summary>
/// System.Web.Caching.SqlCacheDependency equivalent.
///
/// The original invalidates a cache entry when SQL Server reports a change - through the
/// polling tables aspnet_regsql installed, or through query notifications (SqlDependency).
/// Neither mechanism is wired up here, so the dependency never fires: an entry depending on
/// it stays cached until it expires or is removed some other way. That is stated rather
/// than hidden because it is a behavioural difference, not a missing feature - data can be
/// served stale.
/// </summary>
public class SqlCacheDependency : CacheDependency
{
    /// <summary>Table-polling form (database entry name from Web.config, table name).</summary>
    public SqlCacheDependency(string databaseEntryName, string tableName)
    {
        DatabaseEntryName = databaseEntryName;
        TableName = tableName;
    }

    /// <summary>Query-notification form. The command is kept only for inspection.</summary>
    public SqlCacheDependency(System.Data.IDbCommand sqlCmd) => Command = sqlCmd;

    public string DatabaseEntryName { get; }

    public string TableName { get; }

    public System.Data.IDbCommand Command { get; }
}

/// <summary>
/// System.Web.UI.ValidatorCollection equivalent - what Page.Validators returns.
///
/// Built over the validators the page actually registered (see WebFormsHostCore), so a
/// ported "foreach (IValidator v in page.Validators) v.Validate();" runs the real
/// validators rather than an empty list that would report every page as valid.
/// </summary>
public sealed class ValidatorCollection : IEnumerable<IValidator>
{
    private readonly List<IValidator> _validators;

    internal ValidatorCollection(IEnumerable<IValidator> validators) => _validators = [.. validators];

    public int Count => _validators.Count;

    public IValidator this[int index] => _validators[index];

    public void Add(IValidator validator)
    {
        if (validator is not null && !_validators.Contains(validator))
        {
            _validators.Add(validator);
        }
    }

    public bool Contains(IValidator validator) => _validators.Contains(validator);

    public void Remove(IValidator validator) => _validators.Remove(validator);

    public IEnumerator<IValidator> GetEnumerator() => _validators.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>System.Web.HttpCacheValidateHandler equivalent (see HttpCachePolicyShim.AddValidationCallback).</summary>
public delegate void HttpCacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus);