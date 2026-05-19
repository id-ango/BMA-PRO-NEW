using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.Model
{
    public class CbBank
    {
        [Key]
        public int CbBankId { get; set; }
        public string KodeBank { get; set; }
        [StringLength(100)]
        public string NmBank { get; set; }
        [StringLength(3)]
        public string Kurs { get; set; }
        [StringLength(7)]
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
        public string GrpBank { get; set; }
        public string CoKode { get; set; }

    }

    
}
