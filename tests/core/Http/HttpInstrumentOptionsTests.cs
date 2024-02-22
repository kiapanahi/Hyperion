using Hyperion.Core.Monitoring.Http;

namespace Hyperion.Core.Tests.Http;
public sealed class HttpInstrumentOptionsTests
{
    private static readonly Uri Uri = new("https://example.com/");

    [Fact]
    public void NullTimeoutShouldSetDefault()
    {
        var sut = new HttpInstrumentOptions(Uri, null, HttpMethod.Get);

        sut.Target.Should().Be(Uri);
        sut.Method.Should().Be(HttpMethod.Get);
        sut.Timeout.Should().Be(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void NullMethodShouldSetDefault()
    {
        var sut = new HttpInstrumentOptions(Uri, TimeSpan.FromSeconds(1), null);

        sut.Target.Should().Be(Uri);
        sut.Method.Should().Be(HttpMethod.Head);
        sut.Timeout.Should().Be(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void FullCtor()
    {
        var sut = new HttpInstrumentOptions(Uri, TimeSpan.FromSeconds(1), HttpMethod.Post);

        sut.Target.Should().Be(Uri);
        sut.Method.Should().Be(HttpMethod.Post);
        sut.Timeout.Should().Be(TimeSpan.FromSeconds(1));
    }
}
