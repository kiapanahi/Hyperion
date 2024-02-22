using System.Net.NetworkInformation;

using SystemPing = System.Net.NetworkInformation.Ping;

namespace Hyperion.Core.Monitoring.Ping;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "<Pending>")]
public sealed class PingInstrument(PingInstrumentOptions options, CancellationToken cancellationToken) : MonitoringInstrumentBase
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);
    private readonly PingOptions _pingOptions = new();
    private readonly SystemPing _pingSender = new();
    private readonly PingInstrumentOptions _options = options;
    private readonly CancellationToken _cancellationToken = cancellationToken;

    /// <summary>
    /// Sends ICMP echo requests and streams the ICMP reply responses back as an asynchronous stream.
    /// </summary>
    /// <returns><see cref="IAsyncEnumerable{PingResponse}"/></returns>
    public override async IAsyncEnumerable<ProbingResponse> Start()
    {
        using var timer = new PeriodicTimer(Delay);
        while (await AwaitNextTick(timer, _cancellationToken).ConfigureAwait(false))
        {
            PingReply reply;
            try
            {
                reply = await _pingSender.SendPingAsync(
                    address: _options.IPAddress,
                    timeout: _options.Timeout,
                    options: _pingOptions,
                    cancellationToken: _cancellationToken)
                    .WaitAsync(_options.Timeout)
                    .ConfigureAwait(false);
            }
            catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
            {
                break;
            }
            yield return new PingResponse(
                duration: TimeSpan.FromMilliseconds(reply!.RoundtripTime),
                status: reply.Status.ToString(),
                destination: _options.IPAddress.ToString(),
                ttl: reply.Options?.Ttl ?? -1);
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
        _pingSender?.Dispose();
    }
}
