using Dapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using Foundry.Domain.DbModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;
using Microsoft.Extensions.Configuration;
using Foundry.LogService;
using System.Net.Http;
using System.Net.Http.Headers;
using Foundry.Services.PartnerNotificationsLogs;
using AutoMapper;

namespace Foundry.Services
{
    public class BenefactorService : FoundryRepositoryBase<BenefactorUsersLinking>, IBenefactorService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IGeneralSettingService _setting;
        private readonly IInvitationService _invitation;
        private readonly IReloadBalanceService _reloadRequest;
        private readonly IUsersProgram _userprograms;
        private readonly IUserTransactionInfoes _userTransactionInfo;
        private readonly IReloadRule _reloadRule;
        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;
        private readonly IPhotos _photos;
        private readonly IPartnerNotificationsLogServicer _partnerNotificationsLogRepository;
        private readonly IMapper _mapper;
        public BenefactorService(IDatabaseConnectionFactory databaseConnectionFactory,
            IGeneralSettingService setting, IInvitationService invitation, IReloadBalanceService reloadRequest,
            IUsersProgram userprograms, IUserTransactionInfoes userTransactionInfo, IReloadRule reloadRule, IConfiguration configuration,
            ILoggerManager logger, IPhotos photos,  IPartnerNotificationsLogServicer partnerNotificationsLogRepository,IMapper mapper)
        : base(databaseConnectionFactory)
        {
            _setting = setting;
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _invitation = invitation;
            _reloadRequest = reloadRequest;
            _userprograms = userprograms;
            _userTransactionInfo = userTransactionInfo;
            _reloadRule = reloadRule;
            _configuration = configuration;
            _logger = logger;
            _photos = photos;
            _partnerNotificationsLogRepository = partnerNotificationsLogRepository;
            _mapper = mapper;
        }


        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This method will return all the relations exists in the system.
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserRelations>> GetRelations()
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var relationship = await sqlConnection.QueryAsync<UserRelations>(SQLQueryConstants.GetRelationsQuery);

                    return relationship.ToList();
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
        /// This method will return the specific Relation based on its Id.
        /// </summary>
        /// <param name="relationshipId"></param>
        /// <returns></returns>
        public async Task<UserRelations> GetRelationById(int relationshipId)
        {
            object obj = new { Id = relationshipId, IsActive = true, IsDeleted = false };
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<UserRelations>(SQLQueryConstants.GetRelationByIdQuery, obj);
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
        /// This is used to check the existence of benefactor in User's table by its email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User> CheckExistingBenefactor(string email)
        {
            object obj = new { Email = email.Trim(), IsActive = true, IsDeleted = false };
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = await sqlConnection.QuerySingleOrDefaultAsync<User>(SQLQueryConstants.GetUserByEmailQuery, obj);
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
        /// This method is used to check the existence of connection between user and benefactor.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="benefactorId"></param>
        /// <returns></returns>
        public async Task<BenefactorUsersLinking> CheckConnectionUserBenefactor(int userId, int benefactorId)
        {
            try
            {
                object obj = new { UserId = userId, BenefactorId = benefactorId, IsActive = true, IsDeleted = false };
                return await GetDataByIdAsync(obj);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// This method will add/update benefactor in the related table against the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="benefactorId"></param>
        /// <param name="relationshipId"></param>
        /// <returns></returns>
        public async Task<int> AddUpdateBenefactorUserLinking(int userId, int benefactorId, int relationshipId)
        {
            try
            {
                object obj = new { UserId = userId, BenefactorId = benefactorId, IsActive = true, IsDeleted = false };
                var linkConnection = await GetDataByIdAsync(obj);
                if (linkConnection != null)
                {
                    linkConnection.isActive = true;
                    linkConnection.isDeleted = false;
                    linkConnection.relationshipId = relationshipId;
                    linkConnection.IsInvitationSent = true;
                    linkConnection.IsRequestAccepted = false;
                    linkConnection.modifiedDate = DateTime.UtcNow;
                    linkConnection.modifiedBy = userId;
                    linkConnection.canViewTransaction = true;
                    return await UpdateAsync(linkConnection, new { Id = linkConnection.id });
                }
                else
                {
                    var userConnection = new BenefactorUsersLinking
                    {
                        benefactorId = benefactorId,
                        canViewTransaction = true,
                        createdBy = userId,
                        IsInvitationSent = true,
                        IsRequestAccepted = false,
                        relationshipId = relationshipId,
                        userId = userId
                    };
                    return await AddAsync(userConnection);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method will return all the active connections as well as invitees of the user which exists in the system.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<List<BenefactorDto>> GetUserConnections(int userId, int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var reloadSetting = (await _setting.GetGeneratSettingValueByKeyGroup(GeneralSettingsConstants.Reload)).FirstOrDefault().Value;
                    object obj = new { UserId = userId, ProgramId = programId, Minutes = reloadSetting };
                    var result = (await sqlConnection.QueryAsync<BenefactorDto>(SQLQueryConstants.GetUserConnectionsQuery, obj)).ToList();
                    if (result.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            result[i].BenefactorImage = await _photos.GetAWSBucketFilUrl(result[i].BenefactorImage, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
                        }
                    }
                    List<BenefactorDto> asList = result.OrderByDescending(x => x.CreationDate).ToList();
                    return asList;
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
        /// This method will delete the connection for the user.
        /// </summary>
        /// <param name="model.UserId"></param>
        /// <param name="model.BenefactorId"></param>
        /// <returns></returns>
        public async Task<int> DeleteUserConnection(int type, int userId, int benefactorId)
        {
            try
            {
                int deleteResult = 0;
                if (type == ConnectionsType.ExistingConnection)
                {
                    /* Check for existing connection. */
                    object obj = new { UserId = userId, BenefactorId = benefactorId, IsActive = true, IsDeleted = false };
                    var connection = await GetDataByIdAsync(obj);
                    if (connection != null)
                    {
                        /* Delete operation is performed. */
                        connection.isActive = false;
                        connection.isDeleted = true;
                        connection.modifiedBy = userId;
                        connection.modifiedDate = DateTime.UtcNow;
                        object objphoto = new { EntityId = benefactorId, PhotoType = 5 };
                        using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
                        {
                            try
                            {
                                var checkImage = await sqlConnection.QuerySingleOrDefaultAsync<Photo>(SQLQueryConstants.GetBenefactorPhotoQuery, objphoto);
                                if (checkImage != null)
                                {
                                    objphoto = new { Id = checkImage.Id };
                                    await sqlConnection.QuerySingleOrDefaultAsync<Photo>(SQLQueryConstants.DeleteBenefactorPhotoQuery, objphoto);
                                }
                            }
                            catch (Exception)
                            {

                            }
                            finally
                            {
                                sqlConnection.Close();
                                sqlConnection.Dispose();
                            }

                        }
                        try
                        {
                            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
                            {
                                try
                                {
                                    var userBenefactor = (await sqlConnection.QuerySingleOrDefaultAsync<User>(SQLQueryConstants.GetUserByIdQuery, new { Id = benefactorId, IsActive = true, IsDeleted = false })).Email;
                                    var invitation = (await sqlConnection.QuerySingleOrDefaultAsync<Invitation>(SQLQueryConstants.GetInvitationByUserBenefactorEmailQuery, new { userId, EmailAddress = userBenefactor.Trim(), IsActive = true, IsDeleted = false }));
                                    if (invitation != null)
                                    {
                                        invitation.IsActive = false;
                                        invitation.IsDeleted = true;
                                        invitation.ModifiedBy = userId;
                                        invitation.ModifiedDate = DateTime.UtcNow;
                                        await sqlConnection.ExecuteAsync(SQLQueryConstants.UpdateInvitationByEmailQuery, invitation);
                                    }
                                    var reloadRequest = ((await sqlConnection.QueryAsync<ReloadBalanceRequest>(SQLQueryConstants.GetReloadRequestQuery, new { userId, benefactorId, IsActive = true, IsDeleted = false }))).ToList();
                                    if (reloadRequest.Count() > 0)
                                    {
                                        reloadRequest.ForEach(a => a.isActive = false);
                                        reloadRequest.ForEach(a => a.isActive = true);
                                        await sqlConnection.ExecuteAsync(SQLQueryConstants.UpdateReloadBalanceRequest, reloadRequest);
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
                        catch (Exception)
                        {
                            throw;
                        }
                        deleteResult = await UpdateAsync(connection, new { Id = connection.id });
                    }
                }
                else
                {
                    /* Check for existing connection. */
                    var invitation = await _invitation.GetSingleDataByConditionAsync(new { CreatedBy = userId, id = benefactorId, IsActive = true, IsDeleted = false });

                    if (invitation != null)
                    {
                        /* Delete operation is performed. */
                        invitation.IsActive = false;
                        invitation.IsDeleted = true;
                        invitation.ModifiedBy = userId;
                        invitation.ModifiedDate = DateTime.UtcNow;
                        deleteResult = await _invitation.UpdateAsync(invitation, new { Id = invitation.id });
                    }
                }
                return deleteResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> ReloadBalanceRequest(int userId, int benefactorId, int programId, decimal amount = 0)
        {
            try
            {
                int requestId = 0;
                var isReloadExist = true;
                var reloadRequestInfo = await _reloadRequest.FindAsync(new { UserId = userId, BenefactorUserId = benefactorId, IsRequestAccepted = false, IsDeleted = false });
                if (reloadRequestInfo == null)
                {
                    reloadRequestInfo = new ReloadBalanceRequest();
                    isReloadExist = false;
                }
                reloadRequestInfo.userId = userId;
                reloadRequestInfo.benefactorUserId = benefactorId;
                reloadRequestInfo.isRequestAccepted = false;
                reloadRequestInfo.requestedAmount = amount > 0 ? amount : 0;
                reloadRequestInfo.programId = programId;
                reloadRequestInfo.createdBy = userId;
                reloadRequestInfo.modifiedBy = userId;
                reloadRequestInfo.modifiedDate = DateTime.UtcNow;
                reloadRequestInfo.isActive = true;
                reloadRequestInfo.isDeleted = false;

                if (isReloadExist)
                {
                    await _reloadRequest.UpdateAsync(reloadRequestInfo, new { Id = reloadRequestInfo.id }); requestId = reloadRequestInfo.id;
                }
                else { requestId = await _reloadRequest.AddAsync(reloadRequestInfo); }

                return requestId;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<int> PartnerReloadBalanceRequest(int userId, int benefactorId, int programId, string Message, decimal amount = 0)
        {
            try
            {
                int requestId = 0;
                var isReloadExist = true;
                var reloadRequestInfo = await _reloadRequest.FindAsync(new { UserId = userId, BenefactorUserId = benefactorId, IsRequestAccepted = false, IsDeleted = false });
                if (reloadRequestInfo == null)
                {
                    reloadRequestInfo = new ReloadBalanceRequest();
                    isReloadExist = false;
                }
                reloadRequestInfo.userId = userId;
                reloadRequestInfo.benefactorUserId = benefactorId;
                reloadRequestInfo.isRequestAccepted = false;
                reloadRequestInfo.requestedAmount = amount > 0 ? amount : 0;
                reloadRequestInfo.programId = programId;
                reloadRequestInfo.createdBy = userId;
                reloadRequestInfo.modifiedBy = userId;
                reloadRequestInfo.modifiedDate = DateTime.UtcNow;
                reloadRequestInfo.isActive = true;
                reloadRequestInfo.isDeleted = false;
                reloadRequestInfo.Message = Message;

                if (isReloadExist)
                {
                    await _reloadRequest.UpdateAsync(reloadRequestInfo, new { Id = reloadRequestInfo.id }); requestId = reloadRequestInfo.id;
                }
                else { requestId = await _reloadRequest.AddAsync(reloadRequestInfo); }

                return requestId;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<BenefactorUsersLinking> CheckForExistingUserLinkingWithEmail(int userId, string benefactorEmail)
        {

            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                var userBenefactorId = 0;
                try
                {
                    var userBenefactor = (await sqlConnection.QuerySingleOrDefaultAsync<User>(SQLQueryConstants.GetUserByEmailQuery, new { EmailAddress = benefactorEmail, IsActive = true, IsDeleted = false }));
                    userBenefactorId = userBenefactor != null ? userBenefactor.Id : 0;
                    object obj = new { UserId = userId, BenefactorId = userBenefactorId, IsActive = true, IsDeleted = false };
                    return await GetDataByIdAsync(obj);
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

        public async Task<List<LinkedUsersTransactionsDto>> GetLinkedUsersTransactions(int linkedUserId, DateTime? dateMonth, string plan)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var transactions = (await sqlConnection.QueryAsync<LinkedUsersTransactionsDto>(SQLQueryConstants.GetLinkedUsersTransactionQuery, new { LinkedUserId = linkedUserId, DateMonth = dateMonth, Plan = plan })).ToList();
                    return transactions;
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

        public async Task<List<LinkedUsersDto>> GetLinkedUsersOfBenefactor(int benefactorId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var transactions = (await sqlConnection.QueryAsync<LinkedUsersDto>(SQLQueryConstants.GetLinkedUsersOfBenefactorQuery, new { BenefactorId = benefactorId, ImageType = (int)PhotoEntityType.UserProfile })).Distinct().ToList();
                    if (transactions.Count > 0)
                    {
                        for (int i = 0; i < transactions.Count; i++)
                        {
                            transactions[i].ImageUserPath = await _photos.GetAWSBucketFilUrl(transactions[i].ImageUserPath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
                        }
                    }
                    return transactions;
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

            };
        }
        public async Task<List<LinkedUsersDto>> BenefectorDetails(int benefactorId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var transactions = (await sqlConnection.QueryAsync<LinkedUsersDto>(SQLQueryConstants.BenefectorDetails, new { BenefactorId = benefactorId})).Distinct().ToList();
                    
                    return transactions;
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

            };
        }
        public async Task<List<LinkedUsersDto>> GetLinkedUsersInformationOfBenefactor(int benefactorId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var transactions = (await sqlConnection.QueryAsync<LinkedUsersDto>(SQLQueryConstants.GetLinkedUsersInformationOfBenefactorQuery, new { BenefactorId = benefactorId, ImageType = (int)PhotoEntityType.UserProfile })).Distinct().ToList();
                    if (transactions.Count > 0)
                    {
                        for (int i = 0; i < transactions.Count; i++)
                        {
                            transactions[i].ImageUserPath = await _photos.GetAWSBucketFilUrl(transactions[i].ImageUserPath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
                        }
                    }
                    return transactions;

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

            };
        }

        public async Task<List<LinkedUsersDto>> GetLinkedUsersInformationOfBenefactorWithPrivacySettings(int benefactorId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var transactions = (await sqlConnection.QueryAsync<LinkedUsersDto>(SQLQueryConstants.GetLinkedUsersInformationOfBenefactorWithPrivacySettingQuery, new { BenefactorId = benefactorId, ImageType = (int)PhotoEntityType.UserProfile })).Distinct().ToList();
                    if (transactions.Count > 0)
                    {
                        for (int i = 0; i < transactions.Count; i++)
                        {
                            transactions[i].ImageUserPath = await _photos.GetAWSBucketFilUrl(transactions[i].ImageUserPath, string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage));
                        }

                    }
                    return transactions;
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

            };
        }

        public async Task<int> ReloadUserBalance(ReloadRequestModel model)
        {
            try
            {
                if (model.ReloadRequestId.HasValue && model.ReloadRequestId > 0)
                {
                    var reloadRequest = await _reloadRequest.GetSingleDataByConditionAsync(new { id = model.ReloadRequestId, isRequestAccepted = false, isDeleted = false });
                    if (reloadRequest != null)
                    {
                        reloadRequest.isRequestAccepted = true;
                        reloadRequest.programId = model.ProgramId;
                        reloadRequest.modifiedBy = model.BenefactorUserId;
                        reloadRequest.modifiedDate = DateTime.UtcNow;
                        await _reloadRequest.UpdateAsync(reloadRequest, new { Id = reloadRequest.id });
                    }
                }
                var checkAutoReload = await GetReloadRuleOfUser(model.ReloadUserId, model.BenefactorUserId).ConfigureAwait(false);
                int reloadruleId = 0;
                if (model.IsAutoReload)
                {
                    if (checkAutoReload == null) { checkAutoReload = new ReloadRules(); }
                    else { reloadruleId = checkAutoReload.id; }
                    checkAutoReload.userId = model.ReloadUserId;
                    checkAutoReload.benefactorUserId = model.BenefactorUserId;
                    checkAutoReload.isAutoReloadAmount = model.IsAutoReload;
                    checkAutoReload.modifiedDate = DateTime.UtcNow;
                    checkAutoReload.programId = model.ProgramId;
                    checkAutoReload.reloadAmount = model.AutoReloadAmount;
                    checkAutoReload.userDroppedAmount = model.CheckDroppedAmount;
                    checkAutoReload.isActive = true;
                    checkAutoReload.isDeleted = false;

                    if (reloadruleId > 0) { await _reloadRule.UpdateAsync(checkAutoReload, new { Id = reloadruleId }); } else { await _reloadRule.AddAsync(checkAutoReload); }

                }

                return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }


        


        public async Task<int> AddUpdateReloadrule(ReloadRulesModel model)
        {
            try
            {
                var checkAutoReload = await GetReloadRuleOfUser(model.ReloadUserId, model.BenefactorUserId).ConfigureAwait(false);
                int reloadruleId = 0;
                if (model.IsAutoReload)
                {
                    if (checkAutoReload == null)
                    {
                        checkAutoReload = new ReloadRules();
                    }
                    else { reloadruleId = checkAutoReload.id; }
                    checkAutoReload.userId = model.ReloadUserId;
                    checkAutoReload.benefactorUserId = model.BenefactorUserId;
                    checkAutoReload.isAutoReloadAmount = model.IsAutoReload;
                    checkAutoReload.modifiedDate = DateTime.UtcNow;
                    // checkAutoReload.programId = model.ProgramId;
                    checkAutoReload.reloadAmount = model.AutoReloadAmount;
                    checkAutoReload.userDroppedAmount = model.CheckDroppedAmount;
                    checkAutoReload.isActive = true;
                    checkAutoReload.isDeleted = false;
                    checkAutoReload.i2cBankAccountId = model.i2cBankAccountId;
                    checkAutoReload.CardId = model.CardId;

                    if (reloadruleId > 0) { await _reloadRule.UpdateAsync(checkAutoReload, new { Id = reloadruleId }); } else { await _reloadRule.AddAsync(checkAutoReload); }

                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<IEnumerable<ReloadRequestDto>> CancelReloadRule(ReloadRequestModel model)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var rule = (await sqlConnection.QueryAsync<ReloadRequestDto>(SQLQueryConstants.CancelReloadRuleQuery, new { uid = model.ReloadUserId,buid=model.BenefactorUserId }));

                    return rule;
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


        public async Task<decimal> GetRemainingBalanceOfUser(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var currentBalance = (await sqlConnection.QueryAsync<decimal>(SQLQueryConstants.GetRemainingBalanceOfUserQuery, new { UserId = userId })).FirstOrDefault();
                    return currentBalance <= 0 ? 0 : currentBalance;
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

            };
        }


        public async Task<List<ReloadRuleForDisplay>> GetAllReloadRuleforUser(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<ReloadRuleForDisplay>(SQLQueryConstants.GetAllReloadRuleOfUserQuery, new { UserId = userId })).ToList();
                    return result;
                }
                catch (Exception ex)
                {
                    string a = ex.Message;
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            };
        }
        

        public async Task<List<ReloadRules>> GetUserReloadRule(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<ReloadRules>(SQLQueryConstants.GetReloadRuleUserQuery, new { UserId = userId })).ToList();
                    return result;
                }
                catch (Exception ex)
                {
                    string a = ex.Message;
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            };
        }
        public async Task<ReloadRules> GetUserReloadRuleForTrigger(int userId, decimal Balance )
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<ReloadRules>(SQLQueryConstants.GetReloadRuleForTriggerQuery, new { UserId = userId, Balance= Balance })).FirstOrDefault<ReloadRules>();
                    return result;
                }
                catch (Exception ex)
                {
                    string a = ex.Message;
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

            };
        }


        public async Task<ReloadRules> GetReloadRuleOfUser(int userId, int benefactorId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var result = (await sqlConnection.QueryAsync<ReloadRules>(SQLQueryConstants.GetReloadRuleOfUserQuery, new { UserId = userId, BenefactorId = benefactorId })).FirstOrDefault();
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

            };
        }

        public async Task<int> DeleteLinkedAccounts(int type, int userId, int benefactorId)
        {
            try
            {
                int deleteResult = 0;
                if (type == ConnectionsType.ExistingConnection)
                {
                    /* Check for existing connection. */
                    var connection = (await GetDataByIdAsync(new { UserId = userId, BenefactorId = benefactorId, IsActive = true }));

                    if (connection != null)
                    {
                        /* Delete operation is performed. */
                        connection.isActive = false;
                        connection.isDeleted = true;
                        connection.modifiedBy = benefactorId;
                        connection.modifiedDate = DateTime.UtcNow;
                        await UpdateAsync(connection, new { connection.id });

                        try
                        {
                            var userBenefactor = string.Empty;
                            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
                            {
                                try
                                {
                                    userBenefactor = (await sqlConnection.QuerySingleOrDefaultAsync<User>(SQLQueryConstants.GetUserByIdQuery, new { Id = benefactorId, IsActive = true, IsDeleted = false })).Email;
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
                            var invitation = (await _invitation.GetDataByIdAsync(new { CreatedBy = userId, Email = userBenefactor }));
                            if (invitation != null)
                            {
                                invitation.IsActive = false;
                                invitation.IsDeleted = true;
                                invitation.ModifiedBy = benefactorId;
                                invitation.ModifiedDate = DateTime.UtcNow;
                                await _invitation.UpdateAsync(invitation, new { invitation.id });
                            }
                            var reloadRequest = (await _reloadRequest.AllAsync()).ToList().Where(x => x.userId == userId && x.benefactorUserId == benefactorId);
                            if (reloadRequest.Count() > 0)
                            {
                                foreach (var item in reloadRequest)
                                {
                                    item.isActive = false;
                                    item.isDeleted = true;
                                    await _reloadRequest.UpdateAsync(item, new { item.id });
                                }
                            }
                            deleteResult = 1;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    /* Check for existing connection. */
                    var invitation = await _invitation.GetDataByIdAsync(new { CreatedBy = userId, Id = benefactorId, IsActive = true });

                    if (invitation != null)
                    {
                        /* Delete operation is performed. */
                        invitation.IsActive = false;
                        invitation.IsDeleted = true;
                        invitation.ModifiedBy = userId;
                        invitation.ModifiedDate = DateTime.UtcNow;
                        deleteResult = await _invitation.UpdateAsync(invitation, new { invitation.id });
                    }
                }
                return deleteResult;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<PrivacyDto> GetPrivacySettings(int userId, int programId)
        {
            _logger.LogInfo("=================================");
            _logger.LogInfo("Get privacy setting started");
            var privacySettings = new PrivacyDto();
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                _logger.LogInfo("Connection Made.");
                try
                {
                    _logger.LogInfo("UserID :" + userId + ".");
                    _logger.LogInfo("ProgramId :" + programId + ".");
                    _logger.LogInfo("ImagePath :" + string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage) + ".");
                    _logger.LogInfo("IsRequestAccepted :TRUE .");
                    privacySettings.Benefator = (await sqlConnection.QueryAsync<BenefactorDto>(SQLQueryConstants.GetPrivacySettingQuery, new { UserId = userId, ProgramId = programId, ImagePath = string.Concat(_configuration["ServiceAPIURL"], ImagesDefault.UserDefaultImage), IsRequestAccepted = true })).ToList();
                    _logger.LogInfo("Get benefactor Details.");
                    if (privacySettings.Benefator.Count > 0 && privacySettings.Benefator.Where(x => x.CanViewTransaction == true).Count() > 0)
                    {
                        _logger.LogInfo("If benefactor count > 0  && privacySettings.Benefator.Where(x=>x.CanViewTransaction == true).Count() > 0 Then IsOnlyMe = False");
                        privacySettings.IsOnlyMe = false;
                    }
                    else
                    {
                        _logger.LogInfo("IsOnlyMe = True.");
                        privacySettings.IsOnlyMe = true;
                    }
                    _logger.LogInfo("Get privacy setting endeds");
                    return privacySettings;
                }
                catch (Exception ex)
                {
                    _logger.LogInfo("Exception = " + ex.ToString());
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                    _logger.LogInfo("Connection Closed.");
                }
            }
        }
        public async Task<PrivacyDto> UdatePrivacySettings(bool isOnlyMe, int id, int userId, int programId, bool status)
        {
            try
            {
                _logger.LogInfo("=================================");
                _logger.LogInfo("Update Privacy Settings started");
                _logger.LogInfo("isOnlyMe :" + isOnlyMe.ToString() + ".");
                _logger.LogInfo("id :" + id.ToString() + ".");
                _logger.LogInfo("userId :" + userId.ToString() + ".");
                _logger.LogInfo("programId :" + programId.ToString() + ".");
                _logger.LogInfo("status :" + status.ToString() + ".");
                if (!isOnlyMe && id > 0)
                {
                    _logger.LogInfo("!isOnlyMe && id > 0.");
                    object obj = new { Id = id, IsActive = true, IsDeleted = false };
                    _logger.LogInfo("ID : " + id.ToString());
                    _logger.LogInfo("Get data by ID.");
                    var linkConnection = await GetDataByIdAsync(obj);
                    _logger.LogInfo("Data got.");
                    if (linkConnection != null)
                    {
                        linkConnection.modifiedDate = DateTime.UtcNow;
                        linkConnection.modifiedBy = userId;
                        linkConnection.canViewTransaction = status;
                        await UpdateAsync(linkConnection, new { Id = linkConnection.id });
                        _logger.LogInfo("Data updated.");
                    }
                }
                else
                {
                    _logger.LogInfo("ELSE");
                    object obj = new { UserId = userId, IsActive = true, IsDeleted = false, IsRequestAccepted = true };
                    _logger.LogInfo("userId : " + userId.ToString());
                    _logger.LogInfo("Get all data for user.");
                    var linkConnection = await GetDataAsync(obj);
                    _logger.LogInfo("Data got.");
                    if (linkConnection.ToList().Count() > 0)
                    {
                        _logger.LogInfo("foreach started");
                        foreach (var item in linkConnection)
                        {
                            _logger.LogInfo("Inside foreach.");
                            item.canViewTransaction = false;
                            await UpdateAsync(item, new { Id = item.id }); ;
                        }
                        _logger.LogInfo("foreach ENDED");
                    }
                }

                var privacySettings = await GetPrivacySettings(userId, programId).ConfigureAwait(false);
                _logger.LogInfo("Get privacy setting ended");
                _logger.LogInfo("=================================");
                return privacySettings;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<List<GeneralSetting>> GetGeneralSettings()
        {
            var settings = await _setting.GetDataAsync(new { KeyName = SodexoBiteNotification.SodexoBiteBaseUrl });
            return _mapper.Map<List<GeneralSetting>>(settings);
        }
        public async Task sendReloadSetUpNotification(string benefactor, string vPartnerUserId, int userid, decimal vAmountAdded, decimal ThresholdAmount)
        {
            PartnerNotificationsLog PartnerNotificationsLogReq = new PartnerNotificationsLog();
            try
            {
                using (var client = new HttpClient())
                {
                    object obj = new
                    {

                        UserObjectId = vPartnerUserId,
                        Lowbalancethreshold = ThresholdAmount,
                        BenefactorName = benefactor,
                        Addvalueamount = vAmountAdded

                    };
                    List<GeneralSetting> i2cSettings = await GetGeneralSettings();

                    var url = i2cSettings[0].Value;
                    var hostURL = new Uri($"" + url + "/" + SodexoBiteNotification.ReloadSetUp);

                    string myJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    PartnerNotificationsLogReq = await _partnerNotificationsLogRepository.PartnerNotificationsLogRequest("AddUpdateReloadRules", hostURL.ToString(), myJSON, userid);
                    HttpContent stringContent = new StringContent(myJSON);
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                    // var content = new StringContent(myJSON.ToString(), Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(hostURL, stringContent);
                    PartnerNotificationsLogReq.Status = result.StatusCode.ToString();
                    var content = await result.Content.ReadAsStringAsync();
                    await _partnerNotificationsLogRepository.PartnerNotificationsLogResponse(PartnerNotificationsLogReq, content);

                }
            }
            catch (Exception ex)
            {

                PartnerNotificationsLogReq.Status = "Error";
                await _partnerNotificationsLogRepository.PartnerNotificationsLogResponse(PartnerNotificationsLogReq, ex.Message);


            }
        }

            public async Task sendStatuschangeNotification(string vPartnerUserId, int userid, string previousStatus, string currentStatus)
            {
            string date = DateTime.Now.ToString("yyyy-MM-dd");          
            var time = DateTime.Now.ToLongTimeString();
            PartnerNotificationsLog PartnerNotificationsLogReq = new PartnerNotificationsLog();
                try
                {
                    using (var client = new HttpClient())
                    {
                        object obj = new
                        {

                            UserObjectId = vPartnerUserId,
                            Date = date,
                            Time = time,
                            PreviousStatus= previousStatus,
                            CurrentStatus= currentStatus
                        };
                        List<GeneralSetting> i2cSettings = await GetGeneralSettings();

                        var url = i2cSettings[0].Value;
                        var hostURL = new Uri($"" + url + "/" + SodexoBiteNotification.StatusChange);

                        string myJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        PartnerNotificationsLogReq = await _partnerNotificationsLogRepository.PartnerNotificationsLogRequest("ChangeStatus", hostURL.ToString(), myJSON, userid);
                        HttpContent stringContent = new StringContent(myJSON);
                        stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        var result = await client.PostAsync(hostURL, stringContent);

                    PartnerNotificationsLogReq.Status = result.StatusCode.ToString();
                    var content = await result.Content.ReadAsStringAsync();
                        await _partnerNotificationsLogRepository.PartnerNotificationsLogResponse(PartnerNotificationsLogReq, content);

                    }
                }
                catch (Exception ex)
                {

                PartnerNotificationsLogReq.Status = "Error";
                await _partnerNotificationsLogRepository.PartnerNotificationsLogResponse(PartnerNotificationsLogReq, ex.Message);


                }
            }
    }
}
