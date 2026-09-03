namespace Accounting.Services;

public sealed class AuditContext
{
    public string? IpAddress { get; private set; }
    public bool SuppressAudit { get; private set; }

    public void SetIpAddress(string? ipAddress)
    {
        IpAddress = ipAddress;
    }

    public IDisposable BeginSuppressAudit()
    {
        var previousValue = SuppressAudit;
        SuppressAudit = true;
        return new AuditSuppressionScope(this, previousValue);
    }

    private sealed class AuditSuppressionScope : IDisposable
    {
        private readonly AuditContext _context;
        private readonly bool _previousValue;
        private bool _disposed;

        public AuditSuppressionScope(AuditContext context, bool previousValue)
        {
            _context = context;
            _previousValue = previousValue;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _context.SuppressAudit = _previousValue;
            _disposed = true;
        }
    }
}
