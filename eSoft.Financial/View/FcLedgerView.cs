using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace eSoft.Financial.View
{
    public class FcLedgerView
    {

        public int FcTransHId { get; set; }
        public string DocNo { get; set; }
        public string KodeBank { get; set; }
        public string Keterangan { get; set; }
        public DateTime Tanggal { get; set; }
        public string SrcCode { get; set; }
        public string GlAcct { get; set; }
        [Required]
        public string GlNama { get; set; }
        public string TipeGL { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlSaldo { get; set; }
        public Nullable<DateTime> GlPost { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlSldAwal { get; set; }
        public string Kurs { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Saldo { get; set; }
        public decimal Balance { get; set; }
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
    }
}
