using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Piutang.View
{
    public class CustomerView
    {
        [Key]
        public int ArCustId { get; set; }
        [Required]
        [MinLength(5)]
        [StringLength(5, ErrorMessage = "Kode Customer terlalu panjang (5 character limit).")]
        public string Customer { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Nama customer terlalu panjang (100 character limit).")]
        public string NamaCust { get; set; }
        public string Golongan { get; set; }
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
        public string NPWP_Cust { get; set; }
        public string AlmtNPWP { get; set; }
        public string Expedisi { get; set; }
        public int Termin { get; set; } = 0;
        [Column(TypeName = "decimal(18,4)")]
        public decimal Disc1 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Disc2 { get; set; }
        public string Kontak { get; set; }
        public string Fax { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SldAwal { get; set; }
        public bool NonPPN { get; set; }
        public bool Pajak { get; set; }
        public string AcctSet { get; set; }
        public string AcctPjk { get; set; }
        public DateTime TglPost { get; set; }
        public DateTime TglMasuk { get; set; }
        public DateTime LstOrder { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Piutang { get; set; }
        public string NamaLengkap
        {
            get
            {
                return NamaCust + " [" + Customer.ToUpper() + "]" ;
            }
        }
    }
}
