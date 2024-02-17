using System.Net;

namespace Core.Monitoring.Ping;
internal sealed class PingInstrumentOptions
{
    public PingInstrumentOptions(IPAddress ipAddress)
    {
        IPAddress = ipAddress;
    }

    public IPAddress IPAddress { get; }
}
