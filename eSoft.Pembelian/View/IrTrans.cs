using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Pembelian.View
{
    public class IrTrans
    {
        [Key]
        public int IrTransId { get; set; }
        [StringLength(2)]
        public string Kode { get; set; }
        public string NoLpb { get; set; }
      
        public DateTime Tanggal { get; set; }
        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public string Satuan { get; set; }
        public string Lokasi { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Harga { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Persen { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Discount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Qty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal QtyBo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal HrgCost { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal JumDpp { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah { get; set; }
        public string Supplier{ get; set; }
        public string AcctSet { get; set; }
        public int IrTransHId { get; set; }

        public string NoSJ { get; set; }
        public string NoPrj { get; set; }

        public DateTime JthTempo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TtlJumlah { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal DPayment { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Ongkos { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PpnPersen { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Ppn { get; set; }
        [Column(TypeName = "decimal(18,4)")]

        public decimal Tagihan { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalQty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal QtyTerima { get; set; }

        public string NamaSuppl { get; set; }
        public string Keterangan { get; set; }
        public string AlamatKirim { get; set; }
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
    }
}
