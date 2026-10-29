using System.CodeDom.Compiler;
using WebFormsForCore.CodeDom.Compiler;
using VBCodeProvider = WebFormsForCore.CodeDom.Compiler.VBCodeProvider;

namespace FrameworkOnCore.Runtime.Tests.CodeDom;

/// <summary>
/// The VB compiler (VBCompiler): CompilerParameters made vbc's arguments as .NET Framework's VBCodeGenerator makes them
/// (CmdArgsFromParameters, referencesource.microsoft.com: System/compmod/microsoft/visualbasic/VBCodeProvider.cs, "the reference
/// source" below; CodeDomProvider.TryGetProbableCoreAssemblyFilePath), vbc's output lines made CompilerErrors ("file(line) :
/// error BCxxxx: text"), and what Microsoft.CodeDom.Providers.DotNetCompilerPlatform's VBCompiler adds in ASP.NET mode
/// ("DotNetCompilerPlatform"). Where .NET's VB runtime differs (Microsoft.VisualBasic.Core), the code's comments.
/// </summary>
public class VBCompilerTests : CompilationTest
{
    const string Class = "Namespace N\nPublic Class A\nPublic Shared Function Answer() As Integer\nReturn 42\nEnd Function\nEnd Class\nEnd Namespace";
    const string Unused = "Public Class A\nSub M()\nDim unused As Integer\nEnd Sub\nEnd Class";

    CompilerResults Compile(string source, CompilerParameters parameters, IProviderOptions options = null) =>
        new VBCodeProvider(options ?? Plain(".vb")).CompileAssemblyFromSource(parameters, source);

    static string Runtime(string file) => Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), file);

    [Fact] // the docs (CompileAssemblyFromSource): a class compiles; in memory, CompiledAssembly the assembly
    public void A_class_compiles()
    {
        var results = Compile(Class, Parameters(compilerOptions: NoMy, inMemory: true, debug: true));
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.Equal(42, results.CompiledAssembly.GetType("N.A").GetMethod("Answer").Invoke(null, null));
    }

    [Fact]
    // .NET Framework's VBCodeProvider (and DotNetCompilerPlatform's): a class compiles with CompilerParameters' defaults
    public void A_class_compiles_with_the_default_parameters()
    {
        var results = new VBCodeProvider().CompileAssemblyFromSource(Parameters(), Class);
        Assert.Empty(results.Errors);
    }

    [Fact] // the port: in ASP.NET mode _MYTYPE "Empty" (.NET has no My.* web types; DotNetCompilerPlatform's "Web"), not again when the options define it (ASP.NET's AssemblyBuilder does, for this provider)
    public void Asp_net_settings_define_my_type_once()
    {
        var results = new VBCodeProvider().CompileAssemblyFromSource(Parameters(warningLevel: 4, references: "missing-library.dll"), Class);
        Assert.Contains(" /define:_MYTYPE=\\\"Empty\\\" ", CommandLine(results));
        results = new VBCodeProvider().CompileAssemblyFromSource(Parameters(warningLevel: 4, compilerOptions: "/define:_MYTYPE=\\\"Custom\\\"", references: "missing-library.dll"), Class);
        Assert.DoesNotContain("Empty", CommandLine(results));
    }

    // --- Errors

    [Fact] // the reference source: "file(line) : error BCxxxx: text" a CompilerError with its file, line, number and text; warnings IsWarning
    public void Errors_and_warnings_with_their_place()
    {
        var results = Compile("Public Class A\nSub M()\ny = 1\nDim unused As Integer\nEnd Sub\nEnd Class", Parameters(warningLevel: 4, compilerOptions: NoMy));
        Assert.NotEqual(0, results.NativeCompilerReturnValue);
        var errors = results.Errors.Cast<CompilerError>().ToList();
        Assert.Equal(2, errors.Count);
        Assert.EndsWith(".0.vb", errors[0].FileName);
        Assert.Equal(3, errors[0].Line);
        Assert.Equal("BC30451", errors[0].ErrorNumber);
        Assert.False(errors[0].IsWarning);
        Assert.Contains("'y'", errors[0].ErrorText);
        Assert.Equal(4, errors[1].Line);
        Assert.Equal("BC42024", errors[1].ErrorNumber);
        Assert.True(errors[1].IsWarning);
    }

    [Fact] // the reference source: "vbc : error BCxxxx: text" (no line) a CompilerError of line 0
    public void An_error_without_a_line()
    {
        var results = Compile(Class, Parameters(compilerOptions: NoMy, references: "missing-library.dll"));
        var error = Assert.Single(results.Errors.Cast<CompilerError>());
        Assert.Equal(0, error.Line);
        Assert.Equal("BC2017", error.ErrorNumber);
        Assert.Contains("missing-library.dll", error.ErrorText);
    }

    // --- ASP.NET settings, WarnAsError

    [Fact] // DotNetCompilerPlatform: in ASP.NET mode /optionInfer+ (a declaration's type inferred under Option Strict)
    public void Asp_net_settings_infer_types()
    {
        const string inferred = "Option Strict On\nPublic Class A\nPublic Function M() As Integer\nDim x = 1\nReturn x\nEnd Function\nEnd Class";
        var results = new VBCodeProvider().CompileAssemblyFromSource(Parameters(compilerOptions: NoMy), inferred);
        Assert.Empty(results.Errors);
        var plain = Compile(inferred, Parameters(compilerOptions: NoMy));
        Assert.Contains(plain.Errors.Cast<CompilerError>(), e => e.ErrorNumber == "BC30209");

        var noOptions = new VBCodeProvider().CompileAssemblyFromSource(Parameters(warningLevel: 4, references: "missing-library.dll"), Class);
        Assert.Contains(" /optionInfer+ ", CommandLine(noOptions));
    }

    [Fact]
    // DotNetCompilerPlatform (VBCompiler.FixUpCompilerParameters) and .NET Framework's AssemblyBuilder: ASP.NET's VB /nowarn of the obsolete warnings
    public void Asp_net_settings_hide_obsolete_warnings()
    {
        const string obsolete = "Public Class A\n<System.Obsolete> Sub Old()\nEnd Sub\n<System.Obsolete(\"no\")> Sub Older()\nEnd Sub\nSub M()\nOld()\nOlder()\nEnd Sub\nEnd Class";
        var results = new VBCodeProvider().CompileAssemblyFromSource(Parameters(warningLevel: 4, compilerOptions: NoMy), obsolete);
        Assert.Empty(results.Errors);
    }

    [Fact] // DotNetCompilerPlatform: TreatWarningsAsErrors the provider's WarnAsError; the reference source: /warnaserror+ when set (nothing otherwise)
    public void Warn_as_error()
    {
        var parameters = Parameters(warningLevel: 4, compilerOptions: NoMy);
        parameters.TreatWarningsAsErrors = true;
        var results = Compile(Unused, parameters);
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.DoesNotContain("/warnaserror", CommandLine(results));

        var strict = Compile(Unused, Parameters(warningLevel: 4, compilerOptions: NoMy), Plain(".vb", warnAsError: true));
        Assert.NotEqual(0, strict.NativeCompilerReturnValue);
        Assert.Contains(" /warnaserror+ ", CommandLine(strict));
    }

    // --- Arguments

    [Fact] // the reference source: /t:library /utf8output, /out:, /debug-; CompilerOptions last; IncludeDebugInformation /D:DEBUG=1 /debug+
    public void A_library_s_arguments()
    {
        var parameters = Parameters(warningLevel: 4, compilerOptions: NoMy);
        var line = CommandLine(Compile(Class, parameters));
        Assert.Contains(" /t:library /utf8output ", line);
        Assert.Contains(" /out:\"" + parameters.OutputAssembly + "\" /debug- " + NoMy + " ", line);

        const string debug = "Public Class A\nPublic Shared Function Debug() As Boolean\n#If DEBUG Then\nReturn True\n#Else\nReturn False\n#End If\nEnd Function\nEnd Class";
        var results = Compile(debug, Parameters(warningLevel: 4, compilerOptions: NoMy, inMemory: true, debug: true));
        Assert.Contains(" /D:DEBUG=1 /debug+ ", CommandLine(results));
        Assert.Equal(true, results.CompiledAssembly.GetType("A").GetMethod("Debug").Invoke(null, null));
    }

    [Fact] // the reference source: an executable /t:exe, MainClass /main:
    public void An_executable_s_main_class()
    {
        var parameters = Parameters(warningLevel: 4, compilerOptions: NoMy);
        parameters.GenerateExecutable = true;
        parameters.MainClass = "Q";
        var results = Compile("Module P\nSub Main()\nEnd Sub\nEnd Module\nModule Q\nSub Main()\nEnd Sub\nEnd Module", parameters);
        Assert.Equal(0, results.NativeCompilerReturnValue);
        Assert.Contains(" /t:exe /main:Q /utf8output ", CommandLine(results));
        Assert.EndsWith(".exe", results.PathToAssembly);

        foreach (var mainClass in new[] { "", null })
        {
            var noMainClass = Parameters(warningLevel: 4, compilerOptions: NoMy);
            noMainClass.GenerateExecutable = true;
            noMainClass.MainClass = mainClass;
            var single = Compile("Module P\nSub Main()\nEnd Sub\nEnd Module", noMainClass);
            Assert.Equal(0, single.NativeCompilerReturnValue);
            Assert.DoesNotContain("/main:", CommandLine(single));
        }
    }

    [Fact] // the port's choice (its comments): /nostdlib, the runtime's folder the SDK path and its core library referenced; CoreAssemblyFileName when given
    public void The_core_assembly()
    {
        var core = typeof(object).Assembly.Location;
        var line = CommandLine(Compile(Class, Parameters(warningLevel: 4, compilerOptions: NoMy)));
        Assert.Contains(" /nostdlib /sdkpath:\"" + Path.GetDirectoryName(core) + "\" /R:\"" + core + "\" ", line);

        var parameters = Parameters(compilerOptions: NoMy);
        parameters.CoreAssemblyFileName = Path.Combine(Folder, "core.dll");
        Assert.Contains(" /sdkpath:\"" + Folder + "\" /R:\"" + parameters.CoreAssemblyFileName + "\" ", CommandLine(Compile(Class, parameters)));
    }

    [Fact] // the reference source: references of a .NET Framework targeting pack: its mscorlib the core library
    public void A_targeting_pack_s_core_assembly()
    {
        var pack = Path.Combine(Folder, "Reference Assemblies", "Microsoft", "Framework", ".NETFramework", "v4.8");
        var parameters = Parameters(compilerOptions: NoMy, references: Path.Combine(pack, "System.Xml.dll"));
        var results = Compile(Class, parameters);
        Assert.NotEqual(0, results.NativeCompilerReturnValue);
        Assert.Contains(" /nostdlib /sdkpath:\"" + pack + "\" /R:\"" + Path.Combine(pack, "mscorlib.dll") + "\" ", CommandLine(results));
    }

    [Fact] // the port's choice (its comments): System.Runtime, System and Microsoft.VisualBasic referenced; the VB runtime Microsoft.VisualBasic.Core (/vbruntime:), as the .NET SDK gives it
    public void The_runtime_s_references()
    {
        var parameters = Parameters(warningLevel: 4, compilerOptions: NoMy);
        var line = CommandLine(Compile(Class, parameters));
        Assert.Contains("/vbruntime:\"" + Runtime("Microsoft.VisualBasic.Core.dll") + "\" /R:\"" + Runtime("Microsoft.VisualBasic.Core.dll") + "\" ", line);
        foreach (var file in new[] { "System.Runtime.dll", "System.dll", "Microsoft.VisualBasic.dll", "netstandard.dll" })
            Assert.Single(System.Text.RegularExpressions.Regex.Matches(line, System.Text.RegularExpressions.Regex.Escape("/R:\"" + Runtime(file) + "\"")));
        // the references the port added are the parameters' (the reference source does not add them)
        Assert.Contains(Runtime("System.Runtime.dll"), parameters.ReferencedAssemblies.Cast<string>());
    }

    [Fact] // the same: references the parameters have already not added twice; Microsoft.VisualBasic.Core referenced by them not again
    public void References_given_are_not_repeated()
    {
        var given = new[] { "System.Runtime.dll", "System.dll", "Microsoft.VisualBasic.dll", "Microsoft.VisualBasic.Core.dll", "netstandard.dll" };
        var parameters = Parameters(warningLevel: 4, compilerOptions: NoMy, references: given.Select(Runtime).ToArray());
        var results = Compile(Class, parameters);
        Assert.Equal(0, results.NativeCompilerReturnValue);
        foreach (var file in given)
            Assert.Single(System.Text.RegularExpressions.Regex.Matches(CommandLine(results), System.Text.RegularExpressions.Regex.Escape("/R:\"" + Runtime(file) + "\"")));
        Assert.Equal(given.Length, parameters.ReferencedAssemblies.Count);
    }

    [Fact] // the port's choice: a Microsoft.VisualBasic.dll without Microsoft.VisualBasic.Core beside it the VB runtime itself
    public void A_vb_runtime_without_core()
    {
        var elsewhere = Path.Combine(Folder, "Microsoft.VisualBasic.dll");
        var results = Compile(Class, Parameters(compilerOptions: NoMy, references: elsewhere));
        Assert.Contains("/vbruntime:\"" + elsewhere + "\" ", CommandLine(results));
    }

    [Fact] // the reference source: mscorlib.dll among the references not given to vbc (ASURT 81568)
    public void Mscorlib_is_not_given()
    {
        var mscorlib = Path.Combine(Folder, "mscorlib.dll");
        var results = Compile(Class, Parameters(warningLevel: 4, compilerOptions: NoMy, references: mscorlib));
        Assert.DoesNotContain("mscorlib.dll\"", CommandLine(results));
    }

    [Fact] // the reference source: Win32Resource /win32resource:, EmbeddedResources /res:, LinkedResources /linkres:
    public void Resources()
    {
        var embedded = Write("embedded.txt", "in");
        var linked = Write("linked.txt", "out");
        var parameters = Parameters(compilerOptions: NoMy);
        parameters.Win32Resource = Path.Combine(Folder, "missing.res");
        parameters.EmbeddedResources.Add(embedded);
        parameters.LinkedResources.Add(linked);
        var results = Compile(Class, parameters);
        Assert.NotEqual(0, results.NativeCompilerReturnValue);
        Assert.Contains("/win32resource:\"" + parameters.Win32Resource + "\" /res:\"" + embedded + "\" /linkres:\"" + linked + "\" ", CommandLine(results));
    }

    // --- TryGetProbableCoreAssemblyFilePath

    static string Pack(string version, string file) =>
        Path.Combine("C:" + Path.DirectorySeparatorChar, "Program Files", "Reference Assemblies", "Microsoft", "Framework", ".NETFramework", version, file);

    [Theory] // the reference source (CodeDomProvider.TryGetProbableCoreAssemblyFilePath): one targeting pack's folder its mscorlib; none, two packs or a given mscorlib nothing
    [InlineData(true, "v4.8", "v4.8")]
    [InlineData(false, "v4.8", "v4.5")]
    [InlineData(false, "x4.8", "x4.8")]
    public void The_probable_core_assembly(bool found, string first, string second)
    {
        var parameters = new CompilerParameters(new[] { Pack(first, "System.dll"), Pack(second, "System.Xml.dll") });
        Assert.Equal(found, VBCompiler.TryGetProbableCoreAssemblyFilePath(parameters, out var core));
        Assert.Equal(found ? Pack(first, "mscorlib.dll") : "", core);
    }

    [Fact] // the same: a given mscorlib, references of no targeting pack, or a path too short for one: nothing
    public void No_probable_core_assembly()
    {
        foreach (var references in new[]
        {
            new[] { "mscorlib.dll", Pack("v4.8", "System.dll") },
            new[] { "System.dll" },
            new[] { Path.Combine("Reference Assemblies", "Microsoft", "Framework", "System.dll") },
        })
        {
            Assert.False(VBCompiler.TryGetProbableCoreAssemblyFilePath(new CompilerParameters(references), out var core));
            Assert.Equal("", core);
        }
    }
}
