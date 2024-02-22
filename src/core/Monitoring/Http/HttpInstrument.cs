using System.Diagnostics;

namespace Hyperion.Core.Monitoring.Http;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "<Pending>")]
public sealed class HttpInstrument : MonitoringInstrumentBase
{
    private const string HostNotFound = @"Host not found";
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);
    private readonly HttpInstrumentOptions _options;
    private readonly CancellationToken _cancellationToken;

    private static readonly HttpClient StaticClient = new(new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromSeconds(15)
    });

    public HttpInstrument(HttpInstrumentOptions options, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(nameof(options));

        _options = options;
        _cancellationToken = cancellationToken;
    }

    public override async IAsyncEnumerable<ProbingResponse> Start()
    {

        var sw = new Stopwatch();
        using var timer = new PeriodicTimer(Delay);
        while (await AwaitNextTick(timer, _cancellationToken).ConfigureAwait(false))
        {
            sw.Start();
            using var payload = new HttpRequestMessage(_options.Method, _options.Target);
            var statusCode = string.Empty;
            try
            {
                var response = await StaticClient
                    .SendAsync(payload, HttpCompletionOption.ResponseHeadersRead, _cancellationToken)
                    .WaitAsync(_options.Timeout, _cancellationToken)
                    .ConfigureAwait(false);
                statusCode = response.StatusCode.ToString();
            }
            catch (HttpRequestException)
            {
                statusCode = HostNotFound;
            }

            var duration = sw.Elapsed;
            sw.Reset();

            yield return new HttpResponse(duration, _options.Target, statusCode);
        }

        static async Task<bool> AwaitNextTick(PeriodicTimer timer, CancellationToken cancellationToken)
        {
            try
            {
                return await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
    }



    protected override void DisposeCore()
    {
        StaticClient?.Dispose();
    }
}
