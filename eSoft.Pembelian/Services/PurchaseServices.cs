using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Pembelian.Data;
using eSoft.Pembelian.Model;
using eSoft.Pembelian.View;
using eSoft.Hutang.Data;
using eSoft.Hutang.Model;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;

using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace eSoft.Pembelian.Services
{
    public class PurchaseServices : IPurchaseServices
    {
        private readonly IDbContextFactory<DbContextBeli> _context;
        private readonly IDbContextFactory<DbContextHutang> _contextAp;
        private readonly IDbContextFactory<DbContextPersediaan> _contextIc;

        public PurchaseServices(IDbContextFactory<DbContextBeli> context, IDbContextFactory<DbContextHutang> contextHutang, IDbContextFactory<DbContextPersediaan> contextPersediaan)
        {
            _context = context;
            _contextAp = contextHutang;
            _contextIc = contextPersediaan;
        }

        #region getclass

        private ApSuppl GetSupplierId(string id)
        {
            using var dbAp = _contextAp.CreateDbContext();
            return dbAp.ApSuppls.FirstOrDefault(x => x.Supplier == id);
        }

        public ApHutang GetHutang(string bukti)
        {
            using var dbAp = _contextAp.CreateDbContext();
            return dbAp.ApHutangs.FirstOrDefault(x => x.Dokumen == bukti);
        }

        #endregion getclass

        #region IrTransH class

        public IrTransH GetIrTrans(int id)
        {
            using var db = _context.CreateDbContext();
            return db.IrTransHs
                .AsNoTracking()
                .Include(p => p.IrTransDs)
                .FirstOrDefault(x => x.IrTransHId == id);
        }

        public List<IrTransH> GetTransH()
        {
            using var db = _context.CreateDbContext();
            using var dbAp = _contextAp.CreateDbContext();
            List<IrTransH> irTrans = db.IrTransHs
                .AsNoTracking()
                .OrderByDescending(x => x.Tanggal.Date)
                .Where(x => x.Kode == "82" || x.Kode == "83")
                .ToList();

            if (irTrans.Count == 0)
            {
                return irTrans;
            }

            var supplierIds = irTrans.Select(x => x.Supplier).Distinct().ToList();

            var suppliers = dbAp.ApSuppls
                .AsNoTracking()
                .Where(x => supplierIds.Contains(x.Supplier))
                .Select(x => new { x.Supplier, x.NamaLengkap })
                .ToDictionary(x => x.Supplier, x => x.NamaLengkap);

            foreach (var item in irTrans)
            {
                if (suppliers.TryGetValue(item.Supplier, out var namaLengkap))
                {
                    item.NamaSup = namaLengkap;
                }

                if (item.Kode == "83")
                {
                    item.Jumlah = -1 * item.Jumlah;
                    item.TtlJumlah = -1 * item.TtlJumlah;
                    item.Ongkos = -1 * item.Ongkos;
                    item.Ppn = -1 * item.Ppn;
                }
            }

            return irTrans;
        }

        public List<IrTransH> Get3TransH()
        {
            using var db = _context.CreateDbContext();
            return db.IrTransHs
                .AsNoTracking()
                .OrderByDescending(x => x.Tanggal.Date)
                .Where(x =>
                    x.Tanggal.Date > DateTime.Today.Date.AddMonths(-3) &&
                    (x.Kode == "82" || x.Kode == "83"))
                .ToList();
        }

        public List<IrTransD> GetTransD()
        {
            using var db = _context.CreateDbContext();
            return db.IrTransDs
                .AsNoTracking()
                .ToList();
        }

        public IrTransH AddTransH(IrTransHView trans)
        {
            using var db = _context.CreateDbContext();
            using var dbAp = _contextAp.CreateDbContext();
            using var dbIc = _contextIc.CreateDbContext();
            decimal mQty5 = 0;

            IrTransH transH = new IrTransH
            {
                NoLpb = GetNumber(),
                Supplier = trans.Supplier.ToUpper(),
                NamaSup = trans.NamaSup,
                Kurs = trans.Kurs,
                Nilai = trans.Nilai,
                Tanggal = trans.Tanggal,
                NoPrj = trans.NoPrj,
                Keterangan = trans.Keterangan,
                Jumlah = trans.Jumlah,
                Ongkos = trans.Ongkos,
                Ppn = trans.Ppn,
                PpnPersen = trans.PpnPersen,
                TtlJumlah = trans.TtlJumlah,
                DPayment = trans.DPayment,
                Tagihan = trans.Tagihan,
                TotalQty = trans.TotalQty,
                Currency = trans.Currency,
                Kode = "82",
                Cek = "1",
                IrTransDs = new List<IrTransD>()
            };

            var altItemDictAdd = new Dictionary<string, IcAltItem>();

            foreach (var item in trans.IrTransDs)
            {
                IcItem cekItem = dbIc.IcItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);

                if (item.Qty == 0)
                {
                    continue;
                }

                if (transH.TotalQty != 0)
                {
                    if (trans.Kurs != 0)
                    {
                        mQty5 = trans.Kurs * ((item.Jumlah - item.Discount) + (item.Qty / transH.TotalQty * transH.Ppn) + (item.Qty / transH.TotalQty * transH.Ongkos));
                    }
                    else
                    {
                        mQty5 = (item.Jumlah - item.Discount) + (item.Qty / transH.TotalQty * transH.Ppn) + (item.Qty / transH.TotalQty * transH.Ongkos);
                    }
                }
                else
                {
                    if (trans.Kurs != 0)
                    {
                        mQty5 = trans.Kurs * (item.Jumlah - item.Discount);
                    }
                    else
                    {
                        mQty5 = (item.Jumlah - item.Discount);
                    }
                }

                transH.IrTransDs.Add(new IrTransD()
                {
                    ItemCode = item.ItemCode.ToUpper(),
                    NamaItem = item.NamaItem,
                    Satuan = item.Satuan,
                    Lokasi = item.Lokasi,
                    Harga = (cekItem != null && cekItem.CostMethod == (int)costMethod.Moving_Avg ? item.Harga : cekItem?.StdPrice ?? item.Harga),
                    Qty = item.Qty,
                    Persen = item.Persen,
                    Discount = item.Discount,
                    Jumlah = item.Jumlah,
                    Kode = "82",
                    NoLpb = transH.NoLpb,
                    Tanggal = trans.Tanggal,
                    JumDpp = (cekItem != null && cekItem.CostMethod == (int)costMethod.Moving_Avg ? mQty5 : (cekItem?.StdPrice ?? 0) * item.Qty)
                });

                if (cekItem == null)
                {
                    continue;
                }

                var altKey = $"{item.ItemCode}::{item.Lokasi}";
                if (!altItemDictAdd.TryGetValue(altKey, out IcAltItem cekLokasi1))
                {
                    cekLokasi1 = dbIc.IcAltItems.FirstOrDefault(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi);
                    if (cekLokasi1 != null) altItemDictAdd[altKey] = cekLokasi1;
                }

                if (cekLokasi1 == null)
                {
                    cekLokasi1 = new IcAltItem()
                    {
                        ItemCode = cekItem.ItemCode.ToUpper(),
                        NamaItem = cekItem.NamaItem,
                        Satuan = cekItem.Satuan,
                        Lokasi = item.Lokasi,
                        Qty = item.Qty
                    };
                    dbIc.IcAltItems.Add(cekLokasi1);
                    altItemDictAdd[altKey] = cekLokasi1;
                }
                else
                {
                    cekLokasi1.Qty += item.Qty;
                    dbIc.IcAltItems.Update(cekLokasi1);
                }

                if (trans.Kurs != 0)
                {
                    cekItem.Harga = trans.Kurs * item.Harga;
                }
                else
                {
                    cekItem.Harga = item.Harga;
                }

                if (cekItem.JnsBrng == (int)jnsBrng.Stock)
                {
                    cekItem.Qty += item.Qty;
                }

                if (cekItem.CostMethod == (int)costMethod.Moving_Avg)
                {
                    cekItem.Cost += mQty5;
                }
                else
                {
                    cekItem.Cost += item.Qty * cekItem.StdPrice;
                }

                if (cekItem.Qty != 0)
                {
                    cekItem.HrgNetto = cekItem.Cost / cekItem.Qty;
                }
                else
                {
                    cekItem.HrgNetto = cekItem.Harga;
                }

                dbIc.IcItems.Update(cekItem);
            }

            db.IrTransHs.Add(transH);

            ApHutang hutang = new ApHutang
            {
                Kode = "IR",
                Dokumen = transH.NoLpb,
                Tanggal = transH.Tanggal,
                DueDate = transH.Tanggal,
                Supplier = transH.Supplier,
                Keterangan = transH.Keterangan,
                Jumlah = transH.Jumlah,
                Sisa = transH.Jumlah,
                SldSisa = transH.Jumlah,
                Kurs = transH.Kurs,
                Currency = trans.Currency,
                Nilai = transH.Nilai,
                KodeTran = transH.Kode
            };
            dbAp.ApHutangs.Add(hutang);

            var supplier = GetSupplierId(transH.Supplier);
            if (supplier != null)
            {
                supplier.Hutang += transH.Jumlah;
                dbAp.ApSuppls.Update(supplier);
            }

            db.SaveChanges();
            dbAp.SaveChanges();
            dbIc.SaveChanges();

            return GetTransDoc(transH.NoLpb);
        }

        public IrTransH GetTransDoc(string docno)
        {
            using var db = _context.CreateDbContext();
            return db.IrTransHs
                .AsNoTracking()
                .Include(p => p.IrTransDs)
                .FirstOrDefault(x => x.NoLpb == docno);
        }

        public async Task<bool> DelTransH(int id)
        {
            using var db = _context.CreateDbContext();
            using var dbAp = _contextAp.CreateDbContext();
            using var dbIc = _contextIc.CreateDbContext();
            string cKode = "82";

            try
            {
                var existingTrans = db.IrTransHs
                    .Include(x => x.IrTransDs)
                    .FirstOrDefault(x => x.IrTransHId == id);

                if (existingTrans == null)
                {
                    return false;
                }

                if (dbAp.ApHutangs.Any(x => x.Dokumen == existingTrans.NoLpb && x.Bayar > 0))
                {
                    return false;
                }

                cKode = existingTrans.Kode;

                var altItemDictDel = new Dictionary<string, IcAltItem>();

                foreach (var item in existingTrans.IrTransDs)
                {
                    if (item.Qty == 0)
                    {
                        continue;
                    }

                    IcItem cekItem = dbIc.IcItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);
                    if (cekItem == null)
                    {
                        continue;
                    }

                    var altKeyDel = $"{item.ItemCode}::{item.Lokasi}";
                    if (!altItemDictDel.TryGetValue(altKeyDel, out IcAltItem cekLokasi1))
                    {
                        cekLokasi1 = dbIc.IcAltItems.FirstOrDefault(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi);
                        if (cekLokasi1 != null) altItemDictDel[altKeyDel] = cekLokasi1;
                    }

                    if (cekLokasi1 == null)
                    {
                        IcAltItem produk = new IcAltItem()
                        {
                            ItemCode = cekItem.ItemCode.ToUpper(),
                            NamaItem = cekItem.NamaItem,
                            Satuan = cekItem.Satuan,
                            Lokasi = item.Lokasi,
                            Qty = (cKode == "82" ? -1 * item.Qty : item.Qty)
                        };
                        dbIc.IcAltItems.Add(produk);
                        altItemDictDel[altKeyDel] = produk;
                    }
                    else
                    {
                        if (cKode == "82")
                        {
                            cekLokasi1.Qty -= item.Qty;
                        }
                        else
                        {
                            cekLokasi1.Qty += item.Qty;
                        }

                        dbIc.IcAltItems.Update(cekLokasi1);
                    }

                    if (cekItem.JnsBrng == (int)jnsBrng.Stock)
                    {
                        if (cKode == "82")
                        {
                            cekItem.Qty -= item.Qty;
                        }
                        else
                        {
                            cekItem.Qty += item.Qty;
                        }
                    }

                    if (cekItem.CostMethod == (int)costMethod.Moving_Avg)
                    {
                        if (cKode == "82")
                        {
                            cekItem.Cost -= item.JumDpp;
                        }
                        else
                        {
                            cekItem.Cost += item.JumDpp;
                        }
                    }
                    else
                    {
                        if (cKode == "82")
                        {
                            cekItem.Cost -= item.Qty * cekItem.StdPrice;
                        }
                        else
                        {
                            cekItem.Cost += item.Qty * cekItem.StdPrice;
                        }
                    }

                    if (cekItem.Qty != 0)
                    {
                        cekItem.HrgNetto = cekItem.Cost / cekItem.Qty;
                    }
                    else
                    {
                        cekItem.HrgNetto = cekItem.Harga;
                    }

                    dbIc.IcItems.Update(cekItem);
                }

                var supplier = GetSupplierId(existingTrans.Supplier);
                var hutang = GetHutang(existingTrans.NoLpb);

                if (supplier != null)
                {
                    if (cKode == "82")
                    {
                        supplier.Hutang -= existingTrans.Jumlah;
                    }
                    else
                    {
                        supplier.Hutang += existingTrans.Jumlah;
                    }

                    dbAp.ApSuppls.Update(supplier);
                }

                if (hutang != null)
                {
                    dbAp.ApHutangs.Remove(hutang);
                }

                db.IrTransHs.Remove(existingTrans);

                await db.SaveChangesAsync();
                await dbAp.SaveChangesAsync();
                await dbIc.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CekHutang(IrTransH trans)
        {
            using var dbAp = _contextAp.CreateDbContext();
            return dbAp.ApHutangs
                .AsNoTracking()
                .Any(x => x.Dokumen == trans.NoLpb && x.Bayar == 0);
        }

        public async Task<bool> EditTransH(IrTransHView trans)
        {
            using var db = _context.CreateDbContext();
            using var dbAp = _contextAp.CreateDbContext();
            using var dbIc = _contextIc.CreateDbContext();
            decimal mQty5 = 0;
            string cKode = trans.Kode;

            var cekFirst = dbAp.ApHutangs.FirstOrDefault(x => x.Dokumen == trans.NoLpb && x.Bayar == 0);
            if (cekFirst == null)
            {
                return false;
            }

            try
            {
                var existingTrans = db.IrTransHs
                    .Include(x => x.IrTransDs)
                    .FirstOrDefault(x => x.IrTransHId == trans.IrTransHId);

                if (existingTrans == null)
                {
                    return false;
                }

                cKode = existingTrans.Kode;

                var altItemDictEdit1 = new Dictionary<string, IcAltItem>();

                foreach (var item in existingTrans.IrTransDs)
                {
                    if (item.Qty == 0)
                    {
                        continue;
                    }

                    IcItem cekItem = dbIc.IcItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);
                    if (cekItem == null)
                    {
                        continue;
                    }

                    var altKeyE1 = $"{item.ItemCode}::{item.Lokasi}";
                    if (!altItemDictEdit1.TryGetValue(altKeyE1, out IcAltItem cekLokasi1))
                    {
                        cekLokasi1 = dbIc.IcAltItems.FirstOrDefault(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi);
                        if (cekLokasi1 != null) altItemDictEdit1[altKeyE1] = cekLokasi1;
                    }

                    if (cekLokasi1 == null)
                    {
                        IcAltItem produk = new IcAltItem()
                        {
                            ItemCode = cekItem.ItemCode.ToUpper(),
                            NamaItem = cekItem.NamaItem,
                            Satuan = cekItem.Satuan,
                            Lokasi = item.Lokasi,
                            Qty = (existingTrans.Kode == "82" ? -1 * item.Qty : item.Qty)
                        };
                        dbIc.IcAltItems.Add(produk);
                        altItemDictEdit1[altKeyE1] = produk;
                    }
                    else
                    {
                        if (existingTrans.Kode == "82")
                        {
                            cekLokasi1.Qty -= item.Qty;
                        }
                        else
                        {
                            cekLokasi1.Qty += item.Qty;
                        }

                        dbIc.IcAltItems.Update(cekLokasi1);
                    }

                    if (cekItem.JnsBrng == (int)jnsBrng.Stock)
                    {
                        if (existingTrans.Kode == "82")
                        {
                            cekItem.Qty -= item.Qty;
                        }
                        else
                        {
                            cekItem.Qty += item.Qty;
                        }
                    }

                    if (cekItem.CostMethod == (int)costMethod.Moving_Avg)
                    {
                        if (existingTrans.Kode == "82")
                        {
                            cekItem.Cost -= item.JumDpp;
                        }
                        else
                        {
                            cekItem.Cost += item.JumDpp;
                        }
                    }
                    else
                    {
                        if (existingTrans.Kode == "82")
                        {
                            cekItem.Cost -= item.Qty * cekItem.StdPrice;
                        }
                        else
                        {
                            cekItem.Cost += item.Qty * cekItem.StdPrice;
                        }
                    }

                    if (cekItem.Qty != 0)
                    {
                        cekItem.HrgNetto = cekItem.Cost / cekItem.Qty;
                    }
                    else
                    {
                        cekItem.HrgNetto = cekItem.Harga;
                    }

                    dbIc.IcItems.Update(cekItem);
                }

                var existingSupplier = GetSupplierId(existingTrans.Supplier);
                if (existingSupplier != null)
                {
                    if (existingTrans.Kode == "82")
                    {
                        existingSupplier.Hutang -= existingTrans.Jumlah;
                    }
                    else
                    {
                        existingSupplier.Hutang += existingTrans.Jumlah;
                    }

                    dbAp.ApSuppls.Update(existingSupplier);
                }

                dbAp.ApHutangs.Remove(cekFirst);
                db.IrTransHs.Remove(existingTrans);

                IrTransH transH = new IrTransH
                {
                    NoLpb = trans.NoLpb,
                    Supplier = trans.Supplier.ToUpper(),
                    NamaSup = trans.NamaSup,
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
                    Kode = cKode,
                    Cek = "1",

                    IrTransDs = new List<IrTransD>()
                };

                var altItemDictEdit2 = new Dictionary<string, IcAltItem>();

                foreach (var item in trans.IrTransDs)
                {
                    IcItem cekItem = dbIc.IcItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);

                    if (item.Qty == 0)
                    {
                        continue;
                    }

                    if (transH.TotalQty != 0)
                    {
                        mQty5 = (item.Jumlah - item.Discount) - (item.Qty / transH.TotalQty * transH.Ppn) + (item.Qty / transH.TotalQty * transH.Ongkos);
                    }
                    else
                    {
                        mQty5 = (item.Jumlah - item.Discount);
                    }

                    transH.IrTransDs.Add(new IrTransD()
                    {
                        ItemCode = item.ItemCode.ToUpper(),
                        NamaItem = item.NamaItem,
                        Satuan = item.Satuan,
                        Lokasi = item.Lokasi,
                        Harga = (cekItem != null && cekItem.CostMethod == (int)costMethod.Moving_Avg ? item.Harga : cekItem?.StdPrice ?? item.Harga),
                        Qty = item.Qty,
                        Persen = item.Persen,
                        Discount = item.Discount,
                        Jumlah = item.Jumlah,
                        Kode = cKode,
                        NoLpb = transH.NoLpb,
                        Tanggal = trans.Tanggal,
                        JumDpp = (cekItem != null && cekItem.CostMethod == (int)costMethod.Moving_Avg ? mQty5 : (cekItem?.StdPrice ?? 0) * item.Qty)
                    });

                    if (cekItem == null)
                    {
                        continue;
                    }

                    var altKeyE2 = $"{item.ItemCode}::{item.Lokasi}";
                    if (!altItemDictEdit2.TryGetValue(altKeyE2, out IcAltItem cekLokasi1))
                    {
                        cekLokasi1 = dbIc.IcAltItems.FirstOrDefault(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi);
                        if (cekLokasi1 != null) altItemDictEdit2[altKeyE2] = cekLokasi1;
                    }

                    if (cekLokasi1 == null)
                    {
                        cekLokasi1 = new IcAltItem()
                        {
                            ItemCode = cekItem.ItemCode.ToUpper(),
                            NamaItem = cekItem.NamaItem,
                            Satuan = cekItem.Satuan,
                            Lokasi = item.Lokasi,
                            Qty = (cKode == "82" ? item.Qty : -1 * item.Qty)
                        };
                        dbIc.IcAltItems.Add(cekLokasi1);
                        altItemDictEdit2[altKeyE2] = cekLokasi1;
                    }
                    else
                    {
                        if (cKode == "82")
                        {
                            cekLokasi1.Qty += item.Qty;
                        }
                        else
                        {
                            cekLokasi1.Qty -= item.Qty;
                        }

                        dbIc.IcAltItems.Update(cekLokasi1);
                    }

                    if (cekItem.JnsBrng == (int)jnsBrng.Stock)
                    {
                        if (cKode == "82")
                        {
                            cekItem.Qty += item.Qty;
                        }
                        else
                        {
                            cekItem.Qty -= item.Qty;
                        }
                    }

                    if (cekItem.CostMethod == (int)costMethod.Moving_Avg)
                    {
                        if (cKode == "82")
                        {
                            cekItem.Cost += mQty5;
                        }
                        else
                        {
                            cekItem.Cost -= mQty5;
                        }
                    }
                    else
                    {
                        if (existingTrans.Kode == "82")
                        {
                            cekItem.Cost -= item.Qty * cekItem.StdPrice;
                        }
                        else
                        {
                            cekItem.Cost += item.Qty * cekItem.StdPrice;
                        }
                    }

                    if (cekItem.Qty != 0)
                    {
                        cekItem.HrgNetto = cekItem.Cost / cekItem.Qty;
                    }
                    else
                    {
                        cekItem.HrgNetto = cekItem.Harga;
                    }

                    dbIc.IcItems.Update(cekItem);
                }

                var supplier = GetSupplierId(transH.Supplier);

                ApHutang hutang = new ApHutang
                {
                    Kode = "IR",
                    Dokumen = transH.NoLpb,
                    Tanggal = transH.Tanggal,
                    DueDate = supplier != null ? transH.Tanggal.AddDays(supplier.Termin) : transH.Tanggal,
                    Supplier = transH.Supplier,
                    Keterangan = transH.Keterangan,
                    Jumlah = (cKode == "82" ? transH.Jumlah : -1 * transH.Jumlah),
                    Sisa = (cKode == "82" ? transH.Jumlah : -1 * transH.Jumlah),
                    SldSisa = (cKode == "82" ? transH.Jumlah : -1 * transH.Jumlah),
                    KodeTran = transH.Kode
                };

                if (supplier != null)
                {
                    if (cKode == "82")
                    {
                        supplier.Hutang += transH.Jumlah;
                    }
                    else
                    {
                        supplier.Hutang -= transH.Jumlah;
                    }

                    dbAp.ApSuppls.Update(supplier);
                }

                db.IrTransHs.Add(transH);
                dbAp.ApHutangs.Add(hutang);

                await db.SaveChangesAsync();
                await dbAp.SaveChangesAsync();
                await dbIc.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion IrTransH Class

        public string GetNumber()
        {
            using var db = _context.CreateDbContext();
            const string kodeno = "BPB";
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = $"{kodeno}-{thnbln.Substring(0, 2)}2{thnbln.Substring(2, 2)}-";

            string maxvalue = db.IrTransHs
                .AsNoTracking()
                .Where(x => x.NoLpb != null && x.NoLpb.StartsWith(xbukti))
                .Select(x => x.NoLpb)
                .OrderByDescending(x => x)
                .FirstOrDefault();

            int lastNumber = 0;
            if (!string.IsNullOrWhiteSpace(maxvalue) && maxvalue.Length >= xbukti.Length + 5)
            {
                int.TryParse(maxvalue.Substring(xbukti.Length, 5), out lastNumber);
            }

            return xbukti + (lastNumber + 1).ToString("00000");
        }

        #region retur Pembelian

        public IrTransH AddTransHRetur(IrTransHView trans)
        {
            using var db = _context.CreateDbContext();
            using var dbAp = _contextAp.CreateDbContext();
            using var dbIc = _contextIc.CreateDbContext();
            decimal mQty5 = 0;

            IrTransH transH = new IrTransH
            {
                NoLpb = GetNumberRetur(),
                Supplier = trans.Supplier.ToUpper(),
                NamaSup = trans.NamaSup,
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
                Kode = "83",
                Cek = "1",
                IrTransDs = new List<IrTransD>()
            };

            foreach (var item in trans.IrTransDs)
            {
                if (item.Qty == 0)
                {
                    continue;
                }

                if (transH.TotalQty != 0)
                {
                    mQty5 = (item.Jumlah - item.Discount) - (item.Qty / transH.TotalQty * transH.Ppn) + (item.Qty / transH.TotalQty * transH.Ongkos);
                }
                else
                {
                    mQty5 = (item.Jumlah - item.Discount);
                }

                transH.IrTransDs.Add(new IrTransD()
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
                    Kode = "83",
                    NoLpb = transH.NoLpb,
                    Tanggal = trans.Tanggal,
                    JumDpp = mQty5
                });

                IcItem cekItem = dbIc.IcItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);
                if (cekItem == null)
                {
                    continue;
                }

                IcAltItem cekLokasi1 = dbIc.IcAltItems.FirstOrDefault(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi);
                if (cekLokasi1 == null)
                {
                    IcAltItem produk = new IcAltItem()
                    {
                        ItemCode = cekItem.ItemCode.ToUpper(),
                        NamaItem = cekItem.NamaItem,
                        Satuan = cekItem.Satuan,
                        Lokasi = item.Lokasi,
                        Qty = -1 * item.Qty
                    };
                    dbIc.IcAltItems.Add(produk);
                }
                else
                {
                    cekLokasi1.Qty -= item.Qty;
                    dbIc.IcAltItems.Update(cekLokasi1);
                }

                cekItem.Harga = item.Harga;

                if (cekItem.JnsBrng == (int)jnsBrng.Stock)
                {
                    cekItem.Qty -= item.Qty;
                }

                if (cekItem.CostMethod == (int)costMethod.Moving_Avg)
                {
                    cekItem.Cost -= mQty5;
                }

                if (cekItem.Qty != 0)
                {
                    cekItem.HrgNetto = cekItem.Cost / cekItem.Qty;
                }
                else
                {
                    cekItem.HrgNetto = cekItem.Harga;
                }

                dbIc.IcItems.Update(cekItem);
            }

            db.IrTransHs.Add(transH);

            ApHutang hutang = new ApHutang
            {
                Kode = "IR",
                Dokumen = transH.NoLpb,
                Tanggal = transH.Tanggal,
                DueDate = transH.Tanggal,
                Supplier = transH.Supplier,
                Keterangan = transH.Keterangan,
                Jumlah = -1 * transH.Jumlah,
                Sisa = -1 * transH.Jumlah,
                SldSisa = -1 * transH.Jumlah,
                KodeTran = transH.Kode
            };
            dbAp.ApHutangs.Add(hutang);

            var supplier = GetSupplierId(transH.Supplier);
            if (supplier != null)
            {
                supplier.Hutang -= transH.Jumlah;
                dbAp.ApSuppls.Update(supplier);
            }

            db.SaveChanges();
            dbAp.SaveChanges();
            dbIc.SaveChanges();

            return GetTransDoc(transH.NoLpb);
        }

        public string GetNumberRetur()
        {
            using var db = _context.CreateDbContext();
            const string kodeno = "R/B";
            string thnbln = DateTime.Now.ToString("yyMM");
            string xbukti = $"{kodeno}-{thnbln.Substring(0, 2)}2{thnbln.Substring(2, 2)}-";

            string maxvalue = db.IrTransHs
                .AsNoTracking()
                .Where(x => x.NoLpb != null && x.NoLpb.StartsWith(xbukti))
                .Select(x => x.NoLpb)
                .OrderByDescending(x => x)
                .FirstOrDefault();

            int lastNumber = 0;
            if (!string.IsNullOrWhiteSpace(maxvalue) && maxvalue.Length >= xbukti.Length + 5)
            {
                int.TryParse(maxvalue.Substring(xbukti.Length, 5), out lastNumber);
            }

            return xbukti + (lastNumber + 1).ToString("00000");
        }

        #endregion

        #region laporan

        public List<IrTransH> Laporan1(DateTime tgl1, DateTime tgl2)
        {
            using var db = _context.CreateDbContext();
            var transH = db.IrTransHs
                .AsNoTracking()
                .Where(x => x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date)
                .OrderByDescending(t => t.Tanggal.Date)
                .Select(
                    x => new IrTransH
                    {
                        IrTransHId = x.IrTransHId,
                        Kode = x.Kode,
                        Tanggal = x.Tanggal,
                        NoLpb = x.NoLpb,
                        NamaSup = x.NamaSup,
                        Supplier = x.Supplier,
                        Keterangan = x.Keterangan,
                        Kurs = x.Kurs,
                        Nilai = x.Jumlah,
                        TtlJumlah = (x.Kode == "82" ? x.TtlJumlah * (x.Kurs != 0 ? x.Kurs : 1) : -1 * x.TtlJumlah * (x.Kurs != 0 ? x.Kurs : 1)),
                        Ppn = (x.Kode == "82" ? x.Ppn * (x.Kurs != 0 ? x.Kurs : 1) : -1 * x.Ppn * (x.Kurs != 0 ? x.Kurs : 1)),
                        Ongkos = (x.Kode == "82" ? x.Ongkos * (x.Kurs != 0 ? x.Kurs : 1) : -1 * x.Ongkos * (x.Kurs != 0 ? x.Kurs : 1)),
                        Jumlah = (x.Kode == "82" ? x.Jumlah * (x.Kurs != 0 ? x.Kurs : 1) : -1 * x.Jumlah * (x.Kurs != 0 ? x.Kurs : 1))
                    }
                )
                .ToList();

            return transH;
        }

        public List<IrTransD> Detail1(int xKdHeader)
        {
            using var db = _context.CreateDbContext();
            return db.IrTransDs
                .AsNoTracking()
                .Where(x => x.IrTransHId == xKdHeader)
                .ToList();
        }

        public List<IrTrans> Detail2(string xKdHeader, DateTime tgl1, DateTime tgl2)
        {
            using var db = _context.CreateDbContext();
            List<IrTransH> transH = db.IrTransHs
                .AsNoTracking()
                .Where(x => x.NoPrj == xKdHeader && (x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date))
                .ToList();

            List<IrTransD> transD = db.IrTransDs
                .AsNoTracking()
                .Where(x => (x.Tanggal.Date >= tgl1.Date && x.Tanggal.Date <= tgl2.Date))
                .ToList();

            if (transH.Count == 0 || transD.Count == 0)
            {
                return new List<IrTrans>();
            }

            return (from header in transH
                    join detail in transD on header.IrTransHId equals detail.IrTransHId
                    select new IrTrans()
                    {
                        ItemCode = detail.ItemCode,
                        NamaItem = detail.NamaItem,
                        Harga = detail.Harga,
                        Persen = detail.Persen,
                        Discount = detail.Discount,
                        Satuan = detail.Satuan,
                        Ppn = header.Ppn,
                        PpnPersen = header.PpnPersen,
                        Ongkos = header.Ongkos,
                        Qty = detail.Qty,
                        Jumlah = detail.Jumlah,
                        Lokasi = detail.Lokasi,
                        NoLpb = header.NoLpb,
                        Supplier = header.Supplier,
                        NamaSuppl = header.NamaSup,
                        Tanggal = header.Tanggal,
                        NoPrj = header.NoPrj
                    }).ToList();
        }

        #endregion

        #region reproses_pembelian

        public void ReprosesPurchase()
        {
            using var db = _context.CreateDbContext();
            var transH = db.IrTransHs
                .Include(p => p.IrTransDs)
                .Where(x => x.NoPrj != null)
                .ToList();

            var transD = db.IrTransDs.AsQueryable();

            foreach (var item in transH)
            {
                var detail = transD.Where(x => x.IrTransHId == item.IrTransHId).ToList().Sum(y => y.Jumlah);
                item.TtlJumlah = detail;
                item.Jumlah = item.Tagihan;
                if (item.TtlJumlah != item.Jumlah)
                {
                    item.Ongkos = (item.Jumlah - item.TtlJumlah);
                }
            }

            db.IrTransHs.UpdateRange(transH);
            db.SaveChanges();
        }

        #endregion
    }
}
