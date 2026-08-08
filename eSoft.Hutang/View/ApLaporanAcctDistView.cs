using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Hutang.View
{
    /// <summary>
    /// View model untuk Laporan Hutang dengan Account Set dan Distribution
    /// Digunakan untuk reconciliation dengan General Ledger
    /// </summary>
    public class ApLaporanAcctDistView
    {
        [Key]
        public int Id { get; set; }

        // Header Info
        public DateTime Tanggal { get; set; }
        public string Bukti { get; set; }
        public string NoFaktur { get; set; }
        public Nullable<DateTime> DueDate { get; set; }

        // Supplier Info
        public string Supplier { get; set; }
        public string NamaSup { get; set; }

        // Account Set Info (untuk GL reconciliation)
        public string AcctSet { get; set; }
        public string AcctSetDesc { get; set; }

        // Distribution Info
        public string DistCode { get; set; }
        public string DistDesc { get; set; }

        // Document Type
        [StringLength(2)]
        public string Kode { get; set; }
        [StringLength(2)]
        public string KodeTran { get; set; }

        // Amount Info
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PPn { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PPh { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Bayar { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Sisa { get; set; }

        // Currency Info
        public string Kurs { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal KursValue { get; set; }

        public string Currency { get; set; }

        // GL Account Code (dari ApAcct)
        public string GlAcct1 { get; set; }
        public string GlAcct2 { get; set; }
        public string GlAcct3 { get; set; }
        public string GlAcct4 { get; set; }
        public string GlAcct5 { get; set; }
        public string GlAcct6 { get; set; }

        // GL Account Amount (Yellow columns - dari Account Set)
        // Nilai yang masuk ke masing-masing account
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlAcct1Amt { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlAcct2Amt { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlAcct3Amt { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlAcct4Amt { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlAcct5Amt { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlAcct6Amt { get; set; }

        // GL Distribution Account Amount (Red column - dari Distribution)
        [Column(TypeName = "decimal(18,4)")]
        public decimal GlDistributionAmt { get; set; }

        // GL Posting Info (untuk posting ke GL)
        public string PostingGlAccount { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PostingGlAmount { get; set; }

        public string PostingGlType { get; set; } // "DEBIT" atau "CREDIT"

        // Reference
        public string Keterangan { get; set; }
        public string Lpb { get; set; }

        // Status
        public string Cek { get; set; }

        // Audit
        public int ApTransDId { get; set; }
        public int ApTransHId { get; set; }
    }

    /// <summary>
    /// Summary view untuk aggregasi by Account Set
    /// </summary>
    public class ApLaporanAcctSetSummaryView
    {
        [Key]
        public int Id { get; set; }

        public string AcctSet { get; set; }
        public string AcctSetDesc { get; set; }

        public string GlAcct1 { get; set; }
        public string GlAcct2 { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalJumlah { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalPPn { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalPPh { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalDiscount { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalBayar { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalSisa { get; set; }

        public int TransactionCount { get; set; }
    }

    /// <summary>
    /// Summary view untuk aggregasi by Distribution Code
    /// </summary>
    public class ApLaporanDistSummaryView
    {
        [Key]
        public int Id { get; set; }

        public string DistCode { get; set; }
        public string DistDesc { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalJumlah { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalPPn { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalPPh { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalDiscount { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalBayar { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalSisa { get; set; }

        public int TransactionCount { get; set; }
    }

    /// <summary>
    /// Combined summary untuk GL reconciliation
    /// </summary>
    public class ApLaporanGlReconcileView
    {
        [Key]
        public int Id { get; set; }

        // Account Set Link
        public string AcctSet { get; set; }
        public string AcctSetDesc { get; set; }

        // GL Account yang dimapping dari ApAcct
        public string GlAccount { get; set; } // Bisa Acct1-Acct6 tergantung kebutuhan

        // Distribution
        public string DistCode { get; set; }
        public string DistDesc { get; set; }

        // Aggregated Amount untuk GL
        [Column(TypeName = "decimal(18,4)")]
        public decimal DebitAmount { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CreditAmount { get; set; }

        // Transaction Count
        public int TransactionCount { get; set; }

        // Currency
        public string Currency { get; set; }

        // For tracking
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
