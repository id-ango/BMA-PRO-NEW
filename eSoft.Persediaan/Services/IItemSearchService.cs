using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eSoft.Persediaan.Model;

namespace eSoft.Persediaan.Services
{
    public interface IItemSearchService
    {
        Task<List<IcItem>> SearchAsync(string keyword, int take = 100, CancellationToken cancellationToken = default);
    }
}
