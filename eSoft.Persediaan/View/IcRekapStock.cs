using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.View
{
    public class IcRekapStock
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
        public decimal QtyAwal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoAwal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QtyMasuk { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoMasuk { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QtyKeluar { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoKeluar { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QtyAdjust { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoAdjust { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal QtyAkhir
        {
            get
            {
                return QtyAwal + QtyMasuk - QtyKeluar + QtyAdjust;
            }
            private set { }
        }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoAkhir { get
            {
                return SaldoAwal + SaldoMasuk - SaldoKeluar + SaldoAdjust;
            }
            private set { }
        }
    }
}
