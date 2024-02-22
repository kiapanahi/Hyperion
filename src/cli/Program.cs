using System.Net;
using System.Threading.Channels;

using Hyperion.Core.Monitoring.Http;
using Hyperion.Core.Monitoring.Ping;
using Hyperion.Core.Versioning;

PrintHeader();

var table = new Table()
    .Border(TableBorder.DoubleEdge)
    .BorderColor(Color.Yellow)
    .LeftAligned()
    .AddColumn("Ping")
    .AddColumn("Http")
    .AddEmptyRow();

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

await AnsiConsole.Live(table)
    .Cropping(VerticalOverflowCropping.Top)
    .StartAsync(async ctx =>
    {

        await foreach (var output in notificationChannel.Reader.ReadAllAsync())
        {
            IReadOnlyList<TableRow> rows = table.Rows;
            var lastRowIndex = rows.Count - 1;
            var lastRow = lastRowIndex >= 0 ? rows[^1] : null;
            switch (output.Column)
            {
                case "Ping":
                    if (lastRow is not null && lastRow[0] == Text.Empty)
                    {
                        table.UpdateCell(lastRowIndex, 0, new Text(output.Text));
                    }
                    else
                    {
                        table.AddRow(new Text(output.Text), Text.Empty);
                    }
                    break;
                case "Http":
                    if (lastRow is not null && lastRow[1] == Text.Empty)
                    {
                        table.UpdateCell(lastRowIndex, 1, new Text(output.Text));
                    }
                    else
                    {
                        table.AddRow(Text.Empty, new Text(output.Text));
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
            ctx.Refresh();
        }
    });

static Task StartHttpProbe(Channel<ColumnOutput> notificationChannel, CancellationToken cancellationToken)
{
    const string header = "Http";

    return Task.Factory.StartNew(async () =>
    {
        using var httpProbe = new HttpInstrument(
            options: new HttpInstrumentOptions(new Uri("https://example.com/"), null, null),
            cancellationToken: cancellationToken);

        await foreach (var item in httpProbe.Start())
        {
            await notificationChannel.Writer.WriteAsync(new ColumnOutput(header, item.Report()), cancellationToken);
        }
    },
    cancellationToken,
    TaskCreationOptions.LongRunning,
    TaskScheduler.Default);
}

static Task StartPingProbe(Channel<ColumnOutput> notificationChannel, CancellationToken cancellationToken)
{
    const string header = "Ping";
    return Task.Factory.StartNew(async () =>
    {
        using var pingProbe = new PingInstrument(
        options: new PingInstrumentOptions(IPAddress.Parse("8.8.8.8")),
        cancellationToken: cancellationToken);


        await foreach (var item in pingProbe.Start())
        {
            await notificationChannel.Writer.WriteAsync(new ColumnOutput(header, item.Report()), cancellationToken);
        }
    },
    cancellationToken,
    TaskCreationOptions.LongRunning,
    TaskScheduler.Default);
}

static void PrintHeader()
{
    AnsiConsole.Write(new FigletText("Hyperion")
        .Justify(Justify.Left)
        .Color(Color.Cyan3));
    AnsiConsole.WriteLine(VersionProvider.Version);
}

readonly record struct ColumnOutput(string Column, string Text);