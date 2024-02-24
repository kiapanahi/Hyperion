using Hyperion.Core.Management;

namespace Hyperion.Core.Tests.Management;
public sealed class DnsManagerTests
{
    [Fact]
    public void FlushesDnsCache()
    {
        var sut = new DnsManager();

        sut.FlushDnsCache();
    }
}