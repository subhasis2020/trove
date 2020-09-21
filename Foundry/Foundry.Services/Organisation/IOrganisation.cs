using Foundry.Domain.ApiModel;
using org = Foundry.Domain.DbModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundry.Domain.Dto;
using System;

namespace Foundry.Services
{
    public interface IOrganisation : IFoundryRepositoryBase<org.Organisation>
    {
        Task<List<OrganisationDto>> GetOrganisation(int accountTypeId, int userId, string latlong, int programId);
        Task<OrganisationDto> GetOrganisationDetails(int organisationId, int programId, int userId, int accountTypeId);
        Task<decimal?> GetRemainingMeals(int userId, int programId);
        Task<OfferDto> GetOrganisationPromotionDetails(int organisationId, int offerId, int programId, int userId, int accountTypeId);
        Task<List<OfferDto>> GetOffersOfMerchants(int accountTypeId, int userId, string latlong, int programId);
        Task<org.Organisation> GetOrganisationDetailsById(int organisationId);
        Task<org.Organisation> GetMasterOrganisation();
        Task<IEnumerable<org.Organisation>> GetOrganisationsListByTypeWithSearch(int organisationType, bool isActive, bool isDeleted, string searchOrganisation, string roleName, int userId);
        Task<OrgnisationWithProgramTypeDto> GetOrganisationInfoWithProgramTypes(int organisationType, int organisationId);
        Task<IEnumerable<MerchantDto>> GetAllMerchantsWithTransaction(int programId);
        Task<IEnumerable<OrganisationsAdminsDto>> GetOrganisationsAdminsList(int organisationId);
        Task<int> UpdateOrganisationAdminStatus(int userId, bool IsActive);
        Task<int> AddEditOrganisation(OrganisationViewModel model);
        Task<int> DeleteOrganisation(int organisationId);
        Task<OrganisationDetailWithMasterDto> GetOrganisationInfoWithProgramNAccountType(int organisationType, int organisationId, int universityId, int programId);
        Task<string> AddUpdateMerchantDetailInfo(OrganisationViewModel model, string clientIpAddress);
        Task<List<MerchantTransactionDto>> GetAllMerchantsTransaction(int organisationId, DateTime? dateMonth);
        Task<List<MerchantRewardDto>> GetMerchantRewardList(int organisationId);
        Task<MerchantRewardDto> GetMerchantRewardInfoWithBusinessType(int organisationId, int promotionId);
        Task<string> AddUpdateMerchantRewardInfo(MerchantRewardViewModel model);
        Task<int> DeleteOrganisationById(int organisationId);
        Task<string> AddUpdateOrganisationBasicDetail(OrganisationViewModel model,string clientIpAddress);
        Task<MerchantBusinessInfoDto> GetMerchantBusinessInfo(int organisationId);
        Task<List<WeekDayDto>> GetWeekDaysList();
        Task<List<DwellTimeDto>> GetDwellTimeList();
        Task<List<TerminalTypeDto>> GetTerminalTypeList();
        Task<IEnumerable<MerchantDto>> GetAllMerchantsByProgram(int programId);

        Task<int> AddEditOrganisationBusinessInfo(OrganisationViewModel model);
        Task<List<OrganisationDrpDto>> GetOrganisationListBasedOnUserRole(int userId, string userRole);
        Task<IEnumerable<OrganisationsAdminsDto>> GetMerchantAdminsList(int organisationId);
        Task<IEnumerable<MerchantDto>> GetAllMerchantsWithDropdwn(int programId, int userId, string role);
        Task<IEnumerable<MerchantDto>> GetAllMerchantsByMerchantAdminId(int userId);
        Task<PrimaryMerchantDetail> GetPrimaryOrgNPrgDetailOfMerchantAdminQuery(int userId);

    }
}
