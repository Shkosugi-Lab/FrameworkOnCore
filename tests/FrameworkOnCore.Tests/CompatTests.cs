namespace FrameworkOnCore.Tests;

/// <summary>
/// The compatibility assembly's helpers the rewritten code calls: on Windows what the source did; elsewhere the Windows
/// behaviour made the platform's. Run on both (run-tests-linux.ps1 for Linux).
/// </summary>
public class CompatTests
{
    static readonly string root = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);

    [Fact]
    public void Native_makes_windows_separators_the_platform_s()
    {
        if (OperatingSystem.IsWindows()) Assert.Equal("App_Data\\x.config", WindowsPath.Native("App_Data\\x.config"));
        else Assert.Equal("App_Data/x.config", WindowsPath.Native("App_Data\\x.config"));
        Assert.Null(WindowsPath.Native(null!));
    }

    [Fact] // DNN's manifests name "resource-skin.zip", the package has "Resource-Skin.zip"
    public void Native_finds_a_name_in_another_case_in_the_application_s_folder()
    {
        var folder = Path.Combine(root, "CaseProbe");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "Resource-Skin.zip"), "");
        var asked = root + "\\caseprobe\\resource-skin.zip";
        if (OperatingSystem.IsWindows()) Assert.Equal(asked, WindowsPath.Native(asked));
        else Assert.Equal(Path.Combine(folder, "Resource-Skin.zip"), WindowsPath.Native(asked));
        // A file not there (to create): the folders found, the rest as written.
        if (!OperatingSystem.IsWindows()) Assert.Equal(Path.Combine(folder, "New.txt"), WindowsPath.Native(root + "\\CASEPROBE\\New.txt"));
    }

    [Fact] // DNN's Config.Save: Path.Combine(root, filename.TrimStart('\\', '/'))
    public void Trim_start_relative_keeps_an_absolute_path_in_the_application()
    {
        Assert.Equal("web.config", WindowsPath.TrimStartRelative("/web.config", '/', '\\'));
        var absolute = Path.Combine(root, "Config", "web_.config");
        if (OperatingSystem.IsWindows()) Assert.Equal(absolute, WindowsPath.TrimStartRelative(absolute, '/', '\\'));
        else Assert.Equal(absolute, WindowsPath.TrimStartRelative(absolute, '/', '\\'));
        Assert.Equal("a", WindowsPath.TrimRelative("/a/", '/'));
    }

    [Fact] // .NET on Unix takes "/Portals/0/home.css" for an absolute file URI; Windows does not
    public void A_path_from_the_root_is_a_relative_uri_on_every_platform()
    {
        Assert.False(WindowsUri.TryCreate("/Portals/0/home.css", UriKind.Absolute, out _));
        Assert.True(WindowsUri.TryCreate("/Portals/0/home.css", UriKind.RelativeOrAbsolute, out var relative));
        Assert.False(relative!.IsAbsoluteUri);
        Assert.False(WindowsUri.Create("/Login", UriKind.RelativeOrAbsolute).IsAbsoluteUri);
        Assert.True(WindowsUri.TryCreate("https://example.com/x", UriKind.Absolute, out var absolute));
        Assert.True(absolute!.IsAbsoluteUri);
        Assert.True(WindowsUri.TryCreate("//cdn.example.com/x.js", UriKind.RelativeOrAbsolute, out _));
        Assert.Throws<UriFormatException>(() => WindowsUri.Create("/Login", UriKind.Absolute));
    }

    [Fact]
    public void The_windows_principal_policy_and_identity_elsewhere()
    {
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal(System.Security.Principal.PrincipalPolicy.WindowsPrincipal, Platform.WindowsPrincipalPolicy);
            Assert.NotNull(Platform.CurrentWindowsIdentity);
        }
        else
        {
            Assert.Equal(System.Security.Principal.PrincipalPolicy.UnauthenticatedPrincipal, Platform.WindowsPrincipalPolicy);
            Assert.Null(Platform.CurrentWindowsIdentity);
            Assert.Equal(Environment.UserName, Platform.CurrentIdentityName);
        }
    }

    [Fact] // YAF's ModuleScanner: BaseDirectory + RelativeSearchPath
    public void Relative_search_path_is_bin_where_there_is_one()
    {
        // The test's folder has no bin: .NET's (null).
        Assert.Null(Platform.RelativeSearchPath);
    }

    [Fact] // DNN's Scheduler: delegateFunc.BeginInvoke(item, null, null)
    public void Begin_invoke_runs_the_delegate_with_the_call_s_arguments()
    {
        var done = new ManualResetEventSlim();
        Func<int, int> twice = x => x * 2;
        object? state = null;
        var result = AsyncDelegate.BeginInvoke(twice, new object[] { 21 }, r => { state = r.AsyncState; done.Set(); }, "s");
        Assert.True(done.Wait(5000));
        Assert.Equal(42, (int)AsyncDelegate.EndInvoke(result));
        Assert.Equal("s", state);

        Action<int> fail = _ => throw new InvalidOperationException("x");
        var failed = AsyncDelegate.BeginInvoke(fail, new object[] { 1 }, null, null);
        Assert.Throws<InvalidOperationException>(() => AsyncDelegate.EndInvoke(failed));
    }
}
