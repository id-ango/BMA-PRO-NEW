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
        // Per-transaction target: "CB" (cashbank), "AP", or "AR"
        public string Target { get; set; } = "CB";
        public bool IsSelected { get; set; }
        // Marked when a similar transaction already exists in the system
        public bool IsDuplicate { get; set; }
        // Selected customer (for AR) or supplier (for AP)
        public string PartyCode { get; set; }
        // Outstanding documents for selected customer/supplier
        public List<OutstandingDocView> OutstandingDocs { get; set; } = new List<OutstandingDocView>();

        public class OutstandingDocView
        {
            public string Dokumen { get; set; }
            public DateTime Tanggal { get; set; }
            public DateTime? DueDate { get; set; }
            public string KodeTran { get; set; }
            public decimal Sisa { get; set; }
            public decimal Bayar { get; set; }
            public decimal Discount { get; set; }
            public bool IsSelected { get; set; }
            // Exchange rate for foreign-currency AP docs (Kurs=0 or 1 means local IDR)
            public decimal Kurs { get; set; }
            // Sisa converted to IDR for display/comparison purposes
            public decimal SisaIDR => Kurs > 1 ? Sisa * Kurs : Sisa;
            public string Label => Kurs > 1
                ? $"{Dokumen} ({Tanggal:dd/MM/yy}) – {Sisa:N2} [{Kurs:N2} = IDR {SisaIDR:N2}]"
                : $"{Dokumen} ({Tanggal:dd/MM/yy}) – {Sisa:N2}";
        }
    }
}
