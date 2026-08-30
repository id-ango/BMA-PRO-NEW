using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eSoft.CashBank.Model;
using eSoft.CashBank.Services;
using eSoft.Hutang.Services;
using eSoft.Hutang.View;
using eSoft.Piutang.Services;
using eSoft.Piutang.View;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Accounting.Services
{
    public class DashboardCacheService : IDashboardCacheService
    {
        private const string CacheKeyBankList = "Dashboard_BankList";
        private const string CacheKeyAgingPiutang = "Dashboard_AgingPiutang";
        private const string CacheKeyAgingHutang = "Dashboard_AgingHutang";
        private const string CacheKeyDashboardSummary = "Dashboard_Summary";

        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly ICashBankServices _cashBankServices;
        private readonly IReceivableServices _receivableServices;
        private readonly IPayableServices _payableServices;

        public DashboardCacheService(
            IMemoryCache memoryCache,
            IConfiguration configuration,
            ICashBankServices cashBankServices,
            IReceivableServices receivableServices,
            IPayableServices payableServices)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cashBankServices = cashBankServices ?? throw new ArgumentNullException(nameof(cashBankServices));
            _receivableServices = receivableServices ?? throw new ArgumentNullException(nameof(receivableServices));
            _payableServices = payableServices ?? throw new ArgumentNullException(nameof(payableServices));
        }

        public async Task<List<CbBank>> GetBankListAsync(bool forceRefresh = false, int? pageNumber = null, int? pageSize = null)
        {
            var cacheKey = $"{CacheKeyBankList}_{pageNumber}_{pageSize}";
            if (!forceRefresh && _memoryCache.TryGetValue(cacheKey, out List<CbBank> cached))
            {
                return cached;
            }

            var result = await _cashBankServices.GetBankListAsync(pageNumber, pageSize) ?? new List<CbBank>();
            var minutes = _configuration.GetValue<int?>("DashboardCacheSettings:BankListCacheMinutes") ?? 30;
            _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(minutes));
            return result;
        }

        public async Task<List<ArAgingView>> GetAgingPiutangAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && _memoryCache.TryGetValue(CacheKeyAgingPiutang, out List<ArAgingView> cached))
            {
                return cached;
            }

            var result = await _receivableServices.GetAgingScheduleOptimizedAsync() ?? new List<ArAgingView>();
            var minutes = _configuration.GetValue<int?>("DashboardCacheSettings:AgingDataCacheMinutes") ?? 15;
            _memoryCache.Set(CacheKeyAgingPiutang, result, TimeSpan.FromMinutes(minutes));
            return result;
        }

        public async Task<List<ApAgingView>> GetAgingHutangAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && _memoryCache.TryGetValue(CacheKeyAgingHutang, out List<ApAgingView> cached))
            {
                return cached;
            }

            var result = await _payableServices.GetAgingScheduleOptimizedAsync() ?? new List<ApAgingView>();
            var minutes = _configuration.GetValue<int?>("DashboardCacheSettings:AgingDataCacheMinutes") ?? 15;
            _memoryCache.Set(CacheKeyAgingHutang, result, TimeSpan.FromMinutes(minutes));
            return result;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && _memoryCache.TryGetValue(CacheKeyDashboardSummary, out DashboardSummaryDto cachedSummary))
            {
                return cachedSummary;
            }

            var bankTask = GetBankListAsync(forceRefresh);
            var arTask = GetAgingPiutangAsync(forceRefresh);
            var apTask = GetAgingHutangAsync(forceRefresh);

            await Task.WhenAll(bankTask, arTask, apTask);

            var banks = await bankTask;
            var agingAr = await arTask;
            var agingAp = await apTask;

            var summary = new DashboardSummaryDto
            {
                BankList = banks,
                TotalSaldoBank = banks.Sum(x => x.Saldo),
                AgingPiutang = agingAr,
                TotalPiutang = agingAr.Sum(x => x.Sisa),
                JumlahDokPiutang = agingAr.Count,
                ArCurrent = agingAr.Sum(x => x.Jumlah1),
                Ar30 = agingAr.Sum(x => x.Jumlah2),
                Ar60 = agingAr.Sum(x => x.Jumlah3),
                Ar90 = agingAr.Sum(x => x.Jumlah4),
                ArOver = agingAr.Sum(x => x.Jumlah5),

                AgingHutang = agingAp,
                JumlahDokHutang = agingAp.Count,
                JumlahDokValas = agingAp.Count(x => x.Kurs > 1),
                TotalHutangRp = agingAp.Sum(x => x.Sisa * (x.Kurs > 0 ? x.Kurs : 1)),
                ApCurrent = agingAp.Sum(x => x.Jumlah1 * (x.Kurs > 0 ? x.Kurs : 1)),
                Ap30 = agingAp.Sum(x => x.Jumlah2 * (x.Kurs > 0 ? x.Kurs : 1)),
                Ap60 = agingAp.Sum(x => x.Jumlah3 * (x.Kurs > 0 ? x.Kurs : 1)),
                Ap90 = agingAp.Sum(x => x.Jumlah4 * (x.Kurs > 0 ? x.Kurs : 1)),
                ApOver = agingAp.Sum(x => x.Jumlah5 * (x.Kurs > 0 ? x.Kurs : 1)),
                LastLoadedAt = DateTime.Now
            };

            summary.ArMax = new[] { summary.ArCurrent, summary.Ar30, summary.Ar60, summary.Ar90, summary.ArOver }.DefaultIfEmpty().Max();
            summary.ApMax = new[] { summary.ApCurrent, summary.Ap30, summary.Ap60, summary.Ap90, summary.ApOver }.DefaultIfEmpty().Max();

            var minutes = _configuration.GetValue<int?>("DashboardCacheSettings:DashboardSummaryCacheMinutes") ?? 10;
            _memoryCache.Set(CacheKeyDashboardSummary, summary, TimeSpan.FromMinutes(minutes));

            return summary;
        }

        public void InvalidateBankCache()
        {
            _memoryCache.Remove($"{CacheKeyBankList}_null_null");
            _memoryCache.Remove(CacheKeyDashboardSummary);
        }

        public void InvalidateAgingPiutangCache()
        {
            _memoryCache.Remove(CacheKeyAgingPiutang);
            _memoryCache.Remove(CacheKeyDashboardSummary);
        }

        public void InvalidateAgingHutangCache()
        {
            _memoryCache.Remove(CacheKeyAgingHutang);
            _memoryCache.Remove(CacheKeyDashboardSummary);
        }

        public void InvalidateDashboardSummaryCache()
        {
            _memoryCache.Remove(CacheKeyDashboardSummary);
        }

        public void InvalidateAllCache()
        {
            InvalidateBankCache();
            InvalidateAgingPiutangCache();
            InvalidateAgingHutangCache();
            InvalidateDashboardSummaryCache();
        }
    }
}
