using eSoft.Ledger.Model;
using eSoft.Ledger.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace eSoft.Ledger.Services
{
    public interface ILedgerServices
    {
        List<GlAccount> GetGlAccount();
        GlAccount GetGlAccountId(int id);
        bool CekKdAkun(string kodeAkun);
        bool AddGlAccount(GlAccountView glakun);
        Task<bool> EditGlAccount(GlAccountView glakun);
        Task<bool> DelGlAccount(int glakun);
        bool CekKodeGL(string kodeAkun);
        List<GlCode> GetGlCode();
        GlCode GetGlCodeId(int id);
        Task<bool> AddGlCode(GlCodeView codeview);
        Task<bool> EditGlCode(GlCodeView codeview);
        Task<bool> DelGlCode(int codeview);
        GlTransH GetTrans(int id);
        List<GlTransH> GetTransH();
        List<GlTransD> GetTransD();
        GlTransH AddTransH(GlTransHView trans);
        GlTransH EditTransH(GlTransHView trans);
        Task<bool> DelTransH(int id);

    }
}
