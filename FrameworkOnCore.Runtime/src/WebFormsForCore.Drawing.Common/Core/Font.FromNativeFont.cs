// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if TARGET_UNIX
using System.Runtime.InteropServices;
using Gdip = System.Drawing.SafeNativeMethods.Gdip;

namespace System.Drawing
{
    // WebFormsForCore: a Font made from a native one (FromLogFont) is what the native one is, read from it, as on Windows
    // (Font.Windows.cs). The Unix implementation named every such font "Microsoft Sans Serif", 10, whatever the LOGFONT
    // asked for (its Name, Size, Style, Unit: what the application reads and what Clone and serialization keep).
    public sealed partial class Font
    {
        private Font(IntPtr nativeFont, byte gdiCharSet, bool gdiVerticalFont)
        {
            _nativeFont = nativeFont;

            Gdip.CheckStatus(Gdip.GdipGetFontUnit(new HandleRef(this, nativeFont), out GraphicsUnit unit));
            Gdip.CheckStatus(Gdip.GdipGetFontSize(new HandleRef(this, nativeFont), out float size));
            Gdip.CheckStatus(Gdip.GdipGetFontStyle(new HandleRef(this, nativeFont), out FontStyle style));
            Gdip.CheckStatus(Gdip.GdipGetFamily(new HandleRef(this, nativeFont), out IntPtr nativeFamily));

            Initialize(new FontFamily(nativeFamily), size, style, unit, gdiCharSet, gdiVerticalFont);
        }
    }
}
#endif
