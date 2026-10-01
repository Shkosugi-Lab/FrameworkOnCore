using FrameworkOnCore.Analysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The analysis of what an application uses (ApiAnalyzer), on a small repository made for each test: its sources compiled
/// against .NET Framework 4.8's reference assemblies, looked up in .NET 10's (with System.Drawing.Common, as the converter
/// adds it). Each API counted where it is written, its status on .NET, its component.
/// </summary>
public sealed class ApiAnalysisTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-analysis-" + Guid.NewGuid().ToString("N"));

    static readonly Lazy<TargetApis> target = new(() => new TargetApis([
        ("package:System.Drawing.Common", ReferencePacks.Package("System.Drawing.Common", "10.0.0")),
        ("in-box", ReferencePacks.NetCoreApp()),
    ]));

    static string OldProject(string name, string compile, string references, bool visualBasic = false) => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
          <PropertyGroup>
            <OutputType>Library</OutputType>
            <AssemblyName>{name}</AssemblyName>
            <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
            {(visualBasic ? "<RootNamespace>App</RootNamespace><OptionStrict>Off</OptionStrict>" : "")}
          </PropertyGroup>
          <ItemGroup>
            <Reference Include="System" />
            <Reference Include="System.Core" />
        {references}
          </ItemGroup>
          <ItemGroup>
        {compile}
          </ItemGroup>
        </Project>
        """;

    void Write(string relative, string text)
    {
        var path = Path.Combine(root, relative.Replace('\\', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text);
    }

    AnalysisResult Analyze(string project) =>
        new ApiAnalyzer(target.Value, Catalog.Default()).Analyze(Path.Combine(root, project.Replace('\\', Path.DirectorySeparatorChar)), root);

    static ApiUsage Api(AnalysisResult result, string id) => Assert.Single(result.Apis, a => a.Id == id);

    [Fact]
    public void Each_api_is_counted_with_its_status_on_net_and_its_component()
    {
        Write(@"Web\Page1.cs", """
            using System;
            using System.Drawing;
            using System.IO;
            using System.Runtime.Serialization.Formatters.Binary;
            using System.Text;
            using System.Threading;

            public class Page1 : System.Web.UI.Page
            {
                delegate int Work(int x);
                protected override void OnLoad(EventArgs e)
                {
                    using (var b = new Bitmap(10, 10)) { }
                    new BinaryFormatter().Serialize(new MemoryStream(), 1);
                    var bytes = Encoding.Default.GetBytes("x");
                    try { Response.Redirect("/", true); } catch (ThreadAbortException) { Thread.ResetAbort(); }
                    var name = new DirectoryInfo("/").FullName;
                    var c = name[0];
                    Work w = x => x;
                    w.EndInvoke(w.BeginInvoke(1, null, null));
                    Response.Write(name);
                    Response.Write(c);
                }
            }
            """);
        Write(@"Web\Web.csproj", OldProject("Web", "    <Compile Include=\"Page1.cs\" />",
            "    <Reference Include=\"System.Drawing\" />\n    <Reference Include=\"System.Web\" />"));

        var result = Analyze(@"Web\Web.csproj");

        Assert.Equal(ApiStatus.WindowsOnly, Api(result, "M:System.Drawing.Bitmap.#ctor(System.Int32,System.Int32)").Status);
        Assert.Equal("system-drawing", Api(result, "T:System.Drawing.Bitmap").Component);
        var serialize = Api(result, "M:System.Runtime.Serialization.Formatters.Binary.BinaryFormatter.Serialize(System.IO.Stream,System.Object)");
        Assert.Equal(ApiStatus.Throws, serialize.Status);
        Assert.Equal("binary-formatter", serialize.Component);
        Assert.Equal(ApiStatus.Behavior, Api(result, "P:System.Text.Encoding.Default").Status);
        Assert.Equal(ApiStatus.Throws, Api(result, "M:System.Threading.Thread.ResetAbort").Status);
        Assert.Equal(ApiStatus.Throws, Api(result, ApiKey.DelegateBeginInvoke.Id).Status);
        // On a base type on .NET (FileSystemInfo.FullName); an indexer (String's, Chars in metadata): there.
        Assert.Equal(ApiStatus.Available, Api(result, "P:System.IO.DirectoryInfo.FullName").Status);
        Assert.Equal(ApiStatus.Available, Api(result, "P:System.String.Item(System.Int32)").Status);
        // System.Web: not in .NET, nor in these targets (FrameworkOnCore's feed is not in the test): missing.
        var write = Api(result, "M:System.Web.HttpResponse.Write(System.String)");
        Assert.Equal(ApiStatus.Missing, write.Status);
        Assert.Equal("web-forms", write.Component);
        Assert.Equal(1, write.Count);
        Assert.Equal(new SourcePlace("Web/Page1.cs", 21), write.Places.Single());
        // A member overridden counts as used.
        Assert.Equal(1, Api(result, "M:System.Web.UI.Control.OnLoad(System.EventArgs)").Count);

        var components = result.Components.ToDictionary(c => c.Id);
        Assert.Equal(ApiStatus.WindowsOnly, components["system-drawing"].Status);
        Assert.Equal(0, Assert.Single(result.Projects).Unresolved);
    }

    [Fact]
    public void Visual_basic_sources_and_the_projects_referenced_are_analyzed()
    {
        Write(@"Lib\Lib.cs", "public static class Helper { public static System.Text.Encoding Enc() => System.Text.Encoding.GetEncoding(932); }");
        Write(@"Lib\Lib.csproj", OldProject("Lib", "    <Compile Include=\"Lib.cs\" />", ""));
        Write(@"App\Module1.vb", """
            Imports System.Text
            Public Module Module1
                Public Function Bytes(s As String) As Byte()
                    Dim e = Helper.Enc()
                    Return Encoding.Default.GetBytes(s)
                End Function
            End Module
            """);
        Write(@"App\App.vbproj", OldProject("App", "    <Compile Include=\"Module1.vb\" />",
            "    <ProjectReference Include=\"..\\Lib\\Lib.csproj\"><Name>Lib</Name></ProjectReference>", visualBasic: true));

        var result = Analyze(@"App\App.vbproj");

        Assert.Equal(["Lib/Lib.csproj", "App/App.vbproj"], result.Projects.Select(p => p.Path));
        var @default = Api(result, "P:System.Text.Encoding.Default");
        Assert.Equal(["App/Module1.vb"], @default.Files.Select(f => f.File));
        Assert.Equal("code-pages", Api(result, "M:System.Text.Encoding.GetEncoding(System.Int32)~System.Text.Encoding").Component);
        Assert.Contains(result.Libraries, l => l.Assembly == "Lib" && l.Origin == "project");
    }

    [Fact] // openIMIS's ReportViewer: BinaryFormatter used by a DLL without source
    public void A_dll_without_source_is_read_for_what_it_references()
    {
        var framework = ReferencePacks.Framework();
        var library = CSharpCompilation.Create("Vendor",
            [CSharpSyntaxTree.ParseText("public static class Vendor { public static object Load(System.IO.Stream s) => new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(s); }")],
            [MetadataReference.CreateFromFile(framework["mscorlib"])], new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        Directory.CreateDirectory(Path.Combine(root, "lib"));
        Assert.True(library.Emit(Path.Combine(root, "lib", "Vendor.dll")).Success);
        Write(@"Web\Page1.cs", "public class Page1 { public object Get(System.IO.Stream s) => Vendor.Load(s); }");
        Write(@"Web\Web.csproj", OldProject("Web", "    <Compile Include=\"Page1.cs\" />",
            "    <Reference Include=\"Vendor\"><HintPath>..\\lib\\Vendor.dll</HintPath></Reference>"));

        var result = Analyze(@"Web\Web.csproj");

        var deserialize = Api(result, "M:System.Runtime.Serialization.Formatters.Binary.BinaryFormatter.Deserialize(System.IO.Stream)~System.Object");
        Assert.Equal(0, deserialize.Count);
        Assert.Equal("lib/Vendor.dll", Assert.Single(deserialize.Binaries!).File);
        Assert.Equal(ApiStatus.Throws, deserialize.Status);
        Assert.Contains(result.Binaries, b => b.File == "lib/Vendor.dll" && b.Skipped == null);
        Assert.Contains(result.Libraries, l => l.Assembly == "Vendor" && l.Origin == "dll");
    }

    public void Dispose()
    {
        try { Directory.Delete(root, recursive: true); } catch (IOException) { }
    }
}
