// .NET Framework's System.ServiceModel.Activation.AspNetCompatibilityRequirementsAttribute, which CoreWCF has not: a service
// that states it (the "AJAX-enabled WCF service" template does) compiles and runs. Its requirement is not enforced: an
// operation runs without ASP.NET's context (HttpContext.Current is not set in it), as with aspNetCompatibilityEnabled false.

using System.Collections.ObjectModel;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Description;

namespace System.ServiceModel.Activation
{
    public enum AspNetCompatibilityRequirementsMode
    {
        NotAllowed = 0,
        Allowed = 1,
        Required = 2,
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AspNetCompatibilityRequirementsAttribute : Attribute, IServiceBehavior
    {
        public AspNetCompatibilityRequirementsMode RequirementsMode { get; set; } = AspNetCompatibilityRequirementsMode.NotAllowed;

        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}
