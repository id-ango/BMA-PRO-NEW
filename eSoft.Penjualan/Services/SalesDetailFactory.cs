using System.Collections.Generic;
using eSoft.Penjualan.Model;
using eSoft.Penjualan.View;

namespace eSoft.Penjualan.Services
{
    public class SalesDetailFactory : ISalesDetailFactory
    {
        public List<OeTransD> CreateDetails(OeTransHView trans, string noLpb, string kode)
        {
            var details = new List<OeTransD>();

            foreach (var item in trans.OeTransDs)
            {
                if (item.Qty == 0)
                {
                    continue;
                }

                decimal jumDpp;
                if (trans.TotalQty != 0)
                {
                    jumDpp = (item.Jumlah - item.Discount) - (item.Qty / trans.TotalQty * trans.Ppn) + (item.Qty / trans.TotalQty * trans.Ongkos);
                }
                else
                {
                    jumDpp = item.Jumlah - item.Discount;
                }

                details.Add(new OeTransD
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
                    Kode = kode,
                    NoLpb = noLpb,
                    Tanggal = trans.Tanggal,
                    HrgCost = item.HrgCost,
                    Cost = item.HrgCost * item.Qty,
                    JumDpp = jumDpp
                });
            }

            return details;
        }
    }
}
