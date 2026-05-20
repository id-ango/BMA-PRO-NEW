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
        private readonly DbContextJual _context;
        private readonly DbContextPiutang _contextAr;
        private readonly ISalesmanMasterService _salesmanMasterService;
        private readonly IKurirMasterService _kurirMasterService;

        public SalesQueryService(
            DbContextJual context,
            DbContextPiutang contextPiutang,
            ISalesmanMasterService salesmanMasterService,
            IKurirMasterService kurirMasterService)
        {
            _context = context;
            _contextAr = contextPiutang;
            _salesmanMasterService = salesmanMasterService;
            _kurirMasterService = kurirMasterService;
        }

        public ArPiutng GetPiutang(string bukti)
        {
            return _contextAr.ArPiutngs.Where(x => x.Dokumen == bukti).FirstOrDefault();
        }

        public OeTransH GetOeTrans(int id)
        {
            return _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.OeTransHId == id).FirstOrDefault();
        }

        public OeTransH GetOeTransDokumen(string id)
        {
            return _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.NoLpb == id).FirstOrDefault();
        }

        public List<OeTransH> GetFirstTransH()
        {
            int hari = 30;
            DateTime startDate = DateTime.Now.AddDays(-hari);
            DateTime endDate = DateTime.Now.AddDays(hari);

            var transaksi = BuildTransHeaderQuery(startDate, endDate, true).ToList();
            ApplyDisplayNames(transaksi);

            return transaksi;
        }

        public List<OeTransH> GetFirstTransHNon()
        {
            int hari = 30;
            DateTime startDate = DateTime.Now.AddDays(-hari);
            DateTime endDate = DateTime.Now.AddDays(hari);

            var transaksi = BuildTransHeaderQuery(startDate, endDate, false).ToList();
            ApplyDisplayNames(transaksi);

            return transaksi;
        }

        public async Task<List<OeTransH>> GetTransH(DateTime tanggalAwal, DateTime tanggalAkhir)
        {
            var transaksi = await BuildTransHeaderQuery(tanggalAwal, tanggalAkhir, true).ToListAsync();
            ApplyDisplayNames(transaksi);

            return transaksi;
        }

        public async Task<List<OeTransH>> GetTransHNon(DateTime tanggalAwal, DateTime tanggalAkhir)
        {
            var transaksi = await BuildTransHeaderQuery(tanggalAwal, tanggalAkhir, false).ToListAsync();
            ApplyDisplayNames(transaksi);

            return transaksi;
        }

        public List<OeTransH> Get3TransH()
        {
            return _context.OeTransHs
                .OrderByDescending(x => x.Tanggal.Date)
                .Where(x => x.Tanggal.Date > DateTime.Today.Date.AddMonths(-3) && (x.Kode == "94" || x.Kode == "95"))
                .ToList();
        }

        public List<OeTransD> GetTransD()
        {
            return _context.OeTransDs.ToList();
        }

        public async Task<List<OeTransH>> GetTransKurirAsync()
        {
            DateTime date1 = new DateTime(2022, 4, 17, 0, 0, 0);

            var query = _context.OeTransHs
                .AsNoTracking()
                .Where(e => e.Kode == "94" && (e.Kurir == null || e.Kurir == "") && e.Tanggal > date1)
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
            var oeTransH = _context.OeTransHs.Find(transaksi.OeTransHId);

            if (oeTransH != null)
            {
                oeTransH.Kurir = transaksi.Kurir;
                oeTransH.Salesman = transaksi.Salesman;

                _context.SaveChanges();
            }
        }

        public void SimpanSalesman(OeTransH transaksi)
        {
            var oeTransH = _context.OeTransHs.Find(transaksi.OeTransHId);

            if (oeTransH != null)
            {
                oeTransH.Salesman = transaksi.Salesman;

                _context.SaveChanges();
            }
        }

        public List<OeTransD> GetOeTransDByDokumen(string dokumen)
        {
            return _context.OeTransDs
                .AsNoTracking()
                .Where(x => x.NoLpb == dokumen)
                .OrderBy(x => x.OeTransDId)
                .ToList();
        }

        private IQueryable<OeTransH> BuildTransHeaderQuery(DateTime tanggalAwal, DateTime tanggalAkhir, bool pajak)
        {
            return from e in _context.OeTransHs
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

        private void ApplyDisplayNames(List<OeTransH> transaksi)
        {
            transaksi.ForEach(x =>
            {
                x.Kurir = _kurirMasterService.GetKurirKode(string.IsNullOrEmpty(x.Kurir) ? "" : x.Kurir);
                x.Lokasi = _salesmanMasterService.GetSalesmanKode(string.IsNullOrEmpty(x.Salesman) ? "" : x.Salesman);
            });
        }
    }
}
