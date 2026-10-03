// FrameworkOnCore: System.Data.Sql.SqlDataSourceEnumerator (.NET Framework's; .NET's System.Data.SqlClient left it out), as
// SqlClientFactory gives it: the SQL Server Browser services asked by broadcast (SSRP CLNT_BCAST_EX, UDP 1434). A browser of
// the test's own answers, where the port is free (a machine with SQL Server Browser has it: the test is left out there).

using System.Data.Sql;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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

        [Fact]
        public void The_browsers_instances_are_the_table_dotnet_framework_gives()
        {
            UdpClient browser;
            try
            {
                browser = new UdpClient(new IPEndPoint(IPAddress.Any, 1434)) { EnableBroadcast = true };
            }
            catch (SocketException)
            {
                return; // SQL Server Browser of this machine has the port
            }
            using (browser)
            {
                // Two instances, the default one among them (no instance name in the table), and one of them twice.
                var answer = Task.Run(async () =>
                {
                    var request = await browser.ReceiveAsync();
                    Assert.Equal(new byte[] { 0x02 }, request.Buffer);
                    foreach (var text in new[]
                             {
                                 "ServerName;DB1;InstanceName;MSSQLSERVER;IsClustered;No;Version;16.0.1000.6;tcp;1433;;ServerName;DB1;InstanceName;SQLEXPRESS;IsClustered;No;Version;16.0.1000.6;tcp;50000;;",
                                 "ServerName;DB1;InstanceName;SQLEXPRESS;IsClustered;No;Version;16.0.1000.6;tcp;50000;;",
                             })
                    {
                        var body = Encoding.ASCII.GetBytes(text);
                        var packet = new byte[] { 0x05, (byte)body.Length, (byte)(body.Length >> 8) }.Concat(body).ToArray();
                        await browser.SendAsync(packet, packet.Length, request.RemoteEndPoint);
                    }
                });

                DataTable table = SqlDataSourceEnumerator.Instance.GetDataSources();
                answer.Wait(TimeSpan.FromSeconds(5));

                Assert.Equal("SqlDataSources", table.TableName);
                Assert.Equal(new[] { "ServerName", "InstanceName", "IsClustered", "Version" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                Assert.All(table.Columns.Cast<DataColumn>(), c => Assert.True(c.ReadOnly));
                var rows = table.Rows.Cast<DataRow>().Select(r => $"{r["ServerName"]}|{(r["InstanceName"] is DBNull ? "<null>" : r["InstanceName"])}|{r["IsClustered"]}|{r["Version"]}").ToList();
                Assert.Equal(new[] { "DB1|<null>|No|16.0.1000.6", "DB1|SQLEXPRESS|No|16.0.1000.6" }, rows);
            }
        }
    }
}
