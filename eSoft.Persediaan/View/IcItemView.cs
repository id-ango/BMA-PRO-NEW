using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.View
{
    public class IcItemView
    {
        [Key]
        public int IcItemId { get; set; }
        [Required]
        public string ItemCode { get; set; }
        [Required]
        public string NamaItem { get; set; }
        [StringLength(5, ErrorMessage = "Satuan terlalu panjang (5 character limit).")]
        public string Satuan { get; set; }
        public string Divisi { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Harga { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "#,###.##")]
        public decimal Qty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal MinQty { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QtyPo { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal BefNetto { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal HrgNetto { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal HrgJual { get; set; }
        [DisplayFormat(DataFormatString = "#,###.##")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoAwal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "#,###.##")]
        public decimal CostAwal { get; set; }
        public string AcctSet { get; set; }
        public string Category { get; set; }
        public bool SerialNo { get; set; }
        [Required]
        public costMethod CostMethod { get; set; } = costMethod.Moving_Avg;
        [Required]
        public jnsBrng JnsBrng { get; set; } = jnsBrng.Stock;
        [Column(TypeName = "decimal(18,2)")]
        public decimal StdPrice { get; set; }
        public string Foto { get; set; }
        public Nullable<DateTime> TglPost { get; set; }
        public Nullable<DateTime> LastNetto { get; set; }
        public string CurrencyCode { get; set; }
        public bool Disabled { get; set; }
        public string NamaLengkap
        {
            get
            {
                return NamaItem + " [" + ItemCode.ToUpper() + "]" + " (" + Satuan + ")";
            }
        }
    }
   

    public enum costMethod { Moving_Avg = 1, Standard_Cost = 2 };
    public enum jnsBrng { Stock = 1, Service = 2, Consumable = 3 };
}
