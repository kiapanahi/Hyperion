using SystemPing = System.Net.NetworkInformation.Ping;

namespace Core.Monitoring.Ping;
internal sealed class PingInstrument : IMonitoringInstrument, IDisposable
{
    private readonly SystemPing _pingSender;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly PingInstrumentOptions _options;
    private bool _disposedValue;

    public PingInstrument(PingInstrumentOptions options, CancellationToken cancellationToken)
    {
        _pingSender = new SystemPing();
        _pingSender.PingCompleted += PingSender_PingCompleted;

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _options = options;
    }

    private void PingSender_PingCompleted(object sender, System.Net.NetworkInformation.PingCompletedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<ProbingResponse> Probe()
    {
        throw new NotImplementedException();
    }

    private void Dispose(bool disposing)
    {
        _cancellationTokenSource?.Cancel();
        if (!_disposedValue)
        {
            if (disposing)
            {
                _pingSender?.Dispose();
                _cancellationTokenSource?.Dispose();
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
