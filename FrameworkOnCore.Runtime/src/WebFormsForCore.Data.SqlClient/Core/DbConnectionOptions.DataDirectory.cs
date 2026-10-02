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
                    throw ADP.InvalidConnectionOptionValue(keyword);
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
                if (!Path.GetFullPath(fullPath).StartsWith(Path.GetFullPath(rootFolderPath), StringComparison.Ordinal))
                {
                    throw ADP.InvalidConnectionOptionValue(keyword);
                }
            }
            return fullPath;

            static bool IsSeparator(char c) => c == '\\' || c == '/';
        }
    }
}
