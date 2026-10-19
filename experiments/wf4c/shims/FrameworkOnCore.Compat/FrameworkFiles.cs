using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FrameworkOnCore
{
    /// <summary>
    /// Listing files as .NET Framework on Windows did:
    /// - the pattern without regard to case ("*.txt" also readme.TXT; .NET elsewhere than Windows matches with it);
    /// - the order: NTFS gives a folder's names sorted (without regard to case), Linux in the order the folder holds them;
    ///   elsewhere than Windows the names are sorted so, a folder at a time, the folders in the order they are reached
    ///   (.NET's: the folder's names, then its folders');
    /// - the patterns as Windows matched them with the 8.3 names .NET Framework's FindFirstFile saw: "*.xls" also gives
    ///   book.xlsx (its short name BOOK~1.XLS), "*." also .hidden (HIDDEN~1). .NET matches the long names only. The
    ///   AppContext switch FrameworkOnCore.ShortNamePatterns=false turns it off (a volume without 8.3 names);
    /// - Path.GetInvalidPathChars and GetInvalidFileNameChars: Windows' 36 and 41 (.NET on Windows 33 and 41, on Linux 1 and 2).
    /// The converter rewrites the calls (rules/packages.json platformReplacements).
    /// </summary>
    public static class FrameworkFiles
    {
        static bool Windows => Path.DirectorySeparatorChar == '\\';

        // As Directory.GetFiles(path, pattern, option) enumerates (EnumerationOptions.Compatible), the pattern matched
        // without regard to case as on Windows (.NET elsewhere matches it with it: "*.txt" not readme.TXT).
        static EnumerationOptions Options(SearchOption option) => new EnumerationOptions
        {
            MatchCasing = MatchCasing.CaseInsensitive,
            MatchType = MatchType.Win32,
            AttributesToSkip = 0,
            IgnoreInaccessible = false,
            RecurseSubdirectories = option == SearchOption.AllDirectories,
        };

        static bool ShortNamePatterns => !AppContext.TryGetSwitch("FrameworkOnCore.ShortNamePatterns", out var on) || on;

        public static string[] GetFiles(string path) => GetFiles(path, "*", SearchOption.TopDirectoryOnly);
        public static string[] GetFiles(string path, string searchPattern) => GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption) =>
            Ordered(path, Matching(searchPattern, p => Directory.GetFiles(path, p, Options(searchOption))), searchOption).ToArray();

        public static IEnumerable<string> EnumerateFiles(string path) => GetFiles(path);
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern) => GetFiles(path, searchPattern);
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) => GetFiles(path, searchPattern, searchOption);

        public static string[] GetDirectories(string path) => GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
        public static string[] GetDirectories(string path, string searchPattern) => GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) =>
            Ordered(path, Matching(searchPattern, p => Directory.GetDirectories(path, p, Options(searchOption))), searchOption).ToArray();

        public static IEnumerable<string> EnumerateDirectories(string path) => GetDirectories(path);
        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern) => GetDirectories(path, searchPattern);
        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) => GetDirectories(path, searchPattern, searchOption);

        public static string[] GetFileSystemEntries(string path) => GetFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
        public static string[] GetFileSystemEntries(string path, string searchPattern) => GetFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
        public static string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
            Ordered(path, Matching(searchPattern, p => Directory.GetFileSystemEntries(path, p, Options(searchOption))), searchOption).ToArray();

        public static IEnumerable<string> EnumerateFileSystemEntries(string path) => GetFileSystemEntries(path);
        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern) => GetFileSystemEntries(path, searchPattern);
        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) => GetFileSystemEntries(path, searchPattern, searchOption);

        public static FileInfo[] GetFiles(DirectoryInfo directory) => GetFiles(directory, "*", SearchOption.TopDirectoryOnly);
        public static FileInfo[] GetFiles(DirectoryInfo directory, string searchPattern) => GetFiles(directory, searchPattern, SearchOption.TopDirectoryOnly);
        public static FileInfo[] GetFiles(DirectoryInfo directory, string searchPattern, SearchOption searchOption) =>
            GetFiles(directory.FullName, searchPattern, searchOption).Select(f => new FileInfo(f)).ToArray();

        public static IEnumerable<FileInfo> EnumerateFiles(DirectoryInfo directory) => GetFiles(directory);
        public static IEnumerable<FileInfo> EnumerateFiles(DirectoryInfo directory, string searchPattern) => GetFiles(directory, searchPattern);
        public static IEnumerable<FileInfo> EnumerateFiles(DirectoryInfo directory, string searchPattern, SearchOption searchOption) => GetFiles(directory, searchPattern, searchOption);

        public static DirectoryInfo[] GetDirectories(DirectoryInfo directory) => GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);
        public static DirectoryInfo[] GetDirectories(DirectoryInfo directory, string searchPattern) => GetDirectories(directory, searchPattern, SearchOption.TopDirectoryOnly);
        public static DirectoryInfo[] GetDirectories(DirectoryInfo directory, string searchPattern, SearchOption searchOption) =>
            GetDirectories(directory.FullName, searchPattern, searchOption).Select(d => new DirectoryInfo(d)).ToArray();

        public static IEnumerable<DirectoryInfo> EnumerateDirectories(DirectoryInfo directory) => GetDirectories(directory);
        public static IEnumerable<DirectoryInfo> EnumerateDirectories(DirectoryInfo directory, string searchPattern) => GetDirectories(directory, searchPattern);
        public static IEnumerable<DirectoryInfo> EnumerateDirectories(DirectoryInfo directory, string searchPattern, SearchOption searchOption) => GetDirectories(directory, searchPattern, searchOption);

        static readonly char[] invalidPathChars = new[] { '"', '<', '>', '|' }.Concat(Enumerable.Range(0, 32).Select(c => (char)c)).ToArray();
        static readonly char[] invalidFileNameChars = invalidPathChars.Concat(new[] { ':', '*', '?', '\\', '/' }).ToArray();

        public static char[] GetInvalidPathChars() => (char[])invalidPathChars.Clone();
        public static char[] GetInvalidFileNameChars() => (char[])invalidFileNameChars.Clone();

        // The pattern as Windows matched it with the short names: "*.ext" (three characters) also the extensions it
        // begins, "*." also a name of a dot and no other (.hidden: HIDDEN~1, without an extension).
        static IEnumerable<string> Matching(string pattern, Func<string, string[]> list)
        {
            if (pattern == null || !ShortNamePatterns) return list(pattern);
            var dot = pattern.LastIndexOf('.');
            if (dot > 0 && pattern.Length - dot - 1 == 3 && pattern.IndexOf('*') >= 0 && pattern.IndexOf('*') < dot
                && pattern.IndexOfAny(new[] { '*', '?' }, dot) < 0 && pattern.IndexOfAny(new[] { '\\', '/' }) < 0)
            {
                var extension = pattern.Substring(dot);
                return list(pattern + "*").Where(p => Path.GetExtension(p).StartsWith(extension, StringComparison.OrdinalIgnoreCase));
            }
            if (pattern == "*.")
                return list("*").Where(p => { var name = Path.GetFileName(p); return name.IndexOf('.') < 0 || (name[0] == '.' && name.IndexOf('.', 1) < 0); });
            return list(pattern);
        }

        // Elsewhere than Windows: a folder's names as NTFS keeps them (by their upper case, ordinal), the folders in the
        // order .NET reaches them (a folder's entries, then its folders', breadth first).
        static IEnumerable<string> Ordered(string root, IEnumerable<string> paths, SearchOption option)
        {
            if (Windows) return paths;
            var list = paths.ToList();
            if (option == SearchOption.TopDirectoryOnly) return list.OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase).ThenBy(p => p, StringComparer.Ordinal);
            var order = new Dictionary<string, int>(StringComparer.Ordinal);
            var pending = new Queue<string>();
            var top = Path.TrimEndingDirectorySeparator(root);
            pending.Enqueue(top);
            while (pending.Count > 0)
            {
                var folder = pending.Dequeue();
                order[folder] = order.Count;
                string[] folders;
                try { folders = Directory.GetDirectories(folder); }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException) { continue; }
                foreach (var sub in folders.OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)) pending.Enqueue(sub);
            }
            return list.OrderBy(p => order.TryGetValue(Path.GetDirectoryName(p) ?? "", out var i) ? i : int.MaxValue)
                .ThenBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase).ThenBy(p => p, StringComparer.Ordinal);
        }
    }
}
