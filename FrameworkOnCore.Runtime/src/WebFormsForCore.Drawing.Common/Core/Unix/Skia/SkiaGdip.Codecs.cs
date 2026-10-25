// WebFormsForCore: the image formats Skia does not read or write as GDI+ does, read and written here as GDI+ does: BMP
// (its pixel formats, indexed with their palettes), GIF (frames, delays, the palette; written 8 bpp, an image not indexed
// on GDI+'s halftone palette), TIFF (pages; LZW with the horizontal predictor, as GDI+ writes it; also PackBits and
// Deflate), PNG with a palette (8 bpp indexed, as GDI+ gives it); a JPEG's EXIF values and quantization tables as
// property items. Multi-frame images keep their frames; SaveAdd adds pages to a TIFF being written.
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Drawing
{
    internal static partial class SafeNativeMethods
    {
        internal static unsafe partial class SkiaGdip
        {
            internal static readonly Guid FrameDimensionTime = new Guid("6aedbd6d-3fb5-418a-83a6-7f45229dc872");
            private static readonly Guid s_saveFlag = new Guid("292266fc-ac40-47bf-8cfc-a85b89a655de");
            private const long EncoderValueMultiFrame = 18, EncoderValueFlush = 20, EncoderValueFrameDimensionPage = 23;

            internal sealed class Property
            {
                public int Id;
                public short Type;
                public byte[] Value = Array.Empty<byte>();
            }

            // Frames and properties (the image's)

            internal sealed class Frames
            {
                public Guid Dimension = FrameDimensionPage;
                public List<GpBitmap> List = new List<GpBitmap>();   // the frames, the active one's pixels in the image itself
                public int Active;
            }

            /// <summary>The frame at an index made the image's pixels (the active one's put back in its place).</summary>
            internal static void SelectFrame(GpBitmap image, int index)
            {
                Frames f = image.FrameSet!;
                if (index == f.Active)
                    return;
                image.EnsureScan0();
                image.Surface?.Dispose();
                image.Surface = null;
                GpBitmap parked = f.List[f.Active];
                MoveePixels(image, parked);
                MoveePixels(f.List[index], image);
                f.Active = index;
            }

            private static void MoveePixels(GpBitmap from, GpBitmap to)
            {
                to.W = from.W; to.H = from.H; to.Format = from.Format; to.Stride = from.Stride;
                to.Scan0 = from.Scan0; to.OwnsScan0 = from.OwnsScan0; to.Palette = from.Palette; to.PaletteFlags = from.PaletteFlags;
                to.SurfaceDirty = to.Scan0Dirty = false;
                from.Scan0 = IntPtr.Zero;
                from.OwnsScan0 = false;
            }

            /// <summary>A bitmap of several frames: the first its own pixels, the others kept.</summary>
            private static GpBitmap WithFrames(List<GpBitmap> frames, Guid dimension)
            {
                GpBitmap image = frames[0];
                if (frames.Count == 1)
                    return image;
                var set = new Frames { Dimension = dimension };
                set.List.Add(new GpBitmap());   // frame 0's place, its pixels the image's
                for (int i = 1; i < frames.Count; i++)
                    set.List.Add(frames[i]);
                image.FrameSet = set;
                return image;
            }

            internal static int GdipImageGetFrameDimensionsCount(HandleRef image, out int count)
            {
                count = 0;
                if (!Is(image, out GpImage? _))
                    return InvalidParameter;
                count = 1;
                return Ok;
            }

            internal static int GdipImageGetFrameDimensionsList(HandleRef image, Guid* dimensionIDs, int count)
            {
                if (!Is(image, out GpImage? i) || dimensionIDs == null || count < 1)
                    return InvalidParameter;
                dimensionIDs[0] = (i as GpBitmap)?.FrameSet?.Dimension ?? (i.RawFormat == GifGuid ? FrameDimensionTime : FrameDimensionPage);
                return Ok;
            }

            internal static int GdipImageGetFrameCount(HandleRef image, ref Guid dimensionID, out int count)
            {
                count = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                Guid dimension = (i as GpBitmap)?.FrameSet?.Dimension ?? (i.RawFormat == GifGuid ? FrameDimensionTime : FrameDimensionPage);
                if (dimensionID != dimension)
                    return InvalidParameter;
                count = (i as GpBitmap)?.FrameSet?.List.Count ?? 1;
                return Ok;
            }

            internal static int GdipImageSelectActiveFrame(HandleRef image, ref Guid dimensionID, int frameIndex)
            {
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                if (i is GpBitmap b && b.FrameSet != null)
                {
                    if (dimensionID != b.FrameSet.Dimension || frameIndex < 0 || frameIndex >= b.FrameSet.List.Count)
                        return InvalidParameter;
                    if (b.Locked)
                        return WrongState;
                    SelectFrame(b, frameIndex);
                    return Ok;
                }
                return frameIndex == 0 ? Ok : InvalidParameter;
            }

            private static int PropertySize(Property p) => Marshal.SizeOf<PropertyItemInternal>() + p.Value.Length;

            internal static int GdipGetPropertyCount(HandleRef image, out uint numOfProperty)
            {
                numOfProperty = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                numOfProperty = (uint)i.Properties.Count;
                return Ok;
            }

            internal static int GdipGetPropertyIdList(HandleRef image, uint numOfProperty, int* list)
            {
                if (!Is(image, out GpImage? i) || list == null)
                    return InvalidParameter;
                if (numOfProperty != i.Properties.Count)
                    return InvalidParameter;
                for (int k = 0; k < i.Properties.Count; k++)
                    list[k] = i.Properties[k].Id;
                return Ok;
            }

            private static Property? FindProperty(GpImage image, int id)
            {
                foreach (Property p in image.Properties)
                    if (p.Id == id)
                        return p;
                return null;
            }

            internal static int GdipGetPropertyItemSize(HandleRef image, int propid, out uint size)
            {
                size = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                Property? p = FindProperty(i, propid);
                if (p == null)
                    return Gdip.PropertyNotFound;
                size = (uint)PropertySize(p);
                return Ok;
            }

            private static void WriteProperty(Property p, PropertyItemInternal* item, byte* value)
            {
                item->id = p.Id;
                item->len = p.Value.Length;
                item->type = p.Type;
                item->value = value;
                p.Value.AsSpan().CopyTo(new Span<byte>(value, p.Value.Length));
            }

            internal static int GdipGetPropertyItem(HandleRef image, int propid, uint propSize, PropertyItemInternal* buffer)
            {
                if (!Is(image, out GpImage? i) || buffer == null)
                    return InvalidParameter;
                Property? p = FindProperty(i, propid);
                if (p == null)
                    return Gdip.PropertyNotFound;
                if (propSize != PropertySize(p))
                    return InvalidParameter;
                WriteProperty(p, buffer, (byte*)(buffer + 1));
                return Ok;
            }

            internal static int GdipGetPropertySize(HandleRef image, out uint totalBufferSize, out uint numProperties)
            {
                totalBufferSize = numProperties = 0;
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                int size = 0;
                foreach (Property p in i.Properties)
                    size += PropertySize(p);
                totalBufferSize = (uint)size;
                numProperties = (uint)i.Properties.Count;
                return Ok;
            }

            internal static int GdipGetAllPropertyItems(HandleRef image, uint totalBufferSize, uint numProperties, PropertyItemInternal* allItems)
            {
                if (!Is(image, out GpImage? i) || allItems == null)
                    return InvalidParameter;
                if (numProperties != i.Properties.Count)
                    return InvalidParameter;
                byte* values = (byte*)(allItems + i.Properties.Count);
                for (int k = 0; k < i.Properties.Count; k++)
                {
                    WriteProperty(i.Properties[k], allItems + k, values);
                    values += i.Properties[k].Value.Length;
                }
                return Ok;
            }

            internal static int GdipSetPropertyItem(HandleRef image, PropertyItemInternal* item)
            {
                if (!Is(image, out GpImage? i) || item == null)
                    return InvalidParameter;
                var p = new Property { Id = item->id, Type = item->type, Value = item->value == null ? Array.Empty<byte>() : new ReadOnlySpan<byte>(item->value, item->len).ToArray() };
                for (int k = 0; k < i.Properties.Count; k++)
                    if (i.Properties[k].Id == p.Id)
                    {
                        i.Properties[k] = p;
                        return Ok;
                    }
                i.Properties.Add(p);
                return Ok;
            }

            internal static int GdipRemovePropertyItem(HandleRef image, int propid)
            {
                if (!Is(image, out GpImage? i))
                    return InvalidParameter;
                for (int k = 0; k < i.Properties.Count; k++)
                    if (i.Properties[k].Id == propid)
                    {
                        i.Properties.RemoveAt(k);
                        return Ok;
                    }
                return GenericError;   // GDI+'s, for a property it does not have
            }

            // Reading: the formats here, before Skia's.

            private static bool StartsWith(byte[] data, params byte[] signature)
            {
                if (data.Length < signature.Length)
                    return false;
                for (int i = 0; i < signature.Length; i++)
                    if (data[i] != signature[i])
                        return false;
                return true;
            }

            /// <summary>The image of a format read here, or null for Skia's.</summary>
            private static int DecodeHere(byte[] data, out GpImage? image)
            {
                image = null;
                try
                {
                    if (StartsWith(data, 0x42, 0x4d))
                        image = ReadBmp(data);
                    else if (StartsWith(data, 0x47, 0x49, 0x46, 0x38))
                        image = ReadGif(data);
                    else if (StartsWith(data, 0x49, 0x49, 0x2a, 0x00) || StartsWith(data, 0x4d, 0x4d, 0x00, 0x2a))
                        image = ReadTiff(data);
                    else if (StartsWith(data, 0x89, 0x50, 0x4e, 0x47) && PngColorType(data) == 3)
                        image = ReadIndexedPng(data);
                    else if (data.Length >= 88 && BitConverter.ToInt32(data, 0) == 1 && data[40] == 0x20 && data[41] == 0x45 && data[42] == 0x4d && data[43] == 0x46)
                        image = ReadEmfHeader(data);
                    else if (StartsWith(data, 0xd7, 0xcd, 0xc6, 0x9a))
                        image = ReadWmfHeader(data);
                }
                catch (Exception e) when (e is IndexOutOfRangeException or ArgumentException or InvalidDataException or EndOfStreamException or OverflowException)
                {
                    return OutOfMemory;
                }
                return Ok;
            }

            /// <summary>What GDI+ adds to a JPEG it reads: its EXIF values, its quantization tables.</summary>
            private static void JpegProperties(byte[] data, GpImage image)
            {
                int i = 2;
                var luminance = new List<ushort>();
                var chrominance = new List<ushort>();
                int tables = 0;
                while (i + 4 <= data.Length && data[i] == 0xff)
                {
                    byte marker = data[i + 1];
                    if (marker == 0xd8 || marker == 0x01 || (marker >= 0xd0 && marker <= 0xd7)) { i += 2; continue; }
                    if (marker == 0xda || marker == 0xd9) break;
                    int length = (data[i + 2] << 8) | data[i + 3];
                    if (marker == 0xe1 && length >= 8 && i + 10 <= data.Length && data[i + 4] == 'E' && data[i + 5] == 'x' && data[i + 6] == 'i' && data[i + 7] == 'f')
                        ReadExif(data, i + 10, Math.Min(length - 8, data.Length - i - 10), image);
                    else if (marker == 0xdb)
                    {
                        int at = i + 4, end = Math.Min(i + 2 + length, data.Length);
                        while (at < end)
                        {
                            int pq = data[at] >> 4;
                            int id = data[at] & 0x0f;
                            at++;
                            var table = new ushort[64];
                            for (int k = 0; k < 64; k++)
                            {
                                int v = pq == 0 ? data[at++] : (data[at++] << 8) | data[at++];
                                table[s_zigzag[k]] = (ushort)v;
                            }
                            (id == 0 ? luminance : chrominance).AddRange(table);
                            tables++;
                        }
                    }
                    i += 2 + length;
                }
                if (luminance.Count > 0)
                    image.Properties.Add(new Property { Id = 0x5090, Type = 3, Value = Shorts(luminance) });
                if (chrominance.Count > 0)
                    image.Properties.Add(new Property { Id = 0x5091, Type = 3, Value = Shorts(chrominance) });
            }

            private static readonly int[] s_zigzag =
            {
                0, 1, 8, 16, 9, 2, 3, 10, 17, 24, 32, 25, 18, 11, 4, 5, 12, 19, 26, 33, 40, 48, 41, 34, 27, 20, 13, 6, 7, 14, 21, 28,
                35, 42, 49, 56, 57, 50, 43, 36, 29, 22, 15, 23, 30, 37, 44, 51, 58, 59, 52, 45, 38, 31, 39, 46, 53, 60, 61, 54, 47, 55, 62, 63,
            };

            private static byte[] Shorts(List<ushort> values)
            {
                var b = new byte[values.Count * 2];
                for (int i = 0; i < values.Count; i++)
                    BitConverter.TryWriteBytes(b.AsSpan(i * 2), values[i]);
                return b;
            }

            private static int TypeSize(int type) => type switch { 1 or 2 or 6 or 7 => 1, 3 or 8 => 2, 4 or 9 or 11 => 4, 5 or 10 or 12 => 8, _ => 1 };

            /// <summary>A TIFF structure's tags (EXIF's IFD0, its EXIF and GPS IFDs; the thumbnail's as GDI+'s 0x50xx), in the machine's byte order.</summary>
            private static void ReadExif(byte[] data, int start, int length, GpImage image)
            {
                var tiff = new TiffReader(data, start, length);
                var seen = new HashSet<int>();
                void ReadIfd(int offset, bool thumbnail, int depth)
                {
                    if (offset <= 0 || depth > 4)
                        return;
                    foreach (TiffEntry e in tiff.Entries(offset))
                    {
                        if (e.Tag == 0x8769 || e.Tag == 0x8825 || e.Tag == 0xa005)
                        {
                            ReadIfd((int)tiff.Number(e, 0), false, depth + 1);
                            continue;
                        }
                        int id = thumbnail ? ThumbnailId(e.Tag) : e.Tag;
                        if (id < 0 || !seen.Add(id))
                            continue;
                        image.Properties.Add(new Property { Id = id, Type = (short)e.Type, Value = tiff.Value(e) });
                    }
                }
                int first = tiff.FirstIfd;
                ReadIfd(first, false, 0);
                int next = tiff.NextIfd(first);
                if (next > 0)
                    ReadIfd(next, true, 0);
            }

            /// <summary>A thumbnail IFD's tag as GDI+'s PropertyTagThumbnail*.</summary>
            private static int ThumbnailId(int tag) => tag switch
            {
                0x0103 => 0x5023, // Compression
                0x011a => 0x502d, // XResolution
                0x011b => 0x502e, // YResolution
                0x0128 => 0x5030, // ResolutionUnit
                0x0201 => 0x501b, // JPEGInterchangeFormat: the data, below
                0x0202 => -1,
                0x0100 => 0x5020, // ImageWidth
                0x0101 => 0x5021, // ImageHeight
                _ => -1,
            };

            private struct TiffEntry
            {
                public int Tag, Type, Count, ValueOffset;
            }

            /// <summary>A TIFF structure, of either byte order, read.</summary>
            private sealed class TiffReader
            {
                private readonly byte[] _data;
                private readonly int _start, _length;
                public readonly bool BigEndian;

                public TiffReader(byte[] data, int start, int length)
                {
                    _data = data;
                    _start = start;
                    _length = length;
                    BigEndian = data[start] == 'M';
                }

                public int U16(int at) => BigEndian ? (_data[_start + at] << 8) | _data[_start + at + 1] : _data[_start + at] | (_data[_start + at + 1] << 8);

                public uint U32(int at) => BigEndian
                    ? (uint)((_data[_start + at] << 24) | (_data[_start + at + 1] << 16) | (_data[_start + at + 2] << 8) | _data[_start + at + 3])
                    : (uint)(_data[_start + at] | (_data[_start + at + 1] << 8) | (_data[_start + at + 2] << 16) | (_data[_start + at + 3] << 24));

                public int FirstIfd => (int)U32(4);

                public int NextIfd(int ifd)
                {
                    if (ifd <= 0 || ifd + 2 > _length)
                        return 0;
                    int n = U16(ifd);
                    int at = ifd + 2 + n * 12;
                    return at + 4 <= _length ? (int)U32(at) : 0;
                }

                public IEnumerable<TiffEntry> Entries(int ifd)
                {
                    if (ifd <= 0 || ifd + 2 > _length)
                        yield break;
                    int n = U16(ifd);
                    for (int i = 0; i < n; i++)
                    {
                        int at = ifd + 2 + i * 12;
                        if (at + 12 > _length)
                            yield break;
                        var e = new TiffEntry { Tag = U16(at), Type = U16(at + 2), Count = (int)U32(at + 4) };
                        int size = TypeSize(e.Type) * e.Count;
                        e.ValueOffset = size <= 4 ? at + 8 : (int)U32(at + 8);
                        yield return e;
                    }
                }

                /// <summary>An entry's value in the machine's byte order.</summary>
                public byte[] Value(TiffEntry e)
                {
                    int unit = TypeSize(e.Type);
                    int size = unit * e.Count;
                    if (e.ValueOffset < 0 || e.ValueOffset + size > _length)
                        return Array.Empty<byte>();
                    var value = new byte[size];
                    Array.Copy(_data, _start + e.ValueOffset, value, 0, size);
                    if (BigEndian == BitConverter.IsLittleEndian && unit > 1)
                    {
                        int part = e.Type is 5 or 10 ? 4 : unit;
                        for (int i = 0; i + part <= size; i += part)
                            Array.Reverse(value, i, part);
                    }
                    return value;
                }

                public long Number(TiffEntry e, int index)
                {
                    int unit = TypeSize(e.Type);
                    int at = e.ValueOffset + index * unit;
                    return e.Type switch
                    {
                        1 or 7 => _data[_start + at],
                        6 => (sbyte)_data[_start + at],
                        3 => U16(at),
                        8 => (short)U16(at),
                        4 => U32(at),
                        9 => (int)U32(at),
                        _ => U32(at),
                    };
                }

                public double Rational(TiffEntry e)
                {
                    uint n = U32(e.ValueOffset), d = U32(e.ValueOffset + 4);
                    return d == 0 ? 0 : (double)n / d;
                }

                public byte[] Bytes(int offset, int count)
                {
                    count = Math.Max(0, Math.Min(count, _length - offset));
                    var b = new byte[count];
                    Array.Copy(_data, _start + offset, b, 0, count);
                    return b;
                }
            }

            // Metafiles read: their size (EMF's header, a placeable WMF's), not their records (not played: drawn empty).

            private static GpImage ReadEmfHeader(byte[] data)
            {
                int left = BitConverter.ToInt32(data, 24), top = BitConverter.ToInt32(data, 28), right = BitConverter.ToInt32(data, 32), bottom = BitConverter.ToInt32(data, 36);
                int devX = BitConverter.ToInt32(data, 72), devY = BitConverter.ToInt32(data, 76), mmX = BitConverter.ToInt32(data, 80), mmY = BitConverter.ToInt32(data, 84);
                float pxPerMmX = mmX > 0 ? (float)devX / mmX : 96 / 25.4f, pxPerMmY = mmY > 0 ? (float)devY / mmY : 96 / 25.4f;
                var m = new GpMetafile
                {
                    Frame = new RectangleF(0, 0, Math.Max(1, (right - left) / 100f * pxPerMmX), Math.Max(1, (bottom - top) / 100f * pxPerMmY)),
                    EmfType = EmfType.EmfOnly,
                    RawFormat = EmfGuid,
                    DpiX = pxPerMmX * 25.4f,
                    DpiY = pxPerMmY * 25.4f,
                };
                using (var recorder = new SkiaSharp.SKPictureRecorder())
                {
                    recorder.BeginRecording(new SkiaSharp.SKRect(0, 0, m.Frame.Width, m.Frame.Height));
                    m.Picture = recorder.EndRecording();
                }
                return m;
            }

            private static GpImage ReadWmfHeader(byte[] data)
            {
                short left = BitConverter.ToInt16(data, 6), top = BitConverter.ToInt16(data, 8), right = BitConverter.ToInt16(data, 10), bottom = BitConverter.ToInt16(data, 12);
                int inch = BitConverter.ToUInt16(data, 14);
                if (inch == 0) inch = 1440;
                var m = new GpMetafile
                {
                    Frame = new RectangleF(0, 0, Math.Max(1, (right - left) * 96f / inch), Math.Max(1, (bottom - top) * 96f / inch)),
                    EmfType = EmfType.EmfOnly,
                    RawFormat = WmfGuid,
                };
                using (var recorder = new SkiaSharp.SKPictureRecorder())
                {
                    recorder.BeginRecording(new SkiaSharp.SKRect(0, 0, m.Frame.Width, m.Frame.Height));
                    m.Picture = recorder.EndRecording();
                }
                return m;
            }

            // BMP

            private static GpBitmap? ReadBmp(byte[] data)
            {
                int dataOffset = BitConverter.ToInt32(data, 10);
                int headerSize = BitConverter.ToInt32(data, 14);
                int width, height, bpp, compression = 0, colorsUsed = 0;
                float dpiX = 96, dpiY = 96;
                if (headerSize == 12)
                {
                    width = BitConverter.ToInt16(data, 18);
                    height = BitConverter.ToInt16(data, 20);
                    bpp = BitConverter.ToInt16(data, 24);
                }
                else
                {
                    width = BitConverter.ToInt32(data, 18);
                    height = BitConverter.ToInt32(data, 22);
                    bpp = BitConverter.ToInt16(data, 28);
                    compression = BitConverter.ToInt32(data, 30);
                    int x = BitConverter.ToInt32(data, 38), y = BitConverter.ToInt32(data, 42);
                    if (x > 0 && y > 0) { dpiX = (float)(x * 0.0254); dpiY = (float)(y * 0.0254); }
                    colorsUsed = BitConverter.ToInt32(data, 46);
                }
                bool bottomUp = height > 0;
                height = Math.Abs(height);
                if (width <= 0 || height <= 0)
                    return null;
                if (compression != 0 && !(compression == 3 && (bpp == 16 || bpp == 32)))
                    return null;   // RLE, JPEG, PNG in a BMP: Skia's
                PixelFormat format = bpp switch
                {
                    1 => PixelFormat.Format1bppIndexed,
                    4 => PixelFormat.Format4bppIndexed,
                    8 => PixelFormat.Format8bppIndexed,
                    16 => PixelFormat.Format16bppRgb555,
                    24 => PixelFormat.Format24bppRgb,
                    32 => PixelFormat.Format32bppRgb,
                    _ => 0,
                };
                if (format == 0)
                    return null;
                uint redMask = 0, greenMask = 0, blueMask = 0, alphaMask = 0;
                if (compression == 3)
                {
                    redMask = BitConverter.ToUInt32(data, 14 + 40);
                    greenMask = BitConverter.ToUInt32(data, 14 + 44);
                    blueMask = BitConverter.ToUInt32(data, 14 + 48);
                    if (headerSize >= 56)
                        alphaMask = BitConverter.ToUInt32(data, 14 + 52);
                    if (bpp == 16 && redMask == 0xf800 && greenMask == 0x07e0 && blueMask == 0x001f)
                        format = PixelFormat.Format16bppRgb565;
                    if (bpp == 32 && alphaMask != 0)
                        format = PixelFormat.Format32bppArgb;
                }
                else if (bpp == 32 && headerSize >= 56 && BitConverter.ToUInt32(data, 14 + 52) != 0)
                {
                    format = PixelFormat.Format32bppArgb;
                }
                GpBitmap b = GpBitmap.Create(width, height, format);
                if (bpp <= 8)
                {
                    int entries = colorsUsed > 0 ? colorsUsed : 1 << bpp;
                    int paletteAt = 14 + headerSize, entrySize = headerSize == 12 ? 3 : 4;
                    int size = 1 << bpp;
                    var palette = new int[Math.Max(entries, size)];
                    for (int i = 0; i < entries && paletteAt + i * entrySize + 2 < data.Length; i++)
                        palette[i] = unchecked((int)0xFF000000) | (data[paletteAt + i * entrySize + 2] << 16) | (data[paletteAt + i * entrySize + 1] << 8) | data[paletteAt + i * entrySize];
                    for (int i = entries; i < palette.Length; i++)
                        palette[i] = unchecked((int)0xFF000000);
                    b.Palette = palette;
                    b.PaletteFlags = 0;
                }
                int rowSize = (bpp * width + 31) / 32 * 4;
                int copy = Math.Min(rowSize, b.Stride);
                for (int y = 0; y < height; y++)
                {
                    int src = dataOffset + (bottomUp ? height - 1 - y : y) * rowSize;
                    if (src + copy > data.Length)
                        break;
                    Marshal.Copy(data, src, (IntPtr)((byte*)b.Scan0 + (long)y * b.Stride), copy);
                }
                if (format == PixelFormat.Format32bppRgb)
                    for (int y = 0; y < height; y++)
                    {
                        uint* row = (uint*)((byte*)b.Scan0 + (long)y * b.Stride);
                        for (int x = 0; x < width; x++)
                            row[x] |= 0xFF000000;
                    }
                b.RawFormat = BmpGuid;
                b.Flags = (int)(ImageFlags.ColorSpaceRgb | ImageFlags.HasRealPixelSize | ImageFlags.HasRealDpi | ImageFlags.ReadOnly) | (GpBitmap.HasAlpha(format) ? (int)ImageFlags.HasAlpha : 0);
                b.DpiX = dpiX;
                b.DpiY = dpiY;
                return b;
            }

            /// <summary>A BMP as GDI+ writes one: its pixel format's (indexed with the palette; 32 bpp with alpha as it is), bottom-up.</summary>
            private static byte[] WriteBmp(GpBitmap bitmap)
            {
                bitmap.EnsureScan0();
                int bpp = bitmap.Format switch
                {
                    PixelFormat.Format1bppIndexed => 1,
                    PixelFormat.Format4bppIndexed => 4,
                    PixelFormat.Format8bppIndexed => 8,
                    PixelFormat.Format16bppRgb555 or PixelFormat.Format16bppRgb565 or PixelFormat.Format16bppArgb1555 => 16,
                    PixelFormat.Format24bppRgb or PixelFormat.Format48bppRgb => 24,
                    _ => 32,
                };
                int paletteEntries = bpp <= 8 ? 1 << bpp : 0;
                bool masks = bitmap.Format == PixelFormat.Format16bppRgb565;
                int headerSize = 40 + (masks ? 12 : 0);
                int rowSize = (bpp * bitmap.W + 31) / 32 * 4;
                int offset = 14 + headerSize + paletteEntries * 4;
                var bmp = new byte[offset + rowSize * bitmap.H];
                var w = new BinaryWriter(new MemoryStream(bmp));
                w.Write((byte)'B'); w.Write((byte)'M'); w.Write(bmp.Length); w.Write(0); w.Write(offset);
                w.Write(40); w.Write(bitmap.W); w.Write(bitmap.H); w.Write((short)1); w.Write((short)bpp); w.Write(masks ? 3 : 0); w.Write(rowSize * bitmap.H);
                w.Write((int)Math.Round(bitmap.DpiX / 0.0254)); w.Write((int)Math.Round(bitmap.DpiY / 0.0254)); w.Write(paletteEntries); w.Write(0);
                if (masks) { w.Write(0xf800); w.Write(0x07e0); w.Write(0x001f); }
                int[] palette = bitmap.Palette ?? Array.Empty<int>();
                for (int i = 0; i < paletteEntries; i++)
                {
                    int c = i < palette.Length ? palette[i] : 0;
                    w.Write((byte)c); w.Write((byte)(c >> 8)); w.Write((byte)(c >> 16)); w.Write((byte)0);
                }
                bool sameLayout = GpBitmap.BitsPerPixel(bitmap.Format) == bpp && bitmap.Format != PixelFormat.Format32bppPArgb;
                for (int y = 0; y < bitmap.H; y++)
                {
                    int row = offset + (bitmap.H - 1 - y) * rowSize;
                    if (sameLayout)
                    {
                        Marshal.Copy((IntPtr)((byte*)bitmap.Scan0 + (long)y * bitmap.Stride), bmp, row, Math.Min(rowSize, bitmap.Stride));
                        continue;
                    }
                    for (int x = 0; x < bitmap.W; x++)
                    {
                        int argb = bitmap.GetArgb(x, y);
                        int at = row + x * (bpp / 8);
                        bmp[at] = (byte)argb; bmp[at + 1] = (byte)(argb >> 8); bmp[at + 2] = (byte)(argb >> 16);
                        if (bpp == 32)
                            bmp[at + 3] = (byte)(argb >> 24);
                    }
                }
                return bmp;
            }

            // GIF

            private static GpImage? ReadGif(byte[] data)
            {
                int width = data[6] | (data[7] << 8), height = data[8] | (data[9] << 8);
                int flags = data[10];
                int background = data[11];
                int at = 13;
                int[]? global = null;
                if ((flags & 0x80) != 0)
                {
                    int n = 2 << (flags & 7);
                    global = new int[n];
                    for (int i = 0; i < n; i++, at += 3)
                        global[i] = unchecked((int)0xFF000000) | (data[at] << 16) | (data[at + 1] << 8) | data[at + 2];
                }
                if (width <= 0 || height <= 0)
                    return null;
                var frames = new List<GpBitmap>();
                var delays = new List<int>();
                int loop = -1;
                int transparent = -1, disposal = 0, delay = 0;
                var canvas = new int[width * height];
                int fill = global != null && background < global.Length ? 0 : 0;
                for (int i = 0; i < canvas.Length; i++)
                    canvas[i] = fill;
                int[]? previous = null;
                bool anyTransparent = false;
                int[]? singlePalette = null;
                byte[]? singleIndices = null;
                while (at < data.Length)
                {
                    byte block = data[at++];
                    if (block == 0x3b)
                        break;
                    if (block == 0x21)
                    {
                        byte label = data[at++];
                        if (label == 0xf9 && data[at] >= 4)
                        {
                            int f = data[at + 1];
                            disposal = (f >> 2) & 7;
                            delay = data[at + 2] | (data[at + 3] << 8);
                            transparent = (f & 1) != 0 ? data[at + 4] : -1;
                        }
                        else if (label == 0xff && data[at] == 11 && Encoding.ASCII.GetString(data, at + 1, 11) is "NETSCAPE2.0" or "ANIMEXTS1.0")
                        {
                            int sub = at + 12;
                            if (data[sub] >= 3 && data[sub + 1] == 1)
                                loop = data[sub + 2] | (data[sub + 3] << 8);
                        }
                        while (at < data.Length && data[at] != 0)
                            at += data[at] + 1;
                        at++;
                        continue;
                    }
                    if (block != 0x2c)
                        break;
                    int left = data[at] | (data[at + 1] << 8), top = data[at + 2] | (data[at + 3] << 8);
                    int w = data[at + 4] | (data[at + 5] << 8), h = data[at + 6] | (data[at + 7] << 8);
                    int iflags = data[at + 8];
                    at += 9;
                    int[] palette = global ?? DefaultPalette(PixelFormat.Format8bppIndexed, out _);
                    if ((iflags & 0x80) != 0)
                    {
                        int n = 2 << (iflags & 7);
                        palette = new int[n];
                        for (int i = 0; i < n; i++, at += 3)
                            palette[i] = unchecked((int)0xFF000000) | (data[at] << 16) | (data[at + 1] << 8) | data[at + 2];
                    }
                    int minCode = data[at++];
                    var compressed = new MemoryStream();
                    while (at < data.Length && data[at] != 0)
                    {
                        compressed.Write(data, at + 1, data[at]);
                        at += data[at] + 1;
                    }
                    at++;
                    byte[] indices = GifLzwDecode(compressed.ToArray(), minCode, w * h);
                    if ((iflags & 0x40) != 0)
                        indices = Deinterlace(indices, w, h);
                    if (disposal == 3)
                        previous = (int[])canvas.Clone();
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                        {
                            int cx = left + x, cy = top + y;
                            if (cx >= width || cy >= height)
                                continue;
                            int index = indices[y * w + x];
                            if (index == transparent)
                            {
                                anyTransparent = true;
                                continue;
                            }
                            canvas[cy * width + cx] = index < palette.Length ? palette[index] : unchecked((int)0xFF000000);
                        }
                    if (frames.Count == 0 && left == 0 && top == 0 && w == width && h == height)
                    {
                        singleIndices = indices;
                        singlePalette = (int[])palette.Clone();
                        if (transparent >= 0 && transparent < singlePalette.Length)
                            singlePalette[transparent] = 0;
                    }
                    else
                    {
                        singleIndices = null;
                    }
                    GpBitmap frame = GpBitmap.Create(width, height, PixelFormat.Format32bppArgb);
                    for (int y = 0; y < height; y++)
                        for (int x = 0; x < width; x++)
                            frame.SetArgb(x, y, canvas[y * width + x]);
                    frames.Add(frame);
                    delays.Add(delay);
                    if (disposal == 2)
                        for (int y = top; y < Math.Min(top + h, height); y++)
                            for (int x = left; x < Math.Min(left + w, width); x++)
                                canvas[y * width + x] = 0;
                    else if (disposal == 3 && previous != null)
                        canvas = previous;
                    transparent = -1;
                    disposal = 0;
                    delay = 0;
                }
                if (frames.Count == 0)
                    return null;
                GpBitmap image;
                if (frames.Count == 1 && singleIndices != null)
                {
                    // One frame: 8 bpp indexed, its palette, as GDI+ gives it.
                    frames[0].Dispose();
                    image = GpBitmap.Create(width, height, PixelFormat.Format8bppIndexed);
                    image.Palette = singlePalette!.Length >= 256 ? singlePalette : Pad(singlePalette, 256);
                    image.PaletteFlags = anyTransparent ? (int)PaletteFlags.HasAlpha : 0;
                    for (int y = 0; y < height; y++)
                        Marshal.Copy(singleIndices, y * width, (IntPtr)((byte*)image.Scan0 + (long)y * image.Stride), width);
                }
                else
                {
                    image = WithFrames(frames, FrameDimensionTime);
                }
                bool alpha = anyTransparent || (image.Palette != null && Array.Exists(image.Palette, c => ((uint)c >> 24) < 255));
                image.RawFormat = GifGuid;
                image.Flags = (int)(ImageFlags.ColorSpaceRgb | ImageFlags.HasRealPixelSize | ImageFlags.HasRealDpi | ImageFlags.ReadOnly) | (alpha ? (int)ImageFlags.HasAlpha : 0);
                var delayBytes = new byte[delays.Count * 4];
                for (int i = 0; i < delays.Count; i++)
                    BitConverter.TryWriteBytes(delayBytes.AsSpan(i * 4), delays[i]);
                image.Properties.Add(new Property { Id = 0x5100, Type = 4, Value = delayBytes });
                if (loop >= 0)
                    image.Properties.Add(new Property { Id = 0x5101, Type = 3, Value = BitConverter.GetBytes((short)loop) });
                if (global != null)
                {
                    var paletteBytes = new byte[global.Length * 3];
                    for (int i = 0; i < global.Length; i++)
                    {
                        paletteBytes[i * 3] = (byte)(global[i] >> 16); paletteBytes[i * 3 + 1] = (byte)(global[i] >> 8); paletteBytes[i * 3 + 2] = (byte)global[i];
                    }
                    image.Properties.Add(new Property { Id = 0x5102, Type = 1, Value = paletteBytes });
                    image.Properties.Add(new Property { Id = 0x5103, Type = 1, Value = new[] { (byte)background } });
                }
                return image;
            }

            private static int[] Pad(int[] palette, int size)
            {
                var p = new int[size];
                Array.Copy(palette, p, Math.Min(palette.Length, size));
                return p;
            }

            private static byte[] Deinterlace(byte[] indices, int w, int h)
            {
                var result = new byte[indices.Length];
                int row = 0;
                foreach ((int start, int step) in new[] { (0, 8), (4, 8), (2, 4), (1, 2) })
                    for (int y = start; y < h; y += step, row++)
                        Array.Copy(indices, row * w, result, y * w, w);
                return result;
            }

            private static byte[] GifLzwDecode(byte[] data, int minCode, int pixels)
            {
                var output = new byte[pixels];
                int clear = 1 << minCode, end = clear + 1;
                int codeSize = minCode + 1, next = clear + 2;
                var prefix = new int[4096];
                var suffix = new byte[4096];
                var length = new int[4096];
                for (int i = 0; i < clear; i++) { prefix[i] = -1; suffix[i] = (byte)i; length[i] = 1; }
                int old = -1, outAt = 0, bitPos = 0;
                var stack = new byte[4097];
                while (outAt < pixels)
                {
                    if ((bitPos + codeSize) > data.Length * 8)
                        break;
                    int code = 0;
                    for (int i = 0; i < codeSize; i++, bitPos++)
                        if ((data[bitPos >> 3] & (1 << (bitPos & 7))) != 0)
                            code |= 1 << i;
                    if (code == clear)
                    {
                        codeSize = minCode + 1;
                        next = clear + 2;
                        old = -1;
                        continue;
                    }
                    if (code == end)
                        break;
                    int c = code;
                    int sp = 0;
                    byte first;
                    if (code >= next)
                    {
                        if (old < 0) break;
                        c = old;
                        int t = c;
                        while (t >= 0 && sp < stack.Length) { stack[sp++] = suffix[t]; t = prefix[t]; }
                        first = stack[sp - 1];
                        // the code is old + first: written below
                        for (int i = sp - 1; i >= 0 && outAt < pixels; i--) output[outAt++] = stack[i];
                        if (outAt < pixels) output[outAt++] = first;
                    }
                    else
                    {
                        int t = c;
                        while (t >= 0 && sp < stack.Length) { stack[sp++] = suffix[t]; t = prefix[t]; }
                        first = stack[sp - 1];
                        for (int i = sp - 1; i >= 0 && outAt < pixels; i--) output[outAt++] = stack[i];
                    }
                    if (old >= 0 && next < 4096)
                    {
                        prefix[next] = old;
                        suffix[next] = first;
                        length[next] = length[old] + 1;
                        next++;
                        if (next == (1 << codeSize) && codeSize < 12)
                            codeSize++;
                    }
                    old = code;
                }
                return output;
            }

            /// <summary>A GIF of one frame, 8 bpp: the image's palette, or (not indexed) GDI+'s halftone palette.</summary>
            private static byte[] WriteGif(GpBitmap bitmap)
            {
                bitmap.EnsureScan0();
                int[] palette;
                var indices = new byte[bitmap.W * bitmap.H];
                if (bitmap.Format == PixelFormat.Format8bppIndexed || bitmap.Format == PixelFormat.Format4bppIndexed || bitmap.Format == PixelFormat.Format1bppIndexed)
                {
                    palette = Pad(bitmap.Palette ?? DefaultPalette(bitmap.Format, out _), 256);
                    for (int y = 0; y < bitmap.H; y++)
                        for (int x = 0; x < bitmap.W; x++)
                            indices[y * bitmap.W + x] = (byte)bitmap.NearestIndex(bitmap.GetArgb(x, y));
                    if (bitmap.Format == PixelFormat.Format8bppIndexed)
                        for (int y = 0; y < bitmap.H; y++)
                            Marshal.Copy((IntPtr)((byte*)bitmap.Scan0 + (long)y * bitmap.Stride), indices, y * bitmap.W, bitmap.W);
                }
                else
                {
                    palette = HalftonePalette();
                    var cache = new Dictionary<int, byte>();
                    for (int y = 0; y < bitmap.H; y++)
                        for (int x = 0; x < bitmap.W; x++)
                        {
                            int argb = bitmap.GetArgb(x, y) | unchecked((int)0xFF000000);
                            if (!cache.TryGetValue(argb, out byte index))
                            {
                                index = (byte)NearestOpaque(palette, argb);
                                cache[argb] = index;
                            }
                            indices[y * bitmap.W + x] = index;
                        }
                }
                int transparent = Array.FindIndex(palette, c => ((uint)c >> 24) == 0);
                var ms = new MemoryStream();
                var w = new BinaryWriter(ms);
                w.Write(Encoding.ASCII.GetBytes("GIF89a"));
                w.Write((short)bitmap.W); w.Write((short)bitmap.H);
                w.Write((byte)0xf7); w.Write((byte)0); w.Write((byte)0);
                for (int i = 0; i < 256; i++)
                {
                    int c = palette[i];
                    w.Write((byte)(c >> 16)); w.Write((byte)(c >> 8)); w.Write((byte)c);
                }
                if (transparent >= 0)
                {
                    w.Write((byte)0x21); w.Write((byte)0xf9); w.Write((byte)4); w.Write((byte)1); w.Write((short)0); w.Write((byte)transparent); w.Write((byte)0);
                }
                w.Write((byte)0x2c); w.Write((short)0); w.Write((short)0); w.Write((short)bitmap.W); w.Write((short)bitmap.H); w.Write((byte)0);
                w.Write((byte)8);
                byte[] lzw = GifLzwEncode(indices, 8);
                for (int i = 0; i < lzw.Length; i += 255)
                {
                    int n = Math.Min(255, lzw.Length - i);
                    w.Write((byte)n);
                    w.Write(lzw, i, n);
                }
                w.Write((byte)0);
                w.Write((byte)0x3b);
                return ms.ToArray();
            }

            private static int NearestOpaque(int[] palette, int argb)
            {
                int best = 0, bestDistance = int.MaxValue;
                for (int i = 0; i < palette.Length; i++)
                {
                    int p = palette[i];
                    if (((uint)p >> 24) != 255)
                        continue;
                    int dr = ((p >> 16) & 0xff) - ((argb >> 16) & 0xff), dg = ((p >> 8) & 0xff) - ((argb >> 8) & 0xff), db = (p & 0xff) - (argb & 0xff);
                    int d = dr * dr + dg * dg + db * db;
                    if (d < bestDistance) { bestDistance = d; best = i; }
                }
                return best;
            }

            private static byte[] GifLzwEncode(byte[] indices, int minCode)
            {
                var output = new List<byte>();
                int clear = 1 << minCode, end = clear + 1;
                int codeSize = minCode + 1, next = clear + 2;
                var table = new Dictionary<int, int>();
                int bitBuffer = 0, bitCount = 0;
                void Emit(int code)
                {
                    bitBuffer |= code << bitCount;
                    bitCount += codeSize;
                    while (bitCount >= 8)
                    {
                        output.Add((byte)bitBuffer);
                        bitBuffer >>= 8;
                        bitCount -= 8;
                    }
                }
                Emit(clear);
                if (indices.Length == 0)
                {
                    Emit(end);
                    if (bitCount > 0) output.Add((byte)bitBuffer);
                    return output.ToArray();
                }
                int prefix = indices[0];
                for (int i = 1; i < indices.Length; i++)
                {
                    int k = indices[i];
                    int key = (prefix << 8) | k;
                    if (table.TryGetValue(key, out int code))
                    {
                        prefix = code;
                        continue;
                    }
                    Emit(prefix);
                    if (next < 4096)
                    {
                        table[key] = next++;
                        if (next > (1 << codeSize) && codeSize < 12)
                            codeSize++;
                    }
                    else
                    {
                        Emit(clear);
                        table.Clear();
                        codeSize = minCode + 1;
                        next = clear + 2;
                    }
                    prefix = k;
                }
                Emit(prefix);
                Emit(end);
                if (bitCount > 0)
                    output.Add((byte)bitBuffer);
                return output.ToArray();
            }

            // TIFF

            private static GpImage? ReadTiff(byte[] data)
            {
                var tiff = new TiffReader(data, 0, data.Length);
                var pages = new List<GpBitmap>();
                int ifd = tiff.FirstIfd;
                var seenIfds = new HashSet<int>();
                while (ifd > 0 && seenIfds.Add(ifd))
                {
                    GpBitmap? page = ReadTiffPage(tiff, ifd);
                    if (page == null)
                        break;
                    pages.Add(page);
                    ifd = tiff.NextIfd(ifd);
                }
                if (pages.Count == 0)
                    return null;
                GpBitmap image = WithFrames(pages, FrameDimensionPage);
                image.RawFormat = TiffGuid;
                return image;
            }

            private static GpBitmap? ReadTiffPage(TiffReader tiff, int ifd)
            {
                int width = 0, height = 0, compression = 1, photometric = 2, samples = 1, predictor = 1, planar = 1, rowsPerStrip = int.MaxValue, extra = 0;
                int[] bits = { 1 };
                long[] offsets = Array.Empty<long>(), counts = Array.Empty<long>();
                int[]? colormap = null;
                double dpiX = 0, dpiY = 0;
                int unit = 2;
                foreach (TiffEntry e in tiff.Entries(ifd))
                {
                    switch (e.Tag)
                    {
                        case 256: width = (int)tiff.Number(e, 0); break;
                        case 257: height = (int)tiff.Number(e, 0); break;
                        case 258: bits = new int[e.Count]; for (int i = 0; i < e.Count; i++) bits[i] = (int)tiff.Number(e, i); break;
                        case 259: compression = (int)tiff.Number(e, 0); break;
                        case 262: photometric = (int)tiff.Number(e, 0); break;
                        case 273: offsets = new long[e.Count]; for (int i = 0; i < e.Count; i++) offsets[i] = tiff.Number(e, i); break;
                        case 277: samples = (int)tiff.Number(e, 0); break;
                        case 278: rowsPerStrip = (int)tiff.Number(e, 0); break;
                        case 279: counts = new long[e.Count]; for (int i = 0; i < e.Count; i++) counts[i] = tiff.Number(e, i); break;
                        case 282: dpiX = tiff.Rational(e); break;
                        case 283: dpiY = tiff.Rational(e); break;
                        case 284: planar = (int)tiff.Number(e, 0); break;
                        case 296: unit = (int)tiff.Number(e, 0); break;
                        case 317: predictor = (int)tiff.Number(e, 0); break;
                        case 320:
                            {
                                int n = e.Count / 3;
                                colormap = new int[n];
                                for (int i = 0; i < n; i++)
                                    colormap[i] = unchecked((int)0xFF000000) | ((int)(tiff.Number(e, i) >> 8) << 16) | ((int)(tiff.Number(e, n + i) >> 8) << 8) | (int)(tiff.Number(e, 2 * n + i) >> 8);
                                break;
                            }
                        case 338: extra = (int)tiff.Number(e, 0); break;
                    }
                }
                if (width <= 0 || height <= 0 || planar != 1 || offsets.Length == 0)
                    return null;
                int bps = bits[0];
                int rowBytes = (width * samples * bps + 7) / 8;
                var raw = new byte[rowBytes * height];
                int rowAt = 0;
                rowsPerStrip = Math.Min(rowsPerStrip, height);
                for (int s = 0; s < offsets.Length && rowAt < height; s++)
                {
                    int rows = Math.Min(rowsPerStrip, height - rowAt);
                    byte[] strip = tiff.Bytes((int)offsets[s], s < counts.Length ? (int)counts[s] : rowBytes * rows);
                    byte[] decoded = compression switch
                    {
                        1 => strip,
                        5 => TiffLzwDecode(strip, rowBytes * rows),
                        32773 => PackBitsDecode(strip, rowBytes * rows),
                        8 or 32946 => Inflate(strip),
                        _ => throw new InvalidDataException(),
                    };
                    int n = Math.Min(decoded.Length, rowBytes * rows);
                    Array.Copy(decoded, 0, raw, rowAt * rowBytes, n);
                    rowAt += rows;
                }
                if (predictor == 2 && bps == 8)
                    for (int y = 0; y < height; y++)
                        for (int x = samples; x < rowBytes; x++)
                            raw[y * rowBytes + x] = (byte)(raw[y * rowBytes + x] + raw[y * rowBytes + x - samples]);
                PixelFormat format;
                if (photometric == 3 || ((photometric == 0 || photometric == 1) && samples == 1))
                    format = bps switch { 1 => PixelFormat.Format1bppIndexed, 4 => PixelFormat.Format4bppIndexed, _ => PixelFormat.Format8bppIndexed };
                else if (samples >= 4)
                    format = extra == 1 ? PixelFormat.Format32bppPArgb : PixelFormat.Format32bppArgb;
                else
                    format = PixelFormat.Format24bppRgb;
                GpBitmap b = GpBitmap.Create(width, height, format);
                if (GpBitmap.IsIndexed(format))
                {
                    int size = 1 << Math.Min(bps, 8);
                    if (photometric == 3 && colormap != null)
                        b.Palette = Pad(colormap, Math.Max(colormap.Length, size));
                    else
                    {
                        var gray = new int[size];
                        for (int i = 0; i < size; i++)
                        {
                            int v = i * 255 / (size - 1);
                            if (photometric == 0) v = 255 - v;
                            gray[i] = unchecked((int)0xFF000000) | (v << 16) | (v << 8) | v;
                        }
                        b.Palette = gray;
                        b.PaletteFlags = (int)PaletteFlags.GrayScale;
                    }
                    for (int y = 0; y < height; y++)
                        Marshal.Copy(raw, y * rowBytes, (IntPtr)((byte*)b.Scan0 + (long)y * b.Stride), Math.Min(rowBytes, b.Stride));
                }
                else
                {
                    for (int y = 0; y < height; y++)
                        for (int x = 0; x < width; x++)
                        {
                            int at = y * rowBytes + x * samples;
                            int r = raw[at], g = samples >= 3 ? raw[at + 1] : r, bl = samples >= 3 ? raw[at + 2] : r;
                            int a = samples >= 4 ? raw[at + 3] : 255;
                            if (format == PixelFormat.Format32bppPArgb)
                                ((uint*)((byte*)b.Scan0 + (long)y * b.Stride))[x] = (uint)((a << 24) | (r << 16) | (g << 8) | bl);
                            else
                                b.SetArgb(x, y, (a << 24) | (r << 16) | (g << 8) | bl);
                        }
                }
                if (dpiX > 0 && dpiY > 0)
                {
                    double f = unit == 3 ? 2.54 : 1;
                    b.DpiX = (float)(dpiX * f);
                    b.DpiY = (float)(dpiY * f);
                }
                b.RawFormat = TiffGuid;
                b.Flags = (int)(ImageFlags.ColorSpaceRgb | ImageFlags.HasRealPixelSize | ImageFlags.HasRealDpi | ImageFlags.ReadOnly) | (GpBitmap.HasAlpha(format) ? (int)ImageFlags.HasAlpha : 0);
                return b;
            }

            private static byte[] Inflate(byte[] data)
            {
                using var input = new MemoryStream(data);
                using var z = new ZLibStream(input, CompressionMode.Decompress);
                using var output = new MemoryStream();
                z.CopyTo(output);
                return output.ToArray();
            }

            private static byte[] Deflate(byte[] data)
            {
                using var output = new MemoryStream();
                using (var z = new ZLibStream(output, CompressionLevel.Optimal, leaveOpen: true))
                    z.Write(data, 0, data.Length);
                return output.ToArray();
            }

            private static byte[] PackBitsDecode(byte[] data, int expected)
            {
                var output = new List<byte>(expected);
                int i = 0;
                while (i < data.Length && output.Count < expected)
                {
                    sbyte n = (sbyte)data[i++];
                    if (n >= 0)
                    {
                        for (int k = 0; k <= n && i < data.Length; k++) output.Add(data[i++]);
                    }
                    else if (n != -128 && i < data.Length)
                    {
                        byte b = data[i++];
                        for (int k = 0; k < 1 - n; k++) output.Add(b);
                    }
                }
                return output.ToArray();
            }

            /// <summary>TIFF's LZW (MSB first, the early change).</summary>
            private static byte[] TiffLzwDecode(byte[] data, int expected)
            {
                var output = new List<byte>(expected);
                var table = new List<byte[]>(4096);
                void Reset()
                {
                    table.Clear();
                    for (int i = 0; i < 256; i++) table.Add(new[] { (byte)i });
                    table.Add(Array.Empty<byte>());
                    table.Add(Array.Empty<byte>());
                }
                Reset();
                int codeSize = 9, bitPos = 0;
                byte[]? old = null;
                while (bitPos + codeSize <= data.Length * 8)
                {
                    int code = 0;
                    for (int i = 0; i < codeSize; i++, bitPos++)
                        code = (code << 1) | ((data[bitPos >> 3] >> (7 - (bitPos & 7))) & 1);
                    if (code == 256)
                    {
                        Reset();
                        codeSize = 9;
                        old = null;
                        continue;
                    }
                    if (code == 257)
                        break;
                    byte[] entry;
                    if (code < table.Count)
                    {
                        entry = table[code];
                        if (old != null)
                        {
                            var added = new byte[old.Length + 1];
                            old.CopyTo(added, 0);
                            added[^1] = entry[0];
                            table.Add(added);
                        }
                    }
                    else
                    {
                        if (old == null)
                            break;
                        entry = new byte[old.Length + 1];
                        old.CopyTo(entry, 0);
                        entry[^1] = old[0];
                        table.Add(entry);
                    }
                    output.AddRange(entry);
                    old = entry;
                    if (table.Count + 1 >= (1 << codeSize) && codeSize < 12)
                        codeSize++;
                }
                return output.ToArray();
            }

            private static byte[] TiffLzwEncode(byte[] data)
            {
                var output = new List<byte>();
                int bitBuffer = 0, bitCount = 0, codeSize = 9, next = 258;
                var table = new Dictionary<long, int>();
                void Emit(int code)
                {
                    bitBuffer = (bitBuffer << codeSize) | code;
                    bitCount += codeSize;
                    while (bitCount >= 8)
                    {
                        output.Add((byte)(bitBuffer >> (bitCount - 8)));
                        bitCount -= 8;
                    }
                    bitBuffer &= (1 << bitCount) - 1;
                }
                Emit(256);
                if (data.Length == 0)
                {
                    Emit(257);
                    if (bitCount > 0) output.Add((byte)(bitBuffer << (8 - bitCount)));
                    return output.ToArray();
                }
                int prefix = data[0];
                for (int i = 1; i < data.Length; i++)
                {
                    long key = ((long)prefix << 8) | data[i];
                    if (table.TryGetValue(key, out int code))
                    {
                        prefix = code;
                        continue;
                    }
                    Emit(prefix);
                    table[key] = next++;
                    if (next + 1 >= (1 << codeSize))
                    {
                        if (codeSize < 12)
                            codeSize++;
                    }
                    if (next >= 4094)
                    {
                        Emit(256);
                        table.Clear();
                        next = 258;
                        codeSize = 9;
                    }
                    prefix = data[i];
                }
                Emit(prefix);
                if (next + 1 >= (1 << codeSize) && codeSize < 12)
                    codeSize++;
                Emit(257);
                if (bitCount > 0)
                    output.Add((byte)(bitBuffer << (8 - bitCount)));
                return output.ToArray();
            }

            /// <summary>A TIFF of pages, as GDI+ writes them: RGBA (or RGB, or a palette), LZW, the horizontal predictor.</summary>
            private static byte[] WriteTiff(List<GpBitmap> pages)
            {
                var ms = new MemoryStream();
                var w = new BinaryWriter(ms);
                w.Write((byte)'I'); w.Write((byte)'I'); w.Write((short)42); w.Write(0);
                long nextIfdAt = 4;
                foreach (GpBitmap page in pages)
                {
                    page.EnsureScan0();
                    bool indexed = GpBitmap.IsIndexed(page.Format);
                    bool alpha = !indexed && GpBitmap.HasAlpha(page.Format);
                    int samples = indexed ? 1 : alpha ? 4 : 3;
                    int rowBytes = page.W * samples;
                    var raw = new byte[rowBytes * page.H];
                    for (int y = 0; y < page.H; y++)
                        for (int x = 0; x < page.W; x++)
                        {
                            int at = y * rowBytes + x * samples;
                            if (indexed)
                            {
                                raw[at] = (byte)page.NearestIndex(page.GetArgb(x, y));
                                continue;
                            }
                            int argb = page.GetArgb(x, y);
                            raw[at] = (byte)(argb >> 16); raw[at + 1] = (byte)(argb >> 8); raw[at + 2] = (byte)argb;
                            if (alpha) raw[at + 3] = (byte)(argb >> 24);
                        }
                    if (!indexed)
                        for (int y = 0; y < page.H; y++)
                            for (int x = rowBytes - 1; x >= samples; x--)
                                raw[y * rowBytes + x] = (byte)(raw[y * rowBytes + x] - raw[y * rowBytes + x - samples]);
                    byte[] strip = TiffLzwEncode(raw);
                    long stripAt = ms.Position;
                    w.Write(strip);
                    if (ms.Position % 2 != 0) w.Write((byte)0);
                    long bitsAt = ms.Position;
                    for (int i = 0; i < samples; i++) w.Write((short)8);
                    long xresAt = ms.Position;
                    w.Write((int)Math.Round(page.DpiX * 1000)); w.Write(1000);
                    long yresAt = ms.Position;
                    w.Write((int)Math.Round(page.DpiY * 1000)); w.Write(1000);
                    long mapAt = 0;
                    if (indexed)
                    {
                        mapAt = ms.Position;
                        int[] palette = Pad(page.Palette ?? Array.Empty<int>(), 256);
                        for (int c = 0; c < 3; c++)
                            for (int i = 0; i < 256; i++)
                                w.Write((ushort)((((palette[i] >> (16 - 8 * c)) & 0xff) * 257)));
                    }
                    var entries = new List<(int Tag, int Type, int Count, long Value)>
                    {
                        (254, 4, 1, 0),
                        (256, 4, 1, page.W),
                        (257, 4, 1, page.H),
                        (258, 3, samples, samples == 1 ? 8 : bitsAt),
                        (259, 3, 1, 5),
                        (262, 3, 1, indexed ? 3 : 2),
                        (273, 4, 1, stripAt),
                        (277, 3, 1, samples),
                        (278, 4, 1, page.H),
                        (279, 4, 1, strip.Length),
                        (282, 5, 1, xresAt),
                        (283, 5, 1, yresAt),
                        (284, 3, 1, 1),
                        (296, 3, 1, 2),
                    };
                    if (!indexed)
                        entries.Add((317, 3, 1, 2));
                    if (indexed)
                        entries.Add((320, 3, 768, mapAt));
                    if (alpha)
                        entries.Add((338, 3, 1, 2));
                    long ifdAt = ms.Position;
                    ms.Position = nextIfdAt;
                    w.Write((int)ifdAt);
                    ms.Position = ifdAt;
                    w.Write((short)entries.Count);
                    foreach (var e in entries)
                    {
                        w.Write((short)e.Tag); w.Write((short)e.Type); w.Write(e.Count);
                        if (e.Type == 3 && e.Count == 1) { w.Write((short)e.Value); w.Write((short)0); }
                        else w.Write((int)e.Value);
                    }
                    nextIfdAt = ms.Position;
                    w.Write(0);
                }
                return ms.ToArray();
            }

            // PNG with a palette

            private static int PngColorType(byte[] data) => data.Length > 25 ? data[25] : -1;

            private static GpBitmap? ReadIndexedPng(byte[] data)
            {
                int at = 8, width = 0, height = 0, depth = 8, interlace = 0;
                int[]? palette = null;
                var idat = new MemoryStream();
                float dpiX = 96, dpiY = 96;
                while (at + 8 <= data.Length)
                {
                    int length = (data[at] << 24) | (data[at + 1] << 16) | (data[at + 2] << 8) | data[at + 3];
                    string type = Encoding.ASCII.GetString(data, at + 4, 4);
                    int body = at + 8;
                    switch (type)
                    {
                        case "IHDR":
                            width = (data[body] << 24) | (data[body + 1] << 16) | (data[body + 2] << 8) | data[body + 3];
                            height = (data[body + 4] << 24) | (data[body + 5] << 16) | (data[body + 6] << 8) | data[body + 7];
                            depth = data[body + 8];
                            interlace = data[body + 12];
                            break;
                        case "PLTE":
                            palette = new int[length / 3];
                            for (int i = 0; i < palette.Length; i++)
                                palette[i] = unchecked((int)0xFF000000) | (data[body + i * 3] << 16) | (data[body + i * 3 + 1] << 8) | data[body + i * 3 + 2];
                            break;
                        case "tRNS":
                            if (palette != null)
                                for (int i = 0; i < length && i < palette.Length; i++)
                                    palette[i] = (palette[i] & 0x00FFFFFF) | (data[body + i] << 24);
                            break;
                        case "pHYs":
                            if (data[body + 8] == 1)
                            {
                                dpiX = (float)(((data[body] << 24) | (data[body + 1] << 16) | (data[body + 2] << 8) | data[body + 3]) * 0.0254);
                                dpiY = (float)(((data[body + 4] << 24) | (data[body + 5] << 16) | (data[body + 6] << 8) | data[body + 7]) * 0.0254);
                            }
                            break;
                        case "IDAT":
                            idat.Write(data, body, length);
                            break;
                    }
                    if (type == "IEND")
                        break;
                    at += 12 + length;
                }
                if (width <= 0 || height <= 0 || palette == null || interlace != 0)
                    return null;
                byte[] raw = Inflate(idat.ToArray());
                int rowBytes = (width * depth + 7) / 8;
                var pixels = new byte[rowBytes * height];
                var prior = new byte[rowBytes];
                int src = 0;
                for (int y = 0; y < height; y++)
                {
                    int filter = raw[src++];
                    var row = new byte[rowBytes];
                    Array.Copy(raw, src, row, 0, Math.Min(rowBytes, raw.Length - src));
                    src += rowBytes;
                    for (int x = 0; x < rowBytes; x++)
                    {
                        int a = x >= 1 ? row[x - 1] : 0, b = prior[x], c = x >= 1 ? prior[x - 1] : 0;
                        row[x] = filter switch
                        {
                            1 => (byte)(row[x] + a),
                            2 => (byte)(row[x] + b),
                            3 => (byte)(row[x] + ((a + b) >> 1)),
                            4 => (byte)(row[x] + Paeth(a, b, c)),
                            _ => row[x],
                        };
                    }
                    Array.Copy(row, 0, pixels, y * rowBytes, rowBytes);
                    prior = row;
                }
                PixelFormat format = depth switch { 1 => PixelFormat.Format1bppIndexed, 4 => PixelFormat.Format4bppIndexed, _ => PixelFormat.Format8bppIndexed };
                GpBitmap bitmap = GpBitmap.Create(width, height, format);
                bitmap.Palette = Pad(palette, Math.Max(palette.Length, 1 << Math.Min(depth == 2 ? 8 : depth, 8)));
                bitmap.PaletteFlags = Array.Exists(palette, c => ((uint)c >> 24) < 255) ? (int)PaletteFlags.HasAlpha : 0;
                for (int y = 0; y < height; y++)
                {
                    byte* dst = (byte*)bitmap.Scan0 + (long)y * bitmap.Stride;
                    if (depth == 2)
                        for (int x = 0; x < width; x++)
                            dst[x] = (byte)((pixels[y * rowBytes + x / 4] >> (6 - 2 * (x % 4))) & 3);
                    else
                        Marshal.Copy(pixels, y * rowBytes, (IntPtr)dst, Math.Min(rowBytes, bitmap.Stride));
                }
                bitmap.RawFormat = PngGuid;
                bitmap.DpiX = dpiX;
                bitmap.DpiY = dpiY;
                bitmap.Flags = (int)(ImageFlags.ColorSpaceRgb | ImageFlags.HasRealPixelSize | ImageFlags.HasRealDpi | ImageFlags.ReadOnly) | (bitmap.PaletteFlags != 0 ? (int)ImageFlags.HasAlpha : 0);
                return bitmap;
            }

            private static int Paeth(int a, int b, int c)
            {
                int p = a + b - c, pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
                return pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
            }

            /// <summary>An indexed bitmap as a PNG with its palette (GDI+ writes one so).</summary>
            private static byte[] WriteIndexedPng(GpBitmap bitmap)
            {
                bitmap.EnsureScan0();
                int depth = GpBitmap.BitsPerPixel(bitmap.Format);
                int[] palette = bitmap.Palette ?? DefaultPalette(bitmap.Format, out _);
                int entries = Math.Min(palette.Length, 1 << depth);
                var ms = new MemoryStream();
                void Chunk(string type, byte[] body)
                {
                    var header = new byte[8];
                    header[0] = (byte)(body.Length >> 24); header[1] = (byte)(body.Length >> 16); header[2] = (byte)(body.Length >> 8); header[3] = (byte)body.Length;
                    Encoding.ASCII.GetBytes(type).CopyTo(header, 4);
                    ms.Write(header, 0, 8);
                    ms.Write(body, 0, body.Length);
                    var crcData = new byte[4 + body.Length];
                    Encoding.ASCII.GetBytes(type).CopyTo(crcData, 0);
                    body.CopyTo(crcData, 4);
                    uint crc = PngCrc(crcData);
                    ms.Write(new[] { (byte)(crc >> 24), (byte)(crc >> 16), (byte)(crc >> 8), (byte)crc }, 0, 4);
                }
                ms.Write(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }, 0, 8);
                var ihdr = new byte[13];
                ihdr[0] = (byte)(bitmap.W >> 24); ihdr[1] = (byte)(bitmap.W >> 16); ihdr[2] = (byte)(bitmap.W >> 8); ihdr[3] = (byte)bitmap.W;
                ihdr[4] = (byte)(bitmap.H >> 24); ihdr[5] = (byte)(bitmap.H >> 16); ihdr[6] = (byte)(bitmap.H >> 8); ihdr[7] = (byte)bitmap.H;
                ihdr[8] = (byte)depth; ihdr[9] = 3;
                Chunk("IHDR", ihdr);
                var plte = new byte[entries * 3];
                for (int i = 0; i < entries; i++) { plte[i * 3] = (byte)(palette[i] >> 16); plte[i * 3 + 1] = (byte)(palette[i] >> 8); plte[i * 3 + 2] = (byte)palette[i]; }
                Chunk("PLTE", plte);
                if (Array.Exists(palette, c => ((uint)c >> 24) < 255))
                {
                    var trns = new byte[entries];
                    for (int i = 0; i < entries; i++) trns[i] = (byte)((uint)palette[i] >> 24);
                    Chunk("tRNS", trns);
                }
                int rowBytes = (bitmap.W * depth + 7) / 8;
                var raw = new byte[(rowBytes + 1) * bitmap.H];
                for (int y = 0; y < bitmap.H; y++)
                    Marshal.Copy((IntPtr)((byte*)bitmap.Scan0 + (long)y * bitmap.Stride), raw, y * (rowBytes + 1) + 1, rowBytes);
                Chunk("IDAT", Deflate(raw));
                Chunk("IEND", Array.Empty<byte>());
                return ms.ToArray();
            }

            private static uint PngCrc(byte[] data)
            {
                uint crc = 0xffffffff;
                foreach (byte b in data)
                {
                    crc ^= b;
                    for (int k = 0; k < 8; k++)
                        crc = (crc & 1) != 0 ? 0xEDB88320 ^ (crc >> 1) : crc >> 1;
                }
                return crc ^ 0xffffffff;
            }

            // Multi-frame saving (TIFF): the pages added kept, the file written again with each.

            internal sealed class MultiFrameSave
            {
                public List<GpBitmap> Pages = new List<GpBitmap>();
                public StreamPutBytesDelegate? PutBytes;
                public StreamSeekDelegate? Seek;
                public string? FileName;
                public long Start;
            }

            private static long SaveFlag(IntPtr parameters)
            {
                if (parameters == IntPtr.Zero)
                    return -1;
                int count = *(int*)parameters;
                byte* p = (byte*)parameters + IntPtr.Size;
                int size = 16 + 4 + 4 + IntPtr.Size;
                for (int k = 0; k < count; k++, p += size)
                    if (*(Guid*)p == s_saveFlag && *(int*)(p + 16) >= 1)
                    {
                        IntPtr value = *(IntPtr*)(p + 24);
                        if (value != IntPtr.Zero)
                            return *(int*)value;
                    }
                return -1;
            }

            private static int WritePages(MultiFrameSave save)
            {
                byte[] tiff = WriteTiff(save.Pages);
                if (save.FileName != null)
                {
                    File.WriteAllBytes(save.FileName, tiff);
                    return Ok;
                }
                if (save.Seek != null && save.Seek((int)save.Start, 0) < 0)
                    return GenericError;
                fixed (byte* p = tiff)
                    return save.PutBytes!(p, tiff.Length) == tiff.Length ? Ok : GenericError;
            }

            internal static int GdipSaveAdd(IntPtr image, IntPtr encoderParameters)
            {
                if (!Is(image, out GpImage? i) || encoderParameters == IntPtr.Zero)
                    return InvalidParameter;
                if (i.Saving == null)
                    return WrongState;
                long flag = SaveFlag(encoderParameters);
                if (flag == EncoderValueFlush)
                {
                    int status = WritePages(i.Saving);
                    i.Saving = null;
                    return status;
                }
                return Ok;
            }

            internal static int GdipSaveAddImage(IntPtr image, IntPtr imagenew, IntPtr encoderParameters)
            {
                if (!Is(image, out GpImage? i) || !Is(imagenew, out GpBitmap? added) || encoderParameters == IntPtr.Zero)
                    return InvalidParameter;
                if (i.Saving == null)
                    return WrongState;
                i.Saving.Pages.Add((GpBitmap)added.Clone());
                return WritePages(i.Saving);
            }

            /// <summary>What an image's encoder writes, for the formats here; null for Skia's.</summary>
            private static byte[]? EncodeHere(GpBitmap bitmap, Guid format)
            {
                if (format == BmpGuid)
                    return WriteBmp(bitmap);
                if (format == GifGuid)
                    return WriteGif(bitmap);
                if (format == TiffGuid)
                    return WriteTiff(new List<GpBitmap> { bitmap });
                if (format == PngGuid && GpBitmap.IsIndexed(bitmap.Format))
                    return WriteIndexedPng(bitmap);
                return null;
            }
        }
    }
}
