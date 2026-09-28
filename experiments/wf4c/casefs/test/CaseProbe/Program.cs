// What libfoccase.so gives .NET's file APIs on Linux: names found without regard to case, as on Windows. Run under
// /probe (FOC_CASE_ROOTS=/probe/root); /probe/outside is outside the roots. "expect-windows": every check as on
// Windows; "expect-linux": without the library, the lookups by another case fail (the checks that show it).
using System.Reflection;

if (args.Length > 0 && args[0] == "bench")
{
    // The cost: File.Exists 100,000 times for an exact name, a name under another case, a name not there.
    Directory.CreateDirectory("/probe/root/Site/Content");
    File.WriteAllText("/probe/root/Site/Content/Site.css", "");
    foreach (var (label, path) in new[] { ("exact", "/probe/root/Site/Content/Site.css"), ("other case", "/probe/root/site/content/site.css"), ("not there", "/probe/root/Site/Content/None.css") })
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (var i = 0; i < 100_000; i++) File.Exists(path);
        Console.WriteLine($"{label,-12} {watch.Elapsed.TotalMilliseconds * 1000 / 100_000:F2} us per call");
    }
    return 0;
}
var expectWindows = args.Length > 0 && args[0] == "expect-windows";
var root = "/probe/root";
var outside = "/probe/outside";
var failed = 0;

void Check(string name, bool windows, bool linux)
{
    var expected = expectWindows ? windows : linux;
    Console.WriteLine($"{(expected ? "PASS" : "FAIL")}  {name}");
    if (!expected) failed++;
}

bool Try(Action action)
{
    try { action(); return true; }
    catch (Exception e) when (e is IOException or UnauthorizedAccessException) { return false; }
}

string[] Names(string folder) => Directory.GetFileSystemEntries(folder).Select(Path.GetFileName).OrderBy(n => n, StringComparer.Ordinal).ToArray()!;

// The site as deployed: N2/Web.config, bin/App.Plugin.dll, Content/Site.css.
foreach (var d in new[] { root, outside }) { if (Directory.Exists(d)) Directory.Delete(d, true); }
Directory.CreateDirectory(Path.Combine(root, "N2"));
File.WriteAllText(Path.Combine(root, "N2", "Web.config"), "<configuration />");
Directory.CreateDirectory(Path.Combine(root, "Content"));
File.WriteAllText(Path.Combine(root, "Content", "Site.css"), "body{}");
Directory.CreateDirectory(Path.Combine(outside, "N2"));
File.WriteAllText(Path.Combine(outside, "N2", "Web.config"), "x");

// Lookups (ASP.NET's lower-case configuration path, a reference in the markup with another case).
var exists = File.Exists($"{root}/n2/web.config");
Check("File.Exists by another case", windows: exists, linux: !exists);
Check("Directory.Exists by another case", windows: Directory.Exists($"{root}/n2"), linux: !Directory.Exists($"{root}/n2"));
string? text = null;
var read = Try(() => text = File.ReadAllText($"{root}/CONTENT/site.CSS"));
Check("File.ReadAllText by another case", windows: read && text == "body{}", linux: !read);
var listed = Try(() => Directory.GetFiles($"{root}/n2"));
Check("Directory.GetFiles by another case", windows: listed, linux: !listed);
var info = new FileInfo($"{root}/content/site.css");
Check("FileInfo.Length by another case", windows: info.Exists && info.Length == 6, linux: !info.Exists);

// Creation: a name that exists under another case is that file; new names go into the folders as they are.
var wrote = Try(() => File.WriteAllText($"{root}/n2/WEB.CONFIG", "<configuration><!-- 2 --></configuration>"));
Check("write to a name under another case: the same file",
    windows: wrote && Names($"{root}/N2").SequenceEqual(new[] { "Web.config" }) && File.ReadAllText($"{root}/N2/Web.config").Contains("2"),
    linux: !wrote);
var created = Try(() => File.WriteAllText($"{root}/n2/New.txt", "new"));
Check("new file in a folder named with another case: in that folder",
    windows: created && Names(root).SequenceEqual(new[] { "Content", "N2" }) && File.Exists($"{root}/N2/New.txt"),
    linux: !created);
var made = Try(() => Directory.CreateDirectory($"{root}/n2/Sub/Deep"));
Check("CreateDirectory under a folder named with another case",
    windows: made && Directory.Exists($"{root}/N2/Sub/Deep") && Names(root).SequenceEqual(new[] { "Content", "N2" }),
    linux: made && Names(root).SequenceEqual(new[] { "Content", "N2", "n2" }));   // Linux: a second folder n2

// Moves, renames, deletion.
var moved = Try(() => File.Move($"{root}/N2/new.txt", $"{root}/n2/Moved.txt"));
Check("File.Move by other cases", windows: moved && File.Exists($"{root}/N2/Moved.txt"), linux: !moved);
var renamed = Try(() => File.Move($"{root}/N2/Moved.txt", $"{root}/N2/moved.TXT"));
Check("rename changing only the case", windows: renamed && Names($"{root}/N2").Contains("moved.TXT") && !Names($"{root}/N2").Contains("Moved.txt"),
    linux: !renamed);
var deleted = Try(() => File.Delete($"{root}/n2/MOVED.txt"));
Check("File.Delete by another case", windows: deleted && !Names($"{root}/N2").Any(n => n.Equals("moved.txt", StringComparison.OrdinalIgnoreCase)),
    linux: deleted);   // Linux: File.Delete of a missing file does nothing

// Relative paths, the working folder.
Directory.SetCurrentDirectory(root);
Check("relative path by another case", windows: File.Exists("n2/Web.CONFIG"), linux: !File.Exists("n2/Web.CONFIG"));

// Change notification (web.config, bin: the application's restart).
using (var watcher = new FileSystemWatcher())
{
    var changed = new ManualResetEventSlim();
    var watching = Try(() =>
    {
        watcher.Path = $"{root}/n2";
        watcher.Changed += (_, _) => changed.Set();
        watcher.EnableRaisingEvents = true;
    });
    if (watching) File.AppendAllText($"{root}/N2/Web.config", " ");
    var notified = watching && changed.Wait(3000);
    // Linux: the folder n2 made by CreateDirectory above is another folder; no change is seen there.
    Check("FileSystemWatcher on a folder named with another case", windows: notified, linux: !notified);
}

// An assembly by its path under another case (a plugin loaded from bin).
var copied = typeof(Program).Assembly.Location;
Directory.CreateDirectory($"{root}/bin");
File.Copy(Path.Combine(Path.GetDirectoryName(copied)!, "CaseProbe.dll"), $"{root}/bin/App.Plugin.dll", true);
var loaded = Try(() => { try { Assembly.LoadFile($"{root}/BIN/app.plugin.DLL"); } catch (FileNotFoundException) { throw new IOException(); } catch (BadImageFormatException) { } });
Check("Assembly.LoadFile by another case", windows: loaded, linux: !loaded);

// Outside the roots: as Linux has it.
Check("outside the roots: not looked up", windows: !File.Exists($"{outside}/n2/web.config"), linux: !File.Exists($"{outside}/n2/web.config"));

// The exact names: unchanged.
Check("exact names", windows: File.Exists($"{root}/N2/Web.config"), linux: File.Exists($"{root}/N2/Web.config"));

Console.WriteLine(failed == 0 ? "RESULT: all passed" : $"RESULT: {failed} failed");
return failed == 0 ? 0 : 1;
