using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Asset.Model
{
    public class AsItem
    {
        [Key]
        public int AsItemId { get; set; }
       
        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public string Satuan { get; set; }
        public string Divisi { get; set; }
        public int Qty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Saldo { get; set; }

    }
}
