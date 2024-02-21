using System.Net.NetworkInformation;

using SystemPing = System.Net.NetworkInformation.Ping;

namespace Hyperion.Core.Monitoring.Ping;
public sealed class PingInstrument(PingInstrumentOptions options, CancellationToken cancellationToken) : IMonitoringInstrument
{
    private readonly PingOptions _pingOptions = new();
    private readonly SystemPing _pingSender = new();
    private readonly PingInstrumentOptions _options = options;
    private readonly CancellationToken _cancellationToken = cancellationToken;
    private bool _disposedValue;

    /// <summary>
    /// Sends ICMP echo requests and streams the ICMP reply responses back as an asynchronous stream.
    /// </summary>
    /// <returns><see cref="IAsyncEnumerable{PingResponse}"/></returns>
    public async IAsyncEnumerable<ProbingResponse> Start()
    {
        while (true)
        {
            PingReply reply;
            try
            {
                reply = await _pingSender
                .SendPingAsync(_options.IPAddress, _options.Timeout, options: _pingOptions, cancellationToken: _cancellationToken)
                .ConfigureAwait(false);
            }
            catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
            {
                break;
            }
            yield return new PingResponse(
                duration: TimeSpan.FromMilliseconds(reply!.RoundtripTime),
                status: reply.Status.ToString(),
                destination: _options.IPAddress.ToString(),
                ttl: reply.Options!.Ttl);
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _pingSender?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
