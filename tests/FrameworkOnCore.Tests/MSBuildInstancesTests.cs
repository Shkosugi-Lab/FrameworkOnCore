using FrameworkOnCore.Converter;
using static FrameworkOnCore.Converter.MSBuildInstances;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The original build's MSBuild (--msbuild): the user's choice when both Visual Studio and its Build Tools are installed,
/// nothing guessed; one kind installed, its newest.
/// </summary>
public sealed class MSBuildInstancesTests
{
    static Instance Of(string kind, string version, string folder) => new(kind, kind == VisualStudio ? "Visual Studio Community" : "Visual Studio Build Tools", version, folder, Path.Combine(folder, "MSBuild", "Current", "Bin", "MSBuild.exe"));

    static readonly Instance Tools2026 = Of(BuildTools, "18.10.1", @"C:\VS\18\BuildTools");
    static readonly Instance Tools2022 = Of(BuildTools, "17.14.6", @"C:\VS\2022\BuildTools");
    static readonly Instance Community = Of(VisualStudio, "17.14.6", @"C:\VS\2022\Community");

    [Fact]
    public void One_kind_without_a_choice_its_newest()
    {
        Assert.Equal(Tools2026, Select(null, [Tools2026, Tools2022], out var problem));
        Assert.Null(problem);
    }

    [Fact]
    public void Both_kinds_without_a_choice_none_and_how_to_choose()
    {
        Assert.Null(Select(null, [Tools2026, Community], out var problem));
        Assert.Contains("--msbuild visualstudio", problem);
        Assert.Contains(@"C:\VS\2022\Community", problem);
    }

    [Fact]
    public void A_kind_its_newest()
    {
        Assert.Equal(Community, Select("visualstudio", [Tools2026, Community, Tools2022], out _));
        Assert.Equal(Tools2026, Select("BuildTools", [Tools2026, Community, Tools2022], out _));
    }

    [Fact]
    public void A_folder_or_its_msbuild_that_installation()
    {
        Assert.Equal(Tools2022, Select(@"C:\VS\2022\BuildTools\", [Tools2026, Community, Tools2022], out _));
        Assert.Equal(Tools2022, Select(Tools2022.MSBuild, [Tools2026, Community, Tools2022], out _));
    }

    [Fact]
    public void A_kind_or_folder_not_installed_none_and_why()
    {
        Assert.Null(Select("visualstudio", [Tools2026], out var kind));
        Assert.Contains("none is installed", kind);
        Assert.Null(Select(@"C:\Elsewhere", [Tools2026], out var folder));
        Assert.Contains("not an installation", folder);
    }

    [Fact]
    public void None_installed_none_and_what_is_needed()
    {
        Assert.Null(Select(null, [], out var problem));
        Assert.Contains("neither Visual Studio nor Visual Studio Build Tools", problem);
    }
}
