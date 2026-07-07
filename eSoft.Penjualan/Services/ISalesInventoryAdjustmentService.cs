using System.Collections.Generic;
using eSoft.Persediaan.Data;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;

namespace eSoft.Penjualan.Services
{
    public interface ISalesInventoryAdjustmentService
    {
        void ApplySaleDetail(OeTransDView item);
        void ApplyReturnDetail(OeTransDView item);
        void ReverseExistingDetail(OeTransD item, string kode);
        void ApplyDetailsForCode(IEnumerable<OeTransDView> items, string kode, DbContextPersediaan context);
        void ReverseDetails(IEnumerable<OeTransD> items, string kode, DbContextPersediaan context);
    }
}
