//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

namespace System.Web.Services.Configuration
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Security.Permissions;
    using System.Threading;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Xml;
    using System.Runtime.CompilerServices;

    public sealed class WsdlHelpGeneratorElement : ConfigurationElement
    {
        // These three constructors are used by the configuration system. 
        public WsdlHelpGeneratorElement() : base()
        {
            this.properties.Add(this.href);
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        string GetConfigurationDirectory()
        {
            PartialTrustHelpers.FailIfInPartialTrustOutsideAspNet();
            return HttpRuntime.MachineConfigurationDirectory;
        }

        internal string HelpGeneratorVirtualPath
        {
            get { return this.virtualPath + this.Href; }
        }

        internal string HelpGeneratorPath
        {
            get { return Path.Combine(this.actualPath, this.Href); }
        }

        [ConfigurationProperty("href", IsRequired = true)]
        public string Href
        {
            get { return (string)base[this.href]; }
            set 
            {
                if (value == null) 
                {
                    value = string.Empty;
                }
                if (this.needToValidateHref && value.Length > 0)
                {
                    CheckIOReadPermission(this.actualPath, value);
                }
                base[this.href] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return this.properties; }
        }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            PartialTrustHelpers.FailIfInPartialTrustOutsideAspNet();

            base.DeserializeElement(reader, serializeCollectionKey);
            

            // Update paths
            // If we're not running in the context of a web application then skip this setting.
            ContextInformation context = this.EvaluationContext;
            WebContext webContext = context.HostingContext as WebContext;
            if (webContext == null)
            {
                return;
            }

            if (this.Href.Length == 0)
                return;

            string tempVirtualPath = webContext.Path;
            string path = null;

            // If the help page is not in the web app directory hierarchy (the case 
            // for those specified in machine.config like DefaultWsdlHelpGenerator.aspx)
            // then we can't construct a true virtual path. This means that certain web 
            // form features that rely on relative paths won't work.

            if (tempVirtualPath == null)
            {
                tempVirtualPath = HostingEnvironment.ApplicationVirtualPath;
                if (tempVirtualPath == null)
                    tempVirtualPath = "";
                path = GetConfigurationDirectory();
            }
            else
            {
                path = HostingEnvironment.MapPath(tempVirtualPath);
            }
            if (!tempVirtualPath.EndsWith("/", StringComparison.Ordinal))
            {
                tempVirtualPath += "/";
            }
            CheckIOReadPermission(path, this.Href);
            this.actualPath = path;
            this.virtualPath = tempVirtualPath;
            this.needToValidateHref = true;
        }

        protected override void Reset(ConfigurationElement parentElement)
        {
            PartialTrustHelpers.FailIfInPartialTrustOutsideAspNet();

            WsdlHelpGeneratorElement parent = (WsdlHelpGeneratorElement)parentElement;
            ContextInformation context = this.EvaluationContext;
            WebContext webContext = context.HostingContext as WebContext;
            if (webContext != null)
            {
                string tempVirtualPath = webContext.Path;
                bool isMachineConfig = tempVirtualPath == null;
                this.actualPath = parent.actualPath;

                if (isMachineConfig)
                {
                    tempVirtualPath = HostingEnvironment.ApplicationVirtualPath;
                }

                if ((tempVirtualPath != null) && !tempVirtualPath.EndsWith("/", StringComparison.Ordinal))
                {
                    tempVirtualPath += "/";
                }

                if ((tempVirtualPath == null) && (parentElement != null))
                {
                    this.virtualPath = parent.virtualPath;
                }
                else if (tempVirtualPath != null)
                {
                    this.virtualPath = tempVirtualPath;
                }
            }

            base.Reset(parentElement);
        }

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        internal void SetDefaults()
        {
            PartialTrustHelpers.FailIfInPartialTrustOutsideAspNet();
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                this.virtualPath = HostingEnvironment.ApplicationVirtualPath;
            }
            this.actualPath = this.GetConfigurationDirectory();
            if ((this.virtualPath != null) && (!this.virtualPath.EndsWith("/", StringComparison.Ordinal)))
            {
                this.virtualPath += "/";
            }
            if ((this.actualPath != null) && (!this.actualPath.EndsWith(@"\", StringComparison.Ordinal)))
            {
                this.actualPath += Path.DirectorySeparatorChar;
            }
            this.Href = "DefaultWsdlHelpGenerator.aspx";
#if !NETFRAMEWORK
            WriteDefaultHelpGenerator(this.actualPath);
#endif
            CheckIOReadPermission(this.actualPath, this.Href);
            this.needToValidateHref = true;
        }

#if !NETFRAMEWORK
        static readonly object writeLock = new object();

        // .NET Framework's CONFIG folder has DefaultWsdlHelpGenerator.aspx; FrameworkOnCore's (App_Data, where machine.config is
        // written) gets FrameworkOnCore's own from the assembly, written again when it differs (a package update). Without it,
        // the service's help page was empty.
        static void WriteDefaultHelpGenerator(string directory)
        {
            if (string.IsNullOrEmpty(directory)) return;
            try
            {
                string text;
                using (var stream = typeof(WsdlHelpGeneratorElement).Assembly.GetManifestResourceStream("DefaultWsdlHelpGenerator.aspx"))
                {
                    if (stream == null) return;
                    using (var reader = new StreamReader(stream)) text = reader.ReadToEnd();
                }
                var file = Path.Combine(directory, "DefaultWsdlHelpGenerator.aspx");
                lock (writeLock)
                {
                    if (!File.Exists(file) || File.ReadAllText(file) != text)
                    {
                        Directory.CreateDirectory(directory);
                        File.WriteAllText(file, text);
                    }
                }
            }
            catch (IOException)
            {
                // A folder the application cannot write: the page is left out, as before.
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
#endif

        static void CheckIOReadPermission(string path, string file) 
        {
            if (path == null)
                return;
            string fullPath = Path.GetFullPath(Path.Combine(path, file));
            new FileIOPermission(FileIOPermissionAccess.Read, fullPath).Demand();
        }

        ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        readonly ConfigurationProperty href = new ConfigurationProperty("href", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        string virtualPath = null;
        string actualPath = null;
        bool needToValidateHref = false;
    }
}



