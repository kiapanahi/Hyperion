using System.Net;

namespace Hyperion.Core.Monitoring.Ping;
public sealed class PingInstrumentOptions(IPAddress ipAddress, TimeSpan? timeout = null)
{
    private static readonly TimeSpan ThirtySeconds = TimeSpan.FromSeconds(30);

    public IPAddress IPAddress { get; } = ipAddress;
    public TimeSpan Timeout { get; } = timeout ?? ThirtySeconds;
}
