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
        private const string OrderPurchaseKode = "71";
        private const string StatusOpen = "1";
        private const string StatusClosed = "3";

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

        private ApSuppl GetVendorId(string id) =>
            _contextAp.ApSuppls.FirstOrDefault(x => x.Supplier == id);

        public ApHutang GetHutang(string bukti) =>
            _contextAp.ApHutangs.FirstOrDefault(x => x.Dokumen == bukti);

        #endregion getclass

        #region PoTransH class

        public PoTransH GetPoTrans(int id) =>
            _context.PoTransHs
                    .Include(p => p.PoTransDs)
                    .FirstOrDefault(x => x.PoTransHId == id);

        public List<PoTransH> GetTransHAktif() =>
            _context.PoTransHs
                    .Where(x => x.Kode == OrderPurchaseKode && x.Cek == StatusOpen)
                    .OrderByDescending(x => x.Tanggal.Date)
                    .ToList();

        public void SaveOrderAktif(string customer)
        {
            var order = _context.PoTransHs
                                .Include(p => p.PoTransDs)
                                .FirstOrDefault(x => x.NoLpb == customer);

            if (order != null)
            {
                order.Cek = StatusClosed;
                _context.SaveChanges();
            }
        }

        public void DelOrderAktif(string customer)
        {
            var order = _context.PoTransHs
                                .Include(p => p.PoTransDs)
                                .FirstOrDefault(x => x.NoLpb == customer);

            if (order != null)
            {
                order.Cek = StatusOpen;
                _context.SaveChanges();
            }
        }

        public PoTransH GetOrderAktif(string customer) =>
            _context.PoTransHs
                    .Include(p => p.PoTransDs)
                    .FirstOrDefault(x => x.NoLpb == customer);

        public List<PoTransH> GetTransH() =>
            _context.PoTransHs
                    .Where(x => x.Kode == OrderPurchaseKode)
                    .OrderByDescending(x => x.Tanggal.Date)
                    .ToList();

        public List<PoTransH> Get3TransH()
        {
            var threeMonthsAgo = DateTime.Today.AddMonths(-3);

            return _context.PoTransHs
                           .Where(x => x.Kode == OrderPurchaseKode && x.Tanggal.Date > threeMonthsAgo)
                           .OrderByDescending(x => x.Tanggal.Date)
                           .ToList();
        }

        public List<PoTransD> GetTransD() => _context.PoTransDs.ToList();

        public PoTransH AddTransH(PoTransHView trans)
        {
            var transH = new PoTransH
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
                Kode = OrderPurchaseKode,
                Cek = StatusOpen,
                PoTransDs = new List<PoTransD>()
            };

            foreach (var item in trans.PoTransDs)
            {
                if (item.Qty == 0)
                {
                    continue;
                }

                transH.PoTransDs.Add(MapToPoTransD(item, transH, trans.Tanggal));
                UpdateItemPurchasePrice(item, currencyCode: trans.Currency);
            }

            _context.PoTransHs.Add(transH);

            _context.SaveChanges();
            _contextIc.SaveChanges();

            return GetTransDoc(transH.NoLpb);
        }

        public PoTransH GetTransDoc(string docno) =>
            _context.PoTransHs
                    .Include(p => p.PoTransDs)
                    .FirstOrDefault(x => x.NoLpb == docno);

        public async Task<bool> DelTransH(int id)
        {
            var existingTrans = await _context.PoTransHs.FirstOrDefaultAsync(x => x.PoTransHId == id);

            if (existingTrans == null)
            {
                return false;
            }

            _context.PoTransHs.Remove(existingTrans);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditTransH(PoTransHView trans)
        {
            var existingTrans = await _context.PoTransHs
                                               .FirstOrDefaultAsync(x => x.PoTransHId == trans.PoTransHId);

            if (existingTrans == null)
            {
                return false;
            }

            _context.PoTransHs.Remove(existingTrans);

            var transH = new PoTransH
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
                Kode = OrderPurchaseKode,
                Cek = StatusOpen,
                PoTransDs = new List<PoTransD>()
            };

            foreach (var item in trans.PoTransDs)
            {
                if (item.Qty == 0)
                {
                    continue;
                }

                transH.PoTransDs.Add(MapToPoTransD(item, transH, trans.Tanggal));
                UpdateItemPurchasePrice(item, trans.Currency);
            }

            _context.PoTransHs.Add(transH);

            await _contextIc.SaveChangesAsync();
            await _context.SaveChangesAsync();

            return true;
        }

        #endregion PoTransH Class

        public async Task<bool> UpdateKeterangan(int id, string keterangan)
        {
            var trans = await _context.PoTransHs.FirstOrDefaultAsync(x => x.PoTransHId == id);
            if (trans == null)
            {
                return false;
            }

            trans.Keterangan = keterangan;
            _context.PoTransHs.Update(trans);
            await _context.SaveChangesAsync();
            return true;
        }

        public string GetNumber()
        {
            const string prefix = "P/I";
          //  const string separator = "-";
            const string defaultSequence = "00000";

            var thnbln = DateTime.Now.ToString("yyMM");
            var xbukti = $"{prefix}-{thnbln.Substring(0, 2)}2{thnbln.Substring(2, 2)}-";

            var maxNoLpb = _context.PoTransHs
                                   .Where(x => x.NoLpb.Substring(0, 10) == xbukti)
                                   .Select(x => x.NoLpb)
                                   .Max();

            var nourut = maxNoLpb == null
                ? defaultSequence
                : maxNoLpb.Substring(10, 5);

            return $"{xbukti}{(int.Parse(nourut) + 1).ToString("00000")}";
        }

        #region Helpers

        private static decimal CalculateJumDpp(PoTransDView item, PoTransH header)
        {
            var netLine = item.Jumlah - item.Discount;

            if (header.TotalQty == 0)
            {
                return netLine;
            }

            var sharePpn = item.Qty / header.TotalQty * header.Ppn;
            var shareOngkir = item.Qty / header.TotalQty * header.Ongkos;

            return netLine - sharePpn + shareOngkir;
        }

        private static PoTransD MapToPoTransD(PoTransDView item, PoTransH header, DateTime tanggal)
        {
            return new PoTransD
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
                Kode = OrderPurchaseKode,
                NoLpb = header.NoLpb,
                Tanggal = tanggal,
                JumDpp = CalculateJumDpp(item, header)
            };
        }

        private void UpdateItemPurchasePrice(PoTransDView item, string currencyCode = null)
        {
            var cekItem = _contextIc.IcItems.FirstOrDefault(x => x.ItemCode == item.ItemCode);
            if (cekItem == null || item.Harga <= 0)
            {
                return;
            }

            cekItem.HrgUsd = item.Harga;
            cekItem.CurrencyCode = currencyCode ?? item.Currency;
            _contextIc.IcItems.Update(cekItem);
        }

        #endregion

        #region POCurrency

        public List<PoItemQtyByLocationView> GetAllIcItemQtyByLocation(string kodeVendor)
        {
            var altItems = _context.PoTransDs
                                   .Where(x => x.Kode == OrderPurchaseKode)
                                   .ToList();

            var locations = _context.PoTransHs
                                    .Where(x => x.Vendor == kodeVendor && x.Kode == OrderPurchaseKode)
                                    .Select(loc => new PoLocationQtyView
                                    {
                                        Lokasi = loc.NoLpb,
                                        NamaLokasi = string.IsNullOrEmpty(loc.NoPrj) ? loc.NoLpb : loc.NoPrj
                                    })
                                    .ToList();

            var result = _contextIc.IcItems
                .Select(item => new PoItemQtyByLocationView
                {
                    ItemCode = item.ItemCode,
                    NamaItem = item.NamaItem,
                    Satuan = item.Satuan,
                    Qty = item.Harga,
                    Locations = locations.ToList()
                })
                .ToList();

            foreach (var alt in altItems.Where(x => x.Harga > 0))
            {
                var itemQty = result.FirstOrDefault(q => q.ItemCode == alt.ItemCode);
                if (itemQty == null)
                {
                    continue;
                }

                var matchedLocation = itemQty.Locations.FirstOrDefault(loc => loc.Lokasi == alt.NoLpb);
                if (matchedLocation != null)
                {
                    matchedLocation.Qty = alt.Harga;
                    itemQty.QtyAwal++;
                }
            }

            return result.Where(x => x.QtyAwal != 0).ToList();
        }
        #endregion
    }
}
