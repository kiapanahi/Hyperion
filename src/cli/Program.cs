using System.Net;
using System.Threading.Channels;

using Hyperion.Core.Monitoring.Bandwidth;
using Hyperion.Core.Monitoring.Http;
using Hyperion.Core.Monitoring.Ping;
using Hyperion.Core.Versioning;

var table = new Table()
    .Border(TableBorder.Rounded)
    .BorderColor(Color.Yellow)
    .LeftAligned()
    .AddColumn("Ping")
    .AddColumn("Http")
    .AddEmptyRow();

// Create the layout
var headerRowLayout = new Layout("header",
    new Panel(new Markup($"[red]Hyperion[/]\n[blue]{VersionProvider.Version}[/]").Centered()).Expand()
    );

var monitoringRowLayout = new Layout("monitoring", table.Expand()).Size(2);

var layout = new Layout("root").SplitRows(headerRowLayout, new Layout().Ratio(5).Invisible());
AnsiConsole.Write(layout);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, e) => cts.Cancel();

Channel<ColumnOutput> notificationChannel = Channel.CreateUnbounded<ColumnOutput>(new UnboundedChannelOptions()
{
    AllowSynchronousContinuations = true,
    SingleReader = true,
    SingleWriter = false
});
var httpTask = StartHttpProbe(notificationChannel, cts.Token);
var pingTask = StartPingProbe(notificationChannel, cts.Token);

var bandwidthTask = StartBandwidthMonitoring(notificationChannel, cts.Token);

//var liveActionTask = AnsiConsole.Live(table)
//    .Cropping(VerticalOverflowCropping.Top)
//    .StartAsync(async ctx =>
//    {

//        await foreach (var output in notificationChannel.Reader.ReadAllAsync())
//        {
//            IReadOnlyList<TableRow> rows = table.Rows;
//            var lastRowIndex = rows.Count - 1;
//            var lastRow = lastRowIndex >= 0 ? rows[^1] : null;
//            switch (output.Column)
//            {
//                case "Ping":
//                    if (lastRow is not null && lastRow[0] == Text.Empty)
//                    {
//                        table.UpdateCell(lastRowIndex, 0, new Text(output.Text));
//                    }
//                    else
//                    {
//                        table.AddRow(new Text(output.Text), Text.Empty);
//                    }
//                    break;
//                case "Http":
//                    if (lastRow is not null && lastRow[1] == Text.Empty)
//                    {
//                        table.UpdateCell(lastRowIndex, 1, new Text(output.Text));
//                    }
//                    else
//                    {
//                        table.AddRow(Text.Empty, new Text(output.Text));
//                    }
//                    break;
//                default:
//                    throw new InvalidOperationException();
//            }
//            ctx.Refresh();
//        }
//    });

Console.ReadLine();


Task StartHttpProbe(Channel<ColumnOutput> notificationChannel, CancellationToken cancellationToken)
{
    const string header = "Http";

    return Task.Factory.StartNew(async () =>
    {
        using var probe = new HttpInstrument(
            options: new HttpInstrumentOptions(new Uri("https://example.com/"), null, null),
            cancellationToken: cancellationToken);

        await foreach (var item in probe.Start())
        {
            await notificationChannel.Writer.WriteAsync(new ColumnOutput(header, item.Report()), cancellationToken);
        }
    },
    cancellationToken,
    TaskCreationOptions.LongRunning,
    TaskScheduler.Default);
}

Task StartPingProbe(Channel<ColumnOutput> notificationChannel, CancellationToken cancellationToken)
{
    const string header = "Ping";
    return Task.Factory.StartNew(async () =>
    {
        using var probe = new PingInstrument(
        options: new PingInstrumentOptions(IPAddress.Parse("8.8.8.8")),
        cancellationToken: cancellationToken);

        await foreach (var item in probe.Start())
        {
            await notificationChannel.Writer.WriteAsync(new ColumnOutput(header, item.Report()), cancellationToken);
        }
    },
    cancellationToken,
    TaskCreationOptions.LongRunning,
    TaskScheduler.Default);
}

Task StartBandwidthMonitoring(Channel<ColumnOutput> notificationChannel, CancellationToken cancellationToken)
{
    return Task.Factory.StartNew(async () =>
    {
        using var probe = new BandwidthMonitorInstrument(cancellationToken);

        await foreach (var item in probe.Start())
        {
            AnsiConsole.WriteLine(item.Report());
            //await notificationChannel.Writer.WriteAsync(new ColumnOutput(header, item.Report()), cancellationToken);
        }
    },
    cancellationToken,
    TaskCreationOptions.LongRunning,
    TaskScheduler.Default);
}


readonly record struct ColumnOutput(string Column, string Text);