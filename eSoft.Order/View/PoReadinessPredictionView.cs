using System;
using System.Collections.Generic;
using System.Linq;

namespace eSoft.Order.View
{
    /// <summary>
    /// Model untuk analisa prediksi: jika PO tertentu datang, SO mana saja yang bisa ready
    /// </summary>
    public class PoReadinessPredictionView
    {
        /// <summary>
        /// Daftar skenario prediksi per PO aktif
        /// </summary>
        public List<PoPredictionScenario> Scenarios { get; set; } = new();

        /// <summary>
        /// Summary keseluruhan untuk konteks
        /// </summary>
        public PredictionSummary Summary { get; set; } = new();
    }

    /// <summary>
    /// Skenario prediksi untuk satu PO: jika datang, SO mana ready
    /// </summary>
    public class PoPredictionScenario
    {
        /// <summary>
        /// No PO
        /// </summary>
        public string NoLpb { get; set; }

        /// <summary>
        /// Tanggal PO
        /// </summary>
        public DateTime Tanggal { get; set; }

        /// <summary>
        /// No Project/PI
        /// </summary>
        public string NoPrj { get; set; }

        /// <summary>
        /// Supplier/Vendor
        /// </summary>
        public string NamaSupplier { get; set; }

        /// <summary>
        /// Keterangan PO
        /// </summary>
        public string Keterangan { get; set; }

        /// <summary>
        /// Detail item dalam PO ini
        /// </summary>
        public List<PoPredictionItem> Items { get; set; } = new();

        /// <summary>
        /// Daftar SO yang akan menjadi ready jika PO ini datang
        /// </summary>
        public List<SOReadinessResult> ReadySalesOrders { get; set; } = new();

        /// <summary>
        /// SO yang masih pending (incomplete) bahkan dengan PO ini
        /// </summary>
        public List<SOReadinessResult> PendingSalesOrders { get; set; } = new();

        /// <summary>
        /// Jumlah SO yang akan ready
        /// </summary>
        public int CountReadyAfter => ReadySalesOrders.Count;

        /// <summary>
        /// Persentase SO ready setelah PO datang (dari total SO)
        /// </summary>
        public decimal PercentageReady { get; set; }
    }

    /// <summary>
    /// Item detail dalam PO untuk prediksi
    /// </summary>
    public class PoPredictionItem
    {
        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public decimal Qty { get; set; }
        public string Satuan { get; set; }

        /// <summary>
        /// Kebutuhan saat ini dari SO yang masih incomplete
        /// </summary>
        public decimal NeededQty { get; set; }

        /// <summary>
        /// Apakah PO ini cukup untuk memenuhi kebutuhan item ini
        /// </summary>
        public bool IsSufficientForItem { get; set; }
    }

    /// <summary>
    /// Hasil readiness SO dalam konteks PO tertentu
    /// </summary>
    public class SOReadinessResult
    {
        public string NoSO { get; set; }
        public string NamaCustomer { get; set; }
        public DateTime TanggalSO { get; set; }
        public string NoPrj { get; set; }
        public string Keterangan { get; set; }

        /// <summary>
        /// Item apa saja yang missing/incomplete dalam SO ini
        /// </summary>
        public List<string> MissingItems { get; set; } = new();

        /// <summary>
        /// Item yang akan completed jika PO datang
        /// </summary>
        public List<string> WillBeCompletedItems { get; set; } = new();

        /// <summary>
        /// Reason SO masih pending: item apa yang kurang
        /// </summary>
        public string ReasonIfStillPending { get; set; }
    }

    /// <summary>
    /// Summary statistik prediksi
    /// </summary>
    public class PredictionSummary
    {
        /// <summary>
        /// Total SO aktif
        /// </summary>
        public int TotalSalesOrders { get; set; }

        /// <summary>
        /// SO yang sudah ready sebelum ada PO apapun
        /// </summary>
        public int ReadyWithoutPO { get; set; }

        /// <summary>
        /// Daftar PO aktif dengan ranking impact
        /// </summary>
        public List<POImpactRanking> POImpactRanking { get; set; } = new();

        /// <summary>
        /// Rata-rata SO yang akan ready per PO
        /// </summary>
        public decimal AverageSOReadyPerPO { get; set; }

        /// <summary>
        /// PO dengan impact terbesar (paling banyak SO ready)
        /// </summary>
        public string MostImpactfulPO { get; set; }

        /// <summary>
        /// Item yang paling critical (banyak SO menunggu)
        /// </summary>
        public List<CriticalItemAnalysis> CriticalItems { get; set; } = new();
    }

    /// <summary>
    /// Ranking impact PO terhadap jumlah SO ready
    /// </summary>
    public class POImpactRanking
    {
        public string NoLpb { get; set; }
        public string NoPrj { get; set; }
        public int ImpactCount { get; set; }
        public int TotalReadyAfterPO { get; set; }
        public decimal ImpactPercentage { get; set; }
        public int Rank { get; set; }
    }

    /// <summary>
    /// Analisa item yang critical (banyak SO menunggu)
    /// </summary>
    public class CriticalItemAnalysis
    {
        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public int CountSOWaiting { get; set; }
        public decimal TotalQtyWaiting { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal TotalPOPlanned { get; set; }
    }
}
