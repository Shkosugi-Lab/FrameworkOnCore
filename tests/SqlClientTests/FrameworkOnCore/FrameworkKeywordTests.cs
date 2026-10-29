// FrameworkOnCore: the connection string keywords .NET Framework 4.8's SqlClient took and .NET's refuses (Asynchronous
// Processing, Connection Reset, Network Library, Context Connection), and their builder properties (ELMAH's SqlErrorLog
// sets AsynchronousProcessing), as the port gives them back; a connection to the functional tests' TDS server.

using System.Linq;
using System.Net;
using Microsoft.SqlServer.TDS;
using Microsoft.SqlServer.TDS.EndPoint;
using Microsoft.SqlServer.TDS.EnvChange;
using Microsoft.SqlServer.TDS.Servers;
using Xunit;

namespace System.Data.SqlClient.Tests
{
    public class FrameworkKeywordTests
    {
        sealed class Server : IDisposable
        {
            readonly TDSServerEndPoint _endpoint;
            public string DataSource { get; }

            public Server()
            {
                _endpoint = new TDSServerEndPoint(new GenericTDSServer(new TDSServerArguments())) { ServerEndPoint = new IPEndPoint(IPAddress.Any, 0) };
                _endpoint.Start();
                DataSource = "localhost," + _endpoint.ServerEndPoint.Port;
            }

            public void Dispose() => _endpoint.Stop();
        }

        static void Open(string keywords)
        {
            using var server = new Server();
            using var connection = new SqlConnection($"Data Source={server.DataSource};Connect Timeout=30;Encrypt=false;Pooling=false;{keywords}");
            connection.Open();
            Assert.Equal(ConnectionState.Open, connection.State);
        }

        [Theory] // taken, not used: since .NET Framework 4.5 every connection is asynchronous capable and reset from the pool
        [InlineData("Asynchronous Processing=true")]
        [InlineData("async=true")]
        [InlineData("Connection Reset=false")]
        [InlineData("Context Connection=false")]
        public void Keywords_dotnet_refuses_connect_as_on_dotnet_framework(string keyword)
        {
            Open(keyword);
        }

        [Theory] // .NET Framework 4.6.1's: taken, and a host's addresses tried as .NET's are
        [InlineData("TransparentNetworkIPResolution=false")]
        [InlineData("TransparentNetworkIPResolution=true")]
        public void Transparent_network_ip_resolution_connects(string keyword)
        {
            Open(keyword);
        }

        [Fact]
        public void The_builder_has_transparent_network_ip_resolution()
        {
            var builder = new SqlConnectionStringBuilder("Data Source=srv");
            Assert.True(builder.TransparentNetworkIPResolution);
            builder.TransparentNetworkIPResolution = false;
            Assert.Equal("Data Source=srv;TransparentNetworkIPResolution=False", builder.ConnectionString);
            Assert.False(new SqlConnectionStringBuilder(builder.ConnectionString).TransparentNetworkIPResolution);
            Assert.Throws<ArgumentException>(() => new SqlConnection("Data Source=srv;TransparentNetworkIPResolution=maybe"));
        }

        [Fact] // obsolete since .NET Framework 2.0, AddWithValue's twin; libraries built for .NET Framework call it
        public void Parameters_add_with_a_value()
        {
            var command = new SqlCommand();
#pragma warning disable 618
            SqlParameter parameter = command.Parameters.Add("@name", (object)"value");
#pragma warning restore 618
            Assert.Equal("@name", parameter.ParameterName);
            Assert.Equal("value", parameter.Value);
            Assert.Same(parameter, command.Parameters["@name"]);
        }

        [Theory] // the library names of TCP/IP: the protocol of the server name (tcp:)
        [InlineData("Network Library=DBMSSOCN")]
        [InlineData("Network Library=dbmssocn")]
        [InlineData("net=dbmssocn")]
        [InlineData("Network=DBMSSOCN")]
        public void Network_library_tcp_connects(string keyword)
        {
            Open(keyword);
        }

        // The library is the protocol: named pipes' (np:), where the test server listens on TCP only, connects not (where
        // TCP/IP's connects, above); the error is the platform's SNI's (Windows: an np: name with a port is not valid).
        // Named pipes with such names are Windows' (not supported on Unix, as ExceptionTest's).
        [Fact, PlatformSpecific(TestPlatforms.Windows)]
        public void Network_library_named_pipes_is_the_protocol()
        {
            using var server = new Server();
            using var connection = new SqlConnection($"Data Source={server.DataSource};Connect Timeout=5;Encrypt=false;Pooling=false;Network Library=dbnmpntw");
            Assert.Throws<SqlException>(() => connection.Open());
        }

        // A User Instance's first connection (to SQL Server Express, for the instance's name) has the library's protocol
        // (named pipes: Windows', as above).
        [Fact, PlatformSpecific(TestPlatforms.Windows)]
        public void Network_library_with_a_user_instance()
        {
            using var server = new Server();
            using var connection = new SqlConnection($"Data Source={server.DataSource};Connect Timeout=5;Encrypt=false;Pooling=false;User Instance=true;Network Library=dbnmpntw");
            Assert.Throws<SqlException>(() => connection.Open());
        }

        // A database mirroring principal that names its partner (ENVCHANGE 13, real-time log shipping) and stops answering
        // the first login; the partner it names is itself, which answers the next ones.
        sealed class MirroredServer : GenericTDSServer, IDisposable
        {
            readonly TDSServerEndPoint _endpoint;
            public string DataSource { get; }
            public int Logins;

            public MirroredServer() : base(new TDSServerArguments())
            {
                _endpoint = new TDSServerEndPoint(this) { ServerEndPoint = new IPEndPoint(IPAddress.Any, 0) };
                _endpoint.Start();
                DataSource = "localhost," + _endpoint.ServerEndPoint.Port;
            }

            public override TDSMessageCollection OnLogin7Request(ITDSServerSession session, TDSMessage request)
            {
                if (System.Threading.Interlocked.Increment(ref Logins) == 1)
                {
                    return new TDSMessageCollection(new TDSMessage(TDSMessageType.Response, new TDSEnvChangeToken(TDSEnvChangeTokenType.RealTimeLogShipping, DataSource)));
                }
                return base.OnLogin7Request(session, request);
            }

            public void Dispose() => _endpoint.Stop();
        }

        [Theory] // the failover partner the server names is connected to with the library's protocol (.NET Framework's
                 // ServerInfo), or none without the keyword
        [InlineData(";Network Library=dbmssocn")]
        [InlineData("")]
        public void Network_library_with_the_failover_partner_the_server_names(string keyword)
        {
            using var server = new MirroredServer();
            using var connection = new SqlConnection($"Data Source={server.DataSource};Failover Partner=nowhere;Initial Catalog=db;Connect Timeout=30;Encrypt=false;Pooling=false{keyword}");
            connection.Open();
            Assert.Equal(ConnectionState.Open, connection.State);
            Assert.Equal(2, server.Logins);
        }

        [Theory] // as .NET Framework: a value that is not one is refused when the connection string is given
        [InlineData("Asynchronous Processing=maybe")]
        [InlineData("Connection Reset=maybe")]
        [InlineData("Network Library=dbmsfoo")]
        public void An_invalid_value_is_refused(string keyword)
        {
            Assert.Throws<ArgumentException>(() => new SqlConnection("Data Source=localhost;" + keyword));
        }

        [Fact] // SQL CLR's in-process connection: there is none (.NET Framework refused it when the connection string is given,
               // outside SQL Server; with InvalidOperationException there: FrameworkGoldenTests.Bug_context_connection_true_is_an_invalid_operation)
        public void Context_connection_true_is_refused()
        {
            Assert.ThrowsAny<SystemException>(() => new SqlConnection("Data Source=localhost;Context Connection=true"));
        }

        [Fact] // ELMAH's SqlErrorLog: new SqlConnectionStringBuilder(connectionString) { AsynchronousProcessing = true }
        public void The_builder_has_the_properties()
        {
            var builder = new SqlConnectionStringBuilder("Data Source=srv;async=true;net=DBMSSOCN") { ContextConnection = false };
            Assert.True(builder.AsynchronousProcessing);
            Assert.Equal("dbmssocn", builder.NetworkLibrary);
#pragma warning disable 618 // Obsolete, as on .NET Framework
            Assert.True(builder.ConnectionReset);
            builder.ConnectionReset = false;
#pragma warning restore 618
            builder.AsynchronousProcessing = false;
            var written = new SqlConnectionStringBuilder(builder.ConnectionString);
            Assert.False(written.AsynchronousProcessing);
            Assert.Equal("dbmssocn", written.NetworkLibrary);
            Assert.Contains("Context Connection=False", builder.ConnectionString);
            Assert.Contains("Connection Reset=False", builder.ConnectionString);
            Assert.Throws<ArgumentException>(() => builder.NetworkLibrary = "dbmsfoo");

            Assert.True(builder.Remove("Network Library"));
            Assert.Equal("", builder.NetworkLibrary);
            Assert.Contains("Asynchronous Processing", builder.Keys.Cast<string>());
            Assert.True(builder.ContainsKey("network"));
        }
    }
}
