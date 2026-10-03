using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace FrameworkOnCore.Analysis;

/// <summary>
/// A process' output, whichever encoding it writes in. On Windows the tools a build runs do not agree: .NET's tools that
/// set it, node and Go's docker write UTF-8; .NET Framework's (nuget.exe, MSBuild.exe), cmd and appcmd the console's code
/// page (OEM: Shift_JIS on Japanese Windows). Read as one of them, the others' Japanese is garbled ("繧、繝ウ" for UTF-8's
/// "イン"). Each line is taken as UTF-8 if it is valid UTF-8 (the OEM code page's Japanese is not), else as the OEM code page.
/// </summary>
public static class ProcessOutput
{
    static readonly UTF8Encoding Utf8 = new(false, throwOnInvalidBytes: true);

    static readonly Lazy<Encoding> Oem = new(() =>
    {
        if (!OperatingSystem.IsWindows()) return new UTF8Encoding(false);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        try { return Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage); }
        catch (Exception e) when (e is ArgumentException or NotSupportedException) { return new UTF8Encoding(false); }
    });

    /// <summary>A line's bytes (without its line break) as text.</summary>
    public static string Decode(ReadOnlySpan<byte> bytes)
    {
        try { return Utf8.GetString(bytes); }
        catch (DecoderFallbackException) { return Oem.Value.GetString(bytes); }
    }

    /// <summary>
    /// The lines of the process' output and errors to <paramref name="line"/> (from two threads), as they come, instead of
    /// its OutputDataReceived and ErrorDataReceived (the process started with both redirected); ends with them.
    /// </summary>
    public static Task ReadLines(Process process, Action<string> line) =>
        Task.WhenAll(ReadLines(process.StandardOutput.BaseStream, line), ReadLines(process.StandardError.BaseStream, line));

    /// <summary>The process' output and errors, whole, instead of their ReadToEnd.</summary>
    public static (string Output, string Error) ReadAll(Process process)
    {
        var output = new StringBuilder();
        var error = new StringBuilder();
        Task.WaitAll(ReadLines(process.StandardOutput.BaseStream, l => output.Append(l).Append('\n')),
                     ReadLines(process.StandardError.BaseStream, l => error.Append(l).Append('\n')));
        return (output.ToString(), error.ToString());
    }

    /// <summary>
    /// The lines of a stream: ended by \n, \r\n or \r (as StreamReader.ReadLine ends them; progress written over itself
    /// with \r is a line each time). The OEM code pages' second bytes are not \n or \r: the bytes are split before they
    /// are decoded.
    /// </summary>
    public static Task ReadLines(Stream stream, Action<string> line) => Task.Run(() =>
    {
        var pending = new MemoryStream();
        var buffer = new byte[16 * 1024];
        var afterReturn = false;
        void Emit()
        {
            line(Decode(pending.GetBuffer().AsSpan(0, (int)pending.Length)));
            pending.SetLength(0);
        }
        int count;
        while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            var start = 0;
            for (var i = 0; i < count; i++)
            {
                var b = buffer[i];
                if (b == '\n' && afterReturn) { start = i + 1; afterReturn = false; continue; }  // \r\n: one line break
                afterReturn = b == '\r';
                if (b != '\n' && b != '\r') continue;
                pending.Write(buffer, start, i - start);
                Emit();
                start = i + 1;
            }
            pending.Write(buffer, start, count - start);
        }
        if (pending.Length > 0) Emit();
    });
}
