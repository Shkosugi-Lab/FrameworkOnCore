using System.Text;
using System.Xml.Linq;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Project files edited after they were written (ProjectFile): a package's version raised (BuildFixer: NU1605, a family's
/// packages at one version) wherever the reference has it - an attribute in any order, a child element - and the file
/// written back as it was written, only the edit changed.
/// </summary>
public sealed class ProjectFileTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-project-file").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { }
    }

    string WriteProject(string name, string text, Encoding encoding)
    {
        var path = Path.Combine(root, name, name + ".csproj");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text, encoding);
        return path;
    }

    // As the converter writes an old-style project (text, LF, no declaration, no byte order mark) and as a repository has
    // an SDK-style one (CRLF, a declaration, a byte order mark).
    public static TheoryData<string, bool> Files => new()
    {
        { "<Project Sdk=\"Microsoft.NET.Sdk\">\n  <!-- converted -->\n  <PropertyGroup>\n    <NoWarn>$(NoWarn);NU1701</NoWarn>\n  </PropertyGroup>\n\n  <ItemGroup>\n    <PackageReference Include=\"A\" Version=\"1.0.0\" />\n    <Reference Include=\"B\" HintPath=\"$(PkgB)\\lib\\B.dll\" />\n  </ItemGroup>\n</Project>\n", false },
        { "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Project Sdk=\"Microsoft.NET.Sdk\">\r\n  <ItemGroup Condition=\"'$(TargetFramework)' == 'net10.0'\">\r\n    <PackageReference Include=\"A\" Version=\"1.0.0\" />\r\n  </ItemGroup>\r\n</Project>\r\n", true },
    };

    [Theory, MemberData(nameof(Files))]
    public void A_file_read_and_written_back_is_the_same_bytes(string text, bool bom)
    {
        var path = WriteProject("App", text, new UTF8Encoding(bom));
        var before = File.ReadAllBytes(path);
        ProjectFile.Load(path).Save();
        Assert.Equal(before, File.ReadAllBytes(path));
    }

    [Theory, MemberData(nameof(Files))]
    public void Only_the_version_changes(string text, bool bom)
    {
        var path = WriteProject("App", text, new UTF8Encoding(bom));
        var file = ProjectFile.Load(path);
        ProjectFile.SetVersion(file.PackageReferences().Single().Element, "2.0.0");
        file.Save();
        Assert.Equal(new UTF8Encoding(bom).GetPreamble().Concat(Encoding.UTF8.GetBytes(text.Replace("Version=\"1.0.0\"", "Version=\"2.0.0\""))), File.ReadAllBytes(path));
    }

    // Restore's downgrade (NU1605): the version raised wherever the project has it, the id whatever its case.
    [Theory]
    [InlineData("""<PackageReference Include="Newtonsoft.Json" Version="9.0.1" />""")]
    [InlineData("""<PackageReference Version="9.0.1" Include="Newtonsoft.Json" />""")]
    [InlineData("""<PackageReference Include="newtonsoft.json" PrivateAssets="all" Version="9.0.1" />""")]
    [InlineData("""<PackageReference Include="Newtonsoft.Json"><Version>9.0.1</Version></PackageReference>""")]
    [InlineData("<PackageReference Include=\"Newtonsoft.Json\">\n      <PrivateAssets>all</PrivateAssets>\n      <Version> 9.0.1 </Version>\n    </PackageReference>")]
    public void A_version_is_raised_whatever_the_attributes_order_or_as_an_element(string reference)
    {
        var path = WriteProject("App", $"<Project Sdk=\"Microsoft.NET.Sdk\">\n  <ItemGroup>\n    {reference}\n    <PackageReference Include=\"Other\" Version=\"9.0.1\" />\n  </ItemGroup>\n</Project>\n", new UTF8Encoding(false));
        var project = new ConvertedProject("App", path, path, false, []);
        var fixer = new BuildFixer(new Report(), [project], root, RewriteHarness.Rules);

        Assert.True(fixer.RaiseVersion(project, "Newtonsoft.Json", "9.0.1", "13.0.4"));
        Assert.False(fixer.RaiseVersion(project, "Newtonsoft.Json", "9.0.1", "13.0.4"));   // already raised

        var references = XDocument.Load(path).Descendants("PackageReference").ToList();
        Assert.Equal("13.0.4", ProjectFile.VersionOf(references[0]));
        Assert.Equal("9.0.1", ProjectFile.VersionOf(references[1]));   // another package: as it was
        // In the attribute or the element it was in, not both.
        Assert.True(references[0].Attribute("Version") == null || references[0].Element("Version") == null);
    }

    [Fact] // mojoPortal's Microsoft.IdentityModel.Tokens 8.19.1 with JsonWebTokens 8.16.0: MissingMethodException
    public void A_familys_packages_are_raised_to_the_highest_of_them()
    {
        var path = WriteProject("App", """
            <Project Sdk="Microsoft.NET.Sdk">
              <ItemGroup>
                <PackageReference Version="8.16.0" Include="Microsoft.IdentityModel.JsonWebTokens" />
                <PackageReference Include="Microsoft.IdentityModel.Tokens" Version="8.19.1" />
                <PackageReference Include="System.IdentityModel.Tokens.Jwt"><Version>7.1.2</Version></PackageReference>
              </ItemGroup>
            </Project>
            """, new UTF8Encoding(false));
        var project = new ConvertedProject("App", path, path, false, []);
        var fixer = new BuildFixer(new Report(), [project], root, RewriteHarness.Rules);

        Assert.True(fixer.AlignFamilies());
        Assert.All(XDocument.Load(path).Descendants("PackageReference"), r => Assert.Equal("8.19.1", ProjectFile.VersionOf(r)));
        Assert.False(fixer.AlignFamilies());
    }
}
