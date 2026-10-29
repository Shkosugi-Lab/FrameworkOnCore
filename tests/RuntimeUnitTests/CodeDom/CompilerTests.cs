using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using WebFormsForCore.CodeDom.Compiler;
using CSharpCodeProvider = WebFormsForCore.CodeDom.Compiler.CSharpCodeProvider;

namespace FrameworkOnCore.Runtime.Tests.CodeDom;

/// <summary>
/// What the compilers share (Compiler: the C# and VB compilers' base), against .NET Framework's CodeDom compilers
/// (System.CodeDom.Compiler.CodeCompiler, Microsoft.CSharp.CSharpCodeGenerator's FromFileBatch: referencesource.microsoft.com,
/// "the reference source" below) and Microsoft.CodeDom.Providers.DotNetCompilerPlatform's Compiler (github.com/aspnet/
/// RoslynCodeDomProvider, "DotNetCompilerPlatform" below), which ASP.NET projects used: CompilerParameters and CompilerResults
/// as learn.microsoft.com documents them ("the docs"). Where .NET Framework's cannot apply (no csc.exe process: the
/// compiler run in-process), the code's comments.
/// </summary>
public class CompilerTests : CompilationTest
{
    const string Class = "namespace N { public class A { public static int Answer() { return 42; } } }";

#pragma warning disable CS0618 // ICodeCompiler: the compiler itself, its single-item overloads
    static ICodeCompiler Compiler(IProviderOptions options = null) => new CSharpCodeProvider(options ?? CompilationUtil.CSC2).CreateCompiler();
#pragma warning restore CS0618

    // --- Arguments

    [Fact] // the docs (ICodeCompiler): ArgumentNullException for a null options or input
    public void Null_arguments_throw()
    {
        var compiler = Compiler();
        var parameters = Parameters();
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromDom(null, new CodeCompileUnit()));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromDom(parameters, null));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromDomBatch(null, new CodeCompileUnit[0]));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromDomBatch(parameters, null));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromFile(null, "a.cs"));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromFile(parameters, null));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromFileBatch(null, new string[0]));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromFileBatch(parameters, null));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromSource(null, Class));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromSource(parameters, null));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromSourceBatch(null, new string[0]));
        Assert.Throws<ArgumentNullException>(() => compiler.CompileAssemblyFromSourceBatch(parameters, null));
    }

    [Fact] // the reference source (CompileAssemblyFromFileBatch): the files opened first, a missing one throws
    public void A_missing_source_file_throws()
    {
        var compiler = Compiler();
        Assert.Throws<FileNotFoundException>(() => compiler.CompileAssemblyFromFile(Parameters(), Path.Combine(Folder, "missing.cs")));
    }

    [Fact] // DotNetCompilerPlatform (CompilerName): the compiler's file opened first, a missing one throws
    public void A_missing_compiler_throws()
    {
        var provider = new CSharpCodeProvider(new ProviderOptions(Path.Combine(Folder, "csc.dll"), 0));
        Assert.Throws<FileNotFoundException>(() => provider.CompileAssemblyFromSource(Parameters(compilerOptions: ""), Class));
    }

    // --- Sources

    [Fact] // the docs: CompileAssemblyFromSource compiles the sources; in memory, CompiledAssembly the assembly
    public void Sources_compile_in_memory()
    {
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(Parameters(inMemory: true, debug: true),
            Class, "namespace N { public class B { public static int Twice() { return A.Answer() * 2; } } }");
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.Empty(results.Errors);
        Assert.Equal(84, results.CompiledAssembly.GetType("N.B").GetMethod("Twice").Invoke(null, null));
        // the reference source: PathToAssembly set when the assembly is not in memory only
        Assert.Null(results.PathToAssembly);
    }

    [Fact]
    // the reference source: the symbols read when there are, Assembly.Load(assembly, null symbols) otherwise
    public void Sources_compile_in_memory_without_symbols()
    {
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(Parameters(inMemory: true), Class);
        Assert.Equal(42, results.CompiledAssembly.GetType("N.A").GetMethod("Answer").Invoke(null, null));
    }

    [Fact] // the docs: not in memory, the assembly a file (PathToAssembly), kept; the temporary files (sources, command line, output) deleted (TempFiles, KeepFiles false)
    public void Sources_compile_to_a_kept_file()
    {
        var parameters = Parameters();
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(parameters, Class);
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.EndsWith(".dll", results.PathToAssembly);
        Assert.Equal(parameters.OutputAssembly, results.PathToAssembly);
        Assert.Equal(new[] { results.PathToAssembly }, Directory.GetFiles(Folder));
        // the reference source: without a warning level, the output of a compilation that succeeded is not read
        Assert.Empty(results.Output);
    }

    [Fact] // the docs: TempFiles.KeepFiles keeps the temporary files (the sources, the command line, the compiler's output)
    public void Temporary_files_are_kept_when_asked()
    {
        var parameters = Parameters();
        parameters.TempFiles.KeepFiles = true;
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(parameters, Class);
        var basePath = parameters.TempFiles.BasePath;
        Assert.True(File.Exists(basePath + ".0.cs"));
        Assert.True(File.Exists(basePath + ".cmdline"));
        Assert.True(File.Exists(basePath + ".out"));
        Assert.Equal(Class, File.ReadAllText(basePath + ".0.cs"));
    }

    [Fact] // the reference source: the given OutputAssembly the file written; a .pdb kept when the options ask /debug:pdbonly
    public void The_output_assembly_and_a_pdbonly_pdb()
    {
        var parameters = Parameters();
        parameters.OutputAssembly = Path.Combine(Folder, "named.dll");
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(parameters, Class);
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.Equal(parameters.OutputAssembly, results.PathToAssembly);
        Assert.True(File.Exists(parameters.OutputAssembly));

        var pdbOnly = Parameters(compilerOptions: "/debug:pdbonly");
        Assert.Equal(0, new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(pdbOnly, Class).NativeCompilerReturnValue);
        Assert.True(File.Exists(pdbOnly.TempFiles.BasePath + ".pdb"), "the pdb kept");
    }

    [Fact] // the reference source: IncludeDebugInformation's .pdb is a temporary file (deleted unless KeepFiles)
    public void A_debug_pdb_is_temporary()
    {
        var parameters = Parameters(debug: true);
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(parameters, Class);
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.False(File.Exists(parameters.TempFiles.BasePath + ".pdb"));
    }

    [Fact] // the reference source: a failed compilation's empty assembly (made before) deleted; PathToAssembly still set; errors read
    public void A_failed_compilation_leaves_no_assembly()
    {
        var parameters = Parameters();
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(parameters, "class A { void M() { y = 1; } }");
        Assert.NotEqual(0, results.NativeCompilerReturnValue);
        Assert.True(results.Errors.HasErrors);
        Assert.Equal(parameters.OutputAssembly, results.PathToAssembly);
        Assert.False(File.Exists(results.PathToAssembly));
    }

    [Fact] // the reference source: a given OutputAssembly is not deleted on failure (the compiler wrote none); in memory, no CompiledAssembly
    public void A_failed_compilation_of_a_named_assembly()
    {
        var parameters = Parameters(inMemory: true);
        parameters.OutputAssembly = Path.Combine(Folder, "named.dll");
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(parameters, "class A { void M() { y = 1; } }");
        Assert.True(results.Errors.HasErrors);
        Assert.Equal(parameters.OutputAssembly, results.PathToAssembly);
    }

    [Fact] // the reference source: the output read, its first line the compiler's command line with the arguments of the response file ("dir>compiler args")
    public void The_output_begins_with_the_command_line()
    {
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(Parameters(warningLevel: 4), Class);
        var line = CommandLine(results);
        Assert.StartsWith(Environment.CurrentDirectory + ">" + CompilationUtil.CSC2.CompilerFullPath + " /t:library ", line);
        Assert.Contains("/w:4", line);
        Assert.DoesNotContain("@\"", line);
        Assert.True(results.Output.Count > 1);
    }

    // --- Files and CodeDOM

    [Fact] // the docs: CompileAssemblyFromFile compiles the files
    public void Files_compile()
    {
        var a = Write("a.cs", Class);
        var b = Write("b.cs", "namespace N { public class B { } }");
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromFile(Parameters(inMemory: true, debug: true), a, b);
        Assert.NotNull(results.CompiledAssembly.GetType("N.A"));
        Assert.NotNull(results.CompiledAssembly.GetType("N.B"));
#pragma warning disable CS0618
        var one = new CSharpCodeProvider(Plain()).CreateCompiler().CompileAssemblyFromFile(Parameters(inMemory: true, debug: true), a);
#pragma warning restore CS0618
        Assert.NotNull(one.CompiledAssembly.GetType("N.A"));
    }

    [Fact] // the docs (ICodeCompiler.CompileAssemblyFromSource): one source
    public void A_source_compiles()
    {
        var results = Compiler(Plain()).CompileAssemblyFromSource(Parameters(inMemory: true, debug: true), Class);
        Assert.Equal(42, results.CompiledAssembly.GetType("N.A").GetMethod("Answer").Invoke(null, null));
    }

    [Fact] // the reference source: no source no file given to csc (CS2008: no source files, a warning)
    public void No_sources()
    {
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(Parameters(warningLevel: 4));
        Assert.Equal(0, results.NativeCompilerReturnValue);
        var warning = Assert.Single(results.Errors.Cast<CompilerError>());
        Assert.Equal("CS2008", warning.ErrorNumber);
        Assert.True(warning.IsWarning);
    }

    [Fact] // the docs: CompileAssemblyFromDom compiles the CodeDOM graph, the provider's code generated from it
    public void A_code_dom_compiles()
    {
        var unit = new CodeCompileUnit();
        var ns = new CodeNamespace("Dom");
        ns.Types.Add(new CodeTypeDeclaration("Made") { IsClass = true });
        unit.Namespaces.Add(ns);
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromDom(Parameters(inMemory: true, debug: true), unit);
        Assert.NotNull(results.CompiledAssembly.GetType("Dom.Made"));
#pragma warning disable CS0618
        var one = new CSharpCodeProvider(Plain()).CreateCompiler().CompileAssemblyFromDom(Parameters(inMemory: true, debug: true), unit);
#pragma warning restore CS0618
        Assert.NotNull(one.CompiledAssembly.GetType("Dom.Made"));
    }

    [Fact] // the docs: GenerateExecutable an .exe (its OutputAssembly made with that extension)
    public void An_executable()
    {
        var parameters = Parameters();
        parameters.GenerateExecutable = true;
        var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(parameters, "class P { static void Main() { } }");
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.EndsWith(".exe", results.PathToAssembly);
    }

    // --- The compiler run

    [Fact] // the port's choice: the compiler server's time to live (/shared /keepalive) not given to the in-process compiler; it compiles either way
    public void A_compiler_server_s_time_to_live()
    {
        var withServer = new ProviderOptions(CompilationUtil.CSC2.CompilerFullPath, 10);
        var results = new CSharpCodeProvider(withServer).CompileAssemblyFromSource(Parameters(warningLevel: 4), Class);
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.DoesNotContain("/keepalive", CommandLine(results));
    }

    [Theory] // DotNetCompilerPlatform: CLR profiling off during the compilation (COR_ENABLE_PROFILING=0), the process's setting put back after
    [InlineData(null)]
    [InlineData("1")]
    [InlineData("0")]
    public void Profiling_is_put_back(string setting)
    {
        WithEnvironment("COR_ENABLE_PROFILING", setting, () =>
        {
            var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(Parameters(), Class);
            Assert.Equal(0, results.NativeCompilerReturnValue);
            Assert.Equal(setting, Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING"));
        });
    }

    [Fact] // DotNetCompilerPlatform: aspnet:DisableProfilingDuringCompilation=false leaves the profiling setting alone
    public void Profiling_left_alone_when_configured()
    {
        WithAppSettings(new NameValueCollection { { "aspnet:DisableProfilingDuringCompilation", "false" } }, () =>
        {
            Assert.False(AppSettings.DisableProfilingDuringCompilation);
            WithEnvironment("COR_ENABLE_PROFILING", "1", () =>
            {
                var results = new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(Parameters(), Class);
                Assert.Equal(0, results.NativeCompilerReturnValue);
                Assert.Equal("1", Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING"));
            });
        });
    }

    [Fact] // the port's choice: no impersonation on .NET; a user token is not supported
    public void A_user_token_is_not_supported()
    {
        var parameters = Parameters();
        parameters.UserToken = new IntPtr(1);
        Assert.Throws<PlatformNotSupportedException>(() => new CSharpCodeProvider(Plain()).CompileAssemblyFromSource(parameters, Class));
    }

    [Fact] // the reference source: a compiler that is not a .dll run as a process (csc.exe), its exit code NativeCompilerReturnValue, its output parsed
    public void A_compiler_executable_runs_as_a_process()
    {
        // .NET Framework's own csc.exe, where there is one (Windows).
        var csc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET", "Framework64", "v4.0.30319", "csc.exe");
        if (!File.Exists(csc)) return;
        var results = new CSharpCodeProvider(new ProviderOptions(csc, 0)).CompileAssemblyFromSource(Parameters(warningLevel: 4),
            "public class A { void M() { int unused; } }");
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.StartsWith(Environment.CurrentDirectory + ">" + csc + " ", CommandLine(results));
        var warning = Assert.Single(results.Errors.Cast<CompilerError>());
        Assert.Equal("CS0168", warning.ErrorNumber);
        Assert.True(warning.IsWarning);
    }

    // --- ParseArguments, MapPath

    [Fact] // the port's choice: a command line split at the spaces outside quotes (the quotes kept)
    public void Arguments_split_outside_quotes()
    {
        var compiler = (WebFormsForCore.CodeDom.Compiler.Compiler)Compiler();
        Assert.Equal(new[] { "/t:library", "/R:\"a b.dll\"", "x.cs" }, compiler.ParseArguments("/t:library /R:\"a b.dll\" x.cs"));
    }
}

/// <summary>Compiler.MapPath: the running application's HostingEnvironment.MapPath (Site.Run: the Application collection).</summary>
[Collection("Application")]
public class CompilerMapPathTests
{
    [Fact] // the port's choice: a virtual path of the application its file
    public void A_virtual_path_mapped()
    {
        using var site = new FrameworkOnCore.Runtime.Tests.Hosting.Site().Run(watch: false);
        var compiler = new WebFormsForCore.CodeDom.Compiler.CSharpCompiler(null, new ProviderOptions());
        Assert.Equal(Path.Combine(site.Root, "App_Code", "a.cs"), compiler.MapPath("/App_Code/a.cs"));
    }
}
