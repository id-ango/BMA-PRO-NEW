using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Financial.View
{
    public class FcTransDView
    {
        [Key]
        public int FcTransDId { get; set; }
        public string FcComKode { get; set; }
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

        public int FcTransHId { get; set; }
        public FcTransHView FcTransH { get; set; }
    }
}
