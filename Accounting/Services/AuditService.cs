using System.Security.Claims;
using System.Text.Json;
using Accounting.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Accounting.Services;

public sealed class AuditService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuditOptions _options;

    public AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IOptions<AuditOptions> options)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
    }

    public async Task WriteAsync(string action, string entityName = "Authentication", string? entityId = null, object? details = null, CancellationToken cancellationToken = default, bool success = true, ClaimsPrincipal? principal = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var now = DateTime.UtcNow;
        var localNow = ConvertToConfiguredTimeZone(now);
        var user = principal ?? httpContext?.User;

        _context.AuditLogs.Add(new AuditEntry
        {
            OccurredUtc = now,
            UserId = user?.FindFirstValue(ClaimTypes.NameIdentifier),
            UserName = user?.Identity?.Name,
            IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Success = success,
            OutsideWorkingHours = IsOutsideWorkingHours(localNow),
            Details = details is null ? null : JsonSerializer.Serialize(details)
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<AuditEntry> Query() => _context.AuditLogs.AsNoTracking();

    public DateTime ConvertToConfiguredTimeZone(DateTime utc)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.TimeZone);
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), timeZone);
    }

    public bool IsOutsideWorkingHours(DateTime localTime)
    {
        var days = _options.WorkingDays.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => Enum.TryParse<DayOfWeek>(value, true, out var day) ? day : (DayOfWeek?)null)
            .Where(day => day.HasValue)
            .Select(day => day!.Value)
            .ToHashSet();

        return !days.Contains(localTime.DayOfWeek) || localTime.TimeOfDay < _options.WorkdayStart || localTime.TimeOfDay >= _options.WorkdayEnd;
    }
}
