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

        public ExcelServices(DbContextJual context)
        {
            _context = context;
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
                c.Value = $"{item.ItemCode}\n{item.NamaItem}\nStk Awal: {item.QtyStock:N0} {item.Satuan}";
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
                var sisaAkhir = item.QtyStock - totalOrder;
                var xlCell = wsMatrix.Cell(row, footerCol);
                xlCell.Value = $"Order: {totalOrder:N0}\nSisa: {sisaAkhir:N0}";
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

            string[] summaryHeaders = { "No", "Kode Item", "Nama Item", "Satuan", "Stock Tersedia", "Total Dipesan (SO)", "Sisa Stock", "Kekurangan", "Saran Pesan" };
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
                var sisa = item.QtyStock - totalDipesan;
                var kekurangan = sisa < 0 ? Math.Abs(sisa) : 0;

                wsSummary.Cell(sRow, 1).Value = no++;
                wsSummary.Cell(sRow, 2).Value = item.ItemCode;
                wsSummary.Cell(sRow, 3).Value = item.NamaItem;
                wsSummary.Cell(sRow, 4).Value = item.Satuan;
                wsSummary.Cell(sRow, 5).Value = item.QtyStock;
                wsSummary.Cell(sRow, 6).Value = totalDipesan;
                wsSummary.Cell(sRow, 7).Value = sisa;
                wsSummary.Cell(sRow, 8).Value = kekurangan;
                wsSummary.Cell(sRow, 9).Value = kekurangan > 0 ? $"Perlu pesan min. {kekurangan:N0} {item.Satuan}" : "Cukup";

                // warna baris
                var bg = kekurangan > 0 ? redFill : greenFill;
                for (int i = 1; i <= 9; i++)
                    wsSummary.Cell(sRow, i).Style.Fill.BackgroundColor = bg;

                // warna khusus kolom sisa & kekurangan
                wsSummary.Cell(sRow, 7).Style.Font.FontColor = sisa < 0 ? XLColor.FromHtml("#dc3545") : XLColor.FromHtml("#198754");
                wsSummary.Cell(sRow, 7).Style.Font.Bold = true;
                wsSummary.Cell(sRow, 8).Style.Font.FontColor = kekurangan > 0 ? XLColor.FromHtml("#dc3545") : XLColor.Gray;
                wsSummary.Cell(sRow, 9).Style.Font.Bold = kekurangan > 0;

                sRow++;
            }

            wsSummary.Columns().AdjustToContents();
            wsSummary.Column(3).Width = 30;
            wsSummary.Column(9).Width = 28;

            // border semua sel terisi
            var matrixRange = wsMatrix.Range(1, 1, row, col - 1);
            matrixRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            matrixRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            var summaryRange = wsSummary.Range(1, 1, sRow - 1, 9);
            summaryRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            summaryRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

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
    }
}
