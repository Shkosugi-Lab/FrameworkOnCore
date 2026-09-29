using FrameworkOnCore.Analysis;
using Mono.Cecil;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The analysis tells a DLL's reference that binds on .NET 10 as it is ([mscorlib]ArrayList: mscorlib forwards it) from
/// one the converter retargets ([mscorlib]CallContext: .NET has it only in the fork's System.Web): the page and
/// API-ANALYSIS.md say "付け替える" for those (ApiUsage.RetargetedTo).
/// </summary>
public sealed class DllBindingTests : IDisposable
{
    readonly string directory = Directory.CreateTempSubdirectory("foc-binding-").FullName;

    public void Dispose() => Directory.Delete(directory, recursive: true);

    TargetApis Target()
    {
        // The fork's System.Web, with the one type this test needs.
        using var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("System.Web", new Version(4, 0, 0, 0)), "System.Web", ModuleKind.Dll);
        var module = assembly.MainModule;
        var type = new TypeDefinition("System.Runtime.Remoting.Messaging", "CallContext", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, module.TypeSystem.Object);
        module.Types.Add(type);
        var web = Path.Combine(directory, "System.Web.dll");
        assembly.Write(web);
        return new TargetApis([("fork:WebFormsForCore.Web", [web]), ("in-box", ReferencePacks.NetCoreApp())]);
    }

    [Fact]
    public void A_reference_mscorlib_forwards_binds_one_only_the_fork_has_is_retargeted()
    {
        var target = Target();

        Assert.True(target.Binds("mscorlib", "System.Collections.ArrayList"));
        Assert.True(target.Binds("mscorlib", "System.Collections.Generic.List`1"));
        Assert.False(target.Binds("mscorlib", "System.Runtime.Remoting.Messaging.CallContext"));
        Assert.True(target.Binds("System.Web", "System.Runtime.Remoting.Messaging.CallContext"));

        var info = target.Find("T:System.Runtime.Remoting.Messaging.CallContext");
        Assert.NotNull(info);
        Assert.Equal(("System.Web", "System.Runtime.Remoting.Messaging.CallContext"), (info.Assembly, info.Type));
    }

    [Fact]
    public void A_member_gives_its_outermost_type_as_metadata_names_it()
    {
        var info = Target().Find("M:System.Collections.Generic.List`1.Add(`0)");

        Assert.NotNull(info);
        Assert.Equal("System.Collections.Generic.List`1", info.Type);
    }
}
