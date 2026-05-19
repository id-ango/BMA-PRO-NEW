using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.View
{
    public class RekeningView
    {
        public int CbTransHId { get; set; }
        public string DocNo { get; set; } 
        public string KodeBank { get; set; }      
        public string Keterangan { get; set; }
        public DateTime Tanggal { get; set; }
        public string SrcCode { get; set; }

        public string Kurs { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Saldo { get; set; }
        public decimal Balance { get; set; }

    }
}

