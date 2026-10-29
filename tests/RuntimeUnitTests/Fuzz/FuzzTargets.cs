// The differential fuzzing's targets: what each input is given to and what is recorded of the result. Compiled into the
// .NET Framework 4.8 recorder (golden/Recorder.csproj: .NET Framework's System.Web from the GAC) and into the port's tests
// (FuzzTests: FrameworkOnCore's System.Web), which compare the port's results with the recorded ones. The internals are
// reached by reflection, by the same names on both (the port keeps .NET Framework's).
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Web;

namespace FrameworkOnCore.Runtime.Tests.Fuzz
{
    /// <summary>A result's named parts, each shown as text (an exception as its type).</summary>
    sealed class Output
    {
        public readonly List<KeyValuePair<string, string>> Parts = new List<KeyValuePair<string, string>>();

        public void Add(string name, Func<object> value)
        {
            string shown;
            try { shown = Show.Value(value()); }
            catch (Exception e) { shown = Show.Error(e); }
            Parts.Add(new KeyValuePair<string, string>(name, shown));
        }
    }

    static class Show
    {
        /// <summary>
        /// A string as ASCII: '\' doubled, what is not printable ASCII (and '|', the parts' separator) as \uXXXX; null as \N.
        /// What the golden files hold (one line a case: no tab, no line break) and the tests compare.
        /// </summary>
        public static string Text(string s)
        {
            if (s == null) return "\\N";
            var sb = new StringBuilder(s.Length + 8);
            foreach (char c in s)
            {
                if (c == '\\') sb.Append("\\\\");
                else if (c >= 0x20 && c < 0x7F && c != '|') sb.Append(c);
                else sb.Append("\\u").Append(((int)c).ToString("X4", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        public static string Hex(byte[] bytes)
        {
            if (bytes == null) return "\\N";
            var sb = new StringBuilder("0x", bytes.Length * 2 + 2);
            foreach (var b in bytes) sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
            return sb.ToString();
        }

        public static string Error(Exception e)
        {
            while (e is TargetInvocationException && e.InnerException != null) e = e.InnerException;
            var shown = "\\!" + e.GetType().FullName;
            if (e is HttpException http) shown += ":" + http.GetHttpCode().ToString(CultureInfo.InvariantCulture);
            if (Environment.GetEnvironmentVariable("FUZZ_STACK") != null) Console.Error.WriteLine("FUZZSTACK " + e);
            return shown;
        }

        public static string Value(object value)
        {
            switch (value)
            {
                case null: return "\\N";
                case string s: return Text(s);
                case byte[] b: return Hex(b);
                case bool b: return b ? "true" : "false";
                case int i: return i.ToString(CultureInfo.InvariantCulture);
                case long l: return l.ToString(CultureInfo.InvariantCulture);
                case DateTime d: return d.Ticks.ToString(CultureInfo.InvariantCulture) + "/" + d.Kind;
                case NameValueCollection c: return Collection(c);
                case IEnumerable<string> list: return string.Join("|", list.Select(Text));
                default: return Text(Convert.ToString(value, CultureInfo.InvariantCulture));
            }
        }

        /// <summary>A collection's keys and values in order: key=value|..., several values of a key joined by ','.</summary>
        public static string Collection(NameValueCollection c)
        {
            var parts = new List<string>();
            for (int i = 0; i < c.Count; i++)
            {
                var values = c.GetValues(i);
                parts.Add(Text(c.GetKey(i)) + "=" + (values == null ? "\\N" : string.Join(",", values.Select(Text))));
            }
            return "[" + string.Join("|", parts) + "]";
        }
    }

    /// <summary>System.Web's internals by name.</summary>
    static class Web
    {
        public static readonly Assembly Assembly = typeof(HttpRuntime).Assembly;
        const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        static readonly Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();

        public static Type Type(string name) => Assembly.GetType(name, true);

        public static object Call(string type, object target, string name, Type[] types, params object[] args) =>
            Call(Type(type), target, name, types, args);

        public static object Call(Type type, object target, string name, Type[] types, params object[] args)
        {
            var key = type.FullName + "." + name + "(" + string.Join(",", types.Select(t => t.FullName)) + ")";
            MethodInfo method;
            lock (methods)
            {
                if (!methods.TryGetValue(key, out method))
                    methods[key] = method = type.GetMethod(name, All, null, types, null) ?? throw new MissingMethodException(key);
            }
            try { return method.Invoke(target, args); }
            catch (TargetInvocationException e) { ExceptionDispatchInfo.Capture(e.InnerException).Throw(); throw; }
        }

        public static object Get(object target, string name) =>
            target.GetType().GetProperty(name, All).GetValue(target);

        public static object Field(Type type, object target, string name) => type.GetField(name, All).GetValue(target);

        public static void SetField(Type type, object target, string name, object value) => type.GetField(name, All).SetValue(target, value);

        /// <summary>The application's virtual path (HttpRuntime.AppDomainAppVirtualPath) for the call; the previous one put back.</summary>
        public static T WithAppPath<T>(string appPath, Func<T> call)
        {
            var runtime = Field(typeof(HttpRuntime), null, "_theRuntime");
            var previous = Field(typeof(HttpRuntime), runtime, "_appDomainAppVPath");
            var created = Call("System.Web.VirtualPath", null, "CreateNonRelativeTrailingSlash", new[] { typeof(string) }, appPath);
            SetField(typeof(HttpRuntime), runtime, "_appDomainAppVPath", created);
            try { return call(); }
            finally { SetField(typeof(HttpRuntime), runtime, "_appDomainAppVPath", previous); }
        }

        public static object Encoder => typeof(System.Web.Util.HttpEncoder).GetProperty("Default").GetValue(null);
    }

    /// <summary>
    /// HttpUtility as .NET Framework's: on .NET Framework the class itself; on the port, what it does with FrameworkOnCore's
    /// HttpEncoder (System.Web's own code calls it so; an application's HttpUtility is .NET's, which FrameworkOnCore's System.Web
    /// forwards to: FuzzTests compares that one too).
    /// </summary>
    static class Framework
    {
#if NETFRAMEWORK
        public static string UrlEncode(string s, Encoding e) => HttpUtility.UrlEncode(s, e);
        public static string UrlEncodeUnicode(string s) => HttpUtility.UrlEncodeUnicode(s);
        public static string UrlPathEncode(string s) => HttpUtility.UrlPathEncode(s);
        public static string UrlDecode(string s, Encoding e) => HttpUtility.UrlDecode(s, e);
        public static string UrlDecode(byte[] b, Encoding e) => HttpUtility.UrlDecode(b, e);
        public static byte[] UrlDecodeToBytes(byte[] b) => HttpUtility.UrlDecodeToBytes(b);
        public static string HtmlEncode(string s) => HttpUtility.HtmlEncode(s);
        public static string HtmlAttributeEncode(string s) => HttpUtility.HtmlAttributeEncode(s);
        public static string HtmlDecode(string s) => HttpUtility.HtmlDecode(s);
        public static string JavaScriptStringEncode(string s) => HttpUtility.JavaScriptStringEncode(s);
        public static NameValueCollection ParseQueryString(string s, Encoding e) => HttpUtility.ParseQueryString(s, e);
        public const string UtilityType = "System.Web.HttpUtility";
#else
        static System.Web.Util.HttpEncoder Encoder => System.Web.Util.HttpEncoder.Default;
        public static string UrlEncode(string s, Encoding e)
        {
            if (s == null) return null;
            var bytes = e.GetBytes(s);
            return Encoding.ASCII.GetString(Encoder.UrlEncode(bytes, 0, bytes.Length, false));
        }
        public static string UrlEncodeUnicode(string s) => Encoder.UrlEncodeUnicode(s, false);
        public static string UrlPathEncode(string s) => Encoder.UrlPathEncode(s);
        public static string UrlDecode(string s, Encoding e) => Encoder.UrlDecode(s, e);
        public static string UrlDecode(byte[] b, Encoding e) => b == null ? null : Encoder.UrlDecode(b, 0, b.Length, e);
        public static byte[] UrlDecodeToBytes(byte[] b) => b == null ? null : Encoder.UrlDecode(b, 0, b.Length);
        public static string HtmlEncode(string s) => Encoder.HtmlEncode(s);
        public static string HtmlAttributeEncode(string s) => Encoder.HtmlAttributeEncode(s);
        public static string HtmlDecode(string s) => Encoder.HtmlDecode(s);
        public static string JavaScriptStringEncode(string s) => Encoder.JavaScriptStringEncode(s);
        public static NameValueCollection ParseQueryString(string query, Encoding encoding)
        {
            if (query == null) throw new ArgumentNullException("query");
            if (encoding == null) throw new ArgumentNullException("encoding");
            if (query.Length > 0 && query[0] == '?') query = query.Substring(1);
            return new HttpValueCollection(query, false, true, encoding);
        }
        public const string UtilityType = "System.Web.HttpUtilityInternal";
#endif
    }

    /// <summary>A request as a host gives it to System.Web: its URL, headers and body (preloaded whole).</summary>
    sealed class FuzzWorkerRequest : HttpWorkerRequest
    {
        readonly string verb, path, query;
        readonly byte[] queryBytes, body;
        readonly string[] known = new string[RequestHeaderMaximum];
        readonly List<string[]> unknown = new List<string[]>();

        public FuzzWorkerRequest(string verb, string path, string query, byte[] queryBytes, byte[] body, params string[] headers)
        {
            this.verb = verb; this.path = path; this.query = query ?? ""; this.queryBytes = queryBytes; this.body = body ?? new byte[0];
            for (int i = 0; i + 1 < headers.Length; i += 2)
            {
                if (headers[i + 1] == null) continue;
                int index = GetKnownRequestHeaderIndex(headers[i]);
                if (index >= 0) known[index] = headers[i + 1];
                else unknown.Add(new[] { headers[i], headers[i + 1] });
            }
            if (this.body.Length > 0) known[HeaderContentLength] = this.body.Length.ToString(CultureInfo.InvariantCulture);
        }

        public override string GetUriPath() => path;
        public override string GetQueryString() => query;
        public override byte[] GetQueryStringRawBytes() => queryBytes;
        public override string GetRawUrl() => query.Length > 0 ? path + "?" + query : path;
        public override string GetHttpVerbName() => verb;
        public override string GetHttpVersion() => "HTTP/1.1";
        public override string GetRemoteAddress() => "127.0.0.1";
        public override int GetRemotePort() => 50000;
        public override string GetLocalAddress() => "127.0.0.1";
        public override int GetLocalPort() => 80;
        public override string GetServerName() => "localhost";
        public override string GetAppPath() => "/";
        public override string GetKnownRequestHeader(int index) => known[index];
        public override string GetUnknownRequestHeader(string name) => unknown.FirstOrDefault(h => string.Equals(h[0], name, StringComparison.OrdinalIgnoreCase))?[1];
        public override string[][] GetUnknownRequestHeaders() => unknown.ToArray();
        // No body: none preloaded (HasEntityBody false, as a host has it for a request without one).
        public override byte[] GetPreloadedEntityBody() => body.Length > 0 ? body : null;
        public override int GetPreloadedEntityBodyLength() => body.Length;
        public override int GetTotalEntityBodyLength() => body.Length;
        public override bool IsEntireEntityBodyIsPreloaded() => true;
        public override int ReadEntityBody(byte[] buffer, int size) => 0;
        public override void SendStatus(int statusCode, string statusDescription) { }
        public override void SendKnownResponseHeader(int index, string value) { }
        public override void SendUnknownResponseHeader(string name, string value) { }
        public override void SendResponseFromMemory(byte[] data, int length) { }
        public override void SendResponseFromFile(string filename, long offset, long length) { }
        public override void SendResponseFromFile(IntPtr handle, long offset, long length) { }
        public override void FlushResponse(bool finalFlush) { }
        public override void EndOfRequest() { }
    }

    /// <summary>A target: its inputs (fields of text) and what is recorded of each.</summary>
    sealed class Target
    {
        public string Name;
        public int Count;
        /// <summary>What only Windows has (physical paths as Windows has them): not compared on Linux.</summary>
        public bool WindowsOnly;
        public Func<Rng, int, string[]> Input;
        public Action<string[], Output> Run;

        /// <summary>The inputs, from the target's own seed (its name's).</summary>
        public List<string[]> Inputs()
        {
            ulong seed = 14695981039346656037UL;
            foreach (char c in Name) seed = (seed ^ c) * 1099511628211UL;
            var r = new Rng(seed);
            var inputs = new List<string[]>(Count);
            for (int i = 0; i < Count; i++) inputs.Add(Input(r, i));
            return inputs;
        }

        public Output Execute(string[] input)
        {
            var output = new Output();
            Run(input, output);
            return output;
        }

        public static string Line(string[] input) => string.Join("\t", input.Select(Show.Text));

        public static string Line(Output output) => string.Join("\t", output.Parts.Select(p => p.Key + "=" + p.Value));
    }

    static class Targets
    {
        public static readonly string[] Encodings = { "utf-8", "shift_jis", "iso-8859-1" };
        static readonly string[] AppPaths = { "/", "/app", "/App/sub", "/日本" };

        static Encoding Enc(string name) => Encoding.GetEncoding(name);

        /// <summary>
        /// The worker request the request's targets give System.Web: verb, path, query string, its bytes, body, header names and
        /// values. FuzzWorkerRequest; FuzzTests gives the port's own (AspNetCoreWorkerRequest) for a run.
        /// </summary>
        public static Func<string, string, string, byte[], byte[], string[], HttpWorkerRequest> NewWorker =
            (verb, path, query, queryBytes, body, headers) => new FuzzWorkerRequest(verb, path, query, queryBytes, body, headers);

        static HttpWorkerRequest Worker(string verb, string path, string query, byte[] queryBytes, byte[] body, params string[] headers) =>
            NewWorker(verb, path, query, queryBytes, body, headers);

        static List<string> seeds;
        static List<string> Seeds => seeds ?? (seeds = Gen.Seeds());

        static string Any(Rng r, int i) => Gen.String(r, i, rr => Gen.Text(rr, 10), Seeds);

        public static readonly Target[] All =
        {
            new Target
            {
                Name = "UrlEncode", Count = 2400,
                Input = (r, i) => new[] { Any(r, i % 1200), Encodings[i < 1200 ? i % 3 : r.Below(3)] },
                Run = (x, o) =>
                {
                    o.Add("UrlEncode", () => Framework.UrlEncode(x[0], Enc(x[1])));
                    o.Add("UrlEncodeUnicode", () => Framework.UrlEncodeUnicode(x[0]));
                    o.Add("UrlPathEncode", () => Framework.UrlPathEncode(x[0]));
                    o.Add("UrlEncodeNonAscii", () => Web.Call(typeof(System.Web.Util.HttpEncoder), Web.Encoder, "UrlEncodeNonAscii", new[] { typeof(string), typeof(Encoding) }, x[0], Enc(x[1])));
                },
            },
            new Target
            {
                Name = "UrlDecode", Count = 2400,
                Input = (r, i) => new[] { i < Seeds.Count ? Seeds[i] : r.Chance(50) ? Gen.Query(r) : Any(r, int.MaxValue), Encodings[r.Below(3)] },
                Run = (x, o) =>
                {
                    o.Add("UrlDecode", () => Framework.UrlDecode(x[0], Enc(x[1])));
                    o.Add("UrlDecodeBytes", () => Framework.UrlDecode(Enc(x[1]).GetBytes(x[0]), Enc(x[1])));
                    o.Add("UrlDecodeToBytes", () => Framework.UrlDecodeToBytes(Enc(x[1]).GetBytes(x[0])));
                },
            },
            new Target
            {
                Name = "HtmlEncode", Count = 2000,
                Input = (r, i) => new[] { i < Seeds.Count ? Seeds[i] : r.Chance(30) ? Gen.Html(r) : Any(r, int.MaxValue) },
                Run = (x, o) =>
                {
                    o.Add("HtmlEncode", () => Framework.HtmlEncode(x[0]));
                    o.Add("HtmlAttributeEncode", () => Framework.HtmlAttributeEncode(x[0]));
                    o.Add("JavaScriptStringEncode", () => Framework.JavaScriptStringEncode(x[0]));
                },
            },
            new Target
            {
                Name = "HtmlDecode", Count = 2000,
                Input = (r, i) => new[] { i < Seeds.Count ? Seeds[i] : r.Chance(75) ? Gen.Html(r) : Gen.Mutate(r, Gen.Html(r)) },
                Run = (x, o) => o.Add("HtmlDecode", () => Framework.HtmlDecode(x[0])),
            },
            new Target
            {
                Name = "ParseQueryString", Count = 2000,
                Input = (r, i) => new[] { r.Chance(75) ? (r.Chance(10) ? "?" : "") + Gen.Query(r) : Any(r, int.MaxValue), Encodings[r.Below(3)] },
                Run = (x, o) =>
                {
                    NameValueCollection c = null;
                    o.Add("Collection", () => c = Framework.ParseQueryString(x[0], Enc(x[1])));
                    o.Add("ToString", () => c?.ToString());
                },
            },
            new Target
            {
                Name = "UrlToken", Count = 1500,
                Input = (r, i) =>
                {
                    var bytes = new byte[r.Small(40)];
                    for (int k = 0; k < bytes.Length; k++) bytes[k] = (byte)r.Below(256);
                    string token;
                    if (i < Seeds.Count) token = Seeds[i];
                    else
                    {
                        // A token as UrlTokenEncode writes it, then changed; or text.
                        var b64 = Convert.ToBase64String(bytes);
                        int pad = b64.Length - b64.TrimEnd('=').Length;
                        token = r.Chance(70) ? b64.TrimEnd('=').Replace('+', '-').Replace('/', '_') + pad.ToString(CultureInfo.InvariantCulture) : Any(r, int.MaxValue);
                        if (r.Chance(40)) token = Gen.Mutate(r, token);
                    }
                    return new[] { token, Show.Hex(bytes) };
                },
                Run = (x, o) =>
                {
                    o.Add("UrlTokenDecode", () => HttpServerUtility.UrlTokenDecode(x[0]));
                    o.Add("UrlTokenEncode", () => HttpServerUtility.UrlTokenEncode(FromHex(x[1])));
                    o.Add("UrlTokenEncodeText", () => HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(x[0])));
                },
            },
            new Target
            {
                Name = "VirtualPathUtility", Count = 2500,
                Input = (r, i) => new[] { i < Seeds.Count ? Seeds[i] : r.Chance(70) ? Gen.Path(r) : Gen.Mutate(r, Gen.Path(r)), r.Chance(85) ? Gen.Path(r) : Any(r, int.MaxValue), AppPaths[r.Below(AppPaths.Length)] },
                Run = (x, o) => Web.WithAppPath(x[2], () =>
                {
                    o.Add("IsAbsolute", () => VirtualPathUtility.IsAbsolute(x[0]));
                    o.Add("IsAppRelative", () => VirtualPathUtility.IsAppRelative(x[0]));
                    o.Add("ToAppRelative", () => VirtualPathUtility.ToAppRelative(x[0]));
                    o.Add("ToAppRelativeApp", () => VirtualPathUtility.ToAppRelative(x[0], x[2]));
                    o.Add("ToAbsolute", () => VirtualPathUtility.ToAbsolute(x[0]));
                    o.Add("ToAbsoluteApp", () => VirtualPathUtility.ToAbsolute(x[0], x[2]));
                    o.Add("GetDirectory", () => VirtualPathUtility.GetDirectory(x[0]));
                    o.Add("GetExtension", () => VirtualPathUtility.GetExtension(x[0]));
                    o.Add("GetFileName", () => VirtualPathUtility.GetFileName(x[0]));
                    o.Add("AppendTrailingSlash", () => VirtualPathUtility.AppendTrailingSlash(x[0]));
                    o.Add("RemoveTrailingSlash", () => VirtualPathUtility.RemoveTrailingSlash(x[0]));
                    o.Add("Combine", () => VirtualPathUtility.Combine(x[0], x[1]));
                    o.Add("MakeRelative", () => VirtualPathUtility.MakeRelative(x[0], x[1]));
                    return 0;
                }),
            },
            new Target
            {
                Name = "UrlPath", Count = 2500,
                Input = (r, i) => new[] { i < Seeds.Count ? Seeds[i] : r.Chance(70) ? Gen.Path(r) : Gen.Mutate(r, Gen.Path(r)), Gen.Path(r), AppPaths[r.Below(AppPaths.Length)] },
                Run = (x, o) => Web.WithAppPath(x[2], () =>
                {
                    const string U = "System.Web.Util.UrlPath";
                    // The application's path as System.Web passes it to these (HttpRuntime.AppDomainAppVirtualPathString): with its slash.
                    var app = x[2].EndsWith("/", StringComparison.Ordinal) ? x[2] : x[2] + "/";
                    var s = new[] { typeof(string) };
                    var ss = new[] { typeof(string), typeof(string) };
                    o.Add("Reduce", () => Web.Call(U, null, "Reduce", s, x[0]));
                    o.Add("FixVirtualPathSlashes", () => Web.Call(U, null, "FixVirtualPathSlashes", s, x[0]));
                    o.Add("IsValidVirtualPathWithoutProtocol", () => Web.Call(U, null, "IsValidVirtualPathWithoutProtocol", s, x[0]));
                    o.Add("IsRelativeUrl", () => Web.Call(U, null, "IsRelativeUrl", s, x[0]));
                    o.Add("IsRooted", () => Web.Call(U, null, "IsRooted", s, x[0]));
                    o.Add("CheckValidVirtualPath", () => { Web.Call(U, null, "CheckValidVirtualPath", s, x[0]); return "ok"; });
                    o.Add("MakeVirtualPathAppRelative", () => Web.Call(U, null, "MakeVirtualPathAppRelative", new[] { typeof(string), typeof(string), typeof(bool) }, x[0], app, false));
                    o.Add("MakeVirtualPathAppRelativeOrNull", () => Web.Call(U, null, "MakeVirtualPathAppRelative", new[] { typeof(string), typeof(string), typeof(bool) }, x[0], app, true));
                    o.Add("MakeVirtualPathAppAbsolute", () => Web.Call(U, null, "MakeVirtualPathAppAbsolute", ss, x[0], app));
                    o.Add("Combine", () => Web.Call(U, null, "Combine", ss, x[1], x[0]));
                    o.Add("SimpleCombine", () => Web.Call(U, null, "SimpleCombine", ss, x[1], x[0]));
                    o.Add("IsEqualOrSubpath", () => Web.Call(U, null, "IsEqualOrSubpath", ss, x[1], x[0]));
                    o.Add("IsPathOnSameServer", () => Web.Call(U, null, "IsPathOnSameServer", new[] { typeof(string), typeof(Uri) }, x[0], new Uri("http://localhost/app/x.aspx")));
                    o.Add("GetFileName", () => Web.Call(U, null, "GetFileName", s, x[0]));
                    o.Add("GetFileNameWithoutExtension", () => Web.Call(U, null, "GetFileNameWithoutExtension", s, x[0]));
                    var options = Web.Type("System.Web.VirtualPathOptions");
                    object path = null;
                    o.Add("VirtualPath", () => Web.Get(path = Web.Call("System.Web.VirtualPath", null, "Create", new[] { typeof(string), options }, x[0], Enum.ToObject(options, 0x1D)), "VirtualPathString"));
                    o.Add("VirtualPathAppRelative", () => path == null ? null : Web.Get(path, "AppRelativeVirtualPathStringOrNull"));
                    o.Add("VirtualPathExtension", () => path == null ? null : Web.Get(path, "Extension"));
                    o.Add("VirtualPathParent", () => { var parent = path == null ? null : Web.Get(path, "Parent"); return parent == null ? null : Web.Get(parent, "VirtualPathString"); });
                    // VirtualPath's equality (its comparer's, a linguistic one) with the path changed: case, ignorable characters.
                    o.Add("VirtualPathEquals", () =>
                    {
                        if (path == null) return null;
                        var p = x[0];
                        int middle = p.Length / 2;
                        var variants = new[] { p.ToUpperInvariant(), p.Insert(middle, "\u200B"), p.Insert(middle, "\u00AD"), p.Insert(middle, "\u007F"), p.Insert(middle, "\0"), p + "\u0301", p.Replace('a', '\u0430') };
                        return string.Concat(variants.Select(v =>
                        {
                            try { return path.Equals(Web.Call("System.Web.VirtualPath", null, "Create", new[] { typeof(string), options }, v, Enum.ToObject(options, 0x1D))) ? "1" : "0"; }
                            catch (Exception) { return "x"; }
                        }));
                    });
                    o.Add("VirtualPathTrailingSlash", () => Web.Get(Web.Call("System.Web.VirtualPath", null, "Create", new[] { typeof(string), options }, x[0], Enum.ToObject(options, 0x0E)), "VirtualPathString"));
                    return 0;
                }),
            },
            new Target
            {
                Name = "Request", Count = 1500,
                Input = (r, i) =>
                {
                    string path = r.Chance(80) ? Gen.Path(r) : Any(r, int.MaxValue);
                    int q = path.IndexOf('?');
                    if (q >= 0) path = path.Substring(0, q);
                    string header = null;
                    if (r.Chance(60))
                    {
                        // A cookieless URL: /(X(...)...)/ somewhere in it.
                        var cookie = r.Pick(new[] { "(S(abc123))", "(F(tok)S(sid))", "(A(x)F(y)S(z))", "(S())", "(s(x))", "(S(a)", "(S(a))(F(b))", "(SS(a))", "(S(a)x)", "(S(" + Gen.Text(r, 2) + "))", "()", "(S(a/b))" });
                        int at = path.LastIndexOf('/');
                        path = at < 0 ? "/" + cookie + "/" + path : path.Substring(0, at) + "/" + cookie + path.Substring(at);
                        // aspnet_filter's header (AspFilterSessionId) with the values in the path, as IIS gives it.
                        if (r.Chance(50)) header = cookie.Substring(1, cookie.Length - 2);
                    }
                    if (!path.StartsWith("/", StringComparison.Ordinal) && r.Chance(80)) path = "/" + path;
                    return new[] { path, r.Chance(30) ? Gen.Query(r) : "", header ?? (r.Chance(10) ? Gen.Text(r, 3) : "") };
                },
                Run = (x, o) => Web.WithAppPath("/", () =>
                {
                    HttpContext context = null;
                    o.Add("Context", () => { context = new HttpContext(Targets.Worker("GET", x[0], x[1], Encoding.UTF8.GetBytes(x[1]), null, "AspFilterSessionId", x[2].Length > 0 ? x[2] : null)); return "ok"; });
                    if (context == null) return 0;
                    var helper = Web.Get(context, "CookielessHelper");
                    var cookieless = Web.Type("System.Web.Security.CookielessHelperClass");
                    o.Add("Path", () => context.Request.Path);
                    o.Add("FilePath", () => context.Request.FilePath);
                    o.Add("RawUrl", () => context.Request.RawUrl);
                    o.Add("AppRelativeCurrentExecutionFilePath", () => context.Request.AppRelativeCurrentExecutionFilePath);
                    o.Add("CurrentExecutionFilePathExtension", () => context.Request.CurrentExecutionFilePathExtension);
                    foreach (var id in "SFA")
                        o.Add("Cookieless" + id, () => Web.Call(cookieless, helper, "GetCookieValue", new[] { typeof(char) }, id));
                    o.Add("ApplyAppPathModifier", () => context.Response.ApplyAppPathModifier("page.aspx?q=1"));
                    o.Add("ApplyAppPathModifierRooted", () => context.Response.ApplyAppPathModifier("/sub/page.aspx"));
                    o.Add("Url", () => context.Request.Url.AbsoluteUri);
                    return 0;
                }),
            },
            new Target
            {
                Name = "Cookies", Count = 2000,
                Input = (r, i) => new[] { i < Seeds.Count ? Seeds[i] : r.Chance(75) ? Gen.CookieHeader(r) : Gen.Mutate(r, Gen.CookieHeader(r)),
                    r.Chance(85) ? Gen.SetCookie(r) : Gen.Mutate(r, Gen.SetCookie(r)) },
                Run = (x, o) => Web.WithAppPath("/", () =>
                {
                    HttpContext context = null;
                    o.Add("Context", () => { context = new HttpContext(Targets.Worker("GET", "/page.aspx", "", new byte[0], null, "Cookie", x[0])); return "ok"; });
                    if (context == null) return 0;
                    o.Add("Cookies", () =>
                    {
                        var cookies = context.Request.Cookies;
                        var shown = new List<string>();
                        for (int k = 0; k < cookies.Count; k++)
                            shown.Add(cookies.GetKey(k) + "=" + Cookie(cookies[k]));
                        return shown;
                    });
                    o.Add("Get", () => { var c = context.Request.Cookies.Get("a"); return c == null ? null : c.Value; });
                    o.Add("TryParse", () => HttpCookie.TryParse(x[1], out var parsed) ? Cookie(parsed) : "false");
                    o.Add("SetCookieHeader", () =>
                    {
                        context.Response.AppendHeader("Set-Cookie", x[1]);
                        var cookies = context.Response.Cookies;
                        var shown = new List<string>();
                        for (int k = 0; k < cookies.Count; k++)
                            shown.Add(cookies.GetKey(k) + "=" + Cookie(cookies[k]));
                        return shown;
                    });
                    return 0;
                }),
            },
            new Target
            {
                Name = "Form", Count = 2000,
                Input = (r, i) =>
                {
                    var encoding = Encodings[r.Below(3)];
                    var contentType = r.Pick(new[] { "application/x-www-form-urlencoded", "application/x-www-form-urlencoded; charset=" + encoding, "Application/X-WWW-Form-UrlEncoded",
                        "application/x-www-form-urlencoded; charset=\"" + encoding + "\"", "application/x-www-form-urlencoded;charset=bogus", "application/x-www-form-urlencoded; charset=utf-7",
                        "text/plain", "", "application/x-www-form-urlencoded; charset=" + encoding + "; x=y", "application/x-www-form-urlencodedx" });
                    var body = r.Chance(80) ? Gen.Query(r) : Any(r, int.MaxValue);
                    return new[] { contentType, body, r.Chance(50) ? Gen.Query(r) : "", encoding };
                },
                Run = (x, o) => Web.WithAppPath("/", () =>
                {
                    var encoding = Enc(x[3]);
                    HttpContext context = null;
                    o.Add("Context", () => { context = new HttpContext(Targets.Worker("POST", "/page.aspx", x[2], encoding.GetBytes(x[2]), encoding.GetBytes(x[1]), "Content-Type", x[0])); return "ok"; });
                    if (context == null) return 0;
                    o.Add("ContentEncoding", () => context.Request.ContentEncoding.WebName);
                    o.Add("Form", () => context.Request.Form);
                    o.Add("FormToString", () => context.Request.Form.ToString());
                    o.Add("QueryString", () => context.Request.QueryString);
                    o.Add("QueryStringToString", () => context.Request.QueryString.ToString());
                    return 0;
                }),
            },
            new Target
            {
                Name = "Multipart", Count = 1500,
                Input = (r, i) => { var m = Gen.Multipart(r); return new[] { m[0], r.Chance(85) ? m[1] : Gen.Mutate(r, m[1]), Encodings[r.Below(3)] }; },
                Run = (x, o) => Web.WithAppPath("/", () =>
                {
                    var encoding = Enc(x[2]);
                    HttpContext context = null;
                    o.Add("Context", () => { context = new HttpContext(Targets.Worker("POST", "/page.aspx", "", new byte[0], encoding.GetBytes(x[1]), "Content-Type", x[0])); context.Request.ContentEncoding = encoding; return "ok"; });
                    if (context == null) return 0;
                    o.Add("Form", () => context.Request.Form);
                    o.Add("Files", () =>
                    {
                        var files = context.Request.Files;
                        var shown = new List<string>();
                        for (int k = 0; k < files.Count; k++)
                        {
                            var file = files[k];
                            var content = new byte[file.ContentLength];
                            file.InputStream.Read(content, 0, content.Length);
                            shown.Add(files.GetKey(k) + "=" + file.FileName + ";" + file.ContentType + ";" + file.ContentLength + ";" + Show.Hex(content));
                        }
                        return shown;
                    });
                    return 0;
                }),
            },
            new Target
            {
                Name = "RequestValidation", Count = 2500,
                Input = (r, i) => new[] { i < Seeds.Count ? Seeds[i] : r.Chance(40) ? Gen.Html(r) : Any(r, int.MaxValue) },
                Run = (x, o) =>
                {
                    const string V = "System.Web.CrossSiteScriptingValidation";
                    o.Add("IsDangerousString", () =>
                    {
                        var args = new object[] { x[0], 0 };
                        var dangerous = (bool)Web.Call(V, null, "IsDangerousString", new[] { typeof(string), typeof(int).MakeByRefType() }, args);
                        return dangerous + "@" + args[1];
                    });
                    o.Add("IsDangerousUrl", () => Web.Call(V, null, "IsDangerousUrl", new[] { typeof(string) }, x[0]));
                    o.Add("IsValidJavascriptId", () => Web.Call(V, null, "IsValidJavascriptId", new[] { typeof(string) }, x[0]));
                    // The characters' Unicode categories (the identifier check's): the Unicode version's, not System.Web's.
                    o.Add("Categories", () => string.Join(",", x[0].Take(64).Select(c => ((int)CharUnicodeInfo.GetUnicodeCategory(c)).ToString(CultureInfo.InvariantCulture))));
                },
            },
            new Target
            {
                Name = "HeaderEncode", Count = 1500,
                Input = (r, i) => new[] { i < Seeds.Count ? Seeds[i] : Gen.HeaderPart(r), Gen.HeaderPart(r) },
                Run = (x, o) =>
                {
                    o.Add("HeaderNameValueEncode", () =>
                    {
                        var args = new object[] { x[0], x[1], null, null };
                        Web.Call(typeof(System.Web.Util.HttpEncoder), Web.Encoder, "HeaderNameValueEncode", new[] { typeof(string), typeof(string), typeof(string).MakeByRefType(), typeof(string).MakeByRefType() }, args);
                        return new[] { (string)args[2], (string)args[3] };
                    });
                },
            },
            new Target
            {
                Name = "HttpDate", Count = 1500,
                Input = (r, i) =>
                {
                    long ticks = r.Chance(10) ? r.Pick(new[] { 0L, DateTime.MaxValue.Ticks, DateTime.MinValue.Ticks + TimeSpan.TicksPerDay, 630822816000000000L })
                        : (long)(r.Next() % (ulong)DateTime.MaxValue.Ticks);
                    return new[] { r.Chance(85) ? Gen.HttpDate(r) : Gen.Mutate(r, Gen.HttpDate(r)), ticks.ToString(CultureInfo.InvariantCulture) };
                },
                Run = (x, o) =>
                {
                    var date = new DateTime(long.Parse(x[1], CultureInfo.InvariantCulture), DateTimeKind.Utc);
                    o.Add("UtcParse", () => Web.Call("System.Web.Util.HttpDate", null, "UtcParse", new[] { typeof(string) }, x[0]));
                    o.Add("FormatHttpDateTime", () => Web.Call(Framework.UtilityType, null, "FormatHttpDateTime", new[] { typeof(DateTime) }, date));
                    o.Add("FormatHttpCookieDateTime", () => Web.Call(Framework.UtilityType, null, "FormatHttpCookieDateTime", new[] { typeof(DateTime) }, date));
                },
            },
            new Target
            {
                Name = "PhysicalPath", Count = 1500, WindowsOnly = true,
                Input = (r, i) => new[] { r.Chance(85) ? Gen.PhysicalPath(r) : Gen.Mutate(r, Gen.PhysicalPath(r)) },
                Run = (x, o) =>
                {
                    o.Add("IsSuspiciousPhysicalPath", () => Web.Call("System.Web.Util.FileUtil", null, "IsSuspiciousPhysicalPath", new[] { typeof(string) }, x[0]));
                    o.Add("IsSuspiciousPhysicalPathTooLong", () =>
                    {
                        var args = new object[] { x[0], false };
                        var suspicious = Web.Call("System.Web.Util.FileUtil", null, "IsSuspiciousPhysicalPath", new[] { typeof(string), typeof(bool).MakeByRefType() }, args);
                        return suspicious + "/" + args[1];
                    });
                    o.Add("IsAbsolutePhysicalPath", () => Web.Call("System.Web.Util.UrlPath", null, "IsAbsolutePhysicalPath", new[] { typeof(string) }, x[0]));
                    o.Add("IsValidDirectoryName", () => Web.Call("System.Web.Util.FileUtil", null, "IsValidDirectoryName", new[] { typeof(string) }, x[0]));
                    o.Add("RemoveTrailingDirectoryBackSlash", () => Web.Call("System.Web.Util.FileUtil", null, "RemoveTrailingDirectoryBackSlash", new[] { typeof(string) }, x[0]));
                    // A rooted path only (a relative one is the current directory's).
                    o.Add("FixUpPhysicalDirectory", () => !x[0].StartsWith("C:\\", StringComparison.OrdinalIgnoreCase) && !x[0].StartsWith("\\\\", StringComparison.Ordinal) ? "relative"
                        : Web.Call("System.Web.Util.FileUtil", null, "FixUpPhysicalDirectory", new[] { typeof(string) }, x[0]));
                },
            },
        };

        /// <summary>A cookie's name, value, values and attributes.</summary>
        static string Cookie(HttpCookie c) =>
            c.Name + ";" + c.Value + ";" + c.HasKeys + ";" + Show.Collection(c.Values) + ";path=" + c.Path + ";domain=" + c.Domain + ";secure=" + c.Secure +
            ";httponly=" + c.HttpOnly + ";samesite=" + (int)c.SameSite + ";expires=" + Expires(c.Expires);

        // A local time (an Expires with its zone, "GMT", parsed) as UTC: the same whatever the machine's time zone.
        static string Expires(DateTime d) =>
            d.Kind == DateTimeKind.Local ? d.ToUniversalTime().Ticks.ToString(CultureInfo.InvariantCulture) + "/Utc" : d.Ticks.ToString(CultureInfo.InvariantCulture) + "/" + d.Kind;

        static byte[] FromHex(string hex)
        {
            if (hex == "\\N") return null;
            var bytes = new byte[(hex.Length - 2) / 2];
            for (int i = 0; i < bytes.Length; i++) bytes[i] = byte.Parse(hex.Substring(2 + i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            return bytes;
        }
    }
}
