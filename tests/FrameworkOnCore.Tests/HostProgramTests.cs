using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>The converted web project's entry point (Program.cs, Program.vb): its session state as web.config has it.</summary>
public class HostProgramTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-host-" + Guid.NewGuid().ToString("N"));

    public HostProgramTests() => Directory.CreateDirectory(root);

    public void Dispose()
    {
        if (Directory.Exists(root)) Directory.Delete(root, true);
    }

    string Mode(string webConfig)
    {
        File.WriteAllText(Path.Combine(root, "Web.config"), webConfig);
        return HostProgram.SessionStateMode(root);
    }

    [Theory] // IIS's request log (W3C) and its features (headers, URL Rewrite, HTTP redirection), whatever the session state and the services: the log's services, then its middleware first, the rules next
    [InlineData(false, "InProc", false)]
    [InlineData(false, "SQLServer", true)]
    [InlineData(true, "InProc", true)]
    [InlineData(true, "StateServer", false)]
    public void The_W3C_log_is_in_every_program(bool visualBasic, string sessionMode, bool serviceModel)
    {
        var program = HostProgram.Write(visualBasic, false, sessionMode, serviceModel);
        var services = program.IndexOf("builder.AddWebFormsW3CLogging()", StringComparison.Ordinal);
        var build = program.IndexOf(visualBasic ? "Dim app = builder.Build()" : "var app = builder.Build();", StringComparison.Ordinal);
        var middleware = program.IndexOf("app.UseWebFormsW3CLogging()", StringComparison.Ordinal);
        var webForms = program.IndexOf("app.UseWebForms(", middleware + 1, StringComparison.Ordinal);
        Assert.True(services >= 0 && services < build, program);
        Assert.True(middleware > build && middleware < webForms, program);
        // IIS's features (headers, URL Rewrite, HTTP redirection): after the log (its line has the URL asked for), before System.Web.
        var rewrite = program.IndexOf("app.UseWebFormsIis()", StringComparison.Ordinal);
        Assert.True(rewrite > middleware && rewrite < webForms, program);
        var session = program.IndexOf("app.UseSession", StringComparison.Ordinal);
        Assert.True(session < 0 || middleware < session, program);
    }

    [Fact] // ASP.NET's default
    public void No_web_config_is_InProc() => Assert.Equal("InProc", HostProgram.SessionStateMode(root));

    [Fact]
    public void No_sessionState_is_InProc() => Assert.Equal("InProc", Mode("<configuration><system.web /></configuration>"));

    [Fact] // .NET 2.0's configuration namespace (YAF, mojoPortal's old web.configs)
    public void The_mode_is_read_in_any_namespace() =>
        Assert.Equal("SQLServer", Mode("<configuration xmlns=\"http://schemas.microsoft.com/.NetConfiguration/v2.0\"><system.web><sessionState mode=\"SQLServer\" /></system.web></configuration>"));

    [Fact]
    public void A_web_config_that_is_no_XML_is_InProc() => Assert.Equal("InProc", Mode("<configuration>"));

    [Fact] // mojoPortal: system.web in <location path=".">
    public void The_mode_is_read_in_a_location_for_the_site() =>
        Assert.Equal("StateServer", Mode("<configuration><location path=\".\"><system.web><sessionState mode=\"StateServer\" /></system.web></location></configuration>"));

    [Fact] // a folder's own settings are not the application's
    public void A_location_for_a_folder_is_not_the_site() =>
        Assert.Equal("InProc", Mode("<configuration><location path=\"admin\"><system.web><sessionState mode=\"Off\" /></system.web></location></configuration>"));

    [Fact] // the commented-out providers of mojoPortal and nopCommerce 3.90 are none
    public void A_commented_out_sessionState_is_none() =>
        Assert.Equal("InProc", Mode("<configuration><system.web><!-- <sessionState mode=\"Custom\" customProvider=\"X\" /> --></system.web></configuration>"));

    [Fact]
    public void The_timeout_is_read()
    {
        Assert.Equal("SQLServer", Mode("<configuration><system.web><sessionState mode=\"SQLServer\" timeout=\"7\" /></system.web></configuration>"));
        Assert.Equal(7, HostProgram.SessionTimeout(root));
        File.WriteAllText(Path.Combine(root, "Web.config"), "<configuration><system.web><sessionState /></system.web></configuration>");
        Assert.Null(HostProgram.SessionTimeout(root));
    }

    [Theory] // ASP.NET Core's session expires its items (IdleTimeout) as web.config's timeout says; 20 is its default
    [InlineData(false, 7, "builder.Services.AddSession(session => session.IdleTimeout = System.TimeSpan.FromMinutes(7));")]
    [InlineData(true, 60, "builder.Services.AddSession(Sub(session) session.IdleTimeout = System.TimeSpan.FromMinutes(60))")]
    [InlineData(false, 20, "builder.Services.AddSession();")]
    [InlineData(false, null, "builder.Services.AddSession();")]
    public void ASP_NET_Core_session_has_web_config_timeout(bool visualBasic, int? timeout, string addSession) =>
        Assert.Contains(addSession, HostProgram.Write(visualBasic, false, "SQLServer", false, timeout));

    [Theory]
    [InlineData("InProc", false)]
    [InlineData("Off", false)]
    [InlineData("Custom", false)]
    [InlineData("StateServer", true)]
    [InlineData("SQLServer", true)]
    [InlineData("sqlserver", true)]
    public void Only_StateServer_and_SQLServer_serialize(string mode, bool serializes) => Assert.Equal(serializes, HostProgram.SerializesSession(mode));

    [Theory] // InProc: System.Web's own session; the objects themselves kept, as on IIS (no ASP.NET Core session)
    [InlineData(false)]
    [InlineData(true)]
    public void InProc_has_no_ASP_NET_Core_session(bool visualBasic)
    {
        var program = HostProgram.Write(visualBasic, false, "InProc", false);
        Assert.DoesNotContain("AddSession", program);
        Assert.DoesNotContain("AddDistributedMemoryCache", program);
        Assert.DoesNotContain("UseSession", program);
        Assert.DoesNotContain("UseAspNetCoreSessionProvider", program);
        Assert.Contains(visualBasic ? "app.UseWebForms()\n" : "app.UseWebForms();\n", program);
    }

    [Theory]
    [InlineData(false, "app.UseWebForms(options => options.UseAspNetCoreSessionProvider());")]
    [InlineData(true, "app.UseWebForms(Sub(options) options.UseAspNetCoreSessionProvider())")]
    public void SQLServer_has_ASP_NET_Core_session(bool visualBasic, string useWebForms)
    {
        var program = HostProgram.Write(visualBasic, false, "SQLServer", false);
        Assert.Contains("AddSession", program);
        Assert.Contains("UseSession", program);
        Assert.Contains(useWebForms, program);
    }

    [Theory]
    [InlineData(false, "InProc", "app.UseWebForms(options => options.HandleAllRequestsWithWebForms());")]
    [InlineData(false, "StateServer", "app.UseWebForms(options => options.HandleAllRequestsWithWebForms().UseAspNetCoreSessionProvider());")]
    [InlineData(true, "Off", "app.UseWebForms(Sub(options) options.HandleAllRequestsWithWebForms())")]
    [InlineData(true, "SQLServer", "app.UseWebForms(Sub(options) options.HandleAllRequestsWithWebForms().UseAspNetCoreSessionProvider())")]
    public void Every_request_to_Web_Forms_with_either_session(bool visualBasic, string mode, string useWebForms) =>
        Assert.Contains(useWebForms, HostProgram.Write(visualBasic, true, mode, false));

    [Theory] // CoreWCF's services, before Web Forms, whether or not there is ASP.NET Core's session
    [InlineData(false, "InProc")]
    [InlineData(false, "SQLServer")]
    [InlineData(true, "InProc")]
    [InlineData(true, "SQLServer")]
    public void WCF_services_come_before_Web_Forms(bool visualBasic, string mode)
    {
        var program = HostProgram.Write(visualBasic, true, mode, true);
        var add = program.IndexOf("builder.Services.AddWebFormsServiceModel()", StringComparison.Ordinal);
        var build = program.IndexOf("builder.Build()", StringComparison.Ordinal);
        var use = program.IndexOf("app.UseWebFormsServiceModel()", StringComparison.Ordinal);
        var webForms = program.IndexOf("app.UseWebForms(", StringComparison.Ordinal);
        Assert.True(add >= 0 && add < build, program);
        Assert.True(use > build && use < webForms, program);
    }

    [Fact] // the choices' settings for IIS's features into appsettings.json, what it had besides kept
    public void The_hosts_modes_go_to_appsettings()
    {
        File.WriteAllText(Path.Combine(root, "appsettings.json"), "{ // the application's\n \"Logging\": { \"LogLevel\": { \"Default\": \"Warning\" } }, \"W3CLogging\": { \"Enabled\": false, \"LogDirectory\": \"/logs\" } }");
        HostProgram.WriteSettings(root, "iis", "aspnetcore", "off", "iis");
        var settings = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(Path.Combine(root, "appsettings.json")))!;
        Assert.Equal("Warning", (string?)settings["Logging"]!["LogLevel"]!["Default"]);
        Assert.Equal("Iis", (string?)settings["W3CLogging"]!["Mode"]);
        Assert.Null(settings["W3CLogging"]!["Enabled"]);
        Assert.Equal("/logs", (string?)settings["W3CLogging"]!["LogDirectory"]);
        Assert.Equal("AspNetCore", (string?)settings["UrlRewrite"]!["Mode"]);
        Assert.Equal("Off", (string?)settings["HttpErrors"]!["Mode"]);
        Assert.Equal("Iis", (string?)settings["Compression"]!["Mode"]);
        HostProgram.WriteSettings(root, "off", "iis", "iis", "off");
        settings = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(Path.Combine(root, "appsettings.json")))!;
        Assert.False((bool)settings["W3CLogging"]!["Enabled"]!);
        Assert.Equal("Off", (string?)settings["Compression"]!["Mode"]);
    }
}
