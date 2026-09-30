# Makes the sample files the System.Drawing parity cases read (tests/DrawingParity/Samples, committed): made once, on
# Windows with .NET Framework's System.Drawing (Windows PowerShell 5.1), deterministic, ours (no third-party images).
#   icon.ico       16x16 and 32x32, 32 bpp (written byte by byte: Icon.Save of a HICON writes 4 bpp)
#   picture.emf    an EMF+ dual metafile: a rectangle, an ellipse, text
#   exif.jpg       24x16 JPEG with EXIF: Orientation 6 (rotate 90), Make, DateTime
#   anim.gif       3 frames, 100 ms, loop (written byte by byte: GDI+ does not write animated GIF)
#   pages.tif      2 pages (SaveAdd)
#   alpha.png      32x32 with transparency
#   indexed.bmp    8 bpp, a 16-colour palette
$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSEdition -eq 'Core') { throw 'Run under Windows PowerShell 5.1 (.NET Framework System.Drawing).' }
Add-Type -AssemblyName System.Drawing
$out = Join-Path $PSScriptRoot 'Samples'
New-Item -ItemType Directory $out -Force | Out-Null

Add-Type -ReferencedAssemblies System.Drawing -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

public static class FocSamples
{
    static Bitmap Pattern(int w, int h)
    {
        var b = new Bitmap(w, h, PixelFormat.Format32bppArgb);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                b.SetPixel(x, y, Color.FromArgb(255, (x * 255) / Math.Max(1, w - 1), (y * 255) / Math.Max(1, h - 1), ((x + y) % 2) * 200));
        return b;
    }

    // ICO: ICONDIR, one ICONDIRENTRY per size, each image a 32 bpp DIB (BITMAPINFOHEADER with double height, pixels
    // bottom-up BGRA, then the AND mask).
    public static void Icon(string path)
    {
        int[] sizes = { 16, 32 };
        var images = new List<byte[]>();
        foreach (int s in sizes)
        {
            var ms = new MemoryStream();
            var w = new BinaryWriter(ms);
            w.Write(40); w.Write(s); w.Write(s * 2); w.Write((short)1); w.Write((short)32); w.Write(0); w.Write(0); w.Write(0); w.Write(0); w.Write(0); w.Write(0);
            for (int y = s - 1; y >= 0; y--)
                for (int x = 0; x < s; x++)
                {
                    bool inside = (x - s / 2) * (x - s / 2) + (y - s / 2) * (y - s / 2) < (s / 2) * (s / 2);
                    w.Write((byte)(inside ? 200 : 0)); w.Write((byte)(inside ? 80 : 0)); w.Write((byte)(inside ? 20 : 0)); w.Write((byte)(inside ? 255 : 0));
                }
            int maskRow = ((s + 31) / 32) * 4;
            for (int i = 0; i < maskRow * s; i++) w.Write((byte)0);
            images.Add(ms.ToArray());
        }
        using (var f = File.Create(path))
        {
            var w = new BinaryWriter(f);
            w.Write((short)0); w.Write((short)1); w.Write((short)sizes.Length);
            int offset = 6 + 16 * sizes.Length;
            for (int i = 0; i < sizes.Length; i++)
            {
                w.Write((byte)sizes[i]); w.Write((byte)sizes[i]); w.Write((byte)0); w.Write((byte)0);
                w.Write((short)1); w.Write((short)32); w.Write(images[i].Length); w.Write(offset);
                offset += images[i].Length;
            }
            foreach (var image in images) w.Write(image);
        }
    }

    public static void Metafile(string path)
    {
        using (var reference = new Bitmap(1, 1))
        using (var rg = Graphics.FromImage(reference))
        {
            IntPtr hdc = rg.GetHdc();
            using (var f = File.Create(path))
            {
                using (var mf = new System.Drawing.Imaging.Metafile(f, hdc, new RectangleF(0, 0, 32, 24), MetafileFrameUnit.Pixel, EmfType.EmfPlusDual))
                using (var g = Graphics.FromImage(mf))
                {
                    g.FillRectangle(Brushes.SteelBlue, 2, 2, 20, 12);
                    g.DrawEllipse(Pens.DarkRed, 8, 6, 20, 14);
                    g.DrawLine(Pens.Black, 0, 23, 31, 0);
                }
            }
            rg.ReleaseHdc(hdc);
        }
    }

    static PropertyItem Item(int id, short type, byte[] value)
    {
        var item = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
        item.Id = id; item.Type = type; item.Len = value.Length; item.Value = value;
        return item;
    }

    public static void Exif(string path)
    {
        using (var b = Pattern(24, 16))
        {
            b.SetPropertyItem(Item(0x0112, 3, BitConverter.GetBytes((short)6)));
            b.SetPropertyItem(Item(0x010F, 2, Encoding.ASCII.GetBytes("FrameworkOnCore\0")));
            b.SetPropertyItem(Item(0x0132, 2, Encoding.ASCII.GetBytes("2020:01:15 10:30:00\0")));
            var jpeg = Array.Find(ImageCodecInfo.GetImageEncoders(), c => c.FormatID == ImageFormat.Jpeg.Guid);
            var parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
            b.Save(path, jpeg, parameters);
        }
    }

    // GIF89a: global colour table (4 colours), the looping extension, 3 frames of 8x8 with a graphic control
    // extension (100 ms) each, image data LZW-coded with a clear code before every pixel (valid, uncompressed).
    public static void AnimatedGif(string path)
    {
        using (var f = File.Create(path))
        {
            var w = new BinaryWriter(f);
            w.Write(Encoding.ASCII.GetBytes("GIF89a"));
            w.Write((short)8); w.Write((short)8); w.Write((byte)0xF1); w.Write((byte)0); w.Write((byte)0);
            byte[] palette = { 255, 255, 255, 220, 40, 40, 40, 160, 40, 40, 40, 220 };
            w.Write(palette);
            w.Write(new byte[] { 0x21, 0xFF, 0x0B }); w.Write(Encoding.ASCII.GetBytes("NETSCAPE2.0")); w.Write(new byte[] { 3, 1, 0, 0, 0 });
            for (int frame = 0; frame < 3; frame++)
            {
                w.Write(new byte[] { 0x21, 0xF9, 4, 0, 10, 0, 0, 0 });
                w.Write((byte)0x2C); w.Write((short)0); w.Write((short)0); w.Write((short)8); w.Write((short)8); w.Write((byte)0);
                const int minCode = 2;
                w.Write((byte)minCode);
                var bits = new List<bool>();
                Action<int, int> put = (code, size) => { for (int i = 0; i < size; i++) bits.Add(((code >> i) & 1) == 1); };
                for (int p = 0; p < 64; p++)
                {
                    int x = p % 8, y = p / 8;
                    int colour = (x / 2 + frame) % 4 == (y / 2) % 4 ? 1 + frame : 0;
                    put(4, 3);          // clear
                    put(colour, 3);
                }
                put(5, 3);              // end of information
                var data = new List<byte>();
                for (int i = 0; i < bits.Count; i += 8)
                {
                    int b = 0;
                    for (int j = 0; j < 8 && i + j < bits.Count; j++) if (bits[i + j]) b |= 1 << j;
                    data.Add((byte)b);
                }
                for (int i = 0; i < data.Count; i += 255)
                {
                    int n = Math.Min(255, data.Count - i);
                    w.Write((byte)n); w.Write(data.GetRange(i, n).ToArray());
                }
                w.Write((byte)0);
            }
            w.Write((byte)0x3B);
        }
    }

    public static void Tiff(string path)
    {
        var tiff = Array.Find(ImageCodecInfo.GetImageEncoders(), c => c.FormatID == ImageFormat.Tiff.Guid);
        using (var first = Pattern(16, 12))
        using (var second = Pattern(12, 16))
        {
            var multi = new EncoderParameters(1);
            multi.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
            first.Save(path, tiff, multi);
            var next = new EncoderParameters(1);
            next.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
            first.SaveAdd(second, next);
            var flush = new EncoderParameters(1);
            flush.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.Flush);
            first.SaveAdd(flush);
        }
    }

    public static void AlphaPng(string path)
    {
        using (var b = new Bitmap(32, 32, PixelFormat.Format32bppArgb))
        {
            for (int y = 0; y < 32; y++)
                for (int x = 0; x < 32; x++)
                    b.SetPixel(x, y, Color.FromArgb((x * 8) & 255, 30, 120, 220));
            b.Save(path, ImageFormat.Png);
        }
    }

    public static void IndexedBmp(string path)
    {
        using (var b = new Bitmap(16, 16, PixelFormat.Format8bppIndexed))
        {
            var palette = b.Palette;
            for (int i = 0; i < 16; i++) palette.Entries[i] = Color.FromArgb(255, i * 16, 255 - i * 16, (i * 37) & 255);
            b.Palette = palette;
            var data = b.LockBits(new Rectangle(0, 0, 16, 16), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            var row = new byte[16];
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++) row[x] = (byte)((x + y) % 16);
                System.Runtime.InteropServices.Marshal.Copy(row, 0, data.Scan0 + y * data.Stride, 16);
            }
            b.UnlockBits(data);
            b.Save(path, ImageFormat.Bmp);
        }
    }
}
'@

[FocSamples]::Icon((Join-Path $out 'icon.ico'))
[FocSamples]::Metafile((Join-Path $out 'picture.emf'))
[FocSamples]::Exif((Join-Path $out 'exif.jpg'))
[FocSamples]::AnimatedGif((Join-Path $out 'anim.gif'))
[FocSamples]::Tiff((Join-Path $out 'pages.tif'))
[FocSamples]::AlphaPng((Join-Path $out 'alpha.png'))
[FocSamples]::IndexedBmp((Join-Path $out 'indexed.bmp'))
foreach ($f in Get-ChildItem $out) {
    $image = [System.Drawing.Image]::FromFile($f.FullName)
    $frames = try { $image.GetFrameCount([System.Drawing.Imaging.FrameDimension]::Time) } catch { try { $image.GetFrameCount([System.Drawing.Imaging.FrameDimension]::Page) } catch { 1 } }
    "{0,-12} {1,6} bytes  {2}x{3} {4} frames={5} props={6}" -f $f.Name, $f.Length, $image.Width, $image.Height, $image.PixelFormat, $frames, $image.PropertyIdList.Count
    $image.Dispose()
}
