// WebFormsForCore: GDI+'s Matrix, as libgdiplus' matrix.c (a cairo_matrix_t: doubles, its multiplication and inversion).
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            /// <summary>x' = XX x + XY y + X0, y' = YX x + YY y + Y0 (GDI+'s m11 = XX, m12 = YX, m21 = XY, m22 = YY).</summary>
            internal sealed class GpMatrix
            {
                public double XX = 1, YX, XY, YY = 1, X0, Y0;

                public GpMatrix() { }

                public GpMatrix(double xx, double yx, double xy, double yy, double x0, double y0) => Set(xx, yx, xy, yy, x0, y0);

                public void Set(double xx, double yx, double xy, double yy, double x0, double y0)
                {
                    XX = xx; YX = yx; XY = xy; YY = yy; X0 = x0; Y0 = y0;
                }

                public GpMatrix Clone() => new GpMatrix(XX, YX, XY, YY, X0, Y0);

                public void CopyFrom(GpMatrix m) => Set(m.XX, m.YX, m.XY, m.YY, m.X0, m.Y0);

                public static GpMatrix Translation(double x, double y) => new GpMatrix(1, 0, 0, 1, x, y);

                public static GpMatrix Scaling(double x, double y) => new GpMatrix(x, 0, 0, y, 0, 0);

                public static GpMatrix Rotation(double radians)
                {
                    double s = Math.Sin(radians), c = Math.Cos(radians);
                    return new GpMatrix(c, s, -s, c, 0, 0);
                }

                /// <summary>cairo_matrix_multiply: a, then b.</summary>
                public static GpMatrix Multiply(GpMatrix a, GpMatrix b) => new GpMatrix(
                    a.XX * b.XX + a.YX * b.XY,
                    a.XX * b.YX + a.YX * b.YY,
                    a.XY * b.XX + a.YY * b.XY,
                    a.XY * b.YX + a.YY * b.YY,
                    a.X0 * b.XX + a.Y0 * b.XY + b.X0,
                    a.X0 * b.YX + a.Y0 * b.YY + b.Y0);

                /// <summary>GDI+'s Multiply: Append, this then m; Prepend, m then this.</summary>
                public bool Multiply(GpMatrix m, MatrixOrder order)
                {
                    switch (order)
                    {
                        case MatrixOrder.Append: CopyFrom(Multiply(this, m)); return true;
                        case MatrixOrder.Prepend: CopyFrom(Multiply(m, this)); return true;
                        default: return false;
                    }
                }

                public bool HasBoundaryValues =>
                    !double.IsFinite(XX) || !double.IsFinite(XY) || !double.IsFinite(YX) || !double.IsFinite(YY) || !double.IsFinite(X0) || !double.IsFinite(Y0);

                /// <summary>gdip_is_matrix_empty</summary>
                public bool IsIdentity => NearOne(XX) && NearZero(YX) && NearZero(XY) && NearOne(YY) && NearZero(X0) && NearZero(Y0);

                public bool IsTranslation => XX == 1 && YX == 0 && XY == 0 && YY == 1;

                /// <summary>cairo_matrix_invert</summary>
                public bool Invert()
                {
                    if (XY == 0 && YX == 0)
                    {
                        double x0 = -X0, y0 = -Y0, xx = XX, yy = YY;
                        if (xx != 1)
                        {
                            if (xx == 0) return false;
                            xx = 1 / xx;
                            x0 *= xx;
                        }
                        if (yy != 1)
                        {
                            if (yy == 0) return false;
                            yy = 1 / yy;
                            y0 *= yy;
                        }
                        XX = xx; YY = yy; X0 = x0; Y0 = y0;
                        return true;
                    }
                    double det = XX * YY - YX * XY;
                    if (det == 0 || !double.IsFinite(det)) return false;
                    double a = XX, b = YX, c = XY, d = YY, tx = X0, ty = Y0;
                    Set(d, -b, -c, a, c * ty - d * tx, b * tx - a * ty);
                    double s = 1 / det;
                    XX *= s; YX *= s; XY *= s; YY *= s; X0 *= s; Y0 *= s;
                    return true;
                }

                public void Transform(ref double x, ref double y)
                {
                    double nx = XX * x + XY * y + X0;
                    double ny = YX * x + YY * y + Y0;
                    x = nx; y = ny;
                }

                public PointF Transform(PointF p)
                {
                    double x = p.X, y = p.Y;
                    Transform(ref x, ref y);
                    return new PointF((float)x, (float)y);
                }

                public void TransformDistance(ref double x, ref double y)
                {
                    double nx = XX * x + XY * y;
                    double ny = YX * x + YY * y;
                    x = nx; y = ny;
                }

                public SKMatrix ToSK() => new SKMatrix((float)XX, (float)XY, (float)X0, (float)YX, (float)YY, (float)Y0, 0, 0, 1);
            }

            /// <summary>gdip_double_to_float</summary>
            private static float ToFloat(double value)
            {
                if (value != double.PositiveInfinity && value > float.MaxValue) return float.MaxValue;
                if (value != double.NegativeInfinity && value < -float.MaxValue) return -float.MaxValue;
                return (float)value;
            }

            internal static int GdipCreateMatrix(out IntPtr matrix)
            {
                matrix = NewHandle(new GpMatrix());
                return Ok;
            }

            internal static int GdipCreateMatrix2(float m11, float m12, float m21, float m22, float dx, float dy, out IntPtr matrix)
            {
                matrix = NewHandle(new GpMatrix(m11, m12, m21, m22, dx, dy));
                return Ok;
            }

            private static int MatrixFromRect3Points(RectangleF rect, PointF p0, PointF p1, PointF p2, out GpMatrix? matrix)
            {
                matrix = null;
                if (rect.Width == 0 || rect.Height == 0)
                    return OutOfMemory;
                double m11 = (p1.X - p0.X) / rect.Width;
                double m12 = (p1.Y - p0.Y) / rect.Width;
                double m21 = (p2.X - p0.X) / rect.Height;
                double m22 = (p2.Y - p0.Y) / rect.Height;
                matrix = new GpMatrix(m11, m12, m21, m22, p0.X, p0.Y);
                matrix.CopyFrom(GpMatrix.Multiply(GpMatrix.Translation(-rect.X, -rect.Y), matrix));   // cairo_matrix_translate
                return Ok;
            }

            internal static int GdipCreateMatrix3(ref RectangleF rect, PointF* dstplg, out IntPtr matrix)
            {
                matrix = IntPtr.Zero;
                if (dstplg == null)
                    return InvalidParameter;
                int status = MatrixFromRect3Points(rect, dstplg[0], dstplg[1], dstplg[2], out GpMatrix? m);
                if (status == Ok)
                    matrix = NewHandle(m!);
                return status;
            }

            internal static int GdipCreateMatrix3I(ref Rectangle rect, Point* dstplg, out IntPtr matrix)
            {
                matrix = IntPtr.Zero;
                if (dstplg == null)
                    return InvalidParameter;
                int status = MatrixFromRect3Points(rect, dstplg[0], dstplg[1], dstplg[2], out GpMatrix? m);
                if (status == Ok)
                    matrix = NewHandle(m!);
                return status;
            }

            internal static int GdipCloneMatrix(HandleRef matrix, out IntPtr cloneMatrix)
            {
                cloneMatrix = IntPtr.Zero;
                if (!Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                cloneMatrix = NewHandle(m.Clone());
                return Ok;
            }

            internal static int GdipDeleteMatrix(HandleRef matrix) => Delete<GpMatrix>(matrix.Handle);

            internal static int GdipSetMatrixElements(HandleRef matrix, float m11, float m12, float m21, float m22, float dx, float dy)
            {
                if (!Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                m.Set(m11, m12, m21, m22, dx, dy);
                return Ok;
            }

            internal static int GdipGetMatrixElements(HandleRef matrix, float* m)
            {
                if (!Is(matrix, out GpMatrix? x) || m == null)
                    return InvalidParameter;
                m[0] = ToFloat(x.XX);
                m[1] = ToFloat(x.YX);
                m[2] = ToFloat(x.XY);
                m[3] = ToFloat(x.YY);
                m[4] = ToFloat(x.X0);
                m[5] = ToFloat(x.Y0);
                return Ok;
            }

            private static int Multiply(IntPtr matrix, GpMatrix by, MatrixOrder order)
            {
                if (!Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                return m.Multiply(by, order) ? Ok : InvalidParameter;
            }

            internal static int GdipMultiplyMatrix(HandleRef matrix, HandleRef matrix2, MatrixOrder order)
            {
                if (!Is(matrix, out GpMatrix? m) || !Is(matrix2, out GpMatrix? m2))
                    return InvalidParameter;
                if (m == m2)
                    return ObjectBusy;
                return m.Multiply(m2, order) ? Ok : InvalidParameter;
            }

            internal static int GdipTranslateMatrix(HandleRef matrix, float offsetX, float offsetY, MatrixOrder order) =>
                Multiply(matrix.Handle, GpMatrix.Translation(offsetX, offsetY), order);

            internal static int GdipScaleMatrix(HandleRef matrix, float scaleX, float scaleY, MatrixOrder order) =>
                Multiply(matrix.Handle, GpMatrix.Scaling(scaleX, scaleY), order);

            internal static int GdipRotateMatrix(HandleRef matrix, float angle, MatrixOrder order) =>
                Multiply(matrix.Handle, GpMatrix.Rotation(angle * DegToRad), order);

            internal static int GdipShearMatrix(HandleRef matrix, float shearX, float shearY, MatrixOrder order) =>
                Multiply(matrix.Handle, new GpMatrix(1, shearY, shearX, 1, 0, 0), order);

            internal static int GdipInvertMatrix(HandleRef matrix)
            {
                if (!Is(matrix, out GpMatrix? m) || m.HasBoundaryValues)
                    return InvalidParameter;
                return m.Invert() ? Ok : InvalidParameter;   // CAIRO_STATUS_INVALID_MATRIX: gdip_get_status' InvalidParameter
            }

            internal static int GdipTransformMatrixPoints(HandleRef matrix, PointF* pts, int count)
            {
                if (!Is(matrix, out GpMatrix? m) || pts == null || count <= 0)
                    return InvalidParameter;
                for (int i = 0; i < count; i++)
                {
                    double x = pts[i].X, y = pts[i].Y;
                    m.Transform(ref x, ref y);
                    pts[i] = new PointF((float)x, (float)y);
                }
                return Ok;
            }

            internal static int GdipTransformMatrixPointsI(HandleRef matrix, Point* pts, int count)
            {
                if (count < 0)
                    return OutOfMemory;
                if (!Is(matrix, out GpMatrix? m) || pts == null || count == 0)
                    return InvalidParameter;
                for (int i = 0; i < count; i++)
                {
                    double x = pts[i].X, y = pts[i].Y;
                    m.Transform(ref x, ref y);
                    pts[i] = new Point(IRound((float)x), IRound((float)y));
                }
                return Ok;
            }

            internal static int GdipVectorTransformMatrixPoints(HandleRef matrix, PointF* pts, int count)
            {
                if (!Is(matrix, out GpMatrix? m) || pts == null || count <= 0)
                    return InvalidParameter;
                for (int i = 0; i < count; i++)
                {
                    double x = pts[i].X, y = pts[i].Y;
                    m.TransformDistance(ref x, ref y);
                    pts[i] = new PointF((float)x, (float)y);
                }
                return Ok;
            }

            internal static int GdipVectorTransformMatrixPointsI(HandleRef matrix, Point* pts, int count)
            {
                if (!Is(matrix, out GpMatrix? m) || pts == null || count <= 0)
                    return InvalidParameter;
                for (int i = 0; i < count; i++)
                {
                    double x = pts[i].X, y = pts[i].Y;
                    m.TransformDistance(ref x, ref y);
                    pts[i] = new Point(IRound((float)x), IRound((float)y));
                }
                return Ok;
            }

            internal static int GdipIsMatrixInvertible(HandleRef matrix, out int boolean)
            {
                boolean = 0;
                if (!Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                if (m.HasBoundaryValues)
                    return Ok;
                boolean = m.Clone().Invert() ? 1 : 0;
                return Ok;
            }

            internal static int GdipIsMatrixIdentity(HandleRef matrix, out int boolean)
            {
                boolean = 0;
                if (!Is(matrix, out GpMatrix? m))
                    return InvalidParameter;
                boolean = m.IsIdentity ? 1 : 0;
                return Ok;
            }

            internal static int GdipIsMatrixEqual(HandleRef matrix, HandleRef matrix2, out int boolean)
            {
                boolean = 0;
                if (!Is(matrix, out GpMatrix? a) || !Is(matrix2, out GpMatrix? b))
                    return InvalidParameter;
                boolean = a.XX == b.XX && a.YX == b.YX && a.XY == b.XY && a.YY == b.YY && a.X0 == b.X0 && a.Y0 == b.Y0 ? 1 : 0;
                return Ok;
            }
        }
    }
}
