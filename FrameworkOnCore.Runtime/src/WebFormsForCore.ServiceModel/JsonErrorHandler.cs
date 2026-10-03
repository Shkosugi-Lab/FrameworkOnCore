// enableWebScript's error (System.ServiceModel.Web's WebScriptEnablingBehavior.JsonErrorHandler, referencesource): a 500 whose
// body is a JsonFaultDetail and whose header jsonerror says so, as ASP.NET AJAX's proxy reads it (its failed callback's
// error). Without includeExceptionDetailInFaults, only WCF's message of an internal error (as .NET Framework: not the
// exception's, not a FaultException's reason).

using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Dispatcher;

namespace System.ServiceModel.Activation
{
    [DataContract(Name = "JsonFaultDetail", Namespace = "http://schemas.datacontract.org/2004/07/System.ServiceModel.Description")]
    internal sealed class JsonFaultDetail
    {
        [DataMember(Name = "ExceptionDetail")] public CoreWCF.ExceptionDetail ExceptionDetail { get; set; }
        [DataMember(Name = "ExceptionType")] public string ExceptionType { get; set; }
        [DataMember(Name = "Message")] public string Message { get; set; }
        [DataMember(Name = "StackTrace")] public string StackTrace { get; set; }
    }

    internal sealed class JsonErrorHandler : IErrorHandler
    {
        internal const string InternalServerError = "The server was unable to process the request due to an internal error.  For more information about the error, either turn on IncludeExceptionDetailInFaults (either from ServiceBehaviorAttribute or from the <serviceDebug> configuration behavior) on the server in order to send the exception information back to the client, or turn on tracing as per the Microsoft .NET Framework SDK documentation and inspect the server trace logs.";

        private static readonly DataContractJsonSerializer s_serializer = new DataContractJsonSerializer(typeof(JsonFaultDetail));
        private readonly bool _includeExceptionDetailInFaults;

        public JsonErrorHandler(bool includeExceptionDetailInFaults)
        {
            _includeExceptionDetailInFaults = includeExceptionDetailInFaults;
        }

        public bool HandleError(Exception error) => false;

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            var detail = new JsonFaultDetail { Message = InternalServerError };
            if (_includeExceptionDetailInFaults)
            {
                CoreWCF.FaultException converted = error as CoreWCF.FaultException ?? FaultBridge.Convert(error);
                detail.Message = converted?.Reason.ToString() ?? error.Message;
                if (converted == null)
                {
                    var exceptionDetail = new CoreWCF.ExceptionDetail(error);
                    detail.ExceptionDetail = exceptionDetail;
                    detail.ExceptionType = exceptionDetail.Type;
                    detail.StackTrace = exceptionDetail.StackTrace;
                }
            }
            var response = new HttpResponseMessageProperty { StatusCode = HttpStatusCode.InternalServerError };
            response.Headers[HttpResponseHeader.ContentType] = "application/json; charset=utf-8";
            response.Headers["jsonerror"] = "true";
            fault = Message.CreateMessage(version, string.Empty, new JsonBody(detail));
            fault.Properties.Add(HttpResponseMessageProperty.Name, response);
            fault.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Json));
        }

        private sealed class JsonBody : BodyWriter
        {
            private readonly JsonFaultDetail _detail;

            public JsonBody(JsonFaultDetail detail)
                : base(false)
            {
                _detail = detail;
            }

            protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer) => s_serializer.WriteObject(writer, _detail);
        }
    }
}
