// WebFormsForCore: GDI+'s Region, as GDI+ keeps one: the tree of its combinations (rectangles, paths, empty, infinite;
// combined by intersection, union, xor, exclusion, complement), written and read as GDI+'s region data (MS-EMFPLUS'
// EmfPlusRegion, GetRegionData). What it covers is the tree evaluated: rectangles while it is made of rectangles (as
// libgdiplus' RegionTypeRect), a Skia path otherwise (Skia's path operations combine them). Infinite is GDI+'s rectangle
// of -4194304, 8388608.
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
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

            private const uint RegionNodeRect = 0x10000000, RegionNodePath = 0x10000001, RegionNodeEmpty = 0x10000002, RegionNodeInfinite = 0x10000003;
            private const uint RegionDataVersion = 0xDBC01002;

            internal sealed class GpRegion : IDisposable
            {
                /// <summary>A node of the tree: a leaf (rectangle, path, empty, infinite) or a combination of two.</summary>
                internal sealed class Node
                {
                    public uint Kind;              // RegionNode* or a CombineMode (1..5)
                    public RectangleF Rect;
                    public GpPath? Path;
                    public Node? Left, Right;

                    public Node Clone() => new Node { Kind = Kind, Rect = Rect, Path = Path?.Clone(), Left = Left?.Clone(), Right = Right?.Clone() };

                    public int Combinations => Kind < RegionNodeRect ? 1 + Left!.Combinations + Right!.Combinations : 0;
                }

                internal Node Tree = new Node { Kind = RegionNodeInfinite };

                // What the tree covers, computed when asked: rectangles (Rects), or a path.
                private bool _evaluated;
                private List<RectangleF>? _rects;
                private SKPath? _path;

                public static GpRegion Infinite() => new GpRegion();

                public static GpRegion Empty() => new GpRegion { Tree = new Node { Kind = RegionNodeEmpty } };

                public static GpRegion FromRect(RectangleF rect) => new GpRegion { Tree = new Node { Kind = RegionNodeRect, Rect = rect } };

                public static GpRegion FromPath(GpPath path) => new GpRegion { Tree = new Node { Kind = RegionNodePath, Path = path.Clone() } };

                private void Changed()
                {
                    _evaluated = false;
                    _rects = null;
                    _path?.Dispose();
                    _path = null;
                }

                public GpRegion Clone() => new GpRegion { Tree = Tree.Clone() };

                public void CopyFrom(GpRegion other)
                {
                    Tree = other.Tree.Clone();
                    Changed();
                }

                public void SetInfinite()
                {
                    Tree = new Node { Kind = RegionNodeInfinite };
                    Changed();
                }

                public void SetEmpty()
                {
                    Tree = new Node { Kind = RegionNodeEmpty };
                    Changed();
                }

                public void Combine(GpRegion other, CombineMode mode)
                {
                    if (mode == CombineMode.Replace)
                        Tree = other.Tree.Clone();
                    else
                        Tree = new Node { Kind = (uint)mode, Left = Tree, Right = other.Tree.Clone() };
                    Changed();
                }

                public void Transform(GpMatrix matrix)
                {
                    if (matrix.IsIdentity)
                        return;
                    Transform(Tree, matrix);
                    Changed();
                }

                private static void Transform(Node node, GpMatrix matrix)
                {
                    switch (node.Kind)
                    {
                        case RegionNodeRect:
                            if (matrix.XY == 0 && matrix.YX == 0)
                            {
                                PointF a = matrix.Transform(new PointF(node.Rect.Left, node.Rect.Top));
                                PointF b = matrix.Transform(new PointF(node.Rect.Right, node.Rect.Bottom));
                                node.Rect = Normalize(new RectangleF(a.X, a.Y, b.X - a.X, b.Y - a.Y));
                            }
                            else
                            {
                                var path = new GpPath(FillMode.Alternate);
                                path.AddRectangle(node.Rect.X, node.Rect.Y, node.Rect.Width, node.Rect.Height);
                                path.Transform(matrix);
                                node.Kind = RegionNodePath;
                                node.Path = path;
                            }
                            break;
                        case RegionNodePath:
                            node.Path!.Transform(matrix);
                            break;
                        case RegionNodeEmpty:
                        case RegionNodeInfinite:
                            break;
                        default:
                            Transform(node.Left!, matrix);
                            Transform(node.Right!, matrix);
                            break;
                    }
                }

                public void Translate(float dx, float dy) => Transform(GpMatrix.Translation(dx, dy));

                private static RectangleF Normalize(RectangleF r)
                {
                    if (r.Width < 0) { r.X += r.Width; r.Width = -r.Width; }
                    if (r.Height < 0) { r.Y += r.Height; r.Height = -r.Height; }
                    return r;
                }

                // Evaluation

                /// <summary>What a node covers: rectangles (null when it is a path), or a path.</summary>
                private static (List<RectangleF>? Rects, SKPath? Path) Evaluate(Node node)
                {
                    switch (node.Kind)
                    {
                        case RegionNodeRect:
                            {
                                RectangleF r = Normalize(node.Rect);
                                return (r.Width > 0 && r.Height > 0 ? new List<RectangleF> { r } : new List<RectangleF>(), null);
                            }
                        case RegionNodeEmpty:
                            return (new List<RectangleF>(), null);
                        case RegionNodeInfinite:
                            return (new List<RectangleF> { s_infinite }, null);
                        case RegionNodePath:
                            return (null, node.Path!.ToSK());
                    }
                    var a = Evaluate(node.Left!);
                    var b = Evaluate(node.Right!);
                    var mode = (CombineMode)node.Kind;
                    bool aInfinite = a.Rects != null && a.Rects.Count == 1 && a.Rects[0] == s_infinite;
                    bool bInfinite = b.Rects != null && b.Rects.Count == 1 && b.Rects[0] == s_infinite;
                    bool aEmpty = a.Rects != null && a.Rects.Count == 0, bEmpty = b.Rects != null && b.Rects.Count == 0;
                    switch (mode)
                    {
                        case CombineMode.Intersect:
                            if (aInfinite) { a.Path?.Dispose(); return b; }
                            if (bInfinite) { b.Path?.Dispose(); return a; }
                            if (aEmpty || bEmpty) { a.Path?.Dispose(); b.Path?.Dispose(); return (new List<RectangleF>(), null); }
                            if (a.Rects != null && b.Rects != null && a.Rects.Count == 1 && b.Rects.Count == 1)
                            {
                                RectangleF r = RectangleF.Intersect(a.Rects[0], b.Rects[0]);
                                return (r.Width > 0 && r.Height > 0 ? new List<RectangleF> { r } : new List<RectangleF>(), null);
                            }
                            break;
                        case CombineMode.Union:
                            if (aInfinite || bInfinite) { a.Path?.Dispose(); b.Path?.Dispose(); return (new List<RectangleF> { s_infinite }, null); }
                            if (bEmpty) return a;
                            if (aEmpty) return b;
                            break;
                        case CombineMode.Exclude:
                            if (bEmpty || aEmpty) { b.Path?.Dispose(); return a; }
                            if (bInfinite) { a.Path?.Dispose(); return (new List<RectangleF>(), null); }
                            break;
                        case CombineMode.Complement:
                            if (aEmpty || bEmpty) { a.Path?.Dispose(); return b; }
                            if (aInfinite) { b.Path?.Dispose(); return (new List<RectangleF>(), null); }
                            break;
                        case CombineMode.Xor:
                            if (aEmpty) return b;
                            if (bEmpty) return a;
                            break;
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
                    using SKPath pa = a.Path ?? RectsPath(a.Rects!), pb = b.Path ?? RectsPath(b.Rects!);
                    return (null, pa.Op(pb, op) ?? new SKPath());
                }

                private static SKPath RectsPath(List<RectangleF> rects)
                {
                    var sk = new SKPath();
                    foreach (RectangleF r in rects)
                        sk.AddRect(new SKRect(r.Left, r.Top, r.Right, r.Bottom));
                    return sk;
                }

                private void Evaluate()
                {
                    if (_evaluated)
                        return;
                    (_rects, _path) = Evaluate(Tree);
                    _evaluated = true;
                }

                /// <summary>The rectangles it is, when it is made of rectangles; null when it is a path.</summary>
                public List<RectangleF>? Rects
                {
                    get
                    {
                        Evaluate();
                        return _rects;
                    }
                }

                public bool IsInfinite
                {
                    get
                    {
                        Evaluate();
                        return _rects != null && _rects.Count == 1 && _rects[0] == s_infinite;
                    }
                }

                public SKPath ToSK()
                {
                    Evaluate();
                    return _path != null ? new SKPath(_path) : RectsPath(_rects!);
                }

                public bool IsEmpty
                {
                    get
                    {
                        Evaluate();
                        if (_rects != null)
                        {
                            foreach (RectangleF r in _rects)
                                if (r.Width > 0 && r.Height > 0)
                                    return false;
                            return true;
                        }
                        return _path!.IsEmpty || _path.TightBounds.IsEmpty;
                    }
                }

                public RectangleF Bounds
                {
                    get
                    {
                        if (Tree.Kind < RegionNodeRect || Tree.Kind == RegionNodePath)
                        {
                            // GDI+: the pixels a combination or a path covers.
                            List<RectangleF> scans = Scans();
                            if (scans.Count == 0)
                                return RectangleF.Empty;
                            RectangleF u = scans[0];
                            for (int i = 1; i < scans.Count; i++)
                                u = RectangleF.Union(u, scans[i]);
                            return u;
                        }
                        Evaluate();
                        if (_rects != null)
                        {
                            if (_rects.Count == 0)
                                return RectangleF.Empty;
                            RectangleF b = _rects[0];
                            for (int i = 1; i < _rects.Count; i++)
                                b = RectangleF.Union(b, _rects[i]);
                            return b;
                        }
                        if (_path!.IsEmpty)
                            return RectangleF.Empty;
                        SKRect t = _path.TightBounds;
                        return t.IsEmpty ? RectangleF.Empty : new RectangleF(t.Left, t.Top, t.Width, t.Height);
                    }
                }

                public bool Contains(float x, float y)
                {
                    Evaluate();
                    if (_rects != null)
                    {
                        foreach (RectangleF r in _rects)
                            if (x >= r.X && x < r.Right && y >= r.Y && y < r.Bottom)
                                return true;
                        return false;
                    }
                    return _path!.Contains(x, y);
                }

                /// <summary>The region as GDI+'s scans: rectangles of the whole pixels it covers, row by row.</summary>
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
                    // GDI+'s pixels: those whose centre (whole coordinates) is inside, a left or top edge's included.
                    using SKPath sk = ToSK();
                    sk.Offset(PixelCenterOffset, PixelCenterOffset);
                    using var region = new SKRegion();
                    SKRect b = sk.Bounds;
                    region.SetPath(sk, new SKRegion(new SKRectI((int)Math.Floor(b.Left) - 1, (int)Math.Floor(b.Top) - 1, (int)Math.Ceiling(b.Right) + 1, (int)Math.Ceiling(b.Bottom) + 1)));
                    using var iterator = region.CreateRectIterator();
                    while (iterator.Next(out SKRectI r))
                        scans.Add(new RectangleF(r.Left, r.Top, r.Width, r.Height));
                    return scans;
                }

                // GDI+'s region data

                private static void Write(BinaryWriter w, Node node)
                {
                    w.Write(node.Kind);
                    switch (node.Kind)
                    {
                        case RegionNodeRect:
                            w.Write(node.Rect.X); w.Write(node.Rect.Y); w.Write(node.Rect.Width); w.Write(node.Rect.Height);
                            break;
                        case RegionNodePath:
                            {
                                byte[] path = PathData(node.Path!);
                                w.Write(path.Length);
                                w.Write(path);
                                break;
                            }
                        case RegionNodeEmpty:
                        case RegionNodeInfinite:
                            break;
                        default:
                            Write(w, node.Left!);
                            Write(w, node.Right!);
                            break;
                    }
                }

                /// <summary>An EmfPlusPath: version, count, flags (0x4000: 16-bit points, when they are whole), points, types, padding.</summary>
                private static byte[] PathData(GpPath path)
                {
                    var ms = new MemoryStream();
                    var w = new BinaryWriter(ms);
                    bool compressed = true;
                    foreach (PointF p in path.Points)
                        if (p.X != (short)p.X || p.Y != (short)p.Y)
                            compressed = false;
                    w.Write(RegionDataVersion);
                    w.Write(path.Count);
                    w.Write(compressed ? 0x4000 : 0);
                    foreach (PointF p in path.Points)
                    {
                        if (compressed) { w.Write((short)p.X); w.Write((short)p.Y); }
                        else { w.Write(p.X); w.Write(p.Y); }
                    }
                    foreach (byte t in path.Types)
                        w.Write(t);
                    while (ms.Length % 4 != 0)
                        w.Write((byte)0);
                    return ms.ToArray();
                }

                public byte[] Data()
                {
                    var ms = new MemoryStream();
                    var w = new BinaryWriter(ms);
                    w.Write(0);   // size, below
                    w.Write(0);   // checksum, below
                    w.Write(RegionDataVersion);
                    w.Write(Tree.Combinations);
                    Write(w, Tree);
                    byte[] data = ms.ToArray();
                    BitConverter.TryWriteBytes(data.AsSpan(0, 4), data.Length - 8);
                    BitConverter.TryWriteBytes(data.AsSpan(4, 4), Crc32(data.AsSpan(8)));
                    return data;
                }

                private static Node Read(BinaryReader r)
                {
                    uint kind = r.ReadUInt32();
                    switch (kind)
                    {
                        case RegionNodeRect:
                            return new Node { Kind = kind, Rect = new RectangleF(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle()) };
                        case RegionNodePath:
                            {
                                int size = r.ReadInt32();
                                long end = r.BaseStream.Position + size;
                                r.ReadUInt32();   // version
                                int count = r.ReadInt32();
                                int flags = r.ReadInt32();
                                var path = new GpPath(FillMode.Alternate);
                                for (int i = 0; i < count; i++)
                                    path.Points.Add((flags & 0x4000) != 0 ? new PointF(r.ReadInt16(), r.ReadInt16()) : new PointF(r.ReadSingle(), r.ReadSingle()));
                                for (int i = 0; i < count; i++)
                                    path.Types.Add(r.ReadByte());
                                r.BaseStream.Position = end;
                                return new Node { Kind = kind, Path = path };
                            }
                        case RegionNodeEmpty:
                        case RegionNodeInfinite:
                            return new Node { Kind = kind };
                        default:
                            if (kind < 1 || kind > 5)
                                throw new InvalidDataException();
                            Node left = Read(r);
                            return new Node { Kind = kind, Left = left, Right = Read(r) };
                    }
                }

                public static GpRegion? FromData(byte[] data, int size)
                {
                    if (size < 16 || data.Length < size)
                        return null;
                    try
                    {
                        var r = new BinaryReader(new MemoryStream(data, 0, size));
                        r.ReadUInt32();
                        uint checksum = r.ReadUInt32();
                        uint version = r.ReadUInt32();
                        if ((version & 0xfffff000) != 0xdbc01000 || checksum != Crc32(data.AsSpan(8, size - 8)))
                            return null;
                        r.ReadUInt32();
                        return new GpRegion { Tree = Read(r) };
                    }
                    catch (Exception e) when (e is EndOfStreamException or InvalidDataException or ArgumentException)
                    {
                        return null;
                    }
                }

                public void Dispose() => _path?.Dispose();
            }

            /// <summary>GDI+'s CRC-32 of region data: the table's, from zero, not inverted (libgdiplus' gdip_crc32).</summary>
            private static uint Crc32(ReadOnlySpan<byte> data)
            {
                uint crc = 0;
                foreach (byte b in data)
                {
                    uint c = (crc ^ b) & 0xff;
                    for (int k = 0; k < 8; k++)
                        c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
                    crc = c ^ (crc >> 8);
                }
                return crc;
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

            internal static int GdipCreateRegionRgnData(byte[] rgndata, int size, out IntPtr region)
            {
                region = IntPtr.Zero;
                if (rgndata == null || size < 0)
                    return InvalidParameter;
                GpRegion? r = GpRegion.FromData(rgndata, size);
                if (r == null)
                    return GenericError;
                region = NewHandle(r);
                return Ok;
            }

            internal static int GdipGetRegionDataSize(HandleRef region, out int bufferSize)
            {
                bufferSize = 0;
                if (!Is(region, out GpRegion? r))
                    return InvalidParameter;
                bufferSize = r.Data().Length;
                return Ok;
            }

            internal static int GdipGetRegionData(HandleRef region, byte[] regionData, int bufferSize, out int sizeFilled)
            {
                sizeFilled = 0;
                if (!Is(region, out GpRegion? r) || regionData == null || bufferSize <= 0)
                    return InvalidParameter;
                byte[] data = r.Data();
                if (data.Length > bufferSize || data.Length > regionData.Length)
                    return InsufficientBuffer;
                data.CopyTo(regionData, 0);
                sizeFilled = data.Length;
                return Ok;
            }

            /// <summary>An HRGN here is a region of its own (as libgdiplus': a clone).</summary>
            internal static int GdipGetRegionHRgn(HandleRef region, HandleRef graphics, out IntPtr hrgn)
            {
                hrgn = IntPtr.Zero;
                if (!Is(region, out GpRegion? r) || !Is(graphics, out GpGraphics? _))
                    return InvalidParameter;
                if (r.IsInfinite)
                    return Ok;
                hrgn = NewHandle(r.Clone());
                return Ok;
            }

            internal static int GdipCreateRegionHrgn(IntPtr hRgn, out IntPtr region)
            {
                region = IntPtr.Zero;
                if (!Is(hRgn, out GpRegion? r))
                    return InvalidParameter;
                region = NewHandle(r.Clone());
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

            internal static int GdipSetInfinite(HandleRef region)
            {
                if (!Is(region, out GpRegion? r))
                    return InvalidParameter;
                r.SetInfinite();
                return Ok;
            }

            internal static int GdipSetEmpty(HandleRef region)
            {
                if (!Is(region, out GpRegion? r))
                    return InvalidParameter;
                r.SetEmpty();
                return Ok;
            }

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
