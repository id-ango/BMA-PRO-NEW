using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;

using Microsoft.EntityFrameworkCore;


namespace eSoft.Persediaan.Services
{
    public class InventoryServices : IInventoryServices
    {

        private readonly DbContextPersediaan _context;

        public InventoryServices(DbContextPersediaan context)
        {
            _context = context;
        }

        public async Task UpdateProject(IcItem project)
        {
            var existingProject = await _context.IcItems.FindAsync(project.IcItemId);
            if (existingProject != null)
            {
                existingProject.Disabled = project.Disabled;
                // Update other fields as needed
                await _context.SaveChangesAsync();
            }
        }
        public bool CekKdItem(string item)
        {
            if (item != null)
            {
                string test = item.ToUpper();
                var cekFirst = _context.IcItems.Where(x => x.ItemCode == test).ToList();
                if (cekFirst.Count == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public List<IcItem> GetIcItem(bool disabled = true)
        {
            return _context.IcItems
                           .Where(x => disabled || !x.Disabled)
                           .OrderBy(x => x.NamaItem)
                           .ToList();
        }

        /// <summary>
        /// Get IcItem dengan filter status yang lebih fleksibel
        /// </summary>
        /// <param name="filter">"all" = semua, "active" = hanya aktif, "inactive" = hanya non-aktif</param>
        /// <returns>List of IcItem sesuai filter</returns>
        public List<IcItem> GetIcItemByStatus(string filter = "all")
        {
            return filter switch
            {
                "active" => _context.IcItems
                                    .Where(x => !x.Disabled)
                                    .OrderBy(x => x.NamaItem)
                                    .ToList(),
                "inactive" => _context.IcItems
                                      .Where(x => x.Disabled)
                                      .OrderBy(x => x.NamaItem)
                                      .ToList(),
                _ => _context.IcItems  // "all"
                             .OrderBy(x => x.NamaItem)
                             .ToList()
            };
        }


        public List<IcAltItem> GetIcAltItem()
        {
            return _context.IcAltItems.OrderBy(x => x.NamaItem).ToList();
        }

        public IcItem GetIcItemId(int itemKode)
        {
            return _context.IcItems.Where(x => x.IcItemId == itemKode).FirstOrDefault();
        }

        public IcItem GetIcItemProduk(string itemKode)
        {
            return _context.IcItems.Where(x => x.ItemCode == itemKode).FirstOrDefault();
        }

        public async Task<bool> DelIcItem(int codeview)
        {
            try
            {
                var ExistingDist = _context.IcItems.Where(x => x.IcItemId == codeview).FirstOrDefault();
                if (ExistingDist != null)
                {
                    _context.IcItems.Remove(ExistingDist);
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

        public bool AddIcItem(IcItemView produk)
        {
            //  string rootpath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot/ImageItem/");
            //  string filePath = "/ImageItem/";
            //  string filename = Guid.NewGuid().ToString()+".jpg";
            //   string path = Path.Combine(rootpath, filename);

            string test = produk.ItemCode.ToUpper();
            var cekFirst = _context.IcItems.Where(x => x.ItemCode == test).ToList();

            if (cekFirst.Count == 0)
            {
                IcItem Produk = new IcItem()
                {
                    ItemCode = produk.ItemCode.ToUpper(),
                    NamaItem = produk.NamaItem,
                    Satuan = produk.Satuan,
                    Divisi = produk.Divisi,
                    JnsBrng = (int)produk.JnsBrng,
                    CostMethod = (int)produk.CostMethod,
                    AcctSet = produk.AcctSet,
                    Category = produk.Category,
                    StdPrice = produk.StdPrice,
                    CostAwal = produk.CostAwal,
                    SaldoAwal = produk.SaldoAwal,
                    NamaLengkap = produk.NamaLengkap,
                    Foto = produk.Foto

                };

                _context.IcItems.Add(Produk);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditIcItem(IcItemView produk)
        {
            try
            {
                var ExistingItem = _context.IcItems.Where(x => x.IcItemId == produk.IcItemId).FirstOrDefault();
                if (ExistingItem != null)
                {
                    ExistingItem.NamaItem = produk.NamaItem;
                    ExistingItem.Satuan = produk.Satuan;
                    ExistingItem.Divisi = produk.Divisi;
                    ExistingItem.JnsBrng = (int)produk.JnsBrng;
                    ExistingItem.CostMethod = (int)produk.CostMethod;
                    ExistingItem.AcctSet = produk.AcctSet;
                    ExistingItem.Category = produk.Category;
                    ExistingItem.NamaLengkap = produk.NamaLengkap;
                    ExistingItem.StdPrice = produk.StdPrice;
                    ExistingItem.CostAwal = produk.CostAwal;
                    ExistingItem.SaldoAwal = produk.SaldoAwal;
                    ExistingItem.Foto = produk.Foto;

                    _context.IcItems.Update(ExistingItem);
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


        #region IcDiv Class

        public bool CekKdDivisi(string item)
        {
            string test = item.ToUpper();
            var cekFirst = _context.IcDivs.Where(x => x.Divisi == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<IcDiv> GetIcDiv()
        {
            return _context.IcDivs.OrderBy(x => x.Divisi).ToList();
        }

        public IcDiv GetIcDivId(int id)
        {
            return _context.IcDivs.Where(x => x.IcDivId == id).FirstOrDefault();
        }

        public IcDiv GetIcDivKd(string id)
        {
            return _context.IcDivs.Where(x => x.Divisi == id).FirstOrDefault();
        }

        public bool AddIcDiv(IcDivView codeview)
        {
            string test = codeview.Divisi.ToUpper();
            var cekFirst = _context.IcDivs.Where(x => x.Divisi == test).ToList();
            if (cekFirst.Count == 0)
            {
                IcDiv Division = new IcDiv()
                {
                    Divisi = codeview.Divisi.ToUpper(),
                    NamaDiv = codeview.NamaDiv

                };
                _context.IcDivs.Add(Division);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditIcDiv(IcDivView codeview)
        {
            try
            {
                var ExistingDiv = _context.IcDivs.Where(x => x.IcDivId == codeview.IcDivId).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    ExistingDiv.NamaDiv = codeview.NamaDiv;


                    _context.IcDivs.Update(ExistingDiv);
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

        public async Task<bool> DelIcDiv(int codeview)
        {
            try
            {
                var ExistingDiv = _context.IcDivs.Where(x => x.IcDivId == codeview).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    _context.IcDivs.Remove(ExistingDiv);
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

        #endregion IcDiv Class

        #region IcLokasi Class

        public bool CekKdLokasi(string item)
        {
            string test = item.ToUpper();
            var cekFirst = _context.Iclokasis.Where(x => x.Lokasi == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<IcLokasi> GetIcLokasi()
        {
            return _context.Iclokasis.OrderBy(x => x.Lokasi).ToList();
        }

        public IcLokasi GetIcLokasiId(int id)
        {
            return _context.Iclokasis.Where(x => x.IcLokasiId == id).FirstOrDefault();
        }

        public IcLokasi GetIcLokasiKode(string id)
        {
            return _context.Iclokasis.Where(x => x.Lokasi == id).FirstOrDefault();
        }

        public bool AddIcLokasi(IcLokasiView codeview)
        {
            string test = codeview.Lokasi.ToUpper();
            var cekFirst = _context.Iclokasis.Where(x => x.Lokasi == test).ToList();
            if (cekFirst.Count == 0)
            {
                IcLokasi Location = new IcLokasi()
                {
                    Lokasi = codeview.Lokasi.ToUpper(),
                    NamaLokasi = codeview.NamaLokasi

                };
                _context.Iclokasis.Add(Location);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }

        }

        public async Task<bool> EditIcLokasi(IcLokasiView codeview)
        {
            try
            {
                var ExistingDiv = _context.Iclokasis.Where(x => x.IcLokasiId == codeview.IcLokasiId).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    ExistingDiv.NamaLokasi = codeview.NamaLokasi;


                    _context.Iclokasis.Update(ExistingDiv);
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

        public async Task<bool> DelIcLokasi(int codeview)
        {
            try
            {
                var ExistingDiv = _context.Iclokasis.Where(x => x.IcLokasiId == codeview).FirstOrDefault();
                if (ExistingDiv != null)
                {
                    _context.Iclokasis.Remove(ExistingDiv);
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

        #endregion IcLokasi Class

        #region IcCat Class

        public bool CekCategory(string item)
        {
            string test = item.ToUpper();
            var cekFirst = _context.IcCats.Where(x => x.CatCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<IcCat> GetIcCat()
        {
            return _context.IcCats.ToList();
        }

        public IcCat GetIcCatId(int id)
        {
            return _context.IcCats.Where(x => x.IcCatId == id).FirstOrDefault();
        }

        public bool AddIcCat(IcCatView codeview)
        {
            string test = codeview.CatCode.ToUpper();
            var cekFirst = _context.IcCats.Where(x => x.CatCode == test).ToList();
            if (cekFirst.Count == 0)
            {
                IcCat AcctCode = new IcCat()
                {
                    CatCode = codeview.CatCode.ToUpper(),
                    Description = codeview.Description,
                    Cat1 = codeview.Cat1,
                    Cat2 = codeview.Cat2,
                    Cat3 = codeview.Cat3,
                    Cat4 = codeview.Cat4,
                    Cat5 = codeview.Cat5,
                    Cat6 = codeview.Cat6

                };
                _context.IcCats.Add(AcctCode);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }
        }

        public async Task<bool> EditIcCat(IcCatView codeview)
        {
            try
            {
                var ExistingIcCat = _context.IcCats.Where(x => x.IcCatId == codeview.IcCatId).FirstOrDefault();
                if (ExistingIcCat != null)
                {
                    ExistingIcCat.Description = codeview.Description;
                    ExistingIcCat.Cat1 = codeview.Cat1;
                    ExistingIcCat.Cat2 = codeview.Cat2;
                    ExistingIcCat.Cat3 = codeview.Cat3;
                    ExistingIcCat.Cat4 = codeview.Cat4;
                    ExistingIcCat.Cat5 = codeview.Cat5;
                    ExistingIcCat.Cat6 = codeview.Cat6;


                    _context.IcCats.Update(ExistingIcCat);
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

        public async Task<bool> DelIcCat(int codeview)
        {
            try
            {
                var ExistingIcCat = _context.IcCats.Where(x => x.IcCatId == codeview).FirstOrDefault();
                if (ExistingIcCat != null)
                {
                    _context.IcCats.Remove(ExistingIcCat);
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
        #endregion IcCat Class

        #region IcAcct Class

        public bool CekAcctSet(string item)
        {
            string test = item.ToUpper();
            var cekFirst = _context.IcAccts.Where(x => x.AcctSet == test).ToList();
            if (cekFirst.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<IcAcct> GetIcAcct()
        {
            return _context.IcAccts.ToList();
        }

        public IcAcct GetIcAcctId(int id)
        {
            return _context.IcAccts.Where(x => x.IcAcctId == id).FirstOrDefault();
        }

        public bool AddIcAcct(IcAcctView codeview)
        {
            string test = codeview.AcctSet.ToUpper();
            var cekFirst = _context.IcAccts.Where(x => x.AcctSet == test).ToList();
            if (cekFirst.Count == 0)
            {
                IcAcct AcctCode = new IcAcct()
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
                _context.IcAccts.Add(AcctCode);
                _context.SaveChanges();
                return true;
            }
            else
            {

                return false;
            }


        }

        public async Task<bool> EditIcAcct(IcAcctView codeview)
        {
            try
            {
                var ExistingIcAcct = _context.IcAccts.Where(x => x.IcAcctId == codeview.IcAcctId).FirstOrDefault();
                if (ExistingIcAcct != null)
                {
                    ExistingIcAcct.Description = codeview.Description;
                    ExistingIcAcct.Acct1 = codeview.Acct1;
                    ExistingIcAcct.Acct2 = codeview.Acct2;
                    ExistingIcAcct.Acct3 = codeview.Acct3;
                    ExistingIcAcct.Acct4 = codeview.Acct4;
                    ExistingIcAcct.Acct5 = codeview.Acct5;
                    ExistingIcAcct.Acct6 = codeview.Acct6;

                    _context.IcAccts.Update(ExistingIcAcct);
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

        public async Task<bool> DelIcAcct(int codeview)
        {
            try
            {
                var ExistingIcAcct = _context.IcAccts.Where(x => x.IcAcctId == codeview).FirstOrDefault();
                if (ExistingIcAcct != null)
                {
                    _context.IcAccts.Remove(ExistingIcAcct);
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
        #endregion IcAcct Class

        #region Transaksi Mutasi Class



        public IcTransH GetIcTrans(int id)
        {
            return _context.IcTransHs.Include(p => p.IcTransDs).Where(x => x.IcTransHId == id).FirstOrDefault();
        }


        public List<IcTransH> GetTransH()
        {
            List<IcTransH> IcTrans = new List<IcTransH>();
            try
            {
                IcTrans = _context.IcTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "90").ToList();

            }
            catch (Exception)
            {
                throw;
            }
            return IcTrans;
            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //  return await _context.ApTransHs.OrderByDescending(x => x.Tanggal).ToListAsync();
            //  return await _context.ApTransHs.ToListAsync();

        }

        public List<IcTransH> Get3TransH()
        {
            List<IcTransH> IcTrans = new List<IcTransH>();

            IcTrans = _context.IcTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "90").ToList();


            return IcTrans;

            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //   return _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3)).ToListAsync();

        }

        public List<IcTransD> GetTransD()
        {
            return _context.IcTransDs.ToList();
        }

        public IcTransH AddTransH(IcTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();

            IcTransH transH = new IcTransH
            {
                NoFaktur = GetNumber(),
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Lokasi = trans.Lokasi,
                Lokasi2 = trans.Lokasi2,
                Kode = "90",
                IcTransDs = new List<IcTransD>()
            };
            foreach (var item in trans.IcTransDs)
            {
                transH.IcTransDs.Add(new IcTransD()
                {
                    ItemCode = item.ItemCode,
                    NamaItem = item.NamaItem,
                    Satuan = item.Satuan,
                    QtyShp = item.QtyShp,
                    Kode = "90",
                    Lokasi = trans.Lokasi,
                    Lokasi2 = trans.Lokasi2,
                    NoFaktur = transH.NoFaktur,
                    Tanggal = trans.Tanggal
                });

                IcItem cekItem = _context.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                if (cekItem != null)
                {
                    IcAltItem cekLokasi1 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == trans.Lokasi).FirstOrDefault();
                    if (cekLokasi1 == null)
                    {
                        IcAltItem Produk = new IcAltItem()
                        {
                            ItemCode = cekItem.ItemCode.ToUpper(),
                            NamaItem = cekItem.NamaItem,
                            Satuan = cekItem.Satuan,
                            Lokasi = trans.Lokasi,
                            Qty = -1 * item.QtyShp
                        };
                        _context.IcAltItems.Add(Produk);

                    }
                    else
                    {
                        cekLokasi1.Qty -= item.QtyShp;
                        _context.IcAltItems.Update(cekLokasi1);
                    }

                    /** lokasi 2 **/
                    IcAltItem cekLokasi2 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == trans.Lokasi2).FirstOrDefault();
                    if (cekLokasi2 == null)
                    {
                        IcAltItem Produk = new IcAltItem()
                        {
                            ItemCode = cekItem.ItemCode.ToUpper(),
                            NamaItem = cekItem.NamaItem,
                            Satuan = cekItem.Satuan,
                            Lokasi = trans.Lokasi2,
                            Qty = item.QtyShp
                        };
                        _context.IcAltItems.Add(Produk);

                    }
                    else
                    {
                        cekLokasi2.Qty += item.QtyShp;
                        _context.IcAltItems.Update(cekLokasi2);
                    }

                }

            }

            _context.IcTransHs.Add(transH);
            _context.SaveChanges();
            var TempTrans = GetTransDoc(transH.NoFaktur);

            return TempTrans;
            //  return true;


        }

        public IcTransH GetTransDoc(string docno)
        {
            return _context.IcTransHs.Include(p => p.IcTransDs).Where(x => x.NoFaktur == docno).FirstOrDefault();
        }

        public async Task<bool> EditTransH(IcTransHView trans)
        {

            var ExistingTrans = _context.IcTransHs.Where(x => x.IcTransHId == trans.IcTransHId).FirstOrDefault();

            /* transaksi lama dikurangi */

            if (ExistingTrans != null)
            {
                foreach (var item in ExistingTrans.IcTransDs)
                {
                    IcItem cekItem = _context.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                    if (cekItem != null)
                    {
                        IcAltItem itemlokasi1 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == ExistingTrans.Lokasi).FirstOrDefault();
                        if (itemlokasi1 != null)
                        {
                            itemlokasi1.Qty += item.QtyShp;
                            _context.IcAltItems.Update(itemlokasi1);
                        }


                        IcAltItem itemlokasi2 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == ExistingTrans.Lokasi2).FirstOrDefault();
                        if (itemlokasi2 != null)
                        {
                            itemlokasi2.Qty -= item.QtyShp;
                            _context.IcAltItems.Update(itemlokasi2);
                        }



                    }
                    _context.IcTransHs.Remove(ExistingTrans);
                }
            }

            /* transaksi update */

            IcTransH transH = new IcTransH
            {
                NoFaktur = ExistingTrans.NoFaktur,
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Lokasi = trans.Lokasi,
                Lokasi2 = trans.Lokasi2,
                Kode = "90",
                IcTransDs = new List<IcTransD>()
            };
            foreach (var item in trans.IcTransDs)
            {
                transH.IcTransDs.Add(new IcTransD()
                {
                    ItemCode = item.ItemCode,
                    NamaItem = item.NamaItem,
                    QtyShp = item.QtyShp,
                    Satuan = item.Satuan,
                    Kode = "90",
                    Lokasi = transH.Lokasi,
                    Lokasi2 = transH.Lokasi2,
                    NoFaktur = transH.NoFaktur,
                    Tanggal = trans.Tanggal
                });

                IcItem cekItem = _context.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                if (cekItem != null)
                {
                    IcAltItem cekLokasi1 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == transH.Lokasi).FirstOrDefault();
                    if (cekLokasi1 == null)
                    {
                        IcAltItem Produk = new IcAltItem()
                        {
                            ItemCode = cekItem.ItemCode.ToUpper(),
                            NamaItem = cekItem.NamaItem,
                            Satuan = cekItem.Satuan,
                            Lokasi = transH.Lokasi,
                            Qty = -1 * item.QtyShp
                        };
                        _context.IcAltItems.Add(Produk);

                    }
                    else
                    {
                        cekLokasi1.Qty -= item.QtyShp;
                        _context.IcAltItems.Update(cekLokasi1);
                    }

                    /** lokasi 2 **/
                    IcAltItem cekLokasi2 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == transH.Lokasi2).FirstOrDefault();
                    if (cekLokasi2 == null)
                    {
                        IcAltItem Produk = new IcAltItem()
                        {
                            ItemCode = cekItem.ItemCode.ToUpper(),
                            NamaItem = cekItem.NamaItem,
                            Satuan = cekItem.Satuan,
                            Lokasi = transH.Lokasi2,
                            Qty = item.QtyShp
                        };
                        _context.IcAltItems.Add(Produk);

                    }
                    else
                    {
                        cekLokasi2.Qty += item.QtyShp;
                        _context.IcAltItems.Update(cekLokasi2);
                    }

                }

            }

            _context.IcTransHs.Add(transH);

            await _context.SaveChangesAsync();

            return true;


        }

        public async Task<bool> DelTransH(int id)
        {
            try
            {
                var ExistingTrans = _context.IcTransHs.Where(x => x.IcTransHId == id).FirstOrDefault();

                if (ExistingTrans != null)
                {
                    foreach (var item in ExistingTrans.IcTransDs)
                    {
                        IcAltItem itemlokasi1 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                        itemlokasi1.Qty += item.QtyShp;

                        IcAltItem itemlokasi2 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi2).FirstOrDefault();
                        itemlokasi2.Qty -= item.QtyShp;

                        _context.IcAltItems.Update(itemlokasi1);
                    }
                    _context.IcTransHs.Remove(ExistingTrans);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        #endregion Transaksi Mutasi Class



        public string GetNumber()
        {
            string kodeno = "MUT";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.IcTransHs.Where(x => x.NoFaktur.Substring(0, 10).Equals(xbukti)).ToList();
            if (maxlist != null)
            {
                maxvalue = maxlist.Max(x => x.NoFaktur);

            }

            //            var maxvalue = (from e in db.CbTransHs where  e.Docno.Substring(0, 7) == kodeno + thnbln select e).Max();
            string nourut = "00000";
            if (maxvalue == null)
            {
                nourut = "00000";
            }
            else
            {
                nourut = maxvalue.Substring(10, 5);
            }

            //  nourut =Convert.ToString(Int32.Parse(nourut) + 1);


            string cAngNo = xbukti + (Int32.Parse(nourut) + 1).ToString("00000");
            // var maxvalue = (from e in db.AptTranss where e.NoRef.Substring(0, 7) == "ANG" + cAngNo select e.NoRef.Max()).FirstOrDefault();
            return cAngNo;

        }


        #region laporancurrentstock

        public List<IcStockCardView> GetCurrentStock()
        {
            List<IcStockCardView> trans = new List<IcStockCardView>();

            var icStock = _context.IcItems.Where(x => x.Qty != 0).ToList();
            var divisi = _context.IcDivs.ToList();

            if (icStock != null)
            {
                foreach (var ic in icStock)
                {

                    trans.Add(new IcStockCardView
                    {
                        ItemCode = ic.ItemCode,
                        NamaItem = ic.NamaItem,
                        Satuan = ic.Satuan,
                        Foto = ic.Foto,
                        Qty = ic.Qty,
                        KodeDivisi = ic.Divisi,
                        Divisi = (from e in divisi where e.Divisi == ic.Divisi select e.NamaDiv).FirstOrDefault(),

                    });


                }
            };

            return trans.OrderBy(x => x.KodeDivisi).ToList();
        }
        #endregion


        #region laporanlokasi

        public List<IcItemQtyByLocationView> GetAllIcItemQtyByLocation()
        {
            // OPTIMIZATION: Load all data with AsNoTracking since this is read-only
            var icAltItems = _context.IcAltItems.AsNoTracking().ToList();
            var icItems = _context.IcItems.AsNoTracking().ToList();
            var icLocations = _context.Iclokasis.AsNoTracking().ToList();

            // ⬇️ Tambahkan lokasi V1 jika belum ada
            var v1Location = _context.Iclokasis.FirstOrDefault(l => l.Lokasi == "P1");
            if (v1Location == null)
            {
                v1Location = new IcLokasi
                {
                    Lokasi = "P1",
                    NamaLokasi = "Virtual / Default"
                };

                _context.Iclokasis.Add(v1Location);
                _context.SaveChanges();
            }

            // Refresh list setelah penambahan
            icLocations = _context.Iclokasis.ToList();


            var icItemQtyByLocations = icAltItems.GroupJoin(icLocations, alt => alt.Lokasi, loc => loc.Lokasi,
                (alts, loc) => new { alts, loc }).ToList();

            var itembyLocationQty = new List<IcItemQtyByLocationView>();

            foreach (var item in icItems)
            {
                var locations = new List<IcLocationQtyView>();

                locations.AddRange(icLocations.Select(loc => new IcLocationQtyView
                {
                    Lokasi = loc.Lokasi,
                    NamaLokasi = loc.NamaLokasi
                }));
                locations.Where(x => x.Lokasi == "P1").First().Qty += item.SaldoAwal;

                itembyLocationQty.Add(new IcItemQtyByLocationView
                {
                    ItemCode = item.ItemCode,
                    NamaItem = item.NamaItem,
                    Satuan = item.Satuan,
                    Qty = item.Qty,
                    //QtyAwal = item.SaldoAwal,
                    Locations = locations
                });
            }

            foreach (var alts in icAltItems)
            {
                var itemByLocationQty = itembyLocationQty.FirstOrDefault(q => q.ItemCode == alts.ItemCode);
                if (itemByLocationQty != null)
                {
                    foreach (var locationQty in itemByLocationQty.Locations)
                    {
                        if (locationQty.Lokasi == alts.Lokasi)
                        {
                            locationQty.Qty += alts.Qty;
                            break;
                        }
                    }
                }
            }


            //            .SelectMany(x => x.loc.DefaultIfEmpty(), (x, locatio) => new IcItemQtyByLocationView
            //            {
            //                ItemCode = x.alts.ItemCode,
            //                NamaItem = x.alts.NamaItem,
            //                Locations = x.loc.Select(l => new IcLocationQtyView
            //                {
            //                    Lokasi = l.Lokasi,
            //                    NamaLokasi = l.NamaLokasi,
            //                    Qty = x.alts.Qty
            //                }).Concat(
            //        new[] { new IcLocationQtyView
            //        {
            //            Lokasi = locatio?.Lokasi,
            //            NamaLokasi = locatio?.NamaLokasi,
            //            Qty = 0
            //        }}).ToList()
            //            })
            //.GroupBy(x => new { x.ItemCode, x.NamaItem })
            //.Select(g => new IcItemQtyByLocationView
            //{
            //    ItemCode = g.Key.ItemCode,
            //    NamaItem = g.Key.NamaItem,
            //    Locations = g.SelectMany(x => x.Locations).Distinct().ToList()
            //})
            //.ToList();

            //new IcItemQtyByLocationView
            //    {
            //        ItemCode = alts.ItemCode,
            //        NamaItem = alts.NamaItem,
            //        Locations = loc.Select(locatio => new IcLocationQtyView
            //        {
            //            Lokasi = locatio.Lokasi,
            //            NamaLokasi = locatio.NamaLokasi,
            //            Qty = alts.Qty
            //        }).ToList()
            //    }).ToList();

            //icItemQtyByLocations = icItemQtyByLocations.GroupBy(x => new { x.ItemCode, x.NamaItem }).Select(x => new IcItemQtyByLocationView
            //{
            //    ItemCode = x.Key.ItemCode,
            //    NamaItem = x.Key.NamaItem,
            //    Locations = x.SelectMany(y => y.Locations).ToList()
            //}).ToList();

            return itembyLocationQty;
          //  return icItemQtyByLocations;
        }
        #endregion

        #region ubahdivisiitem

        public bool RubahDivisiItem(int xKdBank, string cDivisi)
        {
            try
            {
                var itemToUpdate = _context.IcItems.Where(x => x.IcItemId == xKdBank).FirstOrDefault();
                itemToUpdate.Divisi = cDivisi;
                _context.IcItems.Update(itemToUpdate);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                throw;
            }

            
        }

        #endregion
    }
}
