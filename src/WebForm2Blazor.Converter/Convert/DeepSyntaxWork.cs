namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Runs syntax work on a thread with room to recurse.
///
/// Roslyn's CSharpSyntaxRewriter descends one stack frame per syntax node and calls
/// EnsureSufficientExecutionStack as it goes, so a deeply nested file throws
/// InsufficientExecutionStackException rather than producing output. The default 1 MB
/// thread stack is not enough for the machine-generated and vendored sources real
/// applications carry - YAF.NET ships the whole of Lucene.Net - and the failure is not
/// deterministic, which makes it worse: the same corpus converts on one run and dies on
/// the next.
///
/// The cost is one thread per file, tens of microseconds, against a conversion measured in
/// minutes.
/// </summary>
internal static class DeepSyntaxWork
{
    private const int StackBytes = 64 * 1024 * 1024;

    public static T Run<T>(Func<T> work)
    {
        T result = default!;
        System.Runtime.ExceptionServices.ExceptionDispatchInfo? failure = null;

        var worker = new Thread(
            () =>
            {
                try
                {
                    result = work();
                }
                catch (Exception exception)
                {
                    // Rethrown on the caller's thread with the original stack trace, so a
                    // failure reads the same as it would without this detour.
                    failure = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception);
                }
            },
            StackBytes);

        worker.Start();
        worker.Join();

        failure?.Throw();
        return result;
    }
}
