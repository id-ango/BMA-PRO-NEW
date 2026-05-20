using System;
using System.Collections.Generic;
using System.Linq;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Penjualan.Services
{
    public class SalesReportService : ISalesReportService
    {
        private readonly DbContextJual _context;

        public SalesReportService(DbContextJual context)
        {
            _context = context;
        }

        public List<OeTransH> Laporan1(DateTime tgl1, DateTime tgl2)
        {
            return _context.OeTransHs.Where(x => x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date).OrderByDescending(t => t.Tanggal.Date)
                .Select(x => new OeTransH
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
                    TtlJumlah = x.Kode == "94" ? x.TtlJumlah : -1 * x.TtlJumlah,
                    Ppn = x.Kode == "94" ? x.Ppn : -1 * x.Ppn,
                    Ongkos = x.Kode == "94" ? x.Ongkos : -1 * x.Ongkos,
                    Jumlah = x.Kode == "94" ? x.Jumlah : -1 * x.Jumlah
                })
                .ToList();
        }

        public List<OeTransD> LaporanDownload(DateTime tgl1, DateTime tgl2)
        {
            return _context.OeTransDs.Where(x => x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date && x.Kode == "94" && x.Jumlah != 0).OrderBy(t => t.Tanggal.Date)
                .Select(x => new OeTransD
                {
                    OeTransHId = x.OeTransHId,
                    Kode = x.Kode,
                    Tanggal = x.Tanggal,
                    NoLpb = x.NoLpb,
                    NamaItem = x.NamaItem,
                    ItemCode = x.ItemCode,
                    Qty = x.Qty,
                    Jumlah = x.Kode == "94" ? x.Jumlah : -1 * x.Jumlah
                })
                .ToList();
        }

        public List<OeTransD> Detail1(int xKdHeader)
        {
            return _context.OeTransDs.Where(x => x.OeTransHId == xKdHeader).ToList();
        }

        public List<OeTrans> Detail2(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            List<OeTransH> transH = _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date).ToList();
            List<OeTransD> transD = _context.OeTransDs.Where(x => x.ItemCode == xKdHeader).ToList();
            List<OeTrans> trans = new();

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
            List<OeTransH> transH = _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Customer == xKdHeader && (x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date)).ToList();
            List<OeTransD> transD = _context.OeTransDs.ToList();
            List<OeTrans> trans = new();

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
            List<OeTransH> transH = _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Customer == xKdHeader).ToList();
            List<OeTransD> transD = _context.OeTransDs.ToList();
            List<OeTrans> trans = new();

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
            List<OeTransH> transH = _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Kurir == xKdHeader && (x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date)).ToList();
            List<OeTransD> transD = _context.OeTransDs.ToList();
            List<OeTrans> trans = new();

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
            List<OeTransH> transH = _context.OeTransHs.Include(p => p.OeTransDs).Where(x => x.Salesman == xKdHeader && (x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date)).ToList();
            List<OeTransD> transD = _context.OeTransDs.ToList();
            List<OeTrans> trans = new();

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
    }
}
