using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The application manager (ApplicationManager: learn.microsoft.com/dotnet/api/system.web.hosting.applicationmanager, and
/// referencesource System.Web/Hosting/ApplicationManager.cs, "reference source" below): the application's host made from
/// its web.config, its objects, its shutdown. HostingEnvironment is one per process here (one application per process,
/// SetApplicationData's comment), so each scenario runs in a process of its own (Child) and writes what it sees.
/// A shut down application ends the process with exit code 75 (ProcessRestart: the port's choice, its supervisor starts
/// it again, where .NET Framework unloads the AppDomain).
/// </summary>
// The CodeDom collection: ExecutorPortable's output files, inheritable while its process starts, would be inherited by
// this class's processes started meanwhile (Windows), and held open.
[Collection("CodeDom")]
public class ApplicationManagerTests
{
    // ---- the child's side

    /// <summary>A well-known object (CreateObject's).</summary>
    public sealed class Registered : MarshalByRefObject, IRegisteredObject
    {
        public void Stop(bool immediate) => Say("registered.stop", immediate);
    }

    /// <summary>An object registered with the hosting environment that unregisters only when told to stop immediately.</summary>
    public sealed class Lingering : IRegisteredObject
    {
        public static readonly System.Diagnostics.Stopwatch Clock = new();

        public void Stop(bool immediate)
        {
            Say("lingering.stop." + immediate, Clock.ElapsedMilliseconds + " " + HostingEnvironment.ShutdownReason);
            if (immediate) HostingEnvironment.UnregisterObject(this);
        }
    }

    /// <summary>The application's host, as a host other than ASP.NET's gives it (no separator at the path's end).</summary>
    public sealed class SiteHost : MarshalByRefObject, IApplicationHost
    {
        readonly string root;
        public SiteHost(string root) => this.root = root;
        public string GetVirtualPath() => "/";
        public string GetPhysicalPath() => root;
        public IConfigMapPathFactory GetConfigMapPathFactory() => new SimpleConfigMapPathFactory();
        public IntPtr GetConfigToken() => IntPtr.Zero;
        public string GetSiteName() => "Default Web Site";
        public string GetSiteID() => "1";
        public void MessageReceived() { }
    }

    /// <summary>A cache store (&lt;cache defaultProvider&gt;'s): the application's caches are of it.</summary>
    public sealed class Store : System.Web.Caching.CacheStoreProvider
    {
        readonly System.Collections.Hashtable items = new();
        public override long ItemCount => items.Count;
        public override long SizeInBytes => 0;
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config) { }
        public override object Add(string key, object item, System.Web.Caching.CacheInsertOptions options) { lock (items) { if (items.ContainsKey(key)) return items[key]; items[key] = item; return null; } }
        public override object Get(string key) { lock (items) return items[key]; }
        public override void Insert(string key, object item, System.Web.Caching.CacheInsertOptions options) { lock (items) items[key] = item; }
        public override object Remove(string key) => Remove(key, System.Web.Caching.CacheItemRemovedReason.Removed);
        public override object Remove(string key, System.Web.Caching.CacheItemRemovedReason reason) { lock (items) { var item = items[key]; items.Remove(key); return item; } }
        public override long Trim(int percent) => 0;
        public override bool AddDependent(string key, System.Web.Caching.CacheDependency dependency, out DateTime utcLastUpdated) { utcLastUpdated = DateTime.MinValue; return false; }
        public override void RemoveDependent(string key, System.Web.Caching.CacheDependency dependency) { }
        public override void Dispose() { }
        public override System.Collections.IDictionaryEnumerator GetEnumerator() { lock (items) return new System.Collections.Hashtable(items).GetEnumerator(); }
    }

    sealed class Ping : IProcessPingCallback
    {
        public readonly ManualResetEventSlim Responded = new();
        public void Respond() => Responded.Set();
    }

    static void Say(string key, object value) => Child.Say(key, value);

    static void Throws(string key, Action action) => Child.Throws(key, action);

    /// <summary>Waits for the shutdown to end the process; returns 0 if it did not.</summary>
    static int Wait()
    {
        Thread.Sleep(15000);
        Say("alive", true);
        return 0;
    }

    static int Lifecycle(string[] args)
    {
        var root = args[0];
        var manager = ApplicationManager.GetApplicationManager();
        Say("singleton", ReferenceEquals(manager, ApplicationManager.GetApplicationManager()));
        var host = new SiteHost(root);
        var type = typeof(Registered);

        Throws("null.CreateObject.appHost", () => manager.CreateObject((IApplicationHost)null, type));
        Throws("null.CreateObject.type", () => manager.CreateObject(host, null));
        Throws("null.CreateObject.appId", () => manager.CreateObject(null, type, "/", root, false));
        Throws("null.CreateObjectInternal.appId", () => manager.CreateObjectInternal(null, type, host, false));
        Throws("null.CreateObjectInternal.type", () => manager.CreateObjectInternal("a", null, host, false));
        Throws("null.CreateObjectInternal.appHost", () => manager.CreateObjectInternal("a", type, null, false));
        Throws("null.GetObject.appId", () => manager.GetObject(null, type));
        Throws("null.GetObject.type", () => manager.GetObject("a", null));
        Throws("null.GetAppDomain.appId", () => manager.GetAppDomain((string)null));
        Throws("null.GetAppDomain.appHost", () => manager.GetAppDomain((IApplicationHost)null));
        Throws("null.GetAssemblyLoadContext.appId", () => manager.GetAssemblyLoadContext((string)null));
        Throws("null.GetAssemblyLoadContext.appHost", () => manager.GetAssemblyLoadContext((IApplicationHost)null));
        Throws("null.RemoveFromTableIfRuntimeExists.appId", () => manager.RemoveFromTableIfRuntimeExists(null, type));
        Throws("null.RemoveFromTableIfRuntimeExists.runtimeType", () => manager.RemoveFromTableIfRuntimeExists("a", null));
        Throws("null.StopObject.appId", () => manager.StopObject(null, type));
        Throws("null.StopObject.type", () => manager.StopObject("a", null));
        Throws("null.ShutdownApplication.appId", () => manager.ShutdownApplication(null));
        Throws("notRegisteredObject", () => manager.CreateObject("a", typeof(string), "/", root, false));

        // No application yet.
        Say("before.GetObject", manager.GetObject("a", type) == null);
        Say("before.GetAppDomain", manager.GetAppDomain("a") == null);
        Say("before.GetAssemblyLoadContext", manager.GetAssemblyLoadContext("a") == null);
        Say("before.IsIdle", manager.IsIdle());
        Say("before.running", manager.GetRunningApplications().Length);
        Throws("before.StopObject", () => manager.StopObject("a", type));
        Throws("before.RemoveFromTable", () => manager.RemoveFromTableIfRuntimeExists("a", type));
        Throws("before.ShutdownApplication", () => manager.ShutdownApplication("a"));

        var created = manager.CreateObject(host, type);
        var running = manager.GetRunningApplications();
        Say("running", running.Length);
        var id = running[0].ID;
        Say("running.virtualPath", running[0].VirtualPath);
        Say("running.physicalPath", running[0].PhysicalPath);
        Say("initializationException", HostingEnvironment.InitializationException?.GetType().Name ?? "none");
        Say("initializationException.message", HostingEnvironment.InitializationException?.Message.ReplaceLineEndings(" "));
        Say("hosted", HostingEnvironment.IsHosted);
        Say("IsDevelopmentEnvironment", HostingEnvironment.IsDevelopmentEnvironment);
        Say("again.sameObject", ReferenceEquals(created, manager.CreateObject(id, type, "/", root, false)));
        Say("again.throwOnError.sameObject", ReferenceEquals(created, manager.CreateObject(id, type, "/", root, false, true)));
        Throws("again.failIfExists", () => manager.CreateObject(id, type, "/", root, true));
        Say("GetObject.same", ReferenceEquals(created, manager.GetObject(id, type)));
        Say("GetObject.otherType", manager.GetObject(id, typeof(Lingering)) == null);
        Say("GetAppDomain.current", ReferenceEquals(AppDomain.CurrentDomain, manager.GetAppDomain(id)));
        Say("GetAppDomain.host.current", ReferenceEquals(AppDomain.CurrentDomain, manager.GetAppDomain(host)));
        Say("GetAssemblyLoadContext", manager.GetAssemblyLoadContext(host) == AssemblyLoadContext.GetLoadContext(typeof(HttpRuntime).Assembly));
        var infos = manager.GetAppDomainInfos();
        Say("infos", infos.Length);
        Say("infos.siteId", infos[0].GetSiteId());
        Say("infos.idle", infos[0].IsIdle());
        Say("IsIdle", manager.IsIdle());
        HostingEnvironment.IncrementBusyCount();
        Say("busy.IsIdle", manager.IsIdle());
        HostingEnvironment.DecrementBusyCount();
        Throws("RemoveFromTable.otherType", () => manager.RemoveFromTableIfRuntimeExists(id, typeof(Lingering)));
        Say("RemoveFromTable.otherType.running", manager.GetRunningApplications().Length);

        // The process host's ping (IProcessHostIdleAndHealthCheck.Ping): answered from the thread pool.
        manager.Ping(null);
        manager.RespondToPingIfNeeded();
        var ping = new Ping();
        manager.Ping(ping);
        Say("pinged", ping.Responded.Wait(5000));

        // The process host's suspend and resume (IProcessSuspendListener).
        var state = manager.SuspendAllApplications();
        Say("suspended", ((Array)state).Length);
        manager.ResumeAllApplications(state);

        manager.StopObject(id, type);
        Say("stopped.GetObject", manager.GetObject(id, type) == null);

        HostingEnvironment.RegisterObject(new Lingering());
        Lingering.Clock.Start();
        manager.ShutdownApplication(id);
        return Wait();
    }

    static int Settings(string[] args)
    {
        var manager = new ApplicationManager();
        manager.CreateObject("settings", typeof(Registered), "/", args[0], false);
        Say("GetApplicationManager.hosting", ReferenceEquals(manager, ApplicationManager.GetApplicationManager()));
        Say("initializationException", HostingEnvironment.InitializationException?.ToString() ?? "none");
        Say("TargetFramework", HttpRuntime.TargetFramework);
        foreach (var name in new[] { "On", "Off", "Bad" })
            Say("switch." + name, AppContext.TryGetSwitch("Foc.Test." + name, out var value) ? value.ToString() : "unset");
        Say("FcnMode", HostingEnvironment.HostingParameters?.FcnMode);
        Say("FcnSkipReadAndCacheDacls", HostingEnvironment.FcnSkipReadAndCacheDacls);
        Say("IsDevelopmentEnvironment", HostingEnvironment.IsDevelopmentEnvironment);
        var cache = HostingEnvironment.CacheStoreProviderSettings;
        Say("cacheProvider", cache == null ? "none" : cache["name"] + " " + cache["type"] + " " + cache["size"]);
        Say("cacheStore", HttpRuntime.Cache.GetObjectCache(createIfDoesNotExist: true).GetType().Name);
        return 0;
    }

    static int Failing(string[] args)
    {
        ApplicationManager.GetApplicationManager().CreateObject("failing", typeof(Registered), "/", args[0], false);
        Say("initializationException", HostingEnvironment.InitializationException?.GetType().Name ?? "none");
        Say("initializationException.message", HostingEnvironment.InitializationException?.Message.ReplaceLineEndings(" "));
        Say("running", ApplicationManager.GetApplicationManager().GetRunningApplications().Length);
        return 0;
    }

    static int RegexTimeout(string[] args)
    {
        ApplicationManager.GetApplicationManager().CreateObject("regex", typeof(Registered), "/", args[0], false);
        Say("MatchTimeout", new Regex("a").MatchTimeout);
        return 0;
    }

    static int Close(string[] args)
    {
        var manager = ApplicationManager.GetApplicationManager();
        manager.CreateObject("close", typeof(Registered), "/", args[0], false);
        manager.Open();
        manager.Open();
        manager.Close();
        Say("closed.once.running", manager.GetRunningApplications().Length);
        manager.Close();
        Say("returned", true);
        return 0;
    }

    static int Reduce(string[] args)
    {
        var manager = ApplicationManager.GetApplicationManager();
        manager.CreateObject("reduce", typeof(Registered), "/", args[0], false);
        Say("FcnMode", HostingEnvironment.HostingParameters?.FcnMode);
        Say("FcnSkipReadAndCacheDacls", HostingEnvironment.FcnSkipReadAndCacheDacls);
        manager.GetObject("empty", typeof(Registered)); // an entry without an application
        manager.ReduceAppDomainsCount(2);
        Say("underLimit.count", manager.AppDomainsCount);
        manager.ReduceAppDomainsCount(1);
        Say("overLimit.count", manager.AppDomainsCount);
        return Wait();
    }

    static int Remove(string[] args)
    {
        var manager = ApplicationManager.GetApplicationManager();
        manager.CreateObject("remove", typeof(Registered), "/", args[0], false);
        manager.RemoveFromTableIfRuntimeExists("remove", typeof(Registered));
        Say("running", manager.GetRunningApplications().Length);
        Say("count", manager.AppDomainsCount);
        Say("IsIdle", manager.IsIdle());
        Say("infos", manager.GetAppDomainInfos().Length);
        Say("suspended", ((Array)manager.SuspendAllApplications()).Length);
        manager.ReduceAppDomainsCount(0);
        manager.ShutdownApplication("remove");
        Say("shutdown.ignored", true);
        // A second application in the process: one HostingEnvironment per process (the port's choice).
        Throws("second", () => manager.CreateObject("second", typeof(Registered), "/", args[0], false));
        return 0;
    }

    static int ApplicationHostScenario(string[] args)
    {
        var created = ApplicationHost.CreateApplicationHost(typeof(Registered), "/app", args[0]);
        Say("type", created.GetType().Name);
        Say("virtualPath", HostingEnvironment.ApplicationVirtualPath);
        Say("physicalPath", HostingEnvironment.ApplicationPhysicalPath);
        Say("running", ApplicationManager.GetApplicationManager().GetRunningApplications().Length);
        return 0;
    }

    static int NewLoadContext(string[] args)
    {
        var manager = ApplicationManager.GetApplicationManager();
        var created = manager.CreateInstanceInNewWorkerLoadContext(typeof(Registered), "new", VirtualPath.Create("/"), args[0], newLoadContext: true);
        Say("type", created.GetType().Name);
        Say("physicalPath", HostingEnvironment.ApplicationPhysicalPath);
        // Another application in a load context of its own: one HostingEnvironment per process, its context unloaded.
        Throws("second", () => manager.CreateInstanceInNewWorkerLoadContext(typeof(Registered), "other", VirtualPath.Create("/"), args[0], newLoadContext: true));
        return 0;
    }

    static int NewWorkerAppDomain(string[] args)
    {
        System.Configuration.ClientConfigurationHost.UseNetFXMachineConfig = true;
        var handle = ApplicationManager.GetApplicationManager().CreateInstanceInNewWorkerAppDomain(typeof(Registered), "domain", VirtualPath.Create("/"), args[0]);
        Say("type", handle.Unwrap().GetType().Name);
        Say("running", ApplicationManager.GetApplicationManager().GetRunningApplications().Length);
        return 0;
    }

    static int Precompile(string[] args)
    {
        var manager = ApplicationManager.GetApplicationManager();
        var parameters = new HostingEnvironmentParameters
        {
            HostingFlags = HostingEnvironmentFlags.ClientBuildManager | HostingEnvironmentFlags.DontCallAppInitialize,
            PrecompilationTargetPhysicalDirectory = args[1],
            ClientBuildManagerParameter = new ClientBuildManagerParameter { PrecompilationFlags = PrecompilationFlags.Updatable },
        };
        manager.CreateObjectWithDefaultAppHostAndAppId(args[0], VirtualPath.Create("/"), typeof(Registered), false, parameters, out var updatable, out _);
        Say("updatable", updatable);
        Say("initializationException", HostingEnvironment.InitializationException?.GetType().Name ?? "none");
        string id = null;
        parameters.ClientBuildManagerParameter = new ClientBuildManagerParameter();
        Throws("fixed.create", () => manager.CreateObjectWithDefaultAppHostAndAppId(args[0], VirtualPath.Create("/"), typeof(Registered), false, parameters, out id, out _));
        Say("fixed", id);
        Throws("plain.create", () => manager.CreateObjectWithDefaultAppHostAndAppId(args[0], "/", typeof(Registered), out id, out _));
        Say("plain", id);
        parameters.ClientBuildManagerParameter = null;
        Throws("noParameter.create", () => manager.CreateObjectWithDefaultAppHostAndAppId(args[0], VirtualPath.Create("/"), typeof(Registered), false, parameters, out id, out _));
        Say("noParameter", id);
        return 0;
    }

    static int Fatal(string[] args)
    {
        var field = typeof(ApplicationManager).GetField("_fatalException", BindingFlags.NonPublic | BindingFlags.Static);
        var first = new InvalidOperationException("first");
        ApplicationManager.OnUnhandledException(AppDomain.CurrentDomain, new UnhandledExceptionEventArgs(first, false));
        Say("notTerminating", field.GetValue(null) == null);
        ApplicationManager.OnUnhandledException(AppDomain.CurrentDomain, new UnhandledExceptionEventArgs("not an exception", true));
        Say("notException", field.GetValue(null) == null);
        ApplicationManager.OnUnhandledException(null, new UnhandledExceptionEventArgs(first, true));
        Say("noDomain", field.GetValue(null) == null);
        ApplicationManager.OnUnhandledException(AppDomain.CurrentDomain, new UnhandledExceptionEventArgs(first, true));
        Say("recorded", ReferenceEquals(first, field.GetValue(null)));
        ApplicationManager.OnUnhandledException(AppDomain.CurrentDomain, new UnhandledExceptionEventArgs(new Exception("second"), true));
        ApplicationManager.RecordFatalException(new Exception("third"));
        Say("first.kept", ReferenceEquals(first, field.GetValue(null)));
        return 0;
    }

    // ---- the test's side

    static readonly Dictionary<string, Lazy<(Child.Result Result, Dictionary<string, string> Said)>> runs = new();

    /// <summary>The scenario run once (a site with the web.config) for the tests that look at it.</summary>
    static (Child.Result Result, Dictionary<string, string> Said) Run(string scenario, string webConfig, IDictionary<string, string> environment = null, string key = null, Action<Site> setup = null)
    {
        Lazy<(Child.Result, Dictionary<string, string>)> run;
        lock (runs)
        {
            if (!runs.TryGetValue(key ?? scenario, out run))
                runs[key ?? scenario] = run = new(() =>
                {
                    using var site = new Site(webConfig ?? Web());
                    using var target = new Site();
                    setup?.Invoke(site);
                    var result = Child.Run("ApplicationManagerTests." + scenario, new[] { site.Root, target.Root }, environment);
                    return (result, Child.Said(result));
                });
        }
        return run.Value;
    }

    static string Get((Child.Result Result, Dictionary<string, string> Said) run, string key) => Child.Get(run, key);

    /// <summary>
    /// A web.config with the system.web content (and appSettings'). The port's machine.config compiles with
    /// System.Web.Services, not in the tests' folder: removed (else the application's initialization error).
    /// </summary>
    static string Web(string systemWeb = "", string appSettings = "", string compilation = "") =>
        "<configuration><appSettings>" + appSettings + "</appSettings><system.web>" + systemWeb +
        "<compilation" + compilation + "><assemblies><remove assembly=\"System.Web.Services\"/></assemblies></compilation></system.web></configuration>";

    static readonly string ShutdownTimeout = Web("<hostingEnvironment shutdownTimeout=\"1\"/>");

    static (Child.Result Result, Dictionary<string, string> Said) LifecycleRun() =>
        Run("Lifecycle", ShutdownTimeout, new Dictionary<string, string> { ["DEV_ENVIRONMENT"] = null });

    [Fact] // reference source: each public member checks its arguments (ArgumentNullException), CreateObject's type an IRegisteredObject (ArgumentException)
    public void The_arguments_are_checked()
    {
        var run = LifecycleRun();
        foreach (var key in run.Said.Keys.Where(k => k.StartsWith("null.")))
            Assert.True(run.Said[key] == "ArgumentNullException", key + "=" + run.Said[key]);
        Assert.Equal(17, run.Said.Keys.Count(k => k.StartsWith("null.")));
        Assert.Equal("ArgumentException", Get(run, "notRegisteredObject"));
    }

    [Fact] // reference source: GetObject, GetAppDomain null for an application not running; nothing stopped or shut down; idle
    public void Without_the_application_nothing_is_found()
    {
        var run = LifecycleRun();
        Assert.Equal("True", Get(run, "before.GetObject"));
        Assert.Equal("True", Get(run, "before.GetAppDomain"));
        Assert.Equal("True", Get(run, "before.GetAssemblyLoadContext"));
        Assert.Equal("True", Get(run, "before.IsIdle"));
        Assert.Equal("0", Get(run, "before.running"));
        Assert.Equal("none", Get(run, "before.StopObject"));
        Assert.Equal("none", Get(run, "before.RemoveFromTable"));
        Assert.Equal("none", Get(run, "before.ShutdownApplication"));
    }

    [Fact] // docs GetApplicationManager: the single instance; GetRunningApplications: the application's ID and paths (the physical one with its separator)
    public void The_running_application_is_listed()
    {
        var run = LifecycleRun();
        Assert.Equal("True", Get(run, "singleton"));
        Assert.Equal("1", Get(run, "running"));
        Assert.Equal("/", Get(run, "running.virtualPath"));
        Assert.EndsWith(Path.DirectorySeparatorChar.ToString(), Get(run, "running.physicalPath"));
        Assert.Equal("none", Get(run, "initializationException"));
        Assert.Equal("True", Get(run, "hosted"));
        Assert.Equal("1", Get(run, "infos"));
        Assert.Equal("1", Get(run, "infos.siteId"));
        Assert.Equal("True", Get(run, "infos.idle"));
        Assert.Equal("True", Get(run, "IsIdle"));
        Assert.Equal("False", Get(run, "busy.IsIdle")); // docs HostingEnvironment.IncrementBusyCount: a request in progress
    }

    [Fact] // docs CreateObject: failIfExists true throws when the object exists (InvalidOperationException), else the existing object; GetObject finds it
    public void A_well_known_object_is_created_once()
    {
        var run = LifecycleRun();
        Assert.Equal("True", Get(run, "again.sameObject"));
        Assert.Equal("True", Get(run, "again.throwOnError.sameObject"));
        Assert.Equal("InvalidOperationException", Get(run, "again.failIfExists"));
        Assert.Equal("True", Get(run, "GetObject.same"));
        Assert.Equal("True", Get(run, "GetObject.otherType"));
        Assert.Equal("none", Get(run, "RemoveFromTable.otherType"));
        Assert.Equal("1", Get(run, "RemoveFromTable.otherType.running"));
    }

    [Fact] // the port's choice: the application's AppDomain is the process's (one application per process), its load context System.Web's
    public void The_application_s_domain_is_the_process_s()
    {
        var run = LifecycleRun();
        Assert.Equal("True", Get(run, "GetAppDomain.current"));
        Assert.Equal("True", Get(run, "GetAppDomain.host.current"));
        Assert.Equal("True", Get(run, "GetAssemblyLoadContext"));
    }

    [Fact] // reference source: the process host's ping is answered (from the thread pool); suspend and resume reach every application
    public void A_ping_is_answered_and_the_applications_suspend()
    {
        var run = LifecycleRun();
        Assert.Equal("True", Get(run, "pinged"));
        Assert.Equal("2", Get(run, "suspended")); // the application and the id looked up before it ran (none to suspend)
    }

    [Fact] // docs StopObject: the object is stopped (IRegisteredObject.Stop(false)) and no longer found
    public void A_stopped_object_is_gone()
    {
        var run = LifecycleRun();
        Assert.Equal("False", Get(run, "registered.stop"));
        Assert.Equal("True", Get(run, "stopped.GetObject"));
    }

    [Fact] // docs IRegisteredObject.Stop: Stop(false), then Stop(true) after shutdownTimeout (1 s here) if still registered; ShutdownReason HostingEnvironment; then the port ends the process (75)
    public void ShutdownApplication_stops_the_registered_objects_then_restarts()
    {
        var run = LifecycleRun();
        var soft = Get(run, "lingering.stop.False").Split(' ');
        var hard = Get(run, "lingering.stop.True").Split(' ');
        Assert.Equal("HostingEnvironment", soft[1]);
        Assert.Equal("HostingEnvironment", hard[1]);
        Assert.InRange(long.Parse(hard[0]) - long.Parse(soft[0]), 900, 10000);
        Assert.Equal(75, run.Result.ExitCode);
        Assert.False(run.Said.ContainsKey("alive"));
    }

    static readonly string SettingsConfig = Web(
        """
        <httpRuntime targetFramework="4.5" fcnMode="Single" defaultRegexMatchTimeout="00:00:02"/>
        <caching><cache defaultProvider="Store"><providers><add name="Store" type="FrameworkOnCore.Runtime.Tests.Hosting.ApplicationManagerTests+Store, FrameworkOnCore.Runtime.Tests" size="5"/></providers></cache></caching>
        <trust level="Full"/>
        """,
        """
        <add key="AppContext.SetSwitch:Foc.Test.On" value="true"/>
        <add key="AppContext.SetSwitch:Foc.Test.Off" value="false"/>
        <add key="AppContext.SetSwitch:Foc.Test.Bad" value="maybe"/>
        <add key="AppContext.SetSwitch:" value="true"/>
        <add key="aspnet:UseTaskFriendlySynchronizationContext" value="true"/>
        <add key="aspnet:UseRandomizedStringHashAlgorithm" value="true"/>
        <add key="aspnet:DisableFcnDaclRead" value="true"/>
        """);

    static (Child.Result Result, Dictionary<string, string> Said) SettingsRun() =>
        Run("Settings", SettingsConfig, new Dictionary<string, string> { ["DEV_ENVIRONMENT"] = "1" });

    [Fact] // docs HttpRuntime.TargetFramework: <httpRuntime targetFramework>; GetApplicationManager in the application: the manager that hosts it (reference source)
    public void The_target_framework_is_httpRuntime_s()
    {
        var run = SettingsRun();
        Assert.Equal("none", Get(run, "initializationException"));
        Assert.Equal("4.5", Get(run, "TargetFramework"));
        Assert.Equal("True", Get(run, "GetApplicationManager.hosting"));
    }

    [Fact] // docs <AppContextSwitchOverrides>: ASP.NET's appSettings "AppContext.SetSwitch:name" (a Boolean value) sets the switch
    public void The_AppContext_switches_of_appSettings_are_set()
    {
        var run = SettingsRun();
        Assert.Equal("True", Get(run, "switch.On"));
        Assert.Equal("False", Get(run, "switch.Off"));
        Assert.Equal("unset", Get(run, "switch.Bad"));
    }

    [Fact] // docs httpRuntime fcnMode, aspnet:DisableFcnDaclRead: the file change monitor's settings
    public void The_file_change_settings_reach_the_hosting_environment()
    {
        var run = SettingsRun();
        Assert.Equal("Single", Get(run, "FcnMode"));
        Assert.Equal("True", Get(run, "FcnSkipReadAndCacheDacls"));
    }

    [Fact] // docs HostingEnvironment.IsDevelopmentEnvironment: DEV_ENVIRONMENT=1 and <deployment retail="false"> (the default)
    public void DEV_ENVIRONMENT_makes_a_development_environment()
    {
        Assert.Equal("True", Get(SettingsRun(), "IsDevelopmentEnvironment"));
        Assert.Equal("False", Get(LifecycleRun(), "IsDevelopmentEnvironment"));
    }

    [Fact] // docs <cache defaultProvider> (4.7): the application's cache store provider, its name, type and attributes
    public void The_cache_provider_s_settings_are_kept()
    {
        Assert.Equal("Store FrameworkOnCore.Runtime.Tests.Hosting.ApplicationManagerTests+Store, FrameworkOnCore.Runtime.Tests 5", Get(SettingsRun(), "cacheProvider"));
        Assert.Equal("Store", Get(SettingsRun(), "cacheStore"));
    }

    [Theory] // reference source: a default cache provider not configured (none, or another name) is the application's initialization error (ProviderException), shown by its first request
    [InlineData("<providers/>")]
    [InlineData("<providers><add name=\"Other\" type=\"Foc.Store, Foc\"/></providers>")]
    public void A_missing_cache_provider_fails_the_application(string providers)
    {
        var run = Run("Failing", Web("<caching><cache defaultProvider=\"Store\">" + providers + "</cache></caching>"), key: "Failing" + providers);
        Assert.Equal("ProviderException", Get(run, "initializationException"));
        Assert.Equal("1", Get(run, "running"));
    }

    const string Policy = """
        <configuration><mscorlib><security><policy><PolicyLevel version="1"><SecurityClasses/>
        <NamedPermissionSets><PermissionSet class="NamedPermissionSet" version="1" Name="ASP.Net"/></NamedPermissionSets>
        <CodeGroup class="FirstMatchCodeGroup" version="1" PermissionSetName="Nothing"><IMembershipCondition class="AllMembershipCondition" version="1"/></CodeGroup>
        </PolicyLevel></policy></security></mscorlib></configuration>
        """;

    const string CustomLevel = "<securityPolicy><trustLevel name=\"Custom\" policyFile=\"custom.config\"/></securityPolicy>";

    static (Child.Result Result, Dictionary<string, string> Said) Trust(string trust, string more = "", string compilation = "") =>
        Run("Failing", Web("<trust " + trust + "/>" + more, compilation: compilation), key: "Trust" + trust + more + compilation, setup: site => site.Write("custom.config", Policy));

    [Theory] // reference source: a partially trusted application starts (its policy level from its trust level's policy file, <securityPolicy>; a relative compilation tempDirectory its error)
    [InlineData("level=\"Medium\"", "", "")]
    [InlineData("level=\"Medium\" legacyCasModel=\"true\"", "", "")]
    [InlineData("level=\"Custom\" originUrl=\"http://example.com/\"", CustomLevel, " tempDirectory=\"codegen\"")]
    [InlineData("level=\"Custom\"", CustomLevel, " tempDirectory=\"TEMP\"")]
    [InlineData("level=\"Custom\"", CustomLevel, "")]
    [InlineData("level=\"Missing\"", "", "")]
    public void A_partially_trusted_application_starts(string trust, string more, string compilation)
    {
        var codegen = Path.Combine(Path.GetTempPath(), "foc-codegen-" + Environment.ProcessId);
        try { Assert.Equal("1", Get(Trust(trust, more, compilation.Replace("TEMP", codegen)), "running")); }
        finally { if (Directory.Exists(codegen)) Directory.Delete(codegen, true); }
    }

    [Fact] // the port's choice (docs <trust>: a Medium trust application runs, partially trusted): .NET has no code access
    // security, so every trust level runs as full trust, without its policy file (the port has none: .NET Framework's
    // "Unable to read the security policy file for trust level" otherwise)
    public void A_Medium_trust_application_runs()
    {
        Assert.Equal("none", Get(Trust("level=\"Medium\""), "initializationException"));
        Assert.Equal("none", Get(Trust("level=\"Medium\" legacyCasModel=\"true\""), "initializationException"));
        Assert.Equal("none", Get(Trust("level=\"Missing\""), "initializationException"));
    }

    [Fact] // docs HttpRuntimeSection.DefaultRegexMatchTimeout: the default timeout of the application's regular expressions
    public void defaultRegexMatchTimeout_is_the_regular_expressions_default()
    {
        var run = Run("RegexTimeout", Web("<httpRuntime defaultRegexMatchTimeout=\"00:00:02\"/>"));
        Assert.Equal(TimeSpan.FromSeconds(2).ToString(), Get(run, "MatchTimeout"));
    }

    [Fact] // the port's choice (reference source ShutdownAll: shuts the applications down, waits for them and returns; Close:
    // the last one shuts everything down): a shut down application ends the process with exit code 75 (ProcessRestart),
    // ShutdownAll's too, for its supervisor to start it again; the process cannot run another application (one
    // HostingEnvironment per process, its static state kept), so returning would leave it serving none.
    public void The_last_Close_shuts_the_applications_down_and_ends_the_process()
    {
        var run = Run("Close", null);
        Assert.Equal("1", Get(run, "closed.once.running"));
        Assert.False(run.Said.ContainsKey("returned"));
        Assert.Equal(75, run.Result.ExitCode);
    }

    [Fact] // reference source Open/Close: the application keeps running until the last Close
    public void Close_shuts_down_on_the_last_one_only()
    {
        var run = Run("Close", null);
        Assert.Equal("1", Get(run, "closed.once.running"));
    }

    [Fact] // reference source ReduceAppDomainsCount (processModel maxAppDomains): the least busy application is shut down while the count is at the limit
    public void Over_the_application_limit_one_is_shut_down()
    {
        var run = Run("Reduce", Web("<httpRuntime fcnMode=\"Disabled\"/>", "<add key=\"aspnet:DisableFcnDaclRead\" value=\"true\"/>"));
        Assert.Equal("Disabled", Get(run, "FcnMode"));
        Assert.Equal("True", Get(run, "FcnSkipReadAndCacheDacls"));
        Assert.Equal("1", Get(run, "underLimit.count"));
        Assert.Equal("0", Get(run, "overLimit.count"));
        Assert.Equal(75, run.Result.ExitCode);
    }

    [Fact] // reference source RemoveFromTableIfRuntimeExists: the application leaves the table (no longer listed or shut down by the manager), running on
    public void An_application_removed_from_the_table_is_no_longer_managed()
    {
        var run = Run("Remove", Web("<httpRuntime targetFramework=\"4.5\"/>", "<add key=\"aspnet:DisableFcnDaclRead\" value=\"true\"/>"));
        Assert.Equal("0", Get(run, "running"));
        Assert.Equal("0", Get(run, "count"));
        Assert.Equal("True", Get(run, "IsIdle"));
        Assert.Equal("0", Get(run, "infos"));
        Assert.Equal("1", Get(run, "suspended"));
        Assert.Equal("True", Get(run, "shutdown.ignored"));
        // the port's choice: one HostingEnvironment per process (.NET Framework makes another AppDomain)
        Assert.Equal("InvalidOperationException", Get(run, "second"));
        Assert.Equal(0, run.Result.ExitCode);
    }

    [Fact] // docs ApplicationHost.CreateApplicationHost: the host object in the application at the virtual and physical paths
    public void CreateApplicationHost_makes_the_host_object()
    {
        var run = Run("ApplicationHostScenario", null);
        Assert.Equal("Registered", Get(run, "type"));
        Assert.Equal("/app", Get(run, "virtualPath"));
        Assert.EndsWith(Path.DirectorySeparatorChar.ToString(), Get(run, "physicalPath"));
        // the port's choice: listed by the manager (the reference source hides it, HideFromAppManager): the process's one
        // HostingEnvironment, which WebFormsMiddleware's CreateObject finds there after its BuildManagerHost's
        Assert.Equal("1", Get(run, "running"));
    }

    [Fact] // the port's choice: the application in a load context of its own (collectible) where asked, its types System.Web's
    public void An_application_in_a_load_context_of_its_own()
    {
        var run = Run("NewLoadContext", null);
        Assert.Equal("Registered", Get(run, "type"));
        Assert.Equal("InvalidOperationException", Get(run, "second"));
        Assert.EndsWith(Path.DirectorySeparatorChar.ToString(), Get(run, "physicalPath"));
    }

    [Fact] // reference source CreateInstanceInNewWorkerAppDomain (AppHost.CreateAppHost's, .NET Framework's machine.config): the instance in the application
    public void The_legacy_host_object_is_created()
    {
        var run = Run("NewWorkerAppDomain", null);
        Assert.Equal("Registered", Get(run, "type"));
    }

    [Fact] // reference source: precompiling for deployment uses another application ID, by updatability (_precompile_u, _precompile; VSWhidbey 383239)
    public void Precompilation_has_an_application_ID_of_its_own()
    {
        var run = Run("Precompile", null);
        Assert.EndsWith("_precompile_u", Get(run, "updatable"));
        Assert.Equal("none", Get(run, "initializationException"));
        Assert.EndsWith("_precompile", Get(run, "fixed"));
        Assert.EndsWith("_precompile_u", Get(run, "noParameter"));
        Assert.DoesNotContain("_precompile", Get(run, "plain"));
        Assert.Equal("InvalidOperationException", Get(run, "fixed.create"));
    }

    [Fact] // reference source OnUnhandledException/RecordFatalException: a terminating exception of the domain recorded, the first one kept
    public void The_first_fatal_exception_is_kept()
    {
        var run = Run("Fatal", null);
        Assert.Equal("True", Get(run, "notTerminating"));
        Assert.Equal("True", Get(run, "notException"));
        Assert.Equal("True", Get(run, "noDomain"));
        Assert.Equal("True", Get(run, "recorded"));
        Assert.Equal("True", Get(run, "first.kept"));
    }

    [Fact] // the port's own: data by load context (an application's, where .NET Framework has AppDomain.SetData), the application ID a hash of its paths
    public void Load_context_data_and_application_ids()
    {
        var context = new AssemblyLoadContext("foc-data", isCollectible: true);
        try
        {
            ApplicationManager.SetLoadContextData("key", "value", context);
            Assert.Equal("value", ApplicationManager.GetLoadContextData("key", context));
            Assert.Null(ApplicationManager.GetLoadContextData("other", context));
            ApplicationManager.SetLoadContextData("foc-test", 1, typeof(ApplicationManagerTests).Assembly);
            Assert.Equal(1, ApplicationManager.GetLoadContextData("foc-test", typeof(ApplicationManagerTests).Assembly));
            Assert.Equal(1, ApplicationManager.GetLoadContextData("foc-test"));
            Assert.Same(AssemblyLoadContext.Default, ApplicationManager.GetLoadContext());
            Assert.Same(AssemblyLoadContext.Default, ApplicationManager.GetLoadContext(typeof(HttpRuntime).Assembly));
        }
        finally { ApplicationManager.ContextData.TryRemove(context, out _); context.Unload(); }
        Assert.Equal(ApplicationManager.CreateApplicationId("/", "c:\\a\\"), ApplicationManager.CreateApplicationId("/", "c:\\a\\"));
        Assert.NotEqual(ApplicationManager.CreateApplicationId("/", "c:\\a\\"), ApplicationManager.CreateApplicationId("/b", "c:\\a\\"));
    }
}
