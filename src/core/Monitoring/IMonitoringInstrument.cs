namespace Hyperion.Core.Monitoring;
internal interface IMonitoringInstrument : IDisposable
{
    IAsyncEnumerable<ProbingResponse> Start();
}
