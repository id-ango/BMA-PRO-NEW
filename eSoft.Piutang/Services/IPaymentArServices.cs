using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using eSoft.Piutang.View;
using eSoft.CashBank.Data;
using eSoft.CashBank.Model;
using eSoft.CashBank.View;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Piutang.Services
{
    public interface IPaymentArServices
    {
        ArTransH GetTrans(int id);
        List<ArTransH> GetTransH();
        List<ArTransH> Get3TransH(DateTime tgl1, DateTime tgl2);
        List<ArTransD> GetTransD();
        ArTransH AddTransH(ArTransHView transH);
        Task<bool> DelTransH(int id);
        bool CekAlreadyPosting(string dokumen);
        List<ArPiutng> GetPiutangSisa(string customer);
    }
}
