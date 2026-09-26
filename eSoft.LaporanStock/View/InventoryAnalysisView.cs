using System;
using System.Collections.Generic;
using System.Linq;

namespace eSoft.LaporanStock.View
{
    public class InventoryAnalysisReport
    {
        public DateTime AsOfDate { get; set; }
        public int AgingThresholdDays { get; set; } = 180;
        public List<InventoryAnalysisRow> Rows { get; set; } = new();

        public decimal TotalStockValue => Rows.Sum(x => x.StockValue);
        public decimal DeadStockValue => Rows.Where(x => x.IsDeadStock).Sum(x => x.StockValue);
        public decimal SlowMovingValue => Rows.Where(x => x.IsSlowMoving).Sum(x => x.StockValue);
        public decimal TotalSoRemainingValue => Rows.Sum(x => x.SoRemainingValue);
        public decimal TotalPoRemainingValue => Rows.Sum(x => x.PoRemainingValue);
        public decimal TotalStockQty => Rows.Sum(x => x.StockQty);
        public int DeadStockCount => Rows.Count(x => x.IsDeadStock);
        public int HighRiskCount => Rows.Count(x => x.Status == "Risiko Tinggi");

        public IEnumerable<InventoryAnalysisRow> TopValueRows => Rows
            .OrderByDescending(x => x.StockValue)
            .Take(10);

        public IEnumerable<InventoryAnalysisRow> TopAgingRows => Rows
            .Where(x => x.StockQty > 0)
            .OrderByDescending(x => x.DaysSinceLastOut ?? int.MaxValue)
            .ThenByDescending(x => x.StockValue)
            .Take(10);

        public string ExecutiveSummary
        {
            get
            {
                if (!Rows.Any())
                    return "Tidak terdapat data persediaan untuk dianalisis.";

                var deadPercentage = TotalStockValue == 0 ? 0 : DeadStockValue / TotalStockValue * 100;
                return $"Per tanggal {AsOfDate:dd-MM-yyyy}, total nilai persediaan tercatat sebesar {TotalStockValue:N0}. " +
                       $"Sebanyak {DeadStockCount:N0} item dikategorikan sebagai persediaan dengan pergerakan rendah atau tidak bergerak " +
                       $"lebih dari {AgingThresholdDays} hari, dengan nilai {DeadStockValue:N0} ({deadPercentage:N1}%). " +
                       $"Sisa kebutuhan SO tercatat {Rows.Sum(x => x.SoRemainingQty):N2}, sedangkan PO yang masih outstanding sebesar {Rows.Sum(x => x.PoRemainingQty):N2}.";
            }
        }
    }

    public class InventoryAnalysisRow
    {
        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public string Satuan { get; set; }
        public string Divisi { get; set; }
        public string Category { get; set; }
        public string JenisItem { get; set; }
        public string Lokasi { get; set; }
        public decimal StockQty { get; set; }
        public decimal UnitCost { get; set; }
        public decimal StockValue => StockQty * UnitCost;
        public DateTime? LastOutDate { get; set; }
        public int? DaysSinceLastOut { get; set; }
        public int AgingThresholdDays { get; set; } = 180;
        public decimal SoRemainingQty { get; set; }
        public decimal SoRemainingValue => SoRemainingQty * UnitCost;
        public decimal PoRemainingQty { get; set; }
        public decimal PoRemainingValue => PoRemainingQty * UnitCost;
        public decimal ProjectedQtyAfterPo => StockQty + PoRemainingQty;
        public string Status { get; set; }
        public string Recommendation { get; set; }

        public bool IsSparePart => string.Equals(JenisItem, "Sparepart", StringComparison.OrdinalIgnoreCase);
        public bool IsDeadStock => StockQty > 0 && (!LastOutDate.HasValue || (DaysSinceLastOut ?? 0) >= AgingThresholdDays);
        public bool IsSlowMoving => StockQty > 0 && DaysSinceLastOut.HasValue && DaysSinceLastOut.Value >= 90;
    }

    public static class InventoryAnalysisClassification
    {
        public static string Classify(string category, string divisi, string acctSet, string namaItem)
        {
            var source = string.Join(" ", category, divisi, acctSet, namaItem).ToUpperInvariant();
            return source.Contains("SPARE") || source.Contains("SUKU CADANG") ? "Sparepart" : "Umum";
        }

        public static string GetStatus(InventoryAnalysisRow row, int agingThresholdDays)
        {
            if (row.StockQty <= 0)
                return "Tidak ada stock";
            if (row.SoRemainingQty > row.StockQty && row.PoRemainingQty <= row.SoRemainingQty - row.StockQty)
                return "Prioritas SO";
            if ((!row.LastOutDate.HasValue || row.DaysSinceLastOut >= agingThresholdDays) && row.StockValue > 0)
                return row.IsSparePart ? "Sparepart utilisasi rendah" : "Dead Stock";
            if (row.PoRemainingQty > row.SoRemainingQty + row.StockQty)
                return "Risiko Over Stock";
            if (row.DaysSinceLastOut >= 90)
                return "Slow Moving";
            return "Normal";
        }

        public static string GetRecommendation(string status, bool isSparePart)
        {
            return status switch
            {
                "Prioritas SO" => "Prioritaskan pemenuhan SO dan evaluasi kekurangan pembelian.",
                "Dead Stock" => "Evaluasi promosi, transfer gudang, clearance, atau penghentian pembelian.",
                "Sparepart utilisasi rendah" => "Validasi kebutuhan maintenance sebelum melakukan clearance atau penghapusan.",
                "Risiko Over Stock" => "Tunda pembelian berikutnya dan evaluasi jumlah PO outstanding.",
                "Slow Moving" => "Pantau pemakaian dan batasi pembelian sampai pergerakan membaik.",
                "Tidak ada stock" => "Tidak ada tindakan stock; evaluasi kebutuhan SO atau PO jika ada permintaan.",
                _ => isSparePart ? "Pantau kebutuhan operasional dan jadwal maintenance." : "Persediaan dalam kondisi normal."
            };
        }
    }
}
