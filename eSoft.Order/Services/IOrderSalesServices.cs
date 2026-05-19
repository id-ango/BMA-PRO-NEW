using System.Collections.Generic;
using System.Threading.Tasks;
using eSoft.Order.Model;
using eSoft.Order.View;
using eSoft.Persediaan.View;
using eSoft.Persediaan.Model;


namespace eSoft.Order.Services
{
    public interface IOrderSalesServices
    {
        PoTransH GetPoTrans(int id);
        List<PoTransH> GetTransH();
        List<PoTransH> GetTransHAktif();
        List<PoTransH> Get3TransH();
        List<PoTransD> GetTransD();
        PoTransH GetOrderAktif(string nolpb);
        PoTransH AddTransH(PoTransHView trans);
        Task<bool> DelTransH(int id);
        Task<bool> EditTransH(PoTransHView trans);
        void SaveOrderAktif(string nolpb);
        void DelOrderAktif(string nolpb);
        Task<bool> CloseOrder(int id);
        List<IcStockCardView> GetCurrentOrderJual(List<IcStockCardView> stockCard);

        List<IcStockCardView> GetListOrderAktif(string itemCode, string kodeTrans);
        void SavePdf(PoTransH transH);
        SalesOrderStockMatrixView GetSalesOrderStockMatrix();
    }
}
