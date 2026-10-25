// WebFormsForCore: which implementation of the GDI+ flat API the Unix build calls (GdiplusNative.generated.cs).
//   libgdiplus (the default): Mono's, as .NET 6 had it; archived since 2024.
//   SkiaSharp (Core\Unix\Skia, SkiaGdip): the one replacing it, function by function. Chosen with the environment variable
//     FOC_DRAWING_BACKEND=skia or the AppContext switch FrameworkOnCore.Drawing.UseSkia (runtimeconfig.json). What it does
//     not have yet returns NotImplemented (NotImplementedException).
// One for the process, never both: their handles (Graphics, Bitmap, Pen...) are each its own, a handle of one passed to the
// other would be a wrong pointer.
// FOC_GDIP_TRACE=<file>: each process appends to the file, on exit, its backend ("#backend", tab, the name) and the
// functions it called, with how many times (name, tab, count); to see what an application uses.
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace System.Drawing
{
    internal static class GdipBackend
    {
        internal static readonly bool Skia = UseSkia();
        internal static readonly bool Tracing = Environment.GetEnvironmentVariable("FOC_GDIP_TRACE") is { Length: > 0 };

        private static ConcurrentDictionary<string, int>? s_calls;

        private static bool UseSkia()
        {
            string? backend = Environment.GetEnvironmentVariable("FOC_DRAWING_BACKEND");
            if (!string.IsNullOrEmpty(backend))
                return string.Equals(backend, "skia", StringComparison.OrdinalIgnoreCase);
            return AppContext.TryGetSwitch("FrameworkOnCore.Drawing.UseSkia", out bool skia) && skia;
        }

        internal static void Called(string function)
        {
            if (s_calls == null)
            {
                if (Interlocked.CompareExchange(ref s_calls, new ConcurrentDictionary<string, int>(StringComparer.Ordinal), null) == null)
                    AppDomain.CurrentDomain.ProcessExit += (_, _) => WriteTrace();
            }
            s_calls!.AddOrUpdate(function, 1, (_, n) => n + 1);
        }

        private static void WriteTrace()
        {
            try
            {
                string path = Environment.GetEnvironmentVariable("FOC_GDIP_TRACE")!;
                File.AppendAllLines(path, new[] { "#backend\t" + (Skia ? "skia" : "libgdiplus") }
                    .Concat(s_calls!.OrderBy(c => c.Key, StringComparer.Ordinal).Select(c => c.Key + "\t" + c.Value)));
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
