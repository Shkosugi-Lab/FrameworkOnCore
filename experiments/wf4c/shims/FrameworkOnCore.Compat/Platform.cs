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
    }
}
