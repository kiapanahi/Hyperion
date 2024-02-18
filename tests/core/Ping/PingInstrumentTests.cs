using System.Net;

using Hyperion.Core.Monitoring;
using Hyperion.Core.Monitoring.Ping;

namespace Hyperion.Core.Tests.Ping;

public class PingInstrumentTests
{
    [Fact]
    public void InstantiatesPingInstrument()
    {
        var options = new PingInstrumentOptions(IPAddress.Parse("8.8.8.8"));
        using var cts = new CancellationTokenSource();

        using IMonitoringInstrument sut = new PingInstrument(options, cts.Token);

        sut.Should().NotBeNull();
    }

    [Fact]
    public void InstantiatesPingInstrumentWithNoneCancellation()
    {
        var options = new PingInstrumentOptions(IPAddress.Parse("8.8.8.8"));
        using var cts = new CancellationTokenSource();

        using IMonitoringInstrument sut = new PingInstrument(options, CancellationToken.None);

        sut.Should().NotBeNull();
    }

    [Fact]
    public void InstantiatesPingInstrumentWithNullCancellation()
    {
        var options = new PingInstrumentOptions(IPAddress.Parse("8.8.8.8"));
        using var cts = new CancellationTokenSource();

        using IMonitoringInstrument sut = new PingInstrument(options);

        sut.Should().NotBeNull();
    }

    [Fact]
    public async Task StartsPing()
    {
        var options = new PingInstrumentOptions(IPAddress.Parse("8.8.8.8"));

        using var cts = new CancellationTokenSource();
        cts.IsCancellationRequested.Should().BeFalse();

        using PingInstrument sut = new(options, cts.Token);

        await foreach (var response in sut.Start())
        {
            response.Duration.Should().BeLessThan(TimeSpan.FromSeconds(1));
        }

        cts.IsCancellationRequested.Should().BeFalse();
    }

    [Fact]
    public async Task CanceledPing()
    {
        var options = new PingInstrumentOptions(IPAddress.Parse("8.8.8.8"));

        using var cts = new CancellationTokenSource(100_0000);
        cts.IsCancellationRequested.Should().BeFalse();

        await cts.CancelAsync();
        cts.IsCancellationRequested.Should().BeTrue();

        using PingInstrument sut = new(options, cts.Token);

        sut.Start()
            .Enumerating(r => r.ToBlockingEnumerable())
            .Should()
            .ThrowExactly<OperationCanceledException>();

        cts.IsCancellationRequested.Should().BeTrue();

    }
}