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
    }
}
