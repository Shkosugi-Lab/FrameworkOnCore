using System.Text.RegularExpressions;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// Where the port's step that answers a request may be another than the module IIS's trace names, each with why (null:
/// it may not). The answers themselves are compared apart (Allowed); here it is who gives them, and so in which order.
/// </summary>
static class StepAllowed
{
    public static string Why(string category, string name, string iis, string port)
    {
        bool Has(string text) => name.Contains(text, StringComparison.OrdinalIgnoreCase);
        const string RequestFiltering = "RequestFilteringModule", Static = "StaticFile", Managed = "ManagedPipeline";

        // ---- the port's choices (stricter than IIS)
        if (port == RequestFiltering && iis == Static && (Has("appsettings.json") || Has("/to-settings")))
            return "the port's choice: .NET's settings file is refused by its request filtering (IIS, which has no such file, serves or misses it as a static file)";
        if (port == RequestFiltering && iis == Static && Regex.IsMatch(name, @"\.(asa|razor|resource)\b", RegexOptions.IgnoreCase))
            return "the port's choice: types IIS's static file module has no MIME type for (404.3) are in the port's denied extensions (404.7)";
        if (port == RequestFiltering && iis == Static && Has("~1"))
            return "a short (8.3) name: no file to IIS's core (404.0); the port, which has no short names, refuses the name as global.asa's";
        if (port == Managed && iis == Static && (Has("::$DATA") || Has("::$INDEX_ALLOCATION")))
            return "an NTFS stream's name: no file to IIS's core (404.0); the port refuses the ':' before anything maps it (400)";

        // ---- the same answer of another step
        if (port == Static && iis == Managed && Regex.IsMatch(name, @"/[^/ ]+\.[A-Za-z]+/x\.aspx"))
            return "a page's name after a file's (\"/a.txt/x.aspx\"): IIS maps the handler by the whole URL (the page handler, whose 404 it is); " +
                   "the port finds the file and its path info first (the static file's 404)";
        if (port == Managed && iis == Static && category is "headers" or "bodies" && (Has("unknown method") || Has("TRACK") || Has("100 KB to a static file")))
            return "a verb the static file does not take: IIS's static file module's 405, ASP.NET's static file handler's on the port";
        if (category == "faults")
            return "a file or folder the server cannot open: IIS's core fails as it authenticates (the file's access), the port as its handler opens it";
        if (name == "OPTIONS the asterisk form")
            return "IIS's URL Rewrite fails on \"*\" (500.50); Kestrel gives it the root's path (Allowed)";
        return null;
    }
}
