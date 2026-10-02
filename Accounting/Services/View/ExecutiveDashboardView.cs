using System;

namespace Accounting.Services.View;

public sealed class ExecutiveDashboardView
{
    public DateTime GeneratedAt { get; init; }
    public decimal KasBank { get; init; }
    public decimal Piutang { get; init; }
    public decimal Hutang { get; init; }
    public decimal Persediaan { get; init; }
    public decimal SalesOrder { get; init; }
    public decimal PurchaseOrder { get; init; }
    public int JumlahItemStock { get; init; }
    public int JumlahSalesOrder { get; init; }
    public int JumlahPurchaseOrder { get; init; }
    public decimal PiutangJatuhTempo { get; init; }
    public decimal HutangJatuhTempo { get; init; }
}
