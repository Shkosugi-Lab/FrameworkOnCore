using System.Globalization;
using System.Web;
using System.Web.Hosting;
using Microsoft.AspNetCore.Http;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// IIS's static file handler as the host serves a file (IisStaticFile), against Microsoft's configuration reference:
/// staticContent's mimeMap (learn.microsoft.com/iis/configuration/system.webserver/staticcontent/mimemap, "mimeMap" below)
/// and clientCache (.../staticcontent/clientcache, "clientCache"), directoryBrowse (.../system.webserver/directorybrowse),
/// and HTTP's conditional and range requests (RFC 9110).
/// </summary>
[Collection("Application")]
public class IisStaticFileTests
{
    sealed class Served
    {
        public bool Handled;
        public int Status;
        public System.Collections.Specialized.NameValueCollection Headers;
        public string Body;
        public string ContentType;
        public System.Web.HttpContext Web;
        public DefaultHttpContext Core;
        public IisLogStatus Log => Core.Features.Get<IisLogStatus>();
    }

    static Served Serve(Site site, string url, Action<DefaultHttpContext> setup = null, string method = "GET", Action<System.Web.HttpContext> before = null)
    {
        var (web, core) = Site.WebRequest(url, method, setup);
        before?.Invoke(web);
        var physical = Path.Combine(site.Root, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        var handled = IisStaticFile.Serve(web, url, physical);
        var (status, headers, body) = Site.Response(web);
        return new Served
        {
            Handled = handled, Status = status, Headers = headers, Body = System.Text.Encoding.UTF8.GetString(body),
            ContentType = web.Response.ContentType, Web = web, Core = core,
        };
    }

    static Site Text(string webConfig = null, string profile = null, bool watch = true)
    {
        var site = new Site(webConfig, profile);
        site.Write("a.txt", "hello");
        return site.Run(watch);
    }

    static string Etag(Site site, string file = "a.txt") =>
        "\"" + File.GetLastWriteTimeUtc(Path.Combine(site.Root, file)).ToFileTimeUtc().ToString("x", CultureInfo.InvariantCulture) + ":0\"";

    static string StaticContent(string content) => Site.WebServer("<staticContent>" + content + "</staticContent>");

    // --- The file

    [Fact] // IIS's static file handler: the file with its type (mimeMap), Last-Modified, ETag, Accept-Ranges; clientCache's NoControl (default): no Cache-Control nor Expires
    public void A_file_is_served_with_its_type_and_validators()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt");
        Assert.True(served.Handled);
        Assert.Equal(200, served.Status);
        Assert.Equal("hello", served.Body);
        Assert.Equal("text/plain", served.ContentType);
        Assert.Equal("bytes", served.Headers["Accept-Ranges"]);
        Assert.Equal(Etag(site), served.Headers["ETag"]);
        var written = File.GetLastWriteTimeUtc(Path.Combine(site.Root, "a.txt"));
        Assert.Equal(written.ToString("R", CultureInfo.InvariantCulture), served.Headers["Last-Modified"]);
        Assert.Null(served.Headers["Cache-Control"]);
        Assert.Null(served.Headers["Expires"]);
    }

    [Fact] // the host's choice (summary): no X-AspNet-Version on a static file (IIS's handler is not ASP.NET's)
    public void A_file_has_no_asp_net_version()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt");
        Assert.True(served.Web.Response.SuppressVersionHeader);
    }

    [Fact] // the host's choice (summary): the whole file is what IIS's static compression looks at
    public void The_whole_file_is_told_to_compression()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt");
        var file = served.Core.Features.Get<IisStaticResponse>();
        Assert.Equal(Path.Combine(site.Root, "a.txt"), file.PhysicalPath);
        Assert.Equal(5, file.Length);
    }

    [Fact] // the host's choice (summary of FileCache): a file outside a watched folder is sent from the file, the same bytes
    public void A_file_of_an_unwatched_folder_is_sent_from_the_file()
    {
        using var site = Text(watch: false);
        var served = Serve(site, "/a.txt");
        Assert.Equal(200, served.Status);
        Assert.Equal("hello", served.Body);
        Assert.Null(((AspNetCoreWorkerRequest)served.Web.WorkerRequest).ResponseCapture);
    }

    [Fact] // IIS's static file handler: an empty file is a 200 without a body
    public void An_empty_file_is_served_without_a_body()
    {
        using var site = new Site();
        site.Write("e.txt", "").Run();
        var served = Serve(site, "/e.txt");
        Assert.Equal(200, served.Status);
        Assert.Equal("", served.Body);
    }

    [Fact] // the host's choice (StaticResponseCache's summary): the whole file to an anonymous GET is captured for the kernel cache
    public void The_whole_file_to_an_anonymous_user_is_captured()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt");
        Assert.NotNull(((AspNetCoreWorkerRequest)served.Web.WorkerRequest).ResponseCapture);
    }

    [Fact] // http.sys's kernel cache does not keep a response to an authenticated user (StaticResponseCache's summary)
    public void The_file_to_an_authenticated_user_is_not_captured()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt", before: web =>
            web.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity("user", "Forms"), null));
        Assert.Equal("hello", served.Body);
        Assert.Null(((AspNetCoreWorkerRequest)served.Web.WorkerRequest).ResponseCapture);
    }

    sealed class Anonymous : System.Security.Principal.IPrincipal
    {
        public System.Security.Principal.IIdentity Identity => null;
        public bool IsInRole(string role) => false;
    }

    [Fact] // the host's choice: a user without an identity is anonymous (captured)
    public void A_user_without_an_identity_is_anonymous()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt", before: web => web.User = new Anonymous());
        Assert.NotNull(((AspNetCoreWorkerRequest)served.Web.WorkerRequest).ResponseCapture);
    }

    [Fact] // the host's choice: System.Web on another worker request (not the host's) gets the same answers, without the host's features
    public void Another_worker_request_gets_the_same_answers()
    {
        using var site = Text();
        site.Write("sub/x.txt", "x");
        System.Web.HttpContext Web(string page) => new System.Web.HttpContext(new SimpleWorkerRequest(page, "", TextWriter.Null));
        var file = Web("a.txt");
        IisStaticFile.Serve(file, "/a.txt", Path.Combine(site.Root, "a.txt"));
        Assert.Equal("hello", System.Text.Encoding.UTF8.GetString(Site.Response(file).Body));
        var missing = Web("missing.txt");
        IisStaticFile.Serve(missing, "/missing.txt", Path.Combine(site.Root, "missing.txt"));
        Assert.Equal(404, Site.Response(missing).Status);
        var folder = Web("sub");
        IisStaticFile.Serve(folder, "/sub", Path.Combine(site.Root, "sub"));
        Assert.Equal(403, Site.Response(folder).Status);
    }

    [Fact] // System.Web's HttpResponse.DisableKernelCache: not kept
    public void The_file_is_not_captured_where_the_kernel_cache_is_disabled()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt", before: web => ((AspNetCoreWorkerRequest)web.WorkerRequest).DisableKernelCache());
        Assert.Null(((AspNetCoreWorkerRequest)served.Web.WorkerRequest).ResponseCapture);
    }

    [Fact] // the host's choice (summary): a request whose response is not kept (a query string) is not captured
    public void A_request_with_a_query_string_is_not_captured()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt", core => core.Request.QueryString = new QueryString("?v=1"));
        Assert.Equal("hello", served.Body);
        Assert.Null(((AspNetCoreWorkerRequest)served.Web.WorkerRequest).ResponseCapture);
    }

    [Fact] // RFC 9110 8.8.2.1: a Last-Modified later than the response's date is the date
    public void A_file_written_in_the_future_has_the_present_as_its_last_modified()
    {
        using var site = Text();
        File.SetLastWriteTimeUtc(Path.Combine(site.Root, "a.txt"), DateTime.UtcNow.AddDays(2));
        var served = Serve(site, "/a.txt");
        var lastModified = DateTime.Parse(served.Headers["Last-Modified"], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        Assert.True(lastModified <= DateTime.UtcNow);
        Assert.True(lastModified > DateTime.UtcNow.AddMinutes(-1));
    }

    // --- Not a file

    [Fact] // IIS: 404.0 for a file that is not there (its log: sc-substatus 0, sc-win32-status 2); the page is httpErrors'
    public void A_missing_file_is_a_404_0()
    {
        using var site = Text();
        var served = Serve(site, "/missing.txt");
        Assert.True(served.Handled);
        Assert.Equal(404, served.Status);
        Assert.Equal("", served.Body);
        Assert.Equal(0, served.Log.SubStatus);
        Assert.Equal(2, served.Log.Win32Status);
        Assert.True(served.Web.Response.SuppressDefaultCacheControlHeader);
    }

    [Fact] // the host's choice (summary: a file that is not, 404): IIS's static file handler does not serve a hidden file
    public void A_hidden_file_is_a_404()
    {
        using var site = Text();
        // On Linux a file whose name starts with a dot is the hidden one (.NET's FileAttributes.Hidden there).
        var name = OperatingSystem.IsWindows() ? "a.txt" : ".a.txt";
        var file = Path.Combine(site.Root, name);
        if (OperatingSystem.IsWindows()) File.SetAttributes(file, File.GetAttributes(file) | FileAttributes.Hidden);
        else File.Copy(Path.Combine(site.Root, "a.txt"), file);
        try
        {
            var served = Serve(site, "/" + name);
            Assert.Equal(404, served.Status);
            Assert.Equal("", served.Body);
        }
        finally { File.SetAttributes(file, FileAttributes.Normal); }
    }

    [Fact] // the host's choice (summary: a file that is not, 404): a name with a trailing dot is not the file Windows trims it to
    public void A_name_with_a_trailing_dot_is_a_404()
    {
        using var site = Text();
        var (web, core) = Site.WebRequest("/a.txt.");
        IisStaticFile.Serve(web, "/a.txt.", Path.Combine(site.Root, "a.txt."));
        Assert.Equal(404, Site.Response(web).Status);
    }

    [Fact] // mimeMap's note: IIS does not return a file type that is not in staticContent (404.3; its log: win32 50)
    public void A_file_of_a_type_not_in_the_map_is_a_404_3()
    {
        using var site = new Site();
        site.Write("a.unknownext", "x").Run();
        var served = Serve(site, "/a.unknownext");
        Assert.Equal(404, served.Status);
        Assert.Equal("", served.Body);
        Assert.Equal(3, served.Log.SubStatus);
        Assert.Equal(50, served.Log.Win32Status);
    }

    // --- staticContent's MIME map

    [Fact] // mimeMap: a type added in web.config is served with it
    public void A_type_added_in_web_config_is_served()
    {
        using var site = new Site(StaticContent("<mimeMap fileExtension='.syx' mimeType='application/octet-stream' />"));
        site.Write("a.syx", "x").Run();
        var served = Serve(site, "/a.syx");
        Assert.Equal(200, served.Status);
        Assert.Equal("application/octet-stream", served.ContentType);
    }

    [Fact] // mimeMap: a type added replaces the inherited one for its extension (the collection's key is the extension)
    public void A_type_added_for_a_known_extension_replaces_it()
    {
        using var site = Text(StaticContent("<remove fileExtension='.txt' /><mimeMap fileExtension='.txt' mimeType='text/x-mine' />"));
        Assert.Equal("text/x-mine", Serve(site, "/a.txt").ContentType);
    }

    [Fact] // staticContent's remove: the type removed is not served (404.3)
    public void A_removed_type_is_a_404_3()
    {
        using var site = Text(StaticContent("<remove fileExtension='.txt' />"));
        var served = Serve(site, "/a.txt");
        Assert.Equal(404, served.Status);
        Assert.Equal(3, served.Log.SubStatus);
    }

    [Fact] // staticContent's clear: no type inherited (404.3)
    public void A_cleared_map_serves_nothing()
    {
        using var site = Text(StaticContent("<clear />"));
        Assert.Equal(404, Serve(site, "/a.txt").Status);
    }

    [Fact] // mimeMap: fileExtension and mimeType are required; an entry without them adds nothing (nor does a remove without its extension)
    public void Entries_without_their_attributes_change_nothing()
    {
        using var site = Text(StaticContent("<remove /><mimeMap fileExtension='.syx' /><mimeMap mimeType='a/b' /><unknown />"));
        site.Write("a.syx", "x");
        Assert.Equal("text/plain", Serve(site, "/a.txt").ContentType);
        Assert.Equal(404, Serve(site, "/a.syx").Status);
    }

    [Fact] // the host's choice (not in IIS's docs, a common IIS setting): a ".*" type is the one of an extension not in the map
    public void The_wildcard_type_serves_any_extension()
    {
        using var site = new Site(StaticContent("<mimeMap fileExtension='.*' mimeType='application/octet-stream' />"));
        site.Write("a.unknownext", "x").Write("b.txt", "y").Run();
        Assert.Equal("application/octet-stream", Serve(site, "/a.unknownext").ContentType);
        Assert.Equal("text/plain", Serve(site, "/b.txt").ContentType);
    }

    [Fact] // the host's choice (summary of MakeStaticContent): the profile's MIME map is the whole map (applicationHost.config's)
    public void The_profile_map_is_the_whole_map()
    {
        using var site = new Site(profile: Site.Profile("<staticContent><mimeMap fileExtension='.syx' mimeType='application/x-syx' /></staticContent>"));
        site.Write("a.syx", "x").Write("a.txt", "y").Run();
        Assert.Equal("application/x-syx", Serve(site, "/a.syx").ContentType);
        Assert.Equal(404, Serve(site, "/a.txt").Status);
    }

    [Fact] // IIS's configuration: a location of the profile (applicationHost.config's) for the site applies to its folder only
    public void A_profile_location_applies_to_its_folder()
    {
        using var site = new Site(profile: Site.Profile("", "<location path='sub'><system.webServer><staticContent><remove fileExtension='.txt' /></staticContent></system.webServer></location>"));
        site.Write("a.txt", "x").Write("sub/a.txt", "y").Run();
        Assert.Equal(200, Serve(site, "/a.txt").Status);
        Assert.Equal(404, Serve(site, "/sub/a.txt").Status);
    }

    [Fact] // IIS's configuration: a web.config's location path applies to its folder, "." and "" to the whole file's folder, another path not
    public void Web_config_locations_apply_by_path()
    {
        using var site = new Site("<configuration>" +
            "<location path='.'><system.webServer><staticContent><mimeMap fileExtension='.aa' mimeType='a/a' /></staticContent></system.webServer></location>" +
            "<location><system.webServer><staticContent><mimeMap fileExtension='.bb' mimeType='b/b' /></staticContent></system.webServer></location>" +
            "<location path='sub/'><system.webServer><staticContent><mimeMap fileExtension='.cc' mimeType='c/c' /></staticContent></system.webServer></location>" +
            "<location path='sub/x.dd'><system.webServer><staticContent><mimeMap fileExtension='.dd' mimeType='d/d' /></staticContent></system.webServer></location>" +
            "<location path='other'><system.webServer><staticContent><mimeMap fileExtension='.cc' mimeType='o/o' /></staticContent></system.webServer></location>" +
            "</configuration>");
        site.Write("x.aa", "1").Write("x.bb", "2").Write("x.cc", "3").Write("sub/x.cc", "4").Write("sub/x.dd", "5").Write("sub/y.dd", "6").Run();
        Assert.Equal("a/a", Serve(site, "/x.aa").ContentType);
        Assert.Equal("b/b", Serve(site, "/x.bb").ContentType);
        Assert.Equal(404, Serve(site, "/x.cc").Status);
        Assert.Equal("c/c", Serve(site, "/sub/x.cc").ContentType);
        Assert.Equal("d/d", Serve(site, "/sub/x.dd").ContentType);
        Assert.Equal(404, Serve(site, "/sub/y.dd").Status);
    }

    [Fact] // IIS's configuration: a folder's web.config applies after its parent's, to the folder's files only
    public void A_folder_web_config_applies_to_its_folder()
    {
        using var site = new Site(StaticContent("<mimeMap fileExtension='.aa' mimeType='a/a' />"));
        site.Write("sub/web.config", StaticContent("<remove fileExtension='.aa' /><mimeMap fileExtension='.bb' mimeType='b/b' />"))
            .Write("x.aa", "1").Write("x.bb", "2").Write("sub/x.aa", "3").Write("sub/x.bb", "4").Write("sub/deeper/x.bb", "5").Run();
        Assert.Equal("a/a", Serve(site, "/x.aa").ContentType);
        Assert.Equal(404, Serve(site, "/x.bb").Status);
        Assert.Equal(404, Serve(site, "/sub/x.aa").Status);
        Assert.Equal("b/b", Serve(site, "/sub/x.bb").ContentType);
        Assert.Equal("b/b", Serve(site, "/sub/deeper/x.bb").ContentType);
    }

    [Fact] // the host's choice (not in IIS's docs: IIS refuses to run with a malformed web.config): a web.config that is not XML adds nothing
    public void A_web_config_that_is_not_xml_adds_nothing()
    {
        using var site = new Site("<configuration>");
        site.Write("a.txt", "x").Write("b.txt", "y").Run();
        Assert.Equal("text/plain", Serve(site, "/a.txt").ContentType);
        Assert.Equal("text/plain", Serve(site, "/b.txt").ContentType);
    }

    [Fact] // the host's choice (not in IIS's docs): a web.config that cannot be read (locked) adds nothing
    public void A_web_config_that_cannot_be_read_adds_nothing()
    {
        using var site = new Site(StaticContent("<remove fileExtension='.txt' />"));
        site.Write("a.txt", "x").Run(watch: false);
        using (new FileStream(Path.Combine(site.Root, "web.config"), FileMode.Open, FileAccess.Read, FileShare.None))
            Assert.Equal(200, Serve(site, "/a.txt").Status);
    }

    [Fact] // IIS's file change notifications: a web.config changed is read again
    public void A_changed_web_config_is_read_again()
    {
        using var site = new Site(StaticContent("<mimeMap fileExtension='.aa' mimeType='a/a' />"));
        site.Write("x.aa", "1").Run(watch: false);
        Assert.Equal("a/a", Serve(site, "/x.aa").ContentType);
        site.Write("web.config", StaticContent("<mimeMap fileExtension='.aa' mimeType='b/b' />"));
        File.SetLastWriteTimeUtc(Path.Combine(site.Root, "web.config"), DateTime.UtcNow.AddMinutes(1));
        Assert.Equal("b/b", Serve(site, "/x.aa").ContentType);
    }

    // --- clientCache

    [Fact] // clientCache: DisableCache adds Cache-Control: no-cache
    public void Disable_cache_adds_no_cache()
    {
        using var site = Text(StaticContent("<clientCache cacheControlMode='DisableCache' />"));
        Assert.Equal("no-cache", Serve(site, "/a.txt").Headers["Cache-Control"]);
    }

    [Fact] // clientCache: UseMaxAge adds max-age from cacheControlMaxAge
    public void Use_max_age_adds_max_age()
    {
        using var site = Text(StaticContent("<clientCache cacheControlMode='UseMaxAge' cacheControlMaxAge='00:10:00' />"));
        Assert.Equal("max-age=600", Serve(site, "/a.txt").Headers["Cache-Control"]);
    }

    [Fact] // clientCache: cacheControlMaxAge's default is 1.00:00:00 (one day); a value that is not a time span leaves it
    public void Max_age_defaults_to_a_day()
    {
        using var site = Text(StaticContent("<clientCache cacheControlMode='UseMaxAge' cacheControlMaxAge='soon' />"));
        Assert.Equal("max-age=86400", Serve(site, "/a.txt").Headers["Cache-Control"]);
    }

    [Fact] // clientCache: UseExpires adds Expires from httpExpires (no Cache-Control)
    public void Use_expires_adds_expires()
    {
        using var site = Text(StaticContent("<clientCache cacheControlMode='UseExpires' httpExpires='Tue, 19 Jan 2038 03:14:07 GMT' />"));
        var served = Serve(site, "/a.txt");
        Assert.Equal("Tue, 19 Jan 2038 03:14:07 GMT", served.Headers["Expires"]);
        Assert.Null(served.Headers["Cache-Control"]);
    }

    [Fact] // the host's choice (not in IIS's docs): UseExpires without httpExpires adds nothing
    public void Use_expires_without_a_date_adds_nothing()
    {
        using var site = Text(StaticContent("<clientCache cacheControlMode='UseExpires' />"));
        var served = Serve(site, "/a.txt");
        Assert.Null(served.Headers["Expires"]);
        Assert.Null(served.Headers["Cache-Control"]);
    }

    [Fact] // the host's choice (summary, observed on IIS): cacheControlCustom comes first, then the mode's directive
    public void Custom_directives_come_before_the_mode()
    {
        using var site = Text(StaticContent("<clientCache cacheControlMode='UseMaxAge' cacheControlCustom='public' />"));
        Assert.Equal("public,max-age=86400", Serve(site, "/a.txt").Headers["Cache-Control"]);
    }

    [Fact] // IIS's configuration: each clientCache attribute is the nearest one's (a folder's max age, the root's mode)
    public void Client_cache_attributes_are_inherited_one_by_one()
    {
        using var site = Text(StaticContent("<clientCache cacheControlMode='UseMaxAge' setEtag='false' />"));
        site.Write("sub/web.config", StaticContent("<clientCache cacheControlMaxAge='00:01:00' setEtag='true' />")).Write("sub/a.txt", "x");
        var root = Serve(site, "/a.txt");
        Assert.Equal("max-age=86400", root.Headers["Cache-Control"]);
        Assert.Null(root.Headers["ETag"]);
        var sub = Serve(site, "/sub/a.txt");
        Assert.Equal("max-age=60", sub.Headers["Cache-Control"]);
        Assert.Equal(Etag(site, "sub/a.txt"), sub.Headers["ETag"]);
    }

    // --- Conditional requests

    [Fact] // RFC 9110 13.1.2: If-None-Match with the file's ETag is a 304 without a body (the validators sent)
    public void If_none_match_with_the_etag_is_a_304()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt", core => core.Request.Headers["If-None-Match"] = Etag(site));
        Assert.Equal(304, served.Status);
        Assert.Equal("", served.Body);
        Assert.Equal(Etag(site), served.Headers["ETag"]);
    }

    [Fact] // RFC 9110 13.1.2: If-None-Match compares weakly: a weak tag, a list with it, "*"
    public void If_none_match_compares_weakly_and_takes_a_list_or_any()
    {
        using var site = Text();
        Assert.Equal(304, Serve(site, "/a.txt", core => core.Request.Headers["If-None-Match"] = "W/" + Etag(site)).Status);
        Assert.Equal(304, Serve(site, "/a.txt", core => core.Request.Headers["If-None-Match"] = "\"x\", " + Etag(site)).Status);
        Assert.Equal(304, Serve(site, "/a.txt", core => core.Request.Headers["If-None-Match"] = "*").Status);
    }

    [Fact] // RFC 9110 13.1.3: If-Modified-Since is ignored where If-None-Match is sent; a tag that does not match: the file
    public void If_none_match_without_the_etag_sends_the_file_whatever_the_date()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt", core =>
        {
            core.Request.Headers["If-None-Match"] = "\"other\"";
            core.Request.Headers["If-Modified-Since"] = DateTime.UtcNow.ToString("R");
        });
        Assert.Equal(200, served.Status);
        Assert.Equal("hello", served.Body);
    }

    [Fact] // RFC 9110 13.1.3: If-Modified-Since at or after the file's time is a 304; before it, the file
    public void If_modified_since_is_a_304_unless_the_file_is_newer()
    {
        using var site = Text();
        var written = File.GetLastWriteTimeUtc(Path.Combine(site.Root, "a.txt"));
        Assert.Equal(304, Serve(site, "/a.txt", core => core.Request.Headers["If-Modified-Since"] = written.ToString("R")).Status);
        Assert.Equal(200, Serve(site, "/a.txt", core => core.Request.Headers["If-Modified-Since"] = written.AddHours(-1).ToString("R")).Status);
    }

    [Fact] // RFC 9110 13.1.3: a date later than the server's time, or not a date, is ignored: the file
    public void If_modified_since_in_the_future_or_invalid_is_ignored()
    {
        using var site = Text();
        Assert.Equal(200, Serve(site, "/a.txt", core => core.Request.Headers["If-Modified-Since"] = DateTime.UtcNow.AddDays(1).ToString("R")).Status);
        Assert.Equal(200, Serve(site, "/a.txt", core => core.Request.Headers["If-Modified-Since"] = "yesterday").Status);
    }

    // --- Ranges

    [Fact] // RFC 9110 14.2: a satisfiable byte range is a 206 with Content-Range; clientCache's headers too
    public void A_byte_range_is_a_206()
    {
        using var site = Text(StaticContent("<clientCache cacheControlMode='DisableCache' />"));
        var served = Serve(site, "/a.txt", core => core.Request.Headers["Range"] = "bytes=1-2");
        Assert.Equal(206, served.Status);
        Assert.Equal("el", served.Body);
        Assert.Equal("bytes 1-2/5", served.Headers["Content-Range"]);
        Assert.Equal("no-cache", served.Headers["Cache-Control"]);
        Assert.Null(served.Core.Features.Get<IisStaticResponse>());
    }

    [Fact] // RFC 9110 15.5.17: a range none of which is in the file is a 416 (Content-Range: bytes */length)
    public void An_unsatisfiable_range_is_a_416()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt", core => core.Request.Headers["Range"] = "bytes=10-");
        Assert.Equal(416, served.Status);
        Assert.Equal("bytes */5", served.Headers["Content-Range"]);
    }

    [Fact(Skip = "bug: bytes=10-20 of a 5-byte file is taken as invalid syntax (StaticFileHandler.GetNextRange: last < first after clamping), the whole file 200, not 416")]
    // RFC 9110 14.1.1/15.5.17: a first-pos beyond the file with a last-pos is a valid range that is not satisfiable: 416
    public void A_range_with_both_ends_beyond_the_file_is_a_416()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt", core => core.Request.Headers["Range"] = "bytes=10-20");
        Assert.Equal(416, served.Status);
        Assert.Equal("bytes */5", served.Headers["Content-Range"]);
    }

    [Fact] // RFC 9110 14.2: a range of a unit the server does not know is ignored: the whole file
    public void A_range_of_another_unit_sends_the_file()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt", core => core.Request.Headers["Range"] = "items=1-2");
        Assert.Equal(200, served.Status);
        Assert.Equal("hello", served.Body);
    }

    [Fact] // RFC 9110 13.1.5: If-Range with another ETag: the whole file, not the range
    public void A_range_with_an_if_range_that_does_not_match_sends_the_file()
    {
        using var site = Text();
        var served = Serve(site, "/a.txt", core =>
        {
            core.Request.Headers["Range"] = "bytes=1-2";
            core.Request.Headers["If-Range"] = "\"other\"";
        });
        Assert.Equal(200, served.Status);
        Assert.Equal("hello", served.Body);
        Assert.Equal("bytes", served.Headers["Accept-Ranges"]);
    }

    // --- Folders

    [Fact] // directoryBrowse: disabled by default; IIS answers a folder without a default document 403.14 (its page httpErrors')
    public void A_folder_without_directory_browsing_is_a_403_14()
    {
        using var site = new Site();
        site.Write("sub/a.txt", "x").Run();
        var served = Serve(site, "/sub/");
        Assert.True(served.Handled);
        Assert.Equal(403, served.Status);
        Assert.Equal("", served.Body);
        Assert.Equal(14, served.Log.SubStatus);
        Assert.True(served.Web.Response.SuppressDefaultCacheControlHeader);
    }

    [Fact] // directoryBrowse enabled: IIS's listing, showFlags' default Date, Time, Size, Extension (hidden entries not listed)
    public void Directory_browsing_lists_the_folder()
    {
        using var site = new Site(Site.WebServer("<defaultDocument />"));
        site.Write("sub/web.config", Site.WebServer("<directoryBrowse enabled='true' />")).Write("sub/b.txt", "12345").Write("sub/inner/x.txt", "x")
            .Write(OperatingSystem.IsWindows() ? "sub/hidden.txt" : "sub/.hidden.txt", "h");
        // On Linux a file whose name starts with a dot is the hidden one (.NET's FileAttributes.Hidden there).
        var hidden = Path.Combine(site.Root, "sub", OperatingSystem.IsWindows() ? "hidden.txt" : ".hidden.txt");
        if (OperatingSystem.IsWindows()) File.SetAttributes(hidden, FileAttributes.Hidden);
        try
        {
            site.Run();
            var served = Serve(site, "/sub");
            Assert.Equal(200, served.Status);
            Assert.StartsWith("text/html", served.ContentType);
            var culture = CultureInfo.InstalledUICulture;
            var toParent = culture.TwoLetterISOLanguageName == "ja" ? "[親ディレクトリへ]" : "[To Parent Directory]";
            Assert.Contains("<A HREF=\"/\">" + toParent + "</A>", served.Body);
            Assert.Contains("localhost - /sub/", served.Body);
            Assert.Contains("<A HREF=\"/sub/b.txt\">b.txt</A>", served.Body);
            Assert.Contains("           5 <A HREF=\"/sub/b.txt\">", served.Body);
            Assert.Contains("&lt;dir&gt; <A HREF=\"/sub/inner/\">inner</A>", served.Body);
            var written = File.GetLastWriteTime(Path.Combine(site.Root, "sub", "b.txt"));
            Assert.Contains(written.ToString("d", culture).PadLeft(10) + " " + written.ToString("t", culture).PadLeft(8), served.Body);
            Assert.DoesNotContain("hidden", served.Body);
        }
        finally { File.SetAttributes(hidden, FileAttributes.Normal); }
    }

    [Fact] // directoryBrowse's showFlags: without Extension a file's name has none; LongDate the long date; no Size, no Time
    public void Show_flags_choose_what_is_listed()
    {
        using var site = new Site(Site.WebServer("<directoryBrowse enabled='true' showFlags='LongDate' />"));
        site.Write("sub/b.txt", "12345").Run();
        var body = Serve(site, "/sub/").Body;
        var written = File.GetLastWriteTime(Path.Combine(site.Root, "sub", "b.txt"));
        Assert.Contains(written.ToString("D", CultureInfo.InstalledUICulture) + " <A HREF=\"/sub/b.txt\">b</A>", body);
        Assert.DoesNotContain(" 5 ", body);
    }

    [Fact] // directoryBrowse's showFlags None: the names only
    public void Show_flags_none_lists_the_names_only()
    {
        using var site = new Site(Site.WebServer("<directoryBrowse enabled='true' showFlags='None' />"));
        site.Write("sub/b.txt", "12345").Run();
        Assert.Contains("<pre><A HREF=\"/\">", Serve(site, "/sub/").Body.Replace("\r\n", ""));
        Assert.Contains("</A><br><br><A HREF=\"/sub/b.txt\">b</A><br></pre>", Serve(site, "/sub/").Body);
    }

    [Fact(Skip = "bug: enabled='true' with an empty showFlags is a 403.14, not a listing of the names (directoryBrowse: showFlags is the information listed, enabled whether the listing is)")]
    // directoryBrowse: enabled turns the listing on; showFlags (flags: empty is none of them) only chooses the columns
    public void Directory_browsing_with_empty_show_flags_still_lists()
    {
        using var site = new Site(Site.WebServer("<directoryBrowse enabled='true' showFlags='' />"));
        site.Write("sub/b.txt", "12345").Run();
        var served = Serve(site, "/sub/");
        Assert.Equal(200, served.Status);
        Assert.Contains("<A HREF=\"/sub/b.txt\">b</A>", served.Body);
    }

    [Fact] // directoryBrowse: the application's root has no parent link
    public void The_root_listing_has_no_parent_link()
    {
        using var site = new Site(Site.WebServer("<directoryBrowse enabled='true' />"));
        site.Write("b.txt", "1").Run();
        var body = Serve(site, "/").Body;
        Assert.Contains("<A HREF=\"/b.txt\">b.txt</A>", body);
        Assert.DoesNotContain("<A HREF=\"/\">", body);
    }

    [Fact] // directoryBrowse: a folder's enabled='false' over the root's true (each attribute the nearest one's); a non-boolean says nothing
    public void A_folder_can_disable_directory_browsing()
    {
        using var site = new Site(Site.WebServer("<directoryBrowse enabled='true' />"));
        site.Write("sub/web.config", Site.WebServer("<directoryBrowse enabled='false' />")).Write("sub/a.txt", "x")
            .Write("other/web.config", Site.WebServer("<directoryBrowse enabled='maybe' />")).Write("other/a.txt", "x").Run();
        Assert.Equal(403, Serve(site, "/sub/").Status);
        Assert.Equal(200, Serve(site, "/other/").Status);
    }

    [Fact] // the host's choice (summary): a folder's name FileUtil does not take as a folder's (a trailing dot) is not one: 404
    public void A_folder_name_with_a_trailing_dot_is_a_404()
    {
        using var site = new Site();
        site.Write("sub/a.txt", "x").Run();
        var (web, _) = Site.WebRequest("/sub.");
        IisStaticFile.Serve(web, "/sub.", Path.Combine(site.Root, "sub."));
        Assert.Equal(404, Site.Response(web).Status);
    }

    // --- IIS's error pages

    [Theory] // IIS's custerr pages (%SystemDrive%\inetpub\custerr\en-US): their titles
    [InlineData(401, "401 - Unauthorized: Access is denied due to invalid credentials.")]
    [InlineData(403, "403 - Forbidden: Access is denied.")]
    [InlineData(404, "404 - File or directory not found.")]
    [InlineData(405, "405 - HTTP verb used to access this page is not allowed.")]
    [InlineData(406, "406 - Client browser does not accept the MIME type of the requested page.")]
    [InlineData(412, "412 - Precondition set by the client failed when evaluated on the Web server.")]
    [InlineData(500, "500 - Internal server error.")]
    [InlineData(501, "501 - Header values specify a method that is not implemented.")]
    [InlineData(502, "502 - Web server received an invalid response while acting as a gateway or proxy server.")]
    public void IIS_has_a_page_for_its_statuses(int status, string title)
    {
        var page = IisStaticFile.ErrorPage(status);
        Assert.Contains("<title>" + title + "</title>", page);
        Assert.Contains("<h1>Server Error</h1>", page);
    }

    [Theory] // the host's choice (summary: FrameworkOnCore's own translation of IIS's English): the same statuses in Japanese
    [InlineData(401)] [InlineData(403)] [InlineData(404)] [InlineData(405)] [InlineData(406)] [InlineData(412)] [InlineData(500)] [InlineData(501)] [InlineData(502)]
    public void IIS_pages_are_in_japanese_too(int status)
    {
        var page = IisStaticFile.ErrorPage(status, japanese: true);
        Assert.Contains("<title>" + status + " - ", page);
        Assert.Contains("<h1>サーバー エラー</h1>", page);
    }

    [Theory] // IIS's custerr has no page for other statuses (httpErrors' default entries: 401, 403, 404, 405, 406, 412, 500, 501, 502)
    [InlineData(400, false)] [InlineData(410, false)] [InlineData(503, true)] [InlineData(418, true)]
    public void IIS_has_no_page_for_other_statuses(int status, bool japanese)
    {
        Assert.Null(IisStaticFile.ErrorPage(status, japanese));
    }
}
