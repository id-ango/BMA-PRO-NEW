using eSoft.LaporanStock.View;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Accounting.Services;

public static class InventoryAnalysisPdfExporter
{
    public static byte[] Generate(InventoryAnalysisReport report, bool fullReport)
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(24);
                page.DefaultTextStyle(style => style.FontSize(fullReport ? 6 : 9).FontColor(Colors.Grey.Darken4));
                page.Header().Column(column =>
                {
                    column.Item().Text(fullReport ? "Laporan Lengkap Analisa Stock, SO & PO" : "Ringkasan Analisa Stock, SO & PO")
                        .FontSize(18).Bold().FontColor(Colors.Blue.Darken3);
                    column.Item().Text($"Per tanggal {report.AsOfDate:dd-MM-yyyy} | Dibuat {DateTime.Now:dd-MM-yyyy HH:mm}")
                        .FontSize(8).FontColor(Colors.Grey.Darken2);
                });
                page.Content().PaddingVertical(12).Column(column =>
                {
                    column.Spacing(10);
                    if (fullReport)
                    {
                        AddFullReport(column, report);
                    }
                    else
                    {
                        AddSummary(column, report);
                        AddTopItems(column, report);
                    }
                });
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Halaman ");
                    text.CurrentPageNumber();
                    text.Span(" dari ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static void AddSummary(ColumnDescriptor column, InventoryAnalysisReport report)
    {
        column.Item().Text("Ringkasan Eksekutif").FontSize(13).Bold().FontColor(Colors.Blue.Darken3);
        column.Item().Text(report.ExecutiveSummary);
        column.Item().Row(row =>
        {
            AddKpi(row, "Nilai Stock", report.TotalStockValue);
            AddKpi(row, "Dead Stock", report.DeadStockValue);
            AddKpi(row, "Slow Moving", report.SlowMovingValue);
            AddKpi(row, "Sisa SO", report.TotalSoRemainingValue);
            AddKpi(row, "PO Outstanding", report.TotalPoRemainingValue);
            AddKpi(row, "Saran PO", report.TotalRecommendedPoValue);
        });
        column.Item().Text($"Parameter: Histori penjualan {report.SalesHistoryMonths} bulan | Coverage {report.CoverageMonths} bulan | Lead time {report.LeadTimeMonths} bulan | Safety stock {report.SafetyStockMonths} bulan | Aging {report.AgingThresholdDays} hari");
        column.Item().Background(Colors.Blue.Lighten5).Padding(7).Text("Adjustment netto adalah jumlah bersih penambahan/pengurangan stock sampai tanggal analisa. Nilai ini hanya informasi, tidak menjadi input demand penjualan maupun dasar perhitungan rekomendasi PO.");
    }

    private static void AddKpi(RowDescriptor row, string label, decimal value)
    {
        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Column(column =>
        {
            column.Item().Text(label).FontSize(7).FontColor(Colors.Grey.Darken2);
            column.Item().Text(value.ToString("N0")).Bold();
        });
    }

    private static void AddFullReport(ColumnDescriptor column, InventoryAnalysisReport report)
    {
        var groups = GetGroupedRows(report);

        column.Item().PageBreak();
        column.Item().Text("Daftar Isi Bab Analisa Persediaan").FontSize(13).Bold().FontColor(Colors.Blue.Darken3);
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1);
                columns.RelativeColumn(2.5f);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1.5f);
                columns.RelativeColumn(5);
            });
            Header(table, "Bab", "Kategori Status", "Jumlah Item", "Total Nilai Stock", "Keterangan / Fokus");
            for (var index = 0; index < groups.Count; index++)
            {
                var group = groups[index];
                var sectionName = GetSectionName(index);
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4)
                    .Text(text => text.SectionLink(sectionName, $"Bab {index + 1}").FontColor(Colors.Blue.Darken3).Bold());
                Cells(table, group.Key, group.Rows.Count.ToString("N0"), group.Rows.Sum(item => item.StockValue).ToString("N0"), group.Description);
            }
        });

        foreach (var (group, index) in groups.Select((group, index) => (group, index)))
        {
            var sectionName = GetSectionName(index);
            column.Item().PageBreak();
            column.Item().Section(sectionName).Column(chapter =>
            {
                chapter.Spacing(6);
                chapter.Item().Background(Colors.Blue.Darken3).Padding(7).Text($"Bab {index + 1}: {group.Key} ({group.Rows.Count:N0} item)")
                    .FontSize(12).Bold().FontColor(Colors.White);
                chapter.Item().Text(group.Description).FontSize(8).FontColor(Colors.Grey.Darken2);
                chapter.Item().Text($"Total Nilai Stock: {group.Rows.Sum(item => item.StockValue):N0}").Bold();
                AddDetailTable(chapter, group.Rows);
            });
        }
    }

    private static void AddDetailTable(ColumnDescriptor column, IReadOnlyList<InventoryAnalysisRow> rows)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2.4f); columns.RelativeColumn(1); columns.RelativeColumn(1);
                columns.RelativeColumn(1.2f); columns.RelativeColumn(1); columns.RelativeColumn(1);
                columns.RelativeColumn(1); columns.RelativeColumn(1); columns.RelativeColumn(1);
                columns.RelativeColumn(1); columns.RelativeColumn(1); columns.RelativeColumn(1);
                columns.RelativeColumn(1.6f); columns.RelativeColumn(3);
            });
            Header(table, "Item / Nama", "Jenis", "Stock", "Nilai", "Terakhir Keluar", "Umur", "Sisa SO", "Sisa PO", "Jual 12 Bln", "Rata2/Bln", "Rekom PO", "Adj. Netto", "Status", "Alasan Analisa");
            foreach (var item in rows)
            {
                Cells(table,
                    $"{item.ItemCode} / {item.NamaItem}", item.JenisItem, item.StockQty.ToString("N2"), item.StockValue.ToString("N0"),
                    item.LastOutDate?.ToString("dd-MM-yyyy") ?? "-", item.DaysSinceLastOut?.ToString() ?? "-",
                    item.SoRemainingQty.ToString("N2"), item.PoRemainingQty.ToString("N2"), item.SalesQty12Months.ToString("N2"),
                    item.AverageMonthlySales.ToString("N2"), item.RecommendedPoQty.ToString("N2"), FormatAdjustment(item.NetAdjustmentQty),
                    item.Status, item.DemandAnalysisReason);
            }
        });
    }

    private static List<BookGroup> GetGroupedRows(InventoryAnalysisReport report)
    {
        var statusOrder = new[]
        {
            ("Alokasi SO (Siap Kirim)", "Item dengan saldo stock yang telah terikat pesanan pelanggan (SO aktif). Fokus: Segera lakukan pengiriman (Surat Jalan/DO) ke customer. Jangan clearance."),
            ("Sebagian Alokasi SO", "Item di mana sebagian stock terikat pesanan pelanggan (SO), sedangkan sisa unitnya belum bergerak. Fokus: Kirim bagian yang ada SO, dan evaluasi sisa unitnya."),
            ("Dead Stock", "Item dengan saldo stock tanpa pesanan SO dan tidak ada pergerakan keluar melebihi batas analisa. Fokus: Evaluasi promosi, transfer cabang, atau clearance."),
            ("Sparepart utilisasi rendah", "Item suku cadang/sparepart dengan pergerakan rendah. Fokus: Validasi kebutuhan preventif maintenance mesin sebelum tindakan clearance."),
            ("Prioritas SO", "Item dengan kebutuhan SO outstanding yang belum tertutup stock maupun PO. Fokus: Segera lakukan pengadaan atau percepat pengiriman."),
            ("Rekomendasi PO", "Item dengan pergerakan historis penjualan yang membutuhkan penambahan PO untuk menjamin target persediaan dan waktu tunggu supplier. Fokus: Buat PO rekomendasi."),
            ("Slow Moving", "Item dengan umur pengeluaran terakhir melampaui 90 hari namun belum mencapai batas dead stock. Fokus: Pantau sirkulasi dan batasi re-order."),
            ("PO Tanpa Kebutuhan SO", "Item dengan PO aktif melampaui kebutuhan SO saat ini tetapi item masih aktif bergerak. Fokus: Monitor serapan pasar."),
            ("Risiko Over Stock", "Item dengan PO berlebih di mana pergerakan stock lambat atau tidak ada pengeluaran terbaru. Fokus: Pertimbangkan penundaan kedatangan PO."),
            ("Normal", "Item persediaan dengan pergerakan dan proporsi stock dalam batas wajar. Fokus: Pemantauan rutin operasional."),
            ("Tidak ada stock", "Item tanpa saldo fisik saat ini. Fokus: Tinjau kebutuhan bila ada permintaan aktif.")
        };

        var groups = new List<BookGroup>();
        foreach (var (status, description) in statusOrder)
        {
            var rows = report.Rows.Where(row => row.Status == status).OrderByDescending(row => row.StockValue).ToList();
            if (rows.Count > 0)
                groups.Add(new BookGroup(status, description, rows));
        }

        var handledStatuses = statusOrder.Select(item => item.Item1).ToHashSet();
        groups.AddRange(report.Rows.Where(row => !handledStatuses.Contains(row.Status))
            .GroupBy(row => row.Status)
            .Select(group => new BookGroup(group.Key, $"Kategori status: {group.Key}", group.OrderByDescending(row => row.StockValue).ToList())));
        return groups;
    }

    private static string GetSectionName(int index) => $"inventory-analysis-chapter-{index + 1}";

    private sealed record BookGroup(string Key, string Description, IReadOnlyList<InventoryAnalysisRow> Rows);

    private static void AddTopItems(ColumnDescriptor column, InventoryAnalysisReport report)
    {
        column.Item().Text("Top 30 Nilai Stock").FontSize(12).Bold().FontColor(Colors.Blue.Darken3);
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2); columns.RelativeColumn(3); columns.RelativeColumn(1);
                columns.RelativeColumn(1); columns.RelativeColumn(2); columns.RelativeColumn(2);
            });
            Header(table, "Kode Item", "Nama Item", "Jenis", "Stock", "Nilai Stock", "Status");
            foreach (var item in report.TopValueRows)
                Cells(table, item.ItemCode, item.NamaItem, item.JenisItem, item.StockQty.ToString("N2"), item.StockValue.ToString("N0"), item.Status);
        });

        column.Item().PageBreak();
        column.Item().Text("Dasar Analisa & Alasan Rekomendasi PO").FontSize(12).Bold().FontColor(Colors.Blue.Darken3);
        var recommendations = report.Rows.Where(row => row.HasPurchaseRecommendation).OrderByDescending(row => row.RecommendedPoValue).Take(30).ToList();
        if (recommendations.Count == 0)
        {
            column.Item().Text("Tidak ada item yang direkomendasikan untuk PO berdasarkan parameter dan histori penjualan yang dipilih.");
            return;
        }

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1.5f); columns.RelativeColumn(3); columns.RelativeColumn(1);
                columns.RelativeColumn(1); columns.RelativeColumn(1); columns.RelativeColumn(1);
                columns.RelativeColumn(1);
                columns.RelativeColumn(5);
            });
            Header(table, "Kode Item", "Nama Item", "Stock Saat Ini", "Jual 12 Bln", "Rata-rata/Bln", "Rekom PO", "Adj. Netto", "Alasan");
            foreach (var item in recommendations)
                Cells(table, item.ItemCode, item.NamaItem, item.StockQty.ToString("N2"), item.SalesQty12Months.ToString("N2"), item.AverageMonthlySales.ToString("N2"), item.RecommendedPoQty.ToString("N0"), FormatAdjustment(item.NetAdjustmentQty), item.DemandAnalysisReason);
        });
    }

    private static void AddDetails(ColumnDescriptor column, InventoryAnalysisReport report)
    {
        column.Item().Text("Detail Analisa Persediaan").FontSize(13).Bold().FontColor(Colors.Blue.Darken3);
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2.4f); columns.RelativeColumn(1); columns.RelativeColumn(1);
                columns.RelativeColumn(1.2f); columns.RelativeColumn(1); columns.RelativeColumn(1);
                columns.RelativeColumn(1); columns.RelativeColumn(1); columns.RelativeColumn(1);
                columns.RelativeColumn(1); columns.RelativeColumn(1); columns.RelativeColumn(1);
                columns.RelativeColumn(1.6f); columns.RelativeColumn(3);
            });
            Header(table, "Item / Nama", "Jenis", "Stock", "Nilai", "Terakhir Keluar", "Umur", "Sisa SO", "Sisa PO", "Jual 12 Bln", "Rata2/Bln", "Rekom PO", "Adj. Netto", "Status", "Alasan Analisa");
            foreach (var item in report.Rows.OrderBy(row => row.Status).ThenByDescending(row => row.StockValue))
            {
                Cells(table,
                    $"{item.ItemCode} / {item.NamaItem}", item.JenisItem, item.StockQty.ToString("N2"), item.StockValue.ToString("N0"),
                    item.LastOutDate?.ToString("dd-MM-yyyy") ?? "-", item.DaysSinceLastOut?.ToString() ?? "-",
                    item.SoRemainingQty.ToString("N2"), item.PoRemainingQty.ToString("N2"), item.SalesQty12Months.ToString("N2"),
                    item.AverageMonthlySales.ToString("N2"), item.RecommendedPoQty.ToString("N2"), FormatAdjustment(item.NetAdjustmentQty),
                    item.Status, item.DemandAnalysisReason);
            }
        });
    }

    private static void Header(TableDescriptor table, params string[] labels)
    {
        foreach (var label in labels)
            table.Cell().Background(Colors.Blue.Darken3).Padding(3).Text(label).FontColor(Colors.White).Bold();
    }

    private static void Cells(TableDescriptor table, params string[] values)
    {
        foreach (var value in values)
            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(value ?? string.Empty);
    }

    private static string FormatAdjustment(decimal value) => value > 0 ? $"+{value:N2}" : value.ToString("N2");
}
