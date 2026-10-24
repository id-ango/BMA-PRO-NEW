using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eSoft.Persediaan.Data;
using eSoft.Persediaan.Model;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Persediaan.Repositories
{
    public class ItemGroupRepository : IItemGroupRepository
    {
        private readonly DbContextPersediaan _db;

        public ItemGroupRepository(DbContextPersediaan db)
        {
            _db = db;
        }

        public async Task<List<ItemGroup>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.ItemGroups
                .AsNoTracking()
                .OrderBy(g => g.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<ItemGroup?> GetByIdAsync(int itemGroupId, CancellationToken cancellationToken = default)
        {
            return await _db.ItemGroups
                .FirstOrDefaultAsync(g => g.ItemGroupId == itemGroupId, cancellationToken);
        }

        public async Task<List<ItemGroup>> GetChildrenAsync(int? parentId, CancellationToken cancellationToken = default)
        {
            return await _db.ItemGroups
                .AsNoTracking()
                .Where(g => g.ParentId == parentId)
                .OrderBy(g => g.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(ItemGroup itemGroup, CancellationToken cancellationToken = default)
        {
            _db.ItemGroups.Add(itemGroup);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(ItemGroup itemGroup, CancellationToken cancellationToken = default)
        {
            _db.ItemGroups.Update(itemGroup);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(ItemGroup itemGroup, CancellationToken cancellationToken = default)
        {
            _db.ItemGroups.Remove(itemGroup);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CodeExistsAsync(int? parentId, string code, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            return await _db.ItemGroups.AnyAsync(g =>
                g.ParentId == parentId &&
                g.Code == code &&
                (!excludeId.HasValue || g.ItemGroupId != excludeId.Value), cancellationToken);
        }
    }
}
