// A .NET Framework WebForms host with no IIS and no IIS Express.
//
// ===========================================================================
// STATE: DOES NOT WORK. Do not build a gate on this.
//
// ASP.NET renders through it - a request reaches System.Web and comes back as
// ASP.NET-generated HTML - but every request so far comes back as an ASP.NET
// ERROR PAGE with status 200, not as the application's page. The last one
// standing is "構成システムは既に初期化されています".
//
// It is kept because the road to that point is most of the work, and every
// wrong turn on it is written down below and in corpora\README.md. The build
// half IS done and does work: corpora\build-original.ps1 builds BlogEngine
// without Visual Studio.
// ===========================================================================
//
// WHY THIS EXISTS
//
// The project's final gate is ParityTest: the converted app's DOM compared against the
// ORIGINAL WebForms app's actual rendering. That comparison only ever ran for the four
// hand-built apps under samples\, whose golden-webforms.json files were recorded on a
// machine with Visual Studio Build Tools and IIS Express and committed. None of the six
// real corpora has one, so "BlogEngine builds with 0 errors" has never meant "BlogEngine
// renders what it used to" - it is a proxy, and even at zero it would prove nothing about
// behaviour.
//
// This machine has the .NET Framework runtime but no IIS Express, so the recording half of
// that gate could not run at all.
//
// HOW IT WORKS
//
// ASP.NET does not need a web server to render. HttpRuntime.ProcessRequest drives a request
// through the real pipeline - the same System.Web, the same control rendering - and
// SimpleWorkerRequest's five-argument constructor takes the application's virtual and
// PHYSICAL paths, which is the form meant for hosting outside an ASP.NET-created AppDomain.
// An HttpListener in front of it makes that reachable by a browser, which is what
// ParityTest drives.
//
// WHAT WAS TRIED FIRST, AND WHY IT IS NOT HERE
//
// ApplicationHost.CreateApplicationHost and ApplicationManager.CreateObject both build the
// ASP.NET AppDomain for you and then resolve your host type inside it BY NAME
// (Type.GetType). That lookup never found this assembly - copied into the application's
// bin, copied into the application root, with the ASP.NET temp cache cleared, as a .dll and
// as a .exe. The same failure appeared for a two-line .aspx in an empty directory, so it
// was the hosting call and not the application. The documented remedy is to put the host
// assembly in the GAC, which needs an installer this environment does not have.
//
// Staying in one AppDomain costs the application's own assemblies their probing path, so
// they are resolved explicitly from its bin below.
//
// WHAT IT IS NOT
//
// SimpleWorkerRequest carries a path, a query string and an output writer. That covers GET,
// which is what the corpus scenarios do. It does NOT carry a request body, so a POST is
// refused loudly rather than served as a GET - a postback silently answered with the
// initial render would be written to disk as the golden master, and every later comparison
// would be measured against a lie.

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace WebFormsHost
{
    public static class Program
    {
        private static readonly string[] HandledExtensions =
            { ".aspx", ".ashx", ".asmx", ".axd", ".svc", string.Empty };

        private static string _physicalPath;
        private static string _binPath;

        public static int Main(string[] args)
        {
            string physicalPath = null;
            var port = 8099;

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "--path" && i + 1 < args.Length) physicalPath = args[++i];
                else if (args[i] == "--port" && i + 1 < args.Length) port = int.Parse(args[++i]);
            }

            if (physicalPath == null)
            {
                Console.Error.WriteLine("使い方: WebFormsHostRunner --path <アプリのディレクトリ> [--port 8099]");
                return 2;
            }

            physicalPath = Path.GetFullPath(physicalPath).TrimEnd(Path.DirectorySeparatorChar)
                           + Path.DirectorySeparatorChar;
            if (!Directory.Exists(physicalPath))
            {
                Console.Error.WriteLine("ディレクトリがありません: " + physicalPath);
                return 2;
            }

            _physicalPath = physicalPath;
            _binPath = Path.Combine(physicalPath, "bin");

            // The application's own assemblies are not on this domain's probing path - the
            // domain belongs to this tool, not to the application. Without this, the first
            // page that touches a code-behind type fails to load its assembly.
            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromApplicationBin;

            // What ApplicationHost would have stamped on the AppDomain it created. HttpRuntime
            // reads these on first use to learn which application it is serving; left unset it
            // initializes with an empty application path and its file-change monitor answers
            // every request with "'' ファイルを監視するためのファイル名が無効です" - a 200 whose
            // body is an error page, which is the shape that quietly becomes a golden master.
            var domain = AppDomain.CurrentDomain;
            domain.SetData(".appDomain", "*");
            domain.SetData(".appPath", physicalPath);
            domain.SetData(".appVPath", "/");
            domain.SetData(".appId", "webformshost");
            domain.SetData(".domainId", "webformshost");
            domain.SetData(".hostingVirtualPath", "/");
            domain.SetData(".hostingInstallDir",
                System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());

            // ASP.NET installs its OWN configuration system, and it can only do that while
            // no other one is in place. HttpListener reads <system.net> as it starts, which
            // initializes the default one, and ASP.NET then fails its first request with
            // "構成システムは既に初期化されています" - forever, because the failure is in
            // initialization rather than in the request.
            //
            // So ASP.NET goes first. Reading a setting through WebConfigurationManager is
            // enough - it installs HttpConfigurationSystem - and it is all that is wanted
            // here. Driving a whole warm-up REQUEST instead runs ASP.NET's pre-start
            // initialization on this thread, and compiling a page inside that phase is
            // refused: "このメソッドは、アプリケーション開始前の初期化段階では呼び出すことが
            // できません".
            try
            {
                var _ = System.Web.Configuration.WebConfigurationManager.AppSettings;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ASP.NET 構成の初期化に失敗しました: " + ex.Message);
            }

            var listener = new HttpListener();
            listener.Prefixes.Add(string.Format(CultureInfo.InvariantCulture, "http://localhost:{0}/", port));
            try
            {
                listener.Start();
            }
            catch (HttpListenerException ex)
            {
                Console.Error.WriteLine("待ち受けを開始できません: " + ex.Message);
                return 1;
            }

            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "listening http://localhost:{0}/  ({1})", port, physicalPath));
            Console.Out.Flush();

            while (listener.IsListening)
            {
                HttpListenerContext context;
                try { context = listener.GetContext(); }
                catch (HttpListenerException) { break; }
                catch (ObjectDisposedException) { break; }

                ThreadPool.QueueUserWorkItem(_ => Serve(context));
            }

            return 0;
        }

        private static Assembly ResolveFromApplicationBin(object sender, ResolveEventArgs args)
        {
            var simpleName = new AssemblyName(args.Name).Name;
            foreach (var extension in new[] { ".dll", ".exe" })
            {
                var candidate = Path.Combine(_binPath, simpleName + extension);
                if (File.Exists(candidate))
                {
                    return Assembly.LoadFrom(candidate);
                }
            }
            return null;
        }

        private static void Serve(HttpListenerContext context)
        {
            try
            {
                var path = context.Request.Url.AbsolutePath;
                var query = context.Request.Url.Query.TrimStart('?');

                if (!string.Equals(context.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    // Loud, not helpful. See the header: a postback answered with the
                    // initial render would be recorded as the truth.
                    Fail(context, 405,
                        "このホストは GET のみを通します(SimpleWorkerRequest にリクエスト本文がありません)。"
                        + " POST を含むシナリオは IIS Express で採取してください。");
                    return;
                }

                // A static file is served from disk. ASP.NET's static handler is not wired
                // up outside IIS, and the browser needs the CSS and images to build the DOM
                // the comparison is about.
                var relative = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var candidate = Path.Combine(_physicalPath, relative);
                if (relative.Length > 0 && File.Exists(candidate) && !IsHandled(candidate))
                {
                    WriteFile(context, candidate);
                    return;
                }

                // The THREE-argument constructor. The five-argument one takes the
                // application's paths, and once the AppDomain carries ".appPath" - which it
                // must, or ASP.NET's file-change monitor rejects every request - overriding
                // them is refused: "アプリケーション パスをこのコンテキストでオーバーライド
                // することはできません". The two settings are mutually exclusive, and the
                // domain data is the one that has to win.
                var output = new StringWriter(CultureInfo.InvariantCulture);
                HttpRuntime.ProcessRequest(new SimpleWorkerRequest(PageOf(path), query, output));
                WriteText(context, 200, output.ToString());
            }
            catch (Exception ex)
            {
                // To stderr as well as to the response. If writing the response is itself
                // what failed, the console is the only place the reason can still appear.
                Console.Error.WriteLine(ex.ToString());
                Console.Error.Flush();
                Fail(context, 500, ex.ToString());
            }
        }

        /// <summary>
        /// The page name SimpleWorkerRequest wants: relative to the application, with no
        /// leading slash. Passing the URL path straight through gives it "/Default.aspx",
        /// whose directory part is empty, and ASP.NET answers every request with
        /// "'' ファイルを監視するためのファイル名が無効です" instead of the page.
        ///
        /// A request for the directory itself is mapped to the default document, which IIS
        /// would have done. The list is probed against the application rather than assumed:
        /// BlogEngine's is default.aspx, WingtipToys' is Default.aspx, and a wrong guess
        /// would record a golden master of an error page.
        /// </summary>
        private static string PageOf(string urlPath)
        {
            var page = (urlPath ?? string.Empty).TrimStart('/');
            if (page.Length > 0 && !page.EndsWith("/", StringComparison.Ordinal))
            {
                return page;
            }

            var directory = Path.Combine(_physicalPath, page.Replace('/', Path.DirectorySeparatorChar));
            foreach (var candidate in new[]
                     { "Default.aspx", "default.aspx", "Index.aspx", "index.aspx", "Default.htm", "index.html" })
            {
                if (File.Exists(Path.Combine(directory, candidate)))
                {
                    return page + candidate;
                }
            }
            return page + "Default.aspx";
        }

        private static bool IsHandled(string file)
        {
            var extension = Path.GetExtension(file) ?? string.Empty;
            foreach (var handled in HandledExtensions)
            {
                if (string.Equals(extension, handled, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static void WriteFile(HttpListenerContext context, string file)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = ContentTypeOf(Path.GetExtension(file));
            var bytes = File.ReadAllBytes(file);
            context.Response.ContentLength64 = bytes.Length;
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.OutputStream.Close();
        }

        private static string ContentTypeOf(string extension)
        {
            switch ((extension ?? string.Empty).ToLowerInvariant())
            {
                case ".css": return "text/css";
                case ".js": return "application/javascript";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".svg": return "image/svg+xml";
                case ".ico": return "image/x-icon";
                case ".woff": return "font/woff";
                case ".woff2": return "font/woff2";
                case ".html":
                case ".htm": return "text/html; charset=utf-8";
                case ".xml": return "text/xml; charset=utf-8";
                case ".json": return "application/json; charset=utf-8";
                default: return "application/octet-stream";
            }
        }

        private static void WriteText(HttpListenerContext context, int status, string body)
        {
            context.Response.StatusCode = status;
            context.Response.ContentType = "text/html; charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes(body ?? string.Empty);
            context.Response.ContentLength64 = bytes.Length;
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.OutputStream.Close();
        }

        private static void Fail(HttpListenerContext context, int status, string message)
        {
            // Escaped by hand rather than with HttpUtility.HtmlEncode. Reaching into
            // System.Web here can throw on its own - the failure being reported may BE that
            // System.Web could not initialize - and the catch below would then swallow the
            // only description of what went wrong, leaving a 500 with an empty body.
            var escaped = (message ?? string.Empty)
                .Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            try { WriteText(context, status, "<pre>" + escaped + "</pre>"); }
            catch { }
        }
    }
}
