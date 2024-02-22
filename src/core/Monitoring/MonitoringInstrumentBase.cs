namespace Hyperion.Core.Monitoring;

public abstract class MonitoringInstrumentBase : IMonitoringInstrument
{
    private bool _disposedValue;

    public abstract IAsyncEnumerable<ProbingResponse> Start();

    protected abstract void DisposeCore();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                DisposeCore();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
