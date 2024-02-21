using System.Net.NetworkInformation;

using SystemPing = System.Net.NetworkInformation.Ping;

namespace Hyperion.Core.Monitoring.Ping;
public sealed partial class PingInstrument(PingInstrumentOptions options, CancellationToken cancellationToken) : IMonitoringInstrument
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);
    private readonly PingOptions _pingOptions = new();
    private readonly SystemPing _pingSender = new();
    private readonly PingInstrumentOptions _options = options;
    private readonly CancellationToken _cancellationToken = cancellationToken;
    private bool _disposedValue;

    /// <summary>
    /// Sends ICMP echo requests and streams the ICMP reply responses back as an asynchronous stream.
    /// </summary>
    /// <returns><see cref="IAsyncEnumerable{PingResponse}"/></returns>
    public async IAsyncEnumerable<ProbingResponse> Start()
    {
        using var timer = new PeriodicTimer(Delay);
        while (await AwaitNextTick(timer, _cancellationToken).ConfigureAwait(false))
        {
            PingReply reply;
            try
            {
                reply = await _pingSender.SendPingAsync(address: _options.IPAddress,
                                                        timeout: _options.Timeout,
                                                        options: _pingOptions,
                                                        cancellationToken: _cancellationToken)
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
                ttl: reply.Options!.Ttl);
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
}
