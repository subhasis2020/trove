using Foundry.Domain;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using static Foundry.Domain.Constants;

namespace Foundry.Services
{
    public class Programs : FoundryRepositoryBase<Program>, IPrograms
    {
        /*Creating database connection*/
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IOrganisationProgram _orgProgram;
        private readonly IPhotos _photos;
        private readonly ISharedJPOSService _sharedJPOSService;
        private readonly IProgramTypeService _programTypeService;

        /// <summary>
        /// Constructor for assigning value to database connection.
        /// </summary>
        /// <param name="databaseConnectionFactory"></param>
        public Programs(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, IOrganisationProgram orgProgram,
            IPhotos photos, ISharedJPOSService sharedJPOSService, IProgramTypeService programTypeService)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _orgProgram = orgProgram;
            _photos = photos;
            _sharedJPOSService = sharedJPOSService;
            _programTypeService = programTypeService;
        }
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get All the Programs present in the DB.
        /// </summary>
        /// <returns>Programs List</returns>
        public async Task<IEnumerable<ProgramDto>> GetPrograms()
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<ProgramDto>(SQLQueryConstants.GetAllProgramsQuery, new { })).ToList();
                    if (result.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            result[i].LogoPath = await _photos.GetAWSBucketFilUrl(result[i].LogoPath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.ProgramDefaultImage));
                        }
                    }

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        /// <summary>
        /// Checking the program expiration based on user and program detail.
        /// </summary>
        /// <param name="programId">ProgramId</param>
        /// <param name="userId">UserId</param>
        /// <returns>int (ProgramId)</returns>
        public async Task<int> CheckProgramExpiration(int programId, int userId)
        {
            var program = await GetSingleDataAsync(SQLQueryConstants.CheckProgramExpiryQuery, new { UserId = userId, ProgramId = programId });
            return program != null ? program.id : 0;
        }

        /// <summary>
        /// Gets the program detail based on its programId
        /// </summary>
        /// <param name="programId">ProgramId</param>
        /// <returns>Program</returns>
        public async Task<ProgramDto> GetProgramById(int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<ProgramDto>(SQLQueryConstants.GetProgramByIDQuery, new { ProgramId = programId });
                    if (result != null)
                    {
                        result.LogoPath = await _photos.GetAWSBucketFilUrl(result.LogoPath, string.Concat(_configuration["ServiceAPIURL"], Constants.ImagesDefault.ProgramDefaultImage));
                    }
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        /// <summary>
        /// Gets the program detail based on its programId
        /// </summary>
        /// <param name="programId">ProgramId</param>
        /// <returns>Program</returns>
        public async Task<Program> GetProgramDetailsById(int programId)
        {
            var obj = new { Id = programId };
            return await GetDataByIdAsync(obj);
        }

        /// <summary>
        /// Gets the program detail based on its programId
        /// </summary>
        /// <param name="programId">ProgramId</param>
        /// <returns>Program</returns>
        public async Task<ProgramDto> GetUserProgramByUserId(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<ProgramDto>(SQLQueryConstants.GetUserProgramByUserIDQuery, new { UserId = userId });
                    if (result != null)
                    {
                        result.LogoPath = await _photos.GetAWSBucketFilUrl(result.LogoPath, string.Concat(_configuration["ServiceAPIURL"], Constants.ImagesDefault.ProgramDefaultImage));
                    }
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        public async Task<IEnumerable<ProgramLevelAdminDto>> GetOrganisationsAdminsList(int programId)
        {
            try
            {
                using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
                {
                    try
                    {
                        var result = (await sqlConnection.QueryAsync<ProgramLevelAdminDto>(SQLQueryConstants.GetProgramLevelAdminListQuery, new { ProgramId = programId })).ToList();
                        if (result.Count > 0)
                        {
                            for (int i = 0; i < result.Count; i++)
                            {
                                result[i].UserImagePath = await _photos.GetAWSBucketFilUrl(result[i].UserImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
                            }
                        }

                        return result;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                        sqlConnection.Dispose();
                    }

                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<TransactionViewDto>> GetTransaction(int orgType, int programId, DateTime? dateTime)
        {
            try
            {
                using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
                {
                    try
                    {
                        return await sqlConnection.QueryAsync<TransactionViewDto>(SQLQueryConstants.GetTransactions, new { OrgType = orgType, ProgramId = programId, DateTime = dateTime });
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                        sqlConnection.Dispose();
                    }

                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> AddEditProgramInfo(ProgramInfoDto model, string clientIpAddress)
        {
            try
            {
                string prgTp = string.Empty;
                var id = model.JPOS_IssuerId;
                var prgType = (await _programTypeService.GetProgramTypesDetailByIds(new List<int> { model.ProgramTypeId.Value })).ToList();
                if (prgType.Count > 0)
                {
                    prgTp = prgType.Select(x => x.ProgramTypeName).FirstOrDefault();
                }
                var oIssuerJPOS = new IssuerJPOSDto()
                {
                    active = true,
                    programId = model.ProgramCodeId,
                    name = model.name,
                    tz = model.timeZone,
                    organizationId = model.OrganisationJPOSId,
                    address1 = model.address,
                    city = model.city,
                    country = model.country,
                    programType = prgTp,
                    state = model.state,
                    website = model.website,
                    zip = model.zipcode
                };
                int result = await _sharedJPOSService.PostRespectiveDataJPOS(JPOSAPIURLConstants.Issuers, oIssuerJPOS, id, clientIpAddress,JPOSAPIConstants.Issuers);
                if (result > 0 && model.id <= 0)
                {
                    model.JPOS_IssuerId = result.ToString();
                }
                var chkExist = await FindAsync(new { model.id });
                if (chkExist == null)
                    chkExist = new Program();
                if (model.name != null)
                    chkExist.name = model.name;
                chkExist.organisationId = model.organisationId;
                chkExist.AccountHolderGroups = model.AccountHolderGroups;
                chkExist.AccountHolderUniqueId = model.AccountHolderUniqueId;
                chkExist.address = model.address;
                chkExist.city = model.city;
                chkExist.colorCode = model.colorCode;
                chkExist.country = model.country;
                if (model.createdBy != null)
                    chkExist.createdBy = model.createdBy;
                if (model.createdDate != null)
                    chkExist.createdDate = model.createdDate;
                chkExist.customErrorMessaging = model.customErrorMessaging;
                chkExist.customInputMask = model.customInputMask;
                chkExist.customInstructions = model.customInstructions;
                chkExist.customName = model.customName;
                chkExist.description = model.description;
                if (model.isActive != null)
                    chkExist.isActive = model.isActive;
                if (model.isDeleted != null)
                    chkExist.isDeleted = model.isDeleted;
                if (model.modifiedBy != null)
                    chkExist.modifiedBy = model.modifiedBy;
                if (model.modifiedDate != null)
                    chkExist.modifiedDate = model.modifiedDate;
                if (model.ProgramCodeId != null)
                    chkExist.ProgramCodeId = model.ProgramCodeId;
                chkExist.programCustomFields = model.programCustomFields;
                chkExist.ProgramTypeId = model.ProgramTypeId;
                chkExist.state = model.state;
                chkExist.timeZone = model.timeZone;
                chkExist.website = model.website;
                chkExist.zipcode = model.zipcode;
                chkExist.id = model.id;
                chkExist.IsAllNotificationShow = model.IsAllNotificationShow;
                chkExist.IsRewardsShowInApp = model.IsRewardsShowInApp;
                chkExist.JPOS_IssuerId = model.JPOS_IssuerId;
                var resultPrg = await InsertOrUpdateAsync(chkExist, new { model.id });
                await _orgProgram.AddSpecificOrganisationProgram(resultPrg, model.organisationId);
                return Cryptography.EncryptPlainToCipher(Convert.ToString(resultPrg));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProgramListDto>> GetAllPrograms(bool isActive, bool isDeleted, string roleName, int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return await sqlConnection.QueryAsync<ProgramListDto>(SQLQueryConstants.GetAllProgramsListQuery, new
                    {
                        isActive,
                        isDeleted,
                        OrganisationType = (int)OrganasationType.University,
                        RoleName = roleName,
                        UserId = userId
                    });

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        public async Task<IEnumerable<ProgramListDto>> GetAllProgramsofPrgAdmin(bool isActive, bool isDeleted, int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return await sqlConnection.QueryAsync<ProgramListDto>(SQLQueryConstants.GetProgramsListOfPrgAdminQuery, new
                    {
                        isActive,
                        isDeleted,
                        OrganisationType = (int)OrganasationType.University,
                        UserId = userId
                    });

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
        }

        public async Task<IEnumerable<GeneralSettingDto>> GetMaximumSheetRows()
        {
            try
            {
                using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
                {
                    try
                    {
                        return await sqlConnection.QueryAsync<GeneralSettingDto>(SQLQueryConstants.GetMaximumSheetRows);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                        sqlConnection.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ProgramDrpDto>> GetProgramListBasedOnUserRole(int userId, string userRole, int organisationId)
        {
            List<ProgramDrpDto> prgsInfo = new List<ProgramDrpDto>();
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    if (userRole == Roles.SuperAdmin.Trim().ToLower())
                    {
                        prgsInfo = (sqlConnection.QueryAsync<ProgramDrpDto>(SQLQueryConstants.GetProgramListByOrgIdQuery, new { IsActive = true, IsDeleted = false, UserId = userId, OrganisationId = organisationId }).Result).ToList();
                    }
                    else if (userRole == Roles.OrganizationFull.Trim().ToLower() || userRole == Roles.OrganizationReporting.Trim().ToLower())
                    {
                        prgsInfo = (sqlConnection.QueryAsync<ProgramDrpDto>(SQLQueryConstants.GetProgramLstForRoleOrganisationAdmin, new { IsActive = true, IsDeleted = false, UserId = userId, OrganisationId = organisationId }).Result).ToList();
                    }
                    else if (userRole == Roles.ProgramFull.Trim().ToLower() || userRole == Roles.ProgramReporting.Trim().ToLower())
                    {
                        prgsInfo = (sqlConnection.QueryAsync<ProgramDrpDto>(SQLQueryConstants.GetProgramLstForRolePrgAdmin, new { IsActive = true, IsDeleted = false, UserId = userId, OrganisationId = organisationId }).Result).ToList();
                    }
                    else
                    {
                        // Merchant full OR merchant reporting.
                        prgsInfo = (sqlConnection.QueryAsync<ProgramDrpDto>(SQLQueryConstants.GetProgramLstForRoleMerchant, new { IsActive = true, IsDeleted = false, UserId = userId }).Result).ToList();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return prgsInfo;
        }

        public async Task<List<ProgramInfoDto>> GetProgramListBasedOnIds(List<int> programIds)
        {
            List<ProgramInfoDto> prgsInfo = new List<ProgramInfoDto>();
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    // Merchant full OR merchant reporting.
                    prgsInfo = (sqlConnection.QueryAsync<ProgramInfoDto>(SQLQueryConstants.GetProgramListBasedOnIdsQuery, new { IsActive = true, IsDeleted = false, ProgramIds = programIds }).Result).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return prgsInfo;
        }

        public async Task<IEnumerable<OrganisationsAdminsDto>> GetProgramAdminsList(int programId)
        {
            try
            {
                using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
                {
                    try
                    {
                        var userAdminsOrg = (await sqlConnection.QueryAsync<OrganisationsAdminsDto>(SQLQueryConstants.GetProgramAdminsListForPrgRoleQuery, new { ProgramId = programId })).ToList();
                        if (userAdminsOrg.Count > 0)
                        {
                            for (int i = 0; i < userAdminsOrg.Count; i++)
                            {
                                userAdminsOrg[i].UserImagePath = await _photos.GetAWSBucketFilUrl(userAdminsOrg[i].UserImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
                            }

                        }
                        return userAdminsOrg;

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        sqlConnection.Close();
                        sqlConnection.Dispose();
                    }

                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PrimaryMerchantDetail> GetPrimaryOrgNPrgDetailOfProgramAdminQuery(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new { IsActive = true, IsDeleted = false, UserId = userId };
                    var organisation = (await sqlConnection.QuerySingleOrDefaultAsync<PrimaryMerchantDetail>(SQLQueryConstants.GetPrimaryOrgNPrgDetailOfProgramAdminQuery, obj));
                    return organisation;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        public async Task<string> RefreshPrograms(string organizantionName, int programId, int programCodeId, 
            int accountId, int planId, string name, string startDate, string endDate)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new {
                        OrganizantionName = organizantionName,
                        ProgramId = programId,
                        ProgramCodeId = programCodeId,
                        AccountId = accountId,
                        PlanId= planId,
                        Name= name,
                        StartDate= startDate,
                        EndDate= endDate
                    };
                    await sqlConnection.QueryAsync(SQLQueryConstants.Sp_UpdateIssuer, obj,commandType:System.Data.CommandType.StoredProcedure);
                    return "success";
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }
    }
}
