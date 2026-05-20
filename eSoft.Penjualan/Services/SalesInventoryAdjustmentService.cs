using System.Collections.Generic;
using System.Linq;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using eSoft.Persediaan.View;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;

namespace eSoft.Penjualan.Services
{
    public class SalesInventoryAdjustmentService : ISalesInventoryAdjustmentService
    {
        private readonly DbContextPersediaan _contextIc;

        public SalesInventoryAdjustmentService(DbContextPersediaan contextIc)
        {
            _contextIc = contextIc;
        }

        public void ApplySaleDetail(OeTransDView item)
        {
            ApplySaleDetail(item, null, null);
        }

        public void ApplyDetailsForCode(IEnumerable<OeTransDView> items, string kode)
        {
            var validItems = items.Where(x => x.Qty != 0).ToList();
            if (validItems.Count == 0)
            {
                return;
            }

            var itemMap = LoadItemMap(validItems.Select(x => x.ItemCode));
            var altItemMap = LoadAltItemMap(validItems.Select(x => x.ItemCode));

            foreach (var item in validItems)
            {
                if (kode == "95")
                {
                    ApplyReturnDetail(item, itemMap, altItemMap);
                }
                else
                {
                    ApplySaleDetail(item, itemMap, altItemMap);
                }
            }
        }

        public void ApplyReturnDetail(OeTransDView item)
        {
            ApplyReturnDetail(item, null, null);
        }

        public void ReverseExistingDetail(OeTransD item, string kode)
        {
            ReverseExistingDetail(item, kode, null, null);
        }

        public void ReverseDetails(IEnumerable<OeTransD> items, string kode)
        {
            var validItems = items.Where(x => x.Qty != 0).ToList();
            if (validItems.Count == 0)
            {
                return;
            }

            var itemMap = LoadItemMap(validItems.Select(x => x.ItemCode));
            var altItemMap = LoadAltItemMap(validItems.Select(x => x.ItemCode));

            foreach (var item in validItems)
            {
                ReverseExistingDetail(item, kode, itemMap, altItemMap);
            }
        }

        private void ApplySaleDetail(OeTransDView item, Dictionary<string, IcItem> itemMap, Dictionary<string, IcAltItem> altItemMap)
        {
            var cekItem = GetItem(item.ItemCode, itemMap);
            if (cekItem == null)
            {
                return;
            }

            var cekLokasi1 = GetAltItem(item.ItemCode, item.Lokasi, altItemMap);
            if (cekLokasi1 == null)
            {
                cekLokasi1 = CreateAltItem(cekItem, item.Lokasi, -1 * item.Qty);
                _contextIc.IcAltItems.Add(cekLokasi1);
                SetAltItem(item.ItemCode, item.Lokasi, cekLokasi1, altItemMap);
            }
            else
            {
                cekLokasi1.Qty -= item.Qty;
                _contextIc.IcAltItems.Update(cekLokasi1);
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

            _contextIc.IcItems.Update(cekItem);
        }

        private void ApplyReturnDetail(OeTransDView item, Dictionary<string, IcItem> itemMap, Dictionary<string, IcAltItem> altItemMap)
        {
            var cekItem = GetItem(item.ItemCode, itemMap);
            if (cekItem == null)
            {
                return;
            }

            var cekLokasi1 = GetAltItem(item.ItemCode, item.Lokasi, altItemMap);
            if (cekLokasi1 == null)
            {
                cekLokasi1 = CreateAltItem(cekItem, item.Lokasi, item.Qty);
                _contextIc.IcAltItems.Add(cekLokasi1);
                SetAltItem(item.ItemCode, item.Lokasi, cekLokasi1, altItemMap);
            }
            else
            {
                cekLokasi1.Qty += item.Qty;
                _contextIc.IcAltItems.Update(cekLokasi1);
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

            _contextIc.IcItems.Update(cekItem);
        }

        private void ReverseExistingDetail(OeTransD item, string kode, Dictionary<string, IcItem> itemMap, Dictionary<string, IcAltItem> altItemMap)
        {
            var cekItem = GetItem(item.ItemCode, itemMap);
            if (cekItem == null)
            {
                return;
            }

            var cekLokasi1 = GetAltItem(item.ItemCode, item.Lokasi, altItemMap);
            if (cekLokasi1 == null)
            {
                cekLokasi1 = CreateAltItem(cekItem, item.Lokasi, kode == "95" ? -1 * item.Qty : item.Qty);
                _contextIc.IcAltItems.Add(cekLokasi1);
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

                _contextIc.IcAltItems.Update(cekLokasi1);
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

            _contextIc.IcItems.Update(cekItem);
        }

        private Dictionary<string, IcItem> LoadItemMap(IEnumerable<string> itemCodes)
        {
            var distinctCodes = itemCodes
                .Where(x => string.IsNullOrEmpty(x) == false)
                .Distinct()
                .ToList();

            return _contextIc.IcItems
                .Where(x => distinctCodes.Contains(x.ItemCode))
                .ToDictionary(x => x.ItemCode);
        }

        private Dictionary<string, IcAltItem> LoadAltItemMap(IEnumerable<string> itemCodes)
        {
            var distinctCodes = itemCodes
                .Where(x => string.IsNullOrEmpty(x) == false)
                .Distinct()
                .ToList();

            return _contextIc.IcAltItems
                .Where(x => distinctCodes.Contains(x.ItemCode))
                .ToDictionary(x => CreateAltItemKey(x.ItemCode, x.Lokasi));
        }

        private IcItem GetItem(string itemCode, Dictionary<string, IcItem> itemMap)
        {
            if (itemMap != null)
            {
                itemMap.TryGetValue(itemCode, out var item);
                return item;
            }

            return _contextIc.IcItems.FirstOrDefault(x => x.ItemCode == itemCode);
        }

        private IcAltItem GetAltItem(string itemCode, string lokasi, Dictionary<string, IcAltItem> altItemMap)
        {
            if (altItemMap != null)
            {
                altItemMap.TryGetValue(CreateAltItemKey(itemCode, lokasi), out var altItem);
                return altItem;
            }

            return _contextIc.IcAltItems.FirstOrDefault(x => x.ItemCode == itemCode && x.Lokasi == lokasi);
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
    }
}
