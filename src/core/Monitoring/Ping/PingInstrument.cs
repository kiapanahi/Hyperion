using System.Net.NetworkInformation;

using SystemPing = System.Net.NetworkInformation.Ping;

namespace Hyperion.Core.Monitoring.Ping;
public sealed class PingInstrument(PingInstrumentOptions options, CancellationToken? cancellationToken = null) : IMonitoringInstrument
{
    private readonly PingOptions _pingOptions = new();
    private readonly SystemPing _pingSender = new();
    private readonly PingInstrumentOptions _options = options;
    private readonly CancellationToken _cancellationToken = cancellationToken ?? CancellationToken.None;
    private bool _disposedValue;

    /// <summary>
    /// Sends ICMP echo requests and streams the ICMP reply responses back as an asynchronous stream.
    /// </summary>
    /// <returns><see cref="IAsyncEnumerable{ProbingResponse}"/></returns>
    public async IAsyncEnumerable<ProbingResponse> Start()
    {
        PingReply? reply = null;
        bool canceled = false;
        try
        {
            reply = await _pingSender
            .SendPingAsync(_options.IPAddress, _options.Timeout, options: _pingOptions, cancellationToken: _cancellationToken)
            .ConfigureAwait(false);
        }
        catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
        {
            canceled = true;
        }

        yield return canceled
            ? throw new OperationCanceledException("Canceled")
            : new ProbingResponse(TimeSpan.FromMilliseconds(reply!.RoundtripTime));
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
