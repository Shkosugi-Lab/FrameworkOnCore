using System.Globalization;

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

    [Fact] // BlogEngine.NET's trackback without a url: new Uri("/trackback.axd") threw on Windows, was a file URI on Linux
    public void An_absolute_uri_of_a_path_from_the_root_throws_on_every_platform()
    {
        Assert.Throws<UriFormatException>(() => WindowsUri.Create("/trackback.axd"));
        Assert.Throws<UriFormatException>(() => WindowsUri.Create(""));
        Assert.Equal("http://example.com/x", WindowsUri.Create("http://example.com/x").AbsoluteUri);
        Assert.Equal("http://cdn.example.com/x.js", WindowsUri.Create("http://cdn.example.com/x.js").ToString());
    }

    [Fact] // BlogEngine.NET's admin pages: new CultureInfo("Auto") threw on .NET Framework and Windows; ICU made a culture of it
    public void A_culture_name_net_framework_does_not_know_throws_on_every_platform()
    {
        Assert.Throws<CultureNotFoundException>(() => FrameworkCultures.Create("Auto"));
        Assert.Throws<CultureNotFoundException>(() => FrameworkCultures.Create("auto", false));
        Assert.Throws<CultureNotFoundException>(() => FrameworkCultures.GetCultureInfo("en-ZZZ"));
        Assert.Throws<CultureNotFoundException>(() => FrameworkCultures.CreateSpecificCulture("Auto"));
        Assert.Equal("ja-JP", FrameworkCultures.Create("ja-JP").Name);
        Assert.Equal("de-DE", FrameworkCultures.CreateSpecificCulture("de").Name);
        Assert.Equal("", FrameworkCultures.Create("").Name);
        // Names .NET Framework knows that ICU has not (Windows' NLS): taken, as before.
        Assert.Equal("zh-CHS", FrameworkCultures.Create("zh-CHS").Name, ignoreCase: true);
        Assert.Equal("en-029", FrameworkCultures.GetCultureInfo("en-029").Name, ignoreCase: true);
        Assert.Throws<ArgumentNullException>(() => FrameworkCultures.Create(null!));
    }

    // .NET Framework 4.8 on Windows 11 (Windows PowerShell 5.1, New-Object System.Globalization.CultureInfo(name), 2026-11-02):
    // the names it refused and the ones it made a culture of ("Unknown Locale" for those Windows has no data for).
    [Theory]
    [InlineData("Auto")] [InlineData("abcd")] [InlineData("abcde")] [InlineData("root")] [InlineData("a")] [InlineData("e1")]
    [InlineData("Auto-XX")] [InlineData("auto:ja")] [InlineData("en-Z")] [InlineData("en-ZZZ")] [InlineData("en-ZZZZZ")]
    [InlineData("en-1")] [InlineData("en-12")] [InlineData("en-1234")] [InlineData("en-1a")] [InlineData("en-US-123456789")]
    [InlineData("en-Latn-ZZZ")] [InlineData("en-Latn-12")] [InlineData("en-Latn-Latn")] [InlineData("x")] [InlineData("i")]
    [InlineData("en-x")] [InlineData("zh-yue")] [InlineData("en-")] [InlineData("-en")] [InlineData("en--US")]
    [InlineData("en_US_POSIX")] [InlineData("en.US")] [InlineData("en@x")]
    public void A_name_net_framework_refuses_throws(string name) =>
        Assert.Throws<CultureNotFoundException>(() => FrameworkCultures.Create(name));

    [Theory]
    [InlineData("xx-YY")] [InlineData("en-ZZ")] [InlineData("zz")] [InlineData("xyz")] [InlineData("en-XA")] [InlineData("en-Latn-US")]
    [InlineData("en-419")] [InlineData("en-123")] [InlineData("en-ZZZZ")] [InlineData("en-US-posix")] [InlineData("en-US-abcde")]
    [InlineData("qaa-Qaaa-QM")] [InlineData("EN-us")] [InlineData("ja-JP-u-ca-japanese")] [InlineData("en-x-a")] [InlineData("x-a")]
    [InlineData("en-US-a")] [InlineData("ja-JP-mac")] [InlineData("en-ZZ-ZZ")] [InlineData("sgn-BE-FR")] [InlineData("de-DE_phoneb")]
    public void A_name_net_framework_makes_a_culture_of_is_taken(string name) =>
        Assert.True(FrameworkCultures.WindowsLocaleName(name), name);

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

    [Fact] // nopCommerce's payment hashes, n2's log files, BlogEngine's pingbacks: Encoding.Default
    public void The_default_encoding_is_the_ansi_code_page_as_on_net_framework()
    {
        var encoding = Platform.DefaultEncoding;
        // Windows': the system's (GetACP); elsewhere the culture's (LANG).
        if (!OperatingSystem.IsWindows()) Assert.Equal(System.Globalization.CultureInfo.InstalledUICulture.TextInfo.ANSICodePage, encoding.CodePage);
        // A code page .NET has only from the provider (the one registered with it).
        Assert.Equal("shift_jis", System.Text.Encoding.GetEncoding(932).WebName);
    }

    [Fact] // DNN's URL rewriter: catch (ThreadAbortException) { Thread.ResetAbort(); } after a redirect
    public void Reset_abort_without_a_request_does_nothing()
    {
        // No WebFormsForCore here (no System.Web): nothing to cancel, and no PlatformNotSupportedException.
        Platform.ResetAbort();
    }

    [Fact] // DNN's Scheduler: delegateFunc.BeginInvoke(item, null, null)
    public void Begin_invoke_runs_the_delegate_with_the_call_s_arguments()
    {
        var done = new ManualResetEventSlim();
        Func<int, int> twice = x => x * 2;
        object? state = null;
        var result = AsyncDelegate.BeginInvoke(twice, new object[] { 21 }, r => { state = r.AsyncState; done.Set(); }, "s");
        Assert.True(done.Wait(30000));   // the thread pool's, busy while the other tests run
        Assert.Equal(42, (int)AsyncDelegate.EndInvoke(result));
        Assert.Equal("s", state);

        Action<int> fail = _ => throw new InvalidOperationException("x");
        var failed = AsyncDelegate.BeginInvoke(fail, new object[] { 1 }, null, null);
        Assert.Throws<InvalidOperationException>(() => AsyncDelegate.EndInvoke(failed));
    }

    [Fact] // openIMIS logs every login to the event log: elsewhere a line on the standard error
    public void Event_log_entries_go_to_the_standard_error_elsewhere()
    {
        if (OperatingSystem.IsWindows()) return;   // the event log itself (writing to it needs a registered source)
        var before = Console.Error;
        var written = new StringWriter();
        Console.SetError(written);
        try { EventLogs.WriteEntry("IMIS", "Admin has logged in.", System.Diagnostics.EventLogEntryType.Information, 1); }
        finally { Console.SetError(before); }
        Assert.Equal("IMIS: Information 1: Admin has logged in.", written.ToString().Trim());
        Assert.True(EventLogs.SourceExists("IMIS"));
        EventLogs.CreateEventSource("IMIS", "Application");
    }

    [Fact] // Visual Basic's My (the compiler's ThreadSafeObjectProvider): a value per thread outside a request
    public void Context_value_is_per_thread_outside_a_request()
    {
        var value = new Microsoft.VisualBasic.MyServices.Internal.ContextValue<string> { Value = "here" };
        string? other = "unset";
        var thread = new Thread(() => other = value.Value);
        thread.Start();
        thread.Join();
        Assert.Equal("here", value.Value);
        Assert.Null(other);
    }

    [Fact] // My.Computer.FileSystem (openIMIS); FindInFiles is not in .NET's FileSystem
    public void My_computer_file_system_finds_in_files()
    {
        var folder = Path.Combine(root, "FindProbe");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "a.txt"), "Hello World");
        File.WriteAllText(Path.Combine(folder, "b.txt"), "nothing");
        var fileSystem = new Microsoft.VisualBasic.Devices.ServerComputer().FileSystem;
        var found = fileSystem.FindInFiles(folder, "hello", true, Microsoft.VisualBasic.FileIO.SearchOption.SearchTopLevelOnly, "*.txt");
        Assert.Equal(new[] { Path.Combine(folder, "a.txt") }, found);
        Assert.True(fileSystem.DirectoryExists(folder));
    }

    [Fact] // My.User outside a request: the thread's principal
    public void Web_user_outside_a_request_is_the_thread_s()
    {
        var before = Thread.CurrentPrincipal;
        try
        {
            Thread.CurrentPrincipal = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity("admin"), new[] { "Admins" });
            var user = new Microsoft.VisualBasic.ApplicationServices.WebUser();
            Assert.Equal("admin", user.Name);
            Assert.True(user.IsAuthenticated);
            Assert.True(user.IsInRole("Admins"));
        }
        finally { Thread.CurrentPrincipal = before; }
    }
}