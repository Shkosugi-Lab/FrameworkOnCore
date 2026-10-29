using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Reflection;
using WebFormsForCore.CodeDom.Compiler;

namespace FrameworkOnCore.Runtime.Tests.CodeDom;

/// <summary>
/// A test of the CodeDom providers (WebFormsForCore.CodeDom.Compiler): its own temporary folder, where the compilations
/// keep their files (CompilerParameters.TempFiles), deleted after it. The CodeDom tests are in the CodeDom collection: the
/// providers' settings and the environment variables the compilers read are process-wide.
/// </summary>
[Collection("CodeDom")]
public abstract class CompilationTest : IDisposable
{
    const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    /// <summary>VB's My.* feature off, as ASP.NET's AssemblyBuilder asks for it (AddVBMyFlags): .NET's VB runtime has no My types.</summary>
    public const string NoMy = "/define:_MYTYPE=\\\"Empty\\\"";

    protected readonly string Folder = Directory.CreateTempSubdirectory("foc-codedom").FullName;

    static CompilationTest()
    {
        // The providers' appSettings (aspnet:DisableProfilingDuringCompilation, aspnet:RoslynCompilerLocation): without an
        // application, System.Web's configuration throws the first time, then the defaults (as Site.Run has it). Read before
        // CompilationUtil, whose static constructor reads them.
        try { _ = AppSettings.DisableProfilingDuringCompilation; }
        catch (Exception e) when (e.GetType().Name == "ConfigurationErrorsException") { }
    }

    public void Dispose()
    {
        try { Directory.Delete(Folder, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }

    /// <summary>Parameters whose temporary files are in the test's folder.</summary>
    protected CompilerParameters Parameters(int warningLevel = -1, string compilerOptions = null, bool inMemory = false, bool debug = false, params string[] references)
    {
        var parameters = new CompilerParameters(references)
        {
            TempFiles = new TempFileCollection(Folder),
            WarningLevel = warningLevel,
            CompilerOptions = compilerOptions,
            GenerateInMemory = inMemory,
            IncludeDebugInformation = debug,
        };
        return parameters;
    }

    /// <summary>The provider options of the in-process compiler without ASP.NET's settings and without a compiler server.</summary>
    protected static ProviderOptions Plain(string extension = ".cs", bool warnAsError = false) =>
        new ProviderOptions(extension == ".cs" ? CompilationUtil.CSC2.CompilerFullPath : CompilationUtil.VBC2.CompilerFullPath, 0) { WarnAsError = warnAsError };

    /// <summary>The command line of a compilation the compiler's output was read of (the first line: the compiler and its arguments).</summary>
    protected static string CommandLine(CompilerResults results) => results.Output[0];

    /// <summary>A file of the test's folder.</summary>
    protected string Write(string name, string text)
    {
        var path = Path.Combine(Folder, name);
        File.WriteAllText(path, text);
        return path;
    }

    /// <summary>The providers' appSettings as given, for the action; put back after.</summary>
    protected static void WithAppSettings(NameValueCollection settings, Action action)
    {
        var profiling = typeof(AppSettings).GetField("_disableProfilingDuringCompilation", Static);
        var location = typeof(AppSettings).GetField("_roslynCompilerLocation", Static);
        var (oldProfiling, oldLocation) = (profiling.GetValue(null), location.GetValue(null));
        try
        {
            typeof(AppSettings).GetMethod("LoadSettings", Static).Invoke(null, new object[] { settings });
            action();
        }
        finally
        {
            profiling.SetValue(null, oldProfiling);
            location.SetValue(null, oldLocation);
        }
    }

    /// <summary>The process's environment variable as given, for the action; put back after.</summary>
    protected static void WithEnvironment(string name, string value, Action action)
    {
        var old = Environment.GetEnvironmentVariable(name);
        try
        {
            Environment.SetEnvironmentVariable(name, value);
            action();
        }
        finally { Environment.SetEnvironmentVariable(name, old); }
    }
}

[CollectionDefinition("CodeDom")]
public sealed class CodeDomCollection { }
