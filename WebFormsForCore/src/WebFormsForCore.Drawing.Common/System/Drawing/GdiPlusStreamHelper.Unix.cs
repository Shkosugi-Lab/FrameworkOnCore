// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// System.Drawing.GdiPlusStreamHelper.cs
//   - Originally in System.Drawing.gdipFunctions.cs
//
// Authors:
//    Alexandre Pigolkine (pigolkine@gmx.de)
//    Jordi Mas i Hernandez (jordi@ximian.com)
//    Sanjay Gupta (gsanjay@novell.com)
//    Ravindra (rkumar@novell.com)
//    Peter Dennis Bartok (pbartok@novell.com)
//    Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2004 - 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.IO;
using System.Runtime.InteropServices;

namespace System.Drawing
{
    internal sealed partial class GdiPlusStreamHelper
    {
        private Stream _stream;

        public unsafe GdiPlusStreamHelper(Stream stream, bool seekToOrigin, bool makeSeekable = true)
        {
            // Seeking required
            if (makeSeekable && !stream.CanSeek)
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                _stream = memoryStream;
            }
            else
            {
                _stream = stream;

                if (seekToOrigin)
                {
                    _stream.Seek(0, SeekOrigin.Begin);
                }
            }

            CloseDelegate = StreamCloseImpl;
            GetBytesDelegate = StreamGetBytesImpl;
            GetHeaderDelegate = StreamGetHeaderImpl;
            PutBytesDelegate = StreamPutBytesImpl;
            SeekDelegate = StreamSeekImpl;
            SizeDelegate = StreamSizeImpl;
        }

        public unsafe int StreamGetHeaderImpl(byte* buf, int bufsz)
        {
            return StreamGetBytesImpl(buf, bufsz, peek: true);
        }

        public unsafe int StreamGetBytesImpl(byte* buf, int bufsz, bool peek)
        {
            if ((buf == null && peek) || !_stream.CanRead)
                return -1;

            if (bufsz <= 0)
                return 0;

            int read = 0;
            long originalPosition = 0;
            if (peek)
            {
                originalPosition = _stream.Position;
            }

            try
            {
                Span<byte> buffer = new Span<byte>(buf, bufsz);
                read = _stream.Read(buffer);
            }
            catch (IOException)
            {
                return -1;
            }

            if (peek)
            {
                // If we are peeking bytes, then go back to original position before peeking
                _stream.Seek(originalPosition, SeekOrigin.Begin);
            }
#if WebFormsForCore
            else if (read == 0 && !_padded && buf != null)
            {
                read = ExifPadding(buf, bufsz);
            }
#endif

            return read;
        }

#if WebFormsForCore
        // libgdiplus (6.1) keeps the first 64 KiB of a JPEG for its EXIF data (dstream_load, then libexif) only when its
        // last read filling them returns bytes: from a stream, a JPEG shorter than that lost its properties
        // (PropertyItems, the orientation; from a file it has them, and GDI+ both). Such a JPEG's reads end in zeros up
        // to 64 KiB, once: the decoder reads nothing after the image's end (EOI), and libexif finds the APP1 segment
        // where it is.
        private const int ExifChunk = 65536;
        private bool _padded;

        private unsafe int ExifPadding(byte* buf, int bufsz)
        {
            _padded = true;
            long length, position;
            try
            {
                length = _stream.Length;
                position = _stream.Position;
                if (length >= ExifChunk || position != length || !StartsAsJpeg())
                    return 0;
            }
            catch (NotSupportedException)
            {
                return 0;
            }
            int padding = (int)Math.Min(bufsz, ExifChunk - length);
            new Span<byte>(buf, padding).Clear();
            return padding;
        }

        private bool StartsAsJpeg()
        {
            long position = _stream.Position;
            Span<byte> start = stackalloc byte[3];
            _stream.Seek(0, SeekOrigin.Begin);
            int read = _stream.Read(start);
            _stream.Seek(position, SeekOrigin.Begin);
            return read == 3 && start[0] == 0xFF && start[1] == 0xD8 && start[2] == 0xFF;
        }
#endif

        public long StreamSeekImpl(int offset, int whence)
        {
            // Make sure we have a valid 'whence'.
            if ((whence < 0) || (whence > 2))
                return -1;

            return _stream.Seek((long)offset, (SeekOrigin)whence);
        }

        public unsafe int StreamPutBytesImpl(byte* buf, int bufsz)
        {
            if (!_stream.CanWrite)
                return -1;

            var buffer = new ReadOnlySpan<byte>(buf, bufsz);
            _stream.Write(buffer);

            return bufsz;
        }

        public void StreamCloseImpl()
        {
            _stream.Dispose();
        }

        public long StreamSizeImpl()
        {
            try
            {
                return _stream.Length;
            }
            catch
            {
                return -1;
            }
        }

#if WebFormsForCore
        // The stream read (the application's, or its copy when that cannot seek).
        internal Stream Stream => _stream;

#endif
        public StreamCloseDelegate CloseDelegate { get; }
        public StreamGetBytesDelegate GetBytesDelegate { get; }
        public StreamGetHeaderDelegate GetHeaderDelegate { get; }
        public StreamPutBytesDelegate PutBytesDelegate { get; }
        public StreamSeekDelegate SeekDelegate { get; }
        public StreamSizeDelegate SizeDelegate { get; }
    }
}
