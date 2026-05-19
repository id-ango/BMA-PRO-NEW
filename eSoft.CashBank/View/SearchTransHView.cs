using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.View
{
    public class SearchTransHView
    {
        public int CbTransHId { get; set; }
        public string KodeDoc { get; set; }
        public string DocNo { get; set; }      
        public string KodeBank { get; set; }     
        public string Keterangan { get; set; }
        public DateTime Tanggal { get; set; } 
        public string Kurs { get; set; }

    }
}
