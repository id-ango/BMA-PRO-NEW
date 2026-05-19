using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Pembelian.Data;
using eSoft.Pembelian.Model;
using eSoft.Pembelian.View;
using eSoft.Hutang.Data;
using eSoft.Hutang.Model;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;

namespace eSoft.Pembelian.Services
{
    public interface IPurchaseServices
    {
        IrTransH GetIrTrans(int id);
        List<IrTransH> GetTransH();
        List<IrTransH> Get3TransH();
        List<IrTransD> GetTransD();
        IrTransH AddTransH(IrTransHView trans);
        Task<bool> DelTransH(int id);
        Task<bool> EditTransH(IrTransHView trans);
        IrTransH AddTransHRetur(IrTransHView trans);
        List<IrTransH> Laporan1(DateTime tgl1, DateTime tgl2);
        List<IrTransD> Detail1(int xKdHeader);
        List<IrTrans> Detail2(string xKdHeader, DateTime tgl1, DateTime tgl2);
        bool CekHutang(IrTransH trans);
        void ReprosesPurchase();

    }
}
