using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Penjualan.View
{
    public class OePerTahun
    {
        [Key]
        public int OeTransId { get; set; }
        [StringLength(2)]
        public string Kode { get; set; }
        public string NoLpb { get; set; }
      
        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public string Divisi { get; set; }
        public string NamaDiv { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Qty { get; set; }
        public int Bulan { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BulanTotal { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan01 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan02 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan03 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan04 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan05 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan06 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan07 { get; set; }
      
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan08 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan09 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan10 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan11 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bulan12 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan01 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan02 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan03 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan04 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan05 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan06 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan07 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan08 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan09 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan10 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan11 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BeliBulan12 { get; set; }
      
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan01 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan02 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan03 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan04 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan05 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan06 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan07 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan08 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan09 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan10 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan11 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AwalBulan12 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalJual { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalBeli { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalOrder { get; set; }
    }
}
