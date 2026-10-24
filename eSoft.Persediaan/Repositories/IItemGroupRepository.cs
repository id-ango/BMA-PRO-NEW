using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eSoft.Persediaan.Model;

namespace eSoft.Persediaan.Repositories
{
    public interface IItemGroupRepository
    {
        Task<List<ItemGroup>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ItemGroup?> GetByIdAsync(int itemGroupId, CancellationToken cancellationToken = default);
        Task<List<ItemGroup>> GetChildrenAsync(int? parentId, CancellationToken cancellationToken = default);
        Task AddAsync(ItemGroup itemGroup, CancellationToken cancellationToken = default);
        Task UpdateAsync(ItemGroup itemGroup, CancellationToken cancellationToken = default);
        Task DeleteAsync(ItemGroup itemGroup, CancellationToken cancellationToken = default);
        Task<bool> CodeExistsAsync(int? parentId, string code, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}
