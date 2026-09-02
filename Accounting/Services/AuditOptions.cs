namespace Accounting.Services;

public sealed class AuditOptions
{
    public string TimeZone { get; set; } = "SE Asia Standard Time";
    public string WorkingDays { get; set; } = "Monday,Tuesday,Wednesday,Thursday,Friday";
    public TimeSpan WorkdayStart { get; set; } = new(8, 0, 0);
    public TimeSpan WorkdayEnd { get; set; } = new(17, 0, 0);
}
