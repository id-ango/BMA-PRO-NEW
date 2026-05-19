using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Asset.Data;
using eSoft.Asset.Model;
using eSoft.Asset.View;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Asset.Services
{
    public interface IAssetServices
    {
       
        bool CekAcctSet(string supplier);
        List<AsAcctset> GetAsAkunSet();
        AsAcctset GetAsAkunSetId(int id);
        bool AddAkunSet(AsAcctsetView codeview);
        Task<bool> EditAkunSet(AsAcctsetView codeview);
        Task<bool> DelAkunSet(int codeview);
        bool CekDistCode(string distcode);
        List<AsDistSet> GetDist();
        AsDistSet GetDistId(int id);
        bool AddDist(AsDistSetView codeview);
        Task<bool> EditDist(AsDistSetView codeview);
        Task<bool> DelDist(int codeview);
     
        bool CekKdDivisi(string item);
        List<AsDivisi> GetAsDiv();
        AsDivisi GetAsDivId(int id);
        bool AddAsDiv(AsDivisiView codeview);
        Task<bool> EditAsDiv(AsDivisiView codeview);
        Task<bool> DelAsDiv(int codeview);
        bool CekKdItem(string item);
        bool AddAsItem(AsItemView produk);
        Task<bool> EditAsItem(AsItemView produk);
        AsItem GetAsItemId(int itemKode);
        Task<bool> DelAsItem(int codeview);
        List<AsItem> GetAsItem();
        List<AsAssets> GetAsAsset();
        Task<bool> DelAsAssets(int codeview);
        bool AddAsAsset(AsAssetsView produk);

    }
}
