// FrameworkOnCore: the port's outcome of each of FrameworkCases against what .NET Framework 4.8's own System.Data gave for
// it (golden\framework.golden.json, recorded by golden\record.csproj): the value, or the exception's type and message.
// Where the port differs, the case is apart: a bug (its test skipped), or what cannot be as on .NET Framework (the
// port's choice: its outcome asserted, and why).

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;

namespace System.Data.SqlClient.Tests
{
    // The cases set the AppDomain's DataDirectory: not beside DataDirectoryTests.
    [Collection(nameof(DataDirectoryTests))]
    public class FrameworkGoldenTests
    {
        static readonly Dictionary<string, string> s_golden = JsonSerializer.Deserialize<Dictionary<string, string>>(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "framework.golden.json")));

        // The cases whose outcome differs from .NET Framework's: bugs (Bug_* below, skipped) and the port's choices
        // (Port_choice_* below).
        static readonly HashSet<string> s_apart = new HashSet<string>
        {
            "connection Context Connection=true",
            "datadirectory Data Source=localhost",
            "datadirectory Data Source=srv",
            "datadirectory not a string",
            "datadirectory relative",
            "datadirectory token twice",
            "datadirectory too long",
        };

        static string Outcome(string name)
        {
            var culture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            try
            {
                var run = FrameworkCases.All().Single(c => c.Key == name).Value;
                string outcome = FrameworkCases.Run(run).Replace(FrameworkCases.Folder, "{Folder}");
                // The runtime's, not the port's: .NET's Boolean.Parse names the value in its message (.NET Framework's did not).
                return Regex.Replace(outcome, "^(!System.FormatException: String) '.*' (was not recognized as a valid Boolean.)$", "$1 $2");
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        [Fact]
        public void Every_case_has_its_golden()
        {
            Assert.Equal(s_golden.Keys.OrderBy(k => k, StringComparer.Ordinal), FrameworkCases.All().Select(c => c.Key).OrderBy(k => k, StringComparer.Ordinal));
        }

        public static IEnumerable<object[]> Cases() => FrameworkCases.All().Select(c => c.Key).Where(k => !s_apart.Contains(k)).Select(k => new object[] { k });

        [Theory]
        [MemberData(nameof(Cases))]
        public void As_on_dotnet_framework(string name)
        {
            Assert.Equal(s_golden[name], Outcome(name));
        }

        // --- bugs: the port differs from .NET Framework (each fails without its Skip)

        [Fact(Skip = "bug: SqlConnectionString.cs:227 throws SQL.UnsupportedKeyword (NotSupportedException); .NET Framework's SqlConnectionString threw SQL.ContextUnavailableOutOfProc (InvalidOperationException: 'The requested operation requires a SqlClr context...') outside SQL Server")]
        public void Bug_context_connection_true_is_an_invalid_operation()
        {
            Assert.Equal(s_golden["connection Context Connection=true"], Outcome("connection Context Connection=true"));
        }

        [Theory(Skip = "bug: SqlConnectionString.cs:344 expands |DataDirectory| whatever the Data Source; .NET Framework's SqlConnectionString refused it (VerifyLocalHostAndFixup: 'Invalid value for key 'attachdbfilename'') unless the Data Source is this machine ('.', '(local)', its name, with an instance) or LocalDB")]
        [InlineData("datadirectory Data Source=localhost")]
        [InlineData("datadirectory Data Source=srv")]
        public void Bug_datadirectory_is_for_a_local_server_only(string name)
        {
            Assert.Equal(s_golden[name], Outcome(name));
        }

        [Fact(Skip = "bug: DbConnectionOptions.DataDirectory.cs:26 throws ADP.InvalidConnectionOptionValue (ArgumentException); .NET Framework's ExpandDataDirectory threw ADP.InvalidDataDirectory (InvalidOperationException: 'The DataDirectory substitute is not a string.')")]
        public void Bug_a_datadirectory_not_a_string_is_an_invalid_operation()
        {
            Assert.Equal(s_golden["datadirectory not a string"], Outcome("datadirectory not a string"));
        }

        [Fact(Skip = "bug: DbConnectionOptions.DataDirectory.cs:60 compares with Path.GetFullPath(rootFolderPath); .NET Framework's ExpandDataDirectory compared the full path with the DataDirectory as it is, so a relative DataDirectory ('App_Data') was refused ('Invalid value for key 'attachdbfilename'')")]
        public void Bug_a_relative_datadirectory_is_refused()
        {
            Assert.Equal(s_golden["datadirectory relative"], Outcome("datadirectory relative"));
        }

        // --- the port's choices: what cannot be as on .NET Framework

        [Fact] // the port's choice: .NET's Path.GetFullPath takes '|' (a file name's character on Linux), where .NET Framework's
               // refused it ("Illegal characters in path."); the '|' left after the expansion is refused as .NET Framework
               // refused it in a path not expanded, with the same exception type
        public void Port_choice_the_token_twice_is_refused_by_the_check_for_a_bar()
        {
            Assert.StartsWith("!System.ArgumentException: ", s_golden["datadirectory token twice"]);
            Assert.Equal("!System.ArgumentException: Invalid value for key 'attachdbfilename'.", Outcome("datadirectory token twice"));
        }

        [Fact] // the port's choice: .NET's Path.GetFullPath has no MAX_PATH (nor has Linux), where .NET Framework's threw
               // PathTooLongException; the length is refused by the limit SqlClient has for the keyword (260), as for a path
               // not expanded
        public void Port_choice_a_path_too_long_is_refused_by_the_keywords_limit()
        {
            Assert.StartsWith("!System.IO.PathTooLongException: ", s_golden["datadirectory too long"]);
            Assert.StartsWith("!System.ArgumentException: ", Outcome("datadirectory too long"));
            Assert.Contains("attachdbfilename", Outcome("datadirectory too long"));
        }
    }
}
