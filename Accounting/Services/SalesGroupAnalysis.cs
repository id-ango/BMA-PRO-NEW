#nullable enable

using eSoft.Penjualan.Data;
using eSoft.Persediaan.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Services;

public sealed record SalesGroupHeader(int Id, string Code, decimal Amount);
public sealed record SalesGroupDetail(int HeaderId, string? ItemCode, string? Unit, decimal Amount, decimal Quantity);
public sealed record SalesGroupItem(string? ItemCode, int? GroupId);
public sealed record SalesGroupNode(int Id, int? ParentId, string Code, string Name);
public sealed record SalesGroupValue(string Group, decimal Amount);
public sealed record SalesGroupQuantity(string Group, string Unit, decimal Quantity);
public sealed record SalesGroupResult(int Year, DateTime Through, IReadOnlyList<SalesGroupValue> Values,
    IReadOnlyList<SalesGroupQuantity> Quantities, decimal HeaderTotal, int UnallocatedDocuments, bool HasUnknownUnitQuantities = false);

public static class SalesGroupAnalysis
{
    private sealed record GroupKey(string Id, string Name);
    private static readonly GroupKey Unassigned = new("unassigned", "Belum ada group / item tidak ditemukan");
    private static readonly GroupKey Unallocated = new("unallocated", "Nilai tidak teralokasi");

    public static async Task<SalesGroupResult> LoadAsync(DbContextOptions<DbContextJual> salesOptions,
        DbContextOptions<DbContextPersediaan> inventoryOptions, int year, DateTime reportDate)
    {
        var start = new DateTime(year, 1, 1);
        var through = year == reportDate.Year ? reportDate.Date : new DateTime(year, 12, 31);
        var end = through.AddDays(1);
        await using var salesDb = new DbContextJual(salesOptions);
        var headers = await salesDb.OeTransHs.AsNoTracking()
            .Where(x => x.Tanggal >= start && x.Tanggal < end && (x.Kode == "94" || x.Kode == "95"))
            .Select(x => new SalesGroupHeader(x.OeTransHId, x.Kode, x.Jumlah))
            .ToListAsync();
        var details = await salesDb.OeTransDs.AsNoTracking()
            .Where(x => x.OeTransH.Tanggal >= start && x.OeTransH.Tanggal < end && (x.OeTransH.Kode == "94" || x.OeTransH.Kode == "95"))
            .GroupBy(x => new { x.OeTransHId, x.ItemCode, x.Satuan })
            .Select(g => new SalesGroupDetail(g.Key.OeTransHId, g.Key.ItemCode, g.Key.Satuan,
                g.Sum(x => x.Jumlah), g.Sum(x => x.Qty)))
            .ToListAsync();
        await using var inventoryDb = new DbContextPersediaan(inventoryOptions);
        var items = await inventoryDb.IcItems.AsNoTracking()
            .Select(x => new SalesGroupItem(x.ItemCode, x.ItemGroupId)).ToListAsync();
        var groups = await inventoryDb.IcItemGroups.AsNoTracking()
            .Select(x => new SalesGroupNode(x.ItemGroupId, x.ParentId, x.Code, x.Name)).ToListAsync();
        return Calculate(year, through, headers, details, items, groups);
    }

    public static SalesGroupResult Calculate(int year, DateTime through, IEnumerable<SalesGroupHeader> headers,
        IEnumerable<SalesGroupDetail> details, IEnumerable<SalesGroupItem> items, IEnumerable<SalesGroupNode> groups)
    {
        var nodes = groups.ToDictionary(x => x.Id);
        var itemGroups = items.Where(x => !string.IsNullOrWhiteSpace(x.ItemCode))
            .GroupBy(x => x.ItemCode!.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g =>
            {
                var ids = g.Select(x => x.GroupId).Distinct().ToArray();
                return ids.Length == 1 ? ResolveGroup(ids[0], nodes) : Unassigned;
            }, StringComparer.OrdinalIgnoreCase);
        var detailsByHeader = details.ToLookup(x => x.HeaderId);
        var values = new Dictionary<GroupKey, decimal>();
        var quantities = new Dictionary<(GroupKey Group, string Unit), decimal>();
        decimal headerTotal = 0;
        var unallocatedDocuments = 0;
        var hasUnknownUnitQuantities = false;

        foreach (var header in headers)
        {
            var sign = header.Code == "95" ? -1 : 1;
            var total = sign * header.Amount;
            headerTotal += total;
            var byGroup = new Dictionary<GroupKey, decimal>();
            foreach (var detail in detailsByHeader[header.Id])
            {
                var code = detail.ItemCode?.Trim();
                var group = !string.IsNullOrEmpty(code) && itemGroups.TryGetValue(code, out var found) ? found : Unassigned;
                byGroup[group] = byGroup.GetValueOrDefault(group) + detail.Amount;
                if (string.IsNullOrWhiteSpace(detail.Unit))
                {
                    hasUnknownUnitQuantities |= detail.Quantity != 0;
                }
                else
                {
                    var key = (group, detail.Unit.Trim().ToUpperInvariant());
                    quantities[key] = quantities.GetValueOrDefault(key) + sign * detail.Quantity;
                }
            }

            var basis = byGroup.Values.Sum();
            if (basis == 0)
            {
                unallocatedDocuments++;
                values[Unallocated] = values.GetValueOrDefault(Unallocated) + total;
                continue;
            }

            var allocations = byGroup.Where(x => x.Value != 0).OrderBy(x => x.Key.Id, StringComparer.Ordinal).ToArray();
            decimal allocated = 0;
            for (var index = 0; index < allocations.Length; index++)
            {
                var entry = allocations[index];
                var amount = index == allocations.Length - 1
                    ? total - allocated
                    : decimal.Round(total * (entry.Value / basis), 4);
                values[entry.Key] = values.GetValueOrDefault(entry.Key) + amount;
                allocated += amount;
            }
        }

        return new SalesGroupResult(year, through,
            values.Select(x => new SalesGroupValue(x.Key.Name, x.Value)).OrderByDescending(x => x.Amount).ThenBy(x => x.Group).ToArray(),
            quantities.Select(x => new SalesGroupQuantity(x.Key.Group.Name, x.Key.Unit, x.Value)).OrderBy(x => x.Unit).ThenByDescending(x => x.Quantity).ThenBy(x => x.Group).ToArray(),
            headerTotal, unallocatedDocuments, hasUnknownUnitQuantities);
    }

    private static GroupKey ResolveGroup(int? groupId, IReadOnlyDictionary<int, SalesGroupNode> nodes)
    {
        if (!groupId.HasValue || !nodes.TryGetValue(groupId.Value, out var node))
            return Unassigned;
        var path = new List<SalesGroupNode>();
        var visited = new HashSet<int>();
        while (true)
        {
            if (!visited.Add(node.Id))
                return Unassigned;
            path.Add(node);
            if (!node.ParentId.HasValue)
                break;
            if (!nodes.TryGetValue(node.ParentId.Value, out node!))
                return Unassigned;
        }
        path.Reverse();
        var selected = path[0].Code.Equals("STK", StringComparison.OrdinalIgnoreCase) && path.Count > 1 ? path[1] : path[0];
        var name = selected.Id == path[0].Id ? selected.Name : $"{path[0].Name} > {selected.Name}";
        return new GroupKey($"group:{selected.Id}", name);
    }
}
