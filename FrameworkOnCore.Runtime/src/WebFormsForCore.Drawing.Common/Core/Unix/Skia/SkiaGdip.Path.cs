// WebFormsForCore: GDI+'s GraphicsPath and GraphicsPathIterator, as libgdiplus' graphics-path.c and graphics-pathiterator.c
// (points and types; arcs, ellipses and curves as its Béziers; flattening as its nr_curve_flatten). What libgdiplus asked of
// Cairo (the points inside a path or its outline) Skia answers.
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
            private const byte PathPointTypeStart = 0;
            private const byte PathPointTypeLine = 1;
            private const byte PathPointTypeBezier = 3;
            private const byte PathPointTypePathTypeMask = 0x07;
            private const byte PathPointTypeDashMode = 0x10;
            private const byte PathPointTypePathMarker = 0x20;
            private const byte PathPointTypeCloseSubpath = 0x80;
            private const int FlattenRecursionLimit = 10;
            private const double C1 = 0.5522847498307933;   // 4/3 (sqrt 2 - 1): GDI+'s quarter of an ellipse (libgdiplus: 0.552285)

            internal sealed class GpPath
            {
                public FillMode FillMode;
                public List<PointF> Points = new List<PointF>();
                public List<byte> Types = new List<byte>();
                public bool StartNewFig = true;

                public GpPath(FillMode fillMode) => FillMode = fillMode;

                public int Count => Points.Count;

                public GpPath Clone()
                {
                    var p = new GpPath(FillMode) { StartNewFig = StartNewFig };
                    p.Points.AddRange(Points);
                    p.Types.AddRange(Types);
                    return p;
                }

                public void Reset()
                {
                    Points.Clear();
                    Types.Clear();
                    FillMode = FillMode.Alternate;
                    StartNewFig = true;
                }

                public bool HasCurve
                {
                    get
                    {
                        foreach (byte t in Types)
                            if (t == PathPointTypeBezier)
                                return true;
                        return false;
                    }
                }

                public bool Closed => Count > 0 && (Types[Count - 1] & PathPointTypeCloseSubpath) == PathPointTypeCloseSubpath;

                /// <summary>gdip_get_first_point_type</summary>
                public byte FirstPointType
                {
                    get
                    {
                        if (StartNewFig || Count == 0)
                            return PathPointTypeStart;
                        return (Types[Count - 1] & PathPointTypeCloseSubpath) != 0 ? PathPointTypeStart : PathPointTypeLine;
                    }
                }

                /// <summary>append</summary>
                public void Append(double x, double y, byte type, bool compress)
                {
                    byte t = type;
                    float fx = (float)x, fy = (float)y;
                    if (StartNewFig)
                    {
                        t = PathPointTypeStart;
                    }
                    else if (Count > 0)
                    {
                        if (compress)
                        {
                            PointF last = Points[Count - 1];
                            if (last.X == fx && last.Y == fy && !Closed)
                            {
                                if ((type & PathPointTypeCloseSubpath) == PathPointTypeCloseSubpath)
                                    Types[Count - 1] |= PathPointTypeCloseSubpath;
                                return;
                            }
                        }
                        if ((Types[Count - 1] & PathPointTypeCloseSubpath) != 0)
                            t = PathPointTypeStart;
                    }
                    Points.Add(new PointF(fx, fy));
                    Types.Add(t);
                    StartNewFig = false;
                }

                public void Append(PointF p, byte type, bool compress) => Append(p.X, p.Y, type, compress);

                public void AppendBezier(double x1, double y1, double x2, double y2, double x3, double y3)
                {
                    Append(x1, y1, PathPointTypeBezier, false);
                    Append(x2, y2, PathPointTypeBezier, false);
                    Append(x3, y3, PathPointTypeBezier, false);
                }

                public void CloseFigure()
                {
                    if (Count > 1)
                        Types[Count - 1] |= PathPointTypeCloseSubpath;
                    StartNewFig = true;
                }

                public void AppendCurve(PointF[] points, PointF[] tangents, int offset, int length, bool close)
                {
                    byte ptype = close || Count == 0 ? PathPointTypeStart : PathPointTypeLine;
                    Append(points[offset], ptype, true);
                    int i;
                    for (i = offset; i < offset + length; i++)
                    {
                        int j = i + 1;
                        AppendBezier(
                            (double)points[i].X + tangents[i].X, (double)points[i].Y + tangents[i].Y,
                            (double)points[j].X - tangents[j].X, (double)points[j].Y - tangents[j].Y,
                            points[j].X, points[j].Y);
                    }
                    if (close)
                    {
                        AppendBezier(
                            (double)points[i].X + tangents[i].X, (double)points[i].Y + tangents[i].Y,
                            (double)points[0].X - tangents[0].X, (double)points[0].Y - tangents[0].Y,
                            points[0].X, points[0].Y);
                        CloseFigure();
                    }
                }

                /// <summary>append_arc</summary>
                public void AppendArc(bool start, float x, float y, float width, float height, float startAngle, float endAngle)
                {
                    float rx = width / 2, ry = height / 2;
                    float cx = x + rx, cy = y + ry;
                    float alpha = (float)(startAngle * PI / 180);
                    float beta = (float)(endAngle * PI / 180);
                    alpha = (float)Math.Atan2(rx * Math.Sin(alpha), ry * Math.Cos(alpha));
                    beta = (float)Math.Atan2(rx * Math.Sin(beta), ry * Math.Cos(beta));
                    if (Math.Abs(beta - alpha) > Math.PI)
                    {
                        if (beta > alpha)
                            beta -= (float)(2 * PI);
                        else
                            alpha -= (float)(2 * PI);
                    }
                    float delta = beta - alpha;
                    float bcp = (float)(4.0 / 3 * (1 - Math.Cos(delta / 2)) / Math.Sin(delta / 2));
                    double sinAlpha = Math.Sin(alpha), sinBeta = Math.Sin(beta), cosAlpha = Math.Cos(alpha), cosBeta = Math.Cos(beta);
                    if (start)
                        Append(cx + rx * cosAlpha, cy + ry * sinAlpha, PathPointTypeLine, false);
                    AppendBezier(
                        cx + rx * (cosAlpha - bcp * sinAlpha), cy + ry * (sinAlpha + bcp * cosAlpha),
                        cx + rx * (cosBeta + bcp * sinBeta), cy + ry * (sinBeta - bcp * cosBeta),
                        cx + rx * cosBeta, cy + ry * sinBeta);
                }

                /// <summary>append_arcs</summary>
                public void AppendArcs(float x, float y, float width, float height, float startAngle, float sweepAngle)
                {
                    if (Math.Abs(sweepAngle) >= 360)
                    {
                        AddEllipse(x, y, width, height);
                        return;
                    }
                    float drawn = 0;
                    float endAngle = startAngle + sweepAngle;
                    int increment = endAngle < startAngle ? -90 : 90;
                    bool enough = false;
                    for (int i = 0; i < 4; i++)
                    {
                        float current = startAngle + drawn;
                        if (enough)
                            return;
                        float additional = endAngle - current;
                        if (Math.Abs(additional) > 90)
                        {
                            additional = increment;
                        }
                        else
                        {
                            if (NearZero(additional))
                                return;
                            enough = true;
                        }
                        AppendArc(i == 0, x, y, width, height, current, current + additional);
                        drawn += additional;
                    }
                }

                /// <summary>GDI+'s ellipse: from its right, clockwise (down first), four quarters (libgdiplus went the other way).</summary>
                public void AddEllipse(float x, float y, float width, float height)
                {
                    double rx = width / 2.0, ry = height / 2.0;
                    double cx = x + rx, cy = y + ry;
                    Append(cx + rx, cy, PathPointTypeStart, false);
                    AppendBezier(cx + rx, cy + C1 * ry, cx + C1 * rx, cy + ry, cx, cy + ry);
                    AppendBezier(cx - C1 * rx, cy + ry, cx - rx, cy + C1 * ry, cx - rx, cy);
                    AppendBezier(cx - rx, cy - C1 * ry, cx - C1 * rx, cy - ry, cx, cy - ry);
                    AppendBezier(cx + C1 * rx, cy - ry, cx + rx, cy - C1 * ry, cx + rx, cy);
                    CloseFigure();
                }

                public void AddRectangle(float x, float y, float width, float height)
                {
                    if (width <= 0.0 || height <= 0.0)
                        return;
                    Append(x, y, PathPointTypeStart, false);
                    Append(x + width, y, PathPointTypeLine, false);
                    Append(x + width, y + height, PathPointTypeLine, false);
                    Append(x, y + height, PathPointTypeLine | PathPointTypeCloseSubpath, false);
                }

                /// <summary>GDI+'s pie: the centre, the arc from its start, closed (back to the centre by the closing).</summary>
                public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
                {
                    float rx = width / 2, ry = height / 2;
                    float cx = x + rx, cy = y + ry;
                    if (Math.Abs(sweepAngle) >= 360)
                    {
                        AddEllipse(x, y, width, height);
                        return;
                    }
                    Append(cx, cy, PathPointTypeStart, false);
                    AppendArcs(x, y, width, height, startAngle, sweepAngle);
                    CloseFigure();
                }

                public void AddPolygon(PointF[] points, int count)
                {
                    // GDI+: the points, closed; a last point that is the first not repeated (libgdiplus added the first again).
                    if (count > 3 && points[0] == points[count - 1])
                        count--;
                    Append(points[0], PathPointTypeStart, false);
                    for (int i = 1; i < count; i++)
                        Append(points[i], PathPointTypeLine, false);
                    CloseFigure();
                }

                public void Transform(GpMatrix? matrix)
                {
                    if (Count == 0 || matrix == null || matrix.IsIdentity)
                        return;
                    for (int i = 0; i < Count; i++)
                        Points[i] = matrix.Transform(Points[i]);
                }

                /// <summary>GdipFlattenPath (the matrix first)</summary>
                public void Flatten(GpMatrix? matrix, float flatness)
                {
                    Transform(matrix);
                    if (!HasCurve)
                        return;
                    var flat = new GpPath(FillMode);
                    for (int i = 0; i < Count; i++)
                    {
                        PointF point = Points[i];
                        byte type = Types[i];
                        if ((type & PathPointTypeBezier) == PathPointTypeBezier)
                        {
                            int before = flat.Count;
                            if (!ConvertBezierToLines(i, Math.Abs(flatness), flat))
                            {
                                flat.Reset();
                                flat.FillMode = FillMode;
                                flat.Append(0, 0, PathPointTypeStart, false);
                                flat.Append(0, 0, PathPointTypeLine, false);
                                flat.Append(0, 0, PathPointTypeLine, false);
                                flat.Append(0, 0, PathPointTypeLine, false);
                                break;
                            }
                            // The Bézier's last point's flags (closing, marker) on its last line.
                            byte flags = (byte)(Types[Math.Min(i + 2, Count - 1)] & (PathPointTypeCloseSubpath | PathPointTypePathMarker));
                            if (flat.Count > before)
                                flat.Types[flat.Count - 1] |= flags;
                            i += 2;
                        }
                        else
                        {
                            flat.Append(point, type, false);
                        }
                    }
                    Points = flat.Points;
                    Types = flat.Types;
                }

                private bool ConvertBezierToLines(int index, float flatness, GpPath flat)
                {
                    if (index <= 0 || index + 2 >= Count)
                        return false;
                    PointF start = Points[index - 1], first = Points[index], second = Points[index + 1], end = Points[index + 2];
                    int saved = flat.Count;
                    if (!CurveFlatten(start.X, start.Y, first.X, first.Y, second.X, second.Y, end.X, end.Y, flatness, 0, flat))
                    {
                        flat.Points.RemoveRange(saved, flat.Count - saved);
                        flat.Types.RemoveRange(saved, flat.Types.Count - saved);
                        return false;
                    }
                    return true;
                }

                /// <summary>nr_curve_flatten (Sodipodi's libnr, public domain; Mono: float, a recursion limit)</summary>
                private static bool CurveFlatten(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, float flatness, int level, GpPath flat)
                {
                    float dx1_0 = x1 - x0, dy1_0 = y1 - y0, dx2_0 = x2 - x0, dy2_0 = y2 - y0, dx3_0 = x3 - x0, dy3_0 = y3 - y0;
                    float dx2_3 = x3 - x2, dy2_3 = y3 - y2;
                    float f2 = flatness;
                    float d3_0_2 = dx3_0 * dx3_0 + dy3_0 * dy3_0;
                    bool subdivide;
                    if (d3_0_2 < f2)
                    {
                        float d1_0_2 = dx1_0 * dx1_0 + dy1_0 * dy1_0;
                        float d2_0_2 = dx2_0 * dx2_0 + dy2_0 * dy2_0;
                        subdivide = !(d1_0_2 < f2 && d2_0_2 < f2);
                    }
                    else
                    {
                        float f2_q = f2 * d3_0_2;
                        float s1_q = dx1_0 * dx3_0 + dy1_0 * dy3_0;
                        float t1_q = dy1_0 * dx3_0 - dx1_0 * dy3_0;
                        float s2_q = dx2_0 * dx3_0 + dy2_0 * dy3_0;
                        float t2_q = dy2_0 * dx3_0 - dx2_0 * dy3_0;
                        float v2_q = dx2_3 * dx3_0 + dy2_3 * dy3_0;
                        subdivide = (t1_q * t1_q) > f2_q || (t2_q * t2_q) > f2_q
                            || (s1_q < 0.0 && (s1_q * s1_q) > f2_q) || (v2_q < 0.0 && (v2_q * v2_q) > f2_q) || s1_q >= s2_q;
                    }
                    if (!subdivide)
                    {
                        flat.Append(x3, y3, PathPointTypeLine, false);
                        return true;
                    }
                    if (level >= FlattenRecursionLimit)
                        return false;
                    float x00t = (x0 + x1) * 0.5f, y00t = (y0 + y1) * 0.5f;
                    float x0tt = (x0 + 2 * x1 + x2) * 0.25f, y0tt = (y0 + 2 * y1 + y2) * 0.25f;
                    float x1tt = (x1 + 2 * x2 + x3) * 0.25f, y1tt = (y1 + 2 * y2 + y3) * 0.25f;
                    float x11t = (x2 + x3) * 0.5f, y11t = (y2 + y3) * 0.5f;
                    float xttt = (x0tt + x1tt) * 0.5f, yttt = (y0tt + y1tt) * 0.5f;
                    if (!CurveFlatten(x0, y0, x00t, y00t, x0tt, y0tt, xttt, yttt, flatness, level + 1, flat)) return false;
                    if (!CurveFlatten(xttt, yttt, x1tt, y1tt, x11t, y11t, x3, y3, flatness, level + 1, flat)) return false;
                    return true;
                }

                /// <summary>gdip_prepare_path</summary>
                public void Prepare(GpMatrix? matrix, float flatness)
                {
                    if (HasCurve)
                        Flatten(matrix, flatness);
                    else if (matrix != null && !matrix.IsIdentity)
                        Transform(matrix);
                }

                /// <summary>The path for Skia (gdip_plot_path), offset (the antialiasing offset) if asked.</summary>
                public SKPath ToSK(float dx = 0, float dy = 0)
                {
                    var sk = new SKPath { FillType = FillMode == FillMode.Alternate ? SKPathFillType.EvenOdd : SKPathFillType.Winding };
                    Plot(sk, dx, dy);
                    return sk;
                }

                public void Plot(SKPath sk, float dx, float dy)
                {
                    int idx = 0;
                    Span<PointF> pts = stackalloc PointF[3];
                    for (int i = 0; i < Count; i++)
                    {
                        PointF pt = Points[i];
                        byte type = Types[i];
                        switch (type & PathPointTypePathTypeMask)
                        {
                            case PathPointTypeStart:
                                sk.MoveTo(pt.X + dx, pt.Y + dy);
                                break;
                            case PathPointTypeLine:
                                sk.LineTo(pt.X + dx, pt.Y + dy);
                                break;
                            case PathPointTypeBezier:
                                if (idx < 3)
                                    pts[idx++] = pt;
                                if (idx == 3)
                                {
                                    sk.CubicTo(pts[0].X + dx, pts[0].Y + dy, pts[1].X + dx, pts[1].Y + dy, pts[2].X + dx, pts[2].Y + dy);
                                    idx = 0;
                                }
                                break;
                        }
                        if ((type & PathPointTypeCloseSubpath) != 0)
                            sk.Close();
                    }
                }

                /// <summary>A Skia path's points and types, as GDI+ has them (conics and quadratics as cubic Béziers).</summary>
                public static GpPath FromSK(SKPath sk, FillMode fillMode)
                {
                    var path = new GpPath(fillMode);
                    using var iterator = sk.CreateRawIterator();
                    var p = new SKPoint[4];
                    SKPathVerb verb;
                    while ((verb = iterator.Next(p)) != SKPathVerb.Done)
                    {
                        switch (verb)
                        {
                            case SKPathVerb.Move:
                                path.StartNewFig = true;
                                path.Append(p[0].X, p[0].Y, PathPointTypeStart, false);
                                break;
                            case SKPathVerb.Line:
                                path.Append(p[1].X, p[1].Y, PathPointTypeLine, false);
                                break;
                            case SKPathVerb.Quad:
                                path.AppendBezier(
                                    p[0].X + 2.0 / 3 * (p[1].X - p[0].X), p[0].Y + 2.0 / 3 * (p[1].Y - p[0].Y),
                                    p[2].X + 2.0 / 3 * (p[1].X - p[2].X), p[2].Y + 2.0 / 3 * (p[1].Y - p[2].Y),
                                    p[2].X, p[2].Y);
                                break;
                            case SKPathVerb.Conic:
                                foreach (SKPoint[] quad in SplitConic(p[0], p[1], p[2], iterator.ConicWeight()))
                                    path.AppendBezier(
                                        quad[0].X + 2.0 / 3 * (quad[1].X - quad[0].X), quad[0].Y + 2.0 / 3 * (quad[1].Y - quad[0].Y),
                                        quad[2].X + 2.0 / 3 * (quad[1].X - quad[2].X), quad[2].Y + 2.0 / 3 * (quad[1].Y - quad[2].Y),
                                        quad[2].X, quad[2].Y);
                                break;
                            case SKPathVerb.Cubic:
                                path.AppendBezier(p[1].X, p[1].Y, p[2].X, p[2].Y, p[3].X, p[3].Y);
                                break;
                            case SKPathVerb.Close:
                                path.CloseFigure();
                                break;
                        }
                    }
                    return path;
                }

                private static IEnumerable<SKPoint[]> SplitConic(SKPoint p0, SKPoint p1, SKPoint p2, float weight)
                {
                    SKPoint[] quads = SKPath.ConvertConicToQuads(p0, p1, p2, weight, 2);
                    for (int i = 0; i + 2 < quads.Length; i += 2)
                        yield return new[] { quads[i], quads[i + 1], quads[i + 2] };
                }

                /// <summary>GdipGetPathWorldBounds</summary>
                public RectangleF WorldBounds(GpMatrix? matrix, GpPen? pen)
                {
                    if (Count <= 1)
                        return RectangleF.Empty;
                    GpPath work = Clone();
                    work.Flatten(matrix, 0.25f);
                    PointF p = work.Points[0];
                    float minX = p.X, minY = p.Y, maxX = p.X, maxY = p.Y;
                    if (work.Count == 1)
                        return new RectangleF(minX, minY, 0, 0);
                    if (matrix != null && !matrix.IsIdentity)
                    {
                        // GDI+: the untransformed bounds' corners, transformed.
                        GpPath plain = Clone();
                        plain.Flatten(null, 0.25f);
                        float l = float.MaxValue, t = float.MaxValue, r = float.MinValue, b = float.MinValue;
                        foreach (PointF q in plain.Points)
                        {
                            l = Math.Min(l, q.X); t = Math.Min(t, q.Y); r = Math.Max(r, q.X); b = Math.Max(b, q.Y);
                        }
                        minX = minY = float.MaxValue;
                        maxX = maxY = float.MinValue;
                        foreach (PointF corner in new[] { new PointF(l, t), new PointF(r, t), new PointF(r, b), new PointF(l, b) })
                        {
                            PointF q = matrix.Transform(corner);
                            minX = Math.Min(minX, q.X); minY = Math.Min(minY, q.Y); maxX = Math.Max(maxX, q.X); maxY = Math.Max(maxY, q.Y);
                        }
                    }
                    for (int i = 1; i < work.Count; i++)
                    {
                        p = work.Points[i];
                        if (p.X < minX) minX = p.X;
                        if (p.Y < minY) minY = p.Y;
                        if (p.X > maxX) maxX = p.X;
                        if (p.Y > maxY) maxY = p.Y;
                    }
                    var bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
                    if (pen != null)
                    {
                        float width = pen.Width < 1.0f ? 1.0f : pen.Width;
                        float halfw = width / 2;
                        bounds.X -= halfw;
                        bounds.Y -= halfw;
                        bounds.Width += width;
                        bounds.Height += width;
                    }
                    return bounds;
                }
            }

            /// <summary>gdip_open_curve_tangents</summary>
            internal static PointF[] OpenCurveTangents(PointF[] points, int count, float tension)
            {
                float coefficient = (float)(tension / 3.0);
                var tangents = new PointF[count];
                if (count <= 2)
                    return tangents;
                for (int i = 0; i < count; i++)
                {
                    int r = i + 1, s = i - 1;
                    if (r >= count) r = count - 1;
                    if (s < 0) s = 0;
                    tangents[i].X += coefficient * (points[r].X - points[s].X);
                    tangents[i].Y += coefficient * (points[r].Y - points[s].Y);
                }
                return tangents;
            }

            /// <summary>gdip_closed_curve_tangents</summary>
            internal static PointF[] ClosedCurveTangents(PointF[] points, int count, float tension)
            {
                float coefficient = (float)(tension / 3.0);
                var tangents = new PointF[count];
                if (count <= 2)
                    return tangents;
                for (int i = 0; i < count; i++)
                {
                    int r = i + 1, s = i - 1;
                    if (r >= count) r -= count;
                    if (s < 0) s += count;
                    tangents[i].X += coefficient * (points[r].X - points[s].X);
                    tangents[i].Y += coefficient * (points[r].Y - points[s].Y);
                }
                return tangents;
            }

            private static PointF[] ToF(Point[] points, int count)
            {
                var result = new PointF[Math.Max(count, 0)];
                for (int i = 0; i < result.Length; i++)
                    result[i] = points[i];
                return result;
            }

            /// <summary>gdip_validate_path_types</summary>
            private static bool ValidPathTypes(byte[] types, int count)
            {
                if (count == 1)
                    return true;
                int i = 0, remaining = count;
                while (true)
                {
                    remaining--; i++;
                    if (remaining == 0) return false;
                    if ((types[i] & PathPointTypePathTypeMask) == PathPointTypeStart) return false;
                    do
                    {
                        int kind = types[i] & PathPointTypePathTypeMask;
                        if (kind == PathPointTypeLine)
                        {
                        }
                        else if (kind == PathPointTypeBezier)
                        {
                            remaining--; i++;
                            if (remaining == 0 || (types[i] & PathPointTypePathTypeMask) != PathPointTypeBezier) return false;
                            remaining--; i++;
                            if (remaining == 0 || (types[i] & PathPointTypePathTypeMask) != PathPointTypeBezier) return false;
                        }
                        else
                        {
                            return false;
                        }
                        bool endOfPath = (types[i] & PathPointTypeCloseSubpath) != 0;
                        remaining--; i++;
                        if (remaining == 0) return true;
                        if (endOfPath) break;
                    } while ((types[i] & PathPointTypePathTypeMask) != PathPointTypeStart);
                }
            }

            internal static int GdipCreatePath(FillMode brushMode, out IntPtr path)
            {
                path = NewHandle(new GpPath(brushMode));
                return Ok;
            }

            private static int CreatePath2(PointF[] points, byte[] types, int count, FillMode fillMode, out IntPtr path)
            {
                path = IntPtr.Zero;
                if (points == null || types == null)
                    return InvalidParameter;
                if (count <= 0 || fillMode > FillMode.Winding)
                    return OutOfMemory;
                var result = new GpPath(fillMode);
                if (ValidPathTypes(types, count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        result.Points.Add(points[i]);
                        result.Types.Add(types[i]);
                    }
                    result.Types[0] = PathPointTypeStart;
                }
                path = NewHandle(result);
                return Ok;
            }

            internal static int GdipCreatePath2(PointF[] points, byte[] types, int count, FillMode brushMode, out IntPtr path) =>
                CreatePath2(points, types, count, brushMode, out path);

            internal static int GdipCreatePath2I(Point[] points, byte[] types, int count, FillMode brushMode, out IntPtr path)
            {
                path = IntPtr.Zero;
                if (points == null || types == null)
                    return InvalidParameter;
                if (count < 0)
                    return OutOfMemory;
                return CreatePath2(ToF(points, count), types, count, brushMode, out path);
            }

            internal static int GdipClonePath(IntPtr path, out IntPtr clonePath)
            {
                clonePath = IntPtr.Zero;
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                clonePath = NewHandle(p.Clone());
                return Ok;
            }

            internal static int GdipDeletePath(HandleRef path) => Delete<GpPath>(path.Handle);

            internal static int GdipResetPath(IntPtr path)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                p.Reset();
                return Ok;
            }

            internal static int GdipGetPointCount(IntPtr path, out int count)
            {
                count = 0;
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                count = p.Count;
                return Ok;
            }

            internal static int GdipGetPathTypes(IntPtr path, byte[] types, int count)
            {
                if (!Is(path, out GpPath? p) || types == null || count <= 0)
                    return InvalidParameter;
                if (count < p.Count)
                    return InsufficientBuffer;
                p.Types.CopyTo(types);
                return Ok;
            }

            internal static int GdipGetPathPoints(IntPtr path, PointF[] points, int count)
            {
                if (!Is(path, out GpPath? p) || points == null || count <= 0)
                    return InvalidParameter;
                if (count < p.Count)
                    return InsufficientBuffer;
                p.Points.CopyTo(points);
                return Ok;
            }

            internal static int GdipGetPathPointsI(IntPtr path, Point[] points, int count)
            {
                if (!Is(path, out GpPath? p) || points == null || count <= 0)
                    return InvalidParameter;
                if (count < p.Count)
                    return InsufficientBuffer;
                for (int i = 0; i < p.Count; i++)
                    points[i] = new Point(IRound(p.Points[i].X), IRound(p.Points[i].Y));
                return Ok;
            }

            internal static int GdipGetPathFillMode(IntPtr path, out FillMode fillMode)
            {
                fillMode = default;
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                fillMode = p.FillMode;
                return Ok;
            }

            internal static int GdipSetPathFillMode(IntPtr path, FillMode fillMode)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                p.FillMode = fillMode;
                return Ok;
            }

            internal static int GdipStartPathFigure(IntPtr path)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                p.StartNewFig = true;
                return Ok;
            }

            internal static int GdipClosePathFigure(IntPtr path)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                p.CloseFigure();
                return Ok;
            }

            internal static int GdipClosePathFigures(IntPtr path)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                if (p.Count > 1)
                {
                    p.Types[p.Count - 1] |= PathPointTypeCloseSubpath;
                    for (int index = 1; index < p.Count; index++)
                        if (p.Types[index] == PathPointTypeStart)
                            p.Types[index - 1] |= PathPointTypeCloseSubpath;
                }
                p.StartNewFig = true;
                return Ok;
            }

            internal static int GdipSetPathMarker(IntPtr path)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                if (p.Count > 1)
                    p.Types[p.Count - 1] |= PathPointTypePathMarker;
                return Ok;
            }

            internal static int GdipClearPathMarkers(IntPtr path)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                for (int i = 0; i < p.Count; i++)
                    p.Types[i] = (byte)(p.Types[i] & ~PathPointTypePathMarker);
                return Ok;
            }

            private static void ReverseSubpathAdjustFlags(int start, int end, List<byte> types, ref bool prevHadMarker)
            {
                if (end != start)
                    for (int i = start; i < end; i++)
                        types[i] = types[i + 1];
                byte prevLast = types[end];
                types[end] = PathPointTypeStart;
                if (end != start)
                    types[end - 1] &= PathPointTypePathTypeMask;
                types[start] |= (byte)(prevLast & (PathPointTypeDashMode | PathPointTypeCloseSubpath));
                if (prevHadMarker)
                    types[start] |= PathPointTypePathMarker;
                else
                    types[start] = (byte)(types[start] & ~PathPointTypePathMarker);
                prevHadMarker = (prevLast & PathPointTypePathMarker) == PathPointTypePathMarker;
            }

            internal static int GdipReversePath(IntPtr path)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                int length = p.Count;
                if (length <= 1)
                    return Ok;
                int start = 0;
                bool prevHadMarker = false;
                for (int i = 1; i < length; i++)
                {
                    if ((p.Types[i] & PathPointTypePathTypeMask) == PathPointTypeStart)
                    {
                        ReverseSubpathAdjustFlags(start, i - 1, p.Types, ref prevHadMarker);
                        start = i;
                    }
                }
                if (start < length - 1)
                    ReverseSubpathAdjustFlags(start, length - 1, p.Types, ref prevHadMarker);
                p.Types.Reverse();
                p.Points.Reverse();
                return Ok;
            }

            internal static int GdipGetPathLastPoint(IntPtr path, out PointF lastPoint)
            {
                lastPoint = default;
                if (!Is(path, out GpPath? p) || p.Count <= 0)
                    return InvalidParameter;
                lastPoint = p.Points[p.Count - 1];
                return Ok;
            }

            internal static int GdipAddPathLine(IntPtr path, float x1, float y1, float x2, float y2)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                p.Append(x1, y1, PathPointTypeLine, true);
                p.Append(x2, y2, PathPointTypeLine, false);
                return Ok;
            }

            internal static int GdipAddPathLineI(IntPtr path, int x1, int y1, int x2, int y2) => GdipAddPathLine(path, x1, y1, x2, y2);

            internal static int GdipAddPathLine2(IntPtr path, PointF[] points, int count)
            {
                if (!Is(path, out GpPath? p) || points == null || count < 0)
                    return InvalidParameter;
                for (int i = 0; i < count; i++)
                    p.Append(points[i], PathPointTypeLine, i == 0);
                return Ok;
            }

            internal static int GdipAddPathLine2I(IntPtr path, Point[] points, int count)
            {
                if (!Is(path, out GpPath? _) || points == null || count < 0)
                    return InvalidParameter;
                return GdipAddPathLine2(path, ToF(points, count), count);
            }

            internal static int GdipAddPathArc(IntPtr path, float x, float y, float width, float height, float startAngle, float sweepAngle)
            {
                if (!Is(path, out GpPath? p) || width <= 0 || height <= 0)
                    return InvalidParameter;
                p.AppendArcs(x, y, width, height, startAngle, sweepAngle);
                return Ok;
            }

            internal static int GdipAddPathArcI(IntPtr path, int x, int y, int width, int height, float startAngle, float sweepAngle) =>
                GdipAddPathArc(path, x, y, width, height, startAngle, sweepAngle);

            internal static int GdipAddPathBezier(IntPtr path, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                p.Append(x1, y1, PathPointTypeLine, true);
                p.AppendBezier(x2, y2, x3, y3, x4, y4);
                return Ok;
            }

            internal static int GdipAddPathBezierI(IntPtr path, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4) =>
                GdipAddPathBezier(path, x1, y1, x2, y2, x3, y3, x4, y4);

            internal static int GdipAddPathBeziers(IntPtr path, PointF[] points, int count)
            {
                if (!Is(path, out GpPath? p) || points == null)
                    return InvalidParameter;
                if (count < 4 || (count % 3) != 1)
                    return InvalidParameter;
                p.Append(points[0], PathPointTypeLine, true);
                for (int i = 1; i < count; i++)
                    p.Append(points[i], PathPointTypeBezier, false);
                return Ok;
            }

            internal static int GdipAddPathBeziersI(IntPtr path, Point[] points, int count)
            {
                if (!Is(path, out GpPath? _) || points == null)
                    return InvalidParameter;
                if (count < 4 || (count % 3) != 1)
                    return InvalidParameter;
                return GdipAddPathBeziers(path, ToF(points, count), count);
            }

            internal static int GdipAddPathCurve(IntPtr path, PointF[] points, int count) => GdipAddPathCurve2(path, points, count, 0.5f);

            internal static int GdipAddPathCurve2(IntPtr path, PointF[] points, int count, float tension)
            {
                if (!Is(path, out GpPath? p) || points == null || count < 2)
                    return InvalidParameter;
                p.AppendCurve(points, OpenCurveTangents(points, count, tension), 0, count - 1, false);
                return Ok;
            }

            internal static int GdipAddPathCurve3(IntPtr path, PointF[] points, int count, int offset, int numberOfSegments, float tension)
            {
                if (!Is(path, out GpPath? p) || points == null || numberOfSegments < 1)
                    return InvalidParameter;
                if (offset == 0 && numberOfSegments == 1 && count < 3)
                    return InvalidParameter;
                if (numberOfSegments >= count - offset)
                    return InvalidParameter;
                p.AppendCurve(points, OpenCurveTangents(points, count, tension), offset, numberOfSegments, false);
                return Ok;
            }

            internal static int GdipAddPathClosedCurve(IntPtr path, PointF[] points, int count) => GdipAddPathClosedCurve2(path, points, count, 0.5f);

            internal static int GdipAddPathClosedCurve2(IntPtr path, PointF[] points, int count, float tension)
            {
                if (!Is(path, out GpPath? p) || points == null || count < 3)
                    return InvalidParameter;
                p.AppendCurve(points, ClosedCurveTangents(points, count, tension), 0, count - 1, true);
                p.CloseFigure();
                return Ok;
            }

            internal static int GdipAddPathCurveI(IntPtr path, Point[] points, int count) => GdipAddPathCurve2I(path, points, count, 0.5f);

            internal static int GdipAddPathCurve2I(IntPtr path, Point[] points, int count, float tension)
            {
                if (points == null)
                    return InvalidParameter;
                return GdipAddPathCurve2(path, ToF(points, count), count, tension);
            }

            internal static int GdipAddPathCurve3I(IntPtr path, Point[] points, int count, int offset, int numberOfSegments, float tension)
            {
                if (points == null)
                    return InvalidParameter;
                return GdipAddPathCurve3(path, ToF(points, count), count, offset, numberOfSegments, tension);
            }

            internal static int GdipAddPathClosedCurveI(IntPtr path, Point[] points, int count) => GdipAddPathClosedCurve2I(path, points, count, 0.5f);

            internal static int GdipAddPathClosedCurve2I(IntPtr path, Point[] points, int count, float tension)
            {
                if (!Is(path, out GpPath? _) || points == null)
                    return InvalidParameter;
                return GdipAddPathClosedCurve2(path, ToF(points, count), count, tension);
            }

            internal static int GdipAddPathRectangle(IntPtr path, float x, float y, float width, float height)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                p.AddRectangle(x, y, width, height);
                return Ok;
            }

            internal static int GdipAddPathRectangleI(IntPtr path, int x, int y, int width, int height) => GdipAddPathRectangle(path, x, y, width, height);

            internal static int GdipAddPathRectangles(IntPtr path, RectangleF[] rects, int count)
            {
                if (!Is(path, out GpPath? p) || rects == null)
                    return InvalidParameter;
                for (int i = 0; i < count; i++)
                    p.AddRectangle(rects[i].X, rects[i].Y, rects[i].Width, rects[i].Height);
                return Ok;
            }

            internal static int GdipAddPathRectanglesI(IntPtr path, Rectangle[] rects, int count)
            {
                if (!Is(path, out GpPath? p) || rects == null)
                    return InvalidParameter;
                for (int i = 0; i < count; i++)
                    p.AddRectangle(rects[i].X, rects[i].Y, rects[i].Width, rects[i].Height);
                return Ok;
            }

            internal static int GdipAddPathEllipse(IntPtr path, float x, float y, float width, float height)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                p.AddEllipse(x, y, width, height);
                return Ok;
            }

            internal static int GdipAddPathEllipseI(IntPtr path, int x, int y, int width, int height) => GdipAddPathEllipse(path, x, y, width, height);

            internal static int GdipAddPathPie(IntPtr path, float x, float y, float width, float height, float startAngle, float sweepAngle)
            {
                if (!Is(path, out GpPath? p) || width <= 0 || height <= 0)
                    return InvalidParameter;
                p.AddPie(x, y, width, height, startAngle, sweepAngle);
                return Ok;
            }

            internal static int GdipAddPathPieI(IntPtr path, int x, int y, int width, int height, float startAngle, float sweepAngle) =>
                GdipAddPathPie(path, x, y, width, height, startAngle, sweepAngle);

            internal static int GdipAddPathPolygon(IntPtr path, PointF[] points, int count)
            {
                if (!Is(path, out GpPath? p) || points == null || count < 3)
                    return InvalidParameter;
                p.AddPolygon(points, count);
                return Ok;
            }

            internal static int GdipAddPathPolygonI(IntPtr path, Point[] points, int count)
            {
                if (!Is(path, out GpPath? p) || points == null || count < 3)
                    return InvalidParameter;
                p.AddPolygon(ToF(points, count), count);
                return Ok;
            }

            internal static int GdipAddPathPath(IntPtr path, IntPtr addingPath, bool connect)
            {
                if (!Is(path, out GpPath? p) || !Is(addingPath, out GpPath? adding))
                    return InvalidParameter;
                if (adding.Count == 0)
                    return Ok;
                int at = p.Count;
                byte first = connect ? p.FirstPointType : PathPointTypeStart;
                var points = adding.Points.ToArray();
                var types = adding.Types.ToArray();
                p.Points.AddRange(points);
                p.Types.AddRange(types);
                p.Types[at] = first;
                p.StartNewFig = false;
                return Ok;
            }

            internal static int GdipFlattenPath(IntPtr path, IntPtr matrix, float floatness)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                p.Flatten(Get<GpMatrix>(matrix), floatness);
                return Ok;
            }

            /// <summary>The path's outline with the pen (Skia's stroke), flattened, the matrix applied (libgdiplus only flattened it).</summary>
            internal static int GdipWidenPath(IntPtr path, IntPtr pen, IntPtr matrix, float flatness)
            {
                if (!Is(path, out GpPath? p) || !Is(pen, out GpPen? gpPen))
                    return InvalidParameter;
                if (p.Count <= 1)
                    return OutOfMemory;
                using SKPath sk = p.ToSK();
                using var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = Math.Max(gpPen.Width, 1),
                    StrokeJoin = gpPen.Join,
                    StrokeCap = gpPen.Cap,
                    StrokeMiter = gpPen.MiterLimit,
                };
                using var outline = new SKPath();
                paint.GetFillPath(sk, outline);
                GpPath widened = GpPath.FromSK(outline, FillMode.Winding);
                widened.Flatten(Get<GpMatrix>(matrix), flatness <= 0 ? 0.25f : flatness);
                p.Points = widened.Points;
                p.Types = widened.Types;
                p.FillMode = FillMode.Winding;
                p.StartNewFig = true;
                return Ok;
            }

            /// <summary>As libgdiplus: the path prepared, not warped (its MonoTODO).</summary>
            internal static int GdipWarpPath(IntPtr path, IntPtr matrix, PointF[] points, int count, float srcx, float srcy, float srcwidth, float srcheight, WarpMode mode, float flatness)
            {
                if (!Is(path, out GpPath? p) || points == null || count < 1)
                    return InvalidParameter;
                if (p.Count == 0)
                    return Ok;
                if ((mode != WarpMode.Perspective && mode != WarpMode.Bilinear) || p.Count == 1)
                {
                    p.Reset();
                    return Ok;
                }
                p.Prepare(Get<GpMatrix>(matrix), flatness);
                return Ok;
            }

            internal static int GdipTransformPath(IntPtr path, IntPtr matrix)
            {
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                p.Transform(Get<GpMatrix>(matrix));
                return Ok;
            }

            internal static int GdipGetPathWorldBounds(IntPtr path, out RectangleF bounds, IntPtr matrix, IntPtr pen)
            {
                bounds = default;
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                bounds = p.WorldBounds(Get<GpMatrix>(matrix), Get<GpPen>(pen));
                return Ok;
            }

            internal static int GdipGetPathWorldBoundsI(IntPtr path, out Rectangle bounds, IntPtr matrix, IntPtr pen)
            {
                bounds = default;
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                RectangleF r = p.WorldBounds(Get<GpMatrix>(matrix), Get<GpPen>(pen));
                bounds = new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
                return Ok;
            }

            /// <summary>As libgdiplus (cairo_in_fill, without antialiasing, 1 and 0.5 to the right and down; the page unit not considered).</summary>
            internal static int GdipIsVisiblePathPoint(IntPtr path, float x, float y, IntPtr graphics, out bool result)
            {
                result = false;
                if (!Is(path, out GpPath? p))
                    return InvalidParameter;
                using SKPath sk = p.ToSK();
                result = sk.Contains(x + 1.0f, y + 0.5f);
                return Ok;
            }

            internal static int GdipIsVisiblePathPointI(IntPtr path, int x, int y, IntPtr graphics, out bool result) =>
                GdipIsVisiblePathPoint(path, x, y, graphics, out result);

            /// <summary>As libgdiplus (cairo_in_stroke, the pen's width less 0.5).</summary>
            internal static int GdipIsOutlineVisiblePathPoint(IntPtr path, float x, float y, IntPtr pen, IntPtr graphics, out bool result)
            {
                result = false;
                if (!Is(path, out GpPath? p) || !Is(pen, out GpPen? gpPen))
                    return InvalidParameter;
                using SKPath sk = p.ToSK();
                using var paint = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = Math.Max(gpPen.Width - 0.5f, 0), IsAntialias = false };
                using var outline = new SKPath();
                paint.GetFillPath(sk, outline);
                result = outline.Contains(x, y);
                return Ok;
            }

            internal static int GdipIsOutlineVisiblePathPointI(IntPtr path, int x, int y, IntPtr pen, IntPtr graphics, out bool result) =>
                GdipIsOutlineVisiblePathPoint(path, x, y, pen, graphics, out result);

            // GraphicsPathIterator

            internal sealed class GpPathIterator
            {
                public GpPath? Path;
                public int MarkerPosition, SubpathPosition, PathTypePosition;
            }

            internal static int GdipCreatePathIter(out IntPtr pathIter, HandleRef path)
            {
                pathIter = NewHandle(new GpPathIterator { Path = Get<GpPath>(path.Handle)?.Clone() });
                return Ok;
            }

            internal static int GdipDeletePathIter(HandleRef pathIter) => Delete<GpPathIterator>(pathIter.Handle);

            internal static int GdipPathIterGetCount(HandleRef pathIter, out int count)
            {
                count = 0;
                if (!Is(pathIter, out GpPathIterator? it))
                    return InvalidParameter;
                count = it.Path?.Count ?? 0;
                return Ok;
            }

            internal static int GdipPathIterGetSubpathCount(HandleRef pathIter, out int count)
            {
                count = 0;
                if (!Is(pathIter, out GpPathIterator? it))
                    return InvalidParameter;
                if (it.Path != null)
                    foreach (byte t in it.Path.Types)
                        if (t == PathPointTypeStart)
                            count++;
                return Ok;
            }

            internal static int GdipPathIterCopyData(HandleRef pathIter, out int resultCount, PointF* points, byte* types, int startIndex, int endIndex)
            {
                resultCount = 0;
                if (!Is(pathIter, out GpPathIterator? it) || points == null || types == null)
                    return InvalidParameter;
                GpPath? p = it.Path;
                if (p == null || startIndex >= p.Count || startIndex > endIndex || endIndex >= p.Count || startIndex < 0 || endIndex < 0)
                    return Ok;
                for (int i = startIndex; i <= endIndex; i++)
                {
                    points[i - startIndex] = p.Points[i];
                    types[i - startIndex] = p.Types[i];
                }
                resultCount = endIndex - startIndex + 1;
                return Ok;
            }

            internal static int GdipPathIterEnumerate(HandleRef pathIter, out int resultCount, PointF* points, byte* types, int count)
            {
                resultCount = 0;
                if (!Is(pathIter, out GpPathIterator? it) || points == null || types == null)
                    return InvalidParameter;
                int i = 0;
                if (it.Path != null)
                    for (; i < count && i < it.Path.Count; i++)
                    {
                        points[i] = it.Path.Points[i];
                        types[i] = it.Path.Types[i];
                    }
                resultCount = i;
                return Ok;
            }

            internal static int GdipPathIterHasCurve(HandleRef pathIter, out bool hasCurve)
            {
                hasCurve = false;
                if (!Is(pathIter, out GpPathIterator? it))
                    return InvalidParameter;
                hasCurve = it.Path?.HasCurve ?? false;
                return Ok;
            }

            internal static int GdipPathIterNextMarker(HandleRef pathIter, out int resultCount, out int startIndex, out int endIndex)
            {
                resultCount = 0; startIndex = 0; endIndex = 0;
                if (!Is(pathIter, out GpPathIterator? it))
                    return InvalidParameter;
                GpPath? p = it.Path;
                if (p == null || p.Count == 0 || it.MarkerPosition >= p.Count)
                    return Ok;
                int index;
                for (index = it.MarkerPosition; index < p.Count; index++)
                {
                    if ((p.Types[index] & PathPointTypePathMarker) != 0)
                    {
                        index++;
                        break;
                    }
                }
                startIndex = it.MarkerPosition;
                endIndex = index - 1;
                resultCount = endIndex - startIndex + 1;
                it.MarkerPosition = index;
                return Ok;
            }

            private static void CopyRange(GpPath from, int start, int count, GpPath to)
            {
                to.Reset();
                to.Points.AddRange(from.Points.GetRange(start, count));
                to.Types.AddRange(from.Types.GetRange(start, count));
                to.StartNewFig = true;   // libgdiplus' GdipResetPath, then the points copied
            }

            internal static int GdipPathIterNextMarkerPath(HandleRef pathIter, out int resultCount, HandleRef path)
            {
                int status = GdipPathIterNextMarker(pathIter, out resultCount, out int start, out _);
                if (status == Ok && resultCount > 0 && Is(path, out GpPath? p))
                    CopyRange(Get<GpPathIterator>(pathIter.Handle)!.Path!, start, resultCount, p);
                return status;
            }

            internal static int GdipPathIterNextPathType(HandleRef pathIter, out int resultCount, out byte pathType, out int startIndex, out int endIndex)
            {
                resultCount = 0; pathType = 0; startIndex = 0; endIndex = 0;
                if (!Is(pathIter, out GpPathIterator? it))
                    return InvalidParameter;
                GpPath? p = it.Path;
                if (p == null || p.Count == 0 || it.SubpathPosition == 0)
                    return Ok;
                if (it.PathTypePosition < it.SubpathPosition)
                {
                    byte lastTypeSeen = (byte)(p.Types[it.PathTypePosition + 1] & PathPointTypePathTypeMask);
                    int index;
                    for (index = it.PathTypePosition + 2; index < it.SubpathPosition; index++)
                        if ((p.Types[index] & PathPointTypePathTypeMask) != lastTypeSeen)
                            break;
                    startIndex = it.PathTypePosition;
                    endIndex = index - 1;
                    resultCount = endIndex - startIndex + 1;
                    pathType = lastTypeSeen;
                    if (lastTypeSeen == PathPointTypeLine && index != it.SubpathPosition)
                        it.PathTypePosition = index - 1;
                    else
                        it.PathTypePosition = index;
                }
                return Ok;
            }

            internal static int GdipPathIterNextSubpath(HandleRef pathIter, out int resultCount, out int startIndex, out int endIndex, out bool isClosed)
            {
                resultCount = 0; startIndex = 0; endIndex = 0; isClosed = false;
                if (!Is(pathIter, out GpPathIterator? it))
                    return InvalidParameter;
                GpPath? p = it.Path;
                if (p == null || p.Count == 0 || it.SubpathPosition == p.Count)
                {
                    isClosed = true;
                    return Ok;
                }
                int index;
                for (index = it.SubpathPosition + 1; index < p.Count; index++)
                    if (p.Types[index] == PathPointTypeStart)
                        break;
                startIndex = it.SubpathPosition;
                endIndex = index - 1;
                resultCount = endIndex - startIndex + 1;
                it.PathTypePosition = it.SubpathPosition;
                it.SubpathPosition = index;
                isClosed = (p.Types[index - 1] & PathPointTypeCloseSubpath) != 0;
                return Ok;
            }

            internal static int GdipPathIterNextSubpathPath(HandleRef pathIter, out int resultCount, HandleRef path, out bool isClosed)
            {
                int status = GdipPathIterNextSubpath(pathIter, out resultCount, out int start, out _, out isClosed);
                if (status == Ok && resultCount > 0 && Is(path, out GpPath? p))
                    CopyRange(Get<GpPathIterator>(pathIter.Handle)!.Path!, start, resultCount, p);
                return status;
            }

            internal static int GdipPathIterRewind(HandleRef pathIter)
            {
                if (!Is(pathIter, out GpPathIterator? it))
                    return InvalidParameter;
                it.MarkerPosition = 0;
                it.SubpathPosition = 0;
                it.PathTypePosition = 0;
                return Ok;
            }
        }
    }
}
