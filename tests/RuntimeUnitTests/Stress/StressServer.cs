using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FrameworkOnCore.Runtime.Tests.Hosting
{
    /// <summary>
    /// The child's side of the stress tests' server: a converted application's host as the converter's Program writes it
    /// (src/FrameworkOnCore.Converter/templates/ProgramTemplate.txt, HostProgram: the W3C log, IIS's features, Web Forms;
    /// the session state System.Web's own (InProc, the converter's choice for InProc, Off and Custom), or ASP.NET Core's
    /// (FOC_STRESS_SESSION=core: StateServer and SQLServer, UseAspNetCoreSessionProvider)) over the site's folder, on
    /// Kestrel at 127.0.0.1:port, until its standard input is closed or the application restarts (ProcessRestart: its
    /// exit code then). Supervised: in a worker process (WebFormsProcess.RunInWorker, as the template), the URL in
    /// ASPNETCORE_URLS (the listening sockets kept by the supervisor on Linux: ListenerHandoff).
    /// </summary>
    static class StressHost
    {
        static int Serve(string[] args) => Run(args[0], int.Parse(args[1]), supervised: false);

        static int Supervised(string[] args)
        {
            if (System.Web.Hosting.WebFormsProcess.RunInWorker(new[] { "StressHost.Supervised" }.Concat(args).ToArray(), out var code))
            {
                Child.Say("supervisor.exit", code);
                return code;
            }
            return Run(args[0], int.Parse(args[1]), supervised: true);
        }

        static int Run(string root, int port, bool supervised)
        {
            var core = Environment.GetEnvironmentVariable("FOC_STRESS_SESSION") == "core";
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { ContentRootPath = root, Args = Array.Empty<string>() });
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Warning);
            if (!supervised) builder.WebHost.UseKestrel(options => options.Listen(IPAddress.Loopback, port));
            builder.AddWebFormsW3CLogging();
            if (core)
            {
                builder.Services.AddDistributedMemoryCache();
                var idle = int.TryParse(Environment.GetEnvironmentVariable("FOC_STRESS_IDLE"), out var seconds) ? TimeSpan.FromSeconds(seconds) : TimeSpan.FromMinutes(20);
                builder.Services.AddSession(session => session.IdleTimeout = idle);
            }
            var app = builder.Build();
            app.UseWebFormsW3CLogging();
            app.UseWebFormsIis();
            if (core) app.UseSession();
            app.UseWebForms(options =>
            {
                options.PhysicalPath(root);
                if (core) options.UseAspNetCoreSessionProvider();
            });
            app.StartAsync().GetAwaiter().GetResult();
            Child.Say("listening", port);
            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            new Thread(() =>
            {
                try { Console.In.ReadToEnd(); } catch (IOException) { }
                lifetime.StopApplication();
            }) { IsBackground = true }.Start();
            app.WaitForShutdownAsync().GetAwaiter().GetResult();
            Console.Out.Flush();
            return Environment.ExitCode;
        }
    }
}

namespace FrameworkOnCore.Runtime.Tests.Stress
{
    using FrameworkOnCore.Runtime.Tests.Hosting;

    /// <summary>
    /// A server of a stress test's site (StressHost, in a process of its own; supervised: through WebFormsProcess, from a
    /// copy of the tests' folder as the application's bin), its site's folder and log folder, deleted with it.
    /// </summary>
    public sealed class StressServer : IDisposable
    {
        Process process;
        readonly StringBuilder output = new();
        readonly SemaphoreSlim listening = new(0);
        public readonly string Root;
        public readonly string LogDirectory;
        public readonly int Port;
        readonly string program;    // the supervised application's folder (its bin the copy)

        public StressServer(IEnumerable<(string Path, string Text)> files, bool supervised = false, IDictionary<string, string> environment = null)
        {
            Root = Directory.CreateTempSubdirectory("foc-stress").FullName;
            LogDirectory = Directory.CreateTempSubdirectory("foc-stress-log").FullName;
            foreach (var (path, text) in files) Write(path, text);
            Port = FreePort();
            var start = new ProcessStartInfo(Child.Dotnet)
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = AppContext.BaseDirectory,
            };
            if (supervised)
            {
                program = Directory.CreateTempSubdirectory("foc-stress-app").FullName;
                var bin = Path.Combine(program, "bin");
                CopyProgram(AppContext.BaseDirectory, bin);
                start.ArgumentList.Add(Path.Combine(bin, Path.GetFileName(Child.Program)));
                start.ArgumentList.Add("StressHost.Supervised");
                start.Environment["ASPNETCORE_URLS"] = "http://127.0.0.1:" + Port;
            }
            else
            {
                start.ArgumentList.Add(Child.Program);
                start.ArgumentList.Add("StressHost.Serve");
            }
            start.ArgumentList.Add(Root);
            start.ArgumentList.Add(Port.ToString());
            foreach (var name in new[] { "WEBFORMSFORCORE_WORKER", "WEBFORMSFORCORE_SHADOWCOPY", "WEBFORMSFORCORE_SUPERVISOR", "WEBFORMSFORCORE_APP_PATH", "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES" })
                start.Environment.Remove(name);
            start.Environment["ASPNETCORE_ENVIRONMENT"] = "Production";
            start.Environment["W3CLogging__LogDirectory"] = LogDirectory;
            foreach (var (name, value) in environment ?? new Dictionary<string, string>()) start.Environment[name] = value;
            process = new Process { StartInfo = start };
            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data == null) return;
                lock (output) output.AppendLine(e.Data);
                if (e.Data.StartsWith("listening=")) listening.Release();
            };
            process.ErrorDataReceived += (_, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (!WaitListening(TimeSpan.FromSeconds(60)))
            {
                var said = Output;
                Dispose();
                throw new InvalidOperationException("the stress server did not start:\n" + said);
            }
        }

        /// <summary>A worker listening (the first, or the next after a restart).</summary>
        public bool WaitListening(TimeSpan timeout) => listening.Wait(timeout) && !process.HasExited;

        /// <summary>The workers started so far (the supervised server's: one more for each restart).</summary>
        public int Workers { get { lock (output) return output.ToString().Split('\n').Count(l => l.StartsWith("listening=")); } }

        public string Output { get { lock (output) return output.ToString(); } }

        public bool HasExited => process.HasExited;

        public void Write(string path, string text) => File.WriteAllText(Full(path), text);

        public string Full(string path)
        {
            var file = Path.Combine(Root, path.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            return file;
        }

        public HttpClient Client(bool redirects = true) => Http.Client(Port, redirects);

        static int FreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        // The tests' folder as an application's bin: its files, the native files of this platform's runtimes only (the
        // others, and the native debug symbols, are most of its size).
        static void CopyProgram(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in Directory.EnumerateFiles(from)) File.Copy(file, Path.Combine(to, Path.GetFileName(file)));
            var platforms = OperatingSystem.IsWindows() ? new[] { "win", "win-x64", "win-arm64" } : new[] { "unix", "linux", "linux-x64", "linux-arm64" };
            foreach (var directory in Directory.EnumerateDirectories(from))
            {
                var name = Path.GetFileName(directory);
                if (name == "runtimes")
                {
                    foreach (var platform in Directory.EnumerateDirectories(directory).Where(d => platforms.Contains(Path.GetFileName(d))))
                        CopyTree(platform, Path.Combine(to, "runtimes", Path.GetFileName(platform)));
                }
                else CopyTree(directory, Path.Combine(to, name));
            }
        }

        static void CopyTree(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in Directory.EnumerateFiles(from).Where(f => !f.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase) || !from.Contains("runtimes")))
                File.Copy(file, Path.Combine(to, Path.GetFileName(file)));
            foreach (var directory in Directory.EnumerateDirectories(from)) CopyTree(directory, Path.Combine(to, Path.GetFileName(directory)));
        }

        public void Dispose()
        {
            try
            {
                // The site's compiled files' folder (System.Web's codegen folder: Temporary ASP.NET Files), said by the server.
                if (process != null && !process.HasExited)
                {
                    try
                    {
                        using var client = Client();
                        client.GetStringAsync("diag.ashx?codegen=1").Wait(10000);
                    }
                    catch (Exception) { }
                }
                if (process != null && !process.HasExited)
                {
                    process.StandardInput.Close();
                    if (!process.WaitForExit(20000)) process.Kill(entireProcessTree: true);
                }
                process?.WaitForExit();
            }
            catch (InvalidOperationException) { }
            process?.Dispose();
            process = null;
            foreach (var line in Output.Split('\n').Select(l => l.Trim()).Where(l => l.StartsWith("codegen=")))
                Delete(line["codegen=".Length..]);
            Delete(Root);
            Delete(LogDirectory);
            if (program != null)
            {
                Delete(program);
                // The supervisor's copies of bin (deleted as each worker ends; a worker killed leaves its own).
                Delete(System.Web.Hosting.WebFormsProcess.ShadowBase(program));
            }
        }

        public static void Delete(string folder)
        {
            for (var attempt = 0; attempt < 10; attempt++)
            {
                try
                {
                    if (Directory.Exists(folder)) Directory.Delete(folder, true);
                    return;
                }
                catch (Exception e) when (e is IOException or UnauthorizedAccessException) { Thread.Sleep(300); }
            }
        }
    }

    /// <summary>The clients' side: HTTP clients, a session's cookies, answers retried through a restart.</summary>
    static class Http
    {
        public static HttpClient Client(int port, bool redirects = true) =>
            new(new SocketsHttpHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.None,
                AllowAutoRedirect = redirects,
                MaxConnectionsPerServer = 512,
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(10),
                ConnectTimeout = TimeSpan.FromSeconds(10),
            })
            { BaseAddress = new Uri("http://127.0.0.1:" + port + "/"), Timeout = TimeSpan.FromSeconds(60) };

        /// <summary>The response's cookies as a Cookie header (name=value; ...).</summary>
        public static string Cookies(HttpResponseMessage response) =>
            string.Join("; ", (response.Headers.TryGetValues("Set-Cookie", out var values) ? values : Enumerable.Empty<string>()).Select(c => c.Split(';')[0]));

        public static async Task<(int Status, string Text, HttpResponseMessage Response)> Get(HttpClient client, string url, string cookies = null, Action<HttpRequestMessage> setup = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (cookies != null) request.Headers.TryAddWithoutValidation("Cookie", cookies);
            setup?.Invoke(request);
            var response = await client.SendAsync(request);
            var text = await response.Content.ReadAsStringAsync();
            return ((int)response.StatusCode, text, response);
        }

        /// <summary>The text's "name=value" fields (separated by spaces).</summary>
        public static Dictionary<string, string> Fields(string text) =>
            text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(f => f.Contains('='))
                .GroupBy(f => f[..f.IndexOf('=')]).ToDictionary(g => g.Key, g => g.Last()[(g.Last().IndexOf('=') + 1)..]);
    }

    /// <summary>The stress tests' site: its pages and handlers (compiled by the server), App_Code, static files.</summary>
    static class StressSite
    {
        public const string WebConfig = """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <system.web>
                <compilation debug="false" targetFramework="4.8" />
                <httpRuntime targetFramework="4.8" maxRequestLength="20480" />
                <customErrors mode="Off" />
                <sessionState timeout="20" />
                <httpModules><add name="StressModule" type="StressModule" /></httpModules>
              </system.web>
              <location path="locktimeout.ashx">
                <system.web><httpRuntime executionTimeout="2" /></system.web>
              </location>
              <system.webServer>
                <urlCompression doStaticCompression="true" />
              </system.webServer>
            </configuration>
            """;

        public static string Handler(string name, string body, string interfaces = "", string usings = "") =>
            "<%@ WebHandler Language=\"C#\" Class=\"" + name + "\" %>\r\nusing System;\r\nusing System.IO;\r\nusing System.Linq;\r\nusing System.Threading;\r\nusing System.Threading.Tasks;\r\nusing System.Web;\r\nusing System.Web.SessionState;\r\n" + usings +
            "public class " + name + " : IHttpHandler" + interfaces + "\r\n{\r\n    public bool IsReusable { get { return false; } }\r\n" +
            "    public void ProcessRequest(HttpContext c)\r\n    {\r\n        c.Response.ContentType = \"text/plain\";\r\n" + body + "\r\n    }\r\n}\r\n";

        public static IEnumerable<(string Path, string Text)> Files()
        {
            yield return ("web.config", WebConfig);
            yield return ("App_Code/StressModule.cs", """
                using System;
                using System.Threading.Tasks;
                using System.Web;

                // An async module (EventHandlerTaskAsyncHelper): its BeginRequest awaits, then marks the request; HttpContext.Current
                // must be the request's after the await.
                public class StressModule : IHttpModule
                {
                    public void Init(HttpApplication application)
                    {
                        var helper = new EventHandlerTaskAsyncHelper(BeginRequest);
                        application.AddOnBeginRequestAsync(helper.BeginEventHandler, helper.EndEventHandler);
                    }

                    async Task BeginRequest(object sender, EventArgs e)
                    {
                        var application = (HttpApplication)sender;
                        var context = application.Context;
                        if (context.Request.QueryString["module"] == null) return;
                        await Task.Delay(5);
                        context.Items["module"] = HttpContext.Current == context ? "ok" : "lost";
                    }

                    public void Dispose() { }
                }

                public sealed class CacheValue
                {
                    public readonly string Key;
                    public int Removed;
                    public volatile bool Seen;
                    public CacheValue(string key) { Key = key; }
                }

                public static class Stats
                {
                    public static int Disconnected;
                    public static int Waiting;
                }
                """);
            yield return ("page.aspx", "<%@ Page Language=\"C#\" %>\r\n<html><body>PAGE <%= Request.QueryString[\"i\"] %> <%= (Context.Items[\"module\"] ?? \"-\") %></body></html>\r\n");
            yield return ("sess.ashx", Handler("Sess", """
                        long enter = System.Diagnostics.Stopwatch.GetTimestamp();
                        object o = c.Session["n"];
                        int n = o == null ? 0 : (int)o;
                        int ms;
                        if (int.TryParse(c.Request.QueryString["ms"], out ms)) Thread.Sleep(ms);
                        c.Session["n"] = n + 1;
                        long exit = System.Diagnostics.Stopwatch.GetTimestamp();
                        c.Response.Write("n=" + n + " enter=" + enter + " exit=" + exit + " id=" + c.Session.SessionID);
                """, ", IRequiresSessionState"));
            yield return ("locktimeout.ashx", Handler("LockTimeout", """
                        long enter = System.Diagnostics.Stopwatch.GetTimestamp();
                        object o = c.Session["n"];
                        int n = o == null ? 0 : (int)o;
                        c.Session["n"] = n + 10;
                        c.Response.Write("n=" + n + " enter=" + enter + " exit=" + System.Diagnostics.Stopwatch.GetTimestamp());
                """, ", IRequiresSessionState"));
            yield return ("sessro.ashx", Handler("SessReadOnly", """
                        long enter = System.Diagnostics.Stopwatch.GetTimestamp();
                        object o = c.Session["n"];
                        int ms;
                        if (int.TryParse(c.Request.QueryString["ms"], out ms)) Thread.Sleep(ms);
                        c.Response.Write("n=" + (o == null ? 0 : (int)o) + " enter=" + enter + " exit=" + System.Diagnostics.Stopwatch.GetTimestamp());
                """, ", IReadOnlySessionState"));
            yield return ("app.ashx", Handler("App", """
                        c.Application.Lock();
                        try
                        {
                            object o = c.Application["n"];
                            int n = o == null ? 0 : (int)o;
                            Thread.SpinWait(1000);
                            c.Application["n"] = n + 1;
                            c.Response.Write("n=" + n);
                        }
                        finally { c.Application.UnLock(); }
                """));
            yield return ("block.ashx", Handler("Block", """
                        int ms;
                        int.TryParse(c.Request.QueryString["ms"], out ms);
                        Thread.Sleep(ms);
                        c.Response.Write("BLOCK " + ms);
                """));
            // Waits for the client to go (Response.IsClientConnected), counted in Stats.
            yield return ("wait.ashx", Handler("Wait", """
                        Interlocked.Increment(ref Stats.Waiting);
                        try
                        {
                            var clock = System.Diagnostics.Stopwatch.StartNew();
                            while (c.Response.IsClientConnected && clock.ElapsedMilliseconds < 20000) Thread.Sleep(20);
                            if (!c.Response.IsClientConnected) Interlocked.Increment(ref Stats.Disconnected);
                        }
                        finally { Interlocked.Decrement(ref Stats.Waiting); }
                """));
            yield return ("stats.ashx", Handler("StatsHandler", """
                        c.Response.Write("disconnected=" + Stats.Disconnected + " waiting=" + Stats.Waiting);
                """));
            yield return ("task.ashx", """
                <%@ WebHandler Language="C#" Class="TaskHandler" %>
                using System;
                using System.Threading.Tasks;
                using System.Web;

                public class TaskHandler : HttpTaskAsyncHandler
                {
                    public override async Task ProcessRequestAsync(HttpContext c)
                    {
                        c.Response.ContentType = "text/plain";
                        await Task.Delay(10);
                        var first = HttpContext.Current == c;
                        await Task.Yield();
                        var second = HttpContext.Current == c;
                        await Task.Delay(5).ConfigureAwait(false);
                        c.Response.Write("TASK " + c.Request.QueryString["i"] + " first=" + first + " second=" + second + " module=" + (c.Items["module"] ?? "-"));
                    }
                }
                """);
            yield return ("async.aspx", """
                <%@ Page Language="C#" Async="true" %>
                <script runat="server">
                string seen = "none";
                protected void Page_Load(object sender, EventArgs e)
                {
                    RegisterAsyncTask(new PageAsyncTask(async () =>
                    {
                        await System.Threading.Tasks.Task.Delay(10);
                        var before = HttpContext.Current == Context;
                        await System.Threading.Tasks.Task.WhenAll(System.Threading.Tasks.Task.Delay(5), System.Threading.Tasks.Task.Run(() => System.Threading.Thread.Sleep(5)));
                        seen = "ctx=" + before + "/" + (HttpContext.Current == Context);
                    }));
                }
                </script>
                <html><body>ASYNC <%= Request.QueryString["i"] %> <%= seen %></body></html>
                """);
            yield return ("upload.ashx", Handler("Upload", """
                        var file = c.Request.Files["f"];
                        long sum = 0;
                        using (var s = file.InputStream) { int b; while ((b = s.ReadByte()) >= 0) sum += b; }
                        c.Response.Write("UPLOAD length=" + file.ContentLength + " sum=" + sum);
                """));
            // What the process holds, after a full collection: its memory, handles (Windows) or descriptors (Linux), threads;
            // the uploads' temporary files left; System.Web's codegen folder of the site.
            yield return ("diag.ashx", Handler("Diag", """
                        if (c.Request.QueryString["codegen"] != null) { Console.WriteLine("codegen=" + HttpRuntime.CodegenDir); Console.Out.Flush(); }
                        GC.Collect(2, GCCollectionMode.Forced, true, true);
                        GC.WaitForPendingFinalizers();
                        GC.Collect(2, GCCollectionMode.Forced, true, true);
                        var process = System.Diagnostics.Process.GetCurrentProcess();
                        var handles = Environment.OSVersion.Platform == PlatformID.Win32NT ? process.HandleCount : Directory.GetFiles("/proc/self/fd").Length;
                        var uploads = Path.Combine(HttpRuntime.CodegenDir, "uploads");
                        var left = Directory.Exists(uploads) ? Directory.GetFiles(uploads).Length : 0;
                        c.Response.Write("memory=" + GC.GetTotalMemory(true) + " handles=" + handles + " threads=" + process.Threads.Count +
                            " pool=" + ThreadPool.ThreadCount + " uploads=" + left);
                """));
            // Cache items depending on App_Data/depN.txt (CacheDependency's files): inserted, or counted.
            yield return ("filedep.ashx", Handler("FileDependency", """
                        int k = int.Parse(c.Request.QueryString["file"]);
                        string file = c.Server.MapPath("~/App_Data/dep" + k + ".txt");
                        string prefix = "filedep-" + k + "-";
                        if (c.Request.QueryString["op"] == "insert")
                        {
                            int n = int.Parse(c.Request.QueryString["n"]);
                            for (int i = 0; i < n; i++)
                                c.Cache.Insert(prefix + c.Request.QueryString["tag"] + "-" + i, file, new System.Web.Caching.CacheDependency(file));
                            c.Response.Write("inserted=" + n);
                            return;
                        }
                        int count = 0;
                        foreach (System.Collections.DictionaryEntry e in c.Cache)
                            if (((string)e.Key).StartsWith(prefix)) count++;
                        c.Response.Write("count=" + count);
                """));
            for (var k = 0; k < 4; k++) yield return ("App_Data/dep" + k + ".txt", "0");
            // The Cache from many threads (?threads=&ops=): inserts with key dependencies, expirations and removal callbacks, gets,
            // removes; what Get returns is the key's, each item seen in the cache removed once (its callback once), an item
            // gone at once when its dependency is removed. Its errors counted.
            yield return ("cachestress.ashx", Handler("CacheStress", """
                        var cache = HttpRuntime.Cache;
                        var prefix = "stress-" + Guid.NewGuid().ToString("N") + "-";
                        var values = new System.Collections.Concurrent.ConcurrentBag<CacheValue>();
                        var errors = new System.Collections.Concurrent.ConcurrentQueue<string>();
                        System.Web.Caching.CacheItemRemovedCallback removed = (key, value, reason) =>
                        {
                            var v = value as CacheValue;
                            if (v != null && Interlocked.Increment(ref v.Removed) > 1) errors.Enqueue(key + " removed twice (" + reason + ")");
                        };
                        var keys = Enumerable.Range(0, 64).Select(i => prefix + "k" + i).ToArray();
                        var dependencies = Enumerable.Range(0, 8).Select(i => prefix + "d" + i).ToArray();
                        var none = System.Web.Caching.Cache.NoAbsoluteExpiration;
                        var noSliding = System.Web.Caching.Cache.NoSlidingExpiration;
                        foreach (var d in dependencies) cache.Insert(d, new CacheValue(d), null, none, noSliding, System.Web.Caching.CacheItemPriority.NotRemovable, removed);
                        int threadCount = int.Parse(c.Request.QueryString["threads"]);
                        int operations = int.Parse(c.Request.QueryString["ops"]);
                        var barrier = new Barrier(threadCount);
                        var threads = Enumerable.Range(0, threadCount).Select(t => new Thread(() =>
                        {
                            try
                            {
                                barrier.SignalAndWait();
                                var random = new Random(t);
                                for (int i = 0; i < operations; i++)
                                {
                                    var key = keys[random.Next(keys.Length)];
                                    int op = random.Next(10);
                                    if (op < 2)
                                    {
                                        var value = new CacheValue(key);
                                        values.Add(value);
                                        var dependency = random.Next(3) == 0 ? new System.Web.Caching.CacheDependency(null, new[] { dependencies[random.Next(dependencies.Length)] }) : null;
                                        var absolute = random.Next(4) == 0 ? DateTime.UtcNow.AddMilliseconds(random.Next(1, 50)) : none;
                                        var sliding = absolute == none && random.Next(4) == 0 ? TimeSpan.FromMilliseconds(random.Next(1, 50)) : noSliding;
                                        cache.Insert(key, value, dependency, absolute, sliding, System.Web.Caching.CacheItemPriority.Default, removed);
                                    }
                                    else if (op == 2)
                                    {
                                        var value = new CacheValue(key);
                                        values.Add(value);
                                        var existing = cache.Add(key, value, null, none, noSliding, System.Web.Caching.CacheItemPriority.Default, removed) as CacheValue;
                                        if (existing != null && existing.Key != key) errors.Enqueue(key + ": Add returned the item of " + existing.Key);
                                    }
                                    else if (op == 3) cache.Remove(key);
                                    else if (op == 4)
                                    {
                                        var d = dependencies[random.Next(dependencies.Length)];
                                        cache.Remove(d);
                                        cache.Insert(d, new CacheValue(d), null, none, noSliding, System.Web.Caching.CacheItemPriority.NotRemovable, removed);
                                    }
                                    else
                                    {
                                        var got = cache.Get(key);
                                        var v = got as CacheValue;
                                        if (v != null) { v.Seen = true; if (v.Key != key) errors.Enqueue(key + ": Get returned the item of " + v.Key); }
                                        else if (got != null) errors.Enqueue(key + ": Get returned " + got);
                                    }
                                }
                                var own = prefix + "own" + t;
                                var ownDependency = prefix + "owndep" + t;
                                cache.Insert(ownDependency, new CacheValue(ownDependency));
                                var ownValue = new CacheValue(own);
                                values.Add(ownValue);
                                cache.Insert(own, ownValue, new System.Web.Caching.CacheDependency(null, new[] { ownDependency }), none, noSliding, System.Web.Caching.CacheItemPriority.Default, removed);
                                if (cache.Get(own) != ownValue) errors.Enqueue(own + " not there"); else ownValue.Seen = true;
                                cache.Remove(ownDependency);
                                if (cache.Get(own) != null) errors.Enqueue(own + " still there after its dependency was removed");
                            }
                            catch (Exception e) { errors.Enqueue(e.ToString()); }
                        })).ToList();
                        threads.ForEach(x => x.Start());
                        foreach (var x in threads) if (!x.Join(60000)) errors.Enqueue("a thread did not end (a deadlock?)");
                        foreach (var key in keys.Concat(dependencies)) cache.Remove(key);
                        SpinWait.SpinUntil(() => values.Where(v => v.Seen).All(v => v.Removed == 1), 5000);
                        var missing = values.Where(v => v.Seen && v.Removed != 1).Select(v => v.Key + " " + v.Removed).ToList();
                        var left = keys.Count(k => cache.Get(k) != null);
                        c.Response.Write("errors=" + errors.Count + " missing=" + missing.Count + " left=" + left + " seen=" + values.Count(v => v.Seen) +
                            "\n" + string.Join("\n", errors.Take(10)) + "\n" + string.Join(",", missing.Take(10)));
                """));
            yield return ("small.txt", Content(0, 3000));
            yield return ("large.txt", Content(0, 300 * 1024));
            yield return ("folder/default.aspx", "<%@ Page Language=\"C#\" %>FOLDER");
        }

        /// <summary>A version of a file's content: its version and length in each line, so that a mix of two shows.</summary>
        public static string Content(int version, int length)
        {
            var line = "version " + version.ToString("D6") + " of a stress test's file, with words to compress " + new string('x', 20) + "\r\n";
            var text = new StringBuilder(length + line.Length);
            while (text.Length < length) text.Append(line);
            return text.ToString(0, length - 6) + version.ToString("D6");
        }

        /// <summary>The version a body is entirely (-1: a mix, or not one).</summary>
        public static int VersionOf(string body, int length)
        {
            if (body.Length < 14 || !body.StartsWith("version ")) return -1;
            if (!int.TryParse(body.AsSpan(8, 6), out var version)) return -1;
            return body == Content(version, length) ? version : -1;
        }
    }
}
