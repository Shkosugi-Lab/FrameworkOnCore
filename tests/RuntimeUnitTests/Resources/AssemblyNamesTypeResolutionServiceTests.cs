using System.Reflection;
using System.Resources;

namespace FrameworkOnCore.Runtime.Tests.Resources;

/// <summary>
/// AssemblyNamesTypeResolutionService (what ResXResourceReader and ResXDataNode resolve types with given assembly names),
/// the port of .NET Framework's, against ITypeResolutionService's documentation (learn.microsoft.com
/// System.ComponentModel.Design.ITypeResolutionService: GetAssembly, GetType, GetPathOfAssembly, throwOnError) and the
/// reference source (the assemblies named searched, the one named by the type first).
/// </summary>
public class AssemblyNamesTypeResolutionServiceTests
{
    static readonly AssemblyName Core = typeof(int).Assembly.GetName();
    static readonly AssemblyName Tests = typeof(Plain).Assembly.GetName();

    [Fact] // docs (GetAssembly): the assembly named, loaded (kept: the same one asked again)
    public void An_assembly_is_loaded_by_its_name()
    {
        var service = new AssemblyNamesTypeResolutionService(null);
        Assert.Same(typeof(int).Assembly, service.GetAssembly(Core));
        Assert.Same(typeof(int).Assembly, service.GetAssembly(Core, false));
    }

    [Fact] // docs (ITypeResolutionService.GetAssembly(name, throwOnError)): false, and null when the assembly cannot be located
    public void An_assembly_not_found_is_null_when_asked_not_to_throw()
    {
        Assert.Null(new AssemblyNamesTypeResolutionService(null).GetAssembly(new AssemblyName("No.Such.Assembly"), false));
    }

    [Fact] // docs (GetPathOfAssembly): the assembly name's code base (reference source)
    public void The_path_of_an_assembly_is_its_code_base()
    {
        Assert.Equal("file:///c:/x/a.dll", new AssemblyNamesTypeResolutionService(null).GetPathOfAssembly(new AssemblyName("a") { CodeBase = "file:///c:/x/a.dll" }));
    }

    [Fact] // docs (GetType): an assembly-qualified name; not found: null, or ArgumentException when asked to throw
    public void A_type_is_found_by_its_assembly_qualified_name()
    {
        var service = new AssemblyNamesTypeResolutionService(null);
        Assert.Equal(typeof(int), service.GetType("System.Int32, mscorlib"));
        Assert.Equal(typeof(int), service.GetType("system.int32, mscorlib", false, true));
        Assert.Null(service.GetType("system.int32, mscorlib", false, false));
        Assert.Null(service.GetType("System.Int32", false)); // no assembly, none named
        Assert.Null(service.GetType("No.Such.Type, No.Such.Assembly", false));
        Assert.Throws<ArgumentException>(() => service.GetType("No.Such.Type, mscorlib"));
    }

    [Fact] // reference source (GetType): the assemblies named are searched, the one the type names first; the name without its assembly too
    public void A_type_is_looked_for_in_the_assemblies_named()
    {
        var service = new AssemblyNamesTypeResolutionService(new[] { Core, Tests });
        Assert.Equal(typeof(int), service.GetType("System.Int32", false));
        Assert.Equal(typeof(Plain), service.GetType(typeof(Plain).FullName + ", Other.Assembly", false));
        Assert.Null(service.GetType(typeof(Plain).FullName + "Missing, " + Tests.Name, false));
        // generic types: an argument without its assembly; the first comma within the arguments (no assembly name after it)
        Assert.Equal(typeof(List<int>), service.GetType("System.Collections.Generic.List`1[[System.Int32]], " + Core.Name, false));
        Assert.Null(service.GetType("System.Collections.Generic.List`1[[System.Int32, mscorlib]], No.Such.Assembly", false));
    }

    [Fact] // docs (ReferenceAssembly): not supported (reference source)
    public void Referencing_an_assembly_is_not_supported()
    {
        Assert.Throws<NotSupportedException>(() => new AssemblyNamesTypeResolutionService(null).ReferenceAssembly(Core));
    }
}
