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
        /// The application's web.config's defaultDocument (system.webServer), as IIS reads it over its own list: enabled
        /// false, none; its files' clear, remove and add, the ones added before the ones it has (IIS's schema: the
        /// collection merges with mergeAppend false). It was not read.
        /// </summary>
        void ReadDefaultDocuments(string physicalPath)
        {
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
            if (string.Equals((string)section.Attribute("enabled"), "false", StringComparison.OrdinalIgnoreCase))
            {
                DefaultDocuments = Array.Empty<string>();
                return;
            }
            var inherited = DefaultDocuments.ToList();
            var added = new List<string>();
            foreach (var element in section.Element("files")?.Elements() ?? Enumerable.Empty<System.Xml.Linq.XElement>())
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
                    case "add" when !string.IsNullOrEmpty(value):
                        if (!added.Concat(inherited).Contains(value, StringComparer.OrdinalIgnoreCase)) added.Add(value);
                        break;
                }
            }
            DefaultDocuments = added.Concat(inherited).ToArray();
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
            var path = context.Request.Path.Value;
            if (string.IsNullOrEmpty(path) || path.EndsWith('/') || AspNetCoreWorkerRequest.IsHidden(path)) return false;
            if (HandleExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))) return false;
            string mapped;
            try { mapped = MapPath(context.Request.PathBase.Value + path); }
            catch (HttpException) { return false; }
            catch (ArgumentException) { return false; }
            if (mapped == null || !Directory.Exists(mapped)) return false;
            context.Response.StatusCode = 301;
            context.Response.Headers.Location = (context.Request.PathBase + context.Request.Path).ToUriComponent() + "/" + context.Request.QueryString.Value;
            return true;
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
            if (HandleExtensions.Any(ext => path.LastIndexOf('/') is var index and > 0 && path[..index].EndsWith(ext, StringComparison.OrdinalIgnoreCase))) 
                return true;
            if (ProhibitedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))) return false;

            var mappedPath = MapPath(fullpath);
            if (Directory.Exists(mappedPath))
            {
				// The first of the names a file of the folder has, without regard to case (as on Windows), served by the name
				// as the list has it, as IIS does (/Sub/ is Request.Path /Sub/default.aspx, whatever the file's case).
				var files = Directory.EnumerateFiles(mappedPath).Select(Path.GetFileName).ToList();
				var defaultDoc = DefaultDocuments
                    .Where(doc => files.Contains(doc, StringComparer.OrdinalIgnoreCase))
                    .Select(doc => new { Virtual = $"{pathWithoutSlash}/{doc}" })
                    .FirstOrDefault();
                if (defaultDoc != null)
                {
                    context.Request.Path = defaultDoc.Virtual;
                    return true;
                }
            }

			return HandleAllRequestsWithWebForms || File.Exists(mappedPath) &&
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