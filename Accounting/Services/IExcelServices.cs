using ClosedXML.Excel;
using eSoft.Financial.View;
using eSoft.Hutang.Model;
using eSoft.Order.Model;
using eSoft.Order.View;
using eSoft.Penjualan.Model;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;
using eSoft.Piutang.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Services
{
    public interface IExcelServices
    {
        byte[] CreatePenjualanExport(List<OeTransD> penjualan);
        byte[] CreateCustomerWorksheet(List<ArCust> transaksi);
        byte[] CreateKurirWorksheet(List<OeKurir> transaksi);
        byte[] CreateLedgerExport(List<FcTransDxls> ledger);
        byte[] CreateHargaPIWorksheet(List<PoTransH> transHeader, List<PoItemQtyByLocationView> transItemLokasi);
        byte[] CreateHutangWorksheet(List<ApHutang> ledger);
        byte[] CreateRekapStockWorksheet(List<IcRekapStock> rekapStock);
        byte[] CreateMutasiLokasiWorksheet(List<IcItem> mutasiItems, List<IcItemQtyByLocationView> lokasiItems);

        byte[] CreateRekapStockPjlSlsPpnWorksheet(List<IcRekapStock> rekapStock, DateTime tanggal1, DateTime tanggal2);
        byte[] CreateSalesOrderStockExcel(SalesOrderStockMatrixView matrix);
        byte[] CreateAgingPiutangExcel(List<eSoft.Piutang.View.ArAgingView> aging, List<eSoft.Piutang.View.ArForecastPiutangView> forecast, List<eSoft.Piutang.View.ArCustomerAnalysisView> analisa = null);
    }
}
