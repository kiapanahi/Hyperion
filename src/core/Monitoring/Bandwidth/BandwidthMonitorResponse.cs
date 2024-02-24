namespace Hyperion.Core.Monitoring.Bandwidth;
public sealed class BandwidthMonitorResponse(string text) : ProbingResponse(TimeSpan.Zero)
{
    public string Text { get; } = text;

    public override string Report()
    {
        return Text;
    }
}
