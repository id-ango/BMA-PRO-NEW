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

namespace eSoft.Penjualan.Services
{
    public interface ISalesServices
    {
        OeTransH GetOeTrans(int id);
        List<OeTransH> GetFirstTransH();  // ← TAMBAHKAN INI (untuk Pajak)
        List<OeTransH> GetFirstTransHNon();  // (untuk Non-Pajak)
        Task<List<OeTransH>> GetTransH(DateTime tanggalAwal, DateTime tanggalAkhir);
        Task<List<OeTransH>> GetTransHNon(DateTime tgl1, DateTime tgl2);
        List<OeTransH> Get3TransH();
        List<OeTransD> GetTransD();
        OeTransH AddTransH(OeTransHView trans, bool pajak);
        OeTransH AddTransHRetur(OeTransHView trans, bool pajak);
        Task<bool> DelTransH(int id);
        bool EditTransH(OeTransHView trans);
        List<OeTransH> Laporan1(DateTime tgl1, DateTime tgl2);
        List<OeTransD> LaporanDownload(DateTime tgl1, DateTime tgl2);
        List<OeTransD> Detail1(int xKdHeader);
        List<OeTrans> Detail2(string xKdHeader, DateTime tgl1, DateTime tgl2);
        List<OeTrans> Detail3(string xKdHeader, DateTime tgl1, DateTime tgl2);
        List<OeTrans> Detail3Index(string xKdHeader);
        List<OeTrans> Detail4(string xKdHeader, DateTime tgl1, DateTime tgl2);
        List<OeTrans> Detail5(string xKdHeader, DateTime tgl1, DateTime tgl2);
        OeTransH GetOeTransDokumen(string id);
        bool CekPiutang(OeTransH trans);
        List<OeSalesman> GetSalesman();
        string GetSalesmanKode(string id);
        OeSalesman GetSalesmanId(int id);
        Task<bool> DelSalesman(int kurirs);
        bool CekKdSalesman(string kurir);
        bool AddSalesman(OeSalesmanView customers);
        Task<bool> EditSalesman(OeSalesmanView customers);
        List<OeKurir> GetKurir();
        OeKurir GetKurirId(int id);
        string GetKurirKode(string id);
        bool CekKdKurir(string kurir);
        Task<bool> DelKurir(int kurirs);
        bool AddKurir(OeKurirView customers);
        Task<bool> EditKurir(OeKurirView customers);
        Task<List<OeTransH>> GetTransKurirAsync();
        void SimpanKurir(OeTransH transaksi);
        void SimpanSalesman(OeTransH transaksi);
        List<OeTransD> GetOeTransDByDokumen(string dokumen);
        void SaveOrderAktifSmart(string noLpb);
    }
}

