// WebFormsForCore: GDI+'s images. A Bitmap keeps its pixels as GDI+ lays them out (its pixel format, its stride, Scan0 the
// caller's if it gave one); drawing on it is Skia's, on a premultiplied BGRA copy (the pixels themselves for 32bppPARGB and
// 32bppRGB), the two kept in step. Images are read with Skia's codecs and written with Skia's (PNG, JPEG) or the port's
// (BMP). A metafile is recorded as Skia's picture and played as one (a recorded EMF is saved as a drawn image).
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            private const int ImageTypeBitmap = 1, ImageTypeMetafile = 2;

            internal static readonly Guid MemoryBmpGuid = new Guid("b96b3caa-0728-11d3-9d7b-0000f81ef32e");
            internal static readonly Guid BmpGuid = new Guid("b96b3cab-0728-11d3-9d7b-0000f81ef32e");
            internal static readonly Guid EmfGuid = new Guid("b96b3cac-0728-11d3-9d7b-0000f81ef32e");
            internal static readonly Guid WmfGuid = new Guid("b96b3cad-0728-11d3-9d7b-0000f81ef32e");
            internal static readonly Guid JpegGuid = new Guid("b96b3cae-0728-11d3-9d7b-0000f81ef32e");
            internal static readonly Guid PngGuid = new Guid("b96b3caf-0728-11d3-9d7b-0000f81ef32e");
            internal static readonly Guid GifGuid = new Guid("b96b3cb0-0728-11d3-9d7b-0000f81ef32e");
            internal static readonly Guid TiffGuid = new Guid("b96b3cb1-0728-11d3-9d7b-0000f81ef32e");
            internal static readonly Guid IconGuid = new Guid("b96b3cb5-0728-11d3-9d7b-0000f81ef32e");
            internal static readonly Guid FrameDimensionPage = new Guid("7462dc86-6180-4c7e-8e3f-ee7333a7a483");

            internal abstract class GpImage : IDisposable
            {
                public abstract int ImageType { get; }
                public Guid RawFormat = MemoryBmpGuid;
                public float DpiX = 96, DpiY = 96;
                public int Flags;
                public List<Property> Properties = new List<Property>();
                public MultiFrameSave? Saving;
                public abstract int Width { get; }
                public abstract int Height { get; }
                public abstract GpImage Clone();
                public virtual void Dispose() { }
            }

            internal sealed class GpMetafile : GpImage
            {
                public MetaHeader Header = new MetaHeader();
                public byte[]? Data;            // a metafile read: its bytes (not played)
                public bool Recording;          // a Graphics may record into it (once)
                public SKPictureRecorder? Recorder;
                public SKPicture? Picture;      // what was recorded, in pixels

                public override int ImageType => ImageTypeMetafile;
                public override int Width => Header.Width;
                public override int Height => Header.Height;

                /// <summary>The recorded picture's size in pixels (the frame's, in its unit, at 96 dpi).</summary>
                public SizeF PictureSize => new SizeF(Math.Max(Header.Width, 1), Math.Max(Header.Height, 1));

                public SKCanvas BeginRecording()
                {
                    Recording = false;
                    Recorder = new SKPictureRecorder();
                    return Recorder.BeginRecording(new SKRect(0, 0, PictureSize.Width, PictureSize.Height));
                }

                public void EndRecording()
                {
                    if (Recorder != null)
                    {
                        Picture?.Dispose();
                        Picture = Recorder.EndRecording();
                        Recorder.Dispose();
                        Recorder = null;
                    }
                }

                public override GpImage Clone()
                {
                    EndRecording();
                    return new GpMetafile { Header = Header.Clone(), Data = Data, Picture = Picture, RawFormat = RawFormat, DpiX = DpiX, DpiY = DpiY, Flags = Flags, Properties = new List<Property>(Properties) };
                }

                public override void Dispose()
                {
                    Recorder?.Dispose();
                    Picture?.Dispose();
                }
            }

            internal sealed class GpBitmap : GpImage
            {
                public int W, H;
                public PixelFormat Format;
                public int Stride;
                public IntPtr Scan0;
                public bool OwnsScan0;
                public int[]? Palette;
                public int PaletteFlags;
                public bool Locked;
                public Frames? FrameSet;
                // The drawing surface: premultiplied BGRA, the pixels themselves when Aliased.
                public SKBitmap? Surface;
                public bool Aliased;
                public bool SurfaceDirty;   // drawn on, Scan0 not updated yet
                public bool Scan0Dirty;     // Scan0 changed, the surface not updated yet
                public int GraphicsCount;

                public override int ImageType => ImageTypeBitmap;
                public override int Width => W;
                public override int Height => H;

                public static int BitsPerPixel(PixelFormat format) => ((int)format >> 8) & 0xff;

                public static int StrideOf(PixelFormat format, int width) => (int)(((long)BitsPerPixel(format) * width + 31) / 32 * 4);

                public static bool IsIndexed(PixelFormat format) => (format & PixelFormat.Indexed) != 0;

                public static bool HasAlpha(PixelFormat format) => (format & PixelFormat.Alpha) != 0;

                public static bool Supported(PixelFormat format) => format switch
                {
                    PixelFormat.Format1bppIndexed or PixelFormat.Format4bppIndexed or PixelFormat.Format8bppIndexed => true,
                    PixelFormat.Format16bppRgb555 or PixelFormat.Format16bppRgb565 or PixelFormat.Format16bppArgb1555 or PixelFormat.Format16bppGrayScale => true,
                    PixelFormat.Format24bppRgb or PixelFormat.Format32bppRgb or PixelFormat.Format32bppArgb or PixelFormat.Format32bppPArgb => true,
                    PixelFormat.Format48bppRgb or PixelFormat.Format64bppArgb or PixelFormat.Format64bppPArgb => true,
                    _ => false,
                };

                public static GpBitmap Create(int width, int height, PixelFormat format, int stride = 0, IntPtr scan0 = default)
                {
                    var b = new GpBitmap { W = width, H = height, Format = format };
                    b.Stride = stride != 0 ? stride : StrideOf(format, width);
                    if (scan0 != IntPtr.Zero)
                    {
                        b.Scan0 = scan0;
                    }
                    else
                    {
                        long size = (long)b.Stride * height;
                        b.Scan0 = (IntPtr)NativeMemory.AllocZeroed((nuint)Math.Max(size, 1));
                        b.OwnsScan0 = true;
                        if (format == PixelFormat.Format32bppRgb)
                        {
                            // Black, opaque (the alpha byte is the surface's).
                            for (int y = 0; y < height; y++)
                                new Span<uint>((byte*)b.Scan0 + (long)y * b.Stride, width).Fill(0xFF000000);
                        }
                    }
                    if (HasAlpha(format))
                        b.Flags |= (int)ImageFlags.HasAlpha;
                    if (IsIndexed(format))
                        b.Palette = DefaultPalette(format, out b.PaletteFlags);
                    return b;
                }

                public override void Dispose()
                {
                    Surface?.Dispose();
                    Surface = null;
                    if (OwnsScan0 && Scan0 != IntPtr.Zero)
                        NativeMemory.Free((void*)Scan0);
                    Scan0 = IntPtr.Zero;
                    if (FrameSet != null)
                        foreach (GpBitmap frame in FrameSet.List)
                            frame.Dispose();
                    FrameSet = null;
                }

                private GpBitmap CloneFrame()
                {
                    EnsureScan0();
                    GpBitmap c = Create(W, H, Format);
                    for (int y = 0; y < H; y++)
                        Buffer.MemoryCopy((byte*)Scan0 + (long)y * Stride, (byte*)c.Scan0 + (long)y * c.Stride, c.Stride, Math.Min(Stride, c.Stride));
                    c.Palette = (int[]?)Palette?.Clone();
                    c.PaletteFlags = PaletteFlags;
                    return c;
                }

                public override GpImage Clone()
                {
                    GpBitmap c = CloneFrame();
                    if (FrameSet != null)
                    {
                        c.FrameSet = new Frames { Dimension = FrameSet.Dimension, Active = FrameSet.Active };
                        for (int i = 0; i < FrameSet.List.Count; i++)
                            c.FrameSet.List.Add(i == FrameSet.Active ? new GpBitmap() : FrameSet.List[i].CloneFrame());
                    }
                    c.Properties = new List<Property>(Properties);
                    c.RawFormat = RawFormat;
                    c.DpiX = DpiX;
                    c.DpiY = DpiY;
                    c.Flags = Flags;
                    return c;
                }

                public bool CanDraw => !IsIndexed(Format);

                /// <summary>The surface Skia draws on, current.</summary>
                public SKBitmap EnsureSurface()
                {
                    if (Surface == null)
                    {
                        if (Format is PixelFormat.Format32bppPArgb or PixelFormat.Format32bppRgb && Stride > 0)
                        {
                            Surface = new SKBitmap();
                            Surface.InstallPixels(new SKImageInfo(W, H, SKColorType.Bgra8888, SKAlphaType.Premul), Scan0, Stride);
                            Aliased = true;
                        }
                        else
                        {
                            Surface = new SKBitmap(new SKImageInfo(W, H, SKColorType.Bgra8888, SKAlphaType.Premul));
                            Aliased = false;
                            Scan0Dirty = true;
                        }
                    }
                    if (Scan0Dirty && !Aliased)
                    {
                        uint* dst = (uint*)Surface.GetPixels();
                        int dstStride = Surface.RowBytes / 4;
                        for (int y = 0; y < H; y++)
                            for (int x = 0; x < W; x++)
                                dst[(long)y * dstStride + x] = Premultiply(GetArgb(x, y));
                    }
                    Scan0Dirty = false;
                    return Surface;
                }

                public void Drawn()
                {
                    if (!Aliased)
                        SurfaceDirty = true;
                }

                /// <summary>Scan0 current (what was drawn on the surface written back).</summary>
                public void EnsureScan0()
                {
                    if (!SurfaceDirty || Surface == null)
                        return;
                    SurfaceDirty = false;
                    uint* src = (uint*)Surface.GetPixels();
                    int srcStride = Surface.RowBytes / 4;
                    for (int y = 0; y < H; y++)
                        for (int x = 0; x < W; x++)
                            SetArgb(x, y, (int)Unpremultiply(src[(long)y * srcStride + x]));
                }

                public void Scan0Changed()
                {
                    if (Surface != null && !Aliased)
                        Scan0Dirty = true;
                }

                /// <summary>An image of the pixels for drawing it elsewhere.</summary>
                public SKImage ToSKImage()
                {
                    if (CanDraw)
                    {
                        SKBitmap surface = EnsureSurface();
                        return SKImage.FromPixelCopy(surface.Info, surface.GetPixels(), surface.RowBytes);
                    }
                    var info = new SKImageInfo(W, H, SKColorType.Bgra8888, SKAlphaType.Premul);
                    var pixels = new uint[W * H];
                    for (int y = 0; y < H; y++)
                        for (int x = 0; x < W; x++)
                            pixels[y * W + x] = Premultiply(GetArgb(x, y));
                    fixed (uint* p = pixels)
                        return SKImage.FromPixelCopy(info, (IntPtr)p, W * 4);
                }

                /// <summary>The pixels unpremultiplied, for an encoder.</summary>
                public SKBitmap ToUnpremultiplied(bool opaque)
                {
                    EnsureScan0();
                    var bitmap = new SKBitmap(new SKImageInfo(W, H, SKColorType.Bgra8888, opaque ? SKAlphaType.Opaque : SKAlphaType.Unpremul));
                    uint* dst = (uint*)bitmap.GetPixels();
                    int dstStride = bitmap.RowBytes / 4;
                    for (int y = 0; y < H; y++)
                        for (int x = 0; x < W; x++)
                        {
                            uint argb = (uint)GetArgb(x, y);
                            dst[(long)y * dstStride + x] = opaque ? argb | 0xFF000000 : argb;
                        }
                    return bitmap;
                }

                private byte* Row(int y) => (byte*)Scan0 + (long)y * Stride;

                /// <summary>A pixel of Scan0 as GDI+'s ARGB (not premultiplied).</summary>
                public int GetArgb(int x, int y)
                {
                    byte* row = Row(y);
                    switch (Format)
                    {
                        case PixelFormat.Format32bppArgb:
                            return ((int*)row)[x];
                        case PixelFormat.Format32bppPArgb:
                            return (int)Unpremultiply(((uint*)row)[x]);
                        case PixelFormat.Format32bppRgb:
                            return ((int*)row)[x] | unchecked((int)0xFF000000);
                        case PixelFormat.Format24bppRgb:
                            {
                                byte* p = row + x * 3;
                                return unchecked((int)0xFF000000) | (p[2] << 16) | (p[1] << 8) | p[0];
                            }
                        case PixelFormat.Format16bppRgb555:
                        case PixelFormat.Format16bppArgb1555:
                            {
                                int v = ((ushort*)row)[x];
                                int r = (v >> 10) & 0x1f, g = (v >> 5) & 0x1f, b = v & 0x1f;
                                int a = Format == PixelFormat.Format16bppArgb1555 && (v & 0x8000) == 0 ? 0 : 255;
                                return (a << 24) | (((r << 3) | (r >> 2)) << 16) | (((g << 3) | (g >> 2)) << 8) | ((b << 3) | (b >> 2));
                            }
                        case PixelFormat.Format16bppRgb565:
                            {
                                int v = ((ushort*)row)[x];
                                int r = (v >> 11) & 0x1f, g = (v >> 5) & 0x3f, b = v & 0x1f;
                                return unchecked((int)0xFF000000) | (((r << 3) | (r >> 2)) << 16) | (((g << 2) | (g >> 4)) << 8) | ((b << 3) | (b >> 2));
                            }
                        case PixelFormat.Format16bppGrayScale:
                            {
                                int v = ((ushort*)row)[x] >> 8;
                                return unchecked((int)0xFF000000) | (v << 16) | (v << 8) | v;
                            }
                        case PixelFormat.Format48bppRgb:
                            {
                                ushort* p = (ushort*)(row + x * 6);
                                return unchecked((int)0xFF000000) | (To8(p[2]) << 16) | (To8(p[1]) << 8) | To8(p[0]);
                            }
                        case PixelFormat.Format64bppArgb:
                        case PixelFormat.Format64bppPArgb:
                            {
                                ushort* p = (ushort*)(row + x * 8);
                                int argb = (To8(p[3]) << 24) | (To8(p[2]) << 16) | (To8(p[1]) << 8) | To8(p[0]);
                                return Format == PixelFormat.Format64bppPArgb ? (int)Unpremultiply((uint)argb) : argb;
                            }
                        case PixelFormat.Format8bppIndexed:
                            return PaletteColor(row[x]);
                        case PixelFormat.Format4bppIndexed:
                            return PaletteColor((x & 1) == 0 ? row[x >> 1] >> 4 : row[x >> 1] & 0x0f);
                        case PixelFormat.Format1bppIndexed:
                            return PaletteColor((row[x >> 3] >> (7 - (x & 7))) & 1);
                        default:
                            return 0;
                    }
                }

                private static int To8(ushort v) => Math.Min(255, v * 255 / 8192);   // GDI+'s 48/64bpp hold 0..8192 (gamma 1.0)

                private static ushort From8(int v) => (ushort)(v * 8192 / 255);

                private int PaletteColor(int index) =>
                    Palette != null && index < Palette.Length ? Palette[index] : unchecked((int)0xFF000000);

                /// <summary>A pixel into Scan0 (GDI+'s ARGB, the pixel format's).</summary>
                public void SetArgb(int x, int y, int argb)
                {
                    byte* row = Row(y);
                    switch (Format)
                    {
                        case PixelFormat.Format32bppArgb:
                            ((int*)row)[x] = argb;
                            break;
                        case PixelFormat.Format32bppPArgb:
                            ((uint*)row)[x] = Premultiply(argb);
                            break;
                        case PixelFormat.Format32bppRgb:
                            ((int*)row)[x] = argb | unchecked((int)0xFF000000);
                            break;
                        case PixelFormat.Format24bppRgb:
                            {
                                byte* p = row + x * 3;
                                p[0] = (byte)argb; p[1] = (byte)(argb >> 8); p[2] = (byte)(argb >> 16);
                                break;
                            }
                        case PixelFormat.Format16bppRgb555:
                        case PixelFormat.Format16bppArgb1555:
                            {
                                int v = (((argb >> 19) & 0x1f) << 10) | (((argb >> 11) & 0x1f) << 5) | ((argb >> 3) & 0x1f);
                                if (Format == PixelFormat.Format16bppArgb1555 && ((uint)argb >> 24) >= 128)
                                    v |= 0x8000;
                                ((ushort*)row)[x] = (ushort)v;
                                break;
                            }
                        case PixelFormat.Format16bppRgb565:
                            ((ushort*)row)[x] = (ushort)((((argb >> 19) & 0x1f) << 11) | (((argb >> 10) & 0x3f) << 5) | ((argb >> 3) & 0x1f));
                            break;
                        case PixelFormat.Format48bppRgb:
                            {
                                ushort* p = (ushort*)(row + x * 6);
                                p[0] = From8(argb & 0xff); p[1] = From8((argb >> 8) & 0xff); p[2] = From8((argb >> 16) & 0xff);
                                break;
                            }
                        case PixelFormat.Format64bppArgb:
                        case PixelFormat.Format64bppPArgb:
                            {
                                int v = Format == PixelFormat.Format64bppPArgb ? (int)Premultiply(argb) : argb;
                                ushort* p = (ushort*)(row + x * 8);
                                p[0] = From8(v & 0xff); p[1] = From8((v >> 8) & 0xff); p[2] = From8((v >> 16) & 0xff); p[3] = From8((v >> 24) & 0xff);
                                break;
                            }
                        case PixelFormat.Format8bppIndexed:
                            row[x] = (byte)NearestIndex(argb);
                            break;
                        case PixelFormat.Format4bppIndexed:
                            {
                                int i = NearestIndex(argb) & 0x0f;
                                row[x >> 1] = (x & 1) == 0 ? (byte)((row[x >> 1] & 0x0f) | (i << 4)) : (byte)((row[x >> 1] & 0xf0) | i);
                                break;
                            }
                        case PixelFormat.Format1bppIndexed:
                            {
                                int bit = 7 - (x & 7);
                                row[x >> 3] = NearestIndex(argb) != 0 ? (byte)(row[x >> 3] | (1 << bit)) : (byte)(row[x >> 3] & ~(1 << bit));
                                break;
                            }
                    }
                }

                public int NearestIndex(int argb)
                {
                    if (Palette == null || Palette.Length == 0)
                        return 0;
                    int best = 0, bestDistance = int.MaxValue;
                    for (int i = 0; i < Palette.Length; i++)
                    {
                        int p = Palette[i];
                        int da = ((p >> 24) & 0xff) - ((argb >> 24) & 0xff), dr = ((p >> 16) & 0xff) - ((argb >> 16) & 0xff);
                        int dg = ((p >> 8) & 0xff) - ((argb >> 8) & 0xff), db = (p & 0xff) - (argb & 0xff);
                        int d = da * da + dr * dr + dg * dg + db * db;
                        if (d < bestDistance) { bestDistance = d; best = i; if (d == 0) break; }
                    }
                    return best;
                }
            }

            internal static uint Premultiply(int argb)
            {
                uint a = (uint)argb >> 24;
                if (a == 255)
                    return (uint)argb;
                if (a == 0)
                    return 0;
                uint r = (((uint)argb >> 16) & 0xff) * a / 255, g = (((uint)argb >> 8) & 0xff) * a / 255, b = ((uint)argb & 0xff) * a / 255;
                return (a << 24) | (r << 16) | (g << 8) | b;
            }

            internal static uint Unpremultiply(uint pargb)
            {
                uint a = pargb >> 24;
                if (a == 255)
                    return pargb;
                if (a == 0)
                    return 0;
                uint r = Math.Min(255, ((pargb >> 16) & 0xff) * 255 / a), g = Math.Min(255, ((pargb >> 8) & 0xff) * 255 / a), b = Math.Min(255, (pargb & 0xff) * 255 / a);
                return (a << 24) | (r << 16) | (g << 8) | b;
            }

            /// <summary>GDI+'s default palettes: black and white; the 16 VGA colours; the halftone palette of 8 bpp.</summary>
            internal static int[] DefaultPalette(PixelFormat format, out int flags)
            {
                switch (format)
                {
                    case PixelFormat.Format1bppIndexed:
                        flags = (int)PaletteFlags.GrayScale;
                        return new[] { unchecked((int)0xFF000000), unchecked((int)0xFFFFFFFF) };
                    case PixelFormat.Format4bppIndexed:
                        flags = 0;
                        return new[]
                        {
                            unchecked((int)0xFF000000), unchecked((int)0xFF800000), unchecked((int)0xFF008000), unchecked((int)0xFF808000),
                            unchecked((int)0xFF000080), unchecked((int)0xFF800080), unchecked((int)0xFF008080), unchecked((int)0xFF808080),
                            unchecked((int)0xFFC0C0C0), unchecked((int)0xFFFF0000), unchecked((int)0xFF00FF00), unchecked((int)0xFFFFFF00),
                            unchecked((int)0xFF0000FF), unchecked((int)0xFFFF00FF), unchecked((int)0xFF00FFFF), unchecked((int)0xFFFFFFFF),
                        };
                    default:
                        flags = (int)PaletteFlags.Halftone;
                        return HalftonePalette();
                }
            }

            /// <summary>GDI+'s 8 bpp halftone palette: the 16 VGA colours, 24 transparent, the 6x6x6 cube (blue fastest).</summary>
            private static int[] HalftonePalette()
            {
                var p = new int[256];
                int[] vga = DefaultPalette(PixelFormat.Format4bppIndexed, out _);
                Array.Copy(vga, p, 16);
                int i = 40;
                for (int r = 0; r < 6; r++)
                    for (int g = 0; g < 6; g++)
                        for (int b = 0; b < 6; b++)
                            p[i++] = unchecked((int)0xFF000000) | ((r * 0x33) << 16) | ((g * 0x33) << 8) | (b * 0x33);
                return p;
            }

            // Creation

            internal static int GdipCreateBitmapFromScan0(int width, int height, int stride, int format, IntPtr scan0, out IntPtr bitmap)
            {
                bitmap = IntPtr.Zero;
                if (width <= 0 || height <= 0)
                    return InvalidParameter;
                var pixelFormat = (PixelFormat)format;
                if (!GpBitmap.Supported(pixelFormat))
                    return InvalidParameter;
                if (scan0 != IntPtr.Zero && (stride == 0 || stride % 4 != 0))
                    return InvalidParameter;
                if ((long)GpBitmap.StrideOf(pixelFormat, width) * height > int.MaxValue)
                    return OutOfMemory;
                bitmap = NewHandle(GpBitmap.Create(width, height, pixelFormat, scan0 != IntPtr.Zero ? stride : 0, scan0));
                return Ok;
            }

            internal static int GdipCreateBitmapFromGraphics(int width, int height, HandleRef graphics, out IntPtr bitmap)
            {
                bitmap = IntPtr.Zero;
                if (!Is(graphics, out GpGraphics? g) || width <= 0 || height <= 0)
                    return InvalidParameter;
                GpBitmap b = GpBitmap.Create(width, height, PixelFormat.Format32bppPArgb);
                b.DpiX = g.DpiX;
                b.DpiY = g.DpiY;
                bitmap = NewHandle(b);
                return Ok;
            }

            internal static int GdipDisposeImage(IntPtr image) => Delete<GpImage>(image);

            internal static int GdipDisposeImage(HandleRef image) => Delete<GpImage>(image.Handle);

            internal static int GdipCloneImage(IntPtr image, out IntPtr imageclone)
            {
                imageclone = IntPtr.Zero;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                imageclone = NewHandle(i.Clone());
                return Ok;
            }

            internal static int GdipCloneBitmapAreaI(int x, int y, int width, int height, int format, HandleRef srcbitmap, out IntPtr dstbitmap) =>
                GdipCloneBitmapArea(x, y, width, height, format, srcbitmap, out dstbitmap);

            internal static int GdipCloneBitmapArea(float x, float y, float width, float height, int format, HandleRef srcbitmap, out IntPtr dstbitmap)
            {
                dstbitmap = IntPtr.Zero;
                if (!Is(srcbitmap, out GpBitmap? src))
                    return InvalidParameter;
                int ix = (int)x, iy = (int)y, iw = (int)width, ih = (int)height;
                if (iw <= 0 || ih <= 0 || ix < 0 || iy < 0 || ix + iw > src.W || iy + ih > src.H)
                    return OutOfMemory;
                var pixelFormat = (PixelFormat)format;
                if (pixelFormat == PixelFormat.DontCare || pixelFormat == 0)
                    pixelFormat = src.Format;
                if (!GpBitmap.Supported(pixelFormat))
                    return OutOfMemory;
                src.EnsureScan0();
                GpBitmap dst = GpBitmap.Create(iw, ih, pixelFormat);
                if (GpBitmap.IsIndexed(pixelFormat) && src.Palette != null)
                    dst.Palette = (int[])src.Palette.Clone();
                for (int j = 0; j < ih; j++)
                    for (int i = 0; i < iw; i++)
                        dst.SetArgb(i, j, src.GetArgb(ix + i, iy + j));
                dst.DpiX = src.DpiX;
                dst.DpiY = src.DpiY;
                dst.RawFormat = src.RawFormat;
                dstbitmap = NewHandle(dst);
                return Ok;
            }

            // Properties

            internal static int GdipGetImageType(IntPtr image, out int type)
            {
                type = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                type = i.ImageType;
                return Ok;
            }

            internal static int GdipGetImageType(HandleRef image, out int type) => GdipGetImageType(image.Handle, out type);

            internal static int GdipGetImageWidth(HandleRef image, out int width)
            {
                width = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                width = i.Width;
                return Ok;
            }

            internal static int GdipGetImageHeight(HandleRef image, out int height)
            {
                height = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                height = i.Height;
                return Ok;
            }

            internal static int GdipGetImageDimension(HandleRef image, out float width, out float height)
            {
                width = height = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                if (i is GpMetafile m)
                {
                    // libgdiplus: a metafile's in hundredths of a millimetre.
                    width = m.Header.Width / m.Header.DpiX * 2540;
                    height = m.Header.Height / m.Header.DpiY * 2540;
                    return Ok;
                }
                width = i.Width;
                height = i.Height;
                return Ok;
            }

            internal static int GdipGetImageBounds(IntPtr image, out RectangleF source, ref GraphicsUnit unit)
            {
                source = default;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                source = i is GpMetafile m ? new RectangleF(m.Header.X, m.Header.Y, m.Header.Width, m.Header.Height) : new RectangleF(0, 0, i.Width, i.Height);
                unit = GraphicsUnit.Pixel;
                return Ok;
            }

            internal static int GdipGetImageHorizontalResolution(HandleRef image, out float horzRes)
            {
                horzRes = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                horzRes = i.DpiX;
                return Ok;
            }

            internal static int GdipGetImageVerticalResolution(HandleRef image, out float vertRes)
            {
                vertRes = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                vertRes = i.DpiY;
                return Ok;
            }

            internal static int GdipGetImageFlags(HandleRef image, out int flags)
            {
                flags = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                flags = i.Flags;
                return Ok;
            }

            internal static int GdipGetImagePixelFormat(HandleRef image, out PixelFormat format)
            {
                format = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                format = i is GpBitmap b ? b.Format : PixelFormat.Format32bppRgb;
                return Ok;
            }

            internal static int GdipGetImageRawFormat(HandleRef image, ref Guid format)
            {
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                format = i.RawFormat;
                return Ok;
            }

            internal static int GdipBitmapSetResolution(HandleRef bitmap, float dpix, float dpiy)
            {
                if (!Is(bitmap, out GpBitmap? b) || float.IsNaN(dpix) || float.IsNaN(dpiy) || dpix <= 0.0f || dpiy <= 0.0f)
                    return InvalidParameter;
                b.DpiX = dpix;
                b.DpiY = dpiy;
                b.Flags |= (int)ImageFlags.HasRealDpi;
                return Ok;
            }

            internal static int GdipBitmapGetPixel(HandleRef bitmap, int x, int y, out int argb)
            {
                argb = 0;
                if (!Is(bitmap, out GpBitmap? b))
                    return InvalidParameter;
                if (!GpBitmap.IsIndexed(b.Format) && b.Locked)
                    return WrongState;
                if (x < 0 || x >= b.W || y < 0 || y >= b.H)
                    return InvalidParameter;
                if (b.Format == PixelFormat.Format16bppGrayScale)
                    return InvalidParameter;
                b.EnsureScan0();
                argb = b.GetArgb(x, y);
                return Ok;
            }

            internal static int GdipBitmapSetPixel(HandleRef bitmap, int x, int y, int argb)
            {
                if (!Is(bitmap, out GpBitmap? b))
                    return InvalidParameter;
                if (GpBitmap.IsIndexed(b.Format))
                    return InvalidParameter;
                if (b.Locked)
                    return WrongState;
                if (x < 0 || x >= b.W || y < 0 || y >= b.H)
                    return InvalidParameter;
                if (b.Format == PixelFormat.Format16bppGrayScale)
                    return InvalidParameter;
                b.EnsureScan0();
                b.SetArgb(x, y, argb);
                b.Scan0Changed();
                return Ok;
            }

            // LockBits: the pixels in the format asked, in a buffer of its own unless they are the bitmap's as they are.

            internal static int GdipBitmapLockBits(HandleRef bitmap, ref Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData lockedBitmapData)
            {
                if (!Is(bitmap, out GpBitmap? b) || lockedBitmapData == null)
                    return InvalidParameter;
                if (b.Locked)
                    return WrongState;
                if (rect.X < 0 || rect.Y < 0 || rect.Width <= 0 || rect.Height <= 0 || rect.Right > b.W || rect.Bottom > b.H)
                    return InvalidParameter;
                if (!GpBitmap.Supported(format))
                    return InvalidParameter;
                if (GpBitmap.IsIndexed(format) && format != b.Format)
                    return InvalidParameter;
                b.EnsureScan0();
                var locked = new LockedBits { Rect = rect, Mode = flags, Format = format };
                int stride = GpBitmap.StrideOf(format, rect.Width);
                if (format == b.Format && (GpBitmap.BitsPerPixel(format) >= 8 || rect.X * GpBitmap.BitsPerPixel(format) % 8 == 0))
                {
                    locked.Scan0 = (IntPtr)((byte*)b.Scan0 + (long)rect.Y * b.Stride + (long)rect.X * GpBitmap.BitsPerPixel(format) / 8);
                    stride = b.Stride;
                }
                else
                {
                    GpBitmap temp = GpBitmap.Create(rect.Width, rect.Height, format);
                    if (b.Palette != null)
                        temp.Palette = b.Palette;
                    if ((flags & ImageLockMode.ReadOnly) != 0 || (flags & ImageLockMode.UserInputBuffer) == 0)
                        for (int y = 0; y < rect.Height; y++)
                            for (int x = 0; x < rect.Width; x++)
                                temp.SetArgb(x, y, b.GetArgb(rect.X + x, rect.Y + y));
                    locked.Temp = temp;
                    locked.Scan0 = temp.Scan0;
                    stride = temp.Stride;
                }
                lockedBitmapData.Width = rect.Width;
                lockedBitmapData.Height = rect.Height;
                lockedBitmapData.Stride = stride;
                lockedBitmapData.PixelFormat = format;
                lockedBitmapData.Scan0 = locked.Scan0;
                lockedBitmapData.Reserved = 0;
                b.Locked = true;
                s_locked.AddOrUpdate(b, locked);
                return Ok;
            }

            private sealed class LockedBits
            {
                public Rectangle Rect;
                public ImageLockMode Mode;
                public PixelFormat Format;
                public IntPtr Scan0;
                public GpBitmap? Temp;
            }

            private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<GpBitmap, LockedBits> s_locked = new();

            internal static int GdipBitmapUnlockBits(HandleRef bitmap, BitmapData lockedBitmapData)
            {
                if (!Is(bitmap, out GpBitmap? b) || lockedBitmapData == null)
                    return InvalidParameter;
                if (!b.Locked || !s_locked.TryGetValue(b, out LockedBits? locked))
                    return GenericError;   // GDI+: not locked
                if ((locked.Mode & ImageLockMode.WriteOnly) != 0)
                {
                    if (locked.Temp != null)
                    {
                        if (locked.Temp.Scan0 != lockedBitmapData.Scan0)
                        {
                            // UserInputBuffer: the caller's pixels
                            GpBitmap user = GpBitmap.Create(locked.Rect.Width, locked.Rect.Height, locked.Format, lockedBitmapData.Stride, lockedBitmapData.Scan0);
                            user.Palette = b.Palette;
                            CopyInto(b, user, locked.Rect);
                        }
                        else
                        {
                            CopyInto(b, locked.Temp, locked.Rect);
                        }
                    }
                    b.Scan0Changed();
                }
                locked.Temp?.Dispose();
                s_locked.Remove(b);
                b.Locked = false;
                return Ok;
            }

            private static void CopyInto(GpBitmap target, GpBitmap from, Rectangle rect)
            {
                for (int y = 0; y < rect.Height; y++)
                    for (int x = 0; x < rect.Width; x++)
                        target.SetArgb(rect.X + x, rect.Y + y, from.GetArgb(x, y));
            }

            // Palettes

            internal static int GdipGetImagePaletteSize(IntPtr image, out int size)
            {
                size = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                int count = (i as GpBitmap)?.Palette?.Length ?? 0;
                size = 8 + 4 * count;   // Flags, Count, Entries
                return Ok;
            }

            internal static int GdipGetImagePalette(IntPtr image, IntPtr palette, int size)
            {
                if (!Is(image, out GpImage? i) || palette == IntPtr.Zero)
                    return InvalidParameter;
                int[] entries = (i as GpBitmap)?.Palette ?? Array.Empty<int>();
                if (size < 8 + 4 * entries.Length)
                    return InvalidParameter;
                int* p = (int*)palette;
                p[0] = (i as GpBitmap)?.PaletteFlags ?? 0;
                p[1] = entries.Length;
                for (int k = 0; k < entries.Length; k++)
                    p[2 + k] = entries[k];
                return Ok;
            }

            internal static int GdipSetImagePalette(IntPtr image, IntPtr palette)
            {
                if (!Is(image, out GpBitmap? b) || palette == IntPtr.Zero)
                    return InvalidParameter;
                int* p = (int*)palette;
                int count = p[1];
                if (count <= 0 || count > 256)
                    return InvalidParameter;
                b.PaletteFlags = p[0];
                b.Palette = new ReadOnlySpan<int>(p + 2, count).ToArray();
                b.Scan0Changed();
                return Ok;
            }

            internal static int GdipImageRotateFlip(HandleRef image, int rotateFlipType)
            {
                if (!Is(image, out GpBitmap? b))
                    return InvalidParameter;
                var type = (RotateFlipType)rotateFlipType;
                int rotate = (int)type & 3;
                bool flipX = ((int)type & 4) != 0;
                b.EnsureScan0();
                int w = b.W, h = b.H;
                var pixels = new int[w * h];
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        pixels[y * w + x] = b.GetArgb(x, y);
                int nw = rotate % 2 == 1 ? h : w, nh = rotate % 2 == 1 ? w : h;
                GpBitmap n = GpBitmap.Create(nw, nh, b.Format);
                n.Palette = b.Palette;
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        int sx = flipX ? w - 1 - x : x;
                        int dx, dy;
                        switch (rotate)
                        {
                            case 1: dx = h - 1 - y; dy = sx; break;
                            case 2: dx = w - 1 - sx; dy = h - 1 - y; break;
                            case 3: dx = y; dy = w - 1 - sx; break;
                            default: dx = sx; dy = y; break;
                        }
                        n.SetArgb(dx, dy, pixels[y * w + x]);
                    }
                b.Surface?.Dispose();
                b.Surface = null;
                if (b.OwnsScan0)
                    NativeMemory.Free((void*)b.Scan0);
                b.Scan0 = n.Scan0;
                b.OwnsScan0 = true;
                b.Stride = n.Stride;
                b.W = nw;
                b.H = nh;
                if (rotate % 2 == 1)
                    (b.DpiX, b.DpiY) = (b.DpiY, b.DpiX);
                b.SurfaceDirty = b.Scan0Dirty = false;
                b.RawFormat = MemoryBmpGuid;   // GDI+: a turned image is a memory bitmap
                b.Flags = GpBitmap.HasAlpha(b.Format) ? (int)ImageFlags.HasAlpha : 0;
                return Ok;
            }

            // Codecs

            private sealed class Codec
            {
                public Guid Clsid, FormatID;
                public string Name = "", Description = "", Extension = "", Mime = "";
                public int Flags;
                public byte[][] Patterns = Array.Empty<byte[]>(), Masks = Array.Empty<byte[]>();
            }

            private const int CodecEncoderDecoder = (int)(ImageCodecFlags.Encoder | ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap | ImageCodecFlags.Builtin);
            private const int CodecDecoder = (int)(ImageCodecFlags.Decoder | ImageCodecFlags.SupportBitmap | ImageCodecFlags.Builtin);

            private static byte[] Ones(int n)
            {
                var m = new byte[n];
                m.AsSpan().Fill(0xff);
                return m;
            }

            private static readonly Codec[] s_codecs =
            {
                new Codec { Clsid = new Guid("557cf400-1a04-11d3-9a73-0000f81ef32e"), FormatID = BmpGuid, Name = "Built-in BMP Codec", Description = "BMP", Extension = "*.BMP;*.DIB;*.RLE", Mime = "image/bmp", Flags = CodecEncoderDecoder,
                    Patterns = new[] { new byte[] { 0x42, 0x4d } }, Masks = new[] { Ones(2) } },
                new Codec { Clsid = new Guid("557cf401-1a04-11d3-9a73-0000f81ef32e"), FormatID = JpegGuid, Name = "Built-in JPEG Codec", Description = "JPEG", Extension = "*.JPG;*.JPEG;*.JPE;*.JFIF", Mime = "image/jpeg", Flags = CodecEncoderDecoder,
                    Patterns = new[] { new byte[] { 0xff, 0xd8 } }, Masks = new[] { Ones(2) } },
                new Codec { Clsid = new Guid("557cf402-1a04-11d3-9a73-0000f81ef32e"), FormatID = GifGuid, Name = "Built-in GIF Codec", Description = "GIF", Extension = "*.GIF", Mime = "image/gif", Flags = CodecEncoderDecoder,
                    Patterns = new[] { new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 } }, Masks = new[] { Ones(6), Ones(6) } },
                new Codec { Clsid = new Guid("557cf403-1a04-11d3-9a73-0000f81ef32e"), FormatID = EmfGuid, Name = "Built-in EMF Codec", Description = "EMF", Extension = "*.EMF", Mime = "image/x-emf", Flags = CodecDecoder,
                    Patterns = new[] { EmfPattern(false) }, Masks = new[] { EmfPattern(true) } },
                new Codec { Clsid = new Guid("557cf404-1a04-11d3-9a73-0000f81ef32e"), FormatID = WmfGuid, Name = "Built-in WMF Codec", Description = "WMF", Extension = "*.WMF", Mime = "image/x-wmf", Flags = CodecDecoder,
                    Patterns = new[] { new byte[] { 0xd7, 0xcd, 0xc6, 0x9a } }, Masks = new[] { Ones(4) } },
                new Codec { Clsid = new Guid("557cf405-1a04-11d3-9a73-0000f81ef32e"), FormatID = TiffGuid, Name = "Built-in TIFF Codec", Description = "TIFF", Extension = "*.TIF;*.TIFF", Mime = "image/tiff", Flags = CodecEncoderDecoder,
                    Patterns = new[] { new byte[] { 0x49, 0x49 }, new byte[] { 0x4d, 0x4d } }, Masks = new[] { Ones(2), Ones(2) } },
                new Codec { Clsid = new Guid("557cf406-1a04-11d3-9a73-0000f81ef32e"), FormatID = PngGuid, Name = "Built-in PNG Codec", Description = "PNG", Extension = "*.PNG", Mime = "image/png", Flags = CodecEncoderDecoder,
                    Patterns = new[] { new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a } }, Masks = new[] { Ones(8) } },
                new Codec { Clsid = new Guid("557cf407-1a04-11d3-9a73-0000f81ef32e"), FormatID = IconGuid, Name = "Built-in ICO Codec", Description = "ICO", Extension = "*.ICO", Mime = "image/x-icon", Flags = CodecDecoder,
                    Patterns = new[] { new byte[] { 0x00, 0x00, 0x01, 0x00 } }, Masks = new[] { Ones(4) } },
            };

            private static byte[] EmfPattern(bool mask)
            {
                var p = new byte[44];
                p[40] = mask ? (byte)0xff : (byte)0x20; p[41] = mask ? (byte)0xff : (byte)0x45;
                p[42] = mask ? (byte)0xff : (byte)0x4d; p[43] = mask ? (byte)0xff : (byte)0x46;
                return p;
            }

            private static Codec[] Codecs(bool encoders)
            {
                var list = new List<Codec>();
                foreach (Codec c in s_codecs)
                    if (!encoders || (c.Flags & (int)ImageCodecFlags.Encoder) != 0)
                        list.Add(c);
                return list.ToArray();
            }

            private static int CodecsSize(Codec[] codecs)
            {
                int size = Marshal.SizeOf<ImageCodecInfoPrivate>() * codecs.Length;
                foreach (Codec c in codecs)
                {
                    size += (c.Name.Length + 1 + c.Description.Length + 1 + c.Extension.Length + 1 + c.Mime.Length + 1) * 2;
                    foreach (byte[] p in c.Patterns)
                        size += p.Length * 2;
                }
                return size;
            }

            private static int WriteCodecs(Codec[] codecs, int size, IntPtr buffer)
            {
                if (buffer == IntPtr.Zero || size < CodecsSize(codecs))
                    return GenericError;
                int structSize = Marshal.SizeOf<ImageCodecInfoPrivate>();
                byte* extra = (byte*)buffer + structSize * codecs.Length;
                IntPtr Str(string s)
                {
                    IntPtr at = (IntPtr)extra;
                    fixed (char* c = s)
                        Buffer.MemoryCopy(c, extra, s.Length * 2, s.Length * 2);
                    ((char*)extra)[s.Length] = '\0';
                    extra += (s.Length + 1) * 2;
                    return at;
                }
                for (int i = 0; i < codecs.Length; i++)
                {
                    Codec c = codecs[i];
                    var info = new ImageCodecInfoPrivate
                    {
                        Clsid = c.Clsid,
                        FormatID = c.FormatID,
                        CodecName = Str(c.Name),
                        DllName = IntPtr.Zero,
                        FormatDescription = Str(c.Description),
                        FilenameExtension = Str(c.Extension),
                        MimeType = Str(c.Mime),
                        Flags = c.Flags,
                        Version = 1,
                        SigCount = c.Patterns.Length,
                        SigSize = c.Patterns.Length > 0 ? c.Patterns[0].Length : 0,
                    };
                    info.SigPattern = (IntPtr)extra;
                    foreach (byte[] p in c.Patterns)
                    {
                        p.AsSpan().CopyTo(new Span<byte>(extra, p.Length));
                        extra += p.Length;
                    }
                    info.SigMask = (IntPtr)extra;
                    foreach (byte[] m in c.Masks)
                    {
                        m.AsSpan().CopyTo(new Span<byte>(extra, m.Length));
                        extra += m.Length;
                    }
                    Marshal.StructureToPtr(info, (IntPtr)((byte*)buffer + structSize * i), false);
                }
                return Ok;
            }

            internal static int GdipGetImageEncodersSize(out int numEncoders, out int size)
            {
                Codec[] codecs = Codecs(true);
                numEncoders = codecs.Length;
                size = CodecsSize(codecs);
                return Ok;
            }

            internal static int GdipGetImageEncoders(int numEncoders, int size, IntPtr encoders)
            {
                Codec[] codecs = Codecs(true);
                if (numEncoders != codecs.Length)
                    return GenericError;
                return WriteCodecs(codecs, size, encoders);
            }

            internal static int GdipGetImageDecodersSize(out int numDecoders, out int size)
            {
                Codec[] codecs = Codecs(false);
                numDecoders = codecs.Length;
                size = CodecsSize(codecs);
                return Ok;
            }

            internal static int GdipGetImageDecoders(int numDecoders, int size, IntPtr decoders)
            {
                Codec[] codecs = Codecs(false);
                if (numDecoders != codecs.Length)
                    return GenericError;
                return WriteCodecs(codecs, size, decoders);
            }

            // Reading

            private static byte[]? ReadAll(StreamGetBytesDelegate getBytes)
            {
                var result = new MemoryStream();
                var buffer = new byte[65536];
                fixed (byte* p = buffer)
                {
                    while (true)
                    {
                        int read = getBytes(p, buffer.Length, false);
                        if (read < 0)
                            return null;
                        if (read == 0)
                            break;
                        result.Write(buffer, 0, read);
                    }
                }
                return result.ToArray();
            }

            internal static int GdipLoadImageFromDelegate_linux(StreamGetHeaderDelegate getHeader, StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek, StreamCloseDelegate close, StreamSizeDelegate size, out IntPtr image)
            {
                image = IntPtr.Zero;
                byte[]? data = ReadAll(getBytes);
                if (data == null)
                    return InvalidParameter;
                int status = Decode(data, out GpImage? decoded);
                if (status == Ok)
                    image = NewHandle(decoded!);
                return status;
            }

            internal static int GdipLoadImageFromFile(string filename, out IntPtr image)
            {
                image = IntPtr.Zero;
                if (filename == null)
                    return InvalidParameter;
                byte[] data;
                try
                {
                    data = File.ReadAllBytes(filename);
                }
                catch (IOException)
                {
                    return Gdip.FileNotFound;
                }
                catch (UnauthorizedAccessException)
                {
                    return Gdip.FileNotFound;
                }
                int status = Decode(data, out GpImage? decoded);
                if (status == Ok)
                    image = NewHandle(decoded!);
                return status;
            }

            internal static int GdipLoadImageFromFileICM(string filename, out IntPtr image) => GdipLoadImageFromFile(filename, out image);

            internal static int GdipCreateBitmapFromFile(string filename, out IntPtr bitmap) => GdipLoadImageFromFile(filename, out bitmap);

            internal static int GdipCreateBitmapFromFileICM(string filename, out IntPtr bitmap) => GdipLoadImageFromFile(filename, out bitmap);

            /// <summary>An image read: its pixels in the pixel format GDI+ gives the file's (24 bpp without alpha, 32 bpp ARGB with).</summary>
            internal static int Decode(byte[] data, out GpImage? image)
            {
                int here = DecodeHere(data, out image);
                if (here != Ok || image != null)
                    return here;
                using var stream = new SKMemoryStream(data);
                using SKCodec? codec = SKCodec.Create(stream);
                if (codec == null)
                    return OutOfMemory;   // GDI+: an unknown image is an OutOfMemory
                SKImageInfo info = codec.Info;
                if (info.Width <= 0 || info.Height <= 0)
                    return OutOfMemory;
                Guid raw = codec.EncodedFormat switch
                {
                    SKEncodedImageFormat.Png => PngGuid,
                    SKEncodedImageFormat.Jpeg => JpegGuid,
                    SKEncodedImageFormat.Gif => GifGuid,
                    SKEncodedImageFormat.Bmp => BmpGuid,
                    SKEncodedImageFormat.Ico => IconGuid,
                    _ => MemoryBmpGuid,
                };
                var decodeInfo = new SKImageInfo(info.Width, info.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
                using var decoded = new SKBitmap(decodeInfo);
                SKCodecResult result = codec.GetPixels(decodeInfo, decoded.GetPixels());
                if (result != SKCodecResult.Success && result != SKCodecResult.IncompleteInput)
                    return OutOfMemory;
                bool opaque = info.AlphaType == SKAlphaType.Opaque;
                bool gray = info.ColorType == SKColorType.Gray8 && raw == JpegGuid;
                PixelFormat format = gray ? PixelFormat.Format8bppIndexed : opaque ? PixelFormat.Format24bppRgb : PixelFormat.Format32bppArgb;
                GpBitmap b = GpBitmap.Create(info.Width, info.Height, format);
                uint* src = (uint*)decoded.GetPixels();
                int srcStride = decoded.RowBytes / 4;
                if (gray)
                {
                    var palette = new int[256];
                    for (int i = 0; i < 256; i++)
                        palette[i] = unchecked((int)0xFF000000) | (i << 16) | (i << 8) | i;
                    b.Palette = palette;
                    b.PaletteFlags = (int)PaletteFlags.GrayScale;
                    for (int y = 0; y < info.Height; y++)
                        for (int x = 0; x < info.Width; x++)
                            ((byte*)b.Scan0 + (long)y * b.Stride)[x] = (byte)src[(long)y * srcStride + x];
                }
                else
                {
                    for (int y = 0; y < info.Height; y++)
                        for (int x = 0; x < info.Width; x++)
                            b.SetArgb(x, y, (int)src[(long)y * srcStride + x]);
                }
                b.RawFormat = raw;
                b.Flags = (int)(ImageFlags.ColorSpaceRgb | ImageFlags.HasRealPixelSize | ImageFlags.HasRealDpi | ImageFlags.ReadOnly) | (opaque || gray ? 0 : (int)ImageFlags.HasAlpha);
                if (ReadDpi(data, raw, out float dpiX, out float dpiY))
                {
                    b.DpiX = dpiX;
                    b.DpiY = dpiY;
                }
                if (raw == JpegGuid)
                    JpegProperties(data, b);
                image = b;
                return Ok;
            }

            /// <summary>The resolution a file gives (PNG's pHYs, JPEG's JFIF, BMP's header).</summary>
            private static bool ReadDpi(byte[] data, Guid format, out float dpiX, out float dpiY)
            {
                dpiX = dpiY = 96;
                try
                {
                    if (format == PngGuid)
                    {
                        int at = 8;
                        while (at + 8 <= data.Length)
                        {
                            int length = (data[at] << 24) | (data[at + 1] << 16) | (data[at + 2] << 8) | data[at + 3];
                            string type = System.Text.Encoding.ASCII.GetString(data, at + 4, 4);
                            if (type == "pHYs" && length >= 9 && data[at + 16] == 1)
                            {
                                int x = (data[at + 8] << 24) | (data[at + 9] << 16) | (data[at + 10] << 8) | data[at + 11];
                                int y = (data[at + 12] << 24) | (data[at + 13] << 16) | (data[at + 14] << 8) | data[at + 15];
                                dpiX = (float)(x * 0.0254);
                                dpiY = (float)(y * 0.0254);
                                return true;
                            }
                            if (type == "IDAT" || type == "IEND")
                                break;
                            at += 12 + length;
                        }
                    }
                    else if (format == JpegGuid)
                    {
                        if (data.Length > 18 && data[2] == 0xff && data[3] == 0xe0 && data[6] == 'J' && data[7] == 'F' && data[8] == 'I' && data[9] == 'F')
                        {
                            int units = data[13];
                            int x = (data[14] << 8) | data[15], y = (data[16] << 8) | data[17];
                            if (units == 1 && x > 0 && y > 0) { dpiX = x; dpiY = y; return true; }
                            if (units == 2 && x > 0 && y > 0) { dpiX = x * 2.54f; dpiY = y * 2.54f; return true; }
                        }
                    }
                    else if (format == BmpGuid)
                    {
                        if (data.Length >= 46)
                        {
                            int x = BitConverter.ToInt32(data, 38), y = BitConverter.ToInt32(data, 42);
                            if (x > 0 && y > 0) { dpiX = (float)(x * 0.0254); dpiY = (float)(y * 0.0254); return true; }
                        }
                    }
                }
                catch (ArgumentException)
                {
                }
                return false;
            }

            // Writing

            internal static int GdipSaveImageToDelegate_linux(IntPtr image, StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek, StreamCloseDelegate close, StreamSizeDelegate size, ref Guid encoderClsID, IntPtr encoderParameters)
            {
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                if (i is GpBitmap first && SaveFlag(encoderParameters) == EncoderValueMultiFrame && IsEncoder(encoderClsID, TiffGuid))
                {
                    // The first page of several: written now, again with each page SaveAdd adds.
                    i.Saving = new MultiFrameSave { PutBytes = putBytes, Seek = doSeek, Start = doSeek(0, 1) };
                    i.Saving.Pages.Add((GpBitmap)first.Clone());
                    return WritePages(i.Saving);
                }
                int status = Encode(i, encoderClsID, encoderParameters, out byte[]? encoded);
                if (status != Ok)
                    return status;
                fixed (byte* p = encoded)
                    if (putBytes(p, encoded!.Length) != encoded.Length)
                        return GenericError;
                return Ok;
            }

            internal static int GdipSaveImageToFile(IntPtr image, string filename, ref Guid encoderClsID, IntPtr encoderParameters)
            {
                if (!Is(image, out GpImage? i) || filename == null)
                    return InvalidParameter;
                if (i is GpBitmap first && SaveFlag(encoderParameters) == EncoderValueMultiFrame && IsEncoder(encoderClsID, TiffGuid))
                {
                    i.Saving = new MultiFrameSave { FileName = filename };
                    i.Saving.Pages.Add((GpBitmap)first.Clone());
                    return WritePages(i.Saving);
                }
                int status = Encode(i, encoderClsID, encoderParameters, out byte[]? encoded);
                if (status != Ok)
                    return status;
                try
                {
                    File.WriteAllBytes(filename, encoded!);
                }
                catch (IOException)
                {
                    return GenericError;
                }
                catch (UnauthorizedAccessException)
                {
                    return Gdip.AccessDenied;
                }
                return Ok;
            }

            private static readonly Guid s_qualityEncoder = new Guid("1d5be4b5-fa4a-452d-9cdd-5db35105e7eb");

            private static int Encode(GpImage image, Guid encoder, IntPtr parameters, out byte[]? encoded)
            {
                encoded = null;
                Codec? codec = null;
                foreach (Codec c in s_codecs)
                    if (c.Clsid == encoder && (c.Flags & (int)ImageCodecFlags.Encoder) != 0)
                        codec = c;
                if (codec == null)
                    return Gdip.FileNotFound;   // GDI+: no encoder of that class id
                if (image is not GpBitmap bitmap)
                    return NotImplemented;
                int quality = 75;
                if (parameters != IntPtr.Zero)
                {
                    int count = *(int*)parameters;
                    byte* p = (byte*)parameters + IntPtr.Size;   // EncoderParameters: Count, then (aligned) the parameters
                    int parameterSize = 16 + 4 + 4 + IntPtr.Size;
                    for (int k = 0; k < count; k++, p += parameterSize)
                        if (*(Guid*)p == s_qualityEncoder && *(int*)(p + 16) >= 1)
                        {
                            IntPtr value = *(IntPtr*)(p + 24);
                            if (value != IntPtr.Zero)
                                quality = Math.Clamp(*(int*)value, 0, 100);
                        }
                }
                bool opaque = !GpBitmap.HasAlpha(bitmap.Format) && !GpBitmap.IsIndexed(bitmap.Format);
                encoded = EncodeHere(bitmap, codec.FormatID);
                if (encoded != null)
                    return Ok;
                SKEncodedImageFormat format;
                if (codec.FormatID == PngGuid)
                    format = SKEncodedImageFormat.Png;
                else if (codec.FormatID == JpegGuid)
                    format = SKEncodedImageFormat.Jpeg;
                else
                    return NotImplemented;
                using SKBitmap pixels = bitmap.ToUnpremultiplied(opaque || format == SKEncodedImageFormat.Jpeg);
                using SKData? data = pixels.Encode(format, quality);
                if (data == null)
                    return GenericError;
                encoded = data.ToArray();
                return Ok;
            }

            private static bool IsEncoder(Guid clsid, Guid format)
            {
                foreach (Codec c in s_codecs)
                    if (c.Clsid == clsid)
                        return c.FormatID == format;
                return false;
            }

            internal static int GdipGetEncoderParameterListSize(HandleRef image, ref Guid encoder, out int size)
            {
                size = 0;
                if (!Is(image, out GpImage? _))
                    return InvalidParameter;
                return Gdip.FileNotFound;
            }

            // HBITMAP and HICON: bitmaps of their own here (as libgdiplus' clones).

            internal static int GdipCreateHBITMAPFromBitmap(HandleRef nativeBitmap, out IntPtr hbitmap, int argbBackground)
            {
                hbitmap = IntPtr.Zero;
                if (!Is(nativeBitmap, out GpBitmap? b))
                    return InvalidParameter;
                hbitmap = NewHandle(b.Clone());
                return Ok;
            }

            internal static int GdipCreateBitmapFromHBITMAP(IntPtr hbitmap, IntPtr hpalette, out IntPtr bitmap)
            {
                bitmap = IntPtr.Zero;
                if (!Is(hbitmap, out GpBitmap? b))
                    return InvalidParameter;
                bitmap = NewHandle(b.Clone());
                return Ok;
            }

            internal static int GdipCreateHICONFromBitmap(HandleRef nativeBitmap, out IntPtr hicon)
            {
                hicon = IntPtr.Zero;
                if (!Is(nativeBitmap, out GpBitmap? b))
                    return InvalidParameter;
                hicon = NewHandle(b.Clone());
                return Ok;
            }

            internal static int GdipCreateBitmapFromHICON(IntPtr hicon, out IntPtr bitmap)
            {
                bitmap = IntPtr.Zero;
                if (!Is(hicon, out GpBitmap? b))
                    return InvalidParameter;
                GpBitmap copy = (GpBitmap)b.Clone();
                copy.RawFormat = MemoryBmpGuid;
                bitmap = NewHandle(copy);
                return Ok;
            }

        }
    }
}
