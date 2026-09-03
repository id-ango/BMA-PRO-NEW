using System.Security.Claims;
using System.Text.Json;
using Accounting.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Accounting.Services;

public sealed class AuditService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly AuditContext _auditContext;
    private readonly AuditOptions _options;

    public AuditService(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        AuthenticationStateProvider authenticationStateProvider,
        AuditContext auditContext,
        IOptions<AuditOptions> options)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _authenticationStateProvider = authenticationStateProvider;
        _auditContext = auditContext;
        _options = options.Value;
    }

    public async Task WriteAsync(string action, string entityName = "Authentication", string? entityId = null, object? details = null, CancellationToken cancellationToken = default, bool success = true, ClaimsPrincipal? principal = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var now = DateTime.UtcNow;
        var localNow = ConvertToConfiguredTimeZone(now);
        var user = principal ?? httpContext?.User;

        if (principal is null && (user?.Identity?.IsAuthenticated != true))
        {
            user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
        }

        _context.AuditLogs.Add(new AuditEntry
        {
            OccurredUtc = now,
            UserId = user?.FindFirstValue(ClaimTypes.NameIdentifier),
            UserName = user?.Identity?.Name,
            IpAddress = _auditContext.IpAddress ?? httpContext?.Connection.RemoteIpAddress?.ToString(),
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

    public IQueryable<AuditEntry> Query(
        DateTime? fromLocal,
        DateTime? toLocal,
        string? userName,
        string? action)
    {
        var query = Query();

        if (fromLocal.HasValue)
        {
            query = query.Where(item => item.OccurredUtc >= ConvertConfiguredTimeToUtc(fromLocal.Value));
        }

        if (toLocal.HasValue)
        {
            query = query.Where(item => item.OccurredUtc < ConvertConfiguredTimeToUtc(toLocal.Value));
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            query = query.Where(item => item.UserName != null && item.UserName.Contains(userName));
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(item => item.Action == action);
        }

        return query;
    }

    public DateTime ConvertToConfiguredTimeZone(DateTime utc)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.TimeZone);
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), timeZone);
    }

    public DateTime ConvertConfiguredTimeToUtc(DateTime localTime)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.TimeZone);
        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified), timeZone);
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
