namespace FrameworkOnCore.Studio;

/// <summary>
/// The language of Studio's messages (errors, states, hints): the page's (its Accept-Language, which is the browser's
/// language as the page chose it: Japanese, else English). Studio has one user, on this machine: the language of the last
/// request is the one of the messages written after it, those of the work that runs in the background too.
/// </summary>
public static class Lang
{
    static volatile bool english = true;

    public static bool English => english;

    /// <summary>The page's language, from a request's Accept-Language (its first language: ja, else English).</summary>
    public static void From(string? acceptLanguage)
    {
        if (string.IsNullOrWhiteSpace(acceptLanguage)) return;
        var first = acceptLanguage.Split(',')[0].Split(';')[0].Trim();
        english = !first.StartsWith("ja", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>A message in the page's language.</summary>
    public static string T(string japanese, string english) => English ? english : japanese;

    /// <summary>A message to the terminal (before there is a page): in the OS's language, Japanese else English.</summary>
    public static string Console(string japanese, string english) =>
        System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja" ? japanese : english;
}
