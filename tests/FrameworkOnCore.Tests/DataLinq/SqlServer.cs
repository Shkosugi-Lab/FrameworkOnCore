// System.Data.SqlClient on purpose (obsolete in favor of Microsoft.Data.SqlClient): the provider the port
// and the converted applications use.
#pragma warning disable CS0618
using System.Data.SqlClient;

namespace FrameworkOnCore.Tests.DataLinq;

/// <summary>
/// The SQL Server the LINQ to SQL tests run against: FOC_TEST_SQLSERVER (a connection string) or the
/// local SQL Server Express. Tests needing it return early when it is not reachable (Linux test runs
/// have none by default; the goldens and CRUD then stay Windows-verified).
/// </summary>
public static class SqlServer
{
    public static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("FOC_TEST_SQLSERVER")
        ?? @"Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True";

    static readonly Lazy<bool> available = new(() =>
    {
        try
        {
            using var connection = new SqlConnection(new SqlConnectionStringBuilder(ConnectionString) { ConnectTimeout = 3 }.ConnectionString);
            connection.Open();
            return true;
        }
        catch (Exception e) when (e is System.Data.Common.DbException or InvalidOperationException or PlatformNotSupportedException)
        {
            return false;
        }
    });

    public static bool Available => available.Value;

    /// <summary>A connection string to the given (test) database.</summary>
    public static string For(string database) => new SqlConnectionStringBuilder(ConnectionString) { InitialCatalog = database }.ConnectionString;
}
