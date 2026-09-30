using System.Runtime.Versioning;
using System.Security.Principal;

namespace FrameworkOnCore
{
    /// <summary>
    /// What .NET has on Windows only, as it is on the platform the application runs on. The converter rewrites the
    /// expressions (rules/packages.json platformReplacements); on Windows each is what the source had.
    /// </summary>
    public static class Platform
    {
        /// <summary>
        /// PrincipalPolicy.WindowsPrincipal (DNN's Scheduler: AppDomain.SetPrincipalPolicy): the Windows account's
        /// principal for threads with none. Elsewhere Thread.CurrentPrincipal throws with it, on every request;
        /// there is no Windows account, so the thread's principal is an unauthenticated one.
        /// </summary>
        public static PrincipalPolicy WindowsPrincipalPolicy =>
            System.OperatingSystem.IsWindows() ? PrincipalPolicy.WindowsPrincipal : PrincipalPolicy.UnauthenticatedPrincipal;

        /// <summary>WindowsIdentity.GetCurrent().Name: the account the application runs as (the user elsewhere).</summary>
        public static string CurrentIdentityName => System.OperatingSystem.IsWindows() ? WindowsName() : System.Environment.UserName;

        [SupportedOSPlatform("windows")]
        static string WindowsName() => WindowsIdentity.GetCurrent().Name;

        /// <summary>
        /// WindowsIdentity.GetCurrent(): the Windows account the thread runs as; elsewhere there is none (null), where .NET
        /// throws PlatformNotSupportedException (YAF kept it to impersonate on its timers, when there was one).
        /// </summary>
        public static WindowsIdentity CurrentWindowsIdentity => System.OperatingSystem.IsWindows() ? WindowsCurrent() : null;

        [SupportedOSPlatform("windows")]
        static WindowsIdentity WindowsCurrent() => WindowsIdentity.GetCurrent();

        /// <summary>
        /// AppDomain.CurrentDomain.RelativeSearchPath: ASP.NET's application domain had its BaseDirectory in the
        /// application's folder and searched "bin" under it. WebFormsForCore gives the BaseDirectory as ASP.NET did; .NET's
        /// RelativeSearchPath is always null, and the application looked for its assemblies in its folder (YAF's
        /// ModuleScanner: no data provider found). On every platform.
        /// </summary>
        public static string RelativeSearchPath =>
            System.AppDomain.CurrentDomain.RelativeSearchPath ??
            (System.IO.Directory.Exists(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin")) ? "bin" : null);

        /// <summary>
        /// AppDomain.CurrentDomain.DynamicDirectory: ASP.NET's folder for what it compiles and loads (Temporary ASP.NET
        /// Files\...), in the application domain's probing path. .NET's is always null. The same folder in WebFormsForCore,
        /// HttpRuntime.CodegenDir, which its assembly resolution probes: an assembly copied there is loaded by name as it
        /// was on ASP.NET (nopCommerce's PluginManager copies its plugins there in full trust). Without WebFormsForCore
        /// (a library run elsewhere), a folder of the application in the temporary folder.
        /// </summary>
        public static string DynamicDirectory
        {
            get
            {
                if (System.AppDomain.CurrentDomain.DynamicDirectory is { } own) return own;
                var codegen = System.Type.GetType("System.Web.HttpRuntime, System.Web")?.GetProperty("CodegenDir")?.GetValue(null) as string;
                if (string.IsNullOrEmpty(codegen))
                {
                    var hash = System.Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(System.AppDomain.CurrentDomain.BaseDirectory)))[..16];
                    codegen = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "foc-dynamic", hash);
                }
                System.IO.Directory.CreateDirectory(codegen);
                return codegen;
            }
        }

        /// <summary>
        /// Encoding.Default: .NET Framework's is the system's ANSI code page (Shift_JIS, 932, on Japanese Windows), .NET's
        /// UTF-8 on every platform (the files the application wrote, the hashes of the bytes it took, would differ). As
        /// .NET Framework had it: Windows' ANSI code page; elsewhere the one of the culture the application runs in (LANG,
        /// which deploy/start.sh sets to the original server's culture).
        /// </summary>
        public static System.Text.Encoding DefaultEncoding => defaultEncoding.Value;

        static readonly System.Lazy<System.Text.Encoding> defaultEncoding = new(() =>
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var codePage = System.OperatingSystem.IsWindows() ? (int)GetACP() : System.Globalization.CultureInfo.InstalledUICulture.TextInfo.ANSICodePage;
            return System.Text.Encoding.GetEncoding(codePage);
        });

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern uint GetACP();

        /// <summary>
        /// Thread.ResetAbort(): .NET has no thread abort (it throws PlatformNotSupportedException). WebFormsForCore's
        /// Response.End (Redirect, Transfer) throws a ThreadAbortException, and throws it again at the request's next
        /// steps, as .NET Framework raised the abort again at the end of each catch; the application's catch (DNN's URL
        /// rewriter: after a redirect) called ResetAbort to go on. Here it cancels that (the fork's
        /// HttpResponse.ResetThreadAbort, found at run time: this assembly does not reference the fork). Without a request
        /// or an end pending, nothing.
        /// </summary>
        public static void ResetAbort()
        {
            if (resetAbort.Value is not { } members) return;
            var context = members.Current.GetValue(null);
            if (context == null) return;
            object response;
            // HttpContext.Response throws where the request has none (the application's start).
            try { response = members.Response.GetValue(context); }
            catch (System.Reflection.TargetInvocationException) { return; }
            if (response != null) members.Reset.Invoke(response, null);
        }

        static readonly System.Lazy<(System.Reflection.PropertyInfo Current, System.Reflection.PropertyInfo Response, System.Reflection.MethodInfo Reset)?> resetAbort = new(() =>
        {
            const System.Reflection.BindingFlags any = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
            var context = System.Type.GetType("System.Web.HttpContext, System.Web");
            var current = context?.GetProperty("Current", any | System.Reflection.BindingFlags.Static);
            var response = context?.GetProperty("Response", any | System.Reflection.BindingFlags.Instance);
            var reset = response?.PropertyType.GetMethod("ResetThreadAbort", any | System.Reflection.BindingFlags.Instance, System.Type.EmptyTypes);
            return current != null && response != null && reset != null ? (current, response, reset) : null;
        });
    }
}
