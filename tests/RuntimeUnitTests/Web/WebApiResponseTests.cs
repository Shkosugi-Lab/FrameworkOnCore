using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using FrameworkOnCore.Runtime.Tests.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// Web API's web host (System.Web.Http.WebHost, AspNetWebStack's port): a response message copied to System.Web's response
/// (HttpControllerHandler.CopyResponseAsync). On .NET 5 and later HttpResponseMessage.Content is never null (an empty content
/// when none is set: learn.microsoft.com/dotnet/core/compatibility/networking/5.0/httpresponsemessage-content-not-null); Web
/// API's tests of a null content (no body: no Content-Type) take it as none (ResponseContent). BlogEngine.NET's API, on IIS
/// (recorded: corpora/parity/be.operations.golden.json): a 401 of its authorization filter, a 404, a 200 without content -
/// "Content-Length: 0" and no Content-Type; the port sent "Content-Type: text/html".
/// </summary>
[Collection("Application")]
public class WebApiResponseTests
{
    static (HttpContext Web, Microsoft.AspNetCore.Http.DefaultHttpContext Core) Copy(HttpResponseMessage message)
    {
        var (web, core) = Site.WebRequest("/api/users");
        web.Request.ContentEncoding = System.Text.Encoding.UTF8;
        var copy = typeof(System.Web.Http.WebHost.HttpControllerHandler).GetMethod("CopyResponseAsync", BindingFlags.NonPublic | BindingFlags.Static);
        ((Task)copy.Invoke(null, new object[] { new HttpContextWrapper(web), new HttpRequestMessage(), message, null, null, CancellationToken.None })).GetAwaiter().GetResult();
        web.Response.FinalFlushAtTheEndOfRequestProcessing();
        return (web, core);
    }

    [Theory] // IIS: an answer without content - its status, Content-Length 0, no Content-Type
    [InlineData(HttpStatusCode.Forbidden)] // (a 401 reads appSettings for its forms authentication redirect: no configuration system here)
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.OK)]
    public void A_response_without_content_has_no_content_type(HttpStatusCode status)
    {
        using var site = new Site().Run(watch: false);
        var message = new HttpResponseMessage(status);
        // .NET's (not .NET Framework's): an empty content in the place of none.
        Assert.NotNull(message.Content);
        var (_, core) = Copy(message);
        Assert.Equal((int)status, core.Response.StatusCode);
        Assert.Null(core.Response.ContentType);
        Assert.Equal("", Site.Body(core));
    }

    [Theory] // Web API 5.2.3 (BlogEngine.NET's System.Net.Http.Formatting, observed on .NET Framework: its Content null): Request.CreateResponse(status) and
             // CreateResponse() have no content (AspNetWebStack 3.3's gave a StreamContent of nothing: a 404 of BlogEngine's API with "Content-Type: text/html")
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.OK)]
    public void Request_CreateResponse_has_no_content(HttpStatusCode status)
    {
        using var site = new Site().Run(watch: false);
        var created = new HttpRequestMessage().CreateResponse(status);
        Assert.Equal(status, created.StatusCode);
        Assert.Equal("System.Net.Http.EmptyContent", created.Content.GetType().FullName);
        Assert.Equal("System.Net.Http.EmptyContent", new HttpRequestMessage().CreateResponse().Content.GetType().FullName);
        var (_, core) = Copy(created);
        Assert.Null(core.Response.ContentType);
        Assert.Equal("", Site.Body(core));
    }

    [Fact] // the summary: a content of its own (even of no bytes) is a content: its type sent
    public void A_response_with_content_has_its_type()
    {
        using var site = new Site().Run(watch: false);
        var (_, core) = Copy(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]", System.Text.Encoding.UTF8, "application/json") });
        Assert.Equal("application/json; charset=utf-8", core.Response.ContentType);
        Assert.Equal("[]", Site.Body(core));
        var (_, empty) = Copy(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("", System.Text.Encoding.UTF8, "text/plain") });
        Assert.Equal("text/plain; charset=utf-8", empty.Response.ContentType);
    }
}
