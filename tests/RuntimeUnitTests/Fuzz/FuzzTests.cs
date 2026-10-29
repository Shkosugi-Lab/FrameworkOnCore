using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit.Abstractions;

namespace FrameworkOnCore.Runtime.Tests.Fuzz;

/// <summary>
/// Differential fuzzing: the inputs of FuzzInput.cs given to each target of FuzzTargets.cs, the port's results compared with
/// .NET Framework 4.8's (golden/*.txt.gz, recorded by golden/Program.cs: how to record them again is there). A difference
/// fails the test unless Allowed lists it, with why; the failure names each part that differs with its shortest input.
/// Besides System.Web's own code: the request's targets through the port's worker request (AspNetCoreWorkerRequest), and the
/// encoders as an application's HttpUtility (.NET's) gives them. The application's path (HttpRuntime) is set for the calls:
/// the Application collection (run alone).
/// Triage: FUZZ_DUMP=folder writes every difference (one file a target); FUZZ_STACK=1 writes each exception's stack to the
/// error output.
/// </summary>
[Collection("Application")]
public class FuzzTests
{
    readonly ITestOutputHelper output;

    public FuzzTests(ITestOutputHelper output) => this.output = output;

    static FuzzTests() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    public static IEnumerable<object[]> Names => Targets.All.Where(t => !t.WindowsOnly).Select(t => new object[] { t.Name });

    [Theory] // .NET Framework 4.8's System.Web: the same results for the same inputs (the encoders, paths, a request's parsing)
    [MemberData(nameof(Names))]
    public void The_port_gives_NET_Framework_s_results(string name) => Compare(name);

    [WindowsFact("physical paths as Windows has them (drive letters, '\\', 8.3 names, streams)")]
    public void The_port_checks_physical_paths_as_NET_Framework_on_Windows() => Compare("PhysicalPath");

    [Theory] // the request's targets through the port's own worker request (AspNetCoreWorkerRequest: a request as Kestrel gives it)
    [InlineData("Cookies")]
    [InlineData("Form")]
    [InlineData("Multipart")]
    public void The_port_s_worker_request_gives_NET_Framework_s_results(string name)
    {
        // The running application the worker request is of (its paths: SimpleWorkerRequest's).
        using var site = new Hosting.Site().Run(watch: false);
        var previous = Targets.NewWorker;
        Targets.NewWorker = AspNetCoreWorker;
        try
        {
            var target = Targets.All.Single(t => t.Name == name);
            // A query string with characters beyond ASCII is not one Kestrel takes (raw bytes in the request line): its parts not compared.
            Compare(name, "AspNetCoreWorkerRequest", input => Without(target.Execute(input), name == "Form" && input[2].Any(c => c > 127) ? "QueryString" : null));
        }
        finally
        {
            Targets.NewWorker = previous;
        }
    }

    public static IEnumerable<object[]> UtilityNames => new[] { "UrlEncode", "UrlDecode", "HtmlEncode", "HtmlDecode", "ParseQueryString" }.Select(n => new object[] { n });

    [Theory] // an application's HttpUtility: .NET's (System.Web.HttpUtility.dll; FrameworkOnCore's System.Web forwards the type to it)
    [MemberData(nameof(UtilityNames))]
    public void The_application_s_HttpUtility_gives_NET_Framework_s_results(string name) => Compare(name, "HttpUtility", input => ApplicationHttpUtility(name, input));

    void Compare(string name, string variant = null, Func<string[], Output> run = null)
    {
        var target = Targets.All.Single(t => t.Name == name);
        var differences = Differences(target, run, variant);
        var failing = differences.Where(d => Allowed.Why(d) == null).ToList();
        // FUZZ_DUMP: a folder the differences are written to (one file a target), for triage.
        var dump = Environment.GetEnvironmentVariable("FUZZ_DUMP");
        if (!string.IsNullOrEmpty(dump))
            File.WriteAllLines(Path.Combine(dump, name + (variant == null ? "" : "-" + variant) + ".txt"), differences.Select(d => (Allowed.Why(d) == null ? "" : "(allowed) ") + d.Part +
                "\n  " + d.Line.Replace("\t", " | ") + "\n  net48: " + d.Expected + "\n  port:  " + d.Actual));
        foreach (var group in differences.Where(d => Allowed.Why(d) != null).GroupBy(d => Allowed.Why(d)))
            output.WriteLine("allowed (" + group.Count() + "): " + group.Key);
        Assert.True(failing.Count == 0, (variant == null ? "" : variant + ": ") + Report(target, failing));
    }

    /// <summary>A request as Kestrel gives it, read by the port's AspNetCoreWorkerRequest (as WebFormsMiddleware does).</summary>
    static System.Web.HttpWorkerRequest AspNetCoreWorker(string verb, string path, string query, byte[] queryBytes, byte[] body, string[] headers)
    {
        var core = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        var request = core.Request;
        request.Method = verb;
        request.Scheme = "http";
        request.Host = new Microsoft.AspNetCore.Http.HostString("localhost");
        request.Protocol = "HTTP/1.1";
        request.Path = new Microsoft.AspNetCore.Http.PathString(path);
        var escaped = string.Concat(query.Select(c => c > 127 ? Uri.EscapeDataString(c.ToString()) : c.ToString()));
        request.QueryString = escaped.Length == 0 ? Microsoft.AspNetCore.Http.QueryString.Empty : new Microsoft.AspNetCore.Http.QueryString("?" + escaped);
        core.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpRequestFeature>().RawTarget = path + request.QueryString.Value;
        for (int i = 0; i + 1 < headers.Length; i += 2)
            if (headers[i + 1] != null) request.Headers[headers[i]] = headers[i + 1];
        if (body != null && body.Length > 0)
        {
            request.Body = new MemoryStream(body);
            request.ContentLength = body.Length;
        }
        core.Connection.RemoteIpAddress = System.Net.IPAddress.Loopback;
        core.Connection.LocalIpAddress = System.Net.IPAddress.Loopback;
        core.Response.Body = new MemoryStream();
        var worker = new System.Web.Hosting.AspNetCoreWorkerRequest(null, core);
        typeof(System.Web.Hosting.AspNetCoreWorkerRequest).GetMethod("ParseHeaders", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(worker, null);
        return worker;
    }

    static Output Without(Output output, string prefix)
    {
        if (prefix != null) output.Parts.RemoveAll(p => p.Key.StartsWith(prefix, StringComparison.Ordinal));
        return output;
    }

    /// <summary>The encoders' parts as an application's HttpUtility (.NET's) gives them.</summary>
    static Output ApplicationHttpUtility(string name, string[] x)
    {
        var o = new Output();
        Encoding E() => Encoding.GetEncoding(x[1]);
        switch (name)
        {
            case "UrlEncode":
                o.Add("UrlEncode", () => System.Web.HttpUtility.UrlEncode(x[0], E()));
#pragma warning disable SYSLIB0013
                o.Add("UrlEncodeUnicode", () => System.Web.HttpUtility.UrlEncodeUnicode(x[0]));
#pragma warning restore SYSLIB0013
                o.Add("UrlPathEncode", () => System.Web.HttpUtility.UrlPathEncode(x[0]));
                break;
            case "UrlDecode":
                o.Add("UrlDecode", () => System.Web.HttpUtility.UrlDecode(x[0], E()));
                o.Add("UrlDecodeBytes", () => System.Web.HttpUtility.UrlDecode(E().GetBytes(x[0]), E()));
                o.Add("UrlDecodeToBytes", () => System.Web.HttpUtility.UrlDecodeToBytes(E().GetBytes(x[0])));
                break;
            case "HtmlEncode":
                o.Add("HtmlEncode", () => System.Web.HttpUtility.HtmlEncode(x[0]));
                o.Add("HtmlAttributeEncode", () => System.Web.HttpUtility.HtmlAttributeEncode(x[0]));
                o.Add("JavaScriptStringEncode", () => System.Web.HttpUtility.JavaScriptStringEncode(x[0]));
                break;
            case "HtmlDecode":
                o.Add("HtmlDecode", () => System.Web.HttpUtility.HtmlDecode(x[0]));
                break;
            case "ParseQueryString":
                System.Collections.Specialized.NameValueCollection c = null;
                o.Add("Collection", () => c = System.Web.HttpUtility.ParseQueryString(x[0], E()));
                o.Add("ToString", () => c?.ToString());
                break;
        }
        return o;
    }

    /// <summary>The cases (by the parts that differ) whose result is not .NET Framework's.</summary>
    internal static List<Difference> Differences(Target target, Func<string[], Output> run = null, string variant = null)
    {
        var golden = Golden(target.Name);
        var inputs = target.Inputs();
        Assert.Equal(golden.Count, inputs.Count);
        var differences = new List<Difference>();
        using var configuration = Configuration();
        var culture = CultureInfo.CurrentCulture;
        var uiCulture = CultureInfo.CurrentUICulture;
        // As recorded (golden/Program.cs).
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        try
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                var line = Target.Line(inputs[i]);
                // The same inputs as the recorder's (FuzzInput.cs changed without recording again otherwise).
                Assert.True(line == golden[i].Input, $"{target.Name} #{i}: the input is not the recorded one: re-record (golden/Program.cs)\n  {line}\n  {golden[i].Input}");
                var actual = (run ?? target.Execute)(inputs[i]).Parts;
                var expected = Parse(golden[i].Output);
                foreach (var part in expected)
                {
                    var mine = actual.FirstOrDefault(p => p.Key == part.Key);
                    if (mine.Key == null) continue;  // not computed by this run (another path)
                    if (mine.Value != part.Value)
                        differences.Add(new Difference(target.Name, part.Key, inputs[i], line, part.Value, mine.Value, variant));
                }
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture;
        }
        return differences;
    }

    /// <summary>
    /// The configuration .NET Framework's recorder had (machine.config's: the sections' defaults) where System.Web reads it
    /// without a configuration system (the client configuration's cache and its last-known-good's); AppSettings' defaults.
    /// The previous values put back when disposed.
    /// </summary>
    internal static IDisposable Configuration(System.Web.Configuration.HttpCookiesSection httpCookies = null)
    {
        try { _ = System.Web.Util.AppSettings.MaxHttpCollectionKeys; } catch (Exception e) when (e.GetType().Name == "ConfigurationErrorsException") { }
        const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static, Instance = BindingFlags.NonPublic | BindingFlags.Instance;
        var type = typeof(System.Web.Configuration.RuntimeConfig);
        var client = type.GetMethod("GetClientRuntimeConfig", Static).Invoke(null, null);
        var configs = new[] { client, type.GetProperty("RuntimeConfigLKG", Instance).GetValue(client) };
        var index = type.GetNestedType("ResultsIndex", BindingFlags.NonPublic);
        var sections = new (string Index, object Section)[]
        {
            ("HttpRuntime", new System.Web.Configuration.HttpRuntimeSection()), ("Globalization", new System.Web.Configuration.GlobalizationSection()),
            ("HttpCookies", httpCookies ?? new System.Web.Configuration.HttpCookiesSection()), ("Pages", new System.Web.Configuration.PagesSection()),
        };
        var previous = new List<(object[] Results, int Index, object Value)>();
        foreach (var config in configs)
        {
            var results = (object[])type.GetField("_results", Instance).GetValue(config);
            foreach (var (name, section) in sections)
            {
                int i = (int)Enum.Parse(index, name);
                previous.Add((results, i, results[i]));
                results[i] = section;
            }
        }
        return new Restore(() => { foreach (var (results, i, value) in previous) results[i] = value; });
    }

    sealed class Restore(Action action) : IDisposable
    {
        public void Dispose() => action();
    }

    static List<KeyValuePair<string, string>> Parse(string line) =>
        line.Split('\t').Select(p => { int at = p.IndexOf('='); return new KeyValuePair<string, string>(p[..at], p[(at + 1)..]); }).ToList();

    /// <summary>The differences by part: how many, and the shortest input that shows each.</summary>
    internal static string Report(Target target, List<Difference> differences)
    {
        var sb = new StringBuilder();
        sb.Append(target.Name).Append(": ").Append(differences.Count).Append(" difference(s)\n");
        foreach (var group in differences.GroupBy(d => d.Part))
        {
            var minimal = group.OrderBy(d => d.Line.Length).First();
            sb.Append("  ").Append(group.Key).Append(": ").Append(group.Count()).Append(" case(s); the shortest:\n")
              .Append("    input:  ").Append(minimal.Line.Replace("\t", " | ")).Append('\n')
              .Append("    net48:  ").Append(Cut(minimal.Expected)).Append('\n')
              .Append("    port:   ").Append(Cut(minimal.Actual)).Append('\n');
        }
        return sb.ToString();
    }

    static string Cut(string s) => s.Length > 300 ? s[..300] + "..." : s;

    static readonly Dictionary<string, List<(string Input, string Output)>> golden = new();

    static List<(string Input, string Output)> Golden(string name)
    {
        lock (golden)
        {
            if (golden.TryGetValue(name, out var cases)) return cases;
            cases = new List<(string, string)>();
            using var reader = new StreamReader(new GZipStream(File.OpenRead(Path.Combine(Folder(), "golden", name + ".txt.gz")), CompressionMode.Decompress), Encoding.UTF8);
            string input;
            while ((input = reader.ReadLine()) != null) cases.Add((input, reader.ReadLine()));
            return golden[name] = cases;
        }
    }

    static string Folder([CallerFilePath] string file = null) => Path.GetDirectoryName(file);

    /// <summary>The recorded value of a part of the case with that input (null if there is none).</summary>
    internal static string Expected(string target, string inputLine, string part)
    {
        var line = Golden(target).FirstOrDefault(c => c.Input == inputLine).Output;
        return line == null ? null : Parse(line).FirstOrDefault(p => p.Key == part).Value;
    }
}

/// <summary>A part of a case's result that is not .NET Framework's (Variant: what gave it, if not System.Web's code as the targets call it).</summary>
sealed record Difference(string Target, string Part, string[] Input, string Line, string Expected, string Actual, string Variant = null);
