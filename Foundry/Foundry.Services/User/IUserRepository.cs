using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IUserRepository : IFoundryRepositoryBase<User>
    {
        Task<List<User>> GetAllUsers();
        Task<UserDto> GetUserById(int userId);
        Task<UserDto> RegisterUserWithProgram(User model, int programCodeId, string photoPath = null);
        Task<UserDto> GetUserWithProgramCode(int userId, int programId);
        Task<string> CheckSessionId(int userId, string sessionId);
        Task<IEnumerable<User>> GetUsersDetailByIds(List<int> userIdsList);
        Task<bool?> CheckUserInactivity(string userEmail);
        Task<User> CheckUserByEmail(string userEmail);
        Task<int> AddUpdateUser(User model);
        Task<IEnumerable<User>> GetUsersDetailByEmailIds(List<string> userEmailIdsList);
        Task<UserDto> GetUserBenefactorById(int userId);
        Task<User> CheckPhoneNumberExistence(string phoneNumber = null);
        Task<int> UpdateUserDeviceAndLocationInfo(User model);
        Task<UserDto> EditUserProfile(User model, int programCodeId, string photoPath = null);
        Task<UserWithProgramTypeDto> GetUserAdminInfoWithProgramTypes(int userId);
        Task<int> AddUpdateAdminUser(OrganisationAdminViewDetail model); 
        Task<int> AddUpdateProgramLevelAdminUser(ProgramLevelAdminViewDetail model);
        Task<int> DeleteAdminUser(int userId);
        Task<User> CheckUserExistence(string userEmail);
        Task<int> AddUserFromExcel(User model);
        Task<IEnumerable<AccountHolderDto>> GetAccountHoldersList(int organisationId, int programId, string searchValue, int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection, int? planId);
        Task<int> AddUpdateAccountHolderDetail(User model, List<UserPlans> programPackageIds, string userImagePath,string clientIpAddress,string issuerId);
        Task<AccountHolderDto> GetUserInfoWithUserPlans(int userId, int programId);
        Task<IEnumerable<User>> GetUsersDetailByUserCode(List<string> UserIds);
        Task<List<UserDto>> GetUserByIdWithProgramDetail(List<int> userIds);
        Task<UserDto> GetUserWithProgramCodeBeforeRegister(int userId, int programId);
        Task<int> AddUpdateUserInvitationStatus(User model);
        Task<List<UserDeviceDto>> GetUserDeviceTokenBasedOnProgram(int programId);
        Task<List<UserDeviceDto>> GetUserDeviceTokenByUserIdNdProgram(int programId, int userId);
        Task<int> UpdateUserCardHolderAgreementReadDetail(User model);
        Task<List<UserDeviceDto>> GetUserDeviceTokenBasedOnProgramLst(List<int> programIds);
        Task<int> RegisterPartnerUser(User model);
        Task<UserDto> GetUserbyBiteUserId(string biteUserId,int PartnerId);
        Task<UserDto> GetUserInfoById(int userId);
        Task<string> GetUserType(int Id);

        Task<User> CheckUserByPartnerId(string partnerUserId, int partnerId);
        Task<List<I2CCardBankAccountModel>> GetUserCardwithBankAccount(int byUserId, int toUserId);
        Task<string> GetBiteUserLoyaltyTrackingBalance(int Id);

        Task<IEnumerable<AccountHolderDto>> GetAccountHoldersListByOrganization(int organisationId, int programId, string searchValue, int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection, int? planId);

    }
}