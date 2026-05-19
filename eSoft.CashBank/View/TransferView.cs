using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.View
{
    
    public class TransferView
    {
        [Key]
        public int CbTransferId { get; set; }
        public string DocNo { get; set; }
        public string KodeBank1 { get; set; }
        public string KodeBank2 { get; set; }
        public string Refno { get; set; }
        public string Keterangan { get; set; }
        public DateTime Tanggal { get; set; }
        public string Acctset { get; set; }
        public string Giro { get; set; }
        [StringLength(3)]
        public string Kurs { get; set; }
        [StringLength(3)]
        public string Kurs2 { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KValue { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Saldo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KSaldo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Biaya { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KBiaya { get; set; }
        public bool NonPPN { get; set; }
        public bool Pajak { get; set; }
        public bool Cek { get; set; }
    }
}
