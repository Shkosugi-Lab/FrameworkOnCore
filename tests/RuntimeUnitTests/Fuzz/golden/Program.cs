// Records .NET Framework 4.8's results for the differential fuzzing's inputs (FuzzInput.cs, FuzzTargets.cs: the tests'
// own files): one file a target beside this one, <target>.txt.gz, two lines a case (its input's fields, then its result's
// parts, tab-separated, as Show writes them). FuzzTests makes the same inputs, gives them to the port and compares.
//
// Re-record (Windows, .NET Framework 4.8), after changing an input or a target:
//   dotnet build tests/RuntimeUnitTests/Fuzz/golden/Recorder.csproj -c Release
//   tests/RuntimeUnitTests/Fuzz/golden/bin/Release/net48/Recorder.exe tests/RuntimeUnitTests/Fuzz/golden
//   then delete the folder's bin and obj.
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using FrameworkOnCore.Runtime.Tests.Fuzz;

static class Program
{
    static int Main(string[] args)
    {
        var folder = args.Length > 0 ? args[0] : ".";
        // The invariant culture, as FuzzTests runs the targets.
        Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        foreach (var target in Targets.All)
        {
            var watch = Stopwatch.StartNew();
            var file = Path.Combine(folder, target.Name + ".txt.gz");
            using (var stream = new GZipStream(File.Create(file), CompressionLevel.Optimal))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)) { NewLine = "\n" })
            {
                foreach (var input in target.Inputs())
                {
                    writer.WriteLine(Target.Line(input));
                    writer.WriteLine(Target.Line(target.Execute(input)));
                }
            }
            Console.WriteLine(target.Name + ": " + target.Count + " cases, " + new FileInfo(file).Length + " bytes, " + watch.ElapsedMilliseconds + " ms");
        }
        return 0;
    }
}
