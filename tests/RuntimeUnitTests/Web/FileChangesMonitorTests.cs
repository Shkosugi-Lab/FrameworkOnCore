using System.Collections.Concurrent;
using System.Reflection;
using System.Web;
using System.Web.Util;
using FrameworkOnCore.Runtime.Tests.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// File change notifications (FileChangesMonitor, DirectoryMonitor: what CacheDependency, the configuration and the
/// compilation watch) on .NET's FileSystemWatcher where .NET Framework had webengine4.dll's ReadDirectoryChangesW: the same
/// actions for the same changes (the reference source's DirectoryMonitor.OnFileChange; CacheDependency's documentation:
/// a file changed, deleted, created).
/// </summary>
[Collection("Application")]
public class FileChangesMonitorTests
{
    const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;
    const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    sealed class Changes
    {
        public readonly ConcurrentQueue<(FileAction Action, string Name)> Seen = new();
        public void Callback(object sender, FileChangeEvent e) => Seen.Enqueue((e.Action, e.FileName));
        public bool Wait(FileAction action, int milliseconds = 5000) => SpinWait.SpinUntil(() => Seen.Any(s => s.Action == action), milliseconds);
    }

    static object Completion(DirectoryMonitor monitor) => typeof(DirectoryMonitor).GetField("_dirMonCompletion", Instance).GetValue(monitor);

    static void Invoke(object target, string method, params object[] arguments) =>
        target.GetType().GetMethod(method, Instance).Invoke(target, arguments);

    [Fact] // .NET Framework's actions (reference source; CacheDependency's documentation): a monitored file changed, renamed, deleted, created
    public void A_monitored_file_s_changes_are_notified()
    {
        using var site = new Site().Write("a.txt", "a").Write("b.txt", "b");
        var monitor = new FileChangesMonitor(System.Web.Configuration.FcnMode.Default);
        var changes = new Changes();
        string a = Path.Combine(site.Root, "a.txt"), b = Path.Combine(site.Root, "b.txt"), c = Path.Combine(site.Root, "c.txt");
        monitor.StartMonitoringFile(a, changes.Callback);
        monitor.StartMonitoringFile(b, changes.Callback);
        monitor.StartMonitoringFile(c, changes.Callback);
        try
        {
            File.AppendAllText(a, "more");
            Assert.True(changes.Wait(FileAction.Modified), string.Join(",", changes.Seen));
            File.Move(b, b + ".old");
            Assert.True(changes.Wait(FileAction.RenamedOldName), string.Join(",", changes.Seen));
            File.Delete(a);
            Assert.True(changes.Wait(FileAction.Removed), string.Join(",", changes.Seen));
            File.WriteAllText(c, "c");
            Assert.True(changes.Wait(FileAction.Added), string.Join(",", changes.Seen));
            File.Move(b + ".old", b);
            Assert.True(changes.Wait(FileAction.RenamedNewName), string.Join(",", changes.Seen));
            Assert.All(changes.Seen, s => Assert.Contains(s.Name, new[] { a, b, c }));
        }
        finally
        {
            monitor.StopMonitoringFile(a, changes);
            monitor.StopMonitoringFile(b, changes);
            monitor.StopMonitoringFile(c, changes);
            monitor.Stop();
        }
    }

    [Fact] // the port's choice (IsChangeAfterStartMonitoring's comment): a file as it was when its monitoring started has no change to notify
    public void A_file_unchanged_since_monitoring_started_has_no_change()
    {
        using var site = new Site().Write("a.txt", "a");
        var path = Path.Combine(site.Root, "a.txt");
        var monitor = new DirectoryMonitor(site.Root, false, UnsafeNativeMethods.RDCW_FILTER_FILE_AND_DIR_CHANGES, 0);
        var changes = new Changes();
        var fileMonitor = monitor.StartMonitoringFileWithAssert("a.txt", changes.Callback, path);
        try
        {
            var target = fileMonitor.Targets.Cast<FileMonitorTarget>().Single();
            Assert.NotNull(target.AttributesAtStart);
            FileAttributesData.GetFileAttributes(path, out var now);
            var isChange = typeof(DirectoryMonitor).GetMethod("IsChangeAfterStartMonitoring", Instance);
            Assert.False((bool)isChange.Invoke(monitor, new object[] { now, target, DateTime.UtcNow.AddSeconds(1) }));
            File.AppendAllText(path, "more");
            FileAttributesData.GetFileAttributes(path, out var changed);
            Assert.True((bool)isChange.Invoke(monitor, new object[] { changed, target, DateTime.UtcNow.AddSeconds(1) }));
        }
        finally { monitor.StopMonitoring(); }
    }

    [Fact] // ReadDirectoryChangesW's filters (its documentation) as FileSystemWatcher's of the same names; none: the file changes' filter
    public void The_native_filters_are_the_watcher_s()
    {
        var map = typeof(DirectoryMonitor).Assembly.GetType("System.Web.DirMonCompletion").GetMethod("MapRdcwFilterToNotifyFilters", Static);
        NotifyFilters Map(uint filter) => (NotifyFilters)map.Invoke(null, new object[] { filter });
        Assert.Equal(NotifyFilters.FileName, Map(UnsafeNativeMethods.FILE_NOTIFY_CHANGE_FILE_NAME));
        Assert.Equal(NotifyFilters.DirectoryName, Map(UnsafeNativeMethods.FILE_NOTIFY_CHANGE_DIR_NAME));
        Assert.Equal(NotifyFilters.Attributes, Map(UnsafeNativeMethods.FILE_NOTIFY_CHANGE_ATTRIBUTES));
        Assert.Equal(NotifyFilters.Size, Map(UnsafeNativeMethods.FILE_NOTIFY_CHANGE_SIZE));
        Assert.Equal(NotifyFilters.LastWrite, Map(UnsafeNativeMethods.FILE_NOTIFY_CHANGE_LAST_WRITE));
        Assert.Equal(NotifyFilters.LastAccess, Map(UnsafeNativeMethods.FILE_NOTIFY_CHANGE_LAST_ACCESS));
        Assert.Equal(NotifyFilters.CreationTime, Map(UnsafeNativeMethods.FILE_NOTIFY_CHANGE_CREATION));
        Assert.Equal(NotifyFilters.Security, Map(UnsafeNativeMethods.FILE_NOTIFY_CHANGE_SECURITY));
        Assert.Equal(Map(UnsafeNativeMethods.RDCW_FILTER_FILE_CHANGES), Map(0));
        // A filter of none of these flags: file and directory names, writes and sizes (the port's choice).
        Assert.Equal(NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Size, Map(0x80000000));
    }

    [Fact] // the reference source: an overflowed buffer notifies every monitored file (Overwhelming) and the buffer grows; names it has not are ignored
    public void An_overflow_notifies_every_file_and_grows_the_buffer()
    {
        using var site = new Site().Write("a.txt", "a");
        var monitor = new DirectoryMonitor(site.Root, false, UnsafeNativeMethods.RDCW_FILTER_FILE_AND_DIR_CHANGES, 0);
        var changes = new Changes();
        monitor.StartMonitoringFileWithAssert("a.txt", changes.Callback, Path.Combine(site.Root, "a.txt"));
        try
        {
            var completion = Completion(monitor);
            var size = completion.GetType().GetField("_internalBufferSize", Instance);
            typeof(DirectoryMonitor).GetField("s_notificationBufferSizeIncreased", Static).SetValue(null, 0);
            Invoke(completion, "OnManagedFileSystemError", null, new ErrorEventArgs(new InternalBufferOverflowException()));
            Assert.True(changes.Wait(FileAction.Overwhelming));
            Assert.Equal(16384, size.GetValue(completion));
            Invoke(completion, "TryGrowManagedNotificationBuffer");
            Invoke(completion, "TryGrowManagedNotificationBuffer");
            Assert.Equal(65536, size.GetValue(completion));
            Invoke(completion, "TryGrowManagedNotificationBuffer");
            Assert.Equal(65536, size.GetValue(completion));
            // Events without a name (the watcher's own), are not the files'.
            int seen = changes.Seen.Count;
            Invoke(completion, "OnManagedFileSystemCreated", null, new FileSystemEventArgs(WatcherChangeTypes.Created, site.Root, null));
            Invoke(completion, "OnManagedFileSystemDeleted", null, new FileSystemEventArgs(WatcherChangeTypes.Deleted, site.Root, null));
            Invoke(completion, "OnManagedFileSystemChanged", null, new FileSystemEventArgs(WatcherChangeTypes.Changed, site.Root, null));
            Invoke(completion, "OnManagedFileSystemRenamed", null, new RenamedEventArgs(WatcherChangeTypes.Renamed, site.Root, null, null));
            Thread.Sleep(200);
            Assert.Equal(seen, changes.Seen.Count);
        }
        finally { monitor.StopMonitoring(); }
    }

    [Fact] // the reference source: an error stops the monitoring (the directory monitor disposed); a disposed one's notifications are ignored
    public void An_error_stops_the_monitoring()
    {
        using var site = new Site().Write("a.txt", "a");
        var runtime = typeof(HttpRuntime).GetField("_theRuntime", Static).GetValue(null);
        var fcm = typeof(HttpRuntime).GetField("_fcm", Instance);
        var previous = fcm.GetValue(runtime);
        if (previous == null) fcm.SetValue(runtime, new FileChangesMonitor(System.Web.Configuration.FcnMode.Default));
        var monitor = new DirectoryMonitor(site.Root, false, UnsafeNativeMethods.RDCW_FILTER_FILE_AND_DIR_CHANGES, 0);
        var changes = new Changes();
        monitor.StartMonitoringFileWithAssert("a.txt", changes.Callback, Path.Combine(site.Root, "a.txt"));
        try
        {
            var completion = Completion(monitor);
            Invoke(completion, "OnManagedFileSystemError", null, new ErrorEventArgs(new IOException("gone")));
            Assert.True(changes.Wait(FileAction.Error));
            Assert.Null(Completion(monitor));
            Assert.False(monitor.IsMonitoring());
            int seen = changes.Seen.Count;
            Invoke(completion, "OnManagedFileSystemChanged", null, new FileSystemEventArgs(WatcherChangeTypes.Changed, site.Root, "a.txt"));
            Invoke(completion, "OnManagedFileSystemCreated", null, new FileSystemEventArgs(WatcherChangeTypes.Created, site.Root, "a.txt"));
            Invoke(completion, "OnManagedFileSystemDeleted", null, new FileSystemEventArgs(WatcherChangeTypes.Deleted, site.Root, "a.txt"));
            Invoke(completion, "OnManagedFileSystemRenamed", null, new RenamedEventArgs(WatcherChangeTypes.Renamed, site.Root, "a.txt", "b.txt"));
            Invoke(completion, "OnManagedFileSystemError", null, new ErrorEventArgs(new IOException("gone")));
            Invoke(completion, "TryGrowManagedNotificationBuffer");
            Thread.Sleep(200);
            Assert.Equal(seen, changes.Seen.Count);
        }
        finally
        {
            monitor.StopMonitoring();
            fcm.SetValue(runtime, previous);
        }
    }

    [Fact] // the reference source (DirMonOpen failing: CreateFileMonitoringException): a folder that cannot be watched is an HttpException
    public void A_folder_that_cannot_be_watched_fails()
    {
        var missing = Path.Combine(Path.GetTempPath(), "foc-missing-" + Guid.NewGuid().ToString("N"));
        var monitor = new DirectoryMonitor(missing, false, UnsafeNativeMethods.RDCW_FILTER_FILE_AND_DIR_CHANGES, 0);
        var changes = new Changes();
        Assert.Throws<HttpException>(() => monitor.StartMonitoringFileWithAssert("a.txt", changes.Callback, Path.Combine(missing, "a.txt")));
    }
}
