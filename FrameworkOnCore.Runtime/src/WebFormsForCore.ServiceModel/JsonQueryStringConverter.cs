// enableWebScript's query string values (System.ServiceModel.Web's JsonQueryStringConverter, referencesource): ASP.NET AJAX's
// proxy sends a GET operation's parameters as JSON values ("Taro" quoted, an object as {...}); numbers and booleans as
// they are.

using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using CoreWCF.Description;
using CoreWCF.Dispatcher;

namespace System.ServiceModel.Activation
{
    internal sealed class JsonQueryStringConverter : QueryStringConverter
    {
        private readonly OperationDescription _operation;

        public JsonQueryStringConverter(OperationDescription operation)
        {
            _operation = operation;
        }

        public override bool CanConvert(Type type) => true;

        public override object ConvertStringToValue(string parameter, Type parameterType)
        {
            switch (Type.GetTypeCode(parameterType))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Boolean:
                    return base.ConvertStringToValue(parameter, parameterType);
                case TypeCode.Char:
                case TypeCode.String:
                case TypeCode.DateTime:
                    return StartsWith(parameter, '"') ? Deserialize(parameter.Trim(), parameterType) : base.ConvertStringToValue(parameter, parameterType);
            }
            if (parameterType == typeof(Guid) || parameterType == typeof(Uri) || parameterType == typeof(TimeSpan))
            {
                return parameter == null ? Default(parameterType) : StartsWith(parameter, '"') ? Deserialize(parameter.Trim(), parameterType) : base.ConvertStringToValue(parameter, parameterType);
            }
            if (parameterType == typeof(byte[]))
            {
                return parameter == null ? null : StartsWith(parameter, '[') ? Deserialize(parameter.Trim(), parameterType) : base.ConvertStringToValue(parameter, parameterType);
            }
            if (parameterType == typeof(DateTimeOffset) || parameterType == typeof(object))
            {
                return parameter == null ? Default(parameterType) : StartsWith(parameter, '{') ? Deserialize(parameter.Trim(), parameterType) : base.ConvertStringToValue(parameter, parameterType);
            }
            return parameter == null ? null : Deserialize(parameter.Trim(), parameterType);
        }

        public override string ConvertValueToString(object parameter, Type parameterType)
        {
            if (parameter == null)
            {
                return null;
            }
            using var stream = new MemoryStream();
            using (XmlDictionaryWriter writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, ownsStream: false))
            {
                Serializer(parameterType).WriteObject(writer, parameter);
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private object Deserialize(string parameter, Type parameterType)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(parameter);
            using XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(bytes, 0, bytes.Length, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, null);
            return Serializer(parameterType).ReadObject(reader);
        }

        private DataContractJsonSerializer Serializer(Type type) =>
            _operation == null ? new DataContractJsonSerializer(type) : new DataContractJsonSerializer(type, _operation.KnownTypes);

        private static bool StartsWith(string parameter, char character)
        {
            string trimmed = parameter?.Trim();
            return !string.IsNullOrEmpty(trimmed) && trimmed[0] == character;
        }

        private static object Default(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
