using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Hutang.View
{
    public class ApHutangView
    {
        public int ApHutangId { get; set; }
        public string Kode { get; set; }
        public string Dokumen { get; set; }
        public DateTime Tanggal { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public string KodeTran { get; set; }
        public string LPB { get; set; }
        public string Keterangan { get; set; }
        public string Supplier { get; set; }
        public string NamaSuppl { get; set; }
        public string KdBank { get; set; }
        public bool Pajak { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bayar { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Sisa { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Discount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal UnApplied { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Dpp { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PPn { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PPh { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SldSisa { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SldBayar { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SldDisc { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SldUnpl { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Kurs { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Nilai { get; set; }
        public string Currency { get; set; }
    }
}
