using System.Collections.Generic;
using System.Linq;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Penjualan.Services
{
    public class SalesInventoryAdjustmentService : ISalesInventoryAdjustmentService
    {
        private readonly IDbContextFactory<DbContextPersediaan> _contextIc;

        public SalesInventoryAdjustmentService(IDbContextFactory<DbContextPersediaan> contextIc)
        {
            _contextIc = contextIc;
        }

        public void ApplySaleDetail(OeTransDView item)
        {
            using var context = CreateInventoryContext();
            ApplySaleDetail(context, item, null, null);
        }

        public void ApplyDetailsForCode(IEnumerable<OeTransDView> items, string kode, DbContextPersediaan context)
        {
            var validItems = items.Where(x => x.Qty != 0).ToList();
            if (validItems.Count == 0)
            {
                return;
            }

            var itemMap = LoadItemMap(context, validItems.Select(x => x.ItemCode));
            var altItemMap = LoadAltItemMap(context, validItems.Select(x => x.ItemCode));

            foreach (var item in validItems)
            {
                if (kode == "95")
                {
                    ApplyReturnDetail(context, item, itemMap, altItemMap);
                }
                else
                {
                    ApplySaleDetail(context, item, itemMap, altItemMap);
                }
            }
        }

        public void ApplyReturnDetail(OeTransDView item)
        {
            using var context = CreateInventoryContext();
            ApplyReturnDetail(context, item, null, null);
        }

        public void ReverseExistingDetail(OeTransD item, string kode)
        {
            using var context = CreateInventoryContext();
            ReverseExistingDetail(context, item, kode, null, null);
        }

        public void ReverseDetails(IEnumerable<OeTransD> items, string kode, DbContextPersediaan context)
        {
            var validItems = items.Where(x => x.Qty != 0).ToList();
            if (validItems.Count == 0)
            {
                return;
            }

            var itemMap = LoadItemMap(context, validItems.Select(x => x.ItemCode));
            var altItemMap = LoadAltItemMap(context, validItems.Select(x => x.ItemCode));

            foreach (var item in validItems)
            {
                ReverseExistingDetail(context, item, kode, itemMap, altItemMap);
            }
        }

        private void ApplySaleDetail(DbContextPersediaan context, OeTransDView item, Dictionary<string, IcItem> itemMap, Dictionary<string, IcAltItem> altItemMap)
        {
            var cekItem = GetItem(context, item.ItemCode, itemMap);
            if (cekItem == null)
            {
                return;
            }

            var cekLokasi1 = GetAltItem(context, item.ItemCode, item.Lokasi, altItemMap);
            if (cekLokasi1 == null)
            {
                cekLokasi1 = CreateAltItem(cekItem, item.Lokasi, -1 * item.Qty);
                context.IcAltItems.Add(cekLokasi1);
                SetAltItem(item.ItemCode, item.Lokasi, cekLokasi1, altItemMap);
            }
            else
            {
                cekLokasi1.Qty -= item.Qty;
                context.IcAltItems.Update(cekLokasi1);
            }

            if (item.Harga > 0 && item.Harga > cekItem.HrgJual)
            {
                cekItem.HrgJual = item.Harga;
            }

            if (cekItem.JnsBrng == (int)jnsBrng.Stock)
            {
                cekItem.Qty -= item.Qty;
            }

            if (cekItem.CostMethod == (int)costMethod.Moving_Avg)
            {
                cekItem.Cost -= item.HrgCost * item.Qty;
            }
            else
            {
                cekItem.Cost -= cekItem.StdPrice * item.Qty;
            }

            context.IcItems.Update(cekItem);
        }

        private void ApplyReturnDetail(DbContextPersediaan context, OeTransDView item, Dictionary<string, IcItem> itemMap, Dictionary<string, IcAltItem> altItemMap)
        {
            var cekItem = GetItem(context, item.ItemCode, itemMap);
            if (cekItem == null)
            {
                return;
            }

            var cekLokasi1 = GetAltItem(context, item.ItemCode, item.Lokasi, altItemMap);
            if (cekLokasi1 == null)
            {
                cekLokasi1 = CreateAltItem(cekItem, item.Lokasi, item.Qty);
                context.IcAltItems.Add(cekLokasi1);
                SetAltItem(item.ItemCode, item.Lokasi, cekLokasi1, altItemMap);
            }
            else
            {
                cekLokasi1.Qty += item.Qty;
                context.IcAltItems.Update(cekLokasi1);
            }

            cekItem.Harga = item.Harga;

            if (cekItem.JnsBrng == (int)jnsBrng.Stock)
            {
                cekItem.Qty += item.Qty;
            }

            if (cekItem.CostMethod == (int)costMethod.Moving_Avg)
            {
                cekItem.Cost += item.HrgCost * item.Qty;
            }

            context.IcItems.Update(cekItem);
        }

        private void ReverseExistingDetail(DbContextPersediaan context, OeTransD item, string kode, Dictionary<string, IcItem> itemMap, Dictionary<string, IcAltItem> altItemMap)
        {
            var cekItem = GetItem(context, item.ItemCode, itemMap);
            if (cekItem == null)
            {
                return;
            }

            var cekLokasi1 = GetAltItem(context, item.ItemCode, item.Lokasi, altItemMap);
            if (cekLokasi1 == null)
            {
                cekLokasi1 = CreateAltItem(cekItem, item.Lokasi, kode == "95" ? -1 * item.Qty : item.Qty);
                context.IcAltItems.Add(cekLokasi1);
                SetAltItem(item.ItemCode, item.Lokasi, cekLokasi1, altItemMap);
            }
            else
            {
                if (kode == "95")
                {
                    cekLokasi1.Qty -= item.Qty;
                }
                else
                {
                    cekLokasi1.Qty += item.Qty;
                }

                context.IcAltItems.Update(cekLokasi1);
            }

            if (item.Harga > 0 && item.Harga > cekItem.HrgJual)
            {
                cekItem.HrgJual = item.Harga;
            }

            if (cekItem.JnsBrng == (int)jnsBrng.Stock)
            {
                if (kode == "95")
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
                if (kode == "95")
                {
                    cekItem.Cost -= item.Cost;
                }
                else
                {
                    cekItem.Cost += item.Cost;
                }
            }
            else
            {
                cekItem.Cost = cekItem.StdPrice * cekItem.Qty;
            }

            context.IcItems.Update(cekItem);
        }

        private Dictionary<string, IcItem> LoadItemMap(DbContextPersediaan context, IEnumerable<string> itemCodes)
        {
            var distinctCodes = itemCodes
                .Where(x => string.IsNullOrEmpty(x) == false)
                .Distinct()
                .ToList();

            return context.IcItems
                .Where(x => distinctCodes.Contains(x.ItemCode))
                .AsEnumerable()
                .GroupBy(x => x.ItemCode)
                .ToDictionary(g => g.Key, g => g.First());
        }

        private Dictionary<string, IcAltItem> LoadAltItemMap(DbContextPersediaan context, IEnumerable<string> itemCodes)
        {
            var distinctCodes = itemCodes
                .Where(x => string.IsNullOrEmpty(x) == false)
                .Distinct()
                .ToList();

            return context.IcAltItems
                .Where(x => distinctCodes.Contains(x.ItemCode))
                .AsEnumerable()
                .GroupBy(x => CreateAltItemKey(x.ItemCode, x.Lokasi))
                .ToDictionary(g => g.Key, g => g.First());
        }

        private IcItem GetItem(DbContextPersediaan context, string itemCode, Dictionary<string, IcItem> itemMap)
        {
            if (itemMap != null)
            {
                itemMap.TryGetValue(itemCode, out var item);
                return item;
            }

            return context.IcItems.FirstOrDefault(x => x.ItemCode == itemCode);
        }

        private IcAltItem GetAltItem(DbContextPersediaan context, string itemCode, string lokasi, Dictionary<string, IcAltItem> altItemMap)
        {
            if (altItemMap != null)
            {
                altItemMap.TryGetValue(CreateAltItemKey(itemCode, lokasi), out var altItem);
                return altItem;
            }

            return context.IcAltItems.FirstOrDefault(x => x.ItemCode == itemCode && x.Lokasi == lokasi);
        }

        private void SetAltItem(string itemCode, string lokasi, IcAltItem altItem, Dictionary<string, IcAltItem> altItemMap)
        {
            altItemMap?.TryAdd(CreateAltItemKey(itemCode, lokasi), altItem);
        }

        private static IcAltItem CreateAltItem(IcItem item, string lokasi, decimal qty)
        {
            return new IcAltItem
            {
                ItemCode = item.ItemCode.ToUpper(),
                NamaItem = item.NamaItem,
                Satuan = item.Satuan,
                Lokasi = lokasi,
                Qty = qty
            };
        }

        private static string CreateAltItemKey(string itemCode, string lokasi)
        {
            return $"{itemCode}::{lokasi}";
        }

        private DbContextPersediaan CreateInventoryContext()
        {
            return _contextIc.CreateDbContext();
        }
    }
}
