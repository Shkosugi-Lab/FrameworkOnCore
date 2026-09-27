using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.Security.Principal
{
    /// <summary>
    /// .NET Framework's WindowsImpersonationContext, which .NET removed with WindowsIdentity.Impersonate (it has
    /// WindowsIdentity.RunImpersonated). Code that impersonates the identity it kept from a request on a timer's thread
    /// (YAF's IntermittentBackgroundTask, log4net's WindowsSecurityContext) was stubbed without it, and the timer's
    /// exception ended the process. On Windows the thread impersonates the identity until Undo, which puts back the
    /// thread's token before (none: the process's); elsewhere there is no Windows identity to impersonate.
    /// </summary>
    public class WindowsImpersonationContext : IDisposable
    {
        readonly IntPtr previous;
        bool undone;

        internal WindowsImpersonationContext(WindowsIdentity identity)
        {
            if (!OperatingSystem.IsWindows()) { undone = true; return; }
            previous = Native.CurrentThreadToken();
            if (!Native.ImpersonateLoggedOnUser(identity.AccessToken.DangerousGetHandle()))
            {
                Native.CloseToken(previous);
                throw new SecurityException("Impersonation failed: " + Marshal.GetLastPInvokeError());
            }
        }

        public void Undo()
        {
            if (undone || !OperatingSystem.IsWindows()) return;
            undone = true;
            if (previous != IntPtr.Zero)
            {
                Native.SetThreadToken(IntPtr.Zero, previous);
                Native.CloseToken(previous);
            }
            else
            {
                Native.RevertToSelf();
            }
        }

        public void Dispose()
        {
            Undo();
            GC.SuppressFinalize(this);
        }

        [SupportedOSPlatform("windows")]
        static class Native
        {
            const uint TokenImpersonate = 0x0004, TokenQuery = 0x0008, TokenDuplicate = 0x0002;

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool ImpersonateLoggedOnUser(IntPtr token);

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool RevertToSelf();

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool SetThreadToken(IntPtr thread, IntPtr token);

            [DllImport("advapi32.dll", SetLastError = true)]
            static extern bool OpenThreadToken(IntPtr thread, uint access, bool openAsSelf, out IntPtr token);

            [DllImport("kernel32.dll")]
            static extern IntPtr GetCurrentThread();

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool CloseHandle(IntPtr handle);

            // The token the thread impersonates now, if any.
            internal static IntPtr CurrentThreadToken() =>
                OpenThreadToken(GetCurrentThread(), TokenImpersonate | TokenQuery | TokenDuplicate, true, out var token) ? token : IntPtr.Zero;

            internal static void CloseToken(IntPtr token)
            {
                if (token != IntPtr.Zero) CloseHandle(token);
            }
        }
    }
}
