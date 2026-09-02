using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Accounting.Services;

public sealed class AuditCookieEvents : CookieAuthenticationEvents
{
    private readonly AuditService _auditService;

    public AuditCookieEvents(AuditService auditService)
    {
        _auditService = auditService;
    }

    public override async Task SigningIn(CookieSigningInContext context)
    {
        await _auditService.WriteAsync(
            "Login",
            "Authentication",
            context.Principal?.Identity?.Name,
            new { Result = "Success" },
            context.HttpContext.RequestAborted,
            principal: context.Principal);

        await base.SigningIn(context);
    }

    public override async Task SigningOut(CookieSigningOutContext context)
    {
        await _auditService.WriteAsync(
            "Logout",
            "Authentication",
            context.HttpContext.User.Identity?.Name,
            new { Result = "Success" },
            context.HttpContext.RequestAborted);

        await base.SigningOut(context);
    }
}
