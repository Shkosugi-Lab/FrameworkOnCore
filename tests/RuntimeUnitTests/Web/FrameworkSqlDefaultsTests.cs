using System.Data.Common;
using System.Web.DataAccess;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// The connection strings of the runtime's SQL Server components (FrameworkSqlDefaults): .NET Framework's
/// System.Data.SqlClient's Encrypt (false unless set: SqlConnectionStringBuilder.Encrypt's documentation) said explicitly.
/// </summary>
public class FrameworkSqlDefaultsTests
{
    [Fact] // .NET Framework's SqlClient: Encrypt false unless the connection string says (the class's comment: said explicitly)
    public void A_connection_string_without_encrypt_is_not_encrypted()
    {
        Assert.Equal("Server=.;Database=x;Encrypt=False", FrameworkSqlDefaults.Apply("Server=.;Database=x"));
        Assert.Equal("Server=.;Database=x;Encrypt=False", FrameworkSqlDefaults.Apply("Server=.;Database=x;  "));
        var keys = new DbConnectionStringBuilder { ConnectionString = FrameworkSqlDefaults.Apply("Server=.") };
        Assert.Equal("False", keys["Encrypt"]);
    }

    [Fact] // the class's comment: a connection string that says (any case of the key) is left as it is; none, and one that is not one, too
    public void A_connection_string_that_says_is_left()
    {
        Assert.Equal("Server=.;encrypt=yes", FrameworkSqlDefaults.Apply("Server=.;encrypt=yes"));
        Assert.Equal("Server=.;Encrypt=True;", FrameworkSqlDefaults.Apply("Server=.;Encrypt=True;"));
        Assert.Null(FrameworkSqlDefaults.Apply(null));
        Assert.Equal("  ", FrameworkSqlDefaults.Apply("  "));
        Assert.Equal("Server=.;Database", FrameworkSqlDefaults.Apply("Server=.;Database"));
    }
}
