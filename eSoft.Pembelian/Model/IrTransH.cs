using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Pembelian.Model
{
    public class IrTransH
    {
        [Key]
        public int IrTransHId { get; set; }
        [StringLength(2)]
        public string Kode { get; set; }
        public string NoLpb { get; set; }
        public string NoSJ { get; set; }
        public string NoPrj { get; set; }
        public string Lokasi { get; set; }
        public DateTime Tanggal { get; set; }
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
        public decimal Jumlah { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Tagihan { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalQty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal QtyTerima { get; set; }
        public string Supplier { get; set; }
        public string NamaSup { get; set; }
        public string Keterangan { get; set; }
        public string Cek { get; set; }
        public bool Pajak { get; set; }
        public string Status { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Kurs { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Nilai { get; set; }
        public string Currency { get; set; }
        public List<IrTransD> IrTransDs { get; set; }

    }
}
