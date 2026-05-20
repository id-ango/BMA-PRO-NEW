using System.Collections.Generic;
using System.Threading.Tasks;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;

namespace eSoft.Penjualan.Services
{
    public interface IKurirMasterService
    {
        List<OeKurir> GetKurir();
        OeKurir GetKurirId(int id);
        string GetKurirKode(string id);
        bool CekKdKurir(string kurir);
        bool AddKurir(OeKurirView kurir);
        Task<bool> EditKurir(OeKurirView kurir);
        Task<bool> DelKurir(int id);
    }
}
