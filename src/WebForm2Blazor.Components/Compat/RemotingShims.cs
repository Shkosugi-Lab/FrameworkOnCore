namespace System.Runtime.Remoting.Messaging;

/// <summary>
/// System.Runtime.Remoting.Messaging.CallContext equivalent.
///
/// Remoting is gone from .NET, but CallContext outlived what it was built for: it is how
/// .NET Framework libraries carry ambient per-request state. ServiceStack.OrmLite - which
/// YAF.NET vendors - keeps its open connection and transaction in one, so without this the
/// port has no request context at all.
///
/// Declared in the ORIGINAL namespace on purpose. Ported code says
/// "using System.Runtime.Remoting.Messaging;" and the point is that the line keeps
/// meaning what it meant.
///
/// The two halves of the original API differ in how far the value travels:
///
///   - LogicalGetData / LogicalSetData flowed with the logical call context, across await
///     and into child tasks. AsyncLocal is that, and is what the .NET Framework
///     implementation itself came to be built on.
///   - GetData / SetData were thread-local and did NOT flow. ThreadLocal keeps that
///     difference rather than quietly widening it - code that relies on a value NOT
///     escaping to a continuation would otherwise start seeing it.
/// </summary>
public static class CallContext
{
    private static readonly AsyncLocal<Dictionary<string, object>> LogicalSlots = new();

    private static readonly ThreadLocal<Dictionary<string, object>> ThreadSlots =
        new(() => new Dictionary<string, object>(StringComparer.Ordinal));

    /// <summary>Value stored for this name in the logical call context, or null.</summary>
    public static object LogicalGetData(string name)
        => LogicalSlots.Value is { } slots && slots.TryGetValue(name, out var value) ? value : null;

    /// <summary>
    /// Stores a value that flows with the execution context.
    ///
    /// The dictionary is COPIED rather than mutated: AsyncLocal shares one object with
    /// everything downstream, so writing into it in a child task would be seen by the
    /// parent, which the original never did.
    /// </summary>
    public static void LogicalSetData(string name, object data)
    {
        var slots = LogicalSlots.Value is { } existing
            ? new Dictionary<string, object>(existing, StringComparer.Ordinal)
            : new Dictionary<string, object>(StringComparer.Ordinal);
        slots[name] = data;
        LogicalSlots.Value = slots;
    }

    /// <summary>Value stored for this name on this thread, or null.</summary>
    public static object GetData(string name)
        => ThreadSlots.Value.TryGetValue(name, out var value) ? value : null;

    /// <summary>Stores a value on this thread only.</summary>
    public static void SetData(string name, object data) => ThreadSlots.Value[name] = data;

    /// <summary>Clears the name from both contexts, as the original did.</summary>
    public static void FreeNamedDataSlot(string name)
    {
        if (LogicalSlots.Value is { } logical && logical.ContainsKey(name))
        {
            var slots = new Dictionary<string, object>(logical, StringComparer.Ordinal);
            slots.Remove(name);
            LogicalSlots.Value = slots;
        }

        ThreadSlots.Value.Remove(name);
    }
}
