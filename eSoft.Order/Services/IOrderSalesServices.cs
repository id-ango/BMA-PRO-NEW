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
        Task<bool> UpdateKeterangan(int id, string keterangan);
        void SaveOrderAktif(string nolpb);
        void SaveOrderAktif(string noLpb, List<PoTransDView> soldItems);
        void DelOrderAktif(string nolpb);
        void RestoreSalesOrderStatus(string noLpb);
        (bool hasSalesOrder, bool isComplete, bool isPartial, decimal totalQty, decimal totalTerima, decimal remainingQty) GetSalesOrderFulfillment(string noLpb);
        void RestoreSalesOrderAfterSalesDelete(string noLpb, IEnumerable<PoTransDView> soldItems);
        void ReconcileSalesOrderAfterSalesEdit(string noLpb, IEnumerable<PoTransDView> oldItems, IEnumerable<PoTransDView> newItems);
        void RebuildSalesOrderFulfillment(string noLpb);
        (bool canEdit, string message) ValidateEditSalesOrderQty(string noLpb, decimal newQty, decimal currentQty);
        (bool canDelete, string message) CanDeleteSalesOrder(string noLpb);
        Task<bool> CloseOrder(int id);
        List<IcStockCardView> GetCurrentOrderJual(List<IcStockCardView> stockCard);

        List<IcStockCardView> GetListOrderAktif(string itemCode, string kodeTrans);
        void SavePdf(PoTransH transH);
        SalesOrderStockMatrixView GetSalesOrderStockMatrix();

        // ✅ NEW: Calculate total sold qty for SO item from existing sales
        decimal CalculateTotalSoldQtyForSoItem(string noLpb, string itemCode);
    }
}
