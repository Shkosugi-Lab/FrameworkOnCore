// WebFormsForCore: the builder's properties .NET Framework 4.8 had for the keywords .NET's System.Data.SqlClient refuses
// (referencesource, System.Data's SqlConnectionStringBuilder): an application or a library built for .NET Framework sets
// them (ELMAH's SqlErrorLog sets AsynchronousProcessing), and without them it fails with MissingMethodException. The
// connection takes them as .NET Framework's did (SqlConnectionString).

using System.ComponentModel;
using System.Data.Common;

namespace System.Data.SqlClient
{
    public sealed partial class SqlConnectionStringBuilder
    {
        private bool _asynchronousProcessing;
        private bool _connectionReset = true;
        private bool _contextConnection;
        private string _networkLibrary = string.Empty;

        /// <summary>Asynchronous Processing: kept in the connection string; not used (since .NET Framework 4.5 every connection is capable of asynchronous operations).</summary>
        [DisplayName(DbConnectionStringKeywords.AsynchronousProcessing)]
        [RefreshProperties(RefreshProperties.All)]
        public bool AsynchronousProcessing
        {
            get { return _asynchronousProcessing; }
            set
            {
                SetValue(DbConnectionStringKeywords.AsynchronousProcessing, value);
                _asynchronousProcessing = value;
            }
        }

        /// <summary>Connection Reset: kept in the connection string; a connection drawn from the pool is always reset.</summary>
        [Browsable(false)]
        [DisplayName(DbConnectionStringKeywords.ConnectionReset)]
        [Obsolete("ConnectionReset has been deprecated.  SqlConnection will ignore the 'connection reset' keyword and always reset the connection")]
        [RefreshProperties(RefreshProperties.All)]
        public bool ConnectionReset
        {
            get { return _connectionReset; }
            set
            {
                SetValue(DbConnectionStringKeywords.ConnectionReset, value);
                _connectionReset = value;
            }
        }

        /// <summary>Context Connection: SQL CLR's in-process connection; true is refused by SqlConnection, as there is none.</summary>
        [DisplayName(DbConnectionStringKeywords.ContextConnection)]
        [RefreshProperties(RefreshProperties.All)]
        public bool ContextConnection
        {
            get { return _contextConnection; }
            set
            {
                SetValue(DbConnectionStringKeywords.ContextConnection, value);
                _contextConnection = value;
            }
        }

        /// <summary>Network Library: the client library of the protocol (dbmssocn: TCP/IP, dbnmpntw: named pipes, dbmslpcn: shared memory...).</summary>
        [DisplayName(DbConnectionStringKeywords.NetworkLibrary)]
        [RefreshProperties(RefreshProperties.All)]
        public string NetworkLibrary
        {
            get { return _networkLibrary; }
            set
            {
                value = SqlConnectionString.NormalizeNetworkLibrary(value);
                SetValue(DbConnectionStringKeywords.NetworkLibrary, value);
                _networkLibrary = value;
            }
        }
    }
}
