// web.config's system.serviceModel, as .NET Framework's WCF reads it for the services IIS activates: the services and their
// endpoints, the bindings' configurations (a binding without a name is its kind's default), the behaviors (one without a
// name is every service's or endpoint's that names none: WCF 4's default behaviors), the protocol mapping (the binding of
// a default endpoint) and serviceHostingEnvironment (the activations without a .svc file).

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace System.ServiceModel.Activation
{
    internal sealed class ServiceModelSection
    {
        private readonly XElement _section;

        private ServiceModelSection(XElement section)
        {
            _section = section;
        }

        /// <summary>The site's web.config's section; an empty one when there is none.</summary>
        public static ServiceModelSection Load(string siteRoot)
        {
            string config = Directory.Exists(siteRoot)
                ? Directory.EnumerateFiles(siteRoot, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault()
                : null;
            XElement section = null;
            if (config != null)
            {
                section = XDocument.Load(config).Root?.Element("system.serviceModel");
            }
            return new ServiceModelSection(section ?? new XElement("system.serviceModel"));
        }

        public bool AspNetCompatibilityEnabled =>
            string.Equals((string)_section.Element("serviceHostingEnvironment")?.Attribute("aspNetCompatibilityEnabled"), "true", StringComparison.OrdinalIgnoreCase);

        /// <summary>serviceHostingEnvironment's serviceActivations: a service at an address without a .svc file.</summary>
        public IEnumerable<(string RelativeAddress, string Service, string Factory)> Activations() =>
            _section.Element("serviceHostingEnvironment")?.Element("serviceActivations")?.Elements("add")
                .Select(a => ((string)a.Attribute("relativeAddress"), (string)a.Attribute("service"), (string)a.Attribute("factory")))
            ?? Enumerable.Empty<(string, string, string)>();

        public XElement Service(string name) =>
            _section.Element("services")?.Elements("service").FirstOrDefault(s => (string)s.Attribute("name") == name);

        /// <summary>A binding's configuration: the one named, or else (none named) its kind's default, the one without a name.</summary>
        public XElement Binding(string kind, string name) =>
            _section.Element("bindings")?.Element(kind)?.Elements("binding").FirstOrDefault(b => Named(b, name));

        public XElement ServiceBehavior(string name) =>
            _section.Element("behaviors")?.Element("serviceBehaviors")?.Elements("behavior").FirstOrDefault(b => Named(b, name));

        public XElement EndpointBehavior(string name) =>
            _section.Element("behaviors")?.Element("endpointBehaviors")?.Elements("behavior").FirstOrDefault(b => Named(b, name));

        /// <summary>The binding of a default endpoint for a scheme: protocolMapping's, else WCF's (basicHttpBinding for http).</summary>
        public (string Binding, string Configuration) ProtocolMapping(string scheme)
        {
            XElement mapping = _section.Element("protocolMapping")?.Elements("add").FirstOrDefault(a => string.Equals((string)a.Attribute("scheme"), scheme, StringComparison.OrdinalIgnoreCase));
            return mapping != null
                ? ((string)mapping.Attribute("binding"), (string)mapping.Attribute("bindingConfiguration"))
                : (scheme == "https" ? "basicHttpsBinding" : "basicHttpBinding", null);
        }

        // An element named so, or (no name asked) one without a name: the default.
        private static bool Named(XElement element, string name) =>
            string.IsNullOrEmpty(name) ? string.IsNullOrEmpty((string)element.Attribute("name")) : (string)element.Attribute("name") == name;
    }
}
