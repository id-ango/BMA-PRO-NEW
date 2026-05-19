using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Financial.View
{
    public class FcPrintGLView
    {
        public int FcPrintGlId { get; set; }
        public string FcComKode { get; set; }
        public string KodeCetak { get; set; }
        public string NoBaris { get; set; }
        public string Keterangan { get; set; }
        public string NoRek1 { get; set; }
        public string NoRek2 { get; set; }
        public bool CetakDetil { get; set; }
        public bool CetakGaris1 { get; set; }
        public bool CetakGaris2 { get; set; }
        public bool CetakBln1 { get; set; }
        public bool CetakBln2 { get; set; }
        public int Spasi { get; set; }
        public bool CetakTebal { get; set; }
        public bool CetakNegatif { get; set; }
        public bool CetakHide { get; set; }
        public string RumusBaris { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Persen1 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Persen2 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Qty1 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Qty2 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal JumTran1 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal JumTran2 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal JumRekap1 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal JumRekap2 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal JumSaldo { get; set; }
    }
}
