using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Financial.View;
using eSoft.Financial.Model;

namespace eSoft.Financial.Services
{
    public interface IFinancialServices
    {
        List<FcLedgerView> CetakBukuBesar(DateTime Tanggal1, DateTime Tanggal2, string[] sourceCode);
        List<FcCom> GetFcCom();
        Task<bool> DelFcCom(int id);
        bool CekCompany(string customer);
        bool AddCompany(FcComView codeview);
        FcCom GetCompanyId(int id);
        Task<bool> EditCompany(FcComView codeview);
        List<FcTransH> prosesFinancial1(int tahun, string kodeCompany);
        List<FcTransH> prosesFinancial2(int tahun, string kodeCompany);
        List<FcTransH> prosesFinancial3(int tahun, string kodeCompany);
        List<FcGlTransH> GetTransHFc();
        List<FcGlTransD> GetTransDFc();
        Task<bool> DelTransHFc(int id);
        FcGlTransH AddTransH(FcTransHView trans);
        FcGlTransH GetTransDoc(string docno);
        string GetNumber();
        FcTransH GetTransGL(int id);
        List<FcTransH> GetTransH(int tahun);
        List<FcTransD> GetTransD();
        Task<bool> DelTransH(int id);
        FcGlTransH EditTransH(FcTransHView trans);
        FcGlTransH GetTrans(int id);
        bool CekKdPrintTP(string item);
        List<FcPrintTP> GetPrintTP();
        FcPrintTP GetPrintTPId(int id);
        FcPrintTP GetPrintTPKode(string id);
        bool AddPrintTP(FcPrintTPView codeview);
        Task<bool> EditPrintTP(FcPrintTPView codeview);
        Task<bool> DelPrintTP(int codeview);

        List<FcPrintGl> GetPrintGLPKode(string id);
        bool SavePrintGL(List<FcPrintGl> codeview, string KodeCetak);

        List<FcPrintGLView> printNeraca(string kodeCetak, string company, int Bulan, int Tahun);
        List<FcPrintGLView> printRugiLaba(string kodeCetak, string company, int Bulan, int Tahun);
        List<FcLedgerView> printBukuBesar(string kodeCetak, string company,int BulanAwal, int Bulan, int Tahun);
        List<FcAccount> printTrialBalance( string company, int Bulan, int Tahun);
        IEnumerable<FcTransH> printJurnal(int tahun, string kodeCompany, DateTime TglAwal, DateTime TglAkhir);
        IEnumerable<FcTransD> JurnalDetail(int headerID);
        List<FcTransHView> printJurnalHutang(DateTime TglAwal, DateTime TglAkhir, string supplier = null);
    }
}
