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
               // it (with InvalidOperationException there: FrameworkGoldenTests.Bug_a_datadirectory_not_a_string_is_an_invalid_operation)
        public void A_DataDirectory_not_a_string_is_refused()
        {
            var before = AppDomain.CurrentDomain.GetData("DataDirectory");
            AppDomain.CurrentDomain.SetData("DataDirectory", 42);
            try
            {
                Assert.ThrowsAny<SystemException>(() => new SqlConnection("Data Source=.;AttachDBFilename=|DataDirectory|Database.mdf"));
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
