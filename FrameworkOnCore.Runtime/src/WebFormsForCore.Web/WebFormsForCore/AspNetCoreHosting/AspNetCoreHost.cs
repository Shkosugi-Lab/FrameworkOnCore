#if NETCOREAPP

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Loader;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Util;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
    public class AspNetCoreHost : MarshalByRefObject, IRegisteredObject
    {
        private bool disableDirectoryListing;

        private string installPath;

        private string lowerCasedClientScriptPathWithTrailingSlash;

        private string lowerCasedVirtualPath;

        private string lowerCasedVirtualPathWithTrailingSlash;

        private volatile int pendingCallsCount;

        private string physicalClientScriptPath;

        private string physicalPath;

        private int port;

        private bool requireAuthentication;

		private IntPtr processToken;

		private string processUser;

        private string virtualPath;
        public bool HandleAllRequestsWithWebForms { get; set; } = false;
		public string[] HandleExtensions { get; set; } = new string[] { ".aspx", ".ashx", ".asmx", ".axd" };
        // Not served (404): IIS's request filtering's denied extensions (applicationHost.config, 404.7: .config, the sources,
        // the projects, the databases' files .mdf/.ldf/.mdb, .resources, ...), and .NET's own (Razor's, appsettings.json).
        // It had a few of them only: an App_Data database (.mdf, .ldf) or a project file was served.
        public string[] ProhibitedExtensions { get; set; } = new string[]
        {
            ".asa", ".asax", ".ascx", ".master", ".skin", ".browser", ".sitemap", ".config", ".cs", ".csproj", ".vb", ".vbproj",
            ".webinfo", ".licx", ".resx", ".resources", ".resource", ".mdb", ".vjsproj", ".java", ".jsl", ".ldb", ".dsdgm", ".ssdgm",
            ".lsad", ".ssmap", ".cd", ".dsprototype", ".lsaprototype", ".sdm", ".sdmDocument", ".mdf", ".ldf", ".ad", ".dd",
            ".ldd", ".sd", ".adprototype", ".lddprototype", ".exclude", ".refresh", ".compiled", ".msgx", ".vsdisco", ".rules",
            ".razor", ".cshtml", ".vbhtml", "appsettings.json",
        };
        // IIS's defaultDocument, in its order and with its names (Default.asp, classic ASP, left out): a folder's file is found by
        // them without regard to case. It was another order, "iisstart.htm" was "iistart.htm".
        public string[] DefaultDocuments { get; set; } = new string[] { "Default.htm", "index.htm", "index.html", "iisstart.htm", "default.aspx" };
        public bool UseClassicMode = false;
        public Compilation.ClientBuildManager ClientBuildManager { get; set; } 
		public AppDomain AppDomain
        {
            get { return AppDomain.CurrentDomain; }
        }
		public AssemblyLoadContext LoadContext
		{
			get { return ApplicationManager.GetLoadContext(Assembly.GetCallingAssembly()); }
		}

		public AspNetCoreHost()
        {
            HostingEnvironment.RegisterObject(this);

        }

        public bool DisableDirectoryListing
        {
            get { return disableDirectoryListing; }
        }

        public string InstallPath
        {
            get { return installPath; }
        }

        public string NormalizedClientScriptPath
        {
            get { return lowerCasedClientScriptPathWithTrailingSlash; }
        }

        public string NormalizedVirtualPath
        {
            get { return lowerCasedVirtualPathWithTrailingSlash; }
        }

        public string PhysicalClientScriptPath
        {
            get { return physicalClientScriptPath; }
        }

        public string PhysicalPath
        {
            get { return physicalPath; }
        }

        public int Port
        {
            get { return port; }
        }

        public bool RequireAuthentication
        {
            get { return requireAuthentication; }
        }

        public string VirtualPath
        {
            get { return virtualPath; }
        }

        public HostingEnvironment HostingEnvironment { get; set; }

        #region IRegisteredObject Members

        void IRegisteredObject.Stop(bool immediate)
        {
            // Make sure all the pending calls complete before this Object is unregistered.
            WaitForPendingCallsToFinish();

            HostingEnvironment.UnregisterObject(this);

            Thread.Sleep(100);
            HttpRuntime.Close();
            Thread.Sleep(100);
        }

        #endregion

        public void Configure(string virtualPath, string physicalPath,
                              bool requireAuthentication)
        {
            Configure(virtualPath, physicalPath, requireAuthentication, false);
        }

        public void Configure(string virtualPath, string physicalPath)
        {
            Configure(virtualPath, physicalPath, false, false);
        }

        /// <summary>
        /// The defaultDocument IIS reads (system.webServer): the original server's (IisProfile: the server's, the site's), then
        /// the application's web.config's, each over the list before it: enabled false, none; its files' clear, remove and add,
        /// the ones added before the ones it has (IIS's schema: the collection merges with mergeAppend false). Without a
        /// profile, IIS's own list. Default.asp left out (classic ASP: none here).
        /// </summary>
        void ReadDefaultDocuments(string physicalPath)
        {
            var profile = IisProfile.Of(physicalPath);
            var serverSections = System.Xml.Linq.Extensions.Elements(IisProfile.WebServer(profile), "defaultDocument").ToList();
            if (serverSections.Any(s => s.Element("files") != null)) DefaultDocuments = Array.Empty<string>();
            foreach (var serverSection in serverSections) ApplyDefaultDocuments(serverSection);
            string config;
            try
            {
                config = Directory.EnumerateFiles(physicalPath, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
            }
            catch (IOException) { return; }
            if (config == null) return;
            System.Xml.Linq.XElement section;
            try { section = System.Xml.Linq.XDocument.Load(config).Root?.Element("system.webServer")?.Element("defaultDocument"); }
            catch (System.Xml.XmlException) { return; }
            if (section == null) return;
            ApplyDefaultDocuments(section);
        }

        void ApplyDefaultDocuments(System.Xml.Linq.XElement section) => DefaultDocuments = ApplyDefaultDocuments(DefaultDocuments, section);

        readonly System.Collections.Concurrent.ConcurrentDictionary<string, string[]> defaultDocumentsByFolder =
            new System.Collections.Concurrent.ConcurrentDictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The default documents of the URL's folder, as IIS has them for it: IIS's own list, the original server's, the
        /// application's web.config's, its locations' and its folders' web.config files' down to the folder (IisConfiguration).
        /// </summary>
        internal string[] DefaultDocumentsFor(string path)
        {
            var root = physicalPath;
            if (root == null) return DefaultDocuments;
            return defaultDocumentsByFolder.GetOrAdd(IisConfiguration.Deepest(root, path), _ =>
            {
                var sections = IisConfiguration.For(root, path)
                    .Select(s => (s.Server, Section: IisConfiguration.Child(s.WebServer, "defaultDocument"))).Where(s => s.Section != null).ToList();
                // The original server's list is the one IIS starts from (applicationHost.config's), not IIS's own.
                var documents = sections.Any(s => s.Server && IisConfiguration.Child(s.Section, "files") != null) ? Array.Empty<string>() : IisDefaultDocuments;
                foreach (var (_, section) in sections) documents = ApplyDefaultDocuments(documents, section);
                return documents;
            });
        }

        static readonly string[] IisDefaultDocuments = { "Default.htm", "index.htm", "index.html", "iisstart.htm", "default.aspx" };

        static string[] ApplyDefaultDocuments(string[] current, System.Xml.Linq.XElement section)
        {
            var enabled = (string)section.Attribute("enabled");
            if (string.Equals(enabled, "false", StringComparison.OrdinalIgnoreCase)) return Array.Empty<string>();
            var inherited = current.ToList();
            var added = new List<string>();
            foreach (var element in IisConfiguration.Child(section, "files")?.Elements() ?? Enumerable.Empty<System.Xml.Linq.XElement>())
            {
                var value = ((string)element.Attribute("value"))?.Trim();
                switch (element.Name.LocalName)
                {
                    case "clear":
                        inherited.Clear();
                        added.Clear();
                        break;
                    case "remove" when !string.IsNullOrEmpty(value):
                        inherited.RemoveAll(name => name.Equals(value, StringComparison.OrdinalIgnoreCase));
                        added.RemoveAll(name => name.Equals(value, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "add" when !string.IsNullOrEmpty(value) && !value.EndsWith(".asp", StringComparison.OrdinalIgnoreCase):
                        if (!added.Concat(inherited).Contains(value, StringComparer.OrdinalIgnoreCase)) added.Add(value);
                        break;
                }
            }
            return added.Concat(inherited).ToArray();
        }

        public void Configure(string virtualPath, string physicalPath,
                              bool requireAuthentication, bool disableDirectoryListing)
        {
            installPath = null;
            if (!virtualPath.StartsWith('/')) virtualPath = "/" + virtualPath;
            this.virtualPath = virtualPath;
            this.requireAuthentication = requireAuthentication;
            this.disableDirectoryListing = disableDirectoryListing;
            lowerCasedVirtualPath = CultureInfo.InvariantCulture.TextInfo.ToLower(virtualPath);
            lowerCasedVirtualPathWithTrailingSlash = virtualPath.EndsWith("/", StringComparison.Ordinal)
                                                          ? virtualPath
                                                          : virtualPath + "/";
            lowerCasedVirtualPathWithTrailingSlash =
                CultureInfo.InvariantCulture.TextInfo.ToLower(lowerCasedVirtualPathWithTrailingSlash);
            // FrameworkOnCore: with a separator at the end, as .NET Framework has the application's path
            // (Request.PhysicalApplicationPath, HttpRuntime.AppDomainAppPath: "C:\inetpub\app\"); applications add to it
            // (nopCommerce: PhysicalApplicationPath + "images\\thumbs", which went to "...appimages" otherwise).
            physicalPath = FileUtil.FixUpPhysicalDirectory(physicalPath);
            this.physicalPath = physicalPath;
            ReadDefaultDocuments(physicalPath);
            // The application's files' state kept until they change (as IIS's file cache): the static files' requests.
            FileCache.Watch(physicalPath);

			var assembly = GetType().Assembly;
            ApplicationManager.SetLoadContextData(".appPath", physicalPath, assembly);
			ApplicationManager.SetLoadContextData(".appVPath", virtualPath, assembly);

			// AppDomain.BaseDirectory is the application's root on .NET Framework (bin is its private bin
			// path), and applications find their files from it (DNN: Install\DotNetNuke.install.config);
			// on .NET it is the entry assembly's folder, bin. AppContext.BaseDirectory reads
			// APP_CONTEXT_BASE_DIRECTORY first: the root, once bin's path is kept (AppBinDirectory).
			_ = AppBinDirectory.PhysicalPath;
			AppContext.SetData("APP_CONTEXT_BASE_DIRECTORY", physicalPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar);

			physicalClientScriptPath = HttpRuntime.AspClientScriptPhysicalPath + Path.DirectorySeparatorChar;
            lowerCasedClientScriptPathWithTrailingSlash =
                CultureInfo.InvariantCulture.TextInfo.ToLower(HttpRuntime.AspClientScriptVirtualPath + "/");

        }

		private void ObtainProcessToken()
		{
			/*
			if (Interop.ImpersonateSelf(2))
			{
				Interop.OpenThreadToken(Interop.GetCurrentThread(), 0xf01ff, true, ref processToken);
				Interop.RevertToSelf();
				// ReSharper disable PossibleNullReferenceException
				// ReSharper restore PossibleNullReferenceException
			} */
		    processUser = WindowsIdentity.GetCurrent().Name;
		}


		public SecurityIdentifier GetProcessSid() => WindowsIdentity.GetCurrent().User;
        public IntPtr GetProcessToken() => WindowsIdentity.GetCurrent().Token;
        public string GetProcessUser() => WindowsIdentity.GetCurrent().Name;

        public override object InitializeLifetimeService()
        {
            // never expire the license
            return null;
        }

        public bool IsVirtualPathAppPath(string path)
        {
            if (path == null)
            {
                return false;
            }
            path = CultureInfo.InvariantCulture.TextInfo.ToLower(path);
            return (path == lowerCasedVirtualPath || path == lowerCasedVirtualPathWithTrailingSlash);
        }

        public bool IsVirtualPathInApp(string path, out bool isClientScriptPath)
        {
            isClientScriptPath = false;

            if (path == null)
            {
                return false;
            }

            if (virtualPath == "/" && path.StartsWith("/", StringComparison.Ordinal))
            {
                if (path.StartsWith(lowerCasedClientScriptPathWithTrailingSlash, StringComparison.Ordinal))
                {
                    isClientScriptPath = true;
                }
                return true;
            }

            path = CultureInfo.InvariantCulture.TextInfo.ToLower(path);

            if (path.StartsWith(lowerCasedVirtualPathWithTrailingSlash, StringComparison.Ordinal))
            {
                return true;
            }

            if (path == lowerCasedVirtualPath)
            {
                return true;
            }

            if (path.StartsWith(lowerCasedClientScriptPathWithTrailingSlash, StringComparison.Ordinal))
            {
                isClientScriptPath = true;
                return true;
            }

            return false;
        }

        public string MapPath(string path) => HostingEnvironment.MapPath(path);
        /// <summary>
        /// IIS's courtesy redirect: a folder asked for without its '/' is answered 301 to it with the '/' (the page's
        /// relative URLs are the folder's then). The default document was served without it.
        /// </summary>
        public bool RedirectToFolder(Core.HttpContext context)
        {
            var location = FolderLocation(context);
            if (location == null) return false;
            context.Response.StatusCode = 301;
            context.Response.Headers.Location = location;
            return true;
        }

        /// <summary>A folder's URL without its '/': where IIS redirects it to (the URL with it); null for another URL.</summary>
        internal string FolderLocation(Core.HttpContext context)
        {
            var path = context.Request.Path.Value;
            // (a name ending in whitespace: System.Web's VirtualPath trims it, the folder's name without it is not the URL's)
            if (string.IsNullOrEmpty(path) || path.EndsWith('/') || char.IsWhiteSpace(path[^1]) || AspNetCoreWorkerRequest.IsHidden(path)) return null;
            if (HandleExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))) return null;
            string mapped;
            try { mapped = MapPath(context.Request.PathBase.Value + path); }
            catch (HttpException) { return null; }
            catch (ArgumentException) { return null; }
            if (mapped == null || !FileCache.DirectoryExists(mapped)) return null;
            return (context.Request.PathBase + context.Request.Path).ToUriComponent() + "/" + context.Request.QueryString.Value;
        }

        /// <summary>
        /// A file of a type IIS's request filtering denies: its fileExtensions as applicationHost.config and web.config have them
        /// for the URL (IntegratedModeConfiguration.RequestFiltering: an application's own denied types, allowUnlisted false),
        /// and the port's own (ProhibitedExtensions beyond IIS's: .NET's appsettings.json, Razor's) unless the handled types'.
        /// web.config's fileExtensions were not read: a type an application denied there (.log, .bak) was served.
        /// </summary>
        public bool IsProhibited(Core.HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";
            var filtering = IntegratedModeConfiguration.RequestFiltering((context.Request.PathBase.Value ?? "") + path);
            var name = path.Substring(path.LastIndexOf('/') + 1);
            var dot = name.LastIndexOf('.');
            if (dot >= 0 && filtering.Extensions.TryGetValue(name.Substring(dot), out var allowed)) return !allowed;
            if (!filtering.AllowUnlisted) return true;
            if (HandleExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))) return false;
            return ProhibitedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase) &&
                !IntegratedModeConfiguration.IisDeniedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase));
        }

        public bool IsLegacyRequest(Core.HttpContext context)
        {
            var path = context.Request.Path.Value;
            var pathWithoutSlash = path;
            if (path.EndsWith('/')) pathWithoutSlash = path[..^1];
            if (path == "") path = "/";
            if (path != "/") path = pathWithoutSlash;

			var fullpath = $"{context.Request.PathBase.Value.TrimEnd('/')}/{path}";

			if (HandleExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase) )) 
                return true;
            // A handled file with path info of any depth (/a.aspx/x/y: the worker request's /a.aspx and /x/y, as IIS maps it).
            for (var index = path.IndexOf('/', 1); index > 0; index = path.IndexOf('/', index + 1))
                if (HandleExtensions.Any(ext => path[..index].EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    return true;
            if (ProhibitedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))) return false;

            // A path ASP.NET does not map (a name with a trailing dot or space, an 8.3 short name, a ".." above the root: its
            // UserMapPath's HttpException): none of the application's files, 404 as IIS (it was Kestrel's 500).
            string mappedPath;
            try { mappedPath = MapPath(fullpath); }
            catch (HttpException) { return false; }
            catch (ArgumentException) { return false; }
            if (FileCache.DirectoryExists(mappedPath))
            {
				// The first of the names a file of the folder has, without regard to case (as on Windows), served by the name
				// as the list has it, as IIS does (/Sub/ is Request.Path /Sub/default.aspx, whatever the file's case).
				var files = FileCache.FileNames(mappedPath);
				var defaultDoc = DefaultDocumentsFor((context.Request.PathBase.Value ?? "") + (pathWithoutSlash.Length == 0 ? "/" : pathWithoutSlash))
                    .Where(doc => files.Contains(doc, StringComparer.OrdinalIgnoreCase))
                    .Select(doc => new { Virtual = $"{pathWithoutSlash}/{doc}" })
                    .FirstOrDefault();
                if (defaultDoc != null)
                {
                    context.Request.Path = defaultDoc.Virtual;
                    return true;
                }
                // A folder without a default document: IIS's answer, its 403.14 (or its listing: directoryBrowse), from
                // IisStaticFile, not the empty 404 of the end of ASP.NET Core's pipeline.
                return Path.GetFullPath(mappedPath).TrimEnd(Path.DirectorySeparatorChar) != AppBinDirectory.PhysicalPath.TrimEnd(Path.DirectorySeparatorChar);
            }

			return HandleAllRequestsWithWebForms || FileCache.FileExists(mappedPath) &&
                Path.GetDirectoryName(mappedPath) != AppBinDirectory.PhysicalPath; // Do not serve bin_dotnet directory
		}

		public bool IsVirtualPathInApp(String path)
        {
            bool isClientScriptPath;
            return IsVirtualPathInApp(path, out isClientScriptPath);
        }

        public async Task ProcessRequest(Core.HttpContext context)
        {
            AddPendingCall();

            if (UseClassicMode) HttpRuntime.SetClassicPipeline();

            await new AspNetCoreWorkerRequest(this, context).Process();

            RemovePendingCall();
        }

        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        public void Shutdown()
        {
            HostingEnvironment.InitiateShutdown();
        }

        private void AddPendingCall()
        {
            //TODO: investigate this issue - ref var not volitile
#pragma warning disable 0420
            Interlocked.Increment(ref pendingCallsCount);
#pragma warning restore 0420
        }

        private void RemovePendingCall()
        {
            //TODO: investigate this issue - ref var not volitile
#pragma warning disable 0420
            Interlocked.Decrement(ref pendingCallsCount);
#pragma warning restore 0420
        }

        private void WaitForPendingCallsToFinish()
        {
            for (; ; )
            {
                if (pendingCallsCount <= 0)
                {
                    break;
                }

                Thread.Sleep(250);
            }
        }
    }
}

#endif