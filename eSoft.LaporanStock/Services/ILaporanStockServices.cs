using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;
using eSoft.Pembelian.Data;
using eSoft.Pembelian.Model;
using eSoft.Pembelian.View;
using eSoft.Piutang.Model;
using eSoft.Piutang.View;

using Microsoft.EntityFrameworkCore;
using static eSoft.LaporanStock.Services.LaporanStockServices;

namespace eSoft.LaporanStock.Services
{
    public interface ILaporanStockServices
    {
        List<IcStockCardView> CetakMutasi(DateTime Tanggal1, DateTime Tanggal2, string kodeBank);
        Task prosesStock(IProgress<string> progress = null);
        Task ProsesSalesPiutang();

        void ProsesRubahKodeItem(string Barang, string UbahItem);
        List<IcItem> CetakItemSupplier(DateTime Tanggal1, DateTime Tanggal2);
        List<IcItem> CetakItemCustomer(DateTime Tanggal1, DateTime Tanggal2);
        List<ArCust> CetakCustomerItem(DateTime Tanggal1, DateTime Tanggal2);
        List<OeKurir> CetakKurirCustomer(DateTime Tanggal1, DateTime Tanggal2);
        List<OeSalesman> CetakSalesCustomer(DateTime Tanggal1, DateTime Tanggal2);
        List<OePerTahun> ItemPertahun(int tahun, List<string> kodeDiv);
        List<OePerTahun> ItemTidakLakuPertahun(int tahun, List<string> kodeDiv);
        List<OePerTahun> PenjualanperTahun(int tahun);
        List<OePerTahun> DivisiPertahun(int tahun, List<string> kodeDiv);
       byte[] CustomerPerDivision(List<string> kodeDiv);
        List<IcRekapStock> RekapStock(DateTime Tanggal1, DateTime Tanggal2);
        Task<List<ResultLunasView>> GetPenjualanLunasAsync(DateTime tanggalLunas1, DateTime tanggalLunas2);
    }
}
