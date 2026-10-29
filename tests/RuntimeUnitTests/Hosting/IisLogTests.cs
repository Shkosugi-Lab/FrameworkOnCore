using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Web.Hosting;
using Microsoft.AspNetCore.Http;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// IIS's W3C log as the host writes it (IisLogWriter, IisLogMiddleware, IisLogStatus), against Microsoft's "Log Files for a
/// Web Site &lt;logFile&gt;" (learn.microsoft.com/iis/configuration/system.applicationhost/sites/site/logfile/, "logFile"
/// below), "W3C Logging" of the HTTP Server API (learn.microsoft.com/windows/win32/http/w3c-logging, "W3C Logging": its
/// fields in their order, '-' and '+', the header) and IIS 10's IIS_schema.xml (logExtFileFlags' default).
/// </summary>
public class IisLogTests
{
    const IisLogFields All = (IisLogFields)((1 << 22) - 1);

    static IisLogOptions Options(Site site, IisLogFields fields = IisLogFields.Default) =>
        new IisLogOptions { Directory = Path.Combine(site.Root, "logs"), Fields = fields };

    static string[] Lines(string directory, string file) =>
        File.ReadAllText(Path.Combine(directory, file)).Split("\r\n");

    // --- The file

    [Fact] // W3C Logging: the header (#Software, #Version 1.0, #Date, #Fields), then the lines; logFile: UTC; the host's choice (summary): UTF-8 without a BOM, CRLF, IIS's #Software; u_exYYMMDD.log (IIS's name for a day's file)
    public async Task A_file_has_the_header_then_the_lines()
    {
        using var site = new Site();
        var options = Options(site);
        var writer = new IisLogWriter(options);
        var when = new DateTime(2026, 10, 29, 1, 2, 3, DateTimeKind.Utc);
        await writer.WriteAsync(when, "first");
        await writer.WriteAsync(when.AddMinutes(1), "second");
        await writer.DisposeAsync();
        var bytes = File.ReadAllBytes(Path.Combine(options.Directory, "u_ex261029.log"));
        Assert.NotEqual(0xEF, bytes[0]);
        Assert.Equal(new[]
        {
            "#Software: Microsoft Internet Information Services 10.0",
            "#Version: 1.0",
            "#Date: 2026-10-29 01:02:03",
            "#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status sc-bytes cs-bytes time-taken",
            "first", "second", "",
        }, Encoding.UTF8.GetString(bytes).Split("\r\n"));
    }

    [Fact] // W3C Logging: the fields in the order of its table, whichever are chosen
    public async Task All_the_fields_are_in_their_order()
    {
        using var site = new Site();
        var options = Options(site, All);
        var writer = new IisLogWriter(options);
        await writer.WriteAsync(new DateTime(2026, 10, 29, 0, 0, 0, DateTimeKind.Utc), "x");
        await writer.DisposeAsync();
        Assert.Equal("#Fields: date time s-sitename s-computername s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs-version " +
            "cs(User-Agent) cs(Cookie) cs(Referer) cs-host sc-status sc-substatus sc-win32-status sc-bytes cs-bytes time-taken",
            Lines(options.Directory, "u_ex261029.log")[3]);
    }

    [Fact] // logFile: period Daily (a new file a day); the host's choice (summary): a header each time a file is opened
    public async Task A_new_day_has_a_new_file()
    {
        using var site = new Site();
        var options = Options(site);
        var writer = new IisLogWriter(options);
        await writer.WriteAsync(new DateTime(2026, 10, 29, 23, 59, 59, DateTimeKind.Utc), "a");
        await writer.WriteAsync(new DateTime(2026, 10, 30, 0, 0, 0, DateTimeKind.Utc), "b");
        await writer.DisposeAsync();
        Assert.Equal("a", Lines(options.Directory, "u_ex261029.log")[4]);
        Assert.Equal("b", Lines(options.Directory, "u_ex261030.log")[4]);
        // the same file opened again (the next process): its header again, the lines appended
        writer = new IisLogWriter(options);
        await writer.WriteAsync(new DateTime(2026, 10, 30, 1, 0, 0, DateTimeKind.Utc), "c");
        await writer.DisposeAsync();
        var lines = Lines(options.Directory, "u_ex261030.log");
        Assert.Equal(new[] { "b", "#Software: Microsoft Internet Information Services 10.0" }, lines[4..6]);
        Assert.Equal("c", lines[9]);
    }

    [Theory] // logFile: period Hourly and Monthly (the host's choice: IIS's names u_exYYMMDDHH.log, u_exYYMM.log)
    [InlineData("Hourly", "u_ex26102913.log")]
    [InlineData("Monthly", "u_ex2610.log")]
    [InlineData("Daily", "u_ex261029.log")]
    public async Task The_period_names_the_file(string period, string name)
    {
        using var site = new Site();
        var options = Options(site);
        options.Period = Enum.Parse<IisLogPeriod>(period);
        var writer = new IisLogWriter(options);
        await writer.WriteAsync(new DateTime(2026, 10, 29, 13, 0, 0, DateTimeKind.Utc), "a");
        await writer.DisposeAsync();
        Assert.True(File.Exists(Path.Combine(options.Directory, name)));
    }

    [Fact] // logFile: localTimeRollover: the file named by the local time; the lines' times UTC regardless
    public async Task Local_time_rollover_names_the_file_by_the_local_time()
    {
        using var site = new Site();
        var options = Options(site);
        options.Period = IisLogPeriod.Hourly;
        options.LocalTimeRollover = true;
        var writer = new IisLogWriter(options);
        var when = new DateTime(2026, 10, 29, 13, 0, 0, DateTimeKind.Utc);
        await writer.WriteAsync(when, "a");
        await writer.DisposeAsync();
        var local = TimeZoneInfo.ConvertTimeFromUtc(when, TimeZoneInfo.Local);
        Assert.Equal("#Date: 2026-10-29 13:00:00", Lines(options.Directory, "u_ex" + local.ToString("yyMMddHH", CultureInfo.InvariantCulture) + ".log")[2]);
    }

    [Fact] // the host's choice (summary): a log that cannot be written is told (the console), the requests go on
    public async Task A_log_that_cannot_be_written_does_not_stop_the_requests()
    {
        using var site = new Site();
        site.Write("logs", "a file where the folder would be");
        var writer = new IisLogWriter(Options(site));
        await writer.WriteAsync(DateTime.UtcNow, "a");
        await writer.DisposeAsync();
        await writer.DisposeAsync();   // once only
        Assert.True(File.Exists(Path.Combine(site.Root, "logs")));
    }

    // --- logExtFileFlags

    [Fact] // IIS 10's IIS_schema.xml: logExtFileFlags' default (Date, Time, ClientIP, UserName, SiteName, ServerIP, Method, UriStem, UriQuery, TimeTaken, HttpStatus, Win32Status, BytesSent, BytesRecv, ServerPort, UserAgent, HttpSubStatus, Referer) without SiteName (logFile's logSiteId: a site's own file has no s-sitename)
    public void The_default_fields_are_IIS_10s()
    {
        Assert.Equal(IisLogWriter.ParseFields("Date, Time, ClientIP, UserName, ServerIP, Method, UriStem, UriQuery, TimeTaken, HttpStatus, Win32Status, BytesSent, BytesRecv, ServerPort, UserAgent, HttpSubStatus, Referer"),
            IisLogFields.Default);
    }

    [Fact] // logFile's logExtFileFlags: the names, comma separated (the host's choice: without regard to case, spaces allowed, empty entries skipped)
    public void Fields_are_parsed_from_the_flags()
    {
        Assert.Equal(IisLogFields.Date | IisLogFields.ClientIP | IisLogFields.Cookie, IisLogWriter.ParseFields(" date ,ClientIP,, Cookie"));
        Assert.Equal(IisLogFields.None, IisLogWriter.ParseFields(""));
    }

    [Theory] // the host's choice: a name logExtFileFlags does not have (or this enum's Default) is refused
    [InlineData("Date, Bogus")]
    [InlineData("Default")]
    public void An_unknown_field_is_refused(string flags) => Assert.Null(IisLogWriter.ParseFields(flags));

    // --- Escaping

    [Theory] // W3C Logging: a hyphen for a field with no information; logFile's IIS format: a plus sign for a space (W3C: spaces separate the fields)
    [InlineData(null, "-")]
    [InlineData("", "-")]
    [InlineData("a b  c", "a+b++c")]
    [InlineData("plain", "plain")]
    public void Values_are_escaped(string value, string escaped) => Assert.Equal(escaped, IisLogWriter.Escape(value));

    [Fact]
    // W3C Logging: "If a field contains a nonprintable character, the HTTP Server API replaces it with a plus sign (+)" (a tab in User-Agent: observed on IIS 10)
    public void A_nonprintable_character_is_a_plus() => Assert.Equal("a+b", IisLogWriter.Escape("a\tb"));

    [Fact] // W3C Logging: the other control characters too; logFile's logInUTF8 (default true): a character beyond ASCII as it is
    public void Control_characters_are_plus_and_others_kept() => Assert.Equal("a+b+c+d+é", IisLogWriter.Escape("a\u0001b\u007fc\rd é"));

    // --- The middleware

    /// <summary>A request through the middleware, its response completed, the log written: its line's fields by name (null: no line).</summary>
    static async Task<Dictionary<string, string>> Log(Site site, RequestDelegate app = null, string target = "/a.aspx?x=1", string method = "GET",
        Action<DefaultHttpContext> setup = null, IisLogFields fields = All, Action<HttpContext> after = null)
    {
        var options = Options(site, fields);
        options.Directory += Guid.NewGuid().ToString("N");   // each request's own file
        var writer = new IisLogWriter(options);
        var context = Site.Request(target, method);
        var response = Site.StartingResponse.On(context);
        context.Connection.LocalPort = 8080;
        setup?.Invoke(context);
        await new IisLogMiddleware(app ?? (c => c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("hello")).AsTask()), writer).Invoke(context);
        after?.Invoke(context);
        await response.CompleteAsync();
        await writer.DisposeAsync();
        if (!Directory.Exists(options.Directory)) return null;
        var lines = Lines(options.Directory, Directory.GetFiles(options.Directory).Single());
        var names = lines[3].Substring("#Fields: ".Length).Split(' ');
        var values = lines[4].Split(' ');
        Assert.Equal(names.Length, values.Length);
        return names.Zip(values).ToDictionary(p => p.First, p => p.Second);
    }

    [Fact] // W3C Logging's fields of a request: its date and time (UTC), the site (W3SVC1), the server, the addresses (IPv4 as IIS has them), the method, stem, query, port, version, headers, status
    public async Task A_request_is_logged_with_its_fields()
    {
        var before = DateTime.UtcNow;
        using var site = new Site();
        var line = await Log(site, setup: c =>
        {
            c.Request.Headers.UserAgent = "Mozilla/5.0 (Windows NT 10.0)";
            c.Request.Headers.Cookie = "a=1; b=2";
            c.Request.Headers.Referer = "http://localhost:8080/start.aspx";
        });
        var when = DateTime.ParseExact(line["date"] + " " + line["time"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        Assert.InRange(when, before.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        Assert.Equal("W3SVC1", line["s-sitename"]);
        Assert.Equal(Environment.MachineName, line["s-computername"]);
        Assert.Equal("127.0.0.1", line["s-ip"]);
        Assert.Equal("GET", line["cs-method"]);
        Assert.Equal("/a.aspx", line["cs-uri-stem"]);
        Assert.Equal("x=1", line["cs-uri-query"]);
        Assert.Equal("8080", line["s-port"]);
        Assert.Equal("-", line["cs-username"]);
        Assert.Equal("192.168.1.10", line["c-ip"]);
        Assert.Equal("HTTP/1.1", line["cs-version"]);
        Assert.Equal("Mozilla/5.0+(Windows+NT+10.0)", line["cs(User-Agent)"]);
        Assert.Equal("a=1;+b=2", line["cs(Cookie)"]);
        Assert.Equal("http://localhost:8080/start.aspx", line["cs(Referer)"]);
        Assert.Equal("localhost:8080", line["cs-host"]);
        Assert.Equal("200", line["sc-status"]);
        Assert.Equal("0", line["sc-substatus"]);
        Assert.Equal("0", line["sc-win32-status"]);
        Assert.True(long.TryParse(line["time-taken"], out var taken) && taken >= 0);
    }

    [Fact] // W3C Logging: '-' where there is nothing (no query, no server address, no user agent, an anonymous user)
    public async Task Missing_values_are_hyphens()
    {
        using var site = new Site();
        var line = await Log(site, target: "/a.aspx", setup: c => { c.Connection.LocalIpAddress = null; c.User = new ClaimsPrincipal(); });
        Assert.Equal("-", line["cs-uri-query"]);
        Assert.Equal("-", line["s-ip"]);
        Assert.Equal("-", line["cs(User-Agent)"]);
        Assert.Equal("-", line["cs(Referer)"]);
        Assert.Equal("-", line["cs-username"]);
        line = await Log(site, target: "/a.aspx?", setup: c => c.User = null);
        Assert.Equal("-", line["cs-uri-query"]);
        Assert.Equal("-", line["cs-username"]);
    }

    [Fact] // logFile's UserName: the authenticated user's name
    public async Task The_authenticated_user_is_logged()
    {
        using var site = new Site();
        var line = await Log(site, setup: c => c.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "DOMAIN\\some one") }, "Forms")));
        Assert.Equal("DOMAIN\\some+one", line["cs-username"]);
    }

    [Fact] // the summary: the URL as the client asked for it (the application's base and path), not what the application made of it
    public async Task The_url_asked_for_is_logged()
    {
        using var site = new Site();
        var line = await Log(site, c =>
        {
            c.Request.Path = "/rewritten.aspx";
            c.Request.QueryString = new QueryString("?y=2");
            return Task.CompletedTask;
        }, setup: c => c.Request.PathBase = "/app");
        Assert.Equal("/app/a.aspx", line["cs-uri-stem"]);
        Assert.Equal("x=1", line["cs-uri-query"]);
    }

    [Fact] // the host's choice (summary: URL Rewrite's logRewrittenUrl): the rewritten URL logged where the rules say
    public async Task The_rewritten_url_is_logged_when_set()
    {
        using var site = new Site();
        var line = await Log(site, c =>
        {
            IisLogStatus.SetLoggedUrl(c, "/b.aspx", "?z=3");
            return Task.CompletedTask;
        });
        Assert.Equal("/b.aspx", line["cs-uri-stem"]);
        Assert.Equal("z=3", line["cs-uri-query"]);
        line = await Log(site, c =>
        {
            IisLogStatus.Set(c, 0);
            IisLogStatus.SetLoggedUrl(c, "/b.aspx", null);
            return Task.CompletedTask;
        });
        Assert.Equal("-", line["cs-uri-query"]);
    }

    [Fact] // logFile's HttpSubStatus and Win32Status: what IIS answered (404.3: the MIME map's, Win32 50)
    public async Task The_substatus_and_win32_status_are_logged()
    {
        using var site = new Site();
        var line = await Log(site, c =>
        {
            c.Response.StatusCode = 404;
            IisLogStatus.SetLoggedUrl(c, null, null);
            IisLogStatus.Set(c, 3, 50);
            return Task.CompletedTask;
        });
        Assert.Equal("404", line["sc-status"]);
        Assert.Equal("3", line["sc-substatus"]);
        Assert.Equal("50", line["sc-win32-status"]);
        Assert.Equal("/a.aspx", line["cs-uri-stem"]);
    }

    [Fact] // the host's choice (summary): the status IIS logs where it sent another (httpErrors' Redirect: the error's, not the 302)
    public async Task The_logged_status_is_the_error()
    {
        using var site = new Site();
        var line = await Log(site, c =>
        {
            c.Response.StatusCode = 302;
            c.Features.Set(new IisLogStatus { LoggedStatus = 404 });
            return Task.CompletedTask;
        });
        Assert.Equal("404", line["sc-status"]);
    }

    [Fact] // the host's choice (summary): what http.sys refuses itself goes to its HTTPERR log, not the site's
    public async Task A_request_refused_by_http_sys_is_not_logged()
    {
        using var site = new Site();
        Assert.Null(await Log(site, c =>
        {
            IisLogStatus.SetNotLogged(c);
            IisLogStatus.SetNotLogged(c);
            return Task.CompletedTask;
        }));
    }

    [Fact] // the host's choice: the status set where there is no request (no context) is nothing
    public void Without_a_request_the_status_is_not_set()
    {
        IisLogStatus.Set(null, 3);
        IisLogStatus.SetLoggedUrl(null, "/a", null);
        IisLogStatus.SetNotLogged(null);
    }

    [Fact] // W3C Logging: sc-bytes, the bytes the server sent (the summary: its status line, headers and body); cs-bytes, the bytes received (the request line as sent, headers, body)
    public async Task The_bytes_sent_and_received_are_counted()
    {
        using var site = new Site();
        var line = await Log(site, async c =>
        {
            c.Response.ContentType = "text/plain";
            await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("hel"), 0, 3);
            c.Response.Body.Write(Encoding.UTF8.GetBytes("lo"));
        }, "/a%20b.aspx?x=1", "POST", c => c.Request.ContentLength = 7);
        // "HTTP/1.1 200 OK\r\n" "Content-Type: text/plain\r\n" "\r\n" "hello"
        Assert.Equal((17 + 26 + 2 + 5).ToString(CultureInfo.InvariantCulture), line["sc-bytes"]);
        // "POST /a%20b.aspx?x=1 HTTP/1.1\r\n" "Host: localhost:8080\r\n" "Content-Length: 7\r\n" "\r\n" and the body
        Assert.Equal((31 + 22 + 19 + 2 + 7).ToString(CultureInfo.InvariantCulture), line["cs-bytes"]);
    }

    [Fact] // the host's choice: without sc-bytes the response's body is left as it is (nothing counted)
    public async Task Without_bytes_sent_the_body_is_left()
    {
        using var site = new Site();
        Stream body = null;
        var line = await Log(site, fields: IisLogFields.Default & ~IisLogFields.BytesSent, after: c => body = c.Response.Body);
        Assert.IsType<MemoryStream>(body);
        Assert.False(line.ContainsKey("sc-bytes"));
    }

    [Fact] // the host's choice: the counting stream writes through to the response's, and does not close it
    public async Task The_counting_stream_writes_through()
    {
        var inner = new MemoryStream();
        var counting = new IisLogMiddleware.CountingStream(inner);
        counting.Write(new byte[] { 1, 2 }, 0, 2);
        counting.Write(new byte[] { 3 }.AsSpan());
        await counting.WriteAsync(new byte[] { 4, 5, 6 }, 0, 3);
        await counting.WriteAsync(new ReadOnlyMemory<byte>(new byte[] { 7 }));
        counting.Flush();
        await counting.FlushAsync();
        counting.Dispose();
        Assert.Equal(7, counting.Count);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, inner.ToArray());
        Assert.True(inner.CanWrite);
        Assert.False(counting.CanRead);
        Assert.False(counting.CanSeek);
        Assert.True(counting.CanWrite);
        Assert.Throws<NotSupportedException>(() => counting.Length);
        Assert.Throws<NotSupportedException>(() => counting.Position);
        Assert.Throws<NotSupportedException>(() => counting.Position = 0);
        Assert.Throws<NotSupportedException>(() => counting.Read(new byte[1], 0, 1));
        Assert.Throws<NotSupportedException>(() => counting.Seek(0, SeekOrigin.Begin));
        Assert.Throws<NotSupportedException>(() => counting.SetLength(0));
    }
}
