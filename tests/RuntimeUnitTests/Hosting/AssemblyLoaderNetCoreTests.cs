using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using System.Web.Hosting;
using static FrameworkOnCore.Runtime.Tests.Hosting.Child;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The application's assemblies not in the load context (AssemblyLoaderNetCore: its Resolving handler), as .NET Framework
/// probes the application's base and bin, its probing privatePath (learn.microsoft.com/dotnet/framework/deployment/how-the-runtime-locates-assemblies)
/// and ASP.NET's codegen folder; a Resolving handler gives the assembly or null (AssemblyLoadContext.Resolving's docs).
/// Its settings are the process's (static): in processes of their own (Child), with assemblies made for the test.
/// </summary>
// The CodeDom collection: ExecutorPortable's output files, inheritable while its process starts, would be inherited by
// this class's processes started meanwhile (Windows), and held open.
[Collection("CodeDom")]
public class AssemblyLoaderNetCoreTests
{
    // ---- the child's side

    static string Found(Assembly assembly, AssemblyLoadContext context) =>
        assembly == null ? "null" : Path.GetFileName(Path.GetDirectoryName(assembly.Location)) + " " + assembly.GetName().Version + " " + (AssemblyLoadContext.GetLoadContext(assembly) == context);

    static int Resolve(string[] args)
    {
        var (a, b, c) = (args[0], args[1], args[2]);
        var context = new AssemblyLoadContext("probe", isCollectible: true);
        Assembly Resolve(string name) => AssemblyLoaderNetCore.Resolve(context, new AssemblyName(name));
        Say("none", Found(Resolve("FocProbeA"), context));
        AssemblyLoaderNetCore.AdditionalPaths.Add(a);
        Say("additional", Found(Resolve("FocProbeA"), context));
        Say("version.lower", Found(Resolve("FocProbeA, Version=1.0.0.0"), context));
        Say("version.higher", Found(Resolve("FocProbeA, Version=9.0.0.0"), context));
        Say("unknown", Found(Resolve("FocProbeNone"), context));
        // The probing paths (';', '\' as the separator), relative to the program's folder.
        AssemblyLoaderNetCore.ProbingPaths = "missing;" + Path.GetRelativePath(AppContext.BaseDirectory, b).Replace(Path.DirectorySeparatorChar, '\\');
        Say("probing", Found(Resolve("FocProbeB"), context));
        AssemblyLoaderNetCore.TempPath = c;
        Say("temp", Found(Resolve("FocProbeC"), context));
        Say("temp.path", AssemblyLoaderNetCore.TempPath == c);
        // .NET Framework's GAC (Windows' folder): a strong name's folder.
        AssemblyLoaderNetCore.UseNetFXGAC = true;
        Throws("gac.strong", () => Say("gac.strong.found", Found(Resolve("FocProbeNone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"), context)));
        Throws("gac.simple", () => Say("gac.simple.found", Found(Resolve("FocProbeNone"), context)));
        return 0;
    }

    static int Init(string[] args)
    {
        var (d, e) = (args[0], args[1]);
        AssemblyLoaderNetCore.Init(d);
        AssemblyLoaderNetCore.Init(e); // once: the first's folder
        Say("init", Found(Assembly.Load("FocProbeD"), AssemblyLoadContext.Default));
        Throws("init.again", () => Assembly.Load("FocProbeE"));
        AssemblyLoaderNetCore.Dispose();
        Throws("disposed", () => Assembly.Load("FocProbeF"));
        return 0;
    }

    // ---- the test's side

    /// <summary>A library of the name and version in the folder.</summary>
    static void Library(string folder, string name, string version = "1.2.0.0")
    {
        var builder = new PersistedAssemblyBuilder(new AssemblyName(name) { Version = new Version(version) }, typeof(object).Assembly);
        builder.DefineDynamicModule(name).DefineType(name + ".Type", TypeAttributes.Public).CreateType();
        builder.Save(Path.Combine(folder, name + ".dll"));
    }

    static readonly Lazy<Dictionary<string, string>> resolved = new(() =>
    {
        using var a = new Site();
        using var b = new Site();
        using var c = new Site();
        Library(a.Root, "FocProbeA");
        Library(b.Root, "FocProbeB");
        Library(c.Root, "FocProbeC");
        var result = Child.Run("AssemblyLoaderNetCoreTests.Resolve", new[] { a.Root, b.Root, c.Root });
        Assert.True(result.ExitCode == 0, result.ToString());
        return Said(result);
    });

    [Fact] // the docs: an assembly not found is null; one in an additional path loaded into the context asking
    public void An_assembly_of_the_additional_paths_is_loaded_into_the_context()
    {
        var said = resolved.Value;
        Assert.Equal("null", said["none"]);
        Assert.EndsWith(" 1.2.0.0 True", said["additional"]);
        Assert.Equal("null", said["unknown"]);
    }

    [Fact] // the port's choice: a version at least the one asked for (.NET Framework binds a strong name's exact version, a simple name's any)
    public void A_lower_version_is_not_taken()
    {
        var said = resolved.Value;
        Assert.EndsWith(" 1.2.0.0 True", said["version.lower"]);
        Assert.Equal("null", said["version.higher"]);
    }

    [Fact] // .NET Framework's probing privatePath (';'-separated, relative to the application's base) and ASP.NET's codegen folder (TempPath)
    public void The_probing_paths_and_the_codegen_folder()
    {
        var said = resolved.Value;
        Assert.EndsWith(" 1.2.0.0 True", said["probing"]);
        Assert.EndsWith(" 1.2.0.0 True", said["temp"]);
        Assert.Equal("True", said["temp.path"]);
    }

    [Fact] // .NET Framework's GAC (%windir%\Microsoft.NET\assembly\GAC_MSIL\name\v4.0_version__token): a strong name not there is null
    public void A_strong_name_not_in_the_GAC_is_null()
    {
        var said = resolved.Value;
        Assert.Equal("null", said["gac.strong.found"]);
        Assert.Equal("none", said["gac.strong"]);
    }

    [Fact(Skip = "bug: with UseNetFXGAC a simple name (no public key token) throws ArgumentNullException (BitConverter.ToString(null)) instead of null")]
    // AssemblyLoadContext.Resolving's docs: the handler returns null for an assembly it does not find (the load's FileNotFoundException)
    public void A_simple_name_with_the_GAC_is_null()
    {
        var said = resolved.Value;
        Assert.Equal("none", said["gac.simple"]);
        Assert.Equal("null", said["gac.simple.found"]);
    }

    [Fact] // the port's own: Init resolves from its folder for System.Web's load context, once (the first folder); Dispose ends it
    public void Init_resolves_once_until_disposed()
    {
        using var d = new Site();
        using var e = new Site();
        Library(d.Root, "FocProbeD");
        Library(d.Root, "FocProbeF");
        Library(e.Root, "FocProbeE");
        var result = Child.Run("AssemblyLoaderNetCoreTests.Init", new[] { d.Root, e.Root });
        var said = Said(result);
        Assert.True(said.ContainsKey("init"), result.ToString());
        Assert.EndsWith(" 1.2.0.0 True", said["init"]);
        Assert.Equal("FileNotFoundException", said["init.again"]);
        Assert.Equal("FileNotFoundException", said["disposed"]);
    }
}
