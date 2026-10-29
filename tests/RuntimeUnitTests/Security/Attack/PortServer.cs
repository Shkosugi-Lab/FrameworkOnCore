using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FrameworkOnCore.Runtime.Tests.Hosting
{
    /// <summary>
    /// The child's side of the attack tests' server (Security/AttackTests): a converted application's host as the converter's
    /// Program template makes it (IIS's features, then Web Forms; templates/ProgramTemplate.txt) over the probe site's folder,
    /// on Kestrel at 127.0.0.1:port, until its standard input is closed. One process: System.Web's application (its
    /// configuration system, its compilation of the site's pages) is one per process.
    /// </summary>
    static class AttackServer
    {
        static int Serve(string[] args)
        {
            var root = args[0];
            var port = int.Parse(args[1]);
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { ContentRootPath = root, Args = Array.Empty<string>() });
            builder.Logging.ClearProviders();
            if (Environment.GetEnvironmentVariable("FOC_ATTACK_LOG") == "1") builder.Logging.AddConsole();
            builder.WebHost.UseKestrel(options => options.Listen(IPAddress.Loopback, port));
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            var app = builder.Build();
            app.UseWebFormsIis();
            app.UseSession();
            app.UseWebForms(options => options.PhysicalPath(root).UseAspNetCoreSessionProvider());
            app.StartAsync().GetAwaiter().GetResult();
            Child.Say("listening", port);
            Console.In.ReadToEnd();
            app.StopAsync().Wait(5000);
            return 0;
        }
    }
}

namespace FrameworkOnCore.Runtime.Tests.Security
{
    using FrameworkOnCore.Runtime.Tests.Hosting;

    /// <summary>The port's server of the probe site (AttackServer, in a process of its own), started once for a test class.</summary>
    public sealed class PortServer : IDisposable
    {
        readonly Process process;
        readonly StringBuilder output = new();
        public readonly string Root;
        public readonly int Port;

        public PortServer()
        {
            Root = Directory.CreateTempSubdirectory("foc-attack").FullName;
            ProbeSite.Write(Root);
            Port = FreePort();
            var start = new ProcessStartInfo(Child.Dotnet)
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = AppContext.BaseDirectory,
            };
            start.ArgumentList.Add(Child.Program);
            start.ArgumentList.Add("AttackServer.Serve");
            start.ArgumentList.Add(Root);
            start.ArgumentList.Add(Port.ToString());
            foreach (var name in new[] { "WEBFORMSFORCORE_WORKER", "WEBFORMSFORCORE_SHADOWCOPY", "WEBFORMSFORCORE_SUPERVISOR", "WEBFORMSFORCORE_APP_PATH" })
                start.Environment.Remove(name);
            start.Environment["ASPNETCORE_ENVIRONMENT"] = "Production";
            process = new Process { StartInfo = start };
            var listening = new ManualResetEventSlim();
            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data == null) return;
                lock (output) output.AppendLine(e.Data);
                if (e.Data.StartsWith("listening=")) listening.Set();
            };
            process.ErrorDataReceived += (_, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (!listening.Wait(60000) || process.HasExited)
            {
                Dispose();
                throw new InvalidOperationException("the port's server did not start:\n" + Output);
            }
        }

        public string Output { get { lock (output) return output.ToString(); } }

        static int FreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public void Dispose()
        {
            try
            {
                if (!process.HasExited)
                {
                    process.StandardInput.Close();
                    if (!process.WaitForExit(10000)) process.Kill(entireProcessTree: true);
                }
                process.WaitForExit();
            }
            catch (InvalidOperationException) { }
            process.Dispose();
            ProbeSite.Delete(Root);
        }
    }
}
