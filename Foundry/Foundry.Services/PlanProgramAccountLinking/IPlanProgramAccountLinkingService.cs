using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IPlanProgramAccountLinkingService : IFoundryRepositoryBase<PlanProgramAccountsLinking>
    {
        Task<string> AddUpdatePlanProgramAccount(List<PlanProgramAccountsLinking> model);
        Task<List<ProgramAccountDetailDto>> GetProgramAccountsDetailsByPlanIds(List<int> planIds);
    }
}
