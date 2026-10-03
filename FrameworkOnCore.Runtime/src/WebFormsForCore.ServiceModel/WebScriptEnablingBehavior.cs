// .NET Framework's enableWebScript (System.ServiceModel.Web's WebScriptEnablingBehavior, referencesource), which CoreWCF
// has not: the "AJAX-enabled WCF service" ASP.NET AJAX's ScriptManager calls. On CoreWCF's webHttp: JSON in and out, the
// request's parameters wrapped (WrappedRequest), the reply as ASP.NET AJAX takes it ({"d": ...}: CoreWCF's JSON formatter
// AspNetAjaxReplyFormatter), GET's parameters as JSON values (JsonQueryStringConverter), an error as a JSON fault
// (JsonErrorHandler). Its script proxy (/js, /jsdebug) is WebScriptProxy's.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreWCF.Description;
using CoreWCF.Dispatcher;
using CoreWCF.Web;

namespace System.ServiceModel.Activation
{
    internal sealed class WebScriptEnablingBehavior : WebHttpBehavior
    {
        private static readonly Assembly s_webHttp = typeof(WebHttpBehavior).Assembly;
        private static readonly Type s_multiplexingFormatter = s_webHttp.GetType("CoreWCF.Dispatcher.MultiplexingDispatchMessageFormatter", throwOnError: true);

        public WebScriptEnablingBehavior(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override WebMessageBodyStyle DefaultBodyStyle
        {
            get => WebMessageBodyStyle.WrappedRequest;
            set { }
        }

        public override WebMessageFormat DefaultOutgoingRequestFormat { get; set; } = WebMessageFormat.Json;

        public override WebMessageFormat DefaultOutgoingResponseFormat { get; set; } = WebMessageFormat.Json;

        public override bool HelpEnabled
        {
            get => false;
            set { }
        }

        public override bool AutomaticFormatSelectionEnabled
        {
            get => false;
            set { }
        }

        public override bool FaultExceptionEnabled
        {
            get => false;
            set { }
        }

        // The reply as ASP.NET AJAX takes it: JSON, {"d": the result} (AspNetAjaxReplyFormatter), as CoreWCF's formats set its
        // content type (its multiplexing formatter).
        protected override IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            if (operationDescription.Messages.Count < 2)
            {
                return null;
            }
            DataContractSerializerOperationBehavior dcsob = operationDescription.OperationBehaviors.OfType<DataContractSerializerOperationBehavior>().FirstOrDefault();
            if (dcsob == null)
            {
                return base.GetReplyDispatchFormatter(operationDescription, endpoint);
            }
            var json = new AspNetAjaxReplyFormatter(operationDescription, dcsob.MaxItemsInObjectGraph, dcsob.IgnoreExtensionDataObject);
            var formatters = new Dictionary<WebMessageFormat, IDispatchMessageFormatter> { [WebMessageFormat.Json] = json };
            return (IDispatchMessageFormatter)Activator.CreateInstance(s_multiplexingFormatter,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                new object[] { formatters, WebMessageFormat.Json }, null);
        }

        // CoreWCF gives a webHttpBinding endpoint its WebHttpBehavior as well, applied before this one: its error handler (an
        // HTML page, 400) is not enableWebScript's, which is this one's (JsonErrorHandler).
        public override void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            base.ApplyDispatchBehavior(endpoint, endpointDispatcher);
            var handlers = endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers;
            foreach (IErrorHandler handler in handlers.Where(h => h.GetType().FullName == "CoreWCF.Dispatcher.WebErrorHandler").ToList())
            {
                handlers.Remove(handler);
            }
        }

        protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription) =>
            new JsonQueryStringConverter(operationDescription);

        protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) =>
            endpointDispatcher.DispatchRuntime.ChannelDispatcher.ErrorHandlers.Add(new JsonErrorHandler(endpointDispatcher.DispatchRuntime.ChannelDispatcher.IncludeExceptionDetailInFaults));
    }
}
