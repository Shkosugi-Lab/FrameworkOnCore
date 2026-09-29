using static FrameworkOnCore.Tests.RewriteHarness;

namespace FrameworkOnCore.Tests;

/// <summary>
/// What the converter writes where the analyzers point (SourceEdits): the same rewrites in C# and Visual Basic, each
/// case from a corpus.
/// </summary>
public class RewriteTests
{
    const string CSharpHead = "using System; using System.IO;\npublic class C\n{\n";

    static string InClass(string members) => CSharpHead + members + "\n}";

    [Fact] // N2's TypeCache
    public void A_path_literal_is_wrapped() =>
        Assert.Contains("AppDomain.CurrentDomain.BaseDirectory + global::FrameworkOnCore.WindowsPath.Native(\"bin\\\\\")",
            CSharp(InClass("string Bin() => AppDomain.CurrentDomain.BaseDirectory + \"bin\\\\\";")));

    [Fact] // DNN: Replace('/', '\\')
    public void A_separator_character_becomes_the_platform_s() =>
        Assert.Contains("filePath.Replace('/', global::System.IO.Path.DirectorySeparatorChar)",
            CSharp(InClass("string F(string filePath) => filePath.Replace('/', '\\\\');")));

    [Fact] // DNN's Config.Save: the trim made relative, then the argument wrapped (two edits of one node)
    public void A_trim_before_a_combine_is_made_relative_and_wrapped()
    {
        var written = CSharp(InClass("string F(string root, string filename) => Path.Combine(root, filename.TrimStart('\\\\', '/'));"));
        Assert.Contains("Path.Combine(global::FrameworkOnCore.WindowsPath.Native(root), " +
                        "global::FrameworkOnCore.WindowsPath.Native(global::FrameworkOnCore.WindowsPath.TrimStartRelative(filename, '\\\\', '/')))", written);
    }

    [Fact] // DNN's Globals.glbConfigFolder
    public void A_constant_path_becomes_a_static_readonly_field() =>
        Assert.Contains("public static readonly string ConfigFolder = global::FrameworkOnCore.WindowsPath.Native(\"\\\\Config\\\\\");",
            CSharp(InClass("public const string ConfigFolder = \"\\\\Config\\\\\";")));

    [Fact] // DNN's Scheduler
    public void Begin_and_end_invoke_become_async_delegate_calls()
    {
        var written = CSharp(InClass(
            "delegate void Work(int item); delegate int Count();\n" +
            "void F(Work work, Count count) { work.BeginInvoke(1, null, null); var r = count.BeginInvoke(null, null); var n = count.EndInvoke(r); }"));
        Assert.Contains("global::FrameworkOnCore.AsyncDelegate.BeginInvoke(work, new object[] { 1 }, null, null);", written);
        Assert.Contains("((int)global::FrameworkOnCore.AsyncDelegate.EndInvoke(r))", written);
    }

    [Fact] // DNN's Scheduler, ClientResourceManager, ServerInfo; YAF's background task
    public void Platform_members_are_replaced()
    {
        var written = CSharp(InClass(
            "void F() => AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);\n" +
            "bool G(string s) => Uri.TryCreate(s, UriKind.Absolute, out _);\n" +
            "Uri H(string s) => new Uri(s, UriKind.RelativeOrAbsolute);\n" +
            "string I() => System.Security.Principal.WindowsIdentity.GetCurrent().Name;\n" +
            "object J() => System.Security.Principal.WindowsIdentity.GetCurrent();\n" +
            "string K() => AppDomain.CurrentDomain.RelativeSearchPath;\n" +
            "byte[] L(string s) => System.Text.Encoding.Default.GetBytes(s);\n" +
            "void M() { try { } catch (System.Threading.ThreadAbortException) { System.Threading.Thread.ResetAbort(); } }"));
        Assert.Contains("SetPrincipalPolicy(global::FrameworkOnCore.Platform.WindowsPrincipalPolicy)", written);
        Assert.Contains("global::FrameworkOnCore.WindowsUri.TryCreate(s, UriKind.Absolute, out _)", written);
        Assert.Contains("global::FrameworkOnCore.WindowsUri.Create(s, UriKind.RelativeOrAbsolute)", written);
        Assert.Contains("string I() => global::FrameworkOnCore.Platform.CurrentIdentityName;", written);
        Assert.Contains("object J() => global::FrameworkOnCore.Platform.CurrentWindowsIdentity;", written);
        Assert.Contains("string K() => global::FrameworkOnCore.Platform.RelativeSearchPath;", written);
        Assert.Contains("byte[] L(string s) => global::FrameworkOnCore.Platform.DefaultEncoding.GetBytes(s);", written);
        Assert.Contains("{ global::FrameworkOnCore.Platform.ResetAbort(); }", written);
    }

    [Fact] // openIMIS's log (every login): the event log; a call on an EventLog passes it first
    public void Event_log_calls_are_replaced_with_their_receiver()
    {
        var written = CSharp(InClass(
            "void F(System.Diagnostics.EventLog log) { System.Diagnostics.EventLog.WriteEntry(\"IMIS\", \"m\", System.Diagnostics.EventLogEntryType.Error, 1); log.WriteEntry(\"m\"); }"));
        Assert.Contains("global::FrameworkOnCore.EventLogs.WriteEntry(\"IMIS\", \"m\", System.Diagnostics.EventLogEntryType.Error, 1);", written);
        Assert.Contains("global::FrameworkOnCore.EventLogs.WriteEntry(log, \"m\");", written);
    }

    [Fact] // Uri.TryCreate(Uri, string, out Uri) has no Unix path problem: left
    public void Other_uri_overloads_are_left() =>
        Assert.Contains("Uri.TryCreate(b, s, out _)", CSharp(InClass("bool F(Uri b, string s) => Uri.TryCreate(b, s, out _);")));

    // ---- Visual Basic

    [Fact]
    public void Visual_basic_paths_are_wrapped_in_its_syntax()
    {
        var written = VisualBasic(
            "Imports System\nImports System.IO\nPublic Class C\n" +
            "  Public Const ConfigFolder As String = \"\\Config\\\"\n" +
            "  Function Bin() As String\n    Return AppDomain.CurrentDomain.BaseDirectory & \"bin\\\"\n  End Function\n" +
            "  Function Exists(p As String) As Boolean\n    Return File.Exists(p)\n  End Function\n" +
            "End Class");
        Assert.Contains("Public Shared ReadOnly ConfigFolder As String = Global.FrameworkOnCore.WindowsPath.Native(\"\\Config\\\")", written);
        Assert.Contains("AppDomain.CurrentDomain.BaseDirectory & Global.FrameworkOnCore.WindowsPath.Native(\"bin\\\")", written);
        Assert.Contains("File.Exists(Global.FrameworkOnCore.WindowsPath.Native(p))", written);
    }

    [Fact]
    public void Visual_basic_delegates_and_platform_members_in_its_syntax()
    {
        var written = VisualBasic(
            "Imports System\nPublic Class C\n  Delegate Sub Work(item As Integer)\n  Delegate Function Count() As Integer\n" +
            "  Sub F(work As Work, count As Count)\n    work.BeginInvoke(1, Nothing, Nothing)\n    Dim r = count.BeginInvoke(Nothing, Nothing)\n    Dim n = count.EndInvoke(r)\n  End Sub\n" +
            "  Function G(s As String) As Boolean\n    Dim u As Uri = Nothing\n    Return Uri.TryCreate(s, UriKind.Absolute, u)\n  End Function\n" +
            "  Function H(s As String) As Byte()\n    Return System.Text.Encoding.Default.GetBytes(s)\n  End Function\n" +
            "  Sub I()\n    Try\n    Catch ex As System.Threading.ThreadAbortException\n      System.Threading.Thread.ResetAbort()\n    End Try\n  End Sub\n" +
            "End Class");
        Assert.Contains("Global.FrameworkOnCore.AsyncDelegate.BeginInvoke(work, New Object() {1}, Nothing, Nothing)", written);
        Assert.Contains("CType(Global.FrameworkOnCore.AsyncDelegate.EndInvoke(r), Integer)", written);
        Assert.Contains("Global.FrameworkOnCore.WindowsUri.TryCreate(s, UriKind.Absolute, u)", written);
        Assert.Contains("Return Global.FrameworkOnCore.Platform.DefaultEncoding.GetBytes(s)", written);
        Assert.Contains("Global.FrameworkOnCore.Platform.ResetAbort()", written);
    }

    [Fact] // openIMIS's IMIS_Gen.Log
    public void Visual_basic_event_log_calls_are_replaced()
    {
        var written = VisualBasic(
            "Imports System.Diagnostics\nPublic Class C\n  Sub F(log As EventLog)\n    EventLog.WriteEntry(\"IMIS\", \"m\", EventLogEntryType.Information, 1)\n    log.WriteEntry(\"m\")\n  End Sub\nEnd Class");
        Assert.Contains("Global.FrameworkOnCore.EventLogs.WriteEntry(\"IMIS\", \"m\", EventLogEntryType.Information, 1)", written);
        Assert.Contains("Global.FrameworkOnCore.EventLogs.WriteEntry(log, \"m\")", written);
    }
}
