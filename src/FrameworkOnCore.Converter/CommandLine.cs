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
