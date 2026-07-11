using Accounting.Data;
using Accounting.Services.View;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using eSoft.Financial.Model;
using eSoft.Financial.View;
using eSoft.Hutang.Model;
using eSoft.Hutang.Services;
using eSoft.Hutang.View;
using eSoft.Order.Model;
using eSoft.Order.Services;
using eSoft.Order.View;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Persediaan.View;
using eSoft.Piutang.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Services
{
    public class ExcelServices : IExcelServices
    {
        private readonly DbContextJual _context;
        private readonly IOrderPurchaseServices _purchaseService;

        public ExcelServices(DbContextJual context, IOrderPurchaseServices purchaseService)
        {
            _context = context;
            _purchaseService = purchaseService;
        }

        public string GetCSV(IEnumerable<OeTransD> list)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Nota,Tanggal,Keterangan");
            foreach (var author in list)
            {
                stringBuilder.AppendLine($"{author.NoLpb},{author.Tanggal.ToShortDateString()},{ author.NamaItem}");
            }

            return stringBuilder.ToString();
        }

        #region Excel
        private byte[] ConvertToByte(XLWorkbook workbook)
        {
            var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var content = stream.ToArray();
            return content;
        }

        public byte[] CreatePenjualanExport(List<OeTransD> penjualan)
        {
            var workbook = new XLWorkbook();
            workbook.Properties.Title = "Export from authors";
            workbook.Properties.Author = "Enrico Rossini";
            workbook.Properties.Subject = "Export from authors";
            workbook.Properties.Keywords = "authors, puresourcecode, blazor";

            CreatePenjualanWorksheet(workbook,penjualan);

            return ConvertToByte(workbook);
        }

        public void CreatePenjualanWorksheet(XLWorkbook package, List<OeTransD> penjualan)
        {
            var worksheet = package.Worksheets.Add("Penjualan");
            var Penjualan = penjualan;
        //    var AuthorData = _context.OeTransDs.ToList();

            worksheet.Cell(1, 1).Value = "Nota";
            worksheet.Cell(1, 2).Value = "Tanggal";
            worksheet.Cell(1, 3).Value = "Keterangan";
            worksheet.Cell(1, 4).Value = "Qty";
            worksheet.Cell(1, 5).Value = "Harga";
            worksheet.Cell(1, 6).Value = "Jumlah";
            worksheet.Cell(1, 7).Value = "PPN";
            worksheet.Cell(1, 8).Value = "Total";
            worksheet.Cell(1, 9).Value = "Jumlah";
            worksheet.Cell(1, 10).Value = "Ttl";
            for (int index = 1; index <= penjualan.ToList().Count; index++)
            {
                worksheet.Cell(index + 1, 1).Value = Penjualan[index - 1].NoLpb;
                worksheet.Cell(index + 1, 2).Value = Penjualan[index - 1].Tanggal.ToString("dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                worksheet.Cell(index + 1, 3).Value = Penjualan[index - 1].NamaItem;
                worksheet.Cell(index + 1, 4).Value = Penjualan[index - 1].Qty;
                worksheet.Cell(index + 1, 5).FormulaA1 = string.Format("J{0}/D{0}", index + 1);
                worksheet.Cell(index + 1, 6).FormulaA1 = string.Format("D{0}*E{0}", index + 1);
                worksheet.Cell(index + 1, 7).FormulaA1 = string.Format("F{0}*0.11", index + 1);
                worksheet.Cell(index + 1, 8).FormulaA1 = string.Format("F{0}+G{0}", index + 1);
                worksheet.Cell(index + 1, 9).Value = Penjualan[index - 1].Jumlah;
              
                worksheet.Cell(index + 1, 10).FormulaA1 = string.Format("I{0}/1.11", index + 1);

            }

            // Get the last row index
            int lastRowIndex = penjualan.Count + 1;

            // Add the formula to calculate the sum of the Jumlah column
            string sumFormula = string.Format("SUM(F2:F{0})", lastRowIndex);
            worksheet.Cell(lastRowIndex + 1, 6).FormulaA1 = sumFormula;

            // Add the formula to calculate the sum of the Jumlah / 0.11 column in cell G{lastRowIndex + 1}
            string sumFormula2 = string.Format("SUM(G2:G{0})", lastRowIndex);
            worksheet.Cell(lastRowIndex + 1, 7).FormulaA1 = sumFormula2;
        }

        public byte[] CreateCustomerWorksheet(List<ArCust> transaksi)
        {
            var package = new XLWorkbook();
            var worksheet = package.Worksheets.Add("List Customer");
            var Transaksi = transaksi;
            //    var AuthorData = _context.OeTransDs.ToList();

            worksheet.Cell(1, 1).Value = "No";
            worksheet.Cell(1, 2).Value = "Kode";
            worksheet.Cell(1, 3).Value = "Nama";
            worksheet.Cell(1, 4).Value = "Alamat";
            worksheet.Cell(1, 5).Value = "Kota";
            worksheet.Cell(1, 6).Value = "Telpon";

            for (int index = 1; index <= transaksi.ToList().Count; index++)
            {
                worksheet.Cell(index + 1, 1).Value = index;
                worksheet.Cell(index + 1, 2).Value = Transaksi[index - 1].Customer;
                worksheet.Cell(index + 1, 3).Value = Transaksi[index - 1].NamaCust;
                worksheet.Cell(index + 1, 4).Value = Transaksi[index - 1].Alamat;
                worksheet.Cell(index + 1, 5).Value = Transaksi[index - 1].Kota;
                worksheet.Cell(index + 1, 6).Value = Transaksi[index - 1].Telpon.ToString();
            }
            return ConvertToByte(package);
        }

        public byte[] CreateKurirWorksheet(List<OeKurir> transaksi)
        {
            var package = new XLWorkbook();
            var worksheet = package.Worksheets.Add("List Kurir");
            var Transaksi = transaksi;
            //    var AuthorData = _context.OeTransDs.ToList();

            worksheet.Cell(1, 1).Value = "No";
            worksheet.Cell(1, 2).Value = "Kode";
            worksheet.Cell(1, 3).Value = "Nama";
            worksheet.Cell(1, 4).Value = "Alamat";
            worksheet.Cell(1, 5).Value = "Kota";
            worksheet.Cell(1, 6).Value = "Telpon";

            for (int index = 1; index <= transaksi.ToList().Count; index++)
            {
                worksheet.Cell(index + 1, 1).Value = index;
                worksheet.Cell(index + 1, 2).Value = Transaksi[index - 1].Kurir;
                worksheet.Cell(index + 1, 3).Value = Transaksi[index - 1].NamaKurir;
                worksheet.Cell(index + 1, 4).Value = Transaksi[index - 1].Alamat;
                worksheet.Cell(index + 1, 5).Value = Transaksi[index - 1].Kota;
                worksheet.Cell(index + 1, 6).Value = Transaksi[index - 1].Telpon.ToString();
            }
            return ConvertToByte(package);
        }
        public void CreateOtherWorksheet(XLWorkbook package)
        {
        }

        public byte[] CreateLedgerExport(List<FcTransDxls> Ledger)
        {
            var workbook = new XLWorkbook();
            workbook.Properties.Title = "Export from authors";
            workbook.Properties.Author = "Enrico Rossini";
            workbook.Properties.Subject = "Export from authors";
            workbook.Properties.Keywords = "authors, puresourcecode, blazor";

            CreateLedgerWorksheet(workbook, Ledger);

            return ConvertToByte(workbook);
        }

        public void CreateLedgerWorksheet(XLWorkbook package, List<FcTransDxls> ledger)
        {
            var worksheet = package.Worksheets.Add("Ledger");
            var Penjualan = ledger;
            //    var AuthorData = _context.OeTransDs.ToList();

            worksheet.Cell(1, 1).Value = "Docno";
            worksheet.Cell(1, 2).Value = "Tanggal";
            worksheet.Cell(1, 3).Value = "KodeGL";
            worksheet.Cell(1, 4).Value = "GLAkun";
            worksheet.Cell(1, 5).Value = "Keterangan";
            worksheet.Cell(1, 6).Value = "Debet";
            worksheet.Cell(1, 7).Value = "Kredit";
            for (int index = 1; index <= ledger.ToList().Count; index++)
            {
                worksheet.Cell(index + 1, 1).Value = Penjualan[index - 1].DocNo;
                worksheet.Cell(index + 1, 2).Value = Penjualan[index - 1].Tanggal.ToString("yyyy/MM/dd");
                worksheet.Cell(index + 1, 3).Value = Penjualan[index - 1].KodeGl;
                worksheet.Cell(index + 1, 4).Value = Penjualan[index - 1].GlAcct;
                worksheet.Cell(index + 1, 5).Value = Penjualan[index - 1].Keterangan;
                worksheet.Cell(index + 1, 6).Value = Penjualan[index - 1].Debet;
                worksheet.Cell(index + 1, 7).Value = Penjualan[index - 1].Kredit;
                
            }
        }
        #endregion

        public byte[] CreateHargaPIWorksheet(List<PoTransH> transHeader, List<PoItemQtyByLocationView> transItemLokasi)
        {
            var workbook = new XLWorkbook();
            workbook.Properties.Title = "Export from authors";
            workbook.Properties.Author = "Aldrin";
            workbook.Properties.Subject = "Export from authors";
            workbook.Properties.Keywords = "authors, puresourcecode, blazor";

            var worksheet = workbook.Worksheets.Add("PI Harga");

            //    var AuthorData = _context.OeTransDs.ToList();
            var nourut = 1;
            worksheet.Cell(1, nourut++).Value = "Kode Barang";
            worksheet.Cell(1, nourut++).Value = "Nama Barang";
            worksheet.Cell(1, nourut++).Value = "Satuan";

            foreach (var location in transHeader)
                {
                worksheet.Cell(1, nourut++).Value = location.NoPrj;
            }
            var index  = 1;
           
            foreach (var transaksi in transItemLokasi)
            {
                worksheet.Cell(index + 1, 1).Value = transaksi.ItemCode;
                worksheet.Cell(index + 1, 2).Value = transaksi.NamaItem;
                worksheet.Cell(index + 1, 3).Value = transaksi.Satuan;
                var kolum = 4;

                foreach (var location in transHeader)
                {
                    if (transaksi.Locations.Any(x => x.Lokasi == location.NoLpb))
                    {
                        worksheet.Cell(index + 1, kolum).Value = transaksi.Locations.FirstOrDefault(x => x.Lokasi == location.NoLpb).Qty;
                    }
                    kolum++;
                }
                index++;
            }
            

                return ConvertToByte(workbook);
        }
        public byte[] CreateHutangWorksheet(List<ApHutang> Ledger)
        {
            var workbook = new XLWorkbook();
            workbook.Properties.Title = "Export from authors";
            workbook.Properties.Author = "Enrico Rossini";
            workbook.Properties.Subject = "Export from authors";
            workbook.Properties.Keywords = "authors, puresourcecode, blazor";

            CreateHutang(workbook, Ledger);

            return ConvertToByte(workbook);
        }
        public void CreateHutang(XLWorkbook package, List<ApHutang> ledger)
        {
            var worksheet = package.Worksheets.Add("Kartu Hutang");
            var Penjualan = ledger;
            //    var AuthorData = _context.OeTransDs.ToList();

            worksheet.Cell(1, 1).Value = "Docno";
            worksheet.Cell(1, 2).Value = "Tanggal";
            worksheet.Cell(1, 3).Value = "Keterangan";
            worksheet.Cell(1, 4).Value = "Jumlah";
            worksheet.Cell(1, 5).Value = "Bayar";
            worksheet.Cell(1, 6).Value = "Sisa";
           
            for (int index = 1; index <= ledger.ToList().Count; index++)
            {
                worksheet.Cell(index + 1, 1).Value = Penjualan[index - 1].Dokumen;
                worksheet.Cell(index + 1, 2).Value = Penjualan[index - 1].Tanggal.ToString("yyyy/MM/dd");
                worksheet.Cell(index + 1, 3).Value = Penjualan[index - 1].Keterangan;
                worksheet.Cell(index + 1, 4).Value = Penjualan[index - 1].Jumlah;
                worksheet.Cell(index + 1, 5).Value = Penjualan[index - 1].Bayar;
                worksheet.Cell(index + 1, 6).Value = Penjualan[index - 1].Sisa;
              

            }
        }

        public byte[] CreateRekapStockWorksheet(List<IcRekapStock> rekap)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("RekapStock");

            // Headers
            worksheet.Cell(1, 1).Value = "ItemCode";
            worksheet.Cell(1, 2).Value = "NamaItem";
            worksheet.Cell(1, 3).Value = "Satuan";
            worksheet.Cell(1, 4).Value = "Divisi";
            worksheet.Cell(1, 5).Value = "QtyAwal";
            worksheet.Cell(1, 6).Value = "SaldoAwal";
            worksheet.Cell(1, 7).Value = "QtyMasuk";
            worksheet.Cell(1, 8).Value = "SaldoMasuk";
            worksheet.Cell(1, 9).Value = "QtyKeluar";
            worksheet.Cell(1, 10).Value = "SaldoKeluar";
            worksheet.Cell(1, 11).Value = "QtyAdjust";
            worksheet.Cell(1, 12).Value = "SaldoAdjust";
            worksheet.Cell(1, 13).Value = "QtyAkhir";
            worksheet.Cell(1, 14).Value = "SaldoAkhir";

            for (int i = 0; i < rekap.Count; i++)
            {
                var row = i + 2;
                var item = rekap[i];

                worksheet.Cell(row, 1).Value = item.ItemCode;
                worksheet.Cell(row, 2).Value = item.NamaItem;
                worksheet.Cell(row, 3).Value = item.Satuan;
                worksheet.Cell(row, 4).Value = item.Divisi;
                worksheet.Cell(row, 5).Value = item.QtyAwal;
                worksheet.Cell(row, 6).Value = item.SaldoAwal;
                worksheet.Cell(row, 7).Value = item.QtyMasuk;
                worksheet.Cell(row, 8).Value = item.SaldoMasuk;
                worksheet.Cell(row, 9).Value = item.QtyKeluar;
                worksheet.Cell(row, 10).Value = item.SaldoKeluar;
                worksheet.Cell(row, 11).Value = item.QtyAdjust;
                worksheet.Cell(row, 12).Value = item.SaldoAdjust;
                worksheet.Cell(row, 13).Value = item.QtyAkhir;
                worksheet.Cell(row, 14).Value = item.SaldoAkhir;
            }

            // Optionally auto-fit columns
            worksheet.Columns().AdjustToContents();

            return ConvertToByte(workbook);
        }

        public byte[] CreateRekapStockPjlSlsWorksheet(List<IcRekapStock> rekapStock, DateTime tanggal1, DateTime tanggal2)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("RekapStock_PJL_SLS");

            var awal = tanggal1.Date;
            var akhir = tanggal2.Date.AddDays(1).AddTicks(-1);

            var oePenjualan = _context.OeTransDs
                .Where(x => x.Kode == "94" && x.Tanggal >= awal && x.Tanggal <= akhir)
                .Select(x => new
                {
                    x.ItemCode,
                    x.NoLpb,
                    x.Qty,
                    x.Cost
                })
                .ToList();

            var pjl = oePenjualan
                .Where(x => !string.IsNullOrEmpty(x.NoLpb) && x.NoLpb.StartsWith("PJL-", StringComparison.OrdinalIgnoreCase))
                .GroupBy(x => x.ItemCode)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Qty = g.Sum(x => x.Qty),
                        Saldo = g.Sum(x => x.Cost)
                    });

            var sls = oePenjualan
                .Where(x => !string.IsNullOrEmpty(x.NoLpb) && x.NoLpb.StartsWith("SLS-", StringComparison.OrdinalIgnoreCase))
                .GroupBy(x => x.ItemCode)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Qty = g.Sum(x => x.Qty),
                        Saldo = g.Sum(x => x.Cost)
                    });

            worksheet.Cell(1, 1).Value = "ItemCode";
            worksheet.Cell(1, 2).Value = "NamaItem";
            worksheet.Cell(1, 3).Value = "Satuan";
            worksheet.Cell(1, 4).Value = "Divisi";

            worksheet.Cell(1, 5).Value = "QtyAwal";
            worksheet.Cell(1, 6).Value = "SaldoAwal";
            worksheet.Cell(1, 7).Value = "QtyMasuk";
            worksheet.Cell(1, 8).Value = "SaldoMasuk";

            worksheet.Cell(1, 9).Value = "QtyKeluarTotal";
            worksheet.Cell(1, 10).Value = "SaldoKeluarTotal";

            worksheet.Cell(1, 11).Value = "QtyKeluarPJL";
            worksheet.Cell(1, 12).Value = "SaldoKeluarPJL";
            worksheet.Cell(1, 13).Value = "QtyKeluarSLS";
            worksheet.Cell(1, 14).Value = "SaldoKeluarSLS";

            worksheet.Cell(1, 15).Value = "QtyAdjust";
            worksheet.Cell(1, 16).Value = "SaldoAdjust";
            worksheet.Cell(1, 17).Value = "QtyAkhir";
            worksheet.Cell(1, 18).Value = "SaldoAkhir";

            for (int i = 0; i < rekapStock.Count; i++)
            {
                var row = i + 2;
                var item = rekapStock[i];

                pjl.TryGetValue(item.ItemCode, out var pjlAgg);
                sls.TryGetValue(item.ItemCode, out var slsAgg);

                worksheet.Cell(row, 1).Value = item.ItemCode;
                worksheet.Cell(row, 2).Value = item.NamaItem;
                worksheet.Cell(row, 3).Value = item.Satuan;
                worksheet.Cell(row, 4).Value = item.Divisi;

                worksheet.Cell(row, 5).Value = item.QtyAwal;
                worksheet.Cell(row, 6).Value = item.SaldoAwal;
                worksheet.Cell(row, 7).Value = item.QtyMasuk;
                worksheet.Cell(row, 8).Value = item.SaldoMasuk;

                worksheet.Cell(row, 9).Value = item.QtyKeluar;
                worksheet.Cell(row, 10).Value = item.SaldoKeluar;

                worksheet.Cell(row, 11).Value = pjlAgg?.Qty ?? 0m;
                worksheet.Cell(row, 12).Value = pjlAgg?.Saldo ?? 0m;
                worksheet.Cell(row, 13).Value = slsAgg?.Qty ?? 0m;
                worksheet.Cell(row, 14).Value = slsAgg?.Saldo ?? 0m;

                worksheet.Cell(row, 15).Value = item.QtyAdjust;
                worksheet.Cell(row, 16).Value = item.SaldoAdjust;
                worksheet.Cell(row, 17).Value = item.QtyAkhir;
                worksheet.Cell(row, 18).Value = item.SaldoAkhir;
            }

            worksheet.Columns().AdjustToContents();
            return ConvertToByte(workbook);
        }

        public byte[] CreateRekapStockPjlSlsPpnWorksheet(List<IcRekapStock> rekapStock, DateTime tanggal1, DateTime tanggal2)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("RekapStock_PJL_SLS_PPN");

            var awal = tanggal1.Date;
            var akhir = tanggal2.Date.AddDays(1).AddTicks(-1);

            var headers = _context.OeTransHs
                .AsNoTracking()
                .Where(h => h.Tanggal >= awal && h.Tanggal <= akhir && (h.Kode == "94" || h.Kode == "95"))
                .Select(h => new
                {
                    h.NoLpb,
                    h.Ppn
                })
                .ToList();

            var headerMap = headers
                .Where(h => !string.IsNullOrEmpty(h.NoLpb))
                .GroupBy(h => h.NoLpb)
                .ToDictionary(g => g.Key, g => g.First());

            var details = _context.OeTransDs
                .AsNoTracking()
                .Where(d => d.Kode == "94" && d.Tanggal >= awal && d.Tanggal <= akhir)
                .Select(d => new
                {
                    d.ItemCode,
                    d.NoLpb,
                    d.Qty,
                    d.Cost
                })
                .ToList();

            static bool IsPjl(string? noLpb) => !string.IsNullOrEmpty(noLpb) && noLpb.StartsWith("PJL-", StringComparison.OrdinalIgnoreCase);
            static bool IsSls(string? noLpb) => !string.IsNullOrEmpty(noLpb) && noLpb.StartsWith("SLS-", StringComparison.OrdinalIgnoreCase);

            bool IsPjlPpn(string? noLpb)
            {
                if (!IsPjl(noLpb)) return false;
                if (string.IsNullOrEmpty(noLpb)) return false;

                if (!headerMap.TryGetValue(noLpb, out var h))
                    return false;

                return h.Ppn > 0;
            }

            bool IsPjlNonPpn(string? noLpb)
            {
                if (!IsPjl(noLpb)) return false;
                if (string.IsNullOrEmpty(noLpb)) return false;

                if (!headerMap.TryGetValue(noLpb, out var h))
                    return true; // fallback kalau header tidak ketemu

                return h.Ppn <= 0;
            }

            var sls = details
                .Where(x => IsSls(x.NoLpb))
                .GroupBy(x => x.ItemCode)
                .ToDictionary(g => g.Key, g => new { Qty = g.Sum(x => x.Qty), Saldo = g.Sum(x => x.Cost) });

            var pjlPpn = details
                .Where(x => IsPjlPpn(x.NoLpb))
                .GroupBy(x => x.ItemCode)
                .ToDictionary(g => g.Key, g => new { Qty = g.Sum(x => x.Qty), Saldo = g.Sum(x => x.Cost) });

            var pjlNonPpn = details
                .Where(x => IsPjlNonPpn(x.NoLpb))
                .GroupBy(x => x.ItemCode)
                .ToDictionary(g => g.Key, g => new { Qty = g.Sum(x => x.Qty), Saldo = g.Sum(x => x.Cost) });

            // Headers Excel
            worksheet.Cell(1, 1).Value = "ItemCode";
            worksheet.Cell(1, 2).Value = "NamaItem";
            worksheet.Cell(1, 3).Value = "Satuan";
            worksheet.Cell(1, 4).Value = "Divisi";

            worksheet.Cell(1, 5).Value = "QtyAwal";
            worksheet.Cell(1, 6).Value = "SaldoAwal";
            worksheet.Cell(1, 7).Value = "QtyMasuk";
            worksheet.Cell(1, 8).Value = "SaldoMasuk";

            worksheet.Cell(1, 9).Value = "QtyKeluarTotal";
            worksheet.Cell(1, 10).Value = "SaldoKeluarTotal";

            worksheet.Cell(1, 11).Value = "QtyKeluarSLS";
            worksheet.Cell(1, 12).Value = "SaldoKeluarSLS";

            worksheet.Cell(1, 13).Value = "QtyKeluarPJL_PPN";
            worksheet.Cell(1, 14).Value = "SaldoKeluarPJL_PPN";

            worksheet.Cell(1, 15).Value = "QtyKeluarPJL_NonPPN";
            worksheet.Cell(1, 16).Value = "SaldoKeluarPJL_NonPPN";

            worksheet.Cell(1, 17).Value = "QtyAdjust";
            worksheet.Cell(1, 18).Value = "SaldoAdjust";
            worksheet.Cell(1, 19).Value = "QtyAkhir";
            worksheet.Cell(1, 20).Value = "SaldoAkhir";

            for (int i = 0; i < rekapStock.Count; i++)
            {
                var row = i + 2;
                var item = rekapStock[i];

                sls.TryGetValue(item.ItemCode, out var slsAgg);
                pjlPpn.TryGetValue(item.ItemCode, out var pjlPpnAgg);
                pjlNonPpn.TryGetValue(item.ItemCode, out var pjlNonPpnAgg);

                worksheet.Cell(row, 1).Value = item.ItemCode;
                worksheet.Cell(row, 2).Value = item.NamaItem;
                worksheet.Cell(row, 3).Value = item.Satuan;
                worksheet.Cell(row, 4).Value = item.Divisi;

                worksheet.Cell(row, 5).Value = item.QtyAwal;
                worksheet.Cell(row, 6).Value = item.SaldoAwal;
                worksheet.Cell(row, 7).Value = item.QtyMasuk;
                worksheet.Cell(row, 8).Value = item.SaldoMasuk;

                worksheet.Cell(row, 9).Value = item.QtyKeluar;
                worksheet.Cell(row, 10).Value = item.SaldoKeluar;

                worksheet.Cell(row, 11).Value = slsAgg?.Qty ?? 0m;
                worksheet.Cell(row, 12).Value = slsAgg?.Saldo ?? 0m;

                worksheet.Cell(row, 13).Value = pjlPpnAgg?.Qty ?? 0m;
                worksheet.Cell(row, 14).Value = pjlPpnAgg?.Saldo ?? 0m;

                worksheet.Cell(row, 15).Value = pjlNonPpnAgg?.Qty ?? 0m;
                worksheet.Cell(row, 16).Value = pjlNonPpnAgg?.Saldo ?? 0m;

                worksheet.Cell(row, 17).Value = item.QtyAdjust;
                worksheet.Cell(row, 18).Value = item.SaldoAdjust;
                worksheet.Cell(row, 19).Value = item.QtyAkhir;
                worksheet.Cell(row, 20).Value = item.SaldoAkhir;
            }

            worksheet.Columns().AdjustToContents();
            return ConvertToByte(workbook);
        }

        public byte[] CreateSalesOrderStockExcel(SalesOrderStockMatrixView matrix)
        {
            var workbook = new XLWorkbook();
            var activePurchaseOrders = _purchaseService.GetTransHAktif() ?? new List<PoTransH>();

            var activePurchaseQtyByItem = activePurchaseOrders
                .SelectMany(x => x.PoTransDs ?? new List<PoTransD>())
                .Where(x => !string.IsNullOrWhiteSpace(x.ItemCode))
                .GroupBy(x => x.ItemCode, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Qty), StringComparer.OrdinalIgnoreCase);

            // ── Sheet 1: Matrix SO vs Item ──────────────────────────────────────
            var wsMatrix = workbook.Worksheets.Add("Matrix SO vs Item");

            // Style helper
            var headerFill = XLColor.FromHtml("#212529");
            var greenFill  = XLColor.FromHtml("#d1e7dd");
            var redFill    = XLColor.FromHtml("#f8d7da");
            var yellowFill = XLColor.FromHtml("#fff3cd");
            var grayFill   = XLColor.FromHtml("#f8f9fa");

            // -- Header baris 1: label kolom tetap
            int col = 1;
            string[] fixedHeaders = { "No SO", "Customer", "Tanggal", "No Prj", "Keterangan", "Status" };
            foreach (var h in fixedHeaders)
            {
                var c = wsMatrix.Cell(1, col);
                c.Value = h;
                c.Style.Font.Bold = true;
                c.Style.Font.FontColor = XLColor.White;
                c.Style.Fill.BackgroundColor = headerFill;
                c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                col++;
            }

            // -- Header baris 1: kolom item
            int itemStartCol = col;
            foreach (var item in matrix.ItemHeaders)
            {
                var c = wsMatrix.Cell(1, col);
                var poQty = activePurchaseQtyByItem.TryGetValue(item.ItemCode, out var qtyPo) ? qtyPo : 0m;
                c.Value = $"{item.ItemCode}\n{item.NamaItem}\nStk Awal: {item.QtyStock:N0} {item.Satuan}\nPO Aktif: {poQty:N0}";
                c.Style.Font.Bold = true;
                c.Style.Font.FontColor = XLColor.White;
                c.Style.Fill.BackgroundColor = headerFill;
                c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                c.Style.Alignment.WrapText = true;
                col++;
            }

            // -- Baris data per SO
            int row = 2;
            foreach (var soRow in matrix.Rows)
            {
                var rowBg = soRow.IsComplete ? yellowFill : XLColor.White;

                wsMatrix.Cell(row, 1).Value = soRow.NoLpb;
                wsMatrix.Cell(row, 2).Value = soRow.NamaCustomer;
                wsMatrix.Cell(row, 3).Value = soRow.Tanggal.ToString("dd/MM/yyyy");
                wsMatrix.Cell(row, 4).Value = soRow.NoPrj;
                wsMatrix.Cell(row, 5).Value = soRow.Keterangan;
                wsMatrix.Cell(row, 6).Value = soRow.IsComplete ? "Siap Proses" : "Menunggu";

                for (int c2 = 1; c2 <= 6; c2++)
                    wsMatrix.Cell(row, c2).Style.Fill.BackgroundColor = rowBg;

                int itemCol = itemStartCol;
                foreach (var cell in soRow.Cells)
                {
                    var xlCell = wsMatrix.Cell(row, itemCol);
                    if (!cell.IsOrdered)
                    {
                        xlCell.Value = "-";
                        xlCell.Style.Fill.BackgroundColor = grayFill;
                        xlCell.Style.Font.FontColor = XLColor.Gray;
                    }
                    else if (cell.HasStock)
                    {
                        xlCell.Value = $"Order: {cell.QtyOrder:N0}\nSisa: {cell.QtyStockSisa:N0} → {cell.QtyStockSetelah:N0}";
                        xlCell.Style.Fill.BackgroundColor = greenFill;
                        xlCell.Style.Font.FontColor = XLColor.FromHtml("#198754");
                    }
                    else
                    {
                        xlCell.Value = $"Order: {cell.QtyOrder:N0}\nSisa: {cell.QtyStockSisa:N0} ⚠ Kurang: {(cell.QtyOrder - cell.QtyStockSisa):N0}";
                        xlCell.Style.Fill.BackgroundColor = redFill;
                        xlCell.Style.Font.FontColor = XLColor.FromHtml("#dc3545");
                    }
                    xlCell.Style.Alignment.WrapText = true;
                    xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    itemCol++;
                }
                row++;
            }

            // -- Footer baris Total
            wsMatrix.Cell(row, 1).Value = "TOTAL DIPESAN / SISA AKHIR";
            wsMatrix.Range(row, 1, row, 6).Merge();
            wsMatrix.Cell(row, 1).Style.Font.Bold = true;
            wsMatrix.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#e9ecef");

            int footerCol = itemStartCol;
            foreach (var item in matrix.ItemHeaders)
            {
                var totalOrder = matrix.Rows.SelectMany(r => r.Cells)
                    .Where(c3 => c3.ItemCode == item.ItemCode)
                    .Sum(c3 => c3.QtyOrder);
                var totalPo = activePurchaseQtyByItem.TryGetValue(item.ItemCode, out var qtyPo) ? qtyPo : 0m;
                var sisaAkhir = item.QtyStock + totalPo - totalOrder;
                var xlCell = wsMatrix.Cell(row, footerCol);
                xlCell.Value = $"Order: {totalOrder:N0}\nPO: {totalPo:N0}\nSisa: {sisaAkhir:N0}";
                xlCell.Style.Font.Bold = true;
                xlCell.Style.Alignment.WrapText = true;
                xlCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                xlCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#e9ecef");
                xlCell.Style.Font.FontColor = sisaAkhir < 0 ? XLColor.FromHtml("#dc3545") : XLColor.FromHtml("#198754");
                footerCol++;
            }

            wsMatrix.Columns().AdjustToContents();
            wsMatrix.Column(1).Width = 18;
            wsMatrix.Column(2).Width = 22;
            wsMatrix.Column(5).Width = 25;
            wsMatrix.Row(1).Height = 50;

            // ── Sheet 2: Summary per Item ────────────────────────────────────────
            var wsSummary = workbook.Worksheets.Add("Summary Kebutuhan Item");

            string[] summaryHeaders = { "No", "Kode Item", "Nama Item", "Satuan", "Stock Tersedia", "Total Dipesan (SO)", "PO Aktif", "Sisa/Proyeksi", "Status", "Saran Pesan", "Keterangan No PI" };
            for (int i = 0; i < summaryHeaders.Length; i++)
            {
                var c = wsSummary.Cell(1, i + 1);
                c.Value = summaryHeaders[i];
                c.Style.Font.Bold = true;
                c.Style.Font.FontColor = XLColor.White;
                c.Style.Fill.BackgroundColor = headerFill;
                c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int sRow = 2;
            int no = 1;
            foreach (var item in matrix.ItemHeaders)
            {
                var totalDipesan = matrix.Rows.SelectMany(r => r.Cells)
                    .Where(c4 => c4.ItemCode == item.ItemCode)
                    .Sum(c4 => c4.QtyOrder);
                var totalPo = activePurchaseQtyByItem.TryGetValue(item.ItemCode, out var qtyPo2) ? qtyPo2 : 0m;
                var sisa = item.QtyStock + totalPo - totalDipesan;
                var kekurangan = sisa < 0 ? Math.Abs(sisa) : 0;
                var noPi = string.Join(", ", activePurchaseOrders
                    .Where(h => !string.IsNullOrWhiteSpace(h.NoPrj) && (h.PoTransDs?.Any(d =>
                        string.Equals(d.ItemCode, item.ItemCode, StringComparison.OrdinalIgnoreCase)) ?? false))
                    .Select(h => h.NoPrj)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x));
                var status = totalDipesan <= item.QtyStock
                    ? "Cukup dari stock"
                    : totalPo >= Math.Max(totalDipesan - item.QtyStock, 0)
                        ? "Tertutup oleh PO"
                        : totalPo > 0
                            ? "PO belum cukup"
                            : "Belum ada PO";

                wsSummary.Cell(sRow, 1).Value = no++;
                wsSummary.Cell(sRow, 2).Value = item.ItemCode;
                wsSummary.Cell(sRow, 3).Value = item.NamaItem;
                wsSummary.Cell(sRow, 4).Value = item.Satuan;
                wsSummary.Cell(sRow, 5).Value = item.QtyStock;
                wsSummary.Cell(sRow, 6).Value = totalDipesan;
                wsSummary.Cell(sRow, 7).Value = totalPo;
                wsSummary.Cell(sRow, 8).Value = sisa;
                wsSummary.Cell(sRow, 9).Value = status;
                wsSummary.Cell(sRow, 10).Value = kekurangan > 0 ? $"Perlu pesan min. {kekurangan:N0} {item.Satuan}" : "Cukup";
                wsSummary.Cell(sRow, 11).Value = string.IsNullOrWhiteSpace(noPi) ? "-" : noPi;

                // warna baris
                var bg = kekurangan > 0 ? redFill : greenFill;
                for (int i = 1; i <= 11; i++)
                    wsSummary.Cell(sRow, i).Style.Fill.BackgroundColor = bg;

                // warna khusus kolom sisa & kekurangan
                wsSummary.Cell(sRow, 7).Style.Font.FontColor = totalPo > 0 ? XLColor.FromHtml("#0d6efd") : XLColor.Gray;
                wsSummary.Cell(sRow, 7).Style.Font.Bold = true;
                wsSummary.Cell(sRow, 8).Style.Font.FontColor = sisa < 0 ? XLColor.FromHtml("#dc3545") : XLColor.FromHtml("#198754");
                wsSummary.Cell(sRow, 8).Style.Font.Bold = true;
                wsSummary.Cell(sRow, 9).Style.Font.Bold = true;
                wsSummary.Cell(sRow, 10).Style.Font.Bold = kekurangan > 0;
                wsSummary.Cell(sRow, 11).Style.Font.FontColor = XLColor.FromHtml("#0d6efd");

                sRow++;
            }

            wsSummary.Columns().AdjustToContents();
            wsSummary.Column(3).Width = 30;
            wsSummary.Column(10).Width = 28;
            wsSummary.Column(11).Width = 28;

            // border semua sel terisi
            var matrixRange = wsMatrix.Range(1, 1, row, col - 1);
            matrixRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            matrixRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            var summaryRange = wsSummary.Range(1, 1, sRow - 1, 11);
            summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            summaryRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Sheet 3: Prediksi PO & Readiness
            var purchaseDetails = activePurchaseOrders
                .SelectMany(x => x.PoTransDs ?? new List<PoTransD>())
                .ToList();
            var prediction = GeneratePOReadinessPrediction(matrix, activePurchaseOrders, purchaseDetails);
            BuildPoReadinessPredictionSheet(workbook, prediction);

            // Sheet 4: SO Progression per PI
            var soProgression = GenerateSOProgressionData(matrix, activePurchaseOrders, purchaseDetails);
            if (soProgression.PIsInOrder.Any())
            {
                BuildSOProgressionSheet(workbook, soProgression);
            }

            return ConvertToByte(workbook);
        }

        public byte[] CreateAgingPiutangExcel(
            List<eSoft.Piutang.View.ArAgingView> aging,
            List<eSoft.Piutang.View.ArForecastPiutangView> forecast,
            List<eSoft.Piutang.View.ArCustomerAnalysisView> analisa = null)
        {
            var workbook = new XLWorkbook();

            // ── Sheet 1: Aging Schedule ──────────────────────────────
            var ws = workbook.Worksheets.Add("Aging Schedule");

            string[] headers = {
                "No", "Customer", "Dokumen", "Tanggal", "Due Date", "Terlambat (hr)",
                "Piutang Awal", "Sudah Dibayar", "Cicilan ke-", "Tgl Terakhir Bayar", "Hari Sejak Bayar", "Info Bayar",
                "Sisa Piutang", "Belum JT", "1-30 hr", "31-60 hr", "61-90 hr", ">90 hr",
                "Salesman", "Keterangan"
            };
            for (int i = 0; i < headers.Length; i++)
            {
                var hCell = ws.Cell(1, i + 1);
                hCell.Value = headers[i];
                hCell.Style.Font.Bold = true;
                hCell.Style.Fill.BackgroundColor = XLColor.DarkSlateGray;
                hCell.Style.Font.FontColor = XLColor.White;
                hCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            // Warnai kelompok header bayar (kolom 7-12)
            ws.Range(1, 7, 1, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#084298");

            var agingSorted = aging.OrderBy(x => x.Duedate).ThenBy(x => x.Customer).ToList();
            int row = 2;
            int no = 1;
            foreach (var a in agingSorted)
            {
                var hariTerlambat = (int)(DateTime.Today - a.Duedate).TotalDays;
                ws.Cell(row, 1).Value = no++;
                ws.Cell(row, 2).Value = a.NamaCust;
                ws.Cell(row, 3).Value = a.Dokumen;
                ws.Cell(row, 4).Value = a.Tanggal.ToString("dd/MM/yyyy");
                ws.Cell(row, 5).Value = a.Duedate.ToString("dd/MM/yyyy");
                ws.Cell(row, 6).Value = hariTerlambat > 0 ? hariTerlambat : 0;
                ws.Cell(row, 7).Value = a.JumlahAwal;
                ws.Cell(row, 8).Value = a.SudahBayar;
                ws.Cell(row, 9).Value = a.JumlahCicilan;
                ws.Cell(row, 10).Value = a.TglTerakhirBayar.HasValue
                    ? a.TglTerakhirBayar.Value.ToString("dd/MM/yyyy")
                    : "-";
                ws.Cell(row, 11).Value = a.HariSejakTerakhirBayar;

                // Kolom 12: Info Bayar (teks ringkas, sama seperti badge di UI)
                string infoBayar;
                if (a.JumlahAwal < 0 || a.Sisa < 0)
                {
                    infoBayar = "Uang Muka";
                }
                else if (a.JumlahCicilan == 0)
                {
                    infoBayar = $"Blm bayar | Invoice {a.HariSejakTerakhirBayar}hr lalu";
                }
                else
                {
                    var persen = a.JumlahAwal > 0 ? (int)(a.SudahBayar / a.JumlahAwal * 100) : 0;
                    var tglByr = a.TglTerakhirBayar.HasValue
                        ? $"{a.TglTerakhirBayar:dd/MM/yy} ({a.HariSejakTerakhirBayar}hr lalu)"
                        : "-";
                    infoBayar = $"{a.JumlahCicilan}x cicilan | {persen}% terbayar | Tgl: {tglByr}";
                }
                ws.Cell(row, 12).Value = infoBayar;

                ws.Cell(row, 13).Value = a.Sisa;
                ws.Cell(row, 14).Value = a.Jumlah;
                ws.Cell(row, 15).Value = a.Jumlah1;
                ws.Cell(row, 16).Value = a.Jumlah2;
                ws.Cell(row, 17).Value = a.Jumlah3;
                ws.Cell(row, 18).Value = a.Jumlah4;
                ws.Cell(row, 19).Value = a.NamaSales;
                ws.Cell(row, 20).Value = a.Keterangan;

                // Warna baris berdasarkan aging
                XLColor bg;
                if (a.JumlahAwal < 0 || a.Sisa < 0) bg = XLColor.FromHtml("#EDE7F6"); // uang muka — ungu muda
                else if (a.Jumlah4 > 0)              bg = XLColor.FromHtml("#FFB3B3");
                else if (a.Jumlah3 > 0)              bg = XLColor.FromHtml("#FFD4B0");
                else if (a.Jumlah2 > 0)              bg = XLColor.FromHtml("#FFF3CD");
                else if (a.Jumlah1 > 0)              bg = XLColor.FromHtml("#FFFBE6");
                else                                  bg = XLColor.FromHtml("#F0FFF4");

                ws.Range(row, 1, row, 20).Style.Fill.BackgroundColor = bg;
                foreach (int c in new[] { 7, 8, 13, 14, 15, 16, 17, 18 })
                    ws.Cell(row, c).Style.NumberFormat.Format = "#,##0";
                row++;
            }

            // Footer total
            ws.Cell(row, 1).Value = "TOTAL";
            ws.Cell(row, 1).Style.Font.Bold = true;
            foreach (int c in new[] { 7, 8, 13, 14, 15, 16, 17, 18 })
            {
                ws.Cell(row, c).FormulaA1 = $"=SUM({ws.Cell(2, c).Address}:{ws.Cell(row - 1, c).Address})";
                ws.Cell(row, c).Style.Font.Bold = true;
                ws.Cell(row, c).Style.NumberFormat.Format = "#,##0";
            }
            ws.Range(row, 1, row, 20).Style.Fill.BackgroundColor = XLColor.DarkSlateGray;
            ws.Range(row, 1, row, 20).Style.Font.FontColor = XLColor.White;

            ws.Range(1, 1, row, 20).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 1, row, 20).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Columns().AdjustToContents();

            // ── Sheet 2: Forecast Bulanan ────────────────────────────
            if (forecast != null && forecast.Any())
            {
                var wsFc = workbook.Worksheets.Add("Forecast Tagihan");
                string[] fcH = { "Bulan", "Tahun", "Dok", "Customer", "Due Date", "Sisa", "Salesman", "Keterangan" };
                for (int i = 0; i < fcH.Length; i++)
                {
                    var hc = wsFc.Cell(1, i + 1);
                    hc.Value = fcH[i];
                    hc.Style.Font.Bold = true;
                    hc.Style.Fill.BackgroundColor = XLColor.DarkSlateGray;
                    hc.Style.Font.FontColor = XLColor.White;
                }
                int fcRow = 2;
                foreach (var bulan in forecast.OrderBy(x => x.Tahun).ThenBy(x => x.Bulan))
                {
                    if (!bulan.Details.Any()) continue;
                    foreach (var d in bulan.Details.OrderBy(x => x.DueDate))
                    {
                        wsFc.Cell(fcRow, 1).Value = bulan.NamaBulan;
                        wsFc.Cell(fcRow, 2).Value = bulan.Tahun;
                        wsFc.Cell(fcRow, 3).Value = d.Dokumen;
                        wsFc.Cell(fcRow, 4).Value = d.NamaCust;
                        wsFc.Cell(fcRow, 5).Value = d.DueDate.ToString("dd/MM/yyyy");
                        wsFc.Cell(fcRow, 6).Value = d.Sisa;
                        wsFc.Cell(fcRow, 6).Style.NumberFormat.Format = "#,##0";
                        wsFc.Cell(fcRow, 7).Value = d.Salesman;
                        wsFc.Cell(fcRow, 8).Value = d.Keterangan;
                        fcRow++;
                    }
                }
                wsFc.Range(1, 1, fcRow - 1, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                wsFc.Range(1, 1, fcRow - 1, 8).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                wsFc.Columns().AdjustToContents();
            }

            // ── Sheet 3: Analisa Customer ────────────────────────────
            if (analisa != null && analisa.Any())
            {
                var wsAn = workbook.Worksheets.Add("Analisa Customer");
                string[] anH = {
                    "Customer", "Nama Customer", "Salesman",
                    "Risk Score", "Label Risiko", "Rekomendasi",
                    "Outstanding", "Faktur Open", "Cicilan", "Diam (hr)", "Tgl Bayar Terakhir",
                    "Nunggak >60hr", "Nilai Nunggak >60hr",
                    "Avg Terlambat (hr)", "Max Terlambat (hr)", "On-Time %",
                    "DSO (hr)", "Total Transaksi"
                };
                for (int i = 0; i < anH.Length; i++)
                {
                    var hc = wsAn.Cell(1, i + 1);
                    hc.Value = anH[i];
                    hc.Style.Font.Bold = true;
                    hc.Style.Font.FontColor = XLColor.White;
                    hc.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    hc.Style.Fill.BackgroundColor = XLColor.DarkSlateGray;
                }
                // Warnai kolom scoring
                wsAn.Range(1, 4, 1, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#084298");

                int anRow = 2;
                foreach (var a in analisa)
                {
                    wsAn.Cell(anRow, 1).Value  = a.Customer;
                    wsAn.Cell(anRow, 2).Value  = a.NamaCust;
                    wsAn.Cell(anRow, 3).Value  = a.Salesman;
                    wsAn.Cell(anRow, 4).Value  = a.RiskScore;
                    wsAn.Cell(anRow, 5).Value  = a.RiskLabel;
                    wsAn.Cell(anRow, 6).Value  = a.Rekomendasi;
                    wsAn.Cell(anRow, 7).Value  = a.TotalOutstanding;
                    wsAn.Cell(anRow, 7).Style.NumberFormat.Format = "#,##0";
                    wsAn.Cell(anRow, 8).Value  = a.JumlahFakturOpen;
                    wsAn.Cell(anRow, 9).Value  = a.JumlahFakturCicilan;
                    wsAn.Cell(anRow, 10).Value = a.MaxHariMacetOutstanding;
                    wsAn.Cell(anRow, 11).Value = a.TglTerakhirBayarOutstanding.HasValue
                        ? a.TglTerakhirBayarOutstanding.Value.ToString("dd/MM/yyyy") : "-";
                    wsAn.Cell(anRow, 12).Value = a.CountTelat60;
                    wsAn.Cell(anRow, 13).Value = a.OutstandingTelat60;
                    wsAn.Cell(anRow, 13).Style.NumberFormat.Format = "#,##0";
                    wsAn.Cell(anRow, 14).Value = a.AvgDaysLate;
                    wsAn.Cell(anRow, 14).Style.NumberFormat.Format = "0.0";
                    wsAn.Cell(anRow, 15).Value = a.MaxDaysLate;
                    wsAn.Cell(anRow, 16).Value = a.OnTimeRate;
                    wsAn.Cell(anRow, 16).Style.NumberFormat.Format = "0.0\"%\"";
                    wsAn.Cell(anRow, 17).Value = a.DSO;
                    wsAn.Cell(anRow, 17).Style.NumberFormat.Format = "0.0";
                    wsAn.Cell(anRow, 18).Value = a.TotalNilaiTransaksi;
                    wsAn.Cell(anRow, 18).Style.NumberFormat.Format = "#,##0";

                    // Warna baris sesuai risiko
                    XLColor anBg = a.RiskLabel switch
                    {
                        "Blacklist"  => XLColor.FromHtml("#FECACA"),
                        "Macet"      => XLColor.FromHtml("#FFD5D5"),
                        "Jelek"      => XLColor.FromHtml("#FFE5CC"),
                        "Hati-hati"  => XLColor.FromHtml("#FFF3CD"),
                        "Cukup"      => XLColor.FromHtml("#FFFFF0"),
                        "Baik"       => XLColor.FromHtml("#D1FAE5"),
                        _            => XLColor.White
                    };
                    wsAn.Range(anRow, 1, anRow, anH.Length).Style.Fill.BackgroundColor = anBg;
                    // Score cell — bold jika buruk
                    if (a.RiskScore < 50) wsAn.Cell(anRow, 4).Style.Font.Bold = true;

                    anRow++;
                }
                wsAn.Range(1, 1, anRow - 1, anH.Length).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                wsAn.Range(1, 1, anRow - 1, anH.Length).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                wsAn.Columns().AdjustToContents();
            }

            return ConvertToByte(workbook);
        }

        #region PO Readiness Prediction Helper Methods

        /// <summary>
        /// Build sheet Excel untuk Prediksi PO & Readiness
        /// </summary>
        private void BuildPoReadinessPredictionSheet(XLWorkbook workbook, PoReadinessPredictionView prediction)
        {
            var ws = workbook.Worksheets.Add("Prediksi PO & Readiness");

            // -- COLOR PALETTE
            var headerFill = XLColor.FromHtml("#212529");
            var greenFill = XLColor.FromHtml("#d1e7dd");
            var redFill = XLColor.FromHtml("#f8d7da");
            var yellowFill = XLColor.FromHtml("#fff3cd");
            var blueFill = XLColor.FromHtml("#cfe2ff");
            var lightGrayFill = XLColor.FromHtml("#f8f9fa");

            int currentRow = 1;

            // ========== SECTION 1: QUICK SUMMARY ==========
            var titleCell = ws.Cell(currentRow, 1);
            titleCell.Value = "PREDIKSI PO & READINESS SO";
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontColor = XLColor.White;
            titleCell.Style.Fill.BackgroundColor = headerFill;
            ws.Range(currentRow, 1, currentRow, 8).Merge();
            ws.Range(currentRow, 1, currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            currentRow += 2;

            // Summary metrics
            var summaryRow = currentRow;
            ws.Cell(summaryRow, 1).Value = "RINGKASAN";
            ws.Cell(summaryRow, 1).Style.Font.Bold = true;
            ws.Cell(summaryRow, 1).Style.Fill.BackgroundColor = yellowFill;
            currentRow++;

            ws.Cell(currentRow, 1).Value = "Total SO Aktif";
            ws.Cell(currentRow, 2).Value = prediction.Summary.TotalSalesOrders;
            ws.Cell(currentRow, 2).Style.Font.Bold = true;
            currentRow++;

            ws.Cell(currentRow, 1).Value = "SO Sudah Ready";
            ws.Cell(currentRow, 2).Value = prediction.Summary.ReadyWithoutPO;
            ws.Cell(currentRow, 2).Style.Font.Bold = true;
            ws.Cell(currentRow, 2).Style.Font.FontColor = XLColor.FromHtml("#198754");
            currentRow++;

            ws.Cell(currentRow, 1).Value = "SO Masih Pending";
            var pendingCount = prediction.Summary.TotalSalesOrders - prediction.Summary.ReadyWithoutPO;
            ws.Cell(currentRow, 2).Value = pendingCount;
            ws.Cell(currentRow, 2).Style.Font.Bold = true;
            ws.Cell(currentRow, 2).Style.Font.FontColor = XLColor.FromHtml("#dc3545");
            currentRow++;

            ws.Cell(currentRow, 1).Value = "Total PO Aktif";
            ws.Cell(currentRow, 2).Value = prediction.Scenarios.Count;
            ws.Cell(currentRow, 2).Style.Font.Bold = true;
            currentRow += 2;

            // ========== SECTION 2: PO IMPACT RANKING ==========
            var impactRow = currentRow;
            ws.Cell(impactRow, 1).Value = "RANKING PO BERDASARKAN IMPACT";
            ws.Range(impactRow, 1, impactRow, 6).Merge();
            ws.Cell(impactRow, 1).Style.Font.Bold = true;
            ws.Cell(impactRow, 1).Style.Fill.BackgroundColor = blueFill;
            currentRow++;

            // Header ranking
            string[] rankingHeaders = { "Ranking", "No PO", "No PI", "SO akan Ready", "% dari Total SO", "Keterangan" };
            for (int i = 0; i < rankingHeaders.Length; i++)
            {
                var c = ws.Cell(currentRow, i + 1);
                c.Value = rankingHeaders[i];
                c.Style.Font.Bold = true;
                c.Style.Font.FontColor = XLColor.White;
                c.Style.Fill.BackgroundColor = headerFill;
                c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            currentRow++;

            foreach (var ranking in prediction.Summary.POImpactRanking.OrderBy(r => r.Rank).Take(10))
            {
                var scenario = prediction.Scenarios.FirstOrDefault(s => s.NoLpb == ranking.NoLpb);
                if (scenario == null) continue;

                var rowBg = ranking.Rank == 1 ? yellowFill : (ranking.Rank <= 3 ? greenFill : XLColor.White);

                ws.Cell(currentRow, 1).Value = ranking.Rank;
                ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = rowBg;
                ws.Cell(currentRow, 1).Style.Font.Bold = true;

                ws.Cell(currentRow, 2).Value = ranking.NoLpb;
                ws.Cell(currentRow, 2).Style.Fill.BackgroundColor = rowBg;

                ws.Cell(currentRow, 3).Value = ranking.NoPrj ?? "-";
                ws.Cell(currentRow, 3).Style.Fill.BackgroundColor = rowBg;

                ws.Cell(currentRow, 4).Value = ranking.ImpactCount;
                ws.Cell(currentRow, 4).Style.Fill.BackgroundColor = rowBg;
                ws.Cell(currentRow, 4).Style.Font.Bold = true;
                ws.Cell(currentRow, 4).Style.Font.FontColor = XLColor.FromHtml("#198754");

                ws.Cell(currentRow, 5).Value = ranking.ImpactPercentage / 100;
                ws.Cell(currentRow, 5).Style.Fill.BackgroundColor = rowBg;
                ws.Cell(currentRow, 5).Style.NumberFormat.Format = "0.0%";
                ws.Cell(currentRow, 5).Style.Font.Bold = true;

                var pesan = "";
                if (ranking.Rank == 1) pesan = "⭐ PALING PENTING";
                else if (ranking.Rank <= 3) pesan = "🔥 Prioritas tinggi";
                else if (ranking.Rank <= 5) pesan = "Medium priority";

                ws.Cell(currentRow, 6).Value = pesan;
                ws.Cell(currentRow, 6).Style.Fill.BackgroundColor = rowBg;

                currentRow++;
            }
            currentRow += 1;

            // ========== SECTION 3: CRITICAL ITEMS ==========
            var criticalRow = currentRow;
            ws.Cell(criticalRow, 1).Value = "ITEM YANG PALING CRITICAL (Banyak SO Menunggu)";
            ws.Range(criticalRow, 1, criticalRow, 6).Merge();
            ws.Cell(criticalRow, 1).Style.Font.Bold = true;
            ws.Cell(criticalRow, 1).Style.Fill.BackgroundColor = redFill;
            currentRow++;

            string[] criticalHeaders = { "Item Code", "Nama Item", "Qty Diminta", "Stock Saat Ini", "PO Direncanakan", "Kekurangan" };
            for (int i = 0; i < criticalHeaders.Length; i++)
            {
                var c = ws.Cell(currentRow, i + 1);
                c.Value = criticalHeaders[i];
                c.Style.Font.Bold = true;
                c.Style.Font.FontColor = XLColor.White;
                c.Style.Fill.BackgroundColor = headerFill;
                c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            currentRow++;

            foreach (var critical in prediction.Summary.CriticalItems.Take(10))
            {
                ws.Cell(currentRow, 1).Value = critical.ItemCode;
                ws.Cell(currentRow, 2).Value = critical.NamaItem;
                ws.Cell(currentRow, 3).Value = critical.TotalQtyWaiting;
                ws.Cell(currentRow, 3).Style.Font.Bold = true;
                ws.Cell(currentRow, 4).Value = critical.CurrentStock;
                ws.Cell(currentRow, 5).Value = critical.TotalPOPlanned;
                ws.Cell(currentRow, 5).Style.Font.FontColor = XLColor.FromHtml("#0d6efd");
                var kekurangan = critical.TotalQtyWaiting - critical.CurrentStock;
                ws.Cell(currentRow, 6).Value = Math.Max(kekurangan - critical.TotalPOPlanned, 0);
                ws.Cell(currentRow, 6).Style.Font.FontColor = XLColor.FromHtml("#dc3545");
                ws.Cell(currentRow, 6).Style.Font.Bold = true;

                currentRow++;
            }
            currentRow += 2;

            // ========== SECTION 4: DETAIL SCENARIO PER PO ==========
            var detailHeaderRow = currentRow;
            ws.Cell(detailHeaderRow, 1).Value = "DETAIL PREDIKSI PER PO";
            ws.Range(detailHeaderRow, 1, detailHeaderRow, 8).Merge();
            ws.Cell(detailHeaderRow, 1).Style.Font.Bold = true;
            ws.Cell(detailHeaderRow, 1).Style.Fill.BackgroundColor = blueFill;
            currentRow += 2;

            foreach (var scenario in prediction.Scenarios.OrderByDescending(s => s.ReadySalesOrders.Count))
            {
                // PO Header
                ws.Cell(currentRow, 1).Value = $"PO: {scenario.NoLpb} | {scenario.NoPrj} | Tgl: {scenario.Tanggal:dd/MM/yyyy}";
                ws.Cell(currentRow, 1).Style.Font.Bold = true;
                ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = greenFill;
                ws.Range(currentRow, 1, currentRow, 8).Merge();
                currentRow++;

                // Sub-header: Items dalam PO
                ws.Cell(currentRow, 1).Value = "Item dalam PO:";
                ws.Cell(currentRow, 1).Style.Font.Bold = true;
                ws.Cell(currentRow, 1).Style.Font.Italic = true;
                currentRow++;

                foreach (var item in scenario.Items)
                {
                    ws.Cell(currentRow, 1).Value = "  " + item.ItemCode;
                    ws.Cell(currentRow, 2).Value = item.NamaItem;
                    ws.Cell(currentRow, 3).Value = item.Qty;
                    ws.Cell(currentRow, 4).Value = item.Satuan;
                    currentRow++;
                }
                currentRow++;

                // SO yang akan ready
                ws.Cell(currentRow, 1).Value = $"SO AKAN READY ({scenario.ReadySalesOrders.Count})";
                ws.Cell(currentRow, 1).Style.Font.Bold = true;
                ws.Cell(currentRow, 1).Style.Font.FontColor = XLColor.White;
                ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#198754");
                ws.Range(currentRow, 1, currentRow, 5).Merge();
                currentRow++;

                if (scenario.ReadySalesOrders.Any())
                {
                    foreach (var so in scenario.ReadySalesOrders)
                    {
                        ws.Cell(currentRow, 1).Value = so.NoSO;
                        ws.Cell(currentRow, 2).Value = so.NamaCustomer;
                        ws.Cell(currentRow, 3).Value = so.TanggalSO.ToString("dd/MM/yy");
                        ws.Cell(currentRow, 4).Value = so.NoPrj;
                        ws.Cell(currentRow, 5).Value = so.Keterangan;
                        ws.Range(currentRow, 1, currentRow, 5).Style.Fill.BackgroundColor = lightGrayFill;
                        currentRow++;
                    }
                }
                else
                {
                    ws.Cell(currentRow, 1).Value = "(Tidak ada SO yang akan ready)";
                    ws.Cell(currentRow, 1).Style.Font.Italic = true;
                    ws.Cell(currentRow, 1).Style.Font.FontColor = XLColor.Gray;
                    currentRow++;
                }
                currentRow++;

                // SO yang masih pending
                if (scenario.PendingSalesOrders.Any())
                {
                    ws.Cell(currentRow, 1).Value = $"SO MASIH PENDING ({scenario.PendingSalesOrders.Count})";
                    ws.Cell(currentRow, 1).Style.Font.Bold = true;
                    ws.Cell(currentRow, 1).Style.Font.FontColor = XLColor.White;
                    ws.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#dc3545");
                    ws.Range(currentRow, 1, currentRow, 6).Merge();
                    currentRow++;

                    foreach (var so in scenario.PendingSalesOrders.Take(5))
                    {
                        ws.Cell(currentRow, 1).Value = so.NoSO;
                        ws.Cell(currentRow, 2).Value = so.NamaCustomer;
                        ws.Cell(currentRow, 3).Value = string.Join(", ", so.MissingItems);
                        ws.Cell(currentRow, 4).Value = so.ReasonIfStillPending ?? "-";
                        ws.Range(currentRow, 1, currentRow, 4).Style.Fill.BackgroundColor = lightGrayFill;
                        currentRow++;
                    }
                    currentRow++;
                }

                currentRow += 1;
            }

            // Formatting
            ws.Columns().AdjustToContents();
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 18;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 20;
            ws.Column(6).Width = 25;
            ws.Column(7).Width = 15;
            ws.Column(8).Width = 20;

            // Add border to all cells
            var allCells = ws.Range(1, 1, currentRow, 8);
            allCells.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            allCells.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Freeze panes at row 4 untuk header menus tetap terlihat
            ws.SheetView.FreezeRows(4);
        }

        /// <summary>
        /// Build sheet SO Progression - menampilkan status SO seiring kedatangan PI
        /// </summary>
        private void BuildSOProgressionSheet(XLWorkbook workbook, SOProgressionView progression)
        {
            var ws = workbook.Worksheets.Add("SO Progression Per PI");

            // Color palette
            var headerFill = XLColor.FromHtml("#212529");
            var greenFill = XLColor.FromHtml("#d1e7dd");
            var redFill = XLColor.FromHtml("#f8d7da");
            var yellowFill = XLColor.FromHtml("#fff3cd");
            var lightGrayFill = XLColor.FromHtml("#f8f9fa");

            int currentRow = 1;
            int currentCol = 1;

            // Title
            var titleCell = ws.Cell(currentRow, 1);
            titleCell.Value = $"Status SO Progression seiring Kedatangan PI - {DateTime.Today:dd MMM yyyy}";
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontColor = XLColor.White;
            titleCell.Style.Fill.BackgroundColor = headerFill;
            ws.Range(currentRow, 1, currentRow, 8 + progression.PIsInOrder.Count).Merge();
            ws.Range(currentRow, 1, currentRow, 8 + progression.PIsInOrder.Count).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            currentRow += 2;

            // Header columns
            var headerRow = currentRow;
            currentCol = 1;

            string[] fixedHeaders = { "No. Urut", "No SO", "Customer", "Tanggal Order", "Item Dipesan", "Status Qty Sekarang", "Keterangan", "Catatan SO" };
            foreach (var header in fixedHeaders)
            {
                var c = ws.Cell(currentRow, currentCol);
                c.Value = header;
                c.Style.Font.Bold = true;
                c.Style.Font.FontColor = XLColor.White;
                c.Style.Fill.BackgroundColor = headerFill;
                c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                c.Style.Alignment.WrapText = true;
                currentCol++;
            }

            // Add PI headers
            int piStartCol = currentCol;
            foreach (var pi in progression.PIsInOrder)
            {
                var c = ws.Cell(currentRow, currentCol);
                var piHeaderText = $"Status setelah\n{pi.NoPrj}\n({pi.Tanggal:dd-MMM-yyyy})";
                if (!string.IsNullOrWhiteSpace(pi.Keterangan))
                    piHeaderText += $"\n{pi.Keterangan}";
                c.Value = piHeaderText;
                c.Style.Font.Bold = true;
                c.Style.Font.FontColor = XLColor.White;
                c.Style.Fill.BackgroundColor = headerFill;
                c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                c.Style.Alignment.WrapText = true;
                currentCol++;
            }

            // Set header row height
            ws.Row(currentRow).Height = 55;
            currentRow++;

            // Data rows
            foreach (var row in progression.Rows)
            {
                currentCol = 1;
                var rowColor = row.ProgressionPerPI.Values.Last().IsComplete ? yellowFill : XLColor.White;

                // No. Urut
                ws.Cell(currentRow, currentCol).Value = row.NoUrut;
                ws.Cell(currentRow, currentCol).Style.Fill.BackgroundColor = rowColor;
                ws.Cell(currentRow, currentCol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                currentCol++;

                // No SO
                ws.Cell(currentRow, currentCol).Value = row.NoSO;
                ws.Cell(currentRow, currentCol).Style.Fill.BackgroundColor = rowColor;
                currentCol++;

                // Customer
                ws.Cell(currentRow, currentCol).Value = row.NamaCustomer;
                ws.Cell(currentRow, currentCol).Style.Fill.BackgroundColor = rowColor;
                currentCol++;

                // Tanggal
                ws.Cell(currentRow, currentCol).Value = row.TanggalSO.ToString("dd/MM/yyyy");
                ws.Cell(currentRow, currentCol).Style.Fill.BackgroundColor = rowColor;
                currentCol++;

                // Item Dipesan - format dengan line break dan qty
                var itemDipesanCell = ws.Cell(currentRow, currentCol);
                var itemsFormatted = string.Join("\n", row.ItemStatusSekarang
                    .Select(i => $"{i.NamaItem} ({i.ItemCode})\n{(int)i.QtyOrder} {i.Satuan ?? ""}"));
                itemDipesanCell.Value = itemsFormatted;
                itemDipesanCell.Style.Alignment.WrapText = true;
                itemDipesanCell.Style.Fill.BackgroundColor = rowColor;
                currentCol++;

                // Status Qty Sekarang
                var statusSekarangCell = ws.Cell(currentRow, currentCol);
                var missingItemsText = string.Join("\n", row.ItemStatusSekarang
                    .Where(i => !i.IsComplete)
                    .Select(i => $"✗ {i.NamaItem} ({i.ItemCode}) kurang {(int)i.QtyKurang}"));

                if (string.IsNullOrWhiteSpace(missingItemsText))
                {
                    statusSekarangCell.Value = "✓ Lengkap";
                    statusSekarangCell.Style.Font.FontColor = XLColor.FromHtml("#198754");
                }
                else
                {
                    statusSekarangCell.Value = missingItemsText;
                    statusSekarangCell.Style.Font.FontColor = XLColor.FromHtml("#dc3545");
                }
                statusSekarangCell.Style.Alignment.WrapText = true;
                statusSekarangCell.Style.Fill.BackgroundColor = rowColor;
                currentCol++;

                // Keterangan
                ws.Cell(currentRow, currentCol).Value = row.StatusSekarang;
                ws.Cell(currentRow, currentCol).Style.Fill.BackgroundColor = rowColor;
                currentCol++;

                // Catatan SO
                ws.Cell(currentRow, currentCol).Value = row.CatatanSO;
                ws.Cell(currentRow, currentCol).Style.Fill.BackgroundColor = rowColor;
                ws.Cell(currentRow, currentCol).Style.Alignment.WrapText = true;
                currentCol++;

                // PI progression columns - with auto-empty logic
                int piColIndex = 0;
                var piCol = piStartCol;
                int completedAtPiIndex = -1;

                // Check if SO is already complete in current status
                var statusValue = statusSekarangCell.Value.ToString();
                var isAlreadyComplete = statusValue == "✓ Lengkap";

                if (isAlreadyComplete)
                {
                    // SO already complete, no need to show PI status - all PI cols should be empty with green
                    completedAtPiIndex = -1; // Mark as already complete before first PI
                }
                else
                {
                    // Find at which PI the SO becomes complete
                    foreach (var pi in progression.PIsInOrder)
                    {
                        if (row.ProgressionPerPI.TryGetValue(pi.NoPrj, out var piStatus) && piStatus.IsComplete)
                        {
                            completedAtPiIndex = piColIndex;
                            break;
                        }
                        piColIndex++;
                    }
                }

                piColIndex = 0;
                foreach (var pi in progression.PIsInOrder)
                {
                    if (row.ProgressionPerPI.TryGetValue(pi.NoPrj, out var piStatus))
                    {
                        var piCell = ws.Cell(currentRow, piCol);

                        // If SO was already complete before first PI (in current status), all PI cols empty with green
                        if (completedAtPiIndex == -1)
                        {
                            piCell.Value = "";
                            piCell.Style.Fill.BackgroundColor = YellowGreenFill();
                        }
                        // If SO was already complete in previous PI, just show empty with green background
                        else if (completedAtPiIndex >= 0 && piColIndex > completedAtPiIndex)
                        {
                            piCell.Value = "";
                            piCell.Style.Fill.BackgroundColor = YellowGreenFill();
                        }
                        else if (piStatus.IsComplete)
                        {
                            piCell.Value = "✓ Lengkap";
                            piCell.Style.Fill.BackgroundColor = YellowGreenFill();
                        }
                        else if (piStatus.NewlyCompletedItems.Any())
                        {
                            var newlyCompletedFormatted = FormatItemListWithLineBreak(piStatus.NewlyCompletedItems, row.ItemStatusSekarang);
                            var stillMissingFormatted = FormatItemListWithLineBreak(piStatus.StillMissingItems, row.ItemStatusSekarang);
                            var text = $"✓ Selesai:\n{newlyCompletedFormatted}\n✗ Masih Kurang:\n{stillMissingFormatted}";
                            piCell.Value = text;
                            piCell.Style.Fill.BackgroundColor = yellowFill;
                        }
                        else
                        {
                            var stillMissingFormatted = FormatItemListWithLineBreak(piStatus.StillMissingItems, row.ItemStatusSekarang);
                            var text = $"✗ Kurang:\n{stillMissingFormatted}";
                            piCell.Value = text;
                            piCell.Style.Fill.BackgroundColor = rowColor;
                        }

                        piCell.Style.Alignment.WrapText = true;
                        piCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    piCol++;
                    piColIndex++;
                }

                currentRow++;
            }

            // Formatting
            ws.Columns().AdjustToContents();
            ws.Column(1).Width = 10;    // No. Urut
            ws.Column(2).Width = 18;    // No SO
            ws.Column(3).Width = 16;    // Customer
            ws.Column(4).Width = 14;    // Tanggal Order
            ws.Column(5).Width = 24;    // Item Dipesan
            ws.Column(6).Width = 22;    // Status Qty Sekarang
            ws.Column(7).Width = 16;    // Keterangan
            ws.Column(8).Width = 24;    // Catatan SO

            for (int i = piStartCol; i < piStartCol + progression.PIsInOrder.Count; i++)
                ws.Column(i).Width = 22;

            // Add borders
            var allCells = ws.Range(1, 1, currentRow - 1, piStartCol + progression.PIsInOrder.Count - 1);
            allCells.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            allCells.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            allCells.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            // Freeze panes
            ws.SheetView.FreezeRows(3);
        }

        /// <summary>
        /// Helper untuk format item list dengan nama dan kode dalam kurung (dengan comma)
        /// </summary>
        private string FormatItemList(List<string> itemCodes, List<ItemStatus> itemStatusMap)
        {
            if (itemCodes == null || itemCodes.Count == 0)
                return "";

            var formatted = new List<string>();
            var itemMap = itemStatusMap.ToDictionary(i => i.ItemCode, StringComparer.OrdinalIgnoreCase);

            foreach (var code in itemCodes)
            {
                if (itemMap.TryGetValue(code, out var item))
                    formatted.Add($"{item.NamaItem} ({code})");
                else
                    formatted.Add(code);
            }

            return string.Join(", ", formatted);
        }

        /// <summary>
        /// Helper untuk format item list dengan nama dan kode dalam kurung (dengan line break)
        /// </summary>
        private string FormatItemListWithLineBreak(List<string> itemCodes, List<ItemStatus> itemStatusMap)
        {
            if (itemCodes == null || itemCodes.Count == 0)
                return "";

            var formatted = new List<string>();
            var itemMap = itemStatusMap.ToDictionary(i => i.ItemCode, StringComparer.OrdinalIgnoreCase);

            foreach (var code in itemCodes)
            {
                if (itemMap.TryGetValue(code, out var item))
                    formatted.Add($"{item.NamaItem} ({code})");
                else
                    formatted.Add(code);
            }

            return string.Join("\n", formatted);
        }

        /// <summary>
        /// Apply multi-color rich text formatting untuk PI progression cells
        /// Menggunakan approach sederhana: memberikan warna consistent untuk setiap type
        /// </summary>


        /// <summary>
        /// Helper untuk yellow-green color
        /// </summary>
        private XLColor YellowGreenFill()
        {
            return XLColor.FromHtml("#d4edda");
        }

        private PoReadinessPredictionView GeneratePOReadinessPrediction(
            SalesOrderStockMatrixView matrix,
            List<PoTransH> purchaseOrders,
            List<PoTransD> purchaseDetails)
        {
            var prediction = new PoReadinessPredictionView();

            // Group PO details by document
            var poByDocNo = purchaseDetails
                .Where(d => !string.IsNullOrWhiteSpace(d.NoLpb))
                .GroupBy(d => d.NoLpb, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            // Current stock state
            var currentStockByItem = matrix.ItemHeaders
                .ToDictionary(h => h.ItemCode, h => h.QtyStock, StringComparer.OrdinalIgnoreCase);

            var readySOCount = matrix.Rows.Count(r => r.IsComplete);
            var totalSOCount = matrix.Rows.Count;

            prediction.Summary.TotalSalesOrders = totalSOCount;
            prediction.Summary.ReadyWithoutPO = readySOCount;

            // Analisis per PO aktif
            var impactRankings = new List<POImpactRanking>();
            int rank = 1;

            foreach (var po in purchaseOrders.Where(p => !string.IsNullOrWhiteSpace(p.NoLpb)))
            {
                var scenario = CalculateSinglePOScenario(po, poByDocNo, matrix, currentStockByItem);
                if (scenario != null)
                {
                    prediction.Scenarios.Add(scenario);

                    // Track impact
                    var impactCount = scenario.ReadySalesOrders.Count;
                    if (impactCount > 0)
                    {
                        impactRankings.Add(new POImpactRanking
                        {
                            NoLpb = po.NoLpb,
                            NoPrj = po.NoPrj,
                            ImpactCount = impactCount,
                            ImpactPercentage = totalSOCount > 0 ? (decimal)impactCount / totalSOCount * 100 : 0,
                            Rank = rank++
                        });
                    }
                }
            }

            // Sort by impact
            impactRankings = impactRankings.OrderByDescending(r => r.ImpactCount).ToList();
            for (int i = 0; i < impactRankings.Count; i++)
                impactRankings[i].Rank = i + 1;

            prediction.Summary.POImpactRanking = impactRankings;
            prediction.Summary.AverageSOReadyPerPO = impactRankings.Count > 0
                ? (decimal)impactRankings.Sum(r => r.ImpactCount) / impactRankings.Count
                : 0;
            prediction.Summary.MostImpactfulPO = impactRankings.FirstOrDefault()?.NoLpb;

            // Identifikasi critical items
            var criticalItems = new List<CriticalItemAnalysis>();
            foreach (var item in matrix.ItemHeaders)
            {
                var soWaiting = matrix.Rows.Where(r => !r.IsComplete)
                    .SelectMany(r => r.Cells)
                    .Where(c => c.ItemCode == item.ItemCode && c.IsOrdered && !c.HasStock)
                    .ToList();

                if (soWaiting.Any())
                {
                    criticalItems.Add(new CriticalItemAnalysis
                    {
                        ItemCode = item.ItemCode,
                        NamaItem = item.NamaItem,
                        CountSOWaiting = matrix.Rows.Count(r => !r.IsComplete && 
                            r.Cells.Any(c => c.ItemCode == item.ItemCode && c.IsOrdered && !c.HasStock)),
                        TotalQtyWaiting = soWaiting.Sum(c => c.QtyOrder),
                        CurrentStock = item.QtyStock,
                        TotalPOPlanned = purchaseDetails
                            .Where(d => string.Equals(d.ItemCode, item.ItemCode, StringComparison.OrdinalIgnoreCase))
                            .Sum(d => d.Qty)
                    });
                }
            }

            prediction.Summary.CriticalItems = criticalItems
                .OrderByDescending(c => c.CountSOWaiting)
                .ThenByDescending(c => c.TotalQtyWaiting)
                .ToList();

            return prediction;
        }

        /// <summary>
        /// Hitung skenario untuk satu PO: SO mana yang akan ready jika PO ini datang
        /// </summary>
        private PoPredictionScenario CalculateSinglePOScenario(
            PoTransH po,
            Dictionary<string, List<PoTransD>> poByDocNo,
            SalesOrderStockMatrixView matrix,
            Dictionary<string, decimal> currentStockByItem)
        {
            if (string.IsNullOrWhiteSpace(po.NoLpb) || !poByDocNo.TryGetValue(po.NoLpb, out var details))
                return null;

            var scenario = new PoPredictionScenario
            {
                NoLpb = po.NoLpb,
                Tanggal = po.Tanggal,
                NoPrj = po.NoPrj,
                NamaSupplier = !string.IsNullOrWhiteSpace(po.NamaVendor) ? po.NamaVendor : "Not Specified"
            };

            // Build PO items
            foreach (var detail in details)
            {
                if (!string.IsNullOrWhiteSpace(detail.ItemCode))
                {
                    scenario.Items.Add(new PoPredictionItem
                    {
                        ItemCode = detail.ItemCode,
                        NamaItem = detail.NamaItem,
                        Qty = detail.Qty,
                        Satuan = detail.Satuan,
                        NeededQty = 0 // Will be calculated below
                    });
                }
            }

            // Calculate stock after this PO arrives
            var stockAfterPO = new Dictionary<string, decimal>(currentStockByItem, StringComparer.OrdinalIgnoreCase);
            foreach (var item in scenario.Items)
            {
                if (stockAfterPO.TryGetValue(item.ItemCode, out var currentStock))
                    stockAfterPO[item.ItemCode] = currentStock + item.Qty;
                else
                    stockAfterPO[item.ItemCode] = item.Qty;
            }

            // Evaluate which SO will be ready
            foreach (var so in matrix.Rows)
            {
                if (!so.IsComplete) // Only check incomplete SO
                {
                    var canBeReady = true;
                    var willBeCompletedItems = new List<string>();
                    var stillMissingItems = new List<string>();

                    foreach (var cell in so.Cells.Where(c => c.IsOrdered))
                    {
                        var stockAfterThis = stockAfterPO.TryGetValue(cell.ItemCode, out var s) ? s : 0;

                        if (stockAfterThis >= cell.QtyOrder)
                        {
                            willBeCompletedItems.Add(cell.ItemCode);
                        }
                        else
                        {
                            canBeReady = false;
                            stillMissingItems.Add(cell.ItemCode);
                        }
                    }

                    var result = new SOReadinessResult
                    {
                        NoSO = so.NoLpb,
                        NamaCustomer = so.NamaCustomer,
                        TanggalSO = so.Tanggal,
                        NoPrj = so.NoPrj,
                        Keterangan = so.Keterangan,
                        MissingItems = so.Cells.Where(c => c.IsOrdered && 
                            (!stockAfterPO.TryGetValue(c.ItemCode, out var stock) || stock < c.QtyOrder))
                            .Select(c => c.ItemCode)
                            .ToList(),
                        WillBeCompletedItems = willBeCompletedItems
                    };

                    if (canBeReady)
                    {
                        scenario.ReadySalesOrders.Add(result);
                    }
                    else if (willBeCompletedItems.Any())
                    {
                        // Ada progress tapi belum lengkap
                        var missingItem = stillMissingItems.FirstOrDefault();
                        result.ReasonIfStillPending = $"Masih kurang: {missingItem}";
                        scenario.PendingSalesOrders.Add(result);
                    }
                    else
                    {
                        scenario.PendingSalesOrders.Add(result);
                    }
                }
            }

            if (matrix.Rows.Count > 0)
                scenario.PercentageReady = (decimal)scenario.ReadySalesOrders.Count / matrix.Rows.Count * 100;

            return scenario;
        }

        /// <summary>
        /// Generate data progression SO seiring kedatangan PI yang berbeda
        /// Dengan sequential stock allocation: SO pertama mendapat prioritas pertama
        /// </summary>
        private SOProgressionView GenerateSOProgressionData(
            SalesOrderStockMatrixView matrix,
            List<PoTransH> purchaseOrders,
            List<PoTransD> purchaseDetails)
        {
            var progression = new SOProgressionView();

            // Get unique PIs in order
            progression.PIsInOrder = GetActivePIsInOrder(purchaseOrders, purchaseDetails);

            // Build initial stock availability
            var baseStock = matrix.ItemHeaders
                .ToDictionary(h => h.ItemCode, h => h.QtyStock, StringComparer.OrdinalIgnoreCase);

            // Convert matrix rows to list for multiple passes
            var soRows = matrix.Rows.ToList();

            // FIRST PASS: Calculate current status with sequential allocation
            var currentRemainingStock = new Dictionary<string, decimal>(baseStock, StringComparer.OrdinalIgnoreCase);
            var soAllocationMap = new Dictionary<int, Dictionary<string, decimal>>(); // soIndex -> item allocations

            int soIndex = 0;
            foreach (var soRow in soRows)
            {
                var soAllocations = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

                // For each item in this SO, allocate from remaining stock
                foreach (var cell in soRow.Cells.Where(c => c.IsOrdered))
                {
                    var needed = cell.QtyOrder;
                    var available = currentRemainingStock.TryGetValue(cell.ItemCode, out var stock) ? stock : 0;
                    var allocated = Math.Min(needed, available);

                    soAllocations[cell.ItemCode] = allocated;

                    // Reduce remaining stock for next SO
                    if (currentRemainingStock.TryGetValue(cell.ItemCode, out _))
                        currentRemainingStock[cell.ItemCode] -= allocated;
                    else
                        currentRemainingStock[cell.ItemCode] = 0;
                }

                soAllocationMap[soIndex] = soAllocations;
                soIndex++;
            }

            // SECOND PASS: Create SOProgressionRow with allocated quantities
            soIndex = 0;
            foreach (var soRow in soRows)
            {
                soIndex++;
                var soProgression = new SOProgressionRow
                {
                    NoUrut = soIndex,
                    NoSO = soRow.NoLpb,
                    NamaCustomer = soRow.NamaCustomer,
                    TanggalSO = soRow.Tanggal,
                    StatusSekarang = soRow.IsComplete ? "Lengkap" : "Belum",
                    CatatanSO = soRow.Keterangan ?? ""
                };

                var allocations = soAllocationMap[soIndex - 1];
                var missingItems = new List<ItemStatus>();

                // Calculate status with allocated stock
                foreach (var cell in soRow.Cells.Where(c => c.IsOrdered))
                {
                    var allocated = allocations.TryGetValue(cell.ItemCode, out var qty) ? qty : 0;
                    var kurang = Math.Max(cell.QtyOrder - allocated, 0);
                    var itemHeader = matrix.ItemHeaders.FirstOrDefault(h => h.ItemCode == cell.ItemCode);

                    soProgression.ItemStatusSekarang.Add(new ItemStatus
                    {
                        ItemCode = cell.ItemCode,
                        NamaItem = itemHeader?.NamaItem ?? "",
                        Satuan = itemHeader?.Satuan ?? "",
                        QtyOrder = cell.QtyOrder,
                        QtyAvailable = allocated,
                        QtyKurang = kurang
                    });

                    if (kurang > 0)
                        missingItems.Add(new ItemStatus
                        {
                            ItemCode = cell.ItemCode,
                            QtyKurang = kurang
                        });
                }

                // Count missing items for summary
                if (missingItems.Count == 0)
                    soProgression.StatusSekarang = "Lengkap";
                else if (missingItems.Count == soProgression.ItemStatusSekarang.Count)
                    soProgression.StatusSekarang = "Banyak Kurang";
                else
                    soProgression.StatusSekarang = "Sebagian Kurang";

                // PI Progression - will be calculated after all SOs are processed
                soProgression.ProgressionPerPI = new Dictionary<string, PIProgressionStatus>();

                progression.Rows.Add(soProgression);
            }

            // THIRD PASS: Calculate PI progression for each SO with sequential allocation per PI
            soIndex = 0;
            foreach (var soRow in soRows)
            {
                var soProgression = progression.Rows[soIndex];

                foreach (var pi in progression.PIsInOrder)
                {
                    // Build cumulative stock: baseStock + all prior PIs + this PI
                    var piRemainingStock = new Dictionary<string, decimal>(baseStock, StringComparer.OrdinalIgnoreCase);

                    // Add stock from all PIs up to and including this one
                    var allPisTillNow = progression.PIsInOrder.TakeWhile(p => p.NoPrj != pi.NoPrj).Append(pi).ToList();
                    foreach (var currentPi in allPisTillNow)
                    {
                        var piPOs = purchaseOrders.Where(p => p.NoPrj == currentPi.NoPrj).ToList();
                        var piDetails = purchaseDetails.Where(d => piPOs.Any(p => p.NoLpb == d.NoLpb)).ToList();

                        foreach (var detail in piDetails)
                        {
                            if (!string.IsNullOrWhiteSpace(detail.ItemCode))
                            {
                                if (piRemainingStock.TryGetValue(detail.ItemCode, out var stock))
                                    piRemainingStock[detail.ItemCode] = stock + detail.Qty;
                                else
                                    piRemainingStock[detail.ItemCode] = detail.Qty;
                            }
                        }
                    }

                    // Allocate sequentially: prior SOs consume first, then this SO
                    var piAllocated = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < soIndex; i++)
                    {
                        var priorSO = soRows[i];
                        foreach (var cell in priorSO.Cells.Where(c => c.IsOrdered))
                        {
                            var needed = cell.QtyOrder;
                            var available = piRemainingStock.TryGetValue(cell.ItemCode, out var stock) ? stock : 0;
                            var allocated = Math.Min(needed, available);

                            if (piRemainingStock.TryGetValue(cell.ItemCode, out _))
                                piRemainingStock[cell.ItemCode] -= allocated;
                            else
                                piRemainingStock[cell.ItemCode] = 0;
                        }
                    }

                    // Now allocate for this SO
                    foreach (var cell in soRow.Cells.Where(c => c.IsOrdered))
                    {
                        var needed = cell.QtyOrder;
                        var available = piRemainingStock.TryGetValue(cell.ItemCode, out var stock) ? stock : 0;
                        var allocated = Math.Min(needed, available);
                        piAllocated[cell.ItemCode] = allocated;

                        if (piRemainingStock.TryGetValue(cell.ItemCode, out _))
                            piRemainingStock[cell.ItemCode] -= allocated;
                        else
                            piRemainingStock[cell.ItemCode] = 0;
                    }

                    // Check readiness
                    var stillMissing = new List<string>();
                    var newlyCompleted = new List<string>();
                    var isReady = true;

                    foreach (var cell in soRow.Cells.Where(c => c.IsOrdered))
                    {
                        var allocatedQty = piAllocated.TryGetValue(cell.ItemCode, out var qty) ? qty : 0;
                        var currentAllocated = soAllocationMap[soIndex].TryGetValue(cell.ItemCode, out var curQty) ? curQty : 0;
                        var wasMissing = currentAllocated < cell.QtyOrder;
                        var isNowMissing = allocatedQty < cell.QtyOrder;

                        if (isNowMissing)
                        {
                            stillMissing.Add(cell.ItemCode);
                            isReady = false;
                        }
                        else if (wasMissing)
                        {
                            newlyCompleted.Add(cell.ItemCode);
                        }
                    }

                    var piStatus = new PIProgressionStatus
                    {
                        NoPrj = pi.NoPrj,
                        IsComplete = isReady,
                        NewlyCompletedItems = newlyCompleted,
                        StillMissingItems = stillMissing,
                        StatusSummary = isReady
                            ? "✓ Lengkap"
                            : (newlyCompleted.Any() ? $"~ Progres: {string.Join(", ", newlyCompleted)}" : $"✗ Kurang: {string.Join(", ", stillMissing)}")
                    };

                    soProgression.ProgressionPerPI[pi.NoPrj] = piStatus;
                }

                soIndex++;
            }

            return progression;
        }

        /// <summary>
        /// Get list of active PIs sorted by tanggal (earliest first)
        /// </summary>
        private List<PIInfo> GetActivePIsInOrder(
            List<PoTransH> purchaseOrders,
            List<PoTransD> purchaseDetails)
        {
            var piMap = new Dictionary<string, PIInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var po in purchaseOrders.Where(p => !string.IsNullOrWhiteSpace(p.NoPrj)))
            {
                if (!piMap.ContainsKey(po.NoPrj))
                {
                    var details = purchaseDetails.Where(d => d.NoLpb == po.NoLpb).ToList();
                    var totalQty = details.Sum(d => d.Qty);

                    piMap[po.NoPrj] = new PIInfo
                    {
                        NoPrj = po.NoPrj,
                        NamaVendor = po.NamaVendor,
                        TotalQty = totalQty,
                        Tanggal = po.Tanggal,
                        Keterangan = po.Keterangan,
                        PiIndex = 0 // Will be set after sorting
                    };
                }
            }

            // Sort by Tanggal (earliest first)
            var sorted = piMap.Values
                .OrderBy(p => p.Tanggal)
                .ToList();

            // Set index after sorting
            for (int i = 0; i < sorted.Count; i++)
                sorted[i].PiIndex = i;

            return sorted;
        }

        #endregion
    }
}
