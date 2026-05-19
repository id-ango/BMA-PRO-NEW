using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;
using Microsoft.EntityFrameworkCore;


namespace eSoft.Persediaan.Services
{
    public interface IIcAdjustServices
    {

        IcTransH GetIcTrans(int id);
        List<IcTransH> GetTransH();
        List<IcTransH> Get3TransH();
        List<IcTransD> GetTransD();
        IcTransH AddTransH(IcTransHView codeview);
        Task<bool> EditTransH(IcTransHView codeview);
        Task<bool> DelTransH(int codeview);
    }
}
