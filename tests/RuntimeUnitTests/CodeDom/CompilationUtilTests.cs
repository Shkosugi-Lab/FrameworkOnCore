using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Reflection;
using WebFormsForCore.CodeDom.Compiler;
using CodeDomProvider = WebFormsForCore.CodeDom.Compiler.CodeDomProvider;

namespace FrameworkOnCore.Runtime.Tests.CodeDom;

/// <summary>
/// The providers' options (CompilationUtil, ProviderOptions, AppSettings) as Microsoft.CodeDom.Providers.DotNetCompilerPlatform
/// documents and makes them (github.com/aspnet/RoslynCodeDomProvider: its README's providerOptions, environment variables and
/// appSettings; "DotNetCompilerPlatform" below): the compiler's location (ROSLYN_COMPILER_LOCATION, aspnet:RoslynCompilerLocation,
/// the CompilerLocation provider option, the application's bin), the compiler server's time to live (VBCSCOMPILER_TTL,
/// CompilerServerTTL; 10 seconds, 15 minutes in development), WarnAsError and UseAspNetSettings. Where the port differs (the
/// compilers csc.dll / vbc.dll in the bin itself, not bin\roslyn), the code's comments.
/// </summary>
public class CompilationUtilTests : CompilationTest
{
    const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;

    static string Bin => System.Web.Hosting.AppBinDirectory.PhysicalPath;

    /// <summary>The provider options of the language's &lt;compiler&gt; in system.codedom as given (its providerOption elements), for the action.</summary>
    static void WithProviderOptions(string extension, Dictionary<string, string> options, Action action)
    {
        var info = CodeDomProvider.GetCompilerInfo(CodeDomProvider.GetLanguageFromExtension(extension));
        var field = info.GetType().GetField("_providerOptions", Instance);
        var old = field.GetValue(info);
        try
        {
            field.SetValue(info, options);
            action();
        }
        finally { field.SetValue(info, old); }
    }

    static IProviderOptions For(string extension, string variable = null, string value = null)
    {
        IProviderOptions options = null;
        if (variable == null) options = CompilationUtil.GetProviderOptionsFor(extension);
        else WithEnvironment(variable, value, () => options = CompilationUtil.GetProviderOptionsFor(extension));
        return options;
    }

    // --- The compiler's location

    [Theory] // the port's choice: the compilers csc.dll and vbc.dll in the application's bin (DotNetCompilerPlatform: bin\roslyn\csc.exe)
    [InlineData(".cs", "csc.dll")]
    [InlineData("cs", "csc.dll")]
    [InlineData(".vb", "vbc.dll")]
    [InlineData("VB", "vbc.dll")]
    public void The_compiler_is_in_the_bin(string extension, string compiler)
    {
        Assert.Equal(Path.Combine(Bin, compiler), For(extension).CompilerFullPath);
    }

    [Theory] // DotNetCompilerPlatform: ROSLYN_COMPILER_LOCATION the compilers' folder; without csc.dll / vbc.dll there, csc.exe / vbc.exe
    [InlineData(".cs", "csc.exe")]
    [InlineData(".vb", "vbc.exe")]
    public void The_compiler_location_variable(string extension, string compiler)
    {
        Assert.Equal(Path.Combine(Folder, compiler), For(extension, "ROSLYN_COMPILER_LOCATION", Folder).CompilerFullPath);
        Write(Path.ChangeExtension(compiler, ".dll"), "");
        Assert.Equal(Path.Combine(Folder, Path.ChangeExtension(compiler, ".dll")), For(extension, "ROSLYN_COMPILER_LOCATION", Folder).CompilerFullPath);
    }

    [Fact] // DotNetCompilerPlatform: another language's options the folder itself; an extension no compiler has no options of its own
    public void Another_language()
    {
        var options = For(".xyz", "ROSLYN_COMPILER_LOCATION", Folder);
        Assert.Equal(Folder, options.CompilerFullPath);
        Assert.Empty(options.AllOptions);
        Assert.Equal(Folder, For(" ", "ROSLYN_COMPILER_LOCATION", Folder).CompilerFullPath);
    }

    [Fact] // DotNetCompilerPlatform: aspnet:RoslynCompilerLocation when the variable is not set
    public void The_compiler_location_app_setting()
    {
        WithAppSettings(new NameValueCollection { { "aspnet:RoslynCompilerLocation", Folder } }, () =>
        {
            Assert.Equal(Folder, AppSettings.RoslynCompilerLocation);
            Assert.True(AppSettings.DisableProfilingDuringCompilation, "not a boolean: the default");
            Assert.Equal(Path.Combine(Folder, "csc.exe"), For(".cs", "ROSLYN_COMPILER_LOCATION", null).CompilerFullPath);
        });
    }

    [Fact] // DotNetCompilerPlatform: the CompilerLocation provider option when neither is set
    public void The_compiler_location_provider_option()
    {
        WithProviderOptions(".cs", new Dictionary<string, string> { { "CompilerLocation", Folder } }, () =>
            Assert.Equal(Path.Combine(Folder, "csc.exe"), For(".cs").CompilerFullPath));
    }

    // --- The compiler server's time to live

    [Fact] // DotNetCompilerPlatform: 10 seconds by default; VBCSCOMPILER_TTL, else the CompilerServerTTL provider option
    public void The_time_to_live()
    {
        Assert.Equal(10, For(".cs").CompilerServerTimeToLive);
        Assert.Equal(5, For(".cs", "VBCSCOMPILER_TTL", "5").CompilerServerTimeToLive);
        Assert.Equal(10, For(".cs", "VBCSCOMPILER_TTL", "soon").CompilerServerTimeToLive);
        WithProviderOptions(".cs", new Dictionary<string, string> { { "CompilerServerTTL", "20" } }, () =>
            Assert.Equal(20, For(".cs").CompilerServerTimeToLive));
    }

    [Theory] // DotNetCompilerPlatform: 15 minutes in a development environment (DEV_ENVIRONMENT, IN_DEBUG_MODE)
    [InlineData("DEV_ENVIRONMENT")]
    [InlineData("IN_DEBUG_MODE")]
    public void The_time_to_live_in_development(string variable)
    {
        Assert.Equal(900, For(".cs", variable, "1").CompilerServerTimeToLive);
    }

    // --- The other options

    [Fact] // DotNetCompilerPlatform: CompilerVersion, WarnAsError (default false; not a boolean false) and UseAspNetSettings (default true; not a boolean true) of the provider options
    public void The_provider_options()
    {
        var defaults = For(".cs");
        Assert.False(defaults.WarnAsError);
        Assert.True(defaults.UseAspNetSettings);
        WithProviderOptions(".cs", new Dictionary<string, string> { { "CompilerVersion", "v4.0" }, { "WarnAsError", "true" }, { "UseAspNetSettings", "false" } }, () =>
        {
            var options = For(".cs");
            Assert.Equal("v4.0", options.CompilerVersion);
            Assert.True(options.WarnAsError);
            Assert.False(options.UseAspNetSettings);
            Assert.Equal("v4.0", options.AllOptions["CompilerVersion"]);
        });
        WithProviderOptions(".cs", new Dictionary<string, string> { { "WarnAsError", "yes" }, { "UseAspNetSettings", "no" } }, () =>
        {
            var options = For(".cs");
            Assert.False(options.WarnAsError);
            Assert.True(options.UseAspNetSettings);
            Assert.Null(options.CompilerVersion);
        });
    }

    [Fact] // DotNetCompilerPlatform (CreateProviderOptions): the dictionary's known options over the base's (case-sensitive; not a number or boolean ignored); AllOptions the base's with the AllOptions entry
    public void Options_from_a_dictionary()
    {
        var baseOptions = new ProviderOptions("base.dll", 3) { CompilerVersion = "v3.5", UseAspNetSettings = true, AllOptions = new Dictionary<string, string> { { "Other", "x" } } };
        var options = CompilationUtil.CreateProviderOptions(new Dictionary<string, string>
        {
            { "CompilerFullPath", "given.dll" },
            { "CompilerServerTimeToLive", "8" },
            { "CompilerVersion", "v4.0" },
            { "WarnAsError", "true" },
            { "AllOptions", "all" },
            { "warnaserror", "false" },
            { " ", "blank" },
        }, baseOptions);
        Assert.Equal("given.dll", options.CompilerFullPath);
        Assert.Equal(8, options.CompilerServerTimeToLive);
        Assert.Equal("v4.0", options.CompilerVersion);
        Assert.True(options.WarnAsError);
        Assert.True(options.UseAspNetSettings);
        Assert.Equal(new Dictionary<string, string> { { "Other", "x" }, { "AllOptions", "all" } }, options.AllOptions);

        var unparsed = CompilationUtil.CreateProviderOptions(new Dictionary<string, string>
        {
            { "CompilerServerTimeToLive", "long" },
            { "WarnAsError", "maybe" },
        }, baseOptions);
        Assert.Equal(3, unparsed.CompilerServerTimeToLive);
        Assert.False(unparsed.WarnAsError);
        Assert.Equal(baseOptions.AllOptions, unparsed.AllOptions);
    }

    [Fact] // DotNetCompilerPlatform (PrependCompilerOption): an option before the parameters' others, or the only one
    public void An_option_prepended()
    {
        var parameters = new CompilerParameters();
        CompilationUtil.PrependCompilerOption(parameters, "/a");
        Assert.Equal("/a", parameters.CompilerOptions);
        CompilationUtil.PrependCompilerOption(parameters, "/b");
        Assert.Equal("/b /a", parameters.CompilerOptions);
    }

    [Fact] // the port's choice: the compilers in the application's bin itself
    public void The_default_path_is_the_bin()
    {
        Assert.Equal(Bin, CompilationUtil.CompilerDefaultPath());
    }

    // --- ProviderOptions

    [Fact] // DotNetCompilerPlatform (ProviderOptions): defaults (no compiler server, not ASP.NET mode, no options); a copy of other options
    public void Provider_options()
    {
        var defaults = new ProviderOptions();
        Assert.Null(defaults.CompilerFullPath);
        Assert.Equal(0, defaults.CompilerServerTimeToLive);
        Assert.Null(defaults.CompilerVersion);
        Assert.False(defaults.WarnAsError);
        Assert.False(defaults.UseAspNetSettings);
        Assert.Empty(defaults.AllOptions);

        var copy = new ProviderOptions(CompilationUtil.CSC2);
        Assert.Equal(CompilationUtil.CSC2.CompilerFullPath, copy.CompilerFullPath);
        Assert.Equal(CompilationUtil.CSC2.CompilerServerTimeToLive, copy.CompilerServerTimeToLive);
        Assert.True(copy.UseAspNetSettings);
        Assert.Equal(CompilationUtil.CSC2.AllOptions, copy.AllOptions);
        Assert.Throws<NotSupportedException>(() => copy.AllOptions.Add("x", "y"));

        defaults.AllOptions = null;
        Assert.Null(defaults.AllOptions);
    }

    // --- AppSettings

    [Fact] // DotNetCompilerPlatform (AppSettings): aspnet:DisableProfilingDuringCompilation a boolean (default true); read once
    public void App_settings()
    {
        WithAppSettings(new NameValueCollection { { "aspnet:DisableProfilingDuringCompilation", "False" } }, () =>
        {
            Assert.False(AppSettings.DisableProfilingDuringCompilation);
            Assert.Null(AppSettings.RoslynCompilerLocation);
        });
        WithAppSettings(new NameValueCollection(), () => Assert.True(AppSettings.DisableProfilingDuringCompilation));
    }
}
