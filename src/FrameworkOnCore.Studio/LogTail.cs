using System.Text;

namespace FrameworkOnCore.Studio;

/// <summary>
/// The lines a file gets, as they are written: a build's log, which the converter's original build writes its tools'
/// output to (not to its own output, which the page shows), so that the page shows how the build goes while it runs.
/// A file written again from its start (shorter than what was read) is read from its start.
/// </summary>
public sealed class LogTail(string path)
{
    long offset;
    readonly StringBuilder partial = new();
    Decoder decoder = new UTF8Encoding(false).GetDecoder();

    /// <summary>The whole lines written since the last time; <paramref name="last"/>: the line not ended yet too.</summary>
    public List<string> Read(bool last = false)
    {
        var lines = new List<string>();
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            if (stream.Length < offset)
            {
                offset = 0;
                partial.Clear();
                decoder = new UTF8Encoding(false).GetDecoder();
            }
            stream.Position = offset;
            var bytes = new byte[64 * 1024];
            var chars = new char[bytes.Length + 1];
            int count;
            while ((count = stream.Read(bytes, 0, bytes.Length)) > 0)
            {
                offset += count;
                partial.Append(chars, 0, decoder.GetChars(bytes, 0, count, chars, 0));
                var text = partial.ToString();
                var end = text.LastIndexOf('\n');
                if (end < 0) continue;
                lines.AddRange(text[..end].Split('\n').Select(l => l.TrimEnd('\r')));
                partial.Clear().Append(text[(end + 1)..]);
            }
        }
        catch (FileNotFoundException) { }
        catch (DirectoryNotFoundException) { }
        catch (IOException) { }  // being made: the next time
        if (last && partial.Length > 0)
        {
            lines.Add(partial.ToString().TrimEnd('\r'));
            partial.Clear();
        }
        return lines;
    }

    /// <summary>The file's lines to <paramref name="add"/> every second, until <paramref name="stop"/>; then the rest.</summary>
    public static async Task Follow(string path, Action<string> add, CancellationToken stop)
    {
        var tail = new LogTail(path);
        try
        {
            while (true)
            {
                foreach (var line in tail.Read()) add(line);
                await Task.Delay(1000, stop);
            }
        }
        catch (OperationCanceledException) { }
        foreach (var line in tail.Read(last: true)) add(line);
    }
}
