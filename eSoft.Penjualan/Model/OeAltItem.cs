using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Penjualan.Model
{
    public class OeAltItem
    {
        [Key]
        public int OeAltItemId { get; set; }
        public string ItemCode { get; set; }
        public string Salesman { get; set; }
        public string NamaItem { get; set; }
        public string Satuan { get; set; }
        public string Divisi { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Harga { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Qty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal QtyPo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BefNetto { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal HrgNetto { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        [DisplayFormat(DataFormatString = "#,###.##")]
        public decimal HrgJual { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SaldoAwal { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal CostAwal { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Komisi { get; set; }
        public string AcctSet { get; set; }
        public string Category { get; set; }
        public bool SerialNo { get; set; }
        public int CostMethod { get; set; }
        public int JnsBrng { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal StdPrice { get; set; }

        public Nullable<DateTime> TglPost { get; set; }

        public Nullable<DateTime> LastNetto { get; set; }
    }
}
