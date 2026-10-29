using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// What a server answered to a case, as the golden keeps it and the tests compare it: the status, IIS's substatus where its
/// page shows it, the kind of body (a probe's marker, IIS's error page, http.sys's, ASP.NET's error with its exceptions),
/// the headers that matter (Location, Set-Cookie, the ones an injection would add), the server's host written "{host}".
/// </summary>
sealed class Observation
{
    [JsonPropertyName("category")] public string Category { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("status")] public int Status { get; set; }
    [JsonPropertyName("sub")] public string Sub { get; set; }
    [JsonPropertyName("kind")] public string Kind { get; set; }
    [JsonPropertyName("location")] public string Location { get; set; }
    [JsonPropertyName("cookies")] public string[] Cookies { get; set; }
    [JsonPropertyName("injected")] public bool Injected { get; set; }
    [JsonPropertyName("probe")] public string Probe { get; set; }
    [JsonPropertyName("secrets")] public string[] Secrets { get; set; }
    [JsonPropertyName("interim")] public int[] Interim { get; set; }

    [JsonIgnore] public string Text;

    public static Observation Of(AttackCase @case, RawHttp.Answer answer, Target target)
    {
        string Normal(string s) => s?.Replace(target.Host, "{host}").Replace("127.0.0.1:" + target.Port, "{host}");
        var text = answer.Text;
        var observation = new Observation
        {
            Category = @case.Category,
            Name = @case.Name,
            Status = answer.Status,
            Location = Normal(answer.Header("Location")),
            Cookies = answer.All("Set-Cookie").Select(c => Regex.Replace(c, @"^(ASP\.NET_SessionId|\.ASPXAUTH)=[^;]*", "$1=*")).ToArray(),
            Injected = answer.Headers.Any(h => h.Name.Equals("X-Injected", StringComparison.OrdinalIgnoreCase) ||
                h.Name.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase) && h.Value.StartsWith("evil=")),
            Probe = answer.Header("X-Probe") ?? answer.Header("X-A"),
            Text = text,
        };
        if (observation.Cookies.Length == 0) observation.Cookies = null;
        if (answer.Interim.Count > 0) observation.Interim = answer.Interim.ToArray();
        var iisError = Regex.Match(text, @"<h3>HTTP (?:Error|エラー) (\d{3}\.\d+)");
        var secret = Regex.Matches(text, @"(SECRET|LINKED)-[A-Za-z0-9_.+\-]+").Select(m => m.Value).Distinct().ToArray();
        observation.Secrets = secret.Length == 0 ? null : secret;
        var marker = Regex.Match(text, @"(SECRET-[A-Za-z0-9_.+\-]+|LINKED-outside|PUBLIC-\S+|ECHO [^\r\n<]*|POSTBACK [^\r\n<]*|RETURN [^\r\n<]*|POST form=[^\r\n<]*|HEADER|COOKIE|STATUS|\bFORM\b)");
        var exceptions = Regex.Matches(text, @"\[(\w+Exception)\b").Select(m => m.Groups[1].Value).Distinct().ToArray();
        var httpSys = Regex.Match(text, @"<h2>(Bad Request[^<]*|Not Found|Request Header Too Long|Length Required|Request URL Too Long|Service Unavailable|Not Implemented|Request Entity Too Large)</h2>");
        if (answer.Status == 0) observation.Kind = answer.TimedOut ? "none (timed out)" : "none (closed)";
        else if (iisError.Success)
        {
            observation.Kind = "iis error";
            observation.Sub = iisError.Groups[1].Value;
        }
        else if (httpSys.Success || answer.Header("Server") == "Microsoft-HTTPAPI/2.0") observation.Kind = "http.sys " + (httpSys.Success ? httpSys.Groups[1].Value : answer.Reason);
        else if (exceptions.Length > 0) observation.Kind = "aspnet " + string.Join(" ", exceptions);
        else if (marker.Success) observation.Kind = "content " + Normal(marker.Value).Trim();
        else if (answer.Body.Length == 0) observation.Kind = "empty";
        else observation.Kind = "other " + (answer.Header("Content-Type") ?? "");
        return observation;
    }

    public override string ToString() =>
        Status + (Sub != null ? " (" + Sub + ")" : "") + " " + Kind + (Location != null ? " -> " + Location : "") +
        (Cookies != null ? " cookies=" + string.Join(" | ", Cookies) : "") + (Injected ? " INJECTED" : "") + (Probe != null ? " probe=" + Probe : "") +
        (Secrets != null ? " secrets=" + string.Join(" ", Secrets) : "") + (Interim != null ? " interim=" + string.Join(" ", Interim) : "");
}
