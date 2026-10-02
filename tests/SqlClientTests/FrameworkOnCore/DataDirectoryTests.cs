// FrameworkOnCore: what the port changes (|DataDirectory| in AttachDBFilename expanded, as .NET Framework's SqlClient did),
// against the test server the functional tests use: the path the client sends in its login is the one asserted.

using System.IO;
using System.Net;
using Microsoft.SqlServer.TDS;
using Microsoft.SqlServer.TDS.EndPoint;
using Microsoft.SqlServer.TDS.Login7;
using Microsoft.SqlServer.TDS.Servers;
using Xunit;

namespace System.Data.SqlClient.Tests
{
    // The AppDomain's DataDirectory is the process's: these tests do not run beside each other.
    [CollectionDefinition(nameof(DataDirectoryTests), DisableParallelization = true)]
    public class DataDirectoryCollection { }

    [Collection(nameof(DataDirectoryTests))]
    public class DataDirectoryTests
    {
        // The test server, which keeps the file the client asked to attach.
        sealed class AttachingServer : GenericTDSServer, IDisposable
        {
            readonly TDSServerEndPoint _endpoint;
            public string Attached { get; private set; }
            public string ConnectionString { get; }

            public AttachingServer() : base(new TDSServerArguments())
            {
                _endpoint = new TDSServerEndPoint(this) { ServerEndPoint = new IPEndPoint(IPAddress.Any, 0) };
                _endpoint.Start();
                ConnectionString = new SqlConnectionStringBuilder { DataSource = "localhost," + _endpoint.ServerEndPoint.Port, ConnectTimeout = 30, Encrypt = false, Pooling = false }.ConnectionString;
            }

            public override TDSMessageCollection OnLogin7Request(ITDSServerSession session, TDSMessage request)
            {
                Attached = (request[0] as TDSLogin7Token)?.AttachDatabaseFile;
                return base.OnLogin7Request(session, request);
            }

            public void Dispose() => _endpoint.Stop();
        }

        static string Attached(string attachDbFilename, object dataDirectory)
        {
            var before = AppDomain.CurrentDomain.GetData("DataDirectory");
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
            try
            {
                using var server = new AttachingServer();
                using var connection = new SqlConnection(server.ConnectionString + ";AttachDBFilename=" + attachDbFilename);
                connection.Open();
                return server.Attached;
            }
            finally
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", before);
            }
        }

        static readonly string Folder = Path.Combine(Path.GetTempPath(), "site", "App_Data");

        [Fact] // DNN's install wizard: "|DataDirectory|" + file
        public void The_token_is_the_DataDirectory()
        {
            Assert.Equal(Path.Combine(Folder, "Database.mdf"), Attached("|DataDirectory|Database.mdf", Folder));
        }

        [Fact] // a separator after the token and one at the folder's end: one of them
        public void One_separator_between_the_folder_and_the_file()
        {
            Assert.Equal(Path.Combine(Folder, "Database.mdf"), Attached("|DataDirectory|" + Path.DirectorySeparatorChar + "Database.mdf", Folder + Path.DirectorySeparatorChar));
        }

        [Fact] // without one, the application's base directory (as .NET Framework)
        public void Without_a_DataDirectory_the_base_directory()
        {
            var expected = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + "Database.mdf";
            Assert.Equal(expected, Attached("|DataDirectory|Database.mdf", null));
        }

        [Fact] // a path that leaves the folder is refused, as .NET Framework refused it
        public void A_path_out_of_the_DataDirectory_is_refused()
        {
            var before = AppDomain.CurrentDomain.GetData("DataDirectory");
            AppDomain.CurrentDomain.SetData("DataDirectory", Folder);
            try
            {
                Assert.Throws<ArgumentException>(() => new SqlConnection("Data Source=localhost;AttachDBFilename=|DataDirectory|.." + Path.DirectorySeparatorChar + "Database.mdf"));
            }
            finally
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", before);
            }
        }

        [Fact] // a path given as it is: as it was
        public void A_path_is_sent_as_it_is()
        {
            var path = Path.Combine(Folder, "Other.mdf");
            Assert.Equal(path, Attached(path, Folder));
        }

        [Fact] // the token elsewhere than at the start: refused, as .NET Framework refused it
        public void The_token_elsewhere_is_refused()
        {
            Assert.Throws<ArgumentException>(() => new SqlConnection("Data Source=localhost;AttachDBFilename=x|DataDirectory|Database.mdf"));
        }
    }
}
