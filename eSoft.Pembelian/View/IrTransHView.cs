using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Pembelian.View
{
    public class IrTransHView
    {
        private decimal ppn;

        [Key]
        public int IrTransHId { get; set; }
        [StringLength(2)]
        public string Kode { get; set; }
        public string NoLpb { get; set; }
        public string NoPrj { get; set; }
        public string Lokasi { get; set; }
        public DateTime Tanggal { get; set; }
        public DateTime JthTempo { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TtlJumlah
        {
            get
            {
                return IrTransDs.Sum(p => p.Jumlah);
            }
        }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DPayment { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Ongkos { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PpnPersen { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Ppn
        {
            get
            {
                if (PpnPersen != 0)
                {
                    return TtlJumlah * PpnPersen / 100;
                }
                else
                {
                    return ppn;
                }


            }
            set
            {
                ppn = value;
            }
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Jumlah
        {
            get
            {
                return TtlJumlah + Ongkos + Ppn;
            }
        }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalQty
        {
            get
            {
                return IrTransDs.Sum(p => p.Qty);
            }
        }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Tagihan
        {
            get
            {
                return Jumlah - DPayment;
            }
        }
        [Required]
        public string Supplier { get; set; }
        public string NamaSup { get; set; }
        public string Keterangan { get; set; }
        public string Cek { get; set; }
        public bool Pajak { get; set; }
        public string Status { get; set; }
        public decimal Kurs { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Nilai
        {

            get
            {
                return Kurs * Jumlah;
            }
        }
        
        public string Currency { get; set; }
        public List<IrTransDView> IrTransDs { get; set; }

    }
}
