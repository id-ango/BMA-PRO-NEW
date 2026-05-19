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
        private readonly DbContextPersediaan _context;

        public IcAdjustServices(DbContextPersediaan context)
        {
            _context = context;
        }

        


        public IcTransH GetIcTrans(int id)
        {
            return _context.IcTransHs.Include(p => p.IcTransDs).Where(x => x.IcTransHId == id).FirstOrDefault();
        }


        public List<IcTransH> GetTransH()
        {
            List<IcTransH> IcTrans = new List<IcTransH>();
            try
            {
                IcTrans = _context.IcTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Kode == "81").ToList();

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

            IcTrans = _context.IcTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3) && x.Kode == "81").ToList();


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
                Kode = "81",
                IcTransDs = new List<IcTransD>()
            };
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

                IcItem cekItem = _context.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                if (cekItem != null)
                {
                    if (item.QtyShp != 0)
                    {
                        IcAltItem cekLokasi1 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
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

                            _context.IcAltItems.Add(Produk);

                        }
                        else
                        {
                            cekLokasi1.Qty += item.QtyShp;
                            _context.IcAltItems.Update(cekLokasi1);
                        }
                    }

                    cekItem.Qty += item.QtyShp;
                    cekItem.Cost += item.Jumlah;
                    cekItem.HrgNetto = cekItem.Qty != 0 ? cekItem.Cost / cekItem.Qty : cekItem.Harga;

                    _context.IcItems.Update(cekItem);

                }

            }

            _context.IcTransHs.Add(transH);

            _context.SaveChanges();
            var TempTrans = GetTransDoc(transH.NoFaktur);

            return TempTrans;


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
                        if (item.QtyShp != 0)
                        {
                            IcAltItem itemlokasi1 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                            if (itemlokasi1 != null)
                            {
                                itemlokasi1.Qty -= item.QtyShp;
                                _context.IcAltItems.Update(itemlokasi1);
                            }

                        }


                    }
                    cekItem.Qty -= item.QtyShp;
                    cekItem.Cost -= item.Jumlah;
                    cekItem.HrgNetto = cekItem.Qty != 0 ? cekItem.Cost / cekItem.Qty : cekItem.Harga;

                    _context.IcItems.Update(cekItem);

                }
                _context.IcTransHs.Remove(ExistingTrans);
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

                IcItem cekItem = _context.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                if (cekItem != null)
                {
                    if (item.QtyShp != 0)
                    {
                        IcAltItem cekLokasi1 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
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
                            _context.IcAltItems.Add(Produk);

                        }
                        else
                        {
                            cekLokasi1.Qty += item.QtyShp;
                            _context.IcAltItems.Update(cekLokasi1);
                        }
                    }

                    cekItem.Qty += item.QtyShp;
                    cekItem.Cost += item.Jumlah;
                    cekItem.HrgNetto = cekItem.Qty != 0 ? cekItem.Cost / cekItem.Qty : cekItem.Harga;

                    _context.IcItems.Update(cekItem);
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
                        IcItem cekItem = _context.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                        if (cekItem != null)
                        {
                            if (item.QtyShp != 0)
                            {
                                IcAltItem itemlokasi1 = _context.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
                                if (itemlokasi1 != null)
                                {
                                    itemlokasi1.Qty -= item.QtyShp;
                                    _context.IcAltItems.Update(itemlokasi1);
                                }

                            }


                        }
                        cekItem.Qty -= item.QtyShp;
                        cekItem.Cost -= item.Jumlah;
                        cekItem.HrgNetto = cekItem.Qty != 0 ? cekItem.Cost / cekItem.Qty : cekItem.Harga;

                        _context.IcItems.Update(cekItem);


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

        public IcTransH GetTransDoc(string docno)
        {
            return _context.IcTransHs.Include(p => p.IcTransDs).Where(x => x.NoFaktur == docno).FirstOrDefault();
        }

        public string GetNumber()
        {
            string kodeno = "ADJ";
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
    }
}
