using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Ledger.View
{
    public class GlTransDView
    {
        [Key]
        public int GlTransDId { get; set; }
        public string GlAcct { get; set; }
        public string GlDept { get; set; }
        public string Keterangan { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Debet { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Kredit { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah
        {
            get
            {
                return Debet - Kredit;
            }
        }

        public int GlTransHId { get; set; }
        public GlTransHView GlTransH { get; set; }
    }
}
