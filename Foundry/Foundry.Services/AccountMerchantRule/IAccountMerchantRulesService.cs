using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IAccountMerchantRulesService : IFoundryRepositoryBase<AccountMerchantRules>
    {
        Task<AccountMerchantRuleDto> GetAccountMerchantRule(int programId, string businessTypeId, string accountTypeId, int accountId);
        Task<string> AddEditAccountMerchantRules(AccountMerchantRuleViewModel model);
        Task<string> EditAccountMerchantRuleDetails(List<AccountMerchantRuleAndDetailViewModel> model);
    }
}
