using eSoft.Piutang.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Piutang.View
{
    public class ArTransHView
    {
        [Key]
        public int ArTransHId { get; set; }
        public string Kode { get; set; }
        public string Bukti { get; set; }
        public DateTime Tanggal { get; set; }
        public string KdBank { get; set; }
        [Required]
        public string Customer { get; set; }
        public string NoFaktur { get; set; }
        public string Keterangan { get; set; }
        public DateTime JthTempo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PPn { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PPh { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal JumPPh { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal JumPPn { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bruto { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Netto { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah
        {
            get
            {
                return ArTransDs.Sum(p => p.Jumlah);
            }
        }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Discount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Piutang { get; set; }
        public bool Pajak { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Unapplied { get; set; }
        public string AcctSet { get; set; }
        public List<ArTransDView> ArTransDs { get; set; }
        public int ArCustId { get; set; }
        public ArCust ArCust { get; set; }
        public decimal JumBayar { get; set; }
        public decimal UpdateUnapplied
        {
            get
            {
                return JumBayar - ArTransDs.Sum(p => p.Bayar );
            }
        }

        public decimal JumDiskon
        {
            get
            {
                return ArTransDs.Sum(p => p.Discount);
            }
        }

        public decimal JumPiutang
        {
            get
            {
                return ArTransDs.Sum(p => p.Bayar + p.Discount);
            }
        }
    }
}
