using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Hutang.View
{
    public class SupplierView
    {
        [Key]
        public int ApSupplId { get; set; }
        [Required]
        [MinLength(5)]
        [StringLength(5, ErrorMessage = "Kode Bank terlalu panjang (5 character limit).")]
        public string Supplier { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Nama Supplier terlalu panjang (100 character limit).")]
        public string NamaSup { get; set; }
        public string Person { get; set; }
        [Required(ErrorMessage = "Alamat Harus Diisi")]
        public string Alamat { get; set; }
        [Required(ErrorMessage = "Kota Harus Diisi")]
        public string Kota { get; set; }
        public string Provinsi { get; set; }
        public string AlmtKrm { get; set; }
        public string KotaKrm { get; set; }
        public string ProvKirim { get; set; }
        [Required(ErrorMessage = "Telpon Harus Diisi")]
        public string Telpon { get; set; }
        public string NPWP_Sup { get; set; }
        public string AlmtNPWP { get; set; }
        public string Expedisi { get; set; }
        [StringLength(3)]
        public string Kurs { get; set; }
        public int Termin { get; set; } = 0;
        [Column(TypeName = "decimal(18,4)")]
        public decimal Disc1 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Disc2 { get; set; }
        public string Kontak { get; set; }
        public string Fax { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SldAwal { get; set; }
        public bool Pajak { get; set; }
        public string AcctSet { get; set; }
        public string AcctPjk { get; set; }
        public DateTime TglPost { get; set; }
        public DateTime TglMasuk { get; set; }
        public DateTime LstOrder { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Hutang { get; set; }
        public string NamaLengkap
        {
            get
            {
                return NamaSup + " [" + Supplier.ToUpper() + "]" ;
            }
        }
    }
}
