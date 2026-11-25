using System.Text;
using Accounting.Services;
using eSoft.Penjualan.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;
var through = new DateTime(2026, 3, 10);
var groups = new[]
{
    new SalesGroupNode(1, null, "STK", "STOCK"),
    new SalesGroupNode(2, 1, "MES", "MACHINE"),
    new SalesGroupNode(3, 2, "HMR", "HAMMERMILL"),
    new SalesGroupNode(4, 1, "SPR", "SPAREPART"),
    new SalesGroupNode(5, 4, "SCR", "SCREEN")
};
var items = new[]
{
    new SalesGroupItem("A", 3, "Machine A"),
    new SalesGroupItem("B", 3, "Machine B"),
    new SalesGroupItem("C", 5, "Spare C")
};
var headers = new[]
{
    new SalesGroupHeader(1, "94", 120), new SalesGroupHeader(2, "95", 30),
    new SalesGroupHeader(3, "94", 7), new SalesGroupHeader(4, "94", 11),
    new SalesGroupHeader(5, "94", 8), new SalesGroupHeader(6, "94", 10)
};
var details = new[]
{
    new SalesGroupDetail(1, "a", "PCS", 50, 2, "Old name A"),
    new SalesGroupDetail(1, "B", "PCS", 25, 3),
    new SalesGroupDetail(1, "C", "BOX", 25, 1),
    new SalesGroupDetail(2, "A", "pcs", 20, 1),
    new SalesGroupDetail(2, "C", "BOX", 10, 1),
    new SalesGroupDetail(3, "A", "KG", 10, 3),
    new SalesGroupDetail(3, "B", "PCS", -10, 4),
    new SalesGroupDetail(5, "missing", null, 8, 3, "Missing item"),
    new SalesGroupDetail(6, "A", "PCS", 10, 1),
    new SalesGroupDetail(6, "B", "PCS", -10, 1),
    new SalesGroupDetail(6, "C", "BOX", 20, 2)
};
var result = SalesGroupAnalysis.Calculate(2026, through, headers, details, items, groups);
void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
SalesGroupValue Group(string name) => result.Values.Single(x => x.Group == name);
SalesGroupItemTotal Item(string group, string code) => Group(group).Items.Single(x => x.Code == code);
Check(result.HeaderTotal == 126 && result.Values.Sum(x => x.Amount) == result.HeaderTotal, "Group totals do not reconcile to headers.");
Check(Group("STOCK > MACHINE").Amount == 70 && Group("STOCK > SPAREPART").Amount == 30, "Group allocation or returns are incorrect.");
Check(Item("STOCK > MACHINE", "A").Amount == 45 && Item("STOCK > MACHINE", "B").Amount == 25, "Item allocation or zero-basis group is incorrect.");
Check(Item("STOCK > MACHINE", "A").Quantity == 5 && Item("STOCK > MACHINE", "A").UnitWarning, "Mixed-unit item quantity or warning is incorrect.");
Check(Item("STOCK > MACHINE", "A").Name == "Machine A", "Current item name was not used.");
Check(Item("STOCK > SPAREPART", "C").Amount == 30 && Item("STOCK > SPAREPART", "C").Quantity == 2, "Sparepart item total is incorrect.");
Check(Group("Nilai tidak teralokasi").Amount == 18 && result.UnallocatedDocuments == 2, "Unallocated documents are incorrect.");
Check(Item("Belum ada group / item tidak ditemukan", "MISSING").UnitWarning && Item("Belum ada group / item tidak ditemukan", "MISSING").Quantity == 3, "Unassigned or missing-unit item was lost.");
foreach (var group in result.Values.Where(x => x.Items.Count != 0))
    Check(group.Items.Sum(x => x.Amount) == group.Amount, $"Items do not reconcile for {group.Group}.");
var rounding = SalesGroupAnalysis.Calculate(2026, through,
    new[] { new SalesGroupHeader(10, "94", 1) },
    new[] { new SalesGroupDetail(10, "A", "PCS", 1, 1), new SalesGroupDetail(10, "B", "PCS", 1, 1), new SalesGroupDetail(10, "C", "PCS", 1, 1) }, items, groups);
Check(rounding.Values.Sum(x => x.Amount) == 1 && rounding.Values.All(x => x.Items.Sum(y => y.Amount) == x.Amount), "Rounding remainder did not reconcile.");
var unknownNames = SalesGroupAnalysis.Calculate(2026, through,
    new[] { new SalesGroupHeader(11, "94", 2) },
    new[] { new SalesGroupDetail(11, null, "PCS", 1, 1, "Unknown one"), new SalesGroupDetail(11, null, "PCS", 1, 1, "Unknown two") }, items, groups);
Check(unknownNames.Values.Single().Items.Count == 2, "Different unnamed-code items were merged.");
Check(SalesGroupAnalysis.SortItems(Group("STOCK > MACHINE").Items, true).First().Code == "B", "Quantity sorting is incorrect.");
Check(SalesGroupAnalysis.SortItems(Group("STOCK > MACHINE").Items, false).First().Code == "A", "Value sorting is incorrect.");
var previous = SalesGroupAnalysis.Calculate(2025, new DateTime(2025, 12, 31),
    new[] { new SalesGroupHeader(20, "94", 50), new SalesGroupHeader(21, "95", 10) },
    new[] { new SalesGroupDetail(20, "C", "PCS", 50, 5), new SalesGroupDetail(21, "C", "PCS", 10, 1) }, items, groups);
var emptyYears = Enumerable.Range(2021, 4).Select(year => SalesGroupAnalysis.Calculate(year, new DateTime(year, 12, 31), Array.Empty<SalesGroupHeader>(), Array.Empty<SalesGroupDetail>(), items, groups));
var annual = SalesGroupAnnualAnalysis.Build(emptyYears.Append(previous).Append(result));
Check(annual.Years.SequenceEqual(new[] { 2026, 2025, 2024, 2023, 2022, 2021 }), "Annual year order is incorrect.");
var twoYears = SalesGroupAnnualAnalysis.Build(new[] { previous, result });
Check(twoYears.Years.SequenceEqual(new[] { 2026, 2025 }) && twoYears.HeaderTotals[2025] == 40, "Two-year comparison is incorrect.");
foreach (var year in annual.Years)
    Check(annual.Groups.Sum(group => group.Values.GetValueOrDefault(year)) == annual.HeaderTotals[year], $"Annual group total does not reconcile for {year}.");
var annualSpare = annual.Groups.Single(x => x.Group == "STOCK > SPAREPART");
var annualC = annualSpare.Items.Single(x => x.Code == "C");
Check(annualSpare.Values[2026] == 30 && annualSpare.Values[2025] == 40 && !annualSpare.Values.ContainsKey(2024), "Annual group values or absent year are incorrect.");
Check(annualC.Values[2026].Quantity == 2 && annualC.Values[2025].Quantity == 4 && annualC.UnitWarning, "Annual quantities, returns, or cross-year unit warning are incorrect.");
foreach (var group in annual.Groups.Where(x => x.Items.Count > 0))
    foreach (var year in annual.Years)
        Check(group.Items.Sum(item => item.Values.GetValueOrDefault(year)?.Amount ?? 0) == group.Values.GetValueOrDefault(year), $"Annual item values do not reconcile for {group.Group} in {year}.");
var nameCollision = SalesGroupAnalysis.Calculate(2026, through, new[] { new SalesGroupHeader(22, "94", 2) },
    new[] { new SalesGroupDetail(22, null, "PCS", 1, 1), new SalesGroupDetail(22, null, "PCS", 1, 1, "Nama item tidak diketahui") }, items, groups);
Check(SalesGroupAnnualAnalysis.Build(new[] { nameCollision }).Groups.Single().Items.Count == 2, "Items without codes and distinct identities were merged.");
var manyItems = Enumerable.Range(0, 300).Select(i => new SalesGroupItemTotal($"I{i:D4}", $"Item {i} dengan nama yang cukup panjang", i + 0.1234m, i == 0 ? -123456789012.1234m : 123456789012.1234m + i, i == 0)).ToArray();
var many = new SalesGroupResult(2026, through, new[] { new SalesGroupValue("group:2", "STOCK > MACHINE", manyItems.Sum(x => x.Amount), manyItems) }, manyItems.Sum(x => x.Amount), 0);
foreach (var analysis in new[] { result, many, SalesGroupAnalysis.Calculate(2026, through, Array.Empty<SalesGroupHeader>(), Array.Empty<SalesGroupDetail>(), items, groups) })
{
    foreach (var byQuantity in new[] { true, false })
    {
        var report = new SalesChartReport(through, new[] { 2026, 2025, 2024, 2023, 2022, 2021 }, new Dictionary<(int, int), decimal>(), new Dictionary<(int, int), decimal>(), analysis, analysis.HeaderTotal, byQuantity);
        var pdf = SalesChartPdfExporter.Generate(report);
        Check(pdf.Length > 1000 && Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "PDF generation failed.");
    }
}
var yearCounts = Enumerable.Range(2, 5).Select(count => SalesGroupAnnualAnalysis.Build(new[] { result, previous }.Concat(emptyYears.OrderByDescending(x => x.Year).Take(count - 2))));
foreach (var comparison in yearCounts.Concat(new[] { SalesGroupAnnualAnalysis.Build(emptyYears.Append(previous).Append(many)), SalesGroupAnnualAnalysis.Build(emptyYears) }))
{
    var report = new SalesChartReport(through, new[] { 2026, 2025, 2024, 2023, 2022, 2021 }, new Dictionary<(int, int), decimal>(), new Dictionary<(int, int), decimal>(), AnnualComparison: comparison);
    var pdf = SalesChartPdfExporter.Generate(report);
    Check(pdf.Length > 1000 && Encoding.ASCII.GetString(pdf, 0, 5) == "%PDF-", "Annual comparison PDF generation failed.");
}
using var db = new DbContextJual(new DbContextOptionsBuilder<DbContextJual>().UseSqlServer("Server=localhost;Database=NoConnection;Trusted_Connection=True;TrustServerCertificate=True").Options);
var start = new DateTime(2026, 1, 1);
var end = through.AddDays(1);
var query = db.OeTransDs.AsNoTracking().Where(x => x.OeTransH.Tanggal >= start && x.OeTransH.Tanggal < end && (x.OeTransH.Kode == "94" || x.OeTransH.Kode == "95"))
    .GroupBy(x => new { x.OeTransHId, x.ItemCode, x.NamaItem, x.Satuan })
    .Select(g => new SalesGroupDetail(g.Key.OeTransHId, g.Key.ItemCode, g.Key.Satuan, g.Sum(x => x.Jumlah), g.Sum(x => x.Qty), g.Key.NamaItem));
var sql = query.ToQueryString();
Check(sql.Contains("GROUP BY") && sql.Contains("JOIN"), "Item query did not translate to grouped SQL.");
var headerQuery = db.OeTransHs.AsNoTracking().Where(x => x.Tanggal >= start && x.Tanggal < end && (x.Kode == "94" || x.Kode == "95"))
    .Select(x => new { Year = x.Tanggal.Year, Header = new SalesGroupHeader(x.OeTransHId, x.Kode, x.Jumlah) });
Check(headerQuery.ToQueryString().Contains("DATEPART"), "Annual header query did not translate to SQL.");
Console.WriteLine("Sales group/item and annual reconciliation, returns, unit warnings, sorting, PDF pagination, and SQL translation checks passed.");
