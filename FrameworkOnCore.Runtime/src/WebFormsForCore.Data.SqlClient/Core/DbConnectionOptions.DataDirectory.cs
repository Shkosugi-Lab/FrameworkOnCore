// WebFormsForCore: .NET Framework's expansion of |DataDirectory| (referencesource, System.Data's
// DbConnectionOptions.ExpandDataDirectory), which .NET's System.Data.SqlClient left out (it refuses the token). The
// platform's separator where .NET Framework had '\'; either is taken after the token.

using System.IO;

namespace System.Data.Common
{
    internal partial class DbConnectionOptions
    {
        /// <summary>
        /// The value with the |DataDirectory| it starts with replaced by the AppDomain's DataDirectory (ASP.NET sets it to
        /// App_Data), or else the application's base directory; null when it does not start with it. A path that leaves that
        /// folder ("..") is refused, as .NET Framework refused it.
        /// </summary>
        internal static string ExpandDataDirectory(string keyword, string value)
        {
            string fullPath = null;
            if ((null != value) && value.StartsWith(DataDirectory, StringComparison.OrdinalIgnoreCase))
            {
                // find the replacement path
                object rootFolderObject = AppDomain.CurrentDomain.GetData("DataDirectory");
                string rootFolderPath = (rootFolderObject as string);
                if ((null != rootFolderObject) && (null == rootFolderPath))
                {
                    throw ADP.InvalidDataDirectory();
                }
                else if (string.IsNullOrEmpty(rootFolderPath))
                {
                    rootFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                }
                if (null == rootFolderPath)
                {
                    rootFolderPath = "";
                }

                // We don't know if rootFolderpath ends with a separator, and we don't know if the given name starts with one
                int fileNamePosition = DataDirectory.Length;    // filename starts right after the '|datadirectory|' keyword
                bool rootFolderEndsWith = (0 < rootFolderPath.Length) && IsSeparator(rootFolderPath[rootFolderPath.Length - 1]);
                bool fileNameStartsWith = (fileNamePosition < value.Length) && IsSeparator(value[fileNamePosition]);

                // replace |datadirectory| with root folder path
                if (!rootFolderEndsWith && !fileNameStartsWith)
                {
                    // need to insert a separator
                    fullPath = rootFolderPath + Path.DirectorySeparatorChar + value.Substring(fileNamePosition);
                }
                else if (rootFolderEndsWith && fileNameStartsWith)
                {
                    // need to strip one out
                    fullPath = rootFolderPath + value.Substring(fileNamePosition + 1);
                }
                else
                {
                    // simply concatenate the strings
                    fullPath = rootFolderPath + value.Substring(fileNamePosition);
                }

                // verify root folder path is a real path without unexpected "..\"
                // .NET Framework compared with the DataDirectory as it is: a relative one is refused. The port compares a
                // full one's full path (its choice: '/' and '\' alike).
                if (!Path.IsPathFullyQualified(rootFolderPath) ||
                    !Path.GetFullPath(fullPath).StartsWith(Path.GetFullPath(rootFolderPath), StringComparison.Ordinal))
                {
                    throw ADP.InvalidConnectionOptionValue(keyword);
                }
            }
            return fullPath;

            static bool IsSeparator(char c) => c == '\\' || c == '/';
        }

        /// <summary>
        /// .NET Framework's DbConnectionOptions.Expand: the connection string with |DataDirectory| expanded, the pool
        /// group's key (DbConnectionFactory.GetConnectionPoolGroup), so that the DataDirectory is read for each connection
        /// string given. The string itself when nothing is expanded.
        /// </summary>
        internal virtual string Expand() => _usersConnectionString;

        /// <summary>.NET Framework's DbConnectionOptions.ExpandKeyword: the keyword's value (the one in effect) replaced.</summary>
        protected string ExpandKeyword(string keyword, string replacementValue)
        {
            bool expanded = false;
            int copyPosition = 0;
            var builder = new System.Text.StringBuilder(_usersConnectionString.Length);
            _parsetable.TryGetValue(keyword, out string value);
            for (NameValuePair current = _keyChain; null != current; current = current.Next)
            {
                if ((current.Name == keyword) && (current.Value == value))
                {
                    // only the value in effect replaced: the other values of a keyword given twice stay
                    DbConnectionStringBuilder.AppendKeyValuePair(builder, current.Name, replacementValue);
                    builder.Append(';');
                    expanded = true;
                }
                else
                {
                    builder.Append(_usersConnectionString, copyPosition, current.Length);
                }
                copyPosition += current.Length;
            }
            if (!expanded)
            {
                DbConnectionStringBuilder.AppendKeyValuePair(builder, keyword, replacementValue);
            }
            return builder.ToString();
        }

        /// <summary>
        /// .NET Framework's SqlConnectionString.VerifyLocalHostAndFixup (without its fix-up): a Data Source on this machine,
        /// for a file |DataDirectory| expanded: none, '.', '(local)', the machine's fully qualified DNS name or its first
        /// label, alone or with an instance ("\name"). A port (",1433"), a protocol ("tcp:") or "localhost" is not.
        /// </summary>
        internal static bool IsLocalHost(string host)
        {
            if (string.IsNullOrEmpty(host) || IsHost(host, ".") || IsHost(host, "(local)"))
                return true;
            string name = ComputerNameDnsFullyQualified();
            int separator = name.IndexOf('.');
            return IsHost(host, name) || (separator > 0 && IsHost(host, name.Substring(0, separator)))
                // the port: the NetBIOS name too (what the DNS name is but for its length or case)
                || IsHost(host, Environment.MachineName);

            static bool IsHost(string host, string name) =>
                host.Equals(name, StringComparison.OrdinalIgnoreCase) || host.StartsWith(name + "\\", StringComparison.OrdinalIgnoreCase);
        }

        // GetComputerNameEx(ComputerNameDnsFullyQualified): the host name and the primary DNS suffix
        static string ComputerNameDnsFullyQualified()
        {
            try
            {
                var properties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
                return string.IsNullOrEmpty(properties.DomainName) ? properties.HostName : properties.HostName + "." + properties.DomainName;
            }
            catch (Exception)
            {
                return Environment.MachineName;
            }
        }
    }
}
