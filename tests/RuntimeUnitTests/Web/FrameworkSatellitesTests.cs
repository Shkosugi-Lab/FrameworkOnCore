using System.Reflection;
using System.Runtime.Loader;
using System.Web.Util;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>FrameworkSatellites: a satellite .NET does not find next to its assembly, looked for in the application's folder (its summary).</summary>
public class FrameworkSatellitesTests
{
    static readonly MethodInfo Resolve = typeof(FrameworkSatellites).GetMethod("Resolve", BindingFlags.NonPublic | BindingFlags.Static);

    [Fact] // the class's summary: <culture>\<name>.resources.dll in the application's folder; none there, or not a satellite: not resolved
    public void A_satellite_in_the_application_s_folder_is_found()
    {
        var context = new AssemblyLoadContext("satellites", isCollectible: true);
        try
        {
            Assert.True(File.Exists(Path.Combine(AppContext.BaseDirectory, "ja", "Microsoft.TestPlatform.CoreUtilities.resources.dll")));
            var satellite = (Assembly)Resolve.Invoke(null, new object[] { context, new AssemblyName("Microsoft.TestPlatform.CoreUtilities.resources") { CultureName = "ja" } });
            Assert.Equal("ja", satellite.GetName().CultureName);
            Assert.Null(Resolve.Invoke(null, new object[] { context, new AssemblyName("Missing.resources") { CultureName = "ja" } }));
            Assert.Null(Resolve.Invoke(null, new object[] { context, new AssemblyName("Microsoft.TestPlatform.CoreUtilities") { CultureName = "ja" } }));
            Assert.Null(Resolve.Invoke(null, new object[] { context, new AssemblyName("Microsoft.TestPlatform.CoreUtilities.resources") }));
        }
        finally { context.Unload(); }
    }
}
