using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The native libraries Linux has for x64 only in a site (Studio's container started as x64 by default for them):
/// System.Data.SQLite 1.0.119's SQLite.Interop.dll (runtimes linux-x64, osx-x64, win-x64, win-x86) is; a library with an
/// arm64 build (SQLitePCLRaw's e_sqlite3) is not.
/// </summary>
public sealed class StudioNativeLibrariesTests : IDisposable
{
    readonly string bin = Path.Combine(Path.GetTempPath(), "foc-natives-" + Guid.NewGuid().ToString("N"));

    public StudioNativeLibrariesTests() => Directory.CreateDirectory(bin);

    public void Dispose()
    {
        if (Directory.Exists(bin)) Directory.Delete(bin, true);
    }

    [Fact]
    public void From_the_deps_json()
    {
        File.WriteAllText(Path.Combine(bin, "App.deps.json"), """
            { "targets": { ".NETCoreApp,Version=v10.0": {
              "Stub.System.Data.SQLite.Core.NetStandard/1.0.119": { "runtimeTargets": {
                "runtimes/linux-x64/native/SQLite.Interop.dll": { "rid": "linux-x64", "assetType": "native" },
                "runtimes/osx-x64/native/SQLite.Interop.dll": { "rid": "osx-x64", "assetType": "native" },
                "runtimes/win-x64/native/SQLite.Interop.dll": { "rid": "win-x64", "assetType": "native" } } },
              "SQLitePCLRaw.lib.e_sqlite3/2.1.10": { "runtimeTargets": {
                "runtimes/linux-x64/native/libe_sqlite3.so": { "rid": "linux-x64", "assetType": "native" },
                "runtimes/linux-arm64/native/libe_sqlite3.so": { "rid": "linux-arm64", "assetType": "native" } } },
              "Some.Runtime/1.0.0": { "runtimeTargets": {
                "runtimes/linux-x64/lib/net10.0/Some.dll": { "rid": "linux-x64", "assetType": "runtime" } } } } } }
            """);
        Assert.Equal(["SQLite.Interop.dll"], NativeLibraries.X64Only(bin));
    }

    [Fact] // without a deps.json: the folders
    public void From_the_runtimes_folders()
    {
        Directory.CreateDirectory(Path.Combine(bin, "runtimes", "linux-x64", "native"));
        File.WriteAllText(Path.Combine(bin, "runtimes", "linux-x64", "native", "libfoo.so"), "");
        Directory.CreateDirectory(Path.Combine(bin, "runtimes", "linux-arm64", "native"));
        File.WriteAllText(Path.Combine(bin, "runtimes", "linux-arm64", "native", "libfoo.so"), "");
        File.WriteAllText(Path.Combine(bin, "runtimes", "linux-x64", "native", "libbar.so"), "");
        Assert.Equal(["libbar.so"], NativeLibraries.X64Only(bin));
    }

    [Fact]
    public void None_without_native_libraries() => Assert.Empty(NativeLibraries.X64Only(bin));
}
