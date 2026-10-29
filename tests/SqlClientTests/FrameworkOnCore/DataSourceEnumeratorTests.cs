// FrameworkOnCore: System.Data.Sql.SqlDataSourceEnumerator (.NET Framework's; .NET's System.Data.SqlClient left it out), as
// SqlClientFactory gives it: the SQL Server Browser services asked by broadcast (SSRP CLNT_BCAST_EX, UDP 1434). A browser of
// the test's own answers, where the port is free; where this machine's SQL Server Browser has it, that browser's answer to a
// request of the test's own (CLNT_UCAST_EX, [MC-SQLR] 2.2.3) is what the table must have of this machine.
//
// The answers' parsing and the one row per instance, apart (SqlDataSourceEnumerator.Parse and Distinct, by reflection: the
// network gives no packet the test chooses where a browser of this machine has the port), against .NET Framework's table
// (referencesource, System.Data's SqlDataSourceEnumerator.ParseServerEnumString), or the port's choice where .NET
// Framework's native SNI read the packets.

using System.Collections.Generic;
using System.Data.Sql;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.XUnitExtensions;
using Xunit;

namespace System.Data.SqlClient.Tests
{
    public class DataSourceEnumeratorTests
    {
        [Fact]
        public void The_factory_gives_the_enumerator()
        {
            Assert.True(SqlClientFactory.Instance.CanCreateDataSourceEnumerator);
            Assert.Same(SqlDataSourceEnumerator.Instance, SqlClientFactory.Instance.CreateDataSourceEnumerator());
        }

        static byte[] Packet(string text, byte type = 0x05, int? declared = null)
        {
            var body = Encoding.ASCII.GetBytes(text);
            int length = declared ?? body.Length;
            return new byte[] { type, (byte)length, (byte)(length >> 8) }.Concat(body).ToArray();
        }

        static string Row(DataRow r) => $"{r["ServerName"]}|{(r["InstanceName"] is DBNull ? "<null>" : r["InstanceName"])}|{r["IsClustered"]}|{r["Version"]}";

        static void AssertSchema(DataTable table)
        {
            Assert.Equal("SqlDataSources", table.TableName);
            Assert.Equal(new[] { "ServerName", "InstanceName", "IsClustered", "Version" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            Assert.All(table.Columns.Cast<DataColumn>(), c => Assert.Equal(typeof(string), c.DataType));
            Assert.All(table.Columns.Cast<DataColumn>(), c => Assert.True(c.ReadOnly));
        }

        [ConditionalFact]
        public void The_browsers_instances_are_the_table_dotnet_framework_gives()
        {
            UdpClient browser;
            try
            {
                browser = new UdpClient(new IPEndPoint(IPAddress.Any, 1434)) { EnableBroadcast = true };
            }
            catch (SocketException)
            {
                throw new SkipTestException("SQL Server Browser of this machine has the port (This_machines_browser_instances_are_in_the_table)");
            }
            using (browser)
            {
                // Two instances, the default one among them (no instance name in the table), and one of them twice; the
                // answers the port leaves out (not an SVR_RESP, too short, without a server name).
                var answer = Task.Run(async () =>
                {
                    var request = await browser.ReceiveAsync();
                    Assert.Equal(new byte[] { 0x02 }, request.Buffer);
                    foreach (var packet in new[]
                             {
                                 Packet("ServerName;DB1;InstanceName;MSSQLSERVER;IsClustered;No;Version;16.0.1000.6;tcp;1433;;ServerName;DB1;InstanceName;SQLEXPRESS;IsClustered;No;Version;16.0.1000.6;tcp;50000;;"),
                                 Packet("ServerName;DB1;InstanceName;SQLEXPRESS;IsClustered;No;Version;16.0.1000.6;tcp;50000;;"),
                                 Packet("ServerName;DB9;InstanceName;X;;", type: 0x04),
                                 new byte[] { 0x05, 0x00 },
                                 Packet("InstanceName;X;IsClustered;No;;"),
                             })
                    {
                        await browser.SendAsync(packet, packet.Length, request.RemoteEndPoint);
                    }
                });

                DataTable table = SqlDataSourceEnumerator.Instance.GetDataSources();
                answer.Wait(TimeSpan.FromSeconds(5));

                AssertSchema(table);
                var rows = table.Rows.Cast<DataRow>().Select(Row).ToList();
                Assert.Equal(new[] { "DB1|<null>|No|16.0.1000.6", "DB1|SQLEXPRESS|No|16.0.1000.6" }, rows);
            }
        }

        [ConditionalFact] // where this machine's SQL Server Browser has the port: its instances, as it answers the test itself
        public void This_machines_browser_instances_are_in_the_table()
        {
            try
            {
                new UdpClient(new IPEndPoint(IPAddress.Any, 1434)).Dispose();
                throw new SkipTestException("no SQL Server Browser on this machine (The_browsers_instances_are_the_table_dotnet_framework_gives)");
            }
            catch (SocketException)
            {
            }
            // CLNT_UCAST_EX to this machine's browser; its SVR_RESP: 0x05, the length (2 bytes), then
            // "ServerName;S;InstanceName;I;IsClustered;No;Version;V;tcp;1433;;" per instance ([MC-SQLR] 2.2.5).
            byte[] response;
            using (var client = new UdpClient(AddressFamily.InterNetwork))
            {
                client.Send(new byte[] { 0x03 }, 1, new IPEndPoint(IPAddress.Loopback, 1434));
                var receive = client.ReceiveAsync();
                if (!receive.Wait(TimeSpan.FromSeconds(5)))
                {
                    throw new SkipTestException("this machine's SQL Server Browser does not answer");
                }
                response = receive.Result.Buffer;
            }
            Assert.Equal(0x05, response[0]);
            var expected = new List<string>();
            foreach (var instance in Encoding.ASCII.GetString(response, 3, response.Length - 3).Split(new[] { ";;" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = instance.Split(';');
                var values = Enumerable.Range(0, parts.Length / 2).ToDictionary(i => parts[2 * i], i => parts[2 * i + 1], StringComparer.OrdinalIgnoreCase);
                // .NET Framework's table: no instance name for the default instance
                var name = values["InstanceName"].Equals("MSSQLSERVER", StringComparison.OrdinalIgnoreCase) ? "<null>" : values["InstanceName"];
                expected.Add($"{values["ServerName"]}|{name}|{values["IsClustered"]}|{values["Version"]}");
            }
            Assert.NotEmpty(expected);

            DataTable table = SqlClientFactory.Instance.CreateDataSourceEnumerator().GetDataSources();

            AssertSchema(table);
            var rows = table.Rows.Cast<DataRow>().Select(Row).ToList();
            Assert.All(expected, row => Assert.Contains(row, rows, StringComparer.OrdinalIgnoreCase));
            // one row per server and instance (the browser answers the broadcast on each of this machine's networks)
            Assert.Equal(rows.Count, rows.Distinct(StringComparer.OrdinalIgnoreCase).Count());
            Assert.Throws<ReadOnlyException>(() => table.Rows[0]["ServerName"] = "x");
        }

        // --- the answers as the table has them (SqlDataSourceEnumerator.Parse, then Distinct)

        static List<string> Rows(params byte[][] answers)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            MethodInfo parse = typeof(SqlDataSourceEnumerator).GetMethod("Parse", flags);
            MethodInfo distinct = typeof(SqlDataSourceEnumerator).GetMethod("Distinct", flags);
            var instances = answers.SelectMany(a => (IEnumerable<(string, string, string, string)>)parse.Invoke(null, new object[] { a })).ToList();
            return ((IEnumerable<(string Server, string Instance, string Clustered, string Version)>)distinct.Invoke(null, new object[] { instances }))
                .Select(i => $"{i.Server}|{i.Instance ?? "<null>"}|{i.Clustered ?? "<null>"}|{i.Version ?? "<null>"}").ToList();
        }

        [Fact] // .NET Framework's table: the default instance without a name; a server and instance once (DataTable.Select: case-insensitive)
        public void Instances_as_dotnet_framework_tabled_them()
        {
            Assert.Equal(new[] { "DB1|<null>|No|16.0.1000.6", "DB1|SQLEXPRESS|Yes|15.0.2000.5", "DB2|<null>|No|14.0.1000.169" }, Rows(
                Packet("ServerName;DB1;InstanceName;MSSQLSERVER;IsClustered;No;Version;16.0.1000.6;tcp;1433;;ServerName;DB1;InstanceName;SQLEXPRESS;IsClustered;Yes;Version;15.0.2000.5;np;\\\\DB1\\pipe\\MSSQL$SQLEXPRESS\\sql\\query;;"),
                Packet("ServerName;db1;InstanceName;sqlexpress;IsClustered;No;Version;15.0.2000.5;;"),
                Packet("ServerName;DB2;InstanceName;mssqlserver;IsClustered;No;Version;14.0.1000.169;tcp;1433;;"),
                Packet("ServerName;DB2;InstanceName;MSSQLSERVER;IsClustered;No;Version;14.0.1000.169;;")));
        }

        [Fact(Skip = "bug: SqlDataSourceEnumerator.cs:64 (Distinct) keys a row by server and instance; .NET Framework's ParseServerEnumString looked a default instance up by its server alone (\"ServerName='DB1'\", no InstanceName: SQL BU DT 20006584), so a default instance after a named one of the same server was left out")]
        public void A_default_instance_after_a_named_one_of_its_server_is_left_out_as_on_dotnet_framework()
        {
            Assert.Equal(new[] { "DB1|SQLEXPRESS|No|16.0.1000.6" }, Rows(
                Packet("ServerName;DB1;InstanceName;SQLEXPRESS;IsClustered;No;Version;16.0.1000.6;;ServerName;DB1;InstanceName;MSSQLSERVER;IsClustered;No;Version;16.0.1000.6;;")));
        }

        [Fact] // the port's choice (.NET Framework's native SNI read the packets; what it did with these is not documented):
               // what is not an instance's answer is left out, not an error, as other browsers' answers may still come
        public void Answers_that_are_not_an_instances_are_left_out()
        {
            Assert.Empty(Rows(
                null,
                new byte[0],
                new byte[] { 0x05, 0x00 },
                Packet("ServerName;DB9;InstanceName;X;;", type: 0x04), // not an SVR_RESP
                Packet(""),
                Packet("InstanceName;X;IsClustered;No;;"), // no server name
                Packet("ServerName;;InstanceName;X;;"), // ";;" ends the instance: none has a name
                Packet("ServerName;"))); // a key without its value
        }

        [Fact] // the port's choice: the length the answer gives bounds it (a longer one is read as far as it goes); a key
               // missing leaves the column null
        public void An_answers_length_and_missing_values()
        {
            Assert.Equal(new[] { "DB1|<null>|<null>|<null>" }, Rows(Packet("ServerName;DB1;;ServerName;DB2;;", declared: 15)));
            Assert.Equal(new[] { "DB1|X|<null>|<null>" }, Rows(Packet("ServerName;DB1;InstanceName;X;;", declared: 500)));
            // the keys in any case (as the dictionary .NET's SqlClient reads them with); the last one without its value
            Assert.Equal(new[] { "DB1|<null>|No|<null>" }, Rows(Packet("servername;DB1;isclustered;No;instancename;;")));
        }
    }
}
