using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PipelineProbe
{
    /// <summary>
    /// The request's events as the application sees them, in order (Probe.Log: the request's list, HttpContext.Items), the
    /// module's (ProbeModule, web.config) among them; the events after the handler are read on the next request (Probe.Previous).
    /// Application_Error: the error's exceptions kept for the error page; for ?clear=1 the error handled here.
    /// </summary>
    public class Global : HttpApplication
    {
        static bool started;

        protected void Application_Start(object sender, EventArgs e) => started = true;

        public static bool Started => started;

        void Log(string name) => Probe.Log(Context, "Global." + name);

        protected void Application_BeginRequest(object sender, EventArgs e) => Log("BeginRequest");
        protected void Application_AuthenticateRequest(object sender, EventArgs e) => Log("AuthenticateRequest");
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e) => Log("PostAuthenticateRequest");
        protected void Application_AuthorizeRequest(object sender, EventArgs e) => Log("AuthorizeRequest");
        protected void Application_PostAuthorizeRequest(object sender, EventArgs e) => Log("PostAuthorizeRequest");
        protected void Application_ResolveRequestCache(object sender, EventArgs e) => Log("ResolveRequestCache");
        protected void Application_PostResolveRequestCache(object sender, EventArgs e) => Log("PostResolveRequestCache");
        protected void Application_MapRequestHandler(object sender, EventArgs e) => Log("MapRequestHandler");
        protected void Application_PostMapRequestHandler(object sender, EventArgs e) => Log("PostMapRequestHandler");
        protected void Application_AcquireRequestState(object sender, EventArgs e) => Log("AcquireRequestState");
        protected void Application_PostAcquireRequestState(object sender, EventArgs e) => Log("PostAcquireRequestState");
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e) => Log("PreRequestHandlerExecute");
        protected void Application_PostRequestHandlerExecute(object sender, EventArgs e) => Log("PostRequestHandlerExecute");
        protected void Application_ReleaseRequestState(object sender, EventArgs e) => Log("ReleaseRequestState");
        protected void Application_PostReleaseRequestState(object sender, EventArgs e) => Log("PostReleaseRequestState");
        protected void Application_UpdateRequestCache(object sender, EventArgs e) => Log("UpdateRequestCache");
        protected void Application_PostUpdateRequestCache(object sender, EventArgs e) => Log("PostUpdateRequestCache");
        protected void Application_LogRequest(object sender, EventArgs e) => Log("LogRequest");
        protected void Application_PostLogRequest(object sender, EventArgs e) => Log("PostLogRequest");
        protected void Application_EndRequest(object sender, EventArgs e) => Log("EndRequest");

        protected void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            Probe.LastError = string.Join(" / ", Probe.Chain(error));
            Log("Error(" + error.GetType().Name + ")");
            if (Request.QueryString["clear"] == "1")
            {
                Server.ClearError();
                Response.Clear();
                Response.StatusCode = 200;
                Response.ContentType = "text/html";
                Response.Write("<!DOCTYPE html><html><head><title>Pipeline probe: handled</title></head><body><p id=\"handled\">Handled by Application_Error: " +
                    HttpUtility.HtmlEncode(Probe.LastError) + "</p></body></html>");
            }
        }
    }

    /// <summary>The request's events (HttpContext.Items) and those of the last request to each path.</summary>
    public static class Probe
    {
        static readonly Dictionary<string, List<string>> last = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, List<string>> previous = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        public static string LastError { get; set; }

        public static List<string> Events(HttpContext context)
        {
            var events = context.Items["probe-events"] as List<string>;
            if (events == null)
            {
                context.Items["probe-events"] = events = new List<string>();
                var path = context.Request.Path;
                lock (last)
                {
                    List<string> before;
                    if (last.TryGetValue(path, out before)) previous[path] = before;
                    last[path] = events;
                }
            }
            return events;
        }

        public static void Log(HttpContext context, string name)
        {
            var events = Events(context);
            lock (events) events.Add(name);
        }

        /// <summary>The events of the request before this one to the path (its list went on after the handler).</summary>
        public static List<string> Previous(string path)
        {
            List<string> events;
            lock (last) previous.TryGetValue(path, out events);
            if (events == null) return new List<string>();
            lock (events) return events.ToList();
        }

        public static IEnumerable<string> Chain(Exception error)
        {
            for (var ex = error; ex != null; ex = ex.InnerException)
                yield return ex.GetType().FullName + ": " + ex.Message.Split('\n')[0].Trim();
        }
    }
}
