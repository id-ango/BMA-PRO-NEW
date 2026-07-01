using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;

using Microsoft.EntityFrameworkCore;

namespace eSoft.Persediaan.Services
{
    public class IcAdjustServices : IIcAdjustServices
    {
        private readonly IDbContextFactory<DbContextPersediaan> _context;

        public IcAdjustServices(IDbContextFactory<DbContextPersediaan> context)
        {
            _context = context;
        }

        public IcTransH GetIcTrans(int id)
        {
            using var db = _context.CreateDbContext();
            return db.IcTransHs.Include(p => p.IcTransDs).Where(x => x.IcTransHId == id).FirstOrDefault();
        }


        public List<IcTransH> GetTransH()
        {
            List<IcTransH> IcTrans = new List<IcTransH>();
            using (var db = _context.CreateDbContext())
            {
                try
                {
                    IcTrans = db.IcTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "81").ToList();

                }
                catch (Exception)
                {
                    throw;
                }
            }
            return IcTrans;
            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //  return await _context.ApTransHs.OrderByDescending(x => x.Tanggal).ToListAsync();
            //  return await _context.ApTransHs.ToListAsync();

        }

        public List<IcTransH> Get3TransH()
        {
            List<IcTransH> IcTrans = new List<IcTransH>();

            using (var db = _context.CreateDbContext())
            {
                IcTrans = db.IcTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "81").ToList();
            }


            return IcTrans;

            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //   return _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3)).ToListAsync();

        }

        public List<IcTransD> GetTransD()
        {
            using var db = _context.CreateDbContext();
            return db.IcTransDs.ToList();
        }

        public IcTransH AddTransH(IcTransHView trans)
        {
            using var db = _context.CreateDbContext();
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();

            IcTransH transH = new IcTransH
            {
                NoFaktur = GetNumber(),
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Kode = "81",
                IcTransDs = new List<IcTransD>()
            };
            var altItemDictAdd = new Dictionary<string, IcAltItem>();

            foreach (var item in trans.IcTransDs)
            {
                transH.IcTransDs.Add(new IcTransD()
                {
                    ItemCode = item.ItemCode,
                    NamaItem = item.NamaItem,
                    Harga = item.Harga,
                    Satuan = item.Satuan,
                    Jumlah = item.Jumlah,
                    QtyShp = item.QtyShp,
                    Kode = "81",
                    Lokasi = item.Lokasi,
                    NoFaktur = transH.NoFaktur,
                    Tanggal = trans.Tanggal
                });

                IcItem cekItem = db.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                if (cekItem != null)
                {
                    if (item.QtyShp != 0)
                    {
                        var altKey = $"{item.ItemCode}::{item.Lokasi}";
                        if (!altItemDictAdd.TryGetValue(altKey, out IcAltItem cekLokasi1))
                        {
                            cekLokasi1 = db.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                            if (cekLokasi1 != null) altItemDictAdd[altKey] = cekLokasi1;
                        }

                        if (cekLokasi1 == null)
                        {
                            IcAltItem Produk = new IcAltItem()
                            {
                                ItemCode = cekItem.ItemCode.ToUpper(),
                                NamaItem = cekItem.NamaItem,
                                Satuan = cekItem.Satuan,
                                Lokasi = item.Lokasi,
                                Qty = item.QtyShp
                            };
                            db.IcAltItems.Add(Produk);
                            altItemDictAdd[altKey] = Produk;
                        }
                        else
                        {
                            cekLokasi1.Qty += item.QtyShp;
                            db.IcAltItems.Update(cekLokasi1);
                        }
                    }

                    cekItem.Qty += item.QtyShp;
                    cekItem.Cost += item.Jumlah;
                    cekItem.HrgNetto = cekItem.Qty != 0 ? cekItem.Cost / cekItem.Qty : cekItem.Harga;

                    db.IcItems.Update(cekItem);

                }

            }

            db.IcTransHs.Add(transH);

            db.SaveChanges();
            var TempTrans = GetTransDoc(transH.NoFaktur);

            return TempTrans;


        }

        public async Task<bool> EditTransH(IcTransHView trans)
        {

            using var db = _context.CreateDbContext();
            var ExistingTrans = db.IcTransHs.Where(x => x.IcTransHId == trans.IcTransHId).FirstOrDefault();

            /* transaksi lama dikurangi */

            if (ExistingTrans != null)
            {
                var altItemDictRev = new Dictionary<string, IcAltItem>();

                foreach (var item in ExistingTrans.IcTransDs)
                {
                    IcItem cekItem = db.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                    if (cekItem != null)
                    {
                        if (item.QtyShp != 0)
                        {
                            var altKeyRev = $"{item.ItemCode}::{item.Lokasi}";
                            if (!altItemDictRev.TryGetValue(altKeyRev, out IcAltItem itemlokasi1))
                            {
                                itemlokasi1 = db.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                                if (itemlokasi1 != null) altItemDictRev[altKeyRev] = itemlokasi1;
                            }

                            if (itemlokasi1 != null)
                            {
                                itemlokasi1.Qty -= item.QtyShp;
                                db.IcAltItems.Update(itemlokasi1);
                            }
                        }
                    }
                    cekItem.Qty -= item.QtyShp;
                    cekItem.Cost -= item.Jumlah;
                    cekItem.HrgNetto = cekItem.Qty != 0 ? cekItem.Cost / cekItem.Qty : cekItem.Harga;

                    db.IcItems.Update(cekItem);

                }
                db.IcTransHs.Remove(ExistingTrans);
            }

            /* transaksi update */

            IcTransH transH = new IcTransH
            {
                NoFaktur = ExistingTrans.NoFaktur,
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Kode = "81",
                IcTransDs = new List<IcTransD>()
            };
            var altItemDictEdit = new Dictionary<string, IcAltItem>();

            foreach (var item in trans.IcTransDs)
            {
                transH.IcTransDs.Add(new IcTransD()
                {
                    ItemCode = item.ItemCode,
                    NamaItem = item.NamaItem,
                    Harga = item.Harga,
                    Jumlah = item.Jumlah,
                    QtyShp = item.QtyShp,
                    Satuan = item.Satuan,
                    Kode = "81",
                    Lokasi = item.Lokasi,
                    NoFaktur = transH.NoFaktur,
                    Tanggal = trans.Tanggal
                });

                IcItem cekItem = db.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                if (cekItem != null)
                {
                    if (item.QtyShp != 0)
                    {
                        var altKeyEdit = $"{item.ItemCode}::{item.Lokasi}";
                        if (!altItemDictEdit.TryGetValue(altKeyEdit, out IcAltItem cekLokasi1))
                        {
                            cekLokasi1 = db.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                            if (cekLokasi1 != null) altItemDictEdit[altKeyEdit] = cekLokasi1;
                        }

                        if (cekLokasi1 == null)
                        {
                            IcAltItem Produk = new IcAltItem()
                            {
                                ItemCode = cekItem.ItemCode.ToUpper(),
                                NamaItem = cekItem.NamaItem,
                                Satuan = cekItem.Satuan,
                                Lokasi = item.Lokasi,
                                Qty = item.QtyShp
                            };
                            db.IcAltItems.Add(Produk);
                            altItemDictEdit[altKeyEdit] = Produk;
                        }
                        else
                        {
                            cekLokasi1.Qty += item.QtyShp;
                            db.IcAltItems.Update(cekLokasi1);
                        }
                    }

                    cekItem.Qty += item.QtyShp;
                    cekItem.Cost += item.Jumlah;
                    cekItem.HrgNetto = cekItem.Qty != 0 ? cekItem.Cost / cekItem.Qty : cekItem.Harga;

                    db.IcItems.Update(cekItem);
                }

            }

            db.IcTransHs.Add(transH);

            await db.SaveChangesAsync();

            return true;

        }

        public async Task<bool> DelTransH(int id)
        {
            using var db = _context.CreateDbContext();
            try
            {
                var ExistingTrans = db.IcTransHs.Where(x => x.IcTransHId == id).FirstOrDefault();


                if (ExistingTrans != null)
                {
                    var altItemDictDel = new Dictionary<string, IcAltItem>();

                    foreach (var item in ExistingTrans.IcTransDs)
                    {
                        IcItem cekItem = db.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                        if (cekItem != null)
                        {
                            if (item.QtyShp != 0)
                            {
                                var altKeyDel = $"{item.ItemCode}::{item.Lokasi}";
                                if (!altItemDictDel.TryGetValue(altKeyDel, out IcAltItem itemlokasi1))
                                {
                                    itemlokasi1 = db.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                                    if (itemlokasi1 != null) altItemDictDel[altKeyDel] = itemlokasi1;
                                }

                                if (itemlokasi1 != null)
                                {
                                    itemlokasi1.Qty -= item.QtyShp;
                                    db.IcAltItems.Update(itemlokasi1);
                                }
                            }
                        }
                        cekItem.Qty -= item.QtyShp;
                        cekItem.Cost -= item.Jumlah;
                        cekItem.HrgNetto = cekItem.Qty != 0 ? cekItem.Cost / cekItem.Qty : cekItem.Harga;

                        db.IcItems.Update(cekItem);
                    }
                    db.IcTransHs.Remove(ExistingTrans);
                    await db.SaveChangesAsync();

                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;

        }

        public IcTransH GetTransDoc(string docno)
        {
            using var db = _context.CreateDbContext();
            return db.IcTransHs.Include(p => p.IcTransDs).Where(x => x.NoFaktur == docno).FirstOrDefault();
        }

        public string GetNumber()
        {
            string kodeno = "ADJ";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            using (var db = _context.CreateDbContext())
            {
                var maxlist = db.IcTransHs.Where(x => x.NoFaktur.Substring(0, 10).Equals(xbukti)).ToList();
                if (maxlist != null)
                {
                    maxvalue = maxlist.Max(x => x.NoFaktur);

                }
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
    }
}
