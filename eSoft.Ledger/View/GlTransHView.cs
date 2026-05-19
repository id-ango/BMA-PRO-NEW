using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Ledger.View
{
    public class GlTransHView
    {
        [Key]
        public int GlTransHId { get; set; }
        public string DocNo { get; set; }
        public DateTime Tanggal { get; set; }
        public string GlMemo { get; set; }
        public string KodeGl { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Debet
        {
            get
            {
                return GlTransDs.Sum(p => p.Debet);
            }
        }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Kredit
        {
            get
            {
                return GlTransDs.Sum(p => p.Kredit);
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
        public List<GlTransDView> GlTransDs { get; set; }
    }
}
