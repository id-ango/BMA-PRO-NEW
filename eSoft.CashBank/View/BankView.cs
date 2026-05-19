using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.View
{
  

    // ----- Section View

    public class BankView
    {

        public int BankId { get; set; }
        [Required]
        [MinLength(2)]
        [StringLength(2, ErrorMessage = "Kode Bank terlalu panjang (2 character limit).")]
        public string Kdbank { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Nama Bank terlalu panjang (100 character limit).")]
        public string Namabank { get; set; }
        [StringLength(3, ErrorMessage = "Kurs terlalu panjang (3 character limit).")]
        public string Kurs { get; set; }
        [StringLength(7, ErrorMessage = "Akun terlalu panjang (7 character limit).")]
        public string Acctset { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SldAwal { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KSldAwal { get; set; }
        public DateTime ClrDate { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Saldo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KSaldo { get; set; }
        
        public string Status { get; set; }
        public bool NonPpn { get; set; }
        public bool Pajak { get; set; }
        public string CoKode { get; set; }
    }

  
}
