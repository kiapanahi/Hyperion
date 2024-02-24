using System.Net.NetworkInformation;

namespace Hyperion.Core.Monitoring.Bandwidth;
public sealed class BandwidthMonitorInstrument(CancellationToken cancellationToken) : MonitoringInstrumentBase
{
    private readonly CancellationToken _cancellationToken = cancellationToken;

    public override async IAsyncEnumerable<ProbingResponse> Start()
    {
        _cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask.ConfigureAwait(false);

        IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
        yield return new BandwidthMonitorResponse($"Interface information for {computerProperties.HostName}.{computerProperties.DomainName}");

        var interfaces = NetworkInterface.GetAllNetworkInterfaces();
        yield return new BandwidthMonitorResponse($"Number of interfaces .................... : {interfaces.Length}");

        foreach (NetworkInterface adapter in interfaces)
        {
            IPInterfaceProperties properties = adapter.GetIPProperties();
            yield return new BandwidthMonitorResponse(string.Empty);
            yield return new BandwidthMonitorResponse(adapter.Description);
            yield return new BandwidthMonitorResponse(string.Empty.PadLeft(adapter.Description.Length, '='));
            yield return new BandwidthMonitorResponse($"  Interface type .......................... : {adapter.NetworkInterfaceType}");
            yield return new BandwidthMonitorResponse($"  Physical Address ........................ : {adapter.GetPhysicalAddress().ToString()}");
            yield return new BandwidthMonitorResponse($"  Operational status ...................... : {adapter.OperationalStatus}");
            string versions = "";

            // Create a display string for the supported IP versions.
            if (adapter.Supports(NetworkInterfaceComponent.IPv4))
            {
                versions = "IPv4";
            }
            if (adapter.Supports(NetworkInterfaceComponent.IPv6))
            {
                if (versions.Length > 0)
                {
                    versions += " ";
                }
                versions += "IPv6";
            }
            yield return new BandwidthMonitorResponse($"  IP version .............................. : {versions}");

            // The following information is not useful for loopback adapters.
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
            {
                continue;
            }
            yield return new BandwidthMonitorResponse($"  DNS suffix .............................. : {properties.DnsSuffix}");

            if (adapter.Supports(NetworkInterfaceComponent.IPv4))
            {
                IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                yield return new BandwidthMonitorResponse($"  MTU...................................... : {ipv4.Mtu}");
            }

            yield return new BandwidthMonitorResponse($"  Receive Only ............................ : {adapter.IsReceiveOnly}");
            yield return new BandwidthMonitorResponse($"  Multicast ............................... : {adapter.SupportsMulticast}");

            yield return new BandwidthMonitorResponse(string.Empty);
        }

    }

    protected override void DisposeCore()
    {
        throw new NotImplementedException();
    }
}
