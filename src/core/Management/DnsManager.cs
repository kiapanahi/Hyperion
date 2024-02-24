using System.Runtime.InteropServices;

namespace Hyperion.Core.Management;
public sealed partial class DnsManager
{
    [LibraryImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
#pragma warning disable CA5392 // Use DefaultDllImportSearchPaths attribute for P/Invokes
    private static partial uint FlushDns();
#pragma warning restore CA5392 // Use DefaultDllImportSearchPaths attribute for P/Invokes


#pragma warning disable CA1822 // Mark members as static
    public void FlushDnsCache()
    {
        _ = FlushDns();
    }
#pragma warning restore CA1822 // Mark members as static
}
