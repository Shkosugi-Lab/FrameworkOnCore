using FrameworkOnCore.Converter;
using Mono.Cecil;

namespace FrameworkOnCore.Tests;

/// <summary>
/// DLLs without source bound to where .NET Framework had a type: retargeted to the assembly of the application that has
/// it (FrameworkOnCore's System.Web has CallContext; mscorlib on .NET does not), whatever assembly the reference named; the
/// references nothing has are reported; a reference to a later version than the application has is lowered.
/// </summary>
public sealed class AssemblyRetargeterTests : IDisposable
{
    readonly string directory = Directory.CreateTempSubdirectory("foc-retarget-").FullName;
    string Bin => Directory.CreateDirectory(Path.Combine(directory, "bin")).FullName;

    public void Dispose() => Directory.Delete(directory, recursive: true);

    // An assembly defining public types (Namespace.Name), of a version.
    string Provider(string name, Version version, params string[] types)
    {
        using var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(name, version), name, ModuleKind.Dll);
        var module = assembly.MainModule;
        foreach (var type in types)
        {
            var at = type.LastIndexOf('.');
            module.Types.Add(new TypeDefinition(type[..at], type[(at + 1)..], TypeAttributes.Public | TypeAttributes.Class, module.TypeSystem.Object));
        }
        var path = Path.Combine(Bin, name + ".dll");
        assembly.Write(path);
        return path;
    }

    // An assembly with a field of each type, the type referred to in the assembly named (as a .NET Framework build names it).
    string Consumer(string name, params (string Assembly, Version Version, string Type)[] references)
    {
        using var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(name, new Version(1, 0)), name, ModuleKind.Dll);
        var module = assembly.MainModule;
        var holder = new TypeDefinition("Consumer", "Holder", TypeAttributes.Public | TypeAttributes.Class, module.TypeSystem.Object);
        module.Types.Add(holder);
        var i = 0;
        foreach (var (assemblyName, version, type) in references)
        {
            var scope = module.AssemblyReferences.FirstOrDefault(r => r.Name == assemblyName)
                        ?? new AssemblyNameReference(assemblyName, version);
            if (!module.AssemblyReferences.Contains(scope)) module.AssemblyReferences.Add(scope);
            var at = type.LastIndexOf('.');
            holder.Fields.Add(new FieldDefinition($"f{i++}", FieldAttributes.Public, new TypeReference(type[..at], type[(at + 1)..], module, scope)));
        }
        var path = Path.Combine(Bin, name + ".dll");
        assembly.Write(path);
        return path;
    }

    static List<(string Scope, string Type)> References(string file)
    {
        using var module = ModuleDefinition.ReadModule(file);
        return module.GetTypeReferences().Where(t => t.Scope is AssemblyNameReference)
            .Select(t => (((AssemblyNameReference)t.Scope).Name, t.FullName)).ToList();
    }

    [Fact]
    public void A_type_mscorlib_does_not_have_goes_to_the_assembly_that_has_it()
    {
        Provider("System.Web", new Version(4, 0, 0, 0), "System.Runtime.Remoting.Messaging.CallContext");
        Consumer("SimpleInjector.Scoping",
            ("mscorlib", new Version(4, 0, 0, 0), "System.Runtime.Remoting.Messaging.CallContext"),
            ("mscorlib", new Version(4, 0, 0, 0), "System.Collections.ArrayList"));
        var into = Path.Combine(directory, "retargeted");
        var report = new Report();

        var results = AssemblyRetargeter.RetargetFolder(Bin, into, new HashSet<string> { "System.Web" }, new HashSet<string>(), report);

        var result = Assert.Single(results, r => r.Retargeted.Count > 0);
        Assert.Equal(("System.Runtime.Remoting.Messaging.CallContext", "mscorlib", "System.Web"), Assert.Single(result.Retargeted));
        var references = References(Path.Combine(into, "SimpleInjector.Scoping.dll"));
        Assert.Contains(("System.Web", "System.Runtime.Remoting.Messaging.CallContext"), references);
        // ArrayList resolves: mscorlib on .NET forwards it.
        Assert.Contains(("mscorlib", "System.Collections.ArrayList"), references);
        // The DLL in bin is left as it is.
        Assert.Contains(("mscorlib", "System.Runtime.Remoting.Messaging.CallContext"), References(Path.Combine(Bin, "SimpleInjector.Scoping.dll")));
    }

    [Fact]
    public void Any_assembly_not_only_mscorlib_and_the_shims_first()
    {
        // A type the DLL names in an assembly the application does not have, and two that have it.
        Provider("Other.Library", new Version(1, 0), "System.Web.Routing.RouteTable");
        Provider("System.Web.Routing", new Version(4, 0), "System.Web.Routing.RouteTable");
        Consumer("Plugin", ("System.Web.Routing.Old", new Version(3, 5), "System.Web.Routing.RouteTable"));

        var results = AssemblyRetargeter.RetargetFolder(Bin, Path.Combine(directory, "retargeted"), new HashSet<string> { "System.Web.Routing" }, new HashSet<string>(), new Report());

        Assert.Equal(("System.Web.Routing.RouteTable", "System.Web.Routing.Old", "System.Web.Routing"), Assert.Single(Assert.Single(results, r => r.File.EndsWith("Plugin.dll")).Retargeted));
    }

    [Fact]
    public void A_type_nothing_has_is_reported_and_the_DLL_not_written()
    {
        Consumer("Charts", ("System.Web.DataVisualization", new Version(4, 0), "System.Web.UI.DataVisualization.Charting.Chart"));
        var into = Path.Combine(directory, "retargeted");
        var report = new Report();

        var result = Assert.Single(AssemblyRetargeter.RetargetFolder(Bin, into, new HashSet<string>(), new HashSet<string>(), report));

        Assert.Empty(result.Retargeted);
        Assert.Equal(("System.Web.UI.DataVisualization.Charting.Chart", "System.Web.DataVisualization"), Assert.Single(result.Unresolved));
        Assert.False(File.Exists(Path.Combine(into, "Charts.dll")));
        Assert.Contains("[System.Web.DataVisualization]System.Web.UI.DataVisualization.Charting.Chart", report.ToMarkdown("t"));
    }

    [Fact]
    public void A_later_version_than_the_application_has_is_lowered_to_it()
    {
        Provider("Newtonsoft.Json", new Version(12, 0, 0, 0), "Newtonsoft.Json.JsonConvert");
        Consumer("Client", ("Newtonsoft.Json", new Version(13, 0, 0, 0), "Newtonsoft.Json.JsonConvert"));
        var into = Path.Combine(directory, "retargeted");

        var result = Assert.Single(AssemblyRetargeter.RetargetFolder(Bin, into, new HashSet<string>(), new HashSet<string>(), new Report()), r => r.File.EndsWith("Client.dll"));

        Assert.Equal(("(version 13.0.0.0)", "Newtonsoft.Json", "Newtonsoft.Json 12.0.0.0"), Assert.Single(result.Retargeted));
        using var module = ModuleDefinition.ReadModule(Path.Combine(into, "Client.dll"));
        Assert.Equal(new Version(12, 0, 0, 0), module.AssemblyReferences.Single(r => r.Name == "Newtonsoft.Json").Version);
    }

    [Fact]
    public void A_call_of_a_member_NET_does_not_have_goes_to_the_compatibility_assemblys_extension_member()
    {
        // The compatibility assembly as it is built (AppDomainSetup.PrivateBinPath: an extension property).
        var runtime = RuntimeSetup.Find()!;
        RuntimeSetup.EnsureShims(runtime);
        var compat = RuntimeSetup.ShimAssembly(RuntimeSetup.ShimProjects(runtime).Single(p => Path.GetFileNameWithoutExtension(p) == "FrameworkOnCore.Compat"));
        File.Copy(compat, Path.Combine(Bin, "FrameworkOnCore.Compat.dll"));

        // A .NET Framework library's call: AppDomain.CurrentDomain.SetupInformation.PrivateBinPath (OWIN's startup discovery).
        const string name = "Owin.Like";
        using (var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(name, new Version(1, 0)), name, ModuleKind.Dll))
        {
            var module = assembly.MainModule;
            var mscorlib = new AssemblyNameReference("mscorlib", new Version(4, 0, 0, 0)) { PublicKeyToken = [0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89] };
            module.AssemblyReferences.Add(mscorlib);
            var appDomain = new TypeReference("System", "AppDomain", module, mscorlib);
            var setup = new TypeReference("System", "AppDomainSetup", module, mscorlib);
            var @string = new TypeReference("System", "String", module, mscorlib);
            var holder = new TypeDefinition("Library", "Loader", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed, module.TypeSystem.Object);
            module.Types.Add(holder);
            var method = new MethodDefinition("BinPath", MethodAttributes.Public | MethodAttributes.Static, module.TypeSystem.String);
            holder.Methods.Add(method);
            var il = method.Body.GetILProcessor();
            il.Emit(Mono.Cecil.Cil.OpCodes.Call, new MethodReference("get_CurrentDomain", appDomain, appDomain));
            il.Emit(Mono.Cecil.Cil.OpCodes.Callvirt, new MethodReference("get_SetupInformation", setup, appDomain) { HasThis = true });
            il.Emit(Mono.Cecil.Cil.OpCodes.Callvirt, new MethodReference("get_PrivateBinPath", @string, setup) { HasThis = true });
            il.Emit(Mono.Cecil.Cil.OpCodes.Ret);
            assembly.Write(Path.Combine(Bin, name + ".dll"));
        }
        var into = Path.Combine(directory, "retargeted");

        var result = Assert.Single(AssemblyRetargeter.RetargetFolder(Bin, into, new HashSet<string> { "FrameworkOnCore.Compat" }, new HashSet<string>(), new Report()));

        Assert.Equal(("System.AppDomainSetup::get_PrivateBinPath", "System.AppDomainMembers::get_PrivateBinPath (FrameworkOnCore.Compat)"), Assert.Single(result.Replaced));
        Assert.Empty(result.MissingMembers);
        // It runs: the call is the compatibility assembly's (bin, as ASP.NET had it).
        var context = new System.Runtime.Loader.AssemblyLoadContext("retargeted", isCollectible: true);
        context.Resolving += (c, n) => n.Name == "FrameworkOnCore.Compat" ? c.LoadFromAssemblyPath(Path.Combine(Bin, "FrameworkOnCore.Compat.dll")) : null;
        try
        {
            var loaded = context.LoadFromStream(new MemoryStream(File.ReadAllBytes(Path.Combine(into, name + ".dll"))));
            Assert.Equal("bin", loaded.GetType("Library.Loader")!.GetMethod("BinPath")!.Invoke(null, null));
        }
        finally { context.Unload(); }
    }

    [Fact]
    public void The_applications_own_assemblies_are_not_looked_at()
    {
        Provider("System.Web", new Version(4, 0), "System.Runtime.Remoting.Messaging.CallContext");
        Consumer("MyApp", ("mscorlib", new Version(4, 0, 0, 0), "System.Runtime.Remoting.Messaging.CallContext"));

        var results = AssemblyRetargeter.RetargetFolder(Bin, Path.Combine(directory, "retargeted"), new HashSet<string> { "System.Web" }, new HashSet<string> { "MyApp" }, new Report());

        Assert.DoesNotContain(results, r => r.File.EndsWith("MyApp.dll"));
    }

    [Fact] // System.Web.Mvc.dll references System.Data.Linq while no project does: its package is to be added
    public void A_framework_reference_only_a_dll_makes_is_found()
    {
        Consumer("System.Web.Mvc", ("System.Data.Linq", new Version(4, 0, 0, 0), "System.Data.Linq.Binary"));
        Provider("Present", new Version(1, 0), "Present.SomeType");
        Consumer("Uses.Present", ("Present", new Version(1, 0), "Present.SomeType"));

        var missing = AssemblyRetargeter.ReferencedMissing(Bin, new[] { "System.Data.Linq", "Present" });

        // Present is in bin: only what is referenced and absent is reported, with who references it.
        Assert.Equal("System.Data.Linq", Assert.Single(missing).Key);
        Assert.Equal(["System.Web.Mvc.dll"], missing["System.Data.Linq"]);
    }
}
