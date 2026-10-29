using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Reflection;
using WebFormsForCore.CodeDom.Compiler;
using CSharpCodeProvider = WebFormsForCore.CodeDom.Compiler.CSharpCodeProvider;
using VBCodeProvider = WebFormsForCore.CodeDom.Compiler.VBCodeProvider;
using CodeDomProvider = WebFormsForCore.CodeDom.Compiler.CodeDomProvider;

namespace FrameworkOnCore.Runtime.Tests.CodeDom;

/// <summary>
/// The providers (CSharpCodeProvider, VBCodeProvider and their bases, which stand for .NET Framework's Microsoft.CSharp.
/// CSharpCodeProvider and Microsoft.VisualBasic.VBCodeProvider): their members as learn.microsoft.com documents them ("the
/// docs"), the code generators and attribute converters of System.CodeDom (referencesource.microsoft.com: CSharpCodeGenerator,
/// CSharpMemberAttributeConverter...), and the constructors of Microsoft.CodeDom.Providers.DotNetCompilerPlatform's providers
/// ("DotNetCompilerPlatform": ICompilerSettings, IProviderOptions, the providerOptions dictionary).
/// </summary>
public class CodeProviderTests : CompilationTest
{
    const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;

    static IProviderOptions Options(object provider) =>
        (IProviderOptions)provider.GetType().GetField("_providerOptions", Instance).GetValue(provider);

    // --- Constructors

    [Fact] // DotNetCompilerPlatform: the default options CompilationUtil's (ASP.NET mode, csc.dll / vbc.dll of the application's bin)
    public void The_default_options()
    {
        Assert.Same(CompilationUtil.CSC2, Options(new CSharpCodeProvider()));
        Assert.Same(CompilationUtil.VBC2, Options(new VBCodeProvider()));
        Assert.Same(CompilationUtil.CSC2, Options(new CSharpCodeProvider((IProviderOptions)null)));
        Assert.Same(CompilationUtil.VBC2, Options(new VBCodeProvider((IProviderOptions)null)));
        Assert.True(CompilationUtil.CSC2.UseAspNetSettings);
        Assert.Equal("csc.dll", Path.GetFileName(CompilationUtil.CSC2.CompilerFullPath));
        Assert.Equal("vbc.dll", Path.GetFileName(CompilationUtil.VBC2.CompilerFullPath));
    }

#pragma warning disable CS0618 // ICompilerSettings
    sealed class Settings : ICompilerSettings
    {
        public string CompilerFullPath => "compiler.dll";
        public int CompilerServerTimeToLive => 7;
    }

    [Fact] // DotNetCompilerPlatform: ICompilerSettings its compiler and time to live (not ASP.NET mode); none the defaults
    public void Compiler_settings()
    {
        foreach (var options in new[] { Options(new CSharpCodeProvider(new Settings())), Options(new VBCodeProvider(new Settings())) })
        {
            Assert.Equal("compiler.dll", options.CompilerFullPath);
            Assert.Equal(7, options.CompilerServerTimeToLive);
            Assert.False(options.UseAspNetSettings);
        }
        Assert.Same(CompilationUtil.CSC2, Options(new CSharpCodeProvider((ICompilerSettings)null)));
        Assert.Same(CompilationUtil.VBC2, Options(new VBCodeProvider((ICompilerSettings)null)));
    }
#pragma warning restore CS0618

    [Fact] // DotNetCompilerPlatform: the providerOptions dictionary over the defaults
    public void A_provider_options_dictionary()
    {
        var options = new Dictionary<string, string> { { "WarnAsError", "true" } };
        Assert.True(Options(new CSharpCodeProvider(options)).WarnAsError);
        Assert.True(Options(new VBCodeProvider(options)).WarnAsError);
        Assert.True(Options(new VBCodeProvider(options)).UseAspNetSettings);
    }

    [Fact] // the docs: CreateCompiler the provider's compiler, with its options
    public void The_compiler()
    {
#pragma warning disable CS0618
        Assert.IsType<CSharpCompiler>(new CSharpCodeProvider().CreateCompiler());
        Assert.IsType<VBCompiler>(new VBCodeProvider().CreateCompiler());
#pragma warning restore CS0618
    }

    // --- The bases

    [Fact] // the docs: Microsoft.CSharp.CSharpCodeProvider(IDictionary) and VBCodeProvider(IDictionary): ArgumentNullException for null
    public void A_null_dictionary_throws()
    {
        Assert.Throws<ArgumentNullException>(() => new CSharpCodeProviderBase(null));
        Assert.Throws<ArgumentNullException>(() => new VBCodeProviderBase(null));
    }

    [Fact] // the docs: the generator's code of the language; CreateGenerator, CreateCompiler the generator
    public void The_bases_generate_code()
    {
        Generates(new CSharpCodeProviderBase(), "public class A");
        Generates(new VBCodeProviderBase(), "Public Class A");
    }

    [Fact]
    // the docs: Microsoft.CSharp.CSharpCodeProvider(IDictionary<string,string>) and VBCodeProvider(IDictionary<string,string>): a provider with the options (CompilerVersion)
    public void The_bases_with_provider_options()
    {
        var options = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
        Generates(new CSharpCodeProviderBase(options), "public class A");
        Generates(new VBCodeProviderBase(options), "Public Class A");
    }

    static void Generates(CodeDomProvider provider, string text)
    {
        var unit = new CodeCompileUnit();
        unit.Namespaces.Add(new CodeNamespace("N") { Types = { new CodeTypeDeclaration("A") } });
        {
            var writer = new StringWriter();
            provider.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions());
            Assert.Contains(text, writer.ToString());
#pragma warning disable CS0618
            Assert.NotNull(provider.CreateGenerator());
            Assert.IsAssignableFrom<ICodeCompiler>(provider.GetType().GetMethod("CreateCompiler").Invoke(provider, null));
#pragma warning restore CS0618
        }
    }

    [Fact] // the docs: FileExtension "cs" / "vb"; VB's LanguageOptions CaseInsensitive
    public void Extensions_and_language_options()
    {
        Assert.Equal("cs", new CSharpCodeProvider().FileExtension);
        Assert.Equal("vb", new VBCodeProvider().FileExtension);
        Assert.Equal(LanguageOptions.CaseInsensitive, new VBCodeProvider().LanguageOptions);
        Assert.Equal(LanguageOptions.None, new CSharpCodeProvider().LanguageOptions);
    }

    [Fact] // the docs: GenerateCodeFromMember the member's code in the language
    public void A_member_s_code()
    {
        var field = new CodeMemberField(typeof(int), "count");
        var cs = new StringWriter();
        new CSharpCodeProvider().GenerateCodeFromMember(field, cs, new CodeGeneratorOptions());
        Assert.Equal("private int count;", cs.ToString().Trim());
        var vb = new StringWriter();
        new VBCodeProvider().GenerateCodeFromMember(field, vb, new CodeGeneratorOptions());
        Assert.Equal("Private count As Integer", vb.ToString().Trim());
    }

    [Fact] // the docs (GetConverter) and the reference source: MemberAttributes and TypeAttributes the language's keywords; another type TypeDescriptor's converter
    public void Converters()
    {
        var cs = new CSharpCodeProvider();
        var vb = new VBCodeProvider();
        Assert.Equal("Internal", cs.GetConverter(typeof(MemberAttributes)).ConvertToString(MemberAttributes.Assembly));
        Assert.Equal("Friend", vb.GetConverter(typeof(MemberAttributes)).ConvertToString(MemberAttributes.Assembly));
        Assert.Equal("Internal", cs.GetConverter(typeof(TypeAttributes)).ConvertToString(TypeAttributes.NotPublic));
        Assert.Equal("Friend", vb.GetConverter(typeof(TypeAttributes)).ConvertToString(TypeAttributes.NotPublic));
        Assert.IsType<Int32Converter>(cs.GetConverter(typeof(int)));
        Assert.IsType<Int32Converter>(vb.GetConverter(typeof(int)));
    }
}
