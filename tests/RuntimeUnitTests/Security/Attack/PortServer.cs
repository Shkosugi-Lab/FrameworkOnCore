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
            // (the settings tests': HTTPS on another port, the client's certificate asked for in the handshake as IIS's binding
            // asks for it, any taken by TLS: the host says which it trusts, these tests' own)
            builder.WebHost.UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, port);
                if (args.Length > 2)
                    options.Listen(IPAddress.Loopback, int.Parse(args[2]), listen => listen.UseHttps(new Microsoft.AspNetCore.Server.Kestrel.Https.HttpsConnectionAdapterOptions
                    {
                        ServerCertificate = System.Security.Cryptography.X509Certificates.X509CertificateLoader.LoadPkcs12FromFile(args[3], Security.SettingsCertificates.Password),
                        ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate,
                        ClientCertificateValidation = (_, _, _) => true,
                    }));
            });
            if (args.Length > 4)
                System.Web.Hosting.IisAccess.TrustedClientCertificates = new System.Security.Cryptography.X509Certificates.X509Certificate2Collection(
                    System.Security.Cryptography.X509Certificates.X509CertificateLoader.LoadCertificateFromFile(args[4]));
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            var app = builder.Build();
            // The step that answered, for a request marked as a case's (Target.Marker): the module of IIS's it stands for
            // (IisDecision), which the tests hold to the one IIS's own trace names.
            app.Use((context, next) =>
            {
                if (context.Request.Headers.ContainsKey("X-Foc-Case"))
                    context.Response.OnStarting(() =>
                    {
                        if (context.Features.Get<System.Web.Hosting.IisDecision>() is { } decision) context.Response.Headers["X-Foc-Decided"] = decision.Module;
                        return Task.CompletedTask;
                    });
                return next(context);
            });
            app.UseWebFormsIis();
            app.UseSession();
            // (every request to Web Forms, as the converter's Program.cs has it for an application whose modules IIS runs for
            // every request: HostProgram, ModulesForAllRequests)
            var allRequests = Environment.GetEnvironmentVariable("FOC_ATTACK_ALL_REQUESTS") == "1";
            app.UseWebForms(options =>
            {
                options.PhysicalPath(root).UseAspNetCoreSessionProvider();
                if (allRequests) options.HandleAllRequestsWithWebForms();
            });
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
        /// <summary>Its HTTPS port (0: none).</summary>
        public readonly int HttpsPort;

        readonly Action<string> delete;

        public PortServer() : this(ProbeSite.Write, ProbeSite.Delete) { }

        /// <summary>The port's server of another site (the settings tests'), written into a folder of its own and deleted with it;
        /// with HTTPS on a port of its own (SettingsCertificates: the server's made for it, the trusted client's trusted); every
        /// request to Web Forms (allRequests) as the converter's Program.cs has it for modules IIS runs for every request.</summary>
        public PortServer(Action<string> write, Action<string> delete, bool https = false, bool allRequests = false)
        {
            this.delete = delete;
            Root = Directory.CreateTempSubdirectory("foc-attack").FullName;
            write(Root);
            Port = FreePort();
            var extra = new List<string>();
            if (https)
            {
                HttpsPort = FreePort();
                var folder = Directory.CreateDirectory(Path.Combine(Root, "App_Data", "foc-tls")).FullName;
                using var server = SettingsCertificates.Make(SettingsCertificates.ServerSubject, server: true);
                SettingsCertificates.Write(server, Path.Combine(folder, "server.pfx"), withKey: true);
                SettingsCertificates.Write(SettingsCertificates.Trusted, Path.Combine(folder, "trusted.cer"), withKey: false);
                extra.AddRange(new[] { HttpsPort.ToString(), Path.Combine(folder, "server.pfx"), Path.Combine(folder, "trusted.cer") });
            }
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
            foreach (var argument in extra) start.ArgumentList.Add(argument);
            foreach (var name in new[] { "WEBFORMSFORCORE_WORKER", "WEBFORMSFORCORE_SHADOWCOPY", "WEBFORMSFORCORE_SUPERVISOR", "WEBFORMSFORCORE_APP_PATH" })
                start.Environment.Remove(name);
            start.Environment["ASPNETCORE_ENVIRONMENT"] = "Production";
            if (allRequests) start.Environment["FOC_ATTACK_ALL_REQUESTS"] = "1";
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
            delete(Root);
        }
    }
}
