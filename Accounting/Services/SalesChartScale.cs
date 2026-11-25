namespace Accounting.Services;

public sealed class SalesChartScale
{
    public decimal Minimum { get; }
    public decimal Maximum { get; }
    public decimal Range => Maximum - Minimum;
    public decimal ZeroPosition => Range == 0 ? 0 : -Minimum / Range * 100;
    public IReadOnlyList<SalesChartTick> Ticks { get; }

    public SalesChartScale(IEnumerable<decimal> values)
    {
        var numbers = values.ToArray();
        Minimum = Math.Min(0, numbers.DefaultIfEmpty(0).Min());
        Maximum = Math.Max(0, numbers.DefaultIfEmpty(0).Max());

        if (Range == 0)
        {
            Ticks = new[] { new SalesChartTick(0, 0) };
            return;
        }

        var ticks = Enumerable.Range(0, 5)
            .Select(i => new SalesChartTick(Minimum + Range * i / 4, i * 25))
            .ToList();
        if (Minimum < 0 && Maximum > 0 && ticks.All(tick => Math.Abs(tick.Position - ZeroPosition) >= 8))
        {
            ticks.Add(new SalesChartTick(0, ZeroPosition));
        }

        Ticks = ticks.OrderBy(tick => tick.Position).ToArray();
    }
}

public readonly record struct SalesChartTick(decimal Value, decimal Position);
