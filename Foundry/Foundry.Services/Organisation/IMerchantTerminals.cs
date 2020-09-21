using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;

namespace Foundry.Services
{
    public interface IMerchantTerminals : IFoundryRepositoryBase<MerchantTerminal>
    {
        Task<int> AddEditMerchantTerminal(OrganisationMerchantTerminalModel model);
        Task<int> AddEditMerchantTerminal(List<OrganisationMerchantTerminalModel> model,string clientIpAddress);
    }
}
