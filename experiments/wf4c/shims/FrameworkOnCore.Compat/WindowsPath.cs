namespace FrameworkOnCore
{
    /// <summary>
    /// The application's Windows paths ("bin\\", Globals.ApplicationMapPath + "\\web.config"), on the platform
    /// it runs on. The converter wraps the path literals written with Windows' separator in Native (where the
    /// literal flows into a path: a file API, a path it is joined to, a path searched for it), and makes a
    /// separator character Path.DirectorySeparatorChar. On Windows both are what the source had.
    /// </summary>
    public static class WindowsPath
    {
        /// <summary>
        /// The path with Windows' separators made the platform's, and, in the application's folder, the case of its
        /// names as they are on disk (Windows' file names do not tell case: DNN's manifests name
        /// "resource-skin.zip", the package has "Resource-Skin.zip"). As it is on Windows. Mono's IOMAP did the same.
        /// </summary>
        public static string Native(string windowsPath)
        {
            if (System.IO.Path.DirectorySeparatorChar == '\\' || windowsPath == null) return windowsPath;
            return MatchCase(windowsPath.Replace('\\', System.IO.Path.DirectorySeparatorChar));
        }

        // Where the path is not there as written: each name that is not, the one there that differs in case only
        // (an exact name first); from the first one missing altogether (a file to create), the rest as written.
        static string MatchCase(string path)
        {
            var separator = System.IO.Path.DirectorySeparatorChar;
            var root = System.AppDomain.CurrentDomain.BaseDirectory?.TrimEnd(separator);
            if (string.IsNullOrEmpty(root) || !path.StartsWith(root + separator, System.StringComparison.Ordinal)) return path;
            if (System.IO.File.Exists(path) || System.IO.Directory.Exists(path)) return path;
            var names = path.Substring(root.Length + 1).Split(new[] { separator }, System.StringSplitOptions.RemoveEmptyEntries);
            var current = root;
            for (var i = 0; i < names.Length; i++)
            {
                var exact = current + separator + names[i];
                if (System.IO.Directory.Exists(exact) || System.IO.File.Exists(exact)) { current = exact; continue; }
                string found = null;
                if (System.IO.Directory.Exists(current))
                {
                    foreach (var entry in System.IO.Directory.EnumerateFileSystemEntries(current))
                    {
                        if (string.Equals(System.IO.Path.GetFileName(entry), names[i], System.StringComparison.OrdinalIgnoreCase)) { found = entry; break; }
                    }
                }
                if (found == null)
                {
                    var rest = current + separator + string.Join(separator.ToString(), names, i, names.Length - i);
                    return path.EndsWith(separator.ToString(), System.StringComparison.Ordinal) ? rest + separator : rest;
                }
                current = found;
            }
            return path.EndsWith(separator.ToString(), System.StringComparison.Ordinal) ? current + separator : current;
        }

        /// <summary>
        /// path.TrimStart(separators) where the path is joined to a folder next (Path.Combine(root,
        /// filename.TrimStart('\\', '/')): DNN's Config.Save): a path starting with a separator is relative to the
        /// folder, and an absolute one, which starts with a drive on Windows, keeps it and is what Path.Combine
        /// returns. On Linux an absolute path starts with the separator too: one in the application's folder or
        /// the temporary folder (where the application makes its absolute paths) is left as it is.
        /// </summary>
        public static string TrimStartRelative(string path, params char[] separators)
        {
            if (path == null || System.IO.Path.DirectorySeparatorChar == '\\' || !IsAbsoluteOnThisPlatform(path)) return path?.TrimStart(separators);
            return path;
        }

        /// <summary>path.Trim(separators), as TrimStartRelative at the start.</summary>
        public static string TrimRelative(string path, params char[] separators) => TrimStartRelative(path, separators)?.TrimEnd(separators);

        static bool IsAbsoluteOnThisPlatform(string path)
        {
            foreach (var folder in new[] { System.AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.GetTempPath() })
            {
                if (string.IsNullOrEmpty(folder)) continue;
                var root = folder.TrimEnd(System.IO.Path.DirectorySeparatorChar);
                if (path == root || path.StartsWith(root + System.IO.Path.DirectorySeparatorChar, System.StringComparison.Ordinal)) return true;
            }
            return false;
        }
    }
}
