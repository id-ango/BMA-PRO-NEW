namespace Accounting.Services;

public sealed class AuditContext
{
    public string? IpAddress { get; private set; }

    public void SetIpAddress(string? ipAddress)
    {
        IpAddress = ipAddress;
    }
}
