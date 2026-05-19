using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Hutang.Model
{
    public class ApTransD
    {
        [Key]
        public int ApTransDId { get; set; }
        public string Bukti { get; set; }
        public DateTime Tanggal { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        [StringLength(2)]
        public string Kode { get; set; }
        [StringLength(2)]
        public string KodeTran { get; set; }

        public string Lpb { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bayar { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Sisa { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Discount { get; set; }
        public string Keterangan { get; set; }
        public string DistCode { get; set; }
        public int ApTransHId { get; set; }
        public ApTransH ApTransH { get; set; }
    }
}
