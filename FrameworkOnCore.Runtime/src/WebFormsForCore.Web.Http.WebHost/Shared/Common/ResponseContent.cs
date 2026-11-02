// FrameworkOnCore: not AspNetWebStack's (the same file in System.Web.Http, System.Web.Http.WebHost and System.Web.Http.Owin).

using System.Net.Http;

namespace System.Web.Http
{
    /// <summary>
    /// A response's content as Web API tests it. On .NET Framework a response without content had a null Content; on .NET 5
    /// and later HttpResponseMessage.Content is never null (an empty content, System.Net.Http.EmptyContent, when none is set:
    /// learn.microsoft.com/dotnet/core/compatibility/networking/5.0/httpresponsemessage-content-not-null). Web API's
    /// "Content == null" (no body: Content-Length 0, no Content-Type) became a body of none, with its headers: every empty
    /// answer (a 401 of an authorization filter, a 404, a 200 of an action without content) had "Content-Type: text/html"
    /// (BlogEngine.NET's API; IIS sends none).
    /// </summary>
    internal static class ResponseContent
    {
        static readonly Type EmptyContentType = typeof(HttpContent).Assembly.GetType("System.Net.Http.EmptyContent");

        /// <summary>Whether a response has no content: none, or the empty content .NET puts in its place.</summary>
        internal static bool IsNone(HttpContent content) => content == null || content.GetType() == EmptyContentType;
    }
}
