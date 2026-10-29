using System.CodeDom.Compiler;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using WebFormsForCore.CodeDom.Compiler;
using CSharpCodeProvider = WebFormsForCore.CodeDom.Compiler.CSharpCodeProvider;

namespace FrameworkOnCore.Runtime.Tests.CodeDom;

/// <summary>
/// The C# compiler (CSharpCompiler): CompilerParameters made csc's arguments as .NET Framework's CSharpCodeGenerator makes them
/// (CmdArgsFromParameters, referencesource.microsoft.com: System/compmod/microsoft/csharp/csharpcodeprovider.cs, "the
/// reference source" below), csc's output lines made CompilerErrors ("file(line,col): error CSxxxx: text"), and what
/// Microsoft.CodeDom.Providers.DotNetCompilerPlatform's CSharpCompiler adds ("DotNetCompilerPlatform": its ASP.NET settings,
/// WarnAsError); the docs: learn.microsoft.com's CompilerParameters and CompilerError.
/// </summary>
public class CSharpCompilerTests : CompilationTest
{
    const string Obsolete = "public class A { [System.Obsolete] void Old() { } [System.Obsolete(\"no\")] void Older() { } void M() { Old(); Older(); } }";
    const string Unused = "public class A { void M() { int unused; } }";

    CompilerResults Compile(string source, CompilerParameters parameters, IProviderOptions options = null) =>
        new CSharpCodeProvider(options ?? Plain()).CompileAssemblyFromSource(parameters, source);

    // --- Errors

    [Fact] // the reference source: "file(line,col): error CSxxxx: text" a CompilerError with its file, line, column, number and text
    public void An_error_with_its_place()
    {
        var results = Compile("public class A {\n  void M() { y = 1; }\n}", Parameters());
        var error = Assert.Single(results.Errors.Cast<CompilerError>());
        Assert.EndsWith(".0.cs", error.FileName);
        Assert.Equal(2, error.Line);
        Assert.Equal(14, error.Column);
        Assert.Equal("CS0103", error.ErrorNumber);
        Assert.False(error.IsWarning);
        Assert.Contains("'y'", error.ErrorText);
    }

    [Fact] // the reference source: "error CSxxxx: text" (no place) a CompilerError without file and line
    public void An_error_without_a_place()
    {
        var results = Compile("public class A { }", Parameters(references: "missing-library.dll"));
        var error = Assert.Single(results.Errors.Cast<CompilerError>());
        Assert.Equal("", error.FileName);
        Assert.Equal(0, error.Line);
        Assert.Equal("CS0006", error.ErrorNumber);
        Assert.Contains("missing-library.dll", error.ErrorText);
    }

    [Fact] // the reference source: warnings (read when the warning level is set) IsWarning; the compilation succeeds
    public void A_warning()
    {
        var results = Compile(Unused, Parameters(warningLevel: 4));
        Assert.Equal(0, results.NativeCompilerReturnValue);
        var warning = Assert.Single(results.Errors.Cast<CompilerError>());
        Assert.True(warning.IsWarning);
        Assert.Equal("CS0168", warning.ErrorNumber);
        Assert.False(results.Errors.HasErrors);
    }

    [Fact] // the reference source: WarningLevel /w:n (0: no warning)
    public void The_warning_level()
    {
        var results = Compile(Unused + " class B { void M() { y = 1; } }", Parameters(warningLevel: 0));
        Assert.Equal("CS0103", Assert.Single(results.Errors.Cast<CompilerError>()).ErrorNumber);
        Assert.Contains(" /w:0 ", CommandLine(results));
    }

    // --- WarnAsError, TreatWarningsAsErrors

    [Fact] // DotNetCompilerPlatform: TreatWarningsAsErrors is the provider's WarnAsError (default false), whatever the parameters say
    public void Treat_warnings_as_errors_is_the_provider_s()
    {
        var parameters = Parameters(warningLevel: 4);
        parameters.TreatWarningsAsErrors = true;
        var results = Compile(Unused, parameters);
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.False(parameters.TreatWarningsAsErrors);
        Assert.Contains("/warnaserror- ", CommandLine(results));

        var strict = Compile(Unused, Parameters(warningLevel: 4), Plain(warnAsError: true));
        Assert.NotEqual(0, strict.NativeCompilerReturnValue);
        Assert.False(Assert.Single(strict.Errors.Cast<CompilerError>()).IsWarning);
        Assert.Contains("/warnaserror+ ", CommandLine(strict));
    }

    // --- ASP.NET settings

    [Fact] // DotNetCompilerPlatform: in ASP.NET mode (UseAspNetSettings, the default) /nowarn:1659;1699;1701;612;618 prepended: no obsolete warning
    public void Asp_net_settings_hide_obsolete_warnings()
    {
        var results = new CSharpCodeProvider().CompileAssemblyFromSource(Parameters(warningLevel: 4, compilerOptions: "/define:X"), Obsolete);
        Assert.Empty(results.Errors);
        Assert.Contains(" /nowarn:1659;1699;1701;612;618 /define:X ", CommandLine(results));
    }

    [Fact(Skip = "bug: in ASP.NET mode, CompilerOptions null (CompilerParameters' default) throws NullReferenceException: options.CompilerOptions.Contains (CSharpCompiler.cs:100)")]
    // DotNetCompilerPlatform (CSharpCompiler.FixUpCompilerParameters): the /nowarn prepended to a null CompilerOptions (PrependCompilerOption)
    public void Asp_net_settings_with_no_compiler_options()
    {
        var results = new CSharpCodeProvider().CompileAssemblyFromSource(Parameters(warningLevel: 4), Obsolete);
        Assert.Empty(results.Errors);
    }

    [Fact(Skip = "bug: in ASP.NET mode, CompilerOptions with a /nowarn: of their own get no ASP.NET /nowarn (CSharpCompiler.cs:100): the obsolete warnings reported")]
    // DotNetCompilerPlatform (CSharpCompiler.FixUpCompilerParameters) and .NET Framework's AssemblyBuilder: ASP.NET's /nowarn always prepended (csc adds up its /nowarn options)
    public void Asp_net_settings_with_a_nowarn_of_the_options()
    {
        var results = new CSharpCodeProvider().CompileAssemblyFromSource(Parameters(warningLevel: 4, compilerOptions: "/nowarn:168"), Obsolete + " class B { void M() { int unused; } }");
        Assert.Empty(results.Errors);
    }

    [Fact] // DotNetCompilerPlatform: without ASP.NET mode, the obsolete warnings reported
    public void Without_asp_net_settings_obsolete_warnings()
    {
        var results = Compile(Obsolete, Parameters(warningLevel: 4));
        Assert.Equal(new[] { "CS0612", "CS0618" }, results.Errors.Cast<CompilerError>().Select(e => e.ErrorNumber).OrderBy(n => n));
        Assert.DoesNotContain("/nowarn", CommandLine(results));
    }

    // --- Arguments

    [Fact] // the reference source: /t:library, /utf8output, /out:, /debug- /optimize+ without debug information; CompilerOptions last
    public void A_library_s_arguments()
    {
        var parameters = Parameters(warningLevel: 4, compilerOptions: "/define:LAST");
        var line = CommandLine(Compile("public class A { }", parameters));
        Assert.Contains(" /t:library /utf8output ", line);
        Assert.Contains(" /out:\"" + parameters.OutputAssembly + "\" /debug- /optimize+ ", line);
        Assert.Contains("/w:4 /define:LAST ", line);
    }

    [Fact] // the reference source: IncludeDebugInformation /D:DEBUG /debug+ /optimize-
    public void Debug_information_s_arguments()
    {
        var results = Compile("public class A { public static bool Debug() {\n#if DEBUG\nreturn true;\n#else\nreturn false;\n#endif\n} }",
            Parameters(warningLevel: 4, inMemory: true, debug: true));
        Assert.Contains(" /D:DEBUG /debug+ /optimize- ", CommandLine(results));
        Assert.Equal(true, results.CompiledAssembly.GetType("A").GetMethod("Debug").Invoke(null, null));
    }

    [Fact] // the reference source: an executable /t:exe, MainClass /main:
    public void An_executable_s_main_class()
    {
        const string twoMains = "class P { static void Main() { } } class Q { static void Main() { } }";
        var parameters = Parameters(warningLevel: 4);
        parameters.GenerateExecutable = true;
        parameters.MainClass = "Q";
        var results = Compile(twoMains, parameters);
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.Contains(" /t:exe /main:Q /utf8output ", CommandLine(results));

        var noMain = Parameters();
        noMain.GenerateExecutable = true;
        Assert.Contains(Compile(twoMains, noMain).Errors.Cast<CompilerError>(), e => e.ErrorNumber == "CS0017");
    }

    [Fact] // the docs: CoreAssemblyFileName the core library (/nostdlib+ /R:); the port's choice: the runtime's System.Private.CoreLib otherwise
    public void The_core_assembly()
    {
        var core = typeof(object).Assembly.Location;
        Assert.Contains(" /nostdlib+ /R:\"" + core + "\" ", CommandLine(Compile("public class A { }", Parameters(warningLevel: 4))));

        var parameters = Parameters();
        parameters.CoreAssemblyFileName = Path.Combine(Folder, "core.dll");
        var results = Compile("public class A { }", parameters);
        Assert.Contains(results.Errors.Cast<CompilerError>(), e => e.ErrorNumber == "CS0006" && e.ErrorText.Contains("core.dll"));
    }

    [Fact] // the port's choice (its comments): System.Runtime and the runtime's facades (mscorlib, netstandard...) referenced; one the parameters reference not twice
    public void The_runtime_s_facades()
    {
        var runtime = Path.GetDirectoryName(typeof(object).Assembly.Location);
        var mscorlib = Path.Combine(runtime, "mscorlib.dll");
        var line = CommandLine(Compile("public class A { }", Parameters(warningLevel: 4)));
        Assert.Contains("/R:\"" + Path.Combine(runtime, "System.Runtime.dll") + "\" ", line);
        Assert.Contains("/R:\"" + mscorlib + "\" ", line);
        Assert.Contains("/R:\"" + Path.Combine(runtime, "netstandard.dll") + "\" ", line);

        var given = CommandLine(Compile("public class A { }", Parameters(warningLevel: 4, references: mscorlib)));
        Assert.Single(System.Text.RegularExpressions.Regex.Matches(given, System.Text.RegularExpressions.Regex.Escape("/R:\"" + mscorlib + "\"")));
    }

    [Fact] // the docs: EmbeddedResources embedded (/res:), LinkedResources linked (/linkres:)
    public void Resources()
    {
        var embedded = Write("embedded.txt", "in");
        var linked = Write("linked.txt", "out");
        var parameters = Parameters(warningLevel: 4);
        parameters.EmbeddedResources.Add(embedded);
        parameters.LinkedResources.Add(linked);
        var results = Compile("public class A { }", parameters);
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.Contains("/res:\"" + embedded + "\" /linkres:\"" + linked + "\" ", CommandLine(results));
        using var pe = new PEReader(File.OpenRead(results.PathToAssembly));
        var reader = pe.GetMetadataReader();
        var resources = reader.ManifestResources.Select(reader.GetManifestResource).ToDictionary(r => reader.GetString(r.Name), r => r.Implementation.IsNil);
        Assert.True(resources["embedded.txt"], "embedded");
        Assert.False(resources["linked.txt"], "linked: in a file of its own");
    }

    [Fact] // the reference source: Win32Resource /win32res: (a missing file an error)
    public void A_win32_resource()
    {
        var parameters = Parameters();
        parameters.Win32Resource = Path.Combine(Folder, "missing.res");
        var results = Compile("public class A { }", parameters);
        Assert.NotEqual(0, results.NativeCompilerReturnValue);
        Assert.Contains("/win32res:\"" + parameters.Win32Resource + "\" ", CommandLine(results));
    }
}
