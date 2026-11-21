using System.Text.Json;
using FrameworkOnCore.Analysis;
using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Studio's state as it starts again: the analyses are their folders' own entry.json (not a file of the name in an
/// original build's copy of the repository or in a conversion's output, nor a broken one); a container that was being
/// built or started when Studio stopped is failed, not "building" for good.
/// </summary>
public sealed class StudioStoreTests : IDisposable
{
    readonly string data = Directory.CreateTempSubdirectory("foc-studio-store").FullName;

    public void Dispose()
    {
        try { Directory.Delete(data, true); } catch (IOException) { }
    }

    void WriteEntry(string id, string state = "done")
    {
        var entry = new AnalysisEntry
        {
            Id = id, Name = "App", Project = @"C:\src\App\App.csproj", Root = @"C:\src\App", Configuration = "Release",
            Created = DateTimeOffset.UtcNow, State = state,
        };
        Directory.CreateDirectory(Path.Combine(data, id));
        File.WriteAllText(Path.Combine(data, id, "entry.json"), JsonSerializer.Serialize(entry, AnalysisResult.Json));
    }

    [Fact]
    public void The_analyses_are_their_folders_entries()
    {
        WriteEntry("20261121-000000-aaaaaaaa");
        WriteEntry("20261121-000001-bbbbbbbb", "running");
        // An application's own entry.json (its front end's), in the original build's copy and in the conversion's output.
        foreach (var folder in new[] { @"20261121-000000-aaaaaaaa\original\work-1\Web\Scripts", @"20261121-000000-aaaaaaaa\conversion\out\site" })
        {
            var path = Path.Combine(data, folder.Replace('\\', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, "entry.json"), """{ "main": "index.js" }""");
        }
        // A broken one (a disk that filled up while it was written).
        Directory.CreateDirectory(Path.Combine(data, "20261121-000002-cccccccc"));
        File.WriteAllText(Path.Combine(data, "20261121-000002-cccccccc", "entry.json"), "{ \"id\": ");

        var store = new AnalysisStore(data, data);
        Assert.Equal(["20261121-000000-aaaaaaaa", "20261121-000001-bbbbbbbb"], store.List().Select(e => e.Id).Order(StringComparer.Ordinal));
        Assert.Equal("failed", store.Get("20261121-000001-bbbbbbbb")!.State);   // running when Studio stopped
    }

    [Theory]
    [InlineData("building")]
    [InlineData("starting")]
    public void A_container_started_when_studio_stopped_is_failed(string state)
    {
        const string id = "20261121-000000-aaaaaaaa";
        WriteEntry(id);
        var folder = Path.Combine(data, id, "container");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "container.json"), JsonSerializer.Serialize(new ContainerEntry
        {
            State = state, Started = DateTimeOffset.UtcNow.AddHours(-3), Image = "foc-studio/app:aaaaaaaa", Name = "foc-studio-test-not-there",
        }, AnalysisResult.Json));
        var store = new AnalysisStore(data, data);
        var containers = new Containers(store, new Conversions(store, data));
        Assert.Equal("failed", containers.Get(id)!.State);
        Assert.Equal("failed", new Containers(store, new Conversions(store, data)).Get(id)!.State);   // saved so
    }
}
