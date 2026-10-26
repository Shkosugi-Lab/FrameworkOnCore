using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>A package moved to a higher major version than the code was written for: reported, but where majors only grow.</summary>
public class PackageRaisesTests
{
    [Fact] // DNN: the JWT controller was stubbed (its types left System.IdentityModel.Tokens)
    public void A_known_break_is_said() =>
        Assert.Contains("System.IdentityModel.Tokens", PackageRaises.Note("System.IdentityModel.Tokens.Jwt", "4.0.2.206221351", "8.16.0"));

    [Fact]
    public void An_unknown_package_across_a_major_is_reported() =>
        Assert.Contains("major version", PackageRaises.Note("Autofac", "4.9.4", "8.0.0"));

    [Fact] // what changed between the two versions only
    public void Json_net_says_what_changed_between_the_two()
    {
        Assert.DoesNotContain("Date", PackageRaises.Note("Newtonsoft.Json", "9.0.1", "13.0.4"));
        Assert.Contains("MaxDepth", PackageRaises.Note("Newtonsoft.Json", "9.0.1", "13.0.4"));
        Assert.Contains("Date", PackageRaises.Note("Newtonsoft.Json", "4.0.5", "13.0.4"));
    }

    [Theory]
    [InlineData("Microsoft.Extensions.DependencyInjection", "2.1.1", "10.0.0")] // the API only grows
    [InlineData("Autofac", "8.0.0", "8.1.0")]                                     // not a major
    [InlineData("Newtonsoft.Json", "13.0.1", "13.0.4")]
    public void Others_are_not(string id, string from, string to) => Assert.Null(PackageRaises.Note(id, from, to));
}
