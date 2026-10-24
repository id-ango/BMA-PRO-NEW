using System.Threading;
using System.Threading.Tasks;

namespace eSoft.Persediaan.Services
{
    public interface IItemCodeGenerator
    {
        Task<string> GenerateAsync(int itemGroupId, CancellationToken cancellationToken = default);
    }
}
