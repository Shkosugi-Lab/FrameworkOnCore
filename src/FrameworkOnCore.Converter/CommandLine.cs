namespace FrameworkOnCore.Converter;

/// <summary>A command line given wrong: its message is the usage error (exit code 2).</summary>
public sealed class UsageException(string message) : Exception(message);

/// <summary>
/// The converter's commands' arguments: an option's value (missing at the end: an IndexOutOfRangeException ended the
/// converter), the one positional argument (an option it does not have was taken as the project, a second project replaced
/// the first and was converted), a value of a set.
/// </summary>
public static class CommandLine
{
    public static string Value(string[] args, ref int i)
    {
        if (i + 1 >= args.Length) throw new UsageException($"{args[i]}: its value is missing");
        return args[++i];
    }

    public static void Positional(ref string? slot, string argument, string what)
    {
        if (argument.StartsWith('-')) throw new UsageException($"{argument}: no such option");
        if (slot != null) throw new UsageException($"one {what}, not two: {slot} and {argument}");
        slot = argument;
    }

    public static string OneOf(string option, string value, params string[] values) =>
        values.Contains(value) ? value : throw new UsageException($"{option}: {string.Join(", ", values)}, not {value}");

    /// <summary>The project given: a file that is there (its full path, its names as the disk has them); else a usage error.</summary>
    public static string Project(string given)
    {
        if (Directory.Exists(given)) throw new UsageException($"{given}: a folder; the project's file (.csproj, .vbproj) is to be given");
        if (!File.Exists(given)) throw new UsageException($"{given}: no such project file");
        return Paths.OnDisk(given);
    }

    /// <summary>An option's folder that is to be there (its full path).</summary>
    public static string Folder(string option, string given)
    {
        if (File.Exists(given)) throw new UsageException($"{option} {given}: a file, not a folder");
        if (!Directory.Exists(given)) throw new UsageException($"{option} {given}: no such folder");
        return Paths.OnDisk(given);
    }

    /// <summary>An option's file that is to be there, read as what it is to be (read: throws what the file is not): its full path.</summary>
    public static string File_(string option, string given, string what, Action<string> read)
    {
        if (Directory.Exists(given)) throw new UsageException($"{option} {given}: a folder, not {what}");
        if (!File.Exists(given)) throw new UsageException($"{option} {given}: no such file");
        try { read(given); }
        catch (Exception e) when (e is System.Text.Json.JsonException || e is System.Xml.XmlException || e is InvalidDataException || e is IOException || e is UnauthorizedAccessException)
        {
            throw new UsageException($"{option} {given}: not {what} ({e.Message.Split('\n')[0].Trim()})");
        }
        return Path.GetFullPath(given);
    }

    /// <summary>
    /// What ends a command without its exception's trace (null: an exception the converter did not expect, its trace is
    /// what tells where): a file or a folder that is not there, cannot be read or written, or is not what it is to be
    /// (JSON, XML), and what the converter itself says cannot be done. FOC_TRACE=1: the trace all the same.
    /// </summary>
    public static string? Told(Exception e)
    {
        if (Environment.GetEnvironmentVariable("FOC_TRACE") == "1") return null;
        var own = e.TargetSite?.DeclaringType?.Assembly is { } assembly && (assembly == typeof(CommandLine).Assembly || assembly == typeof(FrameworkOnCore.Analysis.Choices).Assembly);
        return e is IOException || e is UnauthorizedAccessException || e is System.Text.Json.JsonException || e is System.Xml.XmlException || e is InvalidDataException ||
               e is InvalidOperationException && own
            ? $"error: {e.Message} (FOC_TRACE=1: with the exception's trace)"
            : null;
    }

    /// <summary>
    /// The converter's process ends on such an exception with its message and exit code 1 (Told), not with .NET's
    /// "Unhandled exception" and a trace: a user's mistake (a path that is not there) read as the converter's failure.
    /// </summary>
    public static void TellUnhandled() =>
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception exception && Told(exception) is { } message)
            {
                Console.Out.Flush();
                Console.Error.WriteLine(message);
                Environment.Exit(1);
            }
        };

    /// <summary>A command run, its usage errors told (exit code 2).</summary>
    public static int Run(Func<int> command)
    {
        try { return command(); }
        catch (UsageException e)
        {
            Console.Error.WriteLine(e.Message);
            return 2;
        }
    }
}
