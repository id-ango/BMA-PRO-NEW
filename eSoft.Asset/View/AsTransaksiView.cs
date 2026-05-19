using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Asset.View
{
    public class AsTransaksiView
    {
        public int AsTransaksiId { get; set; }
        public string Kode { get; set; }
        public string BarcodeAssets { get; set; }
        public DateTime Tanggal { get; set; }
        public decimal Nilai { get; set; }
        public decimal Saldo { get; set; }
    }
}
