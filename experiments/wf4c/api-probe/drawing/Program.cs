// System.Drawing on Linux (.NET 10): what an application built against System.Drawing.Common 10 (as FrameworkOnCore's WebFormsForCore is) does
// with GDI+ calls (10.0 alone throws: DllNotFound gdiplus.dll; with libgdiplus found, PlatformNotSupported). run.sh: 10.0 left out of the application's assemblies and the 6.0 Unix
// implementation (libgdiplus, System.Drawing.EnableUnixSupport) given by AssemblyLoadContext.Resolving (`resolve`).
// `map` (DllImport resolvers on the GDI+ assemblies) was tried: System.Drawing.Common 10 sets its own, and refuses non-Windows.
//   docker run --rm -v <this folder>:/src:ro mcr.microsoft.com/dotnet/sdk:10.0 bash -c "apt-get update -qq && apt-get install -y -qq python3 >/dev/null; bash /src/run.sh"
using System.Drawing; using System.Drawing.Drawing2D; using System.Drawing.Imaging; using System.Runtime.InteropServices;
if (args.Length > 0 && args[0] == "resolve")
    System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += (context, name) =>
        name.Name == "System.Drawing.Common" ? context.LoadFromAssemblyPath(Path.Combine(AppContext.BaseDirectory, "sdc6", "System.Drawing.Common.dll")) : null;
if (args.Length > 0 && args[0] == "map")
    foreach (var a in args.Skip(1))
        try { NativeLibrary.SetDllImportResolver(System.Reflection.Assembly.Load(a), (name, asm, path) =>
            name.StartsWith("gdiplus", StringComparison.OrdinalIgnoreCase) && NativeLibrary.TryLoad("libgdiplus.so.0", out var h) ? h : IntPtr.Zero); Console.WriteLine("resolver: " + a); }
        catch (Exception e) { Console.WriteLine("no " + a + ": " + e.GetType().Name); }
static void T(string n, Action a) { try { a(); Console.WriteLine($"OK    | {n}"); } catch (Exception e) { var x = e; while (x.InnerException != null) x = x.InnerException; Console.WriteLine($"THROW | {n} | {x.GetType().Name}: {x.Message.Split('\n')[0]}"); } }
T("new Bitmap + SetPixel + Save PNG", () => { using var b = new Bitmap(40, 30); b.SetPixel(1, 1, Color.Red); using var m = new MemoryStream(); b.Save(m, ImageFormat.Png); if (m.Length < 50) throw new Exception("small"); });
T("Graphics.FromImage + FillRectangle + DrawLine", () => { using var b = new Bitmap(40, 30); using var g = Graphics.FromImage(b); g.Clear(Color.White); g.FillRectangle(Brushes.Blue, 2, 2, 10, 10); g.DrawLine(Pens.Black, 0, 0, 39, 29); });
T("Resize (nop's thumbnail: HighQualityBicubic) + JPEG encoder params", () => {
    using var src = new Bitmap(400, 300); using (var g0 = Graphics.FromImage(src)) g0.Clear(Color.Orange);
    using var dst = new Bitmap(100, 75); using var g = Graphics.FromImage(dst);
    g.InterpolationMode = InterpolationMode.HighQualityBicubic; g.SmoothingMode = SmoothingMode.HighQuality; g.DrawImage(src, 0, 0, 100, 75);
    var codec = ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == "image/jpeg");
    using var p = new EncoderParameters(1); p.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
    using var m = new MemoryStream(); dst.Save(m, codec, p); File.WriteAllBytes("/tmp/t.jpg", m.ToArray()); });
T("Image.FromStream (JPEG round trip)", () => { using var i = Image.FromStream(new MemoryStream(File.ReadAllBytes("/tmp/t.jpg"))); if (i.Width != 100) throw new Exception("w=" + i.Width); });
T("Image.FromFile GIF/PNG write+read", () => { using (var b = new Bitmap(10, 10)) b.Save("/tmp/t.gif", ImageFormat.Gif); using var i = Image.FromFile("/tmp/t.gif"); });
T("DrawString (fonts: captcha)", () => { using var b = new Bitmap(120, 40); using var g = Graphics.FromImage(b); using var f = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold); g.DrawString("AbC123", f, Brushes.Black, 2, 2); var s = g.MeasureString("AbC123", f); if (s.Width <= 0) throw new Exception("measure 0"); });
T("FontFamily.Families", () => Console.Write($"      ({FontFamily.Families.Length} families) "));
T("Icon / SystemFonts", () => { var f = SystemFonts.DefaultFont; });
T("PixelFormat.Format24bppRgb LockBits", () => { using var b = new Bitmap(8, 8, PixelFormat.Format24bppRgb); var d = b.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.ReadWrite, b.PixelFormat); b.UnlockBits(d); });
T("GraphicsPath + LinearGradientBrush", () => { using var b = new Bitmap(50, 50); using var g = Graphics.FromImage(b); using var p = new GraphicsPath(); p.AddEllipse(5, 5, 40, 40); using var br = new LinearGradientBrush(new Point(0, 0), new Point(50, 50), Color.Red, Color.Blue); g.FillPath(br, p); });
T("Image.GetThumbnailImage", () => { using var b = new Bitmap(100, 100); using var t = b.GetThumbnailImage(10, 10, () => false, IntPtr.Zero); });
T("PropertyItems (EXIF rotate)", () => { using var i = Image.FromFile("/tmp/t.jpg"); var _ = i.PropertyIdList; i.RotateFlip(RotateFlipType.Rotate90FlipNone); });
