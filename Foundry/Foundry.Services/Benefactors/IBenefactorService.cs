using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IBenefactorService : IFoundryRepositoryBase<BenefactorUsersLinking>
    {
        Task<List<UserRelations>> GetRelations();
        Task<UserRelations> GetRelationById(int relationshipId);
        Task<User> CheckExistingBenefactor(string email);
        Task<BenefactorUsersLinking> CheckConnectionUserBenefactor(int userId, int benefactorId);
        Task<int> AddUpdateBenefactorUserLinking(int userId, int benefactorId, int relationshipId);
        Task<List<BenefactorDto>> GetUserConnections(int userId, int programId);
        Task<int> DeleteUserConnection(int type, int userId, int benefactorId);
        Task<int> ReloadBalanceRequest(int userId, int benefactorId, int programId, decimal amount = 0);
        Task<int> PartnerReloadBalanceRequest(int userId, int benefactorId, int programId, string Message, decimal amount = 0);
        Task<BenefactorUsersLinking> CheckForExistingUserLinkingWithEmail(int userId, string benefactorEmail);
        Task<List<LinkedUsersTransactionsDto>> GetLinkedUsersTransactions(int linkedUserId, DateTime? dateMonth, string plan);
        Task<List<LinkedUsersDto>> GetLinkedUsersOfBenefactor(int benefactorId);
        Task<List<LinkedUsersDto>> BenefectorDetails(int benefactorId);
        Task<List<LinkedUsersDto>> GetLinkedUsersInformationOfBenefactor(int benefactorId);
        Task<int> ReloadUserBalance(ReloadRequestModel model);
        Task<decimal> GetRemainingBalanceOfUser(int userId);
        Task<ReloadRules> GetReloadRuleOfUser(int userId, int benefactorId);
        Task<int> DeleteLinkedAccounts(int type, int userId, int benefactorId);
        Task<PrivacyDto> GetPrivacySettings(int userId, int programId);
        Task<PrivacyDto> UdatePrivacySettings(bool isOnlyMe, int id, int userId, int programId, bool status);
        Task<int> AddUpdateReloadrule(ReloadRulesModel model);
        Task<List<ReloadRuleForDisplay>> GetAllReloadRuleforUser(int userId);
        Task<IEnumerable<ReloadRequestDto>> CancelReloadRule(ReloadRequestModel model);
        Task sendReloadSetUpNotification(string benefactor, string vPartnerUserId, int userid, decimal vAmountAdded, decimal ThresholdAmount);
        Task<List<ReloadRules>> GetUserReloadRule(int userId);
        Task sendStatuschangeNotification(string vPartnerUserId, int userid, string previousStatus, string currentStatus);
        Task<ReloadRules> GetUserReloadRuleForTrigger(int userId, decimal Balance);
    }
}
