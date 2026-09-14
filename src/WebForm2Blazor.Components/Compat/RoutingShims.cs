using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WebForm2Blazor.Components;

// System.Web.Routing.
//
// This namespace used to be on the Framework-only exclusion list, which cost 20 files
// across DNN, n2, mojoPortal and WingtipToys - plus everything that referenced their
// types. That was the same mistake System.Web.Compilation was: routing is not a separate
// framework the way MVC is, it is a small set of data carriers plus a URL pattern
// matcher, and the applications use it mostly to BUILD urls (RouteValueDictionary and
// Route.GetVirtualPath account for most of the references in the corpora) rather than to
// dispatch requests.
//
// So these are real implementations, not empty declarations: a route registered here
// matches and generates the same URLs it did on 4.8. What does NOT happen is dispatch -
// an IRouteHandler returns an IHttpHandler and handlers do not run in Blazor, where a
// converted page is reached by its @page route. Code that asks a route for a path gets
// the right answer; code that expected the routing module to serve a request does not,
// and that is reported separately.

/// <summary>
/// System.Web.Routing.RouteValueDictionary: a case-insensitive string→object dictionary.
/// Also constructible from an anonymous object ("new RouteValueDictionary(new { id = 3 })"),
/// which is how almost every call site in the corpora writes it.
/// </summary>
public class RouteValueDictionary : IDictionary<string, object>
{
    private readonly Dictionary<string, object> _values =
        new(StringComparer.OrdinalIgnoreCase);

    public RouteValueDictionary()
    {
    }

    public RouteValueDictionary(object values)
    {
        switch (values)
        {
            case null:
                break;
            case IDictionary<string, object> typed:
                foreach (var pair in typed)
                {
                    _values[pair.Key] = pair.Value;
                }
                break;
            case IDictionary untyped:
                foreach (DictionaryEntry entry in untyped)
                {
                    _values[System.Convert.ToString(entry.Key) ?? string.Empty] = entry.Value;
                }
                break;
            default:
                // An anonymous type - each readable property becomes an entry, which is
                // exactly what System.Web did via TypeDescriptor.
                foreach (var property in values.GetType().GetProperties())
                {
                    if (property.CanRead && property.GetIndexParameters().Length == 0)
                    {
                        _values[property.Name] = property.GetValue(values);
                    }
                }
                break;
        }
    }

    public RouteValueDictionary(IDictionary<string, object> dictionary)
        : this((object)dictionary)
    {
    }

    /// <summary>Missing keys read as null rather than throwing - the System.Web behaviour.</summary>
    public object this[string key]
    {
        get => key is not null && _values.TryGetValue(key, out var value) ? value : null;
        set => _values[key] = value;
    }

    public ICollection<string> Keys => _values.Keys;

    public ICollection<object> Values => _values.Values;

    public int Count => _values.Count;

    public bool IsReadOnly => false;

    public void Add(string key, object value) => _values.Add(key, value);

    public void Add(KeyValuePair<string, object> item) => _values.Add(item.Key, item.Value);

    public void Clear() => _values.Clear();

    public bool Contains(KeyValuePair<string, object> item)
        => _values.TryGetValue(item.Key, out var value) && Equals(value, item.Value);

    public bool ContainsKey(string key) => key is not null && _values.ContainsKey(key);

    public bool ContainsValue(object value) => _values.ContainsValue(value);

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        => ((IDictionary<string, object>)_values).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

    public bool Remove(string key) => key is not null && _values.Remove(key);

    public bool Remove(KeyValuePair<string, object> item) => Remove(item.Key);

    public bool TryGetValue(string key, out object value)
    {
        value = null;
        return key is not null && _values.TryGetValue(key, out value);
    }
}

/// <summary>System.Web.Routing.RouteDirection.</summary>
public enum RouteDirection
{
    IncomingRequest,
    UrlGeneration,
}

/// <summary>
/// System.Web.Routing.IHttpHandler-producing handler. Handlers do not execute here
/// (a converted page is reached by its @page route), but the interface has to exist
/// because applications declare classes that implement it.
/// </summary>
public interface IRouteHandler
{
    IHttpHandler GetHttpHandler(RequestContext requestContext);
}

/// <summary>System.Web.Routing.StopRoutingHandler: marks a URL as "not routed".</summary>
public class StopRoutingHandler : IRouteHandler
{
    public virtual IHttpHandler GetHttpHandler(RequestContext requestContext) => null;
}

/// <summary>System.Web.Routing.PageRouteHandler: routes a URL to a .aspx virtual path.</summary>
public class PageRouteHandler : IRouteHandler
{
    public PageRouteHandler(string virtualPath)
        : this(virtualPath, true)
    {
    }

    public PageRouteHandler(string virtualPath, bool checkPhysicalUrlAccess)
    {
        VirtualPath = virtualPath;
        CheckPhysicalUrlAccess = checkPhysicalUrlAccess;
    }

    public string VirtualPath { get; }

    public bool CheckPhysicalUrlAccess { get; }

    public virtual IHttpHandler GetHttpHandler(RequestContext requestContext) => null;
}

/// <summary>System.Web.Routing.IRouteConstraint.</summary>
public interface IRouteConstraint
{
    bool Match(HttpContextBase httpContext, Route route, string parameterName,
        RouteValueDictionary values, RouteDirection routeDirection);
}

/// <summary>System.Web.Routing.RouteData: what a matched route produced.</summary>
public class RouteData
{
    public RouteData()
    {
    }

    public RouteData(RouteBase route, IRouteHandler routeHandler)
    {
        Route = route;
        RouteHandler = routeHandler;
    }

    public RouteBase Route { get; set; }

    public IRouteHandler RouteHandler { get; set; }

    public RouteValueDictionary Values { get; } = [];

    public RouteValueDictionary DataTokens { get; } = [];

    public string GetRequiredString(string valueName)
        => Values[valueName] as string is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException(
                $"ルート値 '{valueName}' がありません(RouteData.GetRequiredString)。");
}

/// <summary>System.Web.Routing.RequestContext: the context a route was matched in.</summary>
public class RequestContext
{
    public RequestContext()
    {
    }

    public RequestContext(HttpContextBase httpContext, RouteData routeData)
    {
        HttpContext = httpContext;
        RouteData = routeData;
    }

    public virtual HttpContextBase HttpContext { get; set; }

    public virtual RouteData RouteData { get; set; }
}

/// <summary>System.Web.Routing.VirtualPathData: the URL a route generated.</summary>
public class VirtualPathData
{
    public VirtualPathData(RouteBase route, string virtualPath)
    {
        Route = route;
        VirtualPath = virtualPath ?? string.Empty;
    }

    public RouteBase Route { get; set; }

    public string VirtualPath { get; set; }

    public RouteValueDictionary DataTokens { get; } = [];
}

/// <summary>System.Web.Routing.RouteBase.</summary>
public abstract class RouteBase
{
    public abstract RouteData GetRouteData(HttpContextBase httpContext);

    public abstract VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values);
}

/// <summary>
/// System.Web.Routing.Route: a "{controller}/{action}/{id}" pattern with defaults and
/// constraints. Both directions are implemented, because applications use both: n2 and
/// DNN match incoming paths AND generate links from route values.
/// </summary>
public class Route : RouteBase
{
    public Route(string url, IRouteHandler routeHandler)
        : this(url, null, null, null, routeHandler)
    {
    }

    public Route(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
        : this(url, defaults, null, null, routeHandler)
    {
    }

    public Route(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
        IRouteHandler routeHandler)
        : this(url, defaults, constraints, null, routeHandler)
    {
    }

    public Route(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
        RouteValueDictionary dataTokens, IRouteHandler routeHandler)
    {
        Url = url;
        Defaults = defaults;
        Constraints = constraints;
        DataTokens = dataTokens;
        RouteHandler = routeHandler;
    }

    public string Url { get; set; }

    public RouteValueDictionary Defaults { get; set; }

    public RouteValueDictionary Constraints { get; set; }

    public RouteValueDictionary DataTokens { get; set; }

    public IRouteHandler RouteHandler { get; set; }

    private string[] Segments
        => (Url ?? string.Empty).Trim('~', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);

    public override RouteData GetRouteData(HttpContextBase httpContext)
    {
        var path = httpContext?.Request?.Path ?? string.Empty;
        return Match(path);
    }

    /// <summary>Matching split out so it can be used without a live request.</summary>
    public RouteData Match(string path)
    {
        var actual = (path ?? string.Empty).Split('?')[0].Trim('/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
        var pattern = Segments;

        var data = new RouteData(this, RouteHandler);

        for (var index = 0; index < pattern.Length; index++)
        {
            var segment = pattern[index];
            var isParameter = segment.StartsWith('{') && segment.EndsWith('}');

            // A catch-all ("{*path}") swallows the rest, including slashes.
            if (isParameter && segment.StartsWith("{*", StringComparison.Ordinal))
            {
                data.Values[segment[2..^1]] = index < actual.Length
                    ? string.Join('/', actual.Skip(index))
                    : Defaults?[segment[2..^1]];
                ApplyRemainingDefaults(data);
                return data;
            }

            if (index >= actual.Length)
            {
                // A missing segment is only allowed when the pattern supplies a default.
                if (!isParameter || Defaults?[segment[1..^1]] is null)
                {
                    return null;
                }
                data.Values[segment[1..^1]] = Defaults[segment[1..^1]];
                continue;
            }

            if (isParameter)
            {
                data.Values[segment[1..^1]] = Uri.UnescapeDataString(actual[index]);
            }
            else if (!segment.Equals(actual[index], StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
        }

        // Extra segments the pattern does not cover mean this is a different URL.
        if (actual.Length > pattern.Length)
        {
            return null;
        }

        ApplyRemainingDefaults(data);
        return data;
    }

    private void ApplyRemainingDefaults(RouteData data)
    {
        foreach (var pair in Defaults ?? [])
        {
            if (!data.Values.ContainsKey(pair.Key))
            {
                data.Values[pair.Key] = pair.Value;
            }
        }
        foreach (var pair in DataTokens ?? [])
        {
            data.DataTokens[pair.Key] = pair.Value;
        }
    }

    public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
    {
        var supplied = new RouteValueDictionary();
        foreach (var pair in requestContext?.RouteData?.Values ?? [])
        {
            supplied[pair.Key] = pair.Value;
        }
        foreach (var pair in values ?? [])
        {
            supplied[pair.Key] = pair.Value;
        }

        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var path = new StringBuilder();

        foreach (var segment in Segments)
        {
            var isParameter = segment.StartsWith('{') && segment.EndsWith('}');
            string text;
            if (isParameter)
            {
                var name = segment.StartsWith("{*", StringComparison.Ordinal)
                    ? segment[2..^1]
                    : segment[1..^1];
                var value = supplied[name] ?? Defaults?[name];
                if (value is null)
                {
                    // A parameter with nothing to put in it cannot produce this URL.
                    return null;
                }
                used.Add(name);
                text = Uri.EscapeDataString(System.Convert.ToString(value) ?? string.Empty);
            }
            else
            {
                text = segment;
            }

            if (path.Length > 0)
            {
                path.Append('/');
            }
            path.Append(text);
        }

        // Whatever the pattern did not consume becomes the query string, which is what
        // System.Web did.
        var extras = (values ?? [])
            .Where(pair => !used.Contains(pair.Key) && pair.Value is not null)
            .ToList();
        if (extras.Count > 0)
        {
            path.Append('?').Append(string.Join('&', extras.Select(pair =>
                Uri.EscapeDataString(pair.Key) + "="
                + Uri.EscapeDataString(System.Convert.ToString(pair.Value) ?? string.Empty))));
        }

        return new VirtualPathData(this, path.ToString());
    }
}

/// <summary>System.Web.Routing.RouteCollection.</summary>
public class RouteCollection : Collection<RouteBase>
{
    private readonly Dictionary<string, RouteBase> _named = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// WebForms RouteExistingFiles. Kept as state because applications read it back;
    /// nothing dispatches on it here.
    /// </summary>
    public bool RouteExistingFiles { get; set; }

    public bool AppendTrailingSlash { get; set; }

    public bool LowercaseUrls { get; set; }

    public RouteBase this[string name]
        => name is not null && _named.TryGetValue(name, out var route) ? route : null;

    public void Add(string name, RouteBase item)
    {
        if (!string.IsNullOrEmpty(name))
        {
            _named[name] = item;
        }
        Add(item);
    }

    public void Ignore(string url) => Ignore(url, null);

    public void Ignore(string url, object constraints)
        => Add(new Route(url, null, new RouteValueDictionary(constraints), null, new StopRoutingHandler()));

    public Route MapPageRoute(string routeName, string routeUrl, string physicalFile)
        => MapPageRoute(routeName, routeUrl, physicalFile, true, null, null, null);

    public Route MapPageRoute(string routeName, string routeUrl, string physicalFile,
        bool checkPhysicalUrlAccess)
        => MapPageRoute(routeName, routeUrl, physicalFile, checkPhysicalUrlAccess, null, null, null);

    public Route MapPageRoute(string routeName, string routeUrl, string physicalFile,
        bool checkPhysicalUrlAccess, RouteValueDictionary defaults)
        => MapPageRoute(routeName, routeUrl, physicalFile, checkPhysicalUrlAccess, defaults, null, null);

    public Route MapPageRoute(string routeName, string routeUrl, string physicalFile,
        bool checkPhysicalUrlAccess, RouteValueDictionary defaults, RouteValueDictionary constraints)
        => MapPageRoute(routeName, routeUrl, physicalFile, checkPhysicalUrlAccess, defaults, constraints, null);

    public Route MapPageRoute(string routeName, string routeUrl, string physicalFile,
        bool checkPhysicalUrlAccess, RouteValueDictionary defaults, RouteValueDictionary constraints,
        RouteValueDictionary dataTokens)
    {
        var route = new Route(routeUrl, defaults, constraints, dataTokens,
            new PageRouteHandler(physicalFile, checkPhysicalUrlAccess));
        Add(routeName, route);
        return route;
    }

    public RouteData GetRouteData(HttpContextBase httpContext)
    {
        foreach (var route in this)
        {
            if (route.GetRouteData(httpContext) is { } data)
            {
                return data;
            }
        }
        return null;
    }

    public VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        => GetVirtualPath(requestContext, null, values);

    public VirtualPathData GetVirtualPath(RequestContext requestContext, string name,
        RouteValueDictionary values)
    {
        if (!string.IsNullOrEmpty(name))
        {
            return this[name]?.GetVirtualPath(requestContext, values);
        }

        foreach (var route in this)
        {
            if (route.GetVirtualPath(requestContext, values) is { } path)
            {
                return path;
            }
        }
        return null;
    }

    /// <summary>WebForms read/write locks. Nothing here needs them; they keep using blocks compiling.</summary>
    public IDisposable GetReadLock() => NullLock.Instance;

    public IDisposable GetWriteLock() => NullLock.Instance;

    private sealed class NullLock : IDisposable
    {
        public static readonly NullLock Instance = new();

        public void Dispose()
        {
        }
    }
}

/// <summary>System.Web.Routing.RouteTable: the application-wide route collection.</summary>
public static class RouteTable
{
    public static RouteCollection Routes { get; } = [];
}

/// <summary>
/// System.Web.Routing.UrlRoutingModule. The module never runs - ASP.NET Core has its own
/// pipeline - but applications name the type in Web.config wiring and in code.
/// </summary>
public class UrlRoutingModule
{
    public RouteCollection RouteCollection { get; set; } = RouteTable.Routes;

    public virtual void Dispose()
    {
    }

    public virtual void Init(object application)
    {
    }
}
