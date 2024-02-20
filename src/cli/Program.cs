using System.Net;

using Hyperion.Core.Monitoring.Ping;

var t = new FigletText("Hyperion")
{
    Justification = Justify.Center,
    Color = Color.Cyan3
};
AnsiConsole.Write(t);

var table = new Table()
{
    Border = TableBorder.DoubleEdge,
};
table.Centered()
    .AddColumn("Ping", c => c.Centered());

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += Console_CancelKeyPress;

void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
{
    cts.Cancel();
}

using var pinger = new PingInstrument(new PingInstrumentOptions(IPAddress.Parse("8.8.8.8")), cts.Token);
await AnsiConsole.Live(table)
    .StartAsync(async ctx =>
    {
        await foreach (var item in pinger.Start())
        {
            table.AddRow(new Markup($"{item.Duration}"));
            ctx.Refresh();
        }
    });