// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if TARGET_UNIX
using System.Runtime.InteropServices;
using System.Text;

namespace System.Drawing
{
    // WebFormsForCore: the LOGFONT an application declares itself, as Windows code does
    // ([StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)] class LOGFONT { ... ByValTStr 32 ... }), passed to
    // Font.FromLogFont / ToLogFont. CharSet.Auto is Unicode on Windows (the 92 bytes of the LOGFONT the port has), ANSI on
    // Unix: 60 bytes, the face name in UTF-8. Those were refused (a LOGFONT of another size) or failed a cast: they are
    // taken in both layouts.
    public sealed partial class Font
    {
        // The numbers (five ints, eight bytes), then the face name: 32 chars (Unicode) or 32 bytes (ANSI).
        private const int LogFontNumbersSize = 28;
        private const int LogFontFaceSize = 32;
        private const int AnsiLogFontSize = LogFontNumbersSize + LogFontFaceSize;

        private static bool IsAnsiLogFont(Type type) => Marshal.SizeOf(type) == AnsiLogFontSize;

        // The application's LOGFONT (an ANSI one) as the port's.
        private static unsafe Interop.User32.LOGFONT FromAnsiLogFont(object lf)
        {
            byte* ansi = stackalloc byte[AnsiLogFontSize];
            Marshal.StructureToPtr(lf, new IntPtr(ansi), fDeleteOld: false);
            Interop.User32.LOGFONT logFont = default;
            Buffer.MemoryCopy(ansi, &logFont, LogFontNumbersSize, LogFontNumbersSize);
            var face = new ReadOnlySpan<byte>(ansi + LogFontNumbersSize, LogFontFaceSize);
            int end = face.IndexOf((byte)0);
            string name = Encoding.UTF8.GetString(end < 0 ? face : face.Slice(0, end));
            Span<char> faceName = logFont.lfFaceName;
            name.AsSpan(0, Math.Min(name.Length, LogFontFaceSize - 1)).CopyTo(faceName);
            return logFont;
        }

        // The port's LOGFONT into the application's (an ANSI one).
        private static unsafe void ToAnsiLogFont(Interop.User32.LOGFONT logFont, object target)
        {
            byte* ansi = stackalloc byte[AnsiLogFontSize];
            new Span<byte>(ansi, AnsiLogFontSize).Clear();
            Buffer.MemoryCopy(&logFont, ansi, LogFontNumbersSize, LogFontNumbersSize);
            ReadOnlySpan<char> faceName = logFont.lfFaceName;
            int end = faceName.IndexOf('\0');
            if (end >= 0)
                faceName = faceName.Slice(0, end);
            var face = new Span<byte>(ansi + LogFontNumbersSize, LogFontFaceSize - 1);
            // Whole characters only: as many as fit before the terminating zero.
            while (Encoding.UTF8.GetByteCount(faceName) > face.Length)
                faceName = faceName.Slice(0, faceName.Length - 1);
            Encoding.UTF8.GetBytes(faceName, face);
            if (!target.GetType().IsValueType)
            {
                Marshal.PtrToStructure(new IntPtr(ansi), target);
            }
            else
            {
                GCHandle handle = GCHandle.Alloc(target, GCHandleType.Pinned);
                Buffer.MemoryCopy(ansi, (byte*)handle.AddrOfPinnedObject(), AnsiLogFontSize, AnsiLogFontSize);
                handle.Free();
            }
        }
    }
}
#endif
