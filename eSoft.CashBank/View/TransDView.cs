using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.View
{
    public class TransDView
    {

        public int CbTransDId { get; set; }
        public string NoPrj { get; set; }
        public string SrcCode { get; set; }
        public string GlAcct { get; set; }
        public string Keterangan { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah
        {
            get
            {
                return Terima - Bayar;
            }

        }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Terima { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bayar { get; set; }
        public string Kurs { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KValue { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KJumlah
        {
            get
            {
                return KTerima - KBayar;
            }
        }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KTerima { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KBayar { get; set; }
        public int TransHId { get; set; }
        public TransHView TransH { get; set; }

    }

   
}
