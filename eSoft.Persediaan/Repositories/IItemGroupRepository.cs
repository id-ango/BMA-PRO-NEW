using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eSoft.Persediaan.Model;

namespace eSoft.Persediaan.Repositories
{
    public interface IItemGroupRepository
    {
        Task<List<IcItemGroup>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IcItemGroup?> GetByIdAsync(int itemGroupId, CancellationToken cancellationToken = default);
        Task<List<IcItemGroup>> GetChildrenAsync(int? parentId, CancellationToken cancellationToken = default);
        Task AddAsync(IcItemGroup itemGroup, CancellationToken cancellationToken = default);
        Task UpdateAsync(IcItemGroup itemGroup, CancellationToken cancellationToken = default);
        Task DeleteAsync(IcItemGroup itemGroup, CancellationToken cancellationToken = default);
        Task<bool> CodeExistsAsync(int? parentId, string code, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> IsInUseAsync(int itemGroupId, CancellationToken cancellationToken = default);
    }
}
