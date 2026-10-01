using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DrawingParity
{
    /// <summary>
    /// The values the API cases call with: one sample argument per parameter type (a few by parameter name, where a
    /// member needs one that works: sizes, indexes, angles), one fresh instance per type to call members on. The same
    /// values on .NET Framework and on the port, so what differs is the implementation. Everything made here is disposed
    /// by the scope that made it (Samples.Scope).
    /// </summary>
    public sealed class Samples : ApiSamples
    {
        readonly List<IDisposable> made = new List<IDisposable>();
        readonly List<string> files = new List<string>();

        public override void Dispose()
        {
            for (var i = made.Count - 1; i >= 0; i--) { try { made[i].Dispose(); } catch { } }
            foreach (var file in files) { try { System.IO.File.Delete(file); } catch { } }
        }

        T Keep<T>(T value)
        {
            if (value is IDisposable disposable) made.Add(disposable);
            return value;
        }

        /// <summary>The canvas a Graphics draws on (its pixels are what a drawing member did).</summary>
        public Bitmap Canvas { get; private set; }

        public static byte[] Resource(string name)
        {
            using (var stream = typeof(Samples).Assembly.GetManifestResourceStream(name))
            using (var copy = new MemoryStream())
            {
                if (stream == null) throw new FileNotFoundException(name);
                stream.CopyTo(copy);
                return copy.ToArray();
            }
        }

        /// <summary>A writable stream with the resource's bytes, at its start.</summary>
        public MemoryStream Stream(string name)
        {
            var stream = Keep(new MemoryStream());
            var bytes = Resource(name);
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return stream;
        }

        /// <summary>A file with the resource's bytes (deleted with the scope).</summary>
        public string ResourceFile(string name)
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "foc-drawing-" + Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(name));
            System.IO.File.WriteAllBytes(path, Resource(name));
            files.Add(path);
            return path;
        }

        /// <summary>A file name to write to (deleted with the scope).</summary>
        public string OutputFile(string extension)
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "foc-drawing-" + Guid.NewGuid().ToString("N") + extension);
            files.Add(path);
            return path;
        }

        /// <summary>A 16x16 picture: a gradient, a checker, an opaque and a transparent corner.</summary>
        public Bitmap Picture(int width = 16, int height = 16, PixelFormat format = PixelFormat.Format32bppArgb)
        {
            var bitmap = Keep(new Bitmap(width, height, format));
            if ((format & PixelFormat.Indexed) != 0) return bitmap;
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    bitmap.SetPixel(x, y, Color.FromArgb(x < 3 && y < 3 ? 0 : 255, (x * 255) / Math.Max(1, width - 1), (y * 255) / Math.Max(1, height - 1), ((x + y) % 2) * 200));
            return bitmap;
        }

        /// <summary>A Graphics over a fresh white 32x32 canvas (Canvas).</summary>
        public Graphics NewGraphics()
        {
            Canvas = Keep(new Bitmap(32, 32, PixelFormat.Format32bppArgb));
            var graphics = Keep(System.Drawing.Graphics.FromImage(Canvas));
            graphics.Clear(Color.White);
            return graphics;
        }

        public Font Font() => Keep(new Font("Arial", 10f));
        public GraphicsPath SamplePath() { var path = Keep(new GraphicsPath()); path.AddRectangle(new Rectangle(2, 2, 12, 8)); path.AddEllipse(8, 6, 16, 12); return path; }
        public Icon Icon() => Keep(new Icon(Stream("icon.ico")));
        public Metafile Metafile() => Keep(new Metafile(Stream("picture.emf")));

        public static readonly Point[] Points = { new Point(2, 2), new Point(28, 6), new Point(14, 26) };
        public static readonly PointF[] PointFs = { new PointF(2.5f, 2f), new PointF(28f, 6.5f), new PointF(14f, 26f) };
        public static readonly Rectangle Rect = new Rectangle(4, 5, 20, 12);
        public static readonly RectangleF RectF = new RectangleF(4.5f, 5f, 20f, 12.5f);
        public static readonly Color Color1 = Color.FromArgb(255, 51, 102, 204);
        public static readonly Color Color2 = Color.FromArgb(200, 220, 60, 20);

        /// <summary>A sample argument for a parameter of <paramref name="member"/> (its declaring type decides some).</summary>
        public override object Argument(ParameterInfo parameter, MemberInfo member)
        {
            var type = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
            var name = (parameter.Name ?? "").ToLowerInvariant();
            var owner = member.DeclaringType;
            if (parameter.IsOut) return type.IsValueType ? Activator.CreateInstance(type) : null;
            // A LOGFONT (Font.ToLogFont / FromLogFont take one as object): the structure GDI has.
            if (name == "logfont" || name == "lf") return new LogFont { lfHeight = -13, lfWeight = 400, lfCharSet = 1, lfFaceName = "Arial" };
            if (typeof(TypeConverter).IsAssignableFrom(owner))
            {
                if (type == typeof(object)) return member.Name.StartsWith("ConvertFrom", StringComparison.Ordinal) ? Converted(owner, text: true) : Converted(owner, text: false);
                if (type == typeof(Type)) return typeof(string);
                if (type == typeof(System.Collections.IDictionary)) return ConvertedProperties(owner);
            }

            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(float) || type == typeof(double) || type == typeof(byte) || type == typeof(uint))
                return Convert.ChangeType(Number(name, owner), type, CultureInfo.InvariantCulture);
            if (type == typeof(bool)) return true;
            if (type == typeof(char)) return 'A';
            if (type == typeof(string)) return Text(name, member);
            if (type == typeof(object)) return "Abc";
            if (type == typeof(IntPtr)) return IntPtr.Zero;
            if (type == typeof(Guid)) return ImageFormat.Png.Guid;
            if (type == typeof(Type)) return typeof(Bitmap);
            if (type == typeof(CultureInfo)) return CultureInfo.InvariantCulture;
            if (type == typeof(Stream)) return owner == typeof(Icon) ? Stream("icon.ico") : owner == typeof(Metafile) ? Stream("picture.emf") : (Stream)Stream("alpha.png");
            if (type.IsEnum) return EnumValue(type, name);
            // A warp's destination: four points (the perspective warp reads four, whatever the count; three would have
            // GDI+ read past the array).
            if (name == "destpoints" && owner == typeof(GraphicsPath)) return new[] { new PointF(2.5f, 2f), new PointF(28f, 6.5f), new PointF(4f, 24f), new PointF(30f, 26f) };
            if (type.IsArray) return Array(type.GetElementType(), name);
            if (typeof(Delegate).IsAssignableFrom(type)) return Callback(type);
            return Instance(type, name);
        }

        /// <summary>What a converter converts (a value of its type), or its text (what ConvertFrom reads).</summary>
        object Converted(Type converter, bool text)
        {
            switch (converter.Name)
            {
                case "PointConverter": return text ? "4, 5" : (object)new Point(4, 5);
                case "SizeConverter": return text ? "20, 12" : (object)new Size(20, 12);
                case "SizeFConverter": return text ? "20.5, 12" : (object)new SizeF(20.5f, 12f);
                case "RectangleConverter": return text ? "4, 5, 20, 12" : (object)Rect;
                case "ColorConverter": return text ? "SteelBlue" : (object)Color.SteelBlue;
                case "FontConverter": return text ? "Arial, 10pt, style=Bold" : (object)Font();
                case "FontNameConverter": return "Arial";
                case "FontUnitConverter": return text ? "Point" : (object)GraphicsUnit.Point;
                case "ImageConverter": return text ? (object)Resource("alpha.png") : Picture();
                case "IconConverter": return text ? (object)Resource("icon.ico") : Icon();
                case "ImageFormatConverter": return text ? "Png" : (object)ImageFormat.Png;
                case "MarginsConverter": return text ? "10, 20, 30, 40" : (object)new System.Drawing.Printing.Margins(10, 20, 30, 40);
                default: return "Abc";
            }
        }

        /// <summary>The property values CreateInstance makes a converter's value from.</summary>
        static System.Collections.IDictionary ConvertedProperties(Type converter)
        {
            switch (converter.Name)
            {
                case "PointConverter": return new System.Collections.Hashtable { ["X"] = 4, ["Y"] = 5 };
                case "SizeConverter": return new System.Collections.Hashtable { ["Width"] = 20, ["Height"] = 12 };
                case "SizeFConverter": return new System.Collections.Hashtable { ["Width"] = 20.5f, ["Height"] = 12f };
                case "RectangleConverter": return new System.Collections.Hashtable { ["X"] = 4, ["Y"] = 5, ["Width"] = 20, ["Height"] = 12 };
                case "FontConverter": return new System.Collections.Hashtable { ["Name"] = "Arial", ["Size"] = 10f, ["Style"] = FontStyle.Bold, ["Unit"] = GraphicsUnit.Point, ["GdiCharSet"] = (byte)1, ["GdiVerticalFont"] = false };
                case "MarginsConverter": return new System.Collections.Hashtable { ["Left"] = 10, ["Right"] = 20, ["Top"] = 30, ["Bottom"] = 40 };
                default: return new System.Collections.Hashtable();
            }
        }

        /// <summary>A TrueType font file of the system (PrivateFontCollection.AddFontFile): Windows' Arial, Linux's Liberation Sans.</summary>
        static string SystemFontFile()
        {
            var windows = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            if (System.IO.File.Exists(windows)) return windows;
            foreach (var directory in new[] { "/usr/share/fonts" })
                if (Directory.Exists(directory))
                    foreach (var file in Directory.EnumerateFiles(directory, "LiberationSans-Regular.ttf", SearchOption.AllDirectories)) return file;
            return "no-font.ttf";
        }

        static object Number(string name, Type owner)
        {
            if (name.Contains("index") || name == "frame" || name == "i" || name == "propid") return 0;
            if (name.Contains("angle")) return name.Contains("sweep") ? 90 : 30;
            if (name == "tension") return 0.5;
            if (name.Contains("width") || name == "cx") return 20;
            if (name.Contains("height") || name == "cy") return 12;
            if (name == "x" || name == "x1" || name.StartsWith("left") || name == "dx" || name == "offsetx") return 4;
            if (name == "y" || name == "y1" || name.StartsWith("top") || name == "dy" || name == "offsety") return 5;
            if (name == "x2" || name == "right") return 26;
            if (name == "y2" || name == "bottom") return 22;
            if (name == "argb") return unchecked((int)0xFF3366CC);
            if (name == "red" || name == "r") return 51;
            if (name == "green" || name == "g") return 102;
            if (name == "blue" || name == "b") return 204;
            if (name == "alpha" || name == "a") return 200;
            if (name.Contains("emsize") || name == "size") return 10;
            if (name.Contains("dpi") || name.Contains("resolution")) return 96;
            if (name.Contains("count") || name == "numberofvalues" || name == "levels") return 2;
            if (name.Contains("scale") || name.Contains("factor")) return 1.5;
            if (name.Contains("focus") || name.Contains("position")) return 0.5;
            if (name.Contains("gamma")) return 1.8;
            if (name.Contains("threshold")) return 0.5;
            if (name.Contains("value") || name == "quality") return 75;
            return 3;
        }

        string Text(string name, MemberInfo member)
        {
            var memberName = member.Name;
            // A file to read: the members that load (FromFile, the file constructors); one to write: the others.
            if (name.Contains("file") || name == "path" || name == "url")
            {
                if (memberName == "AddFontFile") return SystemFontFile();
                var reads = memberName == "FromFile" || memberName == "ExtractAssociatedIcon" || member is ConstructorInfo;
                if (member.DeclaringType == typeof(Icon)) return reads ? ResourceFile("icon.ico") : OutputFile(".ico");
                if (member.DeclaringType == typeof(Metafile)) return reads ? ResourceFile("picture.emf") : OutputFile(".emf");
                return reads ? ResourceFile("alpha.png") : OutputFile(".png");
            }
            if (name.Contains("familyname") || name == "name" && member.DeclaringType == typeof(FontFamily)) return "Arial";
            if (name == "familyname" || name == "fontname") return "Arial";
            if (name == "printername") return "No Such Printer";
            if (name.Contains("htmlcolor") || name == "htmlcolor") return "#3366CC";
            if (name == "resource") return "icon.ico";
            if (name == "description") return "Picture";
            return "Abc 123";
        }

        static object EnumValue(Type type, string name)
        {
            var values = Enum.GetValues(type);
            if (type == typeof(PixelFormat)) return PixelFormat.Format32bppArgb;
            if (type == typeof(GraphicsUnit)) return GraphicsUnit.Pixel;
            if (type == typeof(KnownColor)) return KnownColor.SteelBlue;
            if (type == typeof(ImageLockMode)) return ImageLockMode.ReadWrite;
            if (type == typeof(RotateFlipType)) return RotateFlipType.Rotate90FlipNone;
            if (type == typeof(FontStyle)) return FontStyle.Bold;
            if (type == typeof(CombineMode)) return CombineMode.Union;
            if (type == typeof(MatrixOrder)) return MatrixOrder.Append;
            if (type == typeof(CopyPixelOperation)) return CopyPixelOperation.SourceCopy;
            // Else the second value (the first is often None / Default).
            return values.GetValue(values.Length > 1 ? 1 : 0);
        }

        object Array(Type element, string name)
        {
            if (element == typeof(Point)) return Points.ToArray();
            if (element == typeof(PointF)) return PointFs.ToArray();
            if (element == typeof(Rectangle)) return new[] { Rect, new Rectangle(1, 1, 6, 6) };
            if (element == typeof(RectangleF)) return new[] { RectF, new RectangleF(1f, 1f, 6f, 6f) };
            if (element == typeof(byte)) return new byte[] { 0, 1, 1 };
            if (element == typeof(float))
                return name.Contains("position") ? new[] { 0f, 0.5f, 1f } : name.Contains("factor") ? new[] { 0f, 0.8f, 1f } : name.Contains("pattern") ? new[] { 3f, 1f } : new[] { 0f, 0.5f, 1f };
            if (element == typeof(float[])) return ColorMatrixElements();
            if (element == typeof(int)) return new[] { 1, 2 };
            if (element == typeof(short)) return new short[] { 1, 2 };
            if (element == typeof(long)) return new long[] { 1, 2 };
            if (element == typeof(Color)) return new[] { Color1, Color.Green, Color2 };
            if (element == typeof(ColorMap)) return new[] { new ColorMap { OldColor = Color.Red, NewColor = Color.Blue } };
            if (element == typeof(CharacterRange)) return new[] { new CharacterRange(0, 2), new CharacterRange(4, 3) };
            if (element == typeof(string)) return new[] { "a", "b" };
            if (element == typeof(object)) return new object[] { "a" };
            if (element == typeof(Type)) return new[] { typeof(Bitmap) };
            if (element == typeof(EncoderParameter)) return new[] { new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L) };
            if (element == typeof(Attribute)) return new Attribute[0];
            return System.Array.CreateInstance(element, 0);
        }

        public static float[][] ColorMatrixElements() => new[]
        {
            new[] { 0.5f, 0f, 0f, 0f, 0f },
            new[] { 0f, 1f, 0f, 0f, 0f },
            new[] { 0f, 0f, 1f, 0f, 0f },
            new[] { 0f, 0f, 0f, 0.8f, 0f },
            new[] { 0.1f, 0f, 0f, 0f, 1f },
        };

        static object Callback(Type type)
        {
            if (type == typeof(Image.GetThumbnailImageAbort)) return new Image.GetThumbnailImageAbort(() => false);
            if (type == typeof(Graphics.DrawImageAbort)) return new Graphics.DrawImageAbort(data => false);
            if (type == typeof(Graphics.EnumerateMetafileProc)) return new Graphics.EnumerateMetafileProc((recordType, flags, dataSize, data, callbackData) => true);
            if (type == typeof(EventHandler)) return new EventHandler((s, e) => { });
            if (type == typeof(PrintEventHandler)) return new PrintEventHandler((s, e) => { });
            if (type == typeof(PrintPageEventHandler)) return new PrintPageEventHandler((s, e) => { e.HasMorePages = false; });
            if (type == typeof(QueryPageSettingsEventHandler)) return new QueryPageSettingsEventHandler((s, e) => { });
            if (type == typeof(PlayRecordCallback)) return new PlayRecordCallback((recordType, flags, dataSize, recordData) => { });
            return null;
        }

        /// <summary>A fresh instance of a type of the API (its abstract types by a concrete one), for a member to be called on.</summary>
        public override object Instance(Type type) => Instance(type, "");

        public object Instance(Type type, string name)
        {
            if (type == typeof(Color)) return name.Contains("new") || name.Contains("end") || name.Contains("back") || name.Contains("second") ? Color2 : Color1;
            if (type == typeof(Point)) return new Point(4, 5);
            if (type == typeof(PointF)) return new PointF(4.5f, 5f);
            if (type == typeof(Size)) return new Size(20, 12);
            if (type == typeof(SizeF)) return new SizeF(20f, 12.5f);
            if (type == typeof(Rectangle)) return Rect;
            if (type == typeof(RectangleF)) return RectF;
            if (type == typeof(CharacterRange)) return new CharacterRange(0, 3);
            if (type == typeof(Bitmap) || type == typeof(Image)) return Picture();
            if (type == typeof(Graphics)) return NewGraphics();
            if (type == typeof(IDeviceContext)) return NewGraphics();
            if (type == typeof(Icon)) return Icon();
            if (type == typeof(Metafile)) return Metafile();
            if (type == typeof(Font)) return Font();
            if (type == typeof(FontFamily)) return Keep(new FontFamily("Arial"));
            if (type == typeof(FontCollection) || type == typeof(InstalledFontCollection)) return Keep(new InstalledFontCollection());
            if (type == typeof(PrivateFontCollection)) return Keep(new PrivateFontCollection());
            if (type == typeof(Brush) || type == typeof(SolidBrush)) return Keep(new SolidBrush(Color1));
            if (type == typeof(TextureBrush)) return Keep(new TextureBrush(Picture()));
            if (type == typeof(HatchBrush)) return Keep(new HatchBrush(HatchStyle.Cross, Color1, Color.White));
            if (type == typeof(LinearGradientBrush)) return Keep(new LinearGradientBrush(Rect, Color1, Color2, 30f));
            if (type == typeof(PathGradientBrush)) return Keep(new PathGradientBrush(PointFs));
            if (type == typeof(Pen)) return Keep(new Pen(Color2, 2f));
            if (type == typeof(StringFormat)) return Keep(new StringFormat());
            if (type == typeof(Region)) return Keep(new Region(Rect));
            if (type == typeof(GraphicsPath)) return SamplePath();
            if (type == typeof(GraphicsPathIterator)) return Keep(new GraphicsPathIterator(SamplePath()));
            if (type == typeof(Matrix)) return Keep(new Matrix(1f, 0.5f, 0f, 1f, 2f, 3f));
            if (type == typeof(System.Drawing.Design.CategoryNameCollection)) return new System.Drawing.Design.CategoryNameCollection(new[] { "Layout", "Appearance" });
            if (type == typeof(Blend)) return new Blend(3) { Factors = new[] { 0f, 0.8f, 1f }, Positions = new[] { 0f, 0.5f, 1f } };
            if (type == typeof(ColorBlend)) return new ColorBlend(3) { Colors = new[] { Color1, Color.Green, Color2 }, Positions = new[] { 0f, 0.5f, 1f } };
            if (type == typeof(PathData)) return SamplePath().PathData;
            if (type == typeof(CustomLineCap)) { var stroke = Keep(new GraphicsPath()); stroke.AddLine(-2, 0, 2, 0); return Keep(new CustomLineCap(null, stroke)); }
            if (type == typeof(AdjustableArrowCap)) return Keep(new AdjustableArrowCap(3f, 3f));
            if (type == typeof(GraphicsState)) return NewGraphics().Save();
            if (type == typeof(GraphicsContainer)) return NewGraphics().BeginContainer();
            if (type == typeof(ImageAttributes)) return Keep(new ImageAttributes());
            if (type == typeof(ColorMatrix)) return new ColorMatrix(ColorMatrixElements());
            if (type == typeof(ColorMap)) return new ColorMap { OldColor = Color.Red, NewColor = Color.Blue };
            if (type == typeof(ColorPalette)) return Picture(4, 4, PixelFormat.Format8bppIndexed).Palette;
            if (type == typeof(BitmapData)) { var picture = Picture(); return picture.LockBits(new Rectangle(0, 0, 4, 4), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb); }
            if (type == typeof(System.Drawing.Imaging.Encoder)) return System.Drawing.Imaging.Encoder.Quality;
            if (type == typeof(EncoderParameter)) return Keep(new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L));
            if (type == typeof(EncoderParameters)) { var parameters = Keep(new EncoderParameters(1)); parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L); return parameters; }
            if (type == typeof(ImageCodecInfo)) return ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            if (type == typeof(ImageFormat)) return ImageFormat.Png;
            if (type == typeof(FrameDimension)) return FrameDimension.Page;
            if (type == typeof(PropertyItem)) { var jpeg = Keep(Image.FromStream(Stream("exif.jpg"))); return jpeg.PropertyItems.OrderBy(p => p.Id).First(); }
            if (type == typeof(MetafileHeader)) return Metafile().GetMetafileHeader();
            if (type == typeof(MetaHeader)) return new MetaHeader();
            if (type == typeof(WmfPlaceableFileHeader)) return new WmfPlaceableFileHeader();
            if (type == typeof(BufferedGraphicsContext)) return Keep(new BufferedGraphicsContext());
            if (type == typeof(BufferedGraphics)) { var context = Keep(new BufferedGraphicsContext()); return Keep(context.Allocate(NewGraphics(), new Rectangle(0, 0, 32, 32))); }
            if (type == typeof(PrintDocument)) { var document = Keep(new PrintDocument()); document.PrintController = new PreviewPrintController(); return document; }
            if (type == typeof(PrintController) || type == typeof(PreviewPrintController)) return new PreviewPrintController();
            if (type == typeof(StandardPrintController)) return new StandardPrintController();
            if (type == typeof(PrinterSettings)) return new PrinterSettings { PrinterName = "No Such Printer" };
            if (type == typeof(PageSettings)) return new PageSettings(new PrinterSettings { PrinterName = "No Such Printer" });
            if (type == typeof(Margins)) return new Margins(10, 20, 30, 40);
            if (type == typeof(PaperSize)) return new PaperSize("Custom", 100, 200);
            if (type == typeof(PaperSource)) return new PaperSource { SourceName = "Tray", RawKind = 257 };
            if (type == typeof(PrinterResolution)) return new PrinterResolution { X = 600, Y = 600 };
            if (type == typeof(PrintEventArgs)) return new PrintEventArgs();
            if (type == typeof(PrintPageEventArgs)) return new PrintPageEventArgs(NewGraphics(), Rect, new Rectangle(0, 0, 32, 32), (PageSettings)Instance(typeof(PageSettings)));
            if (type == typeof(QueryPageSettingsEventArgs)) return new QueryPageSettingsEventArgs((PageSettings)Instance(typeof(PageSettings)));
            if (type == typeof(PreviewPageInfo)) return new PreviewPageInfo(Picture(), new Size(100, 200));
            if (type == typeof(InvalidPrinterException)) return new InvalidPrinterException(new PrinterSettings { PrinterName = "No Such Printer" });
            if (type == typeof(PrinterSettings.PaperSizeCollection)) return new PrinterSettings.PaperSizeCollection(new[] { new PaperSize("Custom", 100, 200), new PaperSize("Other", 50, 70) });
            if (type == typeof(PrinterSettings.PaperSourceCollection)) return new PrinterSettings.PaperSourceCollection(new[] { new PaperSource { SourceName = "Tray", RawKind = 257 } });
            if (type == typeof(PrinterSettings.PrinterResolutionCollection)) return new PrinterSettings.PrinterResolutionCollection(new[] { new PrinterResolution { X = 600, Y = 600 } });
            if (type == typeof(PrinterSettings.StringCollection)) return new PrinterSettings.StringCollection(new[] { "a", "b" });
            if (type == typeof(RegionData)) return Keep(new Region(Rect)).GetRegionData();
            if (typeof(Delegate).IsAssignableFrom(type)) return Callback(type);
            if (type == typeof(ToolboxBitmapAttribute)) return new ToolboxBitmapAttribute(typeof(Bitmap));
            if (type == typeof(ITypeDescriptorContext) || type == typeof(IServiceProvider)) return null;
            if (type == typeof(Attribute)) return new BrowsableAttribute(true);

            if (type.IsAbstract || type.IsInterface) return null;
            var parameterless = type.GetConstructor(Type.EmptyTypes);
            if (parameterless != null) return Keep(parameterless.Invoke(null));
            if (type.IsValueType) return Activator.CreateInstance(type);
            return null;
        }
    }
}
