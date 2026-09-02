namespace Accounting.Data;

public sealed class AuditEntry
{
    public long Id { get; set; }
    public DateTime OccurredUtc { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? IpAddress { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public bool Success { get; set; }
    public bool OutsideWorkingHours { get; set; }
    public string? Details { get; set; }
}
