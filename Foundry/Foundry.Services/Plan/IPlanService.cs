using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IPlanService : IFoundryRepositoryBase<ProgramPackage>
    {
        Task<IEnumerable<PlanListingDto>> GetPlanListing(int programId);
        Task<int> UpdatePlanStatus(int PlanId, bool IsActive);
        Task<int> DeletePlanById(int planId);
        Task<int> AddEditPlan(PlanViewModel model);
        Task<PlanDetailsWithMasterDto> GetPlanInfoWithProgramAccount(int planId, int programId);
        Task<string> AddEditPlanDetails(PlanViewModel model, string clientIpAddress);
    }
}
