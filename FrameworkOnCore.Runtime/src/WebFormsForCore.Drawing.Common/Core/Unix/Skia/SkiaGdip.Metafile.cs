// WebFormsForCore: GDI+'s metafiles, their header as libgdiplus' metafile.c reads and keeps it (an EMF's header record,
// a placeable WMF's or a WMF's; a recording's frame as given), written as the Unix implementation's MonoMetafileHeader.
// A recorded metafile's drawing is a Skia picture; a metafile read is not played (drawn empty).
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
            private const int ImageFlagsUndocumented = 0x40000;

            internal sealed class MetaHeader
            {
                public MetafileType Type;
                public int Size, Version, EmfPlusFlags;
                public float DpiX = 96, DpiY = 96;
                public int X, Y, Width, Height;
                public byte[] Record = new byte[88];   // EMF's ENHMETAHEADER (88 bytes) or WMF's METAHEADER (18)
                public int EmfPlusHeaderSize, LogicalDpiX, LogicalDpiY;

                public MetaHeader Clone()
                {
                    var h = (MetaHeader)MemberwiseClone();
                    h.Record = (byte[])Record.Clone();
                    return h;
                }

                /// <summary>The Unix implementation's MonoMetafileHeader (140 bytes).</summary>
                public void Write(byte* p)
                {
                    new Span<byte>(p, 140).Clear();
                    *(int*)p = (int)Type;
                    *(int*)(p + 4) = Size;
                    *(int*)(p + 8) = Version;
                    *(int*)(p + 12) = EmfPlusFlags;
                    *(float*)(p + 16) = DpiX;
                    *(float*)(p + 20) = DpiY;
                    *(int*)(p + 24) = X;
                    *(int*)(p + 28) = Y;
                    *(int*)(p + 32) = Width;
                    *(int*)(p + 36) = Height;
                    Record.AsSpan(0, Math.Min(Record.Length, 88)).CopyTo(new Span<byte>(p + 40, 88));
                    *(int*)(p + 128) = EmfPlusHeaderSize;
                    *(int*)(p + 132) = LogicalDpiX;
                    *(int*)(p + 136) = LogicalDpiY;
                }
            }

            /// <summary>gdip_get_metafileheader_from: the header of an EMF, a placeable WMF or a WMF.</summary>
            private static int ReadMetaHeader(byte[] data, MetaHeader header)
            {
                if (data.Length < 4)
                    return OutOfMemory;
                uint key = BitConverter.ToUInt32(data, 0);
                switch (key)
                {
                    case 0x9AC6CDD7:   // placeable WMF
                        {
                            if (data.Length < 22 + 18)
                                return OutOfMemory;
                            Array.Copy(data, 22, header.Record = new byte[88], 0, 18);
                            int type = BitConverter.ToUInt16(data, 22), headerSize = BitConverter.ToUInt16(data, 24), version = BitConverter.ToUInt16(data, 26);
                            if ((type != 1 && type != 2) || headerSize != 9 || (version != 0x0100 && version != 0x0300))
                                return OutOfMemory;
                            short left = BitConverter.ToInt16(data, 6), top = BitConverter.ToInt16(data, 8), right = BitConverter.ToInt16(data, 10), bottom = BitConverter.ToInt16(data, 12);
                            int inch = BitConverter.ToUInt16(data, 14);
                            header.Type = MetafileType.WmfPlaceable;
                            header.X = Math.Min(right, left);
                            header.Y = Math.Min(top, bottom);
                            header.Width = Math.Abs(right - left);
                            header.Height = Math.Abs(bottom - top);
                            header.DpiX = header.DpiY = inch == 0 ? 1440 : inch;
                            header.Size = (int)BitConverter.ToUInt32(data, 28) * 2;
                            header.Version = version;
                            return Ok;
                        }
                    case 0x00090001:   // WMF: type 1, header size 9
                    case 0x00090002:
                        {
                            if (data.Length < 18)
                                return InvalidParameter;
                            Array.Copy(data, 0, header.Record = new byte[88], 0, 18);
                            header.Type = MetafileType.Wmf;
                            header.X = header.Y = 0;
                            header.Width = 1280;
                            header.Height = 1024;
                            header.DpiX = header.DpiY = 96;
                            header.Size = (int)BitConverter.ToUInt32(data, 6) * 2;
                            header.Version = BitConverter.ToUInt16(data, 4);
                            return Ok;
                        }
                    case 1:   // EMF's EMR_HEADER
                        {
                            var record = new byte[88];
                            Array.Copy(data, 0, record, 0, Math.Min(88, data.Length));
                            header.Record = record;
                            int nSize = BitConverter.ToInt32(record, 4);
                            int fl = BitConverter.ToInt32(record, 24), ft = BitConverter.ToInt32(record, 28), fr = BitConverter.ToInt32(record, 32), fb = BitConverter.ToInt32(record, 36);
                            uint signature = BitConverter.ToUInt32(record, 40);
                            int version = BitConverter.ToInt32(record, 44), bytes = BitConverter.ToInt32(record, 48), records = BitConverter.ToInt32(record, 52);
                            int handles = BitConverter.ToUInt16(record, 56);
                            int devX = BitConverter.ToInt32(record, 72), devY = BitConverter.ToInt32(record, 76), mmX = BitConverter.ToInt32(record, 80), mmY = BitConverter.ToInt32(record, 84);
                            if (signature != 0x464D4520 || records < 2 || handles == 0 || bytes < nSize || (bytes & 3) != 0 || devX == 0 || devY == 0 || mmX == 0 || mmY == 0)
                                return OutOfMemory;
                            header.Type = MetafileType.Emf;
                            header.DpiX = devX / (mmX / 25.4f);
                            header.DpiY = devY / (mmY / 25.4f);
                            float inchLeft = fl / (25.4f * 100), inchTop = ft / (25.4f * 100), inchRight = fr / (25.4f * 100), inchBottom = fb / (25.4f * 100);
                            header.X = IRound(inchLeft * header.DpiX);
                            header.Y = IRound(inchTop * header.DpiY);
                            header.Width = IRound((inchRight - inchLeft) * header.DpiX) + 1;
                            header.Height = IRound((inchBottom - inchTop) * header.DpiY) + 1;
                            header.Size = bytes;
                            header.Version = version;
                            return Ok;
                        }
                    default:
                        return OutOfMemory;
                }
            }

            /// <summary>gdip_get_metafile_from: a metafile read (its header; its records kept, not played).</summary>
            private static int ReadMetafile(byte[] data, out GpMetafile? metafile)
            {
                metafile = null;
                var header = new MetaHeader();
                int status = ReadMetaHeader(data, header);
                if (status != Ok)
                    return status;
                if (header.Type is MetafileType.Wmf or MetafileType.WmfPlaceable)
                {
                    int length = header.Size - 18;
                    int maxRecord = header.Record.Length >= 18 ? BitConverter.ToInt32(header.Record, 12) : 0;
                    if (length == 0 ? maxRecord != 3 : length <= 4)
                        return OutOfMemory;
                }
                var m = new GpMetafile { Header = header, RawFormat = header.Type is MetafileType.Wmf or MetafileType.WmfPlaceable ? WmfGuid : EmfGuid, Data = data };
                m.DpiX = header.DpiX;
                m.DpiY = header.DpiY;
                m.Flags = ImageFlagsUndocumented | (int)(ImageFlags.ReadOnly | ImageFlags.HasAlpha | ImageFlags.Scalable);
                metafile = m;
                return Ok;
            }

            internal static int GdipCreateMetafileFromDelegate_linux(StreamGetHeaderDelegate getHeader, StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek, StreamCloseDelegate close, StreamSizeDelegate size, out IntPtr metafile)
            {
                metafile = IntPtr.Zero;
                byte[]? data = ReadAll(getBytes);
                if (data == null)
                    return InvalidParameter;
                int status = ReadMetafile(data, out GpMetafile? m);
                if (status == Ok)
                    metafile = NewHandle(m!);
                return status;
            }

            internal static int GdipCreateMetafileFromFile(string file, out IntPtr metafile)
            {
                metafile = IntPtr.Zero;
                if (file == null)
                    return InvalidParameter;
                byte[] data;
                try
                {
                    data = File.ReadAllBytes(file);
                }
                catch (Exception e) when (e is IOException or UnauthorizedAccessException)
                {
                    return GenericError;
                }
                if (ReadMetafile(data, out GpMetafile? m) != Ok)
                    return GenericError;
                metafile = NewHandle(m!);
                return Ok;
            }

            internal static int GdipCreateMetafileFromEmf(IntPtr hEnhMetafile, bool deleteEmf, out IntPtr metafile)
            {
                metafile = IntPtr.Zero;
                if (!Is(hEnhMetafile, out GpMetafile? m))
                    return InvalidParameter;
                if (m.Header.Type is MetafileType.Wmf or MetafileType.WmfPlaceable)
                    return GenericError;
                metafile = NewHandle(m.Clone());
                return Ok;
            }

            internal static int GdipCreateMetafileFromWmf(IntPtr hMetafile, bool deleteWmf, WmfPlaceableFileHeader wmfplacealbeHeader, out IntPtr metafile)
            {
                metafile = IntPtr.Zero;
                if (!Is(hMetafile, out GpMetafile? m))
                    return InvalidParameter;
                if (m.Header.Type is not (MetafileType.Wmf or MetafileType.WmfPlaceable))
                    return GenericError;
                var clone = (GpMetafile)m.Clone();
                if (wmfplacealbeHeader != null)
                    CombineHeaders(wmfplacealbeHeader, clone.Header);
                metafile = NewHandle(clone);
                return Ok;
            }

            private static void CombineHeaders(WmfPlaceableFileHeader placeable, MetaHeader header)
            {
                header.Type = MetafileType.WmfPlaceable;
                header.X = Math.Min(placeable.BboxRight, placeable.BboxLeft);
                header.Y = Math.Min(placeable.BboxTop, placeable.BboxBottom);
                header.Width = Math.Abs(placeable.BboxRight - placeable.BboxLeft);
                header.Height = Math.Abs(placeable.BboxBottom - placeable.BboxTop);
                header.DpiX = header.DpiY = placeable.Inch == 0 ? 1440 : placeable.Inch;
                header.EmfPlusFlags = header.EmfPlusHeaderSize = header.LogicalDpiX = header.LogicalDpiY = 0;
            }

            internal static int GdipGetMetafileHeaderFromWmf(IntPtr hWmf, IntPtr wmfPlaceableFileHeader, IntPtr header)
            {
                if (!Is(hWmf, out GpMetafile? m) || wmfPlaceableFileHeader == IntPtr.Zero || header == IntPtr.Zero)
                    return InvalidParameter;
                MetaHeader h = m.Header.Clone();
                var placeable = Marshal.PtrToStructure<WmfPlaceableFileHeader>(wmfPlaceableFileHeader)!;
                CombineHeaders(placeable, h);
                h.Write((byte*)header);
                return Ok;
            }

            internal static int GdipGetMetafileHeaderFromEmf(IntPtr hEmf, IntPtr header)
            {
                if (!Is(hEmf, out GpMetafile? m) || header == IntPtr.Zero)
                    return InvalidParameter;
                if (m.Header.Type is MetafileType.Wmf or MetafileType.WmfPlaceable)
                    return InvalidParameter;
                m.Header.Write((byte*)header);
                return Ok;
            }

            internal static int GdipGetMetafileHeaderFromFile(string filename, IntPtr header)
            {
                if (filename == null || header == IntPtr.Zero)
                    return InvalidParameter;
                byte[] data;
                try
                {
                    data = File.ReadAllBytes(filename);
                }
                catch (Exception e) when (e is IOException or UnauthorizedAccessException)
                {
                    return InvalidParameter;
                }
                var h = new MetaHeader();
                int status = ReadMetaHeader(data, h);
                if (status == Ok)
                    h.Write((byte*)header);
                return status;
            }

            internal static int GdipGetMetafileHeaderFromDelegate_linux(StreamGetHeaderDelegate getHeader, StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek, StreamCloseDelegate close, StreamSizeDelegate size, IntPtr header)
            {
                if (header == IntPtr.Zero)
                    return InvalidParameter;
                byte[]? data = ReadAll(getBytes);
                if (data == null)
                    return InvalidParameter;
                var h = new MetaHeader();
                int status = ReadMetaHeader(data, h);
                if (status == Ok)
                    h.Write((byte*)header);
                return status;
            }

            internal static int GdipGetMetafileHeaderFromMetafile(IntPtr metafile, IntPtr header)
            {
                if (!Is(metafile, out GpMetafile? m) || header == IntPtr.Zero)
                    return InvalidParameter;
                m.Header.Write((byte*)header);
                return Ok;
            }

            /// <summary>An HENHMETAFILE here is the metafile itself (as libgdiplus').</summary>
            internal static int GdipGetHemfFromMetafile(IntPtr metafile, out IntPtr hEmf)
            {
                hEmf = IntPtr.Zero;
                if (!Is(metafile, out GpMetafile? _))
                    return InvalidParameter;
                hEmf = metafile;
                return Ok;
            }

            internal static int GdipGetMetafileDownLevelRasterizationLimit(IntPtr metafile, ref uint metafileRasterizationLimitDpi)
            {
                if (!Is(metafile, out GpMetafile? m))
                    return InvalidParameter;
                if (m.Header.Type is MetafileType.Emf or MetafileType.EmfPlusDual)
                {
                    metafileRasterizationLimitDpi = 0;
                    return Ok;
                }
                return WrongState;
            }

            internal static int GdipSetMetafileDownLevelRasterizationLimit(IntPtr metafile, uint metafileRasterizationLimitDpi)
            {
                if (!Is(metafile, out GpMetafile? m))
                    return InvalidParameter;
                return m.Header.Type is MetafileType.Emf or MetafileType.EmfPlusDual ? Ok : WrongState;
            }

            internal static int GdipPlayMetafileRecord(HandleRef metafile, EmfPlusRecordType recordType, int flags, int dataSize, byte[] data)
            {
                if (!Is(metafile, out GpMetafile? _) || (dataSize != 0 && data == null))
                    return InvalidParameter;
                return NotImplemented;
            }

            // Recording

            /// <summary>GdipRecordMetafile (libgdiplus): the frame as given, in its unit; a picture recorded.</summary>
            private static int RecordMetafile(IntPtr referenceHdc, RectangleF? frame, MetafileFrameUnit frameUnit, EmfType type, out IntPtr metafile)
            {
                metafile = IntPtr.Zero;
                if (referenceHdc == IntPtr.Zero || frame == null)
                    return InvalidParameter;
                if (type < EmfType.EmfOnly || type > EmfType.EmfPlusDual)
                    return InvalidParameter;
                if (frameUnit < MetafileFrameUnit.Pixel || frameUnit > MetafileFrameUnit.GdiCompatible)
                    return InvalidParameter;
                RectangleF r = frame.Value;
                if ((r.Width == 0 || r.Height == 0) && frameUnit != MetafileFrameUnit.GdiCompatible)
                    return GenericError;
                var m = new GpMetafile
                {
                    Header = new MetaHeader { X = (int)r.X, Y = (int)r.Y, Width = (int)r.Width, Height = (int)r.Height, Size = 0, Type = (MetafileType)type },
                    RawFormat = EmfGuid,
                    Recording = true,
                    Flags = ImageFlagsUndocumented | (int)(ImageFlags.ReadOnly | ImageFlags.HasAlpha | ImageFlags.Scalable),
                };
                metafile = NewHandle(m);
                return Ok;
            }

            internal static int GdipRecordMetafileFromDelegateI_linux(StreamGetHeaderDelegate getHeader, StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek, StreamCloseDelegate close, StreamSizeDelegate size, IntPtr hdc, EmfType type, ref Rectangle frameRect, MetafileFrameUnit frameUnit, string? description, out IntPtr metafile)
            {
                metafile = IntPtr.Zero;
                if (putBytes == null)
                    return InvalidParameter;
                return RecordMetafile(hdc, frameRect, frameUnit, type, out metafile);
            }

            internal static int GdipRecordMetafileFromDelegate_linux(StreamGetHeaderDelegate getHeader, StreamGetBytesDelegate getBytes, StreamPutBytesDelegate putBytes, StreamSeekDelegate doSeek, StreamCloseDelegate close, StreamSizeDelegate size, IntPtr hdc, EmfType type, ref RectangleF frameRect, MetafileFrameUnit frameUnit, string? description, out IntPtr metafile)
            {
                metafile = IntPtr.Zero;
                if (putBytes == null)
                    return InvalidParameter;
                return RecordMetafile(hdc, frameRect, frameUnit, type, out metafile);
            }

            internal static int GdipRecordMetafileI(IntPtr referenceHdc, EmfType emfType, ref Rectangle frameRect, MetafileFrameUnit frameUnit, string? description, out IntPtr metafile) =>
                RecordMetafile(referenceHdc, frameRect, frameUnit, emfType, out metafile);

            internal static int GdipRecordMetafile(IntPtr referenceHdc, EmfType emfType, ref RectangleF frameRect, MetafileFrameUnit frameUnit, string? description, out IntPtr metafile) =>
                RecordMetafile(referenceHdc, frameRect, frameUnit, emfType, out metafile);

            internal static int GdipRecordMetafile(IntPtr referenceHdc, EmfType emfType, IntPtr pframeRect, MetafileFrameUnit frameUnit, string? description, out IntPtr metafile) =>
                RecordMetafile(referenceHdc, pframeRect == IntPtr.Zero ? null : *(RectangleF*)pframeRect, frameUnit, emfType, out metafile);

            internal static int GdipRecordMetafileFileNameI(string fileName, IntPtr referenceHdc, EmfType emfType, ref Rectangle frameRect, MetafileFrameUnit frameUnit, string? description, out IntPtr metafile)
            {
                metafile = IntPtr.Zero;
                if (fileName == null)
                    return InvalidParameter;
                return RecordMetafile(referenceHdc, frameRect, frameUnit, emfType, out metafile);
            }

            internal static int GdipRecordMetafileFileName(string fileName, IntPtr referenceHdc, EmfType emfType, ref RectangleF frameRect, MetafileFrameUnit frameUnit, string? description, out IntPtr metafile)
            {
                metafile = IntPtr.Zero;
                if (fileName == null)
                    return InvalidParameter;
                return RecordMetafile(referenceHdc, frameRect, frameUnit, emfType, out metafile);
            }

            internal static int GdipRecordMetafileFileName(string fileName, IntPtr referenceHdc, EmfType emfType, IntPtr pframeRect, MetafileFrameUnit frameUnit, string? description, out IntPtr metafile)
            {
                metafile = IntPtr.Zero;
                if (fileName == null)
                    return InvalidParameter;
                return RecordMetafile(referenceHdc, pframeRect == IntPtr.Zero ? null : *(RectangleF*)pframeRect, frameUnit, emfType, out metafile);
            }
        }
    }
}
