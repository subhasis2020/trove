using Foundry.Domain.DbModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IReloadBalanceService : IFoundryRepositoryBase<ReloadBalanceRequest>
    {
        Task<List<ReloadBalanceRequest>> GetAllReloadBalance();
        Task<ReloadBalanceRequest> GetReloadBalanceById(int Id);
    }
}
