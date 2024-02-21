using System.Globalization;
using System.Text;

namespace Hyperion.Core.Monitoring.Ping;
public sealed class PingResponse : ProbingResponse
{
    private static readonly CompositeFormat ReportFormat = CompositeFormat.Parse("Reply from {0}: time={1} TTL={2} ({3})");

    public PingResponse(TimeSpan duration, string status, string destination, int ttl) : base(duration)
    {
        Status = status;
        Destination = destination;
        Ttl = ttl;
    }

    public string Status { get; }
    public string Destination { get; }
    public int Ttl { get; }

    public override string Report()
    {
        return string.Format(CultureInfo.InvariantCulture, ReportFormat,
            Destination,
            Duration.ToString(@"ss\.fff", CultureInfo.InvariantCulture),
            Ttl,
            Status);
    }
}
