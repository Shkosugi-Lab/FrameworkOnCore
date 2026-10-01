//------------------------------------------------------------------------------
// <copyright file="AppSettingsSection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Configuration {
    using System;
    using System.Xml;
    using System.Configuration;
    using System.Collections.Specialized;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;

    public sealed class AppSettingsSection : ConfigurationSection {
        private volatile static ConfigurationPropertyCollection  s_properties;
        private volatile static ConfigurationProperty            s_propAppSettings;
        private volatile static ConfigurationProperty            s_propFile;

        private KeyValueInternalCollection _KeyValueCollection = null;

        private static ConfigurationPropertyCollection EnsureStaticPropertyBag() {
            if (s_properties == null) {
                ConfigurationProperty propAppSettings = new ConfigurationProperty(null, typeof(KeyValueConfigurationCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
                ConfigurationProperty propFile = new ConfigurationProperty("file", typeof(string), String.Empty, ConfigurationPropertyOptions.None);

                ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
                properties.Add(propAppSettings);
                properties.Add(propFile);

                s_propAppSettings = propAppSettings;
                s_propFile = propFile;
                s_properties = properties;
            }

            return s_properties;
        }

        public AppSettingsSection() {
            EnsureStaticPropertyBag();
        }

        protected internal override ConfigurationPropertyCollection Properties {
            get {
                return EnsureStaticPropertyBag();
            }
        }

        protected internal override object GetRuntimeObject() {
            SetReadOnly();
#if !NETFRAMEWORK
            return WithEnvironment(this.InternalSettings);
#else
            return this.InternalSettings;            // return the read only object
#endif
        }

#if !NETFRAMEWORK
        // The deployment's settings from the environment, as Azure App Service gives them to .NET
        // Framework applications: APPSETTING_<key> sets appSettings' <key> (and adds it if absent). A
        // container (-e) or a systemd unit (Environment=) sets them; web.config stays as it is. The
        // application gets a read-only copy with them (the configuration's elements are read-only).
        internal const string EnvironmentPrefix = "APPSETTING_";

        static NameValueCollection WithEnvironment(NameValueCollection settings) {
            SettingsWithEnvironment copy = null;
            foreach (System.Collections.DictionaryEntry variable in Environment.GetEnvironmentVariables()) {
                var name = (string)variable.Key;
                if (!name.StartsWith(EnvironmentPrefix, StringComparison.OrdinalIgnoreCase) || name.Length == EnvironmentPrefix.Length) continue;
                copy ??= new SettingsWithEnvironment(settings);
                copy[name.Substring(EnvironmentPrefix.Length)] = (string)variable.Value;
            }
            if (copy == null) return settings;
            copy.Seal();
            return copy;
        }

        sealed class SettingsWithEnvironment : NameValueCollection {
            internal SettingsWithEnvironment(NameValueCollection settings) : base(settings) { }
            internal void Seal() => IsReadOnly = true;
        }
#endif

        internal NameValueCollection InternalSettings {
            get {
                if (_KeyValueCollection == null) {
                    _KeyValueCollection = new KeyValueInternalCollection(this);
                }
                return (NameValueCollection)_KeyValueCollection;
            }
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public KeyValueConfigurationCollection Settings {
            get {
                return (KeyValueConfigurationCollection)base[s_propAppSettings];
            }
        }

        [ConfigurationProperty("file", DefaultValue = "")]
        public string File {
            get {
                string fileValue = (string)base[s_propFile];
                if (fileValue == null) {
                    return String.Empty;
                }
                return fileValue;
            }
            set {
                base[s_propFile] = value;
            }
        }
        protected internal override void Reset(ConfigurationElement parentSection) {
            _KeyValueCollection = null;
            base.Reset(parentSection);
            if (!String.IsNullOrEmpty((string)base[s_propFile])) { // don't inherit from the parent
                SetPropertyValue(s_propFile,null,true); // ignore the lock to prevent inheritence
            }
        }


        protected internal override bool IsModified() {
            return base.IsModified();
        }

        protected internal override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode) {
            return base.SerializeSection(parentElement, name, saveMode);
        }

        [SuppressMessage("Microsoft.Security.Xml", "CA3074:ReviewClassesDerivedFromXmlTextReader", Justification="Reading trusted input")]
        protected internal override void DeserializeElement(XmlReader reader, bool serializeCollectionKey) {
            string ElementName = reader.Name;

            base.DeserializeElement(reader, serializeCollectionKey);
            if ((File != null) && (File.Length > 0)) {
                string sourceFileFullPath;
                string configFileDirectory;
                string configFile;

                // Determine file location
                configFile = ElementInformation.Source;

                if (String.IsNullOrEmpty(configFile)) {
                    sourceFileFullPath = Internal.InternalConfigHost.FrameworkRelativePath(File);
                }
                else {
                    configFileDirectory = System.IO.Path.GetDirectoryName(configFile);
                    sourceFileFullPath = System.IO.Path.Combine(configFileDirectory, Internal.InternalConfigHost.FrameworkRelativePath(File));
                }

                if (System.IO.File.Exists(sourceFileFullPath)) {
                    int lineOffset = 0;
                    string rawXml = null;

                    using (Stream sourceFileStream = new FileStream(sourceFileFullPath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                        using (XmlUtil xmlUtil = new XmlUtil(sourceFileStream, sourceFileFullPath, true)) {
                            if (xmlUtil.Reader.Name != ElementName) {
                                throw new ConfigurationErrorsException(
                                        SR.GetString(SR.Config_name_value_file_section_file_invalid_root, ElementName),
                                        xmlUtil);
                            }

                            lineOffset = xmlUtil.Reader.LineNumber;
                            rawXml = xmlUtil.CopySection();

                            // Detect if there is any XML left over after the section
                            while (!xmlUtil.Reader.EOF) {
                                XmlNodeType t = xmlUtil.Reader.NodeType;
                                if (t != XmlNodeType.Comment) {
                                    throw new ConfigurationErrorsException(SR.GetString(SR.Config_source_file_format), xmlUtil);
                                }

                                xmlUtil.Reader.Read();
                            }
                        }
                    }

                    ConfigXmlReader internalReader = new ConfigXmlReader(rawXml, sourceFileFullPath, lineOffset);
                    internalReader.Read();
                    if (internalReader.MoveToNextAttribute()) {
                        throw new ConfigurationErrorsException(SR.GetString(SR.Config_base_unrecognized_attribute, internalReader.Name), (XmlReader)internalReader);
                    }

                    internalReader.MoveToElement();

                    base.DeserializeElement(internalReader, serializeCollectionKey);
                }
            }
        }
    }
}

