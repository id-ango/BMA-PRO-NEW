using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eSoft.Penjualan.Data;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using eSoft.Persediaan.Data;

namespace eSoft.Penjualan.Services
{
    public class SalesServices : ISalesServices
    {
        private readonly ISalesmanMasterService _salesmanMasterService;
        private readonly IKurirMasterService _kurirMasterService;
        private readonly ISalesReportService _salesReportService;
        private readonly ISalesQueryService _salesQueryService;
        private readonly ISalesCommandService _salesCommandService;

        public SalesServices(
            ISalesmanMasterService salesmanMasterService,
            IKurirMasterService kurirMasterService,
            ISalesReportService salesReportService,
            ISalesQueryService salesQueryService,
            ISalesCommandService salesCommandService)
        {
            _salesmanMasterService = salesmanMasterService;
            _kurirMasterService = kurirMasterService;
            _salesReportService = salesReportService;
            _salesQueryService = salesQueryService;
            _salesCommandService = salesCommandService;
        }

        #region laporanpenjualan

        public List<OeTransH> Laporan1(DateTime tgl1, DateTime tgl2)
        {
            return _salesReportService.Laporan1(tgl1, tgl2);
        }

        public List<OeTransD> LaporanDownload(DateTime tgl1, DateTime tgl2)
        {
            return _salesReportService.LaporanDownload(tgl1, tgl2);
        }
        public List<OeTransD> Detail1(int xKdHeader)
        {
            return _salesReportService.Detail1(xKdHeader);
        }

        public List<OeTrans> Detail2(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            return _salesReportService.Detail2(xKdHeader, tgl1, tgl2);
        }

        public List<OeTrans> Detail3(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            return _salesReportService.Detail3(xKdHeader, tgl1, tgl2);
        }

        public List<OeTrans> Detail3Index(string xKdHeader)
        {
            return _salesReportService.Detail3Index(xKdHeader);
        }

        public List<OeTrans> GetTransDetailsByNoLpbs(IEnumerable<string> noLpbs)
        {
            return _salesReportService.GetTransDetailsByNoLpbs(noLpbs);
        }

        public async Task<List<OeTrans>> GetTransDetailsByNoLpbsAsync(IEnumerable<string> noLpbs)
        {
            return await _salesReportService.GetTransDetailsByNoLpbsAsync(noLpbs);
        }

        public async Task<Dictionary<string, List<OeTrans>>> GetTransDetailsBatchAsync(IEnumerable<string> noLpbs)
        {
            return await _salesReportService.GetTransDetailsBatchAsync(noLpbs);
        }

        public List<OeTrans> Detail4(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            return _salesReportService.Detail4(xKdHeader, tgl1, tgl2);
        }

        public List<OeTrans> Detail5(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            return _salesReportService.Detail5(xKdHeader, tgl1, tgl2);
        }
        #endregion

        #region Salesman

        public List<OeSalesman> GetSalesman()
        {
            return _salesmanMasterService.GetSalesman();
        }

        public async Task<List<OeSalesman>> GetSalesmanAsync()
        {
            return await _salesmanMasterService.GetSalesmanAsync();
        }

        public OeSalesman GetSalesmanId(int id)
        {
            return _salesmanMasterService.GetSalesmanId(id);
        }
        public string GetSalesmanKode(string id)
        {
            return _salesmanMasterService.GetSalesmanKode(id);
        }
        public async Task<bool> DelSalesman(int kurirs)
        {
            return await _salesmanMasterService.DelSalesman(kurirs);

        }

        public bool CekKdSalesman(string kurir)
        {
            return _salesmanMasterService.CekKdSalesman(kurir);
        }

        public bool AddSalesman(OeSalesmanView customers)
        {
            return _salesmanMasterService.AddSalesman(customers);
        }

        public async Task<bool> EditSalesman(OeSalesmanView customers)
        {
            return await _salesmanMasterService.EditSalesman(customers);

        }
        #endregion


        #region Kurir

        public List<OeKurir> GetKurir()
        {
            return _kurirMasterService.GetKurir();
        }

        public OeKurir GetKurirId(int id)
        {
            return _kurirMasterService.GetKurirId(id);
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        public string GetKurirKode(string id)
        {
            return _kurirMasterService.GetKurirKode(id);
        }

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        public bool CekKdKurir(string kurir)
        {
            return _kurirMasterService.CekKdKurir(kurir);
        }

        public bool AddKurir(OeKurirView customers)
        {
            return _kurirMasterService.AddKurir(customers);
        }

        public async Task<bool> EditKurir(OeKurirView customers)
        {
            return await _kurirMasterService.EditKurir(customers);

        }
        public async Task<bool> DelKurir(int kurirs)
        {
            return await _kurirMasterService.DelKurir(kurirs);

        }

        #endregion

        public ArPiutng GetPiutang(string bukti)
        {
            return _salesQueryService.GetPiutang(bukti);

        }

        #region OeTransH class

        public OeTransH GetOeTrans(int id)
        {
            return _salesQueryService.GetOeTrans(id);
        }

        public OeTransH GetOeTransDokumen(string id)
        {
            return _salesQueryService.GetOeTransDokumen(id);
        }
        public List<OeTransH> GetFirstTransH()
        {
            return _salesQueryService.GetFirstTransH();

        }

        public List<OeTransH> GetFirstTransHNon()
        {
            return _salesQueryService.GetFirstTransHNon();

        }

        public async Task<List<OeTransH>> GetTransH(DateTime tanggalAwal, DateTime tanggalAkhir)
        {
            return await _salesQueryService.GetTransH(tanggalAwal, tanggalAkhir);


        }

        public async Task<List<OeTransH>> GetTransHNon(DateTime tanggalAwal, DateTime tanggalAkhir)
        {
            return await _salesQueryService.GetTransHNon(tanggalAwal, tanggalAkhir);

        }

        public List<OeTransH> Get3TransH()
        {
            return _salesQueryService.Get3TransH();

        }

        public List<OeTransD> GetTransD()
        {
            return _salesQueryService.GetTransD();
        }

        public OeTransH AddTransH(OeTransHView trans, bool pajak)
        {
            return _salesCommandService.AddTransH(trans, pajak);
        }

        public OeTransH GetTransDoc(string docno)
        {
            return _salesQueryService.GetOeTransDokumen(docno);
        }

        public async Task<bool> DelTransH(int id)
        {
            return await _salesCommandService.DelTransH(id);
        }

        public bool CekPiutang(OeTransH trans)
        {
            return _salesCommandService.CekPiutang(trans);
        }

        public bool EditTransH(OeTransHView trans)
        {
            return _salesCommandService.EditTransH(trans);
        }

        #endregion OeTransH Class

        #region retur jual

        public OeTransH AddTransHRetur(OeTransHView trans, bool pajak)
        {
            return _salesCommandService.AddTransHRetur(trans, pajak);
        }

        #endregion

        #region indexjual

        public async Task<List<OeTransH>> GetTransKurirAsync(int? top = null)
        {
            return await _salesQueryService.GetTransKurirAsync(top);
        }


        public void SimpanKurir(OeTransH transaksi)
        {
            _salesQueryService.SimpanKurir(transaksi);
        }

        public void SimpanSalesman(OeTransH transaksi)
        {
            _salesQueryService.SimpanSalesman(transaksi);
        }

        public async Task<List<SalesTransactionWithDetailDto>> GetTransactionsWithDetailsAsync(int? top = null)
        {
            var headers = await _salesQueryService.GetTransKurirAsync(top);
            if (headers == null || headers.Count == 0)
                return new List<SalesTransactionWithDetailDto>();

            var noLpbs = headers.Select(h => h.NoLpb).Where(n => !string.IsNullOrEmpty(n)).ToList();
            var detailsBatch = await _salesReportService.GetTransDetailsBatchAsync(noLpbs);

            var result = new List<SalesTransactionWithDetailDto>(headers.Count);
            foreach (var header in headers)
            {
                detailsBatch.TryGetValue(header.NoLpb, out var details);
                result.Add(new SalesTransactionWithDetailDto
                {
                    Header = header,
                    Details = details ?? new List<OeTrans>()
                });
            }

            return result;
        }

        public List<SalesTransactionWithDetailDto> GetTransactionsWithDetails(int? top = null)
        {
            var headers = _salesQueryService.GetTransKurir(top);
            if (headers == null || headers.Count == 0)
                return new List<SalesTransactionWithDetailDto>();

            var noLpbs = headers.Select(h => h.NoLpb).Where(n => !string.IsNullOrEmpty(n)).ToList();
            var detailsList = _salesReportService.GetTransDetailsByNoLpbs(noLpbs);
            var detailsBatch = detailsList.GroupBy(d => d.NoLpb).ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<SalesTransactionWithDetailDto>(headers.Count);
            foreach (var header in headers)
            {
                detailsBatch.TryGetValue(header.NoLpb, out var details);
                result.Add(new SalesTransactionWithDetailDto
                {
                    Header = header,
                    Details = details ?? new List<OeTrans>()
                });
            }

            return result;
        }

        #endregion

        public List<OeTransD> GetOeTransDByDokumen(string dokumen)
        {
            return _salesQueryService.GetOeTransDByDokumen(dokumen);
        }

        public void SaveOrderAktifSmart(string noLpb)
        {
            // Delegate to command service for smart SO status update
            _salesCommandService.SaveOrderAktifSmart(noLpb);
        }
    }
}
