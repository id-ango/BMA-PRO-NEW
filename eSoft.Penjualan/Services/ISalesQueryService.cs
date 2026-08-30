using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eSoft.Penjualan.Model;
using eSoft.Piutang.Model;

namespace eSoft.Penjualan.Services
{
    public interface ISalesQueryService
    {
        ArPiutng GetPiutang(string bukti);
        OeTransH GetOeTrans(int id);
        OeTransH GetOeTransDokumen(string id);
        List<OeTransH> GetFirstTransH();
        List<OeTransH> GetFirstTransHNon();
        Task<List<OeTransH>> GetTransH(DateTime tanggalAwal, DateTime tanggalAkhir);
        Task<List<OeTransH>> GetTransHNon(DateTime tanggalAwal, DateTime tanggalAkhir);
        List<OeTransH> Get3TransH();
        List<OeTransD> GetTransD();
        Task<List<OeTransH>> GetTransKurirAsync(int? top = null);
        void SimpanKurir(OeTransH transaksi);
        void SimpanSalesman(OeTransH transaksi);
        List<OeTransD> GetOeTransDByDokumen(string dokumen);
    }
}
