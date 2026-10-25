// WebFormsForCore: GDI+'s pens and brushes, as libgdiplus' pen.c, brush.c, solidbrush.c and lineargradientbrush.c; what
// libgdiplus set up on Cairo (the line width, dashes, the gradient pattern) is set up on Skia's paint.
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            internal static SKColor ToSKColor(int argb) => new SKColor((uint)argb);

            internal abstract class GpBrush
            {
                public abstract BrushType Type { get; }

                public abstract GpBrush Clone();

                /// <summary>The paint's colour or shader for the brush (gdip_brush_setup).</summary>
                public abstract int Setup(SKPaint paint, GpGraphics graphics);
            }

            internal sealed class GpSolidFill : GpBrush
            {
                public int Color;

                public GpSolidFill(int color) => Color = color;

                public override BrushType Type => BrushType.SolidColor;

                public override GpBrush Clone() => new GpSolidFill(Color);

                public override int Setup(SKPaint paint, GpGraphics graphics)
                {
                    paint.Shader = null;
                    paint.Color = ToSKColor(Color);
                    return Ok;
                }
            }

            internal sealed class GpLineGradient : GpBrush
            {
                public int[] LineColors = new int[2];
                public PointF[] Points = new PointF[2];
                public RectangleF Rectangle;
                public GpMatrix Matrix = new GpMatrix();
                public WrapMode WrapMode = WrapMode.Tile;
                public float Angle;
                public float[] BlendFactors = { 1.0f };
                public float[] BlendPositions = { 0.0f };
                public int[] PresetColors = Array.Empty<int>();
                public float[] PresetPositions = Array.Empty<float>();
                public bool IsAngleScalable;
                public bool GammaCorrection;

                public override BrushType Type => BrushType.LinearGradient;

                public override GpBrush Clone()
                {
                    var b = (GpLineGradient)MemberwiseClone();
                    b.LineColors = (int[])LineColors.Clone();
                    b.Points = (PointF[])Points.Clone();
                    b.Matrix = Matrix.Clone();
                    b.BlendFactors = (float[])BlendFactors.Clone();
                    b.BlendPositions = (float[])BlendPositions.Clone();
                    b.PresetColors = (int[])PresetColors.Clone();
                    b.PresetPositions = (float[])PresetPositions.Clone();
                    return b;
                }

                /// <summary>gdip_linear_gradient_setup_initial_matrix</summary>
                public void SetupInitialMatrix()
                {
                    float cosAngle = (float)Math.Cos(Angle), sinAngle = (float)Math.Sin(Angle);
                    float absCosAngle = Math.Abs(cosAngle), absSinAngle = Math.Abs(sinAngle);
                    RectangleF r = Rectangle;
                    Matrix = new GpMatrix();
                    float transX = r.X + (r.Width / 2.0f), transY = r.Y + (r.Height / 2.0f);
                    float wRatio = (absCosAngle * r.Width + absSinAngle * r.Height) / r.Width;
                    float hRatio = (absSinAngle * r.Width + absCosAngle * r.Height) / r.Height;
                    // cairo_matrix_translate/rotate/scale: each first, then what the matrix was
                    Matrix.CopyFrom(GpMatrix.Multiply(GpMatrix.Translation(transX, transY), Matrix));
                    Matrix.CopyFrom(GpMatrix.Multiply(GpMatrix.Rotation(Angle), Matrix));
                    Matrix.CopyFrom(GpMatrix.Multiply(GpMatrix.Scaling(wRatio, hRatio), Matrix));
                    Matrix.CopyFrom(GpMatrix.Multiply(GpMatrix.Translation(-transX, -transY), Matrix));
                    if (IsAngleScalable && !NearZero(cosAngle) && !NearZero(sinAngle))
                    {
                        float rectRight = r.X + r.Width, rectBottom = r.Y + r.Height;
                        var pts = new[] { Matrix.Transform(new PointF(r.X, r.Y)), Matrix.Transform(new PointF(rectRight, r.Y)), Matrix.Transform(new PointF(r.X, rectBottom)) };
                        float slope;
                        if (sinAngle > 0 && cosAngle > 0)
                        {
                            slope = (float)(-1.0f / ((r.Width / r.Height) * Math.Tan(Angle)));
                            pts[0].Y = (slope * (pts[0].X - r.X)) + r.Y;
                            pts[1].X = ((pts[1].Y - rectBottom) / slope) + rectRight;
                            pts[2].X = ((pts[2].Y - r.Y) / slope) + r.X;
                        }
                        else if (sinAngle > 0 && cosAngle < 0)
                        {
                            slope = (float)(-1.0f / ((r.Width / r.Height) * Math.Tan(Angle - PI / 2)));
                            pts[0].X = ((pts[0].Y - rectBottom) / slope) + rectRight;
                            pts[1].Y = (slope * (pts[1].X - rectRight)) + rectBottom;
                            pts[2].Y = (slope * (pts[2].X - r.X)) + r.Y;
                        }
                        else if (sinAngle < 0 && cosAngle < 0)
                        {
                            slope = (float)(-1.0f / ((r.Width / r.Height) * Math.Tan(Angle)));
                            pts[0].Y = (slope * (pts[0].X - rectRight)) + rectBottom;
                            pts[1].X = ((pts[1].Y - r.Y) / slope) + r.X;
                            pts[2].X = ((pts[2].Y - rectBottom) / slope) + rectRight;
                        }
                        else
                        {
                            slope = (float)(-1.0f / ((r.Width / r.Height) * Math.Tan(Angle - 3 * PI / 2)));
                            pts[0].X = ((pts[0].Y - r.Y) / slope) + r.X;
                            pts[1].Y = (slope * (pts[1].X - r.X)) + r.Y;
                            pts[2].Y = (slope * (pts[2].X - rectRight)) + rectBottom;
                        }
                        if (MatrixFromRect3Points(r, pts[0], pts[1], pts[2], out GpMatrix? m) == Ok)
                            Matrix = m!;
                    }
                }

                public override int Setup(SKPaint paint, GpGraphics graphics)
                {
                    if (WrapMode == WrapMode.Clamp)
                        return InvalidParameter;
                    SKColor[] colors;
                    float[] positions;
                    if (BlendFactors.Length > 1)
                    {
                        // add_color_stops_from_blend
                        colors = new SKColor[BlendFactors.Length];
                        positions = new float[BlendFactors.Length];
                        SKColor s = ToSKColor(LineColors[0]), e = ToSKColor(LineColors[1]);
                        for (int i = 0; i < BlendFactors.Length; i++)
                        {
                            double f = BlendFactors[i];
                            colors[i] = new SKColor(
                                (byte)Math.Round(s.Red * (1 - f) + e.Red * f), (byte)Math.Round(s.Green * (1 - f) + e.Green * f),
                                (byte)Math.Round(s.Blue * (1 - f) + e.Blue * f), (byte)Math.Round(s.Alpha * (1 - f) + e.Alpha * f));
                            positions[i] = Math.Clamp(BlendPositions[i], 0, 1);
                        }
                    }
                    else if (PresetColors.Length > 1)
                    {
                        colors = new SKColor[PresetColors.Length];
                        positions = new float[PresetColors.Length];
                        for (int i = 0; i < PresetColors.Length; i++)
                        {
                            colors[i] = ToSKColor(PresetColors[i]);
                            positions[i] = Math.Clamp(PresetPositions[i], 0, 1);
                        }
                    }
                    else
                    {
                        colors = new[] { ToSKColor(LineColors[0]), ToSKColor(LineColors[1]) };
                        positions = new[] { 0f, 1f };
                    }
                    SKShaderTileMode mode = WrapMode is WrapMode.TileFlipX or WrapMode.TileFlipXY ? SKShaderTileMode.Mirror : SKShaderTileMode.Repeat;
                    paint.Color = SKColors.Black;
                    paint.Shader = SKShader.CreateLinearGradient(new SKPoint(Points[0].X, Points[0].Y), new SKPoint(Points[1].X, Points[1].Y),
                        colors, positions, mode, Matrix.ToSK());
                    return Ok;
                }
            }

            internal sealed class GpPen
            {
                public int Color;
                public GpBrush Brush;
                public bool OwnBrush;
                public float Width = 1;
                public float MiterLimit = 10;
                public LineJoin LineJoin = LineJoin.Miter;
                public DashStyle DashStyle = DashStyle.Solid;
                public LineCap StartCap = LineCap.Flat;
                public LineCap EndCap = LineCap.Flat;
                public DashCap DashCap = DashCap.Flat;
                public PenAlignment Mode = PenAlignment.Center;
                public float DashOffset;
                public float[]? DashArray;
                public float[]? CompoundArray;
                public GraphicsUnit Unit = GraphicsUnit.World;
                public GpMatrix Matrix = new GpMatrix();
                public GpCustomLineCap? CustomStartCap, CustomEndCap;

                public GpPen(GpBrush brush) => Brush = brush;

                public GpPen Clone()
                {
                    var p = (GpPen)MemberwiseClone();
                    p.DashArray = (float[]?)DashArray?.Clone();
                    p.CompoundArray = (float[]?)CompoundArray?.Clone();
                    p.Matrix = Matrix.Clone();
                    p.CustomStartCap = CustomStartCap?.Clone();
                    p.CustomEndCap = CustomEndCap?.Clone();
                    p.Brush = OwnBrush ? new GpSolidFill(((GpSolidFill)Brush).Color) : Brush;
                    return p;
                }

                /// <summary>The cap of a line (libgdiplus' convert_line_cap: one for both ends).</summary>
                public SKStrokeCap Cap => StartCap switch
                {
                    LineCap.Flat => DashArray != null || Width > 1.0 ? SKStrokeCap.Butt : SKStrokeCap.Square,
                    LineCap.Square => SKStrokeCap.Square,
                    LineCap.Round => SKStrokeCap.Round,
                    _ => SKStrokeCap.Butt,
                };

                public SKStrokeJoin Join => LineJoin switch
                {
                    LineJoin.Bevel => SKStrokeJoin.Bevel,
                    LineJoin.Round => SKStrokeJoin.Round,
                    _ => SKStrokeJoin.Miter,
                };
            }

            internal sealed class GpCustomLineCap
            {
                public CustomLineCapType Type;
                public GpPath? FillPath, StrokePath;
                public LineCap BaseCap;
                public float BaseInset;
                public LineCap StartCap = LineCap.Flat, EndCap = LineCap.Flat;
                public LineJoin StrokeJoin = LineJoin.Miter;
                public float WidthScale = 1;
                // AdjustableArrowCap
                public float Height, ArrowWidth, MiddleInset;
                public bool FillState;

                public GpCustomLineCap Clone()
                {
                    var c = (GpCustomLineCap)MemberwiseClone();
                    c.FillPath = FillPath?.Clone();
                    c.StrokePath = StrokePath?.Clone();
                    return c;
                }
            }

            private static readonly float[] s_dot = { 1.0f, 1.0f };
            private static readonly float[] s_dash = { 3.0f, 1.0f };
            private static readonly float[] s_dashDot = { 3.0f, 1.0f, 1.0f, 1.0f };
            private static readonly float[] s_dashDotDot = { 3.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };

            // Brushes

            internal static int GdipCreateSolidFill(int color, out IntPtr brush)
            {
                brush = NewHandle(new GpSolidFill(color));
                return Ok;
            }

            internal static int GdipSetSolidFillColor(HandleRef brush, int color)
            {
                if (!Is(brush, out GpSolidFill? b))
                    return InvalidParameter;
                b.Color = color;
                return Ok;
            }

            internal static int GdipGetSolidFillColor(HandleRef brush, out int color)
            {
                color = 0;
                if (!Is(brush, out GpSolidFill? b))
                    return InvalidParameter;
                color = b.Color;
                return Ok;
            }

            internal static int GdipCloneBrush(HandleRef brush, out IntPtr clonebrush)
            {
                clonebrush = IntPtr.Zero;
                if (!Is(brush, out GpBrush? b))
                    return InvalidParameter;
                clonebrush = NewHandle(b.Clone());
                return Ok;
            }

            internal static int GdipDeleteBrush(HandleRef brush) => Delete<GpBrush>(brush.Handle);

            internal static int GdipGetBrushType(IntPtr brush, out BrushType type)
            {
                type = default;
                if (!Is(brush, out GpBrush? b))
                    return InvalidParameter;
                type = b.Type;
                return Ok;
            }

            // Linear gradients

            internal static int GdipCreateLineBrush(ref PointF point1, ref PointF point2, int color1, int color2, WrapMode wrapMode, out IntPtr lineGradient)
            {
                lineGradient = IntPtr.Zero;
                if (wrapMode == WrapMode.Clamp)
                    return InvalidParameter;
                if (point1.X == point2.X && point1.Y == point2.Y)
                    return OutOfMemory;
                var linear = new GpLineGradient { WrapMode = wrapMode };
                linear.LineColors[0] = color1;
                linear.LineColors[1] = color2;
                bool xFlipped = false, yFlipped = false;
                float width = point2.X - point1.X, height = point2.Y - point1.Y;
                var r = new RectangleF(width < 0 ? point2.X : point1.X, height < 0 ? point2.Y : point1.Y, width, height);
                if (r.Width < 0) { r.Width = -r.Width; xFlipped = true; }
                if (r.Height < 0) { r.Height = -r.Height; yFlipped = true; }
                if (r.Height == 0)
                {
                    r.Height = r.Width;
                    r.Y -= r.Height / 2.0f;
                    linear.Angle = xFlipped ? 180 : 0;
                }
                else if (r.Width == 0)
                {
                    r.Width = r.Height;
                    r.X -= r.Width / 2.0f;
                    linear.Angle = yFlipped ? 270 : 90;
                }
                else
                {
                    float slope = r.Height / r.Width;
                    float newAngle = (float)(Math.Atan(slope) / DegToRad);
                    if (xFlipped) newAngle = 180 - newAngle;
                    if (yFlipped) newAngle = 360 - newAngle;
                    linear.Angle = newAngle;
                }
                linear.Rectangle = r;
                linear.Points[0] = new PointF(r.X, r.Y);
                linear.Points[1] = new PointF(r.X + r.Width, r.Y);
                linear.Angle = (float)(linear.Angle * DegToRad);
                linear.SetupInitialMatrix();
                lineGradient = NewHandle(linear);
                return Ok;
            }

            internal static int GdipCreateLineBrushI(ref Point point1, ref Point point2, int color1, int color2, WrapMode wrapMode, out IntPtr lineGradient)
            {
                PointF p1 = point1, p2 = point2;
                return GdipCreateLineBrush(ref p1, ref p2, color1, color2, wrapMode, out lineGradient);
            }

            internal static int GdipCreateLineBrushFromRectWithAngle(ref RectangleF rect, int color1, int color2, float angle, bool isAngleScaleable, WrapMode wrapMode, out IntPtr lineGradient)
            {
                lineGradient = IntPtr.Zero;
                if (wrapMode == WrapMode.Clamp)
                    return InvalidParameter;
                if (rect.Width == 0.0 || rect.Height == 0.0)
                    return OutOfMemory;
                var linear = new GpLineGradient
                {
                    WrapMode = wrapMode,
                    Angle = (float)((angle % 360) * DegToRad),
                    IsAngleScalable = isAngleScaleable,
                    Rectangle = rect,
                };
                linear.LineColors[0] = color1;
                linear.LineColors[1] = color2;
                linear.Points[0] = new PointF(rect.X, rect.Y);
                linear.Points[1] = new PointF(rect.X + rect.Width + 1, rect.Y);
                linear.SetupInitialMatrix();
                lineGradient = NewHandle(linear);
                return Ok;
            }

            internal static int GdipCreateLineBrushFromRectWithAngleI(ref Rectangle rect, int color1, int color2, float angle, bool isAngleScaleable, WrapMode wrapMode, out IntPtr lineGradient)
            {
                RectangleF r = rect;
                return GdipCreateLineBrushFromRectWithAngle(ref r, color1, color2, angle, isAngleScaleable, wrapMode, out lineGradient);
            }

            private static float AngleOf(LinearGradientMode mode) => mode switch
            {
                LinearGradientMode.Vertical => 90.0f,
                LinearGradientMode.ForwardDiagonal => 45.0f,
                LinearGradientMode.BackwardDiagonal => 135.0f,
                _ => 0,
            };

            internal static int GdipCreateLineBrushFromRect(ref RectangleF rect, int color1, int color2, LinearGradientMode lineGradientMode, WrapMode wrapMode, out IntPtr lineGradient)
            {
                lineGradient = IntPtr.Zero;
                if (lineGradientMode < LinearGradientMode.Horizontal || lineGradientMode > LinearGradientMode.BackwardDiagonal)
                    return OutOfMemory;
                return GdipCreateLineBrushFromRectWithAngle(ref rect, color1, color2, AngleOf(lineGradientMode), true, wrapMode, out lineGradient);
            }

            internal static int GdipCreateLineBrushFromRectI(ref Rectangle rect, int color1, int color2, LinearGradientMode lineGradientMode, WrapMode wrapMode, out IntPtr lineGradient)
            {
                lineGradient = IntPtr.Zero;
                if (lineGradientMode < LinearGradientMode.Horizontal || lineGradientMode > LinearGradientMode.BackwardDiagonal)
                    return OutOfMemory;
                RectangleF r = rect;
                return GdipCreateLineBrushFromRect(ref r, color1, color2, lineGradientMode, wrapMode, out lineGradient);
            }

            internal static int GdipGetLineBlendCount(HandleRef brush, out int count)
            {
                count = 0;
                if (!Is(brush, out GpLineGradient? b))
                    return InvalidParameter;
                count = b.BlendFactors.Length;
                return Ok;
            }

            internal static int GdipSetLineBlend(HandleRef brush, IntPtr blend, IntPtr positions, int count)
            {
                if (!Is(brush, out GpLineGradient? b) || blend == IntPtr.Zero || positions == IntPtr.Zero || count <= 0)
                    return InvalidParameter;
                float* f = (float*)blend, p = (float*)positions;
                if (count >= 2 && (p[0] != 0.0f || p[count - 1] != 1.0f))
                    return InvalidParameter;
                b.BlendFactors = new ReadOnlySpan<float>(f, count).ToArray();
                b.BlendPositions = new ReadOnlySpan<float>(p, count).ToArray();
                b.PresetColors = Array.Empty<int>();
                b.PresetPositions = Array.Empty<float>();
                return Ok;
            }

            internal static int GdipGetLineBlend(HandleRef brush, IntPtr blend, IntPtr positions, int count)
            {
                if (!Is(brush, out GpLineGradient? b) || blend == IntPtr.Zero || positions == IntPtr.Zero || count <= 0)
                    return InvalidParameter;
                if (count < b.BlendFactors.Length)
                    return InsufficientBuffer;
                if (b.BlendFactors.Length < 1)
                    return WrongState;
                b.BlendFactors.CopyTo(new Span<float>((float*)blend, count));
                if (b.BlendFactors.Length > 1)
                    b.BlendPositions.CopyTo(new Span<float>((float*)positions, count));
                return Ok;
            }

            internal static int GdipSetLineGammaCorrection(HandleRef brush, bool useGammaCorrection)
            {
                if (!Is(brush, out GpLineGradient? b))
                    return InvalidParameter;
                b.GammaCorrection = useGammaCorrection;
                return Ok;
            }

            internal static int GdipGetLineGammaCorrection(HandleRef brush, out bool useGammaCorrection)
            {
                useGammaCorrection = false;
                if (!Is(brush, out GpLineGradient? b))
                    return InvalidParameter;
                useGammaCorrection = b.GammaCorrection;
                return Ok;
            }

            internal static int GdipGetLinePresetBlendCount(HandleRef brush, out int count)
            {
                count = 0;
                if (!Is(brush, out GpLineGradient? b))
                    return InvalidParameter;
                count = b.PresetColors.Length;
                return Ok;
            }

            internal static int GdipSetLinePresetBlend(HandleRef brush, IntPtr blend, IntPtr positions, int count)
            {
                if (!Is(brush, out GpLineGradient? b) || blend == IntPtr.Zero || positions == IntPtr.Zero || count < 2)
                    return InvalidParameter;
                float* p = (float*)positions;
                if (p[0] != 0.0f || p[count - 1] != 1.0f)
                    return InvalidParameter;
                b.PresetColors = new ReadOnlySpan<int>((int*)blend, count).ToArray();
                b.PresetPositions = new ReadOnlySpan<float>(p, count).ToArray();
                b.BlendFactors = Array.Empty<float>();
                b.BlendPositions = Array.Empty<float>();
                return Ok;
            }

            internal static int GdipGetLinePresetBlend(HandleRef brush, IntPtr blend, IntPtr positions, int count)
            {
                if (!Is(brush, out GpLineGradient? b) || blend == IntPtr.Zero || positions == IntPtr.Zero || count < 2)
                    return InvalidParameter;
                if (b.PresetColors.Length == 0)
                    return GenericError;
                if (b.PresetColors.Length < 2)
                    return WrongState;
                int n = Math.Min(count, b.PresetColors.Length);
                b.PresetColors.AsSpan(0, n).CopyTo(new Span<int>((int*)blend, count));
                b.PresetPositions.AsSpan(0, n).CopyTo(new Span<float>((float*)positions, count));
                return Ok;
            }

            internal static int GdipSetLineColors(HandleRef brush, int color1, int color2)
            {
                if (!Is(brush, out GpLineGradient? b))
                    return InvalidParameter;
                b.LineColors[0] = color1;
                b.LineColors[1] = color2;
                return Ok;
            }

            internal static int GdipGetLineColors(HandleRef brush, int[] colors)
            {
                if (!Is(brush, out GpLineGradient? b) || colors == null)
                    return InvalidParameter;
                colors[0] = b.LineColors[0];
                colors[1] = b.LineColors[1];
                return Ok;
            }

            internal static int GdipGetLineRect(HandleRef brush, out RectangleF gprectf)
            {
                gprectf = default;
                if (!Is(brush, out GpLineGradient? b))
                    return InvalidParameter;
                gprectf = b.Rectangle;
                return Ok;
            }

            internal static int GdipGetLineTransform(HandleRef brush, HandleRef matrix)
            {
                if (!Is(brush, out GpLineGradient? b) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                m.CopyFrom(b.Matrix);
                return Ok;
            }

            internal static int GdipSetLineTransform(HandleRef brush, HandleRef matrix)
            {
                if (!Is(brush, out GpLineGradient? b) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                if (m.HasBoundaryValues || !m.Clone().Invert())
                    return InvalidParameter;
                b.Matrix.CopyFrom(m);
                return Ok;
            }

            internal static int GdipGetLineWrapMode(HandleRef brush, out int wrapMode)
            {
                wrapMode = 0;
                if (!Is(brush, out GpLineGradient? b))
                    return InvalidParameter;
                wrapMode = (int)b.WrapMode;
                return Ok;
            }

            internal static int GdipSetLineWrapMode(HandleRef brush, int wrapMode)
            {
                if (!Is(brush, out GpLineGradient? b) || wrapMode == (int)WrapMode.Clamp)
                    return InvalidParameter;
                if (wrapMode > (int)WrapMode.Clamp)
                    return Ok;
                b.WrapMode = (WrapMode)wrapMode;
                return Ok;
            }

            internal static int GdipSetLineLinearBlend(HandleRef brush, float focus, float scale)
            {
                if (!Is(brush, out GpLineGradient? b) || focus < 0 || focus > 1 || scale < 0 || scale > 1)
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
                b.PresetColors = Array.Empty<int>();
                b.PresetPositions = Array.Empty<float>();
                return Ok;
            }

            /// <summary>gdip_erf</summary>
            private static float Erf(float x, float std, float mean)
            {
                float constant = (float)(2.0 / Math.Sqrt(PI));
                float z = (x - mean) / std;
                z /= (float)Math.Sqrt(2.0);
                float series = (float)(z - Math.Pow(z, 3) / 3.0 + Math.Pow(z, 5) / 10.0 - Math.Pow(z, 7) / 42.0 + Math.Pow(z, 9) / 216.0 - Math.Pow(z, 11) / 1420.0);
                return constant * series;
            }

            /// <summary>libgdiplus' GdipSetLineSigmaBlend (the blend factors of a bell-shaped curve).</summary>
            internal static void SigmaBlend(float focus, float scale, out float[] factors, out float[] positions)
            {
                int count = focus == 0 || focus == 1 ? 256 : 511;
                factors = new float[count];
                positions = new float[count];
                float fallOffLen = 2.0f, pos, sigma, mean, delta, curveBottom, curveTop, curveHeight;
                int index;
                if (focus == 0)
                {
                    sigma = 1.0f / fallOffLen; mean = 0.5f; delta = 1.0f / 255.0f;
                    curveBottom = 0.5f * (1.0f - Erf(1.0f, sigma, mean));
                    curveTop = 0.5f * (1.0f - Erf(focus, sigma, mean));
                    curveHeight = curveTop - curveBottom;
                    positions[0] = focus; factors[0] = scale;
                    for (index = 1, pos = delta; index < 255; index++, pos += delta)
                    {
                        positions[index] = pos;
                        factors[index] = (scale / curveHeight) * (0.5f * (1.0f - Erf(pos, sigma, mean)) - curveBottom);
                    }
                    positions[count - 1] = 1.0f; factors[count - 1] = 0.0f;
                }
                else if (focus == 1)
                {
                    sigma = 1.0f / fallOffLen; mean = 0.5f; delta = 1.0f / 255.0f;
                    curveBottom = 0.5f * (1.0f + Erf(0.0f, sigma, mean));
                    curveTop = 0.5f * (1.0f + Erf(focus, sigma, mean));
                    curveHeight = curveTop - curveBottom;
                    positions[0] = 0.0f; factors[0] = 0.0f;
                    for (index = 1, pos = delta; index < 255; index++, pos += delta)
                    {
                        positions[index] = pos;
                        factors[index] = (scale / curveHeight) * (0.5f * (1.0f + Erf(pos, sigma, mean)) - curveBottom);
                    }
                    positions[count - 1] = focus; factors[count - 1] = scale;
                }
                else
                {
                    sigma = focus / (2 * fallOffLen); mean = focus / 2.0f; delta = focus / 255.0f;
                    positions[0] = 0.0f; factors[0] = 0.0f;
                    curveBottom = 0.5f * (1.0f + Erf(0.0f, sigma, mean));
                    curveTop = 0.5f * (1.0f + Erf(focus, sigma, mean));
                    curveHeight = curveTop - curveBottom;
                    for (index = 1, pos = delta; index < 255; index++, pos += delta)
                    {
                        positions[index] = pos;
                        factors[index] = (scale / curveHeight) * (0.5f * (1.0f + Erf(pos, sigma, mean)) - curveBottom);
                    }
                    positions[index] = focus; factors[index] = scale;
                    sigma = (1.0f - focus) / (2 * fallOffLen); mean = (1.0f + focus) / 2.0f; delta = (1.0f - focus) / 255.0f;
                    curveBottom = 0.5f * (1.0f - Erf(1.0f, sigma, mean));
                    curveTop = 0.5f * (1.0f - Erf(focus, sigma, mean));
                    curveHeight = curveTop - curveBottom;
                    index++;
                    pos = focus + delta;
                    for (; index < 510; index++, pos += delta)
                    {
                        positions[index] = pos;
                        factors[index] = (scale / curveHeight) * (0.5f * (1.0f - Erf(pos, sigma, mean)) - curveBottom);
                    }
                    positions[count - 1] = 1.0f; factors[count - 1] = 0.0f;
                }
            }

            internal static int GdipSetLineSigmaBlend(HandleRef brush, float focus, float scale)
            {
                if (!Is(brush, out GpLineGradient? b) || focus < 0 || focus > 1 || scale < 0 || scale > 1)
                    return InvalidParameter;
                SigmaBlend(focus, scale, out b.BlendFactors, out b.BlendPositions);
                b.PresetColors = Array.Empty<int>();
                b.PresetPositions = Array.Empty<float>();
                return Ok;
            }

            private static int MultiplyTransform(GpMatrix target, HandleRef matrix, MatrixOrder order)
            {
                if (!Is(matrix, out GpMatrix? m))
                    return Ok;
                if (m.HasBoundaryValues || !m.Clone().Invert())
                    return InvalidParameter;
                target.CopyFrom(order == MatrixOrder.Prepend ? GpMatrix.Multiply(m, target) : GpMatrix.Multiply(target, m));
                return Ok;
            }

            internal static int GdipMultiplyLineTransform(HandleRef brush, HandleRef matrix, MatrixOrder order) =>
                Is(brush, out GpLineGradient? b) ? MultiplyTransform(b.Matrix, matrix, order) : InvalidParameter;

            internal static int GdipResetLineTransform(HandleRef brush)
            {
                if (!Is(brush, out GpLineGradient? b))
                    return InvalidParameter;
                b.Matrix = new GpMatrix();
                return Ok;
            }

            internal static int GdipRotateLineTransform(HandleRef brush, float angle, MatrixOrder order) =>
                Is(brush, out GpLineGradient? b) ? (b.Matrix.Multiply(GpMatrix.Rotation(angle * DegToRad), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipScaleLineTransform(HandleRef brush, float sx, float sy, MatrixOrder order) =>
                Is(brush, out GpLineGradient? b) ? (b.Matrix.Multiply(GpMatrix.Scaling(sx, sy), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipTranslateLineTransform(HandleRef brush, float dx, float dy, MatrixOrder order) =>
                Is(brush, out GpLineGradient? b) ? (b.Matrix.Multiply(GpMatrix.Translation(dx, dy), order) ? Ok : InvalidParameter) : InvalidParameter;

            // Pens

            internal static int GdipCreatePen1(int argb, float width, int unit, out IntPtr pen)
            {
                pen = IntPtr.Zero;
                if (unit > (int)GraphicsUnit.Millimeter + 1 || unit == (int)GraphicsUnit.Display)
                    return InvalidParameter;
                pen = NewHandle(new GpPen(new GpSolidFill(argb)) { Color = argb, Width = width, Unit = (GraphicsUnit)unit, OwnBrush = true });
                return Ok;
            }

            internal static int GdipCreatePen2(HandleRef brush, float width, int unit, out IntPtr pen)
            {
                pen = IntPtr.Zero;
                if (!Is(brush, out GpBrush? b) || unit > (int)GraphicsUnit.Millimeter + 1 || unit == (int)GraphicsUnit.Display)
                    return InvalidParameter;
                var p = new GpPen(b.Clone()) { Width = width, Unit = (GraphicsUnit)unit, OwnBrush = true };
                if (b is GpSolidFill solid)
                    p.Color = solid.Color;
                pen = NewHandle(p);
                return Ok;
            }

            internal static int GdipClonePen(HandleRef pen, out IntPtr clonepen)
            {
                clonepen = IntPtr.Zero;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                clonepen = NewHandle(p.Clone());
                return Ok;
            }

            internal static int GdipDeletePen(HandleRef Pen) => Delete<GpPen>(Pen.Handle);

            internal static int GdipSetPenWidth(HandleRef pen, float width)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                p.Width = width;
                return Ok;
            }

            internal static int GdipGetPenWidth(HandleRef pen, float[] width)
            {
                if (!Is(pen, out GpPen? p) || width == null)
                    return InvalidParameter;
                width[0] = p.Width;
                return Ok;
            }

            internal static int GdipSetPenBrushFill(HandleRef pen, HandleRef brush)
            {
                if (!Is(pen, out GpPen? p) || !Is(brush, out GpBrush? b))
                    return InvalidParameter;
                p.Color = b is GpSolidFill solid ? solid.Color : 0;
                p.Brush = b;
                p.OwnBrush = false;
                return Ok;
            }

            internal static int GdipGetPenBrushFill(HandleRef pen, out IntPtr brush)
            {
                brush = IntPtr.Zero;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                brush = NewHandle(p.Brush.Clone());
                return Ok;
            }

            internal static int GdipGetPenFillType(HandleRef pen, out int pentype)
            {
                pentype = 0;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                pentype = (int)p.Brush.Type;
                return Ok;
            }

            internal static int GdipSetPenColor(HandleRef pen, int argb)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                if (p.Brush is GpSolidFill solid && solid.Color == argb)
                    return Ok;
                p.Color = argb;
                p.Brush = new GpSolidFill(argb);
                p.OwnBrush = true;
                return Ok;
            }

            internal static int GdipGetPenColor(HandleRef pen, out int argb)
            {
                argb = 0;
                if (!Is(pen, out GpPen? p) || p.Brush.Type != BrushType.SolidColor)
                    return InvalidParameter;
                argb = p.Color;
                return Ok;
            }

            internal static int GdipSetPenMiterLimit(HandleRef pen, float miterLimit)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                p.MiterLimit = miterLimit < 1.0f ? 1.0f : miterLimit;
                return Ok;
            }

            internal static int GdipGetPenMiterLimit(HandleRef pen, float[] miterLimit)
            {
                if (!Is(pen, out GpPen? p) || miterLimit == null)
                    return InvalidParameter;
                miterLimit[0] = p.MiterLimit;
                return Ok;
            }

            internal static int GdipSetPenLineJoin(HandleRef pen, int lineJoin)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                p.LineJoin = (LineJoin)lineJoin;
                return Ok;
            }

            internal static int GdipGetPenLineJoin(HandleRef pen, out int lineJoin)
            {
                lineJoin = 0;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                lineJoin = (int)p.LineJoin;
                return Ok;
            }

            private static DashCap ValidDashCap(int dashCap) => dashCap == (int)DashCap.Round || dashCap == (int)DashCap.Triangle ? (DashCap)dashCap : DashCap.Flat;

            internal static int GdipSetPenLineCap197819(HandleRef pen, int startCap, int endCap, int dashCap)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                p.StartCap = (LineCap)startCap;
                p.EndCap = (LineCap)endCap;
                p.DashCap = ValidDashCap(dashCap);
                return Ok;
            }

            internal static int GdipSetPenMode(HandleRef pen, PenAlignment penAlign)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                if (penAlign == PenAlignment.Inset && p.CompoundArray != null)
                    return NotImplemented;
                p.Mode = penAlign;
                return Ok;
            }

            internal static int GdipGetPenMode(HandleRef pen, out PenAlignment penAlign)
            {
                penAlign = default;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                penAlign = p.Mode;
                return Ok;
            }

            internal static int GdipSetPenTransform(HandleRef pen, HandleRef matrix)
            {
                if (!Is(pen, out GpPen? p) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                if (m.HasBoundaryValues || !m.Clone().Invert())
                    return InvalidParameter;
                p.Matrix.CopyFrom(m);
                return Ok;
            }

            internal static int GdipGetPenTransform(HandleRef pen, HandleRef matrix)
            {
                if (!Is(pen, out GpPen? p) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                m.CopyFrom(p.Matrix);
                return Ok;
            }

            internal static int GdipResetPenTransform(HandleRef brush)
            {
                if (!Is(brush, out GpPen? p))
                    return InvalidParameter;
                p.Matrix = new GpMatrix();
                return Ok;
            }

            internal static int GdipMultiplyPenTransform(HandleRef brush, HandleRef matrix, MatrixOrder order) =>
                Is(brush, out GpPen? p) ? MultiplyTransform(p.Matrix, matrix, order) : InvalidParameter;

            internal static int GdipTranslatePenTransform(HandleRef brush, float dx, float dy, MatrixOrder order) =>
                Is(brush, out GpPen? p) ? (p.Matrix.Multiply(GpMatrix.Translation(dx, dy), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipScalePenTransform(HandleRef brush, float sx, float sy, MatrixOrder order) =>
                Is(brush, out GpPen? p) ? (p.Matrix.Multiply(GpMatrix.Scaling(sx, sy), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipRotatePenTransform(HandleRef brush, float angle, MatrixOrder order) =>
                Is(brush, out GpPen? p) ? (p.Matrix.Multiply(GpMatrix.Rotation(angle * DegToRad), order) ? Ok : InvalidParameter) : InvalidParameter;

            internal static int GdipGetPenDashStyle(HandleRef pen, out int dashstyle)
            {
                dashstyle = 0;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                dashstyle = (int)p.DashStyle;
                return Ok;
            }

            internal static int GdipSetPenDashStyle(HandleRef pen, int dashstyle)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                switch ((DashStyle)dashstyle)
                {
                    case DashStyle.Solid: p.DashArray = null; break;
                    case DashStyle.DashDot: p.DashArray = s_dashDot; break;
                    case DashStyle.DashDotDot: p.DashArray = s_dashDotDot; break;
                    case DashStyle.Dot: p.DashArray = s_dot; break;
                    case DashStyle.Dash: p.DashArray = s_dash; break;
                    case DashStyle.Custom: break;
                    default: return Ok;
                }
                p.DashStyle = (DashStyle)dashstyle;
                return Ok;
            }

            internal static int GdipGetPenDashOffset(HandleRef pen, float[] dashoffset)
            {
                if (!Is(pen, out GpPen? p) || dashoffset == null)
                    return InvalidParameter;
                dashoffset[0] = p.DashOffset;
                return Ok;
            }

            internal static int GdipSetPenDashOffset(HandleRef pen, float dashoffset)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                p.DashOffset = dashoffset;
                return Ok;
            }

            internal static int GdipGetPenDashCount(HandleRef pen, out int dashcount)
            {
                dashcount = 0;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                dashcount = p.DashArray?.Length ?? 0;
                return Ok;
            }

            internal static int GdipGetPenDashArray(HandleRef pen, float[] memorydash, int count)
            {
                if (!Is(pen, out GpPen? p) || memorydash == null || count > (p.DashArray?.Length ?? 0))
                    return InvalidParameter;
                if (p.DashArray == null)
                    return OutOfMemory;
                if (count < 0)
                    return OutOfMemory;
                Array.Copy(p.DashArray, memorydash, count);
                return Ok;
            }

            internal static int GdipSetPenDashArray(HandleRef pen, HandleRef memorydash, int count)
            {
                if (!Is(pen, out GpPen? p) || memorydash.Handle == IntPtr.Zero || count <= 0)
                    return InvalidParameter;
                var dash = new ReadOnlySpan<float>((float*)memorydash.Handle, count).ToArray();
                float sum = 0;
                foreach (float d in dash)
                {
                    sum += d;
                    if (d < 0.0)
                        return InvalidParameter;
                }
                if (sum == 0.0)
                    return InvalidParameter;
                p.DashArray = dash;
                p.DashStyle = DashStyle.Custom;
                return Ok;
            }

            internal static int GdipGetPenCompoundCount(HandleRef pen, out int count)
            {
                count = 0;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                count = p.CompoundArray?.Length ?? 0;
                return Ok;
            }

            internal static int GdipSetPenCompoundArray(HandleRef pen, float[] array, int count)
            {
                if (!Is(pen, out GpPen? p) || array == null || count <= 0 || count % 2 == 1)
                    return InvalidParameter;
                if (p.Mode == PenAlignment.Inset)
                    return NotImplemented;
                float minValue = 0.0f;
                for (int i = 0; i < count; i++)
                {
                    if (array[i] < minValue || array[i] > 1.0f)
                        return InvalidParameter;
                    minValue = array[i];
                }
                p.CompoundArray = array.AsSpan(0, count).ToArray();
                return Ok;
            }

            internal static int GdipGetPenCompoundArray(HandleRef pen, float[] array, int count)
            {
                if (!Is(pen, out GpPen? p) || array == null || count > (p.CompoundArray?.Length ?? 0))
                    return InvalidParameter;
                if (count <= 0)
                    return Ok;
                Array.Copy(p.CompoundArray!, array, count);
                return Ok;
            }

            internal static int GdipSetPenStartCap(HandleRef pen, int startCap)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                p.CustomStartCap = null;
                p.StartCap = (LineCap)startCap;
                return Ok;
            }

            internal static int GdipGetPenStartCap(HandleRef pen, out int startCap)
            {
                startCap = 0;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                startCap = (int)p.StartCap;
                return Ok;
            }

            internal static int GdipSetPenEndCap(HandleRef pen, int endCap)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                p.CustomEndCap = null;
                p.EndCap = (LineCap)endCap;
                return Ok;
            }

            internal static int GdipGetPenEndCap(HandleRef pen, out int endCap)
            {
                endCap = 0;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                endCap = (int)p.EndCap;
                return Ok;
            }

            internal static int GdipSetPenDashCap197819(HandleRef pen, int dashCap)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                p.DashCap = ValidDashCap(dashCap);
                return Ok;
            }

            internal static int GdipGetPenDashCap197819(HandleRef pen, out int dashCap)
            {
                dashCap = 0;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                dashCap = (int)p.DashCap;
                return Ok;
            }

            internal static int GdipSetPenCustomStartCap(HandleRef pen, HandleRef customCap)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                if (!Is(customCap, out GpCustomLineCap? cap))
                    return InvalidParameter;
                p.CustomStartCap = cap.Clone();
                p.StartCap = LineCap.Custom;
                return Ok;
            }

            internal static int GdipGetPenCustomStartCap(HandleRef pen, out IntPtr customCap)
            {
                customCap = IntPtr.Zero;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                if (p.CustomStartCap != null)
                    customCap = NewHandle(p.CustomStartCap.Clone());
                return Ok;
            }

            internal static int GdipSetPenCustomEndCap(HandleRef pen, HandleRef customCap)
            {
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                if (!Is(customCap, out GpCustomLineCap? cap))
                    return InvalidParameter;
                p.CustomEndCap = cap.Clone();
                p.EndCap = LineCap.Custom;
                return Ok;
            }

            internal static int GdipGetPenCustomEndCap(HandleRef pen, out IntPtr customCap)
            {
                customCap = IntPtr.Zero;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                if (p.CustomEndCap != null)
                    customCap = NewHandle(p.CustomEndCap.Clone());
                return Ok;
            }
        }
    }
}
