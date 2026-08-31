using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eSoft.CashBank.Model;
using eSoft.Hutang.View;
using eSoft.Piutang.View;

namespace Accounting.Services
{
    public class DashboardSummaryDto
    {
        public List<CbBank> BankList { get; set; } = new();
        public decimal TotalSaldoBank { get; set; }
        public decimal TotalPiutang { get; set; }
        public decimal TotalHutangRp { get; set; }
        public int JumlahDokPiutang { get; set; }
        public int JumlahDokHutang { get; set; }
        public int JumlahDokValas { get; set; }
        public decimal ArCurrent { get; set; }
        public decimal Ar30 { get; set; }
        public decimal Ar60 { get; set; }
        public decimal Ar90 { get; set; }
        public decimal ArOver { get; set; }
        public decimal ArMax { get; set; }
        public decimal ApCurrent { get; set; }
        public decimal Ap30 { get; set; }
        public decimal Ap60 { get; set; }
        public decimal Ap90 { get; set; }
        public decimal ApOver { get; set; }
        public decimal ApMax { get; set; }
        public List<ArAgingView> AgingPiutang { get; set; } = new();
        public List<ApAgingView> AgingHutang { get; set; } = new();
        public DateTime LastLoadedAt { get; set; }
    }

    public interface IDashboardCacheService
    {
        Task<List<CbBank>> GetBankListAsync(bool forceRefresh = false, int? pageNumber = null, int? pageSize = null);
        Task<List<ArAgingView>> GetAgingPiutangAsync(bool forceRefresh = false);
        Task<List<ApAgingView>> GetAgingHutangAsync(bool forceRefresh = false);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(bool forceRefresh = false);
        void InvalidateBankCache();
        void InvalidateAgingPiutangCache();
        void InvalidateAgingHutangCache();
        void InvalidateDashboardSummaryCache();
        void InvalidateAllCache();
    }
}
