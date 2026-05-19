using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Financial.Model
{
    public class FcGlTransH
    {
        [Key]
        public int FcGlTransHId { get; set; }
        public string FcComKode { get; set; }
        public string DocNo { get; set; }
        public DateTime Tanggal { get; set; }
        public string GlMemo { get; set; }
        public string KodeGl { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Debet { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Kredit { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Saldo { get; set; }
        public string Kurs { get; set; }
        public bool NonPPn { get; set; }
        public string Cek { get; set; }
        public List<FcGlTransD> FcGlTransDs { get; set; }
    }
}
