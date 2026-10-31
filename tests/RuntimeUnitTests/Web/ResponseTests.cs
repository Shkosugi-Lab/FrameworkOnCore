using System.Threading;
using System.Web;
using FrameworkOnCore.Runtime.Tests.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// HttpResponse's port: Response.Headers without IIS (the collection integrated mode has, backed by the response's header
/// lists), Response.End without Thread.Abort, a request past its timeout, a redirect's URL encoded.
/// </summary>
[Collection("Application")]
public class ResponseTests
{
    static (HttpContext Web, Microsoft.AspNetCore.Http.DefaultHttpContext Core) Request(string target = "/page.aspx")
    {
        var (web, core) = Site.WebRequest(target);
        web.Request.Browser = new HttpBrowserCapabilities { Capabilities = new System.Collections.Hashtable() };
        // <globalization requestEncoding>'s default (no configuration system here).
        web.Request.ContentEncoding = System.Text.Encoding.UTF8;
        return (web, core);
    }

    /// <summary>A request through the site's host (its server variables need it: Site.WebRequest's has none).</summary>
    static (HttpContext Web, Microsoft.AspNetCore.Http.DefaultHttpContext Core) Hosted(Site site)
    {
        var core = Site.Request("/page.aspx");
        var worker = new System.Web.Hosting.AspNetCoreWorkerRequest(site.Host(), core);
        typeof(System.Web.Hosting.AspNetCoreWorkerRequest).GetMethod("ParseHeaders", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(worker, null);
        var web = new HttpContext(worker);
        web.Request.ContentEncoding = System.Text.Encoding.UTF8;
        return (web, core);
    }

    static string[] Written(HttpResponse response) =>
        response.GenerateResponseHeaders(false).Cast<HttpResponseHeader>().Select(h => h.Name + ": " + h.Value).ToArray();

    [Fact] // HttpResponse.Headers' documentation (integrated mode): the response's headers; AppendHeader's, ContentType's, RedirectLocation's in it, before and after it is read
    public void The_headers_collection_has_the_response_s_headers()
    {
        using var site = new Site().Run(watch: false);
        var response = Request().Web.Response;
        response.AppendHeader("X-One", "1");
        response.ContentType = "text/plain";
        response.RedirectLocation = "/there";
        var headers = response.Headers;
        Assert.Equal("1", headers["X-One"]);
        Assert.Equal("text/plain", headers["Content-Type"]);
        Assert.Equal("/there", headers["Location"]);
        response.AppendHeader("X-Two", "2");
        Assert.Equal("2", headers["X-Two"]);
        Assert.Same(headers, response.Headers);
    }

    [Fact] // HttpResponse.Headers' documentation (integrated mode): Add appends, Set replaces, Remove removes: what is written
    public void The_headers_collection_writes_the_response_s_headers()
    {
        using var site = new Site().Run(watch: false);
        var response = Request().Web.Response;
        var headers = response.Headers;
        headers.Add("X-A", "1");
        headers.Add("X-A", "2");
        headers.Set("X-B", "1");
        headers.Set("X-B", "2");
        headers.Add("X-C", "1");
        headers.Remove("x-c");
        response.Cache.SetCacheability(HttpCacheability.Private);
        var written = Written(response);
        Assert.Equal(new[] { "X-A: 1", "X-A: 2" }, written.Where(h => h.StartsWith("X-A")).ToArray());
        Assert.Equal(new[] { "X-B: 2" }, written.Where(h => h.StartsWith("X-B")).ToArray());
        Assert.DoesNotContain(written, h => h.StartsWith("X-C"));
        Assert.Equal("1,2", headers["X-A"]);
        Assert.Equal("2", headers["X-B"]);
        Assert.Null(headers["X-C"]);
        Assert.Throws<ArgumentNullException>(() => headers.Add(null, "v"));
        Assert.Throws<ArgumentNullException>(() => headers.Add("n", null));
        Assert.Throws<ArgumentNullException>(() => headers.Set(null, "v"));
        Assert.Throws<ArgumentNullException>(() => headers.Set("n", null));
        Assert.Throws<ArgumentNullException>(() => headers.Remove(null));
    }

    [Fact] // the port's choice (Headers' comments): Content-Type and Location removed are the response's own (its default content type, no redirect)
    public void Removing_content_type_and_location_resets_them()
    {
        using var site = new Site().Run(watch: false);
        var response = Request().Web.Response;
        response.ContentType = "text/plain";
        response.RedirectLocation = "/there";
        var headers = response.Headers;
        headers.Remove("Content-Type");
        headers.Remove("Location");
        Assert.Equal("text/html", response.ContentType);
        Assert.Null(response.RedirectLocation);
        Assert.Null(headers["Content-Type"]);
        Assert.Null(headers["Location"]);
    }

    [Fact] // .NET Framework (HttpResponse.AppendHeader's documentation): no header after the headers are sent (HttpException), through the collection too
    public void No_header_after_the_headers_are_sent()
    {
        using var site = new Site().Run(watch: false);
        var response = Request().Web.Response;
        var headers = response.Headers;
        response.Write("x");
        response.Flush();
        Assert.Throws<HttpException>(() => response.AppendHeader("X-A", "1"));
        Assert.Throws<HttpException>(() => headers.Add("X-A", "1"));
        Assert.Throws<HttpException>(() => headers.Remove("X-A"));
    }

    [Fact] // the port's choice (Flush's comment, IIS's integrated pipeline): the characters written before the headers of a status other than 200 give Content-Type its charset
    public void A_flushed_error_s_content_type_has_its_charset()
    {
        using var site = new Site().Run(watch: false);
        var (web, core) = Request();
        web.Response.StatusCode = 404;
        web.Response.Write("not found");
        web.Response.Flush();
        Assert.Equal(404, core.Response.StatusCode);
        Assert.Equal("text/html; charset=utf-8", core.Response.ContentType);
    }

    [Fact] // .NET Framework (reference source): Response.End outside a cancellable period flushes and ends; the end observed then aborts (ThreadAbortException)
    public void End_outside_a_cancellable_period_is_observed_later()
    {
        using var site = new Site().Run(watch: false);
        var (web, core) = Request();
        web.Response.Write("ended");
        web.Response.End();
        web.Response.Write(" not sent");
        Assert.Contains("ended", Site.Body(core));
        Assert.DoesNotContain("not sent", Site.Body(core));
        Assert.Throws<ThreadAbortException>(() => web.Response.ObserveResponseEndCalled());
        web.Response.ObserveResponseEndCalled();
    }

    [Fact] // .NET Framework (reference source): in a cancellable period Response.End only aborts the thread (ThreadAbortException); the output goes at the end of the request
    public void End_in_a_cancellable_period_aborts()
    {
        using var site = new Site().Run(watch: false);
        var (web, core) = Request();
        web.Response.Write("ended");
        web.BeginCancellablePeriod();
        try { Assert.Throws<ThreadAbortException>(() => web.Response.End()); }
        finally { web.EndCancellablePeriod(); }
        Assert.Equal("", Site.Body(core));
        web.Response.ResetThreadAbort();
        web.Response.FinalFlushAtTheEndOfRequestProcessing();
        Assert.Equal("ended", Site.Body(core));
    }

    [Fact] // IIS (WingtipToys' log in, recorded): a page's Response.Redirect after OWIN's SignIn answers 302 with the sign-in cookie and its length - EndRequest's handlers (OWIN's cookie middleware) still set headers after Response.End
    public void Headers_set_at_EndRequest_after_a_redirect_are_sent()
    {
        using var site = new Site().Run(watch: false);
        using var config = Security.MachineKeys.ClientConfig(("HttpRuntime", new System.Web.Configuration.HttpRuntimeSection()), ("Globalization", new System.Web.Configuration.GlobalizationSection()));
        var (web, core) = Request();
        web.BeginCancellablePeriod();
        // Response.Redirect(url): Redirect(url, false), then End (no configuration system here for the one-argument form).
        web.Response.Redirect("/there", false);
        try { Assert.Throws<ThreadAbortException>(() => web.Response.End()); }
        finally { web.EndCancellablePeriod(); }
        // The ended code goes on (no thread aborted on .NET: a catch swallowed the end): what it writes is not kept.
        web.Response.Write(" written after the end");
        Assert.False(core.Response.Headers.ContainsKey("Location"));

        // The pipeline observes the end, EndRequest runs: its handlers set a cookie and a header, and write.
        web.Response.ResetThreadAbort();
        // As Katana's host writes OWIN's response headers: into Response.Headers.
        web.Response.Headers.Add("Set-Cookie", ".AspNet.ApplicationCookie=signed-in; path=/; HttpOnly");
        web.Response.Headers.Set("Pragma", "no-cache");
        web.Response.FinalFlushAtTheEndOfRequestProcessing();

        Assert.Equal(302, core.Response.StatusCode);
        Assert.Equal("/there", core.Response.Headers.Location.ToString());
        Assert.Contains(".AspNet.ApplicationCookie=signed-in", core.Response.Headers.SetCookie.ToString());
        Assert.Equal("no-cache", core.Response.Headers.Pragma.ToString());
        var body = Site.Body(core);
        Assert.Contains("Object moved", body);
        Assert.DoesNotContain("written after the end", body);
        Assert.Equal(System.Text.Encoding.UTF8.GetByteCount(body), core.Response.ContentLength);
    }

    [Fact] // the port's choice (End's and Flush's comments: no thread aborted on .NET): past executionTimeout Flush and End fail ("Request timed out."), once
    public void A_timed_out_request_fails_once()
    {
        using var site = new Site().Run(watch: false);
        var web = Request().Web;
        web.MarkTimedOut();
        Assert.Throws<HttpException>(() => web.Response.Flush());
        web.Response.Flush();
        var other = Request().Web;
        other.MarkTimedOut();
        Assert.Throws<HttpException>(() => other.Response.End());
    }

    [Fact] // .NET Framework (Response.Redirect, reference source): non-ASCII in the path as UTF-8, in the query string in the request's encoding; an absolute URL's host left
    public void A_redirect_s_url_is_encoded()
    {
        using var site = new Site().Run(watch: false);
        using var config = Security.MachineKeys.ClientConfig(("HttpRuntime", new System.Web.Configuration.HttpRuntimeSection()), ("Globalization", new System.Web.Configuration.GlobalizationSection()));
        var response = Request().Web.Response;
        response.Redirect("/パス a?q=値 b", false);
        Assert.Equal(302, response.StatusCode);
        Assert.Equal("/%e3%83%91%e3%82%b9%20a?q=%e5%80%a4 b", response.RedirectLocation);
        response = Request().Web.Response;
        response.Redirect("http://example.com/パス", false);
        Assert.Equal("http://example.com/%e3%83%91%e3%82%b9", response.RedirectLocation);
        response = Request().Web.Response;
        response.Redirect("パス", false);
        // A relative URL rooted at the request's folder first (ApplyAppPathModifier, reference source).
        Assert.Equal("/%e3%83%91%e3%82%b9", response.RedirectLocation);
    }

    [Fact] // HttpResponse.SubStatusCode's documentation (integrated mode, which the port's is): set and read before the headers are sent
    public void The_sub_status_code_is_kept()
    {
        using var site = new Site().Run(watch: false);
        Assert.True(HttpRuntime.UsingIntegratedPipeline);
        var response = Request().Web.Response;
        response.SubStatusCode = 5;
        Assert.Equal(5, response.SubStatusCode);
    }

    [Fact]
    // .NET Framework's integrated mode (reference source: HttpHeaderCollection.Add/Set/Remove of a request's headers ->
    // IIS7WorkerRequest.SetRequestHeader, Params invalidated); the port's pipeline is integrated (HttpRuntime.UsingIntegratedPipeline)
    public void The_request_s_headers_are_written_in_integrated_mode()
    {
        using var site = new Site().Run(watch: false);
        Assert.True(HttpRuntime.UsingIntegratedPipeline);
        var request = Hosted(site).Web.Request;
        request.Headers.Add("X-Added", "1");
        request.Headers.Set("X-Set", "2");
        Assert.Equal("1", request.Headers["X-Added"]);
        Assert.Equal("2", request.Params["X-Set"] ?? request.Headers["X-Set"]);
        request.Headers.Remove("X-Added");
        Assert.Null(request.Headers["X-Added"]);
    }

    [Fact] // the same, on the ASP.NET Core request (IIS' header block there) and in the server variables (HTTP_*)
    public void The_request_s_headers_written_are_the_core_request_s()
    {
        using var site = new Site().Run(watch: false);
        var (web, core) = Hosted(site);
        var request = web.Request;
        Assert.Null(request.ServerVariables["HTTP_X_SET"]);
        request.Headers.Set("X-Set", "2");
        request.Headers.Add("X-Set", "3");
        Assert.Equal(new[] { "2", "3" }, core.Request.Headers["X-Set"].ToArray());
        Assert.Equal("2,3", request.ServerVariables["HTTP_X_SET"]);
        request.Headers.Remove("X-Set");
        Assert.False(core.Request.Headers.ContainsKey("X-Set"));
        Assert.Null(request.ServerVariables["HTTP_X_SET"]);
    }

    [Fact] // .NET Framework's integrated mode (reference source: HttpServerVarsCollection.Set/Remove -> IIS7WorkerRequest.SetServerVariable, an HTTP_ variable the request's header)
    public void The_server_variables_are_written_in_integrated_mode()
    {
        using var site = new Site().Run(watch: false);
        var (web, core) = Hosted(site);
        var request = web.Request;
        request.ServerVariables.Set("HTTP_X_VAR", "1");
        Assert.Equal("1", request.ServerVariables["HTTP_X_VAR"]);
        Assert.Equal("1", request.Headers["X-VAR"]);
        Assert.Equal("1", core.Request.Headers["X-VAR"].ToString());
        request.ServerVariables.Set("MY_VAR", "2");
        Assert.Equal("2", request.ServerVariables["MY_VAR"]);
        request.ServerVariables.Remove("HTTP_X_VAR");
        Assert.Null(request.ServerVariables["HTTP_X_VAR"]);
        Assert.Null(request.Headers["X-VAR"]);
        Assert.False(core.Request.Headers.ContainsKey("X-VAR"));
    }
}
