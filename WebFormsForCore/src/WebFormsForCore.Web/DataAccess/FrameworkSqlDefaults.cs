namespace System.Web.DataAccess {
    using System.Data.Common;

    // The runtime's SQL Server components (membership, roles, profile, session state, SqlDataSource,
    // cache dependencies) use Microsoft.Data.SqlClient where .NET Framework used System.Data.SqlClient.
    // The applications' connection strings were written for the latter, whose Encrypt was false unless
    // set; Microsoft.Data.SqlClient's is true (4.0 and later), and the connection fails against a
    // server with its own certificate ("The certificate chain was issued by an authority that is not
    // trusted": SQL Server Express's). A connection string that does not say is given .NET Framework's.
    internal static class FrameworkSqlDefaults {
        internal static string Apply(string connectionString) {
#if NETFRAMEWORK
            return connectionString;
#else
            if (string.IsNullOrWhiteSpace(connectionString)) return connectionString;
            try {
                var keys = new DbConnectionStringBuilder { ConnectionString = connectionString };
                if (keys.ContainsKey("Encrypt")) return connectionString;
            }
            catch (ArgumentException) {
                return connectionString;
            }
            return connectionString.TrimEnd().TrimEnd(';') + ";Encrypt=False";
#endif
        }
    }
}
