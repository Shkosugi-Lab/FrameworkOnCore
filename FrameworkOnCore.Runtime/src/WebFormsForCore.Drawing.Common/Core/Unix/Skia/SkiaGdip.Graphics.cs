// WebFormsForCore: GDI+'s Graphics over Skia. Its state is libgdiplus' graphics.c (the world transform, the clip in world
// space with its matrix, containers, saved states); the drawing is Skia's, the coordinates GDI+'s: a pixel's centre is at
// whole coordinates (PixelOffsetMode None, the default), so the device is moved half a pixel (less 1/64: GDI+ takes a pixel
// whose centre is on a shape's left or top edge, Skia the one on its right or bottom).
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            private const float PixelCenterOffset = 0.5f - 1f / 64;

            internal sealed class GpGraphics : IDisposable
            {
                public GpBitmap? Bitmap;
                public GpMetafile? Metafile;
                public SKCanvas Canvas;
                private SKBitmap? _canvasSurface;
                public GpMatrix World = new GpMatrix();
                public GpMatrix PreviousMatrix = new GpMatrix();
                public GpRegion Clip = GpRegion.Infinite();
                public GpRegion? PreviousClip;
                public GpRegion OverallClip;
                public GpMatrix ClipMatrix = new GpMatrix();
                public Rectangle Bounds, OrigBounds;
                public GraphicsUnit PageUnit = GraphicsUnit.Display;
                public float PageScale = 1.0f;
                public InterpolationMode Interpolation = InterpolationMode.Bilinear;
                public SmoothingMode DrawMode = SmoothingMode.None;
                public TextRenderingHint TextMode = TextRenderingHint.SystemDefault;
                public CompositingMode CompositeMode = CompositingMode.SourceOver;
                public CompositingQuality CompositeQuality = CompositingQuality.Default;
                public PixelOffsetMode PixelMode = PixelOffsetMode.Default;
                public int RenderOriginX, RenderOriginY;
                public float DpiX = 96, DpiY = 96;
                public int TextContrast = 4;
                public bool Busy;
                public readonly List<GpState> SavedStates = new List<GpState>();

                public GpGraphics(SKCanvas canvas)
                {
                    Canvas = canvas;
                    OverallClip = Clip;
                }

                public static GpGraphics ForBitmap(GpBitmap bitmap)
                {
                    SKBitmap surface = bitmap.EnsureSurface();
                    var g = new GpGraphics(new SKCanvas(surface)) { Bitmap = bitmap, _canvasSurface = surface };
                    g.DpiX = bitmap.DpiX <= 0 ? 96 : bitmap.DpiX;
                    g.DpiY = bitmap.DpiY <= 0 ? 96 : bitmap.DpiY;
                    g.Bounds = g.OrigBounds = new Rectangle(0, 0, bitmap.W, bitmap.H);
                    bitmap.GraphicsCount++;
                    return g;
                }

                public void Dispose()
                {
                    if (Metafile != null)
                        Metafile.EndRecording();
                    else
                        Canvas.Dispose();
                    if (Bitmap != null)
                        Bitmap.GraphicsCount--;
                    Clip.Dispose();
                }

                /// <summary>gdip_graphics_reset (a container's start)</summary>
                public void Reset()
                {
                    PreviousMatrix = new GpMatrix();
                    ResetClip();
                    ClipMatrix = new GpMatrix();
                    PageUnit = GraphicsUnit.Display;
                    PageScale = 1.0f;
                    Interpolation = InterpolationMode.Bilinear;
                    CompositeQuality = CompositingQuality.Default;
                    CompositeMode = CompositingMode.SourceOver;
                    TextMode = TextRenderingHint.SystemDefault;
                    PixelMode = PixelOffsetMode.Default;
                    TextContrast = 4;
                    DrawMode = SmoothingMode.None;
                }

                public void ResetClip()
                {
                    Clip.SetInfinite();
                    if (!PreviousMatrix.IsIdentity)
                    {
                        ClipMatrix = PreviousMatrix.Clone();
                        ClipMatrix.Invert();
                    }
                    else
                    {
                        ClipMatrix = new GpMatrix();
                    }
                    CalculateOverallClipping();
                }

                /// <summary>gdip_calculate_overall_clipping</summary>
                public void CalculateOverallClipping()
                {
                    if (PreviousClip == null)
                    {
                        OverallClip = Clip;
                    }
                    else
                    {
                        OverallClip = PreviousClip.Clone();
                        OverallClip.Combine(Clip, CombineMode.Intersect);
                    }
                }

                /// <summary>apply_world_to_bounds</summary>
                public void ApplyWorldToBounds()
                {
                    PointF a = ClipMatrix.Transform(new PointF(OrigBounds.X, OrigBounds.Y));
                    PointF b = ClipMatrix.Transform(new PointF(OrigBounds.X + OrigBounds.Width, OrigBounds.Y + OrigBounds.Height));
                    Bounds = new Rectangle(
                        (int)Math.Min(a.X, b.X), (int)Math.Min(a.Y, b.Y),
                        IRound(Math.Abs(b.X - a.X)), IRound(Math.Abs(b.Y - a.Y)));
                }

                /// <summary>The page unit's pixels per unit (gdip_unit_conversion to pixels), times the page scale.</summary>
                public float PageScaleX => UnitToPixels(PageUnit, DpiX) * (PageUnit == GraphicsUnit.World || PageUnit == GraphicsUnit.Pixel ? 1 : PageScale);

                public float PageScaleY => UnitToPixels(PageUnit, DpiY) * (PageUnit == GraphicsUnit.World || PageUnit == GraphicsUnit.Pixel ? 1 : PageScale);

                /// <summary>Page units (and the page scale) to the device: the pixels of the target.</summary>
                public SKMatrix PageMatrix => SKMatrix.CreateScale(PageScaleX, PageScaleY);

                public bool Antialias => DrawMode == SmoothingMode.AntiAlias || DrawMode == SmoothingMode.HighQuality;

                public float DeviceOffset => PixelMode == PixelOffsetMode.Half || PixelMode == PixelOffsetMode.HighQuality ? 0 : PixelCenterOffset;

                /// <summary>World to device: the world transform, the page's units, the pixels' centres.</summary>
                public SKMatrix DeviceMatrix
                {
                    get
                    {
                        SKMatrix m = SKMatrix.Concat(PageMatrix, World.ToSK());
                        float o = DeviceOffset;
                        return o == 0 ? m : SKMatrix.Concat(SKMatrix.CreateTranslation(o, o), m);
                    }
                }

                /// <summary>The canvas ready for a drawing: its surface current, the clip set, the world transform.</summary>
                public SKCanvas Begin()
                {
                    if (Bitmap != null)
                    {
                        SKBitmap surface = Bitmap.EnsureSurface();
                        if (surface != _canvasSurface)
                        {
                            Canvas.Dispose();
                            Canvas = new SKCanvas(surface);
                            _canvasSurface = surface;
                        }
                    }
                    Canvas.Save();
                    if (!OverallClip.IsInfinite)
                    {
                        float o = DeviceOffset;
                        SKMatrix clipMatrix = o == 0 ? PageMatrix : SKMatrix.Concat(SKMatrix.CreateTranslation(o, o), PageMatrix);
                        Canvas.SetMatrix(clipMatrix);
                        if (OverallClip.Rects != null && OverallClip.Rects.Count == 1)
                        {
                            RectangleF r = OverallClip.Rects[0];
                            Canvas.ClipRect(new SKRect(r.Left, r.Top, r.Right, r.Bottom), SKClipOperation.Intersect, false);
                        }
                        else
                        {
                            using SKPath clip = OverallClip.ToSK();
                            Canvas.ClipPath(clip, SKClipOperation.Intersect, false);
                        }
                    }
                    Canvas.SetMatrix(DeviceMatrix);
                    return Canvas;
                }

                public void End()
                {
                    Canvas.Restore();
                    Bitmap?.Drawn();
                }

                public SKPaint NewPaint(bool antialias)
                {
                    var paint = new SKPaint { IsAntialias = antialias };
                    if (CompositeMode == CompositingMode.SourceCopy)
                        paint.BlendMode = SKBlendMode.Src;
                    return paint;
                }
            }

            internal sealed class GpState
            {
                public GpMatrix Matrix = new GpMatrix(), PreviousMatrix = new GpMatrix(), ClipMatrix = new GpMatrix();
                public GpRegion Clip = GpRegion.Infinite();
                public GpRegion? PreviousClip;
                public CompositingMode CompositeMode;
                public CompositingQuality CompositeQuality;
                public InterpolationMode Interpolation;
                public GraphicsUnit PageUnit;
                public float PageScale;
                public SmoothingMode DrawMode;
                public TextRenderingHint TextMode;
                public PixelOffsetMode PixelMode;
                public int OrgX, OrgY, TextContrast;
            }

            /// <summary>Pixels per unit (gdip_unit_conversion).</summary>
            internal static float UnitToPixels(GraphicsUnit unit, float dpi) => unit switch
            {
                GraphicsUnit.Document => dpi / 300.0f,
                GraphicsUnit.Inch => dpi,
                GraphicsUnit.Millimeter => dpi / 25.4f,
                GraphicsUnit.Point => dpi / 72.0f,
                _ => 1.0f,   // World, Display, Pixel
            };

            // Creation

            internal static int GdipGetImageGraphicsContext(IntPtr image, out IntPtr graphics)
            {
                graphics = IntPtr.Zero;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                if (i is GpMetafile m)
                {
                    if (m.Picture != null || m.Recorder != null)
                        return OutOfMemory;   // not recording (any more)
                    SKCanvas canvas = m.BeginRecording();
                    var g = new GpGraphics(canvas) { Metafile = m };
                    g.Bounds = g.OrigBounds = new Rectangle(0, 0, m.Width, m.Height);
                    graphics = NewHandle(g);
                    return Ok;
                }
                var b = (GpBitmap)i;
                switch (b.Format)
                {
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppPArgb:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format48bppRgb:
                    case PixelFormat.Format64bppArgb:
                    case PixelFormat.Format64bppPArgb:
                        break;
                    default:
                        return OutOfMemory;
                }
                graphics = NewHandle(GpGraphics.ForBitmap(b));
                return Ok;
            }

            internal static int GdipCreateFromHDC(IntPtr hdc, out IntPtr graphics)
            {
                graphics = IntPtr.Zero;
                if (hdc == IntPtr.Zero)
                    return OutOfMemory;
                // An HDC here is a Graphics' (GdipGetDC): a new Graphics on the same image.
                if (Is(hdc, out GpGraphics? g) && g.Bitmap != null)
                {
                    graphics = NewHandle(GpGraphics.ForBitmap(g.Bitmap));
                    return Ok;
                }
                return NotImplemented;
            }

            internal static int GdipCreateFromHWND(IntPtr hwnd, out IntPtr graphics)
            {
                graphics = IntPtr.Zero;
                return NotImplemented;
            }

            internal static int GdipDeleteGraphics(HandleRef graphics)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                return Delete<GpGraphics>(graphics.Handle);
            }

            internal static int GdipGetDC(HandleRef graphics, out IntPtr hdc)
            {
                hdc = IntPtr.Zero;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                hdc = graphics.Handle;
                g.Busy = true;
                return Ok;
            }

            internal static int GdipReleaseDC(HandleRef graphics, HandleRef hdc)
            {
                if (!Is(graphics, out GpGraphics? g) || hdc.Handle == IntPtr.Zero || !g.Busy)
                    return InvalidParameter;
                if (hdc.Handle != graphics.Handle)
                    return InvalidParameter;
                g.Busy = false;
                return Ok;
            }

            internal static int GdipFlush(HandleRef graphics, FlushIntention intention)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                g.Canvas.Flush();
                return Ok;
            }

            internal static int GdipSetVisibleClip_linux(IntPtr graphics, ref Rectangle rect)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                g.OrigBounds = g.Bounds = rect;
                return Ok;
            }

            // Saved states and containers

            internal static int GdipSaveGraphics(HandleRef graphics, out int state)
            {
                state = 0;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                var s = new GpState
                {
                    Matrix = g.World.Clone(),
                    PreviousMatrix = g.PreviousMatrix.Clone(),
                    Clip = g.Clip.Clone(),
                    PreviousClip = g.PreviousClip?.Clone(),
                    ClipMatrix = g.ClipMatrix.Clone(),
                    CompositeMode = g.CompositeMode,
                    CompositeQuality = g.CompositeQuality,
                    Interpolation = g.Interpolation,
                    PageUnit = g.PageUnit,
                    PageScale = g.PageScale,
                    DrawMode = g.DrawMode,
                    TextMode = g.TextMode,
                    PixelMode = g.PixelMode,
                    OrgX = g.RenderOriginX,
                    OrgY = g.RenderOriginY,
                    TextContrast = g.TextContrast,
                };
                if (g.SavedStates.Count >= 512)
                    return OutOfMemory;
                g.SavedStates.Add(s);
                state = g.SavedStates.Count;   // not zero, as GDI+'s
                return Ok;
            }

            internal static int GdipRestoreGraphics(IntPtr graphics, uint graphicsState)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (graphicsState == 0 || graphicsState > g.SavedStates.Count)
                    return Ok;
                GpState s = g.SavedStates[(int)graphicsState - 1];
                g.World = s.Matrix.Clone();
                g.PreviousMatrix = s.PreviousMatrix.Clone();
                g.RenderOriginX = s.OrgX;
                g.RenderOriginY = s.OrgY;
                g.Clip = s.Clip.Clone();
                g.PreviousClip = s.PreviousClip?.Clone();
                g.ClipMatrix = s.ClipMatrix.Clone();
                g.CalculateOverallClipping();
                g.CompositeMode = s.CompositeMode;
                g.CompositeQuality = s.CompositeQuality;
                g.Interpolation = s.Interpolation;
                g.PageUnit = s.PageUnit;
                g.PageScale = s.PageScale;
                g.DrawMode = s.DrawMode;
                g.TextMode = s.TextMode;
                g.PixelMode = s.PixelMode;
                g.TextContrast = s.TextContrast;
                g.SavedStates.RemoveRange((int)graphicsState - 1, g.SavedStates.Count - ((int)graphicsState - 1));
                return Ok;
            }

            internal static int GdipBeginContainer2(HandleRef graphics, out int state)
            {
                int status = GdipSaveGraphics(graphics, out state);
                if (status == Ok && Is(graphics, out GpGraphics? g))
                {
                    if (g.PreviousClip != null)
                        g.PreviousClip = g.OverallClip.Clone();
                    else if (!g.Clip.IsInfinite)
                        g.PreviousClip = g.Clip.Clone();
                    g.Reset();
                    g.PreviousMatrix = g.World.Clone();
                    g.CalculateOverallClipping();
                }
                return status;
            }

            internal static int GdipBeginContainer(HandleRef graphics, ref RectangleF dstRect, ref RectangleF srcRect, GraphicsUnit unit, out int state)
            {
                state = 0;
                if (unit < GraphicsUnit.Pixel || unit > GraphicsUnit.Millimeter)
                    return InvalidParameter;
                return GdipBeginContainer2(graphics, out state);
            }

            internal static int GdipBeginContainerI(HandleRef graphics, ref Rectangle dstRect, ref Rectangle srcRect, GraphicsUnit unit, out int state)
            {
                RectangleF d = dstRect, s = srcRect;
                return GdipBeginContainer(graphics, ref d, ref s, unit, out state);
            }

            internal static int GdipEndContainer(HandleRef graphics, int state) => GdipRestoreGraphics(graphics.Handle, (uint)state);

            // World transform

            internal static int GdipResetWorldTransform(HandleRef graphics)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (!g.PreviousMatrix.IsIdentity)
                {
                    g.World = g.PreviousMatrix.Clone();
                    g.ClipMatrix = g.PreviousMatrix.Clone();
                    g.ClipMatrix.Invert();
                }
                else
                {
                    g.World = new GpMatrix();
                    g.ClipMatrix = new GpMatrix();
                }
                g.ApplyWorldToBounds();
                return Ok;
            }

            internal static int GdipSetWorldTransform(HandleRef graphics, HandleRef matrix)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (!Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                if (m.IsIdentity)
                    return GdipResetWorldTransform(graphics);
                if (m.HasBoundaryValues || !m.Clone().Invert())
                    return InvalidParameter;
                GpMatrix copy = m.Clone();
                if (!g.PreviousMatrix.IsIdentity)
                    copy.Multiply(g.PreviousMatrix, MatrixOrder.Append);
                g.World = copy.Clone();
                g.ClipMatrix = copy.Clone();
                g.ClipMatrix.Invert();
                g.ApplyWorldToBounds();
                return Ok;
            }

            internal static int GdipGetWorldTransform(HandleRef graphics, HandleRef matrix)
            {
                if (!Is(graphics, out GpGraphics? g) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                m.CopyFrom(g.World);
                if (!g.PreviousMatrix.IsIdentity)
                {
                    GpMatrix inverted = g.PreviousMatrix.Clone();
                    inverted.Invert();
                    m.Multiply(inverted, MatrixOrder.Append);
                }
                return Ok;
            }

            private static MatrixOrder Reverse(MatrixOrder order) => order == MatrixOrder.Append ? MatrixOrder.Prepend : MatrixOrder.Append;

            internal static int GdipMultiplyWorldTransform(HandleRef graphics, HandleRef matrix, MatrixOrder order)
            {
                if (!Is(graphics, out GpGraphics? g) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                GpMatrix inverted = m.Clone();
                if (m.HasBoundaryValues || !inverted.Invert())
                    return InvalidParameter;
                if (!g.World.Multiply(m, order))
                    return InvalidParameter;
                if (!g.ClipMatrix.Multiply(inverted, order))
                    return InvalidParameter;
                g.ApplyWorldToBounds();
                return Ok;
            }

            internal static int GdipRotateWorldTransform(HandleRef graphics, float angle, MatrixOrder order)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (!g.World.Multiply(GpMatrix.Rotation(angle * DegToRad), order))
                    return InvalidParameter;
                if (!g.ClipMatrix.Multiply(GpMatrix.Rotation(-angle * DegToRad), Reverse(order)))
                    return InvalidParameter;
                g.ApplyWorldToBounds();
                return Ok;
            }

            internal static int GdipScaleWorldTransform(HandleRef graphics, float sx, float sy, MatrixOrder order)
            {
                if (!Is(graphics, out GpGraphics? g) || sx == 0.0f || sy == 0.0f)
                    return InvalidParameter;
                if (!g.World.Multiply(GpMatrix.Scaling(sx, sy), order))
                    return InvalidParameter;
                if (!g.ClipMatrix.Multiply(GpMatrix.Scaling(1.0f / sx, 1.0f / sy), Reverse(order)))
                    return InvalidParameter;
                g.ApplyWorldToBounds();
                return Ok;
            }

            internal static int GdipTranslateWorldTransform(HandleRef graphics, float dx, float dy, MatrixOrder order)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (!g.World.Multiply(GpMatrix.Translation(dx, dy), order))
                    return InvalidParameter;
                if (!g.ClipMatrix.Multiply(GpMatrix.Translation(-dx, -dy), Reverse(order)))
                    return InvalidParameter;
                g.ApplyWorldToBounds();
                return Ok;
            }

            /// <summary>Between world, page and device coordinates (libgdiplus did nothing).</summary>
            private static GpMatrix? SpaceMatrix(GpGraphics g, CoordinateSpace dest, CoordinateSpace src)
            {
                // world -> page: the world transform; page -> device: the page unit and scale.
                GpMatrix page = GpMatrix.Scaling(g.PageScaleX, g.PageScaleY);
                GpMatrix ToDevice(CoordinateSpace space) => space switch
                {
                    CoordinateSpace.World => GpMatrix.Multiply(g.World, page),
                    CoordinateSpace.Page => page,
                    _ => new GpMatrix(),
                };
                GpMatrix fromSrc = ToDevice(src), toDest = ToDevice(dest);
                if (!toDest.Invert())
                    return null;
                return GpMatrix.Multiply(fromSrc, toDest);
            }

            internal static int GdipTransformPoints(HandleRef graphics, int destSpace, int srcSpace, PointF* points, int count)
            {
                if (!Is(graphics, out GpGraphics? g) || points == null || count <= 0)
                    return InvalidParameter;
                if (destSpace < 0 || destSpace > 2 || srcSpace < 0 || srcSpace > 2)
                    return InvalidParameter;
                if (destSpace == srcSpace)
                    return Ok;
                GpMatrix? m = SpaceMatrix(g, (CoordinateSpace)destSpace, (CoordinateSpace)srcSpace);
                if (m == null)
                    return InvalidParameter;
                for (int i = 0; i < count; i++)
                    points[i] = m.Transform(points[i]);
                return Ok;
            }

            internal static int GdipTransformPointsI(HandleRef graphics, int destSpace, int srcSpace, Point* points, int count)
            {
                if (!Is(graphics, out GpGraphics? g) || points == null || count <= 0)
                    return InvalidParameter;
                if (destSpace < 0 || destSpace > 2 || srcSpace < 0 || srcSpace > 2)
                    return InvalidParameter;
                if (destSpace == srcSpace)
                    return Ok;
                GpMatrix? m = SpaceMatrix(g, (CoordinateSpace)destSpace, (CoordinateSpace)srcSpace);
                if (m == null)
                    return InvalidParameter;
                for (int i = 0; i < count; i++)
                {
                    PointF p = m.Transform(points[i]);
                    points[i] = new Point(IRound(p.X), IRound(p.Y));
                }
                return Ok;
            }

            // Modes

            internal static int GdipSetRenderingOrigin(HandleRef graphics, int x, int y)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                g.RenderOriginX = x;
                g.RenderOriginY = y;
                return Ok;
            }

            internal static int GdipGetRenderingOrigin(HandleRef graphics, out int x, out int y)
            {
                x = y = 0;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                x = g.RenderOriginX;
                y = g.RenderOriginY;
                return Ok;
            }

            internal static int GdipGetDpiX(HandleRef graphics, out float dpi)
            {
                dpi = 0;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                dpi = g.DpiX;
                return Ok;
            }

            internal static int GdipGetDpiY(HandleRef graphics, out float dpi)
            {
                dpi = 0;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                dpi = g.DpiY;
                return Ok;
            }

            internal static int GdipSetInterpolationMode(HandleRef graphics, InterpolationMode interpolationMode)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (interpolationMode <= InterpolationMode.Invalid || interpolationMode > InterpolationMode.HighQualityBicubic)
                    return InvalidParameter;
                g.Interpolation = interpolationMode switch
                {
                    InterpolationMode.Default or InterpolationMode.Low => InterpolationMode.Bilinear,
                    InterpolationMode.High => InterpolationMode.HighQualityBicubic,
                    _ => interpolationMode,
                };
                return Ok;
            }

            internal static int GdipGetInterpolationMode(HandleRef graphics, out InterpolationMode interpolationMode)
            {
                interpolationMode = default;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                interpolationMode = g.Interpolation;
                return Ok;
            }

            internal static int GdipSetTextRenderingHint(HandleRef graphics, TextRenderingHint textRenderingHint)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (textRenderingHint > TextRenderingHint.ClearTypeGridFit)
                    return InvalidParameter;
                g.TextMode = textRenderingHint;
                return Ok;
            }

            internal static int GdipGetTextRenderingHint(HandleRef graphics, out TextRenderingHint textRenderingHint)
            {
                textRenderingHint = default;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                textRenderingHint = g.TextMode;
                return Ok;
            }

            internal static int GdipSetPixelOffsetMode(HandleRef graphics, PixelOffsetMode pixelOffsetMode)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (pixelOffsetMode <= PixelOffsetMode.Invalid || pixelOffsetMode > PixelOffsetMode.Half)
                    return InvalidParameter;
                g.PixelMode = pixelOffsetMode;
                return Ok;
            }

            internal static int GdipGetPixelOffsetMode(HandleRef graphics, out PixelOffsetMode pixelOffsetMode)
            {
                pixelOffsetMode = default;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                pixelOffsetMode = g.PixelMode;
                return Ok;
            }

            internal static int GdipSetTextContrast(HandleRef graphics, int textContrast)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (textContrast < 0 || textContrast > 12)
                    return InvalidParameter;
                g.TextContrast = textContrast;
                return Ok;
            }

            internal static int GdipGetTextContrast(HandleRef graphics, out int textContrast)
            {
                textContrast = 0;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                textContrast = g.TextContrast;
                return Ok;
            }

            internal static int GdipSetSmoothingMode(HandleRef graphics, SmoothingMode smoothingMode)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (smoothingMode <= SmoothingMode.Invalid || smoothingMode > SmoothingMode.AntiAlias + 1)
                    return InvalidParameter;
                g.DrawMode = smoothingMode switch
                {
                    SmoothingMode.Default or SmoothingMode.HighSpeed => SmoothingMode.None,
                    SmoothingMode.HighQuality => SmoothingMode.AntiAlias,
                    _ => smoothingMode,
                };
                return Ok;
            }

            internal static int GdipGetSmoothingMode(HandleRef graphics, out SmoothingMode smoothingMode)
            {
                smoothingMode = default;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                smoothingMode = g.DrawMode;
                return Ok;
            }

            internal static int GdipSetCompositingMode(HandleRef graphics, CompositingMode compositingMode)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                g.CompositeMode = compositingMode;
                return Ok;
            }

            internal static int GdipGetCompositingMode(HandleRef graphics, out CompositingMode compositingMode)
            {
                compositingMode = default;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                compositingMode = g.CompositeMode;
                return Ok;
            }

            internal static int GdipSetCompositingQuality(HandleRef graphics, CompositingQuality quality)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                g.CompositeQuality = quality;
                return Ok;
            }

            internal static int GdipGetCompositingQuality(HandleRef graphics, out CompositingQuality quality)
            {
                quality = default;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                quality = g.CompositeQuality;
                return Ok;
            }

            internal static int GdipGetNearestColor(IntPtr graphics, ref int argb) => Ok;

            internal static int GdipSetPageScale(HandleRef graphics, float scale)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (scale <= 0.0 || scale > 1000000032)
                    return InvalidParameter;
                g.PageScale = scale;
                return Ok;
            }

            internal static int GdipGetPageScale(HandleRef graphics, out float scale)
            {
                scale = 0;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                scale = g.PageScale;
                return Ok;
            }

            internal static int GdipSetPageUnit(HandleRef graphics, GraphicsUnit unit)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (unit <= GraphicsUnit.World || unit > GraphicsUnit.Millimeter + 1)
                    return InvalidParameter;
                g.PageUnit = unit;
                return Ok;
            }

            internal static int GdipGetPageUnit(HandleRef graphics, out GraphicsUnit unit)
            {
                unit = default;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                unit = g.PageUnit;
                return Ok;
            }

            // Clipping

            private static int SetClip(GpGraphics g, GpRegion region, CombineMode mode)
            {
                GpRegion work = region;
                if (!g.ClipMatrix.IsIdentity)
                {
                    GpMatrix inverted = g.ClipMatrix.Clone();
                    inverted.Invert();
                    work = region.Clone();
                    work.Transform(inverted);
                }
                g.Clip.Combine(work, mode);
                g.CalculateOverallClipping();
                if (work != region)
                    work.Dispose();
                return Ok;
            }

            internal static int GdipSetClipGraphics(HandleRef graphics, HandleRef srcgraphics, CombineMode mode)
            {
                if (!Is(graphics, out GpGraphics? g) || !Is(srcgraphics, out GpGraphics? src))
                    return InvalidParameter;
                if (g.Busy || src.Busy)
                    return ObjectBusy;
                return SetClip(g, src.Clip, mode);
            }

            internal static int GdipSetClipRect(HandleRef graphics, float x, float y, float width, float height, CombineMode mode)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (mode > CombineMode.Complement)
                    return InvalidParameter;
                var rect = new RectangleF(x, y, width, height);
                if (rect.Width < 0) { rect.X += rect.Width; rect.Width = -rect.Width; }
                if (rect.Height < 0) { rect.Y += rect.Height; rect.Height = -rect.Height; }
                using GpRegion region = rect.Width == 0 || rect.Height == 0 ? GpRegion.Empty() : GpRegion.FromRect(rect);
                return SetClip(g, region, mode);
            }

            internal static int GdipSetClipRectI(HandleRef graphics, int x, int y, int width, int height, CombineMode mode) =>
                GdipSetClipRect(graphics, x, y, width, height, mode);

            internal static int GdipSetClipPath(HandleRef graphics, HandleRef path, CombineMode mode)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (!Is(path, out GpPath? p) || mode > CombineMode.Complement)
                    return InvalidParameter;
                using GpRegion region = GpRegion.FromPath(p);
                return SetClip(g, region, mode);
            }

            internal static int GdipSetClipRegion(HandleRef graphics, HandleRef region, CombineMode mode)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (!Is(region, out GpRegion? r) || mode > CombineMode.Complement)
                    return InvalidParameter;
                return SetClip(g, r, mode);
            }

            internal static int GdipResetClip(HandleRef graphics)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                g.ResetClip();
                return Ok;
            }

            internal static int GdipTranslateClip(HandleRef graphics, float dx, float dy)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                g.Clip.Translate(dx, dy);
                g.CalculateOverallClipping();
                return Ok;
            }

            internal static int GdipGetClip(HandleRef graphics, HandleRef region)
            {
                if (!Is(graphics, out GpGraphics? g) || !Is(region, out GpRegion? r))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                r.CopyFrom(g.Clip);
                if (!g.ClipMatrix.IsIdentity)
                    r.Transform(g.ClipMatrix);
                return Ok;
            }

            internal static int GdipGetClipBounds(HandleRef graphics, out RectangleF rect)
            {
                rect = default;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                if (g.Clip.IsEmpty)
                {
                    rect = g.Clip.Bounds;
                    rect.X += (float)g.ClipMatrix.X0;
                    rect.Y += (float)g.ClipMatrix.Y0;
                    return Ok;
                }
                if (g.ClipMatrix.IsIdentity)
                {
                    rect = g.Clip.Bounds;
                    return Ok;
                }
                using GpRegion work = g.Clip.Clone();
                work.Transform(g.ClipMatrix);
                rect = work.Bounds;
                return Ok;
            }

            internal static int GdipIsClipEmpty(HandleRef graphics, out bool result)
            {
                result = false;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                result = g.Clip.IsEmpty;
                return Ok;
            }

            private static GpRegion VisibleClip(GpGraphics g)
            {
                if (g.OverallClip.IsEmpty)
                    return g.OverallClip.Clone();
                if (!g.OverallClip.IsInfinite)
                {
                    GpRegion clip = g.OverallClip.Clone();
                    if (!g.ClipMatrix.IsIdentity)
                        clip.Transform(g.ClipMatrix);
                    using GpRegion bounds = GpRegion.FromRect(g.Bounds);
                    clip.Combine(bounds, CombineMode.Intersect);
                    return clip;
                }
                return GpRegion.FromRect(g.Bounds);
            }

            internal static int GdipGetVisibleClipBounds(HandleRef graphics, out RectangleF rect)
            {
                rect = default;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                GpRegion clip = g.OverallClip;
                if (clip.IsEmpty)
                {
                    rect = clip.Bounds;
                    rect.X += (float)g.ClipMatrix.X0;
                    rect.Y += (float)g.ClipMatrix.Y0;
                }
                else if (!clip.IsInfinite)
                {
                    RectangleF clipBound;
                    if (!g.ClipMatrix.IsIdentity)
                    {
                        using GpRegion work = clip.Clone();
                        work.Transform(g.ClipMatrix);
                        clipBound = work.Bounds;
                    }
                    else
                    {
                        clipBound = clip.Bounds;
                    }
                    Rectangle b = g.Bounds;
                    rect.X = Math.Max(clipBound.X, b.X);
                    rect.Y = Math.Max(clipBound.Y, b.Y);
                    rect.Width = Math.Min(clipBound.Right, b.Right) - rect.X;
                    rect.Height = Math.Min(clipBound.Bottom, b.Bottom) - rect.Y;
                }
                else
                {
                    rect = g.Bounds;
                }
                return Ok;
            }

            internal static int GdipIsVisibleClipEmpty(HandleRef graphics, out bool result)
            {
                result = false;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (g.Busy)
                    return ObjectBusy;
                GdipGetVisibleClipBounds(graphics, out RectangleF bounds);
                result = bounds.Width == 0 || bounds.Height == 0;
                return Ok;
            }

            internal static int GdipIsVisiblePoint(HandleRef graphics, float x, float y, out bool result)
            {
                result = false;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                using GpRegion clip = VisibleClip(g);
                result = clip.Contains(x, y);
                return Ok;
            }

            internal static int GdipIsVisiblePointI(HandleRef graphics, int x, int y, out bool result) => GdipIsVisiblePoint(graphics, x, y, out result);

            internal static int GdipIsVisibleRect(HandleRef graphics, float x, float y, float width, float height, out bool result)
            {
                result = false;
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                if (width == 0 || height == 0)
                    return Ok;
                using GpRegion clip = VisibleClip(g);
                using GpRegion rect = GpRegion.FromRect(new RectangleF(x, y, width, height));
                clip.Combine(rect, CombineMode.Intersect);
                result = !clip.IsEmpty;
                return Ok;
            }

            internal static int GdipIsVisibleRectI(HandleRef graphics, int x, int y, int width, int height, out bool result) =>
                GdipIsVisibleRect(graphics, x, y, width, height, out result);

            // Drawing

            /// <summary>A path stroked with a pen (gdip_pen_setup, cairo_stroke).</summary>
            private static int Stroke(GpGraphics g, GpPen pen, SKPath path)
            {
                SKCanvas canvas = g.Begin();
                try
                {
                    using SKPaint paint = g.NewPaint(g.Antialias);
                    int status = pen.Brush.Setup(paint, g);
                    if (status != Ok)
                        return status;
                    paint.Style = SKPaintStyle.Stroke;
                    // The stroke's space: the pen's transform, then the world's (and the page's).
                    SKMatrix device = canvas.TotalMatrix;
                    SKMatrix strokeSpace = device;
                    bool penTransform = !pen.Matrix.IsIdentity;
                    if (penTransform)
                    {
                        GpMatrix product = GpMatrix.Multiply(pen.Matrix, g.World);
                        if (NearZero(product.XX) || NearZero(product.YY))
                            product.XX = product.YY = 0.0001f;
                        SKMatrix m = SKMatrix.Concat(g.PageMatrix, product.ToSK());
                        float o = g.DeviceOffset;
                        strokeSpace = o == 0 ? m : SKMatrix.Concat(SKMatrix.CreateTranslation(o, o), m);
                    }
                    // A pen thinner than a pixel draws a pixel wide line.
                    float minimum = 1;
                    if (strokeSpace.TryInvert(out SKMatrix inverse))
                    {
                        SKPoint d = inverse.MapVector(1, 1);
                        minimum = Math.Max(Math.Abs(d.X), Math.Abs(d.Y));
                    }
                    float width = pen.Width > minimum ? pen.Width : minimum;
                    paint.StrokeWidth = width;
                    paint.StrokeMiter = pen.MiterLimit;
                    paint.StrokeJoin = pen.Join;
                    paint.StrokeCap = pen.Cap;
                    if (pen.DashArray != null && pen.DashArray.Length > 0)
                    {
                        float[] intervals = pen.DashArray.Length % 2 == 0 ? new float[pen.DashArray.Length] : new float[pen.DashArray.Length * 2];
                        for (int i = 0; i < intervals.Length; i++)
                            intervals[i] = Math.Max(pen.DashArray[i % pen.DashArray.Length] * width, 0.0001f);
                        paint.PathEffect = SKPathEffect.CreateDash(intervals, pen.DashOffset * width);
                    }
                    if (penTransform)
                    {
                        using var devicePath = new SKPath(path);
                        devicePath.Transform(device);
                        if (strokeSpace.TryInvert(out SKMatrix toStroke))
                            devicePath.Transform(toStroke);
                        canvas.SetMatrix(strokeSpace);
                        canvas.DrawPath(devicePath, paint);
                    }
                    else
                    {
                        canvas.DrawPath(path, paint);
                    }
                    paint.PathEffect?.Dispose();
                    paint.Shader?.Dispose();
                    return Ok;
                }
                finally
                {
                    g.End();
                }
            }

            /// <summary>A path filled with a brush.</summary>
            private static int Fill(GpGraphics g, GpBrush brush, SKPath path)
            {
                SKCanvas canvas = g.Begin();
                try
                {
                    using SKPaint paint = g.NewPaint(g.Antialias);
                    int status = brush.Setup(paint, g);
                    if (status != Ok)
                        return status;
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawPath(path, paint);
                    paint.Shader?.Dispose();
                    return Ok;
                }
                finally
                {
                    g.End();
                }
            }

            private static bool Drawable(HandleRef graphics, out GpGraphics g, out int status)
            {
                status = Ok;
                if (!Is(graphics, out GpGraphics? gg))
                {
                    g = null!;
                    status = InvalidParameter;
                    return false;
                }
                g = gg;
                if (g.Busy)
                {
                    status = ObjectBusy;
                    return false;
                }
                return true;
            }

            private static PointF[] Points(PointF* points, int count)
            {
                var result = new PointF[count];
                for (int i = 0; i < count; i++)
                    result[i] = points[i];
                return result;
            }

            private static PointF[] Points(Point* points, int count)
            {
                var result = new PointF[count];
                for (int i = 0; i < count; i++)
                    result[i] = points[i];
                return result;
            }

            private static SKPath Lines(PointF[] points, bool close)
            {
                var path = new SKPath();
                path.MoveTo(points[0].X, points[0].Y);
                for (int i = 1; i < points.Length; i++)
                    path.LineTo(points[i].X, points[i].Y);
                if (close)
                    path.Close();
                return path;
            }

            /// <summary>make_polygon: the points, the first again unless the last is it, closed.</summary>
            private static SKPath Polygon(PointF[] points, FillMode fillMode)
            {
                SKPath path = Lines(points, false);
                if (points[0].X != points[^1].X && points[0].Y != points[^1].Y)
                    path.LineTo(points[0].X, points[0].Y);
                path.Close();
                path.FillType = fillMode == FillMode.Alternate ? SKPathFillType.EvenOdd : SKPathFillType.Winding;
                return path;
            }

            private static SKPath Shape(Action<GpPath> add, FillMode fillMode = FillMode.Alternate)
            {
                var p = new GpPath(fillMode);
                add(p);
                return p.ToSK();
            }

            private static SKPath Curve(PointF[] points, PointF[] tangents, int offset, int length, bool close)
            {
                var path = new SKPath();
                path.MoveTo(points[offset].X, points[offset].Y);
                int i;
                for (i = offset; i < offset + length; i++)
                {
                    int j = i + 1;
                    path.CubicTo(points[i].X + tangents[i].X, points[i].Y + tangents[i].Y,
                        points[j].X - tangents[j].X, points[j].Y - tangents[j].Y, points[j].X, points[j].Y);
                }
                if (close)
                {
                    path.CubicTo(points[i].X + tangents[i].X, points[i].Y + tangents[i].Y,
                        points[0].X - tangents[0].X, points[0].Y - tangents[0].Y, points[0].X, points[0].Y);
                    path.Close();
                }
                return path;
            }

            private static int StrokeWith(HandleRef graphics, HandleRef pen, Func<SKPath?> path)
            {
                if (!Drawable(graphics, out GpGraphics g, out int status))
                    return status;
                if (!Is(pen, out GpPen? p))
                    return InvalidParameter;
                using SKPath? sk = path();
                return sk == null ? Ok : Stroke(g, p, sk);
            }

            private static int FillWith(HandleRef graphics, HandleRef brush, Func<SKPath?> path)
            {
                if (!Drawable(graphics, out GpGraphics g, out int status))
                    return status;
                if (!Is(brush, out GpBrush? b))
                    return InvalidParameter;
                using SKPath? sk = path();
                return sk == null ? Ok : Fill(g, b, sk);
            }

            internal static int GdipDrawArc(HandleRef graphics, HandleRef pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
            {
                if (width <= 0 || height <= 0)
                    return Is(graphics, out GpGraphics? _) ? InvalidParameter : InvalidParameter;
                return StrokeWith(graphics, pen, () => Shape(p => p.AppendArcs(x, y, width, height, startAngle, sweepAngle)));
            }

            internal static int GdipDrawArcI(HandleRef graphics, HandleRef pen, int x, int y, int width, int height, float startAngle, float sweepAngle) =>
                GdipDrawArc(graphics, pen, x, y, width, height, startAngle, sweepAngle);

            internal static int GdipDrawBezier(HandleRef graphics, HandleRef pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) =>
                StrokeWith(graphics, pen, () =>
                {
                    var path = new SKPath();
                    path.MoveTo(x1, y1);
                    path.CubicTo(x2, y2, x3, y3, x4, y4);
                    return path;
                });

            internal static int GdipDrawBeziers(HandleRef graphics, HandleRef pen, PointF* points, int count)
            {
                if (points == null || count <= 0 || (count > 3 && (count % 3) != 1))
                    return InvalidParameter;
                if (count < 3)
                    return Is(graphics, out GpGraphics? _) && Is(pen, out GpPen? _) ? Ok : InvalidParameter;
                PointF[] pts = Points(points, count);
                return StrokeWith(graphics, pen, () =>
                {
                    var path = new SKPath();
                    path.MoveTo(pts[0].X, pts[0].Y);
                    for (int i = 1; i + 2 < count; i += 3)
                        path.CubicTo(pts[i].X, pts[i].Y, pts[i + 1].X, pts[i + 1].Y, pts[i + 2].X, pts[i + 2].Y);
                    return path;
                });
            }

            internal static int GdipDrawBeziersI(HandleRef graphics, HandleRef pen, Point* points, int count)
            {
                if (points == null || count < 0)
                    return InvalidParameter;
                fixed (PointF* p = Points(points, count))
                    return GdipDrawBeziers(graphics, pen, p, count);
            }

            internal static int GdipDrawEllipse(HandleRef graphics, HandleRef pen, float x, float y, float width, float height) =>
                StrokeWith(graphics, pen, () => Shape(p => p.AddEllipse(x, y, width, height)));

            internal static int GdipDrawEllipseI(HandleRef graphics, HandleRef pen, int x, int y, int width, int height) =>
                GdipDrawEllipse(graphics, pen, x, y, width, height);

            internal static int GdipDrawLine(HandleRef graphics, HandleRef pen, float x1, float y1, float x2, float y2)
            {
                PointF* pts = stackalloc PointF[2];
                pts[0] = new PointF(x1, y1);
                pts[1] = new PointF(x2, y2);
                return GdipDrawLines(graphics, pen, pts, 2);
            }

            internal static int GdipDrawLineI(HandleRef graphics, HandleRef pen, int x1, int y1, int x2, int y2) =>
                GdipDrawLine(graphics, pen, x1, y1, x2, y2);

            internal static int GdipDrawLines(HandleRef graphics, HandleRef pen, PointF* points, int count)
            {
                if (points == null || count <= 0)
                    return InvalidParameter;
                if (count < 2)
                    return Is(graphics, out GpGraphics? g) && g.Busy ? ObjectBusy : InvalidParameter;
                PointF[] pts = Points(points, count);
                return StrokeWith(graphics, pen, () => Lines(pts, false));
            }

            internal static int GdipDrawLinesI(HandleRef graphics, HandleRef pen, Point* points, int count)
            {
                if (count < 0)
                    return OutOfMemory;
                if (points == null)
                    return InvalidParameter;
                fixed (PointF* p = Points(points, count))
                    return GdipDrawLines(graphics, pen, p, count);
            }

            internal static int GdipDrawPath(HandleRef graphics, HandleRef pen, HandleRef path)
            {
                if (!Is(path, out GpPath? p))
                    return Is(graphics, out GpGraphics? _) ? InvalidParameter : InvalidParameter;
                return StrokeWith(graphics, pen, () => p.ToSK());
            }

            internal static int GdipDrawPie(HandleRef graphics, HandleRef pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
            {
                if (!Drawable(graphics, out GpGraphics _, out int status))
                    return status;
                if (!Is(pen, out GpPen? _) || width <= 0 || height <= 0)
                    return InvalidParameter;
                if (sweepAngle == 0)
                    return Ok;
                return StrokeWith(graphics, pen, () => Pie(x, y, width, height, startAngle, sweepAngle));
            }

            internal static int GdipDrawPieI(HandleRef graphics, HandleRef pen, int x, int y, int width, int height, float startAngle, float sweepAngle) =>
                GdipDrawPie(graphics, pen, x, y, width, height, startAngle, sweepAngle);

            /// <summary>make_pie: the centre, the arc, the centre again (a float centre, unlike libgdiplus' path).</summary>
            private static SKPath Pie(float x, float y, float width, float height, float startAngle, float sweepAngle)
            {
                float rx = width / 2, ry = height / 2, cx = x + rx, cy = y + ry;
                double alpha = Math.Atan2(rx * Math.Sin(startAngle * PI / 180), ry * Math.Cos(startAngle * PI / 180));
                var p = new GpPath(FillMode.Alternate);
                if (Math.Abs(sweepAngle) >= 360)
                {
                    p.AddEllipse(x, y, width, height);
                }
                else
                {
                    p.Append(cx, cy, PathPointTypeStart, false);
                    p.Append(cx + rx * Math.Cos(alpha), cy + ry * Math.Sin(alpha), PathPointTypeLine, false);
                    p.AppendArcs(x, y, width, height, startAngle, sweepAngle);
                    p.Append(cx, cy, PathPointTypeLine, false);
                    p.CloseFigure();
                }
                return p.ToSK();
            }

            internal static int GdipDrawPolygon(HandleRef graphics, HandleRef pen, PointF* points, int count)
            {
                if (points == null || count <= 0)
                    return InvalidParameter;
                if (count < 2)
                    return InvalidParameter;
                PointF[] pts = Points(points, count);
                return StrokeWith(graphics, pen, () => Polygon(pts, FillMode.Alternate));
            }

            internal static int GdipDrawPolygonI(HandleRef graphics, HandleRef pen, Point* points, int count)
            {
                if (count < 0)
                    return OutOfMemory;
                if (points == null)
                    return InvalidParameter;
                fixed (PointF* p = Points(points, count))
                    return GdipDrawPolygon(graphics, pen, p, count);
            }

            internal static int GdipDrawRectangle(HandleRef graphics, HandleRef pen, float x, float y, float width, float height)
            {
                RectangleF rect = new RectangleF(x, y, width, height);
                return GdipDrawRectangles(graphics, pen, &rect, 1);
            }

            internal static int GdipDrawRectangleI(HandleRef graphics, HandleRef pen, int x, int y, int width, int height) =>
                GdipDrawRectangle(graphics, pen, x, y, width, height);

            internal static int GdipDrawRectangles(HandleRef graphics, HandleRef pen, RectangleF* rects, int count)
            {
                if (rects == null || count <= 0)
                    return InvalidParameter;
                var copy = new RectangleF[count];
                for (int i = 0; i < count; i++)
                    copy[i] = rects[i];
                return StrokeWith(graphics, pen, () =>
                {
                    var path = new SKPath();
                    bool any = false;
                    foreach (RectangleF r in copy)
                    {
                        if (r.Width < 0 || r.Height < 0)
                            continue;
                        path.AddRect(new SKRect(r.Left, r.Top, r.Right, r.Bottom));
                        any = true;
                    }
                    if (!any)
                    {
                        path.Dispose();
                        return null;
                    }
                    return path;
                });
            }

            internal static int GdipDrawRectanglesI(HandleRef graphics, HandleRef pen, Rectangle* rects, int count)
            {
                if (rects == null || count < 0)
                    return InvalidParameter;
                var copy = new RectangleF[count];
                for (int i = 0; i < count; i++)
                    copy[i] = rects[i];
                fixed (RectangleF* r = copy)
                    return GdipDrawRectangles(graphics, pen, r, count);
            }

            internal static int GdipDrawClosedCurve(HandleRef graphics, HandleRef pen, PointF* points, int count) =>
                GdipDrawClosedCurve2(graphics, pen, points, count, 0.5f);

            internal static int GdipDrawClosedCurveI(HandleRef graphics, HandleRef pen, Point* points, int count) =>
                GdipDrawClosedCurve2I(graphics, pen, points, count, 0.5f);

            internal static int GdipDrawClosedCurve2(HandleRef graphics, HandleRef pen, PointF* points, int count, float tension)
            {
                if (points == null || count <= 0)
                    return InvalidParameter;
                if (count < 3)
                    return InvalidParameter;
                if (tension == 0)
                    return GdipDrawPolygon(graphics, pen, points, count);
                PointF[] pts = Points(points, count);
                return StrokeWith(graphics, pen, () => Curve(pts, ClosedCurveTangents(pts, count, tension), 0, count - 1, true));
            }

            internal static int GdipDrawClosedCurve2I(HandleRef graphics, HandleRef pen, Point* points, int count, float tension)
            {
                if (count < 0)
                    return OutOfMemory;
                if (points == null)
                    return InvalidParameter;
                fixed (PointF* p = Points(points, count))
                    return GdipDrawClosedCurve2(graphics, pen, p, count, tension);
            }

            internal static int GdipDrawCurve(HandleRef graphics, HandleRef pen, PointF* points, int count) =>
                GdipDrawCurve2(graphics, pen, points, count, 0.5f);

            internal static int GdipDrawCurveI(HandleRef graphics, HandleRef pen, Point* points, int count) =>
                GdipDrawCurve2I(graphics, pen, points, count, 0.5f);

            internal static int GdipDrawCurve2(HandleRef graphics, HandleRef pen, PointF* points, int count, float tension)
            {
                if (count == 2)
                    return GdipDrawLines(graphics, pen, points, count);
                return GdipDrawCurve3(graphics, pen, points, count, 0, count - 1, tension);
            }

            internal static int GdipDrawCurve2I(HandleRef graphics, HandleRef pen, Point* points, int count, float tension)
            {
                if (count < 0)
                    return OutOfMemory;
                if (points == null)
                    return InvalidParameter;
                fixed (PointF* p = Points(points, count))
                    return GdipDrawCurve2(graphics, pen, p, count, tension);
            }

            internal static int GdipDrawCurve3(HandleRef graphics, HandleRef pen, PointF* points, int count, int offset, int numberOfSegments, float tension)
            {
                if (points == null || count <= 0)
                    return InvalidParameter;
                if (count < 2 || offset < 0 || offset >= count)
                    return InvalidParameter;
                if (numberOfSegments < 1 || numberOfSegments >= count - offset)
                    return InvalidParameter;
                if (tension == 0)
                    return GdipDrawLines(graphics, pen, points, count);
                PointF[] pts = Points(points, count);
                return StrokeWith(graphics, pen, () => Curve(pts, OpenCurveTangents(pts, count, tension), offset, numberOfSegments, false));
            }

            internal static int GdipDrawCurve3I(HandleRef graphics, HandleRef pen, Point* points, int count, int offset, int numberOfSegments, float tension)
            {
                if (count < 0)
                    return OutOfMemory;
                if (points == null)
                    return InvalidParameter;
                fixed (PointF* p = Points(points, count))
                    return GdipDrawCurve3(graphics, pen, p, count, offset, numberOfSegments, tension);
            }

            // Fills

            internal static int GdipFillEllipse(HandleRef graphics, HandleRef brush, float x, float y, float width, float height) =>
                FillWith(graphics, brush, () => Shape(p => p.AddEllipse(x, y, width, height)));

            internal static int GdipFillEllipseI(HandleRef graphics, HandleRef brush, int x, int y, int width, int height) =>
                GdipFillEllipse(graphics, brush, x, y, width, height);

            internal static int GdipFillRectangle(HandleRef graphics, HandleRef brush, float x, float y, float width, float height)
            {
                RectangleF rect = new RectangleF(x, y, width, height);
                return GdipFillRectangles(graphics, brush, &rect, 1);
            }

            internal static int GdipFillRectangleI(HandleRef graphics, HandleRef brush, int x, int y, int width, int height) =>
                GdipFillRectangle(graphics, brush, x, y, width, height);

            internal static int GdipFillRectangles(HandleRef graphics, HandleRef brush, RectangleF* rects, int count)
            {
                if (rects == null || count <= 0)
                    return InvalidParameter;
                var copy = new RectangleF[count];
                for (int i = 0; i < count; i++)
                    copy[i] = rects[i];
                return FillWith(graphics, brush, () =>
                {
                    var path = new SKPath();
                    bool any = false;
                    foreach (RectangleF r in copy)
                    {
                        if (r.Width < 0 || r.Height < 0)
                            continue;
                        path.AddRect(new SKRect(r.Left, r.Top, r.Right, r.Bottom));
                        any = true;
                    }
                    if (!any)
                    {
                        path.Dispose();
                        return null;
                    }
                    return path;
                });
            }

            internal static int GdipFillRectanglesI(HandleRef graphics, HandleRef brush, Rectangle* rects, int count)
            {
                if (count < 0)
                    return OutOfMemory;
                if (rects == null)
                    return InvalidParameter;
                var copy = new RectangleF[count];
                for (int i = 0; i < count; i++)
                    copy[i] = rects[i];
                fixed (RectangleF* r = copy)
                    return GdipFillRectangles(graphics, brush, r, count);
            }

            internal static int GdipFillPie(HandleRef graphics, HandleRef brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
            {
                if (!Drawable(graphics, out GpGraphics _, out int status))
                    return status;
                if (!Is(brush, out GpBrush? _) || width <= 0 || height <= 0)
                    return InvalidParameter;
                if (sweepAngle == 0)
                    return Ok;
                return FillWith(graphics, brush, () => Pie(x, y, width, height, startAngle, sweepAngle));
            }

            internal static int GdipFillPieI(HandleRef graphics, HandleRef brush, int x, int y, int width, int height, float startAngle, float sweepAngle) =>
                GdipFillPie(graphics, brush, x, y, width, height, startAngle, sweepAngle);

            internal static int GdipFillPath(IntPtr graphics, IntPtr brush, IntPtr path)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                return FillWith(new HandleRef(null, graphics), new HandleRef(null, brush), () => p.ToSK());
            }

            internal static int GdipFillPolygon(HandleRef graphics, HandleRef brush, PointF* points, int count, FillMode brushMode)
            {
                if (points == null || count <= 0)
                    return InvalidParameter;
                if (!Drawable(graphics, out GpGraphics _, out int status))
                    return status;
                if (!Is(brush, out GpBrush? _) || brushMode > FillMode.Winding)
                    return InvalidParameter;
                if (count < 2)
                    return Ok;
                PointF[] pts = Points(points, count);
                return FillWith(graphics, brush, () => Polygon(pts, brushMode));
            }

            internal static int GdipFillPolygonI(HandleRef graphics, HandleRef brush, Point* points, int count, FillMode brushMode)
            {
                if (count < 0)
                    return OutOfMemory;
                if (points == null)
                    return InvalidParameter;
                fixed (PointF* p = Points(points, count))
                    return GdipFillPolygon(graphics, brush, p, count, brushMode);
            }

            internal static int GdipFillPolygon2(HandleRef graphics, HandleRef brush, PointF* points, int count) =>
                GdipFillPolygon(graphics, brush, points, count, FillMode.Alternate);

            internal static int GdipFillPolygon2I(HandleRef graphics, HandleRef brush, Point* points, int count) =>
                GdipFillPolygonI(graphics, brush, points, count, FillMode.Alternate);

            internal static int GdipFillClosedCurve(HandleRef graphics, HandleRef brush, PointF* points, int count) =>
                GdipFillClosedCurve2(graphics, brush, points, count, 0.5f, FillMode.Alternate);

            internal static int GdipFillClosedCurveI(HandleRef graphics, HandleRef brush, Point* points, int count) =>
                GdipFillClosedCurve2I(graphics, brush, points, count, 0.5f, FillMode.Alternate);

            internal static int GdipFillClosedCurve2(HandleRef graphics, HandleRef brush, PointF* points, int count, float tension, FillMode mode)
            {
                if (points == null || count <= 0)
                    return InvalidParameter;
                if (!Drawable(graphics, out GpGraphics _, out int status))
                    return status;
                if (!Is(brush, out GpBrush? _) || mode > FillMode.Winding)
                    return InvalidParameter;
                if (count < 3)
                    return Ok;
                if (tension == 0)
                    return GdipFillPolygon(graphics, brush, points, count, FillMode.Alternate);
                PointF[] pts = Points(points, count);
                return FillWith(graphics, brush, () =>
                {
                    SKPath path = Curve(pts, ClosedCurveTangents(pts, count, tension), 0, count - 1, true);
                    path.FillType = mode == FillMode.Alternate ? SKPathFillType.EvenOdd : SKPathFillType.Winding;
                    return path;
                });
            }

            internal static int GdipFillClosedCurve2I(HandleRef graphics, HandleRef brush, Point* points, int count, float tension, FillMode mode)
            {
                if (count < 0)
                    return OutOfMemory;
                if (points == null)
                    return InvalidParameter;
                fixed (PointF* p = Points(points, count))
                    return GdipFillClosedCurve2(graphics, brush, p, count, tension, mode);
            }

            internal static int GdipFillRegion(HandleRef graphics, HandleRef brush, HandleRef region)
            {
                if (!Is(region, out GpRegion? r))
                    return InvalidParameter;
                if (r.IsEmpty)
                    return Is(graphics, out GpGraphics? _) && Is(brush, out GpBrush? _) ? Ok : InvalidParameter;
                return FillWith(graphics, brush, () =>
                {
                    if (r.IsInfinite && Is(graphics, out GpGraphics? g))
                    {
                        // The whole target, in world coordinates.
                        var path = new SKPath();
                        path.AddRect(new SKRect(-1e6f, -1e6f, 1e6f, 1e6f));
                        return path;
                    }
                    return r.ToSK();
                });
            }

            internal static int GdipGraphicsClear(HandleRef graphics, int argb)
            {
                if (!Is(graphics, out GpGraphics? g))
                    return InvalidParameter;
                SKCanvas canvas = g.Begin();
                try
                {
                    canvas.ResetMatrix();
                    using var paint = new SKPaint { Color = ToSKColor(argb), BlendMode = SKBlendMode.Src, IsAntialias = false };
                    canvas.DrawPaint(paint);
                }
                finally
                {
                    g.End();
                }
                return Ok;
            }
        }
    }
}
