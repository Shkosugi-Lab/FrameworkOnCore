//------------------------------------------------------------------------------
// <copyright file="ConnectionStringSettings.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace System.Configuration {
    using System;
    using System.Xml;
    using System.Configuration;
    using System.Collections.Specialized;
    using System.Collections;
    using System.IO;
    using System.Text;

    public sealed class ConnectionStringSettings : ConfigurationElement {
        private static ConfigurationPropertyCollection _properties;
        private static readonly ConfigurationProperty _propName =
            new ConfigurationProperty( "name", typeof(string), null, null,
                                        ConfigurationProperty.NonEmptyStringValidator,
                                        ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
        private static readonly ConfigurationProperty _propConnectionString =
            new ConfigurationProperty("connectionString", typeof(string), "", ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propProviderName =
            new ConfigurationProperty("providerName", typeof(string), String.Empty, ConfigurationPropertyOptions.None);

        static ConnectionStringSettings() {
            // Property initialization
            _properties = new ConfigurationPropertyCollection();
            _properties.Add(_propName);
            _properties.Add(_propConnectionString);
            _properties.Add(_propProviderName);
        }
        public ConnectionStringSettings() {
        }

        public ConnectionStringSettings(String name, String connectionString)
            : this() {
            Name = name;
            ConnectionString = connectionString;
            // ProviderName = (string) _propProviderName.DefaultValue;
        }

        public ConnectionStringSettings(String name, String connectionString, String providerName)
            : this() {
            Name = name;
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        internal string Key {
            get {
                return Name;
            }
        }

        protected internal override ConfigurationPropertyCollection Properties {
            get {
                return _properties;
            }
        }

        [ConfigurationProperty("name", Options = ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey, DefaultValue = "")]
        public string Name {
            get {
                return (string)base[_propName];
            }
            set {
                base[_propName] = value;
            }
        }

        [ConfigurationProperty("connectionString", Options = ConfigurationPropertyOptions.IsRequired, DefaultValue = "")]
        public string ConnectionString {
            get {
#if NETFRAMEWORK
                return (string)base[_propConnectionString];
#else
                return ExpandDataDirectory((string)base[_propConnectionString]);
#endif
            }
            set {
                base[_propConnectionString] = value;
            }
        }

#if !NETFRAMEWORK
        // |DataDirectory| (AttachDBFilename=|DataDirectory|Site.mdf): on .NET Framework the data
        // providers expanded it (SqlClient, OleDb, Odbc) from the application domain's DataDirectory,
        // ~/App_Data in a web application. .NET's System.Data.SqlClient does not, and rejects the
        // connection string ("Invalid value for key 'attachdbfilename'"): the connection strings the
        // application reads are expanded here, as the provider would have. The configuration file keeps
        // the token (the stored value is not changed).
        const string DataDirectoryToken = "|DataDirectory|";

        static string ExpandDataDirectory(string connectionString) {
            if (connectionString == null || connectionString.IndexOf(DataDirectoryToken, StringComparison.OrdinalIgnoreCase) < 0)
                return connectionString;
            if (!(AppDomain.CurrentDomain.GetData("DataDirectory") is string dataDirectory) || dataDirectory.Length == 0)
                return connectionString;
            if (!dataDirectory.EndsWith("\\") && !dataDirectory.EndsWith("/"))
                dataDirectory += IO.Path.DirectorySeparatorChar;
            var text = new Text.StringBuilder();
            var start = 0;
            for (int at; (at = connectionString.IndexOf(DataDirectoryToken, start, StringComparison.OrdinalIgnoreCase)) >= 0; start = at + DataDirectoryToken.Length) {
                text.Append(connectionString, start, at - start).Append(dataDirectory);
                // "|DataDirectory|\Site.mdf": one separator
                var next = at + DataDirectoryToken.Length;
                if (next < connectionString.Length && (connectionString[next] == '\\' || connectionString[next] == '/')) at++;
            }
            return text.Append(connectionString, start, connectionString.Length - start).ToString();
        }
#endif

        public override string ToString() {
            return ConnectionString;
        }

        [ConfigurationProperty("providerName", DefaultValue = "System.Data.SqlClient")]
        public string ProviderName {
            get {
                return (string)base[_propProviderName];
            }
            set {
                base[_propProviderName] = value;
            }
        }

    } // class ConnectionStringSettings
}
