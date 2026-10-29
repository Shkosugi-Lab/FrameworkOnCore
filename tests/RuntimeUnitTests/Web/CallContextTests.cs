using System.Runtime.Remoting.Messaging;
using System.Security.Principal;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// CallContext as .NET Framework's (its documentation): the logical call context flows with the execution (a task, an
/// await), the illogical one (SetData of data that is not ILogicalThreadAffinative, HostContext) stays with the thread;
/// GetData reads either; FreeNamedDataSlot empties both.
/// </summary>
public class CallContextTests
{
    static string Name() => "test." + Guid.NewGuid().ToString("N");

    // What another thread (its execution context flowed from this one's, as a task's is) sees.
    static T OnAnotherThread<T>(Func<T> read)
    {
        T value = default;
        var thread = new Thread(() => value = read());
        thread.Start();
        thread.Join();
        return value;
    }

    [Fact] // CallContext's documentation: LogicalSetData's data flows with the execution context; SetData's (not ILogicalThreadAffinative) stays with the thread
    public void Logical_data_flows_illogical_data_stays()
    {
        string logical = Name(), illogical = Name();
        CallContext.LogicalSetData(logical, "l");
        CallContext.SetData(illogical, "i");
        Assert.Equal("l", CallContext.LogicalGetData(logical));
        Assert.Equal("l", CallContext.GetData(logical));
        Assert.Equal("i", CallContext.GetData(illogical));
        Assert.Null(CallContext.LogicalGetData(illogical));
        var seen = OnAnotherThread(() => (CallContext.GetData(logical), CallContext.GetData(illogical)));
        Assert.Equal(("l", (object)null), seen);
        CallContext.FreeNamedDataSlot(logical);
        CallContext.FreeNamedDataSlot(illogical);
    }

    [Fact] // .NET Framework's SetData/LogicalSetData (reference source): each frees the name in the other context
    public void Setting_a_name_in_one_context_frees_it_in_the_other()
    {
        var name = Name();
        CallContext.LogicalSetData(name, "l");
        CallContext.SetData(name, "i");
        Assert.Null(CallContext.LogicalGetData(name));
        Assert.Equal("i", CallContext.GetData(name));
        CallContext.LogicalSetData(name, "l2");
        CallContext.LogicalSetData(name, "l3");
        Assert.Equal("l3", CallContext.GetData(name));
        CallContext.SetData(name, "i2");
        CallContext.FreeNamedDataSlot(name);
        Assert.Null(CallContext.GetData(name));
        Assert.Null(CallContext.LogicalGetData(Name()));
    }

    [Fact] // the port's choice (the field's comment): HostContext is the thread's, not flowed; ClearIllogical (a worker thread used again) empties the thread's
    public void The_host_context_is_the_thread_s()
    {
        var name = Name();
        CallContext.HostContext = "host";
        CallContext.SetData(name, "i");
        Assert.Equal("host", CallContext.HostContext);
        Assert.Null(OnAnotherThread(() => CallContext.HostContext));
        CallContext.ClearIllogical();
        Assert.Null(CallContext.HostContext);
        Assert.Null(CallContext.GetData(name));
    }

    [Fact] // the port's choice (no LogicalCallContext.Principal on .NET): the principal set in the logical call context; none: the thread's
    public void The_principal_is_the_logical_call_context_s()
    {
        var principal = new GenericPrincipal(new GenericIdentity("u"), null);
        Assert.Same(Thread.CurrentPrincipal, CallContext.Principal);
        CallContext.Principal = principal;
        Assert.Same(principal, CallContext.Principal);
        CallContext.Principal = principal;
        CallContext.FreeNamedDataSlot("CallContext.Principal");
    }
}
