namespace Core.Monitoring;
internal interface IMonitoringInstrument
{
    IAsyncEnumerable<ProbingResponse> Probe();
}
