using System.Data.Common;
using System.Text.Json;

namespace FrameworkOnCore.Tests.DataLinq;

/// <summary>
/// The SQL the port generates, against golden\dlinq-sql.golden.json: what .NET Framework 4.8's own
/// System.Data.Linq generated for the same queries (golden\record.ps1). Needs a SQL Server: LINQ to SQL
/// (both of them) opens the connection to pick its SQL generation mode from the server's version, so the
/// texts assume SQL Server 2008 or newer on both sides.
/// </summary>
public class DataLinqSqlGoldenTests
{
    [Fact]
    public void Every_query_translates_as_NET_Framework_did()
    {
        if (!SqlServer.Available) return;

        var golden = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "DataLinq", "dlinq-sql.golden.json")));
        using var db = new StoreDb(SqlServer.ConnectionString);
        var cases = SqlCases.All(db);
        Assert.Equal(cases.Keys.OrderBy(k => k), golden.RootElement.EnumerateObject().Select(p => p.Name).OrderBy(k => k));

        foreach (var (name, query) in cases)
        {
            var expected = golden.RootElement.GetProperty(name);
            var command = db.GetCommand(query);
            // Line breaks follow Environment.NewLine (the original code's): \r\n in the golden (recorded on
            // Windows), \n on Linux. The same SQL either way.
            Assert.Equal(expected.GetProperty("sql").GetString()!.Replace("\r\n", "\n"), command.CommandText.Replace("\r\n", "\n"));
            Assert.Equal(
                expected.GetProperty("parameters").EnumerateArray().Select(p => (p.GetProperty("name").GetString()!, p.GetProperty("value").GetString())),
                command.Parameters.Cast<DbParameter>().Select(p => (p.ParameterName, (string?)Convert.ToString(p.Value, System.Globalization.CultureInfo.InvariantCulture))));
        }
    }
}
