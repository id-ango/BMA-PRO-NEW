using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Hutang.Model;

namespace eSoft.Hutang.View
{
    public class ApTransHView
    {
        [Key]
        public int ApTransHId { get; set; }
        public string Kode { get; set; }
        public string Bukti { get; set; }
        public DateTime Tanggal { get; set; }
        public string KdBank { get; set; }
        public string Supplier { get; set; }
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
                return ApTransDs.Sum(p => p.Jumlah);
            }
        }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Discount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Hutang { get; set; }
        public bool Pajak { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Unapplied { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Kurs { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Nilai
        {
           
            get
            {
                return Kurs * JumBayar;
            }
        }
        public decimal JumNilai
        {
           
            get
            {
                return Kurs * Jumlah;
            }
        }
        public string Currency { get; set; }
        public string AcctSet { get; set; }
        public List<ApTransDView> ApTransDs { get; set; }
        public int ApSupplId { get; set; }
        public ApSuppl ApSuppl { get; set; }
        public decimal JumBayar { get; set; }
        public decimal UpdateUnapplied
        {
            get
            {
                return JumBayar - ApTransDs.Sum(p => p.Bayar);
            }
        }

        public decimal JumDiskon
        {
            get
            {
                return ApTransDs.Sum(p => p.Discount);
            }
        }

        public decimal JumHutang
        {
            get
            {
                return ApTransDs.Sum(p => p.Bayar + p.Discount);
            }
        }
    }
}
