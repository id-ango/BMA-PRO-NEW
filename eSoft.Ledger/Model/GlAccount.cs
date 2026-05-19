using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Ledger.Model
{
    public class GlAccount
    {
        [Key]
        public int GlAccountId { get; set; }
        public string GlAcct { get; set; }
        public string GlNama { get; set; }
        public string GlDept { get; set; }
        public string TipeGl { get; set; }
        public string GlStatus { get; set; }
        public int GlTipe { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlSaldo { get; set; }
        public Nullable<DateTime> GlPost { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlSldAwal { get; set; }
        public string GlKurs { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc1 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc2 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc3 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc4 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc5 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc6 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc7 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc8 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc9 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc10 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc11 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlFisc12 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc1 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc2 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc3 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc4 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc5 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc6 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc7 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc8 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc9 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc10 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc11 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlPreFisc12 { get; set; }
        public string NamaLengkap { get; set; }
    }
}

