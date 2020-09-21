using Foundry.Domain.Dto;
using Foundry.Domain.DbModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IBenefactorNotifications : IFoundryRepositoryBase<ReloadBalanceRequest>
    {
        Task<List<BenefactorNotificationsDto>> GetBenefactorNotifications(int benefactorId);
    }
}
