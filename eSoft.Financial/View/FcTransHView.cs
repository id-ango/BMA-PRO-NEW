using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Financial.View
{
    public class FcTransHView
    {
        [Key]
        public int GlTransHId { get; set; }
        public string FcComKode { get; set; }
        public string DocNo { get; set; }
        public DateTime Tanggal { get; set; }
        public string GlMemo { get; set; }
        public string KodeGl { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Debet
        {
            get
            {
                return FcTransDs.Sum(p => p.Debet);
            }
        }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Kredit
        {
            get
            {
                return FcTransDs.Sum(p => p.Kredit);
            }
        }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Saldo
        {
            get
            {
                return Debet - Kredit;
            }
        }

        public string Kurs { get; set; }
        public bool Pajak { get; set; }
        public List<FcTransDView> FcTransDs { get; set; }
    }
}
