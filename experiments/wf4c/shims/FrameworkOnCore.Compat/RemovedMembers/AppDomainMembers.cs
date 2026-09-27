using System.Reflection;
using System.Reflection.Emit;

namespace System
{
    /// <summary>
    /// Members .NET's AppDomain and AppDomainSetup do not have, as .NET Framework's applications use them.
    /// </summary>
    public static class AppDomainMembers
    {
        extension(AppDomain domain)
        {
            /// <summary>
            /// A dynamic assembly: .NET's AssemblyBuilder.DefineDynamicAssembly (the assembly is in memory; a
            /// .NET Framework application saving it - RunAndSave, a directory - saved it to look at it).
            /// </summary>
            public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access) =>
                AssemblyBuilder.DefineDynamicAssembly(name, access);

            public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir) =>
                AssemblyBuilder.DefineDynamicAssembly(name, access);
        }

        extension(AppDomainSetup setup)
        {
            /// <summary>
            /// The application's configuration file: in a web application, its web.config (log4net finds its
            /// configuration from it). AppDomain.BaseDirectory is the application's root (WebFormsForCore).
            /// </summary>
            public string ConfigurationFile =>
                AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE") as string ??
                IO.Path.Combine(AppContext.BaseDirectory, "web.config");
        }
    }
}
