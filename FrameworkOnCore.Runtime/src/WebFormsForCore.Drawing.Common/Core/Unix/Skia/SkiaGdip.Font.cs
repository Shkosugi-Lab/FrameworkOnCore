// WebFormsForCore: GDI+'s fonts over Skia's typefaces (found through fontconfig, as libgdiplus found them): FontFamily,
// Font, the installed and private collections. A family's metrics are its font's tables, read as libgdiplus read them
// (gdip_get_fontfamily_details_from_freetype: OS/2's typographic values when it says to use them, otherwise the larger of
// hhea's and OS/2's Windows values). A family not found is the default font's, as fontconfig gives one.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            internal sealed class GpFontCollection
            {
                public bool Installed;
                public readonly List<SKTypeface> Typefaces = new List<SKTypeface>();

                public string[] FamilyNames()
                {
                    if (Installed)
                        return SKFontManager.Default.FontFamilies.Distinct(StringComparer.Ordinal).OrderBy(n => n, StringComparer.Ordinal).ToArray();
                    return Typefaces.Select(t => t.FamilyName).Distinct(StringComparer.Ordinal).ToArray();
                }
            }

            internal sealed class GpFontFamily
            {
                public string Name;
                public GpFontCollection? Collection;
                private readonly Dictionary<int, SKTypeface> _typefaces = new Dictionary<int, SKTypeface>();
                private Metrics? _metrics;

                public GpFontFamily(string name, GpFontCollection? collection)
                {
                    Name = name;
                    Collection = collection;
                }

                public GpFontFamily Clone() => this;   // immutable: shared (libgdiplus copies the pattern)

                public SKTypeface Typeface(FontStyle style)
                {
                    int key = (int)(style & (FontStyle.Bold | FontStyle.Italic));
                    lock (_typefaces)
                    {
                        if (_typefaces.TryGetValue(key, out SKTypeface? t))
                            return t;
                        var skStyle = new SKFontStyle((style & FontStyle.Bold) != 0 ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
                            SKFontStyleWidth.Normal, (style & FontStyle.Italic) != 0 ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
                        SKTypeface? typeface = null;
                        if (Collection != null && !Collection.Installed)
                        {
                            // A private collection's: the closest of its styles.
                            typeface = Collection.Typefaces.Where(f => f.FamilyName == Name)
                                .OrderBy(f => Math.Abs(f.FontStyle.Weight - skStyle.Weight) + (f.FontStyle.Slant == skStyle.Slant ? 0 : 1000)).FirstOrDefault();
                        }
                        typeface ??= SKFontManager.Default.MatchFamily(Name, skStyle) ?? SKTypeface.FromFamilyName(Name, skStyle) ?? SKTypeface.Default;
                        _typefaces[key] = typeface;
                        return typeface;
                    }
                }

                internal sealed class Metrics
                {
                    public int EmHeight, CellAscent, CellDescent, LineSpacing;
                }

                public Metrics FamilyMetrics
                {
                    get
                    {
                        if (_metrics != null)
                            return _metrics;
                        SKTypeface t = Typeface(FontStyle.Regular);
                        var m = new Metrics { EmHeight = t.UnitsPerEm > 0 ? t.UnitsPerEm : 2048 };
                        byte[]? hhea = Table(t, "hhea"), os2 = Table(t, "OS/2");
                        static short S16(byte[] d, int at) => (short)((d[at] << 8) | d[at + 1]);
                        static ushort U16(byte[] d, int at) => (ushort)((d[at] << 8) | d[at + 1]);
                        if (hhea != null && hhea.Length >= 10)
                        {
                            bool useTypo = os2 != null && os2.Length >= 78 && (U16(os2, 62) & (1 << 7)) != 0;
                            if (useTypo)
                            {
                                m.LineSpacing = S16(os2!, 68) - S16(os2!, 70) + S16(os2!, 72);
                                m.CellDescent = -S16(os2!, 70);
                                m.CellAscent = S16(os2!, 68);
                            }
                            else
                            {
                                int hheaLineSpacing = S16(hhea, 4) + Math.Abs((int)S16(hhea, 6)) + S16(hhea, 8);
                                int os2LineSpacing = os2 != null && os2.Length >= 78 ? U16(os2, 74) + U16(os2, 76) : 0;
                                m.LineSpacing = Math.Max(hheaLineSpacing, os2LineSpacing);
                                m.CellDescent = os2 != null && os2.Length >= 78 ? U16(os2, 76) : S16(hhea, 6);
                                m.CellAscent = os2 != null && os2.Length >= 78 ? U16(os2, 74) : S16(hhea, 4);
                            }
                        }
                        else
                        {
                            using var font = new SKFont(t, m.EmHeight);
                            SKFontMetrics fm = font.Metrics;
                            m.CellAscent = (int)Math.Round(-fm.Ascent);
                            m.CellDescent = (int)Math.Round(fm.Descent);
                            m.LineSpacing = (int)Math.Round(-fm.Ascent + fm.Descent + fm.Leading);
                        }
                        return _metrics = m;
                    }
                }

                private static byte[]? Table(SKTypeface typeface, string tag)
                {
                    uint t = ((uint)tag[0] << 24) | ((uint)tag[1] << 16) | ((uint)tag[2] << 8) | tag[3];
                    return typeface.TryGetTableData(t, out byte[] data) ? data : null;
                }
            }

            internal sealed class GpFont
            {
                public GpFontFamily Family;
                public float EmSize;
                public FontStyle Style;
                public GraphicsUnit Unit;

                public GpFont(GpFontFamily family, float emSize, FontStyle style, GraphicsUnit unit)
                {
                    Family = family;
                    EmSize = emSize;
                    Style = style;
                    Unit = unit;
                }

                public SKTypeface Typeface => Family.Typeface(Style);

                /// <summary>The em size in pixels at the given resolution.</summary>
                public float PixelSize(float dpi) => Unit switch
                {
                    GraphicsUnit.World or GraphicsUnit.Pixel or GraphicsUnit.Display => EmSize,
                    _ => EmSize * UnitToPixels(Unit, dpi),
                };

                /// <summary>The em size in a Graphics' world units.</summary>
                public float WorldSize(GpGraphics g) => Unit == GraphicsUnit.World ? EmSize : PixelSize(g.DpiY) / g.PageScaleY;
            }

            // Collections

            private static GpFontCollection? s_installed;

            internal static int GdipNewInstalledFontCollection(out IntPtr fontCollection)
            {
                s_installed ??= new GpFontCollection { Installed = true };
                fontCollection = s_installedHandle != IntPtr.Zero ? s_installedHandle : (s_installedHandle = NewHandle(s_installed));
                return Ok;
            }

            private static IntPtr s_installedHandle;

            internal static int GdipNewPrivateFontCollection(out IntPtr fontCollection)
            {
                fontCollection = NewHandle(new GpFontCollection());
                return Ok;
            }

            internal static int GdipDeletePrivateFontCollection(ref IntPtr fontCollection)
            {
                if (fontCollection != IntPtr.Zero && fontCollection != s_installedHandle && Is(fontCollection, out GpFontCollection? _))
                    GCHandle.FromIntPtr(fontCollection).Free();
                fontCollection = IntPtr.Zero;
                return Ok;
            }

            internal static int GdipPrivateAddFontFile(HandleRef fontCollection, string filename)
            {
                if (!Is(fontCollection, out GpFontCollection? c) || filename == null)
                    return InvalidParameter;
                if (!File.Exists(filename))
                    return Gdip.FileNotFound;
                using var data = SKData.Create(filename);
                if (data == null)
                    return Gdip.FileNotFound;
                for (int index = 0; ; index++)
                {
                    SKTypeface? t = SKTypeface.FromData(data, index);
                    if (t == null)
                        break;
                    c.Typefaces.Add(t);
                }
                return Ok;
            }

            internal static int GdipPrivateAddMemoryFont(HandleRef fontCollection, IntPtr memory, int length)
            {
                if (!Is(fontCollection, out GpFontCollection? c) || memory == IntPtr.Zero || length <= 0)
                    return InvalidParameter;
                using var data = SKData.CreateCopy(memory, (ulong)length);
                SKTypeface? t = SKTypeface.FromData(data);
                if (t == null)
                    return Gdip.FileNotFound;
                c.Typefaces.Add(t);
                return Ok;
            }

            internal static int GdipGetFontCollectionFamilyCount(HandleRef fontCollection, out int numFound)
            {
                numFound = 0;
                if (!Is(fontCollection, out GpFontCollection? c))
                    return InvalidParameter;
                numFound = c.FamilyNames().Length;
                return Ok;
            }

            internal static int GdipGetFontCollectionFamilyList(HandleRef fontCollection, int numSought, IntPtr[] gpfamilies, out int numFound)
            {
                numFound = 0;
                if (!Is(fontCollection, out GpFontCollection? c) || gpfamilies == null)
                    return InvalidParameter;
                string[] names = c.FamilyNames();
                int i;
                for (i = 0; i < numSought && i < names.Length; i++)
                    gpfamilies[i] = NewHandle(new GpFontFamily(names[i], c));
                numFound = i;
                return Ok;
            }

            // Families

            private static readonly Dictionary<string, string> s_matched = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            /// <summary>The family fontconfig matches for a name (the default font's when none).</summary>
            private static string MatchedFamily(string name)
            {
                lock (s_matched)
                {
                    if (s_matched.TryGetValue(name, out string? matched))
                        return matched;
                    SKTypeface? t = SKFontManager.Default.MatchFamily(name) ?? SKTypeface.FromFamilyName(name) ?? SKTypeface.Default;
                    matched = t.FamilyName;
                    s_matched[name] = matched;
                    return matched;
                }
            }

            internal static int GdipCreateFontFamilyFromName(string name, HandleRef fontCollection, out IntPtr FontFamily)
            {
                FontFamily = IntPtr.Zero;
                if (name == null)
                    return InvalidParameter;
                if (Is(fontCollection, out GpFontCollection? c) && !c.Installed)
                {
                    string? found = c.FamilyNames().FirstOrDefault(n => n == name);
                    if (found == null)
                        return FontFamilyNotFound;
                    FontFamily = NewHandle(new GpFontFamily(found, c));
                    return Ok;
                }
                FontFamily = NewHandle(new GpFontFamily(MatchedFamily(name), s_installed ??= new GpFontCollection { Installed = true }));
                return Ok;
            }

            internal static int GdipCloneFontFamily(IntPtr fontfamily, out IntPtr clonefontfamily)
            {
                clonefontfamily = IntPtr.Zero;
                if (!Is(fontfamily, out GpFontFamily? f))
                    return InvalidParameter;
                clonefontfamily = NewHandle(f.Clone());
                return Ok;
            }

            internal static int GdipDeleteFontFamily(HandleRef fontFamily)
            {
                if (!Is(fontFamily, out GpFontFamily? _))
                    return InvalidParameter;
                GCHandle.FromIntPtr(fontFamily.Handle).Free();
                return Ok;
            }

            internal static int GdipGetFamilyName(HandleRef family, char* name, int language)
            {
                if (!Is(family, out GpFontFamily? f))
                    return InvalidParameter;
                if (name == null)
                    return Ok;
                const int LF_FACESIZE = 32;
                int n = Math.Min(f.Name.Length, LF_FACESIZE - 1);
                for (int i = 0; i < n; i++)
                    name[i] = f.Name[i];
                name[n] = '\0';
                return Ok;
            }

            private static int Generic(string name, out IntPtr fontfamily)
            {
                fontfamily = NewHandle(new GpFontFamily(MatchedFamily(name), s_installed ??= new GpFontCollection { Installed = true }));
                return Ok;
            }

            internal static int GdipGetGenericFontFamilySansSerif(out IntPtr fontfamily) => Generic("MS Sans Serif", out fontfamily);

            internal static int GdipGetGenericFontFamilySerif(out IntPtr fontfamily) => Generic("Serif", out fontfamily);

            internal static int GdipGetGenericFontFamilyMonospace(out IntPtr fontfamily) => Generic("Courier New", out fontfamily);

            internal static int GdipGetEmHeight(HandleRef family, FontStyle style, out int EmHeight)
            {
                EmHeight = 0;
                if (!Is(family, out GpFontFamily? f))
                    return InvalidParameter;
                EmHeight = f.FamilyMetrics.EmHeight;
                return Ok;
            }

            internal static int GdipGetCellAscent(HandleRef family, FontStyle style, out int CellAscent)
            {
                CellAscent = 0;
                if (!Is(family, out GpFontFamily? f))
                    return InvalidParameter;
                CellAscent = f.FamilyMetrics.CellAscent;
                return Ok;
            }

            internal static int GdipGetCellDescent(HandleRef family, FontStyle style, out int CellDescent)
            {
                CellDescent = 0;
                if (!Is(family, out GpFontFamily? f))
                    return InvalidParameter;
                CellDescent = f.FamilyMetrics.CellDescent;
                return Ok;
            }

            internal static int GdipGetLineSpacing(HandleRef family, FontStyle style, out int LineSpaceing)
            {
                LineSpaceing = 0;
                if (!Is(family, out GpFontFamily? f))
                    return InvalidParameter;
                LineSpaceing = f.FamilyMetrics.LineSpacing;
                return Ok;
            }

            internal static int GdipIsStyleAvailable(HandleRef family, FontStyle style, out int isStyleAvailable)
            {
                isStyleAvailable = 0;
                if (!Is(family, out GpFontFamily? _))
                    return InvalidParameter;
                isStyleAvailable = 1;
                return Ok;
            }

            // Fonts

            internal static int GdipCreateFont(HandleRef fontFamily, float emSize, FontStyle style, GraphicsUnit unit, out IntPtr font)
            {
                font = IntPtr.Zero;
                if (!Is(fontFamily, out GpFontFamily? f) || unit == GraphicsUnit.Display || unit < GraphicsUnit.World || unit > GraphicsUnit.Millimeter + 1 || emSize <= 0)
                    return InvalidParameter;
                font = NewHandle(new GpFont(f, emSize, style, unit));
                return Ok;
            }

            internal static int GdipCloneFont(HandleRef font, out IntPtr cloneFont)
            {
                cloneFont = IntPtr.Zero;
                if (!Is(font, out GpFont? f))
                    return InvalidParameter;
                cloneFont = NewHandle(new GpFont(f.Family, f.EmSize, f.Style, f.Unit));
                return Ok;
            }

            internal static int GdipDeleteFont(HandleRef font) => Delete<GpFont>(font.Handle);

            internal static int GdipGetFamily(HandleRef font, out IntPtr family)
            {
                family = IntPtr.Zero;
                if (!Is(font, out GpFont? f))
                    return InvalidParameter;
                family = NewHandle(f.Family.Clone());
                return Ok;
            }

            internal static int GdipGetFontSize(HandleRef font, out float size)
            {
                size = 0;
                if (!Is(font, out GpFont? f))
                    return InvalidParameter;
                size = f.EmSize;
                return Ok;
            }

            internal static int GdipGetFontStyle(HandleRef font, out FontStyle style)
            {
                style = 0;
                if (!Is(font, out GpFont? f))
                    return InvalidParameter;
                style = f.Style;
                return Ok;
            }

            internal static int GdipGetFontUnit(HandleRef font, out GraphicsUnit unit)
            {
                unit = 0;
                if (!Is(font, out GpFont? f))
                    return InvalidParameter;
                unit = f.Unit;
                return Ok;
            }

            internal static int GdipGetFontHeight(HandleRef font, HandleRef graphics, out float size)
            {
                size = 0;
                if (!Is(font, out GpFont? f))
                    return InvalidParameter;
                GpFontFamily.Metrics m = f.Family.FamilyMetrics;
                if (Is(graphics, out GpGraphics? g))
                {
                    // In the Graphics' page units.
                    float px = m.LineSpacing * (f.PixelSize(g.DpiY) / m.EmHeight);
                    size = f.Unit == GraphicsUnit.World ? m.LineSpacing * (f.EmSize / m.EmHeight) : px / g.PageScaleY;
                }
                else
                {
                    size = m.LineSpacing * (f.PixelSize(96) / m.EmHeight);
                }
                return Ok;
            }

            internal static int GdipGetFontHeightGivenDPI(HandleRef font, float dpi, out float size)
            {
                size = 0;
                if (!Is(font, out GpFont? f))
                    return InvalidParameter;
                GpFontFamily.Metrics m = f.Family.FamilyMetrics;
                float h = m.LineSpacing * (f.EmSize / m.EmHeight);
                size = f.Unit switch
                {
                    GraphicsUnit.World or GraphicsUnit.Pixel or GraphicsUnit.Display => h,
                    _ => h * UnitToPixels(f.Unit, dpi),
                };
                return Ok;
            }

            internal static int GdipGetLogFontW(HandleRef font, HandleRef graphics, ref Interop.User32.LOGFONT lf)
            {
                lf.lfCharSet = 0;
                if (!Is(font, out GpFont? f) || !Is(graphics, out GpGraphics? g))
                {
                    lf.lfFaceName.Clear();
                    return InvalidParameter;
                }
                lf.lfHeight = -(int)f.PixelSize(96);
                lf.lfWidth = 0;
                lf.lfEscapement = 0;
                lf.lfOrientation = 0;
                lf.lfWeight = (f.Style & FontStyle.Bold) != 0 ? 700 : 400;
                lf.lfItalic = (byte)((f.Style & FontStyle.Italic) != 0 ? 1 : 0);
                lf.lfUnderline = (byte)((f.Style & FontStyle.Underline) != 0 ? 1 : 0);
                lf.lfStrikeOut = (byte)((f.Style & FontStyle.Strikeout) != 0 ? 1 : 0);
                lf.lfOutPrecision = 0;
                lf.lfClipPrecision = 0;
                lf.lfQuality = g.TextMode switch
                {
                    System.Drawing.Text.TextRenderingHint.SystemDefault => 0,
                    System.Drawing.Text.TextRenderingHint.ClearTypeGridFit => 5,
                    _ => 3,
                };
                lf.lfPitchAndFamily = 0;
                Span<char> face = lf.lfFaceName;
                face.Clear();
                f.Family.Name.AsSpan(0, Math.Min(f.Family.Name.Length, face.Length - 1)).CopyTo(face);
                return Ok;
            }

            private static int FontFromLogfont(ref Interop.User32.LOGFONT lf, out IntPtr font)
            {
                float size = Math.Abs(lf.lfHeight);
                FontStyle style = 0;
                if (lf.lfItalic != 0) style |= FontStyle.Italic;
                if (lf.lfWeight > 400) style |= FontStyle.Bold;
                if (lf.lfUnderline != 0) style |= FontStyle.Underline;
                if (lf.lfStrikeOut != 0) style |= FontStyle.Strikeout;
                Span<char> face = lf.lfFaceName;
                int end = face.IndexOf('\0');
                string name = new string(end < 0 ? face : face.Slice(0, end));
                var family = new GpFontFamily(MatchedFamily(name), s_installed ??= new GpFontCollection { Installed = true });
                font = NewHandle(new GpFont(family, size, style, GraphicsUnit.World));
                return Ok;
            }

            internal static int GdipCreateFontFromLogfont(IntPtr hdc, ref Interop.User32.LOGFONT lf, out IntPtr ptr)
            {
                ptr = IntPtr.Zero;
                if (hdc == IntPtr.Zero)
                    return InvalidParameter;
                return FontFromLogfont(ref lf, out ptr);
            }

            internal static int GdipCreateFontFromHfont(IntPtr hdc, out IntPtr font, ref Interop.User32.LOGFONT lf)
            {
                // The HFONT is a font of its own (Font.ToHfont, as libgdiplus): a copy of it, as Windows makes one.
                font = IntPtr.Zero;
                if (!Is(hdc, out GpFont? f))
                    return InvalidParameter;
                font = NewHandle(new GpFont(f.Family, f.EmSize, f.Style, f.Unit));
                lf.lfCharSet = 1;
                return Ok;
            }

            internal static int GdipCreateFontFromDC(IntPtr hdc, ref IntPtr font)
            {
                if (hdc == IntPtr.Zero)
                    return InvalidParameter;
                return NotImplemented;
            }
        }
    }
}
