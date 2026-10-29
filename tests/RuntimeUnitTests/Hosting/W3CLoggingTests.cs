using System.Reflection;
using System.Web.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The host's W3C log set up as IIS logs (WebFormsW3CLoggingExtensions), against Microsoft's "Log Files for a Web Site
/// &lt;logFile&gt;" (learn.microsoft.com/iis/configuration/system.applicationhost/sites/site/logfile/, "logFile" below) and
/// "HTTP Logging &lt;httpLogging&gt;" (.../system.webserver/httplogging, "httpLogging"); the configuration's settings are the
/// host's (its summary).
/// </summary>
public class W3CLoggingTests
{
    static readonly Type On = typeof(WebFormsW3CLoggingExtensions).GetNestedType("W3CLoggingOn", BindingFlags.NonPublic);

    sealed class Built : IAsyncDisposable
    {
        public WebApplication App;
        public object On => App.Services.GetService(W3CLoggingTests.On);
        public IisLogWriter Writer => (IisLogWriter)W3CLoggingTests.On.GetField("Iis").GetValue(On);
        public string Directory => (string)W3CLoggingTests.On.GetField("Directory").GetValue(On);

        public async ValueTask DisposeAsync()
        {
            if (On != null && Writer != null) await Writer.DisposeAsync();
            await App.DisposeAsync();
        }
    }

    /// <summary>The application's builder for the folder, with the configuration's settings (the log in the folder's "logs" unless said).</summary>
    static Built Build(Site site, params (string Key, string Value)[] settings)
    {
        var values = settings.ToDictionary(s => s.Key, s => s.Value);
        if (!values.ContainsKey("W3CLogging:LogDirectory")) values["W3CLogging:LogDirectory"] = "logs";
        else if (values["W3CLogging:LogDirectory"] == null) values.Remove("W3CLogging:LogDirectory");
        return new Built { App = Site.Application(site.Root, values, builder => builder.AddWebFormsW3CLogging()) };
    }

    static string LogFile(string profileLogFile, string more = "") =>
        "<profile><site id='3'><logFile " + profileLogFile + " /></site>" + more + "</profile>";

    // --- On or off

    [Fact] // logFile: enabled's default true: IIS logs the requests (Iis mode, the default: IIS's own writer), in the folder said
    public async Task The_log_is_on_by_default()
    {
        using var site = new Site();
        await using var built = Build(site);
        Assert.NotNull(built.Writer);
        Assert.Equal(Path.Combine(site.Root, "logs"), built.Directory);
        Assert.Equal(IisLogFields.Default, built.Writer.Options.Fields);
        Assert.Equal(IisLogPeriod.Daily, built.Writer.Options.Period);
        Assert.Equal("W3SVC1", built.Writer.Options.SiteName);
    }

    [Theory] // the host's choice (summary): W3CLogging:Enabled over web.config and the server's settings
    [InlineData("false", false)]
    [InlineData("true", true)]
    public async Task The_configuration_turns_the_log_on_or_off(string enabled, bool on)
    {
        using var site = new Site(Site.WebServer("<httpLogging dontLog='true' />"));
        await using var built = Build(site, ("W3CLogging:Enabled", enabled));
        Assert.Equal(on, built.On != null);
    }

    [Fact] // logFile: enabled false (the site's, or the sites' defaults) turns the site's log off
    public async Task The_server_turns_the_log_off()
    {
        using var site = new Site(profile: "<profile><siteDefaults><logFile enabled='False' /></siteDefaults></profile>");
        await using var built = Build(site);
        Assert.Null(built.On);
        using var on = new Site(profile: "<profile><siteDefaults><logFile enabled='false' /></siteDefaults><site><logFile enabled='true' /></site></profile>");
        await using var builtOn = Build(on);
        Assert.NotNull(builtOn.On);
    }

    [Fact] // httpLogging: dontLog true turns logging off; the application's web.config over the server's
    public async Task Dont_log_turns_the_log_off()
    {
        using var server = new Site(profile: Site.Profile("<httpLogging dontLog='true' />"));
        await using (var built = Build(server)) Assert.Null(built.On);
        using var application = new Site(Site.WebServer("<httpLogging dontLog='false' />"), Site.Profile("<httpLogging dontLog='true' />"));
        await using (var built = Build(application)) Assert.NotNull(built.On);
        using var location = new Site("<configuration><location path=''><system.webServer><httpLogging dontLog='true' /></system.webServer></location>" +
            "<location path='sub'><system.webServer><httpLogging dontLog='false' /></system.webServer></location></configuration>");
        await using (var built = Build(location)) Assert.Null(built.On);
        using var unsaid = new Site(Site.WebServer("<httpLogging dontLog='maybe' />"), Site.Profile("<httpLogging dontLog='true' />"));
        await using (var built = Build(unsaid)) Assert.Null(built.On);
    }

    [Fact] // the host's choice: a web.config that is not XML leaves the server's setting (the application fails on it elsewhere)
    public async Task A_broken_web_config_leaves_the_server_setting()
    {
        using var site = new Site("<configuration>", Site.Profile("<httpLogging dontLog='true' />"));
        await using (var built = Build(site)) Assert.Null(built.On);
        using var on = new Site("<configuration>");
        await using (var built = Build(on)) Assert.NotNull(built.On);
    }

    // --- The server's settings (IisProfile) and the configuration's

    [Fact] // logFile: the site's logExtFileFlags, period, localTimeRollover; s-sitename W3SVC and the site's id
    public async Task The_server_log_settings_are_taken()
    {
        using var site = new Site(profile: LogFile("logExtFileFlags='Date, Time, Cookie' period='Hourly' localTimeRollover='true' logFormat='W3C'"));
        await using var built = Build(site);
        var options = built.Writer.Options;
        Assert.Equal(IisLogFields.Date | IisLogFields.Time | IisLogFields.Cookie, options.Fields);
        Assert.Equal(IisLogPeriod.Hourly, options.Period);
        Assert.True(options.LocalTimeRollover);
        Assert.Equal("W3SVC3", options.SiteName);
    }

    [Fact] // the host's choice (summary): what this log does not write (a field it does not have, Weekly or MaxSize, another format) told, IIS's defaults written
    public async Task Server_settings_the_log_does_not_have_keep_the_defaults()
    {
        using var site = new Site(profile: LogFile("logExtFileFlags='Date, CustomThing' period='Weekly' localTimeRollover='maybe' logFormat='NCSA'"));
        await using var built = Build(site);
        var options = built.Writer.Options;
        Assert.Equal(IisLogFields.Default, options.Fields);
        Assert.Equal(IisLogPeriod.Daily, options.Period);
        Assert.False(options.LocalTimeRollover);
        using var empty = new Site(profile: "<profile><site><logFile logExtFileFlags='' period='' logFormat='' /></site></profile>");
        await using var builtEmpty = Build(empty);
        Assert.Equal(IisLogFields.Default, builtEmpty.Writer.Options.Fields);
        Assert.Equal("W3SVC1", builtEmpty.Writer.Options.SiteName);
    }

    [Fact] // the host's choice (summary): the configuration's W3CLogging:Fields, Period, LocalTimeRollover, SiteName, Software over the server's
    public async Task The_configuration_settings_are_over_the_server_ones()
    {
        using var site = new Site(profile: LogFile("logExtFileFlags='Date' period='Hourly'"));
        await using var built = Build(site, ("W3CLogging:Mode", "IIS"), ("W3CLogging:Fields", "Date, Time, Host"), ("W3CLogging:Period", "monthly"), ("W3CLogging:LocalTimeRollover", "true"),
            ("W3CLogging:SiteName", "W3SVC9"), ("W3CLogging:Software", "Mine"));
        var options = built.Writer.Options;
        Assert.Equal(IisLogFields.Date | IisLogFields.Time | IisLogFields.Host, options.Fields);
        Assert.Equal(IisLogPeriod.Monthly, options.Period);
        Assert.True(options.LocalTimeRollover);
        Assert.Equal("W3SVC9", options.SiteName);
        Assert.Equal("Mine", options.Software);
    }

    [Theory] // the host's choice (summary): a setting the log does not have stops the application at its start
    [InlineData("W3CLogging:Fields", "Date, Bogus")]
    [InlineData("W3CLogging:Period", "Weekly2")]
    [InlineData("W3CLogging:Mode", "Other")]
    public void A_wrong_setting_stops_the_start(string key, string value)
    {
        using var site = new Site();
        Assert.Throws<InvalidOperationException>(() => Build(site, (key, value)));
    }

    // --- Where

    [Fact] // the host's choice (summary): W3CLogging:LogDirectory relative to the application's folder; none to write to: not written
    public async Task The_log_directory()
    {
        using var site = new Site();
        await using (var built = Build(site, ("W3CLogging:LogDirectory", "a/b"))) Assert.Equal(Path.Combine(site.Root, "a", "b"), built.Directory);
        site.Write("file", "in the way");
        await using (var built = Build(site, ("W3CLogging:LogDirectory", "file"))) Assert.Null(built.On);
    }

    [Fact] // the host's choice (summary): by default %ProgramData%\FrameworkOnCore\LogFiles\<folder> on Windows
    public async Task The_default_directory_is_under_program_data()
    {
        if (!OperatingSystem.IsWindows()) return;
        using var site = new Site();
        var expected = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "FrameworkOnCore", "LogFiles", Path.GetFileName(site.Root));
        try
        {
            await using var built = Build(site, ("W3CLogging:LogDirectory", null));
            Assert.Equal(expected, built.Directory);
        }
        finally
        {
            try { Directory.Delete(expected, true); } catch (IOException) { }
        }
    }

    [Fact] // the host's choice: an application at a drive's root: its folder named "site"
    public async Task An_application_at_the_root_is_named_site()
    {
        if (!OperatingSystem.IsWindows()) return;
        var root = Path.GetPathRoot(Path.GetTempPath());
        var expected = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "FrameworkOnCore", "LogFiles", "site");
        var existed = Directory.Exists(expected);
        await using var app = Site.Application(root, new Dictionary<string, string> { ["W3CLogging:Mode"] = "AspNetCore" }, builder => builder.AddWebFormsW3CLogging());
        try
        {
            Assert.Equal(expected, (string)On.GetField("Directory").GetValue(app.Services.GetService(On)));
        }
        finally
        {
            if (!existed) try { Directory.Delete(expected); } catch (IOException) { }
        }
    }

    // --- The middleware

    [Fact] // the host's choice (summary): the addresses as IIS logs them (IPv4, not Kestrel's ::ffff:a.b.c.d), every request logged
    public async Task The_middleware_logs_the_requests()
    {
        using var site = new Site();
        await using var built = Build(site);
        var app = (IApplicationBuilder)built.App;
        app.UseWebFormsW3CLogging();
        app.Run(c => c.Response.Body.WriteAsync(new byte[] { 1 }).AsTask());
        var pipeline = app.Build();
        var context = Site.Request("/a.aspx");
        context.Connection.LocalIpAddress = System.Net.IPAddress.Parse("::ffff:127.0.0.1");
        var response = Site.StartingResponse.On(context);
        await pipeline(context);
        await response.CompleteAsync();
        Assert.Equal("192.168.1.10", context.Connection.RemoteIpAddress.ToString());
        Assert.Equal("127.0.0.1", context.Connection.LocalIpAddress.ToString());
        var other = Site.Request("/b.aspx");
        other.Connection.RemoteIpAddress = System.Net.IPAddress.IPv6Loopback;
        other.Connection.LocalIpAddress = null;
        response = Site.StartingResponse.On(other);
        await pipeline(other);
        await response.CompleteAsync();
        Assert.Equal("::1", other.Connection.RemoteIpAddress.ToString());
        var none = Site.Request("/c.aspx");
        none.Connection.RemoteIpAddress = null;
        response = Site.StartingResponse.On(none);
        await pipeline(none);
        await response.CompleteAsync();
        await built.Writer.DisposeAsync();
        var lines = File.ReadAllLines(Directory.GetFiles(built.Directory).Single());
        Assert.Contains(" /a.aspx ", lines[4]);
        Assert.Contains(" /b.aspx ", lines[5]);
        Assert.Contains(" /c.aspx ", lines[6]);
    }

    [Fact] // the host's choice: without the log, no middleware
    public async Task Without_the_log_there_is_no_middleware()
    {
        using var site = new Site();
        await using var built = Build(site, ("W3CLogging:Enabled", "false"));
        var app = (IApplicationBuilder)built.App;
        Assert.Same(app, app.UseWebFormsW3CLogging());
    }

    // --- ASP.NET Core's W3C logging (Mode AspNetCore)

    [Fact] // the host's choice (summary): Mode AspNetCore: ASP.NET Core's W3C logging, IIS's default fields (without those it does not have), u_ex, no size limit, FlushInterval 20 ms
    public async Task AspNetCore_mode_has_IIS_defaults()
    {
        using var site = new Site();
        await using var built = Build(site, ("W3CLogging:Mode", "aspnetcore"));
        Assert.Null(built.Writer);
        var options = built.App.Services.GetRequiredService<IOptionsMonitor<W3CLoggerOptions>>().CurrentValue;
        Assert.Equal(W3CLoggingFields.Date | W3CLoggingFields.Time | W3CLoggingFields.ServerIpAddress | W3CLoggingFields.Method | W3CLoggingFields.UriStem |
            W3CLoggingFields.UriQuery | W3CLoggingFields.ServerPort | W3CLoggingFields.UserName | W3CLoggingFields.ClientIpAddress | W3CLoggingFields.UserAgent |
            W3CLoggingFields.Referer | W3CLoggingFields.ProtocolStatus | W3CLoggingFields.TimeTaken, options.LoggingFields);
        Assert.Equal(Path.Combine(site.Root, "logs"), options.LogDirectory);
        Assert.Equal("u_ex", options.FileName);
        Assert.Null(options.FileSizeLimit);
        Assert.Equal(TimeSpan.FromMilliseconds(20), options.FlushInterval);
        var app = (IApplicationBuilder)built.App;
        Assert.Same(app, app.UseWebFormsW3CLogging());
    }

    [Theory] // the host's choice (summary): W3CLogging:FlushInterval, a positive time; otherwise 20 ms
    [InlineData("00:00:01", 1000)]
    [InlineData("00:00:00", 20)]
    [InlineData("soon", 20)]
    public async Task AspNetCore_mode_flush_interval(string interval, int milliseconds)
    {
        using var site = new Site();
        await using var built = Build(site, ("W3CLogging:Mode", "AspNetCore"), ("W3CLogging:FlushInterval", interval));
        var options = built.App.Services.GetRequiredService<IOptionsMonitor<W3CLoggerOptions>>().CurrentValue;
        Assert.Equal(TimeSpan.FromMilliseconds(milliseconds), options.FlushInterval);
    }
}
