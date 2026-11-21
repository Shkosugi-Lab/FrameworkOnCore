using FrameworkOnCore.Studio;
using Microsoft.AspNetCore.Http;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Studio answers this machine's browser only (LocalOnly): a Host that is not this machine's name for Studio (a page of
/// another site whose name DNS points at 127.0.0.1) and an Origin that is not Studio's page are refused.
/// </summary>
public sealed class StudioLocalOnlyTests
{
    static HttpRequest Request(string host, string? origin)
    {
        var context = new DefaultHttpContext();
        context.Request.Host = HostString.FromUriComponent(host);
        if (origin != null) context.Request.Headers.Origin = origin;
        return context.Request;
    }

    [Theory]
    [InlineData("127.0.0.1:5300", null, true)]
    [InlineData("localhost:5300", null, true)]
    [InlineData("LOCALHOST:5300", null, true)]
    [InlineData("[::1]:5300", null, true)]
    [InlineData("127.0.0.1:5300", "http://127.0.0.1:5300", true)]
    [InlineData("127.0.0.1:5300", "http://localhost:5300", true)]
    [InlineData("evil.example:5300", null, false)]              // DNS rebinding: the page's own name
    [InlineData("127.0.0.1", null, false)]                      // another port's
    [InlineData("127.0.0.1:5301", null, false)]
    [InlineData("127.0.0.1:5300", "http://evil.example", false)] // another site's page (a form, a fetch)
    [InlineData("127.0.0.1:5300", "http://127.0.0.1:8080", false)]
    [InlineData("127.0.0.1:5300", "https://127.0.0.1:5300", false)]
    [InlineData("127.0.0.1:5300", "null", false)]               // a sandboxed page, a file
    public void Only_this_machine_s_browser_on_studio_s_page(string host, string? origin, bool allowed) =>
        Assert.Equal(allowed, LocalOnly.Allowed(Request(host, origin), 5300));
}
