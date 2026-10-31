using FrameworkOnCore.Analyzers;
using static FrameworkOnCore.Tests.AnalyzerHarness;
using static FrameworkOnCore.Tests.RewriteHarness;

namespace FrameworkOnCore.Tests;

/// <summary>
/// FOC1009: a catch that can catch Response.End's ThreadAbortException (a Response.End, Redirect or Server.Transfer in its
/// try), its code run in try / finally with System.Web.Util.ResponseEnd.RaiseAgain(): .NET Framework raised the abort again
/// at the end of the catch (NavigationProbe's redirect.tae, redirect.ex, transfer.catch-threadabort, vb.catch-when: the code
/// after the catch ran on .NET, its page rendered and its session written).
/// </summary>
public class EndInCatchTests
{
    // System.Web's members, as the application's build sees them.
    const string Web = "namespace System.Web {\n" +
        "  public class HttpResponse { public void End() { } public void Redirect(string url) { } public void Redirect(string url, bool endResponse) { } public void RedirectPermanent(string url) { } public void Write(string s) { } }\n" +
        "  public class HttpServerUtility { public void Transfer(string path) { } public void Execute(string path) { } }\n" +
        "  namespace Util { public static class ResponseEnd { public static void RaiseAgain() { } } }\n}\n";

    const string Page = "public class P\n{\n  System.Web.HttpResponse Response = new System.Web.HttpResponse();\n  System.Web.HttpServerUtility Server = new System.Web.HttpServerUtility();\n  static void Log(object o) { }\n";

    static List<(string Id, string At)> Found(string members) => Run(new EndInCatchAnalyzer(), Web + Page + members + "\n}");

    [Fact] // .NET Framework: a ThreadAbortException is caught by catch (ThreadAbortException), its bases' catches and a catch without a type
    public void The_catches_that_can_catch_the_abort_are_found()
    {
        var found = Found(
            "  void A() { try { Response.Redirect(\"a\"); } catch (System.Threading.ThreadAbortException) { Log(1); } }\n" +
            "  void B() { try { Response.End(); } catch (System.Exception ex) { Log(ex); } }\n" +
            "  void C() { try { Server.Transfer(\"c\"); } catch (System.SystemException) { } catch { Log(3); } }\n" +
            "  void D() { try { Response.RedirectPermanent(\"d\"); } catch (System.Exception ex) when (ex != null) { Log(ex); } }\n" +
            "  void E() { try { Response.Redirect(\"e\", true); } catch (System.ArgumentException) { } catch (System.Exception) { } }");
        Assert.Equal(6, found.Count);
        Assert.All(found, d => Assert.Equal(EndInCatchAnalyzer.Id, d.Id));
        Assert.StartsWith("catch (System.Threading.ThreadAbortException)", found[0].At);
        Assert.DoesNotContain(found, d => d.At.Contains("ArgumentException"));
    }

    [Fact] // the summary: Redirect(url, false), Server.Execute, a call in a lambda, a catch already rewritten: not an end in that try
    public void Catches_without_an_end_in_their_try_are_left() =>
        Assert.Empty(Found(
            "  void A() { try { Response.Redirect(\"a\", false); } catch (System.Exception) { } }\n" +
            "  void B() { try { Server.Execute(\"b\"); Response.Write(\"b\"); } catch (System.Exception) { } }\n" +
            "  void C() { try { System.Action a = () => Response.End(); } catch (System.Exception) { } }\n" +
            "  void D() { try { Response.End(); } catch (System.Exception) { try { } finally { global::System.Web.Util.ResponseEnd.RaiseAgain(); } } }"));

    [Fact]
    public void A_csharp_catch_raises_the_end_again_in_a_finally()
    {
        var written = CSharp(Web + Page +
            "  int A(string s)\n  {\n    try { Response.Redirect(s); }\n    catch (System.Threading.ThreadAbortException) { Log(1); return 1; }\n    Log(2);\n    return 2;\n  }\n" +
            "  void B()\n  {\n    try\n    {\n      Response.End();\n    }\n    catch (System.Exception ex)\n    {\n      Log(ex);\n      throw;\n    }\n  }\n}");
        Assert.Contains("catch (System.Threading.ThreadAbortException) { try { Log(1); return 1; } finally { global::System.Web.Util.ResponseEnd.RaiseAgain(); } }", written);
        Assert.Contains("catch (System.Exception ex)\n    { try {\n      Log(ex);\n      throw;\n    } finally { global::System.Web.Util.ResponseEnd.RaiseAgain(); } }", written);
        Assert.Empty(CompileCSharp(written));
        // Once: the build after the rewrite finds nothing more.
        Assert.Empty(Run(new EndInCatchAnalyzer(), written));
    }

    [Fact] // NavigationProbe's V/Handles.aspx: Catch ex As ThreadAbortException When filter()
    public void A_visual_basic_catch_raises_the_end_again_in_a_finally()
    {
        var web = "Namespace Global.System.Web\n" +
            "  Public Class HttpResponse\n    Public Sub Redirect(url As String)\n    End Sub\n  End Class\n" +
            "  Namespace Util\n    Public Module ResponseEnd\n      Public Sub RaiseAgain()\n      End Sub\n    End Module\n  End Namespace\n" +
            "End Namespace\n";
        var written = VisualBasic(web +
            "Public Class P\n  Private Response As New System.Web.HttpResponse()\n  Function Note(s As String) As Boolean\n    Return True\n  End Function\n" +
            "  Sub A()\n    Try\n      Response.Redirect(\"a\")\n    Catch ex As System.Threading.ThreadAbortException When Note(\"filter\")\n      Note(\"catch\")\n    End Try\n    Note(\"after\")\n  End Sub\n" +
            "End Class\n");
        Assert.Contains("    Catch ex As System.Threading.ThreadAbortException When Note(\"filter\")\n" +
                        "        Try\n      Note(\"catch\")\n        Finally\n            Global.System.Web.Util.ResponseEnd.RaiseAgain()\n        End Try\n" +
                        "    End Try", written.Replace("\r\n", "\n"));
        Assert.Empty(CompileVisualBasic(written));
    }
}
