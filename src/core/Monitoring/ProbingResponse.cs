namespace Hyperion.Core.Monitoring;
public abstract class ProbingResponse(TimeSpan duration)
{
    public TimeSpan Duration { get; } = duration;

    public abstract string Report();
}
