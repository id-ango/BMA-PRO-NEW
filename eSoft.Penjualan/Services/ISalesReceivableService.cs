using eSoft.Penjualan.Model;

namespace eSoft.Penjualan.Services
{
    public interface ISalesReceivableService
    {
        bool HasSettlement(string documentNo);
        void ApplySaleReceivable(OeTransH transH, bool nonPiutang);
        void ApplyReturnReceivable(OeTransH transH);
        void ReverseExistingReceivable(OeTransH existingTrans);
        void ReverseExistingReceivableForEdit(OeTransH existingTrans);
        void ApplyEditedReceivable(OeTransH transH, bool nonPiutang);
    }
}
