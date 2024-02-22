using Hyperion.Core.Monitoring.Http;

namespace Hyperion.Core.Tests.Http;
public sealed class HttpResponseTests
{
    [Fact]
    public void ReportIsFormattedCorrectly()
    {
        const string expected = @"Reply: https://example.com/ [OK] time=00.18026";

        var response = new HttpResponse(TimeSpan.FromMicroseconds(180260), new Uri("https://example.com/"), "OK");

        response.Report().Should().Be(expected);
    }
}
