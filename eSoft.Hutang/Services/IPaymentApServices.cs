using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Hutang.Data;
using eSoft.Hutang.Model;
using eSoft.Hutang.View;
using eSoft.CashBank.Data;
using eSoft.CashBank.Model;
using eSoft.CashBank.View;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Hutang.Services
{
    public interface IPaymentApServices
    {

        ApTransH GetTrans(int id);
        List<ApTransH> GetTransH();
        List<ApTransH> Get3TransH();
        List<ApTransD> GetTransD();
        ApTransH AddTransH(ApTransHView transH);
        Task<bool> DelTransH(int id);
        bool CekAlreadyPosting(string dokumen);
        List<ApHutang> GetHutangSisa(string Supplier);
    }
}
