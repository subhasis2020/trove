using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;

namespace Foundry.Services


{
    public interface IMealPeriodService : IFoundryRepositoryBase<OrganisationMealPeriod>
    {
        Task<int> AddEditMealPeriod(List<OrganisationMealPeriodModel> model);
    }
}
