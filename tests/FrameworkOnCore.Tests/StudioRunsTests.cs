using System.Net;
using System.Net.Http.Headers;
using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Studio's wait for a site it started (native and container runs): the 503 with Retry-After FrameworkOnCore's runtime
/// answers while the application restarts (DNN's install wizard writes web.config as it starts) is asked again, not taken
/// for the site's first answer ("サイトは動いていますが、エラー(500 番台)を返しています").
/// </summary>
public sealed class StudioRunsTests
{
    static HttpResponseMessage Answer(HttpStatusCode status, int? retryAfter)
    {
        var response = new HttpResponseMessage(status);
        if (retryAfter != null) response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(retryAfter.Value));
        return response;
    }

    [Fact]
    public void The_restarts_503_is_asked_again()
    {
        DateTime? since = null;
        Assert.Equal(TimeSpan.FromSeconds(1), Runs.RestartingAnswer(Answer(HttpStatusCode.ServiceUnavailable, 1), ref since));
        Assert.NotNull(since);
    }

    [Theory] // a site's answer, whatever it is: a 503 of its own (no Retry-After), a 500, a 200
    [InlineData(HttpStatusCode.ServiceUnavailable, null)]
    [InlineData(HttpStatusCode.InternalServerError, 1)]
    [InlineData(HttpStatusCode.OK, null)]
    public void Other_answers_are_the_sites(HttpStatusCode status, int? retryAfter)
    {
        DateTime? since = null;
        Assert.Null(Runs.RestartingAnswer(Answer(status, retryAfter), ref since));
    }

    [Fact] // a site that always answers so: after two minutes, its answer
    public void Not_for_more_than_two_minutes()
    {
        DateTime? since = DateTime.UtcNow.AddMinutes(-3);
        Assert.Null(Runs.RestartingAnswer(Answer(HttpStatusCode.ServiceUnavailable, 1), ref since));
    }
}
