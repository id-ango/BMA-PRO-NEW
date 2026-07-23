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

        // ============== NEW FIELDS FOR APDP/ARDP SUPPORT ==============
        /// <summary>
        /// Currency code (USD, EUR, SGD, etc.) for multi-currency transactions
        /// Only used for AP/APDP transactions. Defaults to "IDR"
        /// </summary>
        public string Currency { get; set; } = "IDR";

        /// <summary>
        /// Exchange rate (e.g., 15500 = 1 USD = 15,500 IDR)
        /// Only used for AP/APDP with foreign currency. Defaults to 1 (IDR only)
        /// </summary>
        public decimal Kurs { get; set; } = 1m;

        /// <summary>
        /// Amount in foreign currency (e.g., 300 USD)
        /// Calculated from Amount / Kurs when needed
        /// </summary>
        public decimal Nilai { get; set; }

        /// <summary>
        /// Transaction type: "PAYMENT" or "DOWNPAYMENT"
        /// Used to distinguish regular payments from down payments
        /// Visible for AR/AP, auto-set for ARDP/APDP
        /// </summary>
        public string TransactionType { get; set; } = "PAYMENT";

        /// <summary>
        /// Supplier/Customer ID (for setting master data links)
        /// Populated when party (supplier/customer) is selected
        /// </summary>
        public int PartyId { get; set; }

        /// <summary>
        /// Supplier/Customer Name (for display and audit trail)
        /// Populated when party (supplier/customer) is selected
        /// </summary>
        public string PartyName { get; set; }
        // ============================================================

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
