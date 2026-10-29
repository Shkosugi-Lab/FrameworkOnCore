using System.IO.Compression;
using System.Text;
using System.Web.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// IIS's static and dynamic compression as the host does it (IisCompression), against Microsoft's "HTTP Compression
/// &lt;httpCompression&gt;" (learn.microsoft.com/iis/configuration/system.webserver/httpcompression/, "httpCompression"
/// below), "URL Compression &lt;urlCompression&gt;" (.../system.webserver/urlcompression/, "urlCompression"), "Server Runtime
/// &lt;serverRuntime&gt;" (.../system.webserver/serverruntime, "serverRuntime"), IIS 10's applicationHost.config and
/// IIS_schema.xml (its defaults: "applicationHost.config"), and HTTP's Accept-Encoding (RFC 9110 12.5.3).
/// </summary>
public class IisCompressionTests
{
    const string Dynamic = "<globalModules><add name='DynamicCompressionModule' /></globalModules>";
    const string Both = "<globalModules><add name='StaticCompressionModule' /><add name='DynamicCompressionModule' /></globalModules>";

    static readonly string Text = string.Concat(Enumerable.Repeat("IIS compresses what it is told to compress. ", 100));   // 4,400 bytes

    static string Server(string modules = Dynamic, string compression = "", string more = "") =>
        "<profile>" + modules + "<system.webServer>" + compression + more + "</system.webServer></profile>";

    sealed class Answered
    {
        public DefaultHttpContext Context;
        public byte[] Raw;
        public string Encoding => Context.Response.Headers.ContentEncoding.ToString();
        public bool Compressed => Encoding.Length > 0;
        public string Body => System.Text.Encoding.UTF8.GetString(Decompress(Encoding, Raw));
    }

    static byte[] Decompress(string encoding, byte[] raw)
    {
        using var input = new MemoryStream(raw);
        using Stream stream = encoding switch
        {
            "gzip" => new GZipStream(input, CompressionMode.Decompress),
            "deflate" => new ZLibStream(input, CompressionMode.Decompress),
            "br" => new BrotliStream(input, CompressionMode.Decompress),
            _ => input,
        };
        using var output = new MemoryStream();
        stream.CopyTo(output);
        return output.ToArray();
    }

    /// <summary>The application's answer: the status, the type, then the text written.</summary>
    static RequestDelegate Page(string text = null, string type = "text/html; charset=utf-8", int status = 200) => async c =>
    {
        c.Response.StatusCode = status;
        c.Response.ContentType = type;
        await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text ?? Text));
    };

    static async Task<Answered> Run(IisCompression compression, RequestDelegate app = null, string target = "/page.aspx", string accept = "gzip",
        Action<DefaultHttpContext> setup = null, string method = "GET")
    {
        var context = Site.Request(target, method);
        if (accept != null) context.Request.Headers.AcceptEncoding = accept;
        setup?.Invoke(context);
        await new IisCompression.Middleware(app ?? Page(), compression).Invoke(context);
        return new Answered { Context = context, Raw = ((MemoryStream)context.Response.Body).ToArray() };
    }

    /// <summary>A static file's answer, as IisStaticFile gives it (its IisStaticResponse).</summary>
    static RequestDelegate File(Site site, string name = "a.txt", string type = "text/plain", string text = null, int status = 200)
    {
        var path = Path.Combine(site.Root, name);
        if (!System.IO.File.Exists(path)) System.IO.File.WriteAllText(path, text ?? Text);
        var length = new FileInfo(path).Length;
        var page = Page(System.IO.File.ReadAllText(path), type, status);
        return c =>
        {
            c.Features.Set(new IisStaticResponse { PhysicalPath = path, Length = length, LastWriteTicks = System.IO.File.GetLastWriteTimeUtc(path).Ticks });
            return page(c);
        };
    }

    // --- The modules

    [Fact] // httpCompression's setup: "only static compression is installed by default" (no original server's settings: Windows' default)
    public async Task Without_the_server_settings_a_page_is_not_compressed()
    {
        using var site = new Site();
        var compression = new IisCompression(site.Root);
        Assert.True(compression.Any);
        Assert.False((await Run(compression)).Compressed);
    }

    [Fact] // the summary: the server's modules (its globalModules); without either, nothing compressed
    public void A_server_without_the_modules_compresses_nothing()
    {
        using var site = new Site(profile: Server(""));
        Assert.False(new IisCompression(site.Root).Any);
    }

    [Fact] // httpCompression: dynamic compression of a page, its type in dynamicTypes (text/*); Content-Encoding and Vary: Accept-Encoding, the whole answer's Content-Length
    public async Task A_page_is_compressed_by_the_dynamic_module()
    {
        using var site = new Site(profile: Server());
        var answered = await Run(new IisCompression(site.Root));
        Assert.Equal("gzip", answered.Encoding);
        Assert.Equal("Accept-Encoding", answered.Context.Response.Headers.Vary.ToString());
        Assert.Equal(answered.Raw.Length, answered.Context.Response.ContentLength);
        Assert.Equal(Text, answered.Body);
        var added = answered.Context.Features.Get<IisAddedHeaders>();
        Assert.True(added.Has("Content-Encoding") && added.Has("Vary"));
    }

    // --- The client

    [Fact] // httpCompression's note: a client that sends no Accept-Encoding gets the content uncompressed
    public async Task Without_accept_encoding_nothing_is_compressed()
    {
        using var site = new Site(profile: Server());
        var answered = await Run(new IisCompression(site.Root), accept: null);
        Assert.False(answered.Compressed);
        Assert.Equal(Text, answered.Body);
    }

    [Theory] // RFC 9110 12.5.3: q=0 is "not acceptable"; a coding's parameters other than q do not refuse it; names without regard to case
    [InlineData("gzip;q=0", null)]
    [InlineData("gzip; q=0.0, identity", null)]
    [InlineData("deflate, gzip;q=0.5", "gzip")]
    [InlineData("GZIP;Q=1", "gzip")]
    [InlineData("gzip;level=1", "gzip")]
    [InlineData("gzip;flag", "gzip")]
    [InlineData(",gzip,", "gzip")]
    [InlineData("identity", null)]
    public async Task The_client_accepts_the_scheme(string accept, string expected)
    {
        using var site = new Site(profile: Server());
        var answered = await Run(new IisCompression(site.Root), accept: accept);
        Assert.Equal(expected ?? "", answered.Encoding);
    }

    [Theory] // httpCompression's scheme elements (gzip, deflate; IIS 10's iisbrotli: br): the first of the server's the client takes (the summary: the server's order)
    [InlineData("deflate", "deflate")]
    [InlineData("br", "br")]
    [InlineData("br, deflate, gzip", "gzip")]
    [InlineData("deflate, br", "deflate")]
    public async Task The_server_schemes_in_its_order(string accept, string expected)
    {
        using var site = new Site(profile: Server(compression: "<httpCompression><scheme name='gzip' /><scheme name='Deflate' /><scheme name='br' /><scheme name='zstd' /></httpCompression>"));
        var answered = await Run(new IisCompression(site.Root), accept: accept);
        Assert.Equal(expected, answered.Encoding);
        Assert.Equal(Text, answered.Body);
    }

    [Fact] // the host's choice: a server whose schemes this host does not have keeps gzip (IIS's default scheme)
    public async Task Unknown_schemes_keep_gzip()
    {
        using var site = new Site(profile: Server(compression: "<httpCompression><scheme name='zstd' /><scheme /></httpCompression>"));
        Assert.Equal("gzip", (await Run(new IisCompression(site.Root), accept: "zstd, gzip")).Encoding);
    }

    [Fact] // httpCompression: noCompressionForHttp10 (default true); false compresses for HTTP/1.0 too
    public async Task Http10_is_not_compressed_by_default()
    {
        using var site = new Site(profile: Server());
        Assert.False((await Run(new IisCompression(site.Root), setup: c => c.Request.Protocol = "HTTP/1.0")).Compressed);
        using var off = new Site(profile: Server(compression: "<httpCompression noCompressionForHttp10='false' />"));
        Assert.True((await Run(new IisCompression(off.Root), setup: c => c.Request.Protocol = "HTTP/1.0")).Compressed);
    }

    [Fact] // httpCompression: noCompressionForProxies (default true): a request through a proxy (Via); false compresses it
    public async Task A_proxy_request_is_not_compressed_by_default()
    {
        using var site = new Site(profile: Server());
        Assert.False((await Run(new IisCompression(site.Root), setup: c => c.Request.Headers.Via = "1.1 proxy")).Compressed);
        using var off = new Site(profile: Server(compression: "<httpCompression noCompressionForProxies='false' />"));
        Assert.True((await Run(new IisCompression(off.Root), setup: c => c.Request.Headers.Via = "1.1 proxy")).Compressed);
    }

    [Fact] // httpCompression: noCompressionForRange (default true): a request with Range; false compresses it
    public async Task A_range_request_is_not_compressed_by_default()
    {
        using var site = new Site(profile: Server());
        Assert.False((await Run(new IisCompression(site.Root), setup: c => c.Request.Headers.Range = "bytes=0-9")).Compressed);
        using var off = new Site(profile: Server(compression: "<httpCompression noCompressionForRange='false' />"));
        Assert.True((await Run(new IisCompression(off.Root), setup: c => c.Request.Headers.Range = "bytes=0-9")).Compressed);
    }

    // --- The answer

    [Fact] // the host's choice (summary: checked on IIS): 200 answers only, not the errors nor the redirects
    public async Task An_error_is_not_compressed()
    {
        using var site = new Site(profile: Server());
        var answered = await Run(new IisCompression(site.Root), Page(status: 404));
        Assert.False(answered.Compressed);
        Assert.Equal(Text, answered.Body);
    }

    [Fact] // the host's choice: an answer the application encoded itself is left as it is
    public async Task An_encoded_answer_is_left()
    {
        using var site = new Site(profile: Server());
        var answered = await Run(new IisCompression(site.Root), async c =>
        {
            c.Response.Headers.ContentEncoding = "x-own";
            await Page()(c);
        });
        Assert.Equal("x-own", answered.Encoding);
        Assert.Equal(Text, Encoding.UTF8.GetString(answered.Raw));
    }

    [Theory] // httpCompression's dynamicTypes (applicationHost.config: text/*, message/*, application/x-javascript, application/javascript, */* off); the type without its parameters
    [InlineData("text/html; charset=utf-8", true)]
    [InlineData("TEXT/PLAIN", true)]
    [InlineData("message/http", true)]
    [InlineData("application/javascript", true)]
    [InlineData("application/x-javascript", true)]
    [InlineData("application/json", false)]
    [InlineData("image/svg+xml", false)]
    [InlineData("", false)]
    public async Task The_dynamic_types(string type, bool compressed)
    {
        using var site = new Site(profile: Server());
        Assert.Equal(compressed, (await Run(new IisCompression(site.Root), Page(type: type))).Compressed);
    }

    [Fact] // httpCompression's dynamicTypes: the first entry that matches decides (its enabled); clear, add (enabled's default true)
    public async Task The_server_dynamic_types_in_their_order()
    {
        using var site = new Site(profile: Server(compression: "<httpCompression><dynamicTypes><add mimeType='text/plain' />" +
            "<clear /><add mimeType='application/json' /><add mimeType='text/html' enabled='false' /><add mimeType='text/*' enabled='true' /><add enabled='true' /></dynamicTypes></httpCompression>"));
        var compression = new IisCompression(site.Root);
        Assert.True((await Run(compression, Page(type: "application/json"))).Compressed);
        Assert.False((await Run(compression, Page(type: "text/html"))).Compressed);
        Assert.True((await Run(compression, Page(type: "text/css"))).Compressed);
        Assert.False((await Run(compression, Page(type: "image/png"))).Compressed);
    }

    [Fact] // urlCompression: doDynamicCompression (default true); a folder's false
    public async Task A_folder_turns_dynamic_compression_off()
    {
        using var site = new Site("<configuration><location path='raw'><system.webServer><urlCompression doDynamicCompression='false' /></system.webServer></location></configuration>",
            Server());
        var compression = new IisCompression(site.Root);
        Assert.False((await Run(compression, target: "/raw/page.aspx")).Compressed);
        Assert.True((await Run(compression, target: "/page.aspx")).Compressed);
        Assert.False((await Run(compression, target: "/raw/other.aspx")).Compressed);
    }

    [Fact] // HTTP's HEAD: the headers of the compressed answer (its length), no body
    public async Task A_head_request_has_the_compressed_length()
    {
        using var site = new Site(profile: Server());
        var answered = await Run(new IisCompression(site.Root), method: "HEAD");
        Assert.Equal("gzip", answered.Encoding);
        Assert.True(answered.Context.Response.ContentLength > 0);
        Assert.Empty(answered.Raw);
    }

    [Fact] // the summary: once the application flushes, compressed as it goes (no Content-Length: chunked)
    public async Task A_flushed_answer_is_compressed_as_it_goes()
    {
        using var site = new Site(profile: Server());
        var answered = await Run(new IisCompression(site.Root), async c =>
        {
            await c.Response.Body.FlushAsync();   // nothing written yet: nothing to send
            c.Response.ContentType = "text/plain";
            await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("first "));
            await c.Response.Body.FlushAsync();
            c.Response.Body.Flush();
            c.Response.Body.Write(Encoding.UTF8.GetBytes("second "));
            await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("third"), 0, 5);
        });
        Assert.Equal("gzip", answered.Encoding);
        Assert.Null(answered.Context.Response.ContentLength);
        Assert.Equal("first second third", answered.Body);
    }

    [Fact] // the summary: the whole answer compressed, written as the application writes (its synchronous writes too)
    public async Task Synchronous_writes_are_compressed()
    {
        using var site = new Site(profile: Server());
        var answered = await Run(new IisCompression(site.Root), c =>
        {
            c.Response.ContentType = "text/plain";
            c.Response.Body.Write(Encoding.UTF8.GetBytes("sync"), 0, 4);
            return Task.CompletedTask;
        });
        Assert.Equal("gzip", answered.Encoding);
        Assert.Equal("sync", answered.Body);
    }

    [Fact] // the host's choice: an answer flushed and not compressed goes on as it is
    public async Task A_flushed_answer_not_compressed_goes_as_it_is()
    {
        using var site = new Site(profile: Server());
        var answered = await Run(new IisCompression(site.Root), async c =>
        {
            c.Response.ContentType = "image/png";
            c.Response.Body.Write(Encoding.UTF8.GetBytes("png "), 0, 4);
            await c.Response.Body.FlushAsync();
            await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("data"));
            await c.Response.Body.WriteAsync(Array.Empty<byte>());
            c.Response.Body.Write(Array.Empty<byte>(), 0, 0);
        });
        Assert.False(answered.Compressed);
        Assert.Equal("png data", Encoding.UTF8.GetString(answered.Raw));
    }

    [Fact] // the host's choice: what the application writes through the body's PipeWriter compressed as the rest
    public async Task The_body_writer_is_compressed()
    {
        using var site = new Site(profile: Server());
        var answered = await Run(new IisCompression(site.Root), async c =>
        {
            c.Response.ContentType = "text/plain";
            await c.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("piped"));
        });
        Assert.Equal("gzip", answered.Encoding);
        Assert.Equal("piped", answered.Body);
    }

    [Fact] // the host's choice: a file sent (SendFileAsync: whole, a part, past its end) compressed as the rest; before and after a flush
    public async Task A_sent_file_is_compressed()
    {
        using var site = new Site(profile: Server());
        var path = Path.Combine(site.Root, "f.txt");
        System.IO.File.WriteAllText(path, "0123456789");
        var answered = await Run(new IisCompression(site.Root), async c =>
        {
            c.Response.ContentType = "text/plain";
            await c.Response.SendFileAsync(path, 0, null);
            await c.Response.SendFileAsync(path, 2, 3);
            await c.Response.Body.FlushAsync();
            await c.Response.SendFileAsync(path, 8, 100);
        });
        Assert.Equal("gzip", answered.Encoding);
        Assert.Equal("012345678923489", answered.Body);
    }

    [Fact] // the host's choice: a file not compressed is sent by the server as it is
    public async Task A_sent_file_not_compressed_is_sent_as_it_is()
    {
        using var site = new Site(profile: Server());
        var path = Path.Combine(site.Root, "f.png");
        System.IO.File.WriteAllText(path, "0123456789");
        var answered = await Run(new IisCompression(site.Root), async c =>
        {
            c.Response.ContentType = "image/png";
            await c.Response.SendFileAsync(path, 0, null);
        });
        Assert.False(answered.Compressed);
        Assert.Equal("0123456789", Encoding.UTF8.GetString(answered.Raw));
    }

    [Fact] // the host's choice: starting the response decides it; completing it before the end leaves the compressed answer to the middleware
    public async Task Start_and_complete_are_left_to_the_end()
    {
        using var site = new Site(profile: Server());
        var compression = new IisCompression(site.Root);
        var answered = await Run(compression, async c =>
        {
            c.Response.ContentType = "text/plain";
            await c.Response.StartAsync();
            await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("done"));
            await c.Response.CompleteAsync();
        });
        Assert.Equal("done", answered.Body);
        var plain = await Run(compression, async c =>
        {
            c.Response.ContentType = "image/png";
            c.Features.Get<IHttpResponseBodyFeature>().DisableBuffering();
            await c.Response.StartAsync();
            await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("png"));
            await c.Response.CompleteAsync();
        });
        Assert.Equal("png", Encoding.UTF8.GetString(plain.Raw));
        var empty = await Run(compression, c => c.Response.CompleteAsync());
        Assert.Empty(empty.Raw);
    }

    [Fact] // the host's choice: the body is a stream to write only
    public async Task The_body_is_write_only()
    {
        using var site = new Site(profile: Server());
        await Run(new IisCompression(site.Root), c =>
        {
            var body = c.Response.Body;
            Assert.False(body.CanRead);
            Assert.False(body.CanSeek);
            Assert.True(body.CanWrite);
            Assert.Throws<NotSupportedException>(() => body.Length);
            Assert.Throws<NotSupportedException>(() => body.Position);
            Assert.Throws<NotSupportedException>(() => body.Position = 0);
            Assert.Throws<NotSupportedException>(() => body.Read(new byte[1], 0, 1));
            Assert.Throws<NotSupportedException>(() => body.Seek(0, SeekOrigin.Begin));
            Assert.Throws<NotSupportedException>(() => body.SetLength(0));
            return Task.CompletedTask;
        });
    }

    // --- Static files

    [Fact] // httpCompression's staticCompressionIgnoreHitFrequency: a static file compressed once hit twice in 10 seconds (serverRuntime's frequentHitThreshold 2, frequentHitTimePeriod 10 s)
    public async Task A_static_file_is_compressed_once_frequent()
    {
        using var site = new Site();
        var compression = new IisCompression(site.Root);
        var first = await Run(compression, File(site), "/a.txt");
        Assert.False(first.Compressed);
        var second = await Run(compression, File(site), "/a.txt");
        Assert.Equal("gzip", second.Encoding);
        Assert.Equal(Text, second.Body);
        // the host's choice (summary: IIS's "IIS Temporary Compressed Files"): the compressed file kept
        var third = await Run(compression, File(site), "/a.txt");
        Assert.Equal(second.Raw, third.Raw);
    }

    [Fact] // the summary: a static file's hits counted whatever the client takes (IIS's file cache counts them all), also when the application answers later
    public async Task A_static_hit_is_counted_without_compression()
    {
        using var site = new Site();
        var compression = new IisCompression(site.Root);
        var file = File(site);
        Assert.False((await Run(compression, async c => { await Task.Yield(); await file(c); }, "/a.txt", accept: null)).Compressed);
        Assert.True((await Run(compression, file, "/a.txt")).Compressed);
    }

    [Fact] // the host's choice: an error's answer (a static file's 404) is not a hit
    public async Task A_static_error_is_not_a_hit()
    {
        using var site = new Site();
        var compression = new IisCompression(site.Root);
        await Run(compression, File(site, status: 404), "/a.txt", accept: null);
        Assert.False((await Run(compression, File(site), "/a.txt")).Compressed);
    }

    [Fact] // httpCompression: staticCompressionIgnoreHitFrequency true: compressed at the first hit
    public async Task Ignore_hit_frequency_compresses_the_first_hit()
    {
        using var site = new Site(profile: Server("<globalModules><add name='StaticCompressionModule' /></globalModules>",
            "<httpCompression staticCompressionIgnoreHitFrequency='true' />"));
        var compression = new IisCompression(site.Root);
        await Run(compression, File(site), "/a.txt", accept: null);
        Assert.True((await Run(compression, File(site), "/a.txt")).Compressed);
    }

    [Fact] // httpCompression: minFileSizeForComp (2,700 bytes): a smaller file is not compressed; the server's value
    public async Task A_small_static_file_is_not_compressed()
    {
        using var site = new Site();
        var compression = new IisCompression(site.Root);
        var small = File(site, text: "small");
        await Run(compression, small, "/a.txt");
        Assert.False((await Run(compression, small, "/a.txt")).Compressed);
        using var low = new Site(profile: Server(Both, "<httpCompression minFileSizeForComp='3' staticCompressionIgnoreHitFrequency='true' />"));
        Assert.True((await Run(new IisCompression(low.Root), File(low, text: "small"), "/a.txt")).Compressed);
    }

    [Theory] // httpCompression's staticTypes (applicationHost.config: text/*, message/*, application/javascript, application/atom+xml, application/xaml+xml, image/svg+xml, */* off)
    [InlineData("text/css", true)]
    [InlineData("application/javascript", true)]
    [InlineData("application/atom+xml", true)]
    [InlineData("image/svg+xml", true)]
    [InlineData("image/png", false)]
    [InlineData("application/x-javascript", false)]
    public async Task The_static_types(string type, bool compressed)
    {
        using var site = new Site(profile: Server("<globalModules><add name='StaticCompressionModule' /></globalModules>", "<httpCompression staticCompressionIgnoreHitFrequency='true' />"));
        Assert.Equal(compressed, (await Run(new IisCompression(site.Root), File(site, type: type), "/a.txt")).Compressed);
    }

    [Fact] // httpCompression's staticTypes: the server's list replaces the default
    public async Task The_server_static_types()
    {
        using var site = new Site(profile: Server("<globalModules><add name='StaticCompressionModule' /></globalModules>",
            "<httpCompression staticCompressionIgnoreHitFrequency='true'><staticTypes><clear /><add mimeType='image/png' /></staticTypes></httpCompression>"));
        var compression = new IisCompression(site.Root);
        Assert.True((await Run(compression, File(site, type: "image/png"), "/a.txt")).Compressed);
        Assert.False((await Run(compression, File(site, "b.txt", type: "text/plain"), "/b.txt")).Compressed);
    }

    [Fact] // the summary: a static file before it is frequent, dynamic compression if the server has it
    public async Task A_static_file_not_yet_frequent_has_dynamic_compression()
    {
        using var site = new Site(profile: Server(Both));
        Assert.True((await Run(new IisCompression(site.Root), File(site), "/a.txt")).Compressed);
        using var dynamicOnly = new Site(profile: Server(Dynamic, "<httpCompression staticCompressionIgnoreHitFrequency='true' />"));
        Assert.True((await Run(new IisCompression(dynamicOnly.Root), File(dynamicOnly), "/a.txt")).Compressed);
    }

    [Fact] // urlCompression: doStaticCompression false: a folder's static files not compressed
    public async Task A_folder_turns_static_compression_off()
    {
        using var site = new Site(Site.WebServer("<urlCompression doStaticCompression='false' />"),
            Server("<globalModules><add name='StaticCompressionModule' /></globalModules>", "<httpCompression staticCompressionIgnoreHitFrequency='true' />"));
        Assert.False((await Run(new IisCompression(site.Root), File(site), "/a.txt")).Compressed);
    }

    [Fact] // serverRuntime: frequentHitThreshold (a folder's 1: the first hit)
    public async Task The_frequent_hit_threshold()
    {
        using var site = new Site(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        Assert.True((await Run(new IisCompression(site.Root), File(site), "/a.txt")).Compressed);
    }

    [Fact] // serverRuntime: frequentHitTimePeriod: hits further apart than it are not frequent
    public async Task The_frequent_hit_time_period()
    {
        using var site = new Site(Site.WebServer("<serverRuntime frequentHitTimePeriod='00:00:00.001' />"));
        var compression = new IisCompression(site.Root);
        await Run(compression, File(site), "/a.txt");
        Thread.Sleep(50);
        Assert.False((await Run(compression, File(site), "/a.txt")).Compressed);
    }

    [Fact] // the host's choice: the hits of at most 10,000 files kept (then counted again from none)
    public async Task The_hits_kept_are_limited()
    {
        using var site = new Site();
        var compression = new IisCompression(site.Root);
        var file = File(site);
        await Run(compression, file, "/a.txt", accept: null);
        for (var i = 0; i < 10000; i++)
        {
            var context = Site.Request("/x.txt");
            await new IisCompression.Middleware(c =>
            {
                c.Features.Set(new IisStaticResponse { PhysicalPath = "x" + i });
                return Task.CompletedTask;
            }, compression).Invoke(context);
        }
        Assert.False((await Run(compression, file, "/a.txt")).Compressed);
    }
}
