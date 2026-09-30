using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DrawingParity
{
    /// <summary>
    /// How the cases write a System.Drawing object: what it holds (a path's points, a matrix's elements, a pen's settings,
    /// an image's size and format), not its ToString. Two kinds of line are marked, for Linux where libgdiplus draws
    /// (the port's Unix implementation) and other fonts are installed:
    ///   "~px "   pixels: exact on Windows (the same gdiplus.dll as .NET Framework), within a tolerance on Linux;
    ///   "~font " what depends on the fonts installed: exact on Windows, loosely on Linux.
    /// Everything else is compared exactly everywhere.
    /// </summary>
    public static class Describe
    {
        public const string Pixels = "~px ";
        public const string FontDependent = "~font ";

        /// <summary>Writes what <paramref name="value"/> is, as one line or more.</summary>
        public static void Record(Probe p, string label, object value, bool fontDependent = false)
        {
            var prefix = fontDependent ? FontDependent : "";
            switch (value)
            {
                case Metafile metafile:
                    p.Is(prefix + label, Image(metafile));
                    using (var rendered = new Bitmap(Math.Min(64, Math.Max(1, metafile.Width)), Math.Min(64, Math.Max(1, metafile.Height))))
                    {
                        using (var g = Graphics.FromImage(rendered)) { g.Clear(Color.White); g.DrawImage(metafile, 0, 0, rendered.Width, rendered.Height); }
                        p.Is((fontDependent ? FontDependent : Pixels) + label, PixelText(rendered));
                    }
                    return;
                case Bitmap bitmap:
                    p.Is(prefix + label, Image(bitmap));
                    p.Is((fontDependent ? FontDependent : Pixels) + label, PixelText(bitmap));
                    return;
                case Icon icon:
                    p.Is(prefix + label, "Icon " + icon.Width + "x" + icon.Height);
                    using (var bitmap = icon.ToBitmap()) p.Is(Pixels + label, PixelText(bitmap));
                    return;
                case Font font:
                    p.Is(FontDependent + label, Font(font));
                    return;
                case FontFamily family:
                    p.Is(FontDependent + label, "FontFamily " + family.Name);
                    return;
                case GraphicsPath path:
                    p.Is(prefix + label, Path(path));
                    return;
                case Region region:
                    p.Is(prefix + label, Region(region));
                    return;
                case BitmapData data:
                    p.Is(prefix + label, $"BitmapData {data.Width}x{data.Height} {Probe.EnumName(data.PixelFormat)} stride={data.Stride} scan0={(data.Scan0 == IntPtr.Zero ? "zero" : "set")}");
                    return;
                case Graphics graphics:
                    p.Is(prefix + label, GraphicsText(graphics));
                    return;
                case IntPtr handle:
                    // A handle's value is the system's: whether there is one.
                    p.Is(prefix + label, handle == IntPtr.Zero ? "IntPtr zero" : "IntPtr set");
                    return;
                case string _:
                case null:
                    p.Is(prefix + label, value);
                    return;
                case Array array when array.Length > 0 && (array.GetValue(0) is Image || array.GetValue(0) is FontFamily):
                    p.Is(prefix + label, "[" + array.Length + "]");
                    for (var i = 0; i < Math.Min(array.Length, 8); i++) Record(p, label + "[" + i + "]", array.GetValue(i), fontDependent);
                    return;
                default:
                    p.Is(prefix + label, Text(value));
                    return;
            }
        }

        /// <summary>
        /// The structures whose ToString embeds floats (written by .NET in the shortest round-trip form, by .NET Framework
        /// with 7 digits): their values exactly (G9), for Probe.Format.
        /// </summary>
        public static string Custom(object value)
        {
            string F(float f) => Probe.Exact(f);
            switch (value)
            {
                case PointF point: return $"PointF {{X={F(point.X)}, Y={F(point.Y)}}}";
                case SizeF size: return $"SizeF {{Width={F(size.Width)}, Height={F(size.Height)}}}";
                case RectangleF rect: return $"RectangleF {{X={F(rect.X)},Y={F(rect.Y)},Width={F(rect.Width)},Height={F(rect.Height)}}}";
                default: return null;
            }
        }

        /// <summary>A value as one line (the objects that are settings: pens, brushes, matrices...).</summary>
        public static string Text(object value)
        {
            switch (value)
            {
                case null: return "null";
                case Matrix matrix: return "Matrix " + Probe.Format(matrix.Elements) + " identity=" + Probe.Format(matrix.IsIdentity) + " invertible=" + Probe.Format(matrix.IsInvertible);
                case Pen pen: return $"Pen {Text(pen.Color)} width={Probe.Format(pen.Width)} type={Probe.EnumName(pen.PenType)} dash={Probe.EnumName(pen.DashStyle)} dashCap={Probe.EnumName(pen.DashCap)} start={Probe.EnumName(pen.StartCap)} end={Probe.EnumName(pen.EndCap)} join={Probe.EnumName(pen.LineJoin)} align={Probe.EnumName(pen.Alignment)} miter={Probe.Format(pen.MiterLimit)} offset={Probe.Format(pen.DashOffset)} compound={Probe.Format(pen.CompoundArray)} transform={Text(pen.Transform)}";
                case SolidBrush solid: return "SolidBrush " + Text(solid.Color);
                case HatchBrush hatch: return $"HatchBrush {Probe.EnumName(hatch.HatchStyle)} {Text(hatch.ForegroundColor)} {Text(hatch.BackgroundColor)}";
                case TextureBrush texture: return $"TextureBrush wrap={Probe.EnumName(texture.WrapMode)} transform={Text(texture.Transform)} image={Image(texture.Image)}";
                case LinearGradientBrush linear: return $"LinearGradientBrush {Probe.Format(linear.LinearColors.Select(c => Text(c)))} rect={Probe.Format(linear.Rectangle)} wrap={Probe.EnumName(linear.WrapMode)} gamma={Probe.Format(linear.GammaCorrection)} transform={Text(linear.Transform)} blend={Text(linear.Blend)}";
                case PathGradientBrush gradient: return $"PathGradientBrush center={Text(gradient.CenterColor)} at={Probe.Format(gradient.CenterPoint)} surround={Probe.Format(gradient.SurroundColors.Select(c => Text(c)))} rect={Probe.Format(gradient.Rectangle)} wrap={Probe.EnumName(gradient.WrapMode)} focus={Probe.Format(gradient.FocusScales)}";
                case Color color: return $"Color #{color.ToArgb():X8} {color}";
                case StringFormat format: return $"StringFormat align={Probe.EnumName(format.Alignment)} line={Probe.EnumName(format.LineAlignment)} flags={Probe.EnumName(format.FormatFlags)} trimming={Probe.EnumName(format.Trimming)} hotkey={Probe.EnumName(format.HotkeyPrefix)} digits={Probe.EnumName(format.DigitSubstitutionMethod)}/{format.DigitSubstitutionLanguage}";
                // A brush's blend of one factor: GDI+ does not set its position (what is in the memory; on .NET Framework too).
                case Blend blend: return "Blend factors=" + Probe.Format(blend.Factors) + " positions=" + (blend.Factors.Length == 1 ? "(unset by GDI+)" : Probe.Format(blend.Positions));
                case ColorBlend blend: return "ColorBlend colors=" + Probe.Format(blend.Colors.Select(c => Text(c))) + " positions=" + Probe.Format(blend.Positions);
                case ColorMatrix matrix: return "ColorMatrix " + Probe.Format(Enumerable.Range(0, 5).Select(i => Enumerable.Range(0, 5).Select(j => matrix[i, j])));
                case ColorPalette palette: return "ColorPalette " + (palette.Entries.Length == 0 ? "empty (an empty palette's flags: .NET Framework left them unset)" : "flags=" + palette.Flags + " " + Probe.Format(palette.Entries.Select(c => Text(c))));
                case PropertyItem item: return $"PropertyItem 0x{item.Id:X4} type={item.Type} len={item.Len} {Probe.Format(item.Value)}";
                case ImageCodecInfo codec: return $"ImageCodecInfo {codec.FormatDescription} {codec.MimeType} {codec.FilenameExtension} format={codec.FormatID} flags={Probe.EnumName(codec.Flags)}";
                case EncoderParameter parameter: return $"EncoderParameter {parameter.Encoder.Guid} {Probe.EnumName(parameter.ValueType)} n={parameter.NumberOfValues}";
                case EncoderParameters parameters: return "EncoderParameters " + Probe.Format(parameters.Param.Select(c => Text(c)));
                case System.Drawing.Imaging.Encoder encoder: return "Encoder " + encoder.Guid;
                case ImageFormat format: return "ImageFormat " + format.Guid + " " + format;
                case FrameDimension dimension: return "FrameDimension " + dimension.Guid + " " + dimension;
                case PathData data: return "PathData " + Probe.Format(data.Points) + " " + Probe.Format(data.Types);
                case Margins margins: return "Margins " + margins;
                case PaperSize size: return $"PaperSize {size.PaperName} {Probe.EnumName(size.Kind)} {size.Width}x{size.Height} raw={size.RawKind}";
                case PrinterResolution resolution: return $"PrinterResolution {Probe.EnumName(resolution.Kind)} {resolution.X}x{resolution.Y}";
                case GraphicsState _: return "GraphicsState";
                case GraphicsContainer _: return "GraphicsContainer";
                case Image image: return Image(image);
                case float f: return Probe.Format(f);
                case double d: return Probe.Format(d);
                case IEnumerable sequence when !(value is string): return "[" + string.Join(", ", sequence.Cast<object>().Take(64).Select(c => Text(c))) + "]";
                default:
                    if (value.GetType().IsValueType || value is Exception) return Probe.Format(value);
                    // A reference object without value semantics: its type (its ToString may hold a hash).
                    var text = value.ToString();
                    return value.GetType().FullName + (text != null && text != value.GetType().FullName && text != value.GetType().ToString() ? " " + Probe.Normalize(text) : "");
            }
        }

        public static string Image(Image image) =>
            $"{image.GetType().Name} {image.Width}x{image.Height} {Probe.EnumName(image.PixelFormat)} raw={RawFormat(image)} flags={image.Flags} dpi={Probe.Format(image.HorizontalResolution)}x{Probe.Format(image.VerticalResolution)}";

        static string RawFormat(Image image)
        {
            var guid = image.RawFormat.Guid;
            foreach (var known in new[] { ImageFormat.MemoryBmp, ImageFormat.Bmp, ImageFormat.Emf, ImageFormat.Wmf, ImageFormat.Gif, ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Tiff, ImageFormat.Exif, ImageFormat.Icon })
                if (known.Guid == guid) return known.ToString();
            return guid.ToString();
        }

        static string Font(Font font) =>
            $"Font {font.Name} {Probe.Format(font.Size)} {Probe.EnumName(font.Style)} {Probe.EnumName(font.Unit)} charset={font.GdiCharSet} vertical={Probe.Format(font.GdiVerticalFont)} height={font.Height} family={font.FontFamily.Name}";

        static string Path(GraphicsPath path)
        {
            if (path.PointCount == 0) return "GraphicsPath empty fill=" + Probe.EnumName(path.FillMode);
            return $"GraphicsPath n={path.PointCount} fill={Probe.EnumName(path.FillMode)} points={Probe.Format(path.PathPoints)} types={Probe.Format(path.PathTypes)}";
        }

        static string Region(Region region)
        {
            using (var bitmap = new Bitmap(1, 1))
            using (var g = System.Drawing.Graphics.FromImage(bitmap))
            using (var identity = new Matrix())
                return $"Region empty={Probe.Format(region.IsEmpty(g))} infinite={Probe.Format(region.IsInfinite(g))} bounds={Probe.Format(region.GetBounds(g))} scans={Probe.Format(region.GetRegionScans(identity).Take(16))}";
        }

        static string GraphicsText(Graphics g) =>
            $"Graphics unit={Probe.EnumName(g.PageUnit)} scale={Probe.Format(g.PageScale)} smoothing={Probe.EnumName(g.SmoothingMode)} text={Probe.EnumName(g.TextRenderingHint)} interpolation={Probe.EnumName(g.InterpolationMode)} compositing={Probe.EnumName(g.CompositingMode)}/{Probe.EnumName(g.CompositingQuality)} pixelOffset={Probe.EnumName(g.PixelOffsetMode)} clip={Probe.Format(g.ClipBounds)} transform={Text(g.Transform)}";

        /// <summary>
        /// An image's pixels: its size, a hash of every pixel (Windows compares it), and 8x8 cells of averaged colour (Linux
        /// compares those, within a tolerance: its rendering is libgdiplus').
        /// </summary>
        public static string PixelText(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            var argb = new int[width * height];
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    argb[y * width + x] = bitmap.GetPixel(x, y).ToArgb();
            var bytes = new byte[argb.Length * 4];
            Buffer.BlockCopy(argb, 0, bytes, 0, bytes.Length);
            string hash;
            using (var sha = SHA256.Create()) hash = BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").Substring(0, 16).ToLowerInvariant();
            var grid = new StringBuilder();
            for (var cy = 0; cy < 8; cy++)
                for (var cx = 0; cx < 8; cx++)
                {
                    int x0 = cx * width / 8, x1 = Math.Max(x0 + 1, (cx + 1) * width / 8), y0 = cy * height / 8, y1 = Math.Max(y0 + 1, (cy + 1) * height / 8);
                    long a = 0, r = 0, g = 0, b = 0, n = 0;
                    for (var y = y0; y < Math.Min(y1, height); y++)
                        for (var x = x0; x < Math.Min(x1, width); x++)
                        {
                            var c = argb[y * width + x];
                            a += (c >> 24) & 255; r += (c >> 16) & 255; g += (c >> 8) & 255; b += c & 255; n++;
                        }
                    if (n == 0) n = 1;
                    grid.Append(((a / n) & 255).ToString("x2")).Append(((r / n) & 255).ToString("x2")).Append(((g / n) & 255).ToString("x2")).Append(((b / n) & 255).ToString("x2"));
                }
            return $"{width}x{height} sha:{hash} grid:{grid}";
        }
    }
}
