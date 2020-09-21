using Foundry.Domain.DbModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IUserPlanService : IFoundryRepositoryBase<UserPlans>
    {
        Task<int> AddUpdateUserPlans(List<UserPlans> model);
    }
}
