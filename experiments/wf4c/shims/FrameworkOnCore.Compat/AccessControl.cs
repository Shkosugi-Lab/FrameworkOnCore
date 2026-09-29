using System;
using System.IO;
using System.Security.AccessControl;

// Windows' access control lists: called on Windows only.
#pragma warning disable CA1416

namespace FrameworkOnCore;

/// <summary>
/// .NET Framework's Directory.GetAccessControl and SetAccessControl (static, by path): .NET has them as extensions of
/// DirectoryInfo (FileSystemAclExtensions), which a call by path does not find (CS1929). The converter calls these instead
/// (rules/packages.json memberReplacements). Windows only, as on .NET: elsewhere PlatformNotSupportedException (nopCommerce's
/// installer checks folder permissions this way, and takes an exception as "allowed").
/// </summary>
public static class DirectoryAcl
{
    public static DirectorySecurity GetAccessControl(string path) =>
        OperatingSystem.IsWindows() ? new DirectoryInfo(path).GetAccessControl() : throw new PlatformNotSupportedException("access control lists are Windows'");

    public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections) =>
        OperatingSystem.IsWindows() ? new DirectoryInfo(path).GetAccessControl(includeSections) : throw new PlatformNotSupportedException("access control lists are Windows'");

    public static void SetAccessControl(string path, DirectorySecurity directorySecurity)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("access control lists are Windows'");
        new DirectoryInfo(path).SetAccessControl(directorySecurity);
    }
}

/// <summary>.NET Framework's File.GetAccessControl and SetAccessControl (by path), as DirectoryAcl.</summary>
public static class FileAcl
{
    public static FileSecurity GetAccessControl(string path) =>
        OperatingSystem.IsWindows() ? new FileInfo(path).GetAccessControl() : throw new PlatformNotSupportedException("access control lists are Windows'");

    public static FileSecurity GetAccessControl(string path, AccessControlSections includeSections) =>
        OperatingSystem.IsWindows() ? new FileInfo(path).GetAccessControl(includeSections) : throw new PlatformNotSupportedException("access control lists are Windows'");

    public static void SetAccessControl(string path, FileSecurity fileSecurity)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("access control lists are Windows'");
        new FileInfo(path).SetAccessControl(fileSecurity);
    }
}
