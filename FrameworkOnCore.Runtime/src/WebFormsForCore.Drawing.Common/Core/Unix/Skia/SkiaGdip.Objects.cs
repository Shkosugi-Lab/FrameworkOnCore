// WebFormsForCore: SkiaGdip's objects. A GDI+ handle (Graphics, Bitmap, Pen, Matrix...) is a GCHandle of the object here;
// deleting it frees the GCHandle (and disposes what the object holds of Skia).
// Where libgdiplus computes something itself (paths, matrices, layout rules), the port does as libgdiplus did (mono/libgdiplus,
// MIT, Novell): the results the Linux build gave before stay the same. What libgdiplus left to Cairo, Skia does.
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            private const int Ok = Gdip.Ok;
            private const int GenericError = Gdip.GenericError;
            private const int InvalidParameter = Gdip.InvalidParameter;
            private const int OutOfMemory = Gdip.OutOfMemory;
            private const int ObjectBusy = Gdip.ObjectBusy;
            private const int InsufficientBuffer = Gdip.InsufficientBuffer;
            private const int NotImplemented = Gdip.NotImplemented;
            private const int WrongState = Gdip.WrongState;
            private const int FontFamilyNotFound = Gdip.FontFamilyNotFound;
            private const int UnknownImageFormat = Gdip.UnknownImageFormat;

            internal static IntPtr NewHandle(object value) => GCHandle.ToIntPtr(GCHandle.Alloc(value));

            /// <summary>The object of a handle, if it is one of the type.</summary>
            internal static bool Is<T>(IntPtr handle, [NotNullWhen(true)] out T? value) where T : class
            {
                value = handle == IntPtr.Zero ? null : GCHandle.FromIntPtr(handle).Target as T;
                return value != null;
            }

            internal static bool Is<T>(HandleRef handle, [NotNullWhen(true)] out T? value) where T : class => Is(handle.Handle, out value);

            internal static T? Get<T>(IntPtr handle) where T : class => Is(handle, out T? value) ? value : null;

            internal static int Delete<T>(IntPtr handle) where T : class
            {
                if (!Is(handle, out T? value))
                    return InvalidParameter;
                (value as IDisposable)?.Dispose();
                GCHandle.FromIntPtr(handle).Free();
                return Ok;
            }

            // libgdiplus' general-private.h
            internal static bool NearZero(double value) => value >= -0.00059604645f && value <= 0.00059604645f;

            internal static bool NearOne(double value) => NearZero(value - 1.0f);

            internal static int IRound(float d)
            {
                double intPart = Math.Floor(d);
                return (int)((d - intPart) >= 0.5 ? intPart + 1.0 : intPart);
            }

            internal const double PI = 3.14159265358979323846;
            internal const double DegToRad = PI / 180.0;
        }
    }
}
