using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace WebForm2Blazor.ConvertedAppTests;

/// <summary>
/// Runs the CONVERTED DefaultsProbe.ProbeTypeFactory - the shape of Castle DynamicProxy's
/// ModuleScope that n2cms vendors - on .NET. The converter rewrote its AppDomain-based
/// Reflection.Emit calls onto AssemblyBuilder; compiling proves the calls exist, and this
/// proves they still generate a working type at run time.
/// </summary>
public class RemovedEmitApiTests
{
    private static string Greet(object greeter)
        => (string)greeter.GetType().GetMethod("Greet")!.Invoke(greeter, null)!;

    [Fact]
    public void 書き換えた動的アセンブリ生成で実行時に型を作れる()
    {
        var greeter = new DefaultsProbe.ProbeTypeFactory(savePhysicalAssembly: false).CreateGreeter();

        Assert.Equal("こんにちは", Greet(greeter));
    }

    [Fact]
    public void 保存指定の経路でもメモリ上の型として作れる()
    {
        // RunAndSave became Run: the same builder, minus the copy on disk.
        var greeter = new DefaultsProbe.ProbeTypeFactory(savePhysicalAssembly: true).CreateGreeter();

        Assert.Equal("こんにちは", Greet(greeter));
    }

    [Fact]
    public void 保存はPlatformNotSupportedExceptionになる()
    {
        var factory = new DefaultsProbe.ProbeTypeFactory(savePhysicalAssembly: true);
        var module = factory.CreateModule();

        Assert.Throws<PlatformNotSupportedException>(() => factory.Save(module));
    }

    [Fact]
    public void IPermissionへの拡張メソッドで権限を問い合わせられる()
    {
        // Compiles only because the compat SecurityPermission is an IPermission; on .NET the
        // question has no enforcement behind it, so what matters is that it answers.
        var exception = Record.Exception(() => new DefaultsProbe.ProbeTypeFactory(false).CanControlPolicy());

        Assert.Null(exception);
    }
}
