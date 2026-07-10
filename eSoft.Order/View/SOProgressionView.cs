using System;
using System.Collections.Generic;

namespace eSoft.Order.View
{
    /// <summary>
    /// Model untuk menampilkan progression status SO seiring kedatangan PI/Projects
    /// </summary>
    public class SOProgressionView
    {
        /// <summary>
        /// Daftar SO dengan progression statusnya
        /// </summary>
        public List<SOProgressionRow> Rows { get; set; } = new();

        /// <summary>
        /// Daftar PI/Projects dalam urutan, untuk header kolom
        /// </summary>
        public List<PIInfo> PIsInOrder { get; set; } = new();
    }

    /// <summary>
    /// Info tentang satu PI/Project
    /// </summary>
    public class PIInfo
    {
        /// <summary>
        /// No Project/PI
        /// </summary>
        public string NoPrj { get; set; }

        /// <summary>
        /// Vendor/Supplier untuk PI ini (jika ada multiple vendors, ambil yang pertama)
        /// </summary>
        public string NamaVendor { get; set; }

        /// <summary>
        /// Total PO untuk PI ini
        /// </summary>
        public decimal TotalQty { get; set; }

        /// <summary>
        /// Tanggal PO (untuk sorting chronologically)
        /// </summary>
        public DateTime Tanggal { get; set; }

        /// <summary>
        /// Index PI dalam urutan (0, 1, 2, ...)
        /// </summary>
        public int PiIndex { get; set; }
    }

    /// <summary>
    /// Satu baris SO dengan status progressionnya dalam berbagai PI scenarios
    /// </summary>
    public class SOProgressionRow
    {
        /// <summary>
        /// No SO
        /// </summary>
        public string NoSO { get; set; }

        /// <summary>
        /// Nama Customer
        /// </summary>
        public string NamaCustomer { get; set; }

        /// <summary>
        /// Tanggal SO
        /// </summary>
        public DateTime TanggalSO { get; set; }

        /// <summary>
        /// Status saat ini (Lengkap / Sebagian Kurang / Banyak Kurang)
        /// </summary>
        public string StatusSekarang { get; set; }

        /// <summary>
        /// Status item saat ini - daftar item yang kurang
        /// </summary>
        public List<ItemStatus> ItemStatusSekarang { get; set; } = new();

        /// <summary>
        /// Progression status untuk setiap PI
        /// Key: NoPrj, Value: Status SetelahPI ini
        /// </summary>
        public Dictionary<string, PIProgressionStatus> ProgressionPerPI { get; set; } = new();
    }

    /// <summary>
    /// Status item saat ini atau setelah PI tertentu
    /// </summary>
    public class ItemStatus
    {
        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public decimal QtyOrder { get; set; }
        public decimal QtyAvailable { get; set; }

        /// <summary>
        /// Qty yang kurang (0 jika cukup)
        /// </summary>
        public decimal QtyKurang { get; set; }

        /// <summary>
        /// True jika item sudah lengkap
        /// </summary>
        public bool IsComplete => QtyKurang <= 0;
    }

    /// <summary>
    /// Status progression SO setelah PI tertentu datang
    /// </summary>
    public class PIProgressionStatus
    {
        /// <summary>
        /// No PI/Project
        /// </summary>
        public string NoPrj { get; set; }

        /// <summary>
        /// Apakah SO lengkap setelah PI ini
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Item yang bertransisi dari incomplete menjadi complete
        /// </summary>
        public List<string> NewlyCompletedItems { get; set; } = new();

        /// <summary>
        /// Item yang masih belum lengkap
        /// </summary>
        public List<string> StillMissingItems { get; set; } = new();

        /// <summary>
        /// Summary status: "Lengkap", "Masih Kurang: ItemA, ItemB", dst
        /// </summary>
        public string StatusSummary { get; set; }
    }
}
