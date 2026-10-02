using System.Threading.Tasks;
using Accounting.Services.View;

namespace Accounting.Services;

public interface IExecutiveDashboardService
{
    Task<ExecutiveDashboardView> GetDashboardAsync();
}
