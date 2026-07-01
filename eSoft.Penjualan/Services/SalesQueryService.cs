using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Penjualan.Services
{
    public class SalesQueryService : ISalesQueryService
    {
        private readonly IDbContextFactory<DbContextJual> _context;
        private readonly IDbContextFactory<DbContextPiutang> _contextAr;

        public SalesQueryService(
            IDbContextFactory<DbContextJual> context,
            IDbContextFactory<DbContextPiutang> contextPiutang)
        {
            _context = context;
            _contextAr = contextPiutang;
        }

        public ArPiutng GetPiutang(string bukti)
        {
            using var contextAr = CreatePiutangContext();

            return contextAr.ArPiutngs
                .AsNoTracking()
                .FirstOrDefault(x => x.Dokumen == bukti);
        }

        public OeTransH GetOeTrans(int id)
        {
            using var context = CreateJualContext();

            return context.OeTransHs
                .AsNoTracking()
                .Include(p => p.OeTransDs)
                .FirstOrDefault(x => x.OeTransHId == id);
        }

        public OeTransH GetOeTransDokumen(string id)
        {
            using var context = CreateJualContext();

            return context.OeTransHs
                .AsNoTracking()
                .Include(p => p.OeTransDs)
                .FirstOrDefault(x => x.NoLpb == id);
        }

        public List<OeTransH> GetFirstTransH()
        {
            var (startDate, endDate) = GetDefaultDateRange();

            using var context = CreateJualContext();
            var transaksi = BuildTransHeaderQuery(context, startDate, endDate, true).ToList();
            ApplyDisplayNames(context, transaksi);

            return transaksi;
        }

        public List<OeTransH> GetFirstTransHNon()
        {
            var (startDate, endDate) = GetDefaultDateRange();

            using var context = CreateJualContext();
            var transaksi = BuildTransHeaderQuery(context, startDate, endDate, false).ToList();
            ApplyDisplayNames(context, transaksi);

            return transaksi;
        }

        public async Task<List<OeTransH>> GetTransH(DateTime tanggalAwal, DateTime tanggalAkhir)
        {
            using var context = CreateJualContext();
            var transaksi = await BuildTransHeaderQuery(context, tanggalAwal, tanggalAkhir, true).ToListAsync();
            ApplyDisplayNames(context, transaksi);

            return transaksi;
        }

        public async Task<List<OeTransH>> GetTransHNon(DateTime tanggalAwal, DateTime tanggalAkhir)
        {
            using var context = CreateJualContext();
            var transaksi = await BuildTransHeaderQuery(context, tanggalAwal, tanggalAkhir, false).ToListAsync();
            ApplyDisplayNames(context, transaksi);

            return transaksi;
        }

        public List<OeTransH> Get3TransH()
        {
            using var context = CreateJualContext();

            return context.OeTransHs
                .AsNoTracking()
                .Where(x => x.Tanggal > DateTime.Today.Date.AddMonths(-3) && (x.Kode == "94" || x.Kode == "95"))
                .OrderByDescending(x => x.Tanggal)
                .ToList();
        }

        public List<OeTransD> GetTransD()
        {
            using var context = CreateJualContext();

            return context.OeTransDs
                .AsNoTracking()
                .ToList();
        }

        public async Task<List<OeTransH>> GetTransKurirAsync()
        {
            using var context = CreateJualContext();
            DateTime date1 = new DateTime(2022, 4, 17, 0, 0, 0);

            var query = context.OeTransHs
                .AsNoTracking()
                .Where(e => e.Kode == "94" && string.IsNullOrEmpty(e.Kurir) && e.Tanggal > date1)
                .OrderByDescending(e => e.Tanggal)
                .Select(e => new OeTransH
                {
                    OeTransHId = e.OeTransHId,
                    NoLpb = e.NoLpb,
                    NoPrj = e.NoPrj,
                    Customer = e.Customer,
                    NamaCust = e.NamaCust,
                    AlamatKirim = e.AlamatKirim,
                    Tanggal = e.Tanggal,
                    Keterangan = e.Keterangan,
                    Salesman = e.Salesman,
                    Jumlah = e.Jumlah,
                    TtlJumlah = e.TtlJumlah,
                    Ongkos = e.Ongkos,
                    Ppn = e.Ppn,
                    PpnPersen = e.PpnPersen,
                    DPayment = e.DPayment,
                    Tagihan = e.Tagihan,
                    TotalQty = e.TotalQty,
                    Kode = e.Kode,
                    Cek = e.Cek,
                    Pajak = e.Pajak
                });

            return await query.ToListAsync();
        }

        public void SimpanKurir(OeTransH transaksi)
        {
            using var context = CreateJualContext();
            var oeTransH = context.OeTransHs.Find(transaksi.OeTransHId);

            if (oeTransH != null)
            {
                oeTransH.Kurir = transaksi.Kurir;
                oeTransH.Salesman = transaksi.Salesman;

                context.SaveChanges();
            }
        }

        public void SimpanSalesman(OeTransH transaksi)
        {
            using var context = CreateJualContext();
            var oeTransH = context.OeTransHs.Find(transaksi.OeTransHId);

            if (oeTransH != null)
            {
                oeTransH.Salesman = transaksi.Salesman;

                context.SaveChanges();
            }
        }

        public List<OeTransD> GetOeTransDByDokumen(string dokumen)
        {
            using var context = CreateJualContext();

            return context.OeTransDs
                .AsNoTracking()
                .Where(x => x.NoLpb == dokumen)
                .OrderBy(x => x.OeTransDId)
                .ToList();
        }

        private IQueryable<OeTransH> BuildTransHeaderQuery(DbContextJual context, DateTime tanggalAwal, DateTime tanggalAkhir, bool pajak)
        {
            return from e in context.OeTransHs.AsNoTracking()
                   orderby e.Tanggal descending
                   where (e.Kode == "94" || e.Kode == "95")
                       && e.Pajak == pajak
                       && e.Tanggal >= tanggalAwal
                       && e.Tanggal <= tanggalAkhir
                   select new OeTransH
                   {
                       OeTransHId = e.OeTransHId,
                       NoLpb = e.NoLpb,
                       Customer = e.Customer,
                       NamaCust = e.NamaCust,
                       AlamatKirim = e.AlamatKirim,
                       NoPrj = e.NoPrj,
                       Tanggal = e.Tanggal,
                       Keterangan = e.Keterangan,
                       Kurir = e.Kurir,
                       Salesman = e.Salesman,
                       Jumlah = e.Kode == "94" ? e.Jumlah : -1 * e.Jumlah,
                       TtlJumlah = e.Kode == "94" ? e.TtlJumlah : -1 * e.TtlJumlah,
                       Ongkos = e.Kode == "94" ? e.Ongkos : -1 * e.Ongkos,
                       Ppn = e.Kode == "94" ? e.Ppn : -1 * e.Ppn,
                       Discount = e.Discount,
                       PpnPersen = e.PpnPersen,
                       DPayment = e.DPayment,
                       Tagihan = e.Tagihan,
                       TotalQty = e.TotalQty,
                       Kode = e.Kode,
                       Cek = e.Cek,
                       Pajak = e.Pajak
                   };
        }

        private void ApplyDisplayNames(DbContextJual context, List<OeTransH> transaksi)
        {
            if (transaksi.Count == 0)
            {
                return;
            }

            var kurirCodes = transaksi
                .Select(x => x.Kurir)
                .Where(x => string.IsNullOrEmpty(x) == false)
                .Distinct()
                .ToList();

            var salesmanCodes = transaksi
                .Select(x => x.Salesman)
                .Where(x => string.IsNullOrEmpty(x) == false)
                .Distinct()
                .ToList();

            var kurirMap = context.OeKurirs
                .AsNoTracking()
                .Where(x => kurirCodes.Contains(x.Kurir))
                .ToDictionary(x => x.Kurir, x => x.NamaKurir);

            var salesmanMap = context.OeSalesmans
                .AsNoTracking()
                .Where(x => salesmanCodes.Contains(x.Salesman))
                .ToDictionary(x => x.Salesman, x => x.NamaSales);

            transaksi.ForEach(x =>
            {
                x.Kurir = string.IsNullOrEmpty(x.Kurir)
                    ? string.Empty
                    : kurirMap.GetValueOrDefault(x.Kurir, x.Kurir);

                x.Lokasi = string.IsNullOrEmpty(x.Salesman)
                    ? string.Empty
                    : salesmanMap.GetValueOrDefault(x.Salesman, x.Salesman);
            });
        }

        private (DateTime Start, DateTime End) GetDefaultDateRange()
        {
            const int hari = 30;
            var now = DateTime.Now;

            return (now.AddDays(-hari), now.AddDays(hari));
        }

        private DbContextJual CreateJualContext()
        {
            return _context.CreateDbContext();
        }

        private DbContextPiutang CreatePiutangContext()
        {
            return _contextAr.CreateDbContext();
        }
    }
}
