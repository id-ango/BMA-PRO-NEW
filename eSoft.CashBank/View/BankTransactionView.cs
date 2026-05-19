using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.View
{
    public class BankTransactionView
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public string Branch { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public decimal Balance { get; set; }
        public DateTime Tanggal { get; set; }
        public string NoPrj { get; set; }
        public string SrcCode { get; set; }
        public bool IsSelected { get; set; }
    }
}
