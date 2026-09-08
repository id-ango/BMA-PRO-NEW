using System;
using System.Collections.Generic;

namespace eSoft.Persediaan.View
{
    public class IcStockOpnameView
    {
        public DateTime Tanggal { get; set; }
        public string Keterangan { get; set; }
        public List<IcStockOpnameLineView> Lines { get; set; } = new();
    }

    public class IcStockOpnameLineView
    {
        public string ItemCode { get; set; }
        public string Lokasi { get; set; }
        public decimal QtySistem { get; set; }
        public decimal QtyFisik { get; set; }
    }
}
