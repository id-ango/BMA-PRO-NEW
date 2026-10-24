using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Persediaan.Services
{
    public class ItemSearchService : IItemSearchService
    {
        private readonly DbContextPersediaan _db;

        public ItemSearchService(DbContextPersediaan db)
        {
            _db = db;
        }

        public async Task<List<IcItem>> SearchAsync(string keyword, int take = 100, CancellationToken cancellationToken = default)
        {
            var tokens = (keyword ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var query = _db.IcItems.AsNoTracking().Include(i => i.ItemGroup).ThenInclude(g => g!.Parent).AsQueryable();

            foreach (var token in tokens)
            {
                var term = token;
                query = query.Where(i =>
                    i.ItemCode.Contains(term) ||
                    i.NamaItem.Contains(term) ||
                    (i.PartNumber != null && i.PartNumber.Contains(term)) ||
                    (i.SupplierCode != null && i.SupplierCode.Contains(term)) ||
                    (i.SearchTags != null && i.SearchTags.Contains(term)) ||
                    (i.ItemGroup != null && (i.ItemGroup.Name.Contains(term) ||
                                             (i.ItemGroup.Parent != null && i.ItemGroup.Parent.Name.Contains(term)))));
            }

            return await query
                .OrderBy(i => i.ItemCode)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
    }
}
