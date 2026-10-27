using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Why a stub was made: what .NET has not (expected), or the conversion (unresolved): each from a corpus's report.
/// </summary>
public class StubCausesTests
{
    static readonly Lazy<FrameworkIndex?> index = new(FrameworkIndex.Load);
    static readonly StubCauses causes = new(() => index.Value);

    [Theory]
    // mojoPortal's CombinePath: a rewrite made the argument another type
    [InlineData("CS1503: Argument 1: cannot convert from 'string[]' to 'string'")]
    // mojoPortal's MediaPlayer: a namespace the original project did not see
    [InlineData("CS0118: 'MediaPlayer' is a namespace but is used like a type")]
    // DNN's language settings: a replaced member of an anonymous type
    [InlineData("CS0746: Invalid anonymous type member declarator. Anonymous type members must be declared with a member assignment, simple name or member access.")]
    // YAF.Types without its Newtonsoft package
    [InlineData("CS0246: The type or namespace name 'Newtonsoft' could not be found (are you missing a using directive or an assembly reference?)")]
    // DNN's JWT on a package version without the type
    [InlineData("CS0246: The type or namespace name 'JwtSecurityToken' could not be found (are you missing a using directive or an assembly reference?)")]
    // mojoPortal: a namespace of the application's that a reference brought
    [InlineData("CS0234: The type or namespace name 'AudioPlayerFeatureGuid' does not exist in the namespace 'mojoPortal.MediaPlayer' (are you missing an assembly reference?)")]
    public void The_conversion_s_are_told(string reason) => Assert.NotNull(causes.FromConversion(reason));

    [Theory]
    // .NET has not WCF Data Services, WCF's ServiceHost, ICertificatePolicy, IDataContractSurrogate
    [InlineData("CS0234: The type or namespace name 'Services' does not exist in the namespace 'System.Data' (are you missing an assembly reference?)")]
    [InlineData("CS0246: The type or namespace name 'ServiceHost' could not be found (are you missing a using directive or an assembly reference?)")]
    [InlineData("CS0246: The type or namespace name 'DataServiceQuery<>' could not be found (are you missing a using directive or an assembly reference?)")]
    [InlineData("CS0246: The type or namespace name 'IDataContractSurrogate' could not be found (are you missing a using directive or an assembly reference?)")]
    // .NET's types without .NET Framework's members (Remoting, AssemblyBuilder.Save)
    [InlineData("CS0117: 'RemotingServices' does not contain a definition for 'Marshal'")]
    [InlineData("CS1061: 'AssemblyBuilder' does not contain a definition for 'Save' and no accessible extension method 'Save' accepting a first argument of type 'AssemblyBuilder' could be found")]
    // what follows a base removed (the generated client of WCF Data Services)
    [InlineData("CS1729: 'object' does not contain a constructor that takes 1 arguments")]
    [InlineData("CS0117: 'object' does not contain a definition for 'AddObject'")]
    // a DLL built for .NET Framework's System.IdentityModel
    [InlineData("CS0012: The type 'SecurityToken' is defined in an assembly that is not referenced. You must add a reference to assembly 'System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'.")]
    // a type .NET added that the application defines too (YAF.Lucene.Net's attributes)
    [InlineData("CS0433: The type 'MaybeNullWhenAttribute' exists in both 'YAF.Lucene.Net, Version=4.0.0.0, Culture=neutral, PublicKeyToken=85089178b9ac3181' and 'System.Runtime, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'")]
    public void What_dotnet_has_not_is_expected(string reason) => Assert.Null(causes.FromConversion(reason));
}
