using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Hutang.Model
{
    public class ApSuppl
    {
        [Key]
        public int ApSupplId { get; set; }
        public string Supplier { get; set; }
        public string NamaSup { get; set; }
        public string Person { get; set; }
        public string Alamat { get; set; }
        public string Kota { get; set; }
        public string Provinsi { get; set; }
        public string Telpon { get; set; }
        public string NPWP_Sup { get; set; }
        public string AlmtNPWP { get; set; }
        [StringLength(3)]
        public string Kurs { get; set; }
        public int Termin { get; set; }
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
        public Nullable<DateTime> TglPost { get; set; }
        public Nullable<DateTime> TglMasuk { get; set; }
        public Nullable<DateTime> LstOrder { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Hutang { get; set; }
        public string NamaLengkap { get; set; }


    }
}
