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
        private readonly DbContextOrder _context;
        private readonly DbContextHutang _contextAp;
        private readonly DbContextPersediaan _contextIc;

        public OrderPurchaseServices(DbContextOrder context, DbContextHutang contextHutang, DbContextPersediaan contextPersediaan)
        {
            _context = context;
            _contextAp = contextHutang;
            _contextIc = contextPersediaan;
        }

        #region getclass

        private ApSuppl GetVendorId(string id)
        {
            return _contextAp.ApSuppls.Where(x => x.Supplier == id).FirstOrDefault();
        }

        public ApHutang GetHutang(string bukti)
        {
            return _contextAp.ApHutangs.Where(x => x.Dokumen == bukti).FirstOrDefault();

        }

        #endregion getclass

        #region PoTransH class

        public PoTransH GetPoTrans(int id)
        {
            return _context.PoTransHs.Include(p => p.PoTransDs).Where(x => x.PoTransHId == id).FirstOrDefault();
        }

        public List<PoTransH> GetTransHAktif()
        {
            List<PoTransH> PoTrans = new List<PoTransH>();


            try
            {
                PoTrans = _context.PoTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Kode == "71" && x.Cek == "1").ToList();

            }
            catch (Exception)
            {
                throw;
            }
            return PoTrans;


        }
        public void SaveOrderAktif(string customer)
        {

            _context.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == customer).FirstOrDefault().Cek = "3";
            _context.SaveChanges();

            //  return true;
        }

        public void DelOrderAktif(string customer)
        {

            _context.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == customer).FirstOrDefault().Cek = "1";
            _context.SaveChanges();

            //  return true;
        }

        public PoTransH GetOrderAktif(string customer)
        {

            return _context.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == customer).FirstOrDefault();
        }

        public List<PoTransH> GetTransH()
        {
            List<PoTransH> PoTrans = new List<PoTransH>();


            try
            {
                PoTrans = _context.PoTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Kode == "71").ToList();
                //  PoTrans = (from e in _context.PoTransHs orderby e.Tanggal where e.Kode == "71" select e).ToList();

                //foreach (var item in PoTrans)
                //{
                //    item.NamaVendor = _contextAp.ApSuppls.Where(x => x.Supplier == item.Vendor).FirstOrDefault().NamaLengkap;
                //}

            }
            catch (Exception)
            {
                throw;
            }
            return PoTrans;
            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //  return await _context.ApTransHs.OrderByDescending(x => x.Tanggal).ToListAsync();
            //  return await _context.ApTransHs.ToListAsync();

        }

        public List<PoTransH> Get3TransH()
        {
            List<PoTransH> PoTrans = new List<PoTransH>();

            PoTrans = _context.PoTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Tanggal.Date > DateTime.Today.Date.AddMonths(-3) && x.Kode == "71").ToList();

            return PoTrans;

            // return  _context.CbTransHs.Include(p =>p.CbTransDs).OrderByDescending(x =>x.Tanggal).ToListAsync();
            //   return _context.ApTransHs.OrderByDescending(x => x.Tanggal).Where(x => x.Tanggal > DateTime.Today.AddMonths(-3)).ToListAsync();

        }

        public List<PoTransD> GetTransD()
        {
            return _context.PoTransDs.ToList();
        }

        public PoTransH AddTransH(PoTransHView trans)
        {
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = _context.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
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

                    IcItem cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();

                    if (cekItem != null)
                    {

                        //  if(item.Harga > 0 && item.Harga > cekItem.HrgUsd)
                        //          cekItem.HrgUsd = item.Harga;  // harga beli barang

                        if (item.Harga > 0)
                        {
                            cekItem.HrgUsd = item.Harga;  // harga beli barang
                            cekItem.CurrencyCode = trans.Currency;
                        }

                        _contextIc.IcItems.Update(cekItem);

                    }
                }
                _context.PoTransHs.Add(transH);
            }



            _context.SaveChanges();

            _contextIc.SaveChanges();

            var TempTrans = GetTransDoc(transH.NoLpb);

            return TempTrans;

        }

        public PoTransH GetTransDoc(string docno)
        {
            return _context.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == docno).FirstOrDefault();
        }

        public async Task<bool> DelTransH(int id)
        {
            try
            {
                var ExistingTrans = _context.PoTransHs.Where(x => x.PoTransHId == id).FirstOrDefault();

                if (ExistingTrans != null)
                {

                    _context.PoTransHs.Remove(ExistingTrans);
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

        public async Task<bool> EditTransH(PoTransHView trans)
        {
            decimal mQty5 = 0;

            //   var cekFirst = _contextAp.ApHutangs.Where(x => x.Dokumen == trans.NoLpb && x.Bayar == 0).FirstOrDefault();

            if (true)
            {
                try
                {

                    var ExistingTrans = _context.PoTransHs.Where(x => x.PoTransHId == trans.PoTransHId).FirstOrDefault();
                    //    var ExistingTrans = _context.PoTransHs.Include(x => x.PoTransDs).Where(x => x.PoTransHId == trans.PoTransHId).FirstOrDefault();

                    if (ExistingTrans != null)
                    {

                        _context.PoTransHs.Remove(ExistingTrans);

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


                                IcItem cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
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
                                    _contextIc.IcItems.Update(cekItem);

                                }
                            }

                        }



                        _context.PoTransHs.Add(transH);


                        await _contextIc.SaveChangesAsync();
                        await _context.SaveChangesAsync();

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

        public async Task<bool> UpdateKeterangan(int id, string keterangan)
        {
            try
            {
                var trans = await _context.PoTransHs.FirstOrDefaultAsync(x => x.PoTransHId == id);
                if (trans != null)
                {
                    trans.Keterangan = keterangan;
                    _context.PoTransHs.Update(trans);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetNumber()
        {
            string kodeno = "P/I";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '2' + thnbln.Substring(2, 2) + '-';
            var maxvalue = "";
            var maxlist = _context.PoTransHs.Where(x => x.NoLpb.Substring(0, 10).Equals(xbukti)).ToList();
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
            var icAltItems = _context.PoTransDs.Where(x => x.Kode == "71").ToList();
            var icItems = _contextIc.IcItems.ToList();
            var icLocations = _context.PoTransHs.Where(x => x.Vendor == KodeVendor && x.Kode == "71").ToList();


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
