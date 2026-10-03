// enableWebScript's reply, as .NET Framework's JSON formatter wrote it with useAspNetAjaxJson (referencesource,
// System.ServiceModel.Web's DataContractJsonSerializerOperationFormatter: a bare reply in "d"): {"d": the result}, its
// objects with their types (__type, always: ASP.NET AJAX's proxies make them their client types), {"d":null} for void.
// CoreWCF's formatter has useAspNetAjaxJson for the serializer's type information, not the "d" of a bare reply.

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;
using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;

namespace System.ServiceModel.Activation
{
    internal sealed class AspNetAjaxReplyFormatter : IDispatchMessageFormatter
    {
        private readonly DataContractJsonSerializer _serializer;

        public AspNetAjaxReplyFormatter(OperationDescription operation, int maxItemsInObjectGraph, bool ignoreExtensionDataObject)
        {
            Type returnType = operation.Messages[1].Body.ReturnValue?.Type;
            if (returnType != null && returnType != typeof(void))
            {
                _serializer = new DataContractJsonSerializer(returnType, new DataContractJsonSerializerSettings
                {
                    RootName = "d",
                    KnownTypes = operation.KnownTypes,
                    MaxItemsInObjectGraph = maxItemsInObjectGraph,
                    IgnoreExtensionDataObject = ignoreExtensionDataObject,
                    EmitTypeInformation = EmitTypeInformation.Always,
                });
            }
        }

        public void DeserializeRequest(Message message, object[] parameters) =>
            throw new NotSupportedException("the reply's formatter does not read requests");

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            Message message = Message.CreateMessage(messageVersion, null, new Body(_serializer, result));
            message.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Json));
            return message;
        }

        private sealed class Body : BodyWriter
        {
            private readonly DataContractJsonSerializer _serializer;
            private readonly object _result;

            public Body(DataContractJsonSerializer serializer, object result)
                : base(true)
            {
                _serializer = serializer;
                _result = result;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("root");
                writer.WriteAttributeString("type", "object");
                if (_serializer == null)
                {
                    writer.WriteStartElement("d");
                    writer.WriteAttributeString("type", "null");
                    writer.WriteEndElement();
                }
                else
                {
                    _serializer.WriteObject(writer, _result);
                }
                writer.WriteEndElement();
            }
        }
    }
}
