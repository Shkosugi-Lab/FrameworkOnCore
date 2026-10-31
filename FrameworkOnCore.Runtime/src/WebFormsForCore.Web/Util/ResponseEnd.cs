using System;
using System.ComponentModel;

namespace System.Web.Util
{
    /// <summary>
    /// FrameworkOnCore: what the end of a catch that can catch Response.End's ThreadAbortException calls (FOC1009,
    /// FrameworkOnCore.Analyzers' EndInCatchAnalyzer: the converter rewrites the application's code, System.Web what the
    /// server compiles). .NET Framework raised a thread abort again at the end of each catch, until Thread.ResetAbort:
    /// code that swallowed a Response.End, a Response.Redirect or a Server.Transfer left its method there. .NET has no thread
    /// abort: Response.End throws its ThreadAbortException once (HttpResponse), and here, at the end of the catch, again while
    /// the end is pending. Nothing when none is (another exception; ResetAbort called: FrameworkOnCore.Platform.ResetAbort),
    /// or outside a request. Public: the rewritten code calls it.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ResponseEnd
    {
        public static void RaiseAgain()
        {
            var context = HttpContext.Current;
            if (context == null) return;
            HttpResponse response;
            // HttpContext.Response throws where the request has none (the application's start).
            try { response = context.Response; }
            catch (HttpException) { return; }
            response?.RethrowIfResponseEnd();
        }
    }
}
