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
            var cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
            if (cekItem == null)
            {
                return;
            }

            var cekLokasi1 = _contextIc.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
            if (cekLokasi1 == null)
            {
                var produk = new IcAltItem
                {
                    ItemCode = cekItem.ItemCode.ToUpper(),
                    NamaItem = cekItem.NamaItem,
                    Satuan = cekItem.Satuan,
                    Lokasi = item.Lokasi,
                    Qty = -1 * item.Qty
                };
                _contextIc.IcAltItems.Add(produk);
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

        public void ApplyDetailsForCode(IEnumerable<OeTransDView> items, string kode)
        {
            foreach (var item in items)
            {
                if (item.Qty == 0)
                {
                    continue;
                }

                if (kode == "95")
                {
                    ApplyReturnDetail(item);
                }
                else
                {
                    ApplySaleDetail(item);
                }
            }
        }

        public void ApplyReturnDetail(OeTransDView item)
        {
            var cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
            if (cekItem == null)
            {
                return;
            }

            var cekLokasi1 = _contextIc.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
            if (cekLokasi1 == null)
            {
                var produk = new IcAltItem
                {
                    ItemCode = cekItem.ItemCode.ToUpper(),
                    NamaItem = cekItem.NamaItem,
                    Satuan = cekItem.Satuan,
                    Lokasi = item.Lokasi,
                    Qty = item.Qty
                };
                _contextIc.IcAltItems.Add(produk);
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

        public void ReverseExistingDetail(OeTransD item, string kode)
        {
            var cekItem = _contextIc.IcItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
            if (cekItem == null)
            {
                return;
            }

            var cekLokasi1 = _contextIc.IcAltItems.Where(x => x.ItemCode == item.ItemCode && x.Lokasi == item.Lokasi).FirstOrDefault();
            if (cekLokasi1 == null)
            {
                var produk = new IcAltItem
                {
                    ItemCode = cekItem.ItemCode.ToUpper(),
                    NamaItem = cekItem.NamaItem,
                    Satuan = cekItem.Satuan,
                    Lokasi = item.Lokasi,
                    Qty = kode == "95" ? -1 * item.Qty : item.Qty
                };
                _contextIc.IcAltItems.Add(produk);
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

        public void ReverseDetails(IEnumerable<OeTransD> items, string kode)
        {
            foreach (var item in items)
            {
                if (item.Qty == 0)
                {
                    continue;
                }

                ReverseExistingDetail(item, kode);
            }
        }
    }
}
