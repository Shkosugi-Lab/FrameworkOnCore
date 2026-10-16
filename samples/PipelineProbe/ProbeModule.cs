using System;
using System.Threading.Tasks;
using System.Web;

namespace PipelineProbe
{
    /// <summary>A module of web.config's (system.webServer/modules): its events among the application's, a header of its own.</summary>
    public class ProbeModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.BeginRequest += (s, e) =>
            {
                Probe.Log(application.Context, "Module.BeginRequest");
                application.Response.AppendHeader("X-Probe-Module", "begin");
            };
            application.AuthenticateRequest += (s, e) => Probe.Log(application.Context, "Module.AuthenticateRequest");
            application.PostMapRequestHandler += (s, e) =>
                Probe.Log(application.Context, "Module.PostMapRequestHandler(" + (application.Context.Handler == null ? "none" : application.Context.Handler.GetType().Name) + ")");
            application.PreRequestHandlerExecute += (s, e) => Probe.Log(application.Context, "Module.PreRequestHandlerExecute");
            application.EndRequest += (s, e) => Probe.Log(application.Context, "Module.EndRequest(" + application.Response.StatusCode + ")");
        }

        public void Dispose()
        {
        }
    }

    /// <summary>A handler of web.config's (*.probe): what it is asked and what ran before it.</summary>
    public class ProbeHandler : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            Probe.Log(context, "ProbeHandler.ProcessRequest");
            context.Response.ContentType = "text/html";
            context.Response.Write("<!DOCTYPE html><html><head><title>Pipeline probe: handler</title></head><body><pre id=\"log\">" + HttpUtility.HtmlEncode(
                "Path=" + context.Request.Path + " FilePath=" + context.Request.FilePath + " PathInfo=" + context.Request.PathInfo + " Query=" + context.Request.Url.Query +
                "\nHandler=" + context.Handler.GetType().FullName + " Method=" + context.Request.HttpMethod +
                "\nEvents: " + string.Join(", ", Probe.Events(context))) + "</pre></body></html>");
        }
    }

    /// <summary>An asynchronous handler (HttpTaskAsyncHandler, *.aprobe).</summary>
    public class AsyncProbeHandler : HttpTaskAsyncHandler
    {
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            Probe.Log(context, "AsyncProbeHandler.before");
            await Task.Delay(20);
            Probe.Log(context, "AsyncProbeHandler.after");
            context.Response.ContentType = "text/html";
            context.Response.Write("<!DOCTYPE html><html><head><title>Pipeline probe: async handler</title></head><body><pre id=\"log\">" + HttpUtility.HtmlEncode(
                "Path=" + context.Request.Path + " HttpContext.Current set " + (HttpContext.Current != null) +
                "\nEvents: " + string.Join(", ", Probe.Events(context))) + "</pre></body></html>");
        }
    }
}
