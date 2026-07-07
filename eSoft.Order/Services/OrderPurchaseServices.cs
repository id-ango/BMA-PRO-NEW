using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eSoft.Order.Data;
using eSoft.Order.Model;
using eSoft.Order.View;
using eSoft.Hutang.Data;
using eSoft.Hutang.Model;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;

using Microsoft.EntityFrameworkCore;


namespace eSoft.Order.Services
{
    public class OrderPurchaseServices : IOrderPurchaseServices
    {
        private readonly IDbContextFactory<DbContextOrder> _context;
        private readonly IDbContextFactory<DbContextHutang> _contextAp;
        private readonly IDbContextFactory<DbContextPersediaan> _contextIc;

        public OrderPurchaseServices(IDbContextFactory<DbContextOrder> context, IDbContextFactory<DbContextHutang> contextHutang, IDbContextFactory<DbContextPersediaan> contextPersediaan)
        {
            _context = context;
            _contextAp = contextHutang;
            _contextIc = contextPersediaan;
        }

        #region getclass

        private ApSuppl GetVendorId(string id)
        {
            using var contextAp = _contextAp.CreateDbContext();
            return contextAp.ApSuppls.Where(x => x.Supplier == id).FirstOrDefault();
        }

        public ApHutang GetHutang(string bukti)
        {
            using var contextAp = _contextAp.CreateDbContext();
            return contextAp.ApHutangs.Where(x => x.Dokumen == bukti).FirstOrDefault();

        }

        #endregion getclass

        #region PoTransH class

        public PoTransH GetPoTrans(int id)
        {
            using var db = _context.CreateDbContext();
            return db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.PoTransHId == id).FirstOrDefault();
        }

        public List<PoTransH> GetTransHAktif()
        {
            using var db = _context.CreateDbContext();
            List<PoTransH> PoTrans = new List<PoTransH>();


            try
            {
                PoTrans = db.PoTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Kode == "71" && x.Cek == "1").ToList();

            }
            catch (Exception)
            {
                throw;
            }
            return PoTrans;


        }
        public void SaveOrderAktif(string customer)
        {
            using var db = _context.CreateDbContext();
            db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == customer).FirstOrDefault().Cek = "3";
            db.SaveChanges();

            //  return true;
        }

        public void DelOrderAktif(string customer)
        {
            using var db = _context.CreateDbContext();
            db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == customer).FirstOrDefault().Cek = "1";
            db.SaveChanges();

            //  return true;
        }

        public PoTransH GetOrderAktif(string customer)
        {
            using var db = _context.CreateDbContext();
            return db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == customer).FirstOrDefault();
        }

        public List<PoTransH> GetTransH()
        {
            using var db = _context.CreateDbContext();
            List<PoTransH> PoTrans = new List<PoTransH>();


            try
            {
                PoTrans = db.PoTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Kode == "71").ToList();
                //  PoTrans = (from e in db.PoTransHs orderby e.Tanggal where e.Kode == "71" select e).ToList();

                //foreach (var item in PoTrans)
                //{
                //    item.NamaVendor = dbAp.ApSuppls.Where(x => x.Supplier == item.Vendor).FirstOrDefault().NamaLengkap;
                //}

            }
            catch (Exception)
            {
                throw;
            }
            return PoTrans;
            // return  db.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //  return await db.ApTransHs.OrderByDescending(x => x.Tanggal).ToListAsync();
            //  return await db.ApTransHs.ToListAsync();

        }

        public List<PoTransH> Get3TransH()
        {
            using var db = _context.CreateDbContext();
            List<PoTransH> PoTrans = new List<PoTransH>();

            PoTrans = db.PoTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Tanggal.Date > DateTime.Today.Date.AddMonths(-3) && x.Kode == "71").ToList();

            return PoTrans;

            // return  db.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //   return db.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3)).ToListAsync();

        }

        public List<PoTransD> GetTransD()
        {
            using var db = _context.CreateDbContext();
            return db.PoTransDs.ToList();
        }

        public PoTransH AddTransH(PoTransHView trans)
        {
            using var db = _context.CreateDbContext();
            using var dbIc = _contextIc.CreateDbContext();
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = db.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            decimal mQty5 = 0;

            PoTransH transH = new PoTransH
            {
                NoLpb = GetNumber(),
                Vendor = trans.Vendor.ToUpper(),
                NamaVendor = trans.NamaVendor,
                Currency = trans.Currency,
                NoPrj = trans.NoPrj,
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Jumlah = trans.Jumlah,
                Ongkos = trans.Ongkos,
                Ppn = trans.Ppn,
                PpnPersen = trans.PpnPersen,
                TtlJumlah = trans.TtlJumlah,
                DPayment = trans.DPayment,
                Tagihan = trans.Tagihan,
                TotalQty = trans.TotalQty,
                Kode = "71",
                Cek = "1",

                PoTransDs = new List<PoTransD>()
            };

            foreach (var item in trans.PoTransDs)
            {
                if (item.Qty != 0)
                {
                    if (transH.TotalQty != 0)
                    {
                        mQty5 = (item.Jumlah - item.Discount) - (item.Qty / transH.TotalQty * transH.Ppn) + (item.Qty / transH.TotalQty * transH.Ongkos);
                    }
                    else
                    {
                        mQty5 = (item.Jumlah - item.Discount);
                    }

                    transH.PoTransDs.Add(new PoTransD()
                    {
                        ItemCode = item.ItemCode.ToUpper(),
                        NamaItem = item.NamaItem,
                        Satuan = item.Satuan,
                        Lokasi = item.Lokasi,
                        Harga = item.Harga,
                        Qty = item.Qty,
                        Persen = item.Persen,
                        Discount = item.Discount,
                        Jumlah = item.Jumlah,
                        Kode = "71",
                        NoLpb = transH.NoLpb,
                        Tanggal = trans.Tanggal,
                        JumDpp = mQty5
                    });

                    IcItem cekItem = dbIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();

                    if (cekItem != null)
                    {

                        //  if(item.Harga > 0 && item.Harga > cekItem.HrgUsd)
                        //          cekItem.HrgUsd = item.Harga;  // harga beli barang

                        if (item.Harga > 0)
                        {
                            cekItem.HrgUsd = item.Harga;  // harga beli barang
                            cekItem.CurrencyCode = trans.Currency;
                        }

                        dbIc.IcItems.Update(cekItem);

                    }
                }
                db.PoTransHs.Add(transH);
            }



            db.SaveChanges();

            dbIc.SaveChanges();

            var TempTrans = GetTransDoc(transH.NoLpb);

            return TempTrans;

        }

        public PoTransH GetTransDoc(string docno)
        {
            using var db = _context.CreateDbContext();
            return db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == docno).FirstOrDefault();
        }

        public async Task<bool> DelTransH(int id)
        {
            using var db = _context.CreateDbContext();
            try
            {
                var ExistingTrans = db.PoTransHs.Where(x => x.PoTransHId == id).FirstOrDefault();

                if (ExistingTrans != null)
                {

                    db.PoTransHs.Remove(ExistingTrans);
                    await db.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        public async Task<bool> EditTransH(PoTransHView trans)
        {
            using var db = _context.CreateDbContext();
            using var dbIc = _contextIc.CreateDbContext();
            decimal mQty5 = 0;

            //   var cekFirst = dbAp.ApHutangs.Where(x => x.Dokumen == trans.NoLpb && x.Bayar == 0).FirstOrDefault();

            if (true)
            {
                try
                {

                    var ExistingTrans = db.PoTransHs.Where(x => x.PoTransHId == trans.PoTransHId).FirstOrDefault();
                    //    var ExistingTrans = db.PoTransHs.Include(x => x.PoTransDs).Where(x => x.PoTransHId == trans.PoTransHId).FirstOrDefault();

                    if (ExistingTrans != null)
                    {

                        db.PoTransHs.Remove(ExistingTrans);

                        /* update nya */
                        PoTransH transH = new PoTransH
                        {
                            NoLpb = trans.NoLpb,
                            Vendor = trans.Vendor.ToUpper(),
                            Currency = trans.Currency,
                            NamaVendor = trans.NamaVendor,
                            NoPrj = trans.NoPrj,
                            Tanggal = trans.Tanggal,
                            Keterangan = trans.Keterangan,
                            Jumlah = trans.Jumlah,
                            Ongkos = trans.Ongkos,
                            Ppn = trans.Ppn,
                            PpnPersen = trans.PpnPersen,
                            TtlJumlah = trans.TtlJumlah,
                            DPayment = trans.DPayment,
                            Tagihan = trans.Tagihan,
                            TotalQty = trans.TotalQty,
                            Kode = "71",
                            Cek = "1",

                            PoTransDs = new List<PoTransD>()
                        };

                        foreach (var item in trans.PoTransDs)
                        {
                            if (item.Qty != 0)
                            {
                                if (transH.TotalQty != 0)
                                {
                                    mQty5 = (item.Jumlah - item.Discount) - (item.Qty / transH.TotalQty * transH.Ppn) + (item.Qty / transH.TotalQty * transH.Ongkos);
                                }

                                transH.PoTransDs.Add(new PoTransD()
                                {
                                    ItemCode = item.ItemCode.ToUpper(),
                                    NamaItem = item.NamaItem,
                                    Satuan = item.Satuan,
                                    Lokasi = item.Lokasi,
                                    Harga = item.Harga,
                                    Qty = item.Qty,
                                    Persen = item.Persen,
                                    Discount = item.Discount,
                                    Jumlah = item.Jumlah,
                                    Kode = "71",
                                    NoLpb = transH.NoLpb,
                                    Tanggal = trans.Tanggal,
                                    JumDpp = mQty5
                                });


                                IcItem cekItem = dbIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                                if (cekItem != null)
                                {
                                    //  cekItem.HrgUsd = item.Harga;
                                    ///  if (item.Harga > 0 && item.Harga > cekItem.HrgUsd)
                                    ///      cekItem.HrgUsd = item.Harga;  // harga beli barang

                                    if (item.Harga > 0)
                                    {
                                        cekItem.HrgUsd = item.Harga;  // harga beli barang
                                        cekItem.CurrencyCode = trans.Currency;
                                    }
                                    dbIc.IcItems.Update(cekItem);

                                }
                            }

                        }



                        db.PoTransHs.Add(transH);


                        await dbIc.SaveChangesAsync();
                        await db.SaveChangesAsync();

                        //  var TempTrans = GetTransDoc(transH.NoLpb);

                        //   return transH;
                        return true;

                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
            // return false;



        }

        #endregion PoTransH Class

        public string GetNumber()
        {
            using var db = _context.CreateDbContext();
            string kodeno = "P/I";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = db.PoTransHs.Where(x => x.NoLpb.Substring(0, 10).Equals(xbukti)).ToList();
            if (maxlist != null)
            {
                maxvalue = maxlist.Max(x => x.NoLpb);

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

        #region POCurrency

        public List<PoItemQtyByLocationView> GetAllIcItemQtyByLocation(string KodeVendor)
        {
            using var db = _context.CreateDbContext();
            using var dbIc = _contextIc.CreateDbContext();
            var icAltItems = db.PoTransDs.Where(x => x.Kode == "71").ToList();
            var icItems = dbIc.IcItems.ToList();
            var icLocations = db.PoTransHs.Where(x => x.Vendor == KodeVendor && x.Kode == "71").ToList();


            //var icItemQtyByLocations = icAltItems.GroupJoin(icLocations, alt => alt.PoTransHId, loc => loc.PoTransHId,
            //    (alts, loc) => new { alts, loc }).ToList();

            var itembyLocationQty = new List<PoItemQtyByLocationView>();

            foreach (var item in icItems)
            {
                var locations = new List<PoLocationQtyView>();

                locations.AddRange(icLocations.Select(loc => new PoLocationQtyView
                {
                    Lokasi = loc.NoLpb,
                    NamaLokasi = (string.IsNullOrEmpty(loc.NoPrj) ?  loc.NoLpb : loc.NoPrj)
                }));
                //  locations.Where(x => x.Lokasi == "V1").First().Qty += item.SaldoAwal;

                itembyLocationQty.Add(new PoItemQtyByLocationView
                {
                    ItemCode = item.ItemCode,
                    NamaItem = item.NamaItem,
                    Satuan = item.Satuan,
                    Qty = item.Harga,
                    //QtyAwal = item.SaldoAwal,
                    Locations = locations
                });
            }

            foreach (var alts in icAltItems)
            {
                var itemByLocationQty = itembyLocationQty.FirstOrDefault(q => q.ItemCode == alts.ItemCode);
                if (itemByLocationQty != null)
                {
                    if (alts.Harga > 0)
                    {

                        foreach (var locationQty in itemByLocationQty.Locations)
                        {
                            if (locationQty.Lokasi == alts.NoLpb)
                            {

                                locationQty.Qty = alts.Harga;
                                itemByLocationQty.QtyAwal++;
                                break;
                            }
                        }

                    } else
                    {

                    }


                }
            }



            return itembyLocationQty.Where(x => x.QtyAwal != 0).ToList();
            //  return icItemQtyByLocations;
        }
        #endregion
    }
}
