using System.Net;

namespace Hyperion.Core.Monitoring.Ping;
/// <summary>
/// Options for configuring an instance of <see cref="PingInstrument"/> probe
/// </summary>
/// <param name="ipAddress">The IP address to ping</param>
/// <param name="timeout">The ICMP echo request timeout. Defaults to 1 second</param>
public sealed class PingInstrumentOptions(IPAddress ipAddress, TimeSpan? timeout = null)
{
    private static readonly TimeSpan ThirtySeconds = TimeSpan.FromSeconds(1);

    public IPAddress IPAddress { get; } = ipAddress;
    public TimeSpan Timeout { get; } = timeout ?? ThirtySeconds;
}
