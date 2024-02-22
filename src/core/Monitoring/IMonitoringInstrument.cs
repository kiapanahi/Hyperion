namespace Hyperion.Core.Monitoring;
public interface IMonitoringInstrument : IDisposable
{
    IAsyncEnumerable<ProbingResponse> Start();
}
