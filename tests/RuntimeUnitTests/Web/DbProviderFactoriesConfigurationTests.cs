using System.Data.Common;
using System.Reflection;
using System.Web;
using FrameworkOnCore.Runtime.Tests.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// The ADO.NET providers by invariant name as .NET Framework has them (DbProviderFactoriesConfiguration): its built-in
/// ones (machine.config's System.Data.SqlClient, OleDb, Odbc) and the application's web.config's
/// system.data/DbProviderFactories (add, remove, clear: the element's documentation). DbProviderFactories is
/// process-wide: the Application collection.
/// </summary>
[Collection("Application")]
public class DbProviderFactoriesConfigurationTests : IDisposable
{
    static readonly FieldInfo DefaultsRegistered = typeof(DbProviderFactoriesConfiguration).GetField("defaultsRegistered", BindingFlags.NonPublic | BindingFlags.Static);

    public DbProviderFactoriesConfigurationTests() => Restore();

    public void Dispose() => Restore();

    // The defaults only (registered again), as the process has them.
    static void Restore()
    {
        foreach (var name in DbProviderFactories.GetProviderInvariantNames().ToList()) DbProviderFactories.UnregisterFactory(name);
        DefaultsRegistered.SetValue(null, 0);
        DbProviderFactoriesConfiguration.EnsureDefaults();
    }

    sealed class OtherFactory : DbProviderFactory
    {
        public static readonly OtherFactory Instance = new();
    }

    static string[] Names() => DbProviderFactories.GetProviderInvariantNames().OrderBy(n => n, StringComparer.Ordinal).ToArray();

    static readonly string[] Defaults = { "System.Data.Odbc", "System.Data.OleDb", "System.Data.SqlClient" };

    static string Config(string factories) =>
        "<configuration><system.data><DbProviderFactories>" + factories + "</DbProviderFactories></system.data></configuration>";

    [Fact] // .NET Framework 4.8's machine.config: System.Data.SqlClient, OleDb and Odbc registered (by type name: resolved when asked for); once, a registration kept
    public void The_built_in_providers_are_registered_once()
    {
        Assert.Equal(Defaults, Names());
        DbProviderFactories.UnregisterFactory("System.Data.Odbc");
        DbProviderFactories.RegisterFactory("System.Data.OleDb", OtherFactory.Instance);
        DbProviderFactoriesConfiguration.EnsureDefaults();
        Assert.Equal(new[] { "System.Data.OleDb", "System.Data.SqlClient" }, Names());
        DefaultsRegistered.SetValue(null, 0);
        DbProviderFactoriesConfiguration.EnsureDefaults();
        Assert.Equal(Defaults, Names());
        Assert.Same(OtherFactory.Instance, DbProviderFactories.GetFactory("System.Data.OleDb"));
    }

    [Fact] // system.data/DbProviderFactories' documentation: add (an invariant name and a type), remove, clear, in the order written
    public void The_application_s_providers_are_added_removed_and_cleared()
    {
        using var site = new Site(Config(
            "<add name='A' invariant='Test.A' type='Test.A.Factory, Test.A' />" +
            "<add name='B' invariant='Test.B' type='Test.B.Factory, Test.B' />" +
            "<remove invariant='Test.B' /><remove invariant='System.Data.Odbc' /><remove />" +
            "<add name='no type' invariant='Test.C' /><add name='no invariant' type='Test.D.Factory, Test.D' />"));
        DbProviderFactoriesConfiguration.Register(site.Root);
        Assert.Equal(new[] { "System.Data.OleDb", "System.Data.SqlClient", "Test.A" }, Names());

        using var cleared = new Site(Config("<clear /><add name='E' invariant='Test.E' type='Test.E.Factory, Test.E' />"));
        DbProviderFactoriesConfiguration.Register(cleared.Root);
        Assert.Equal(new[] { "Test.E" }, Names());
    }

    [Fact] // the class's summary: the application's web.config (its name in any case, as on Windows)
    public void The_web_config_s_name_in_any_case()
    {
        using var site = new Site();
        site.Write("Web.Config", Config("<add name='A' invariant='Test.A' type='Test.A.Factory, Test.A' />"));
        DbProviderFactoriesConfiguration.Register(site.Root);
        Assert.Contains("Test.A", Names());
    }

    [Fact] // the class's comments: no folder, no web.config, no system.data section, a web.config that is no XML: the built-in providers only
    public void Without_the_application_s_providers_the_built_in_ones()
    {
        DbProviderFactoriesConfiguration.Register(null);
        DbProviderFactoriesConfiguration.Register(Path.Combine(Path.GetTempPath(), "foc-missing-" + Guid.NewGuid().ToString("N")));
        using (var empty = new Site()) DbProviderFactoriesConfiguration.Register(empty.Root);
        using (var noData = new Site("<configuration><system.web /></configuration>")) DbProviderFactoriesConfiguration.Register(noData.Root);
        using (var noFactories = new Site("<configuration><system.data /></configuration>")) DbProviderFactoriesConfiguration.Register(noFactories.Root);
        using (var broken = new Site("<configuration><system.data>")) DbProviderFactoriesConfiguration.Register(broken.Root);
        Assert.Equal(Defaults, Names());
    }
}
