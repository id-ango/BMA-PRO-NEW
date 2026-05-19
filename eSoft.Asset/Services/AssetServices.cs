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
        private readonly DbContextAssets _context;

        public AssetServices(DbContextAssets context)
        {
            _context = context;
        }

        public bool CekKdItem(string item)
        {
            if (item != null)
            {
                string test = item.ToUpper();
                var cekFirst = _context.AsItems.Where(x => x.ItemCode == test).ToList();
                if (cekFirst.Count == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public List<AsItem> GetAsItem()
        {
            return _context.AsItems.OrderBy(x => x.NamaItem).ToList();
        }

       
        public AsItem GetAsItemId(int itemKode)
        {
            return _context.AsItems.Where(x => x.AsItemId == itemKode).FirstOrDefault();
        }

        public AsItem GetAsItemProduk(string itemKode)
        {
            return _context.AsItems.Where(x => x.ItemCode == itemKode).FirstOrDefault();
        }

        public async Task<bool> DelAsItem(int codeview)
        {
            try
            {
                var ExistingDist = _context.AsItems.Where(x => x.AsItemId == codeview).FirstOrDefault();
                if (ExistingDist != null)
                {
                    _context.AsItems.Remove(ExistingDist);
                    await _context.SaveChangesAsync();
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
            var cekFirst = _context.AsItems.Where(x => x.ItemCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                AsItem Produk = new AsItem()
                {
                    ItemCode = produk.ItemCode.ToUpper(),
                    NamaItem = produk.NamaItem,
                    Satuan = produk.Satuan,
                    Divisi = produk.Divisi,
                    

                };
                _context.AsItems.Add(Produk);
                _context.SaveChanges();
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
                var ExistingItem = _context.AsItems.Where(x => x.AsItemId == produk.AsItemId).FirstOrDefault();
                if (ExistingItem != null)
                {
                    ExistingItem.NamaItem = produk.NamaItem;
                    ExistingItem.Satuan = produk.Satuan;
                    ExistingItem.Divisi = produk.Divisi;

                    
                    _context.AsItems.Update(ExistingItem);
                    await _context.SaveChangesAsync();
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
            var cekFirst = _context.AsDivisis.Where(x => x.Divisi == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<AsDivisi> GetAsDiv()
        {
            return _context.AsDivisis.OrderBy(x => x.Divisi).ToList();
        }

        public AsDivisi GetAsDivId(int id)
        {
            return _context.AsDivisis.Where(x => x.AsDivId == id).FirstOrDefault();
        }

        public bool AddAsDiv(AsDivisiView codeview)
        {
            string test = codeview.Divisi.ToUpper();
            var cekFirst = _context.AsDivisis.Where(x => x.Divisi == test).ToList();
            if (cekFirst.Count == 0)
            {
                AsDivisi Division = new AsDivisi()
                {
                    Divisi = codeview.Divisi.ToUpper(),
                    NamaDiv = codeview.NamaDiv

                };
                _context.AsDivisis.Add(Division);
                _context.SaveChanges();
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
                var ExistingDiv = _context.AsDivisis.Where(x => x.AsDivId == codeview.AsDivId).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    ExistingDiv.NamaDiv = codeview.NamaDiv;


                    _context.AsDivisis.Update(ExistingDiv);
                    await _context.SaveChangesAsync();
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
                var ExistingDiv = _context.AsDivisis.Where(x => x.AsDivId == codeview).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    _context.AsDivisis.Remove(ExistingDiv);
                    await _context.SaveChangesAsync();
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
            var cekFirst = _context.AsAcctsets.Where(x => x.AcctSet == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<AsAcctset> GetAsAkunSet()
        {
            return _context.AsAcctsets.ToList();
        }

        public AsAcctset GetAsAkunSetId(int id)
        {
            return _context.AsAcctsets.Where(x => x.AsAcctId == id).FirstOrDefault();
        }

        public bool AddAkunSet(AsAcctsetView codeview)
        {
            string test = codeview.AcctSet.ToUpper();
            var cekFirst = _context.AsAcctsets.Where(x => x.AcctSet == test).ToList();
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
                _context.AsAcctsets.Add(AcctCode);
                _context.SaveChanges();
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
                var ExistingAkunSet = _context.AsAcctsets.Where(x => x.AsAcctId == codeview.AsAcctId).FirstOrDefault();
                if (ExistingAkunSet != null)
                {
                    ExistingAkunSet.Description = codeview.Description;
                    ExistingAkunSet.Acct1 = codeview.Acct1;
                    ExistingAkunSet.Acct2 = codeview.Acct2;
                    ExistingAkunSet.Acct3 = codeview.Acct3;
                    ExistingAkunSet.Acct4 = codeview.Acct4;
                    ExistingAkunSet.Acct5 = codeview.Acct5;
                    ExistingAkunSet.Acct6 = codeview.Acct6;

                    _context.AsAcctsets.Update(ExistingAkunSet);
                    await _context.SaveChangesAsync();
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
                var ExistingAkunSet = _context.AsAcctsets.Where(x => x.AsAcctId == codeview).FirstOrDefault();
                if (ExistingAkunSet != null)
                {
                    _context.AsAcctsets.Remove(ExistingAkunSet);
                    await _context.SaveChangesAsync();
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
            var cekFirst = _context.AsDistSets.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<AsDistSet> GetDist()
        {
            return _context.AsDistSets.ToList();
        }

        public AsDistSet GetDistId(int id)
        {
            return _context.AsDistSets.Where(x => x.AsDistId == id).FirstOrDefault();
        }

        public bool AddDist(AsDistSetView codeview)
        {
            string test = codeview.DistCode.ToUpper();
            var cekFirst = _context.AsDistSets.Where(x => x.DistCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                AsDistSet AcctCode = new AsDistSet()
                {
                    DistCode = codeview.DistCode.ToUpper(),
                    Description = codeview.Description,
                    Dist1 = codeview.Dist1

                };
                _context.AsDistSets.Add(AcctCode);
                _context.SaveChanges();
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
                var ExistingDist = _context.AsDistSets.Where(x => x.AsDistId == codeview.AsDistId).FirstOrDefault();
                if (ExistingDist != null)
                {
                    ExistingDist.Description = codeview.Description;
                    ExistingDist.Dist1 = codeview.Dist1;

                    _context.AsDistSets.Update(ExistingDist);
                    await _context.SaveChangesAsync();
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
                var ExistingDist = _context.AsDistSets.Where(x => x.AsDistId == codeview).FirstOrDefault();
                if (ExistingDist != null)
                {
                    _context.AsDistSets.Remove(ExistingDist);
                    await _context.SaveChangesAsync();
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
            return _context.AsAssetss.Where(x =>x.Qty!=0).OrderBy(x => x.TglBeli).ToList();
        }

        public async Task<bool> DelAsAssets(int codeview)
        {
            try
            {
                var ExistingDist = _context.AsAssetss.Where(x => x.AsAssetsId == codeview).FirstOrDefault();
                if (ExistingDist != null && ExistingDist.Nilai == ExistingDist.SisaNilai)
                {
                    var existingAsset = _context.AsTransaksis.Where(x => x.BarcodeAssets == ExistingDist.BarcodeAssets).ToList();

                    _context.AsAssetss.Remove(ExistingDist);
                    _context.AsTransaksis.RemoveRange(existingAsset);
                    await _context.SaveChangesAsync();
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
               
                _context.AsAssetss.AddRange(daftarAsset);
                _context.AsTransaksis.AddRange(daftarTransaksi);
                _context.SaveChanges();
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
            var maxlist = _context.AsAssetss.Where(x => x.BarcodeAssets.Substring(0, 6).Equals(xbukti)).ToList();
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

        #endregion
    }
}
