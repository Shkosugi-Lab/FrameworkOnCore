using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace FrameworkOnCore.Tests;

/// <summary>
/// A conversion from the deployed site (--site: what the application is made of, the original build's web folder or the
/// folder on the IIS server), the site in every shape of one thing at a time: what its bin has besides the application's
/// assemblies (a native library, an empty file, files in folders, Roslyn's compiler), no bin at all, an assembly built by a
/// project the web project does not reference, files only the site has, a package of the site with the application's
/// assembly in it, the site in a folder with a space and Japanese, the site the web project's own folder. Held:
/// <list type="bullet">
/// <item>the conversion builds, and the site is assembled (out\site) with the application's assemblies those built for
/// .NET (not the deployed .NET Framework ones);</item>
/// <item>what else the deployed site has is there as it was, byte for byte;</item>
/// <item>the application's folder and the deployed site are left as they were.</item>
/// </list>
/// The packages' feed is needed (each case is a build): where it is.
/// </summary>
public sealed class SiteSweepTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-site").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }

    static readonly byte[] NotAnAssembly = Encoding.ASCII.GetBytes("MZ this is the deployed .NET Framework assembly's place");

    // The deployed site of the application: the web project's folder without its sources and project, and bin with the
    // application's assemblies (their place: the conversion replaces them).
    static void Deploy(string source, string site, bool bin = true)
    {
        foreach (var file in Directory.EnumerateFiles(Path.Combine(source, "Web"), "*", SearchOption.AllDirectories))
        {
            if (file.EndsWith(".cs") || file.EndsWith(".csproj") || file.EndsWith("packages.config")) continue;
            var target = Path.Combine(site, Path.GetRelativePath(Path.Combine(source, "Web"), file));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target, overwrite: true);
        }
        if (!bin) return;
        Directory.CreateDirectory(Path.Combine(site, "bin"));
        File.WriteAllBytes(Path.Combine(site, "bin", "App.dll"), NotAnAssembly);
        File.WriteAllBytes(Path.Combine(site, "bin", "Library.dll"), NotAnAssembly);
    }

    static void Put(string folder, string relative, byte[] bytes)
    {
        var file = Path.Combine(folder, relative.Replace('\\', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllBytes(file, bytes);
    }

    static bool IsNetAssembly(string file)
    {
        try { return AssemblyName.GetAssemblyName(file) != null; }
        catch (BadImageFormatException) { return false; }
    }

    [Theory]
    [InlineData("plain")]
    [InlineData("other files in bin")]
    [InlineData("no bin")]
    [InlineData("a project the web project does not reference")]
    [InlineData("files only the site has")]
    [InlineData("a package of the site")]
    [InlineData("in a folder with a space and Japanese")]
    [InlineData("the web project's own folder")]
    public void A_conversion_from_the_deployed_site(string shape)
    {
        var runtime = ConverterSweepTests.RealRuntime();
        if (runtime == null) return;   // (no feed here)
        var source = Path.Combine(root, "source");
        var output = Path.Combine(root, "output");
        var site = Path.Combine(root, shape == "in a folder with a space and Japanese" ? "deployed サイト (1)" : "deployed");
        var project = ConverterSweepTests.Write(source, new ConverterSweepTests.App());
        // What the site has that the conversion is to leave as it is: (path in the site, bytes).
        var kept = new List<(string Path, byte[] Bytes)>();
        void Keep(string path, string text) { kept.Add((path, Encoding.UTF8.GetBytes(text))); Put(site, path, Encoding.UTF8.GetBytes(text)); }
        if (shape == "the web project's own folder")
        {
            site = Path.Combine(source, "Web");
            Put(site, @"bin\App.dll", NotAnAssembly);
            Put(site, @"bin\Library.dll", NotAnAssembly);
        }
        else Deploy(source, site, bin: shape != "no bin");
        switch (shape)
        {
            case "other files in bin":
                Keep(@"bin\Native.dll", "a native library: no assembly");
                Keep(@"bin\Empty.dll", "");
                Keep(@"bin\x86\SQLite.Interop.dll", "a native library in a folder");
                Keep(@"bin\roslyn\csc.exe", "the compiler ASP.NET ran");
                Put(site, @"bin\App.pdb", Encoding.UTF8.GetBytes("symbols"));   // (the application's assembly's: the .NET build's replaces it)
                Keep(@"bin\Notes.txt", "notes");
                Keep(@"bin\ja\App.resources.dll", "a satellite's place");
                break;
            case "a project the web project does not reference":
                // (a feature built into the site by the repository's build, as mojoPortal's)
                Put(source, @"Feature\Feature.csproj", Encoding.UTF8.GetBytes("""
                    <?xml version="1.0" encoding="utf-8"?>
                    <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                      <PropertyGroup><OutputType>Library</OutputType><AssemblyName>Feature</AssemblyName><TargetFrameworkVersion>v4.8</TargetFrameworkVersion></PropertyGroup>
                      <ItemGroup><Reference Include="System" /></ItemGroup>
                      <ItemGroup><Compile Include="Feature.cs" /></ItemGroup>
                    </Project>
                    """));
                Put(source, @"Feature\Feature.cs", Encoding.UTF8.GetBytes("namespace Feature { public class Thing { public string Name = \"feature\"; } }\n"));
                Put(site, @"bin\Feature.dll", NotAnAssembly);
                break;
            case "files only the site has":
                Keep("Deployed.aspx", "<%@ Page Language=\"C#\" %><html><body>deployed only</body></html>");
                Keep(@"Uploads\a.txt", "uploaded");
                Keep(@"App_Data\data.xml", "<data />");
                Keep("robots.txt", "User-agent: *");
                break;
            case "a package of the site":
                // (an installer's package with the application's assembly in it, as DNN's: installed, it would replace the .NET one)
                using (var zip = ZipFile.Open(Path.Combine(Directory.CreateDirectory(Path.Combine(site, "Install")).FullName, "module.zip"), ZipArchiveMode.Create))
                {
                    using (var entry = zip.CreateEntry("bin/App.dll").Open()) entry.Write(NotAnAssembly);
                    using (var entry = zip.CreateEntry("readme.txt").Open()) entry.Write(Encoding.UTF8.GetBytes("readme"));
                }
                break;
        }
        var sourceBefore = ConverterSweepTests.Tree(source);
        var siteBefore = ConverterSweepTests.Tree(site);

        var (exit, text) = ConverterSweepTests.Run(root, "dotnet", Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll"), project, "--out", output, "--runtime", runtime, "--site", site, "--deploy", "none");
        Assert.True(exit == 0 && text.Contains("build succeeded") && !text.Contains("Unhandled exception"), $"exit {exit}\n{text}");
        Assert.Equal("", ConverterSweepTests.Differences(sourceBefore, ConverterSweepTests.Tree(source)));
        Assert.Equal("", ConverterSweepTests.Differences(siteBefore, ConverterSweepTests.Tree(site)));

        // The site assembled: the application's assemblies those built for .NET, the page and its configuration there.
        var assembled = Path.Combine(output, "site");
        Assert.True(Directory.Exists(assembled), text);
        foreach (var assembly in shape == "a project the web project does not reference" ? new[] { "App.dll", "Library.dll", "Feature.dll" } : new[] { "App.dll", "Library.dll" })
            Assert.True(IsNetAssembly(Path.Combine(assembled, "bin", assembly)), $"site\\bin\\{assembly} is not the .NET build\n{text}");
        Assert.True(File.Exists(Path.Combine(assembled, "Default.aspx")) && File.Exists(Path.Combine(assembled, "Web.config")), string.Join("\n", ConverterSweepTests.Tree(assembled).Keys.Take(60)));
        // What else the site has: as it was.
        foreach (var (path, bytes) in kept)
        {
            var file = Path.Combine(assembled, path.Replace('\\', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(file), $"site\\{path} is not there\n{text}");
            Assert.True(bytes.AsSpan().SequenceEqual(File.ReadAllBytes(file)), $"site\\{path} is not as it was");
        }
        if (shape == "a package of the site")
        {
            using var zip = ZipFile.OpenRead(Path.Combine(assembled, "Install", "module.zip"));
            var inside = Path.Combine(root, "inside.dll");
            zip.GetEntry("bin/App.dll")!.ExtractToFile(inside, overwrite: true);
            Assert.True(IsNetAssembly(inside), "the package's App.dll is the deployed .NET Framework one still");
            Assert.NotNull(zip.GetEntry("readme.txt"));
        }
    }
}
