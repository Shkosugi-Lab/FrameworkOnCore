namespace FrameworkOnCore.Analysis;

/// <summary>
/// Deleting a folder a build has been in. Directory.Delete(recursive) stops at what builds leave: a read-only file or
/// folder (git's objects, a package's files) and links (the junctions yarn and npm workspaces make in node_modules, as
/// DNN's build does: "Access to the path 'dnn-react-common' is denied"). Here a link is removed itself, not what it points
/// to, read-only is cleared, and a file another process is letting go of (an antivirus scan, a build node ending) is tried
/// again for a while.
/// </summary>
public static class FileTrees
{
    /// <summary>Deletes the folder and everything in it; nothing when there is none.</summary>
    public static void Delete(string path)
    {
        var root = new DirectoryInfo(path);
        if (!root.Exists) return;
        try
        {
            DeleteDirectory(root);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            throw new IOException($"could not delete {path}: {e.Message} (a process may still have a file in it open: a build, a running application, an editor; close it and try again)", e);
        }
    }

    static void DeleteDirectory(DirectoryInfo directory)
    {
        // A junction or a symbolic link: the link, not the folder it points to (which may be outside, or another part of this tree).
        if (directory.Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            Retry(() =>
            {
                if (directory.Attributes.HasFlag(FileAttributes.ReadOnly)) directory.Attributes &= ~FileAttributes.ReadOnly;
                directory.Delete();
            });
            return;
        }
        foreach (var entry in directory.EnumerateFileSystemInfos())
        {
            if (entry is DirectoryInfo subdirectory) DeleteDirectory(subdirectory);
            else Retry(() =>
            {
                if ((entry.Attributes & (FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System)) != 0) entry.Attributes = FileAttributes.Normal;
                entry.Delete();
            });
        }
        Retry(() =>
        {
            if (directory.Attributes.HasFlag(FileAttributes.ReadOnly)) directory.Attributes &= ~FileAttributes.ReadOnly;
            directory.Delete();
        });
    }

    static void Retry(Action delete)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                delete();
                return;
            }
            catch (DirectoryNotFoundException) { return; }
            catch (FileNotFoundException) { return; }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException && attempt < 10)
            {
                Thread.Sleep(200 * attempt);
            }
        }
    }
}
