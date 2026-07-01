using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Asset.Data;
using eSoft.Asset.Model;
using eSoft.Asset.View;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace eSoft.Asset.Services
{
    public class AssetServices : IAssetServices
    {
        private readonly IDbContextFactory<DbContextAssets> _context;

        public AssetServices(IDbContextFactory<DbContextAssets> context)
        {
            _context = context;
        }

        public bool CekKdItem(string item)
        {
            if (item != null)
            {
                string test = item.ToUpper();
                using var context = CreateContext();
                var cekFirst = context.AsItems.Where(x => x.ItemCode == test).ToList();
                if (cekFirst.Count == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public List<AsItem> GetAsItem()
        {
            using var context = CreateContext();
            return context.AsItems.OrderBy(x => x.NamaItem).ToList();
        }

       
        public AsItem GetAsItemId(int itemKode)
        {
            using var context = CreateContext();
            return context.AsItems.Where(x => x.AsItemId == itemKode).FirstOrDefault();
        }

        public AsItem GetAsItemProduk(string itemKode)
        {
            using var context = CreateContext();
            return context.AsItems.Where(x => x.ItemCode == itemKode).FirstOrDefault();
        }

        public async Task<bool> DelAsItem(int codeview)
        {
            try
            {
                using var context = CreateContext();
                var ExistingDist = context.AsItems.Where(x => x.AsItemId == codeview).FirstOrDefault();
                if (ExistingDist != null)
                {
                    context.AsItems.Remove(ExistingDist);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public bool AddAsItem(AsItemView produk)
        {
            string test = produk.ItemCode.ToUpper();
            using var context = CreateContext();
            var cekFirst = context.AsItems.Where(x => x.ItemCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                AsItem Produk = new AsItem()
                {
                    ItemCode = produk.ItemCode.ToUpper(),
                    NamaItem = produk.NamaItem,
                    Satuan = produk.Satuan,
                    Divisi = produk.Divisi,
                    

                };
                context.AsItems.Add(Produk);
                context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditAsItem(AsItemView produk)
        {
            try
            {
                using var context = CreateContext();
                var ExistingItem = context.AsItems.Where(x => x.AsItemId == produk.AsItemId).FirstOrDefault();
                if (ExistingItem != null)
                {
                    ExistingItem.NamaItem = produk.NamaItem;
                    ExistingItem.Satuan = produk.Satuan;
                    ExistingItem.Divisi = produk.Divisi;

                    
                    context.AsItems.Update(ExistingItem);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }


        #region AsDiv Class

        public bool CekKdDivisi(string item)
        {
            string test = item.ToUpper();
            using var context = CreateContext();
            var cekFirst = context.AsDivisis.Where(x => x.Divisi == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<AsDivisi> GetAsDiv()
        {
            using var context = CreateContext();
            return context.AsDivisis.OrderBy(x => x.Divisi).ToList();
        }

        public AsDivisi GetAsDivId(int id)
        {
            using var context = CreateContext();
            return context.AsDivisis.Where(x => x.AsDivId == id).FirstOrDefault();
        }

        public bool AddAsDiv(AsDivisiView codeview)
        {
            string test = codeview.Divisi.ToUpper();
                using var context = CreateContext();
                var cekFirst = context.AsDivisis.Where(x => x.Divisi == test).ToList();
            if (cekFirst.Count == 0)
            {
                AsDivisi Division = new AsDivisi()
                {
                    Divisi = codeview.Divisi.ToUpper(),
                    NamaDiv = codeview.NamaDiv

                };
                context.AsDivisis.Add(Division);
                context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditAsDiv(AsDivisiView codeview)
        {
            try
            {
                using var context = CreateContext();
                var ExistingDiv = context.AsDivisis.Where(x => x.AsDivId == codeview.AsDivId).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    ExistingDiv.NamaDiv = codeview.NamaDiv;


                    context.AsDivisis.Update(ExistingDiv);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public async Task<bool> DelAsDiv(int codeview)
        {
            try
            {
                using var context = CreateContext();
                var ExistingDiv = context.AsDivisis.Where(x => x.AsDivId == codeview).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    context.AsDivisis.Remove(ExistingDiv);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }
        #endregion Asdiv

        #region AsAcct Class

        public bool CekAcctSet(string supplier)
        {
            string test = supplier.ToUpper();
            using var context = CreateContext();
            var cekFirst = context.AsAcctsets.Where(x => x.AcctSet == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<AsAcctset> GetAsAkunSet()
        {
            using var context = CreateContext();
            return context.AsAcctsets.ToList();
        }

        public AsAcctset GetAsAkunSetId(int id)
        {
            using var context = CreateContext();
            return context.AsAcctsets.Where(x => x.AsAcctId == id).FirstOrDefault();
        }

        public bool AddAkunSet(AsAcctsetView codeview)
        {
            string test = codeview.AcctSet.ToUpper();
                using var context = CreateContext();
                var cekFirst = context.AsAcctsets.Where(x => x.AcctSet == test).ToList();
            if (cekFirst.Count == 0)
            {
                AsAcctset AcctCode = new AsAcctset()
                {
                    AcctSet = codeview.AcctSet.ToUpper(),
                    Description = codeview.Description,
                    Acct1 = codeview.Acct1,
                    Acct2 = codeview.Acct2,
                    Acct3 = codeview.Acct3,
                    Acct4 = codeview.Acct4,
                    Acct5 = codeview.Acct5,
                    Acct6 = codeview.Acct6

                };
                context.AsAcctsets.Add(AcctCode);
                context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }


        }

        public async Task<bool> EditAkunSet(AsAcctsetView codeview)
        {
            try
            {
                using var context = CreateContext();
                var ExistingAkunSet = context.AsAcctsets.Where(x => x.AsAcctId == codeview.AsAcctId).FirstOrDefault();
                if (ExistingAkunSet != null)
                {
                    ExistingAkunSet.Description = codeview.Description;
                    ExistingAkunSet.Acct1 = codeview.Acct1;
                    ExistingAkunSet.Acct2 = codeview.Acct2;
                    ExistingAkunSet.Acct3 = codeview.Acct3;
                    ExistingAkunSet.Acct4 = codeview.Acct4;
                    ExistingAkunSet.Acct5 = codeview.Acct5;
                    ExistingAkunSet.Acct6 = codeview.Acct6;

                    context.AsAcctsets.Update(ExistingAkunSet);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public async Task<bool> DelAkunSet(int codeview)
        {
            try
            {
                using var context = CreateContext();
                var ExistingAkunSet = context.AsAcctsets.Where(x => x.AsAcctId == codeview).FirstOrDefault();
                if (ExistingAkunSet != null)
                {
                    context.AsAcctsets.Remove(ExistingAkunSet);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }
        #endregion AsAcct Class

        #region AsDist Class

        public bool CekDistCode(string distcode)
        {
            string test = distcode.ToUpper();
            using var context = CreateContext();
            var cekFirst = context.AsDistSets.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<AsDistSet> GetDist()
        {
            using var context = CreateContext();
            return context.AsDistSets.ToList();
        }

        public AsDistSet GetDistId(int id)
        {
            using var context = CreateContext();
            return context.AsDistSets.Where(x => x.AsDistId == id).FirstOrDefault();
        }

        public bool AddDist(AsDistSetView codeview)
        {
            string test = codeview.DistCode.ToUpper();
            using var context = CreateContext();
            var cekFirst = context.AsDistSets.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                AsDistSet AcctCode = new AsDistSet()
                {
                    DistCode = codeview.DistCode.ToUpper(),
                    Description = codeview.Description,
                    Dist1 = codeview.Dist1

                };
                context.AsDistSets.Add(AcctCode);
                context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }


        }

        public async Task<bool> EditDist(AsDistSetView codeview)
        {
            try
            {
                using var context = CreateContext();
                var ExistingDist = context.AsDistSets.Where(x => x.AsDistId == codeview.AsDistId).FirstOrDefault();
                if (ExistingDist != null)
                {
                    ExistingDist.Description = codeview.Description;
                    ExistingDist.Dist1 = codeview.Dist1;

                    context.AsDistSets.Update(ExistingDist);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public async Task<bool> DelDist(int codeview)
        {
            try
            {
                using var context = CreateContext();
                var ExistingDist = context.AsDistSets.Where(x => x.AsDistId == codeview).FirstOrDefault();
                if (ExistingDist != null)
                {
                    context.AsDistSets.Remove(ExistingDist);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }
        #endregion ApDist Class

        #region AsAsset Class
        public List<AsAssets> GetAsAsset()
        {
            using var context = CreateContext();
            return context.AsAssetss.Where(x =>x.Qty!=0).OrderBy(x => x.TglBeli).ToList();
        }

        public async Task<bool> DelAsAssets(int codeview)
        {
            try
            {
                using var context = CreateContext();
                var ExistingDist = context.AsAssetss.Where(x => x.AsAssetsId == codeview).FirstOrDefault();
                if (ExistingDist != null && ExistingDist.Nilai == ExistingDist.SisaNilai)
                {
                    var existingAsset = context.AsTransaksis.Where(x => x.BarcodeAssets == ExistingDist.BarcodeAssets).ToList();

                    context.AsAssetss.Remove(ExistingDist);
                    context.AsTransaksis.RemoveRange(existingAsset);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public bool AddAsAsset(AsAssetsView produk)
        {
            List<AsAssets> daftarAsset = new();
            List<AsTransaksi> daftarTransaksi = new();

            string kodeno = produk.KodeBarcodeAssets.ToUpper();
       
      
            string kodeurut = kodeno + '-';           
            string xbukti = kodeurut;
            string nourut = GetNumber(produk.KodeBarcodeAssets);
           

            for (var i=0; i<produk.Qty;i++)
            {
                daftarAsset.Add(new AsAssets()
                {
                    BarcodeAssets = xbukti + (Int32.Parse(nourut) + 1+i).ToString("00000"),
                    NamaBarang = produk.NamaBarang,
                    AsItemCode = produk.AsItemCode,
                    Nilai = produk.Nilai,
                    SisaNilai = produk.Nilai,
                    TglBeli = produk.TglBeli,
                    Termin = produk.Termin,
                    Acctset = produk.Acctset,
                    DistCode = produk.DistCode ,
                    Penyusutan = produk.Penyusutan,
                    Qty = 1,
                    JatuhTempo = produk.TglBeli.AddMonths((int)produk.Termin)
                    
                });;
            }
            if (daftarAsset.Any())
            {
                foreach(var detail in daftarAsset)
                {
                    decimal TotalNilai = detail.Nilai;

                    for (int i = 0; i < detail.Termin; i++)
                    {
                        TotalNilai -= detail.Penyusutan;

                        daftarTransaksi.Add(new()
                        {
                            Kode = "01",                                 // kode for Penerimaan Asset
                            BarcodeAssets = detail.BarcodeAssets,
                            Tanggal = detail.TglBeli.AddMonths(i),
                            Nilai = detail.Penyusutan,
                            Saldo = TotalNilai
                        }) ;
                    }
                }
               
                using var context = CreateContext();
                context.AsAssetss.AddRange(daftarAsset);
                context.AsTransaksis.AddRange(daftarTransaksi);
                context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public string GetNumber(string kodebarcode)
        {
            string kodeno = kodebarcode.Trim();
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut ;
            var maxvalue = "";
            using var context = CreateContext();
            var maxlist = context.AsAssetss.Where(x => x.BarcodeAssets.Substring(0, 6).Equals(xbukti)).ToList();
            if (maxlist != null)
            {
                maxvalue = maxlist.Max(x => x.BarcodeAssets);

            }

            //            var maxvalue = (from e in db.CbTransHs where  e.Docno.Substring(0, 7) == kodeno + thnbln select e).Max();
            string nourut = "00000";
            if (maxvalue == null)
            {
                nourut = "00000";
            }
            else
            {
                nourut = maxvalue.Substring(6, 5);
            }

            //  nourut =Convert.ToString(Int32.Parse(nourut) + 1);


        //    string cAngNo = xbukti + (Int32.Parse(nourut) + 1).ToString("00000");
            // var maxvalue = (from e in db.AptTranss where e.NoRef.Substring(0, 7) == "ANG" + cAngNo select e.NoRef.Max()).FirstOrDefault();
            return nourut;

        }

        private DbContextAssets CreateContext()
        {
            return _context.CreateDbContext();
        }

        #endregion
    }
}
