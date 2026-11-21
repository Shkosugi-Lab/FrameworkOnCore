using Microsoft.AspNetCore.Http;

namespace FrameworkOnCore.Studio;

/// <summary>
/// Studio answers this machine's browser only. It listens on 127.0.0.1, but that alone does not keep a web page out: a page
/// whose host name its DNS then points at 127.0.0.1 (DNS rebinding) is the same origin to the browser, and could call the
/// API (read files under an analysis' folder, run MSBuild on a project, start Docker). A request is taken when its Host
/// is this machine's name for Studio (127.0.0.1, localhost, [::1], with Studio's port), and its Origin, when it has one
/// (a browser sends it with every request that changes something, and with every request another site makes), is Studio's
/// own page. A client that sends no Origin (curl, a script) is not a web page another site runs.
/// </summary>
public static class LocalOnly
{
    static readonly string[] Names = ["127.0.0.1", "localhost", "[::1]"];

    public static bool Allowed(HttpRequest request, int port)
    {
        if (!Local(request.Host.Host, request.Host.Port, port)) return false;
        var origin = request.Headers.Origin.ToString();
        if (origin.Length == 0) return true;
        return Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttp &&
               Local(uri.IsLoopback && uri.HostNameType == UriHostNameType.IPv6 ? "[::1]" : uri.Host, uri.Port, port);
    }

    static bool Local(string host, int? hostPort, int port) =>
        Names.Contains(host, StringComparer.OrdinalIgnoreCase) && hostPort == port;
}
