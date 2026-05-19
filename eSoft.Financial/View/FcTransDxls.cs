using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Financial.View
{
    public class FcTransDxls
    {
        [Key]
        public int FcTransDId { get; set; }
        public string DocNo { get; set; }
        public DateTime Tanggal { get; set; }
        public string GlMemo { get; set; }
        public string KodeGl { get; set; }

        public string FcComKode { get; set; }
        public string GlAcct { get; set; }
        public string GlDept { get; set; }
        public string Keterangan { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Debet { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Kredit { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah { get; set; }
        [StringLength(3)]
        public string Kurs { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal NomKurs { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal JumKurs { get; set; }
        public bool NonPPn { get; set; }
        public int FcTransHId { get; set; }
       
    }
}
