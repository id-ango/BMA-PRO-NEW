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

        public async Task<List<IcItemGroup>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.IcItemGroups
                .AsNoTracking()
                .OrderBy(g => g.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<IcItemGroup?> GetByIdAsync(int itemGroupId, CancellationToken cancellationToken = default)
        {
            return await _db.IcItemGroups
                .FirstOrDefaultAsync(g => g.ItemGroupId == itemGroupId, cancellationToken);
        }

        public async Task<List<IcItemGroup>> GetChildrenAsync(int? parentId, CancellationToken cancellationToken = default)
        {
            return await _db.IcItemGroups
                .AsNoTracking()
                .Where(g => g.ParentId == parentId)
                .OrderBy(g => g.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(IcItemGroup itemGroup, CancellationToken cancellationToken = default)
        {
            _db.IcItemGroups.Add(itemGroup);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(IcItemGroup itemGroup, CancellationToken cancellationToken = default)
        {
            _db.IcItemGroups.Update(itemGroup);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(IcItemGroup itemGroup, CancellationToken cancellationToken = default)
        {
            _db.IcItemGroups.Remove(itemGroup);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CodeExistsAsync(int? parentId, string code, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            return await _db.IcItemGroups.AnyAsync(g =>
                g.ParentId == parentId &&
                g.Code == code &&
                (!excludeId.HasValue || g.ItemGroupId != excludeId.Value), cancellationToken);
        }

        public async Task<bool> IsInUseAsync(int itemGroupId, CancellationToken cancellationToken = default)
        {
            return await _db.IcItemGroups.AnyAsync(g => g.ParentId == itemGroupId, cancellationToken)
                || await _db.IcItems.AnyAsync(i => i.ItemGroupId == itemGroupId, cancellationToken);
        }
    }
}
