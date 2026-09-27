using System;

namespace FrameworkOnCore
{
    /// <summary>
    /// System.Uri's creation as on Windows. On Linux and macOS .NET takes "/Portals/0/home.css" for an absolute file
    /// URI (file:///Portals/0/home.css): Uri.TryCreate(path, UriKind.Absolute) is true, new Uri(path,
    /// UriKind.RelativeOrAbsolute).IsAbsoluteUri too, and the application's "is this an absolute URL" says yes to its
    /// own paths (DNN's ClientResourceManager registered every style sheet, found or not). On Windows such a string
    /// is relative; here as well. The converter rewrites the calls (rules/packages.json platformReplacements).
    /// </summary>
    public static class WindowsUri
    {
        // A path from the root ("/x", not "//host"), which only Unix reads as a file.
        static bool IsRootPath(string uriString) =>
            !OperatingSystem.IsWindows() && uriString != null && uriString.Length > 0 && uriString[0] == '/' &&
            (uriString.Length == 1 || uriString[1] != '/');

        public static bool TryCreate(string uriString, UriKind uriKind, out Uri result)
        {
            if (IsRootPath(uriString))
            {
                if (uriKind == UriKind.Absolute) { result = null; return false; }
                return Uri.TryCreate(uriString, UriKind.Relative, out result);
            }
            return Uri.TryCreate(uriString, uriKind, out result);
        }

        public static bool TryCreate(Uri baseUri, string relativeUri, out Uri result) => Uri.TryCreate(baseUri, relativeUri, out result);

        public static bool TryCreate(Uri baseUri, Uri relativeUri, out Uri result) => Uri.TryCreate(baseUri, relativeUri, out result);

        public static bool IsWellFormedUriString(string uriString, UriKind uriKind) =>
            IsRootPath(uriString) ? uriKind != UriKind.Absolute && Uri.IsWellFormedUriString(uriString, UriKind.Relative) : Uri.IsWellFormedUriString(uriString, uriKind);

        /// <summary>new Uri(uriString, uriKind).</summary>
        public static Uri Create(string uriString, UriKind uriKind)
        {
            if (IsRootPath(uriString))
            {
                if (uriKind == UriKind.Absolute) throw new UriFormatException("Invalid URI: The format of the URI could not be determined.");
                return new Uri(uriString, UriKind.Relative);
            }
            return new Uri(uriString, uriKind);
        }
    }
}
