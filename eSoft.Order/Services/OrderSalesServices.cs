using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eSoft.Order.Data;
using eSoft.Order.Model;
using eSoft.Order.View;
using eSoft.Piutang.Data;
using eSoft.Piutang.Model;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.View;
using eSoft.Persediaan.Model;

using Microsoft.EntityFrameworkCore;


namespace eSoft.Order.Services
{
    public class OrderSalesServices : IOrderSalesServices
    {
        private readonly IDbContextFactory<DbContextOrder> _context;
        private readonly IDbContextFactory<DbContextPiutang> _contextAr;
        private readonly IDbContextFactory<DbContextPersediaan> _contextIc;

        public OrderSalesServices(IDbContextFactory<DbContextOrder> context, IDbContextFactory<DbContextPiutang> contextPiutang, IDbContextFactory<DbContextPersediaan> contextPersediaan)
        {
            _context = context;
            _contextAr = contextPiutang;
            _contextIc = contextPersediaan;
        }

        #region getclass

        private IcItem GetItemKode(string kodeItem)
        {
            using var dbIc = _contextIc.CreateDbContext();
            var item = dbIc.IcItems.Where(x => x.ItemCode == kodeItem).FirstOrDefault();

            return item;
        }
        private ArCust GetVendorId(string id)
        {
            using var dbAr = _contextAr.CreateDbContext();
            return dbAr.ArCusts.Where(x => x.Customer == id).FirstOrDefault();
        }

        private IcDiv GetDivisiKode(string kodeItem)
        {
            using var dbIc = _contextIc.CreateDbContext();
            var item = dbIc.IcItems.Where(x => x.ItemCode == kodeItem).FirstOrDefault();

            return dbIc.IcDivs.Where(x => x.Divisi == item.Divisi).FirstOrDefault();
        }
        public ArPiutng GetPiutang(string bukti)
        {
            using var dbAr = _contextAr.CreateDbContext();
            return dbAr.ArPiutngs.Where(x => x.Dokumen == bukti).FirstOrDefault();

        }

        #endregion getclass

        #region PoTransH class

        public PoTransH GetPoTrans(int id)
        {
            using var db = _context.CreateDbContext();
            return db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.PoTransHId == id).FirstOrDefault();
        }

        public List<IcStockCardView> GetListOrderAktif(string itemCode, string kodeTrans)
        {
            using var db = _context.CreateDbContext();
            List<PoTransH> poTransH = new List<PoTransH>();
            List<PoTransD> poTransD = new();

            poTransH = db.PoTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Cek == "1" && x.Kode == kodeTrans).ToList();
            poTransD = db.PoTransDs.Where(x => x.ItemCode == itemCode).ToList();

            var Transaksi = (from header in poTransH
                          join detail in poTransD
                          on header.NoLpb equals detail.NoLpb
                          select new IcStockCardView()
                          {
                              Vendor = header.Vendor,
                              NamaVendor = header.NamaVendor,
                              Tanggal = header.Tanggal,
                              Keterangan = header.Keterangan,
                              NoLpb = header.NoLpb,
                              NoPrj = header.NoPrj,
                              ItemCode = detail.ItemCode,
                              NamaItem = detail.NamaItem,
                              Qty = detail.Qty,
                              Harga = detail.Harga
                          }).ToList() ;

            return Transaksi;
        }

        public List<IcStockCardView> GetCurrentOrderJual(List<IcStockCardView> stockCard)
        {
            using var db = _context.CreateDbContext();
            List<PoTransH> PoTrans = new List<PoTransH>();
            PoTrans = db.PoTransHs.Include(t =>t.PoTransDs).OrderByDescending(x => x.Tanggal.Date).Where(x => x.Cek == "1").ToList();
          

            if (PoTrans != null)
            {
                foreach (var item in PoTrans)
                {
                    foreach (var stockCardView in item.PoTransDs)
                    {
                        //var testing = stockCard.Where(x =>x.ItemCode == stockCardView.ItemCode).FirstOrDefault();
                        if (stockCard.Find(x => x.ItemCode == stockCardView.ItemCode) != null)
                        {
                            if(stockCardView.Kode == "76")
                                stockCard.Find(x => x.ItemCode == stockCardView.ItemCode).QtyJual -= stockCardView.Qty;
                            if (stockCardView.Kode == "71")
                            {
                                stockCard.Find(x => x.ItemCode == stockCardView.ItemCode).QtyBeli += stockCardView.Qty;
                                stockCard.Find(x => x.ItemCode == stockCardView.ItemCode).Keterangan += ", " + item.NoPrj;
                            }
                                
                        }
                        else
                        {
                            if (stockCardView.Kode == "76")
                            {
                                stockCard.Add(new IcStockCardView
                                {
                                    ItemCode = stockCardView.ItemCode,
                                    NamaItem = GetItemKode(stockCardView.ItemCode).NamaItem,
                                    Foto = GetItemKode(stockCardView.ItemCode).Foto,
                                    Satuan = stockCardView.Satuan,
                                    QtyJual = -1 * stockCardView.Qty,
                                   
                                    KodeDivisi = GetDivisiKode(stockCardView.ItemCode).Divisi,
                                    Divisi = GetDivisiKode(stockCardView.ItemCode).NamaDiv

                                });
                            }
                            if (stockCardView.Kode == "71")
                            {
                                stockCard.Add(new IcStockCardView
                                {
                                    ItemCode = stockCardView.ItemCode,
                                    NamaItem = GetItemKode(stockCardView.ItemCode).NamaItem,
                                    Foto = GetItemKode(stockCardView.ItemCode).Foto,
                                    Satuan = stockCardView.Satuan,
                                    QtyBeli = stockCardView.Qty,
                                    Keterangan = item.NoPrj,
                                    KodeDivisi = GetDivisiKode(stockCardView.ItemCode).Divisi,
                                    Divisi = GetDivisiKode(stockCardView.ItemCode).NamaDiv

                                });
                            }

                        }
                    }
                }
            }

           

            return stockCard.OrderBy(x => x.KodeDivisi).ThenBy(x =>x.ItemCode).ToList();
        }


        public List<PoTransH> GetTransHAktif()
        {
            using var db = _context.CreateDbContext();
            List<PoTransH> PoTrans = new List<PoTransH>();


            try
            {
                PoTrans = db.PoTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Kode == "76" && x.Cek == "1").ToList();

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

        public void DelOrderAktif(string nolpb)
        {
            using var db = _context.CreateDbContext();
            db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == nolpb).FirstOrDefault().Cek = "1";
            db.SaveChanges();

            //  return true;
        }

        public PoTransH GetOrderAktif(string nolpb)
        {
            using var db = _context.CreateDbContext();
            return db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == nolpb).FirstOrDefault();
        }

        public void SavePdf(PoTransH transH)
        {
            using var db = _context.CreateDbContext();
            try
            {
                db.PoTransHs.Update(transH);
                db.SaveChanges();

            }
            catch
            {
                throw;
            }

        }

        public List<PoTransH> GetTransH()
        {
            using var db = _context.CreateDbContext();
            List<PoTransH> PoTrans = new List<PoTransH>();


            try
            {
                PoTrans = db.PoTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Kode == "76").ToList();
                //  PoTrans = (from e in db.PoTransHs orderby e.Tanggal where e.Kode == "76" select e).ToList();

                //foreach (var item in PoTrans)
                //{
                //    item.NamaVendor = dbAr.ApSuppls.Where(x => x.Supplier == item.Vendor).FirstOrDefault().NamaLengkap;
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

            PoTrans = db.PoTransHs.OrderByDescending(x => x.Tanggal.Date).Where(x => x.Tanggal.Date > DateTime.Today.Date.AddMonths(-3) && x.Kode == "76").ToList();

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
            //string test = codeview.SrcCode.ToUpper();
            //var cekFirst = db.CbSrcCodes.Where(x => x.SrcCode == test).ToList();
            decimal mQty5 = 0;

            PoTransH transH = new PoTransH
            {
                NoLpb = GetNumber(),
                Vendor = trans.Vendor.ToUpper(),
                NamaVendor = trans.NamaVendor,
                NoPrj = trans.NoPrj,
                Tanggal = trans.Tanggal,
                Keterangan = trans.Keterangan,
                Jumlah = trans.Jumlah,
                Ongkos = trans.Ongkos,
                Discount = trans.Discount,
                Ppn = trans.Ppn,
                PpnPersen = trans.PpnPersen,
                TtlJumlah = trans.TtlJumlah,
                DPayment = trans.DPayment,
                Tagihan = trans.Tagihan,
                TotalQty = trans.TotalQty,
                Kode = "76",
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
                        Kode = "76",
                        NoLpb = transH.NoLpb,
                        Tanggal = trans.Tanggal,
                        JumDpp = mQty5
                    });

                    //IcItem cekItem = dbIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();

                    //if (cekItem != null)
                    //{

                    //    if (item.Harga > 0 && item.Harga > cekItem.HrgUsd)
                    //        cekItem.HrgUsd = item.Harga;  // harga beli barang



                    //    dbIc.IcItems.Update(cekItem);

                    //}
                }
                db.PoTransHs.Add(transH);
            }



            db.SaveChanges();

            //   dbIc.SaveChanges();

            var TempTrans = GetTransDoc(transH.NoLpb);

            return TempTrans;

        }

        public PoTransH GetTransDoc(string docno)
        {
            using var db = _context.CreateDbContext();
            return db.PoTransHs.Include(p => p.PoTransDs).Where(x => x.NoLpb == docno).FirstOrDefault();
        }

        public async Task<bool> CloseOrder(int id)
        {
            using var db = _context.CreateDbContext();
            try
            {
                var ExistingTrans = db.PoTransHs.Where(x => x.PoTransHId == id).FirstOrDefault();
                if(ExistingTrans != null)
                {
                    ExistingTrans.Cek = "3";
                    db.PoTransHs.Update(ExistingTrans);
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
            decimal mQty5 = 0;

            //   var cekFirst = dbAr.ApHutangs.Where(x => x.Dokumen == trans.NoLpb && x.Bayar == 0).FirstOrDefault();

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

                            NamaVendor = trans.NamaVendor,
                            NoPrj = trans.NoPrj,
                            Tanggal = trans.Tanggal,
                            Keterangan = trans.Keterangan,
                            Jumlah = trans.Jumlah,
                            Discount = trans.Discount,
                            Ongkos = trans.Ongkos,
                            Ppn = trans.Ppn,
                            PpnPersen = trans.PpnPersen,
                            TtlJumlah = trans.TtlJumlah,
                            DPayment = trans.DPayment,
                            Tagihan = trans.Tagihan,
                            TotalQty = trans.TotalQty,
                            Kode = "76",
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
                                    Kode = "76",
                                    NoLpb = transH.NoLpb,
                                    Tanggal = trans.Tanggal,
                                    JumDpp = mQty5
                                });


                                //IcItem cekItem = dbIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                                //if (cekItem != null)
                                //{
                                //    //  cekItem.HrgUsd = item.Harga;
                                //    if (item.Harga > 0 && item.Harga > cekItem.HrgUsd)
                                //        cekItem.HrgUsd = item.Harga;  // harga beli barang

                                //    dbIc.IcItems.Update(cekItem);

                                //}
                            }

                        }



                        db.PoTransHs.Add(transH);


                        //  await dbIc.SaveChangesAsync();
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
            string kodeno = "S/O";
            string kodeurut = kodeno + '-';
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = kodeurut + thnbln.Substring(0, 2) + '5' + thnbln.Substring(2, 2) + '-';
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
            return cAngNo;
        }

        public SalesOrderStockMatrixView GetSalesOrderStockMatrix()
        {
            using var db = _context.CreateDbContext();
            using var dbIc = _contextIc.CreateDbContext();
            var result = new SalesOrderStockMatrixView();

            // Ambil semua Sales Order yang masih aktif (Kode=76, Cek=1)
            var orders = db.PoTransHs
                .Include(p => p.PoTransDs)
                .Where(x => x.Kode == "76" && x.Cek == "1")
                .OrderBy(x => x.Tanggal)
                .ToList();

            if (!orders.Any())
                return result;

            // Kumpulkan semua item unik dari semua SO
            var allItemCodes = orders
                .SelectMany(o => o.PoTransDs.Select(d => d.ItemCode))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // Ambil stock saat ini dari IcItem
            var stockItems = dbIc.IcItems
                .Where(x => allItemCodes.Contains(x.ItemCode))
                .ToList();

            // Bangun header kolom (item)
            foreach (var itemCode in allItemCodes)
            {
                var master = stockItems.FirstOrDefault(x => x.ItemCode == itemCode);
                result.ItemHeaders.Add(new SalesOrderItemHeader
                {
                    ItemCode = itemCode,
                    NamaItem = master?.NamaItem ?? itemCode,
                    Satuan = master?.Satuan ?? "",
                    QtyStock = master?.Qty ?? 0
                });
            }

            // Sisa stock per item yang akan dikurangi secara rolling (FIFO urutan tanggal SO)
            var stockSisa = result.ItemHeaders.ToDictionary(h => h.ItemCode, h => h.QtyStock);

            // Bangun baris per SO
            foreach (var order in orders)
            {
                var row = new SalesOrderMatrixRow
                {
                    PoTransHId = order.PoTransHId,
                    NoLpb = order.NoLpb,
                    NamaCustomer = order.NamaVendor,
                    Tanggal = order.Tanggal,
                    Keterangan = order.Keterangan,
                    NoPrj = order.NoPrj
                };

                // Buat cell untuk setiap item, pakai sisa stock rolling
                foreach (var itemHeader in result.ItemHeaders)
                {
                    var detail = order.PoTransDs.FirstOrDefault(d => d.ItemCode == itemHeader.ItemCode);
                    var qtyOrder = detail?.Qty ?? 0;
                    var sisaSebelum = stockSisa[itemHeader.ItemCode];

                    row.Cells.Add(new SalesOrderMatrixCell
                    {
                        ItemCode = itemHeader.ItemCode,
                        QtyOrder = qtyOrder,
                        QtyStockSisa = sisaSebelum
                    });

                    // Kurangi sisa stock untuk SO berikutnya (tidak boleh negatif agar akurat)
                    if (qtyOrder > 0)
                        stockSisa[itemHeader.ItemCode] = sisaSebelum - qtyOrder;
                }

                // SO dianggap siap jika semua item yang dipesan stocknya masih cukup
                var orderedCells = row.Cells.Where(c => c.IsOrdered).ToList();
                row.IsComplete = orderedCells.Any() && orderedCells.All(c => c.HasStock);

                result.Rows.Add(row);
            }

            return result;
        }
    }
}


