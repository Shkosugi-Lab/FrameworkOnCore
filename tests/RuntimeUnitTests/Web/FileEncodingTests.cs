using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Configuration;
using System.Web.Util;
using FrameworkOnCore.Runtime.Tests.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace FrameworkOnCore.Runtime.Tests.Web
{
    /// <summary>
    /// The files the server reads in Shift_JIS (code page 932) without a byte order mark, as a Japanese Windows' Visual Studio
    /// saved them: .NET Framework's ASP.NET read the pages, App_Code and the other files it compiles in web.config's
    /// &lt;globalization fileEncoding&gt;, else in Encoding.Default, the system's ANSI code page (AnsiCodePage: the original
    /// server's, FrameworkOnCore.AnsiCodePage, which the converter writes); .NET's Encoding.Default is UTF-8 (U+FFFD where the
    /// Japanese was, App_Code not compiling). requestEncoding and responseEncoding shift_jis, web.config in Shift_JIS.
    /// </summary>
    public class FileEncodingTests
    {
        static FileEncodingTests() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        static readonly Encoding ShiftJis = Encoding.GetEncoding(932);

        [Fact]
        public void The_ansi_code_page_is_the_options_else_the_systems_else_the_cultures()
        {
            Assert.Equal(932, AnsiCodePage.Of("932", 1252, CultureInfo.GetCultureInfo("en-US")).CodePage);
            Assert.Equal(932, AnsiCodePage.Of(932, null, CultureInfo.InvariantCulture).CodePage);
            Assert.Equal(1252, AnsiCodePage.Of(null, 1252, CultureInfo.GetCultureInfo("ja-JP")).CodePage);
            // Linux: no system code page; the culture's (LANG: the original server's culture).
            Assert.Equal(932, AnsiCodePage.Of(null, null, CultureInfo.GetCultureInfo("ja-JP")).CodePage);
            Assert.Equal(1252, AnsiCodePage.Of(null, null, CultureInfo.GetCultureInfo("en-US")).CodePage);
            // An option that is no code page: as without it.
            Assert.Equal(936, AnsiCodePage.Of("shift_jis", null, CultureInfo.GetCultureInfo("zh-CN")).CodePage);
            Assert.Equal(936, AnsiCodePage.Of("0", null, CultureInfo.GetCultureInfo("zh-CN")).CodePage);
        }

        [Fact]
        public void Without_a_configured_encoding_a_file_is_its_byte_order_marks_else_utf8_else_the_ansi_code_page()
        {
            const string text = "表示・ソフト・能力 <%= \"十\" %>";
            var shiftJis = ShiftJis.GetBytes(text);
            Assert.Same(AnsiCodePage.Encoding, AnsiCodePage.EncodingOf(shiftJis));
            Assert.Equal(AnsiCodePage.Encoding.GetString(shiftJis), Read(shiftJis, null));
            Assert.Equal(text, Read(Encoding.UTF8.GetBytes(text), null));
            Assert.Equal(text, Read(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(text)).ToArray(), null));
            Assert.Equal(text, Read(Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(text)).ToArray(), null));
            Assert.Equal(text, AnsiCodePage.Decode(Encoding.UTF8.GetBytes(text), null));
        }

        [Fact]
        public void A_configured_encoding_is_taken_a_byte_order_mark_over_it_as_on_net_framework()
        {
            const string text = "宣言された fileEncoding:申込表";
            Assert.Equal(text, Read(ShiftJis.GetBytes(text), ShiftJis));
            Assert.Equal(text, Read(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(text)).ToArray(), ShiftJis));
            // utf-8 configured, a Shift_JIS file: what .NET Framework read too (not the code page).
            Assert.NotEqual(text, Read(ShiftJis.GetBytes(text), Encoding.UTF8));
        }

        static string Read(byte[] bytes, Encoding configured)
        {
            using var reader = AnsiCodePage.Reader(new MemoryStream(bytes), configured);
            return reader.ReadToEnd();
        }

        [Fact] // a .resx's file reference to a text file without an encoding named (.NET Framework: Encoding.Default)
        public void A_resx_text_file_without_an_encoding_is_read_as_net_framework_read_it()
        {
            var shiftJis = Path.GetTempFileName();
            var utf8 = Path.GetTempFileName();
            File.WriteAllBytes(shiftJis, ShiftJis.GetBytes("表示能力"));
            File.WriteAllBytes(utf8, Encoding.UTF8.GetBytes("表示能力"));
            try
            {
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(System.Resources.ResXFileRef));
                Assert.Equal(AnsiCodePage.Encoding.GetString(ShiftJis.GetBytes("表示能力")), converter.ConvertFromInvariantString(shiftJis + ";System.String"));
                Assert.Equal("表示能力", converter.ConvertFromInvariantString(shiftJis + ";System.String;shift_jis"));
                Assert.Equal("表示能力", converter.ConvertFromInvariantString(utf8 + ";System.String"));
            }
            finally { File.Delete(shiftJis); File.Delete(utf8); }
        }

        [Fact]
        public void Globalization_tells_whether_web_config_gives_a_file_encoding()
        {
            var section = new GlobalizationSection();
            Assert.False(section.IsFileEncodingSet);
            Assert.Same(AnsiCodePage.Encoding, section.FileEncoding);
            section.FileEncoding = ShiftJis;
            Assert.True(section.IsFileEncodingSet);
            Assert.Equal(932, section.FileEncoding.CodePage);
        }

        // ------------------------------------------------------------------ a site in Shift_JIS, served

        const string WebConfig = """
            <?xml version="1.0" encoding="shift_jis"?>
            <!-- Shift_JIS の web.config -->
            <configuration>
              <appSettings><add key="SiteTitle" value="ソフト表示能力" /></appSettings>
              <system.web>
                <compilation debug="false" targetFramework="4.8" />
                <globalization requestEncoding="shift_jis" responseEncoding="shift_jis" />
                <customErrors mode="Off" />
              </system.web>
            </configuration>
            """;

        static void WriteSite(string root)
        {
            void Write(string path, string text, Encoding encoding)
            {
                var file = Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                File.WriteAllBytes(file, encoding.GetPreamble().Concat(encoding.GetBytes(text)).ToArray());
            }
            var shiftJis = Encoding.GetEncoding(932);
            Write("web.config", WebConfig, shiftJis);
            Write("App_Code/Greeting.cs", """
                // App_Code(Shift_JIS):サーバーがコンパイルする
                public static class Greeting
                {
                    public const string 表題 = "表示能力(ソ十)";
                    public static string Hello(string 名前) { return "ようこそ、" + 名前 + " 様"; }
                }
                """, shiftJis);
            Write("default.aspx", """
                <%@ Page Language="C#" %>
                <script runat="server">
                    string 案内() { return "スクリプト:申込表"; }
                </script>
                <html><body>
                <p>マークアップ:ソフト表示</p>
                <p><%= Greeting.Hello("太郎") %> <%= Greeting.表題 %> <%= 案内() %></p>
                <p><%= System.Configuration.ConfigurationManager.AppSettings["SiteTitle"] %></p>
                <p><%= Response.ContentEncoding.WebName %></p>
                </body></html>
                """, shiftJis);
            Write("declared/web.config", """
                <?xml version="1.0" encoding="shift_jis"?>
                <configuration><system.web><globalization fileEncoding="shift_jis" /></system.web></configuration>
                """, shiftJis);
            Write("declared/page.aspx", """
                <%@ Page Language="C#" %>
                <html><body><p>宣言あり:<%= "能力十分" %></p></body></html>
                """, shiftJis);
            Write("bom.aspx", """
                <%@ Page Language="C#" %>
                <html><body><p>BOM 付き UTF-8:<%= "表示" %></p></body></html>
                """, new UTF8Encoding(true));
            Write("form.aspx", """
                <%@ Page Language="C#" %>
                <html><body><p>送信:<%= Request.Form["name"] %> (<%= Request.Form["name"] == null ? 0 : Request.Form["name"].Length %>)</p></body></html>
                """, shiftJis);
        }

        [Fact]
        public void A_shift_jis_site_is_served_as_iis_on_a_japanese_windows_served_it()
        {
            using var server = new EncodingServer(WriteSite, "932");
            using var client = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:" + server.Port) };
            string Get(string path)
            {
                var response = client.GetAsync(path).GetAwaiter().GetResult();
                var body = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                Assert.True(response.IsSuccessStatusCode, path + ": " + (int)response.StatusCode + "\n" + ShiftJis.GetString(body) + "\n" + server.Output);
                Assert.Equal("shift_jis", response.Content.Headers.ContentType?.CharSet);
                return ShiftJis.GetString(body);
            }

            var page = Get("/default.aspx");
            Assert.Contains("マークアップ:ソフト表示", page);
            Assert.Contains("ようこそ、太郎 様 表示能力(ソ十) スクリプト:申込表", page);
            Assert.Contains("ソフト表示能力", page);
            Assert.Contains("<p>shift_jis</p>", page);
            Assert.Contains("宣言あり:能力十分", Get("/declared/page.aspx"));
            Assert.Contains("BOM 付き UTF-8:表示", Get("/bom.aspx"));

            // A form posted in Shift_JIS (the page's charset): requestEncoding.
            var form = new ByteArrayContent(Encoding.ASCII.GetBytes("name=" + HttpUtility.UrlEncode("山田 ソフト表", ShiftJis)));
            form.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var posted = client.PostAsync("/form.aspx", form).GetAwaiter().GetResult();
            Assert.Contains("送信:山田 ソフト表 (7)", ShiftJis.GetString(posted.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult()));
        }

        [Fact] // what the port did before (its default fileEncoding .NET's Encoding.Default, UTF-8)
        public void Read_as_utf8_the_shift_jis_app_code_does_not_compile()
        {
            using var server = new EncodingServer(WriteSite, "65001");
            using var client = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:" + server.Port) };
            var response = client.GetAsync("/default.aspx").GetAwaiter().GetResult();
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            // The pages in a folder whose web.config gives fileEncoding are read in it whatever the code page.
            var declared = client.GetAsync("/declared/page.aspx").GetAwaiter().GetResult();
            Assert.Equal(HttpStatusCode.InternalServerError, declared.StatusCode);  // App_Code is the whole application's
        }

        static class HttpUtility
        {
            public static string UrlEncode(string text, Encoding encoding) => string.Concat(encoding.GetBytes(text).Select(b =>
                b is >= (byte)'a' and <= (byte)'z' or >= (byte)'A' and <= (byte)'Z' or >= (byte)'0' and <= (byte)'9' ? ((char)b).ToString() : b == ' ' ? "+" : "%" + b.ToString("X2")));
        }

        /// <summary>The host as the converter's Program writes it (Security/Attack/PortServer's), over a site written for
        /// the test, the runtime option FrameworkOnCore.AnsiCodePage given, in a process of its own (System.Web's
        /// application is one per process).</summary>
        sealed class EncodingServer : IDisposable
        {
            readonly Process process;
            readonly StringBuilder output = new();
            readonly string root = Directory.CreateTempSubdirectory("foc-encoding").FullName;
            public readonly int Port;

            public EncodingServer(Action<string> write, string ansiCodePage)
            {
                write(root);
                var listener = new TcpListener(IPAddress.Loopback, 0);
                listener.Start();
                Port = ((IPEndPoint)listener.LocalEndpoint).Port;
                listener.Stop();
                var start = new ProcessStartInfo(Child.Dotnet)
                {
                    UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true,
                    WorkingDirectory = AppContext.BaseDirectory,
                };
                start.ArgumentList.Add(Child.Program);
                start.ArgumentList.Add("EncodingSite.Serve");
                start.ArgumentList.Add(root);
                start.ArgumentList.Add(Port.ToString(CultureInfo.InvariantCulture));
                start.ArgumentList.Add(ansiCodePage);
                foreach (var name in new[] { "WEBFORMSFORCORE_WORKER", "WEBFORMSFORCORE_SHADOWCOPY", "WEBFORMSFORCORE_SUPERVISOR", "WEBFORMSFORCORE_APP_PATH" })
                    start.Environment.Remove(name);
                start.Environment["ASPNETCORE_ENVIRONMENT"] = "Production";
                process = new Process { StartInfo = start };
                var listening = new ManualResetEventSlim();
                process.OutputDataReceived += (_, e) =>
                {
                    if (e.Data == null) return;
                    lock (output) output.AppendLine(e.Data);
                    if (e.Data.StartsWith("listening=", StringComparison.Ordinal)) listening.Set();
                };
                process.ErrorDataReceived += (_, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (!listening.Wait(60000) || process.HasExited)
                {
                    Dispose();
                    throw new InvalidOperationException("the server did not start:\n" + Output);
                }
            }

            public string Output { get { lock (output) return output.ToString(); } }

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
                try { Directory.Delete(root, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
            }
        }
    }
}

namespace FrameworkOnCore.Runtime.Tests.Hosting
{
    /// <summary>The child's side of FileEncodingTests' server: the runtime option FrameworkOnCore.AnsiCodePage (the
    /// converter writes it into runtimeconfig.json) given, then the host as AttackServer's.</summary>
    static class EncodingSite
    {
        static int Serve(string[] args)
        {
            var root = args[0];
            var port = int.Parse(args[1], CultureInfo.InvariantCulture);
            AppContext.SetData("FrameworkOnCore.AnsiCodePage", args[2]);
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { ContentRootPath = root, Args = Array.Empty<string>() });
            builder.Logging.ClearProviders();
            builder.WebHost.UseKestrel(options => options.Listen(IPAddress.Loopback, port));
            var app = builder.Build();
            app.UseWebFormsIis();
            app.UseWebForms(options => options.PhysicalPath(root));
            app.StartAsync().GetAwaiter().GetResult();
            Child.Say("listening", port);
            Console.In.ReadToEnd();
            app.StopAsync().Wait(5000);
            return 0;
        }
    }
}
