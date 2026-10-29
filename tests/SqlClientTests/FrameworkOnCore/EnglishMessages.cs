using System.Globalization;

namespace FrameworkOnCore.Tests.Shared
{
    /// <summary>
    /// The package's tests compare the exceptions' English messages; FrameworkOnCore's port has translations of them
    /// (its satellite assemblies), so on a Japanese Windows they would read Japanese: the tests run with English
    /// messages (the UI culture) whatever the culture they format with (the machine's, or FOC_TEST_CULTURE's:
    /// tests/Shared/TestEnvironment.cs), unless FOC_TEST_UICULTURE names another.
    /// </summary>
    static partial class TestEnvironment
    {
        static partial void AfterApply()
        {
            if (System.Environment.GetEnvironmentVariable("FOC_TEST_UICULTURE") != null) return;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        }
    }
}
