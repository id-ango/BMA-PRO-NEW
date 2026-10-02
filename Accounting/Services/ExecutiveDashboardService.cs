using Accounting.Services.View;
using eSoft.CashBank.Services;
using eSoft.Hutang.Services;
using eSoft.Order.Services;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Services;
using eSoft.Piutang.Services;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Services;

public sealed class ExecutiveDashboardService : IExecutiveDashboardService
{
    private readonly ICashBankServices _cashBankService;
    private readonly IReceivableServices _receivableService;
    private readonly IPayableServices _payableService;
    private readonly IInventoryServices _inventoryService;
    private readonly IOrderSalesServices _salesOrderService;
    private readonly IOrderPurchaseServices _purchaseOrderService;
    private readonly DbContextPersediaan _inventoryContext;

    public ExecutiveDashboardService(
        ICashBankServices cashBankService,
        IReceivableServices receivableService,
        IPayableServices payableService,
        IInventoryServices inventoryService,
        IOrderSalesServices salesOrderService,
        IOrderPurchaseServices purchaseOrderService,
        DbContextPersediaan inventoryContext)
    {
        _cashBankService = cashBankService;
        _receivableService = receivableService;
        _payableService = payableService;
        _inventoryService = inventoryService;
        _salesOrderService = salesOrderService;
        _purchaseOrderService = purchaseOrderService;
        _inventoryContext = inventoryContext;
    }

    public Task<ExecutiveDashboardView> GetDashboardAsync()
    {
        var today = DateTime.Today;
        var cutoff = new DateTime(today.Year, today.Month, 1).AddMonths(-11);
        var banks = _cashBankService.GetBank() ?? new();
        var receivables = _receivableService.GetAgingSchedule() ?? new();
        var payables = _payableService.GetAgingSchedule() ?? new();
        var stock = _inventoryService.GetCurrentStock() ?? new();
        var activeSalesOrders = _salesOrderService.GetTransHAktif() ?? new();
        var activePurchaseOrders = _purchaseOrderService.GetTransHAktif() ?? new();

        var inventoryValue = _inventoryContext.IcItems
            .AsNoTracking()
            .Where(x => x.Qty != 0)
            .Select(x => x.Qty * x.Cost)
            .AsEnumerable()
            .Sum();

        var trends = Enumerable.Range(0, 12)
            .Select(offset => cutoff.AddMonths(offset))
            .Select(month => new ExecutiveDashboardTrendView
            {
                Label = month.ToString("MMM yy"),
                SalesOrder = activeSalesOrders
                    .Where(x => x.Tanggal.Year == month.Year && x.Tanggal.Month == month.Month)
                    .Sum(ValueOf),
                PurchaseOrder = activePurchaseOrders
                    .Where(x => x.Tanggal.Year == month.Year && x.Tanggal.Month == month.Month)
                    .Sum(ValueOf)
            })
            .ToList();

        var result = new ExecutiveDashboardView
        {
            GeneratedAt = DateTime.Now,
            KasBank = banks.Sum(x => x.Saldo),
            Piutang = receivables.Sum(x => x.Sisa),
            Hutang = payables.Sum(x => x.Sisa * EffectiveRate(x.Kurs)),
            Persediaan = inventoryValue,
            SalesOrder = activeSalesOrders.Sum(ValueOf),
            PurchaseOrder = activePurchaseOrders.Sum(ValueOf),
            JumlahItemStock = stock.Count,
            JumlahSalesOrder = activeSalesOrders.Count,
            JumlahPurchaseOrder = activePurchaseOrders.Count,
            PiutangJatuhTempo = receivables.Sum(x => x.Jumlah2 + x.Jumlah3 + x.Jumlah4 + x.Jumlah5),
            HutangJatuhTempo = payables.Sum(x => (x.Jumlah2 + x.Jumlah3 + x.Jumlah4 + x.Jumlah5) * EffectiveRate(x.Kurs)),
            Trends = trends
        };

        return Task.FromResult(result);
    }

    private static decimal ValueOf(eSoft.Order.Model.PoTransH order)
    {
        var value = order.Jumlah != 0 ? order.Jumlah : order.TtlJumlah;
        return value * EffectiveRate(order.Kurs);
    }

    private static decimal EffectiveRate(decimal kurs) => kurs > 0 ? kurs : 1;
}
