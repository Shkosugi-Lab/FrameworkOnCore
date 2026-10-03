// WebFormsForCore: System.Data.Sql.SqlDataSourceEnumerator, which .NET Framework had and .NET's System.Data.SqlClient left
// out: the SQL Server instances of the network, as .NET Framework's lists them (referencesource, System.Data's
// SqlDataSourceEnumerator: its table, columns and rows). .NET Framework asked its native SNI (SNIServerEnumOpen), which
// .NET's has not: the SQL Server Browser services are asked here as that SNI asks them, by the broadcast of SSRP's
// CLNT_BCAST_EX (UDP 1434), and their answers read until none comes for a while.

using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Sql
{
    public sealed class SqlDataSourceEnumerator : DbDataSourceEnumerator
    {
        internal const string ServerName = "ServerName";
        internal const string InstanceName = "InstanceName";
        internal const string IsClustered = "IsClustered";
        internal const string Version = "Version";

        private const int SqlServerBrowserPort = 1434;
        private const byte ClntBcastEx = 0x02;
        private const byte SvrResp = 0x05;
        // .NET Framework's limit for the whole enumeration (ADP.DefaultCommandTimeout), and the quiet after which no more
        // answers are waited for.
        private const int TimeoutSeconds = 30;
        private const int QuietMilliseconds = 2000;

        private static readonly SqlDataSourceEnumerator s_instance = new SqlDataSourceEnumerator();

        private SqlDataSourceEnumerator()
        {
        }

        public static SqlDataSourceEnumerator Instance => s_instance;

        public override DataTable GetDataSources()
        {
            var table = new DataTable("SqlDataSources") { Locale = CultureInfo.InvariantCulture };
            table.Columns.Add(ServerName, typeof(string));
            table.Columns.Add(InstanceName, typeof(string));
            table.Columns.Add(IsClustered, typeof(string));
            table.Columns.Add(Version, typeof(string));
            foreach (var (server, instance, clustered, version) in Distinct(Ask()))
            {
                table.Rows.Add(server, instance, clustered, version);
            }
            foreach (DataColumn column in table.Columns)
            {
                column.ReadOnly = true;
            }
            return table;
        }

        // The answers' instances: one row per server and instance, as .NET Framework's table has them.
        private static IEnumerable<(string, string, string, string)> Distinct(IEnumerable<(string, string, string, string)> instances)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var instance in instances)
            {
                if (seen.Add(instance.Item1 + "\\" + instance.Item2))
                {
                    yield return instance;
                }
            }
        }

        private static List<(string, string, string, string)> Ask()
        {
            var instances = new List<(string, string, string, string)>();
            using var client = new UdpClient(AddressFamily.InterNetwork) { EnableBroadcast = true };
            client.Send(new[] { ClntBcastEx }, 1, new IPEndPoint(IPAddress.Broadcast, SqlServerBrowserPort));
            var end = DateTime.UtcNow.AddSeconds(TimeoutSeconds);
            while (DateTime.UtcNow < end)
            {
                Task<UdpReceiveResult> receive = client.ReceiveAsync();
                if (!receive.Wait(QuietMilliseconds))
                {
                    break;
                }
                instances.AddRange(Parse(receive.Result.Buffer));
            }
            return instances;
        }

        /// <summary>
        /// An SVR_RESP's instances: "ServerName;S;InstanceName;I;IsClustered;No;Version;15.0.2000.5;tcp;1433;;" each. The
        /// default instance (MSSQLSERVER) has no instance name in .NET Framework's table.
        /// </summary>
        internal static IEnumerable<(string, string, string, string)> Parse(byte[] response)
        {
            if (response == null || response.Length < 3 || response[0] != SvrResp)
            {
                yield break;
            }
            int length = Math.Min(response[1] | (response[2] << 8), response.Length - 3);
            string text = Encoding.ASCII.GetString(response, 3, length);
            foreach (string entry in text.Split(new[] { ";;" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string[] parts = entry.Split(';');
                for (int i = 0; i + 1 < parts.Length; i += 2)
                {
                    values[parts[i]] = parts[i + 1];
                }
                if (!values.TryGetValue(ServerName, out string server) || string.IsNullOrEmpty(server))
                {
                    continue;
                }
                values.TryGetValue(InstanceName, out string instance);
                if (string.Equals(instance, "MSSQLSERVER", StringComparison.OrdinalIgnoreCase))
                {
                    instance = null;
                }
                values.TryGetValue(IsClustered, out string clustered);
                values.TryGetValue(Version, out string version);
                yield return (server, string.IsNullOrEmpty(instance) ? null : instance, clustered, version);
            }
        }
    }
}
