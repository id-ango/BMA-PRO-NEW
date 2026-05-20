using System.Collections.Generic;
using System.Threading.Tasks;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;

namespace eSoft.Penjualan.Services
{
    public interface ISalesmanMasterService
    {
        List<OeSalesman> GetSalesman();
        OeSalesman GetSalesmanId(int id);
        string GetSalesmanKode(string id);
        Task<bool> DelSalesman(int id);
        bool CekKdSalesman(string salesman);
        bool AddSalesman(OeSalesmanView salesman);
        Task<bool> EditSalesman(OeSalesmanView salesman);
    }
}
