#nullable enable

using System.Globalization;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Accounting.Services;

public sealed record SalesChartReport(
    DateTime AsOfDate,
    IReadOnlyList<int> Years,
    IReadOnlyDictionary<(int Year, int Month), decimal> Sales,
    IReadOnlyDictionary<(int Year, int Month), decimal> Purchases,
    SalesGroupResult? GroupAnalysis = null,
    decimal? GroupChartTotal = null);

public static class SalesChartPdfExporter
{
    private static readonly CultureInfo IndonesianCulture = CultureInfo.GetCultureInfo("id-ID");
    private static readonly string[] MonthNames = { "Jan", "Feb", "Mar", "Apr", "Mei", "Jun", "Jul", "Agu", "Sep", "Okt", "Nov", "Des" };
    private static readonly string[] YearColors = { "#2563eb", "#10b981", "#f59e0b", "#8b5cf6", "#ef4444", "#0891b2" };

    public static byte[] Generate(SalesChartReport report)
    {
        var salesChart = CreateChart(report, false);
        var combinedChart = CreateChart(report, true);
        var differences = report.Years.SelectMany(year => Enumerable.Range(1, 12)
            .Select(month => (Year: year, Month: month)))
            .ToDictionary(key => key, key => report.Sales.GetValueOrDefault(key) - report.Purchases.GetValueOrDefault(key));
        var differenceChart = CreateChart(report, false, differences);

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(24);
                page.Content().AlignCenter().AlignMiddle().Column(column =>
                {
                    column.Spacing(12);
                    column.Item().AlignCenter().Text("Laporan Grafik Penjualan dan Pembelian").FontSize(24).Bold().FontColor(Colors.Blue.Darken3);
                    column.Item().AlignCenter().Text($"Tahun berjalan hingga {report.AsOfDate.ToString("dd MMMM yyyy", IndonesianCulture)}").FontSize(12);
                    column.Item().AlignCenter().Text($"Tahun: {string.Join(", ", report.Years)}").FontSize(11);
                });
            });
            AddChartPage(document, report, "Grafik Penjualan Bersih", salesChart, "Penjualan bersih (IDR); retur mengurangi nilai transaksi.");
            AddChartPage(document, report, "Perbandingan Penjualan dan Pembelian", combinedChart, "Penjualan dan pembelian bersih (IDR); retur mengurangi nilai transaksi.");
            AddChartPage(document, report, "Selisih Penjualan dan Pembelian", differenceChart, "Penjualan bersih dikurangi pembelian bersih per bulan (IDR); selisih ini bukan laba.");
            document.Page(page =>
            {
                ConfigurePage(page, report, "Rincian Bulanan");
                page.Content().PaddingVertical(8).Column(column =>
                {
                    column.Spacing(8);
                    column.Item().Text("Penjualan bersih (IDR)").FontSize(11).Bold();
                    AddTable(column, report, report.Sales);
                    column.Item().Text("Pembelian bersih (IDR)").FontSize(11).Bold();
                    AddTable(column, report, report.Purchases);
                    column.Item().Text("Tahun berjalan sampai tanggal laporan; tahun pembanding setahun penuh. Tanda - menunjukkan bulan yang belum berjalan.").FontSize(8);
                    column.Item().Text("Pembelian menggunakan nilai rupiah tersimpan jika ada kurs, atau jumlah transaksi jika tidak ada kurs.").FontSize(8);
                });
            });
            document.Page(page =>
            {
                ConfigurePage(page, report, "Rincian Selisih Bulanan");
                page.Content().PaddingVertical(8).Column(column =>
                {
                    column.Spacing(8);
                    column.Item().Text("Penjualan bersih dikurangi pembelian bersih (IDR); selisih ini bukan laba.").FontSize(11).Bold();
                    AddTable(column, report, differences);
                    column.Item().Text("Tahun berjalan sampai tanggal laporan; tahun pembanding setahun penuh. Tanda - menunjukkan bulan yang belum berjalan.").FontSize(8);
                });
            });
            if (report.GroupAnalysis is not null)
                AddGroupAnalysisPage(document, report, report.GroupAnalysis);
        }).GeneratePdf();
    }

    private static void AddGroupAnalysisPage(IDocumentContainer document, SalesChartReport report, SalesGroupResult analysis)
    {
        document.Page(page =>
        {
            ConfigurePage(page, report, "Analisis Penjualan per Group");
            page.Content().PaddingVertical(8).Column(column =>
            {
                column.Spacing(8);
                column.Item().Text($"Tahun {analysis.Year} hingga {analysis.Through.ToString("dd MMMM yyyy", IndonesianCulture)}. Penjualan dikurangi retur.").FontSize(9);
                if (report.GroupChartTotal.HasValue && report.GroupChartTotal.Value != analysis.HeaderTotal)
                    column.Item().Text("Total transaksi berubah sejak grafik dimuat; refresh data dan muat ulang analisis untuk membandingkan periode yang sama.").FontColor(Colors.Red.Darken2);
                column.Item().Text("Peringkat berdasarkan nilai penjualan bersih (IDR)").FontSize(11).Bold();
                if (analysis.Values.Count == 0)
                    column.Item().Text("Tidak ada transaksi dalam periode ini.");
                else
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns => { columns.RelativeColumn(3); columns.RelativeColumn(2); });
                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCell).Text("Group");
                            header.Cell().Element(HeaderCell).AlignRight().Text("Nilai bersih (IDR)");
                        });
                        foreach (var value in analysis.Values)
                        {
                            table.Cell().Element(BodyCell).Text(value.Group);
                            table.Cell().Element(BodyCell).AlignRight().Text(DetailedNumber(value.Amount));
                        }
                        table.Cell().Element(HeaderCell).Text("Total");
                        table.Cell().Element(HeaderCell).AlignRight().Text(DetailedNumber(analysis.HeaderTotal));
                    });
                column.Item().Text("Peringkat berdasarkan jumlah bersih per satuan").FontSize(11).Bold();
                if (analysis.Quantities.Count == 0)
                    column.Item().Text("Tidak ada detail jumlah dengan satuan dalam periode ini.");
                else
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns => { columns.RelativeColumn(2); columns.RelativeColumn(3); columns.RelativeColumn(2); });
                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCell).Text("Satuan");
                            header.Cell().Element(HeaderCell).Text("Group");
                            header.Cell().Element(HeaderCell).AlignRight().Text("Jumlah bersih");
                        });
                        foreach (var quantity in analysis.Quantities)
                        {
                            table.Cell().Element(BodyCell).Text(quantity.Unit);
                            table.Cell().Element(BodyCell).Text(quantity.Group);
                            table.Cell().Element(BodyCell).AlignRight().Text(DetailedNumber(quantity.Quantity));
                        }
                    });
                if (analysis.HasUnknownUnitQuantities)
                    column.Item().Text("Jumlah pada detail tanpa satuan tidak dimasukkan dalam peringkat jumlah. Periksa satuan pada transaksi terkait.").FontColor(Colors.Red.Darken2).FontSize(8);
                column.Item().Text($"Total faktur dialokasikan sesuai proporsi nilai detail; retur dikurangi. Group mengikuti master item saat ini. Satuan berbeda tidak dijumlahkan. Faktur dengan detail kosong atau total detail nol: {analysis.UnallocatedDocuments}.").FontSize(8);
            });
        });
    }

    private static void ConfigurePage(PageDescriptor page, SalesChartReport report, string title)
    {
        page.Size(PageSizes.A4.Landscape());
        page.Margin(24);
        page.DefaultTextStyle(style => style.FontSize(9).FontColor(Colors.Grey.Darken4));
        page.Header().Column(column =>
        {
            column.Item().Text(title).FontSize(16).Bold().FontColor(Colors.Blue.Darken3);
            column.Item().Text($"Tahun berjalan hingga {report.AsOfDate.ToString("dd MMMM yyyy", IndonesianCulture)} | Tahun: {string.Join(", ", report.Years)}").FontSize(8);
        });
        page.Footer().AlignCenter().Text(text =>
        {
            text.Span("Halaman ");
            text.CurrentPageNumber();
            text.Span(" dari ");
            text.TotalPages();
        });
    }

    private static void AddChartPage(IDocumentContainer document, SalesChartReport report, string title, string svg, string description)
    {
        document.Page(page =>
        {
            ConfigurePage(page, report, title);
            page.Content().PaddingVertical(8).Column(column =>
            {
                column.Item().Text(description).FontSize(9);
                column.Item().PaddingTop(8).Svg(svg);
                column.Item().PaddingTop(6).Text("Bulan berikutnya pada tahun berjalan belum ditampilkan; nilai lengkap dan total tersedia pada tabel rincian.").FontSize(8);
            });
        });
    }

    private static void AddTable(ColumnDescriptor column, SalesChartReport report, IReadOnlyDictionary<(int Year, int Month), decimal> values)
    {
        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1);
                foreach (var _ in report.Years)
                    columns.RelativeColumn(1.7f);
            });
            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("Bulan");
                foreach (var year in report.Years)
                    header.Cell().Element(HeaderCell).AlignRight().Text(year.ToString(CultureInfo.InvariantCulture));
            });
            for (var month = 1; month <= 12; month++)
            {
                table.Cell().Element(BodyCell).Text(MonthNames[month - 1]);
                foreach (var year in report.Years)
                {
                    var value = GetValue(report, values, year, month);
                    table.Cell().Element(BodyCell).AlignRight().Text(value.HasValue ? Money(value.Value) : "-");
                }
            }
            table.Cell().Element(HeaderCell).Text("Total");
            foreach (var year in report.Years)
            {
                var total = Enumerable.Range(1, 12).Sum(month => GetValue(report, values, year, month) ?? 0);
                table.Cell().Element(HeaderCell).AlignRight().Text(Money(total));
            }
        });
    }

    private static IContainer HeaderCell(IContainer container) => container.Background(Colors.Grey.Lighten3).PaddingVertical(2).PaddingHorizontal(4);
    private static IContainer BodyCell(IContainer container) => container.BorderBottom(.3f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2).PaddingHorizontal(4);

    private static string CreateChart(SalesChartReport report, bool combined, IReadOnlyDictionary<(int Year, int Month), decimal>? primaryValues = null)
    {
        primaryValues ??= report.Sales;
        const decimal left = 185, top = 70, width = 995, height = 300;
        var values = report.Years.SelectMany(year => Enumerable.Range(1, 12)
            .SelectMany(month => combined
                ? new[] { GetValue(report, primaryValues, year, month), GetValue(report, report.Purchases, year, month) }
                : new[] { GetValue(report, primaryValues, year, month) }))
            .Where(value => value.HasValue).Select(value => value!.Value);
        var scale = new SalesChartScale(values);
        var svg = new StringBuilder();
        svg.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"1200\" height=\"430\" viewBox=\"0 0 1200 430\">");
        svg.Append("<rect width=\"1200\" height=\"430\" fill=\"white\"/>");
        for (var index = 0; index < report.Years.Count; index++)
        {
            var x = 20 + index * 125;
            svg.Append($"<rect x=\"{x}\" y=\"12\" width=\"15\" height=\"15\" fill=\"{YearColors[report.AsOfDate.Year - report.Years[index]]}\"/>");
            svg.Append($"<text x=\"{x + 21}\" y=\"25\" font-family=\"Arial\" font-size=\"16\" fill=\"#334155\">{report.Years[index]}</text>");
        }
        if (combined)
            svg.Append("<text x=\"20\" y=\"51\" font-family=\"Arial\" font-size=\"15\" fill=\"#334155\">Warna penuh: Penjualan   |   Warna lebih muda: Pembelian</text>");

        foreach (var tick in scale.Ticks)
        {
            var y = top + height * (100 - tick.Position) / 100;
            svg.Append($"<line x1=\"{F(left)}\" y1=\"{F(y)}\" x2=\"{F(left + width)}\" y2=\"{F(y)}\" stroke=\"#e2e8f0\"/>");
            svg.Append($"<text x=\"{F(left - 10)}\" y=\"{F(y + 5)}\" text-anchor=\"end\" font-family=\"Arial\" font-size=\"15\" fill=\"#475569\">{Money(tick.Value)}</text>");
        }
        var zeroY = top + height * (100 - scale.ZeroPosition) / 100;
        svg.Append($"<line x1=\"{F(left)}\" y1=\"{F(zeroY)}\" x2=\"{F(left + width)}\" y2=\"{F(zeroY)}\" stroke=\"#64748b\"/>");
        var monthWidth = width / 12;
        var barCount = report.Years.Count * (combined ? 2 : 1);
        var barWidth = (monthWidth - 10) / barCount;
        for (var month = 1; month <= 12; month++)
        {
            var monthX = left + (month - 1) * monthWidth;
            svg.Append($"<text x=\"{F(monthX + monthWidth / 2)}\" y=\"{F(top + height + 24)}\" text-anchor=\"middle\" font-family=\"Arial\" font-size=\"15\" fill=\"#475569\">{MonthNames[month - 1]}</text>");
            for (var index = 0; index < report.Years.Count; index++)
            {
                var year = report.Years[index];
                var color = YearColors[report.AsOfDate.Year - year];
                var x = monthX + 5 + index * (combined ? 2 : 1) * barWidth;
                AddBar(svg, GetValue(report, primaryValues, year, month), scale, x, barWidth, top, height, color, false);
                if (combined)
                    AddBar(svg, GetValue(report, report.Purchases, year, month), scale, x + barWidth, barWidth, top, height, color, true);
            }
        }
        svg.Append("</svg>");
        return svg.ToString();
    }

    private static void AddBar(StringBuilder svg, decimal? value, SalesChartScale scale, decimal x, decimal width, decimal top, decimal height, string color, bool purchase)
    {
        if (!value.HasValue || value.Value == 0 || scale.Range == 0)
            return;
        var barHeight = Math.Abs(value.Value) / scale.Range * height;
        var zeroY = top + height * (100 - scale.ZeroPosition) / 100;
        var y = value.Value < 0 ? zeroY : zeroY - barHeight;
        svg.Append($"<rect x=\"{F(x)}\" y=\"{F(y)}\" width=\"{F(Math.Max(.5m, width - .8m))}\" height=\"{F(barHeight)}\" fill=\"{color}\"{(purchase ? " fill-opacity=\"0.45\"" : "")}/>");
    }

    private static decimal? GetValue(SalesChartReport report, IReadOnlyDictionary<(int Year, int Month), decimal> values, int year, int month)
    {
        if (year == report.AsOfDate.Year && month > report.AsOfDate.Month)
            return null;
        return values.TryGetValue((year, month), out var value) ? value : 0;
    }

    private static string Money(decimal value) => value.ToString("N0", IndonesianCulture);
    private static string DetailedNumber(decimal value) => value.ToString("N4", IndonesianCulture);
    private static string F(decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
