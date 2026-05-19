using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace eSoft.Penjualan.Services
{
    public class SalesServices : ISalesServices
    {
        private readonly DbContextJual _context;
        private readonly DbContextPiutang _contextAr;
        private readonly DbContextPersediaan _contextIc;

        public SalesServices(DbContextJual context, DbContextPiutang contextPiutang, DbContextPersediaan contextPersediaan)
        {
            _context = context;
            _contextAr = contextPiutang;
            _contextIc = contextPersediaan;
        }

        #region laporanpenjualan

        public List<OeTransH> Laporan1(DateTime tgl1, DateTime tgl2)
        {
            List<OeTransH> transH = new();

            transH = _context.OeTransHs.Where(x => x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date).OrderByDescending(t => t.Tanggal.Date)
                .Select(
                x => new OeTransH
                {
                    OeTransHId = x.OeTransHId,
                    Kode = x.Kode,
                    Tanggal = x.Tanggal,
                    NoLpb = x.NoLpb,
                    Kurir = x.Kurir,
                    Salesman = x.Salesman,
                    AlamatKirim = x.AlamatKirim,
                    NamaCust = x.NamaCust,
                    Customer = x.Customer,
                    Keterangan = x.Keterangan,
                    TtlJumlah = (x.Kode == "94" ? x.TtlJumlah : -1 * x.TtlJumlah),
                    Ppn = (x.Kode == "94" ? x.Ppn : -1 * x.Ppn),
                    Ongkos = (x.Kode == "94" ? x.Ongkos : -1 * x.Ongkos),
                    Jumlah = (x.Kode == "94" ? x.Jumlah : -1 * x.Jumlah)
                }
                ).
                ToList();

            //foreach (var item in transH)
            //{

            //    item.Jumlah = (item.Kode == "94" ? item.Jumlah : -1 * item.Jumlah);
            //    item.TtlJumlah = (item.Kode == "94" ? item.TtlJumlah : -1 * item.TtlJumlah);
            //    item.Ongkos = (item.Kode == "94" ? item.Ongkos : -1 * item.Ongkos);
            //    item.Ppn = (item.Kode == "94" ? item.Ppn : -1 * item.Ppn);
            //}

            return transH;
        }

        public List<OeTransD> LaporanDownload(DateTime tgl1, DateTime tgl2)
        {
            List<OeTransD> transD = new();


            transD = _context.OeTransDs.Where(x => x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date && x.Kode == "94" && x.Jumlah != 0).OrderBy(t => t.Tanggal.Date)
                .Select(
                x => new OeTransD
                {
                    OeTransHId = x.OeTransHId,
                    Kode = x.Kode,
                    Tanggal = x.Tanggal,
                    NoLpb = x.NoLpb,
                    NamaItem = x.NamaItem,
                    ItemCode = x.ItemCode,
                    Qty = x.Qty,
                    Jumlah = (x.Kode == "94" ? x.Jumlah : -1 * x.Jumlah)
                }
                ).
                ToList();



            return transD;
        }
        public List<OeTransD> Detail1(int xKdHeader)
        {
            List<OeTransD> transD = new();

            transD = _context.OeTransDs.Where(x => x.OeTransHId == xKdHeader).ToList();

            return transD;
        }

        public List<OeTrans> Detail2(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            List<OeTransH> transH = new();
            List<OeTransD> transD = new();
            List<OeTrans> trans = new();

            transH = _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date).ToList();
            transD = _context.OeTransDs.Where(x => x.ItemCode == xKdHeader).ToList();

            if (transH != null && transD != null)
            {
                trans = (from header in transH
                         join detail in transD on header.OeTransHId equals detail.OeTransHId
                         select new OeTrans()
                         {
                             ItemCode = detail.ItemCode,
                             NamaItem = detail.NamaItem,
                             Harga = detail.Harga,
                             Persen = detail.Persen,
                             Discount = detail.Discount,
                             Satuan = detail.Satuan,
                             Ppn = header.Ppn,
                             PpnPersen = header.PpnPersen,
                             Ongkos = header.Ongkos,
                             Qty = detail.Qty,
                             Jumlah = detail.Jumlah,
                             Lokasi = detail.Lokasi,
                             NoLpb = header.NoLpb,
                             Customer = header.Customer,
                             NamaCust = header.NamaCust,
                             Tanggal = header.Tanggal,
                             Keterangan = header.Keterangan,
                             AlamatKirim = header.AlamatKirim
                         }).ToList();
            }




            return trans;
        }

        public List<OeTrans> Detail3(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            List<OeTransH> transH = new();
            List<OeTransD> transD = new();
            List<OeTrans> trans = new List<OeTrans>();

            transH = _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Customer == xKdHeader && (x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date)).ToList();
            transD = _context.OeTransDs.ToList();

            if (transH != null && transD != null)
            {

                trans = (from header in transH
                         join detail in transD on header.OeTransHId equals detail.OeTransHId
                         select new OeTrans()
                         {
                             ItemCode = detail.ItemCode,
                             NamaItem = detail.NamaItem,
                             Harga = detail.Harga,
                             Persen = detail.Persen,
                             Discount = detail.Discount,
                             Satuan = detail.Satuan,
                             Ppn = header.Ppn,
                             PpnPersen = header.PpnPersen,
                             Ongkos = header.Ongkos,
                             Qty = detail.Qty,
                             Jumlah = detail.Jumlah,
                             Lokasi = detail.Lokasi,
                             NoLpb = header.NoLpb,
                             Customer = header.Customer,
                             NamaCust = header.NamaCust,
                             Tanggal = header.Tanggal,
                             Keterangan = header.Keterangan,
                             AlamatKirim = header.AlamatKirim
                         }).ToList();
            }




            return trans;
        }

        public List<OeTrans> Detail3Index(string xKdHeader)
        {
            List<OeTransH> transH = new List<OeTransH>();
            List<OeTransD> transD = new List<OeTransD>();
            List<OeTrans> trans = new List<OeTrans>();

            transH = _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Customer == xKdHeader).ToList();
            transD = _context.OeTransDs.ToList();

            if (transH != null && transD != null)
            {

                trans = (from header in transH
                         join detail in transD on header.OeTransHId equals detail.OeTransHId
                         select new OeTrans()
                         {
                             ItemCode = detail.ItemCode,
                             NamaItem = detail.NamaItem,
                             Harga = detail.Harga,
                             Persen = detail.Persen,
                             Discount = detail.Discount,
                             Satuan = detail.Satuan,
                             Ppn = header.Ppn,
                             PpnPersen = header.PpnPersen,
                             Ongkos = header.Ongkos,
                             Qty = detail.Qty,
                             Jumlah = detail.Jumlah,
                             Lokasi = detail.Lokasi,
                             NoLpb = header.NoLpb,
                             Customer = header.Customer,
                             NamaCust = header.NamaCust,
                             Tanggal = header.Tanggal,
                             Keterangan = header.Keterangan,
                             AlamatKirim = header.AlamatKirim
                         }).ToList();
            }




            return trans;
        }
        public List<OeTrans> Detail4(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            List<OeTransH> transH = new List<OeTransH>();
            List<OeTransD> transD = new List<OeTransD>();
            List<OeTrans> trans = new List<OeTrans>();

            transH = _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Kurir == xKdHeader && (x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date)).ToList();
            transD = _context.OeTransDs.ToList();

            if (transH != null && transD != null)
            {
                trans = (from header in transH

                         select new OeTrans()
                         {

                             NoLpb = header.NoLpb,
                             Customer = header.Customer,
                             NamaCust = header.NamaCust,
                             Tanggal = header.Tanggal,
                             AlamatKirim = header.AlamatKirim
                         }).ToList();
            }




            return trans;
        }

        public List<OeTrans> Detail5(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            List<OeTransH> transH = new List<OeTransH>();
            List<OeTransD> transD = new List<OeTransD>();
            List<OeTrans> trans = new List<OeTrans>();

            transH = _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Salesman == xKdHeader && (x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date)).ToList();
            transD = _context.OeTransDs.ToList();

            if (transH != null && transD != null)
            {

                trans = (from header in transH

                         select new OeTrans()
                         {

                             NoLpb = header.NoLpb,
                             Customer = header.Customer,
                             NamaCust = header.NamaCust,
                             Tanggal = header.Tanggal,
                             AlamatKirim = header.AlamatKirim,
                             Jumlah = header.Jumlah
                         }).ToList();
            }




            return trans;
        }
        #endregion

        #region Salesman

        public List<OeSalesman> GetSalesman()
        {
            return _context.OeSalesmans.ToList();
        }
        public OeSalesman GetSalesmanId(int id)
        {
            return _context.OeSalesmans.Where(x => x.OeSalesmanId == id).FirstOrDefault();
        }
        public string GetSalesmanKode(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            return _context.OeSalesmans.Where(x => x.Salesman == id).FirstOrDefault().NamaSales;
        }
        public async Task<bool> DelSalesman(int kurirs)
        {
            try
            {
                var Existingkurir = _context.OeSalesmans.Where(x => x.OeSalesmanId == kurirs).FirstOrDefault();
                if (Existingkurir != null)
                {
                    _context.OeSalesmans.Remove(Existingkurir);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public bool CekKdSalesman(string kurir)
        {
            string test = kurir.ToUpper();
            var cekFirst = _context.OeSalesmans.Where(x => x.Salesman == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool AddSalesman(OeSalesmanView customers)
        {
            string test = customers.Salesman.ToUpper();
            var cekFirst = _context.OeSalesmans.Where(x => x.Salesman == test).ToList();
            if (cekFirst.Count == 0)
            {
                OeSalesman Courier = new()
                {
                    Salesman = customers.Salesman.ToUpper(),
                    NamaSales = customers.NamaSales,
                    Termin = customers.Termin,
                    Alamat = customers.Alamat,
                    Kota = customers.Kota,
                    Telpon = customers.Telpon,
                    NamaLengkap = customers.NamaLengkap,
                    AcctSet = customers.AcctSet,
                    //AcctPjk = customers.AcctPjk,
                    AlmtKrm = customers.AlmtKrm,
                    KotaKrm = customers.KotaKrm,
                    //ProvKirim = customers.ProvKirim,
                    NPWP_Sales = customers.NPWP_Sales,
                    Kontak = customers.Kontak



                };
                _context.OeSalesmans.Add(Courier);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditSalesman(OeSalesmanView customers)
        {
            try
            {
                var ExistingCustomer = _context.OeSalesmans.Where(x => x.OeSalesmanId == customers.OeSalesmanId).FirstOrDefault();
                if (ExistingCustomer != null)
                {
                    ExistingCustomer.NamaSales = customers.NamaSales;
                    ExistingCustomer.Alamat = customers.Alamat;
                    ExistingCustomer.Kota = customers.Kota;
                    ExistingCustomer.Telpon = customers.Telpon;
                    ExistingCustomer.Termin = customers.Termin;
                    ExistingCustomer.NamaLengkap = customers.NamaLengkap;
                    ExistingCustomer.AcctSet = customers.AcctSet;
                    ExistingCustomer.AcctPjk = customers.AcctPjk;
                    ExistingCustomer.AlmtKrm = customers.AlmtKrm;
                    ExistingCustomer.KotaKrm = customers.KotaKrm;
                    ExistingCustomer.ProvKirim = customers.ProvKirim;
                    ExistingCustomer.Kontak = customers.Kontak;
                    ExistingCustomer.NPWP_Sales = customers.NPWP_Sales;

                    _context.OeSalesmans.Update(ExistingCustomer);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }
        #endregion


        #region Kurir

        public List<OeKurir> GetKurir()
        {
            return _context.OeKurirs.ToList();
        }

        public OeKurir GetKurirId(int id)
        {
            return _context.OeKurirs.Where(x => x.OeKurirId == id).FirstOrDefault();
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        public string GetKurirKode(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;

            return _context.OeKurirs.Where(x => x.Kurir == id).FirstOrDefault().NamaKurir;
        }

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        public bool CekKdKurir(string kurir)
        {
            string test = kurir.ToUpper();
            var cekFirst = _context.OeKurirs.Where(x => x.Kurir == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool AddKurir(OeKurirView customers)
        {
            string test = customers.Kurir.ToUpper();
            var cekFirst = _context.OeKurirs.Where(x => x.Kurir == test).ToList();
            if (cekFirst.Count == 0)
            {
                OeKurir Courier = new()
                {
                    Kurir = customers.Kurir.ToUpper(),
                    NamaKurir = customers.NamaKurir,
                    Termin = customers.Termin,
                    Alamat = customers.Alamat,
                    Kota = customers.Kota,
                    Telpon = customers.Telpon,
                    NamaLengkap = customers.NamaLengkap,
                    AcctSet = customers.AcctSet,
                    //AcctPjk = customers.AcctPjk,
                    AlmtKrm = customers.AlmtKrm,
                    KotaKrm = customers.KotaKrm,
                    //ProvKirim = customers.ProvKirim,
                    NPWP_Kurir = customers.NPWP_Kurir,
                    Kontak = customers.Kontak



                };
                _context.OeKurirs.Add(Courier);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditKurir(OeKurirView customers)
        {
            try
            {
                var ExistingCustomer = _context.OeKurirs.Where(x => x.OeKurirId == customers.OeKurirId).FirstOrDefault();
                if (ExistingCustomer != null)
                {
                    ExistingCustomer.NamaKurir = customers.NamaKurir;
                    ExistingCustomer.Alamat = customers.Alamat;
                    ExistingCustomer.Kota = customers.Kota;
                    ExistingCustomer.Telpon = customers.Telpon;
                    ExistingCustomer.Termin = customers.Termin;
                    ExistingCustomer.NamaLengkap = customers.NamaLengkap;
                    ExistingCustomer.AcctSet = customers.AcctSet;
                    ExistingCustomer.AcctPjk = customers.AcctPjk;
                    ExistingCustomer.AlmtKrm = customers.AlmtKrm;
                    ExistingCustomer.KotaKrm = customers.KotaKrm;
                    ExistingCustomer.ProvKirim = customers.ProvKirim;
                    ExistingCustomer.Kontak = customers.Kontak;
                    ExistingCustomer.NPWP_Kurir = customers.NPWP_Kurir;

                    _context.OeKurirs.Update(ExistingCustomer);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }
        public async Task<bool> DelKurir(int kurirs)
        {
            try
            {
                var Existingkurir = _context.OeKurirs.Where(x => x.OeKurirId == kurirs).FirstOrDefault();
                if (Existingkurir != null)
                {
                    _context.OeKurirs.Remove(Existingkurir);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        #endregion

        #region getclass

        private ArCust GetCustomerId(string id)
        {
            return _contextAr.ArCusts.Where(x => x.Customer == id).FirstOrDefault();
        }

        public ArPiutng GetPiutang(string bukti)
        {
            return _contextAr.ArPiutngs.Where(x => x.Dokumen == bukti).FirstOrDefault();

        }

        #endregion getclass

        #region OeTransH class

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
            IQueryable<OeTransH> OeTrans;
            int hari = 30;
            DateTime startDate = DateTime.Now.AddDays(-hari);
            DateTime endDate = DateTime.Now.AddDays(hari); ;



            try
            {
                OeTrans = (from e in _context.OeTransHs
                           orderby e.Tanggal descending
                           where (((e.Kode == "94" || e.Kode == "95") && e.Pajak == true) && (e.Tanggal >= startDate && e.Tanggal <= endDate))
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
                               Jumlah = (e.Kode == "94" ? e.Jumlah : -1 * e.Jumlah),
                               TtlJumlah = (e.Kode == "94" ? e.TtlJumlah : -1 * e.TtlJumlah),
                               Ongkos = (e.Kode == "94" ? e.Ongkos : -1 * e.Ongkos),
                               Ppn = (e.Kode == "94" ? e.Ppn : -1 * e.Ppn),
                               Discount = e.Discount,
                               PpnPersen = e.PpnPersen,

                               DPayment = e.DPayment,
                               Tagihan = e.Tagihan,
                               TotalQty = e.TotalQty,
                               Kode = e.Kode,
                               Cek = e.Cek,
                               Pajak = e.Pajak


                           });



            }
            catch (Exception)
            {
                throw;
            }
            var transaksi = OeTrans.ToList();

            transaksi
                .ForEach
                (
                    x =>
                    {
                        x.Kurir = GetKurirKode(string.IsNullOrEmpty(x.Kurir) ? "" : x.Kurir);
                        x.Lokasi = GetSalesmanKode(string.IsNullOrEmpty(x.Salesman) ? "" : x.Salesman);
                        //  x.Salesman = GetSalesmanKode(string.IsNullOrEmpty(x.Salesman) ? "" : x.Salesman);
                    }
                 );

            return transaksi;

        }

        public List<OeTransH> GetFirstTransHNon()
        {

            IQueryable<OeTransH> OeTrans;
            int hari = 30;
            DateTime startDate = DateTime.Now.AddDays(-hari);
            DateTime endDate = DateTime.Now.AddDays(hari); ;


            try
            {


                OeTrans = (from e in _context.OeTransHs
                           orderby e.Tanggal descending
                           where (((e.Kode == "94" || e.Kode == "95") && e.Pajak == false) && (e.Tanggal >= startDate && e.Tanggal <= endDate))
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
                               Jumlah = (e.Kode == "94" ? e.Jumlah : -1 * e.Jumlah),
                               TtlJumlah = (e.Kode == "94" ? e.TtlJumlah : -1 * e.TtlJumlah),
                               Ongkos = (e.Kode == "94" ? e.Ongkos : -1 * e.Ongkos),
                               Ppn = (e.Kode == "94" ? e.Ppn : -1 * e.Ppn),
                               Discount = e.Discount,
                               PpnPersen = e.PpnPersen,

                               DPayment = e.DPayment,
                               Tagihan = e.Tagihan,
                               TotalQty = e.TotalQty,
                               Kode = e.Kode,
                               Cek = e.Cek,
                               Pajak = e.Pajak,



                           });



            }
            catch (Exception)
            {
                throw;
            }
            var transaksi = OeTrans.ToList();
            transaksi
                .ForEach
                (
                    x =>
                    {
                        x.Kurir = GetKurirKode(string.IsNullOrEmpty(x.Kurir) ? "" : x.Kurir);
                        x.Lokasi = GetSalesmanKode(string.IsNullOrEmpty(x.Salesman) ? "" : x.Salesman);
                    }
                 );

            return transaksi;

        }

        public async Task<List<OeTransH>> GetTransH(DateTime tanggalAwal, DateTime tanggalAkhir)
        {


            IQueryable<OeTransH> OeTrans;

            try
            {
                OeTrans = (from e in _context.OeTransHs
                           orderby e.Tanggal descending
                           where
                           ((e.Kode == "94" || e.Kode == "95")
                               && e.Pajak == true
                               && e.Tanggal >= tanggalAwal
                               && e.Tanggal <= tanggalAkhir)
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
                               Jumlah = (e.Kode == "94" ? e.Jumlah : -1 * e.Jumlah),
                               TtlJumlah = (e.Kode == "94" ? e.TtlJumlah : -1 * e.TtlJumlah),
                               Ongkos = (e.Kode == "94" ? e.Ongkos : -1 * e.Ongkos),
                               Ppn = (e.Kode == "94" ? e.Ppn : -1 * e.Ppn),
                               Discount = e.Discount,
                               PpnPersen = e.PpnPersen,

                               DPayment = e.DPayment,
                               Tagihan = e.Tagihan,
                               TotalQty = e.TotalQty,
                               Kode = e.Kode,
                               Cek = e.Cek,
                               Pajak = e.Pajak


                           });



            }
            catch (Exception)
            {
                throw;
            }

            var transaksi = await OeTrans.ToListAsync();


            transaksi
                 .ForEach
                 (
                     x =>
                     {
                         x.Kurir = GetKurirKode(string.IsNullOrEmpty(x.Kurir) ? "" : x.Kurir);
                         x.Lokasi = GetSalesmanKode(string.IsNullOrEmpty(x.Salesman) ? "" : x.Salesman);
                     }
                  );



            return transaksi;


        }

        public async Task<List<OeTransH>> GetTransHNon(DateTime tanggalAwal, DateTime tanggalAkhir)
        {

            IQueryable<OeTransH> OeTrans;


            try
            {


                OeTrans = (from e in _context.OeTransHs
                           orderby e.Tanggal descending
                           where
                           ((e.Kode == "94" || e.Kode == "95")
                               && e.Pajak == false
                               && e.Tanggal >= tanggalAwal
                               && e.Tanggal <= tanggalAkhir)
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
                               Jumlah = (e.Kode == "94" ? e.Jumlah : -1 * e.Jumlah),
                               TtlJumlah = (e.Kode == "94" ? e.TtlJumlah : -1 * e.TtlJumlah),
                               Ongkos = (e.Kode == "94" ? e.Ongkos : -1 * e.Ongkos),
                               Ppn = (e.Kode == "94" ? e.Ppn : -1 * e.Ppn),
                               Discount = e.Discount,
                               PpnPersen = e.PpnPersen,

                               DPayment = e.DPayment,
                               Tagihan = e.Tagihan,
                               TotalQty = e.TotalQty,
                               Kode = e.Kode,
                               Cek = e.Cek,
                               Pajak = e.Pajak


                           });



            }
            catch (Exception)
            {
                throw;
            }
            var transaksi = await OeTrans.ToListAsync();


            transaksi
                .ForEach
                (
                    x =>
                       {
                           x.Kurir = GetKurirKode(string.IsNullOrEmpty(x.Kurir) ? "" : x.Kurir);
                           x.Lokasi = GetSalesmanKode(string.IsNullOrEmpty(x.Salesman) ? "" : x.Salesman);
                       }
                 );

            return transaksi;

        }

        public List<OeTransH> Get3TransH()
        {
            List<OeTransH> OeTrans = new List<OeTransH>();

            OeTrans = _context.OeTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Tanggal.Date > DateTime.Today.Date.AddMonths(-3) && (x.Kode == "94" || x.Kode == "95")).ToList();

            return OeTrans;

            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //   return _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3)).ToListAsync();

        }

        public List<OeTransD> GetTransD()
        {
            return _context.OeTransDs.ToList();
        }

        public OeTransH AddTransH(OeTransHView trans, bool pajak)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            decimal mQty5 = 0;

            OeTransH transH = new OeTransH
            {

                NoLpb = (pajak ? GetNumberTax() : GetNumber()),
                Customer = trans.Customer.ToUpper(),
                NamaCust = trans.NamaCust,
                AlamatKirim = trans.AlamatKirim,
                Tanggal = trans.Tanggal,
                JthTempo = trans.JthTempo,
                Keterangan = trans.Keterangan,
                NoPrj = trans.NoPrj,
                Salesman = trans.Salesman,
                Jumlah = trans.Jumlah,
                Ongkos = trans.Ongkos,
                Ppn = trans.Ppn,
                Discount = trans.Discount,
                PpnPersen = trans.PpnPersen,
                TtlJumlah = trans.TtlJumlah,
                DPayment = trans.DPayment,
                Tagihan = trans.Tagihan,
                TotalQty = trans.TotalQty,
                Kode = "94",
                Cek = (trans.NonPiutang ? "" : "1"),
                Pajak = pajak,
                OeTransDs = new List<OeTransD>()
            };

            foreach (var item in trans.OeTransDs)
            {
                if (item.Qty != 0)
                {
                    if (transH.TotalQty != 0)
                    {
                        mQty5 = (item.Jumlah - item.Discount) - (item.Qty / transH.TotalQty * transH.Ppn) + (item.Qty / transH.TotalQty * transH.Ongkos);
                    }
                    else
                    {
                        mQty5 = (item.Jumlah - item.Discount);
                    }

                    transH.OeTransDs.Add(new OeTransD()
                    {
                        ItemCode = item.ItemCode.ToUpper(),
                        NamaItem = item.NamaItem,
                        Satuan = item.Satuan,
                        Lokasi = item.Lokasi,
                        Harga = item.Harga,
                        Qty = item.Qty,
                        Persen = item.Persen,
                        Discount = item.Discount,
                        Jumlah = item.Jumlah,
                        Kode = "94",
                        NoLpb = transH.NoLpb,
                        Tanggal = trans.Tanggal,
                        HrgCost = item.HrgCost,
                        Cost = item.HrgCost * item.Qty,
                        JumDpp = mQty5
                    });

                    IcItem cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();

                    if (cekItem != null)
                    {
                        #region altitem

                        IcAltItem cekLokasi1 = _contextIc.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                        if (cekLokasi1 == null)
                        {
                            IcAltItem Produk = new IcAltItem()
                            {
                                ItemCode = cekItem.ItemCode.ToUpper(),
                                NamaItem = cekItem.NamaItem,
                                Satuan = cekItem.Satuan,
                                Lokasi = item.Lokasi,
                                Qty = -1 * item.Qty
                            };
                            _contextIc.IcAltItems.Add(Produk);

                        }
                        else
                        {
                            cekLokasi1.Qty -= item.Qty;
                            _contextIc.IcAltItems.Update(cekLokasi1);
                        }

                        #endregion altitem
                        //  if (item.Harga != 0)
                        //      cekItem.HrgJual = item.Harga;  // harga jual barang

                        if (item.Harga > 0 && item.Harga > cekItem.HrgJual)
                            cekItem.HrgJual = item.Harga;  // harga jual barang

                        if (cekItem.JnsBrng == (int)jnsBrng.Stock)   // jika stock
                        {
                            cekItem.Qty -= item.Qty;
                        }

                        if (cekItem.CostMethod == (int)costMethod.Moving_Avg)  // jika moving avarage
                        {

                            cekItem.Cost -= (item.HrgCost * item.Qty);
                        }
                        else
                        {
                            cekItem.Cost -= (cekItem.StdPrice * item.Qty);
                        }

                        //if (cekItem.Qty != 0)
                        //{
                        //    cekItem.HrgNetto = cekItem.Cost / cekItem.Qty;
                        //}
                        //else
                        //{
                        //    cekItem.HrgNetto = cekItem.Harga;
                        //}

                        _contextIc.IcItems.Update(cekItem);

                    }
                }
                _context.OeTransHs.Add(transH);
            }
            var Customer = GetCustomerId(transH.Customer);

            if (!trans.NonPiutang)      // jika NonPiutang false maka Piutang ditambahkan jika tidak maka hanya persediaan saja yang diproses
            {
                ArPiutng piutang = new ArPiutng
                {
                    Kode = "OE",
                    Dokumen = transH.NoLpb,
                    Tanggal = transH.Tanggal,
                    Salesman = transH.Salesman,
                    DueDate = transH.JthTempo,
                    Customer = transH.Customer,
                    Keterangan = transH.Keterangan,
                    Jumlah = transH.Jumlah,
                    Sisa = transH.Jumlah,
                    SldSisa = transH.Jumlah,
                    KodeTran = transH.Kode
                };
                _contextAr.ArPiutngs.Add(piutang);

                //   DueDate = transH.Tanggal.AddDays(Customer.Termin),

                Customer.Piutang += transH.Jumlah;

                _contextAr.ArCusts.Update(Customer);
            }
            _context.SaveChanges();
            _contextAr.SaveChanges();
            _contextIc.SaveChanges();

            var TempTrans = GetTransDoc(transH.NoLpb);

            return TempTrans;

        }

        public OeTransH GetTransDoc(string docno)
        {
            return _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.NoLpb == docno).FirstOrDefault();
        }

        public async Task<bool> DelTransH(int id)
        {
            string cKode = "94";

            try
            {
                var ExistingTrans = _context.OeTransHs.Include(y => y.OeTransDs).Where(x => x.OeTransHId == id).FirstOrDefault();

                if (ExistingTrans != null)
                {
                    cKode = ExistingTrans.Kode;

                    foreach (var item in ExistingTrans.OeTransDs)
                    {
                        if (item.Qty != 0)
                        {

                            IcItem cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                            if (cekItem != null)
                            {
                                IcAltItem cekLokasi1 = _contextIc.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                                if (cekLokasi1 == null)
                                {
                                    IcAltItem Produk = new IcAltItem()
                                    {
                                        ItemCode = cekItem.ItemCode.ToUpper(),
                                        NamaItem = cekItem.NamaItem,
                                        Satuan = cekItem.Satuan,
                                        Lokasi = item.Lokasi,
                                        Qty = (cKode == "95" ? -1 * item.Qty : item.Qty)

                                    };
                                    _contextIc.IcAltItems.Add(Produk);

                                }
                                else
                                {
                                    if (cKode == "95")
                                        cekLokasi1.Qty -= item.Qty;
                                    else
                                        cekLokasi1.Qty += item.Qty;

                                    //  cekLokasi1.Qty += item.Qty;
                                    _contextIc.IcAltItems.Update(cekLokasi1);
                                }
                                //   cekItem.Qty -= item.Qty;
                                //   cekItem.Cost -= item.JumDpp;
                                if (item.Harga > 0 && item.Harga > cekItem.HrgJual)
                                    cekItem.HrgJual = item.Harga;  // harga jual barang

                                if (cekItem.JnsBrng == (int)jnsBrng.Stock)   // jika stock
                                {
                                    if (cKode == "95")
                                        cekItem.Qty -= item.Qty;
                                    else
                                        cekItem.Qty += item.Qty;


                                    //    cekItem.Qty += item.Qty;
                                }

                                if (cekItem.CostMethod == (int)costMethod.Moving_Avg)  // jika moving avarage
                                {
                                    if (cKode == "95")
                                        cekItem.Cost -= item.Cost;
                                    else
                                        cekItem.Cost += item.Cost;

                                }
                                else
                                {
                                    //if (cKode == "95")
                                    //    cekItem.Cost -= (cekItem.StdPrice * cekItem.Qty);
                                    //else
                                    //    cekItem.Cost += (cekItem.StdPrice * cekItem.Qty);

                                    cekItem.Cost = (cekItem.StdPrice * cekItem.Qty);
                                }

                                //if (cekItem.Qty != 0)
                                //{
                                //    cekItem.HrgNetto = cekItem.Cost / cekItem.Qty;
                                //}
                                //else
                                //{
                                //    cekItem.HrgNetto = cekItem.Harga;
                                //}

                                _contextIc.IcItems.Update(cekItem);

                            }
                        }

                    }
                    if (ExistingTrans.Cek == "1")   // jika Piutang maka cari customer dan piutang
                    {
                        var Customer = GetCustomerId(ExistingTrans.Customer);
                        var piutang = GetPiutang(ExistingTrans.NoLpb);
                        Customer.Piutang -= ExistingTrans.Jumlah;
                        _contextAr.ArCusts.Update(Customer);
                        _contextAr.ArPiutngs.Remove(piutang);
                    }


                    _context.OeTransHs.Remove(ExistingTrans);
                    await _context.SaveChangesAsync();
                    await _contextAr.SaveChangesAsync();
                    await _contextIc.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        public bool CekPiutang(OeTransH trans)
        {

            var cekFirst = _contextAr.ArPiutngs.Where(x => x.Dokumen == trans.NoLpb && x.Sisa == 0).FirstOrDefault();

            if (cekFirst == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool EditTransH(OeTransHView trans)
        {
            decimal mQty5 = 0;
            string cKode = "94";

            cKode = trans.Kode;

            // Ambil data header asli dari database (belum diedit)
            var ExistingTrans = _context.OeTransHs
                .Include(h => h.OeTransDs) // Include detail (child records)
                .FirstOrDefault(x => x.NoLpb == trans.NoLpb);

            if (ExistingTrans == null)
            {
                Console.WriteLine("Data transaksi tidak ditemukan.");
                return false;
            }

            // Pengecekan pelunasan ArPiutng jika cek == 1
            if (string.IsNullOrEmpty(ExistingTrans.Cek) == false)  // berarti cek == 1
            {

                var pelunasan = _contextAr.ArPiutngs
                    .Where(x => x.Dokumen == ExistingTrans.NoLpb && x.Bayar > 0)
                    .FirstOrDefault();

                if (pelunasan != null)
                {
                    Console.WriteLine("Transaksi tidak dapat diubah/hapus karena sudah ada pelunasan.");
                    return false;
                }
            }

            var cekFirst = _contextAr.ArPiutngs.Where(x => x.Dokumen == trans.NoLpb && x.Bayar == 0).FirstOrDefault();

            if (true)
            {
                try
                {

                    //  var ExistingTrans = _context.OeTransHs.Where(x => x.OeTransHId == trans.OeTransHId).FirstOrDefault();

                    if (ExistingTrans != null)
                    {
                        cKode = ExistingTrans.Kode;

                        foreach (var item in ExistingTrans.OeTransDs)
                        {
                            if (item.Qty != 0)
                            {

                                IcItem cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                                if (cekItem != null)
                                {
                                    IcAltItem cekLokasi1 = _contextIc.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                                    if (cekLokasi1 == null)
                                    {
                                        IcAltItem Produk = new IcAltItem()
                                        {
                                            ItemCode = cekItem.ItemCode.ToUpper(),
                                            NamaItem = cekItem.NamaItem,
                                            Satuan = cekItem.Satuan,
                                            Lokasi = item.Lokasi,
                                            Qty = (ExistingTrans.Kode == "95" ? -1 * item.Qty : item.Qty)
                                        };
                                        _contextIc.IcAltItems.Add(Produk);

                                    }
                                    else
                                    {
                                        if (ExistingTrans.Kode == "95")
                                            cekLokasi1.Qty -= item.Qty;
                                        else
                                            cekLokasi1.Qty += item.Qty;

                                        _contextIc.IcAltItems.Update(cekLokasi1);
                                    }
                                    if (cekItem.JnsBrng == (int)jnsBrng.Stock)   // jika stock
                                    {
                                        if (ExistingTrans.Kode == "95")
                                            cekItem.Qty -= item.Qty;
                                        else
                                            cekItem.Qty += item.Qty;
                                    }

                                    if (cekItem.CostMethod == (int)costMethod.Moving_Avg)  // jika moving avarage
                                    {
                                        if (ExistingTrans.Kode == "95")
                                            cekItem.Cost -= item.Cost;
                                        else
                                            cekItem.Cost += item.Cost;

                                    }
                                    else
                                    {
                                        //if (ExistingTrans.Kode == "95")
                                        //    cekItem.Cost -= cekItem.Qty * cekItem.StdPrice;
                                        //else
                                        //    cekItem.Cost += cekItem.Qty * cekItem.StdPrice;
                                        cekItem.Cost = cekItem.Qty * cekItem.StdPrice;
                                    }

                                    //if (cekItem.Qty != 0)
                                    //{
                                    //    cekItem.HrgNetto = cekItem.Cost / cekItem.Qty;
                                    //}
                                    //else
                                    //{
                                    //    cekItem.HrgNetto = cekItem.Harga;
                                    //}

                                    _contextIc.IcItems.Update(cekItem);

                                }
                            }

                        }

                        var existingCustomer = GetCustomerId(ExistingTrans.Customer);
                        if (ExistingTrans.Kode == "94")
                            existingCustomer.Piutang -= ExistingTrans.Jumlah;
                        else
                            existingCustomer.Piutang += ExistingTrans.Jumlah;

                        if (cekFirst != null)
                        {
                            _contextAr.ArPiutngs.Remove(cekFirst);
                        }

                        _contextAr.ArCusts.Update(existingCustomer);

                        _context.OeTransHs.Remove(ExistingTrans);

                        /* update nya */
                        OeTransH transH = new OeTransH
                        {
                            NoLpb = trans.NoLpb,
                            Customer = trans.Customer.ToUpper(),
                            NamaCust = GetCustomerId(trans.Customer.ToUpper()).NamaLengkap,
                            Salesman = trans.Salesman,
                            Tanggal = trans.Tanggal,
                            JthTempo = trans.JthTempo,
                            Keterangan = trans.Keterangan,
                            AlamatKirim = trans.AlamatKirim,
                            NoPrj = trans.NoPrj,
                            Jumlah = trans.Jumlah,
                            Discount = trans.Discount,
                            Ongkos = trans.Ongkos,
                            Ppn = trans.Ppn,
                            PpnPersen = trans.PpnPersen,
                            TtlJumlah = trans.TtlJumlah,
                            DPayment = trans.DPayment,
                            Tagihan = trans.Tagihan,
                            TotalQty = trans.TotalQty,
                            Kode = cKode,
                            Cek = (trans.NonPiutang ? "" : "1"),
                            Pajak = trans.Pajak,
                            Kurir = trans.Kurir,
                            OeTransDs = new List<OeTransD>()
                        };

                        foreach (var item in trans.OeTransDs)
                        {
                            if (item.Qty != 0)
                            {
                                if (transH.TotalQty != 0)
                                {
                                    mQty5 = (item.Jumlah - item.Discount) - (item.Qty / transH.TotalQty * transH.Ppn) + (item.Qty / transH.TotalQty * transH.Ongkos);
                                }

                                transH.OeTransDs.Add(new OeTransD()
                                {
                                    ItemCode = item.ItemCode.ToUpper(),
                                    NamaItem = item.NamaItem,
                                    Satuan = item.Satuan,
                                    Lokasi = item.Lokasi,
                                    Harga = item.Harga,
                                    Qty = item.Qty,
                                    Persen = item.Persen,
                                    Discount = item.Discount,
                                    Jumlah = item.Jumlah,
                                    Kode = cKode,
                                    NoLpb = transH.NoLpb,
                                    Tanggal = trans.Tanggal,
                                    HrgCost = item.HrgCost,
                                    Cost = item.HrgCost * item.Qty,
                                    JumDpp = mQty5
                                });


                                IcItem cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                                if (cekItem != null)
                                {
                                    IcAltItem cekLokasi1 = _contextIc.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                                    if (cekLokasi1 == null)
                                    {
                                        IcAltItem Produk = new IcAltItem()
                                        {
                                            ItemCode = cekItem.ItemCode.ToUpper(),
                                            NamaItem = cekItem.NamaItem,
                                            Satuan = cekItem.Satuan,
                                            Lokasi = item.Lokasi,
                                            Qty = (cKode == "95" ? item.Qty : -1 * item.Qty)
                                        };
                                        _contextIc.IcAltItems.Add(Produk);

                                    }
                                    else
                                    {
                                        cekLokasi1.Qty += item.Qty;
                                        _contextIc.IcAltItems.Update(cekLokasi1);
                                    }

                                    if (cekItem.JnsBrng == (int)jnsBrng.Stock)   // jika stock
                                    {
                                        if (cKode == "95")
                                            cekItem.Qty += item.Qty;
                                        else
                                            cekItem.Qty -= item.Qty;
                                    }

                                    if (cekItem.CostMethod == (int)costMethod.Moving_Avg)  // jika moving avarage
                                    {
                                        if (cKode == "95")
                                            cekItem.Cost += (item.HrgCost * item.Qty);
                                        else
                                            cekItem.Cost -= (item.HrgCost * item.Qty);


                                    }
                                    else
                                    {
                                        //if (ExistingTrans.Kode == "95")
                                        //    cekItem.Cost += cekItem.Qty * cekItem.StdPrice;
                                        //else
                                        //    cekItem.Cost -= cekItem.Qty * cekItem.StdPrice;
                                        cekItem.Cost = cekItem.Qty * cekItem.StdPrice;
                                    }

                                    //if (cekItem.Qty != 0)
                                    //{
                                    //    cekItem.HrgNetto = cekItem.Cost / cekItem.Qty;
                                    //}
                                    //else
                                    //{
                                    //    cekItem.HrgNetto = cekItem.Harga;
                                    //}

                                    _contextIc.IcItems.Update(cekItem);

                                }
                            }

                        }

                        // Step 3: Tambah Data Piutang (AR)
                        if (!trans.NonPiutang)
                        {
                            var Customer = GetCustomerId(transH.Customer);

                            ArPiutng piutang = new ArPiutng
                            {
                                Kode = "OE",
                                Dokumen = transH.NoLpb,
                                Tanggal = transH.Tanggal,
                                DueDate = transH.JthTempo,

                                Customer = transH.Customer,
                                Salesman = transH.Salesman,
                                Keterangan = transH.Keterangan,
                                Jumlah = (cKode == "94" ? transH.Jumlah : -1 * transH.Jumlah),
                                Sisa = (cKode == "94" ? transH.Jumlah : -1 * transH.Jumlah),
                                SldSisa = (cKode == "94" ? transH.Jumlah : -1 * transH.Jumlah),
                                KodeTran = transH.Kode
                            };

                            //   DueDate = transH.Tanggal.AddDays(Customer.Termin),

                            if (cKode == "94")
                                Customer.Piutang += transH.Jumlah;
                            else
                                Customer.Piutang -= transH.Jumlah;


                            _contextAr.ArCusts.Update(Customer);
                            _contextAr.ArPiutngs.Add(piutang);
                        }

                        _context.OeTransHs.Add(transH);
                        _contextAr.SaveChanges();
                        _contextIc.SaveChanges();
                        _context.SaveChanges();

                        //  var TempTrans = GetTransDoc(transH.NoLpb);

                        //   return transH;
                        return true;

                    }
                    else
                    {
                        // return false;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("caught exception" + e.Message);
                }
            }
            return false;



        }

        #endregion OeTransH Class

        public string GetNumber()
        {
            string kodeno = "SLS";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.OeTransHs.Where(x => x.NoLpb.Substring(0, 10).Equals(xbukti)).ToList();
            if (maxlist != null)
            {
                maxvalue = maxlist.Max(x => x.NoLpb);

            }

            //            var maxvalue = (from e in db.CbTransHs where  e.Docno.Substring(0, 7) == kodeno + thnbln select e).Max();
            string nourut = "00000";
            if (maxvalue == null)
            {
                nourut = "00000";
            }
            else
            {
                nourut = maxvalue.Substring(10, 5);
            }

            //  nourut =Convert.ToString(Int32.Parse(nourut) + 1);


            string cAngNo = xbukti + (Int32.Parse(nourut) + 1).ToString("00000");
            // var maxvalue = (from e in db.AptTranss where e.NoRef.Substring(0, 7) == "ANG" + cAngNo select e.NoRef.Max()).FirstOrDefault();
            return cAngNo;

        }

        public string GetNumberTax()
        {
            string kodeno = "PJL";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.OeTransHs.Where(x => x.NoLpb.Substring(0, 10).Equals(xbukti)).ToList();
            if (maxlist != null)
            {
                maxvalue = maxlist.Max(x => x.NoLpb);

            }

            //            var maxvalue = (from e in db.CbTransHs where  e.Docno.Substring(0, 7) == kodeno + thnbln select e).Max();
            string nourut = "00000";
            if (maxvalue == null)
            {
                nourut = "00000";
            }
            else
            {
                nourut = maxvalue.Substring(10, 5);
            }

            //  nourut =Convert.ToString(Int32.Parse(nourut) + 1);


            string cAngNo = xbukti + (Int32.Parse(nourut) + 1).ToString("00000");
            // var maxvalue = (from e in db.AptTranss where e.NoRef.Substring(0, 7) == "ANG" + cAngNo select e.NoRef.Max()).FirstOrDefault();
            return cAngNo;

        }

        #region retur jual

        public OeTransH AddTransHRetur(OeTransHView trans, bool pajak)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            decimal mQty5 = 0;

            OeTransH transH = new OeTransH
            {

                NoLpb = (pajak ? GetNumberTaxRetur() : GetNumberRetur()),
                Customer = trans.Customer.ToUpper(),
                NamaCust = GetCustomerId(trans.Customer.ToUpper()).NamaLengkap,

                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                AlamatKirim = trans.AlamatKirim,
                Jumlah = trans.Jumlah,
                Ongkos = trans.Ongkos,
                Ppn = trans.Ppn,
                PpnPersen = trans.PpnPersen,
                TtlJumlah = trans.TtlJumlah,
                DPayment = trans.DPayment,
                Tagihan = trans.Tagihan,
                TotalQty = trans.TotalQty,
                Kode = "95",
                Cek = "1",
                Pajak = pajak,
                OeTransDs = new List<OeTransD>()
            };

            foreach (var item in trans.OeTransDs)
            {
                if (item.Qty != 0)
                {
                    if (transH.TotalQty != 0)
                    {
                        mQty5 = (item.Jumlah - item.Discount) - (item.Qty / transH.TotalQty * transH.Ppn) + (item.Qty / transH.TotalQty * transH.Ongkos);
                    }
                    else
                    {
                        mQty5 = (item.Jumlah - item.Discount);
                    }

                    transH.OeTransDs.Add(new OeTransD()
                    {
                        ItemCode = item.ItemCode.ToUpper(),
                        NamaItem = item.NamaItem,
                        Satuan = item.Satuan,
                        Lokasi = item.Lokasi,
                        Harga = item.Harga,
                        Qty = item.Qty,
                        Persen = item.Persen,
                        Discount = item.Discount,
                        Jumlah = item.Jumlah,
                        Kode = "95",
                        NoLpb = transH.NoLpb,
                        Tanggal = trans.Tanggal,
                        HrgCost = item.HrgCost,
                        Cost = item.HrgCost * item.Qty,
                        JumDpp = mQty5
                    });

                    IcItem cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();

                    if (cekItem != null)
                    {
                        #region altitem

                        IcAltItem cekLokasi1 = _contextIc.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                        if (cekLokasi1 == null)
                        {
                            IcAltItem Produk = new IcAltItem()
                            {
                                ItemCode = cekItem.ItemCode.ToUpper(),
                                NamaItem = cekItem.NamaItem,
                                Satuan = cekItem.Satuan,
                                Lokasi = item.Lokasi,
                                Qty = item.Qty
                            };
                            _contextIc.IcAltItems.Add(Produk);

                        }
                        else
                        {
                            cekLokasi1.Qty += item.Qty;
                            _contextIc.IcAltItems.Update(cekLokasi1);
                        }

                        #endregion altitem

                        cekItem.Harga = item.Harga;  // harga beli barang

                        if (cekItem.JnsBrng == (int)jnsBrng.Stock)   // jika stock
                        {
                            cekItem.Qty += item.Qty;
                        }

                        if (cekItem.CostMethod == (int)costMethod.Moving_Avg)  // jika moving avarage
                        {

                            cekItem.Cost += item.HrgCost * item.Qty;
                        }

                        //if (cekItem.Qty != 0)
                        //{
                        //    cekItem.HrgNetto = cekItem.Cost / cekItem.Qty;
                        //}
                        //else
                        //{
                        //    cekItem.HrgNetto = cekItem.Harga;
                        //}

                        _contextIc.IcItems.Update(cekItem);

                    }
                }
                _context.OeTransHs.Add(transH);
            }

            var Customer = GetCustomerId(transH.Customer);

            ArPiutng piutang = new()
            {
                Kode = "OE",
                Dokumen = transH.NoLpb,
                Tanggal = transH.Tanggal,
                DueDate = transH.Tanggal.AddDays(Customer.Termin),
                Customer = transH.Customer,
                Keterangan = transH.Keterangan,
                Jumlah = -1 * transH.Jumlah,
                Sisa = -1 * transH.Jumlah,
                SldSisa = -1 * transH.Jumlah,
                KodeTran = transH.Kode
            };
            _contextAr.ArPiutngs.Add(piutang);


            Customer.Piutang -= transH.Jumlah;

            _contextAr.ArCusts.Update(Customer);

            _context.SaveChanges();
            _contextAr.SaveChanges();
            _contextIc.SaveChanges();

            var TempTrans = GetTransDoc(transH.NoLpb);

            return TempTrans;

        }

        public string GetNumberRetur()
        {
            string kodeno = "R/J";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.OeTransHs.Where(x => x.NoLpb.Substring(0, 10).Equals(xbukti)).ToList();
            if (maxlist != null)
            {
                maxvalue = maxlist.Max(x => x.NoLpb);

            }

            //            var maxvalue = (from e in db.CbTransHs where  e.Docno.Substring(0, 7) == kodeno + thnbln select e).Max();
            string nourut = "00000";
            if (maxvalue == null)
            {
                nourut = "00000";
            }
            else
            {
                nourut = maxvalue.Substring(10, 5);
            }

            //  nourut =Convert.ToString(Int32.Parse(nourut) + 1);


            string cAngNo = xbukti + (Int32.Parse(nourut) + 1).ToString("00000");
            // var maxvalue = (from e in db.AptTranss where e.NoRef.Substring(0, 7) == "ANG" + cAngNo select e.NoRef.Max()).FirstOrDefault();
            return cAngNo;

        }

        public string GetNumberTaxRetur()
        {
            string kodeno = "RTJ";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.OeTransHs.Where(x => x.NoLpb.Substring(0, 10).Equals(xbukti)).ToList();
            if (maxlist != null)
            {
                maxvalue = maxlist.Max(x => x.NoLpb);

            }

            //            var maxvalue = (from e in db.CbTransHs where  e.Docno.Substring(0, 7) == kodeno + thnbln select e).Max();
            string nourut = "00000";
            if (maxvalue == null)
            {
                nourut = "00000";
            }
            else
            {
                nourut = maxvalue.Substring(10, 5);
            }

            //  nourut =Convert.ToString(Int32.Parse(nourut) + 1);


            string cAngNo = xbukti + (Int32.Parse(nourut) + 1).ToString("00000");
            // var maxvalue = (from e in db.AptTranss where e.NoRef.Substring(0, 7) == "ANG" + cAngNo select e.NoRef.Max()).FirstOrDefault();
            return cAngNo;

        }

        #endregion

        #region indexjual

        public async Task<List<OeTransH>> GetTransKurirAsync()
        {
            DateTime date1 = new DateTime(2022, 4, 17, 0, 0, 0);

            try
            {
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

                return await query.ToListAsync(); // Eksekusi query secara async
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Logging error untuk debugging
                throw;
            }
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
                //  oeTransH.Kurir = transaksi.Kurir;
                oeTransH.Salesman = transaksi.Salesman;

                _context.SaveChanges();
            }


        }

        #endregion

        public List<OeTransD> GetOeTransDByDokumen(string dokumen)
        {
            return _context.OeTransDs
                .AsNoTracking()
                .Where(x => x.NoLpb == dokumen)
                .OrderBy(x => x.OeTransDId)
                .ToList();
        }
    }
}
