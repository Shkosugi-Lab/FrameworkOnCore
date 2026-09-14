using System;

namespace WebForm2Blazor.Components;

// System.Web.Services (ASMX) and System.Web.Script.Services.
//
// This namespace used to be on the Framework-only exclusion list, which emptied 12 files
// in mojoPortal. What the corpora actually use of it is tiny - WebMethod 15 references,
// WebService 13, WsiProfiles 12, WebServiceBinding 12, ScriptService 1 - and all of it
// is declarations: a base class and four attributes.
//
// An ASMX endpoint is not served here, the same way an IHttpHandler is not; SOAP is a
// different protocol surface and porting it is the application's decision, reported
// separately. But a [WebMethod] is an ordinary public method on an ordinary class, and
// the classes around it hold logic the rest of the application calls directly. Excluding
// the file threw that away to avoid five declarations.

/// <summary>
/// System.Web.Services.WebService equivalent: the base class an .asmx code-behind derives
/// from. Its members are the same request-context accessors a Page has.
/// </summary>
public class WebService : MarshalByRefObject, IDisposable
{
    public HttpContext Context => HttpContext.Current;

    public WebFormsSession Session => Context?.Session;

    // Application state is a singleton service rather than a member of the context shim,
    // so it is resolved the same way WebFormsPage's [Inject] does.
    public WebFormsApplicationState Application
        => HttpContext.Services?.GetService(typeof(WebFormsApplicationState)) as WebFormsApplicationState;

    public ServerUtilityShim Server => Context?.Server;

    public System.Security.Principal.IPrincipal User => Context?.User;

    /// <summary>System.Web.Services.WebService.SoapVersion is not modelled; SOAP is not served.</summary>
    public void Dispose() => Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
    }
}

/// <summary>System.Web.Services.WebMethodAttribute.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class WebMethodAttribute : Attribute
{
    public WebMethodAttribute()
    {
    }

    public WebMethodAttribute(bool enableSession) => EnableSession = enableSession;

    public WebMethodAttribute(bool enableSession, TransactionOptionShim transactionOption)
    {
        EnableSession = enableSession;
        TransactionOption = transactionOption;
    }

    public bool EnableSession { get; set; }

    public int CacheDuration { get; set; }

    public bool BufferResponse { get; set; } = true;

    public string Description { get; set; }

    public string MessageName { get; set; }

    public TransactionOptionShim TransactionOption { get; set; }
}

/// <summary>
/// System.EnterpriseServices.TransactionOption equivalent - named by WebMethodAttribute's
/// three-argument form. Suffixed because nothing distributed-transactional runs here.
/// </summary>
public enum TransactionOptionShim
{
    Disabled,
    NotSupported,
    Supported,
    Required,
    RequiresNew,
}

/// <summary>System.Web.Services.WebServiceAttribute.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class WebServiceAttribute : Attribute
{
    public const string DefaultNamespace = "http://tempuri.org/";

    public string Namespace { get; set; } = DefaultNamespace;

    public string Description { get; set; }

    public string Name { get; set; }
}

/// <summary>System.Web.Services.WsiProfiles.</summary>
[Flags]
public enum WsiProfiles
{
    None = 0,
    BasicProfile1_1 = 1,
}

/// <summary>System.Web.Services.WebServiceBindingAttribute.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class WebServiceBindingAttribute : Attribute
{
    public WebServiceBindingAttribute()
    {
    }

    public WebServiceBindingAttribute(string name) => Name = name;

    public string Name { get; set; }

    public string Namespace { get; set; }

    public string Location { get; set; }

    public WsiProfiles ConformsTo { get; set; }

    public bool EmitConformanceClaims { get; set; }
}

/// <summary>
/// System.Web.Script.Services.ScriptServiceAttribute - marked an ASMX service callable
/// from the ASP.NET AJAX client proxy.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ScriptServiceAttribute : Attribute
{
}

/// <summary>System.Web.Script.Services.ScriptMethodAttribute.</summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ScriptMethodAttribute : Attribute
{
    public ResponseFormat ResponseFormat { get; set; }

    public bool UseHttpGet { get; set; }

    public bool XmlSerializeString { get; set; }
}

/// <summary>System.Web.Script.Services.ResponseFormat.</summary>
public enum ResponseFormat
{
    Json,
    Xml,
}
