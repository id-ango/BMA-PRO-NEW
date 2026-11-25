#nullable enable

using eSoft.Penjualan.Data;
using eSoft.Persediaan.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Services;

public sealed record SalesGroupHeader(int Id, string Code, decimal Amount);
public sealed record SalesGroupDetail(int HeaderId, string? ItemCode, string? Unit, decimal Amount, decimal Quantity, string? Name = null);
public sealed record SalesGroupItem(string? ItemCode, int? GroupId, string? Name = null);
public sealed record SalesGroupNode(int Id, int? ParentId, string Code, string Name);
public sealed record SalesGroupItemTotal(string Code, string Name, decimal Quantity, decimal Amount, bool UnitWarning, IReadOnlyList<string>? Units = null, string? MissingCodeName = null);
public sealed record SalesGroupValue(string Id, string Group, decimal Amount, IReadOnlyList<SalesGroupItemTotal> Items);
public sealed record SalesGroupResult(int Year, DateTime Through, IReadOnlyList<SalesGroupValue> Values,
    decimal HeaderTotal, int UnallocatedDocuments);

public static class SalesGroupAnalysis
{
    private sealed record GroupKey(string Id, string Name);
    private sealed record ItemKey(GroupKey Group, string Code, string MissingCodeName);
    private sealed class ItemTotals
    {
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public HashSet<string> Units { get; } = new(StringComparer.OrdinalIgnoreCase);
        public bool MissingUnit { get; set; }
    }
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
            .GroupBy(x => new { x.OeTransHId, x.ItemCode, x.NamaItem, x.Satuan })
            .Select(g => new SalesGroupDetail(g.Key.OeTransHId, g.Key.ItemCode, g.Key.Satuan,
                g.Sum(x => x.Jumlah), g.Sum(x => x.Qty), g.Key.NamaItem))
            .ToListAsync();
        await using var inventoryDb = new DbContextPersediaan(inventoryOptions);
        var items = await inventoryDb.IcItems.AsNoTracking()
            .Select(x => new SalesGroupItem(x.ItemCode, x.ItemGroupId, x.NamaItem)).ToListAsync();
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
                var name = g.Select(x => x.Name?.Trim()).Where(x => !string.IsNullOrEmpty(x)).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
                return (Group: ids.Length == 1 ? ResolveGroup(ids[0], nodes) : Unassigned, Name: name);
            }, StringComparer.OrdinalIgnoreCase);
        var detailsByHeader = details.ToLookup(x => x.HeaderId);
        var values = new Dictionary<GroupKey, decimal>();
        var itemTotals = new Dictionary<ItemKey, ItemTotals>();
        decimal headerTotal = 0;
        var unallocatedDocuments = 0;

        foreach (var header in headers)
        {
            var sign = header.Code == "95" ? -1 : 1;
            var total = sign * header.Amount;
            headerTotal += total;
            var byGroup = new Dictionary<GroupKey, decimal>();
            var byItem = new Dictionary<ItemKey, decimal>();
            foreach (var detail in detailsByHeader[header.Id])
            {
                var code = detail.ItemCode?.Trim().ToUpperInvariant() ?? string.Empty;
                var found = itemGroups.TryGetValue(code, out var item);
                var group = found ? item.Group : Unassigned;
                var detailName = detail.Name?.Trim() ?? string.Empty;
                var key = new ItemKey(group, code, code.Length == 0 ? detailName.ToUpperInvariant() : string.Empty);
                if (!itemTotals.TryGetValue(key, out var itemTotal))
                {
                    itemTotal = new ItemTotals();
                    itemTotals.Add(key, itemTotal);
                }
                if (string.IsNullOrEmpty(itemTotal.Name))
                    itemTotal.Name = (found ? item.Name : null) ?? detailName;
                itemTotal.Quantity += sign * detail.Quantity;
                var unit = detail.Unit?.Trim();
                if (string.IsNullOrEmpty(unit) || unit == "-")
                    itemTotal.MissingUnit = true;
                else
                    itemTotal.Units.Add(unit);
                values.TryAdd(group, 0);
                byGroup[group] = byGroup.GetValueOrDefault(group) + detail.Amount;
                byItem[key] = byItem.GetValueOrDefault(key) + detail.Amount;
            }

            var basis = byGroup.Values.Sum();
            if (basis == 0)
            {
                unallocatedDocuments++;
                values[Unallocated] = values.GetValueOrDefault(Unallocated) + total;
                continue;
            }

            var allocations = byGroup.Where(x => x.Value != 0).OrderBy(x => x.Key.Id, StringComparer.Ordinal).ToArray();
            var groupAllocations = new Dictionary<GroupKey, decimal>();
            decimal allocated = 0;
            for (var index = 0; index < allocations.Length; index++)
            {
                var entry = allocations[index];
                var amount = index == allocations.Length - 1
                    ? total - allocated
                    : decimal.Round(total * (entry.Value / basis), 4);
                values[entry.Key] = values.GetValueOrDefault(entry.Key) + amount;
                groupAllocations[entry.Key] = amount;
                allocated += amount;
            }

            foreach (var groupItems in byItem.GroupBy(x => x.Key.Group))
            {
                var groupAmount = groupAllocations.GetValueOrDefault(groupItems.Key);
                var groupBasis = byGroup[groupItems.Key];
                var itemAllocations = groupItems.Where(x => x.Value != 0)
                    .OrderBy(x => x.Key.Code, StringComparer.Ordinal).ThenBy(x => x.Key.MissingCodeName, StringComparer.Ordinal).ToArray();
                decimal itemAllocated = 0;
                for (var index = 0; index < itemAllocations.Length; index++)
                {
                    var entry = itemAllocations[index];
                    var amount = index == itemAllocations.Length - 1
                        ? groupAmount - itemAllocated
                        : groupBasis == 0
                            ? decimal.Round(total * (entry.Value / basis), 4)
                            : decimal.Round(groupAmount * (entry.Value / groupBasis), 4);
                    itemTotals[entry.Key].Amount += amount;
                    itemAllocated += amount;
                }
            }
        }

        var itemsByGroup = itemTotals.ToLookup(item => item.Key.Group);
        return new SalesGroupResult(year, through,
            values.Select(x => new SalesGroupValue(x.Key.Id, x.Key.Name, x.Value,
                itemsByGroup[x.Key]
                    .Select(item => new SalesGroupItemTotal(item.Key.Code, string.IsNullOrEmpty(item.Value.Name) ? "Nama item tidak diketahui" : item.Value.Name,
                        item.Value.Quantity, item.Value.Amount, item.Value.MissingUnit || item.Value.Units.Count > 1, item.Value.Units.ToArray(), item.Key.MissingCodeName))
                    .OrderByDescending(item => item.Amount).ThenBy(item => item.Code).ToArray()))
                .OrderByDescending(x => x.Amount).ThenBy(x => x.Group).ToArray(),
            headerTotal, unallocatedDocuments);
    }

    public static IEnumerable<SalesGroupItemTotal> SortItems(IEnumerable<SalesGroupItemTotal> items, bool byQuantity) =>
        (byQuantity ? items.OrderByDescending(x => x.Quantity) : items.OrderByDescending(x => x.Amount))
            .ThenBy(x => x.Code, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase);

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
