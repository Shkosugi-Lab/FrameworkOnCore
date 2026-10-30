using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Web.Util
{
    /// <summary>
    /// FrameworkOnCore: .NET Framework's Encoding.Default, the system's ANSI code page (932, Shift_JIS, on a Japanese
    /// Windows), which System.Web took where nothing else said: the pages, user controls, App_Code and the other files it
    /// compiles where web.config has no &lt;globalization fileEncoding&gt; (a file with a byte order mark is read in its
    /// encoding), the response's encoding without a configured one, a session's code page outside a request. .NET's
    /// Encoding.Default is UTF-8 on every platform: a Shift_JIS page read so is U+FFFD where its Japanese was. The original
    /// server's code page: the runtime option FrameworkOnCore.AnsiCodePage (the converter writes it where the pages or the
    /// sources are in one); else Windows' (GetACP); else the one of the culture the application runs in (LANG, which the
    /// deployment sets to the original server's culture), as FrameworkOnCore.Compat's Platform.DefaultEncoding.
    /// </summary>
    internal static class AnsiCodePage
    {
        /// <summary>The runtime option (runtimeconfig.json's configProperties) of the original server's ANSI code page.</summary>
        public const string Option = "FrameworkOnCore.AnsiCodePage";

        static AnsiCodePage() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        /// <summary>The code pages .NET Framework had (shift_jis, windows-1252 ...), which web.config names: registered.</summary>
        public static void Register() { }

        static readonly Lazy<Encoding> encoding = new Lazy<Encoding>(() =>
            Of(AppContext.GetData(Option), OperatingSystem.IsWindows() ? (int?)GetACP() : null, CultureInfo.InstalledUICulture));

        /// <summary>The original server's ANSI code page (.NET Framework's Encoding.Default).</summary>
        public static Encoding Encoding => encoding.Value;

        /// <summary>The code page of the option's value, else the system's (Windows), else the culture's; UTF-8 when none is one .NET has.</summary>
        internal static Encoding Of(object option, int? system, CultureInfo culture)
        {
            Register();
            int page;
            if (option != null && int.TryParse(Convert.ToString(option, CultureInfo.InvariantCulture)?.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out page) && page > 0 && Get(page) is Encoding configured)
                return configured;
            if (system is int windows && Get(windows) is Encoding systems) return systems;
            return Get(culture?.TextInfo.ANSICodePage ?? 0) ?? new UTF8Encoding(false);
        }

        static Encoding Get(int page)
        {
            if (page <= 0) return null;
            try { return Encoding.GetEncoding(page); }
            catch (ArgumentException) { return null; }
            catch (NotSupportedException) { return null; }
        }

        static readonly UTF8Encoding strictUtf8 = new UTF8Encoding(false, true);

        /// <summary>
        /// A reader of a file ASP.NET reads in its fileEncoding: the configured one (a byte order mark over it, as .NET
        /// Framework did); without one, a byte order mark's encoding, else UTF-8 when the file is valid UTF-8, else the ANSI
        /// code page. (.NET Framework read a file without a byte order mark in the ANSI code page: a valid UTF-8 one too,
        /// whose characters beyond ASCII it garbled, which no site that ran kept; Shift_JIS is not valid UTF-8.)
        /// </summary>
        public static StreamReader Reader(Stream stream, Encoding configured)
        {
            if (configured != null) return new StreamReader(stream, configured, true /*detectEncodingFromByteOrderMarks*/, 4096);
            var bytes = ReadAll(stream);
            return new StreamReader(new MemoryStream(bytes, false), EncodingOf(bytes), true, 4096);
        }

        /// <summary>The text of a file's bytes as <see cref="Reader"/> reads it.</summary>
        public static string Decode(byte[] bytes, Encoding configured)
        {
            using (var reader = configured != null
                ? new StreamReader(new MemoryStream(bytes, false), configured, true, 4096)
                : new StreamReader(new MemoryStream(bytes, false), EncodingOf(bytes), true, 4096))
                return reader.ReadToEnd();
        }

        /// <summary>The encoding of bytes without a configured one: UTF-8 when they are (a byte order mark is taken by the
        /// reader), else the ANSI code page.</summary>
        internal static Encoding EncodingOf(byte[] bytes)
        {
            if (bytes.Length >= 2 && (bytes[0] == 0xFF && bytes[1] == 0xFE || bytes[0] == 0xFE && bytes[1] == 0xFF)) return Encoding.UTF8;
            try
            {
                strictUtf8.GetCharCount(bytes);
                return new UTF8Encoding(false);
            }
            catch (DecoderFallbackException)
            {
                return Encoding;
            }
        }

        static byte[] ReadAll(Stream stream)
        {
            var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            return buffer.ToArray();
        }

        [DllImport("kernel32.dll")]
        static extern uint GetACP();

        /// <summary>The code pages registered as System.Web is loaded, not only by UseWebForms: what reads web.config before
        /// it (the host's IIS features: default documents, URL rewrites, static files, logging) does not read one in Shift_JIS
        /// (encoding="shift_jis") without them (XmlException: "System does not support 'shift_jis' encoding", taken as no
        /// such section).</summary>
        [System.Runtime.CompilerServices.ModuleInitializer]
        internal static void RegisterOnLoad() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}
