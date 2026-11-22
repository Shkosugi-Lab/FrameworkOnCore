namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// What IIS's schema does not tell of a setting (SettingsSchema): the values of a text, a collection's entry (and the entries
/// IIS's own lists have, to remove or to add again), the context a setting needs to show in an answer, why one is not tried.
/// Keys: a section ("httpErrors"), an attribute ("httpErrors/[error]@path"), a collection's entry ("httpErrors/[error]").
/// "{F}" is the variant's folder ("/s/v0001"). The probes (SettingsProbes) are the same for every variant: a value here is
/// one some probe shows.
/// </summary>
static class SettingsSamples
{
    public sealed class Sample
    {
        /// <summary>The values tried (instead of the type's).</summary>
        public string[] Values;
        /// <summary>Attributes put on the section's element first ("errorMode=\"Custom\""): the setting's context.</summary>
        public string WithAttributes;
        /// <summary>Elements put in the section's element first.</summary>
        public string With;
        /// <summary>Elements put in system.webServer beside the section (another section the setting needs).</summary>
        public string Beside;
        /// <summary>Elements put in system.web.</summary>
        public string SystemWeb;
        /// <summary>A collection's entry (the attribute's entry, for an attribute's key: its context).</summary>
        public string Item;
        /// <summary>Put in the collection before the entry (an inherited entry of the same key removed).</summary>
        public string Prefix;
        /// <summary>The removes tried (the server's own entries).</summary>
        public string[] Remove;
        /// <summary>An entry whose key the server's list has.</summary>
        public string Duplicate;
        /// <summary>Not tried: why.</summary>
        public string NoEffect;
    }

    public static Sample Of(string key) => Samples.TryGetValue(key, out var sample) ? sample : null;

    const string Installed = " is not installed on this machine (its Windows feature): recorded where it is";

    /// <summary>The sections of system.webServer not tried, and why.</summary>
    public static readonly Dictionary<string, string> SectionsNotTried = new()
    {
        ["asp"] = "classic ASP's settings: classic ASP is not run by the port (the conversion report tells it)",
        ["cgi"] = "CGI programs: not run by the port (reported)",
        ["fastCgi"] = "FastCGI programs: not run by the port (reported); applicationHost.config only",
        ["globalModules"] = "the server's native modules: applicationHost.config only",
        ["isapiFilters"] = "ISAPI filters: not run by the port (reported); set at the application's root only",
        ["httpTracing"] = "IIS's tracing: no answer of its",
        ["odbcLogging"] = "ODBC logging: no answer of its",
        ["httpLogging"] = "the log only (IisLog's tests): no answer of its",
        ["tracing/traceFailedRequests"] = "failed request tracing: no answer of its",
        ["tracing/traceProviderDefinitions"] = "failed request tracing: no answer of its",
        ["security/applicationDependencies"] = "applicationHost.config only",
        ["security/isapiCgiRestriction"] = "applicationHost.config only",
        ["serverSideInclude"] = "server side includes: IIS's module" + Installed,
        ["webdav/globalSettings"] = "WebDAV: not done by the port (reported)",
        ["webdav/authoring"] = "WebDAV: not done by the port (reported)",
        ["webdav/authoringRules"] = "WebDAV: not done by the port (reported)",
        ["rewrite/globalRules"] = "URL Rewrite's global rules: applicationHost.config only (the port reads them from the server's settings)",
        ["rewrite/allowedServerVariables"] = "applicationHost.config only",
        ["rewrite/providers"] = "URL Rewrite's providers: not run by the port (reported)",
        ["applicationInitialization"] = "set at the application's root only; its warm-up requests are not answers",
        ["security/authentication/digestAuthentication"] = "digest authentication" + Installed,
        ["security/authentication/clientCertificateMappingAuthentication"] = "client certificates mapped to Active Directory's accounts: the port has no Windows accounts (reported)",
        ["security/authentication/iisClientCertificateMappingAuthentication"] = "client certificates mapped to Windows accounts: the port has none to log on (reported)",
        ["management/authentication"] = "IIS Manager's delegation (administration.config): no setting of a site",
        ["management/authorization"] = "IIS Manager's delegation (administration.config): no setting of a site",
        ["management/trustedProviders"] = "IIS Manager's delegation (administration.config): no setting of a site",
    };

    /// <summary>The sections set at the application's root only (a folder's is IIS's 500.19): tried as an application's (SettingsSite.Application).</summary>
    public static readonly HashSet<string> ApplicationSections = new() { "modules" };

    const string Rule = """<rule name="foc" stopProcessing="true"><match url="^rw/(.*)$" /><action type="Rewrite" url="{R:1}" /></rule>""";
    const string RedirectRule = """<rule name="foc" stopProcessing="true"><match url="^rw/(.*)$" /><action type="Redirect" url="{R:1}" /></rule>""";
    const string CustomRule = """<rule name="foc" stopProcessing="true"><match url="^rw/(.*)$" /><action type="CustomResponse" statusCode="403" /></rule>""";
    const string OutboundRule = """<rule name="foc"><match serverVariable="RESPONSE_X_Powered_By" pattern=".*" /><action type="Rewrite" value="foc" /></rule>""";
    const string AnonymousOff = """<security><authentication><anonymousAuthentication enabled="false" /></authentication></security>""";

    static readonly Dictionary<string, Sample> Samples = new()
    {
        // ---- caching (output caching of the probe handler's answers)
        ["caching/profiles/[add]"] = new() { Item = """<add extension=".probe" policy="CacheForTimePeriod" kernelCachePolicy="DontCache" duration="00:00:30" location="Any" />""", Remove = new[] { """<remove extension=".probe" />""" } },
        ["caching/profiles/[add]@extension"] = new() { Values = new[] { ".probe", ".txt" } },
        ["caching/profiles/[add]@duration"] = new() { Values = new[] { "00:00:01", "00:00:30", "00:00:31", "00:00:59", "00:01:00", "00:01:40", "00:10:00" } },
        ["caching/profiles/[add]@varyByHeaders"] = new() { Values = new[] { "Accept-Encoding" } },
        ["caching/profiles/[add]@varyByQueryString"] = new() { Values = new[] { "*", "do" } },

        // ---- default documents
        ["defaultDocument/files/[add]"] = new() { Item = """<add value="a.htm" />""", Remove = new[] { """<remove value="Default.htm" />""" }, Duplicate = """<add value="Default.htm" />""" },
        ["defaultDocument/files/[add]@value"] = new() { Values = new[] { "a.htm", "page.probe", "nothing.htm", "list/a.txt" } },
        ["directoryBrowse@showFlags"] = new() { WithAttributes = "enabled=\"true\"" },
        ["defaultDocument@enabled"] = new() { Values = new[] { "false" } },

        // ---- modules, at the application's root (SettingsModule: App_Code's module, marking the answers it runs for)
        ["modules/[add]"] = new()
        {
            Item = """<add name="foc" type="SettingsModule" />""",
            Remove = new[] { """<remove name="OutputCache" />""", """<remove name="RewriteModule" />""", """<remove name="StaticFileModule" />""" },
            Duplicate = """<add name="OutputCache" type="System.Web.Caching.OutputCacheModule" preCondition="managedHandler" />""",
        },
        ["modules/[add]@name"] = new() { Values = new[] { "foc2" } },
        ["modules/[add]@type"] = new() { Values = new[] { "SettingsModule", "No.Such.Type" } },
        ["modules/[add]@preCondition"] = new() { Values = new[] { "managedHandler", "integratedMode", "classicMode", "runtimeVersionv2.0", "managedHandler,runtimeVersionv4.0", "bitness32" } },
        ["modules@runAllManagedModulesForAllRequests"] = new() { With = """<add name="foc" type="SettingsModule" preCondition="managedHandler" />""" },
        ["modules@runManagedModulesForWebDavRequests"] = new() { With = """<add name="foc" type="SettingsModule" preCondition="managedHandler" />""" },

        // ---- access: TLS and the client's certificate (the HTTPS probes)
        ["security/access@sslFlags"] = new() { Values = new[] { "Ssl", "SslNegotiateCert", "SslRequireCert", "Ssl128", "Ssl, SslNegotiateCert", "Ssl, SslNegotiateCert, SslRequireCert", "SslNegotiateCert, SslRequireCert", "Ssl, Ssl128" } },

        // ---- handlers
        ["handlers/[add]"] = new()
        {
            Item = """<add name="foc" path="*.txt" verb="*" type="System.Web.HttpForbiddenHandler" resourceType="Unspecified" />""",
            Remove = new[] { """<remove name="StaticFile" />""", """<remove name="ExtensionlessUrlHandler-Integrated-4.0" />""", """<remove name="OPTIONSVerbHandler" />""", """<remove name="TRACEVerbHandler" />""", """<remove name="SettingsProbe" />""" },
            Duplicate = """<add name="StaticFile" path="*" verb="*" modules="StaticFileModule,DefaultDocumentModule,DirectoryListingModule" resourceType="Either" requireAccess="Read" />""",
        },
        ["handlers/[add]@name"] = new() { Values = new[] { "foc2" } },
        ["handlers/[add]@path"] = new() { Values = new[] { "a.txt", "*.htm", "*" } },
        ["handlers/[add]@verb"] = new() { Values = new[] { "GET", "POST", "GET,HEAD" } },
        ["handlers/[add]@type"] = new() { Values = new[] { "System.Web.StaticFileHandler", "System.Web.DefaultHttpHandler", "No.Such.Type" } },
        ["handlers/[add]@modules"] = new() { Values = new[] { "StaticFileModule", "ProtocolSupportModule" } },
        ["handlers/[add]@scriptProcessor"] = new() { NoEffect = "a FastCGI, CGI or ISAPI program: not run by the port (reported)" },
        ["handlers/[add]@preCondition"] = new() { Values = new[] { "integratedMode", "classicMode", "bitness32", "runtimeVersionv2.0", "managedHandler" } },

        // ---- compression
        ["httpCompression@directory"] = new() { NoEffect = "the compressed files' folder: no answer of its" },
        ["httpCompression@doDiskSpaceLimiting"] = new() { NoEffect = "the compressed files' folder's size: no answer of its" },
        ["httpCompression@maxDiskSpaceUsage"] = new() { NoEffect = "the compressed files' folder's size: no answer of its" },
        ["httpCompression@expiresHeader"] = new() { WithAttributes = "sendCacheHeaders=\"true\"", Values = new[] { "Thu, 01 Jan 2004 00:00:00 GMT" } },
        ["httpCompression@cacheControlHeader"] = new() { WithAttributes = "sendCacheHeaders=\"true\"", Values = new[] { "max-age=60" } },
        ["httpCompression/[scheme]"] = new()
        {
            Prefix = """<remove name="gzip" />""", Item = """<scheme name="gzip" dll="%Windir%\system32\inetsrv\gzip.dll" />""",
            Remove = new[] { """<remove name="gzip" />""" }, Duplicate = """<scheme name="gzip" dll="%Windir%\system32\inetsrv\gzip.dll" />""",
        },
        ["httpCompression/[scheme]@name"] = new() { Values = new[] { "deflate" } },
        ["httpCompression/[scheme]@dll"] = new() { Values = new[] { """%Windir%\system32\inetsrv\nothing.dll""" } },
        ["httpCompression/staticTypes/[add]"] = new() { Prefix = """<remove mimeType="text/*" />""", Item = """<add mimeType="text/*" enabled="false" />""", Remove = new[] { """<remove mimeType="text/*" />""" }, Duplicate = """<add mimeType="text/*" enabled="true" />""" },
        ["httpCompression/staticTypes/[add]@mimeType"] = new() { Values = new[] { "text/plain", "*/*" } },
        ["httpCompression/dynamicTypes/[add]"] = new() { Prefix = """<remove mimeType="text/*" />""", Item = """<add mimeType="text/*" enabled="false" />""", Remove = new[] { """<remove mimeType="text/*" />""" }, Duplicate = """<add mimeType="text/*" enabled="true" />""" },
        ["httpCompression/dynamicTypes/[add]@mimeType"] = new() { Values = new[] { "text/html", "*/*" } },

        // ---- error pages (the probes are local: the entries show with errorMode Custom)
        ["httpErrors@defaultPath"] = new() { WithAttributes = "errorMode=\"Custom\"", With = "<clear />", Values = new[] { "a.htm", "{F}/a.htm" } },
        ["httpErrors@defaultResponseMode"] = new() { WithAttributes = "errorMode=\"Custom\" defaultPath=\"a.htm\"", With = "<clear />" },
        ["httpErrors@detailedMoreInformationLink"] = new() { Values = new[] { "http://example.invalid/more" } },
        ["httpErrors/[error]"] = new()
        {
            WithAttributes = "errorMode=\"Custom\"", Prefix = "<clear />",
            Item = """<error statusCode="404" path="a.htm" responseMode="File" prefixLanguageFilePath="" />""",
            Remove = new[] { """<remove statusCode="404" subStatusCode="-1" />""" },
            Duplicate = """<error statusCode="404" prefixLanguageFilePath="%SystemDrive%\inetpub\custerr" path="404.htm" />""",
        },
        ["httpErrors/[error]@statusCode"] = new() { Values = new[] { "403", "405", "500" } },
        ["httpErrors/[error]@subStatusCode"] = new() { Values = new[] { "0", "3", "7" } },
        ["httpErrors/[error]@prefixLanguageFilePath"] = new() { Values = new[] { """%SystemDrive%\inetpub\custerr""" } },
        ["httpErrors/[error]@path"] = new() { Values = new[] { "{F}/a.htm", "/s/base/a.htm", "nothing.htm", "http://example.invalid/error" } },

        // ---- headers
        ["httpProtocol/customHeaders/[add]"] = new() { Item = """<add name="X-Sample" value="1" />""", Remove = new[] { """<remove name="X-Powered-By" />""" }, Duplicate = """<add name="X-Powered-By" value="ASP.NET" />""" },
        ["httpProtocol/customHeaders/[add]@name"] = new() { Values = new[] { "X-Sample", "Cache-Control", "Server" } },
        ["httpProtocol/customHeaders/[add]@value"] = new() { Values = new[] { "", "a, b" } },
        ["httpProtocol/redirectHeaders/[add]"] = new() { Item = """<add name="X-Redirect" value="1" />""", Remove = new[] { """<remove name="X-Redirect" />""" } },
        ["httpProtocol/redirectHeaders/[add]@name"] = new() { Values = new[] { "X-Redirect" } },
        ["httpProtocol/redirectHeaders/[add]@value"] = new() { Values = new[] { "", "2" } },

        // ---- httpRedirect (on, to another host, but for its own attributes)
        ["httpRedirect"] = new() { WithAttributes = "enabled=\"true\"" },
        ["httpRedirect@enabled"] = new() { WithAttributes = "destination=\"http://example.invalid/target\"" },
        ["httpRedirect@exactDestination"] = new() { WithAttributes = "destination=\"http://example.invalid/target\"" },
        ["httpRedirect@childOnly"] = new() { WithAttributes = "destination=\"http://example.invalid/target\"" },
        ["httpRedirect@httpResponseStatus"] = new() { WithAttributes = "destination=\"http://example.invalid/target\"" },
        ["httpRedirect@destination"] = new() { Values = new[] { "http://example.invalid/target", "/s/base/a.txt", "a.txt", "http://example.invalid/$S$Q" } },
        ["httpRedirect/[add]"] = new() { Item = """<add wildcard="*.htm" destination="http://example.invalid/htm" />""", Remove = new[] { """<remove wildcard="*.htm" />""" } },
        ["httpRedirect/[add]@wildcard"] = new() { Values = new[] { "*.htm", "{F}/a.txt", "*/list/*" } },
        ["httpRedirect/[add]@destination"] = new() { WithAttributes = "exactDestination=\"true\"", Values = new[] { "/s/base/a.txt$Q", "http://example.invalid/$V" } },

        // ---- authentication
        ["security/authentication/anonymousAuthentication@userName"] = new() { Values = new[] { "", "foc-nobody" } },
        ["security/authentication/anonymousAuthentication@password"] = new() { NoEffect = "the anonymous user's password: with its userName only" },
        ["security/authentication/windowsAuthentication"] = new() { WithAttributes = "enabled=\"true\"", Beside = AnonymousOff },
        ["security/authentication/windowsAuthentication/providers/[add]"] = new() { Prefix = "<clear />", Item = """<add value="NTLM" />""", Remove = new[] { """<remove value="Negotiate" />""" }, Duplicate = """<add value="Negotiate" />""" },
        ["security/authentication/windowsAuthentication/providers/[add]@value"] = new() { Values = new[] { "Negotiate", "Negotiate:Kerberos", "NTLM" } },
        ["security/authentication/windowsAuthentication/extendedProtection/[spn]"] = new() { Item = """<spn name="HTTP/foc" />""" },
        ["security/authentication/windowsAuthentication/extendedProtection/[spn]@name"] = new() { Values = new[] { "HTTP/foc" } },

        // ---- URL authorization (IIS's own, security/authorization: the probes are anonymous)
        ["security/authorization/[add]"] = new() { Item = """<add accessType="Deny" users="?" />""", Remove = new[] { """<remove users="*" roles="" verbs="" />""" }, Duplicate = """<add accessType="Allow" users="*" />""" },
        ["security/authorization/[add]@users"] = new() { Values = new[] { "?", "*", "foc" } },
        ["security/authorization/[add]@roles"] = new() { Values = new[] { "foc" } },
        ["security/authorization/[add]@verbs"] = new() { Values = new[] { "POST", "GET", "GET,HEAD" } },
        ["security/authorization@bypassLoginPages"] = new() { With = """<add accessType="Deny" users="?" />""" },

        // ---- IP restrictions (the probes come from 127.0.0.1)
        ["security/ipSecurity/[add]"] = new() { Item = """<add ipAddress="127.0.0.1" allowed="false" />""" },
        ["security/ipSecurity/[add]@ipAddress"] = new() { Values = new[] { "127.0.0.1", "10.0.0.1", "127.0.0.0", "127.0.0.2", "127.0.1.0", "127.1.0.0", "128.0.0.0", "0.0.0.0" } },
        ["security/ipSecurity/[add]@subnetMask"] = new() { Values = new[] { "255.0.0.0", "255.255.255.0" } },
        ["security/ipSecurity/[add]@domainName"] = new() { Item = """<add domainName="localhost" allowed="false" />""", Values = new[] { "localhost", "foc.example" } },
        ["security/ipSecurity@denyAction"] = new() { With = """<add ipAddress="127.0.0.1" allowed="false" />""" },
        ["security/ipSecurity@enableReverseDns"] = new() { With = """<add domainName="localhost" allowed="false" />""" },
        ["security/ipSecurity@enableProxyMode"] = new() { With = """<add ipAddress="127.0.0.1" allowed="false" />""" },

        // ---- dynamic IP restrictions (the probes are sent one after another, as fast as answered)
        ["security/dynamicIpSecurity/denyByRequestRate@maxRequests"] = new() { With = """<denyByRequestRate enabled="true" />""", Values = new[] { "1", "5", "100000" } },
        ["security/dynamicIpSecurity/denyByRequestRate@requestIntervalInMilliseconds"] = new() { With = """<denyByRequestRate enabled="true" maxRequests="5" />""", Values = new[] { "10", "1000", "100000" } },
        ["security/dynamicIpSecurity/denyByConcurrentRequests@maxConcurrentRequests"] = new() { With = """<denyByConcurrentRequests enabled="true" />""", Values = new[] { "0", "1" } },
        ["security/dynamicIpSecurity@denyAction"] = new() { With = """<denyByRequestRate enabled="true" maxRequests="5" requestIntervalInMilliseconds="100000" />""" },
        ["security/dynamicIpSecurity@enableLoggingOnlyMode"] = new() { With = """<denyByRequestRate enabled="true" maxRequests="5" requestIntervalInMilliseconds="100000" />""" },

        // ---- basic authentication (anonymous off: the probes have no credentials)
        ["security/authentication/basicAuthentication"] = new() { WithAttributes = "enabled=\"true\"", Beside = AnonymousOff },
        ["security/authentication/basicAuthentication@realm"] = new() { Values = new[] { "foc" } },
        ["security/authentication/basicAuthentication@defaultLogonDomain"] = new() { Values = new[] { "foc" } },

        // ---- request filtering
        ["security/requestFiltering/fileExtensions/[add]"] = new() { Item = """<add fileExtension=".foo" allowed="false" />""", Remove = new[] { """<remove fileExtension=".config" />""" }, Duplicate = """<add fileExtension=".config" allowed="false" />""" },
        ["security/requestFiltering/fileExtensions/[add]@fileExtension"] = new() { Values = new[] { ".txt", ".", ".htm", ".probe" } },
        ["security/requestFiltering/requestLimits/headerLimits/[add]"] = new() { Item = """<add header="X-Long" sizeLimit="10" />""" },
        ["security/requestFiltering/requestLimits/headerLimits/[add]@header"] = new() { Values = new[] { "X-Long", "Host", "User-Agent" } },
        ["security/requestFiltering/requestLimits/headerLimits/[add]@sizeLimit"] = new() { Values = new[] { "0", "10", "1000" } },
        ["security/requestFiltering/verbs/[add]"] = new() { Item = """<add verb="POST" allowed="false" />""" },
        ["security/requestFiltering/verbs/[add]@verb"] = new() { Values = new[] { "GET", "OPTIONS", "PUT" } },
        ["security/requestFiltering/hiddenSegments/[add]"] = new() { Item = """<add segment="list" />""", Remove = new[] { """<remove segment="App_Data" />""", """<remove segment="bin" />""" }, Duplicate = """<add segment="bin" />""" },
        ["security/requestFiltering/hiddenSegments/[add]@segment"] = new() { Values = new[] { "list", "a.txt", "LIST" } },
        ["security/requestFiltering/alwaysAllowedUrls/[add]"] = new() { Item = """<add url="{F}/x.config" />""" },
        ["security/requestFiltering/alwaysAllowedUrls/[add]@url"] = new() { Values = new[] { "{F}/x.config", "{F}/bin/a.txt", "{F}/bad.txt" }, With = """<denyUrlSequences><add sequence="bad" /></denyUrlSequences>""" },
        ["security/requestFiltering/alwaysAllowedQueryStrings/[add]"] = new() { Item = """<add queryString="q=bad" />""", With = """<denyQueryStringSequences><add sequence="bad" /></denyQueryStringSequences>""" },
        ["security/requestFiltering/alwaysAllowedQueryStrings/[add]@queryString"] = new() { Values = new[] { "q=bad", "bad" } },
        ["security/requestFiltering/denyUrlSequences/[add]"] = new() { Item = """<add sequence="bad" />""" },
        ["security/requestFiltering/denyUrlSequences/[add]@sequence"] = new() { Values = new[] { "bad", "..", "a.t" } },
        ["security/requestFiltering/denyQueryStringSequences/[add]"] = new() { Item = """<add sequence="bad" />""" },
        ["security/requestFiltering/denyQueryStringSequences/[add]@sequence"] = new() { Values = new[] { "bad", "q=" } },
        ["security/requestFiltering@unescapeQueryString"] = new() { With = """<denyQueryStringSequences><add sequence="bad" /></denyQueryStringSequences>""" },
        ["security/requestFiltering/filteringRules/[filteringRule]"] = new() { Item = """<filteringRule name="foc" scanUrl="true" scanQueryString="true"><denyStrings><add string="bad" /></denyStrings></filteringRule>""" },
        ["security/requestFiltering/filteringRules/[filteringRule]@name"] = new() { Values = new[] { "foc2" } },
        ["security/requestFiltering/filteringRules/[filteringRule]/scanHeaders/[add]"] = new() { Item = """<add requestHeader="X-Long" />""" },
        ["security/requestFiltering/filteringRules/[filteringRule]/scanHeaders/[add]@requestHeader"] = new() { Values = new[] { "X-Long", "Host" } },
        ["security/requestFiltering/filteringRules/[filteringRule]/appliesTo/[add]"] = new() { Item = """<add fileExtension=".txt" />""" },
        ["security/requestFiltering/filteringRules/[filteringRule]/appliesTo/[add]@fileExtension"] = new() { Values = new[] { ".txt", ".htm" } },
        ["security/requestFiltering/filteringRules/[filteringRule]/denyStrings/[add]"] = new() { Item = """<add string="bad" />""" },
        ["security/requestFiltering/filteringRules/[filteringRule]/denyStrings/[add]@string"] = new() { Values = new[] { "bad", "xxxx", "a.t" } },

        // ---- the server's runtime
        ["serverRuntime@alternateHostName"] = new() { Values = new[] { "foc.example" } },

        // ---- static content
        ["staticContent/clientCache@cacheControlMaxAge"] = new() { With = """<clientCache cacheControlMode="UseMaxAge" />""" },
        ["staticContent/clientCache@httpExpires"] = new() { With = """<clientCache cacheControlMode="UseExpires" />""", Values = new[] { "Fri, 01 Jan 2100 00:00:00 GMT" } },
        ["staticContent/clientCache@cacheControlCustom"] = new() { Values = new[] { "public", "no-transform" } },
        ["staticContent/clientCache@cacheControlMode"] = new() { With = """<clientCache httpExpires="Fri, 01 Jan 2100 00:00:00 GMT" />""" },
        ["staticContent@defaultDocFooter"] = new() { WithAttributes = "enableDocFooter=\"true\"", Values = new[] { "PUBLIC-footer" } },
        ["staticContent@isDocFooterFileName"] = new() { WithAttributes = "enableDocFooter=\"true\" defaultDocFooter=\"PUBLIC-footer\"" },
        ["staticContent@enableDocFooter"] = new() { WithAttributes = "defaultDocFooter=\"PUBLIC-footer\"" },
        ["staticContent/[mimeMap]"] = new() { Item = """<mimeMap fileExtension=".foo" mimeType="text/x-foo" />""", Remove = new[] { """<remove fileExtension=".txt" />""" }, Duplicate = """<mimeMap fileExtension=".txt" mimeType="text/plain" />""" },
        ["staticContent/[mimeMap]@fileExtension"] = new() { Values = new[] { ".foo", ".*", "." } },
        ["staticContent/[mimeMap]@mimeType"] = new() { Values = new[] { "text/x-foo", "application/octet-stream", "text/html" } },

        // ---- URL Rewrite: the folder's rules ("rw/x" rewritten to "x")
        ["rewrite/rules@useOriginalURLEncoding"] = new() { With = Rule },
        ["rewrite/rules/[rule]"] = new() { Item = Rule, Remove = new[] { """<remove name="foc" />""" } },
        ["rewrite/rules/[rule]@name"] = new() { Values = new[] { "foc2" } },
        ["rewrite/rules/[rule]/match@url"] = new() { Values = new[] { "^rw/(.*)$", "rw/*", "^RW/(.*)$", "(.*)" } },
        ["rewrite/rules/[rule]/conditions@logicalGrouping"] = new() { Item = """<rule name="foc" stopProcessing="true"><match url="^rw/(.*)$" /><conditions><add input="{QUERY_STRING}" pattern="^$" /><add input="{QUERY_STRING}" pattern="q=" /></conditions><action type="Rewrite" url="{R:1}" /></rule>""" },
        ["rewrite/rules/[rule]/conditions@trackAllCaptures"] = new() { Item = """<rule name="foc" stopProcessing="true"><match url="^rw/(.*)$" /><conditions><add input="{QUERY_STRING}" pattern="(q)=(.*)" /><add input="{HTTP_HOST}" pattern="(.*)" /></conditions><action type="Rewrite" url="{R:1}?c={C:1}" /></rule>""" },
        ["rewrite/rules/[rule]/conditions/[add]"] = new() { Item = """<add input="{QUERY_STRING}" pattern="^$" />""" },
        ["rewrite/rules/[rule]/conditions/[add]@input"] = new() { Values = new[] { "{QUERY_STRING}", "{HTTP_X_LONG}", "{REQUEST_FILENAME}" } },
        ["rewrite/rules/[rule]/conditions/[add]@pattern"] = new() { Values = new[] { "^$", "q=", "x" } },
        ["rewrite/rules/[rule]/conditions/[add]@matchType"] = new() { Item = """<add input="{REQUEST_FILENAME}" matchType="Pattern" pattern=".*" />""" },
        ["rewrite/rules/[rule]/serverVariables/[set]"] = new() { Item = """<set name="HTTP_X_FOC" value="1" />""" },
        ["rewrite/rules/[rule]/serverVariables/[set]@name"] = new() { Values = new[] { "HTTP_X_FOC", "QUERY_STRING" } },
        ["rewrite/rules/[rule]/serverVariables/[set]@value"] = new() { Values = new[] { "1", "" } },
        ["rewrite/rules/[rule]/action@url"] = new() { Values = new[] { "{R:1}", "/s/base/{R:1}", "http://example.invalid/{R:1}", "{F}/list/{R:1}" } },
        ["rewrite/rules/[rule]/action@appendQueryString"] = new() { Item = RedirectRule },
        ["rewrite/rules/[rule]/action@redirectType"] = new() { Item = RedirectRule },
        ["rewrite/rules/[rule]/action@statusCode"] = new() { Item = CustomRule, Values = new[] { "200", "403", "503", "301" } },
        ["rewrite/rules/[rule]/action@subStatusCode"] = new() { Item = CustomRule, Values = new[] { "5", "999" } },
        ["rewrite/rules/[rule]/action@statusReason"] = new() { Item = CustomRule, Values = new[] { "Nope" } },
        ["rewrite/rules/[rule]/action@statusDescription"] = new() { Item = CustomRule, Values = new[] { "Nope description" } },

        // ---- URL Rewrite's maps (a rule of the folder's that uses one: "map/x")
        ["rewrite/rewriteMaps"] = new() { Beside = """<rewrite><rules><rule name="foc-map" stopProcessing="true"><match url="^map/(.*)$" /><action type="Rewrite" url="{foc:{R:1}}" /></rule></rules></rewrite>""" },
        ["rewrite/rewriteMaps/[rewriteMap]"] = new() { Item = """<rewriteMap name="foc" defaultValue="a.htm"><add key="x" value="a.txt" /></rewriteMap>""", Remove = new[] { """<remove name="foc" />""" } },
        ["rewrite/rewriteMaps/[rewriteMap]@name"] = new() { Values = new[] { "foc", "FOC" } },
        ["rewrite/rewriteMaps/[rewriteMap]@defaultValue"] = new() { Values = new[] { "a.htm", "" } },
        ["rewrite/rewriteMaps/[rewriteMap]/[add]"] = new() { Item = """<add key="x" value="a.txt" />""" },
        ["rewrite/rewriteMaps/[rewriteMap]/[add]@key"] = new() { Values = new[] { "x", "X" } },
        ["rewrite/rewriteMaps/[rewriteMap]/[add]@value"] = new() { Values = new[] { "a.txt", "nothing.txt" } },

        // ---- URL Rewrite's outbound rules (the port does not run them: reported)
        ["rewrite/outboundRules@rewriteBeforeCache"] = new() { With = OutboundRule },
        ["rewrite/outboundRules/[rule]"] = new() { Item = OutboundRule, Remove = new[] { """<remove name="foc" />""" } },
        ["rewrite/outboundRules/[rule]@name"] = new() { Values = new[] { "foc2" } },
        ["rewrite/outboundRules/[rule]@preCondition"] = new() { Values = new[] { "foc-html" }, With = """<preConditions><preCondition name="foc-html"><add input="{RESPONSE_CONTENT_TYPE}" pattern="^text/html" /></preCondition></preConditions>""" },
        ["rewrite/outboundRules/[rule]/match@filterByTags"] = new() { Item = """<rule name="foc"><match filterByTags="A" pattern="PUBLIC" /><action type="Rewrite" value="FOC" /></rule>""" },
        ["rewrite/outboundRules/[rule]/match@customTags"] = new() { Item = """<rule name="foc"><match filterByTags="CustomTags" pattern="PUBLIC" /><action type="Rewrite" value="FOC" /></rule>""", Values = new[] { "foc-tags" }, With = """<customTags><tags name="foc-tags"><tag name="a" attribute="href" /></tags></customTags>""" },
        ["rewrite/outboundRules/[rule]/match@serverVariable"] = new() { Values = new[] { "RESPONSE_X_Powered_By", "RESPONSE_Cache_Control" } },
        ["rewrite/outboundRules/[rule]/match@pattern"] = new() { Values = new[] { ".*", "ASP" } },
        ["rewrite/outboundRules/[rule]/conditions/[add]"] = new() { Item = """<add input="{REQUEST_URI}" pattern="a" />""" },
        ["rewrite/outboundRules/[rule]/conditions/[add]@input"] = new() { Values = new[] { "{REQUEST_URI}", "{RESPONSE_STATUS}" } },
        ["rewrite/outboundRules/[rule]/conditions/[add]@pattern"] = new() { Values = new[] { "a", "^200" } },
        ["rewrite/outboundRules/[rule]/action@value"] = new() { Values = new[] { "foc", "" } },
        ["rewrite/outboundRules/customTags/[tags]"] = new() { Item = """<tags name="foc-tags"><tag name="a" attribute="href" /></tags>""", With = """<rule name="foc"><match filterByTags="CustomTags" customTags="foc-tags" pattern="PUBLIC" /><action type="Rewrite" value="FOC" /></rule>""" },
        ["rewrite/outboundRules/customTags/[tags]@name"] = new() { Values = new[] { "foc-tags" } },
        ["rewrite/outboundRules/customTags/[tags]/[tag]"] = new() { Item = """<tag name="a" attribute="href" />""" },
        ["rewrite/outboundRules/customTags/[tags]/[tag]@name"] = new() { Values = new[] { "a", "img" } },
        ["rewrite/outboundRules/customTags/[tags]/[tag]@attribute"] = new() { Values = new[] { "href", "src" } },
        ["rewrite/outboundRules/preConditions/[preCondition]"] = new() { Item = """<preCondition name="foc-html"><add input="{RESPONSE_CONTENT_TYPE}" pattern="^text/html" /></preCondition>""", With = """<rule name="foc" preCondition="foc-html"><match serverVariable="RESPONSE_X_Powered_By" pattern=".*" /><action type="Rewrite" value="foc" /></rule>""" },
        ["rewrite/outboundRules/preConditions/[preCondition]@name"] = new() { Values = new[] { "foc-html" } },
        ["rewrite/outboundRules/preConditions/[preCondition]/[add]"] = new() { Item = """<add input="{RESPONSE_CONTENT_TYPE}" pattern="^text/html" />""" },
        ["rewrite/outboundRules/preConditions/[preCondition]/[add]@input"] = new() { Values = new[] { "{RESPONSE_CONTENT_TYPE}", "{RESPONSE_STATUS}" } },
        ["rewrite/outboundRules/preConditions/[preCondition]/[add]@pattern"] = new() { Values = new[] { "^text/html", "^200" } },
    };
}
