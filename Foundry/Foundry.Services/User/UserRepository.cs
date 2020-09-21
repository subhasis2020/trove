using AutoMapper;
using Dapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;

namespace Foundry.Services
{
    public class UserRepository : FoundryRepositoryBase<User>, IUserRepository
    {
        /*Creating database connection*/
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IPhotos _photos;
        private readonly IConfiguration _configuration;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IAdminProgramAccessService _adminProgramTypeService;
        private readonly IMerchantAdminService _merchantAdminService;
        private readonly IProgramAdminService _programAdminService;
        private readonly IUserPlanService _userPlanService;
        private readonly ISharedJPOSService _sharedJPOSService;
        private readonly IMapper _mapper;
        /// <summary>
        /// Constructor for assigning value to database connection.
        /// </summary>
        /// <param name="databaseConnectionFactory"></param>
        public UserRepository(IDatabaseConnectionFactory databaseConnectionFactory, IPhotos photos, IConfiguration configuration, IUserRoleRepository userRoleRepository,
            IMapper mapper, IAdminProgramAccessService adminProgramTypeService, IUserPlanService userPlanService, IMerchantAdminService merchantAdminService,
            IProgramAdminService programAdminService, ISharedJPOSService sharedJPOSService)
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _photos = photos;
            _configuration = configuration;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _adminProgramTypeService = adminProgramTypeService;
            _userPlanService = userPlanService;
            _merchantAdminService = merchantAdminService;
            _programAdminService = programAdminService;
            _sharedJPOSService = sharedJPOSService;
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This method is returning all the users for the foundry.
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUsers()
        {
            var users = await AllAsync();
            return users.OrderBy(u => u.Email).ToList();
        }

        public async Task<UserDto> GetUserById(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<UserDto>(SQLQueryConstants.GetUserDetailByIdQuery, new { UserId = userId, DefaultProgramPhotoPath = string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.ProgramDefaultImage) });
                    if (result != null)
                    { result.ImagePath = await _photos.GetAWSBucketFilUrl(result.ImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage)); }
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
        public async Task<UserDto> GetUserInfoById(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<UserDto>(SQLQueryConstants.GetUserDetailById, new { UserId = userId});
                   
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
        public async Task<List<UserDto>> GetUserByIdWithProgramDetail(List<int> userIds)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<UserDto>(SQLQueryConstants.GetUsersDetailByIdsWithProgramQuery, new { UserIds = userIds, DefaultProgramPhotoPath = string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.ProgramDefaultImage) })).ToList();
                    if (result.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            result[i].ImagePath = await _photos.GetAWSBucketFilUrl(result[i].ImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
                        }

                    }
                    return result.ToList();
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
        public async Task<UserDto> GetUserBenefactorById(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<UserDto>(SQLQueryConstants.GetUserBenefactorDetailByIdQuery, new { UserId = userId, DefaultProgramPhotoPath = string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.ProgramDefaultImage) });
                    if (result != null)
                    { result.ImagePath = await _photos.GetAWSBucketFilUrl(result.ImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage)); }
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

        public async Task<UserDto> RegisterUserWithProgram(User model, int programCodeId, string photoPath = null)
        {
            try
            {
                var userInfo = await FindAsync(new { Email = model.Email, IsActive = true });
                if (userInfo == null)
                {
                    userInfo = new User();
                }
                /* Saving the data for the user*/
                userInfo.FirstName = model.FirstName;
                userInfo.LastName = model.LastName ?? null;
                userInfo.PhoneNumber = model.PhoneNumber;
                userInfo.Email = model.Email;
                userInfo.UserDeviceId = model.UserDeviceId ?? null;
                userInfo.UserDeviceType = model.UserDeviceType ?? null;
                userInfo.Location = model.Location ?? null;
                userInfo.SessionId = Guid.NewGuid().ToString();
                userInfo.InvitationStatus = 3;
                userInfo.EmailConfirmed = true;
                userInfo.IsMobileRegistered = true;
                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    PasswordHasher<User> _usr = new PasswordHasher<User>();
                    userInfo.PasswordHash = _usr.HashPassword(userInfo, model.PasswordHash);
                }


                var userId = 0;
                if (userInfo.Id > 0) { await UpdateAsync(userInfo, new { id = userInfo.Id }); userId = userInfo.Id; } else { userId = await AddAsync(userInfo); }

                /* Saving the data for the user image if the profile path is not null */
                // if (!string.IsNullOrEmpty(photoPath))
                //  {
                await _photos.SaveUpdateImage(photoPath, userInfo?.Id > 0 ? userInfo.Id : userId, userInfo?.Id > 0 ? userInfo.Id : userId, (int)PhotoEntityType.UserProfile);
                //  }

                return await GetUserWithProgramCode(userId, programCodeId).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserDto> EditUserProfile(User model, int programCodeId, string photoPath = null)
        {
            try
            {
                var userInfo = await FindAsync(new { Email = model.Email, IsActive = true, Id = model.Id });
                if (userInfo == null)
                {
                    return null;
                }
                /* Saving the data for the user*/
                userInfo.FirstName = model.FirstName;
                userInfo.LastName = model.LastName ?? null;
                userInfo.PhoneNumber = model.PhoneNumber;
                var userId = 0;
                if (userInfo.Id > 0) { await UpdateAsync(userInfo, new { id = userInfo.Id }); userId = userInfo.Id; }
                await _photos.SaveUpdateImage(photoPath, userInfo?.Id > 0 ? userInfo.Id : userId, userInfo?.Id > 0 ? userInfo.Id : userId, (int)PhotoEntityType.UserProfile);
                return await GetUserWithProgramCode(userId, programCodeId).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserDto> GetUserWithProgramCode(int userId, int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<UserDto>(SQLQueryConstants.GetUserWithProgramCodeQuery, new { UserId = userId, DefaultProgramPhotoPath = string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.ProgramDefaultImage), ProgramId = programId });
                    if (result != null)
                    {
                        result.ImageFileName = result.ImagePath;
                        result.ImagePath = await _photos.GetAWSBucketFilUrl(result.ImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
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

        public async Task<UserDto> GetUserWithProgramCodeBeforeRegister(int userId, int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<UserDto>(SQLQueryConstants.GetUserWithProgramCodeBeforeRegisterQuery, new { UserId = userId, DefaultProgramPhotoPath = string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.ProgramDefaultImage), ProgramId = programId });
                    if (result != null)
                    {
                        result.ImageFileName = result.ImagePath;
                        result.ImagePath = await _photos.GetAWSBucketFilUrl(result.ImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
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
        /// This method is checking the session of the user based on their device usage.
        /// </summary>
        /// <param name="model">UserId,SessionId</param>
        /// <returns></returns>
        public async Task<string> CheckSessionId(int userId, string sessionId)
        {
            var sessionCheck = await FindAsync(new { Id = userId, SessionId = sessionId.Trim() });
            return sessionCheck?.SessionId;
        }

        public async Task<IEnumerable<User>> GetUsersDetailByIds(List<int> UserIds)
        {
            try
            {
                var users = await GetDataAsync(SQLQueryConstants.GetUsersDetailByIdsQuery, new { UserIds });
                if (users != null)
                {
                    return users.Select(x => new User { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, Email = x.Email, PhoneNumber = x.PhoneNumber, PartnerUserId =x.PartnerUserId });
                }
                else { return null; }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetUsersDetailByUserCode(List<string> UserIds)
        {
            try
            {
                var users = await GetDataAsync(SQLQueryConstants.GetUserByUserCodeQuery, new { IsActive = true, IsDeleted = false, UserIds });
                if (users != null)
                {
                    return users.Select(x => new User { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, Email = x.Email, PhoneNumber = x.PhoneNumber, PartnerUserId= x.PartnerUserId });
                }
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetUsersDetailByEmailIds(List<string> userEmailIdsList)
        {
            try
            {
                var users = await GetDataAsync(SQLQueryConstants.GetUsersDetailByEmailIdsQuery, new { UserEmailIds = userEmailIdsList });
                return users.Select(x => new User { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, Email = x.Email, PhoneNumber = x.PhoneNumber, IsActive = x.IsActive, IsDeleted = x.IsDeleted });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method is used to check the existence of the user based on the given email.
        /// </summary>
        /// <param name="userEmail">email</param>
        /// <returns></returns>
        public async Task<bool?> CheckUserInactivity(string userEmail)
        {
            var userActiveCheck = await FindAsync(new { Email = userEmail });
            return userActiveCheck != null ? true : false;
        }

        public async Task<User> CheckUserExistence(string userEmail)
        {
            return await FindAsync(new { Email = userEmail, IsActive = true, IsDeleted = false });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task<User> CheckUserByEmail(string userEmail)
        {
            var userActiveCheck = await FindAsync(new { Email = userEmail, IsActive = true, IsDeleted = false });
            return userActiveCheck;
        }


      
        public async Task<User> CheckUserByPartnerId(string partnerUserId , int partnerId)
        {
            var userActiveCheck = await FindAsync(new { PartnerUserId= partnerUserId ,  PartnerId = partnerId, IsActive = true, IsDeleted = false });
            return userActiveCheck;
        }




        public async Task<int> AddUpdateUser(User model)
        {
            try
            {
                var userInfo = await FindAsync(new { Id = model.Id });
                if (userInfo == null)
                {
                    userInfo = new User();
                    userInfo.CreatedDate = model.CreatedDate;
                    if (model.Email != null)
                    {
                        userInfo.Email = model.Email;
                        userInfo.NormalizedEmail = model.Email;
                        userInfo.NormalizedUserName = model.Email.ToUpper();
                        userInfo.UserName = model.Email;

                    }
                    if (model.InvitationStatus != null)
                    {
                        userInfo.InvitationStatus = model.InvitationStatus;
                    }
                    if (model.IsMobileRegistered != null)
                    {
                        userInfo.IsMobileRegistered = model.IsMobileRegistered;
                    }
                    userInfo.EmailConfirmed = model.EmailConfirmed;
                    userInfo.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
                }
                if (model.FirstName != null)
                {
                    userInfo.FirstName = model.FirstName;
                }
                if (model.LastName != null)
                {
                    userInfo.LastName = model.LastName;
                }
                userInfo.AccessFailedCount = model.AccessFailedCount > 0 ? model.AccessFailedCount : 0;
                userInfo.PhoneNumber = model.PhoneNumber;
                userInfo.Address = model.Address;

                userInfo.UserDeviceId = model.UserDeviceId;
                userInfo.UserDeviceType = model.UserDeviceType;
                userInfo.Location = model.Location;
                if (model.Id <= 0)
                    userInfo.SessionId = Guid.NewGuid().ToString();
                userInfo.IsActive = true;
                userInfo.IsDeleted = false;
                if (model.PasswordHash != null)
                {
                    PasswordHasher<User> _usr = new PasswordHasher<User>();
                    userInfo.PasswordHash = _usr.HashPassword(userInfo, model.PasswordHash);
                }

                userInfo.TwoFactorEnabled = model.TwoFactorEnabled;
                userInfo.LockoutEnabled = model.LockoutEnabled;
                userInfo.OrganisationId = model.OrganisationId;
                userInfo.UserCode = model.UserCode;
                if (model.ModifiedDate != null)
                    userInfo.ModifiedDate = model.ModifiedDate;
                if (model.IsAdmin != null)
                    userInfo.IsAdmin = model.IsAdmin;
                userInfo.ProgramId = model.ProgramId;
                userInfo.secondaryEmail = model.secondaryEmail;
                userInfo.genderId = model.genderId;
                userInfo.dateOfBirth = model.dateOfBirth;
                userInfo.customInfo = model.customInfo;


                return await InsertOrUpdateAsync(userInfo, new { Id = model.Id });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> AddUpdateUserInvitationStatus(User model)
        {
            try
            {
                var userInfo = await FindAsync(new { Id = model.Id });
                if (userInfo != null)
                {
                    if (model.InvitationStatus != null)
                    {
                        userInfo.InvitationStatus = model.InvitationStatus;
                    }
                    if (model.IsMobileRegistered != null)
                    {
                        userInfo.IsMobileRegistered = model.IsMobileRegistered;
                    }
                }
                return await InsertOrUpdateAsync(userInfo, new { Id = model.Id });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> CheckPhoneNumberExistence(string phoneNumber)
        {
            return await FindAsync(new { PhoneNumber = phoneNumber, IsActive = true, IsDeleted = false });
        }

        public async Task<int> UpdateUserDeviceAndLocationInfo(User model)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {

                    var userInfo = await FindAsync(new { Email = model.Email });
                    if (userInfo != null)
                    {
                        userInfo.UserDeviceId = model.UserDeviceId;
                        userInfo.UserDeviceType = model.UserDeviceType;
                        userInfo.Location = model.Location;
                        userInfo.SessionId = Guid.NewGuid().ToString();
                        return await sqlConnection.ExecuteAsync(SQLQueryConstants.UpdateUserDeviceNLocationQuery, new { UserDeviceId = userInfo.UserDeviceId, UserDeviceType = userInfo.UserDeviceType, Location = userInfo.Location, SessionId = userInfo.SessionId, Email = model.Email });
                    }
                    return 0;
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


        public async Task<int> UpdateUserCardHolderAgreementReadDetail(User model)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var userInfo = await FindAsync(new { id = model.Id });
                    if (userInfo != null)
                    {
                        userInfo.IsAgreementRead = model.IsAgreementRead;
                        userInfo.AgreementVersionNo = model.AgreementVersionNo;
                        userInfo.AgreementReadDateTime = DateTime.UtcNow;
                        return await InsertOrUpdateAsync(userInfo, new { id = model.Id > 0 ? model.Id : 0 });
                    }
                    return 0;
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

        public async Task<UserWithProgramTypeDto> GetUserAdminInfoWithProgramTypes(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetUserAdminDetailByIdQuery, new { UserId = userId }).Result)
                    {
                        try
                        {
                            var orgInfo = multi.Read<UserWithProgramTypeDto>().FirstOrDefault();
                            if (orgInfo != null)
                            {
                                orgInfo.ImageFileName = orgInfo.UserImagePath;
                                orgInfo.UserImagePath = await _photos.GetAWSBucketFilUrl(orgInfo.UserImagePath, null);
                            }
                            var usrProgranType = multi.Read<ProgramTypeIdDto>().ToList();
                            var merchantAdmin = multi.Read<MercahntIdDto>().ToList();
                            orgInfo.MerchantIds = merchantAdmin.Count > 0 ? merchantAdmin : null;
                            orgInfo.UserProgramType = usrProgranType.Count > 0 ? usrProgranType : null;
                            var programAdmin = multi.Read<AdminProgramIdDto>().ToList();
                            orgInfo.AdminProgramIds = programAdmin.Count > 0 ? programAdmin : null;
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
                catch (Exception ex)
                {
                    throw;
                }

            }
        }

        public async Task<int> AddUpdateAdminUser(OrganisationAdminViewDetail model)
        {
            try
            {
                var userInfo = await FindAsync(new { id = model.UserId });
                if (userInfo == null)
                {
                    userInfo = new User();
                    userInfo.IsActive = true;
                }
                userInfo.FirstName = model.Name;
                userInfo.LastName = model.LastName ?? null;
                userInfo.PhoneNumber = model.PhoneNumber;
                userInfo.Email = model.EmailAddress;
                userInfo.NormalizedEmail = model.EmailAddress;
                userInfo.NormalizedUserName = model.EmailAddress.ToUpper();
                userInfo.UserName = model.EmailAddress;
                userInfo.IsDeleted = false;
                userInfo.IsAdmin = true;
                userInfo.OrganisationId = model.OrganisationId;
                userInfo.Custom1 = model.Custom1;

                var userId = await InsertOrUpdateAsync(userInfo, new { id = model.UserId > 0 ? model.UserId : 0 });
                if (userId > 0)
                {
                    await _userRoleRepository.AddUserRole(new UserRole { UserId = userId, RoleId = model.RoleId });
                    await _photos.SaveUpdateImage(model.UserImagePath, userId, userId, (int)PhotoEntityType.UserProfile);

                    if (model.ProgramsAccessibility.Count > 0)
                    {
                        var programTypes = _mapper.Map<List<AdminProgramAccess>>(model.ProgramsAccessibility);
                        programTypes.ToList().ForEach(x => x.UserId = userId);
                        await _adminProgramTypeService.AddUpdateAdminProgramType(programTypes);
                    }
                    if (model.MerchantAccessibility.Count > 0)
                    {
                        List<MerchantAdmins> oMerchanrAdmin = new List<MerchantAdmins>();
                        foreach (var item in model.MerchantAccessibility)
                        {
                            var oAdmin = new MerchantAdmins()
                            {
                                adminUserId = userId,
                                merchantId = item.merchantId
                            };
                            oMerchanrAdmin.Add(oAdmin);
                        }
                        await _merchantAdminService.AddUpdateMerchantFromAdmins(oMerchanrAdmin);
                    }
                    return userId;
                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> AddUpdateProgramLevelAdminUser(ProgramLevelAdminViewDetail model)
        {
            try
            {
                var userInfo = await FindAsync(new { id = model.UserId });
                if (userInfo == null)
                {
                    userInfo = new User();
                    userInfo.IsActive = true;
                }
                userInfo.FirstName = model.Name;
                userInfo.LastName = model.LastName ?? null;
                userInfo.PhoneNumber = model.PhoneNumber;
                userInfo.Email = model.EmailAddress;
                userInfo.NormalizedEmail = model.EmailAddress;
                userInfo.NormalizedUserName = model.EmailAddress.ToUpper();
                userInfo.UserName = model.EmailAddress;
                userInfo.IsDeleted = false;
                userInfo.IsAdmin = true;
                userInfo.ProgramId = model.ProgramId;
                userInfo.Custom1 = model.Custom1;
                var userId = await InsertOrUpdateAsync(userInfo, new { id = model.UserId > 0 ? model.UserId : 0 });
                if (userId > 0)
                {
                    await _userRoleRepository.AddUserRole(new UserRole { UserId = userId, RoleId = model.RoleId });
                    await _photos.SaveUpdateImage(model.UserImagePath, userId, userId, (int)PhotoEntityType.UserProfile);
                    if (model.ProgramsAccessibility.Count > 0)
                    {
                        var programTypes = _mapper.Map<List<AdminProgramAccess>>(model.ProgramsAccessibility);
                        programTypes.ToList().ForEach(x => x.UserId = userId);
                        await _adminProgramTypeService.AddUpdateAdminProgramType(programTypes);
                    }
                    if (model.ProgramAdminAccessibility.Count > 0)
                    {
                        List<ProgramAdmins> oPrgAdmin = new List<ProgramAdmins>();
                        foreach (var item in model.ProgramAdminAccessibility)
                        {
                            var oAdmin = new ProgramAdmins()
                            {
                                adminUserId = userId,
                                programId = item.programId
                            };
                            oPrgAdmin.Add(oAdmin);
                        }
                        await _programAdminService.AddUpdateProgramFromPrgAdmins(oPrgAdmin);
                    }
                    return userId;
                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> AddUserFromExcel(User model)
        {
            try
            {
                var userInfo = new User();
                userInfo.FirstName = model.FirstName;
                userInfo.LastName = model.LastName ?? null;
                userInfo.PhoneNumber = model.PhoneNumber;
                userInfo.Email = model.Email;
                userInfo.NormalizedEmail = model.Email;
                userInfo.NormalizedUserName = model.Email.ToUpper();
                userInfo.UserName = model.Email;
                userInfo.UserDeviceId = model.UserDeviceId ?? null;
                userInfo.UserDeviceType = model.UserDeviceType ?? null;
                userInfo.Location = model.Location ?? null;
                userInfo.SessionId = Guid.NewGuid().ToString();
                userInfo.IsActive = true;
                userInfo.IsDeleted = false;
                userInfo.UserCode = model.UserCode;
                userInfo.dateOfBirth = model.dateOfBirth;
                userInfo.secondaryEmail = model.secondaryEmail;
                userInfo.UserCode = model.UserCode;
                userInfo.InvitationStatus = 1;
                userInfo.IsMobileRegistered = false;
                userInfo.EmailConfirmed = false;
                userInfo.PhoneNumberConfirmed = false;
                userInfo.IsAdmin = false;
                userInfo.OrganisationId = model.OrganisationId;

                return await AddAsync(userInfo);

            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<int> DeleteAdminUser(int userId)
        {
            try
            {
                await _adminProgramTypeService.DeleteAdminProgramType(userId);
                await _merchantAdminService.DeleteAdminMerchant(userId);
                return await RemoveAsync(new { Id = userId });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AccountHolderDto>> GetAccountHoldersList(int organisationId, int programId, string searchValue, int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection, int? planId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var users = (await sqlConnection.QueryAsync<AccountHolderDto>(SQLQueryConstants.GetAccountHoldersBySP, new { OrganisationId = organisationId, ProgramId = programId, SearchValue = searchValue, PageNumber = pageNumber, PageSize = pageSize, SortColumnName = sortColumnName, SortOrderDirection = sortOrderDirection, PhotoTypeDetail = (int)PhotoEntityType.UserProfile, DefaultUserPhotoPath = "", PlanId = planId }, commandType: CommandType.StoredProcedure)).ToList();
                    if (users.Count > 0)
                    {
                        for (int i = 0; i < users.Count; i++)
                        {
                            users[i].UserImagePath = await _photos.GetAWSBucketFilUrl(users[i].UserImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
                        }
                    }
                    return users;
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

        public async Task<IEnumerable<AccountHolderDto>> GetAccountHoldersListByOrganization(int organisationId, int programId, string searchValue, int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection, int? planId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var users = (await sqlConnection.QueryAsync<AccountHolderDto>(SQLQueryConstants.GetAccountHoldersByOrganization, new { OrganisationId = organisationId, ProgramId = programId, SearchValue = searchValue, PageNumber = pageNumber, PageSize = pageSize, SortColumnName = sortColumnName, SortOrderDirection = sortOrderDirection, PhotoTypeDetail = (int)PhotoEntityType.UserProfile, DefaultUserPhotoPath = "", PlanId = planId }, commandType: CommandType.StoredProcedure)).ToList();
                    if (users.Count > 0)
                    {
                        for (int i = 0; i < users.Count; i++)
                        {
                            users[i].UserImagePath = await _photos.GetAWSBucketFilUrl(users[i].UserImagePath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
                        }
                    }
                    return users;
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

        public async Task<int> AddUpdateAccountHolderDetail(User model, List<UserPlans> programPackageIds, string userImagePath, string clientIpAddress, string issuerId)
        {
            try
            {
                var id = model.Jpos_AccountHolderId;
                var oACHJPOS = new AccountHolderJposDto()
                {
                    active = true,
                    birthDate = model.dateOfBirth.HasValue ? model.dateOfBirth : null,
                    email = model.Email,
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    address1 = model.Address,
                    phone = model.PhoneNumber,
                    realId = model.UserCode,
                    gender = model.genderId == 1 ? "M" : "F",
                    issuer = issuerId
                };
                int result = await _sharedJPOSService.PostRespectiveDataJPOS(JPOSAPIURLConstants.AccountHolder, oACHJPOS, id, clientIpAddress, JPOSAPIConstants.AccountHolder);
                if (result > 0 && model.Id <= 0)
                {
                    model.Jpos_AccountHolderId = result.ToString();
                }
                var userId = await AddUpdateUser(model);
                if (userId > 0)
                {
                    await _photos.SaveUpdateImage(userImagePath, userId, userId, (int)PhotoEntityType.UserProfile);
                    if (programPackageIds.Count > 0)
                    {
                        programPackageIds.ToList().ForEach(x => x.userId = userId);
                        await _userPlanService.AddUpdateUserPlans(programPackageIds);
                    }
                    return userId;
                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AccountHolderDto> GetUserInfoWithUserPlans(int userId, int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                var userInfo = new AccountHolderDto();
                using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetMultipleQueryUserDetailNUserPlansQuery, new { UserId = userId, ProgramId = programId, PhotoTypeDetail = (int)PhotoEntityType.UserProfile }).Result)
                {
                    try
                    {
                        userInfo = multi.Read<AccountHolderDto>().FirstOrDefault();
                        if (userInfo != null)
                        {
                            userInfo.ImageFileName = userInfo.UserImagePath;
                            userInfo.UserImagePath = await _photos.GetAWSBucketFilUrl(userInfo.UserImagePath, null);
                        }
                        var userPlans = multi.Read<int>().ToList();
                        if (userInfo != null)
                        {
                            userInfo.planIds = new List<PlanIdDto>();
                            foreach (var item in userPlans)
                            {
                                userInfo.planIds.Add(new PlanIdDto() { PlanId = item });
                            }
                        }
                        return userInfo;
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

        public async Task<List<UserDeviceDto>> GetUserDeviceTokenBasedOnProgram(int programId)
        {

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return (await sqlConnection.QueryAsync<UserDeviceDto>(SQLQueryConstants.GetUserDeviceTokenByProgram, new { ProgramId = programId })).ToList();

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

        public async Task<List<UserDeviceDto>> GetUserDeviceTokenBasedOnProgramLst(List<int> programIds)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return (await sqlConnection.QueryAsync<UserDeviceDto>(SQLQueryConstants.GetUserDeviceTokenByProgramLst, new { ProgramId = programIds })).ToList();
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

        public async Task<List<UserDeviceDto>> GetUserDeviceTokenByUserIdNdProgram(int programId, int userId)
        {

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return (await sqlConnection.QueryAsync<UserDeviceDto>(SQLQueryConstants.GetUserDeviceTokenByUserId, new { ProgramId = programId, UserId = userId })).ToList();
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

        #region Sodexo 
        public async Task<int> RegisterPartnerUser(User model)
        {
            try
            {
                var userInfo = await FindAsync(new { Email = model.Email, IsActive = true });
                if (userInfo == null)
                {
                    userInfo = new User();
                }
                /* Saving the data for the user*/


                userInfo.Email = model.Email;
                userInfo.NormalizedEmail = model.Email.ToUpper();
                userInfo.NormalizedUserName = model.Email.ToUpper();
                userInfo.UserName = model.Email;
                userInfo.PartnerUserId = model.PartnerUserId;
                userInfo.PartnerId = model.PartnerId;
                userInfo.genderId = model.genderId;
                userInfo.FirstName = model.FirstName;
                userInfo.LastName = model.LastName ?? null;
                userInfo.PhoneNumber = model.PhoneNumber;
                userInfo.UserDeviceId = model.UserDeviceId ?? null;
                userInfo.UserDeviceType = model.UserDeviceType ?? null;
                userInfo.Location = model.Location ?? null;
                userInfo.SessionId = Guid.NewGuid().ToString();
                userInfo.InvitationStatus = 3;
                userInfo.EmailConfirmed = true;
                userInfo.IsMobileRegistered = true;
                userInfo.OrganisationId = model.OrganisationId;
                userInfo.ProgramId = model.ProgramId;
                userInfo.dateOfBirth = model.dateOfBirth;
                userInfo.UserCode = model.UserCode;

                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    PasswordHasher<User> _usr = new PasswordHasher<User>();
                    userInfo.PasswordHash = _usr.HashPassword(userInfo, model.PasswordHash);
                }
                if (userInfo.Id > 0)
                { return await UpdateAsync(userInfo, new { id = userInfo.Id });

                }
                else
                { int userid = await AddAsync(userInfo);

                    var userInfo1 = await FindAsync(new { id = userid });

                    userInfo1.UserCode = string.Concat("AHD1000-", Convert.ToString(userid));
                   await UpdateAsync(userInfo1, new { id = userid });
                    return userid;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<UserDto> GetUserbyBiteUserId(string partnerUserId,int partnerId)
        {
                using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
                {
                    try
                    {
                        var result = await sqlConnection.QuerySingleOrDefaultAsync<UserDto>(SQLQueryConstants.GetUserDetailByBiteUserIdQuery, new { PartnerUserId = partnerUserId  , PartnerId = partnerId });
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

        public async Task<string> GetUserType(int Id)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var usertype = (await sqlConnection.QueryAsync(SQLQueryConstants.GetUserTypeBySP, new { Id = Id }, commandType: CommandType.StoredProcedure));
                    var firstRow = usertype.FirstOrDefault();
                    var Heading = ((IDictionary<string, object>)firstRow).Keys.ToArray();
                    var details = ((IDictionary<string, object>)firstRow);
                    var values = details[Heading[0]];
                    return values.ToString();
                }

                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        public async Task<List<I2CCardBankAccountModel>> GetUserCardwithBankAccount(int byUserId,int toUserId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    I2CCardBankAccountModel obj = new I2CCardBankAccountModel();
                    var details1 = (await sqlConnection.QueryAsync<I2CCardBankAccountModel>(SQLQueryConstants.GetUserCardwithBankAccountBySP, new { byUserId = byUserId, toUserId = toUserId }, commandType: CommandType.StoredProcedure));               
                  
                    return details1.ToList();
                }

                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        public async Task<string> GetBiteUserLoyaltyTrackingBalance(int Id)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var userbal = (await sqlConnection.QueryAsync(SQLQueryConstants.GetBiteUserLoyaltyTrackingBalanceQuery, new { Id = Id })).FirstOrDefault();
                    if(userbal==null)
                    {
                        userbal = 0;
                    }
                    var value = ((IDictionary<string, object>)userbal).Values.ToArray();
                    return value[0].ToString();
                }

                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            }
        }

        #endregion


    }
}
