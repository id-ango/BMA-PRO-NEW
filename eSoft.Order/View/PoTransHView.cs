using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Order.View
{
    public class PoTransHView
    {
        private decimal ppn;

        [Key]
        public int PoTransHId { get; set; }
        [StringLength(2)]
        public string Kode { get; set; }
        public string NoLpb { get; set; }
        public string NoPrj { get; set; }
        public string Lokasi { get; set; }
        public DateTime Tanggal { get; set; }
        public DateTime JthTempo { get; set; }
        public string Currency { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Kurs { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TtlJumlah
        {
            get
            {
                return PoTransDs.Sum(p => p.Jumlah);
            }
        }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DPayment { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }
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
                    return (TtlJumlah-Discount) * PpnPersen / 100;
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
                return TtlJumlah - Discount + Ongkos + Ppn;
            }
        }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalQty
        {
            get
            {
                return PoTransDs.Sum(p => p.Qty);
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

        public decimal Nilai
        {

            get
            {
                return Kurs * Jumlah;
            }
        }
       

        [Required]
        public string Vendor { get; set; }
        public string NamaVendor { get; set; }
        public string Keterangan { get; set; }
        public string Cek { get; set; }
        public bool Pajak { get; set; }
        public string Status { get; set; }
        public List<PoTransDView> PoTransDs { get; set; }

    }
}
