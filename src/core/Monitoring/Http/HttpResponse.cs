using System.Globalization;
using System.Text;

namespace Hyperion.Core.Monitoring.Http;
public sealed class HttpResponse : ProbingResponse
{
    private static readonly CompositeFormat ReportFormat = CompositeFormat.Parse("Reply: {0} [{1}] time={2}");
    private static readonly CompositeFormat ErrorFormat = CompositeFormat.Parse("Reply: {0} Error: {1}");

    public HttpResponse(TimeSpan duration, Uri target, string statusCode) : base(duration)
    {
        Target = target;
        StatusCode = statusCode;
        Error = null;
    }

    public HttpResponse(TimeSpan duration, string error) : base(duration)
    {
        Error = error;
        StatusCode = string.Empty;
    }

    public Uri? Target { get; }
    public string StatusCode { get; }
    public string? Error { get; }

    public override string Report()
    {
        return !string.IsNullOrEmpty(Error)
            ? string.Format(CultureInfo.InvariantCulture, ErrorFormat, Target, Error)
            : string.Format(CultureInfo.InvariantCulture, ReportFormat, Target, StatusCode,
                            Duration.ToString(@"ss\.fffff", CultureInfo.InvariantCulture));
    }
}
