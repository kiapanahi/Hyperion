using System.Net;

using Hyperion.Core.Monitoring;
using Hyperion.Core.Monitoring.Ping;

namespace Hyperion.Core.Tests.Ping;

public class PingInstrumentTests
{
    private const string IpAddress = "127.0.0.1";
    [Fact]
    public void InstantiatesPingInstrument()
    {
        var options = new PingInstrumentOptions(IPAddress.Parse(IpAddress));
        using var cts = new CancellationTokenSource();

        using IMonitoringInstrument sut = new PingInstrument(options, cts.Token);

        sut.Should().NotBeNull();
    }

    [Fact]
    public void InstantiatesPingInstrumentWithNoneCancellation()
    {
        var options = new PingInstrumentOptions(IPAddress.Parse(IpAddress));
        using var cts = new CancellationTokenSource();

        using IMonitoringInstrument sut = new PingInstrument(options, CancellationToken.None);

        sut.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public async Task StartsPing(int durationInSeconds)
    {
        var options = new PingInstrumentOptions(IPAddress.Parse(IpAddress));

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(durationInSeconds));
        cts.IsCancellationRequested.Should().BeFalse();

        using PingInstrument sut = new(options, cts.Token);

        await foreach (var response in sut.Start())
        {
            response.Duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
        }

        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public async Task CanceledPing()
    {
        var options = new PingInstrumentOptions(IPAddress.Parse(IpAddress));

        using var cts = new CancellationTokenSource();
        cts.IsCancellationRequested.Should().BeFalse();

        await cts.CancelAsync();
        cts.IsCancellationRequested.Should().BeTrue();

        using PingInstrument sut = new(options, cts.Token);

        sut.Start()
            .Enumerating(r => r.ToBlockingEnumerable())
            .Should()
            .NotThrow();

        cts.IsCancellationRequested.Should().BeTrue();

    }
}