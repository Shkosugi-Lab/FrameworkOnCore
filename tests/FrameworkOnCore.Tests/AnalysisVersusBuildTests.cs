using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The analysis held to what it foretells: the conversion's build. An application made here uses .NET Framework's APIs one
/// a source file (of every kind of answer: there as it was, there in a package or in FrameworkOnCore, there and throwing, Windows
/// only, obsolete, not there), analyzed (analyze: each API's status on .NET 10), then converted and built. Held:
/// <list type="bullet">
/// <item>an API the analysis says .NET has not (Missing) does not compile once converted: its source is among the ones the
/// build changed to compile (stubbed), or the build fails;</item>
/// <item>an API it says .NET has, in whatever way, compiles: its source is not among them;</item>
/// <item>every API used is in the analysis, with the uses there are (one a file here), and its report (API-ANALYSIS.md)
/// names the ones to decide on.</item>
/// </list>
/// The packages' feed and .NET Framework's reference assemblies are needed (the analysis compiles against them): where
/// they are.
/// </summary>
public sealed class AnalysisVersusBuildTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-avb").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }

    // (a name for the file, the API's documentation id as the analysis has it, a method's body that uses it)
    public static readonly (string File, string Api, string Body)[] Uses =
    {
        ("FileRead", "M:System.IO.File.ReadAllText(System.String)", "return System.IO.File.ReadAllText(\"x\");"),
        ("HtmlEncode", "M:System.Web.HttpUtility.HtmlEncode(System.String)", "return System.Web.HttpUtility.HtmlEncode(\"x\");"),
        ("AppSettings", "P:System.Configuration.ConfigurationManager.AppSettings", "return System.Configuration.ConfigurationManager.AppSettings[\"x\"];"),
        ("SqlConnection", "M:System.Data.SqlClient.SqlConnection.#ctor(System.String)", "using (var c = new System.Data.SqlClient.SqlConnection(\"x\")) return c.Database;"),
        ("HttpContext", "P:System.Web.HttpContext.Current", "return System.Web.HttpContext.Current == null ? \"\" : \"x\";"),
        ("Cache", "P:System.Web.HttpRuntime.Cache", "return System.Web.HttpRuntime.Cache.Count.ToString();"),
        ("FormsAuth", "M:System.Web.Security.FormsAuthentication.SignOut", "System.Web.Security.FormsAuthentication.SignOut(); return \"\";"),
        ("Bitmap", "M:System.Drawing.Bitmap.#ctor(System.Int32,System.Int32)", "using (var b = new System.Drawing.Bitmap(1, 1)) return b.Width.ToString();"),
        ("BinaryFormatter", "M:System.Runtime.Serialization.Formatters.Binary.BinaryFormatter.#ctor", "return new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().ToString();"),
        ("ThreadAbort", "M:System.Threading.Thread.Abort", "System.Threading.Thread.CurrentThread.Abort(); return \"\";"),
        ("WebRequest", "M:System.Net.WebRequest.Create(System.String)", "return System.Net.WebRequest.Create(\"http://example.invalid/\").Method;"),
        ("EncodingDefault", "P:System.Text.Encoding.Default", "return System.Text.Encoding.Default.WebName;"),
        ("EventLog", "M:System.Diagnostics.EventLog.WriteEntry(System.String,System.String)", "System.Diagnostics.EventLog.WriteEntry(\"x\", \"y\"); return \"\";"),
        ("Registry", "F:Microsoft.Win32.Registry.LocalMachine", "return Microsoft.Win32.Registry.LocalMachine.Name;"),
        ("DataContext", "M:System.Data.Linq.DataContext.#ctor(System.String)", "using (var c = new System.Data.Linq.DataContext(\"x\")) return c.ToString();"),
        ("Transactions", "M:System.Transactions.TransactionScope.#ctor", "using (var t = new System.Transactions.TransactionScope()) return t.ToString();"),
        ("DirectoryServices", "M:System.DirectoryServices.DirectoryEntry.#ctor(System.String)", "using (var e = new System.DirectoryServices.DirectoryEntry(\"LDAP://x\")) return e.Path;"),
        ("ServiceBase", "P:System.ServiceProcess.ServiceController.ServiceName", "using (var s = new System.ServiceProcess.ServiceController(\"x\")) return s.ServiceName;"),
        ("Remoting", "M:System.Runtime.Remoting.RemotingConfiguration.Configure(System.String,System.Boolean)", "System.Runtime.Remoting.RemotingConfiguration.Configure(\"x\", false); return \"\";"),
        ("Channels", "M:System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(System.Runtime.Remoting.Channels.IChannel,System.Boolean)", "System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(null, false); return \"\";"),
        ("EnterpriseServices", "M:System.EnterpriseServices.ContextUtil.SetComplete", "System.EnterpriseServices.ContextUtil.SetComplete(); return \"\";"),
        ("Messaging", "M:System.Messaging.MessageQueue.#ctor(System.String)", "using (var q = new System.Messaging.MessageQueue(\"x\")) return q.Path;"),
        ("OracleClient", "M:System.Data.OracleClient.OracleConnection.#ctor(System.String)", "using (var c = new System.Data.OracleClient.OracleConnection(\"x\")) return c.DataSource;"),
        ("WebMail", "M:System.Web.Mail.SmtpMail.Send(System.String,System.String,System.String,System.String)", "System.Web.Mail.SmtpMail.Send(\"a\", \"b\", \"c\", \"d\"); return \"\";"),
        ("AppDomainCreate", "M:System.AppDomain.CreateDomain(System.String)", "return System.AppDomain.CreateDomain(\"x\").FriendlyName;"),
        ("CodeAccess", "M:System.Security.Permissions.FileIOPermission.#ctor(System.Security.Permissions.PermissionState)", "new System.Security.Permissions.FileIOPermission(System.Security.Permissions.PermissionState.None).Demand(); return \"\";"),
        ("SoapClient", "M:System.Web.Services.Protocols.SoapHttpClientProtocol.#ctor", "return new System.Web.Services.Protocols.SoapHttpClientProtocol().Url;"),
        ("WindowsIdentity", "M:System.Security.Principal.WindowsIdentity.GetCurrent", "return System.Security.Principal.WindowsIdentity.GetCurrent().Name;"),
        ("PerformanceCounter", "M:System.Diagnostics.PerformanceCounter.#ctor(System.String,System.String)", "using (var p = new System.Diagnostics.PerformanceCounter(\"x\", \"y\")) return p.CounterName;"),
        ("SmtpClient", "M:System.Net.Mail.SmtpClient.#ctor(System.String)", "using (var c = new System.Net.Mail.SmtpClient(\"x\")) return c.Host;"),
        ("JavaScriptSerializer", "M:System.Web.Script.Serialization.JavaScriptSerializer.Serialize(System.Object)", "return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(1);"),
        ("ObjectCache", "P:System.Runtime.Caching.MemoryCache.Default", "return System.Runtime.Caching.MemoryCache.Default.Name;"),
        ("WindowsForms", "M:System.Windows.Forms.MessageBox.Show(System.String)", "return System.Windows.Forms.MessageBox.Show(\"x\").ToString();"),
    };

    static readonly string[] References =
    {
        "System.Data", "System.Drawing", "System.Data.Linq", "System.Transactions", "System.DirectoryServices", "System.ServiceProcess", "System.Runtime.Remoting",
        "System.EnterpriseServices", "System.Messaging", "System.Data.OracleClient", "System.Web.Services", "System.Web.Extensions", "System.Runtime.Caching", "System.Windows.Forms", "System.Xml", "System.Core",
    };

    [Fact]
    public void What_the_analysis_says_of_an_API_is_what_the_build_does_with_it()
    {
        var runtime = ConverterSweepTests.RealRuntime();
        if (runtime == null) return;   // (no feed here)
        var app = new ConverterSweepTests.App
        {
            References = string.Concat(References.Select(r => $"<Reference Include=\"{r}\" />")),
            Items = string.Concat(Uses.Select(u => $"<Compile Include=\"Apis\\{u.File}.cs\" />")),
            Files = Uses.Select(u => ($"Apis\\{u.File}.cs", Encoding.UTF8.GetBytes($"namespace Sweep.Apis\n{{\n    public static class {u.File}\n    {{\n        public static string Use()\n        {{\n            {u.Body}\n        }}\n    }}\n}}\n"))).ToList(),
        };
        var source = Path.Combine(root, "source");
        var project = ConverterSweepTests.Write(source, app);
        var converter = Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll");

        // -- the analysis
        var analysisFolder = Path.Combine(root, "analysis");
        var (analyzed, analyzedText) = ConverterSweepTests.Run(root, "dotnet", converter, "analyze", project, "--out", analysisFolder, "--runtime", runtime);
        Assert.True(analyzed == 0, analyzedText);
        using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(analysisFolder, "api-analysis.json")));
        var apis = json.RootElement.GetProperty("apis").EnumerateArray().ToDictionary(a => a.GetProperty("id").GetString()!, a => a);
        var markdown = File.ReadAllText(Path.Combine(analysisFolder, "API-ANALYSIS.md"));
        var problems = new List<string>();
        var status = new Dictionary<string, string>();
        foreach (var (file, api, _) in Uses)
        {
            // (the analysis' id of a method that gives a value has its type after it: "...ReadAllText(System.String)~System.String")
            var id = apis.Keys.FirstOrDefault(k => k == api || k.StartsWith(api + "~", StringComparison.Ordinal));
            var found = id != null ? apis[id] : default;
            if (id == null)
            {
                // (told with the ids the analysis has of the same member's name, to see what it calls it)
                var member = Regex.Match(api, @"\.([\w#]+)(\(|$)").Groups[1].Value;
                problems.Add($"{file}: {api} is not in the analysis ({string.Join(", ", apis.Keys.Where(k => k.Contains("." + member)).Take(4))})");
                continue;
            }
            status[file] = found.GetProperty("status").GetString()!;
            if (found.GetProperty("count").GetInt32() != 1) problems.Add($"{file}: {api} used once, counted {found.GetProperty("count").GetInt32()}");
            var files = found.GetProperty("files").EnumerateArray().Select(f => f.GetProperty("file").GetString()!.Replace('\\', '/')).ToList();
            if (!files.Contains($"Web/Apis/{file}.cs")) problems.Add($"{file}: {api} in {string.Join(", ", files)}, not Web/Apis/{file}.cs");
        }
        Assert.True(problems.Count == 0, string.Join("\n", problems));
        // (no absolute path in the result: the repository's name, paths from it)
        Assert.DoesNotContain(source, File.ReadAllText(Path.Combine(analysisFolder, "api-analysis.json")).Replace("\\\\", "\\"));

        // -- the conversion's build
        var output = Path.Combine(root, "output");
        var (converted, convertedText) = ConverterSweepTests.Run(root, "dotnet", converter, project, "--out", output, "--runtime", runtime, "--deploy", "none");
        Assert.False(convertedText.Contains("Unhandled exception"), convertedText);
        var report = File.ReadAllText(Path.Combine(output, "CONVERSION-REPORT.md"));
        // The sources the build changed to compile (stubbed, removed): the report's section of them, and the unresolved.
        string Section(string heading) => Regex.Match(report, @"## " + Regex.Escape(heading) + @"[^\n]*\n(?<body>.*?)(\n## |\z)", RegexOptions.Singleline).Groups["body"].Value;
        var changed = Section("コンパイルのために変えたソース") + Section("未解決");
        foreach (var (file, api, _) in Uses)
        {
            var stubbed = changed.Contains($"{file}.cs", StringComparison.Ordinal);
            var missing = status[file] == "Missing";
            if (missing && !stubbed && converted == 0) problems.Add($"{file}: the analysis says .NET has not {api}; the converted source compiled as it was");
            if (!missing && stubbed) problems.Add($"{file}: the analysis says {status[file]} of {api}; the build changed the source to compile: {Regex.Match(changed, @"[^\n]*" + Regex.Escape(file + ".cs") + @"[^\n]*").Value}");
        }
        Assert.True(problems.Count == 0, string.Join("\n", problems) + "\n\nstatuses: " + string.Join(", ", status.Select(s => s.Key + "=" + s.Value)));
        // (the test's own: both kinds of answer are among the APIs, or it holds nothing)
        var told = "statuses: " + string.Join(", ", status.OrderBy(s => s.Value).Select(s => s.Key + "=" + s.Value));
        Assert.True(status.Values.Count(v => v == "Missing") >= 3 && status.Values.Count(v => v != "Missing") >= 10 && status.Values.Distinct().Count() >= 4, told);
        if (Environment.GetEnvironmentVariable("FOC_AVB_TELL") == "1") Assert.Fail(told);
        // The ones to decide on are in the analysis' report.
        foreach (var (file, _) in status.Where(s => s.Value == "Missing"))
            Assert.Contains(Uses.Single(u => u.File == file).Api.Split('(')[0].Split('.').Reverse().Skip(1).First(), markdown);
    }
}
