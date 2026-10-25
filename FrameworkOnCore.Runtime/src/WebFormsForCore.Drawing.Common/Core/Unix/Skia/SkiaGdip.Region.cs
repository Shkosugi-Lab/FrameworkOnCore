// WebFormsForCore: GDI+'s Region: rectangles while it is made of rectangles (as libgdiplus' RegionTypeRect), a Skia path
// otherwise (its path tree and bitmap: Skia's path operations combine them). Infinite is GDI+'s rectangle of
// -4194304, 8388608.
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            private const float InfiniteStart = -4194304, InfiniteSize = 8388608;
            private static readonly RectangleF s_infinite = new RectangleF(InfiniteStart, InfiniteStart, InfiniteSize, InfiniteSize);

            internal sealed class GpRegion : IDisposable
            {
                /// <summary>The rectangles, while it is made of them (null when it is a path).</summary>
                public List<RectangleF>? Rects;
                public SKPath? Path;

                public static GpRegion Infinite() => new GpRegion { Rects = new List<RectangleF> { s_infinite } };

                public static GpRegion Empty() => new GpRegion { Rects = new List<RectangleF>() };

                public static GpRegion FromRect(RectangleF rect) => new GpRegion { Rects = new List<RectangleF> { Normalize(rect) } };

                public static GpRegion FromPath(GpPath path)
                {
                    var sk = path.ToSK();
                    return new GpRegion { Path = sk };
                }

                public bool IsInfinite => Rects != null && Rects.Count == 1 && Rects[0] == s_infinite;

                public GpRegion Clone() => new GpRegion { Rects = Rects == null ? null : new List<RectangleF>(Rects), Path = Path == null ? null : new SKPath(Path) };

                public void CopyFrom(GpRegion other)
                {
                    Path?.Dispose();
                    Rects = other.Rects == null ? null : new List<RectangleF>(other.Rects);
                    Path = other.Path == null ? null : new SKPath(other.Path);
                }

                public void SetInfinite() => CopyFrom(Infinite());

                public void SetEmpty() => CopyFrom(Empty());

                public SKPath ToSK()
                {
                    if (Path != null)
                        return new SKPath(Path);
                    var sk = new SKPath();
                    foreach (RectangleF r in Rects!)
                        sk.AddRect(new SKRect(r.Left, r.Top, r.Right, r.Bottom));
                    return sk;
                }

                public bool IsEmpty
                {
                    get
                    {
                        if (Rects != null)
                        {
                            foreach (RectangleF r in Rects)
                                if (r.Width > 0 && r.Height > 0)
                                    return false;
                            return true;
                        }
                        return Path!.IsEmpty || Path.TightBounds.IsEmpty;
                    }
                }

                public RectangleF Bounds
                {
                    get
                    {
                        if (Rects != null)
                        {
                            if (Rects.Count == 0)
                                return RectangleF.Empty;
                            RectangleF b = Rects[0];
                            for (int i = 1; i < Rects.Count; i++)
                                b = RectangleF.Union(b, Rects[i]);
                            return b;
                        }
                        if (Path!.IsEmpty)
                            return RectangleF.Empty;
                        SKRect t = Path.TightBounds;
                        return t.IsEmpty ? RectangleF.Empty : new RectangleF(t.Left, t.Top, t.Width, t.Height);
                    }
                }

                private static RectangleF Normalize(RectangleF r)
                {
                    if (r.Width < 0) { r.X += r.Width; r.Width = -r.Width; }
                    if (r.Height < 0) { r.Y += r.Height; r.Height = -r.Height; }
                    return r;
                }

                public void Combine(GpRegion other, CombineMode mode)
                {
                    if (mode == CombineMode.Replace)
                    {
                        CopyFrom(other);
                        return;
                    }
                    // Rectangles kept as rectangles where they stay so (one each, or infinite or empty on a side).
                    if (Rects != null && other.Rects != null)
                    {
                        if (mode == CombineMode.Intersect)
                        {
                            if (IsInfinite) { CopyFrom(other); return; }
                            if (other.IsInfinite) return;
                            if (Rects.Count <= 1 && other.Rects.Count <= 1)
                            {
                                if (Rects.Count == 0 || other.Rects.Count == 0) { SetEmpty(); return; }
                                RectangleF r = RectangleF.Intersect(Rects[0], other.Rects[0]);
                                Rects = r.Width > 0 && r.Height > 0 ? new List<RectangleF> { r } : new List<RectangleF>();
                                return;
                            }
                        }
                        else if (mode == CombineMode.Union)
                        {
                            if (IsInfinite) return;
                            if (other.IsInfinite) { SetInfinite(); return; }
                            if (other.IsEmpty) return;
                            if (IsEmpty) { CopyFrom(other); return; }
                        }
                        else if (mode == CombineMode.Exclude && other.IsEmpty)
                        {
                            return;
                        }
                    }
                    SKPathOp op = mode switch
                    {
                        CombineMode.Intersect => SKPathOp.Intersect,
                        CombineMode.Union => SKPathOp.Union,
                        CombineMode.Xor => SKPathOp.Xor,
                        CombineMode.Exclude => SKPathOp.Difference,
                        CombineMode.Complement => SKPathOp.ReverseDifference,
                        _ => SKPathOp.Union,
                    };
                    using SKPath a = ToSK(), b = other.ToSK();
                    SKPath result = a.Op(b, op) ?? new SKPath();
                    Path?.Dispose();
                    Rects = null;
                    Path = result;
                }

                public void Transform(GpMatrix matrix)
                {
                    if (matrix.IsIdentity || IsInfinite)
                        return;
                    if (Rects != null && matrix.XY == 0 && matrix.YX == 0)
                    {
                        for (int i = 0; i < Rects.Count; i++)
                        {
                            PointF a = matrix.Transform(new PointF(Rects[i].Left, Rects[i].Top));
                            PointF b = matrix.Transform(new PointF(Rects[i].Right, Rects[i].Bottom));
                            Rects[i] = Normalize(new RectangleF(a.X, a.Y, b.X - a.X, b.Y - a.Y));
                        }
                        return;
                    }
                    SKPath sk = ToSK();
                    sk.Transform(matrix.ToSK());
                    Path?.Dispose();
                    Rects = null;
                    Path = sk;
                }

                public void Translate(float dx, float dy)
                {
                    if (IsInfinite)
                        return;
                    if (Rects != null)
                    {
                        for (int i = 0; i < Rects.Count; i++)
                            Rects[i] = new RectangleF(Rects[i].X + dx, Rects[i].Y + dy, Rects[i].Width, Rects[i].Height);
                        return;
                    }
                    Path!.Offset(dx, dy);
                }

                public bool Contains(float x, float y)
                {
                    if (Rects != null)
                    {
                        foreach (RectangleF r in Rects)
                            if (x >= r.X && x < r.Right && y >= r.Y && y < r.Bottom)
                                return true;
                        return false;
                    }
                    return Path!.Contains(x, y);
                }

                /// <summary>The region as GDI+'s scans: rectangles of whole pixels, row by row.</summary>
                public List<RectangleF> Scans()
                {
                    var scans = new List<RectangleF>();
                    if (IsEmpty)
                        return scans;
                    if (IsInfinite)
                    {
                        scans.Add(s_infinite);
                        return scans;
                    }
                    if (Rects != null && Rects.Count == 1)
                    {
                        scans.Add(Rects[0]);
                        return scans;
                    }
                    using SKPath sk = ToSK();
                    using var region = new SKRegion();
                    SKRect b = sk.Bounds;
                    region.SetPath(sk, new SKRegion(new SKRectI((int)Math.Floor(b.Left) - 1, (int)Math.Floor(b.Top) - 1, (int)Math.Ceiling(b.Right) + 1, (int)Math.Ceiling(b.Bottom) + 1)));
                    using var iterator = region.CreateRectIterator();
                    while (iterator.Next(out SKRectI r))
                        scans.Add(new RectangleF(r.Left, r.Top, r.Width, r.Height));
                    return scans;
                }

                public void Dispose() => Path?.Dispose();
            }

            internal static int GdipCreateRegion(out IntPtr region)
            {
                region = NewHandle(GpRegion.Infinite());
                return Ok;
            }

            internal static int GdipCreateRegionRect(ref RectangleF gprectf, out IntPtr region)
            {
                region = NewHandle(GpRegion.FromRect(gprectf));
                return Ok;
            }

            internal static int GdipCreateRegionRectI(ref Rectangle gprect, out IntPtr region)
            {
                region = NewHandle(GpRegion.FromRect(gprect));
                return Ok;
            }

            internal static int GdipCreateRegionPath(HandleRef path, out IntPtr region)
            {
                region = IntPtr.Zero;
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                region = NewHandle(GpRegion.FromPath(p));
                return Ok;
            }

            internal static int GdipCloneRegion(HandleRef region, out IntPtr cloneregion)
            {
                cloneregion = IntPtr.Zero;
                if (!Is(region, out GpRegion? r))
                    return InvalidParameter;
                cloneregion = NewHandle(r.Clone());
                return Ok;
            }

            internal static int GdipDeleteRegion(HandleRef region) => Delete<GpRegion>(region.Handle);

            internal static int GdipCombineRegionRect(HandleRef region, ref RectangleF gprectf, CombineMode mode)
            {
                if (!Is(region, out GpRegion? r) || mode < CombineMode.Replace || mode > CombineMode.Complement)
                    return InvalidParameter;
                using GpRegion other = GpRegion.FromRect(gprectf);
                r.Combine(other, mode);
                return Ok;
            }

            internal static int GdipCombineRegionRectI(HandleRef region, ref Rectangle gprect, CombineMode mode)
            {
                RectangleF rect = gprect;
                return GdipCombineRegionRect(region, ref rect, mode);
            }

            internal static int GdipCombineRegionPath(HandleRef region, HandleRef path, CombineMode mode)
            {
                if (!Is(region, out GpRegion? r) || !Is(path, out GpPath? p) || mode < CombineMode.Replace || mode > CombineMode.Complement)
                    return InvalidParameter;
                using GpRegion other = GpRegion.FromPath(p);
                r.Combine(other, mode);
                return Ok;
            }

            internal static int GdipCombineRegionRegion(HandleRef region, HandleRef region2, CombineMode mode)
            {
                if (!Is(region, out GpRegion? r) || !Is(region2, out GpRegion? r2) || mode < CombineMode.Replace || mode > CombineMode.Complement)
                    return InvalidParameter;
                r.Combine(r == r2 ? r2.Clone() : r2, mode);
                return Ok;
            }

            internal static int GdipGetRegionBounds(HandleRef region, HandleRef graphics, out RectangleF gprectf)
            {
                gprectf = default;
                if (!Is(region, out GpRegion? r) || !Is(graphics, out GpGraphics? _))
                    return InvalidParameter;
                gprectf = r.Bounds;
                return Ok;
            }

            internal static int GdipIsEmptyRegion(HandleRef region, HandleRef graphics, out int boolean)
            {
                boolean = 0;
                if (!Is(region, out GpRegion? r) || !Is(graphics, out GpGraphics? _))
                    return InvalidParameter;
                boolean = r.IsEmpty ? 1 : 0;
                return Ok;
            }

            internal static int GdipIsInfiniteRegion(HandleRef region, HandleRef graphics, out int boolean)
            {
                boolean = 0;
                if (!Is(region, out GpRegion? r) || !Is(graphics, out GpGraphics? _))
                    return InvalidParameter;
                boolean = r.IsInfinite ? 1 : 0;
                return Ok;
            }

            internal static int GdipIsEqualRegion(HandleRef region, HandleRef region2, HandleRef graphics, out int boolean)
            {
                boolean = 0;
                if (!Is(region, out GpRegion? r) || !Is(region2, out GpRegion? r2) || !Is(graphics, out GpGraphics? _))
                    return InvalidParameter;
                if (r.IsInfinite || r2.IsInfinite)
                {
                    boolean = r.IsInfinite == r2.IsInfinite ? 1 : 0;
                    return Ok;
                }
                using GpRegion x = r.Clone();
                x.Combine(r2, CombineMode.Xor);
                boolean = x.IsEmpty ? 1 : 0;
                return Ok;
            }

            internal static int GdipIsVisibleRegionPoint(HandleRef region, float X, float Y, HandleRef graphics, out int boolean)
            {
                boolean = 0;
                if (!Is(region, out GpRegion? r))
                    return InvalidParameter;
                boolean = r.Contains(X, Y) ? 1 : 0;
                return Ok;
            }

            internal static int GdipIsVisibleRegionPointI(HandleRef region, int X, int Y, HandleRef graphics, out int boolean) =>
                GdipIsVisibleRegionPoint(region, X, Y, graphics, out boolean);

            internal static int GdipIsVisibleRegionRect(HandleRef region, float X, float Y, float width, float height, HandleRef graphics, out int boolean)
            {
                boolean = 0;
                if (!Is(region, out GpRegion? r))
                    return InvalidParameter;
                if (width == 0 || height == 0)
                    return Ok;
                using GpRegion x = r.Clone();
                using GpRegion rect = GpRegion.FromRect(new RectangleF(X, Y, width, height));
                x.Combine(rect, CombineMode.Intersect);
                boolean = x.IsEmpty ? 0 : 1;
                return Ok;
            }

            internal static int GdipIsVisibleRegionRectI(HandleRef region, int X, int Y, int width, int height, HandleRef graphics, out int boolean) =>
                GdipIsVisibleRegionRect(region, X, Y, width, height, graphics, out boolean);

            internal static int GdipTransformRegion(HandleRef region, HandleRef matrix)
            {
                if (!Is(region, out GpRegion? r) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                r.Transform(m);
                return Ok;
            }

            internal static int GdipTranslateRegion(HandleRef region, float dx, float dy)
            {
                if (!Is(region, out GpRegion? r))
                    return InvalidParameter;
                r.Translate(dx, dy);
                return Ok;
            }

            internal static int GdipTranslateRegionI(HandleRef region, int dx, int dy) => GdipTranslateRegion(region, dx, dy);

            internal static int GdipGetRegionScansCount(HandleRef region, out int count, HandleRef matrix)
            {
                count = 0;
                if (!Is(region, out GpRegion? r) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                using GpRegion work = r.Clone();
                work.Transform(m);
                count = work.Scans().Count;
                return Ok;
            }

            internal static int GdipGetRegionScans(HandleRef region, RectangleF* rects, out int count, HandleRef matrix)
            {
                count = 0;
                if (!Is(region, out GpRegion? r) || !Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                using GpRegion work = r.Clone();
                work.Transform(m);
                List<RectangleF> scans = work.Scans();
                if (rects != null)
                    for (int i = 0; i < scans.Count; i++)
                        rects[i] = scans[i];
                count = scans.Count;
                return Ok;
            }
        }
    }
}
