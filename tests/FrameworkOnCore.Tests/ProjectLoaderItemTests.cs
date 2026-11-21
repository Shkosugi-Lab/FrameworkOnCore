using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The sources the analysis reads (ProjectLoader) are the project's files as MSBuild finds them: in folders ('\' in the
/// project, on any system), escaped (Visual Studio writes "%28" for '(' and "%3b" for ';'), by wildcards whose segment has
/// a name's start before the '*'.
/// </summary>
public sealed class ProjectLoaderItemTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-items").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { }
    }

    void Write(string relative, string text)
    {
        var path = Path.Combine(root, relative.Replace('\\', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text);
    }

    [Fact]
    public void Items_in_folders_escaped_and_by_a_name_s_wildcard_are_read()
    {
        Write(@"App\Properties\AssemblyInfo.cs", "");
        Write(@"App\Copy (1) of Report.cs", "");
        Write(@"App\Semi;colon.cs", "");
        Write(@"App\Models\Generated.Customer.cs", "");
        Write(@"App\Models\Generated.Order.cs", "");
        Write(@"App\Models\Handwritten.cs", "");
        Write(@"App\App.csproj", """
            <?xml version="1.0" encoding="utf-8"?>
            <Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <PropertyGroup><AssemblyName>App</AssemblyName><TargetFrameworkVersion>v4.8</TargetFrameworkVersion></PropertyGroup>
              <ItemGroup>
                <Compile Include="Properties\AssemblyInfo.cs" />
                <Compile Include="Copy %281%29 of Report.cs" />
                <Compile Include="Semi%3bcolon.cs" />
                <Compile Include="Models\Generated*.cs" />
              </ItemGroup>
            </Project>
            """);
        var project = new ProjectLoader(root, "Release").Load(Path.Combine(root, "App", "App.csproj"));
        var names = project.Sources.Select(s => Path.GetRelativePath(Path.Combine(root, "App"), s).Replace('\\', '/')).OrderBy(n => n, StringComparer.Ordinal).ToArray();
        Assert.Equal(new[] { "Copy (1) of Report.cs", "Models/Generated.Customer.cs", "Models/Generated.Order.cs", "Properties/AssemblyInfo.cs", "Semi;colon.cs" }, names);
    }

    // The configurations' groups as Visual Studio writes them.
    const string Configurations = """
          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
            <DefineConstants>DEBUG;TRACE</DefineConstants>
            <DefineDebug>true</DefineDebug>
            <DefineTrace>true</DefineTrace>
          </PropertyGroup>
          <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
            <DefineConstants>TRACE</DefineConstants>
            <DefineDebug>false</DefineDebug>
            <DefineTrace>true</DefineTrace>
          </PropertyGroup>
        """;

    [Theory] // the symbols are the configuration's: DEBUG for Debug only (the analysis defined it for every one); Visual Basic's CONFIG and PLATFORM, DEBUG and TRACE by DefineDebug and DefineTrace
    [InlineData("App.csproj", "Debug", new[] { "DEBUG", "TRACE" })]
    [InlineData("App.csproj", "Release", new[] { "TRACE" })]
    [InlineData("App.vbproj", "Debug", new[] { "CONFIG=\"Debug\"", "DEBUG", "PLATFORM=\"AnyCPU\"", "TRACE" })]
    [InlineData("App.vbproj", "Release", new[] { "CONFIG=\"Release\"", "PLATFORM=\"AnyCPU\"", "TRACE" })]
    public void The_symbols_are_the_configuration_s(string file, string configuration, string[] symbols)
    {
        Write(@"App\Module1." + (file.EndsWith("vbproj", StringComparison.Ordinal) ? "vb" : "cs"), "");
        Write(@"App\" + file, $"""
            <?xml version="1.0" encoding="utf-8"?>
            <Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <PropertyGroup>
                <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
                <AssemblyName>App</AssemblyName><TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
              </PropertyGroup>
            {Configurations}
              <ItemGroup><Compile Include="Module1.{(file.EndsWith("vbproj", StringComparison.Ordinal) ? "vb" : "cs")}" /></ItemGroup>
            </Project>
            """);
        var project = new ProjectLoader(root, configuration).Load(Path.Combine(root, "App", file));
        Assert.Equal(symbols, project.Defines.OrderBy(s => s, StringComparer.Ordinal).ToArray());
    }
}
