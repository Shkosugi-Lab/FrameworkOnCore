// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if TARGET_UNIX
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;

namespace System.Drawing
{
    // WebFormsForCore: a JPEG's EXIF values in the machine's byte order, as GDI+ gives them. libgdiplus passes them on as
    // the file has them (libexif's entries): a camera's JPEG in Motorola order ("MM", big-endian: most cameras, and GDI+
    // itself) gave its Orientation 6 as 00-06, which BitConverter reads as 1536 (the picture left unturned). The order is
    // the file's, found when it is read (a stream, a file); the values the application sets itself are its own.
    public abstract partial class Image
    {
        // The loaded EXIF values are in the other byte order than the machine's.
        private bool _exifSwapped;

        // The properties the application set (SetPropertyItem): in the machine's order already.
        private HashSet<int>? _propertiesSet;

        /// <summary>Whether the JPEG in <paramref name="stream"/> has its EXIF data in the other byte order than the machine's.</summary>
        internal static bool ExifSwapped(Stream stream)
        {
            if (!stream.CanSeek)
                return false;
            long position = stream.Position;
            try
            {
                // libgdiplus reads EXIF data from the first 64 KiB.
                byte[] start = new byte[65536];
                stream.Seek(0, SeekOrigin.Begin);
                int length = 0, read;
                while (length < start.Length && (read = stream.Read(start, length, start.Length - length)) > 0)
                    length += read;
                return ExifSwapped(start, length);
            }
            catch (IOException)
            {
                return false;
            }
            finally
            {
                stream.Seek(position, SeekOrigin.Begin);
            }
        }

        internal static bool ExifSwapped(string filename)
        {
            try
            {
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    return ExifSwapped(stream);
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        // The JPEG's segments up to its APP1 "Exif" one: its TIFF header's byte order ("MM" or "II").
        private static bool ExifSwapped(byte[] data, int length)
        {
            if (length < 4 || data[0] != 0xFF || data[1] != 0xD8)
                return false;
            int i = 2;
            while (i + 4 <= length)
            {
                if (data[i] != 0xFF)
                    return false;
                byte marker = data[i + 1];
                if (marker == 0xFF)
                {
                    i++;   // fill byte
                    continue;
                }
                if (marker == 0x01 || (marker >= 0xD0 && marker <= 0xD7))
                {
                    i += 2;   // no length
                    continue;
                }
                if (marker == 0xDA || marker == 0xD9)
                    return false;   // the image data: no EXIF before it
                int segment = (data[i + 2] << 8) | data[i + 3];
                if (marker == 0xE1 && segment >= 8 && i + 12 <= length
                    && data[i + 4] == (byte)'E' && data[i + 5] == (byte)'x' && data[i + 6] == (byte)'i' && data[i + 7] == (byte)'f' && data[i + 8] == 0 && data[i + 9] == 0)
                {
                    bool bigEndian = data[i + 10] == (byte)'M' && data[i + 11] == (byte)'M';
                    bool littleEndian = data[i + 10] == (byte)'I' && data[i + 11] == (byte)'I';
                    return BitConverter.IsLittleEndian ? bigEndian : littleEndian;
                }
                i += 2 + segment;
            }
            return false;
        }

        internal void SetExifSwapped(bool swapped) => _exifSwapped = swapped;

        // A copy (Clone) has the properties of its original, in the same order.
        internal void CopyExifOrder(Image original)
        {
            _exifSwapped = original._exifSwapped;
            _propertiesSet = original._propertiesSet == null ? null : new HashSet<int>(original._propertiesSet);
        }

        private void PropertySetHere(int id) => (_propertiesSet ??= new HashSet<int>()).Add(id);

        // The item's value in the machine's order: its numbers (SHORT, LONG, RATIONAL, SLONG, SRATIONAL) turned.
        private PropertyItem InMachineOrder(PropertyItem item)
        {
            if (!_exifSwapped || item.Value == null || (_propertiesSet != null && _propertiesSet.Contains(item.Id)))
                return item;
            int unit = item.Type switch
            {
                3 => 2,
                4 or 5 or 9 or 10 => 4,
                _ => 0,
            };
            if (unit == 0)
                return item;
            byte[] value = item.Value;
            for (int i = 0; i + unit <= value.Length; i += unit)
                Array.Reverse(value, i, unit);
            return item;
        }
    }
}
#endif
