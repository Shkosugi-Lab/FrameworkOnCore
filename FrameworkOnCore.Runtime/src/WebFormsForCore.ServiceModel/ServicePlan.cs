// The services of a site, as IIS's WCF activation found them on .NET Framework: a .svc file (<%@ ServiceHost Service="..."
// %>, its address the file's) or an activation in web.config (serviceActivations), each service's endpoints from web.config
// (its service element's), or else WCF 4's default endpoints (one per contract the service implements, of the protocol
// mapping's binding, at the service's address). What CoreWCF cannot serve is said (Log) and left out.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Description;

namespace System.ServiceModel.Activation
{
    internal sealed class ServicePlan
    {
        internal sealed class Endpoint
        {
            public Type Contract;
            public Binding Binding;
            public string Address;
            /// <summary>webHttpBinding's: its webHttp behavior's settings (null: the default ones).</summary>
            public XElement WebHttp;
            public bool Web;
        }

        internal sealed class Service
        {
            public Type Type;
            public bool IncludeExceptionDetailInFaults;
            public bool HttpHelpPageEnabled = true;
            public readonly List<Endpoint> Endpoints = new List<Endpoint>();
            public readonly List<string> Addresses = new List<string>();
        }

        public readonly List<Service> Services = new List<Service>();
        public bool HttpGetEnabled;
        public bool HttpsGetEnabled;
        /// <summary>The services' addresses ("/Calculator.svc"): what CoreWCF serves; another .svc is not found (404).</summary>
        public readonly List<string> Addresses = new List<string>();

        private readonly ServiceModelSection _config;
        private readonly Action<string> _log;

        private ServicePlan(ServiceModelSection config, Action<string> log)
        {
            _config = config;
            _log = log;
        }

        private static readonly Regex s_directive = new Regex(@"<%@\s*ServiceHost\b(?<attributes>[^%]*)%>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex s_attribute = new Regex(@"(?<name>\w+)\s*=\s*(""(?<value>[^""]*)""|'(?<value>[^']*)')", RegexOptions.Compiled);

        public static ServicePlan Build(string siteRoot, Action<string> log)
        {
            var config = ServiceModelSection.Load(siteRoot);
            var plan = new ServicePlan(config, log);
            var activations = new List<(string Address, string Service, string Factory, string Source)>();
            if (Directory.Exists(siteRoot))
            {
                foreach (string file in Directory.EnumerateFiles(siteRoot, "*.svc", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive, RecurseSubdirectories = true }))
                {
                    string relative = Path.GetRelativePath(siteRoot, file).Replace('\\', '/');
                    if (Regex.IsMatch(relative, @"^(bin|obj|App_Data)/", RegexOptions.IgnoreCase))
                    {
                        continue;
                    }
                    Match directive = s_directive.Match(File.ReadAllText(file));
                    if (!directive.Success)
                    {
                        log($"{relative}: no ServiceHost directive, not served");
                        continue;
                    }
                    var attributes = s_attribute.Matches(directive.Groups["attributes"].Value)
                        .ToDictionary(m => m.Groups["name"].Value, m => m.Groups["value"].Value, StringComparer.OrdinalIgnoreCase);
                    attributes.TryGetValue("Service", out string service);
                    attributes.TryGetValue("Factory", out string factory);
                    activations.Add(("/" + relative, service, factory, relative));
                }
            }
            foreach (var (relativeAddress, service, factory) in config.Activations())
            {
                string address = "/" + (relativeAddress ?? "").TrimStart('~').TrimStart('/');
                activations.Add((address, service, factory, $"web.config serviceActivations {relativeAddress}"));
            }

            foreach (var activation in activations)
            {
                plan.Add(activation.Address, activation.Service, activation.Factory, activation.Source);
            }
            if (config.AspNetCompatibilityEnabled && plan.Services.Count > 0)
            {
                log("aspNetCompatibilityEnabled: the services' operations run without ASP.NET's context (HttpContext.Current, the session are not theirs)");
            }
            return plan;
        }

        private void Add(string address, string serviceName, string factory, string source)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                _log($"{source}: no Service, not served");
                return;
            }
            Type type = FindType(serviceName);
            if (type == null)
            {
                _log($"{source}: the service type {serviceName} is not found, not served");
                return;
            }
            bool webFactory = false;
            if (!string.IsNullOrEmpty(factory))
            {
                string factoryName = factory.Split(',')[0].Trim();
                if (factoryName == "System.ServiceModel.Activation.WebServiceHostFactory")
                {
                    webFactory = true;
                }
                else if (factoryName == "System.ServiceModel.Activation.WebScriptServiceHostFactory")
                {
                    _log($"{source}: WebScriptServiceHostFactory (ASP.NET AJAX's script services: enableWebScript) is not served by CoreWCF");
                    return;
                }
                else
                {
                    _log($"{source}: its factory {factoryName} is not run (CoreWCF has no ServiceHostFactory): the service is served as web.config configures it");
                }
            }

            Service service = Services.FirstOrDefault(s => s.Type == type);
            if (service == null)
            {
                service = new Service { Type = type };
                XElement element = _config.Service(serviceName) ?? _config.Service(type.FullName);
                ApplyServiceBehavior(service, _config.ServiceBehavior((string)element?.Attribute("behaviorConfiguration")), source);
                Services.Add(service);
            }
            service.Addresses.Add(address);
            Addresses.Add(address);

            XElement serviceElement = _config.Service(serviceName) ?? _config.Service(type.FullName);
            var endpoints = serviceElement?.Elements("endpoint").ToList() ?? new List<XElement>();
            if (endpoints.Count == 0)
            {
                // WCF 4's default endpoints: each contract the service implements, of the protocol mapping's binding (a
                // WebServiceHost's: webHttpBinding with webHttp).
                var (bindingKind, bindingConfiguration) = webFactory ? ("webHttpBinding", null) : _config.ProtocolMapping("http");
                foreach (Type contract in Contracts(type))
                {
                    AddEndpoint(service, contract, bindingKind, bindingConfiguration, address, null, source);
                }
                if (!Contracts(type).Any())
                {
                    _log($"{source}: {type.FullName} implements no service contract, not served");
                }
                return;
            }
            foreach (XElement endpoint in endpoints)
            {
                string contractName = (string)endpoint.Attribute("contract");
                string bindingKind = (string)endpoint.Attribute("binding");
                if (contractName == "IMetadataExchange" || bindingKind is "mexHttpBinding" or "mexHttpsBinding")
                {
                    _log($"{source}: its metadata exchange endpoint (mex) is not served; its WSDL is (?wsdl, when serviceMetadata allows it)");
                    continue;
                }
                Type contract = FindContract(type, contractName);
                if (contract == null)
                {
                    _log($"{source}: the contract {contractName} is not one {type.FullName} implements, its endpoint not served");
                    continue;
                }
                AddEndpoint(service, contract, bindingKind, (string)endpoint.Attribute("bindingConfiguration"),
                    EndpointAddress(address, (string)endpoint.Attribute("address")), _config.EndpointBehavior((string)endpoint.Attribute("behaviorConfiguration")), source);
            }
        }

        private void AddEndpoint(Service service, Type contract, string bindingKind, string bindingConfiguration, string address, XElement behavior, string source)
        {
            XElement configuration = _config.Binding(bindingKind, bindingConfiguration);
            if (!string.IsNullOrEmpty(bindingConfiguration) && configuration == null)
            {
                _log($"{source}: the binding configuration {bindingConfiguration} ({bindingKind}) is not in web.config, the binding's defaults used");
            }
            Binding binding = CreateBinding(bindingKind, configuration, source);
            if (binding == null)
            {
                return;
            }
            var endpoint = new Endpoint { Contract = contract, Binding = binding, Address = address, Web = binding is CoreWCF.WebHttpBinding };
            if (behavior != null)
            {
                foreach (XElement element in behavior.Elements())
                {
                    switch (element.Name.LocalName)
                    {
                        case "webHttp":
                            endpoint.WebHttp = element;
                            break;
                        case "enableWebScript":
                            _log($"{source}: enableWebScript (ASP.NET AJAX's script services) is not served by CoreWCF, its endpoint {address} left out");
                            return;
                        default:
                            _log($"{source}: the endpoint behavior {element.Name.LocalName} is not applied");
                            break;
                    }
                }
            }
            service.Endpoints.Add(endpoint);
        }

        private void ApplyServiceBehavior(Service service, XElement behavior, string source)
        {
            if (behavior == null)
            {
                return;
            }
            foreach (XElement element in behavior.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "serviceMetadata":
                        HttpGetEnabled |= True(element, "httpGetEnabled");
                        HttpsGetEnabled |= True(element, "httpsGetEnabled");
                        break;
                    case "serviceDebug":
                        service.IncludeExceptionDetailInFaults = True(element, "includeExceptionDetailInFaults");
                        if (element.Attribute("httpHelpPageEnabled") != null)
                        {
                            service.HttpHelpPageEnabled = True(element, "httpHelpPageEnabled");
                        }
                        break;
                    default:
                        _log($"{source}: the service behavior {element.Name.LocalName} is not applied");
                        break;
                }
            }
        }

        private static bool True(XElement element, string attribute) =>
            string.Equals((string)element.Attribute(attribute), "true", StringComparison.OrdinalIgnoreCase);

        // An endpoint's address, relative to its service's (the .svc file's): "" the service's, "rest" below it, an absolute
        // URI's path as it is.
        private static string EndpointAddress(string serviceAddress, string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return serviceAddress;
            }
            if (Uri.TryCreate(address, UriKind.Absolute, out Uri absolute) && (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps))
            {
                return absolute.AbsolutePath;
            }
            return serviceAddress.TrimEnd('/') + "/" + address.TrimStart('/');
        }

        private Binding CreateBinding(string kind, XElement configuration, string source)
        {
            string mode = (string)configuration?.Element("security")?.Attribute("mode");
            Binding binding;
            switch (kind)
            {
                case "basicHttpBinding":
                    binding = new CoreWCF.BasicHttpBinding(Parse(mode, CoreWCF.Channels.BasicHttpSecurityMode.None));
                    break;
                case "basicHttpsBinding":
                    binding = new CoreWCF.BasicHttpBinding(Parse(mode, CoreWCF.Channels.BasicHttpSecurityMode.Transport));
                    break;
                case "wsHttpBinding":
                    binding = new CoreWCF.WSHttpBinding(Parse(mode, CoreWCF.SecurityMode.Message));
                    break;
                case "webHttpBinding":
                    binding = new CoreWCF.WebHttpBinding(Parse(mode, CoreWCF.WebHttpSecurityMode.None));
                    break;
                default:
                    _log($"{source}: the binding {kind} is not served by CoreWCF in the site (basicHttpBinding, basicHttpsBinding, wsHttpBinding, webHttpBinding are)");
                    return null;
            }
            if (configuration != null)
            {
                Apply(binding, configuration, $"{source}: {kind}", "name");
                if (configuration.Element("readerQuotas") is { } quotas && binding.GetType().GetProperty("ReaderQuotas")?.GetValue(binding) is { } readerQuotas)
                {
                    Apply(readerQuotas, quotas, $"{source}: {kind} readerQuotas");
                }
            }
            return binding;
        }

        private static T Parse<T>(string value, T otherwise) where T : struct, Enum =>
            !string.IsNullOrEmpty(value) && Enum.TryParse(value, ignoreCase: true, out T parsed) ? parsed : otherwise;

        /// <summary>Configuration attributes onto the properties of the same names (binding, readerQuotas, webHttp behavior).</summary>
        internal void Apply(object target, XElement element, string what, params string[] skipped)
        {
            foreach (XAttribute attribute in element.Attributes())
            {
                string name = attribute.Name.LocalName;
                if (skipped.Contains(name) || attribute.IsNamespaceDeclaration)
                {
                    continue;
                }
                PropertyInfo property = target.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null || !property.CanWrite)
                {
                    _log($"{what}: {name} is not applied");
                    continue;
                }
                try
                {
                    property.SetValue(target, ConvertValue(attribute.Value, property.PropertyType));
                }
                catch (Exception e)
                {
                    _log($"{what}: {name}=\"{attribute.Value}\" is not applied ({e.GetBaseException().Message})");
                }
            }
        }

        private static object ConvertValue(string value, Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsEnum)
            {
                return Enum.Parse(type, value, ignoreCase: true);
            }
            if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(value, CultureInfo.InvariantCulture);
            }
            if (type == typeof(Encoding))
            {
                return Encoding.GetEncoding(value);
            }
            if (type == typeof(Uri))
            {
                return new Uri(value, UriKind.RelativeOrAbsolute);
            }
            return System.Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        // ------------------------------------------------------------------------------------------
        // Types

        private static bool IsContract(Type type) =>
            type.GetCustomAttributesData().Any(a => a.AttributeType.Name == "ServiceContractAttribute");

        private static string ConfigurationName(Type contract) =>
            contract.GetCustomAttributesData().Where(a => a.AttributeType.Name == "ServiceContractAttribute")
                .SelectMany(a => a.NamedArguments).Where(a => a.MemberName == "ConfigurationName").Select(a => a.TypedValue.Value as string)
                .FirstOrDefault(n => !string.IsNullOrEmpty(n)) ?? contract.FullName;

        /// <summary>The contracts a service implements (its interfaces with ServiceContract, and itself when it has it).</summary>
        internal static IEnumerable<Type> Contracts(Type service) =>
            service.GetInterfaces().Where(IsContract).Concat(IsContract(service) ? new[] { service } : Array.Empty<Type>());

        private static Type FindContract(Type service, string name) =>
            Contracts(service).FirstOrDefault(c => ConfigurationName(c) == name || c.FullName == name || c.Name == name);

        /// <summary>A type by its name ("Namespace.Type" or "Namespace.Type, Assembly"): in the loaded assemblies, else the application's (bin).</summary>
        internal static Type FindType(string name)
        {
            Type type = Type.GetType(name, throwOnError: false);
            if (type != null)
            {
                return type;
            }
            string typeName = name.Split(',')[0].Trim();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName, throwOnError: false);
                if (type != null)
                {
                    return type;
                }
            }
            foreach (string dll in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll"))
            {
                try
                {
                    type = Assembly.Load(AssemblyName.GetAssemblyName(dll)).GetType(typeName, throwOnError: false);
                    if (type != null)
                    {
                        return type;
                    }
                }
                catch (Exception e) when (e is BadImageFormatException or FileLoadException or FileNotFoundException)
                {
                }
            }
            return null;
        }
    }
}
