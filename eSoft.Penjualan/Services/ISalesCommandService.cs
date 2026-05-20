using System.Threading.Tasks;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;

namespace eSoft.Penjualan.Services
{
    public interface ISalesCommandService
    {
        OeTransH AddTransH(OeTransHView trans, bool pajak);
        OeTransH AddTransHRetur(OeTransHView trans, bool pajak);
        Task<bool> DelTransH(int id);
        bool EditTransH(OeTransHView trans);
        bool CekPiutang(OeTransH trans);
    }
}
