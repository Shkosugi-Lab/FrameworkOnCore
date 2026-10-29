// FrameworkOnCore: what the port changes (|DataDirectory| in AttachDBFilename expanded, as .NET Framework's SqlClient did):
// the path the connection attaches. .NET Framework expanded it for a server on this machine only ('.', '(local)', its
// name; not "localhost", nor with a port: FrameworkGoldenTests), so the expanded path is read from the connection's
// options; the test server the functional tests use asserts that the path the client sends in its login is that one.

using System.IO;
using System.Net;
using System.Reflection;
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

        /// <summary>The file the connection attaches (its options', which its login sends: A_path_is_sent_as_it_is).</summary>
        static string Attached(string attachDbFilename, object dataDirectory)
        {
            var before = AppDomain.CurrentDomain.GetData("DataDirectory");
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
            try
            {
                using var connection = new SqlConnection("Data Source=.;AttachDBFilename=" + attachDbFilename);
                return AttachedBy(connection);
            }
            finally
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", before);
            }
        }

        static string AttachedBy(SqlConnection connection)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            object options = typeof(SqlConnection).GetProperty("ConnectionOptions", flags).GetValue(connection);
            return (string)options.GetType().GetProperty("AttachDBFilename", flags).GetValue(options);
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

        [Theory] // without one (or an empty one), the application's base directory (as .NET Framework)
        [InlineData(null)]
        [InlineData("")]
        public void Without_a_DataDirectory_the_base_directory(string dataDirectory)
        {
            var expected = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + "Database.mdf";
            Assert.Equal(expected, Attached("|DataDirectory|Database.mdf", dataDirectory));
        }

        [Fact] // the token alone: the folder, with the separator .NET Framework inserted
        public void The_token_alone_is_the_folder()
        {
            Assert.Equal(Folder + Path.DirectorySeparatorChar, Attached("|DataDirectory|", Folder));
        }

        [Fact] // the case of the token is not looked at (.NET Framework: StartsWith, OrdinalIgnoreCase)
        public void The_token_in_any_case()
        {
            Assert.Equal(Path.Combine(Folder, "Database.mdf"), Attached("|datadirectory|Database.mdf", Folder));
        }

        [Fact] // the folder ending with a separator, the file without one: as they are
        public void A_separator_at_the_folders_end_only()
        {
            Assert.Equal(Path.Combine(Folder, "Database.mdf"), Attached("|DataDirectory|Database.mdf", Folder + Path.DirectorySeparatorChar));
        }

        [Theory] // the port's choice (the code's comment): '/' is a separator as '\' is, on every platform (.NET Framework
                 // took '\' only, the separator of the one it ran on)
        [InlineData("/", "/")]
        [InlineData("/", "\\")]
        [InlineData("\\", "/")]
        public void Either_separator_between_the_folder_and_the_file(string folderEnd, string fileStart)
        {
            Assert.Equal(Folder + folderEnd + "Database.mdf", Attached("|DataDirectory|" + fileStart + "Database.mdf", Folder + folderEnd));
        }

        [Fact] // a file in a folder of the DataDirectory's, and back
        public void A_path_within_the_DataDirectory()
        {
            var sep = Path.DirectorySeparatorChar;
            Assert.Equal(Folder + sep + "sub" + sep + ".." + sep + "Database.mdf", Attached("|DataDirectory|sub" + sep + ".." + sep + "Database.mdf", Folder));
        }

        [Fact] // a DataDirectory that is not a string is refused when the connection string is given, as .NET Framework refused
               // it (InvalidOperationException: the golden "datadirectory not a string")
        public void A_DataDirectory_not_a_string_is_refused()
        {
            var before = AppDomain.CurrentDomain.GetData("DataDirectory");
            AppDomain.CurrentDomain.SetData("DataDirectory", 42);
            try
            {
                Assert.Throws<InvalidOperationException>(() => new SqlConnection("Data Source=.;AttachDBFilename=|DataDirectory|Database.mdf"));
            }
            finally
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", before);
            }
        }

        [Fact] // a path that leaves the folder is refused, as .NET Framework refused it
        public void A_path_out_of_the_DataDirectory_is_refused()
        {
            var before = AppDomain.CurrentDomain.GetData("DataDirectory");
            AppDomain.CurrentDomain.SetData("DataDirectory", Folder);
            try
            {
                Assert.Throws<ArgumentException>(() => new SqlConnection("Data Source=.;AttachDBFilename=|DataDirectory|.." + Path.DirectorySeparatorChar + "Database.mdf"));
            }
            finally
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", before);
            }
        }

        [Fact] // a path given as it is: as it was, the one the login sends (a server elsewhere: no |DataDirectory|, no check)
        public void A_path_is_sent_as_it_is()
        {
            var path = Path.Combine(Folder, "Other.mdf");
            using var server = new AttachingServer();
            using var connection = new SqlConnection(server.ConnectionString + ";AttachDBFilename=" + path);
            Assert.Equal(path, AttachedBy(connection));
            connection.Open();
            Assert.Equal(path, server.Attached);
        }

        [Theory] // .NET Framework's VerifyLocalHostAndFixup: the expanded file for a server on this machine only, an instance of it
                 // too; a port, a protocol, "localhost" or another machine refused (the golden: localhost and srv)
        [InlineData(@".\SQLEXPRESS", true)]
        [InlineData("(local)", true)]
        [InlineData("", true)]
        [InlineData("localhost", false)]
        [InlineData(".,1433", false)]
        [InlineData("tcp:.", false)]
        [InlineData(@"srv\SQLEXPRESS", false)]
        public void The_DataDirectory_is_for_a_server_on_this_machine(string dataSource, bool taken)
        {
            var before = AppDomain.CurrentDomain.GetData("DataDirectory");
            AppDomain.CurrentDomain.SetData("DataDirectory", Folder);
            try
            {
                var connectionString = "Data Source=" + dataSource + ";AttachDBFilename=|DataDirectory|Database.mdf";
                if (taken)
                    using (var connection = new SqlConnection(connectionString)) Assert.Equal(Path.Combine(Folder, "Database.mdf"), AttachedBy(connection));
                else
                    Assert.Throws<ArgumentException>(() => new SqlConnection(connectionString));
                // the name of the machine, as the golden's "machine"
                using (var connection = new SqlConnection("Data Source=" + Environment.MachineName.ToLowerInvariant() + ";AttachDBFilename=|DataDirectory|Database.mdf"))
                    Assert.Equal(Path.Combine(Folder, "Database.mdf"), AttachedBy(connection));
            }
            finally
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", before);
            }
        }

        [Fact] // .NET Framework's DbConnectionFactory: the pool group is the expanded string's, so the DataDirectory is read for
               // each connection string given (the same string, another DataDirectory: another file); the string kept as given
        public void The_DataDirectory_is_read_for_each_connection_string()
        {
            var other = Path.Combine(Path.GetTempPath(), "other", "App_Data");
            Assert.Equal(Path.Combine(Folder, "Database.mdf"), Attached("|DataDirectory|Database.mdf", Folder));
            Assert.Equal(Path.Combine(other, "Database.mdf"), Attached("|DataDirectory|Database.mdf", other));
            Assert.Throws<InvalidOperationException>(() => Attached("|DataDirectory|Database.mdf", 42));
            using var connection = new SqlConnection("Data Source=.;AttachDBFilename=|DataDirectory|Database.mdf");
            Assert.Equal("Data Source=.;AttachDBFilename=|DataDirectory|Database.mdf", connection.ConnectionString);
        }

        [Fact] // the token elsewhere than at the start: refused, as .NET Framework refused it
        public void The_token_elsewhere_is_refused()
        {
            Assert.Throws<ArgumentException>(() => new SqlConnection("Data Source=.;AttachDBFilename=x|DataDirectory|Database.mdf"));
        }
    }
}
