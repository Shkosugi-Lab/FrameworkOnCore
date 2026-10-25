// WebFormsForCore: GDI+'s DrawImage and ImageAttributes over Skia. An image is drawn through the matrix that takes its
// source rectangle (in pixels) to the destination; drawn unscaled at whole pixels it is copied as it is (nearest), scaled
// it is sampled as the interpolation mode says. The attributes (colour keys, remap tables, the colour matrix, gamma,
// threshold) are applied to a copy of the pixels first. A metafile is its recorded picture, drawn.
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            internal sealed class GpImageAttributes
            {
                public sealed class Adjust
                {
                    public bool NoOp;
                    public bool HasColorKey;
                    public int ColorLow, ColorHigh;
                    public ColorMap[]? Remap;
                    public float[,]? Matrix, GrayMatrix;
                    public ColorMatrixFlag MatrixFlags;
                    public float Gamma = 1;
                    public bool HasGamma;
                    public float Threshold;
                    public bool HasThreshold;
                    public ColorChannelFlag? OutputChannel;

                    public Adjust Clone()
                    {
                        var a = (Adjust)MemberwiseClone();
                        a.Remap = (ColorMap[]?)Remap?.Clone();
                        a.Matrix = (float[,]?)Matrix?.Clone();
                        a.GrayMatrix = (float[,]?)GrayMatrix?.Clone();
                        return a;
                    }

                    public bool IsIdentity => NoOp || (!HasColorKey && Remap == null && Matrix == null && !HasGamma && !HasThreshold && OutputChannel == null);
                }

                public readonly Dictionary<ColorAdjustType, Adjust> Adjusts = new Dictionary<ColorAdjustType, Adjust>();
                public WrapMode WrapMode = WrapMode.Clamp;
                public int WrapColor;
                public bool Clamp;

                public Adjust For(ColorAdjustType type)
                {
                    if (!Adjusts.TryGetValue(type, out Adjust? a))
                        Adjusts[type] = a = new Adjust();
                    return a;
                }

                /// <summary>A bitmap's adjustments: its own, or the default ones.</summary>
                public Adjust? ForBitmap() =>
                    Adjusts.TryGetValue(ColorAdjustType.Bitmap, out Adjust? a) ? a : Adjusts.TryGetValue(ColorAdjustType.Default, out a) ? a : null;

                public GpImageAttributes Clone()
                {
                    var c = new GpImageAttributes { WrapMode = WrapMode, WrapColor = WrapColor, Clamp = Clamp };
                    foreach (KeyValuePair<ColorAdjustType, Adjust> kv in Adjusts)
                        c.Adjusts[kv.Key] = kv.Value.Clone();
                    return c;
                }

                /// <summary>A colour as the adjustments make it.</summary>
                public static int Apply(Adjust a, int argb)
                {
                    int alpha = (argb >> 24) & 0xff, r = (argb >> 16) & 0xff, g = (argb >> 8) & 0xff, b = argb & 0xff;
                    if (a.HasColorKey)
                    {
                        if (r >= ((a.ColorLow >> 16) & 0xff) && r <= ((a.ColorHigh >> 16) & 0xff)
                            && g >= ((a.ColorLow >> 8) & 0xff) && g <= ((a.ColorHigh >> 8) & 0xff)
                            && b >= (a.ColorLow & 0xff) && b <= (a.ColorHigh & 0xff))
                            return 0;
                    }
                    if (a.Remap != null)
                    {
                        foreach (ColorMap map in a.Remap)
                            if (map.OldColor.ToArgb() == argb)
                            {
                                argb = map.NewColor.ToArgb();
                                alpha = (argb >> 24) & 0xff; r = (argb >> 16) & 0xff; g = (argb >> 8) & 0xff; b = argb & 0xff;
                                break;
                            }
                    }
                    if (a.Matrix != null)
                    {
                        bool gray = r == g && g == b;
                        if (!(a.MatrixFlags == ColorMatrixFlag.SkipGrays && gray))
                        {
                            float[,] m = a.MatrixFlags == ColorMatrixFlag.AltGrays && gray && a.GrayMatrix != null ? a.GrayMatrix : a.Matrix;
                            float fr = r / 255f, fg = g / 255f, fb = b / 255f, fa = alpha / 255f;
                            float nr = fr * m[0, 0] + fg * m[1, 0] + fb * m[2, 0] + fa * m[3, 0] + m[4, 0];
                            float ng = fr * m[0, 1] + fg * m[1, 1] + fb * m[2, 1] + fa * m[3, 1] + m[4, 1];
                            float nb = fr * m[0, 2] + fg * m[1, 2] + fb * m[2, 2] + fa * m[3, 2] + m[4, 2];
                            float na = fr * m[0, 3] + fg * m[1, 3] + fb * m[2, 3] + fa * m[3, 3] + m[4, 3];
                            r = ToByte(nr); g = ToByte(ng); b = ToByte(nb); alpha = ToByte(na);
                        }
                    }
                    if (a.HasGamma && a.Gamma > 0)
                    {
                        r = ToByte((float)Math.Pow(r / 255.0, a.Gamma));
                        g = ToByte((float)Math.Pow(g / 255.0, a.Gamma));
                        b = ToByte((float)Math.Pow(b / 255.0, a.Gamma));
                    }
                    if (a.HasThreshold)
                    {
                        int t = (int)(a.Threshold * 255);
                        r = r > t ? 255 : 0; g = g > t ? 255 : 0; b = b > t ? 255 : 0;
                    }
                    if (a.OutputChannel is ColorChannelFlag channel)
                    {
                        // CMYK channels as GDI+ gives them: the intensity of the channel, gray.
                        int c = 255 - r, mm = 255 - g, y = 255 - b, k = Math.Min(c, Math.Min(mm, y));
                        int v = channel switch
                        {
                            ColorChannelFlag.ColorChannelC => c - k,
                            ColorChannelFlag.ColorChannelM => mm - k,
                            ColorChannelFlag.ColorChannelY => y - k,
                            _ => k,
                        };
                        r = g = b = 255 - v;
                    }
                    return (alpha << 24) | (r << 16) | (g << 8) | b;
                }

                private static int ToByte(float v) => (int)Math.Clamp(Math.Round(v * 255), 0, 255);
            }

            internal static int GdipCreateImageAttributes(out IntPtr imageattr)
            {
                imageattr = NewHandle(new GpImageAttributes());
                return Ok;
            }

            internal static int GdipCloneImageAttributes(HandleRef imageattr, out IntPtr cloneImageattr)
            {
                cloneImageattr = IntPtr.Zero;
                if (!Is(imageattr, out GpImageAttributes? a))
                    return InvalidParameter;
                cloneImageattr = NewHandle(a.Clone());
                return Ok;
            }

            internal static int GdipDisposeImageAttributes(HandleRef imageattr) => Delete<GpImageAttributes>(imageattr.Handle);

            private static int SetAdjust(HandleRef imageattr, ColorAdjustType type, Action<GpImageAttributes.Adjust> set)
            {
                if (!Is(imageattr, out GpImageAttributes? a) || type < ColorAdjustType.Default || type >= ColorAdjustType.Count)
                    return InvalidParameter;
                set(a.For(type));
                return Ok;
            }

            internal static int GdipSetImageAttributesColorKeys(HandleRef imageattr, ColorAdjustType type, bool enableFlag, int colorLow, int colorHigh) =>
                SetAdjust(imageattr, type, a => { a.HasColorKey = enableFlag; a.ColorLow = colorLow; a.ColorHigh = colorHigh; });

            internal static int GdipSetImageAttributesColorMatrix(HandleRef imageattr, ColorAdjustType type, bool enableFlag, ColorMatrix? colorMatrix, ColorMatrix? grayMatrix, ColorMatrixFlag flags)
            {
                if (enableFlag && colorMatrix == null)
                    return InvalidParameter;
                if (flags < ColorMatrixFlag.Default || flags > ColorMatrixFlag.AltGrays || (flags == ColorMatrixFlag.AltGrays && grayMatrix == null && enableFlag))
                    return InvalidParameter;
                static float[,]? Copy(ColorMatrix? m)
                {
                    if (m == null)
                        return null;
                    var a = new float[5, 5];
                    for (int i = 0; i < 5; i++)
                        for (int j = 0; j < 5; j++)
                            a[i, j] = m[i, j];
                    return a;
                }
                return SetAdjust(imageattr, type, a =>
                {
                    a.Matrix = enableFlag ? Copy(colorMatrix) : null;
                    a.GrayMatrix = enableFlag ? Copy(grayMatrix) : null;
                    a.MatrixFlags = flags;
                });
            }

            internal static int GdipSetImageAttributesGamma(HandleRef imageattr, ColorAdjustType type, bool enableFlag, float gamma)
            {
                if (enableFlag && gamma <= 0)
                    return InvalidParameter;
                return SetAdjust(imageattr, type, a => { a.HasGamma = enableFlag; a.Gamma = gamma; });
            }

            internal static int GdipSetImageAttributesNoOp(HandleRef imageattr, ColorAdjustType type, bool enableFlag) =>
                SetAdjust(imageattr, type, a => a.NoOp = enableFlag);

            internal static int GdipSetImageAttributesThreshold(HandleRef imageattr, ColorAdjustType type, bool enableFlag, float threshold) =>
                SetAdjust(imageattr, type, a => { a.HasThreshold = enableFlag; a.Threshold = threshold; });

            internal static int GdipSetImageAttributesOutputChannel(HandleRef imageattr, ColorAdjustType type, bool enableFlag, ColorChannelFlag flags)
            {
                if (enableFlag && (flags < ColorChannelFlag.ColorChannelC || flags > ColorChannelFlag.ColorChannelK))
                    return InvalidParameter;
                return SetAdjust(imageattr, type, a => a.OutputChannel = enableFlag ? flags : null);
            }

            internal static int GdipSetImageAttributesOutputChannelColorProfile(HandleRef imageattr, ColorAdjustType type, bool enableFlag, string colorProfileFilename)
            {
                if (!Is(imageattr, out GpImageAttributes? _))
                    return InvalidParameter;
                if (enableFlag && !System.IO.File.Exists(colorProfileFilename))
                    return OutOfMemory;   // GDI+'s, for a profile it cannot read
                return Ok;
            }

            internal static int GdipSetImageAttributesRemapTable(HandleRef imageattr, ColorAdjustType type, bool enableFlag, int mapSize, IntPtr map)
            {
                if (enableFlag && (map == IntPtr.Zero || mapSize <= 0))
                    return InvalidParameter;
                ColorMap[]? maps = null;
                if (enableFlag)
                {
                    maps = new ColorMap[mapSize];
                    int* p = (int*)map;
                    for (int i = 0; i < mapSize; i++)
                        maps[i] = new ColorMap { OldColor = Color.FromArgb(p[2 * i]), NewColor = Color.FromArgb(p[2 * i + 1]) };
                }
                return SetAdjust(imageattr, type, a => a.Remap = maps);
            }

            internal static int GdipSetImageAttributesWrapMode(HandleRef imageattr, int wrapmode, int argb, bool clamp)
            {
                if (!Is(imageattr, out GpImageAttributes? a) || wrapmode < (int)WrapMode.Tile || wrapmode > (int)WrapMode.Clamp)
                    return InvalidParameter;
                a.WrapMode = (WrapMode)wrapmode;
                a.WrapColor = argb;
                a.Clamp = clamp;
                return Ok;
            }

            internal static int GdipGetImageAttributesAdjustedPalette(HandleRef imageattr, IntPtr palette, ColorAdjustType type)
            {
                if (!Is(imageattr, out GpImageAttributes? a) || palette == IntPtr.Zero || type < ColorAdjustType.Default || type >= ColorAdjustType.Count)
                    return InvalidParameter;
                int* p = (int*)palette;
                int count = p[1];
                if (a.Adjusts.TryGetValue(type, out GpImageAttributes.Adjust? adjust) || a.Adjusts.TryGetValue(ColorAdjustType.Default, out adjust))
                    if (!adjust.IsIdentity)
                        for (int i = 0; i < count; i++)
                            p[2 + i] = GpImageAttributes.Apply(adjust, p[2 + i]);
                return Ok;
            }

            // Drawing

            private static SKSamplingOptions Sampling(InterpolationMode mode) => mode switch
            {
                InterpolationMode.NearestNeighbor => new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None),
                InterpolationMode.Bicubic or InterpolationMode.HighQualityBicubic or InterpolationMode.High => new SKSamplingOptions(SKCubicResampler.Mitchell),
                InterpolationMode.HighQualityBilinear => new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear),
                _ => new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None),
            };

            /// <summary>The image's pixels as Skia draws them, the attributes applied.</summary>
            private static SKImage ImageFor(GpBitmap bitmap, GpImageAttributes? attributes)
            {
                GpImageAttributes.Adjust? adjust = attributes?.ForBitmap();
                if (adjust == null || adjust.IsIdentity)
                    return bitmap.ToSKImage();
                bitmap.EnsureScan0();
                var info = new SKImageInfo(bitmap.W, bitmap.H, SKColorType.Bgra8888, SKAlphaType.Premul);
                var pixels = new uint[bitmap.W * bitmap.H];
                for (int y = 0; y < bitmap.H; y++)
                    for (int x = 0; x < bitmap.W; x++)
                        pixels[y * bitmap.W + x] = Premultiply(GpImageAttributes.Apply(adjust, bitmap.GetArgb(x, y)));
                fixed (uint* p = pixels)
                    return SKImage.FromPixelCopy(info, (IntPtr)p, bitmap.W * 4);
            }

            /// <summary>An image's source rectangle (in pixels) drawn through a matrix to world coordinates.</summary>
            private static int DrawImageThrough(GpGraphics g, GpImage image, SKRect source, SKMatrix toWorld, GpImageAttributes? attributes)
            {
                if (source.Width == 0 || source.Height == 0)
                    return Ok;
                SKCanvas canvas = g.Begin();
                try
                {
                    canvas.Concat(in toWorld);
                    if (image is GpMetafile metafile)
                    {
                        metafile.EndRecording();
                        if (metafile.Picture != null)
                        {
                            canvas.ClipRect(source);
                            canvas.DrawPicture(metafile.Picture);
                        }
                        return Ok;
                    }
                    var bitmap = (GpBitmap)image;
                    using SKImage skImage = ImageFor(bitmap, attributes);
                    using SKPaint paint = g.NewPaint(false);
                    SKMatrix total = canvas.TotalMatrix;
                    bool unscaled = total.ScaleX == 1 && total.ScaleY == 1 && total.SkewX == 0 && total.SkewY == 0
                        && Math.Abs(total.TransX - g.DeviceOffset - Math.Round(total.TransX - g.DeviceOffset)) < 0.001
                        && Math.Abs(total.TransY - g.DeviceOffset - Math.Round(total.TransY - g.DeviceOffset)) < 0.001;
                    SKSamplingOptions sampling = unscaled ? new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None) : Sampling(g.Interpolation);
                    if (attributes != null && attributes.WrapMode != WrapMode.Clamp && !unscaled)
                    {
                        SKShaderTileMode mx = attributes.WrapMode is WrapMode.TileFlipX or WrapMode.TileFlipXY ? SKShaderTileMode.Mirror : SKShaderTileMode.Repeat;
                        SKShaderTileMode my = attributes.WrapMode is WrapMode.TileFlipY or WrapMode.TileFlipXY ? SKShaderTileMode.Mirror : SKShaderTileMode.Repeat;
                        using SKImage subset = skImage.Subset(SKRectI.Round(source)) ?? skImage;
                        paint.Shader = subset.ToShader(mx, my, sampling, SKMatrix.CreateTranslation(source.Left, source.Top));
                        canvas.DrawRect(source, paint);
                        paint.Shader.Dispose();
                    }
                    else
                    {
                        canvas.DrawImage(skImage, source, source, sampling, paint);
                    }
                    return Ok;
                }
                finally
                {
                    g.End();
                }
            }

            /// <summary>A source rectangle in a unit, in pixels (an image's resolution).</summary>
            private static bool SourceInPixels(GpImage image, GraphicsUnit unit, ref float x, ref float y, ref float width, ref float height)
            {
                float sx, sy;
                switch (unit)
                {
                    case GraphicsUnit.Pixel:
                        return true;
                    case GraphicsUnit.Point:
                    case GraphicsUnit.Inch:
                    case GraphicsUnit.Document:
                    case GraphicsUnit.Millimeter:
                        sx = UnitToPixels(unit, image.DpiX);
                        sy = UnitToPixels(unit, image.DpiY);
                        break;
                    default:
                        return false;
                }
                x *= sx; width *= sx; y *= sy; height *= sy;
                return true;
            }

            private static SKMatrix RectToRect(SKRect source, float x, float y, float width, float height) =>
                SKMatrix.Concat(SKMatrix.CreateTranslation(x, y), SKMatrix.Concat(SKMatrix.CreateScale(width / source.Width, height / source.Height), SKMatrix.CreateTranslation(-source.Left, -source.Top)));

            /// <summary>The parallelogram of three points (upper left, upper right, lower left) for a source rectangle.</summary>
            private static SKMatrix RectToPoints(SKRect source, PointF p0, PointF p1, PointF p2)
            {
                var toUnit = SKMatrix.Concat(SKMatrix.CreateScale(1 / source.Width, 1 / source.Height), SKMatrix.CreateTranslation(-source.Left, -source.Top));
                var toPoints = new SKMatrix(p1.X - p0.X, p2.X - p0.X, p0.X, p1.Y - p0.Y, p2.Y - p0.Y, p0.Y, 0, 0, 1);
                return SKMatrix.Concat(toPoints, toUnit);
            }

            private static bool Drawable(HandleRef graphics, HandleRef image, out GpGraphics g, out GpImage i, out int status)
            {
                i = null!;
                if (!Drawable(graphics, out g, out status))
                    return false;
                if (!Is(image, out GpImage? img))
                {
                    status = InvalidParameter;
                    return false;
                }
                i = img;
                return true;
            }

            /// <summary>An image's size where DrawImage at a point draws it: its physical size, in the Graphics' page units.</summary>
            private static SizeF PhysicalSize(GpGraphics g, GpImage image)
            {
                float dpiX = image is GpMetafile ? 96 : image.DpiX, dpiY = image is GpMetafile ? 96 : image.DpiY;
                return new SizeF(image.Width * g.DpiX / dpiX / g.PageScaleX, image.Height * g.DpiY / dpiY / g.PageScaleY);
            }

            internal static int GdipDrawImage(HandleRef graphics, HandleRef image, float x, float y)
            {
                if (!Drawable(graphics, image, out GpGraphics g, out GpImage i, out int status))
                    return status;
                SizeF size = PhysicalSize(g, i);
                var source = new SKRect(0, 0, i.Width, i.Height);
                return DrawImageThrough(g, i, source, RectToRect(source, x, y, size.Width, size.Height), null);
            }

            internal static int GdipDrawImageI(HandleRef graphics, HandleRef image, int x, int y) => GdipDrawImage(graphics, image, x, y);

            internal static int GdipDrawImageRect(HandleRef graphics, HandleRef image, float x, float y, float width, float height)
            {
                if (!Drawable(graphics, image, out GpGraphics g, out GpImage i, out int status))
                    return status;
                if (width == 0 || height == 0)
                    return Ok;
                var source = new SKRect(0, 0, i.Width, i.Height);
                return DrawImageThrough(g, i, source, RectToRect(source, x, y, width, height), null);
            }

            internal static int GdipDrawImageRectI(HandleRef graphics, HandleRef image, int x, int y, int width, int height) =>
                GdipDrawImageRect(graphics, image, x, y, width, height);

            internal static int GdipDrawImagePoints(HandleRef graphics, HandleRef image, PointF* points, int count)
            {
                if (!Drawable(graphics, image, out GpGraphics g, out GpImage i, out int status))
                    return status;
                if (points == null || count != 3)
                    return count == 4 ? NotImplemented : InvalidParameter;
                var source = new SKRect(0, 0, i.Width, i.Height);
                return DrawImageThrough(g, i, source, RectToPoints(source, points[0], points[1], points[2]), null);
            }

            internal static int GdipDrawImagePointsI(HandleRef graphics, HandleRef image, Point* points, int count)
            {
                if (points == null)
                    return InvalidParameter;
                fixed (PointF* p = Points(points, count))
                    return GdipDrawImagePoints(graphics, image, p, count);
            }

            internal static int GdipDrawImagePointRect(HandleRef graphics, HandleRef image, float x, float y, float srcx, float srcy, float srcwidth, float srcheight, int srcunit)
            {
                if (!Drawable(graphics, image, out GpGraphics g, out GpImage i, out int status))
                    return status;
                if (!SourceInPixels(i, (GraphicsUnit)srcunit, ref srcx, ref srcy, ref srcwidth, ref srcheight))
                    return InvalidParameter;
                var source = new SKRect(srcx, srcy, srcx + srcwidth, srcy + srcheight);
                float dpiX = i is GpMetafile ? 96 : i.DpiX, dpiY = i is GpMetafile ? 96 : i.DpiY;
                float width = srcwidth * g.DpiX / dpiX / g.PageScaleX, height = srcheight * g.DpiY / dpiY / g.PageScaleY;
                return DrawImageThrough(g, i, source, RectToRect(source, x, y, width, height), null);
            }

            internal static int GdipDrawImagePointRectI(HandleRef graphics, HandleRef image, int x, int y, int srcx, int srcy, int srcwidth, int srcheight, int srcunit) =>
                GdipDrawImagePointRect(graphics, image, x, y, srcx, srcy, srcwidth, srcheight, srcunit);

            internal static int GdipDrawImageRectRect(HandleRef graphics, HandleRef image, float dstx, float dsty, float dstwidth, float dstheight, float srcx, float srcy, float srcwidth, float srcheight, GraphicsUnit srcunit, HandleRef imageAttributes, Graphics.DrawImageAbort? callback, HandleRef callbackdata)
            {
                if (!Drawable(graphics, image, out GpGraphics g, out GpImage i, out int status))
                    return status;
                if (!SourceInPixels(i, srcunit, ref srcx, ref srcy, ref srcwidth, ref srcheight))
                    return InvalidParameter;
                if (NearZero(srcwidth) || NearZero(dstwidth) || NearZero(srcheight) || NearZero(dstheight))
                    return Ok;
                var source = new SKRect(srcx, srcy, srcx + srcwidth, srcy + srcheight);
                return DrawImageThrough(g, i, source, RectToRect(source, dstx, dsty, dstwidth, dstheight), Get<GpImageAttributes>(imageAttributes.Handle));
            }

            internal static int GdipDrawImageRectRectI(HandleRef graphics, HandleRef image, int dstx, int dsty, int dstwidth, int dstheight, int srcx, int srcy, int srcwidth, int srcheight, GraphicsUnit srcunit, HandleRef imageAttributes, Graphics.DrawImageAbort? callback, HandleRef callbackdata) =>
                GdipDrawImageRectRect(graphics, image, dstx, dsty, dstwidth, dstheight, srcx, srcy, srcwidth, srcheight, srcunit, imageAttributes, callback, callbackdata);

            internal static int GdipDrawImagePointsRect(HandleRef graphics, HandleRef image, PointF* points, int count, float srcx, float srcy, float srcwidth, float srcheight, GraphicsUnit srcunit, HandleRef imageAttributes, Graphics.DrawImageAbort? callback, HandleRef callbackdata)
            {
                if (!Drawable(graphics, image, out GpGraphics g, out GpImage i, out int status))
                    return status;
                if (points == null || count != 3)
                    return count == 4 ? NotImplemented : InvalidParameter;
                if (!SourceInPixels(i, srcunit, ref srcx, ref srcy, ref srcwidth, ref srcheight))
                    return InvalidParameter;
                var source = new SKRect(srcx, srcy, srcx + srcwidth, srcy + srcheight);
                return DrawImageThrough(g, i, source, RectToPoints(source, points[0], points[1], points[2]), Get<GpImageAttributes>(imageAttributes.Handle));
            }

            internal static int GdipDrawImagePointsRectI(HandleRef graphics, HandleRef image, Point* points, int count, int srcx, int srcy, int srcwidth, int srcheight, GraphicsUnit srcunit, HandleRef imageAttributes, Graphics.DrawImageAbort? callback, HandleRef callbackdata)
            {
                if (points == null)
                    return InvalidParameter;
                fixed (PointF* p = Points(points, count))
                    return GdipDrawImagePointsRect(graphics, image, p, count, srcx, srcy, srcwidth, srcheight, srcunit, imageAttributes, callback, callbackdata);
            }
        }
    }
}
