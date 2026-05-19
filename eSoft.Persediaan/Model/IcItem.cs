using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace eSoft.Persediaan.Model
{
    public class IcItem
    {
        [Key]
        public int IcItemId { get; set; }
        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public string Satuan { get; set; }
        public string Divisi { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal HrgUsd { get; set; }                        // harga beli dalam mata uang asing
        [Column(TypeName = "decimal(18,4)")]
        public decimal Cost { get; set; }                               // total cost
        [Column(TypeName = "decimal(18,4)")]
        public decimal Harga { get; set; }                          // harga beli
        [Column(TypeName = "decimal(18,4)")]
        public decimal Qty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal MinQty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal QtyPo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BefNetto { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal HrgNetto { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        [DisplayFormat(DataFormatString = "#,###.##")]
        public decimal HrgJual { get; set; }                        // harga jual
        [Column(TypeName = "decimal(18,4)")]
        public decimal SaldoAwal { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal CostAwal { get; set; }
        public string AcctSet { get; set; }
        public string Category { get; set; }
        public bool SerialNo { get; set; }
        public int CostMethod { get; set; }
        public int JnsBrng { get; set; }
        public string Foto { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal StdPrice { get; set; }                       // standard price       
        public Nullable<DateTime> TglPost { get; set; }
        public Nullable<DateTime> LastNetto { get; set; }
        public string NamaLengkap { get; set; }
        public string CurrencyCode { get; set; }
        public bool Disabled { get; set; }

    }
}
