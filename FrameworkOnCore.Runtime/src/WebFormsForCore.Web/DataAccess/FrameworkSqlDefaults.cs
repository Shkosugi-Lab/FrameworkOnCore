namespace System.Web.DataAccess {
    using System.Data.Common;

    // The runtime's SQL Server components (membership, roles, profile, session state, SqlDataSource,
    // cache dependencies) use FrameworkOnCore's System.Data.SqlClient, as .NET Framework's did (and the
    // applications do): its Encrypt is false unless set. A connection string that does not say is given
    // that explicitly (Microsoft.Data.SqlClient, which they used before, encrypts by default: the
    // connection failed against a server with its own certificate, SQL Server Express's).
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
