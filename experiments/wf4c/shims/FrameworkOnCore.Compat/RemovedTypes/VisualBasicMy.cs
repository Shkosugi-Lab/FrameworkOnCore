using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic.FileIO;

// Visual Basic's My namespace for web projects (MyType Web and Custom: My.Computer, My.User, My.Log in the
// project's MyWebExtension.vb, and the compiler's ThreadSafeObjectProvider). .NET Framework had these types in
// Microsoft.VisualBasic.dll; .NET has some of them only in the desktop's Microsoft.VisualBasic.Forms (Windows)
// and not the web ones. Here as a server has them: the current request's user, the file system of
// Microsoft.VisualBasic.FileIO (which .NET has), a log on System.Diagnostics' trace.

namespace Microsoft.VisualBasic.MyServices.Internal
{
    /// <summary>
    /// A value per request (HttpContext.Items), or per thread outside one: what the compiler's My template keeps
    /// My.Computer, My.User and My.Log in. The request's context is looked up by name, as .NET Framework's did
    /// (System.Web may not be loaded).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ContextValue<T> : IDisposable
    {
        static readonly Lazy<PropertyInfo> current = new(() => Type.GetType("System.Web.HttpContext, System.Web")?.GetProperty("Current"));
        static readonly Lazy<PropertyInfo> items = new(() => Type.GetType("System.Web.HttpContext, System.Web")?.GetProperty("Items"));

        readonly string key = Guid.NewGuid().ToString();
        readonly ThreadLocal<T> perThread = new();

        public T Value
        {
            get => Items() is System.Collections.IDictionary request ? (request[key] is T value ? value : default) : perThread.Value;
            set
            {
                if (Items() is System.Collections.IDictionary request) request[key] = value;
                else perThread.Value = value;
            }
        }

        static System.Collections.IDictionary Items()
        {
            var context = current.Value?.GetValue(null);
            return context == null ? null : (System.Collections.IDictionary)items.Value.GetValue(context);
        }

        public void Dispose() => perThread.Dispose();
    }
}

namespace Microsoft.VisualBasic.Devices
{
    /// <summary>My.Computer of a web project: the server's name and file system.</summary>
    public class ServerComputer
    {
        public string Name => Environment.MachineName;

        public MyServices.FileSystemProxy FileSystem { get; } = new();
    }
}

namespace Microsoft.VisualBasic.MyServices
{
    /// <summary>
    /// My.Computer.FileSystem: Microsoft.VisualBasic.FileIO.FileSystem, which .NET has (qualified: in this namespace
    /// FileSystem is Microsoft.VisualBasic's file functions).
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FileSystemProxy
    {
        public string CurrentDirectory
        {
            get => FileIO.FileSystem.CurrentDirectory;
            set => FileIO.FileSystem.CurrentDirectory = value;
        }

        public ReadOnlyCollection<System.IO.DriveInfo> Drives => FileIO.FileSystem.Drives;

        public string CombinePath(string baseDirectory, string relativePath) => FileIO.FileSystem.CombinePath(baseDirectory, relativePath);

        public void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName) => FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName);

        public void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, bool overwrite) => FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, overwrite);

        public void CopyFile(string sourceFileName, string destinationFileName) => FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName);

        public void CopyFile(string sourceFileName, string destinationFileName, bool overwrite) => FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, overwrite);

        public void CreateDirectory(string directory) => FileIO.FileSystem.CreateDirectory(directory);

        public void DeleteDirectory(string directory, DeleteDirectoryOption onDirectoryNotEmpty) => FileIO.FileSystem.DeleteDirectory(directory, onDirectoryNotEmpty);

        public void DeleteFile(string file) => FileIO.FileSystem.DeleteFile(file);

        public bool DirectoryExists(string directory) => FileIO.FileSystem.DirectoryExists(directory);

        public bool FileExists(string file) => FileIO.FileSystem.FileExists(file);

        // Not in .NET's FileSystem: the files whose text (its encoding detected) contains the text.
        public ReadOnlyCollection<string> FindInFiles(string directory, string containsText, bool ignoreCase, SearchOption searchType) =>
            FindInFiles(directory, containsText, ignoreCase, searchType, null);

        public ReadOnlyCollection<string> FindInFiles(string directory, string containsText, bool ignoreCase, SearchOption searchType, params string[] fileWildcards)
        {
            var comparison = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            var found = new System.Collections.Generic.List<string>();
            foreach (var file in FileIO.FileSystem.GetFiles(directory, searchType, fileWildcards ?? Array.Empty<string>()))
            {
                if (string.IsNullOrEmpty(containsText) || System.IO.File.ReadAllText(file).Contains(containsText, comparison)) found.Add(file);
            }
            return found.AsReadOnly();
        }

        public ReadOnlyCollection<string> GetDirectories(string directory) => FileIO.FileSystem.GetDirectories(directory);

        public ReadOnlyCollection<string> GetDirectories(string directory, SearchOption searchType, params string[] wildcards) => FileIO.FileSystem.GetDirectories(directory, searchType, wildcards);

        public System.IO.DirectoryInfo GetDirectoryInfo(string directory) => FileIO.FileSystem.GetDirectoryInfo(directory);

        public System.IO.DriveInfo GetDriveInfo(string drive) => FileIO.FileSystem.GetDriveInfo(drive);

        public System.IO.FileInfo GetFileInfo(string file) => FileIO.FileSystem.GetFileInfo(file);

        public ReadOnlyCollection<string> GetFiles(string directory) => FileIO.FileSystem.GetFiles(directory);

        public ReadOnlyCollection<string> GetFiles(string directory, SearchOption searchType, params string[] wildcards) => FileIO.FileSystem.GetFiles(directory, searchType, wildcards);

        public string GetName(string path) => FileIO.FileSystem.GetName(path);

        public string GetParentPath(string path) => FileIO.FileSystem.GetParentPath(path);

        public string GetTempFileName() => FileIO.FileSystem.GetTempFileName();

        public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName) => FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName);

        public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, bool overwrite) => FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, overwrite);

        public void MoveFile(string sourceFileName, string destinationFileName) => FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName);

        public void MoveFile(string sourceFileName, string destinationFileName, bool overwrite) => FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, overwrite);

        public TextFieldParser OpenTextFieldParser(string file) => FileIO.FileSystem.OpenTextFieldParser(file);

        public TextFieldParser OpenTextFieldParser(string file, params string[] delimiters) => FileIO.FileSystem.OpenTextFieldParser(file, delimiters);

        public TextFieldParser OpenTextFieldParser(string file, params int[] fieldWidths) => FileIO.FileSystem.OpenTextFieldParser(file, fieldWidths);

        public System.IO.StreamReader OpenTextFileReader(string file) => FileIO.FileSystem.OpenTextFileReader(file);

        public System.IO.StreamReader OpenTextFileReader(string file, Encoding encoding) => FileIO.FileSystem.OpenTextFileReader(file, encoding);

        public System.IO.StreamWriter OpenTextFileWriter(string file, bool append) => FileIO.FileSystem.OpenTextFileWriter(file, append);

        public System.IO.StreamWriter OpenTextFileWriter(string file, bool append, Encoding encoding) => FileIO.FileSystem.OpenTextFileWriter(file, append, encoding);

        public byte[] ReadAllBytes(string file) => FileIO.FileSystem.ReadAllBytes(file);

        public string ReadAllText(string file) => FileIO.FileSystem.ReadAllText(file);

        public string ReadAllText(string file, Encoding encoding) => FileIO.FileSystem.ReadAllText(file, encoding);

        public void RenameDirectory(string directory, string newName) => FileIO.FileSystem.RenameDirectory(directory, newName);

        public void RenameFile(string file, string newName) => FileIO.FileSystem.RenameFile(file, newName);

        public void WriteAllBytes(string file, byte[] data, bool append) => FileIO.FileSystem.WriteAllBytes(file, data, append);

        public void WriteAllText(string file, string text, bool append) => FileIO.FileSystem.WriteAllText(file, text, append);

        public void WriteAllText(string file, string text, bool append, Encoding encoding) => FileIO.FileSystem.WriteAllText(file, text, append, encoding);
        // The overloads with a user interface: a server has none; errors are thrown, as .NET's FileSystem does
        // without a desktop (UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently: no dialog, no recycle bin).
        public void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, UIOption showUI) => FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, showUI);

        public void CopyDirectory(string sourceDirectoryName, string destinationDirectoryName, UIOption showUI, UICancelOption onUserCancel) => FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, showUI, onUserCancel);

        public void CopyFile(string sourceFileName, string destinationFileName, UIOption showUI) => FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, showUI);

        public void CopyFile(string sourceFileName, string destinationFileName, UIOption showUI, UICancelOption onUserCancel) => FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, showUI, onUserCancel);

        public void DeleteDirectory(string directory, UIOption showUI, RecycleOption recycle) => FileIO.FileSystem.DeleteDirectory(directory, showUI, recycle);

        public void DeleteDirectory(string directory, UIOption showUI, RecycleOption recycle, UICancelOption onUserCancel) => FileIO.FileSystem.DeleteDirectory(directory, showUI, recycle, onUserCancel);

        public void DeleteFile(string file, UIOption showUI, RecycleOption recycle) => FileIO.FileSystem.DeleteFile(file, showUI, recycle);

        public void DeleteFile(string file, UIOption showUI, RecycleOption recycle, UICancelOption onUserCancel) => FileIO.FileSystem.DeleteFile(file, showUI, recycle, onUserCancel);

        public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, UIOption showUI) => FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, showUI);

        public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName, UIOption showUI, UICancelOption onUserCancel) => FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, showUI, onUserCancel);

        public void MoveFile(string sourceFileName, string destinationFileName, UIOption showUI) => FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, showUI);

        public void MoveFile(string sourceFileName, string destinationFileName, UIOption showUI, UICancelOption onUserCancel) => FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, showUI, onUserCancel);
    }
}

namespace Microsoft.VisualBasic.ApplicationServices
{
    /// <summary>My.User of a web project: the current request's user (HttpContext.User), else the thread's.</summary>
    public class WebUser
    {
        static readonly Lazy<PropertyInfo> current = new(() => Type.GetType("System.Web.HttpContext, System.Web")?.GetProperty("Current"));
        static readonly Lazy<PropertyInfo> user = new(() => Type.GetType("System.Web.HttpContext, System.Web")?.GetProperty("User"));

        public IPrincipal CurrentPrincipal
        {
            get => current.Value?.GetValue(null) is { } context ? (IPrincipal)user.Value.GetValue(context) : Thread.CurrentPrincipal;
            set
            {
                if (current.Value?.GetValue(null) is { } context) user.Value.SetValue(context, value);
                else Thread.CurrentPrincipal = value;
            }
        }

        public string Name => CurrentPrincipal?.Identity?.Name ?? "";

        public bool IsAuthenticated => CurrentPrincipal?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role) => CurrentPrincipal?.IsInRole(role) ?? false;
    }
}

namespace Microsoft.VisualBasic.Logging
{
    /// <summary>My.Log of a web project: System.Diagnostics' trace (the DefaultSource source, as .NET Framework's).</summary>
    public class AspLog
    {
        public AspLog() : this("DefaultSource") { }

        public AspLog(string name) => TraceSource = new TraceSource(name);

        public TraceSource TraceSource { get; }

        public void WriteEntry(string message) => WriteEntry(message, TraceEventType.Information);

        public void WriteEntry(string message, TraceEventType severity) => WriteEntry(message, severity, 0);

        public void WriteEntry(string message, TraceEventType severity, int id) => TraceSource.TraceEvent(severity, id, message);

        public void WriteException(Exception ex) => WriteException(ex, TraceEventType.Error, "");

        public void WriteException(Exception ex, TraceEventType severity, string additionalInfo) => WriteException(ex, severity, additionalInfo, 0);

        public void WriteException(Exception ex, TraceEventType severity, string additionalInfo, int id) =>
            TraceSource.TraceEvent(severity, id, string.IsNullOrEmpty(additionalInfo) ? ex.Message : ex.Message + " " + additionalInfo);
    }
}
