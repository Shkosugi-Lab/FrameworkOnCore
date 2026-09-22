using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WebForm2Blazor.Components;

// ASP.NET MVC (System.Web.Mvc) compatibility surface.
//
// WHY THIS EXISTS AT ALL, given the converter targets WebForms:
//
// Because real applications do not keep the two apart. n2cms builds ONE assembly, N2.dll,
// out of WebForms code and MVC code, and the WebForms half calls the MVC half -
// Web/UI/WebControls/ControlPanel.cs, a WebForms server control, constructs
// Mvc.Html.ControlPanelExtensions.ControlPanelHelper directly. DNN Platform does the same
// across 45 files, mojoPortal across 9.
//
// The converter used to answer that by EXCLUDING any file importing System.Web.Mvc, and
// then undoing the exclusion whenever a ported file named one of its types - handing the
// file back with its MVC types unresolved, and counting those errors separately because
// nothing in the converter could remove them. That worked while everything compiled as a
// single project: one compilation reports every error it finds, so the uncounted ones cost
// nothing but noise. It stops working the moment the output has more than one project,
// which is what --split-projects makes it have. MSBuild stops a branch at its first
// failure, so an uncounted-but-real error in a library means the library, the application
// and everything downstream are never compiled at all.
//
// So the MVC types are declared here instead, the way System.Web's are. The exclusion rule
// loses its System.Web.Mvc entry and those 133 files are ported like any others.
//
// WHAT THESE ARE AND ARE NOT:
//
// They are DECLARATIONS with the original shapes, so that code which mentions MVC types
// compiles. They are not an MVC runtime. Nothing here dispatches a request to a controller,
// resolves a view, or renders one: a converted application is Blazor Server, its pages are
// components reached by @page routes, and there is no MVC pipeline for a controller to be
// invoked from. Members whose whole purpose is that pipeline throw NotSupportedException
// rather than return a plausible default - the same rule the rest of the compatibility
// layer follows, because a silent no-op here would render an empty page and look like a
// conversion that merely lost some content.
//
// Shapes follow MVC 5 (System.Web.Mvc 5.2), which is what the corpora reference.

/// <summary>
/// System.Web.Mvc.MvcHtmlString equivalent - a string already encoded for HTML output.
///
/// Derives from the compatibility layer's HtmlString for the same reason the original
/// derives from System.Web.HtmlString: code passes one where an IHtmlString is expected,
/// and WebForms-side code does exactly that when it writes MVC helper output into a
/// control's markup.
/// </summary>
public class MvcHtmlString : HtmlString
{
    public MvcHtmlString(string value) : base(value)
    {
    }

    public static readonly MvcHtmlString Empty = new(string.Empty);

    public static MvcHtmlString Create(string value) => new(value ?? string.Empty);

    /// <summary>True when the value is null or empty, as the original's IsNullOrEmpty is.</summary>
    public static bool IsNullOrEmpty(MvcHtmlString value)
        => value is null || value.ToString().Length == 0;
}

/// <summary>How <see cref="TagBuilder"/> closes the tag it renders.</summary>
public enum TagRenderMode
{
    Normal,
    StartTag,
    EndTag,
    SelfClosing,
}

/// <summary>
/// System.Web.Mvc.TagBuilder equivalent.
///
/// Fully implemented, unlike most of this file: it is a string builder for one HTML
/// element with no dependency on the MVC pipeline at all, and ported code uses it to
/// PRODUCE markup that a WebForms control then writes. A stub here would silently empty
/// that markup.
/// </summary>
public class TagBuilder
{
    public TagBuilder(string tagName)
    {
        if (string.IsNullOrEmpty(tagName))
        {
            throw new ArgumentException("タグ名が空です。", nameof(tagName));
        }
        TagName = tagName;
    }

    public string TagName { get; }

    public IDictionary<string, string> Attributes { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public string InnerHtml { get; set; } = string.Empty;

    public string IdAttributeDotReplacement { get; set; } = "_";

    public void AddCssClass(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        // Prepended, as the original does, so the caller's later classes win the cascade.
        Attributes["class"] = Attributes.TryGetValue("class", out var existing) && existing.Length > 0
            ? value + " " + existing
            : value;
    }

    public void MergeAttribute(string key, string value) => MergeAttribute(key, value, replaceExisting: false);

    public void MergeAttribute(string key, string value, bool replaceExisting)
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        if (replaceExisting || !Attributes.ContainsKey(key))
        {
            Attributes[key] = value;
        }
    }

    public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes)
        => MergeAttributes(attributes, replaceExisting: false);

    public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes, bool replaceExisting)
    {
        foreach (var pair in attributes ?? new Dictionary<TKey, TValue>())
        {
            MergeAttribute(
                Convert.ToString(pair.Key, System.Globalization.CultureInfo.InvariantCulture),
                Convert.ToString(pair.Value, System.Globalization.CultureInfo.InvariantCulture),
                replaceExisting);
        }
    }

    public void SetInnerText(string innerText)
        => InnerHtml = System.Net.WebUtility.HtmlEncode(innerText ?? string.Empty);

    /// <summary>
    /// Sets the id attribute, dropping it when the value cannot be a valid id.
    /// The original does the same - a null id attribute is worse than none.
    /// </summary>
    public void GenerateId(string name)
    {
        if (string.IsNullOrEmpty(name) || Attributes.ContainsKey("id"))
        {
            return;
        }
        Attributes["id"] = name.Replace(".", IdAttributeDotReplacement, StringComparison.Ordinal);
    }

    public override string ToString() => ToString(TagRenderMode.Normal);

    public string ToString(TagRenderMode renderMode)
    {
        var attributes = string.Concat(Attributes.Select(pair =>
            $" {pair.Key}=\"{System.Net.WebUtility.HtmlEncode(pair.Value ?? string.Empty)}\""));

        return renderMode switch
        {
            TagRenderMode.StartTag => $"<{TagName}{attributes}>",
            TagRenderMode.EndTag => $"</{TagName}>",
            TagRenderMode.SelfClosing => $"<{TagName}{attributes} />",
            _ => $"<{TagName}{attributes}>{InnerHtml}</{TagName}>",
        };
    }
}

/// <summary>System.Web.Mvc.IViewDataContainer equivalent.</summary>
public interface IViewDataContainer
{
    ViewDataDictionary ViewData { get; set; }
}

/// <summary>
/// System.Web.Mvc.ViewDataDictionary equivalent.
///
/// Implemented rather than stubbed: it is a string-keyed bag plus a Model reference, with
/// no pipeline behind it, and ported code both fills it and reads it.
/// </summary>
public class ViewDataDictionary : IDictionary<string, object>
{
    private readonly Dictionary<string, object> _entries = new(StringComparer.OrdinalIgnoreCase);

    public ViewDataDictionary()
    {
    }

    public ViewDataDictionary(object model) => Model = model;

    public ViewDataDictionary(ViewDataDictionary source)
    {
        foreach (var pair in source ?? [])
        {
            _entries[pair.Key] = pair.Value;
        }
        Model = source?.Model;
        ModelState = source?.ModelState ?? new ModelStateDictionary();
    }

    public object Model { get; set; }

    public ModelStateDictionary ModelState { get; private set; } = new();

    /// <summary>Unknown keys return null rather than throwing, exactly as the original does.</summary>
    public object this[string key]
    {
        get => key is not null && _entries.TryGetValue(key, out var value) ? value : null;
        set => _entries[key] = value;
    }

    public ICollection<string> Keys => _entries.Keys;

    public ICollection<object> Values => _entries.Values;

    public int Count => _entries.Count;

    public bool IsReadOnly => false;

    public void Add(string key, object value) => _entries.Add(key, value);

    public void Add(KeyValuePair<string, object> item) => _entries.Add(item.Key, item.Value);

    public void Clear() => _entries.Clear();

    public bool Contains(KeyValuePair<string, object> item)
        => _entries.TryGetValue(item.Key, out var value) && Equals(value, item.Value);

    public bool ContainsKey(string key) => key is not null && _entries.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        => ((IDictionary<string, object>)_entries).CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _entries.GetEnumerator();

    public bool Remove(string key) => key is not null && _entries.Remove(key);

    public bool Remove(KeyValuePair<string, object> item) => _entries.Remove(item.Key);

    public bool TryGetValue(string key, out object value)
    {
        value = null;
        return key is not null && _entries.TryGetValue(key, out value);
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>System.Web.Mvc.ViewDataDictionary&lt;TModel&gt; equivalent.</summary>
public class ViewDataDictionary<TModel> : ViewDataDictionary
{
    public ViewDataDictionary()
    {
    }

    public ViewDataDictionary(TModel model) : base(model)
    {
    }

    public new TModel Model
    {
        get => base.Model is TModel typed ? typed : default;
        set => base.Model = value;
    }
}

/// <summary>System.Web.Mvc.TempDataDictionary equivalent (a per-request string-keyed bag).</summary>
public class TempDataDictionary : Dictionary<string, object>
{
    public TempDataDictionary() : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    public void Keep()
    {
    }

    public void Keep(string key)
    {
    }

    public object Peek(string key) => TryGetValue(key, out var value) ? value : null;
}

/// <summary>
/// System.Web.Mvc.ControllerContext equivalent - what a controller was told about the
/// request it is handling.
/// </summary>
public class ControllerContext
{
    public ControllerContext()
    {
    }

    public ControllerContext(RequestContext requestContext, ControllerBase controller)
    {
        RequestContext = requestContext;
        Controller = controller;
        HttpContext = requestContext?.HttpContext;
        RouteData = requestContext?.RouteData;
    }

    public ControllerContext(HttpContextBase httpContext, RouteData routeData, ControllerBase controller)
    {
        HttpContext = httpContext;
        RouteData = routeData;
        Controller = controller;
    }

    /// <summary>
    /// The view that invoked this controller as a CHILD action (Html.Action), or null.
    /// Always null here: there are no child actions (see HtmlHelper.Action).
    /// </summary>
    public ViewContext ParentActionViewContext { get; set; }

    public virtual HttpContextBase HttpContext { get; set; }

    public virtual RequestContext RequestContext { get; set; }

    public virtual RouteData RouteData { get; set; }

    public virtual ControllerBase Controller { get; set; }

    public bool IsChildAction { get; set; }
}

/// <summary>
/// System.Web.Mvc.ViewContext equivalent.
///
/// Carries the writer a view renders into, which is the one part of it WebForms-side code
/// genuinely uses: an MVC helper called from a server control is handed the control's own
/// HtmlTextWriter through here.
/// </summary>
public class ViewContext : ControllerContext
{
    public ViewContext()
    {
    }

    public ViewContext(ControllerContext controllerContext, IView view, ViewDataDictionary viewData, TempDataDictionary tempData, System.IO.TextWriter writer)
    {
        if (controllerContext is not null)
        {
            HttpContext = controllerContext.HttpContext;
            RequestContext = controllerContext.RequestContext;
            RouteData = controllerContext.RouteData;
            Controller = controllerContext.Controller;
        }
        View = view;
        ViewData = viewData;
        TempData = tempData;
        Writer = writer;
    }

    public IView View { get; set; }

    public ViewDataDictionary ViewData { get; set; } = new();

    public TempDataDictionary TempData { get; set; } = new();

    public System.IO.TextWriter Writer { get; set; }

    public bool ClientValidationEnabled { get; set; }

    public bool UnobtrusiveJavaScriptEnabled { get; set; }
}

/// <summary>
/// System.Web.Mvc.HtmlHelper equivalent.
///
/// The most-referenced MVC type in the corpora by a wide margin (99 of n2cms's 257
/// references). Almost all of that is EXTENSION METHODS the applications define themselves
/// - "public static MvcHtmlString Zone(this HtmlHelper html, ...)" - so what this has to
/// provide is the receiver and the few properties those extensions read off it.
/// </summary>
public class HtmlHelper
{
    public HtmlHelper()
    {
    }

    public HtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
    {
        ViewContext = viewContext;
        ViewDataContainer = viewDataContainer;
    }

    public HtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection)
        : this(viewContext, viewDataContainer)
        => RouteCollection = routeCollection;

    public ViewContext ViewContext { get; set; }

    public IViewDataContainer ViewDataContainer { get; set; }

    public RouteCollection RouteCollection { get; set; }

    public ViewDataDictionary ViewData
        => ViewDataContainer?.ViewData ?? ViewContext?.ViewData ?? new ViewDataDictionary();

    public dynamic ViewBag => ViewData;

    public static string IdAttributeDotReplacement { get; set; } = "_";

    /// <summary>Marks an already-encoded string, as the original's Raw does.</summary>
    public MvcHtmlString Raw(string value) => new(value ?? string.Empty);

    public MvcHtmlString Raw(object value)
        => new(value?.ToString() ?? string.Empty);

    public static string Encode(string value) => System.Net.WebUtility.HtmlEncode(value ?? string.Empty);

    public static string Encode(object value)
        => System.Net.WebUtility.HtmlEncode(value?.ToString() ?? string.Empty);

    public string AttributeEncode(string value) => Encode(value);

    public string AttributeEncode(object value) => Encode(value);
}

/// <summary>System.Web.Mvc.HtmlHelper&lt;TModel&gt; equivalent.</summary>
public class HtmlHelper<TModel> : HtmlHelper
{
    public HtmlHelper()
    {
    }

    public HtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
        : base(viewContext, viewDataContainer)
    {
    }

    public new ViewDataDictionary<TModel> ViewData { get; set; } = new();
}

/// <summary>
/// System.Web.Mvc.UrlHelper equivalent.
///
/// Builds URLs from route values. The compatibility layer already carries the routing
/// types the original builds them with (RouteCollection / RouteData / RequestContext), so
/// generation is delegated there rather than re-implemented - a URL that comes out
/// different from the original's is a broken link, not a compile error, and nothing would
/// report it.
/// </summary>
public class UrlHelper
{
    public UrlHelper()
    {
    }

    public UrlHelper(RequestContext requestContext) => RequestContext = requestContext;

    public UrlHelper(RequestContext requestContext, RouteCollection routeCollection)
        : this(requestContext)
        => RouteCollection = routeCollection;

    public RequestContext RequestContext { get; set; }

    public RouteCollection RouteCollection { get; set; }

    /// <summary>"~/x" -> "/x", which is what the original's Content does.</summary>
    public string Content(string contentPath)
        => string.IsNullOrEmpty(contentPath) ? contentPath
            : contentPath.StartsWith("~/", StringComparison.Ordinal) ? contentPath[1..]
            : contentPath;

    public string Encode(string url) => Uri.EscapeDataString(url ?? string.Empty);

    /// <summary>
    /// UrlHelper.Action - the URL of an action, generated from the route table. Delegated to
    /// the compatibility layer's own RouteCollection, so it produces what the routes the
    /// application registered produce (or null when none matches, as the original does).
    /// </summary>
    public string Action(string actionName) => Action(actionName, null, (RouteValueDictionary)null);

    public string Action(string actionName, object routeValues)
        => Action(actionName, null, new RouteValueDictionary(routeValues));

    public string Action(string actionName, RouteValueDictionary routeValues)
        => Action(actionName, null, routeValues);

    public string Action(string actionName, string controllerName)
        => Action(actionName, controllerName, (RouteValueDictionary)null);

    public string Action(string actionName, string controllerName, object routeValues)
        => Action(actionName, controllerName, new RouteValueDictionary(routeValues));

    public string Action(string actionName, string controllerName, RouteValueDictionary routeValues)
    {
        var values = routeValues is null ? new RouteValueDictionary() : new RouteValueDictionary(routeValues);
        values["action"] = actionName;
        if (controllerName is not null)
        {
            values["controller"] = controllerName;
        }
        var path = (RouteCollection ?? RouteTable.Routes).GetVirtualPath(RequestContext, values)?.VirtualPath;
        return path is null ? null : "/" + path.TrimStart('/');
    }

    public bool IsLocalUrl(string url)
        => !string.IsNullOrEmpty(url)
           && (url[0] == '/' ? url.Length == 1 || (url[1] != '/' && url[1] != '\\')
               : url.Length > 1 && url[0] == '~' && url[1] == '/');
}

/// <summary>
/// System.Web.Mvc.IController equivalent. Execute is the MVC pipeline's entry point into a
/// controller, and there is no pipeline here.
/// </summary>
public interface IController
{
    void Execute(RequestContext requestContext);
}

/// <summary>System.Web.Mvc.ControllerBase equivalent.</summary>
public abstract class ControllerBase : IController
{
    public ControllerContext ControllerContext { get; set; }

    public ViewDataDictionary ViewData { get; set; } = new();

    public TempDataDictionary TempData { get; set; } = new();

    public dynamic ViewBag => ViewData;

    public bool ValidateRequest { get; set; } = true;

    protected virtual void Execute(RequestContext requestContext)
        => throw new NotSupportedException(
            "MVC のコントローラ実行パイプラインは変換後のアプリケーションに存在しません"
            + "(ページは Blazor のコンポーネントとして @page ルートで到達します)。"
            + "この呼び出しは手動移行が必要です。");

    void IController.Execute(RequestContext requestContext) => Execute(requestContext);
}

/// <summary>
/// System.Web.Mvc.ActionResult equivalent. A result object is produced by ordinary
/// application code and only EXECUTED by the pipeline, so the type and its construction
/// are real and ExecuteResult is where the missing pipeline surfaces.
/// </summary>
public abstract class ActionResult
{
    public abstract void ExecuteResult(ControllerContext context);
}

/// <summary>System.Web.Mvc.EmptyResult equivalent.</summary>
public class EmptyResult : ActionResult
{
    public override void ExecuteResult(ControllerContext context)
    {
    }
}

/// <summary>System.Web.Mvc.ContentResult equivalent.</summary>
public class ContentResult : ActionResult
{
    public string Content { get; set; }

    public string ContentType { get; set; }

    public System.Text.Encoding ContentEncoding { get; set; }

    public override void ExecuteResult(ControllerContext context)
        => context?.HttpContext?.Response?.Write(Content ?? string.Empty);
}

/// <summary>System.Web.Mvc.RedirectResult equivalent.</summary>
public class RedirectResult : ActionResult
{
    public RedirectResult(string url) => Url = url;

    public RedirectResult(string url, bool permanent) : this(url) => Permanent = permanent;

    public string Url { get; }

    public bool Permanent { get; }

    public override void ExecuteResult(ControllerContext context)
        => context?.HttpContext?.Response?.Redirect(Url);
}

/// <summary>System.Web.Mvc.ViewResultBase equivalent.</summary>
public abstract class ViewResultBase : ActionResult
{
    public string ViewName { get; set; } = string.Empty;

    public string MasterName { get; set; } = string.Empty;

    public object Model => ViewData?.Model;

    public ViewDataDictionary ViewData { get; set; } = new();

    public TempDataDictionary TempData { get; set; } = new();

    public IView View { get; set; }

    public ViewEngineCollection ViewEngineCollection { get; set; }

    public override void ExecuteResult(ControllerContext context)
        => throw new NotSupportedException(
            "MVC のビュー描画は変換後のアプリケーションに存在しません"
            + "(ビューエンジンごと Blazor のコンポーネントに置き換わっています)。"
            + $"ビュー '{ViewName}' は手動移行が必要です。");
}

/// <summary>System.Web.Mvc.ViewResult equivalent.</summary>
public class ViewResult : ViewResultBase
{
}

/// <summary>System.Web.Mvc.PartialViewResult equivalent.</summary>
public class PartialViewResult : ViewResultBase
{
}

/// <summary>
/// System.Web.Mvc.Controller equivalent.
///
/// The View() family is kept because application code calls it to BUILD a result, which is
/// ordinary object construction. What cannot work is executing that result (see
/// ViewResultBase.ExecuteResult).
/// </summary>
public abstract class Controller : ControllerBase, IDisposable
{
    public HttpContextBase HttpContext => ControllerContext?.HttpContext;

    public RouteData RouteData => ControllerContext?.RouteData;

    public UrlHelper Url { get; set; } = new();

    public ModelStateDictionary ModelState => ViewData?.ModelState;

    protected internal ViewResult View() => View(null, null, null);

    protected internal ViewResult View(object model) => View(null, null, model);

    protected internal ViewResult View(string viewName) => View(viewName, null, null);

    protected internal ViewResult View(string viewName, object model) => View(viewName, null, model);

    protected internal virtual ViewResult View(string viewName, string masterName, object model)
    {
        if (model is not null)
        {
            ViewData.Model = model;
        }
        return new ViewResult
        {
            ViewName = viewName ?? string.Empty,
            MasterName = masterName ?? string.Empty,
            ViewData = ViewData,
            TempData = TempData,
        };
    }

    protected internal PartialViewResult PartialView() => PartialView(null, null);

    protected internal PartialViewResult PartialView(object model) => PartialView(null, model);

    protected internal PartialViewResult PartialView(string viewName) => PartialView(viewName, null);

    /// <summary>
    /// MVC Controller's own filter hooks. A controller overrides these (and calls base) to run
    /// code around its actions; nothing invokes them here, but the declarations must exist
    /// for the override to compile.
    /// </summary>
    protected virtual void OnActionExecuting(ActionExecutingContext filterContext)
    {
    }

    protected virtual void OnActionExecuted(ActionExecutedContext filterContext)
    {
    }

    protected virtual void OnResultExecuting(ResultExecutingContext filterContext)
    {
    }

    protected virtual void OnResultExecuted(ResultExecutedContext filterContext)
    {
    }

    /// <summary>The request's user, through the same HttpContext the rest of the layer uses.</summary>
    public System.Security.Principal.IPrincipal User => HttpContext?.User;

    public IActionInvoker ActionInvoker { get; set; }

    protected internal virtual PartialViewResult PartialView(string viewName, object model)
    {
        if (model is not null)
        {
            ViewData.Model = model;
        }
        return new PartialViewResult { ViewName = viewName ?? string.Empty, ViewData = ViewData, TempData = TempData };
    }

    protected internal virtual ContentResult Content(string content) => new() { Content = content };

    protected internal virtual RedirectResult Redirect(string url) => new(url);

    protected internal virtual EmptyResult Empty() => new();

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

/// <summary>System.Web.Mvc.IActionInvoker equivalent.</summary>
public interface IActionInvoker
{
    bool InvokeAction(ControllerContext controllerContext, string actionName);
}

/// <summary>System.Web.Mvc.IControllerFactory equivalent.</summary>
public interface IControllerFactory
{
    IController CreateController(RequestContext requestContext, string controllerName);

    void ReleaseController(IController controller);
}

/// <summary>
/// System.Web.Mvc.DefaultControllerFactory equivalent.
///
/// Applications subclass this to plug their own IoC container in - n2cms's
/// ServiceLocatingControllerFactory is exactly that - so the members they override have to
/// exist with the original signatures. Creating a controller by name needs the MVC type
/// cache, which does not exist here; the subclass's override normally replaces that anyway.
/// </summary>
public class DefaultControllerFactory : IControllerFactory
{
    public virtual IController CreateController(RequestContext requestContext, string controllerName)
    {
        var type = GetControllerType(requestContext, controllerName);
        return type is null
            ? throw new NotSupportedException(
                $"コントローラ '{controllerName}' を解決できません。"
                + "MVC のコントローラ探索は変換後のアプリケーションに存在しません。")
            : GetControllerInstance(requestContext, type);
    }

    protected internal virtual IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        => controllerType is null ? null : Activator.CreateInstance(controllerType) as IController;

    protected internal virtual Type GetControllerType(RequestContext requestContext, string controllerName) => null;

    public virtual void ReleaseController(IController controller) => (controller as IDisposable)?.Dispose();

    protected internal virtual SessionStateBehavior GetControllerSessionBehavior(
        RequestContext requestContext, Type controllerType)
        => SessionStateBehavior.Default;
}

/// <summary>
/// System.Web.SessionState.SessionStateBehavior equivalent.
///
/// Here rather than beside the other session types because this is the only thing that
/// asks for it: DefaultControllerFactory.GetControllerSessionBehavior returns one, and a
/// subclass overriding that method needs the return type to exist. The value decides how
/// the WebForms pipeline locks session state per request, which the converted application
/// has no equivalent of - the compat session is per circuit.
/// </summary>
public enum SessionStateBehavior
{
    Default,
    Required,
    ReadOnly,
    Disabled,
}

/// <summary>System.Web.Mvc.IView equivalent.</summary>
public interface IView
{
    void Render(ViewContext viewContext, System.IO.TextWriter writer);
}

/// <summary>System.Web.Mvc.IViewEngine equivalent.</summary>
public interface IViewEngine
{
    ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache);

    ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache);

    void ReleaseView(ControllerContext controllerContext, IView view);
}

/// <summary>System.Web.Mvc.ViewEngineResult equivalent (a found view, or where it was looked for).</summary>
public class ViewEngineResult
{
    public ViewEngineResult(IEnumerable<string> searchedLocations)
        => SearchedLocations = searchedLocations ?? [];

    public ViewEngineResult(IView view, IViewEngine viewEngine)
    {
        View = view;
        ViewEngine = viewEngine;
    }

    public IEnumerable<string> SearchedLocations { get; } = [];

    public IView View { get; }

    public IViewEngine ViewEngine { get; }
}

/// <summary>System.Web.Mvc.ViewEngineCollection equivalent.</summary>
public class ViewEngineCollection : Collection<IViewEngine>
{
    public ViewEngineCollection()
    {
    }

    public ViewEngineCollection(IList<IViewEngine> list) : base(list)
    {
    }

    public virtual ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName)
        => Find(engine => engine.FindView(controllerContext, viewName, masterName, useCache: false));

    public virtual ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName)
        => Find(engine => engine.FindPartialView(controllerContext, partialViewName, useCache: false));

    private ViewEngineResult Find(Func<IViewEngine, ViewEngineResult> probe)
    {
        var searched = new List<string>();
        foreach (var engine in this)
        {
            var result = probe(engine);
            if (result?.View is not null)
            {
                return result;
            }
            searched.AddRange(result?.SearchedLocations ?? []);
        }
        return new ViewEngineResult(searched);
    }
}

/// <summary>System.Web.Mvc.IViewLocationCache equivalent.</summary>
public interface IViewLocationCache
{
    string GetViewLocation(HttpContextBase httpContext, string key);

    void InsertViewLocation(HttpContextBase httpContext, string key, string virtualPath);
}

/// <summary>
/// System.Web.Mvc.VirtualPathProviderViewEngine equivalent.
///
/// Applications subclass it to change where views are looked for, so the location-format
/// properties they set in their constructor have to exist. Finding a view needs the
/// virtual path provider, which a converted application does not have.
/// </summary>
public abstract class VirtualPathProviderViewEngine : IViewEngine
{
    public string[] ViewLocationFormats { get; set; } = [];

    public string[] MasterLocationFormats { get; set; } = [];

    public string[] PartialViewLocationFormats { get; set; } = [];

    public string[] AreaViewLocationFormats { get; set; } = [];

    public string[] AreaMasterLocationFormats { get; set; } = [];

    public string[] AreaPartialViewLocationFormats { get; set; } = [];

    public string[] FileExtensions { get; set; } = [];

    public IViewLocationCache ViewLocationCache { get; set; }

    protected abstract IView CreatePartialView(ControllerContext controllerContext, string partialPath);

    protected abstract IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath);

    protected virtual bool FileExists(ControllerContext controllerContext, string virtualPath) => false;

    public virtual ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        => new(PartialViewLocationFormats);

    public virtual ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        => new(ViewLocationFormats);

    public virtual void ReleaseView(ControllerContext controllerContext, IView view) => (view as IDisposable)?.Dispose();
}

/// <summary>System.Web.Mvc.WebFormViewEngine equivalent (the .aspx-based view engine).</summary>
public class WebFormViewEngine : VirtualPathProviderViewEngine
{
    protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        => new WebFormView(controllerContext, partialPath, null);

    protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        => new WebFormView(controllerContext, viewPath, masterPath);
}

/// <summary>System.Web.Mvc.RazorViewEngine equivalent (the .cshtml-based view engine).</summary>
public class RazorViewEngine : VirtualPathProviderViewEngine
{
    protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        => new WebFormView(controllerContext, partialPath, null);

    protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        => new WebFormView(controllerContext, viewPath, masterPath);
}

/// <summary>
/// System.Web.Mvc.WebFormView equivalent - a view identified by a virtual path.
///
/// The path is kept and reported; rendering is not attempted. A converted application has
/// no .aspx or .cshtml left to render (they became components), so a view engine that
/// "succeeded" and wrote nothing would show an empty region instead of a failure.
/// </summary>
public class WebFormView : IView
{
    public WebFormView(ControllerContext controllerContext, string viewPath)
        : this(controllerContext, viewPath, null)
    {
    }

    public WebFormView(ControllerContext controllerContext, string viewPath, string masterPath)
    {
        ViewPath = viewPath;
        MasterPath = masterPath ?? string.Empty;
    }

    public string ViewPath { get; }

    public string MasterPath { get; }

    public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        => throw new NotSupportedException(
            $"ビュー '{ViewPath}' の描画は変換後のアプリケーションに存在しません"
            + "(ビューは Blazor のコンポーネントに置き換わっています)。手動移行が必要です。");
}

/// <summary>
/// System.Web.Mvc.DefaultViewLocationCache equivalent.
///
/// A no-op cache. The original caches view lookups for performance only, and nothing here
/// looks views up, so remembering nothing is the honest implementation rather than a lie.
/// </summary>
public class DefaultViewLocationCache : IViewLocationCache
{
    public static readonly IViewLocationCache Null = new DefaultViewLocationCache();

    public string GetViewLocation(HttpContextBase httpContext, string key) => null;

    public void InsertViewLocation(HttpContextBase httpContext, string key, string virtualPath)
    {
    }
}

/// <summary>
/// System.Web.Mvc.GlobalFilterCollection equivalent.
///
/// Applications register their global filters into this at startup, which is ordinary
/// object work and runs. Nothing invokes the filters afterwards - there is no action to
/// filter - so registration succeeding is not the same as the filter taking effect.
/// </summary>
public class GlobalFilterCollection : Collection<object>
{
    public void Add(object filter, int order) => Add(filter);
}

/// <summary>
/// System.Web.Mvc.ViewPage equivalent (a WebForms-engine MVC view).
///
/// Non-generic at the root, as in MVC, with ViewPage&lt;TModel&gt; deriving from it: ported
/// code instantiates the plain form ("new ViewPage()" to get a page to render a helper
/// into, as n2cms's HtmlHelperProvider does) as well as subclassing the typed one.
/// </summary>
public class ViewPage : Page, IViewDataContainer
{
    public ViewDataDictionary ViewData { get; set; } = new();

    public object Model => ViewData?.Model;

    public HtmlHelper<object> Html { get; set; }

    public UrlHelper Url { get; set; }

    public ViewContext ViewContext { get; set; }

    public TempDataDictionary TempData { get; set; } = new();

    /// <summary>
    /// The hook MVC's WebForms view engine calls to build Html / Url before rendering.
    /// Virtual because applications override it to add helpers of their own - n2cms's
    /// ContentViewPage does, and without the declaration that override is CS0115.
    /// </summary>
    public virtual void InitHelpers()
    {
        Html ??= new HtmlHelper<object>(ViewContext, this);
        Url ??= new UrlHelper(ViewContext?.RequestContext);
    }
}

/// <summary>System.Web.Mvc.ViewPage&lt;TModel&gt; equivalent.</summary>
public class ViewPage<TModel> : ViewPage
{
    public new TModel Model => ViewData?.Model is TModel typed ? typed : default;

    public new HtmlHelper<TModel> Html { get; set; }

    public override void InitHelpers()
    {
        base.InitHelpers();
        Html ??= new HtmlHelper<TModel>(ViewContext, this);
    }
}

/// <summary>System.Web.Mvc.ViewUserControl equivalent (a WebForms-engine partial view).</summary>
public class ViewUserControl : UserControl, IViewDataContainer
{
    public ViewDataDictionary ViewData { get; set; } = new();

    public object Model => ViewData?.Model;

    public HtmlHelper<object> Html { get; set; }

    public UrlHelper Url { get; set; }

    public ViewContext ViewContext { get; set; }

    public TempDataDictionary TempData { get; set; } = new();

    /// <summary>See <see cref="ViewPage.InitHelpers"/>.</summary>
    public virtual void InitHelpers()
    {
        Html ??= new HtmlHelper<object>(ViewContext, this);
        Url ??= new UrlHelper(ViewContext?.RequestContext);
    }
}

/// <summary>System.Web.Mvc.ViewUserControl&lt;TModel&gt; equivalent.</summary>
public class ViewUserControl<TModel> : ViewUserControl
{
    public new TModel Model => ViewData?.Model is TModel typed ? typed : default;

    public new HtmlHelper<TModel> Html { get; set; }

    public override void InitHelpers()
    {
        base.InitHelpers();
        Html ??= new HtmlHelper<TModel>(ViewContext, this);
    }
}

/// <summary>
/// System.Web.Mvc.ViewEngines equivalent - the process-wide view engine list.
///
/// A real collection that applications add their engines to at startup, as they did. It
/// stays empty unless they do, and looking a view up through it finds nothing: the
/// engines' FindView answers "searched these locations" rather than a view (see
/// VirtualPathProviderViewEngine), which is the truthful answer for an application whose
/// views became components.
/// </summary>
public static class ViewEngines
{
    public static ViewEngineCollection Engines { get; } = [];
}

/// <summary>
/// System.Web.Mvc.ExpressionHelper equivalent.
///
/// Fully implemented: it turns "m =&gt; m.Address.City" into "Address.City", a pure
/// computation over the expression tree with no pipeline behind it. n2cms builds its
/// editable-property registrations from exactly these strings, so a stub returning ""
/// would register every property under the same empty name.
/// </summary>
public static class ExpressionHelper
{
    public static string GetExpressionText(string expression)
        => string.Equals(expression, "model", StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : expression;

    public static string GetExpressionText(System.Linq.Expressions.LambdaExpression expression)
    {
        var names = new Stack<string>();
        var part = expression?.Body;
        while (part is not null)
        {
            if (part.NodeType == System.Linq.Expressions.ExpressionType.Convert
                || part.NodeType == System.Linq.Expressions.ExpressionType.ConvertChecked)
            {
                part = ((System.Linq.Expressions.UnaryExpression)part).Operand;
                continue;
            }
            if (part is System.Linq.Expressions.MemberExpression member)
            {
                names.Push(member.Member.Name);
                part = member.Expression;
                continue;
            }
            break;
        }

        // The lambda's own parameter is the model, not part of the name.
        return string.Join(".", names);
    }
}

/// <summary>System.Web.Mvc.HttpStatusCodeResult equivalent.</summary>
public class HttpStatusCodeResult : ActionResult
{
    public HttpStatusCodeResult(int statusCode) : this(statusCode, null)
    {
    }

    public HttpStatusCodeResult(int statusCode, string statusDescription)
    {
        StatusCode = statusCode;
        StatusDescription = statusDescription;
    }

    public int StatusCode { get; }

    public string StatusDescription { get; }

    public override void ExecuteResult(ControllerContext context)
    {
        if (context?.HttpContext?.Response is { } response)
        {
            response.StatusCode = StatusCode;
        }
    }
}

/// <summary>System.Web.Mvc.HttpUnauthorizedResult equivalent (401).</summary>
public class HttpUnauthorizedResult : HttpStatusCodeResult
{
    public HttpUnauthorizedResult() : base(401)
    {
    }

    public HttpUnauthorizedResult(string statusDescription) : base(401, statusDescription)
    {
    }
}

/// <summary>System.Web.Mvc.HttpNotFoundResult equivalent (404).</summary>
public class HttpNotFoundResult : HttpStatusCodeResult
{
    public HttpNotFoundResult() : base(404)
    {
    }

    public HttpNotFoundResult(string statusDescription) : base(404, statusDescription)
    {
    }
}

/// <summary>
/// System.Web.Mvc.MvcRouteHandler equivalent. A route handed one of these would dispatch to
/// an MVC controller; there is no MVC dispatch, so GetHttpHandler says so.
/// </summary>
public class MvcRouteHandler : IRouteHandler
{
    public MvcRouteHandler()
    {
    }

    public MvcRouteHandler(IControllerFactory controllerFactory) => ControllerFactory = controllerFactory;

    public IControllerFactory ControllerFactory { get; }

    public IHttpHandler GetHttpHandler(RequestContext requestContext)
        => throw new NotSupportedException(
            "MVC のルートからコントローラへのディスパッチは変換後のアプリケーションに存在しません。");
}

/// <summary>
/// System.Web.Mvc.ControllerBuilder equivalent - where an application installs its
/// controller factory at startup. Installing works; nothing ever asks it for a controller.
/// </summary>
public class ControllerBuilder
{
    private IControllerFactory _factory = new DefaultControllerFactory();

    public static ControllerBuilder Current { get; } = new();

    public HashSet<string> DefaultNamespaces { get; } = new(StringComparer.OrdinalIgnoreCase);

    public IControllerFactory GetControllerFactory() => _factory;

    public void SetControllerFactory(IControllerFactory controllerFactory)
        => _factory = controllerFactory ?? throw new ArgumentNullException(nameof(controllerFactory));

    public void SetControllerFactory(Type controllerFactoryType)
        => _factory = (IControllerFactory)Activator.CreateInstance(
            controllerFactoryType ?? throw new ArgumentNullException(nameof(controllerFactoryType)));
}

/// <summary>System.Web.Mvc.ActionDescriptor equivalent (what reflection found about one action).</summary>
public abstract class ActionDescriptor
{
    public abstract string ActionName { get; }

    public virtual object[] GetCustomAttributes(bool inherit) => [];

    public virtual object[] GetCustomAttributes(Type attributeType, bool inherit) => [];
}

/// <summary>System.Web.Mvc.ReflectedActionDescriptor equivalent.</summary>
public class ReflectedActionDescriptor : ActionDescriptor
{
    public ReflectedActionDescriptor(System.Reflection.MethodInfo methodInfo, string actionName)
    {
        MethodInfo = methodInfo;
        ActionName = actionName;
    }

    public System.Reflection.MethodInfo MethodInfo { get; }

    public override string ActionName { get; }

    public override object[] GetCustomAttributes(bool inherit)
        => MethodInfo?.GetCustomAttributes(inherit) ?? [];

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        => MethodInfo?.GetCustomAttributes(attributeType, inherit) ?? [];
}

/// <summary>
/// System.Web.Mvc.ReflectedControllerDescriptor equivalent.
///
/// Implemented, because it is reflection over the controller type and nothing more: n2cms
/// uses it to learn which actions a controller has so it can map content types to them.
/// The action set is the controller's public instance methods returning an ActionResult,
/// which is MVC's own rule.
/// </summary>
public class ReflectedControllerDescriptor
{
    public ReflectedControllerDescriptor(Type controllerType)
        => ControllerType = controllerType ?? throw new ArgumentNullException(nameof(controllerType));

    public Type ControllerType { get; }

    public string ControllerName
        => ControllerType.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
            ? ControllerType.Name[..^"Controller".Length]
            : ControllerType.Name;

    public ActionDescriptor[] GetCanonicalActions()
        => [.. ControllerType
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(method => !method.IsSpecialName
                             && typeof(ActionResult).IsAssignableFrom(method.ReturnType))
            .Select(method => new ReflectedActionDescriptor(method, method.Name))];

    public ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        => GetCanonicalActions().FirstOrDefault(action =>
            string.Equals(action.ActionName, actionName, StringComparison.OrdinalIgnoreCase));
}

/// <summary>
/// System.Web.Mvc.Html.ChildActionExtensions equivalent - Html.Action / Html.RenderAction,
/// which run ANOTHER controller action inside the current view.
///
/// That is the MVC pipeline in miniature, and there is none, so both throw. Returning an
/// empty string instead would render the page with a region silently missing - the one
/// outcome this layer is designed never to produce.
/// </summary>
public static class ChildActionExtensions
{
    private static NotSupportedException NoChildActions(string actionName)
        => new($"子アクション '{actionName}' の実行(Html.Action / RenderAction)は変換後の"
               + "アプリケーションに存在しません。その領域は Blazor のコンポーネントとして移行が必要です。");

    public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName)
        => throw NoChildActions(actionName);

    public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, object routeValues)
        => throw NoChildActions(actionName);

    public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, RouteValueDictionary routeValues)
        => throw NoChildActions(actionName);

    public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, string controllerName)
        => throw NoChildActions(actionName);

    public static MvcHtmlString Action(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues)
        => throw NoChildActions(actionName);

    public static void RenderAction(this HtmlHelper htmlHelper, string actionName)
        => throw NoChildActions(actionName);

    public static void RenderAction(this HtmlHelper htmlHelper, string actionName, object routeValues)
        => throw NoChildActions(actionName);

    public static void RenderAction(this HtmlHelper htmlHelper, string actionName, RouteValueDictionary routeValues)
        => throw NoChildActions(actionName);

    public static void RenderAction(this HtmlHelper htmlHelper, string actionName, string controllerName)
        => throw NoChildActions(actionName);

    public static void RenderAction(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues)
        => throw NoChildActions(actionName);
}
/// <summary>System.Web.Mvc.FilterAttribute equivalent.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public abstract class FilterAttribute : Attribute
{
    public int Order { get; set; } = -1;
}

/// <summary>System.Web.Mvc.ActionExecutingContext equivalent.</summary>
public class ActionExecutingContext : ControllerContext
{
    public ActionExecutingContext()
    {
    }

    public ActionExecutingContext(ControllerContext controllerContext, IDictionary<string, object> actionParameters)
    {
        if (controllerContext is not null)
        {
            HttpContext = controllerContext.HttpContext;
            RequestContext = controllerContext.RequestContext;
            RouteData = controllerContext.RouteData;
            Controller = controllerContext.Controller;
        }
        ActionParameters = actionParameters ?? new Dictionary<string, object>(StringComparer.Ordinal);
    }

    public IDictionary<string, object> ActionParameters { get; set; } =
        new Dictionary<string, object>(StringComparer.Ordinal);

    /// <summary>Setting this short-circuits the action, as it does in MVC.</summary>
    public ActionResult Result { get; set; }
}

/// <summary>System.Web.Mvc.ActionExecutedContext equivalent.</summary>
public class ActionExecutedContext : ControllerContext
{
    public bool Canceled { get; set; }

    public Exception Exception { get; set; }

    public bool ExceptionHandled { get; set; }

    public ActionResult Result { get; set; }
}

/// <summary>System.Web.Mvc.ResultExecutingContext equivalent.</summary>
public class ResultExecutingContext : ControllerContext
{
    public bool Cancel { get; set; }

    public ActionResult Result { get; set; }
}

/// <summary>System.Web.Mvc.ResultExecutedContext equivalent.</summary>
public class ResultExecutedContext : ControllerContext
{
    public bool Canceled { get; set; }

    public Exception Exception { get; set; }

    public bool ExceptionHandled { get; set; }

    public ActionResult Result { get; set; }
}

/// <summary>
/// System.Web.Mvc.ActionFilterAttribute equivalent.
///
/// Applications derive from this and override the four hooks. Nothing calls them here -
/// there is no action to filter - but the DECLARATIONS have to match or every subclass is
/// CS0115. Left as empty virtuals rather than throwing, because these are overridden, not
/// called: a throwing base would fire from any subclass that calls base.OnActionExecuting.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public abstract class ActionFilterAttribute : FilterAttribute
{
    public virtual void OnActionExecuting(ActionExecutingContext filterContext)
    {
    }

    public virtual void OnActionExecuted(ActionExecutedContext filterContext)
    {
    }

    public virtual void OnResultExecuting(ResultExecutingContext filterContext)
    {
    }

    public virtual void OnResultExecuted(ResultExecutedContext filterContext)
    {
    }
}

/// <summary>System.Web.Mvc.IRouteWithArea equivalent.</summary>
public interface IRouteWithArea
{
    string Area { get; }
}

/// <summary>
/// System.Web.Mvc.AreaRegistrationContext equivalent.
///
/// An area registers its routes through this. The compatibility layer carries the routing
/// types, so the registration runs and the route table is filled - what does not happen is
/// MVC dispatch through those routes.
/// </summary>
public class AreaRegistrationContext
{
    public AreaRegistrationContext(string areaName, RouteCollection routes)
    {
        AreaName = areaName;
        Routes = routes;
    }

    public string AreaName { get; }

    public RouteCollection Routes { get; }

    public ICollection<string> Namespaces { get; } = new List<string>();

    public object State { get; set; }
}
