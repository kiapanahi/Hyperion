using System.Net;

using Hyperion.Core.Monitoring.Ping;

namespace Hyperion.Core.Tests.Ping;
public sealed class PingInstrumentOptionsTests
{
    private static readonly IPAddress Ip = IPAddress.Parse("127.0.0.1");

    [Fact]
    public void DefaultTimeout()
    {
        var sut = new PingInstrumentOptions(Ip);
        sut.Timeout.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void CustomTimeout()
    {
        var sut = new PingInstrumentOptions(Ip, TimeSpan.FromSeconds(1));
        sut.Timeout.Should().Be(TimeSpan.FromSeconds(1));
    }
}
