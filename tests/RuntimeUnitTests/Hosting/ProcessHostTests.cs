using System.Runtime.InteropServices;
using System.Web.Hosting;
using static FrameworkOnCore.Runtime.Tests.Hosting.Child;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The process host (ProcessHost: IIS integrated mode's host of the worker process, called by IIS's webengine4.dll;
/// referencesource System.Web/Hosting/ProcessHost.cs, "reference source" below): its arguments, the process protocol
/// handlers, the applications it lists, its ping and suspend, the application's preload (IProcessHostPreloadClient,
/// learn.microsoft.com/dotnet/api/system.web.hosting.iprocesshostpreloadclient) and the custom loader's failure. One
/// per process with the application it starts: each scenario in a process of its own (Child). What needs IIS's native
/// webengine4.dll (the IIS application host, the application pool's state) is not reached.
/// </summary>
// The CodeDom collection: ExecutorPortable's output files, inheritable while its process starts, would be inherited by
// this class's processes started meanwhile (Windows), and held open.
[Collection("CodeDom")]
public class ProcessHostTests
{
    // ---- the child's side

    /// <summary>IIS's support functions: the application at the folder, "/" of the site 1.</summary>
    public sealed class Functions : IProcessHostSupportFunctions
    {
        readonly string root;
        public Functions(string root) => this.root = root;
        public void GetApplicationProperties(string appId, out string virtualPath, out string physicalPath, out string siteName, out string siteId)
        { virtualPath = "/"; physicalPath = root; siteName = "Default Web Site"; siteId = "1"; }
        public void MapPath(string appId, string virtualPath, out string physicalPath) => physicalPath = Path.Combine(root, virtualPath.TrimStart('/'));
        public IntPtr GetConfigToken(string appId) => IntPtr.Zero;
        public string GetAppHostConfigFilename() => null;
        public string GetRootWebConfigFilename() => null;
        public IntPtr GetNativeConfigurationSystem() => IntPtr.Zero;
    }

    /// <summary>IIS's preload settings of the application: enabled, the client's type.</summary>
    public sealed class PreloadUtil : IApplicationPreloadUtil
    {
        public bool Enabled;
        public string Type;
        public void GetApplicationPreloadInfo(string context, out bool enabled, out string startupObjType, out string[] parametersForStartupObj)
        {
            enabled = Enabled; startupObjType = Type; parametersForStartupObj = new[] { "p1", "p2" };
        }
        public void ReportApplicationPreloadFailure(string context, int errorCode, string errorMessage) =>
            Say("preload.failure", context + " " + errorCode.ToString("x") + " " + errorMessage.Contains("Throwing"));
    }

    public sealed class PreloadClient : IProcessHostPreloadClient
    {
        public void Preload(string[] parameters) => Say("preload.client", string.Join(",", parameters) + " " + HostingEnvironment.IsHosted);
    }

    public sealed class ThrowingClient : IProcessHostPreloadClient
    {
        public void Preload(string[] parameters) => throw new InvalidOperationException("Throwing");
    }

    sealed class Callback : IListenerChannelCallback
    {
        public void ReportStarted() { }
        public void ReportStopped(int hr) { }
        public void ReportMessageReceived() { }
        public int GetId() => 7;
        public int GetBlobLength() => 0;
        public void GetBlob(byte[] buffer, ref int bufferSize) { }
    }

    public sealed class Handler : AppDomainProtocolHandler
    {
        public override void StartListenerChannel(IListenerChannelCallback listenerChannelCallback) => Say("handler.started", listenerChannelCallback.GetId());
        public override void StopListenerChannel(int listenerChannelId, bool immediate) => Say("handler.stopChannel", listenerChannelId + " " + immediate);
        public override void StopProtocol(bool immediate) => Say("handler.stopProtocol", immediate);
    }

    /// <summary>The process protocol handler <protocols> names.</summary>
    public sealed class ProcessHandler : ProcessProtocolHandler
    {
        static int starts;
        public override void StartListenerChannel(IListenerChannelCallback listenerChannelCallback, IAdphManager AdphManager) =>
            Say("process.handler.start", ++starts + " " + listenerChannelCallback.GetId() + " " + ReferenceEquals(AdphManager, ProcessHost.DefaultHost));
        public override void StopListenerChannel(int listenerChannelId, bool immediate) => Say("process.handler.stopChannel", listenerChannelId + " " + immediate);
        public override void StopProtocol(bool immediate) => Say("process.handler.stopProtocol." + immediate, true);
    }

    /// <summary>A custom runtime (Helios') that IIS's suspend reaches.</summary>
    sealed class CustomRuntime : ICustomRuntime, IProcessSuspendListener, IProcessResumeCallback
    {
        public void Start(IntPtr reserved0, int reserved1) { }
        public void ResolveModules(IntPtr pResolveModuleData, int resolveModuleDataSize) { }
        public void Stop(IntPtr reserved0, int reserved1) { }
        public IProcessResumeCallback Suspend() { Say("customRuntime.suspended", true); return this; }
        public void Resume() => Say("customRuntime.resumed", true);
    }

    sealed class Ping : IProcessPingCallback
    {
        public readonly ManualResetEventSlim Responded = new();
        public void Respond() => Responded.Set();
    }

    const string AppId = "/LM/W3SVC/1/ROOT";

    static int Calls(string[] args)
    {
        var root = args[0];
        var functions = new Functions(root);
        var host = ProcessHost.GetProcessHost(functions);
        Say("singleton", ReferenceEquals(host, ProcessHost.GetProcessHost(new Functions(root))) && ReferenceEquals(host, ProcessHost.DefaultHost));
        Say("functions", ReferenceEquals(functions, host.SupportFunctions));

        Throws("null.StartProcessProtocolListenerChannel", () => host.StartProcessProtocolListenerChannel(null, null));
        Throws("null.StopProcessProtocolListenerChannel", () => host.StopProcessProtocolListenerChannel(null, 1, false));
        Throws("null.StopProcessProtocol", () => host.StopProcessProtocol(null, false));
        Throws("null.StartAppDomainProtocolListenerChannel.appId", () => host.StartAppDomainProtocolListenerChannel(null, "p", null));
        Throws("null.StartAppDomainProtocolListenerChannel.protocolId", () => host.StartAppDomainProtocolListenerChannel(AppId, null, null));
        Throws("null.StopAppDomainProtocolListenerChannel.appId", () => host.StopAppDomainProtocolListenerChannel(null, "p", 1, false));
        Throws("null.StopAppDomainProtocolListenerChannel.protocolId", () => host.StopAppDomainProtocolListenerChannel(AppId, null, 1, false));
        Throws("null.StopAppDomainProtocol.appId", () => host.StopAppDomainProtocol(null, "p", false));
        Throws("null.StopAppDomainProtocol.protocolId", () => host.StopAppDomainProtocol(AppId, null, false));
        Throws("null.StartApplication.appId", () => host.StartApplication(null, root, out _));
        Throws("null.StartApplication.appPath", () => host.StartApplication(AppId, null, out _));

        // The preload state's checks.
        Throws("preload.context.empty", () => host.SetApplicationPreloadState("", AppId, false));
        Throws("preload.appId.empty", () => host.SetApplicationPreloadState("ctx", null, false));
        Throws("preload.enabled.noUtil", () => host.SetApplicationPreloadState("ctx", AppId, true));
        var util = new PreloadUtil();
        host.SetApplicationPreloadUtil(util);
        Throws("preload.util.again", () => host.SetApplicationPreloadUtil(util));
        Throws("preload.notEnabled", () => host.SetApplicationPreloadState("ctx", AppId, true));
        util.Enabled = true;
        Throws("preload.noType", () => host.SetApplicationPreloadState("ctx", AppId, true));

        // The custom loader (bin\AspNet.Loader.dll): none.
        var lite = (IProcessHostLite)host;
        Say("customLoader.none", lite.GetCustomLoader(AppId, "MACHINE/WEBROOT/APPHOST/Default Web Site", out var support, out _) == null && support == null);
        Say("fullMessage", lite.GetFullExceptionMessage(unchecked((int)0x80004005), IntPtr.Zero).Contains("0x80004005"));
        Say("fullMessage.success", lite.GetFullExceptionMessage(0, IntPtr.Zero) == "");
        Say("enumerate.before", Enumerate(host).Count());

        // The application preloaded (IIS's auto-start): its client called in it, with IIS's parameters.
        util.Type = typeof(PreloadClient).AssemblyQualifiedName;
        var manager = ApplicationManager.GetApplicationManager();
        host.PreloadApplicationIfRequired(AppId, new ApplicationManagerTests.SiteHost(root), null, manager.GetLockableAppDomainContext(AppId));
        Say("preloaded.running", manager.GetRunningApplications().Length);
        // Preloaded already: not again.
        host.PreloadApplicationIfRequired(AppId, null, null, manager.GetLockableAppDomainContext(AppId));

        // (After the application: what reads the configuration before it would start HttpRuntime without the
        // application's paths; .NET Framework's application has an AppDomain of its own.)
        // No process protocol handler started: nothing to stop.
        Throws("unknown.StopProcessProtocolListenerChannel", () => host.StopProcessProtocolListenerChannel("none", 1, false));
        Throws("unknown.StopProcessProtocol", () => host.StopProcessProtocol("none", false));
        // An unknown protocol's application handler: no type (reported), so GetObject's argument check.
        Throws("unknown.StopAppDomainProtocol", () => host.StopAppDomainProtocol(AppId, "none", false));
        Throws("unknown.StopAppDomainProtocolListenerChannel", () => host.StopAppDomainProtocolListenerChannel(AppId, "none", 1, false));

        // The process protocol handlers of <protocols> (the root web.config's): created once, stopped, removed.
        Throws("process.unknown", () => host.StartProcessProtocolListenerChannel("none", new Callback()));
        Throws("process.badType", () => host.StartProcessProtocolListenerChannel("bad", new Callback()));
        Throws("process.start", () => host.StartProcessProtocolListenerChannel("foc", new Callback()));
        Throws("process.start.again", () => host.StartProcessProtocolListenerChannel("foc", new Callback()));
        Throws("process.stopChannel", () => host.StopProcessProtocolListenerChannel("foc", 7, true));
        Throws("process.stop", () => host.StopProcessProtocol("foc", false));
        Throws("process.stop.again", () => host.StopProcessProtocol("foc", false));
        // The application's protocol handler, where it runs.
        Throws("appDomain.stop.none", () => host.StopAppDomainProtocol(AppId, "foc", false));
        manager.CreateObject(AppId, typeof(Handler), "/", root, false);
        Throws("appDomain.stopChannel", () => host.StopAppDomainProtocolListenerChannel(AppId, "foc", 3, false));
        Throws("appDomain.stop", () => host.StopAppDomainProtocol(AppId, "foc", true));

        // IIS's own application host (webengine4.dll), not in the port: what needs it fails.
        Throws("iis.StartApplication", () => host.StartApplication(AppId, "." + Path.DirectorySeparatorChar + "app", out _));
        Throws("iis.StartAppDomainProtocolListenerChannel", () => host.StartAppDomainProtocolListenerChannel(AppId, "foc", new Callback()));
        Throws("ShutdownApplication.null", () => host.ShutdownApplication(null));
        Throws("ShutdownApplication.unknown", () => host.ShutdownApplication("/LM/W3SVC/9/ROOT"));

        var infos = Enumerate(host).ToList();
        Say("enumerate", infos.Count);
        Say("enumerate.info", infos[0].GetId() + " " + infos[0].GetVirtualPath() + " " + (infos[0].GetPhysicalPath() == root + Path.DirectorySeparatorChar) + " " + infos[0].GetSiteId() + " " + infos[0].IsIdle());
        Say("IsIdle", host.IsIdle());
        host.Ping(null);
        var ping = new Ping();
        host.Ping(ping);
        Say("pinged", ping.Responded.Wait(5000));
        ((ICustomRuntimeManager)host).Register(new CustomRuntime());
        var resume = ((IProcessSuspendListener)host).Suspend();
        resume.Resume();
        Say("resumed", true);

        // The listener adapter's shim: the callback (a COM object from IIS) as the application's, to the handler.
        var shim = new ListenerAdapterDispatchShim();
        Say("shim.proxy", shim.MarshalComProxy(new Callback()) != null);
        shim.StartListenerChannel(new Handler(), new Callback());
        HostingEnvironment.RegisterObject(shim);
        ((IRegisteredObject)shim).Stop(true);

        // IIS's shutdown of the worker process: the process protocols stopped at once, then the applications (the port: the
        // process ends, 75).
        host.StartProcessProtocolListenerChannel("foc", new Callback());
        host.Shutdown();
        Thread.Sleep(15000);
        Say("alive", true);
        return 0;
    }

    static IEnumerable<IAppDomainInfo> Enumerate(ProcessHost host)
    {
        host.EnumerateAppDomains(out var e);
        var list = new List<IAppDomainInfo>();
        Say("enumerate.count", e.Count());
        while (e.MoveNext()) list.Add(e.GetData());
        e.Reset();
        Say("enumerate.again", e.MoveNext() == list.Count > 0);
        return list;
    }

    static int PreloadFailure(string[] args)
    {
        var host = ProcessHost.GetProcessHost(new Functions(args[0]));
        var util = new PreloadUtil { Enabled = true, Type = typeof(ThrowingClient).AssemblyQualifiedName };
        host.SetApplicationPreloadUtil(util);
        host.SetApplicationPreloadState("ctx", AppId, false);
        var manager = ApplicationManager.GetApplicationManager();
        Throws("preload", () => host.PreloadApplicationIfRequired(AppId, new ApplicationManagerTests.SiteHost(args[0]), null, manager.GetLockableAppDomainContext(AppId)));
        Say("running", manager.GetRunningApplications().Length);
        return 0;
    }

    /// <summary>The preloaded application's initialization error (its cache provider missing): reported, thrown.</summary>
    static int PreloadInitFailure(string[] args)
    {
        var host = ProcessHost.GetProcessHost(new Functions(args[0]));
        host.SetApplicationPreloadUtil(new PreloadUtil { Enabled = true, Type = typeof(PreloadClient).AssemblyQualifiedName });
        host.SetApplicationPreloadState("ctx", AppId, false);
        var manager = ApplicationManager.GetApplicationManager();
        Throws("preload", () => host.PreloadApplicationIfRequired(AppId, new ApplicationManagerTests.SiteHost(args[0]), null, manager.GetLockableAppDomainContext(AppId)));
        return 0;
    }

    /// <summary>The preloaded application's I/O error (a sharing violation of its config): tried again (Dev10 858421).</summary>
    static int PreloadRetry(string[] args)
    {
        var host = ProcessHost.GetProcessHost(new Functions(args[0]));
        host.SetApplicationPreloadUtil(new PreloadUtil { Enabled = true, Type = typeof(PreloadClient).AssemblyQualifiedName });
        host.SetApplicationPreloadState("ctx", AppId, false);
        // The custom loader's failure, an I/O error (COR_E_IO): the application's initialization error.
        ((IProcessHostLite)host).ReportCustomLoaderError(AppId, unchecked((int)0x80131620), null);
        var manager = ApplicationManager.GetApplicationManager();
        Throws("preload", () => host.PreloadApplicationIfRequired(AppId, new ApplicationManagerTests.SiteHost(args[0]), null, manager.GetLockableAppDomainContext(AppId)));
        Thread.Sleep(15000);
        Say("alive", true);
        return 0;
    }

    static int CustomLoaderFailure(string[] args)
    {
        var host = ProcessHost.GetProcessHost(new Functions(args[0]));
        var lite = (IProcessHostLite)host;
        // A failure reported for another application, then none (S_OK), then one for this application.
        lite.ReportCustomLoaderError("/LM/W3SVC/2/ROOT", unchecked((int)0x80070005), null);
        lite.ReportCustomLoaderError(AppId, 0, null);
        Say("loader", lite.GetCustomLoader(AppId, "MACHINE/WEBROOT/APPHOST/Default Web Site", out _, out _) == null);
        var manager = ApplicationManager.GetApplicationManager();
        manager.CreateObject(AppId, typeof(ApplicationManagerTests.Registered), "/", args[0], false);
        Say("initializationException", HostingEnvironment.InitializationException?.GetType().Name ?? "none");
        Say("after", ProcessHost.GetExistingCustomLoaderFailureAndClear(AppId) == null);
        lite.ReportCustomLoaderError(AppId, unchecked((int)0x80070005), null);
        Say("reported.other", ProcessHost.GetExistingCustomLoaderFailureAndClear("/LM/W3SVC/2/ROOT") == null);
        Say("reported", ProcessHost.GetExistingCustomLoaderFailureAndClear(AppId)?.SourceException.GetType().Name);
        return 0;
    }

    // ---- the test's side

    static readonly Dictionary<string, Lazy<(Result Result, Dictionary<string, string> Said)>> runs = new();

    static (Result Result, Dictionary<string, string> Said) Run(string scenario, Action<Site> setup = null, string systemWeb = "")
    {
        Lazy<(Result, Dictionary<string, string>)> run;
        lock (runs)
        {
            if (!runs.TryGetValue(scenario, out run))
                runs[scenario] = run = new(() =>
                {
                    using var site = new Site("<configuration><system.web>" + systemWeb + "<compilation><assemblies><remove assembly=\"System.Web.Services\"/></assemblies></compilation></system.web></configuration>");
                    // The root web.config (the port's: beside its machine.config in the application's App_Data): its protocols.
                    site.Write("App_Data/web.config", $"""
                        <configuration><system.web><protocols>
                          <add name="foc" processHandlerType="{typeof(ProcessHandler).AssemblyQualifiedName}" appDomainHandlerType="{typeof(Handler).AssemblyQualifiedName}"/>
                          <add name="bad" processHandlerType="Nope.Handler, Nowhere" appDomainHandlerType="Nope.Handler, Nowhere" validate="false"/>
                        </protocols></system.web></configuration>
                        """);
                    setup?.Invoke(site);
                    var result = Child.Run("ProcessHostTests." + scenario, new[] { site.Root });
                    return (result, Said(result));
                });
        }
        return run.Value;
    }

    static (Result Result, Dictionary<string, string> Said) CallsRun() => Run("Calls");

    [Fact] // reference source GetProcessHost: one per process, with IIS's support functions
    public void The_process_host_is_one()
    {
        var run = CallsRun();
        Assert.Equal("True", run.Get("singleton"));
        Assert.Equal("True", run.Get("functions"));
    }

    [Fact] // reference source: each method checks its arguments (ArgumentNullException, reported and thrown)
    public void The_arguments_are_checked()
    {
        var run = CallsRun();
        var keys = run.Said.Keys.Where(k => k.StartsWith("null.")).ToList();
        Assert.Equal(11, keys.Count);
        foreach (var key in keys) Assert.True(run.Said[key] == "ArgumentNullException", key + "=" + run.Said[key]);
    }

    [Fact] // reference source: stopping a protocol not started does nothing; an unknown protocol's application handler has no type (GetObject's ArgumentNullException)
    public void Unknown_protocols()
    {
        var run = CallsRun();
        Assert.Equal("none", run.Get("unknown.StopProcessProtocolListenerChannel"));
        Assert.Equal("none", run.Get("unknown.StopProcessProtocol"));
        Assert.Equal("ArgumentNullException", run.Get("unknown.StopAppDomainProtocol"));
        Assert.Equal("ArgumentNullException", run.Get("unknown.StopAppDomainProtocolListenerChannel"));
    }

    [Fact] // reference source SetApplicationPreloadState/Util: context and appId required, enabled only with the util, the util set once; nothing preloaded while IIS says it is disabled or names no type (Dev10 782385)
    public void The_preload_state_is_checked()
    {
        var run = CallsRun();
        Assert.Equal("ArgumentException", run.Get("preload.context.empty"));
        Assert.Equal("ArgumentException", run.Get("preload.appId.empty"));
        Assert.Equal("ArgumentException", run.Get("preload.enabled.noUtil"));
        Assert.Equal("InvalidOperationException", run.Get("preload.util.again"));
        Assert.Equal("none", run.Get("preload.notEnabled"));
        Assert.Equal("none", run.Get("preload.noType"));
        Assert.Equal("0", run.Get("enumerate.before"));
    }

    [Fact] // docs IProcessHostPreloadClient.Preload: called in the started application with IIS's parameters
    public void The_application_is_preloaded()
    {
        var run = CallsRun();
        Assert.Equal("p1,p2 True", run.Get("preload.client"));
        Assert.Equal("1", run.Get("preloaded.running"));
    }

    [Fact] // reference source EnumerateAppDomains: the running applications (id, paths, site, idle), enumerated again after Reset; IsIdle, Ping answered, suspend and resume
    public void The_applications_are_enumerated()
    {
        var run = CallsRun();
        Assert.Equal("1", run.Get("enumerate"));
        Assert.Equal(AppId + " / True 1 True", run.Get("enumerate.info"));
        Assert.Equal("True", run.Get("enumerate.again"));
        Assert.Equal("True", run.Get("IsIdle"));
        Assert.Equal("True", run.Get("pinged"));
        Assert.Equal("True", run.Get("resumed"));
    }

    [Fact] // reference source GetCustomLoader: none without bin\AspNet.Loader.dll; GetFullExceptionMessage: the HRESULT's exception (empty for a success)
    public void Without_a_custom_loader_there_is_none()
    {
        var run = CallsRun();
        Assert.Equal("True", run.Get("customLoader.none"));
        Assert.Equal("True", run.Get("fullMessage"));
        Assert.Equal("True", run.Get("fullMessage.success"));
    }

    [WindowsFact("the listener adapter's COM proxy (MarshalComProxy)")] // reference source ListenerAdapterDispatchShim: the listener's callback given to the protocol handler in the application
    public void The_listener_channel_reaches_the_handler()
    {
        var run = CallsRun();
        Assert.Equal("True", run.Get("shim.proxy"));
        Assert.Equal("7", run.Get("handler.started"));
    }

    [Fact] // reference source: <protocols>' process handler created once for its listener channels (unknown id ArgumentException, a type not found ConfigurationErrorsException), stopped, removed
    public void The_process_protocol_handlers()
    {
        var run = CallsRun();
        Assert.Equal("ArgumentException", run.Get("process.unknown"));
        Assert.Equal("ConfigurationErrorsException", run.Get("process.badType"));
        Assert.Equal("none", run.Get("process.start"));
        Assert.Equal("none", run.Get("process.start.again"));
        Assert.Equal("7 True", run.Get("process.handler.stopChannel"));
        Assert.Equal("none", run.Get("process.stop.again"));
        Assert.Equal("True", run.Get("process.handler.stopProtocol.False"));
        Assert.EndsWith(" 7 True", run.Get("process.handler.start")); // the listener's callback, the process host as the manager
    }

    [Fact] // reference source StopAppDomainProtocol(ListenerChannel): the application's handler of the protocol, when it exists
    public void The_application_s_protocol_handler_is_stopped()
    {
        var run = CallsRun();
        Assert.Equal("none", run.Get("appDomain.stop.none"));
        Assert.Equal("3 False", run.Get("handler.stopChannel"));
        Assert.Equal("True", run.Get("handler.stopProtocol"));
    }

    [WindowsFact("the listener adapter's COM proxy (MarshalComProxy)")] // reference source Shutdown: the process protocols stopped at once (immediate), then every application (the port: the process ends, 75)
    public void Shutdown_stops_the_protocols_then_the_applications()
    {
        var run = CallsRun();
        Assert.Equal("True", run.Get("process.handler.stopProtocol.True"));
        Assert.False(run.Said.ContainsKey("alive"));
        Assert.Equal(75, run.Result.ExitCode);
    }

    [Fact] // reference source PreloadApplicationIfRequired: the client's failure reported to IIS (E_FAIL) and thrown; the application stays started
    public void A_failing_preload_is_reported()
    {
        var run = Run("PreloadFailure");
        Assert.Equal("ctx 80004005 True", run.Get("preload.failure"));
        Assert.Equal("InvalidOperationException", run.Get("preload"));
        Assert.Equal("1", run.Get("running"));
    }

    [Fact] // reference source PreloadApplicationIfRequired: the application's initialization error reported to IIS (E_FAIL, for it to recycle the process) and thrown
    public void A_preloaded_application_s_error_is_reported()
    {
        var run = Run("PreloadInitFailure", systemWeb: "<caching><cache defaultProvider=\"Missing\"/></caching>");
        Assert.Equal("ctx 80004005 False", run.Get("preload.failure"));
        Assert.Equal("ProviderException", run.Get("preload"));
        Assert.False(run.Said.ContainsKey("preload.client"));
    }

    [Fact] // reference source (Dev10 858421): an I/O error initializing the preloaded application, its old one shut down and tried again; the port: the shutdown ends the process (75), which starts it again
    public void A_preloaded_application_s_IO_error_is_tried_again()
    {
        var run = Run("PreloadRetry");
        Assert.Equal(75, run.Result.ExitCode);
        Assert.False(run.Said.ContainsKey("alive"));
    }

    [Fact] // the port's choice: IIS's own application host (webengine4.dll) is not there, what needs it fails; ShutdownApplication of an application not running does nothing (reference source)
    public void Without_IIS_its_applications_do_not_start()
    {
        var run = CallsRun();
        Assert.Equal("DllNotFoundException", run.Get("iis.StartApplication"));
        Assert.Equal("DllNotFoundException", run.Get("iis.StartAppDomainProtocolListenerChannel"));
        Assert.Equal("ArgumentNullException", run.Get("ShutdownApplication.null"));
        Assert.Equal("none", run.Get("ShutdownApplication.unknown"));
    }

    [Fact] // reference source IProcessSuspendListener.Suspend: the custom runtimes registered are suspended and resumed too
    public void The_custom_runtimes_are_suspended()
    {
        var run = CallsRun();
        Assert.Equal("True", run.Get("customRuntime.suspended"));
        Assert.Equal("True", run.Get("customRuntime.resumed"));
    }

    [Fact] // reference source: the custom loader's failure (its HRESULT) is the application's startup error when it starts (shown by its first request), once
    public void A_custom_loader_s_failure_is_the_application_s_error()
    {
        var run = Run("CustomLoaderFailure", site => site.Write("bin/AspNet.Loader.dll", "not an assembly"));
        Assert.Equal("True", run.Get("loader"));
        Assert.NotEqual("none", run.Get("initializationException"));
        Assert.Equal("True", run.Get("after"));
        Assert.Equal("True", run.Get("reported.other"));
        Assert.Equal("UnauthorizedAccessException", run.Get("reported"));
    }
}
