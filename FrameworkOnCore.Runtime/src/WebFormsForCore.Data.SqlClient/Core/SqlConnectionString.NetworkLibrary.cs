// WebFormsForCore: .NET Framework's Network Library keyword (referencesource, System.Data's SqlConnectionString:
// NetlibMapping, and ServerInfo's UserProtocol), which .NET's System.Data.SqlClient refuses. The library names of the
// old client network utilities, the protocol prefixed to the server name; the protocols SNI does not have (rpc, bv,
// adsp, spx, via) fail when connecting, as on .NET Framework.

using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;

namespace System.Data.SqlClient
{
    internal sealed partial class SqlConnectionString
    {
        internal static class NETLIB
        {
            internal const string AppleTalk = "dbmsadsn";
            internal const string BanyanVines = "dbmsvinn";
            internal const string IPXSPX = "dbmsspxn";
            internal const string Multiprotocol = "dbmsrpcn";
            internal const string NamedPipes = "dbnmpntw";
            internal const string SharedMemory = "dbmslpcn";
            internal const string TCPIP = "dbmssocn";
            internal const string VIA = "dbmsgnet";
        }

        private static readonly Dictionary<string, string> s_netlibMapping = new Dictionary<string, string>
        {
            { NETLIB.TCPIP, TdsEnums.TCP },
            { NETLIB.NamedPipes, TdsEnums.NP },
            { NETLIB.Multiprotocol, TdsEnums.RPC },
            { NETLIB.BanyanVines, TdsEnums.BV },
            { NETLIB.AppleTalk, TdsEnums.ADSP },
            { NETLIB.IPXSPX, TdsEnums.SPX },
            { NETLIB.VIA, TdsEnums.VIA },
            { NETLIB.SharedMemory, TdsEnums.LPC },
        };

        /// <summary>The protocol (tcp, np, lpc...) the Network Library value names; null without one. Another value is refused.</summary>
        private static string NetworkLibraryProtocol(string networkLibrary)
        {
            if (networkLibrary == null)
            {
                return null;
            }
            if (!s_netlibMapping.TryGetValue(networkLibrary.Trim().ToLower(CultureInfo.InvariantCulture), out string protocol))
            {
                throw ADP.InvalidConnectionOptionValue(KEY.Network_Library);
            }
            return protocol;
        }

        /// <summary>The protocol the Network Library keyword gives the server name (ServerInfo's UserProtocol); null without it.</summary>
        internal string NetworkLibrary => _networkLibrary;

        /// <summary>A Network Library value as .NET Framework's builder keeps it (the library's name); another is refused.</summary>
        internal static string NormalizeNetworkLibrary(string value)
        {
            if (value == null)
            {
                return null;
            }
            string library = value.Trim().ToLower(CultureInfo.InvariantCulture);
            if (!s_netlibMapping.ContainsKey(library))
            {
                throw ADP.InvalidConnectionOptionValue(DbConnectionStringKeywords.NetworkLibrary);
            }
            return library;
        }
    }
}
