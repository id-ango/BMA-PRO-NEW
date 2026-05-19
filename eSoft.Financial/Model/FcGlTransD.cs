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
    public class FcGlTransD
    {
        [Key]
        public int FcGlTransDId { get; set; }
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
        public int FcGlTransHId { get; set; }
        public FcGlTransH FcGlTransH { get; set; }
    }
}
