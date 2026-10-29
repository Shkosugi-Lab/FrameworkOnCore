using System.Resources;
using System.Security;

namespace FrameworkOnCore.Runtime.Tests.Resources;

/// <summary>
/// ResXSerializationBinder (binding BinaryFormatter's types for ResXDataNode) and MultitargetUtil (the typeNameConverter's
/// names), the port of .NET Framework's, against SerializationBinder's documentation (learn.microsoft.com
/// System.Runtime.Serialization.SerializationBinder: BindToType, BindToName: null leaves the formatter's) and the reference
/// source (the full name, then without its version, then the type's name alone; a converter that throws is not used).
/// </summary>
public class ResXSerializationBinderTests
{
    [Fact] // reference source (BindToType): without a type resolution service, the formatter's own binding (null)
    public void Without_a_resolver_types_are_not_bound()
    {
        Assert.Null(new ResXSerializationBinder((System.ComponentModel.Design.ITypeResolutionService)null).BindToType("A", "T"));
    }

    [Fact] // reference source (BindToType): "T, assembly", then the assembly without its version, then T alone
    public void Types_are_bound_by_their_name_then_without_version_then_alone()
    {
        var resolver = new TableResolver();
        resolver.Types["T, A, Version=1.0.0.0, Culture=neutral"] = typeof(int);
        Assert.Equal(typeof(int), new ResXSerializationBinder(resolver).BindToType("A, Version=1.0.0.0, Culture=neutral", "T"));

        resolver = new TableResolver();
        resolver.Types["T, A, Culture=neutral"] = typeof(long);
        Assert.Equal(typeof(long), new ResXSerializationBinder(resolver).BindToType("A, version=1.0.0.0, Culture=neutral", "T"));
        Assert.Equal(new[] { "T, A, version=1.0.0.0, Culture=neutral", "T, A, Culture=neutral" }, resolver.Asked);

        resolver = new TableResolver();
        resolver.Types["T"] = typeof(short);
        Assert.Equal(typeof(short), new ResXSerializationBinder(resolver).BindToType("A, Version=1.0.0.0", "T"));
        Assert.Equal(new[] { "T, A, Version=1.0.0.0", "T, A", "T" }, resolver.Asked);

        resolver = new TableResolver();
        Assert.Null(new ResXSerializationBinder(resolver).BindToType("A", "T")); // no version to drop
        Assert.Equal(new[] { "T, A" }, resolver.Asked);
    }

    [Fact] // reference source (BindToName): without a converter, or a name it gives that is not assembly-qualified: the formatter's (null)
    public void Names_are_the_formatters_without_a_converter()
    {
        new ResXSerializationBinder((Func<Type, string>)null).BindToName(typeof(int), out var assembly, out var type);
        Assert.Null(assembly);
        Assert.Null(type);
        new ResXSerializationBinder(t => "NoAssembly").BindToName(typeof(int), out assembly, out type);
        Assert.Null(assembly);
        Assert.Null(type);
        new ResXSerializationBinder(t => "T,").BindToName(typeof(int), out assembly, out type);
        Assert.Null(assembly);
        // a type without a name (a generic parameter): none
        new ResXSerializationBinder(t => null).BindToName(typeof(List<>).GetGenericArguments()[0], out assembly, out type);
        Assert.Null(assembly);
        Assert.Null(type);
    }

    [Fact] // reference source (BindToName): the converter's assembly; its type name only when it differs from the type's
    public void Names_are_the_converters()
    {
        new ResXSerializationBinder(t => "System.Int32, mscorlib, Version=2.0.0.0").BindToName(typeof(int), out var assembly, out var type);
        Assert.Equal("mscorlib, Version=2.0.0.0", assembly);
        Assert.Null(type);
        new ResXSerializationBinder(t => "Old.Name, Old.Assembly").BindToName(typeof(int), out assembly, out type);
        Assert.Equal("Old.Assembly", assembly);
        Assert.Equal("Old.Name", type);
    }

    [Fact] // docs (ResXResourceWriter(..., typeNameConverter)): serialized values name their types as the converter does
    public void A_serialized_value_names_its_type_as_the_converter_does()
    {
        var text = Resx.Write(w => w.AddResource("p", new Plain { X = 1 }), t => t == typeof(Plain) ? "Old.Plain, Old.Assembly" : null);
        var base64 = text.Substring(text.IndexOf("<value>", text.IndexOf("name=\"p\"")) + 7);
        var bytes = Convert.FromBase64String(base64.Substring(0, base64.IndexOf('<')).Replace("\r\n", "").Replace(" ", ""));
        var serialized = System.Text.Encoding.UTF8.GetString(bytes);
        Assert.Contains("Old.Assembly", serialized);
        Assert.Contains("Old.Plain", serialized);
    }

    // --- MultitargetUtil

    [Fact] // reference source (MultitargetUtil.GetAssemblyQualifiedName): the converter's name; none, empty, or a converter that throws: the type's own
    public void A_type_is_named_by_the_converter_else_by_itself()
    {
        Assert.Null(MultitargetUtil.GetAssemblyQualifiedName(null, t => "x"));
        Assert.Equal("x", MultitargetUtil.GetAssemblyQualifiedName(typeof(int), t => "x"));
        Assert.Equal(Resx.Aqn<int>(), MultitargetUtil.GetAssemblyQualifiedName(typeof(int), null));
        Assert.Equal(Resx.Aqn<int>(), MultitargetUtil.GetAssemblyQualifiedName(typeof(int), t => ""));
        Assert.Equal(Resx.Aqn<int>(), MultitargetUtil.GetAssemblyQualifiedName(typeof(int), t => throw new InvalidOperationException()));
    }

    [Fact] // reference source (IsSecurityOrCriticalException): critical and security exceptions are not caught; the port's choice: ResponseEndException is (as ThreadAbortException)
    public void A_converter_throwing_a_critical_exception_is_not_caught()
    {
        foreach (var e in new Exception[]
        {
            new NullReferenceException(), new StackOverflowException(), new OutOfMemoryException(), new System.Web.ResponseEndException(),
            new ExecutionEngineException(), new IndexOutOfRangeException(), new AccessViolationException(), new SecurityException(),
        })
        {
            var thrown = Assert.ThrowsAny<Exception>(() => MultitargetUtil.GetAssemblyQualifiedName(typeof(int), t => throw e));
            Assert.Same(e, thrown);
        }
    }
}
