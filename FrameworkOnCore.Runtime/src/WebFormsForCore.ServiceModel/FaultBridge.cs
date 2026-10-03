// The faults a converted service throws: compiled against .NET's WCF client (System.ServiceModel.FaultException and
// FaultException<TDetail>, the types its using System.ServiceModel finds), which CoreWCF does not know (it answers an
// internal error for them). Each is answered as the CoreWCF fault it is: its reason, code, action and detail, as .NET
// Framework's WCF answered it.

using System.Collections.ObjectModel;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;

namespace System.ServiceModel.Activation
{
    internal sealed class FaultBridge : IServiceBehavior, IErrorHandler
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase dispatcher in serviceHostBase.ChannelDispatchers)
            {
                if (dispatcher is ChannelDispatcher channelDispatcher)
                {
                    channelDispatcher.ErrorHandlers.Add(this);
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public bool HandleError(Exception error) => error is System.ServiceModel.FaultException;

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (Convert(error) is not { } converted)
            {
                return;
            }
            fault = Message.CreateMessage(version, converted.CreateMessageFault(), converted.Action ?? FaultAction(version));
        }

        /// <summary>The CoreWCF fault of a .NET WCF client fault; null for another exception.</summary>
        internal static CoreWCF.FaultException Convert(Exception error)
        {
            if (error is not System.ServiceModel.FaultException fault)
            {
                return null;
            }
            CoreWCF.FaultCode code = Convert(fault.Code);
            var reason = new CoreWCF.FaultReason(fault.Reason.ToString());
            Type type = fault.GetType();
            while (type != null && !(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.ServiceModel.FaultException<>)))
            {
                type = type.BaseType;
            }
            if (type == null)
            {
                return new CoreWCF.FaultException(reason, code, fault.Action);
            }
            Type detailType = type.GetGenericArguments()[0];
            object detail = type.GetProperty("Detail").GetValue(fault);
            return (CoreWCF.FaultException)Activator.CreateInstance(typeof(CoreWCF.FaultException<>).MakeGenericType(detailType), detail, reason, code, fault.Action);
        }

        private static CoreWCF.FaultCode Convert(System.ServiceModel.FaultCode code)
        {
            if (code == null)
            {
                return null;
            }
            CoreWCF.FaultCode sub = Convert(code.SubCode);
            return code.IsSenderFault && string.IsNullOrEmpty(code.Namespace) ? CoreWCF.FaultCode.CreateSenderFaultCode(sub)
                 : code.IsReceiverFault && string.IsNullOrEmpty(code.Namespace) ? CoreWCF.FaultCode.CreateReceiverFaultCode(sub)
                 : new CoreWCF.FaultCode(code.Name, code.Namespace, sub);
        }

        private static string FaultAction(MessageVersion version) =>
            version.Addressing == AddressingVersion.None ? null : "http://www.w3.org/2005/08/addressing/soap/fault";
    }
}
