using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IProgramAccountService : IFoundryRepositoryBase<ProgramAccounts>
    {
        Task<IEnumerable<AccountListingDto>> GetAccountListing(int programId);
        Task<int> UpdateProgramAccountStatus(int Id, bool IsActive);
        Task<int> DeleteAccountById(int planId);
        Task<ProgramAccountDetailsWithMasterDto> GetProgramAccountInfoWithAccountType(int Id);
        Task<List<PassTypeDto>> GetPassTypeList();
        Task<List<ResetPeriodDto>> GetResetPeriodList();
        Task<List<ExchangeResetPeriodDto>> ExchangeResetPeriodList();
        Task<List<WeekDayDto>> GetWeekDaysList();
        Task<string> AddEditProgramAccoutDetails(ProgramAccountViewModel model, string clientIpAddress);
        Task<AccountInfoDto> GetAccountDetailNCheckForBranding(int accountId);
        Task<IEnumerable<AccountListingDto>> GetAccountBasedOnPlanNProgramSelection(int programId, int planId);
        Task<List<AccountInfoDto>> GetAccountsDetailsByIds(List<int> accountIds);
        Task<List<AccountListingDto>> GetProgramAccountsDropdownForUser(int userId);
        Task<AccountPlanProgramDto> GetUserProgramPlanNOrgByProgramAccountId(int programAccountId,int userId);
    }
}
