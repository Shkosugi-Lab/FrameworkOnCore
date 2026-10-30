using System.Xml;
using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>
/// The web project's entry point (Program.cs, Program.vb): FrameworkOnCore's host, from the template (templates/), as the
/// application needs it: every request to Web Forms (routes, modules for every request), the session state as its
/// web.config has it, WCF's services.
/// </summary>
public static class HostProgram
{
    /// <summary>The session state mode of the application's web.config (system.web/sessionState mode); InProc when it says none, as ASP.NET's default.</summary>
    public static string SessionStateMode(string webDirectory) => (string?)SessionState(webDirectory)?.Attribute("mode") ?? "InProc";

    /// <summary>web.config's session timeout in minutes (sessionState timeout); null when it says none (20, ASP.NET's default and ASP.NET Core's).</summary>
    public static int? SessionTimeout(string webDirectory) =>
        int.TryParse((string?)SessionState(webDirectory)?.Attribute("timeout"), out var minutes) && minutes > 0 ? minutes : null;

    // The application's sessionState: in system.web, or in a <location> for the whole site (path "." or none, as
    // mojoPortal has it). By local name: old web.configs have the .NET 2.0 configuration namespace.
    static XElement? SessionState(string webDirectory)
    {
        var config = Paths.WebConfig(webDirectory);
        if (config == null) return null;
        try
        {
            var root = XDocument.Load(config).Root;
            if (root == null) return null;
            var sites = root.Elements().Where(e => e.Name.LocalName == "location" && ((string?)e.Attribute("path") ?? ".") is "." or "").Prepend(root);
            return sites.SelectMany(e => e.Elements()).Where(e => e.Name.LocalName == "system.web")
                .SelectMany(e => e.Elements()).LastOrDefault(e => e.Name.LocalName == "sessionState");
        }
        catch (XmlException) { return null; }
    }

    /// <summary>
    /// Whether the session's items are serialized (ASP.NET Core's session): StateServer and SQLServer, servers the converted
    /// application has not, serialize them too. InProc (the default), Off and Custom are System.Web's own, in the process as
    /// on IIS: InProc keeps the objects themselves (they need not be serializable), runs Session_End, and its cookie is
    /// ASP.NET_SessionId.
    /// </summary>
    public static bool SerializesSession(string sessionMode) =>
        sessionMode.Equals("StateServer", StringComparison.OrdinalIgnoreCase) || sessionMode.Equals("SQLServer", StringComparison.OrdinalIgnoreCase);

    /// <summary>The template, its lines by \n whatever the checkout made of them (the changes below are by lines).</summary>
    public static string Template(bool visualBasic)
    {
        using var stream = typeof(HostProgram).Assembly.GetManifestResourceStream(visualBasic ? "ProgramTemplate.vb.txt" : "ProgramTemplate.txt")!;
        return new StreamReader(stream).ReadToEnd().Replace("\r\n", "\n");
    }

    /// <summary>The entry point's text.</summary>
    /// <param name="allRequests">Every request to Web Forms (HandleAllRequestsWithWebForms).</param>
    /// <summary>
    /// The host's modes for IIS's features, as the choices' settings have them (iis, aspnetcore, off), into the web project's
    /// appsettings.json (kept what it has besides): W3CLogging:Mode (off: W3CLogging:Enabled false), UrlRewrite:Mode,
    /// HttpErrors:Mode, Compression:Mode. The host reads them there; they can be changed after the conversion.
    /// </summary>
    public static void WriteSettings(string webDirectory, string w3cLog, string urlRewrite, string httpErrors, string compression)
    {
        var file = Path.Combine(webDirectory, "appsettings.json");
        var options = new System.Text.Json.JsonDocumentOptions { CommentHandling = System.Text.Json.JsonCommentHandling.Skip, AllowTrailingCommas = true };
        System.Text.Json.Nodes.JsonObject root;
        try
        {
            root = File.Exists(file) ? System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(file), documentOptions: options) as System.Text.Json.Nodes.JsonObject ?? new() : new();
        }
        catch (System.Text.Json.JsonException)
        {
            root = new();
        }
        static string Mode(string option) => option switch { "aspnetcore" => "AspNetCore", "off" => "Off", _ => "Iis" };
        System.Text.Json.Nodes.JsonObject Section(string name)
        {
            if (root[name] is System.Text.Json.Nodes.JsonObject section) return section;
            root[name] = section = new System.Text.Json.Nodes.JsonObject();
            return section;
        }
        var log = Section("W3CLogging");
        log["Mode"] = w3cLog == "aspnetcore" ? "AspNetCore" : "Iis";
        if (w3cLog == "off") log["Enabled"] = false;
        else log.Remove("Enabled");
        Section("UrlRewrite")["Mode"] = Mode(urlRewrite);
        Section("HttpErrors")["Mode"] = Mode(httpErrors);
        Section("Compression")["Mode"] = Mode(compression);
        File.WriteAllText(file, root.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true }) + Environment.NewLine, new System.Text.UTF8Encoding(false));
    }

    /// <param name="sessionMode">web.config's session state mode (SessionStateMode).</param>
    /// <param name="serviceModel">WCF's services (CoreWCF: FrameworkOnCore.ServiceModel) before Web Forms.</param>
    /// <param name="sessionTimeout">web.config's session timeout (SessionTimeout): ASP.NET Core's session expires the items
    /// (its IdleTimeout, 20 minutes unless told), where it keeps them.</param>
    public static string Write(bool visualBasic, bool allRequests, string sessionMode, bool serviceModel, int? sessionTimeout = null)
    {
        var program = Template(visualBasic);
        var options = new List<string>();
        if (allRequests) options.Add("HandleAllRequestsWithWebForms()");
        if (SerializesSession(sessionMode))
        {
            options.Add("UseAspNetCoreSessionProvider()");
            if (sessionTimeout is { } minutes && minutes != 20)
                program = visualBasic
                    ? program.Replace("builder.Services.AddSession()", $"builder.Services.AddSession(Sub(session) session.IdleTimeout = System.TimeSpan.FromMinutes({minutes}))")
                    : program.Replace("builder.Services.AddSession();", $"builder.Services.AddSession(session => session.IdleTimeout = System.TimeSpan.FromMinutes({minutes}));");
        }
        else
            program = visualBasic
                ? program.Replace("            builder.Services.AddDistributedMemoryCache()\n            builder.Services.AddSession()\n", "")
                         .Replace("            app.UseSession()\n", "")
                : program.Replace("        builder.Services.AddDistributedMemoryCache();\n        builder.Services.AddSession();\n", "")
                         .Replace("        app.UseSession();\n", "");
        program = visualBasic
            ? program.Replace("app.UseWebForms(Sub(options) options.UseAspNetCoreSessionProvider())",
                options.Count == 0 ? "app.UseWebForms()" : "app.UseWebForms(Sub(options) options." + string.Join(".", options) + ")")
            : program.Replace("app.UseWebForms(options => options.UseAspNetCoreSessionProvider());",
                options.Count == 0 ? "app.UseWebForms();" : "app.UseWebForms(options => options." + string.Join(".", options) + ");");
        if (serviceModel)
            program = visualBasic
                ? program.Replace("            Dim app = builder.Build()", "            builder.Services.AddWebFormsServiceModel()\n\n            Dim app = builder.Build()")
                         .Replace("app.UseWebForms(", "app.UseWebFormsServiceModel()\n            app.UseWebForms(")
                : program.Replace("        var app = builder.Build();", "        builder.Services.AddWebFormsServiceModel();\n\n        var app = builder.Build();")
                         .Replace("app.UseWebForms(", "app.UseWebFormsServiceModel();\n        app.UseWebForms(");
        return program;
    }
}
