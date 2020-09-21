using Dapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using org = Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using System.Data;

namespace Foundry.Services
{
    public class Organisation : FoundryRepositoryBase<org.Organisation>, IOrganisation
    {
        private readonly IGeneralSettingService _setting;
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IOrganisationProgramTypeService _orgProgramType;
        private readonly IPrograms _program;
        private readonly IMapper _mapper;
        private readonly IOrganisationProgram _orgProgram;
        private readonly IPhotos _photos;
        private readonly IPromotions _promotion;
        private readonly IProgramAccountLinkService _programAccountLinkService;
        private readonly ISharedJPOSService _sharedJPOSService;
        private readonly IProgramTypeService _programTypeService;
        public Organisation(IGeneralSettingService setting, IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration,
            IPrograms program, IOrganisationProgram orgProgram, IMapper mapper, IPhotos photos, IPromotions promotion, IOrganisationProgramTypeService orgProgramType
            , IProgramAccountLinkService programAccountLinkService, ISharedJPOSService sharedJPOSService,
            IProgramTypeService programTypeService)
       : base(databaseConnectionFactory)
        {
            _setting = setting;
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _program = program;
            _orgProgram = orgProgram;
            _mapper = mapper;
            _photos = photos;
            _promotion = promotion;
            _orgProgramType = orgProgramType;
            _programAccountLinkService = programAccountLinkService;
            _sharedJPOSService = sharedJPOSService;
            _programTypeService = programTypeService;
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region API Methods
        /// <summary>
        /// This method will get the list of Merchants.
        /// </summary>
        /// <param name="accountTypeId"></param>
        /// <param name="userId"></param>
        /// <param name="latlong"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<List<OrganisationDto>> GetOrganisation(int accountTypeId, int userId, string latlong, int programId)
        {
            List<OrganisationDto> organisation = new List<OrganisationDto>();
            string timezone = "";
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        ProgramId = programId,
                        AccountTypeId = accountTypeId,
                        UserId = userId,
                        OrganisationType = OrganasationType.Merchant,
                        Latlon = latlong,
                        IsActive = true,
                        IsDeleted = false
                    };
                    organisation = (await sqlConnection.QueryAsync<OrganisationDto>(SQLQueryConstants.GetOrganisationQuery, obj)).ToList();
                    if (organisation.Count > 0)
                    {
                        timezone = (await _program.AllAsync()).ToList().FirstOrDefault(x => x.id == programId).timeZone;
                        organisation.ToList().ForEach(x => x.ClosingStatus = getClosingStatus(x.Id, timezone, x.IsClosed));
                        for (int i = 0; i < organisation.Count; i++)
                        {
                            organisation[i].LogoPath = await _photos.GetAWSBucketFilUrl(organisation[i].LogoPath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.OrganisationDefaultImage));
                        }

                        organisation = organisation.OrderBy(x => x.Distance).ToList();
                    }
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
            return organisation;
        }

        /// <summary>
        /// This method will get the Merchants Details.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="accountTypeId"></param>
        /// <param name="userId"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<OrganisationDto> GetOrganisationDetails(int organisationId, int programId, int userId, int accountTypeId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        Id = organisationId,
                        ProgramId = programId,
                        AccountTypeId = accountTypeId,
                        UserId = userId,
                        OrganisationType = OrganasationType.Merchant,
                        IsActive = true,
                        IsDeleted = false
                    };

                    var organisation = (await sqlConnection.QueryAsync<OrganisationDto>(SQLQueryConstants.GetOrganisationDetailByIdQuery, obj)).FirstOrDefault();
                    if (organisation != null)
                    {
                        var timezone = (await _program.AllAsync()).ToList().FirstOrDefault(x => x.id == programId).timeZone;

                        organisation.ClosingStatus = getClosingStatus(organisationId, timezone, organisation.IsClosed);
                        organisation.OpenCloseTime = !string.IsNullOrEmpty(organisation.OpenCloseTime) ? organisation.OpenCloseTime.ToLower(CultureInfo.InvariantCulture) : "";
                        organisation.MaxSeatCapacityOccupied = GetOrgMaxCapacity(organisation.dwellTime.HasValue ? organisation.dwellTime.Value : 0, organisation.MaxCapacity.HasValue ? organisation.MaxCapacity.Value : 0);
                        organisation.LogoPath = await _photos.GetAWSBucketFilUrl(organisation.LogoPath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.OrganisationDefaultImage));
                        OrganisationScheduleAndHolidayDto ogranisationScheduleHoliday = new OrganisationScheduleAndHolidayDto();
                        using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetOrganisationScheduleByIdSP, new { Id = organisationId, IsActive = true, IsDeleted = false }, commandType: CommandType.StoredProcedure).Result)
                        {
                            try
                            {
                                ogranisationScheduleHoliday.OrganisationSchedule = new List<OrganisationScheduleDto>();
                                ogranisationScheduleHoliday.HolidaySchedule = new HolidayScheduleDto();
                                ogranisationScheduleHoliday.OrganisationSchedule = multi.Read<OrganisationScheduleDto>().ToList();
                                var holidaySchedule = multi.Read<HolidayScheduleDto>().ToList();
                                if (holidaySchedule.Count > 0)
                                {
                                    if (holidaySchedule.Any(x => x.IsForHolidayNameToShow == true))
                                    { ogranisationScheduleHoliday.HolidaySchedule = holidaySchedule.Where(x => x.IsForHolidayNameToShow == true).FirstOrDefault(); }
                                    else { ogranisationScheduleHoliday.HolidaySchedule = holidaySchedule.FirstOrDefault(); }

                                }
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

                        organisation.organisationScheduleAndHoliday = ogranisationScheduleHoliday;
                    }

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
        /// <summary>
        /// This method will get the List of Merchant's promotion.
        /// </summary>
        /// <param name="accountTypeId"></param>
        /// <param name="userId"></param>
        /// <param name="latlong"></param>
        /// <param name="programId"></param>
        public async Task<List<OfferDto>> GetOffersOfMerchants(int accountTypeId, int userId, string latlong, int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var radiussetting = Convert.ToInt32((await _setting.GetGeneratSettingValueByKeyGroup(GeneralSettingsConstants.Radius)).FirstOrDefault().Value);
                    var obj = new
                    {
                        ProgramId = programId,
                        AccountTypeId = accountTypeId,
                        UserId = userId,
                        OrganisationType = OrganasationType.Merchant,
                        Latlon = latlong,
                        IsActive = true,
                        IsDeleted = false
                    };
                    var organisationOffer = (await sqlConnection.QueryAsync<OfferDto>(SQLQueryConstants.GetOffersOfMerchantsQuery, obj)).ToList();
                    if (organisationOffer.Count > 0)
                    {
                        for (int i = 0; i < organisationOffer.Count; i++)
                        {
                            organisationOffer[i].OfferImagePath = await _photos.GetAWSBucketFilUrl(organisationOffer[i].OfferImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.FoodDefaultImage));
                        }

                    }
                    return organisationOffer.Where(x => x.Distance <= radiussetting).OrderBy(x => x.Distance).ToList();
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
        /// This method will get the Merchant's promotion Details .
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="offerId"></param>
        /// <param name="programId"></param>
        /// <param name="userId"></param>
        /// <param name="accountTypeId"></param>
        /// <returns></returns>
        public async Task<OfferDto> GetOrganisationPromotionDetails(int organisationId, int offerId, int programId, int userId, int accountTypeId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        Id = organisationId,
                        OfferId = offerId,
                        ProgramId = programId,
                        AccountTypeId = accountTypeId,
                        UserId = userId,
                        OrganisationType = OrganasationType.Merchant,
                        IsActive = true,
                        IsDeleted = false
                    };
                    var organisationOffer = (await sqlConnection.QueryAsync<OfferDto>(SQLQueryConstants.GetOffersOfMerchantsDetailByIdQuery, obj)).FirstOrDefault();
                    if (organisationOffer != null)
                    {
                        organisationOffer.OfferImagePath = await _photos.GetAWSBucketFilUrl(organisationOffer.OfferImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.FoodDefaultImage));
                        OrganisationScheduleAndHolidayDto ogranisationScheduleHoliday = new OrganisationScheduleAndHolidayDto();
                        using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetOrganisationScheduleByIdSP, new { Id = organisationId, IsActive = true, IsDeleted = false }, commandType: CommandType.StoredProcedure).Result)
                        {
                            try
                            {
                                ogranisationScheduleHoliday.OrganisationSchedule = new List<OrganisationScheduleDto>();
                                ogranisationScheduleHoliday.HolidaySchedule = new HolidayScheduleDto();
                                ogranisationScheduleHoliday.OrganisationSchedule = multi.Read<OrganisationScheduleDto>().ToList();
                                var holidaySchedule = multi.Read<HolidayScheduleDto>().ToList();
                                if (holidaySchedule.Count > 0)
                                {
                                    if (holidaySchedule.Any(x => x.IsForHolidayNameToShow == true))
                                    { ogranisationScheduleHoliday.HolidaySchedule = holidaySchedule.Where(x => x.IsForHolidayNameToShow == true).FirstOrDefault(); }
                                    else { ogranisationScheduleHoliday.HolidaySchedule = holidaySchedule.FirstOrDefault(); }

                                }

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

                        organisationOffer.organisationScheduleAndHoliday = ogranisationScheduleHoliday;
                    }
                    return organisationOffer;
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

        public async Task<decimal?> GetRemainingMeals(int userId, int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        ProgramId = programId,
                        UserId = userId,
                        IsActive = true,
                        IsDeleted = false
                    };
                    var totalPasses = (await sqlConnection.QueryAsync<int>(SQLQueryConstants.GetNoOfRemainingMealPassesQuery, obj)).FirstOrDefault();
                    return totalPasses;
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
        /// This method will get the Data for Get Help Screen.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        public async Task<org.Organisation> GetOrganisationDetailsById(int organisationId)
        {
            var obj = new
            {
                Id = organisationId,
            };
            return await GetDataByIdAsync(obj);
        }

        /// <summary>
        /// This method will get the Data For Foundry having isMasterTrue.
        /// </summary>
        /// <returns></returns>
        public async Task<org.Organisation> GetMasterOrganisation()
        {
            var obj = new
            {
                IsMaster = true,
                IsActive = true,
                IsDeleted = false
            };
            return await GetDataByIdAsync(obj);
        }

        public async Task<IEnumerable<org.Organisation>> GetOrganisationsListByTypeWithSearch(int organisationType, bool isActive, bool isDeleted, string searchOrganisation, string roleName, int userId)
        {
            try
            {

                return await GetDataAsync(SQLQueryConstants.GetOrganisationListByTypeQuery, new { IsActive = isActive, IsDeleted = isDeleted, OrganisationType = organisationType, SearchName = "%" + searchOrganisation + "%", RoleName = roleName, UserId = userId });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrgnisationWithProgramTypeDto> GetOrganisationInfoWithProgramTypes(int organisationType, int organisationId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {

                using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetMultipleOrganisationDetailNOrganisationProgramQuery, new { OrganisationType = organisationType, OrganisationId = organisationId }).Result)
                {
                    try
                    {
                        var orgInfo = multi.Read<OrgnisationWithProgramTypeDto>().FirstOrDefault();
                        var orgProgranType = multi.Read<ProgramTypeIdDto>().ToList();
                        orgInfo.OrganisationProgramType = orgProgranType;
                        return orgInfo;
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

        public async Task<IEnumerable<OrganisationsAdminsDto>> GetOrganisationsAdminsList(int organisationId)
        {
            try
            {
                using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
                {
                    try
                    {
                        var userAdminsOrg = (await sqlConnection.QueryAsync<OrganisationsAdminsDto>(SQLQueryConstants.GetOrganisationAdminsListQuery, new { OrganisationID = organisationId })).ToList();
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

        public async Task<IEnumerable<OrganisationsAdminsDto>> GetMerchantAdminsList(int organisationId)
        {
            try
            {
                using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
                {
                    try
                    {
                        var userAdminsOrg = (await sqlConnection.QueryAsync<OrganisationsAdminsDto>(SQLQueryConstants.GetMerchantAdminsListQuery, new { OrganisationID = organisationId })).ToList();
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


        /// <summary>
        /// This method will get the Data For Foundry having isMasterTrue.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MerchantDto>> GetAllMerchantsWithTransaction(int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new { IsActive = true, IsDeleted = false, OrganisationType = Convert.ToInt32(OrganasationType.Merchant), ProgramId = programId };
                    var organisation = (await sqlConnection.QueryAsync<MerchantDto>(SQLQueryConstants.GetAllMerchantsWithTransactionsQuery, obj)).ToList();
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


        /// <summary>
        /// This method will get the Data For Foundry having isMasterTrue.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MerchantDto>> GetAllMerchantsWithDropdwn(int programId, int userId, string role)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                List<string> merchantRole = new List<string>() { Roles.MerchantFull.ToLower(), Roles.MerchantReporting.ToLower() };
                try
                {
                    if (merchantRole.Contains(role.ToLower()))
                    {
                        var obj = new { IsActive = true, IsDeleted = false, OrganisationType = Convert.ToInt32(OrganasationType.Merchant), ProgramId = programId, UserId = userId };
                        var organisation = (await sqlConnection.QueryAsync<MerchantDto>(SQLQueryConstants.GetAllMerchantsWithMerchantAdminQuery, obj)).ToList();
                        return organisation;
                    }
                    else
                    {
                        var obj = new { IsActive = true, IsDeleted = false, OrganisationType = Convert.ToInt32(OrganasationType.Merchant), ProgramId = programId };
                        var organisation = (await sqlConnection.QueryAsync<MerchantDto>(SQLQueryConstants.GetAllMerchantsWithTransactionsQuery, obj)).ToList();
                        return organisation;
                    }
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
        public async Task<IEnumerable<MerchantDto>> GetAllMerchantsWithMerchantAdmin(int programId, int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new { IsActive = true, IsDeleted = false, OrganisationType = Convert.ToInt32(OrganasationType.Merchant), ProgramId = programId, UserId = userId };
                    var organisation = (await sqlConnection.QueryAsync<MerchantDto>(SQLQueryConstants.GetAllMerchantsForMerchantAdminQuery, obj)).ToList();
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

        public async Task<IEnumerable<MerchantDto>> GetAllMerchantsByMerchantAdminId(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new { IsActive = true, IsDeleted = false, OrganisationType = Convert.ToInt32(OrganasationType.Merchant), UserId = userId };
                    var organisation = (await sqlConnection.QueryAsync<MerchantDto>(SQLQueryConstants.GetAllMerchantsByMerchantAdminIdQuery, obj)).ToList();
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

        public async Task<PrimaryMerchantDetail> GetPrimaryOrgNPrgDetailOfMerchantAdminQuery(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new { IsActive = true, IsDeleted = false, OrganisationType = Convert.ToInt32(OrganasationType.Merchant), UserId = userId };
                    var organisation = (await sqlConnection.QuerySingleOrDefaultAsync<PrimaryMerchantDetail>(SQLQueryConstants.GetPrimaryOrgNPrgDetailOfMerchantAdminQuery, obj));
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
        public async Task<IEnumerable<MerchantDto>> GetAllMerchantsByProgram(int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new { IsActive = true, IsDeleted = false, OrganisationType = Convert.ToInt32(OrganasationType.Merchant), ProgramId = programId };
                    var organisation = (await sqlConnection.QueryAsync<MerchantDto>(SQLQueryConstants.GetAllMerchantsByProgram, obj)).ToList();
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
        public async Task<int> AddEditOrganisation(OrganisationViewModel model)
        {
            try
            {
                var chkExist = await FindAsync(new { id = model.id });
                if (chkExist == null)
                {
                    chkExist = new org.Organisation();
                }
                if (model.addressLine1 != null)
                {
                    chkExist.addressLine1 = model.addressLine1;
                }
                chkExist.addressLine2 = model.addressLine2;
                if (model.businessTypeId != null)
                {
                    chkExist.businessTypeId = model.businessTypeId;
                }
                if (model.city != null)
                {
                    chkExist.city = model.city;
                }
                if (model.ContactName != null)
                {
                    chkExist.ContactName = model.ContactName;
                }
                chkExist.contactNumber = model.contactNumber;
                chkExist.ContactTitle = model.ContactTitle;
                chkExist.country = model.country;
                if (model.createdBy != null)
                {
                    chkExist.createdBy = model.createdBy;
                }
                if (model.createdDate != null)
                {
                    chkExist.createdDate = model.createdDate;
                }
                chkExist.description = model.description;
                if (model.emailAddress != null)
                {
                    chkExist.emailAddress = model.emailAddress;
                }
                chkExist.facebookURL = model.facebookURL;
                chkExist.getHelpEmail = model.getHelpEmail;
                chkExist.getHelpPhone1 = model.getHelpPhone1;
                chkExist.getHelpPhone2 = model.getHelpPhone2;
                if (model.isActive != null)
                {
                    chkExist.isActive = model.isActive;
                }
                if (model.isClosed != null)
                {
                    chkExist.isClosed = model.isClosed;
                }
                if (model.isDeleted != null)
                {
                    chkExist.isDeleted = model.isDeleted;
                }
                if (model.isMapVisible != null)
                {
                    chkExist.isMapVisible = model.isMapVisible;
                }

                chkExist.isMaster = model.isMaster;
                chkExist.location = model.location;
                if (model.MerchantId != null)
                {
                    chkExist.MerchantId = model.MerchantId;
                }
                chkExist.maxCapacity = model.maxCapacity;
                if (model.modifiedBy != null)
                {
                    chkExist.modifiedBy = model.modifiedBy;
                }
                if (model.modifiedDate != null)
                {
                    chkExist.modifiedDate = model.modifiedDate;
                }
                chkExist.name = model.name;
                if (model.organisationType > 0)
                {
                    chkExist.organisationType = model.organisationType;
                }
                chkExist.skypeHandle = model.skypeHandle;
                chkExist.state = model.state;
                chkExist.twitterURL = model.twitterURL;
                chkExist.websiteURL = model.websiteURL;
                chkExist.zip = model.zip;
                chkExist.OrganisationSubTitle = model.OrganisationSubTitle;
                if (model.dwellTime != null)
                {
                    chkExist.dwellTime = model.dwellTime;
                }
                if (model.JPOS_OrgId != null)
                {
                    chkExist.JPOS_MerchantId = model.JPOS_OrgId;
                }
                chkExist.InstagramHandle = model.InstagramHandle;
                chkExist.isTrafficChartVisible = model.isTrafficChartVisible;
                chkExist.id = model.id;
                return await InsertOrUpdateAsync(chkExist, new { model.id });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> AddEditOrganisationBusinessInfo(OrganisationViewModel model)
        {
            try
            {
                var chkExist = await FindAsync(new { id = model.id });
                if (chkExist == null)
                {
                    chkExist = new org.Organisation();
                }
                if (model.isClosed != null)
                {
                    chkExist.isClosed = model.isClosed;
                }
                chkExist.maxCapacity = model.maxCapacity;
                if (model.modifiedBy != null)
                {
                    chkExist.modifiedBy = model.modifiedBy;
                }
                if (model.modifiedDate != null)
                {
                    chkExist.modifiedDate = model.modifiedDate;
                }
                if (model.dwellTime != null)
                {
                    chkExist.dwellTime = model.dwellTime;
                }
                chkExist.isTrafficChartVisible = model.isTrafficChartVisible;
                chkExist.id = model.id;
                return await InsertOrUpdateAsync(chkExist, new { model.id });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> AddUpdateOrganisationBasicDetail(OrganisationViewModel model, string clientIpAddress)
        {
            try
            {
                var id = model.JPOS_OrgId;
                string programTypeNames = string.Empty;
                var programType = (await _programTypeService.GetProgramTypes()).Where(x => model.OrganisationProgramTypes.ToList().Select(m => m.ProgramTypeId).Contains(x.Id)).ToList();
                if (programType.Count > 0) {
                     programTypeNames = string.Join(", ", programType.Select(x => x.ProgramTypeName));
                }
                var oOrgJPOS = new OrganizationJPOSDto()
                {
                    active = true,
                    address1 = model.addressLine1,
                    address2 = model.addressLine2,
                    city = model.city,
                    contactEmail = model.emailAddress,
                    contactName = model.ContactName,
                    contactNumber = model.contactNumber,
                    contactTitle = model.ContactTitle,
                    description = model.description,
                    facebook = model.facebookURL,
                    name = model.name,
                    skype = model.skypeHandle,
                    title = model.OrganisationSubTitle,
                    twitter = model.twitterURL,
                    type = programTypeNames,
                    website = model.websiteURL
                };

                int result = await _sharedJPOSService.PostRespectiveDataJPOS(JPOSAPIURLConstants.Organization, oOrgJPOS, id, clientIpAddress,JPOSAPIConstants.Organization);
                if (result > 0 && model.id <= 0)
                {
                    model.JPOS_OrgId = result.ToString();
                }
                var orgId = await AddEditOrganisation(model).ConfigureAwait(false);
                if (orgId > 0)
                {
                    var programTypes = _mapper.Map<List<org.OrganisationProgramType>>(model.OrganisationProgramTypes);
                    programTypes.ToList().ForEach(x => x.OrganisationId = orgId);
                    await _orgProgramType.AddUpdateOrganisationProgramType(programTypes);
                    return Cryptography.EncryptPlainToCipher(orgId.ToString());
                }
                return "0";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateOrganisationAdminStatus(int userId, bool IsActive)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return await sqlConnection.ExecuteAsync(SQLQueryConstants.UpdateOrganisationAdminStatusQuery, new { UserId = userId, IsActive });
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
        public async Task<int> DeleteOrganisation(int organisationId)
        {
            return await RemoveAsync(new { id = organisationId });
        }
        public async Task<OrganisationDetailWithMasterDto> GetOrganisationInfoWithProgramNAccountType(int organisationType, int organisationId, int universityId, int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetMerchantDetailInformationWithMasterQuery, new { OrganisationType = organisationType, OrganisationId = organisationId, UniversityId = universityId, ProgramId = programId }).Result)
                {
                    try
                    {
                        var orgInfo = new OrganisationDetailWithMasterDto();
                        var program = multi.Read<ProgramDto>().ToList(); // Masster programs
                        var accType = multi.Read<AccountTypeDto>().ToList();  // Master account types
                        var businessType = multi.Read<BusinessTypeDto>().ToList(); // Master business type
                        if (organisationId > 0)
                            orgInfo = multi.Read<OrganisationDetailWithMasterDto>().FirstOrDefault();
                        var orgProgram = multi.Read<OrganisationProgramDto>().ToList();
                        var orgAccType = multi.Read<AccountTypeDto>().ToList();
                        orgInfo.Program = program;
                        orgInfo.AccType = accType;
                        orgInfo.BusinessType = businessType;
                        orgInfo.OrgProgram = orgProgram;
                        orgInfo.OrgAccType = orgAccType;
                        if (orgInfo != null)
                        {
                            orgInfo.ImageFileName = orgInfo.ImagePath;
                            orgInfo.ImagePath = await _photos.GetAWSBucketFilUrl(orgInfo.ImagePath, null);
                        }
                        return orgInfo;
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
        public async Task<string> AddUpdateMerchantDetailInfo(OrganisationViewModel model, string clientIpAddress)
        {
            try
            {
                var id = model.JPOS_OrgId;
                var orgId = await AddEditOrganisation(model).ConfigureAwait(false);
                if (orgId > 0)
                {
                    if (model.id == 0 && model.organisationType == Convert.ToInt32(Constants.OrganasationType.Merchant))
                    {
                        var merchantid = "M1000-" + orgId.ToString();
                        var oMerchantJPOS = new MerchantJposDto()
                        {
                            merchantId = merchantid,
                            active = true,
                            address1 = model.addressLine1,
                            address2 = model.addressLine2,
                            city = model.city,
                            contactName = model.ContactName,
                            name = model.name,
                            country = model.country,
                            state = model.state,
                            zip = model.zip,
                            subclass = "M",
                            acquirer = (await _program.FindAsync(new { id = model.programId }).ConfigureAwait(false)).JPOS_IssuerId
                        };

                        int result = await _sharedJPOSService.PostRespectiveDataJPOS(JPOSAPIURLConstants.Merchants, oMerchantJPOS, id, clientIpAddress, JPOSAPIConstants.Merchants);
                        //if (result > 0 && model.id <= 0)
                        //{
                        //    model.JPOS_OrgId = result.ToString();
                        //}
                        var chkExist = await FindAsync(new { id = orgId });
                        chkExist.MerchantId = merchantid;
                        chkExist.JPOS_MerchantId = result.ToString();
                        await UpdateAsync(chkExist, new { id = orgId });
                    }
                    var program = _mapper.Map<List<org.OrganisationProgram>>(model.OrganisationProgram);
                    program.ToList().ForEach(x => x.organisationId = orgId);
                    await _orgProgram.AddUpdateOrganisationProgram(program);
                    await AddUpdateOrganisationAccount(model.programId, orgId, model.OrganisationAccountType);
                    await _photos.SaveUpdateImage(model.ImagePath, orgId, orgId, (int)PhotoEntityType.Organisation);
                    return Cryptography.EncryptPlainToCipher(orgId.ToString());
                }
                return "0";
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> AddUpdateOrganisationAccount(int programId, int organisationId, List<OrganisationAccTypeModel> model)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new { ProgramId = programId, OrganisationId = organisationId };
                    (await sqlConnection.QueryAsync(SQLQueryConstants.DeleteOrganisationAccountTypeQuery, obj)).ToList();
                    foreach (var item in model)
                    {
                        var cnt = (await _programAccountLinkService.GetDataAsync(new { programId, AccountTypeId = item.Id })).Count();
                        if (cnt == 0)
                        {
                            org.ProgramAccountLinking objpal = new org.ProgramAccountLinking();
                            objpal.accountTypeId = item.Id;
                            objpal.programId = programId;
                            await _programAccountLinkService.AddAsync(objpal);
                        }

                        var objinsert = new { ProgramId = programId, OrganisationId = organisationId, AccountTypeId = item.Id };
                        (await sqlConnection.QueryAsync<MerchantDto>(SQLQueryConstants.AddUpdateOrganisationAccountTypeQuery, objinsert)).ToList();
                    }
                    return 1;
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
        public async Task<List<MerchantRewardDto>> GetMerchantRewardList(int organisationId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var merchantRewardInfo = sqlConnection.QueryAsync<MerchantRewardDto>(SQLQueryConstants.GetMerchantRewardListQuery, new { MerchantId = organisationId, IsDeleted = false, OfferTypeId = OfferType.Rewards }).Result;
                    return merchantRewardInfo.ToList();
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
        public async Task<MerchantRewardDto> GetMerchantRewardInfoWithBusinessType(int organisationId, int promotionId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {

                using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetMerchantRewardInformationWithMasterQuery, new { MerchantId = organisationId, IsDeleted = false, OfferTypeId = OfferType.Rewards }).Result)
                {
                    try
                    {
                        var merchantRewardInfo = new MerchantRewardDto();
                        var businessType = multi.Read<BusinessTypeDto>().ToList();
                        var offerSubType = multi.Read<OfferSubTypeDto>().ToList();
                        if (promotionId > 0)
                            merchantRewardInfo = multi.Read<MerchantRewardDto>().Where(x => x.Id == promotionId.ToString()).FirstOrDefault();
                        merchantRewardInfo.BusinessType = businessType;
                        merchantRewardInfo.OfferSubType = offerSubType;
                        return merchantRewardInfo;
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

        public async Task<string> AddUpdateMerchantRewardInfo(MerchantRewardViewModel model)
        {
            try
            {
                var promotionId = await _promotion.AddEditPromotion(model);
                if (promotionId > 0)
                {
                    await _promotion.EditPromotionStatus(promotionId, model.isActive.Value);
                }
                return Cryptography.EncryptPlainToCipher(promotionId.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> DeleteOrganisationById(int organisationId)
        {
            return await RemoveAsync(new { Id = organisationId });
        }
        public async Task<List<MerchantTransactionDto>> GetAllMerchantsTransaction(int organisationId, DateTime? dateMonth)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new { organisationId, OrganisationType = Convert.ToInt32(OrganasationType.Merchant), DateMonth = dateMonth };
                    var organisation = (await sqlConnection.QueryAsync<MerchantTransactionDto>(SQLQueryConstants.GetAllMerchantsTransactionsQuery, obj)).ToList();
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
        public async Task<MerchantBusinessInfoDto> GetMerchantBusinessInfo(int organisationId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {

                using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetMerchantBusinessInfoQuery, new { OrganisationId = organisationId }).Result)
                {
                    try
                    {
                        var orgInfo = new MerchantBusinessInfoDto();
                        var hrsOfOperation = multi.Read<OrganisationScheduleDto>().ToList();
                        var holidayHrs = multi.Read<OrganisationScheduleDto>().ToList();
                        if (organisationId > 0)
                            orgInfo.Merchant = multi.Read<MerchantBusinessDto>().FirstOrDefault();
                        var merchantTerminal = multi.Read<MerchantTerminalDto>().ToList();
                        var mealPeriod = multi.Read<MealPeriodDto>().ToList();
                        orgInfo.HoursOfOperation = hrsOfOperation;
                        orgInfo.HolidayHours = holidayHrs;
                        orgInfo.MerchantTerminal = merchantTerminal;
                        orgInfo.MealPeriod = mealPeriod;
                        return orgInfo;
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

        public async Task<List<OrganisationDrpDto>> GetOrganisationListBasedOnUserRole(int userId, string userRole)
        {
            List<OrganisationDrpDto> orgsInfo = new List<OrganisationDrpDto>();
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    if (userRole == Roles.SuperAdmin.Trim().ToLower() || userRole == Roles.OrganizationFull.Trim().ToLower() || userRole == Roles.OrganizationReporting.Trim().ToLower())
                    {
                        //return await GetDataAsync(SQLQueryConstants.GetOrganisationListByTypeQuery, new { IsActive = true, IsDeleted = false, OrganisationType = OrganasationType.University, SearchName = "%" + "" + "%", RoleName = userRole, UserId = userId });
                        orgsInfo = (sqlConnection.QueryAsync<OrganisationDrpDto>(SQLQueryConstants.GetOrganisationListByTypeQuery, new { IsActive = true, IsDeleted = false, OrganisationType = OrganasationType.University, SearchName = "%" + "" + "%", RoleName = userRole, UserId = userId }).Result).ToList();
                    }
                    else if (userRole == Roles.ProgramFull.Trim().ToLower() || userRole == Roles.ProgramReporting.Trim().ToLower())
                    {
                        orgsInfo = (sqlConnection.QueryAsync<OrganisationDrpDto>(SQLQueryConstants.GetOrgnisationListBasedOnUserRolePrgram, new { IsActive = true, IsDeleted = false, OrganisationType = OrganasationType.University, UserId = userId }).Result).ToList();
                    }
                    else
                    {
                        // Merchant full OR merchant reporting.
                        orgsInfo = (sqlConnection.QueryAsync<OrganisationDrpDto>(SQLQueryConstants.GetOrganisationListBasedOnUserRoleMerchant, new { IsActive = true, IsDeleted = false, OrganisationType = OrganasationType.University, UserId = userId }).Result).ToList();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return orgsInfo;
        }
        public async Task<List<WeekDayDto>> GetWeekDaysList()
        {
            List<WeekDayDto> result = new List<WeekDayDto>();
            foreach (var item in Enum.GetNames(typeof(DayOfWeek)))
            {
                result.Add(new WeekDayDto() { DayName = item });
            }
            return result;
        }
        public async Task<List<DwellTimeDto>> GetDwellTimeList()
        {
            List<DwellTimeDto> result = new List<DwellTimeDto>();
            DwellTimeDto dt = new DwellTimeDto();
            dt.Id = 0;
            dt.Time = "Select";
            dt = new DwellTimeDto();
            dt.Id = 5;
            dt.Time = "5 minutes";
            result.Add(dt);
            dt = new DwellTimeDto();
            dt.Id = 10;
            dt.Time = "10 minutes";
            result.Add(dt);
            dt = new DwellTimeDto();
            dt.Id = 15;
            dt.Time = "15 minutes";
            result.Add(dt);
            dt = new DwellTimeDto();
            dt.Id = 20;
            dt.Time = "20 minutes";
            result.Add(dt);
            dt = new DwellTimeDto();
            dt.Id = 25;
            dt.Time = "25 minutes";
            result.Add(dt);
            dt = new DwellTimeDto();
            dt.Id = 30;
            dt.Time = "30 minutes";
            result.Add(dt);
            return result;
        }
        public async Task<List<TerminalTypeDto>> GetTerminalTypeList()
        {
            List<TerminalTypeDto> result = new List<TerminalTypeDto>();
            TerminalTypeDto dt = new TerminalTypeDto();
            dt.Id = 0;
            dt.TerminalType = "Select";
            dt = new TerminalTypeDto();
            dt.Id = 1;
            dt.TerminalType = "Foundry Poynt Smart Terminal";
            result.Add(dt);
            dt = new TerminalTypeDto();
            dt.Id = 2;
            dt.TerminalType = "Foundry Poynt 5";
            result.Add(dt);
            dt = new TerminalTypeDto();
            dt.Id = 3;
            dt.TerminalType = "Vivonet POS";
            result.Add(dt);
            dt = new TerminalTypeDto();
            dt.Id = 4;
            dt.TerminalType = "Appetize POS";
            result.Add(dt);
            dt = new TerminalTypeDto();
            dt.Id = 5;
            dt.TerminalType = "Bypass mobile";
            result.Add(dt);
            return result;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will get closing status of the Merchant.
        /// </summary>
        /// <param name="organisationId"></param>
        /// <param name="orgschedule"></param>
        /// <returns></returns>
        private string getClosingStatus(int organisationId, string programTimeZone, bool isOrganisationClosed)
        {
            string strstatus = "Closed Now";
            if (isOrganisationClosed) { strstatus = "Closed Now"; }
            else
            {
                using (var sqlConnection = _databaseConnectionFactory.CreateConnection())
                {
                    try
                    {
                        OrganisationScheduleAndHolidayDto ogranisationScheduleHoliday = new OrganisationScheduleAndHolidayDto();
                        using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetOrganisationScheduleByIdSP, new { Id = organisationId, IsActive = true, IsDeleted = false }, commandType: CommandType.StoredProcedure).Result)
                        {
                            try
                            {
                                ogranisationScheduleHoliday.OrganisationSchedule = new List<OrganisationScheduleDto>();
                                ogranisationScheduleHoliday.HolidaySchedule = new HolidayScheduleDto();
                                ogranisationScheduleHoliday.OrganisationSchedule = multi.Read<OrganisationScheduleDto>().ToList();
                                var holidaySchedule = multi.Read<HolidayScheduleDto>().ToList();
                                if (holidaySchedule.Count > 0)
                                {
                                    if (holidaySchedule.Any(x => x.IsForHolidayNameToShow == true))
                                    { ogranisationScheduleHoliday.HolidaySchedule = holidaySchedule.Where(x => x.IsForHolidayNameToShow == true).FirstOrDefault(); }
                                    else { ogranisationScheduleHoliday.HolidaySchedule = holidaySchedule.FirstOrDefault(); }

                                }
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
                        if (ogranisationScheduleHoliday.OrganisationSchedule.Count > 0)
                        {
                            var dtUtc = DateTime.UtcNow;
                            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(programTimeZone);
                            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(dtUtc, cstZone);
                            var currentday = cstTime.DayOfWeek.ToString();
                            var orgopentime = ogranisationScheduleHoliday.OrganisationSchedule.Where(x => x.WorkingDay.ToLower(CultureInfo.InvariantCulture).Trim() == currentday.ToLower(CultureInfo.InvariantCulture).Trim()).Select(x => x.OpenTime).FirstOrDefault().ToString();
                            var orgclosedtime = ogranisationScheduleHoliday.OrganisationSchedule.Where(x => x.WorkingDay.ToLower(CultureInfo.InvariantCulture).Trim() == currentday.ToLower(CultureInfo.InvariantCulture).Trim()).Select(x => x.ClosedTime).FirstOrDefault().ToString();
                            var orgopendate = Convert.ToDateTime(cstTime.Date.ToShortDateString() + " " + orgopentime);
                            var orgclosedate = Convert.ToDateTime(cstTime.Date.ToShortDateString() + " " + orgclosedtime);
                            TimeSpan duration = orgclosedate.Subtract(cstTime);
                            if (cstTime >= orgopendate && cstTime <= orgclosedate && duration.Hours >= 1)
                            {
                                strstatus = "Open";
                            }
                            else if (cstTime >= orgopendate && cstTime <= orgclosedate && duration.Hours == 0 && duration.Minutes > 1)
                            {
                                strstatus = "Closing Soon";
                            }
                        }
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
            return strstatus;
        }

        /// <summary>
        /// This method will calculate the distance of the restourent from the current location based on lattitude and longitude.
        /// </summary>
        /// <param name="latlon1"></param>
        /// <param name="latlon2"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        private double getdistance(string latlon1, string latlon2, char unit)
        {
            try
            {
                if (!string.IsNullOrEmpty(latlon1) && !string.IsNullOrEmpty(latlon2))
                {
                    var lat1 = Convert.ToDouble(latlon1.Split(',')[0].Trim());
                    var lon1 = Convert.ToDouble(latlon1.Split(',')[1].Trim());
                    var lat2 = Convert.ToDouble(latlon2.Split(',')[0].Trim());
                    var lon2 = Convert.ToDouble(latlon2.Split(',')[1].Trim());
                    if ((lat1 == lat2) && (lon1 == lon2))
                    {
                        return 0;
                    }
                    else
                    {
                        double theta = lon1 - lon2;
                        double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                        dist = Math.Acos(dist);
                        dist = rad2deg(dist);
                        dist = dist * 60 * 1.1515;
                        if (unit == 'K')
                        {
                            dist = Math.Round(dist * 1.609344, 1);
                        }
                        else if (unit == 'N')
                        {
                            dist = Math.Round(dist * 0.8684, 1);
                        }
                        return (dist);
                    }
                }
                return -1;
            }
            catch (Exception)
            {
                throw;
            }

        }

        //private string GetOfferValue(Offer o, int OfferBannerId)
        //{
        //    string offerValue = string.Empty;
        //    switch (OfferBannerId)
        //    {
        //        case 1:
        //            offerValue = o.DiscountInPercentage.ToString();
        //            break;
        //        case 2:
        //            offerValue = o.OfferDayName.ToString();
        //            break;
        //        case 3:
        //            offerValue = o.FreeQuantity.ToString();
        //            break;
        //        default:
        //            offerValue = string.Empty;
        //            break;

        //    }

        //    return offerValue;
        //}

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
            try
            {
                return (deg * Math.PI / 180.0);
            }
            catch (Exception)
            {
                throw;
            }

        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            try
            {
                return (rad / Math.PI * 180.0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int GetOrgMaxCapacity(int dwellTime, int maxSeats)
        {
            float NumMealPassesInSegment = 100;
            float PercentMealPasses = 90;
            var totalTransaction = (NumMealPassesInSegment * 100) / PercentMealPasses;
            if (maxSeats > 0)
                return Convert.ToInt32((totalTransaction / maxSeats) * 100);
            else
                return 0;
        }

        #endregion
    }
}
