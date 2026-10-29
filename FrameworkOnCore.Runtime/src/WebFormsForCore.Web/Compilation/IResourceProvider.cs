//------------------------------------------------------------------------------
// <copyright file="IResourceProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Web.Compilation {
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.IO;
    using System.Security.Permissions;
    using System.CodeDom;
    using System.Globalization;
    using System.Resources;
    using System.Web.Compilation;
    using System.Web.Util;
    using System.Web.UI;


    /*
     * Basic interface to access and enumerate resources
     */
    public interface IResourceProvider {

        /*
         * Retrieve a resource object for the passed in key and culture
         */
        object GetObject(string resourceKey, CultureInfo culture);

        /*
         * Returns a reader that enumerates all the neutral resources for this provider
         */
        IResourceReader ResourceReader { get; }
    }

    /*
     * Base class for resource providers based on a standard ResourceManager
     */
    internal abstract class BaseResXResourceProvider: IResourceProvider {

        private ResourceManager _resourceManager;

        ///// IResourceProvider implementation

        public virtual object GetObject(string resourceKey, CultureInfo culture) {

            // Attempt to get the resource manager
            EnsureResourceManager();

            // If we couldn't get a resource manager, return null
            if (_resourceManager == null)
                return null;

            if (culture == null)
                culture = CultureInfo.CurrentUICulture;

#if !NETFRAMEWORK
            // FrameworkOnCore: .NET Framework's parents of the Chinese cultures, zh-CN -> zh-CHS -> zh-Hans and zh-TW (zh-HK,
            // zh-MO) -> zh-CHT -> zh-Hant; .NET's skip the old names (zh-CN -> zh-Hans). An application's Strings.zh-CHS.resx
            // (App_GlobalResources, App_LocalResources) is found for zh-CN, as on .NET Framework: the chain walked culture by
            // culture, by a ResourceManager of its own (one that falls back caches the parent's set under the culture's name).
            if (HasLegacyParent(culture)) {
                if (_exact == null) _exact = CreateResourceManager();
                for (var c = culture; c != null; c = FrameworkParent(c)) {
                    if (ResourceSetOf(c)?.GetObject(resourceKey) is { } value) return value;
                    if (c.Name.Length == 0) break;
                }
                return null;
            }
#endif
            return _resourceManager.GetObject(resourceKey, culture);
        }

#if !NETFRAMEWORK
        private ResourceManager _exact;

        ResourceSet ResourceSetOf(CultureInfo culture) {
            try {
                return _exact.GetResourceSet(culture, createIfNotExists: true, tryParents: false);
            }
            catch (MissingManifestResourceException) {
                return null;
            }
        }

        // A culture's parent as .NET Framework's (FrameworkOnCore.Compat's FrameworkCultures.Parent is the same).
        static CultureInfo FrameworkParent(CultureInfo culture) {
            switch (culture.Name.ToLowerInvariant()) {
                case "zh-cn": case "zh-sg": return CultureInfo.GetCultureInfo("zh-CHS");
                case "zh-tw": case "zh-hk": case "zh-mo": return CultureInfo.GetCultureInfo("zh-CHT");
                // (ICU's parents of the old names are not these)
                case "zh-chs": return CultureInfo.GetCultureInfo("zh-Hans");
                case "zh-cht": return CultureInfo.GetCultureInfo("zh-Hant");
                default: return culture.Parent;
            }
        }

        static bool HasLegacyParent(CultureInfo culture) {
            for (var c = culture; c != null && c.Name.Length > 0; c = c.Parent) {
                if (FrameworkParent(c).Name != c.Parent.Name) return true;
                if (c.Parent.Name == c.Name) break;
            }
            return false;
        }
#endif

        public virtual IResourceReader ResourceReader { get { return null; } }

        ///// End of IResourceProvider implementation


        protected abstract ResourceManager CreateResourceManager();

        private void EnsureResourceManager() {
            if (_resourceManager != null)
                return;

            _resourceManager = CreateResourceManager();
        }
    }

    /*
     * ResourceManager based provider for application resources.
     */
    internal class GlobalResXResourceProvider : BaseResXResourceProvider {

        private string _classKey;

        internal GlobalResXResourceProvider(string classKey) {
            _classKey = classKey;
        }

        protected override ResourceManager CreateResourceManager() {

            string fullClassName = BaseResourcesBuildProvider.DefaultResourcesNamespace +
                "." + _classKey;

            // If there is no app resource assembly, return null
            if (BuildManager.AppResourcesAssembly == null)
                return null;

#if WebFormsForCore
            // the resources' objects other than strings and primitives read by the port's formatter (SerializedResources)
            ResourceManager resourceManager = new ResourceManager(fullClassName,
                BuildManager.AppResourcesAssembly, SerializedResources.SetType);
#else
            ResourceManager resourceManager = new ResourceManager(fullClassName,
                BuildManager.AppResourcesAssembly);
#endif
            resourceManager.IgnoreCase = true;

            return resourceManager;
        }

        public override IResourceReader ResourceReader {
            get {
                // App resources don't support implicit resources, so the IResourceReader
                // should never be needed
                throw new NotSupportedException();
            }
        }
    }

    /*
     * ResourceManager based provider for page (local) resources.
     */
    internal class LocalResXResourceProvider : BaseResXResourceProvider {

        private VirtualPath _virtualPath;

        internal LocalResXResourceProvider(VirtualPath virtualPath) {
            _virtualPath = virtualPath;
        }

        protected override ResourceManager CreateResourceManager() {

            ResourceManager resourceManager = null;

            Assembly pageResAssembly = GetLocalResourceAssembly();

            if (pageResAssembly != null) {
                string fileName = _virtualPath.FileName;

#if WebFormsForCore
                resourceManager = new ResourceManager(fileName, pageResAssembly, SerializedResources.SetType);
#else
                resourceManager = new ResourceManager(fileName, pageResAssembly);
#endif
                resourceManager.IgnoreCase = true;
            }
            else {
                throw new InvalidOperationException(SR.GetString(SR.ResourceExpresionBuilder_PageResourceNotFound));
            }

            return resourceManager;
        }

        public override IResourceReader ResourceReader {
            get {
                // Get the local resource assembly for this page
                Assembly pageResAssembly = GetLocalResourceAssembly();

                if (pageResAssembly == null)
                    return null;

                // Get the name of the embedded .resource file for this page
                string resourceFileName = _virtualPath.FileName + ".resources";

                // Make it lower case, since GetManifestResourceStream is case sensitive
                resourceFileName = resourceFileName.ToLower(CultureInfo.InvariantCulture);

                // Get the resource stream from the resource assembly
                Stream resourceStream = pageResAssembly.GetManifestResourceStream(resourceFileName);

                // If this page has no resources, return null
                if (resourceStream == null)
                    return null;

#if WebFormsForCore
                return new SerializedResourceReader(resourceStream);
#else
                return new ResourceReader(resourceStream);
#endif
            }
        }

        // Need to Assert here in order to access the codegen dir (VSWhidbey 387312)
        [PermissionSet(SecurityAction.Assert, Unrestricted=true)]
        private Assembly GetLocalResourceAssembly() {
            // Remove the page file name to get its directory
            VirtualPath virtualDir = _virtualPath.Parent;

            // Get the name of the local resource assembly
            string cacheKey = BuildManager.GetLocalResourcesAssemblyName(virtualDir);
            BuildResult result = BuildManager.GetBuildResultFromCache(cacheKey);

            if (result != null) {
                return ((BuildResultCompiledAssembly)result).ResultAssembly;
            }

            return null;
        }
    }

}

