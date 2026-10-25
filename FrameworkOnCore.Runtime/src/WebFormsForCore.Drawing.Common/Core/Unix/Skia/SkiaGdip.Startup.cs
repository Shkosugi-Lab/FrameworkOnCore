// WebFormsForCore: the GDI+ flat API over SkiaSharp (GdipBackend.Skia), its functions written here by area; those not written
// yet are SkiaGdip.Stubs.generated.cs' (experiments/wf4c/generate-gdip-dispatch.ps1, rerun after adding one here).
// The signatures are GdiplusNative's (the libgdiplus imports), so that the generated dispatch calls either.
using System.Runtime.InteropServices;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            // Nothing to start: a token other than zero (Gdip.Initialized).
            internal static int GdiplusStartup(out IntPtr token, ref StartupInput input, out StartupOutput output)
            {
                token = (IntPtr)1;
                output = default;
                return Gdip.Ok;
            }

            internal static void GdiplusShutdown(ref ulong token)
            {
            }

            // libgdiplus' GdipAlloc and GdipFree are malloc and free.
            internal static IntPtr GdipAlloc(int size) => Marshal.AllocHGlobal(size);

            internal static void GdipFree(IntPtr ptr) => Marshal.FreeHGlobal(ptr);
        }
    }
}
