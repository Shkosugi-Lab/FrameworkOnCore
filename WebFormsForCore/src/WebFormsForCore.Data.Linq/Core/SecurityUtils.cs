// WebFormsForCore: replaces misc\SecurityUtils.cs (excluded from the build). The original demanded
// ReflectionPermission before invoking (Code Access Security); .NET has no CAS and runs fully trusted,
// so the invoke stays and the demand goes.
namespace System.Data.Linq
{
    using System.Reflection;

    internal static class SecurityUtils
    {
        internal static object MethodInfoInvoke(MethodInfo method, object target, object[] args)
        {
            return method.Invoke(target, args);
        }
    }
}
