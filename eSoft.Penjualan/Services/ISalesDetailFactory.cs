using System.Collections.Generic;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;

namespace eSoft.Penjualan.Services
{
    public interface ISalesDetailFactory
    {
        List<OeTransD> CreateDetails(OeTransHView trans, string noLpb, string kode);
    }
}
