using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Order.Data;
using eSoft.Order.Model;
using eSoft.Order.View;
using eSoft.Hutang.Data;
using eSoft.Hutang.Model;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;

namespace eSoft.Order.Services
{
    public interface IOrderPurchaseServices
    {
        PoTransH GetPoTrans(int id);
        List<PoTransH> GetTransH();
        List<PoTransH> GetTransHAktif();
        List<PoTransH> Get3TransH();
        List<PoTransD> GetTransD();
        PoTransH GetOrderAktif(string customer);
        PoTransH AddTransH(PoTransHView trans);
        Task<bool> DelTransH(int id);
        Task<bool> EditTransH(PoTransHView trans);
        void SaveOrderAktif(string customer);
        void  DelOrderAktif(string customer);
        List<PoItemQtyByLocationView> GetAllIcItemQtyByLocation(string KodeVendor);
    }
}
