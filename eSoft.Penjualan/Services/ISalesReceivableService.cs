using eSoft.Penjualan.Model;
using eSoft.Piutang.Data;

namespace eSoft.Penjualan.Services
{
    public interface ISalesReceivableService
    {
        bool HasSettlement(string documentNo);
        void ApplySaleReceivable(OeTransH transH, bool nonPiutang, DbContextPiutang contextAr);
        void ApplyReturnReceivable(OeTransH transH, DbContextPiutang contextAr);
        void ReverseExistingReceivable(OeTransH existingTrans, DbContextPiutang contextAr);
        void ReverseExistingReceivableForEdit(OeTransH existingTrans, DbContextPiutang contextAr);
        void ApplyEditedReceivable(OeTransH transH, bool nonPiutang, DbContextPiutang contextAr);
    }
}
