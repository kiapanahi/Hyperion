using System.Net;

using Hyperion.Core.Monitoring.Ping;

PrintHeader();

var table = new Table()
    .Border(TableBorder.DoubleEdge)
    .BorderColor(Color.Yellow)
    .LeftAligned()
    .AddColumn("Ping");

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, e) => cts.Cancel();

using var pinger = new PingInstrument(
    options: new PingInstrumentOptions(IPAddress.Parse("8.8.8.8")),
    cancellationToken: cts.Token);

await AnsiConsole.Live(table)
    .StartAsync(async ctx =>
    {
        await foreach (var item in pinger.Start())
        {
            table.AddRow(new Markup(item.Report()));
            ctx.Refresh();
        }
    });

static void PrintHeader()
{
    AnsiConsole.Write(new FigletText("Hyperion")
        .Justify(Justify.Left)
        .Color(Color.Cyan3));
}