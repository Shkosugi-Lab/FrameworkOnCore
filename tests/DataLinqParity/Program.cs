using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DataLinqParity
{
    /// <summary>
    /// Runs every case and writes what it observed: under net48 against .NET Framework's System.Data.Linq, the goldens
    /// (record.ps1); under net10.0 against the port, the same file to compare by hand (the tests compare case by case:
    /// tests/FrameworkOnCore.Tests/DataLinq/DataLinqParityTests.cs).
    ///
    ///   DataLinqParity [--connection "server connection string"] [--out folder]
    ///
    /// Writes cases.json (case -> lines) and api.txt (the assembly's API as documentation ids). Without a connection
    /// the database cases are left out.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            string connection = null, output = ".";
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "--connection") connection = args[++i];
                else if (args[i] == "--out") output = args[++i];
            }
            Directory.CreateDirectory(output);
            var assembly = typeof(System.Data.Linq.DataContext).Assembly;
            Console.WriteLine("System.Data.Linq: " + assembly.FullName + " (" + assembly.Location + ")");

            File.WriteAllLines(Path.Combine(output, "api.txt"), DocId.Api(assembly), new UTF8Encoding(false));

            if (connection != null) Fixture.Create(connection);
            var results = Goldens.RunAll(DataLinqCases.Runner, connection != null, Console.Out);
            Goldens.Write(Path.Combine(output, "cases.json"), results);
            Console.WriteLine("-> " + Path.GetFullPath(output));
            return 0;
        }
    }
}
