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

        // The cases whose outcome differs from .NET Framework's: the port's choices (Port_choice_* below).
        static readonly HashSet<string> s_apart = new HashSet<string>
        {
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
