#nullable enable

using eSoft.Penjualan.Data;
using eSoft.Persediaan.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Services;

public sealed record SalesGroupAnnualItem(string Code, string Name, IReadOnlyDictionary<int, SalesGroupItemTotal> Values, bool UnitWarning);
public sealed record SalesGroupAnnualGroup(string Id, string Group, IReadOnlyDictionary<int, decimal> Values, IReadOnlyList<SalesGroupAnnualItem> Items);
public sealed record SalesGroupAnnualResult(IReadOnlyList<int> Years, IReadOnlyDictionary<int, decimal> HeaderTotals,
    IReadOnlyDictionary<int, int> UnallocatedDocuments, IReadOnlyList<SalesGroupAnnualGroup> Groups);

public static class SalesGroupAnnualAnalysis
{
    public static async Task<SalesGroupAnnualResult> LoadAsync(DbContextOptions<DbContextJual> salesOptions,
        DbContextOptions<DbContextPersediaan> inventoryOptions, IEnumerable<int> selectedYears, DateTime reportDate)
    {
        var years = selectedYears.Distinct().OrderByDescending(x => x).ToArray();
        if (years.Length == 0 || years.Any(year => year < 1 || year > reportDate.Year))
            throw new ArgumentOutOfRangeException(nameof(selectedYears));
        var start = new DateTime(years[^1], 1, 1);
        var end = years[0] == reportDate.Year ? reportDate.Date.AddDays(1) : new DateTime(years[0], 12, 31).AddDays(1);
        await using var salesDb = new DbContextJual(salesOptions);
        var headers = await salesDb.OeTransHs.AsNoTracking()
            .Where(x => x.Tanggal >= start && x.Tanggal < end && (x.Kode == "94" || x.Kode == "95"))
            .Select(x => new { Year = x.Tanggal.Year, Header = new SalesGroupHeader(x.OeTransHId, x.Kode, x.Jumlah) })
            .ToListAsync();
        var details = await salesDb.OeTransDs.AsNoTracking()
            .Where(x => x.OeTransH.Tanggal >= start && x.OeTransH.Tanggal < end && (x.OeTransH.Kode == "94" || x.OeTransH.Kode == "95"))
            .GroupBy(x => new { x.OeTransHId, x.ItemCode, x.NamaItem, x.Satuan })
            .Select(g => new SalesGroupDetail(g.Key.OeTransHId, g.Key.ItemCode, g.Key.Satuan,
                g.Sum(x => x.Jumlah), g.Sum(x => x.Qty), g.Key.NamaItem))
            .ToListAsync();
        await using var inventoryDb = new DbContextPersediaan(inventoryOptions);
        var items = await inventoryDb.IcItems.AsNoTracking()
            .Select(x => new SalesGroupItem(x.ItemCode, x.ItemGroupId, x.NamaItem)).ToListAsync();
        var groups = await inventoryDb.IcItemGroups.AsNoTracking()
            .Select(x => new SalesGroupNode(x.ItemGroupId, x.ParentId, x.Code, x.Name)).ToListAsync();
        var headerYears = headers.ToDictionary(x => x.Header.Id, x => x.Year);
        var headersByYear = headers.ToLookup(x => x.Year, x => x.Header);
        var detailsByYear = details.Where(x => headerYears.ContainsKey(x.HeaderId)).ToLookup(x => headerYears[x.HeaderId]);
        var results = years.Select(year => SalesGroupAnalysis.Calculate(year,
            year == reportDate.Year ? reportDate.Date : new DateTime(year, 12, 31),
            headersByYear[year], detailsByYear[year], items, groups));
        return Build(results);
    }

    public static SalesGroupAnnualResult Build(IEnumerable<SalesGroupResult> source)
    {
        var results = source.OrderByDescending(x => x.Year).ToArray();
        var years = results.Select(x => x.Year).ToArray();
        if (years.Distinct().Count() != years.Length)
            throw new ArgumentException("Duplicate comparison year.", nameof(source));
        var annualGroups = results.SelectMany(result => result.Values.Select(group => (result.Year, Group: group)))
            .GroupBy(x => x.Group.Id)
            .Select(group =>
            {
                var groupYears = group.ToArray();
                var annualItems = groupYears.SelectMany(entry => entry.Group.Items.Select(item => (entry.Year, Item: item)))
                    .GroupBy(entry => (Code: entry.Item.Code.Trim().ToUpperInvariant(),
                        MissingName: string.IsNullOrWhiteSpace(entry.Item.Code) ? entry.Item.MissingCodeName ?? entry.Item.Name.Trim().ToUpperInvariant() : string.Empty))
                    .Select(entries =>
                    {
                        var values = entries.ToDictionary(x => x.Year, x => x.Item);
                        var latest = entries.OrderByDescending(x => x.Year).First().Item;
                        var units = entries.SelectMany(x => x.Item.Units ?? Array.Empty<string>()).Distinct(StringComparer.OrdinalIgnoreCase).Count();
                        return new SalesGroupAnnualItem(latest.Code, latest.Name, values, units > 1 || entries.Any(x => x.Item.UnitWarning));
                    }).ToArray();
                return new SalesGroupAnnualGroup(group.Key, groupYears[0].Group.Group,
                    groupYears.ToDictionary(x => x.Year, x => x.Group.Amount), annualItems);
            })
            .OrderByDescending(group => years.Length == 0 ? 0 : group.Values.GetValueOrDefault(years[0]))
            .ThenBy(group => group.Group).ToArray();
        return new SalesGroupAnnualResult(years, results.ToDictionary(x => x.Year, x => x.HeaderTotal),
            results.ToDictionary(x => x.Year, x => x.UnallocatedDocuments), annualGroups);
    }

    public static IEnumerable<SalesGroupAnnualItem> SortItems(IEnumerable<SalesGroupAnnualItem> items, int year, bool byQuantity) =>
        (byQuantity ? items.OrderByDescending(x => x.Values.GetValueOrDefault(year)?.Quantity ?? 0)
                    : items.OrderByDescending(x => x.Values.GetValueOrDefault(year)?.Amount ?? 0))
        .ThenBy(x => x.Code, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase);
}
