using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// What IIS's own trace of a request says (failed request tracing: iis-golden/trace.ps1), for a case: the handler it
/// mapped the request to, the module that set the answer's status and in which notification, the URLs a module changed
/// it to, the URL Rewrite rules that matched, and the modules it notified in order. IIS's answer says what it did; its
/// trace says which module did it and when: the order the port's steps are held to.
/// </summary>
sealed class IisTraceCase
{
    [JsonPropertyName("category")] public string Category { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    /// <summary>
    /// The handlers mapped, in order ("ExtensionlessUrlHandler-Integrated-4.0 > [child] StaticFile": the request's, then a
    /// child request's - a rewrite's, the extensionless handler's transfer to the static files).
    /// </summary>
    [JsonPropertyName("handlers")] public string Handlers { get; set; }
    /// <summary>
    /// The modules that set the answer's status, in order, each with its notification and status
    /// ("RewriteModule@BeginRequest=301 > ScriptModule-4.0@EndRequest=404.0"): the last one's is the answer's. Null: none
    /// set one (the handler answered 200).
    /// </summary>
    [JsonPropertyName("decided")] public string Decided { get; set; }
    /// <summary>The child requests started, by URL path ("/App_Data/users.xml": a rewrite's; the URL again: a transfer's).</summary>
    [JsonPropertyName("children")] public string[] Children { get; set; }
    /// <summary>URL Rewrite's rules that matched, with their action ("go: Redirect /a.txt").</summary>
    [JsonPropertyName("rules")] public string[] Rules { get; set; }
    /// <summary>The modules notified, in order, by notification (an index into the trace file's "paths").</summary>
    [JsonPropertyName("path")] public int Path { get; set; }
}

sealed class IisTraceFile
{
    [JsonPropertyName("comment")] public string Comment { get; set; }
    [JsonPropertyName("server")] public string Server { get; set; }
    /// <summary>The distinct sequences of modules notified ("BeginRequest: A, B | AuthenticateRequest: C").</summary>
    [JsonPropertyName("paths")] public List<string> Paths { get; set; }
    [JsonPropertyName("cases")] public List<IisTraceCase> Cases { get; set; }
}

static class IisTrace
{
    static readonly XNamespace E = "http://schemas.microsoft.com/win/2004/08/events/event";

    // IIS's request notifications (httpserv.h: RQ_*), and the global ones a trace has.
    static readonly Dictionary<string, string> Notifications = new()
    {
        ["1"] = "BeginRequest", ["2"] = "AuthenticateRequest", ["4"] = "AuthorizeRequest", ["8"] = "ResolveRequestCache", ["16"] = "MapRequestHandler",
        ["32"] = "AcquireRequestState", ["64"] = "PreExecuteRequestHandler", ["128"] = "ExecuteRequestHandler", ["256"] = "ReleaseRequestState",
        ["512"] = "UpdateRequestCache", ["1024"] = "LogRequest", ["2048"] = "EndRequest", ["536870912"] = "SendResponse", ["2147483648"] = "PreBeginRequest",
        ["268435456"] = "CustomNotification", ["1073741824"] = "MapPath", ["134217728"] = "ReadEntity",
    };

    // A URL's path and query ("http://host:port/a?b": "/a?b").
    static string UrlPath(string url)
    {
        var scheme = url.IndexOf("://", StringComparison.Ordinal);
        var slash = scheme < 0 ? url.IndexOf('/') : url.IndexOf('/', scheme + 3);
        return slash < 0 ? url : url[slash..];
    }

    /// <summary>
    /// A case's trace: its last request's (a case of several requests). A child request has no trace of its own: the
    /// default document's and the extensionless handler's are in their parent's in full, URL Rewrite's rewritten URL only
    /// as its start and end (what answered it is not traced).
    /// </summary>
    public static (IisTraceCase Case, string Path) Whole(List<(IisTraceCase Case, string Path, string Url)> traces) => (traces[^1].Case, traces[^1].Path);

    static string Notification(string number) => Notifications.TryGetValue(number ?? "", out var name) ? name : "Notification" + number;

    /// <summary>The marker a trace's request had (Target.Marker), and what the trace says; null for a trace without one.</summary>
    public static (string Marker, IisTraceCase Case, string Path, string Url)? Read(string file)
    {
        XDocument document;
        try { document = XDocument.Load(file); }
        catch (System.Xml.XmlException) { return null; }
        var root = document.Root;
        var events = root.Elements(E + "Event").Select(e => (
            Name: (string)e.Element(E + "RenderingInfo")?.Element(E + "Opcode") ?? "",
            Data: e.Element(E + "EventData")?.Elements(E + "Data").GroupBy(d => (string)d.Attribute("Name") ?? "").ToDictionary(g => g.Key, g => g.First().Value) ?? new())).ToList();
        string marker = null;
        foreach (var (name, data) in events)
        {
            if (name != "GENERAL_REQUEST_HEADERS" || !data.TryGetValue("Headers", out var headers)) continue;
            var match = Regex.Match(headers, @"X-Foc-Case: (\d+)");
            if (match.Success) marker = match.Groups[1].Value;
        }
        if (marker == null) return null;

        string Get(Dictionary<string, string> data, string key) => data.TryGetValue(key, out var value) ? value : "";
        var handlers = new List<string>();
        var decided = new List<string>();
        var children = new List<string>();
        var rules = new List<string>();
        string action = null;
        var depth = 0;
        foreach (var (name, data) in events)
        {
            switch (name)
            {
                case "GENERAL_CHILD_REQUEST_START":
                    depth++;
                    children.Add(UrlPath(Get(data, "RequestURL")));
                    break;
                case "GENERAL_CHILD_REQUEST_END": depth--; break;
                case "HANDLER_CHANGED": handlers.Add((depth > 0 ? "[child] " : "") + Get(data, "NewHandlerName")); break;
                case "MODULE_SET_RESPONSE_ERROR_STATUS":
                    decided.Add($"{Get(data, "ModuleName")}@{Notification(Get(data, "Notification"))}={Get(data, "HttpStatus")}.{Get(data, "HttpSubStatus")}");
                    break;
                case "MODULE_SET_RESPONSE_SUCCESS_STATUS":
                    decided.Add($"{Get(data, "ModuleName")}@{Notification(Get(data, "Notification"))}={Get(data, "HttpStatus")}");
                    break;
                // URL Rewrite: the rule that matched (RULE_EVALUATION_END Succeeded), with the action before it.
                case "REDIRECT_ACTION": action = "Redirect " + Get(data, "RedirectURL"); break;
                case "REWRITE_ACTION": action = "Rewrite " + Get(data, "RewriteURL"); break;
                case "ABORT_REQUEST_ACTION": action = "AbortRequest"; break;
                case "CUSTOM_RESPONSE_ACTION": action = "CustomResponse " + Get(data, "StatusCode"); break;
                case "RULE_EVALUATION_END":
                    if (Get(data, "Succeeded") == "true") rules.Add(Get(data, "RuleName") + ": " + (action ?? "None"));
                    action = null;
                    break;
            }
        }
        // The modules notified, in order, by notification (the same module again in a notification once).
        var path = new StringBuilder();
        string current = null;
        var seen = new HashSet<string>();
        foreach (var (name, data) in events)
        {
            if (name != "NOTIFY_MODULE_START") continue;
            var notification = Notification(Get(data, "Notification"));
            var module = Get(data, "ModuleName");
            if (notification != current)
            {
                if (!seen.Add(notification + " ")) continue;   // a notification again (a child request): its first pass
                path.Append(current == null ? "" : " | ").Append(notification).Append(": ");
                current = notification;
                path.Append(module);
                seen.Add(notification + ":" + module);
            }
            else if (seen.Add(notification + ":" + module)) path.Append(", ").Append(module);
        }
        return (marker, new IisTraceCase
        {
            Handlers = string.Join(" > ", handlers), Decided = decided.Count == 0 ? null : string.Join(" > ", decided),
            Children = children.Count == 0 ? null : children.ToArray(), Rules = rules.Count == 0 ? null : rules.ToArray(),
        }, path.ToString(), UrlPath((string)root.Attribute("url") ?? ""));
    }

    /// <summary>
    /// IIS's request pipeline as the traces show it (IIS-PIPELINE.md): the modules of each notification in their order, the
    /// module and notification behind each status, the handlers the requests were mapped to.
    /// </summary>
    public static string Describe(IisTraceFile file)
    {
        var text = new StringBuilder();
        text.Append("# IIS's request pipeline, as its own traces show it\n\n");
        text.Append("Made by `trace.ps1` from IIS's failed request tracing of the attack tests' cases (" + file.Cases.Count + " cases; " + file.Server + "). Do not edit: record again.\n\n");
        text.Append("IIS answers a request by notifying its modules, notification after notification; within a notification, in the order below. ");
        text.Append("The port's steps (UseWebFormsIis's summary) follow this order.\n\n");

        // The order within each notification: every trace's sequence merged (a module before another in every trace that has both).
        text.Append("## The modules of each notification, in order\n\n");
        var order = new[] { "PreBeginRequest", "BeginRequest", "AuthenticateRequest", "AuthorizeRequest", "ResolveRequestCache", "MapRequestHandler", "AcquireRequestState",
            "PreExecuteRequestHandler", "ExecuteRequestHandler", "ReleaseRequestState", "UpdateRequestCache", "LogRequest", "EndRequest", "SendResponse" };
        var sequences = new Dictionary<string, List<string[]>>();
        foreach (var path in file.Paths)
            foreach (var part in path.Split(" | "))
            {
                var colon = part.IndexOf(": ", StringComparison.Ordinal);
                if (colon < 0) continue;
                var notification = part[..colon];
                if (!sequences.TryGetValue(notification, out var list)) sequences[notification] = list = new();
                list.Add(part[(colon + 2)..].Split(", "));
            }
        text.Append("| Notification | Modules |\n|---|---|\n");
        foreach (var notification in order.Concat(sequences.Keys.Except(order).OrderBy(k => k, StringComparer.Ordinal)))
        {
            if (!sequences.TryGetValue(notification, out var lists)) continue;
            text.Append("| ").Append(notification).Append(" | ").Append(string.Join(" → ", Merge(lists))).Append(" |\n");
        }

        text.Append("\n## Which module sets each status, and when\n\n");
        text.Append("Every status a module set, in the request or in a child request of it; where several did, the last one's is the answer's.\n\n");
        text.Append("| Status | Module | Notification | Times | For example |\n|---|---|---|---|---|\n");
        var sets = file.Cases.Where(c => c.Decided != null).SelectMany(c => c.Decided.Split(" > ").Select(d => (Set: d, c.Name))).ToList();
        foreach (var group in sets.GroupBy(d => d.Set).OrderBy(g => g.Key[(g.Key.IndexOf('=') + 1)..], StringComparer.Ordinal).ThenByDescending(g => g.Count()))
        {
            var at = group.Key.IndexOf('@');
            var equals = group.Key.IndexOf('=');
            text.Append($"| {group.Key[(equals + 1)..]} | {group.Key[..at]} | {group.Key[(at + 1)..equals]} | {group.Count()} | `{Cell(group.First().Name)}` |\n");
        }

        text.Append("\n## A status set after another\n\n");
        text.Append("A later module's status replaces an earlier one's (URL Rewrite's redirect, then ASP.NET's 404 as its modules end the request).\n\n");
        text.Append("| The statuses, in order | Cases | For example |\n|---|---|---|\n");
        foreach (var group in file.Cases.Where(c => c.Decided != null && c.Decided.Contains(" > ")).GroupBy(c => c.Decided).OrderByDescending(g => g.Count()))
            text.Append($"| {group.Key} | {group.Count()} | `{Cell(group.First().Name)}` |\n");

        text.Append("\n## The handlers the requests were mapped to\n\n");
        text.Append("The handler is mapped before BeginRequest, for the whole URL as sent and the verb (IIS has no path info for a static file: \"/a.txt/x\" is an extensionless URL). ");
        text.Append("A child request maps again: URL Rewrite's rewritten URL runs as one, from BeginRequest; the extensionless handler (ASP.NET's TransferRequestHandler) hands a URL no route takes to a child request, which the static file handler answers.\n\n");
        text.Append("| Handlers, in order | Cases | For example |\n|---|---|---|\n");
        foreach (var group in file.Cases.GroupBy(c => c.Handlers ?? "").OrderByDescending(g => g.Count()))
            text.Append($"| {(group.Key.Length == 0 ? "(none: answered before a handler was mapped)" : group.Key)} | {group.Count()} | {string.Join(", ", group.Take(2).Select(c => "`" + Cell(c.Name) + "`"))} |\n");

        text.Append("\n## URL Rewrite: when its rules are evaluated\n\n");
        var rewritten = file.Cases.Where(c => c.Rules != null).ToList();
        text.Append("Rules matched in " + rewritten.Count + " cases. A request a module refused before RewriteModule's BeginRequest has no rule evaluated:\n\n");
        text.Append("| Refused by | Cases whose URL a rule's pattern would have matched (\"/rw/\", \"/go/\", \"/to-\") | For example |\n|---|---|---|\n");
        foreach (var group in file.Cases.Where(c => c.Rules == null && c.Decided != null && Regex.IsMatch(c.Name, @" (/\(S\([a-z]+\)\))?/(rw|go|to-)"))
                     .GroupBy(c => c.Decided.Split(" > ")[0]).OrderByDescending(g => g.Count()))
            text.Append($"| {group.Key} | {group.Count()} | `{Cell(group.First().Name)}` |\n");
        text.Append("\nA rewritten URL's child request, refused in its own BeginRequest (the request filtering runs again for it):\n\n");
        text.Append("| Rule | Child request | Refused by | For example |\n|---|---|---|---|\n");
        foreach (var group in file.Cases.Where(c => c.Rules != null && c.Children != null && c.Decided != null && c.Rules[0].Contains("Rewrite"))
                     .GroupBy(c => (Rule: c.Rules[0], Decided: c.Decided)).OrderByDescending(g => g.Count()).Take(12))
            text.Append($"| {Cell(group.Key.Rule)} | {group.First().Children[0]} | {group.Key.Decided} | `{Cell(group.First().Name)}` |\n");
        return text.ToString();
    }

    static string Cell(string value) => value.Replace("|", "\\|").Replace("`", "'");

    // One order of the modules that every sequence agrees with (a module is placed once all it follows somewhere are).
    static List<string> Merge(List<string[]> lists)
    {
        var after = new Dictionary<string, HashSet<string>>();
        foreach (var list in lists)
            for (var i = 0; i < list.Length; i++)
            {
                if (!after.TryGetValue(list[i], out var set)) after[list[i]] = set = new();
                for (var j = 0; j < i; j++) set.Add(list[j]);
            }
        var placed = new List<string>();
        var left = lists.SelectMany(l => l).Distinct().ToList();   // as first seen
        while (left.Count > 0)
        {
            // (a cycle: two traces that disagree; the first left then)
            var next = left.FirstOrDefault(m => after[m].All(b => placed.Contains(b) || b == m || after[b].Contains(m))) ?? left[0];
            placed.Add(next);
            left.Remove(next);
        }
        return placed;
    }
}
