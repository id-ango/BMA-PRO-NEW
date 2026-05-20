using System;
using System.Collections.Generic;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;

namespace eSoft.Penjualan.Services
{
    public interface ISalesReportService
    {
        List<OeTransH> Laporan1(DateTime tgl1, DateTime tgl2);
        List<OeTransD> LaporanDownload(DateTime tgl1, DateTime tgl2);
        List<OeTransD> Detail1(int xKdHeader);
        List<OeTrans> Detail2(string xKdHeader, DateTime tgl1, DateTime tgl2);
        List<OeTrans> Detail3(string xKdHeader, DateTime tgl1, DateTime tgl2);
        List<OeTrans> Detail3Index(string xKdHeader);
        List<OeTrans> Detail4(string xKdHeader, DateTime tgl1, DateTime tgl2);
        List<OeTrans> Detail5(string xKdHeader, DateTime tgl1, DateTime tgl2);
    }
}
