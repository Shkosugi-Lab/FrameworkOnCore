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

            /// <summary>
            /// Where the application's assemblies are, under ApplicationBase: bin, as ASP.NET set it (OWIN's startup
            /// discovery scans these folders). Setting it has no effect: .NET does not probe such folders.
            /// </summary>
            public string PrivateBinPath
            {
                get => "bin";
                set { }
            }

            /// <summary>
            /// Not null: the application's base folder itself is not searched, only PrivateBinPath ("*", as ASP.NET set it).
            /// </summary>
            public string PrivateBinPathProbe
            {
                get => "*";
                set { }
            }
        }
    }
}
