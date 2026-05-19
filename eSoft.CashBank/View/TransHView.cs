using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSoft.CashBank.View
{
    public class TransHView
    {

        public int CbTransHId { get; set; }
       
        [StringLength(3, ErrorMessage = "Code terlalu panjang (3 character limit).")]
        public string KodeDoc { get; set; }
        public string DocNo { get; set; }
        [Required]
        public string KodeBank { get; set; }
        public string Refno { get; set; }
        public string Keterangan { get; set; }

        public DateTime Tanggal { get; set; }
        [StringLength(3)]
        public string Kurs { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Saldo
        {
            get
            {
                return TransDs.Sum(p => p.Jumlah);
            }
        }
        [Column(TypeName = "decimal(18,4)")]
        public decimal KSaldo
        {
            get
            {
                return TransDs.Sum(p => p.KJumlah);
            }
        }

        public bool NonPPN { get; set; }
        public bool Pajak { get; set; }
        public bool Cek { get; set; }


        public List<TransDView> TransDs { get; set; }

    }


}
