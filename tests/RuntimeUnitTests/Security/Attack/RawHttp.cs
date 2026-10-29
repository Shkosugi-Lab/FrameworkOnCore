using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// A request sent as its bytes over a socket (what HttpClient would normalise: the target as written, "..", "%2e", '\',
/// raw high-bit bytes, duplicated or broken headers, bodies that end early), and its answer as read until the server
/// closes the connection (every request says Connection: close).
/// </summary>
static class RawHttp
{
    public sealed class Answer
    {
        public int Status;                 // 0: no answer (the connection closed or reset without one)
        public string Reason = "";
        public List<(string Name, string Value)> Headers = new();
        public byte[] Body = Array.Empty<byte>();
        public bool TimedOut;
        public List<int> Interim = new();

        public string Header(string name) => Headers.Where(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Select(h => h.Value).FirstOrDefault();
        public IEnumerable<string> All(string name) => Headers.Where(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Select(h => h.Value);
        public string Text => Encoding.UTF8.GetString(Body);
    }

    /// <summary>
    /// Sends the request (its head given as text, latin-1 bytes unless the text has non-ASCII characters, written as UTF-8;
    /// "\u00XX" in a target: the byte XX), then the body; closes the sending side when told (a body that ends early).
    /// </summary>
    public static Answer Send(int port, byte[] request, bool closeSending = false, int timeout = 15000, Action<NetworkStream> during = null)
    {
        using var client = new TcpClient();
        client.NoDelay = true;
        client.Connect("127.0.0.1", port);
        client.ReceiveTimeout = timeout;
        client.SendTimeout = timeout;
        var stream = client.GetStream();
        var answer = new Answer();
        try
        {
            stream.Write(request);
            during?.Invoke(stream);
            if (closeSending) client.Client.Shutdown(SocketShutdown.Send);
        }
        catch (IOException) { }
        var received = new MemoryStream();
        var buffer = new byte[65536];
        var clock = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            while (true)
            {
                var read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0) break;
                received.Write(buffer, 0, read);
                if (Complete(received.ToArray())) break;
            }
        }
        catch (IOException e) when (e.InnerException is SocketException s)
        {
            if (s.SocketErrorCode == SocketError.TimedOut || clock.ElapsedMilliseconds >= timeout - 100) answer.TimedOut = true;
        }
        Parse(received.ToArray(), answer);
        return answer;
    }

    /// <summary>Whether the bytes are a whole response (its length, or its chunks, all there).</summary>
    static bool Complete(byte[] bytes)
    {
        var end = IndexOf(bytes, "\r\n\r\n"u8.ToArray());
        if (end < 0) return false;
        var head = Encoding.Latin1.GetString(bytes, 0, end);
        var length = Regex.Match(head, @"\r\nContent-Length:\s*(\d+)", RegexOptions.IgnoreCase);
        if (length.Success) return bytes.Length - end - 4 >= long.Parse(length.Groups[1].Value);
        if (Regex.IsMatch(head, @"\r\nTransfer-Encoding:\s*chunked", RegexOptions.IgnoreCase))
            return IndexOf(bytes, "\r\n0\r\n\r\n"u8.ToArray(), end) >= 0 || IndexOf(bytes, "\r\n\r\n0\r\n\r\n"u8.ToArray(), end) >= 0;
        // A HEAD's, a 204's or 304's: no body.
        var status = Regex.Match(head, @"^HTTP/\d\.\d (\d{3})");
        return status.Success && (status.Groups[1].Value is "204" or "304");
    }

    static int IndexOf(byte[] bytes, byte[] pattern, int from = 0)
    {
        for (var i = from; i <= bytes.Length - pattern.Length; i++)
        {
            var j = 0;
            while (j < pattern.Length && bytes[i + j] == pattern[j]) j++;
            if (j == pattern.Length) return i;
        }
        return -1;
    }

    static void Parse(byte[] bytes, Answer answer)
    {
        // An interim answer (100 Continue) before the final one: noted, skipped.
        while (bytes.Length > 12 && Encoding.ASCII.GetString(bytes, 0, 10) is "HTTP/1.1 1" or "HTTP/1.0 1")
        {
            var interim = IndexOf(bytes, "\r\n\r\n"u8.ToArray());
            if (interim < 0) break;
            answer.Interim.Add(int.Parse(Encoding.ASCII.GetString(bytes, 9, 3)));
            bytes = bytes[(interim + 4)..];
        }
        var end = IndexOf(bytes, "\r\n\r\n"u8.ToArray());
        if (end < 0) return;
        var lines = Encoding.UTF8.GetString(bytes, 0, end).Split("\r\n");
        var status = Regex.Match(lines[0], @"^HTTP/\d\.\d (\d{3}) ?(.*)$");
        if (!status.Success) return;
        answer.Status = int.Parse(status.Groups[1].Value);
        answer.Reason = status.Groups[2].Value;
        foreach (var line in lines.Skip(1))
        {
            var colon = line.IndexOf(':');
            if (colon > 0) answer.Headers.Add((line[..colon], line[(colon + 1)..].Trim()));
        }
        var body = bytes[(end + 4)..];
        if (string.Equals(answer.Header("Transfer-Encoding"), "chunked", StringComparison.OrdinalIgnoreCase)) body = Dechunk(body);
        else if (long.TryParse(answer.Header("Content-Length"), out var length) && length < body.Length) body = body[..(int)length];
        answer.Body = body;
    }

    static byte[] Dechunk(byte[] bytes)
    {
        var output = new MemoryStream();
        var position = 0;
        while (position < bytes.Length)
        {
            var line = IndexOf(bytes, "\r\n"u8.ToArray(), position);
            if (line < 0) break;
            var size = Convert.ToInt32(Encoding.ASCII.GetString(bytes, position, line - position).Split(';')[0].Trim(), 16);
            if (size == 0) break;
            output.Write(bytes, line + 2, Math.Min(size, bytes.Length - line - 2));
            position = line + 2 + size + 2;
        }
        return output.ToArray();
    }

    /// <summary>A request's head as bytes: latin-1, "\u00XX" its byte XX (the raw high-bit bytes of a target).</summary>
    public static byte[] Bytes(string text) => text.Any(c => c > 0xFF) ? Encoding.UTF8.GetBytes(text) : Encoding.Latin1.GetBytes(text);
}
