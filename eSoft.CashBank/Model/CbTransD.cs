using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.Model
{
    
    

    public class CbTransD
    {
        [Key]
        public int CbTransDId { get; set; }
        public string NoPrj { get; set; }
        public int SrcCodeId { get; set; }
        public string SrcCode { get; set; }
        public string GlAcct { get; set; }
        public string Keterangan { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Terima { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bayar { get; set; }
        public string Kurs { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KValue { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KJumlah { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KTerima { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KBayar { get; set; }
        public int CbTransHId { get; set; }
        public CbTransH CbTransH { get; set; }

    }

   
}
