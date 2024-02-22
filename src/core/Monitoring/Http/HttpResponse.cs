using System.Globalization;
using System.Text;

namespace Hyperion.Core.Monitoring.Http;
public sealed class HttpResponse : ProbingResponse
{
    private static readonly CompositeFormat ReportFormat = CompositeFormat.Parse("Reply: {0} [{1}] time={2}");

    public HttpResponse(TimeSpan duration, Uri target, string statusCode) : base(duration)
    {
        Target = target;
        StatusCode = statusCode;
    }

    public Uri Target { get; }
    public string StatusCode { get; }

    public override string Report()
    {
        return string.Format(CultureInfo.InvariantCulture, ReportFormat,
            Target,
            StatusCode,
            Duration.ToString(@"ss\.fffff", CultureInfo.InvariantCulture));
    }
}
