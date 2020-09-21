using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IPrograms : IFoundryRepositoryBase<Program>
    {
        Task<IEnumerable<ProgramDto>> GetPrograms();
        Task<int> CheckProgramExpiration(int programId, int userId = 0);
        Task<ProgramDto> GetProgramById(int programId);
        Task<Program> GetProgramDetailsById(int programId);
        Task<ProgramDto> GetUserProgramByUserId(int userId);
        Task<IEnumerable<ProgramLevelAdminDto>> GetOrganisationsAdminsList(int programId);
        Task<IEnumerable<TransactionViewDto>> GetTransaction(int orgType, int programId, DateTime? dateTime);
        Task<IEnumerable<GeneralSettingDto>> GetMaximumSheetRows();
        Task<string> AddEditProgramInfo(ProgramInfoDto model,string clientIpAddress);
        Task<IEnumerable<ProgramListDto>> GetAllPrograms(bool isActive, bool isDeleted, string roleName, int userId);
        Task<List<ProgramDrpDto>> GetProgramListBasedOnUserRole(int userId, string userRole, int organisationId);
        Task<List<ProgramInfoDto>> GetProgramListBasedOnIds(List<int> programIds);
        Task<IEnumerable<OrganisationsAdminsDto>> GetProgramAdminsList(int programId);
        Task<IEnumerable<ProgramListDto>> GetAllProgramsofPrgAdmin(bool isActive, bool isDeleted, int userId);
        Task<PrimaryMerchantDetail> GetPrimaryOrgNPrgDetailOfProgramAdminQuery(int userId);

        Task<string> RefreshPrograms(string organizantionName, int programId, int programCodeId, int accountId, 
            int planId, string name,string startDate, string endDate);
    }
}
