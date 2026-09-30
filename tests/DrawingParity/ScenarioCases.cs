using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DrawingParity
{
    /// <summary>
    /// What web applications do with System.Drawing, end to end (the API cases call one member at a time, with sample
    /// arguments): a thumbnail, a JPEG at a quality, a photo turned by its EXIF orientation, a GIF's frames, a TIFF's
    /// pages, a CAPTCHA-like text picture, a chart-like drawing, a PNG kept transparent, an indexed picture's palette.
    /// And the members ApiCases does not call: EncoderParameter over the caller's memory, CopyFromScreen's arguments.
    /// </summary>
    public static class ScenarioCases
    {
        /// <summary>A picture saved and read back: its format, size, and pixels (the encoder's output).</summary>
        static void Saved(Probe p, string label, Image image, ImageFormat format, bool fontDependent = false)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, format);
                stream.Position = 0;
                using (var read = new Bitmap(stream)) Describe.Record(p, label, read, fontDependent);
            }
        }

        static ImageCodecInfo Codec(ImageFormat format) => ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == format.Guid);

        [Case]
        public static void Thumbnail(Probe p)
        {
            using (var samples = new Samples())
            using (var photo = new Bitmap(samples.Stream("exif.jpg")))
            {
                Describe.Record(p, "photo", photo);
                using (var thumbnail = new Bitmap(24, 16))
                {
                    using (var g = Graphics.FromImage(thumbnail))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.DrawImage(photo, new Rectangle(0, 0, 24, 16));
                    }
                    Describe.Record(p, "thumbnail", thumbnail);
                    Saved(p, "thumbnail as PNG", thumbnail, ImageFormat.Png);
                }
                // GetThumbnailImage: the photo's embedded thumbnail or a scaled copy.
                using (var small = photo.GetThumbnailImage(12, 8, () => false, IntPtr.Zero)) Describe.Record(p, "GetThumbnailImage", small);
            }
        }

        [Case]
        public static void Jpeg_quality(Probe p)
        {
            using (var samples = new Samples())
            using (var picture = new Bitmap(samples.Stream("alpha.png")))
            {
                foreach (var quality in new[] { 10L, 50L, 90L })
                    using (var parameters = new EncoderParameters(1))
                    {
                        parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                        using (var stream = new MemoryStream())
                        {
                            picture.Save(stream, Codec(ImageFormat.Jpeg), parameters);
                            p.Is("quality " + quality + " size", stream.Length > 0);
                            stream.Position = 0;
                            using (var read = new Bitmap(stream)) Describe.Record(p, "quality " + quality, read);
                        }
                    }
                p.Try("JPEG encoder's parameters", () => picture.GetEncoderParameterList(ImageFormat.Jpeg.Guid).Param.Select(x => x.Encoder.Guid + " " + x.NumberOfValues + " " + x.ValueType).ToList());
            }
        }

        [Case]
        public static void Exif_orientation(Probe p)
        {
            using (var samples = new Samples())
            using (var photo = new Bitmap(samples.Stream("exif.jpg")))
            {
                p.Is("property ids", photo.PropertyIdList.OrderBy(id => id).ToList());
                const int orientation = 0x0112;
                var item = photo.PropertyItems.FirstOrDefault(x => x.Id == orientation);
                p.Is("orientation", item == null ? "none" : item.Type + " " + BitConverter.ToString(item.Value));
                var turn = item == null ? RotateFlipType.RotateNoneFlipNone : item.Value[0] switch
                {
                    3 => RotateFlipType.Rotate180FlipNone,
                    6 => RotateFlipType.Rotate90FlipNone,
                    8 => RotateFlipType.Rotate270FlipNone,
                    _ => RotateFlipType.RotateNoneFlipNone,
                };
                photo.RotateFlip(turn);
                if (item != null) photo.RemovePropertyItem(orientation);
                Describe.Record(p, "turned", photo);
                p.Is("property ids after", photo.PropertyIdList.OrderBy(id => id).ToList());
                Saved(p, "turned as JPEG", photo, ImageFormat.Jpeg);
            }
        }

        [Case]
        public static void Gif_frames(Probe p)
        {
            using (var samples = new Samples())
            using (var gif = Image.FromStream(samples.Stream("anim.gif")))
            {
                var dimension = new FrameDimension(gif.FrameDimensionsList[0]);
                var frames = gif.GetFrameCount(dimension);
                p.Is("frames", frames);
                p.Is("can animate", ImageAnimator.CanAnimate(gif));
                var delays = gif.PropertyItems.FirstOrDefault(x => x.Id == 0x5100);
                p.Is("delays", delays == null ? "none" : BitConverter.ToString(delays.Value));
                for (var i = 0; i < frames; i++)
                {
                    gif.SelectActiveFrame(dimension, i);
                    using (var frame = new Bitmap(gif)) Describe.Record(p, "frame " + i, frame);
                }
            }
        }

        [Case]
        public static void Tiff_pages(Probe p)
        {
            using (var samples = new Samples())
            using (var tiff = Image.FromStream(samples.Stream("pages.tif")))
            {
                var pages = tiff.GetFrameCount(FrameDimension.Page);
                p.Is("pages", pages);
                for (var i = 0; i < pages; i++)
                {
                    tiff.SelectActiveFrame(FrameDimension.Page, i);
                    using (var page = new Bitmap(tiff)) Describe.Record(p, "page " + i, page);
                }

                // A multipage TIFF written: SaveAdd with the multi-frame encoder parameter.
                using (var first = new Bitmap(8, 8))
                using (var second = new Bitmap(8, 8))
                using (var stream = new MemoryStream())
                using (var parameters = new EncoderParameters(1))
                {
                    using (var g = Graphics.FromImage(second)) g.Clear(Color.Teal);
                    parameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                    first.Save(stream, Codec(ImageFormat.Tiff), parameters);
                    parameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
                    first.SaveAdd(second, parameters);
                    parameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);
                    first.SaveAdd(parameters);
                    stream.Position = 0;
                    using (var read = Image.FromStream(stream)) p.Is("written pages", read.GetFrameCount(FrameDimension.Page));
                }
            }
        }

        [Case]
        public static void Captcha_text(Probe p)
        {
            using (var picture = new Bitmap(80, 24))
            using (var g = Graphics.FromImage(picture))
            using (var font = new Font("Arial", 12f, FontStyle.Bold))
            using (var brush = new LinearGradientBrush(new Rectangle(0, 0, 80, 24), Color.DarkBlue, Color.DarkRed, 30f))
            using (var noise = new HatchBrush(HatchStyle.Percent10, Color.LightGray, Color.White))
            {
                g.FillRectangle(noise, 0, 0, 80, 24);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var size = g.MeasureString("A3x9", font);
                Describe.Record(p, "measured", size, fontDependent: true);
                using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString("A3x9", font, brush, new RectangleF(0, 0, 80, 24), format);
                using (var pen = new Pen(Color.Gray, 1f)) g.DrawBezier(pen, 0, 20, 20, 0, 60, 24, 80, 4);
                Describe.Record(p, "picture", picture, fontDependent: true);
                Saved(p, "as GIF", picture, ImageFormat.Gif, fontDependent: true);
            }
        }

        [Case]
        public static void Chart_drawing(Probe p)
        {
            var values = new[] { 30, 55, 15, 80, 45 };
            using (var chart = new Bitmap(64, 48))
            using (var g = Graphics.FromImage(chart))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var axis = new Pen(Color.Black, 1f))
                {
                    g.DrawLine(axis, 4, 44, 60, 44);
                    g.DrawLine(axis, 4, 4, 4, 44);
                }
                for (var i = 0; i < values.Length; i++)
                    using (var bar = new SolidBrush(Color.FromArgb(255, 40 * i, 120, 200 - 30 * i)))
                        g.FillRectangle(bar, 8 + i * 10, 44 - values[i] / 2, 8, values[i] / 2);
                using (var line = new Pen(Color.OrangeRed, 2f) { LineJoin = LineJoin.Round, DashStyle = DashStyle.Dash })
                    g.DrawLines(line, values.Select((v, i) => new PointF(12 + i * 10, 44 - v / 2f)).ToArray());
                g.FillPie(Brushes.SteelBlue, new Rectangle(40, 4, 20, 20), 0, 250);
                Describe.Record(p, "chart", chart);
                Saved(p, "chart as PNG", chart, ImageFormat.Png);
            }
        }

        [Case]
        public static void Png_transparency(Probe p)
        {
            using (var samples = new Samples())
            using (var png = new Bitmap(samples.Stream("alpha.png")))
            {
                Describe.Record(p, "read", png);
                p.Is("corner", png.GetPixel(0, 0));
                using (var copy = new Bitmap(png.Width, png.Height, PixelFormat.Format32bppArgb))
                {
                    using (var g = Graphics.FromImage(copy))
                    {
                        g.Clear(Color.Transparent);
                        g.CompositingMode = CompositingMode.SourceOver;
                        g.DrawImage(png, 0, 0, png.Width, png.Height);
                    }
                    copy.MakeTransparent(Color.White);
                    Describe.Record(p, "made transparent", copy);
                    Saved(p, "as PNG", copy, ImageFormat.Png);
                }
            }
        }

        [Case]
        public static void Indexed_palette(Probe p)
        {
            using (var samples = new Samples())
            using (var bmp = new Bitmap(samples.Stream("indexed.bmp")))
            {
                Describe.Record(p, "read", bmp);
                p.Is("palette", bmp.Palette.Entries.Take(8).ToList());
                var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                try
                {
                    var bytes = new byte[Math.Abs(data.Stride) * data.Height];
                    Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                    p.Is("stride", data.Stride);
                    p.Is("indexes", bytes.Take(16).ToArray());
                }
                finally { bmp.UnlockBits(data); }
                Saved(p, "as GIF", bmp, ImageFormat.Gif);
            }
        }

        /// <summary>
        /// A picture read from the application's stream leaves it open (GDI+ reads it later, as it needs: the stream must
        /// live as long as the picture), and a picture saved to it too.
        /// </summary>
        [Case]
        public static void Streams_left_open(Probe p)
        {
            foreach (var name in new[] { "alpha.png", "exif.jpg", "anim.gif", "pages.tif", "indexed.bmp", "icon.ico", "picture.emf" })
            {
                var stream = new MemoryStream(Samples.Resource(name));
                try
                {
                    using (var image = name.EndsWith(".ico", StringComparison.Ordinal) ? new Icon(stream).ToBitmap() : Image.FromStream(stream))
                    {
                        p.Is(name + " open after reading", stream.CanRead);
                        using (var copy = new MemoryStream())
                        {
                            image.Save(copy, ImageFormat.Png);
                            p.Is(name + " saved, open", copy.CanWrite);
                        }
                    }
                    p.Is(name + " open after disposing", stream.CanRead);
                }
                catch (Exception e) { p.Is(name + " threw", Probe.Exception(e)); }
            }
        }

        /// <summary>EncoderParameter over the caller's memory (ApiCases passes none: undefined).</summary>
        [Case]
        public static void EncoderParameter_over_memory(Probe p)
        {
            var values = new long[] { 40, 70 };
            var handle = GCHandle.Alloc(values, GCHandleType.Pinned);
            try
            {
                using (var parameter = new EncoderParameter(Encoder.Quality, 2, EncoderParameterValueType.ValueTypeLong, handle.AddrOfPinnedObject()))
                    p.Is("typed", parameter.NumberOfValues + " " + parameter.ValueType + " " + parameter.Encoder.Guid);

            }
            catch (Exception e) { p.Is("threw", Probe.Exception(e)); }
            finally { handle.Free(); }
        }

        /// <summary>CopyFromScreen's argument checks (what it copies is the screen's: not compared).</summary>
        [Case]
        public static void CopyFromScreen_arguments(Probe p)
        {
            using (var picture = new Bitmap(8, 8))
            using (var g = Graphics.FromImage(picture))
            {
                p.Does("invalid operation", () => g.CopyFromScreen(0, 0, 0, 0, new Size(4, 4), (CopyPixelOperation)0x12345));
                p.Does("empty size", () => g.CopyFromScreen(Point.Empty, Point.Empty, Size.Empty));
            }
        }
    }
}
