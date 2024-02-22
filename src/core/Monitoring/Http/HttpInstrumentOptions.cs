namespace Hyperion.Core.Monitoring.Http;
public sealed class HttpInstrumentOptions
{
    public Uri Target { get; }
    public TimeSpan Timeout { get; }
    public HttpMethod Method { get; }

    /// <summary>
    /// Options for configuring an instance of <see cref="HttpInstrument"/> probe
    /// </summary>
    /// <param name="target">The HTTP/HTTPS endpoint to call</param>
    /// <param name="timeout">The HTTP call timeout. Defaults to 2 seconds</param>
    /// <param name="method">The HTTP method to use. Defaults to HEAD</param>
    public HttpInstrumentOptions(Uri target, TimeSpan? timeout, HttpMethod? method)
    {
        Target = target;
        Timeout = timeout ?? TimeSpan.FromSeconds(5);
        Method = method ?? HttpMethod.Head;
    }
}
