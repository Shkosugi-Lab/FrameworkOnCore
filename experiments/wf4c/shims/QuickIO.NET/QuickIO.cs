using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchwabenCode.QuickIO
{
    /// <summary>How QuickIO returned paths: as given, or as \\?\ (UNC) paths. Only the regular form on .NET.</summary>
    public enum QuickIOPathType { Regular, UNC }

    /// <summary>Whether enumerations skip what cannot be read.</summary>
    public enum QuickIOEnumerateOptions { None, SuppressAllExceptions }

    /// <summary>A file system entry's kind.</summary>
    public enum QuickIOFileSystemEntryType { File, Directory }

    // QuickIO's exceptions: what the applications catch (DNN's PackageInstaller: PathNotFoundException). System.IO's
    // not-found exceptions are given as PathNotFoundException, as QuickIO threw it.
    public abstract class QuickIOBaseException : Exception
    {
        protected QuickIOBaseException(string message, string path, Exception inner = null) : base(message, inner) { Path = path; }
        public string Path { get; }
    }
    public class PathNotFoundException : QuickIOBaseException
    {
        public PathNotFoundException(string path) : base("Path not found: " + path, path) { }
        public PathNotFoundException(string message, string path) : base(message, path) { }
        internal PathNotFoundException(string path, Exception inner) : base(inner.Message, path, inner) { }
    }
    public class InvalidPathException : QuickIOBaseException
    {
        public InvalidPathException(string path) : base("Invalid path: " + path, path) { }
        public InvalidPathException(string message, string path) : base(message, path) { }
    }
    public class FileAlreadyExistsException : QuickIOBaseException
    {
        public FileAlreadyExistsException(string path) : base("File already exists: " + path, path) { }
        public FileAlreadyExistsException(string message, string path) : base(message, path) { }
    }
    public class PathAlreadyExistsException : QuickIOBaseException { public PathAlreadyExistsException(string message, string path) : base(message, path) { } }
    public class DirectoryAlreadyExistsException : QuickIOBaseException { public DirectoryAlreadyExistsException(string message, string path) : base(message, path) { } }
    public class DirectoryNotEmptyException : QuickIOBaseException { public DirectoryNotEmptyException(string message, string path) : base(message, path) { } }
    public class FileSystemIsBusyException : QuickIOBaseException { public FileSystemIsBusyException(string message, string path) : base(message, path) { } }
    public class UnsupportedDriveTypeException : QuickIOBaseException { public UnsupportedDriveTypeException(string path) : base("Unsupported drive type: " + path, path) { } }
    public class UnsupportedShareTypeException : QuickIOBaseException { public UnsupportedShareTypeException(string message, string path) : base(message, path) { } }

    static class NotFound
    {
        internal static T Translate<T>(string path, Func<T> operation)
        {
            try { return operation(); }
            catch (FileNotFoundException e) { throw new PathNotFoundException(path, e); }
            catch (DirectoryNotFoundException e) { throw new PathNotFoundException(path, e); }
        }

        internal static void Translate(string path, Action operation) => Translate(path, () => { operation(); return 0; });
    }

    /// <summary>A path (QuickIO parsed and checked it; here it is System.IO's).</summary>
    public class QuickIOPathInfo
    {
        public QuickIOPathInfo(string path) { FullName = Path.GetFullPath(path); }

        public string FullName { get; }
        public string FullNameUnc => FullName;
        public string Name => Path.GetFileName(FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        public string ParentFullName => Path.GetDirectoryName(FullName);
        public bool Exists => File.Exists(FullName) || Directory.Exists(FullName);
        public FileAttributes Attributes => File.GetAttributes(FullName);

        public override string ToString() => FullName;
    }

    public abstract class QuickIOFileSystemEntryBase
    {
        protected QuickIOFileSystemEntryBase(string path) { PathInfo = new QuickIOPathInfo(path); }

        public QuickIOPathInfo PathInfo { get; }
        public string FullName => PathInfo.FullName;
        public string FullNameUnc => PathInfo.FullName;
        public string Name => PathInfo.Name;
        public string ParentFullName => PathInfo.ParentFullName;
        public abstract bool Exists { get; }
        public Task<bool> ExistsAsync => Task.FromResult(Exists);
        public FileAttributes Attributes => File.GetAttributes(FullName);
        public bool IsReadOnly => (Attributes & FileAttributes.ReadOnly) != 0;
        public DateTime CreationTime => File.GetCreationTime(FullName);
        public DateTime CreationTimeUtc => File.GetCreationTimeUtc(FullName);
        public DateTime LastAccessTime => File.GetLastAccessTime(FullName);
        public DateTime LastAccessTimeUtc => File.GetLastAccessTimeUtc(FullName);
        public DateTime LastWriteTime => File.GetLastWriteTime(FullName);
        public DateTime LastWriteTimeUtc => File.GetLastWriteTimeUtc(FullName);

        public override string ToString() => FullName;
    }

    public class QuickIOFileInfo : QuickIOFileSystemEntryBase
    {
        public QuickIOFileInfo(string path) : base(path) { }
        public QuickIOFileInfo(FileInfo fileInfo) : base(fileInfo.FullName) { }
        public QuickIOFileInfo(QuickIOPathInfo pathInfo) : base(pathInfo.FullName) { }

        public override bool Exists => File.Exists(FullName);
        public ulong Bytes => (ulong)new FileInfo(FullName).Length;

        public FileStream Open(FileMode mode = FileMode.Open) => QuickIOFile.Open(FullName, mode);
        public FileStream Open(FileMode mode, FileAccess access) => QuickIOFile.Open(FullName, mode, access);
        public FileStream Open(FileMode mode, FileAccess access, FileShare share) => QuickIOFile.Open(FullName, mode, access, share);
        public FileStream OpenRead() => QuickIOFile.OpenRead(FullName);
        public StreamReader OpenText() => QuickIOFile.OpenText(FullName);
        public byte[] ReadAllBytes() => File.ReadAllBytes(FullName);
        public Task<byte[]> ReadAllBytesAsync(int readBuffer = 4096) => File.ReadAllBytesAsync(FullName);
    }

    public class QuickIODirectoryInfo : QuickIOFileSystemEntryBase
    {
        public QuickIODirectoryInfo(string path) : base(path) { }
        public QuickIODirectoryInfo(DirectoryInfo directoryInfo) : base(directoryInfo.FullName) { }
        public QuickIODirectoryInfo(QuickIOPathInfo pathInfo) : base(pathInfo.FullName) { }

        public override bool Exists => Directory.Exists(FullName);
        public bool IsRoot => Path.GetPathRoot(FullName) == FullName;

        public IEnumerable<string> EnumerateDirectoryPaths(string pattern = QuickIODirectory.QuickIOPatternAll, SearchOption searchOption = SearchOption.TopDirectoryOnly,
            QuickIOPathType pathFormatReturn = QuickIOPathType.Regular, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            QuickIODirectory.EnumerateDirectoryPaths(FullName, pattern, searchOption, pathFormatReturn, enumerateOptions);

        public IEnumerable<string> EnumerateFilePaths(string pattern = QuickIODirectory.QuickIOPatternAll, SearchOption searchOption = SearchOption.TopDirectoryOnly,
            QuickIOPathType pathFormatReturn = QuickIOPathType.Regular, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            QuickIODirectory.EnumerateFilePaths(FullName, pattern, searchOption, pathFormatReturn, enumerateOptions);

        public IEnumerable<KeyValuePair<QuickIOPathInfo, QuickIOFileSystemEntryType>> EnumerateFileSystemEntries(string pattern = QuickIODirectory.QuickIOPatternAll,
            SearchOption searchOption = SearchOption.TopDirectoryOnly, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            QuickIODirectory.EnumerateFileSystemEntries(FullName, pattern, searchOption, enumerateOptions);
    }

    /// <summary>QuickIO's file operations, as System.IO's.</summary>
    public static class QuickIOFile
    {
        public static bool Exists(string path) => File.Exists(path);
        public static bool Exists(QuickIOPathInfo pathInfo) => File.Exists(pathInfo.FullName);
        public static bool Exists(QuickIOFileInfo fileInfo) => File.Exists(fileInfo.FullName);
        public static Task<bool> ExistsAsync(string path) => Task.FromResult(File.Exists(path));
        public static Task<bool> ExistsAsync(QuickIOPathInfo pathInfo) => ExistsAsync(pathInfo.FullName);
        public static Task<bool> ExistsAsync(QuickIOFileInfo fileInfo) => ExistsAsync(fileInfo.FullName);

        public static void Create(string fullName, FileAccess fileAccess = FileAccess.Write, FileShare fileShare = FileShare.None, FileMode fileMode = FileMode.Create, FileAttributes fileAttributes = 0)
        {
            using (new FileStream(fullName, fileMode, fileAccess, fileShare)) { }
            if (fileAttributes != 0) File.SetAttributes(fullName, fileAttributes);
        }
        public static void Create(QuickIOPathInfo pathInfo, FileAccess fileAccess = FileAccess.Write, FileShare fileShare = FileShare.None, FileMode fileMode = FileMode.Create, FileAttributes fileAttributes = 0) =>
            Create(pathInfo.FullName, fileAccess, fileShare, fileMode, fileAttributes);
        public static FileStream Create(string path, int bufferSize) => new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize);
        public static FileStream Create(QuickIOPathInfo pathInfo, int bufferSize) => Create(pathInfo.FullName, bufferSize);

        public static FileStream Open(string path, FileMode mode = FileMode.Open) => NotFound.Translate(path, () => File.Open(path, mode));
        public static FileStream Open(string path, FileMode mode, FileAccess access) => NotFound.Translate(path, () => File.Open(path, mode, access));
        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) => NotFound.Translate(path, () => File.Open(path, mode, access, share));
        public static FileStream Open(QuickIOPathInfo info, FileMode mode = FileMode.Open) => File.Open(info.FullName, mode);
        public static FileStream Open(QuickIOPathInfo pathInfo, FileMode mode, FileAccess access) => File.Open(pathInfo.FullName, mode, access);
        public static FileStream Open(QuickIOPathInfo pathInfo, FileMode mode, FileAccess access, FileShare share) => File.Open(pathInfo.FullName, mode, access, share);
        public static FileStream OpenRead(string path) => NotFound.Translate(path, () => File.OpenRead(path));
        public static FileStream OpenRead(QuickIOPathInfo pathInfo) => File.OpenRead(pathInfo.FullName);
        public static StreamReader OpenText(string path) => NotFound.Translate(path, () => File.OpenText(path));
        public static StreamReader OpenText(QuickIOPathInfo pathInfo) => File.OpenText(pathInfo.FullName);

        public static void Copy(string uncSourceFullName, string uncTargetFullName, bool overwrite = false) => NotFound.Translate(uncSourceFullName, () => File.Copy(uncSourceFullName, uncTargetFullName, overwrite));
        public static Task CopyAsync(string uncSourceFullName, string uncTargetFullName, bool overwrite = false) =>
            Task.Run(() => Copy(uncSourceFullName, uncTargetFullName, overwrite));

        public static void Move(string sourceFileName, string destinationFileName) => NotFound.Translate(sourceFileName, () => File.Move(sourceFileName, destinationFileName));
        public static Task MoveAsync(string sourceFileName, string destinationFileName) => Task.Run(() => Move(sourceFileName, destinationFileName));
        public static Task MoveAsync(QuickIOPathInfo sourceFileInfo, QuickIOPathInfo destinationFolder) =>
            MoveAsync(sourceFileInfo.FullName, Path.Combine(destinationFolder.FullName, sourceFileInfo.Name));
        public static Task MoveAsync(QuickIOPathInfo sourceFileInfo, QuickIODirectoryInfo destinationFolder) =>
            MoveAsync(sourceFileInfo.FullName, Path.Combine(destinationFolder.FullName, sourceFileInfo.Name));

        public static void Delete(string path) => File.Delete(path);
        public static void Delete(QuickIOPathInfo pathInfo) => File.Delete(pathInfo.FullName);
        public static Task DeleteAsync(string path) => Task.Run(() => File.Delete(path));
        public static Task DeleteAsync(QuickIOPathInfo pathInfo) => DeleteAsync(pathInfo.FullName);

        public static FileAttributes GetAttributes(string path) => NotFound.Translate(path, () => File.GetAttributes(path));
        public static Task<FileAttributes> GetAttributesAsync(string path) => Task.FromResult(GetAttributes(path));
        public static Task<FileAttributes> GetAttributesAsync(QuickIOPathInfo info) => GetAttributesAsync(info.FullName);
        public static Task<FileAttributes> GetAttributesAsync(QuickIOFileInfo info) => GetAttributesAsync(info.FullName);
        public static void SetAttributes(string path, FileAttributes attributes) => NotFound.Translate(path, () => File.SetAttributes(path, attributes));
        public static void SetAttributes(QuickIOPathInfo info, FileAttributes attributes) => File.SetAttributes(info.FullName, attributes);
        public static void SetAttributes(QuickIOFileInfo info, FileAttributes attributes) => File.SetAttributes(info.FullName, attributes);
        public static Task SetAttributesAsync(string path, FileAttributes attributes) => Task.Run(() => SetAttributes(path, attributes));
        public static Task SetAttributesAsync(QuickIOPathInfo info, FileAttributes attributes) => SetAttributesAsync(info.FullName, attributes);
        public static Task SetAttributesAsync(QuickIOFileInfo info, FileAttributes attributes) => SetAttributesAsync(info.FullName, attributes);

        public static DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);
        public static Task<DateTime> GetLastWriteTimeAsync(string path) => Task.FromResult(File.GetLastWriteTime(path));
        public static Task<DateTime> GetLastWriteTimeAsync(QuickIOPathInfo info) => GetLastWriteTimeAsync(info.FullName);
        public static Task<DateTime> GetLastWriteTimeAsync(QuickIOFileInfo info) => GetLastWriteTimeAsync(info.FullName);
    }

    /// <summary>QuickIO's directory operations, as System.IO's.</summary>
    public static class QuickIODirectory
    {
        public const string QuickIOPatternAll = "*";

        public static bool Exists(string path) => Directory.Exists(path);
        public static bool Exists(QuickIOPathInfo pathInfo) => Directory.Exists(pathInfo.FullName);
        public static bool Exists(QuickIODirectoryInfo directoryInfo) => Directory.Exists(directoryInfo.FullName);
        public static Task<bool> ExistsAsync(string path) => Task.FromResult(Directory.Exists(path));
        public static Task<bool> ExistsAsync(QuickIOPathInfo pathInfo) => ExistsAsync(pathInfo.FullName);
        public static Task<bool> ExistsAsync(QuickIODirectoryInfo directoryInfo) => ExistsAsync(directoryInfo.FullName);

        // QuickIO created one level unless recursive; System.IO creates the parents too.
        public static void Create(string path, bool recursive = false) => Directory.CreateDirectory(path);
        public static void Create(QuickIOPathInfo pathInfo, bool recursive = false) => Directory.CreateDirectory(pathInfo.FullName);
        public static Task CreateAsync(string path, bool recursive = false) => Task.Run(() => Directory.CreateDirectory(path));
        public static Task CreateAsync(QuickIOPathInfo pathInfo, bool recursive = false) => CreateAsync(pathInfo.FullName, recursive);

        public static void Delete(string path, bool recursive = false) => NotFound.Translate(path, () => Directory.Delete(path, recursive));
        public static void Delete(QuickIOPathInfo info, bool recursive = false) => Directory.Delete(info.FullName, recursive);
        public static Task DeleteAsync(string path, bool recursive = false) => Task.Run(() => Delete(path, recursive));
        public static Task DeleteAsync(QuickIOPathInfo info, bool recursive = false) => DeleteAsync(info.FullName, recursive);

        public static void Copy(string source, string target, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(target);
            foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var destination = Path.Combine(target, Path.GetRelativePath(source, file));
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                File.Copy(file, destination, overwrite);
            }
        }
        public static Task CopyAsync(string source, string target, bool overwrite = false, CancellationToken cancellationToken = default) =>
            Task.Run(() => Copy(source, target, overwrite, cancellationToken), cancellationToken);
        public static Task MoveAsync(string from, string to, bool overwrite = false) => Task.Run(() => Directory.Move(from, to));

        public static FileAttributes GetAttributes(string path) => NotFound.Translate(path, () => File.GetAttributes(path));
        public static Task<FileAttributes> GetAttributesAsync(string path) => Task.FromResult(GetAttributes(path));
        public static void SetAttributes(string path, FileAttributes attributes) => NotFound.Translate(path, () => File.SetAttributes(path, attributes));
        public static void SetAttributes(QuickIOPathInfo info, FileAttributes attributes) => File.SetAttributes(info.FullName, attributes);
        public static void SetAttributes(QuickIODirectoryInfo info, FileAttributes attributes) => File.SetAttributes(info.FullName, attributes);
        public static Task SetAttributesAsync(string path, FileAttributes attributes) => Task.Run(() => SetAttributes(path, attributes));
        public static Task<DateTime> GetLastWriteTimeAsync(string path) => Task.FromResult(Directory.GetLastWriteTime(path));

        static EnumerationOptions Options(SearchOption searchOption, QuickIOEnumerateOptions enumerateOptions) => new EnumerationOptions
        {
            RecurseSubdirectories = searchOption == SearchOption.AllDirectories,
            IgnoreInaccessible = enumerateOptions == QuickIOEnumerateOptions.SuppressAllExceptions,
            // Windows' matching, as QuickIO had it (FindFirstFile): "*.*" is every file, case does not matter.
            MatchType = MatchType.Win32,
            MatchCasing = MatchCasing.CaseInsensitive,
            AttributesToSkip = 0,
        };

        public static IEnumerable<string> EnumerateDirectoryPaths(string path, string pattern = QuickIOPatternAll, SearchOption searchOption = SearchOption.TopDirectoryOnly,
            QuickIOPathType pathFormatReturn = QuickIOPathType.Regular, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            Directory.EnumerateDirectories(path, pattern, Options(searchOption, enumerateOptions));
        public static IEnumerable<string> EnumerateDirectoryPaths(QuickIOPathInfo info, string pattern = QuickIOPatternAll, SearchOption searchOption = SearchOption.TopDirectoryOnly,
            QuickIOPathType pathFormatReturn = QuickIOPathType.Regular, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            EnumerateDirectoryPaths(info.FullName, pattern, searchOption, pathFormatReturn, enumerateOptions);
        public static IEnumerable<string> EnumerateDirectoryPaths(QuickIODirectoryInfo directoryInfo, string pattern = QuickIOPatternAll, SearchOption searchOption = SearchOption.TopDirectoryOnly,
            QuickIOPathType pathFormatReturn = QuickIOPathType.Regular, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            EnumerateDirectoryPaths(directoryInfo.FullName, pattern, searchOption, pathFormatReturn, enumerateOptions);

        public static IEnumerable<string> EnumerateFilePaths(string path, string pattern = QuickIOPatternAll, SearchOption searchOption = SearchOption.TopDirectoryOnly,
            QuickIOPathType pathFormatReturn = QuickIOPathType.Regular, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            Directory.EnumerateFiles(path, pattern, Options(searchOption, enumerateOptions));
        public static IEnumerable<string> EnumerateFilePaths(QuickIOPathInfo info, string pattern = QuickIOPatternAll, SearchOption searchOption = SearchOption.TopDirectoryOnly,
            QuickIOPathType pathFormatReturn = QuickIOPathType.Regular, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            EnumerateFilePaths(info.FullName, pattern, searchOption, pathFormatReturn, enumerateOptions);
        public static IEnumerable<string> EnumerateFilePaths(QuickIODirectoryInfo directoryInfo, string pattern = QuickIOPatternAll, SearchOption searchOption = SearchOption.TopDirectoryOnly,
            QuickIOPathType pathFormatReturn = QuickIOPathType.Regular, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            EnumerateFilePaths(directoryInfo.FullName, pattern, searchOption, pathFormatReturn, enumerateOptions);

        public static IEnumerable<KeyValuePair<QuickIOPathInfo, QuickIOFileSystemEntryType>> EnumerateFileSystemEntries(string path, string pattern = QuickIOPatternAll,
            SearchOption searchOption = SearchOption.TopDirectoryOnly, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            Directory.EnumerateFileSystemEntries(path, pattern, Options(searchOption, enumerateOptions))
                .Select(e => new KeyValuePair<QuickIOPathInfo, QuickIOFileSystemEntryType>(new QuickIOPathInfo(e),
                    Directory.Exists(e) ? QuickIOFileSystemEntryType.Directory : QuickIOFileSystemEntryType.File));
        public static IEnumerable<KeyValuePair<QuickIOPathInfo, QuickIOFileSystemEntryType>> EnumerateFileSystemEntries(QuickIOPathInfo pathInfo, string pattern = QuickIOPatternAll,
            SearchOption searchOption = SearchOption.TopDirectoryOnly, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            EnumerateFileSystemEntries(pathInfo.FullName, pattern, searchOption, enumerateOptions);
        public static IEnumerable<KeyValuePair<QuickIOPathInfo, QuickIOFileSystemEntryType>> EnumerateFileSystemEntries(QuickIODirectoryInfo directoryInfo, string pattern = QuickIOPatternAll,
            SearchOption searchOption = SearchOption.TopDirectoryOnly, QuickIOEnumerateOptions enumerateOptions = QuickIOEnumerateOptions.None) =>
            EnumerateFileSystemEntries(directoryInfo.FullName, pattern, searchOption, enumerateOptions);
    }
}
