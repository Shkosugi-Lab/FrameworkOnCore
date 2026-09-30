using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FrameworkOnCore
{
    /// <summary>
    /// What a library's DLL calls in place of its own code where .NET differs from .NET Framework in a way the library did
    /// not expect (rules/packages.json: dllCallReplacements; the converter rewrites the call in the DLL, AssemblyRetargeter).
    /// </summary>
    public static class DllFixes
    {
        /// <summary>
        /// A type's methods declared in it, the public ones: AutoMapper (before 12) gathers LINQ's extension methods from
        /// every method of Enumerable marked [Extension], private ones too. .NET Framework's Enumerable had none private;
        /// .NET's has (MaxInteger&lt;T&gt; where T : IBinaryInteger&lt;T&gt;), and AutoMapper's MakeGenericMethod on them breaks their
        /// constraints: every MapperConfiguration failed (nopCommerce 3.90's startup).
        /// </summary>
        public static IEnumerable<MethodInfo> PublicDeclaredMethods(System.Type type) =>
            type.GetTypeInfo().DeclaredMethods.Where(m => m.IsPublic);
    }
}
