// WebFormsForCore: GDI+'s texture, hatch and path gradient brushes, and custom line caps. Their properties as libgdiplus
// keeps them (texturebrush.c, hatchbrush.c, pathgradientbrush.c, customlinecap.c, adjustablearrowcap.c); their drawing
// GDI+'s: a hatch is its 8x8 pattern in device pixels, a path gradient its colours from the boundary to the centre (and the
// focus) through the path's triangles, as Skia's vertices.
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
            // Texture

            internal sealed class GpTexture : GpBrush
            {
                public GpBitmap Image;
                public WrapMode WrapMode;
                public GpMatrix Matrix = new GpMatrix();

                public GpTexture(GpBitmap image, WrapMode wrapMode)
                {
                    Image = image;
                    WrapMode = wrapMode;
                }

                public override BrushType Type => BrushType.TextureFill;

                public override GpBrush Clone() => new GpTexture((GpBitmap)Image.Clone(), WrapMode) { Matrix = Matrix.Clone() };

                public override int Setup(SKPaint paint, GpGraphics graphics)
                {
                    SKShaderTileMode x = WrapMode switch
                    {
                        WrapMode.TileFlipX or WrapMode.TileFlipXY => SKShaderTileMode.Mirror,
                        WrapMode.Clamp => SKShaderTileMode.Decal,
                        _ => SKShaderTileMode.Repeat,
                    };
                    SKShaderTileMode y = WrapMode switch
                    {
                        WrapMode.TileFlipY or WrapMode.TileFlipXY => SKShaderTileMode.Mirror,
                        WrapMode.Clamp => SKShaderTileMode.Decal,
                        _ => SKShaderTileMode.Repeat,
                    };
                    using SKImage image = Image.ToSKImage();
                    paint.Color = SKColors.Black;
                    paint.Shader = image.ToShader(x, y, Sampling(graphics.Interpolation), Matrix.ToSK());
                    return Ok;
                }
            }

            private static int CreateTexture(GpImage image, WrapMode wrapMode, Rectangle? area, GpImageAttributes? attributes, out IntPtr texture)
            {
                texture = IntPtr.Zero;
                if (wrapMode > WrapMode.Clamp)
                    return OutOfMemory;
                GpBitmap bitmap;
                if (image is GpBitmap b)
                {
                    if (area is Rectangle r)
                    {
                        if (r.X < 0 || r.Y < 0 || r.Width <= 0 || r.Height <= 0 || b.W < r.Right || b.H < r.Bottom)
                            return OutOfMemory;
                        int status = GdipCloneBitmapAreaI(r.X, r.Y, r.Width, r.Height, (int)b.Format, new HandleRef(null, NewHandleTemp(b, out GCHandle h)), out IntPtr clone);
                        h.Free();
                        if (status != Ok)
                            return status;
                        bitmap = Get<GpBitmap>(clone)!;
                        GCHandle.FromIntPtr(clone).Free();
                    }
                    else
                    {
                        bitmap = (GpBitmap)b.Clone();
                    }
                }
                else
                {
                    // A metafile: drawn on a bitmap.
                    var m = (GpMetafile)image;
                    int w = area?.Width ?? (int)m.PictureSize.Width, h = area?.Height ?? (int)m.PictureSize.Height;
                    bitmap = GpBitmap.Create(Math.Max(w, 1), Math.Max(h, 1), PixelFormat.Format32bppArgb);
                    m.EndRecording();
                    if (m.Picture != null)
                    {
                        using var canvas = new SKCanvas(bitmap.EnsureSurface());
                        canvas.Scale(bitmap.W / m.PictureSize.Width, bitmap.H / m.PictureSize.Height);
                        canvas.DrawPicture(m.Picture);
                        bitmap.Drawn();
                    }
                }
                // GDI+ keeps a texture's image premultiplied (32bppPArgb), the attributes applied.
                GpImageAttributes.Adjust? adjust = attributes?.ForBitmap();
                bitmap.EnsureScan0();
                GpBitmap premultiplied = GpBitmap.Create(bitmap.W, bitmap.H, PixelFormat.Format32bppPArgb);
                for (int y = 0; y < bitmap.H; y++)
                    for (int x = 0; x < bitmap.W; x++)
                    {
                        int argb = bitmap.GetArgb(x, y);
                        premultiplied.SetArgb(x, y, adjust != null && !adjust.IsIdentity ? GpImageAttributes.Apply(adjust, argb) : argb);
                    }
                premultiplied.DpiX = bitmap.DpiX;
                premultiplied.DpiY = bitmap.DpiY;
                bitmap.Dispose();
                texture = NewHandle(new GpTexture(premultiplied, wrapMode));
                return Ok;
            }

            private static IntPtr NewHandleTemp(object value, out GCHandle handle)
            {
                handle = GCHandle.Alloc(value);
                return GCHandle.ToIntPtr(handle);
            }

            internal static int GdipCreateTexture(HandleRef bitmap, int wrapmode, out IntPtr texture)
            {
                texture = IntPtr.Zero;
                if (!Is(bitmap, out GpImage? i))
                    return InvalidParameter;
                return CreateTexture(i, (WrapMode)wrapmode, null, null, out texture);
            }

            internal static int GdipCreateTexture2I(HandleRef bitmap, int wrapmode, int x, int y, int width, int height, out IntPtr texture)
            {
                texture = IntPtr.Zero;
                if (!Is(bitmap, out GpImage? i))
                    return InvalidParameter;
                return CreateTexture(i, (WrapMode)wrapmode, new Rectangle(x, y, width, height), null, out texture);
            }

            internal static int GdipCreateTexture2(HandleRef bitmap, int wrapmode, float x, float y, float width, float height, out IntPtr texture) =>
                GdipCreateTexture2I(bitmap, wrapmode, (int)x, (int)y, (int)width, (int)height, out texture);

            internal static int GdipCreateTextureIAI(HandleRef bitmap, HandleRef imageAttrib, int x, int y, int width, int height, out IntPtr texture)
            {
                texture = IntPtr.Zero;
                if (!Is(bitmap, out GpImage? i))
                    return InvalidParameter;
                GpImageAttributes? attributes = Get<GpImageAttributes>(imageAttrib.Handle);
                WrapMode mode = attributes?.WrapMode ?? WrapMode.Tile;
                return CreateTexture(i, mode, new Rectangle(x, y, width, height), attributes, out texture);
            }

            internal static int GdipCreateTextureIA(HandleRef bitmap, HandleRef imageAttrib, float x, float y, float width, float height, out IntPtr texture) =>
                GdipCreateTextureIAI(bitmap, imageAttrib, (int)x, (int)y, (int)width, (int)height, out texture);

            internal static int GdipGetTextureTransform(HandleRef brush, HandleRef matrix)
            {
                if (!Is(brush, out GpTexture? t) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                m.CopyFrom(t.Matrix);
                return Ok;
            }

            internal static int GdipSetTextureTransform(HandleRef brush, HandleRef matrix)
            {
                if (!Is(brush, out GpTexture? t) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                if (m.HasBoundaryValues || !m.Clone().Invert())
                    return InvalidParameter;
                t.Matrix.CopyFrom(m);
                return Ok;
            }

            internal static int GdipResetTextureTransform(HandleRef brush)
            {
                if (!Is(brush, out GpTexture? t))
                    return InvalidParameter;
                t.Matrix = new GpMatrix();
                return Ok;
            }

            internal static int GdipMultiplyTextureTransform(HandleRef brush, HandleRef matrix, MatrixOrder order) =>
                Is(brush, out GpTexture? t) ? MultiplyTransform(t.Matrix, matrix, order) : InvalidParameter;

            internal static int GdipTranslateTextureTransform(HandleRef brush, float dx, float dy, MatrixOrder order) =>
                Is(brush, out GpTexture? t) ? (t.Matrix.Multiply(GpMatrix.Translation(dx, dy), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipScaleTextureTransform(HandleRef brush, float sx, float sy, MatrixOrder order) =>
                Is(brush, out GpTexture? t) ? (t.Matrix.Multiply(GpMatrix.Scaling(sx, sy), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipRotateTextureTransform(HandleRef brush, float angle, MatrixOrder order) =>
                Is(brush, out GpTexture? t) ? (t.Matrix.Multiply(GpMatrix.Rotation(angle * DegToRad), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipSetTextureWrapMode(HandleRef brush, int wrapMode)
            {
                if (!Is(brush, out GpTexture? t))
                    return InvalidParameter;
                if (wrapMode > (int)WrapMode.Clamp)
                    return Ok;
                t.WrapMode = (WrapMode)wrapMode;
                return Ok;
            }

            internal static int GdipGetTextureWrapMode(HandleRef brush, out int wrapMode)
            {
                wrapMode = 0;
                if (!Is(brush, out GpTexture? t))
                    return InvalidParameter;
                wrapMode = (int)t.WrapMode;
                return Ok;
            }

            internal static int GdipGetTextureImage(HandleRef brush, out IntPtr image)
            {
                image = IntPtr.Zero;
                if (!Is(brush, out GpTexture? t))
                    return InvalidParameter;
                image = NewHandle(t.Image.Clone());
                return Ok;
            }

            // Hatch

            /// <summary>GDI+'s 53 hatch patterns, 8x8, a row a byte (the leftmost pixel the high bit).</summary>
            private static readonly byte[][] s_hatches =
            {
                new byte[] { 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, // Horizontal
                new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 }, // Vertical
                new byte[] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 }, // ForwardDiagonal
                new byte[] { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 }, // BackwardDiagonal
                new byte[] { 0xff, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 }, // Cross
                new byte[] { 0x81, 0x42, 0x24, 0x18, 0x18, 0x24, 0x42, 0x81 }, // DiagonalCross
                new byte[] { 0x80, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00 }, // Percent05
                new byte[] { 0x80, 0x00, 0x08, 0x00, 0x80, 0x00, 0x08, 0x00 }, // Percent10
                new byte[] { 0x88, 0x00, 0x22, 0x00, 0x88, 0x00, 0x22, 0x00 }, // Percent20
                new byte[] { 0x88, 0x22, 0x88, 0x22, 0x88, 0x22, 0x88, 0x22 }, // Percent25
                new byte[] { 0xaa, 0x44, 0xaa, 0x11, 0xaa, 0x44, 0xaa, 0x11 }, // Percent30
                new byte[] { 0xaa, 0x55, 0xaa, 0x51, 0xaa, 0x55, 0xaa, 0x15 }, // Percent40
                new byte[] { 0xaa, 0x55, 0xaa, 0x55, 0xaa, 0x55, 0xaa, 0x55 }, // Percent50
                new byte[] { 0xae, 0x55, 0xea, 0x55, 0xae, 0x55, 0xea, 0x55 }, // Percent60
                new byte[] { 0x55, 0xbb, 0x55, 0xee, 0x55, 0xbb, 0x55, 0xee }, // Percent70
                new byte[] { 0x77, 0xdd, 0x77, 0xdd, 0x77, 0xdd, 0x77, 0xdd }, // Percent75
                new byte[] { 0x77, 0xff, 0xdd, 0xff, 0x77, 0xff, 0xdd, 0xff }, // Percent80
                new byte[] { 0x7f, 0xff, 0xf7, 0xff, 0x7f, 0xff, 0xf7, 0xff }, // Percent90
                new byte[] { 0x88, 0x44, 0x22, 0x11, 0x88, 0x44, 0x22, 0x11 }, // LightDownwardDiagonal
                new byte[] { 0x11, 0x22, 0x44, 0x88, 0x11, 0x22, 0x44, 0x88 }, // LightUpwardDiagonal
                new byte[] { 0xcc, 0x66, 0x33, 0x99, 0xcc, 0x66, 0x33, 0x99 }, // DarkDownwardDiagonal
                new byte[] { 0x33, 0x66, 0xcc, 0x99, 0x33, 0x66, 0xcc, 0x99 }, // DarkUpwardDiagonal
                new byte[] { 0xc1, 0xe0, 0x70, 0x38, 0x1c, 0x0e, 0x07, 0x83 }, // WideDownwardDiagonal
                new byte[] { 0x83, 0x07, 0x0e, 0x1c, 0x38, 0x70, 0xe0, 0xc1 }, // WideUpwardDiagonal
                new byte[] { 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88 }, // LightVertical
                new byte[] { 0xff, 0x00, 0x00, 0x00, 0xff, 0x00, 0x00, 0x00 }, // LightHorizontal
                new byte[] { 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa, 0xaa }, // NarrowVertical
                new byte[] { 0xff, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x00 }, // NarrowHorizontal
                new byte[] { 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc, 0xcc }, // DarkVertical
                new byte[] { 0xff, 0xff, 0x00, 0x00, 0xff, 0xff, 0x00, 0x00 }, // DarkHorizontal
                new byte[] { 0x00, 0x00, 0x88, 0x44, 0x22, 0x11, 0x00, 0x00 }, // DashedDownwardDiagonal
                new byte[] { 0x00, 0x00, 0x11, 0x22, 0x44, 0x88, 0x00, 0x00 }, // DashedUpwardDiagonal
                new byte[] { 0xf0, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00 }, // DashedHorizontal
                new byte[] { 0x80, 0x80, 0x80, 0x80, 0x08, 0x08, 0x08, 0x08 }, // DashedVertical
                new byte[] { 0x80, 0x08, 0x40, 0x02, 0x10, 0x01, 0x20, 0x04 }, // SmallConfetti
                new byte[] { 0xb1, 0x30, 0x03, 0x1b, 0xd8, 0xc0, 0x0c, 0x8d }, // LargeConfetti
                new byte[] { 0x81, 0x42, 0x24, 0x18, 0x81, 0x42, 0x24, 0x18 }, // ZigZag
                new byte[] { 0x00, 0x18, 0xa4, 0x03, 0x00, 0x18, 0xa4, 0x03 }, // Wave
                new byte[] { 0x01, 0x02, 0x04, 0x08, 0x18, 0x24, 0x42, 0x81 }, // DiagonalBrick
                new byte[] { 0xff, 0x80, 0x80, 0x80, 0xff, 0x08, 0x08, 0x08 }, // HorizontalBrick
                new byte[] { 0x88, 0x54, 0x22, 0x45, 0x88, 0x14, 0x22, 0x51 }, // Weave
                new byte[] { 0xaa, 0x55, 0xaa, 0x55, 0xf0, 0xf0, 0xf0, 0xf0 }, // Plaid
                new byte[] { 0x80, 0x40, 0x80, 0x00, 0x08, 0x04, 0x08, 0x00 }, // Divot
                new byte[] { 0xaa, 0x00, 0x80, 0x00, 0x80, 0x00, 0x80, 0x00 }, // DottedGrid
                new byte[] { 0x80, 0x00, 0x22, 0x00, 0x08, 0x00, 0x22, 0x00 }, // DottedDiamond
                new byte[] { 0x03, 0x84, 0x48, 0x30, 0x0c, 0x02, 0x01, 0x01 }, // Shingle
                new byte[] { 0xff, 0x66, 0xff, 0x99, 0xff, 0x66, 0xff, 0x99 }, // Trellis
                new byte[] { 0x77, 0x89, 0x8f, 0x8f, 0x77, 0x98, 0xf8, 0xf8 }, // Sphere
                new byte[] { 0xff, 0x88, 0x88, 0x88, 0xff, 0x88, 0x88, 0x88 }, // SmallGrid
                new byte[] { 0x99, 0x66, 0x66, 0x99, 0x99, 0x66, 0x66, 0x99 }, // SmallCheckerBoard
                new byte[] { 0xf0, 0xf0, 0xf0, 0xf0, 0x0f, 0x0f, 0x0f, 0x0f }, // LargeCheckerBoard
                new byte[] { 0x41, 0x22, 0x14, 0x08, 0x14, 0x22, 0x41, 0x80 }, // OutlinedDiamond
                new byte[] { 0x10, 0x38, 0x7c, 0xfe, 0x7c, 0x38, 0x10, 0x00 }, // SolidDiamond
            };

            internal sealed class GpHatch : GpBrush
            {
                public HatchStyle Style;
                public int ForeColor, BackColor;

                public override BrushType Type => BrushType.HatchFill;

                public override GpBrush Clone() => new GpHatch { Style = Style, ForeColor = ForeColor, BackColor = BackColor };

                public override int Setup(SKPaint paint, GpGraphics graphics)
                {
                    byte[] rows = s_hatches[(int)Style];
                    var info = new SKImageInfo(8, 8, SKColorType.Bgra8888, SKAlphaType.Premul);
                    var pixels = new uint[64];
                    uint fore = Premultiply(ForeColor), back = Premultiply(BackColor);
                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < 8; x++)
                            pixels[y * 8 + x] = (rows[y] & (0x80 >> x)) != 0 ? fore : back;
                    SKImage image;
                    fixed (uint* p = pixels)
                        image = SKImage.FromPixelCopy(info, (IntPtr)p, 32);
                    // The pattern in device pixels from the rendering origin, whatever the transform.
                    SKMatrix local = SKMatrix.CreateTranslation(graphics.RenderOriginX, graphics.RenderOriginY);
                    if (graphics.DeviceMatrix.TryInvert(out SKMatrix inverse))
                        local = SKMatrix.Concat(inverse, local);
                    paint.Color = SKColors.Black;
                    paint.Shader = image.ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat, new SKSamplingOptions(SKFilterMode.Nearest, SKMipmapMode.None), local);
                    image.Dispose();
                    return Ok;
                }
            }

            internal static int GdipCreateHatchBrush(int hatchstyle, int forecol, int backcol, out IntPtr brush)
            {
                brush = IntPtr.Zero;
                if (hatchstyle < (int)HatchStyle.Min || hatchstyle > (int)HatchStyle.SolidDiamond)   // HatchStyle.Max is LargeGrid (4)
                    return InvalidParameter;
                brush = NewHandle(new GpHatch { Style = (HatchStyle)hatchstyle, ForeColor = forecol, BackColor = backcol });
                return Ok;
            }

            internal static int GdipGetHatchStyle(HandleRef brush, out int hatchstyle)
            {
                hatchstyle = 0;
                if (!Is(brush, out GpHatch? h))
                    return InvalidParameter;
                hatchstyle = (int)h.Style;
                return Ok;
            }

            internal static int GdipGetHatchForegroundColor(HandleRef brush, out int forecol)
            {
                forecol = 0;
                if (!Is(brush, out GpHatch? h))
                    return InvalidParameter;
                forecol = h.ForeColor;
                return Ok;
            }

            internal static int GdipGetHatchBackgroundColor(HandleRef brush, out int backcol)
            {
                backcol = 0;
                if (!Is(brush, out GpHatch? h))
                    return InvalidParameter;
                backcol = h.BackColor;
                return Ok;
            }

            // Path gradient

            internal sealed class GpPathGradient : GpBrush
            {
                public GpPath Boundary = new GpPath(FillMode.Alternate);
                public int[] SurroundColors = { unchecked((int)0xFFFFFFFF) };
                public PointF Center;
                public int CenterColor;
                public PointF FocusScales;
                public RectangleF Rectangle;
                public float[] BlendFactors = { 1.0f };
                public float[] BlendPositions = { 0.0f };
                public int[] PresetColors = Array.Empty<int>();
                public float[] PresetPositions = Array.Empty<float>();
                public WrapMode WrapMode = WrapMode.Clamp;
                public GpMatrix Transform = new GpMatrix();
                public bool GammaCorrection;

                public override BrushType Type => BrushType.PathGradient;

                public override GpBrush Clone()
                {
                    var b = (GpPathGradient)MemberwiseClone();
                    b.Boundary = Boundary.Clone();
                    b.SurroundColors = (int[])SurroundColors.Clone();
                    b.BlendFactors = (float[])BlendFactors.Clone();
                    b.BlendPositions = (float[])BlendPositions.Clone();
                    b.PresetColors = (int[])PresetColors.Clone();
                    b.PresetPositions = (float[])PresetPositions.Clone();
                    b.Transform = Transform.Clone();
                    return b;
                }

                /// <summary>The colour at a distance from the boundary (0) to the centre (1), for a boundary colour.</summary>
                private SKColor ColorAt(float t, int surround)
                {
                    if (PresetColors.Length > 1)
                    {
                        int i = 0;
                        while (i < PresetPositions.Length - 1 && PresetPositions[i + 1] < t) i++;
                        int j = Math.Min(i + 1, PresetColors.Length - 1);
                        float span = PresetPositions[j] - PresetPositions[i];
                        float f = span > 0 ? Math.Clamp((t - PresetPositions[i]) / span, 0, 1) : 0;
                        return Lerp(PresetColors[i], PresetColors[j], f);
                    }
                    float factor = t;
                    if (BlendFactors.Length > 1)
                    {
                        int i = 0;
                        while (i < BlendPositions.Length - 1 && BlendPositions[i + 1] < t) i++;
                        int j = Math.Min(i + 1, BlendFactors.Length - 1);
                        float span = BlendPositions[j] - BlendPositions[i];
                        float f = span > 0 ? Math.Clamp((t - BlendPositions[i]) / span, 0, 1) : 0;
                        factor = BlendFactors[i] + (BlendFactors[j] - BlendFactors[i]) * f;
                    }
                    return Lerp(surround, CenterColor, factor);
                }

                private static SKColor Lerp(int a, int b, float f)
                {
                    int Channel(int shift) => (int)Math.Round(((a >> shift) & 0xff) * (1 - f) + ((b >> shift) & 0xff) * f);
                    return new SKColor((byte)Channel(16), (byte)Channel(8), (byte)Channel(0), (byte)Channel(24));
                }

                public override int Setup(SKPaint paint, GpGraphics graphics)
                {
                    GpPath flat = Boundary.Clone();
                    flat.Flatten(null, 0.25f);
                    int n = flat.Count;
                    if (n < 2)
                    {
                        paint.Color = ToSKColor(CenterColor);
                        return Ok;
                    }
                    const int rings = 32;
                    var positions = new List<SKPoint>();
                    var colors = new List<SKColor>();
                    PointF c = Center;
                    float fx = FocusScales.X, fy = FocusScales.Y;
                    SKPoint Ring(PointF p, float s) => new SKPoint(
                        c.X + (p.X - c.X) * (1 - s * (1 - fx)),
                        c.Y + (p.Y - c.Y) * (1 - s * (1 - fy)));
                    int Surround(int i) => SurroundColors.Length == 0 ? unchecked((int)0xFFFFFFFF) : SurroundColors[Math.Min(i, SurroundColors.Length - 1)];
                    for (int i = 0; i < n; i++)
                    {
                        int j = (i + 1) % n;
                        PointF pi = flat.Points[i], pj = flat.Points[j];
                        int ci = Surround(i), cj = Surround(j);
                        for (int k = 0; k < rings; k++)
                        {
                            float s0 = k / (float)rings, s1 = (k + 1) / (float)rings;
                            SKPoint a0 = Ring(pi, s0), b0 = Ring(pj, s0), a1 = Ring(pi, s1), b1 = Ring(pj, s1);
                            SKColor ca0 = ColorAt(s0, ci), cb0 = ColorAt(s0, cj), ca1 = ColorAt(s1, ci), cb1 = ColorAt(s1, cj);
                            positions.Add(a0); colors.Add(ca0);
                            positions.Add(b0); colors.Add(cb0);
                            positions.Add(a1); colors.Add(ca1);
                            positions.Add(b0); colors.Add(cb0);
                            positions.Add(b1); colors.Add(cb1);
                            positions.Add(a1); colors.Add(ca1);
                        }
                        // The focus: the centre's colour.
                        if (fx > 0 || fy > 0)
                        {
                            positions.Add(Ring(pi, 1)); colors.Add(ColorAt(1, ci));
                            positions.Add(Ring(pj, 1)); colors.Add(ColorAt(1, cj));
                            positions.Add(new SKPoint(c.X, c.Y)); colors.Add(ColorAt(1, ci));
                        }
                    }
                    SKRect bounds = new SKRect(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
                    if (bounds.Width <= 0 || bounds.Height <= 0)
                        bounds = new SKRect(c.X - 1, c.Y - 1, c.X + 1, c.Y + 1);
                    using var recorder = new SKPictureRecorder();
                    SKCanvas canvas = recorder.BeginRecording(bounds);
                    using (var vertices = SKVertices.CreateCopy(SKVertexMode.Triangles, positions.ToArray(), colors.ToArray()))
                    using (var white = new SKPaint { Color = SKColors.White, IsAntialias = false })
                        canvas.DrawVertices(vertices, SKBlendMode.Modulate, white);
                    using SKPicture picture = recorder.EndRecording();
                    SKShaderTileMode x = WrapMode switch { WrapMode.Clamp => SKShaderTileMode.Decal, WrapMode.TileFlipX or WrapMode.TileFlipXY => SKShaderTileMode.Mirror, _ => SKShaderTileMode.Repeat };
                    SKShaderTileMode y = WrapMode switch { WrapMode.Clamp => SKShaderTileMode.Decal, WrapMode.TileFlipY or WrapMode.TileFlipXY => SKShaderTileMode.Mirror, _ => SKShaderTileMode.Repeat };
                    paint.Color = SKColors.Black;
                    paint.Shader = picture.ToShader(x, y, SKFilterMode.Linear, Transform.ToSK(), bounds);
                    return Ok;
                }
            }

            private static void ExpandBy(ref RectangleF rect, PointF point)
            {
                float x0 = rect.X, y0 = rect.Y, x1 = x0 + rect.Width, y1 = y0 + rect.Height;
                if (point.X < x0) x0 = point.X; else if (point.X > x1) x1 = point.X;
                if (point.Y < y0) y0 = point.Y; else if (point.Y > y1) y1 = point.Y;
                rect = new RectangleF(x0, y0, x1 - x0, y1 - y0);
            }

            private static PointF CenterOf(List<PointF> points, int count)
            {
                float x = 0, y = 0;
                for (int i = 0; i < count; i++) { x += points[i].X; y += points[i].Y; }
                return new PointF(x / count, y / count);
            }

            internal static int GdipCreatePathGradient(PointF* points, int count, WrapMode wrapMode, out IntPtr brush)
            {
                brush = IntPtr.Zero;
                if (points == null || count < 2 || wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
                    return OutOfMemory;
                var gp = new GpPathGradient { WrapMode = wrapMode, CenterColor = unchecked((int)0xFF000000) };
                for (int i = 0; i < count; i++)
                    gp.Boundary.Append(points[i], PathPointTypeLine, i == 0);
                gp.Center = CenterOf(gp.Boundary.Points, gp.Boundary.Count);
                var rect = new RectangleF(gp.Boundary.Points[0], SizeF.Empty);
                for (int i = 1; i < gp.Boundary.Count; i++)
                    ExpandBy(ref rect, gp.Boundary.Points[i]);
                if (rect.Width == 0 || rect.Height == 0)
                    return OutOfMemory;
                gp.Rectangle = rect;
                brush = NewHandle(gp);
                return Ok;
            }

            internal static int GdipCreatePathGradientI(Point* points, int count, WrapMode wrapMode, out IntPtr brush)
            {
                brush = IntPtr.Zero;
                if (points == null)
                    return InvalidParameter;
                if (count < 0)
                    return OutOfMemory;
                fixed (PointF* p = Points(points, count))
                    return GdipCreatePathGradient(p, count, wrapMode, out brush);
            }

            internal static int GdipCreatePathGradientFromPath(HandleRef path, out IntPtr brush)
            {
                brush = IntPtr.Zero;
                if (!Is(path, out GpPath? p) || p.Count < 2)
                    return OutOfMemory;
                var gp = new GpPathGradient { Boundary = p.Clone(), CenterColor = unchecked((int)0xFFFFFFFF) };
                gp.Center = CenterOf(p.Points, p.Count);
                var rect = new RectangleF(p.Points[0], SizeF.Empty);
                for (int i = 1; i < p.Count; i++)
                    ExpandBy(ref rect, p.Points[i]);
                gp.Rectangle = rect;
                brush = NewHandle(gp);
                return Ok;
            }

            internal static int GdipGetPathGradientCenterColor(HandleRef brush, out int color)
            {
                color = 0;
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                color = b.CenterColor;
                return Ok;
            }

            internal static int GdipSetPathGradientCenterColor(HandleRef brush, int color)
            {
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                b.CenterColor = color;
                return Ok;
            }

            internal static int GdipGetPathGradientSurroundColorsWithCount(HandleRef brush, int[] color, ref int count)
            {
                if (!Is(brush, out GpPathGradient? b) || color == null || count < b.Boundary.Count)
                    return InvalidParameter;
                for (int i = 0; i < b.Boundary.Count; i++)
                    color[i] = i < b.SurroundColors.Length ? b.SurroundColors[i] : b.SurroundColors[^1];
                count = b.SurroundColors.Length;
                return Ok;
            }

            internal static int GdipSetPathGradientSurroundColorsWithCount(HandleRef brush, int[] argb, ref int count)
            {
                if (!Is(brush, out GpPathGradient? b) || argb == null)
                    return InvalidParameter;
                int n = count;
                if (n < 1 || n > b.Boundary.Count)
                    return InvalidParameter;
                bool equal = true;
                for (int i = 1; i < n; i++)
                    if (argb[i] != argb[i - 1])
                        equal = false;
                if (n > 1 && equal)
                    n = 1;
                b.SurroundColors = argb.AsSpan(0, n).ToArray();
                return Ok;
            }

            internal static int GdipGetPathGradientCenterPoint(HandleRef brush, out PointF point)
            {
                point = default;
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                point = b.Center;
                return Ok;
            }

            internal static int GdipSetPathGradientCenterPoint(HandleRef brush, ref PointF point)
            {
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                b.Center = point;
                return Ok;
            }

            internal static int GdipGetPathGradientRect(HandleRef brush, out RectangleF gprectf)
            {
                gprectf = default;
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                gprectf = b.Rectangle;
                return Ok;
            }

            internal static int GdipGetPathGradientPointCount(HandleRef brush, out int count)
            {
                count = 0;
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                count = b.Boundary.Count;
                return Ok;
            }

            internal static int GdipGetPathGradientSurroundColorCount(HandleRef brush, out int count) => GdipGetPathGradientPointCount(brush, out count);

            internal static int GdipGetPathGradientBlendCount(HandleRef brush, out int count)
            {
                count = 0;
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                count = b.BlendFactors.Length;
                return Ok;
            }

            internal static int GdipGetPathGradientBlend(HandleRef brush, float[] blend, float[] positions, int count)
            {
                if (!Is(brush, out GpPathGradient? b) || blend == null || positions == null || count <= 0)
                    return InvalidParameter;
                if (count < b.BlendFactors.Length)
                    return InsufficientBuffer;
                Array.Copy(b.BlendFactors, blend, b.BlendFactors.Length);
                if (b.BlendFactors.Length > 1)
                    Array.Copy(b.BlendPositions, positions, b.BlendPositions.Length);
                return Ok;
            }

            internal static int GdipSetPathGradientBlend(HandleRef brush, IntPtr blend, IntPtr positions, int count)
            {
                if (!Is(brush, out GpPathGradient? b) || blend == IntPtr.Zero || positions == IntPtr.Zero || count <= 0)
                    return InvalidParameter;
                float* p = (float*)positions;
                if (count >= 2 && (p[0] != 0.0f || p[count - 1] != 1.0f))
                    return InvalidParameter;
                b.BlendFactors = new ReadOnlySpan<float>((float*)blend, count).ToArray();
                b.BlendPositions = new ReadOnlySpan<float>(p, count).ToArray();
                b.PresetColors = Array.Empty<int>();
                b.PresetPositions = Array.Empty<float>();
                return Ok;
            }

            internal static int GdipGetPathGradientPresetBlendCount(HandleRef brush, out int count)
            {
                count = 0;
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                count = b.PresetColors.Length;
                return Ok;
            }

            internal static int GdipGetPathGradientPresetBlend(HandleRef brush, int[] blend, float[] positions, int count)
            {
                if (!Is(brush, out GpPathGradient? b) || blend == null)
                    return InvalidParameter;
                if (count < 0)
                    return OutOfMemory;
                if (positions == null || count < 2)
                    return InvalidParameter;
                if (b.PresetColors.Length == 0)
                    return GenericError;
                if (b.PresetColors.Length != count)
                    return InvalidParameter;
                Array.Copy(b.PresetColors, blend, count);
                Array.Copy(b.PresetPositions, positions, count);
                return Ok;
            }

            internal static int GdipSetPathGradientPresetBlend(HandleRef brush, int[] blend, float[] positions, int count)
            {
                if (!Is(brush, out GpPathGradient? b) || blend == null || positions == null || count < 2 || positions[0] != 0.0f || positions[count - 1] != 1.0f)
                    return InvalidParameter;
                b.PresetColors = blend.AsSpan(0, count).ToArray();
                b.PresetPositions = positions.AsSpan(0, count).ToArray();
                b.BlendFactors = Array.Empty<float>();
                b.BlendPositions = Array.Empty<float>();
                return Ok;
            }

            internal static int GdipSetPathGradientSigmaBlend(HandleRef brush, float focus, float scale)
            {
                if (!Is(brush, out GpPathGradient? b) || focus < 0 || focus > 1 || scale < 0 || scale > 1)
                    return InvalidParameter;
                SigmaBlend(focus, scale, out b.BlendFactors, out b.BlendPositions);
                b.PresetColors = new[] { 0 };
                b.PresetPositions = new[] { 0f };
                return Ok;
            }

            internal static int GdipSetPathGradientLinearBlend(HandleRef brush, float focus, float scale)
            {
                if (!Is(brush, out GpPathGradient? b) || focus < 0 || focus > 1 || scale < 0 || scale > 1)
                    return InvalidParameter;
                if (focus == 0)
                {
                    b.BlendPositions = new[] { focus, 1f };
                    b.BlendFactors = new[] { scale, 0f };
                }
                else if (focus == 1)
                {
                    b.BlendPositions = new[] { 0f, focus };
                    b.BlendFactors = new[] { 0f, scale };
                }
                else
                {
                    b.BlendPositions = new[] { 0f, focus, 1f };
                    b.BlendFactors = new[] { 0f, scale, 0f };
                }
                b.PresetColors = new[] { 0 };
                b.PresetPositions = new[] { 0f };
                return Ok;
            }

            internal static int GdipGetPathGradientWrapMode(HandleRef brush, out int wrapmode)
            {
                wrapmode = 0;
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                wrapmode = (int)b.WrapMode;
                return Ok;
            }

            internal static int GdipSetPathGradientWrapMode(HandleRef brush, int wrapmode)
            {
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                if (wrapmode > (int)WrapMode.Clamp)
                    return Ok;
                b.WrapMode = (WrapMode)wrapmode;
                return Ok;
            }

            internal static int GdipGetPathGradientTransform(HandleRef brush, HandleRef matrix)
            {
                if (!Is(brush, out GpPathGradient? b) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                m.CopyFrom(b.Transform);
                return Ok;
            }

            internal static int GdipSetPathGradientTransform(HandleRef brush, HandleRef matrix)
            {
                if (!Is(brush, out GpPathGradient? b) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                if (m.HasBoundaryValues || !m.Clone().Invert())
                    return InvalidParameter;
                b.Transform.CopyFrom(m);
                return Ok;
            }

            internal static int GdipResetPathGradientTransform(HandleRef brush)
            {
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                b.Transform = new GpMatrix();
                return Ok;
            }

            internal static int GdipMultiplyPathGradientTransform(HandleRef brush, HandleRef matrix, MatrixOrder order) =>
                Is(brush, out GpPathGradient? b) ? MultiplyTransform(b.Transform, matrix, order) : InvalidParameter;

            internal static int GdipTranslatePathGradientTransform(HandleRef brush, float dx, float dy, MatrixOrder order) =>
                Is(brush, out GpPathGradient? b) ? (b.Transform.Multiply(GpMatrix.Translation(dx, dy), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipScalePathGradientTransform(HandleRef brush, float sx, float sy, MatrixOrder order) =>
                Is(brush, out GpPathGradient? b) ? (b.Transform.Multiply(GpMatrix.Scaling(sx, sy), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipRotatePathGradientTransform(HandleRef brush, float angle, MatrixOrder order) =>
                Is(brush, out GpPathGradient? b) ? (b.Transform.Multiply(GpMatrix.Rotation(angle * DegToRad), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipGetPathGradientFocusScales(HandleRef brush, float[] xScale, float[] yScale)
            {
                if (!Is(brush, out GpPathGradient? b) || xScale == null || yScale == null)
                    return InvalidParameter;
                xScale[0] = b.FocusScales.X;
                yScale[0] = b.FocusScales.Y;
                return Ok;
            }

            internal static int GdipSetPathGradientFocusScales(HandleRef brush, float xScale, float yScale)
            {
                if (!Is(brush, out GpPathGradient? b))
                    return InvalidParameter;
                b.FocusScales = new PointF(xScale, yScale);
                return Ok;
            }

            // Custom line caps

            internal static int GdipCreateCustomLineCap(HandleRef fillpath, HandleRef strokepath, LineCap baseCap, float baseInset, out IntPtr customCap)
            {
                customCap = IntPtr.Zero;
                GpPath? fill = Get<GpPath>(fillpath.Handle), stroke = Get<GpPath>(strokepath.Handle);
                if (fill == null && stroke == null)
                    return InvalidParameter;
                if (fill != null && stroke != null)
                    return NotImplemented;   // GDI+: a cap of one path, a fill's or a stroke's
                customCap = NewHandle(new GpCustomLineCap
                {
                    Type = CustomLineCapType.Default,
                    FillPath = fill?.Clone(),
                    StrokePath = stroke?.Clone(),
                    BaseCap = baseCap >= LineCap.Flat && baseCap <= LineCap.Triangle ? baseCap : LineCap.Flat,
                    BaseInset = baseInset,
                });
                return Ok;
            }

            internal static int GdipDeleteCustomLineCap(IntPtr customCap) => Delete<GpCustomLineCap>(customCap);

            internal static int GdipDeleteCustomLineCap(HandleRef customCap) => Delete<GpCustomLineCap>(customCap.Handle);

            internal static int GdipCloneCustomLineCap(HandleRef customCap, out IntPtr clonedCap)
            {
                clonedCap = IntPtr.Zero;
                if (!Is(customCap, out GpCustomLineCap? c))
                    return InvalidParameter;
                clonedCap = NewHandle(c.Clone());
                return Ok;
            }

            internal static int GdipGetCustomLineCapType(IntPtr customCap, out CustomLineCapType capType)
            {
                capType = 0;
                if (!Is(customCap, out GpCustomLineCap? c))
                    return InvalidParameter;
                capType = c.Type;
                return Ok;
            }

            internal static int GdipSetCustomLineCapStrokeCaps(HandleRef customCap, LineCap startCap, LineCap endCap)
            {
                if (!Is(customCap, out GpCustomLineCap? c) || startCap < LineCap.Flat || startCap > LineCap.Triangle || endCap < LineCap.Flat || endCap > LineCap.Triangle)
                    return InvalidParameter;
                c.StartCap = startCap;
                c.EndCap = endCap;
                return Ok;
            }

            internal static int GdipGetCustomLineCapStrokeCaps(HandleRef customCap, out LineCap startCap, out LineCap endCap)
            {
                startCap = endCap = 0;
                if (!Is(customCap, out GpCustomLineCap? c))
                    return InvalidParameter;
                startCap = c.StartCap;
                endCap = c.EndCap;
                return Ok;
            }

            internal static int GdipSetCustomLineCapStrokeJoin(HandleRef customCap, LineJoin lineJoin)
            {
                if (!Is(customCap, out GpCustomLineCap? c))
                    return InvalidParameter;
                c.StrokeJoin = lineJoin;
                return Ok;
            }

            internal static int GdipGetCustomLineCapStrokeJoin(HandleRef customCap, out LineJoin lineJoin)
            {
                lineJoin = 0;
                if (!Is(customCap, out GpCustomLineCap? c))
                    return InvalidParameter;
                lineJoin = c.StrokeJoin;
                return Ok;
            }

            internal static int GdipSetCustomLineCapBaseCap(HandleRef customCap, LineCap baseCap)
            {
                if (!Is(customCap, out GpCustomLineCap? c) || baseCap > LineCap.Triangle)
                    return InvalidParameter;
                c.BaseCap = baseCap;
                return Ok;
            }

            internal static int GdipGetCustomLineCapBaseCap(HandleRef customCap, out LineCap baseCap)
            {
                baseCap = 0;
                if (!Is(customCap, out GpCustomLineCap? c))
                    return InvalidParameter;
                baseCap = c.BaseCap;
                return Ok;
            }

            internal static int GdipSetCustomLineCapBaseInset(HandleRef customCap, float inset)
            {
                if (!Is(customCap, out GpCustomLineCap? c))
                    return InvalidParameter;
                c.BaseInset = inset;
                return Ok;
            }

            internal static int GdipGetCustomLineCapBaseInset(HandleRef customCap, out float inset)
            {
                inset = 0;
                if (!Is(customCap, out GpCustomLineCap? c))
                    return InvalidParameter;
                inset = c.BaseInset;
                return Ok;
            }

            internal static int GdipSetCustomLineCapWidthScale(HandleRef customCap, float widthScale)
            {
                if (!Is(customCap, out GpCustomLineCap? c))
                    return InvalidParameter;
                c.WidthScale = widthScale;
                return Ok;
            }

            internal static int GdipGetCustomLineCapWidthScale(HandleRef customCap, out float widthScale)
            {
                widthScale = 0;
                if (!Is(customCap, out GpCustomLineCap? c))
                    return InvalidParameter;
                widthScale = c.WidthScale;
                return Ok;
            }

            // Adjustable arrow caps

            private static void UpdateArrowCap(GpCustomLineCap cap)
            {
                cap.BaseInset = cap.ArrowWidth != 0 ? cap.Height / cap.ArrowWidth : 0;
                cap.BaseCap = LineCap.Triangle;
            }

            internal static int GdipCreateAdjustableArrowCap(float height, float width, bool isFilled, out IntPtr adjustableArrowCap)
            {
                var cap = new GpCustomLineCap { Type = CustomLineCapType.AdjustableArrowCap, FillState = isFilled, ArrowWidth = width, Height = height };
                UpdateArrowCap(cap);
                adjustableArrowCap = NewHandle(cap);
                return Ok;
            }

            private static bool IsArrow(HandleRef cap, out GpCustomLineCap c)
            {
                bool ok = Is(cap, out GpCustomLineCap? found) && found.Type == CustomLineCapType.AdjustableArrowCap;
                c = found!;
                return ok;
            }

            internal static int GdipSetAdjustableArrowCapHeight(HandleRef adjustableArrowCap, float height)
            {
                if (!IsArrow(adjustableArrowCap, out GpCustomLineCap c))
                    return InvalidParameter;
                if (height != c.Height)
                {
                    c.Height = height;
                    UpdateArrowCap(c);
                }
                return Ok;
            }

            internal static int GdipGetAdjustableArrowCapHeight(HandleRef adjustableArrowCap, out float height)
            {
                height = 0;
                if (!IsArrow(adjustableArrowCap, out GpCustomLineCap c))
                    return InvalidParameter;
                height = c.Height;
                return Ok;
            }

            internal static int GdipSetAdjustableArrowCapWidth(HandleRef adjustableArrowCap, float width)
            {
                if (!IsArrow(adjustableArrowCap, out GpCustomLineCap c))
                    return InvalidParameter;
                if (width != c.ArrowWidth)
                {
                    c.ArrowWidth = width;
                    UpdateArrowCap(c);
                }
                return Ok;
            }

            internal static int GdipGetAdjustableArrowCapWidth(HandleRef adjustableArrowCap, out float width)
            {
                width = 0;
                if (!IsArrow(adjustableArrowCap, out GpCustomLineCap c))
                    return InvalidParameter;
                width = c.ArrowWidth;
                return Ok;
            }

            internal static int GdipSetAdjustableArrowCapMiddleInset(HandleRef adjustableArrowCap, float middleInset)
            {
                if (!IsArrow(adjustableArrowCap, out GpCustomLineCap c))
                    return InvalidParameter;
                if (middleInset != c.MiddleInset)
                {
                    c.MiddleInset = middleInset;
                    UpdateArrowCap(c);
                }
                return Ok;
            }

            internal static int GdipGetAdjustableArrowCapMiddleInset(HandleRef adjustableArrowCap, out float middleInset)
            {
                middleInset = 0;
                if (!IsArrow(adjustableArrowCap, out GpCustomLineCap c))
                    return InvalidParameter;
                middleInset = c.MiddleInset;
                return Ok;
            }

            internal static int GdipSetAdjustableArrowCapFillState(HandleRef adjustableArrowCap, bool fillState)
            {
                if (!IsArrow(adjustableArrowCap, out GpCustomLineCap c))
                    return InvalidParameter;
                if (fillState != c.FillState)
                {
                    c.FillState = fillState;
                    UpdateArrowCap(c);
                }
                return Ok;
            }

            internal static int GdipGetAdjustableArrowCapFillState(HandleRef adjustableArrowCap, out bool fillState)
            {
                fillState = false;
                if (!IsArrow(adjustableArrowCap, out GpCustomLineCap c))
                    return InvalidParameter;
                fillState = c.FillState;
                return Ok;
            }

            /// <summary>A custom cap drawn at a line's end, towards the other point (gdip_custom_linecap_draw, and the arrow's).</summary>
            private static void DrawCap(GpGraphics g, GpPen pen, GpCustomLineCap cap, PointF at, PointF other)
            {
                double angle = Math.Atan2(other.Y - at.Y, other.X - at.X) - Math.PI / 2;
                SKCanvas canvas = g.Begin();
                try
                {
                    canvas.Translate(at.X, at.Y);
                    canvas.RotateRadians((float)angle);
                    float scale = Math.Max(pen.Width, 1) * cap.WidthScale;
                    using SKPaint paint = g.NewPaint(g.Antialias);
                    pen.Brush.Setup(paint, g);
                    if (cap.Type == CustomLineCapType.AdjustableArrowCap)
                    {
                        float penWidth = Math.Max(pen.Width, 2);
                        float w = cap.ArrowWidth / 2 * penWidth, h = cap.Height * penWidth;
                        using var arrow = new SKPath();
                        arrow.MoveTo(0, 0);
                        arrow.LineTo(-w, -h);
                        if (cap.MiddleInset != 0)
                            arrow.LineTo(0, -h + cap.MiddleInset * penWidth);
                        arrow.LineTo(w, -h);
                        arrow.Close();
                        paint.Style = cap.FillState ? SKPaintStyle.StrokeAndFill : SKPaintStyle.Stroke;
                        paint.StrokeWidth = pen.Width;
                        canvas.DrawPath(arrow, paint);
                    }
                    else
                    {
                        if (cap.FillPath != null)
                        {
                            using SKPath fill = cap.FillPath.ToSK();
                            fill.Transform(SKMatrix.CreateScale(scale, scale));
                            paint.Style = SKPaintStyle.Fill;
                            canvas.DrawPath(fill, paint);
                        }
                        if (cap.StrokePath != null)
                        {
                            using SKPath stroke = cap.StrokePath.ToSK();
                            stroke.Transform(SKMatrix.CreateScale(scale, scale));
                            paint.Style = SKPaintStyle.Stroke;
                            paint.StrokeWidth = pen.Width;
                            canvas.DrawPath(stroke, paint);
                        }
                    }
                    paint.Shader?.Dispose();
                }
                finally
                {
                    g.End();
                }
            }

            /// <summary>The pen's custom caps at the ends of what it drew.</summary>
            private static void DrawCaps(GpGraphics g, GpPen pen, IReadOnlyList<PointF> points)
            {
                if (points.Count < 2)
                    return;
                if (pen.CustomStartCap != null)
                    DrawCap(g, pen, pen.CustomStartCap, points[0], points[1]);
                if (pen.CustomEndCap != null)
                    DrawCap(g, pen, pen.CustomEndCap, points[^1], points[^2]);
            }
        }
    }
}
