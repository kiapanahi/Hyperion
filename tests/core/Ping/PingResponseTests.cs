using Hyperion.Core.Monitoring.Ping;

namespace Hyperion.Core.Tests.Ping;
public sealed class PingResponseTests
{

    [Fact]
    public void ReportIsFormattedCorrectly()
    {
        const string expected = @"Reply from 8.8.8.8: time=00.031 TTL=51 (Success)";
        var pr = new PingResponse(TimeSpan.FromMilliseconds(31), "Success", "8.8.8.8", 51);

        pr.Report().Should().Be(expected);
    }
}
