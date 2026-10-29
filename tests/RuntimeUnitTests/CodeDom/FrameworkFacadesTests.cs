using WebFormsForCore.CodeDom.Compiler;

namespace FrameworkOnCore.Runtime.Tests.CodeDom;

/// <summary>
/// The runtime's facades every compilation references (FrameworkFacades), as its summary has them (the port's choice: .NET
/// Framework's compilers had the assemblies themselves): the assemblies of the runtime's folder that only forward types
/// (mscorlib, netstandard, System, System.Core...), not one the application's bin has, not System.Web.
/// </summary>
public class FrameworkFacadesTests : CompilationTest
{
    static readonly string Runtime = Path.GetDirectoryName(typeof(object).Assembly.Location);

    [Fact]
    public void The_type_forwarding_assemblies_of_the_runtime()
    {
        var names = FrameworkFacades.Paths.Select(Path.GetFileName).ToList();
        Assert.All(FrameworkFacades.Paths, path => Assert.Equal(Runtime, Path.GetDirectoryName(path)));
        Assert.Contains("mscorlib.dll", names);
        Assert.Contains("netstandard.dll", names);
        Assert.Contains("System.Core.dll", names);
        // assemblies with types of their own, and native ones, are not
        Assert.DoesNotContain("System.Private.CoreLib.dll", names);
        Assert.DoesNotContain("System.Linq.dll", names);
        Assert.DoesNotContain("coreclr.dll", names);
        Assert.DoesNotContain("System.Web.dll", names);
        Assert.Equal(names.OrderBy(n => n, StringComparer.OrdinalIgnoreCase), names);
    }

    [Fact]
    public void Not_one_the_bin_has()
    {
        var bin = System.Web.Hosting.AppBinDirectory.PhysicalPath;
        Assert.DoesNotContain(FrameworkFacades.Paths, path => File.Exists(Path.Combine(bin, Path.GetFileName(path))));
    }
}
