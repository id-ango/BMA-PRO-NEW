using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.Model
{
    public class IcStockCard
    {
        [Key]
        public int IcStockCardId { get; set; }
        public string Kode { get; set; }
        public string Tanggal { get; set; }
        public string ItemCode { get; set; }
        public string Dokumen { get; set; }
        public string MenuApp { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Harga { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Persen { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Discount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Qty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal HrgCost { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Cost { get; set; }
        public string Lokasi { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal QtyShp { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Qtybo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumfps { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal JumDpp { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah { get; set; }
        public string CurrencyCode { get; set; }
        public bool Disabled { get; set; }
    }
}
