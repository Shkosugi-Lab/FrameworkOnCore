using System.Collections;
using System.Resources;
using System.Security;
using ClientUtils = System.Windows.Forms.ClientUtils;

namespace FrameworkOnCore.Runtime.Tests.Resources;

/// <summary>
/// ClientUtils (System.Windows.Forms' internal helpers, carried with the ResX classes), against the reference source
/// (dotnet/winforms' ClientUtils, the one .NET Framework had): the critical exceptions, the enum checks, WeakRefCollection;
/// and ResXResourceSet (learn.microsoft.com System.Resources.ResXResourceSet).
/// </summary>
public class ClientUtilsTests
{
    [Fact] // reference source (IsCriticalException, IsSecurityOrCriticalException); the port's choice: ResponseEndException is critical (as ThreadAbortException)
    public void Critical_exceptions()
    {
        foreach (var e in new Exception[]
        {
            new NullReferenceException(), new StackOverflowException(), new OutOfMemoryException(), new System.Web.ResponseEndException(),
            new ExecutionEngineException(), new IndexOutOfRangeException(), new AccessViolationException(),
        })
        {
            Assert.True(ClientUtils.IsCriticalException(e));
            Assert.True(ClientUtils.IsSecurityOrCriticalException(e));
        }
        Assert.False(ClientUtils.IsCriticalException(new SecurityException()));
        Assert.True(ClientUtils.IsSecurityOrCriticalException(new SecurityException()));
        Assert.False(ClientUtils.IsCriticalException(new InvalidOperationException()));
        Assert.False(ClientUtils.IsSecurityOrCriticalException(new InvalidOperationException()));
    }

    [Fact] // reference source (GetBitCount, IsEnumValid*): a range, a range with at most n bits on, a mask, a list
    public void Enum_values_are_checked()
    {
        Assert.Equal(0, ClientUtils.GetBitCount(0));
        Assert.Equal(3, ClientUtils.GetBitCount(0b1011 & 0b0111 | 0b1000));
        Assert.Equal(32, ClientUtils.GetBitCount(uint.MaxValue));
        Assert.True(ClientUtils.IsEnumValid(DayOfWeek.Monday, 3, 0, 6));
        Assert.False(ClientUtils.IsEnumValid(DayOfWeek.Monday, -1, 0, 6));
        Assert.False(ClientUtils.IsEnumValid(DayOfWeek.Monday, 7, 0, 6));
        Assert.True(ClientUtils.IsEnumValid(DayOfWeek.Monday, 4, 0, 8, 1));
        Assert.False(ClientUtils.IsEnumValid(DayOfWeek.Monday, 3, 0, 8, 1));
        Assert.False(ClientUtils.IsEnumValid(DayOfWeek.Monday, 16, 0, 8, 1));
        Assert.False(ClientUtils.IsEnumValid(DayOfWeek.Monday, -1, 0, 8, 1));
        Assert.True(ClientUtils.IsEnumValid_Masked(DayOfWeek.Monday, 0x5, 0x7));
        Assert.False(ClientUtils.IsEnumValid_Masked(DayOfWeek.Monday, 0x9, 0x7));
        Assert.True(ClientUtils.IsEnumValid_NotSequential(DayOfWeek.Monday, 8, 1, 4, 8));
        Assert.False(ClientUtils.IsEnumValid_NotSequential(DayOfWeek.Monday, 2, 1, 4, 8));
    }

    // --- WeakRefCollection

    [Fact] // reference source: a list of weak references to its items (null for none); IndexOf, Contains, Remove compare the items
    public void A_weak_collection_lists_its_items()
    {
        var a = new object();
        var b = "b";
        var list = new ClientUtils.WeakRefCollection(2) { a, b };
        Assert.Equal(2, list.Count);
        Assert.Same(a, list[0]);
        Assert.Equal(1, list.IndexOf(b));
        Assert.True(list.Contains(a));
        Assert.False(list.Contains(new object()));
        list[1] = null;
        Assert.Null(list[1]);
        list.Insert(0, "c");
        Assert.Equal("c", list[0]);
        list.Remove("c");
        Assert.Same(a, list[0]);
        list.RemoveAt(1);
        Assert.Equal(1, list.Count);
        Assert.False(list.IsFixedSize);
        Assert.False(list.IsReadOnly);
        Assert.False(((ICollection)list).IsSynchronized);
        Assert.NotNull(((ICollection)list).SyncRoot);
        var array = new object[1];
        list.CopyTo(array, 0);
        Assert.IsType<ClientUtils.WeakRefCollection.WeakRefObject>(array[0]);
        Assert.Single(list.Cast<object>());
        list.Clear();
        Assert.Equal(0, list.Count);
        Assert.Equal(int.MaxValue, new ClientUtils.WeakRefCollection().RefCheckThreshold);
    }

    [Fact] // reference source (ScavengeReferences, Add past RefCheckThreshold): the items collected (and the empty slots) are removed
    public void Collected_items_are_scavenged()
    {
        var kept = new object();
        var list = new ClientUtils.WeakRefCollection { RefCheckThreshold = 1 };
        list.Add(kept);
        list.Add(null);
        list.Add("c"); // more than 1: the null slot goes
        Assert.Equal(2, list.Count);
        Assert.Same(kept, list[0]);
        Assert.Equal("c", list[1]);
        GC.KeepAlive(kept);
    }

    [Fact] // reference source (RemoveByHashCode): the first item of the value's hash code; null: nothing
    public void An_item_is_removed_by_its_hash_code()
    {
        var list = new ClientUtils.WeakRefCollection { null, "a", "b" };
        list.RemoveByHashCode(null);
        Assert.Equal(3, list.Count);
        list.RemoveByHashCode("b");
        Assert.Equal(2, list.Count);
        list.RemoveByHashCode("z");
        Assert.Equal(2, list.Count);
        Assert.Equal("a", list[1]);
    }

    [Fact] // reference source (Equals): the same items in the same order (empty slots too); a list of another type is not equal
    public void Collections_with_the_same_items_are_equal()
    {
        var list = new ClientUtils.WeakRefCollection { "a", null };
        Assert.True(list.Equals(list));
        Assert.True(list.Equals(new ClientUtils.WeakRefCollection { "a", null }));
        Assert.False(list.Equals(new ClientUtils.WeakRefCollection { "a", "b" }));
        Assert.False(new ClientUtils.WeakRefCollection { null, "a" }.Equals(new ClientUtils.WeakRefCollection { "b", "a" }));
        Assert.False(list.Equals(new ClientUtils.WeakRefCollection { "a" }));
        Assert.False(list.Equals(new ArrayList()));
        Assert.Equal(list.GetHashCode(), list.GetHashCode());
    }

    [Fact] // reference source (WeakRefObject.Equals, GetHashCode): the same target, or equal ones; the target's hash code
    public void Weak_references_to_equal_items_are_equal()
    {
        var a = new ClientUtils.WeakRefCollection.WeakRefObject("x");
        Assert.True(a.Equals(a));
        Assert.True(a.Equals(new ClientUtils.WeakRefCollection.WeakRefObject("x")));
        Assert.True(a.Equals(new ClientUtils.WeakRefCollection.WeakRefObject(new string('x', 1))));
        Assert.False(a.Equals(new ClientUtils.WeakRefCollection.WeakRefObject("y")));
        Assert.False(a.Equals("x"));
        Assert.Equal("x".GetHashCode(), a.GetHashCode());
        Assert.True(a.IsAlive);
        var dead = Dead();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        if (!dead.IsAlive)
        {
            Assert.Null(dead.Target);
            Assert.False(dead.Equals(a));
            Assert.True(dead.Equals(Dead2()));
        }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    static ClientUtils.WeakRefCollection.WeakRefObject Dead() => new(new object());

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    static ClientUtils.WeakRefCollection.WeakRefObject Dead2()
    {
        var r = Dead();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        return r;
    }

    // --- ResXResourceSet

    [Fact] // docs (ResXResourceSet): the resources of a .resx file or stream; its reader and writer are ResX's
    public void A_resource_set_reads_a_resx_document()
    {
        var text = Resx.Document("<data name=\"a\"><value>v</value></data>");
        using (var set = new ResXResourceSet(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text))))
        {
            Assert.Equal("v", set.GetString("a"));
            Assert.Equal(typeof(ResXResourceReader), set.GetDefaultReader());
            Assert.Equal(typeof(ResXResourceWriter), set.GetDefaultWriter());
        }
        var path = Resx.TempFile(".resx", System.Text.Encoding.UTF8.GetBytes(text));
        try
        {
            using var set = new ResXResourceSet(path);
            Assert.Equal("v", set.GetObject("a"));
        }
        finally { File.Delete(path); }
    }
}
