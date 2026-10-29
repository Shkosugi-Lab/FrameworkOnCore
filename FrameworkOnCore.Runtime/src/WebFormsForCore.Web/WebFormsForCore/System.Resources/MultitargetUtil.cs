#if !NETFRAMEWORK

using System.Security;
using System.Threading;

#nullable disable
namespace System.Resources
{
  internal static class MultitargetUtil
  {
    public static string GetAssemblyQualifiedName(Type type, Func<Type, string> typeNameConverter)
    {
      string assemblyQualifiedName = (string) null;
      if (type != (Type) null)
      {
        if (typeNameConverter != null)
        {
          try
          {
            assemblyQualifiedName = typeNameConverter(type);
          }
          catch (Exception ex)
          {
            if (MultitargetUtil.IsSecurityOrCriticalException(ex))
              throw;
          }
        }
        if (string.IsNullOrEmpty(assemblyQualifiedName))
          assemblyQualifiedName = FrameworkName(type);
      }
      return assemblyQualifiedName;
    }

    // WebFormsForCore: the name .NET Framework's ResX writer wrote (and its reader, Visual Studio and MSBuild read): .NET's
    // types by their .NET Framework assemblies (mscorlib, System.Drawing... not System.Private.CoreLib), the ResX types
    // (here in System.Web) by System.Windows.Forms'.
    private const string WindowsForms = "System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

    internal static string FrameworkName(Type type)
    {
      if (type.Assembly == typeof(MultitargetUtil).Assembly && type.Namespace == "System.Resources" && type.Name.StartsWith("ResX", StringComparison.Ordinal))
        return type.FullName + ", " + WindowsForms;
      try
      {
        return WebFormsForCore.Serialization.Formatters.Binary.FormatterServices.GetFrameworkAssemblyQualifiedName(type);
      }
      catch (Exception ex) when (!MultitargetUtil.IsSecurityOrCriticalException(ex))
      {
        return type.AssemblyQualifiedName;
      }
    }

    private static bool IsSecurityOrCriticalException(Exception ex)
    {
      switch (ex)
      {
        case NullReferenceException _:
        case StackOverflowException _:
        case OutOfMemoryException _:
        case ThreadAbortException _:
        case Web.ResponseEndException _:
        case ExecutionEngineException _:
        case IndexOutOfRangeException _:
        case AccessViolationException _:
          return true;
        default:
          return ex is SecurityException;
      }
    }
  }
}
#endif