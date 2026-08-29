using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;
using Microsoft.EntityFrameworkCore;


namespace eSoft.Persediaan.Services
{
    public interface IInventoryServices
    {
        bool CekKdItem(string item);
        List<IcItem> GetIcItem(bool disabled = false);
        Task<List<IcItem>> GetIcItemAsync(bool disabled = false);
        List<IcItem> GetIcItemByStatus(string filter = "all");
        Task UpdateProject(IcItem project);
        List<IcAltItem> GetIcAltItem();
        IcItem GetIcItemId(int itemKode);
        IcItem GetIcItemProduk(string produk);
        Task<bool> DelIcItem(int codeview);
        bool AddIcItem(IcItemView produk);
        Task<bool> EditIcItem(IcItemView produk);
        bool CekKdDivisi(string item);
        List<IcDiv> GetIcDiv();
        IcDiv GetIcDivId(int id);
        IcDiv GetIcDivKd(string id);
        bool AddIcDiv(IcDivView codeview);
        Task<bool> EditIcDiv(IcDivView codeview);
        Task<bool> DelIcDiv(int codeview);
        bool CekKdLokasi(string item);
        List<IcLokasi> GetIcLokasi();
        IcLokasi GetIcLokasiId(int id);
        IcLokasi GetIcLokasiKode(string id);
        bool AddIcLokasi(IcLokasiView codeview);
        Task<bool> EditIcLokasi(IcLokasiView codeview);
        Task<bool> DelIcLokasi(int codeview);
        bool CekCategory(string item);
        List<IcCat> GetIcCat();
        IcCat GetIcCatId(int id);
        bool AddIcCat(IcCatView codeview);
        Task<bool> EditIcCat(IcCatView codeview);
        Task<bool> DelIcCat(int codeview);
        bool CekAcctSet(string item);
        List<IcAcct> GetIcAcct();
        IcAcct GetIcAcctId(int id);
        bool AddIcAcct(IcAcctView codeview);
        Task<bool> EditIcAcct(IcAcctView codeview);
        Task<bool> DelIcAcct(int codeview);
        IcTransH GetIcTrans(int id);
        List<IcTransH> GetTransH();
        List<IcTransH> Get3TransH();
        List<IcTransD> GetTransD();
        IcTransH AddTransH(IcTransHView codeview);
        Task<bool> EditTransH(IcTransHView codeview);
        Task<bool> DelTransH(int codeview);
        List<IcStockCardView> GetCurrentStock();

        List<IcItemQtyByLocationView> GetAllIcItemQtyByLocation();

        bool RubahDivisiItem(int xKdBank, string cDivisi);
    }

}
