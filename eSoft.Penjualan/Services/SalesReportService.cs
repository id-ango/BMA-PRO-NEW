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
            var startDate = tgl1.Date;
            var endDateExclusive = tgl2.Date.AddDays(1);

            return _context.OeTransHs
                .AsNoTracking()
                .Where(x => x.Tanggal >= startDate && x.Tanggal < endDateExclusive)
                .OrderByDescending(t => t.Tanggal)
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
            var startDate = tgl1.Date;
            var endDateExclusive = tgl2.Date.AddDays(1);

            return _context.OeTransDs
                .AsNoTracking()
                .Where(x => x.Tanggal >= startDate && x.Tanggal < endDateExclusive && x.Kode == "94" && x.Jumlah != 0)
                .OrderBy(t => t.Tanggal)
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
            return _context.OeTransDs
                .AsNoTracking()
                .Where(x => x.OeTransHId == xKdHeader)
                .ToList();
        }

        public List<OeTrans> Detail2(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            var startDate = tgl1.Date;
            var endDateExclusive = tgl2.Date.AddDays(1);

            return (from header in _context.OeTransHs.AsNoTracking()
                    join detail in _context.OeTransDs.AsNoTracking() on header.OeTransHId equals detail.OeTransHId
                    where detail.ItemCode == xKdHeader
                        && header.Tanggal >= startDate
                        && header.Tanggal < endDateExclusive
                    select new OeTrans
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

        public List<OeTrans> Detail3(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            var startDate = tgl1.Date;
            var endDateExclusive = tgl2.Date.AddDays(1);

            return (from header in _context.OeTransHs.AsNoTracking()
                    join detail in _context.OeTransDs.AsNoTracking() on header.OeTransHId equals detail.OeTransHId
                    where header.Customer == xKdHeader
                        && header.Tanggal >= startDate
                        && header.Tanggal < endDateExclusive
                    select new OeTrans
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

        public List<OeTrans> Detail3Index(string xKdHeader)
        {
            return (from header in _context.OeTransHs.AsNoTracking()
                    join detail in _context.OeTransDs.AsNoTracking() on header.OeTransHId equals detail.OeTransHId
                    where header.NoLpb == xKdHeader
                    select new OeTrans
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

        public List<OeTrans> Detail4(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            var startDate = tgl1.Date;
            var endDateExclusive = tgl2.Date.AddDays(1);

            return _context.OeTransHs
                .AsNoTracking()
                .Where(x => x.Kurir == xKdHeader && x.Tanggal >= startDate && x.Tanggal < endDateExclusive)
                .Select(header => new OeTrans()
                {
                    NoLpb = header.NoLpb,
                    Customer = header.Customer,
                    NamaCust = header.NamaCust,
                    Tanggal = header.Tanggal,
                    AlamatKirim = header.AlamatKirim
                }).ToList();
        }

        public List<OeTrans> Detail5(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            var startDate = tgl1.Date;
            var endDateExclusive = tgl2.Date.AddDays(1);

            return _context.OeTransHs
                .AsNoTracking()
                .Where(x => x.Salesman == xKdHeader && x.Tanggal >= startDate && x.Tanggal < endDateExclusive)
                .Select(header => new OeTrans()
                {
                    NoLpb = header.NoLpb,
                    Customer = header.Customer,
                    NamaCust = header.NamaCust,
                    Tanggal = header.Tanggal,
                    AlamatKirim = header.AlamatKirim,
                    Jumlah = header.Jumlah
                }).ToList();
        }
    }
}
