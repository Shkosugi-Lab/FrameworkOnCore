using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebForm2Blazor.Components;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// Test context for converted components.
/// Registers the WebForms compatibility runtime just like the generated Program.cs.
/// </summary>
public abstract class WebFormsTestContext : TestContext
{
    protected WebFormsTestContext(Dictionary<string, string?>? appSettings = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(appSettings ?? [])
            .Build();

        Services.AddWebFormsCompat(configuration);

        // Let OnClientClick JS execution (eval) pass through in tests
        // (Loose mode returns the default null for unconfigured calls = the script
        // is treated as continuing)
        JSInterop.Mode = JSRuntimeMode.Loose;
    }
}
