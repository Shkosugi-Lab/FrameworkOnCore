using System.Diagnostics;
using System.IO.Compression;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// libfoccase.so comes from the build GitHub Actions published of its sources' key (a zip) unless casefs/out has the build
/// of that key; a build of other sources is taken when there is no other. The key is casefs/key.ps1's.
/// </summary>
public sealed class CaseLibrariesTests : IDisposable
{
    readonly string directory = Directory.CreateTempSubdirectory("foc-casefs-").FullName;
    string Runtime => Path.Combine(directory, "wf4c");
    string Output => Path.Combine(Runtime, "casefs", "out");

    public CaseLibrariesTests()
    {
        var casefs = Directory.CreateDirectory(Path.Combine(Runtime, "casefs")).FullName;
        foreach (var name in new[] { "Dockerfile.build", "build.sh", "casemap.h", "foccase.c" })
            File.WriteAllText(Path.Combine(casefs, name), $"// {name}\nline\n");
    }

    public void Dispose() => Directory.Delete(directory, recursive: true);

    string Zip(params string[] runtimes)
    {
        var zip = Path.Combine(directory, "libfoccase.zip");
        using var archive = ZipFile.Open(zip, ZipArchiveMode.Create);
        foreach (var runtime in runtimes)
            using (var writer = new StreamWriter(archive.CreateEntry($"{runtime}/libfoccase.so").Open())) writer.Write("released");
        return zip;
    }

    void Build(string key)
    {
        foreach (var runtime in RuntimeSetup.CaseRuntimes)
            File.WriteAllText(Path.Combine(Directory.CreateDirectory(Path.Combine(Output, runtime)).FullName, "libfoccase.so"), "local");
        File.WriteAllText(Path.Combine(Output, "key.txt"), key);
    }

    [Fact]
    public void Without_a_build_the_release_of_the_key_is_fetched()
    {
        Assert.True(RuntimeSetup.EnsureCaseLibraries(Runtime, source: Zip("linux-x64", "linux-arm64")));

        foreach (var runtime in RuntimeSetup.CaseRuntimes)
            Assert.Equal("released", File.ReadAllText(Path.Combine(Output, runtime, "libfoccase.so")));
        Assert.Equal(RuntimeSetup.CaseLibraryKey(Runtime), File.ReadAllText(Path.Combine(Output, "key.txt")));
        Assert.Equal(["key.txt", "linux-arm64", "linux-x64"], Directory.GetFileSystemEntries(Output).Select(Path.GetFileName).Order());
        Assert.Single(Directory.GetDirectories(Path.Combine(Runtime, "casefs")));  // no staging folder left
    }

    [Fact]
    public void A_build_of_the_key_is_left_as_it_is()
    {
        Build(RuntimeSetup.CaseLibraryKey(Runtime)!);

        Assert.True(RuntimeSetup.EnsureCaseLibraries(Runtime, source: Path.Combine(directory, "no-such.zip")));

        Assert.Equal("local", File.ReadAllText(Path.Combine(Output, "linux-x64", "libfoccase.so")));
    }

    [Fact]
    public void A_build_of_other_sources_is_replaced_by_the_release()
    {
        Build("000000000000");

        Assert.True(RuntimeSetup.EnsureCaseLibraries(Runtime, source: Zip("linux-x64", "linux-arm64")));

        Assert.Equal("released", File.ReadAllText(Path.Combine(Output, "linux-arm64", "libfoccase.so")));
    }

    [Fact]
    public void Not_fetched_a_build_of_other_sources_is_taken_and_none_is_none()
    {
        var messages = new List<string>();
        Assert.False(RuntimeSetup.EnsureCaseLibraries(Runtime, messages.Add, Path.Combine(directory, "no-such.zip")));

        Build("000000000000");
        Assert.True(RuntimeSetup.EnsureCaseLibraries(Runtime, messages.Add, Path.Combine(directory, "no-such.zip")));
        Assert.Contains(messages, m => m.Contains("may be of other sources"));
    }

    [Fact]
    public void A_release_without_a_processor_is_not_taken()
    {
        Assert.False(RuntimeSetup.EnsureCaseLibraries(Runtime, source: Zip("linux-x64")));

        Assert.False(File.Exists(Path.Combine(Output, "linux-x64", "libfoccase.so")));
    }

    [Fact]
    public void The_key_does_not_depend_on_line_endings()
    {
        var key = RuntimeSetup.CaseLibraryKey(Runtime);
        var source = Path.Combine(Runtime, "casefs", "foccase.c");
        File.WriteAllText(source, File.ReadAllText(source).Replace("\n", "\r\n"));
        Assert.Equal(key, RuntimeSetup.CaseLibraryKey(Runtime));

        File.AppendAllText(source, "more\n");
        Assert.NotEqual(key, RuntimeSetup.CaseLibraryKey(Runtime));
    }

    [Fact]
    public void The_key_is_key_ps1s()
    {
        if (!OperatingSystem.IsWindows()) return;  // Windows PowerShell
        var runtime = RuntimeSetup.Find()!;
        using var process = Process.Start(new ProcessStartInfo("powershell", $"-NoProfile -ExecutionPolicy Bypass -File \"{Path.Combine(runtime, "casefs", "key.ps1")}\"")
            { RedirectStandardOutput = true, UseShellExecute = false })!;
        var output = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();

        Assert.Equal(RuntimeSetup.CaseLibraryKey(runtime), output);
    }
}
