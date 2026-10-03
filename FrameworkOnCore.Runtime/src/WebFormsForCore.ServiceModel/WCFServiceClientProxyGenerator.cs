// The script proxy of an enableWebScript endpoint (Service.svc/js, /jsdebug: ScriptManager's ServiceReference loads it), as
// .NET Framework's System.ServiceModel.Web wrote it (WCFServiceClientProxyGenerator, referencesource): System.Web.Extensions'
// ClientProxyGenerator (FrameworkOnCore's port) over the contract's operations. The contract is read from its type (its
// ServiceContract's name and namespace, its operations, their parameters, WebGet's ones called by GET), as WCF's
// ContractDescription had it.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Services;
using System.Xml;

namespace System.ServiceModel.Activation
{
    internal sealed class WCFServiceClientProxyGenerator : ClientProxyGenerator
    {
        private const int MaxIdentifierLength = 511;
        private const string DataContractXsdBaseNamespace = "http://schemas.datacontract.org/2004/07/";
        private const string DefaultNamespace = "http://tempuri.org/";

        private readonly string _path;

        private WCFServiceClientProxyGenerator(string path, bool debugMode)
        {
            _path = path;
            _debugMode = debugMode;
        }

        public static string GetClientProxyScript(Type contractType, string path, bool debugMode) =>
            new WCFServiceClientProxyGenerator(path, debugMode).GetClientProxyScript(GetWebServiceData(contractType));

        protected override string GetProxyPath() => _path;

        private static Type ReplaceMessageWithObject(Type type) =>
            type.FullName is "CoreWCF.Channels.Message" or "System.ServiceModel.Channels.Message" ? typeof(object) : type;

        private static CustomAttributeData Attribute(MemberInfo member, string name) =>
            member.GetCustomAttributesData().FirstOrDefault(a => a.AttributeType.Name == name);

        private static string Named(CustomAttributeData attribute, string name) =>
            attribute?.NamedArguments.Where(a => a.MemberName == name).Select(a => a.TypedValue.Value as string).FirstOrDefault();

        private static bool HasNamed(CustomAttributeData attribute, string name) =>
            attribute != null && attribute.NamedArguments.Any(a => a.MemberName == name);

        // The contract and the contracts it inherits: their operations (OperationContract), as WCF's ContractDescription.
        private static IEnumerable<MethodInfo> Operations(Type contract) =>
            new[] { contract }.Concat(contract.GetInterfaces().Where(i => Attribute(i, "ServiceContractAttribute") != null))
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                .Where(m => Attribute(m, "OperationContractAttribute") != null);

        private static IEnumerable<Type> KnownTypes(MemberInfo member) =>
            member.GetCustomAttributesData().Where(a => a.AttributeType.Name == "ServiceKnownTypeAttribute" && a.ConstructorArguments.Count == 1 && a.ConstructorArguments[0].Value is Type)
                .Select(a => (Type)a.ConstructorArguments[0].Value);

        private static WebServiceData GetWebServiceData(Type contractType)
        {
            CustomAttributeData serviceContract = Attribute(contractType, "ServiceContractAttribute");
            string name = Named(serviceContract, "Name") ?? contractType.Name;
            string ns = HasNamed(serviceContract, "Namespace") ? Named(serviceContract, "Namespace") ?? "" : DefaultNamespace;

            var serviceData = new WebServiceData();
            var methods = new Dictionary<string, WebServiceMethodData>();
            serviceData.Initialize(new WebServiceTypeData(XmlConvert.DecodeName(name), XmlConvert.DecodeName(ns), contractType), methods);
            var contractKnownTypes = KnownTypes(contractType).ToList();
            foreach (MethodInfo operation in Operations(contractType))
            {
                CustomAttributeData operationContract = Attribute(operation, "OperationContractAttribute");
                string operationName = Named(operationContract, "Name") ?? operation.Name;
                var parameters = new Dictionary<string, WebServiceParameterData>();
                bool useHttpGet = Attribute(operation, "WebGetAttribute") != null;
                var methodData = new WebServiceMethodData(serviceData, XmlConvert.DecodeName(operationName), parameters, useHttpGet);
                int index = 0;
                foreach (ParameterInfo parameter in operation.GetParameters().Where(p => !p.IsOut))
                {
                    Type type = ReplaceMessageWithObject(parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType);
                    string parameterName = parameter.GetCustomAttributesData().Where(a => a.AttributeType.Name == "MessageParameterAttribute")
                        .Select(a => Named(a, "Name")).FirstOrDefault(n => n != null) ?? parameter.Name;
                    var parameterData = new WebServiceParameterData(XmlConvert.DecodeName(parameterName), type, index++);
                    parameters[parameterData.ParameterName] = parameterData;
                    serviceData.ProcessClientType(type, false, true);
                }
                if (operation.ReturnType != typeof(void))
                {
                    serviceData.ProcessClientType(ReplaceMessageWithObject(operation.ReturnType), false, true);
                }
                foreach (Type knownType in contractKnownTypes.Concat(KnownTypes(operation)))
                {
                    serviceData.ProcessClientType(knownType, false, true);
                }
                methods[methodData.MethodName] = methodData;
            }
            serviceData.ClearProcessedTypes();
            return serviceData;
        }

        protected override string GetClientTypeNamespace(string ns)
        {
            if (string.IsNullOrEmpty(ns))
            {
                return string.Empty;
            }
            var builder = new StringBuilder();
            if (Uri.TryCreate(ns, UriKind.RelativeOrAbsolute, out Uri uri))
            {
                if (!uri.IsAbsoluteUri)
                {
                    AddToNamespace(builder, uri.OriginalString);
                }
                else
                {
                    string uriString = uri.AbsoluteUri;
                    if (uriString.StartsWith(DataContractXsdBaseNamespace, StringComparison.Ordinal))
                    {
                        AddToNamespace(builder, uriString.Substring(DataContractXsdBaseNamespace.Length));
                    }
                    else
                    {
                        if (uri.Host != null)
                        {
                            AddToNamespace(builder, uri.Host);
                        }
                        if (uri.PathAndQuery != null)
                        {
                            AddToNamespace(builder, uri.PathAndQuery);
                        }
                    }
                }
            }
            if (builder.Length == 0)
            {
                return string.Empty;
            }
            int length = builder.Length;
            if (builder[builder.Length - 1] == '.')
            {
                length--;
            }
            return builder.ToString(0, Math.Min(MaxIdentifierLength, length));
        }

        protected override string GetProxyTypeName(WebServiceData data) => GetClientTypeNamespace(data.TypeData.TypeName);

        private static void AddToNamespace(StringBuilder builder, string fragment)
        {
            if (fragment == null)
            {
                return;
            }
            bool isStart = true;
            for (int i = 0; i < fragment.Length && builder.Length < MaxIdentifierLength; i++)
            {
                char c = fragment[i];
                if (IsValid(c))
                {
                    if (isStart && !IsValidStart(c))
                    {
                        builder.Append('_');
                    }
                    builder.Append(c);
                    isStart = false;
                }
                else if ((c == '.' || c == '/' || c == ':') && (builder.Length == 1 || (builder.Length > 1 && builder[builder.Length - 1] != '.')))
                {
                    builder.Append('.');
                    isStart = true;
                }
            }
        }

        private static bool IsValid(char c)
        {
            switch (char.GetUnicodeCategory(c))
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.ConnectorPunctuation:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsValidStart(char c) => char.GetUnicodeCategory(c) != UnicodeCategory.DecimalDigitNumber;
    }
}
