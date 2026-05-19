using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Asset.View
{
    public class AsItemView
    {
       
        public int AsItemId { get; set; }

        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public string Satuan { get; set; }
        public string Divisi { get; set; }
        public int Qty { get; set; }
        public decimal Saldo { get; set; }
    }
}
