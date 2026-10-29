using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Data.SqlClient.Tests
{
    /// <summary>
    /// The package's tests compare the exceptions' English messages; FrameworkOnCore's port has translations of them
    /// (its satellite assemblies), so on a Japanese Windows they would read Japanese: the tests run in English.
    /// </summary>
    static class EnglishMessages
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        }
    }
}
