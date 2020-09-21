using AutoMapper;
using Dapper;
using Foundry.Domain;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.LogService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;

namespace Foundry.Services
{
    public class ProgramAccountService : FoundryRepositoryBase<ProgramAccounts>, IProgramAccountService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IPlanProgramAccountLinkingService _planProgramAccount;
        private readonly IAccountTypeService _accountType;
        private readonly IPrograms _program;
        private readonly ISharedJPOSService _sharedJPOSService;
        public ProgramAccountService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, ILoggerManager logger,
            IMapper mapper, IPlanProgramAccountLinkingService planProgramAccount, IAccountTypeService accountType, IPrograms program,
            ISharedJPOSService sharedJPOSService)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _planProgramAccount = planProgramAccount;
            _accountType = accountType;
            _program = program;
            _sharedJPOSService = sharedJPOSService;
        }
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// This method will get the list of Plans.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AccountListingDto>> GetAccountListing(int programId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        ProgramId = programId,
                        IsActive = true,
                        IsDeleted = false
                    };
                    var accountListings = await sqlConnection.QueryAsync<AccountListingDto>(SQLQueryConstants.GetAccountLisitngQuery, obj);
                    return accountListings.ToList();
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

        public async Task<List<AccountListingDto>> GetProgramAccountsDropdownForUser(int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        IsActive = true,
                        IsDeleted = false,
                        UserId = userId
                    };
                    var accountListings = (await sqlConnection.QueryAsync<AccountListingDto>(SQLQueryConstants.GetProgramAccountsDropdownForUser, obj)).ToList();
                    accountListings = accountListings.Where(x => x.accountTypeId != 1).ToList();
                    return accountListings;
                }
                catch (Exception ex)
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

        public async Task<AccountPlanProgramDto> GetUserProgramPlanNOrgByProgramAccountId(int programAccountId, int userId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    return await sqlConnection.QuerySingleOrDefaultAsync<AccountPlanProgramDto>(SQLQueryConstants.GetUserProgramPlanNOrgByProgramAccountId, new { ProgramAccountId = programAccountId, IsActive = true, IsDeleted = false, OrganizationType = OrganasationType.University, UserId = userId });
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
        /// This method will get the list of Plans.
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AccountListingDto>> GetAccountBasedOnPlanNProgramSelection(int programId, int planId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        ProgramId = programId,
                        IsActive = true,
                        IsDeleted = false,
                        PlanId = planId
                    };
                    var accountListings = await sqlConnection.QueryAsync<AccountListingDto>(SQLQueryConstants.GetAccountBasedOnPlanNProgramSelection, obj);
                    return accountListings.ToList();
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
        public async Task<int> UpdateProgramAccountStatus(int Id, bool IsActive)
        {
            try
            {
                var progacc = await FindAsync(new { Id = Id });
                progacc.isActive = IsActive;
                return await UpdateAsync(progacc, new { progacc.id });
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<int> DeleteAccountById(int planId)
        {
            return await RemoveAsync(new { Id = planId });
        }
        public async Task<ProgramAccountDetailsWithMasterDto> GetProgramAccountInfoWithAccountType(int Id)
        {
            try
            {
                var accInfo = new ProgramAccountDetailsWithMasterDto();
                var accountType = _mapper.Map<List<AccountTypeDto>>(await _accountType.GetDataAsync(new { IsDeleted = false }));
                if (Id > 0)
                    accInfo = _mapper.Map<ProgramAccountDetailsWithMasterDto>(await GetDataByIdAsync(new { id = Id }));
                accInfo.AccountType = accountType;
                return accInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> AddEditProgramAccount(ProgramAccountViewModel model)
        {
            try
            {
                var chkExist = await FindAsync(new { id = model.id });
                if (chkExist == null)
                    chkExist = new ProgramAccounts();
                if (model.accountName != null)
                    chkExist.accountName = model.accountName;
                if (model.accountTypeId != 0)
                    chkExist.accountTypeId = model.accountTypeId;
                if (model.createdBy != null)
                    chkExist.createdBy = model.createdBy;
                if (model.createdDate != null)
                    chkExist.createdDate = model.createdDate;
                chkExist.exchangePassValue = model.exchangePassValue;
                chkExist.exchangeResetDay = model.exchangeResetDay;
                chkExist.exchangeResetPeriodType = model.exchangeResetPeriodType;
                chkExist.exchangeResetTime = model.exchangeResetTime;
                chkExist.flexEndDate = model.flexEndDate;
                if (model.intialBalanceCount != null)
                    chkExist.intialBalanceCount = model.intialBalanceCount;
                if (model.isActive != null)
                    chkExist.isActive = model.isActive;
                if (model.isDeleted != null)
                    chkExist.isDeleted = model.isDeleted;
                if (model.isPassExchangeEnabled != null)
                    chkExist.isPassExchangeEnabled = model.isPassExchangeEnabled;
                chkExist.isRollOver = model.isRollOver;
                if (model.maxPassUsage != null)
                    chkExist.maxPassUsage = model.maxPassUsage;
                if (model.modifiedBy != null)
                    chkExist.modifiedBy = model.modifiedBy;
                if (model.modifiedDate != null)
                    chkExist.modifiedDate = model.modifiedDate;
                chkExist.passType = model.passType;
                if (model.ProgramAccountId != null)
                    chkExist.ProgramAccountId = model.ProgramAccountId;
                chkExist.resetDay = model.resetDay;
                if (model.resetPeriodType != 0)
                    chkExist.resetPeriodType = model.resetPeriodType;
                if (model.programId != null)
                    chkExist.programId = model.programId;
                if (model.resetTime != null)
                    chkExist.resetTime = model.resetTime;
                chkExist.maxPassPerWeek = model.maxPassPerWeek;
                chkExist.maxPassPerMonth = model.maxPassPerMonth;
                chkExist.resetDateOfMonth = model.resetDateOfMonth;
                chkExist.flexMaxSpendPerDay = model.flexMaxSpendPerDay;
                chkExist.flexMaxSpendPerWeek = model.flexMaxSpendPerWeek;
                chkExist.flexMaxSpendPerMonth = model.flexMaxSpendPerMonth;

                chkExist.exchangeResetDateOfMonth = model.exchangeResetDateOfMonth;
                chkExist.vplMaxBalance = model.vplMaxBalance;
                chkExist.vplMaxAddValueAmount = model.vplMaxAddValueAmount;
                chkExist.vplMaxNumberOfTransaction = model.vplMaxNumberOfTransaction;
                chkExist.id = model.id;
                return await InsertOrUpdateAsync(chkExist, new { id = model.id });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> AddEditProgramAccoutDetails(ProgramAccountViewModel model, string clientIpAddress)
        {
            try
            {

                var Id = await AddEditProgramAccount(model);
                if (Id > 0)
                {
                    if (model.id == 0)
                    {
                        var id = model.Jpos_ProgramAccountId;
                        var progIssuerId = (await _program.FindAsync(new { id = model.programId }))?.JPOS_IssuerId;
                        var paid = "PA1000-" + Id.ToString();
                        var oPlanJPOS = new AccountsJposDto()
                        {
                            dailyLimit = Convert.ToDouble(model.flexMaxSpendPerDay.Value),
                            weeklyLimit = Convert.ToDouble(model.flexMaxSpendPerWeek.Value),
                            initialBalance = Convert.ToDouble(model.intialBalanceCount),
                            monthlyLimit = Convert.ToDouble(model.flexMaxSpendPerMonth),
                            issuer = progIssuerId,
                            active = true,
                            name = model.accountName,
                            layer = "001",
                            roleOver = model.isRollOver.HasValue && model.isRollOver.Value ? "Y" : "N"

                            // accounts = accounts.Count > 0 ? String.Join(", ", accounts.Select(x => x.accountName).ToList()) : ""
                        };
                        int result = await _sharedJPOSService.PostRespectiveDataJPOS(JPOSAPIURLConstants.Accounts, oPlanJPOS, id, clientIpAddress, JPOSAPIConstants.Accounts);
                        var chkExist = await FindAsync(new { id = Id });
                        chkExist.ProgramAccountId = paid;
                        await UpdateAsync(chkExist, new { id = Id });
                    }
                    return Cryptography.EncryptPlainToCipher(Id.ToString());
                }
                return "0";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AccountInfoDto> GetAccountDetailNCheckForBranding(int accountId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        AccountId = accountId,
                        IsActive = true,
                        IsDeleted = false
                    };
                    return await sqlConnection.QueryFirstOrDefaultAsync<AccountInfoDto>(SQLQueryConstants.GetAccountsDetailsByIdNCheckBranding, obj);

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

        public async Task<List<AccountInfoDto>> GetAccountsDetailsByIds(List<int> accountIds)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        AccountId = accountIds,
                        IsActive = true,
                        IsDeleted = false
                    };
                    return await sqlConnection.QueryFirstOrDefaultAsync<List<AccountInfoDto>>(SQLQueryConstants.GetAccountsDetailsByIds, obj);
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


        #region PrivateMethods
        public async Task<List<PassTypeDto>> GetPassTypeList()
        {
            List<PassTypeDto> result = new List<PassTypeDto>();
            PassTypeDto dt = new PassTypeDto();
            dt.Id = 1;
            dt.Type = "Unlimited";
            result.Add(dt);
            dt = new PassTypeDto();
            dt.Id = 2;
            dt.Type = "Block";
            result.Add(dt);
            dt = new PassTypeDto();
            dt.Id = 3;
            dt.Type = "Daily";
            result.Add(dt);
            dt = new PassTypeDto();
            dt.Id = 4;
            dt.Type = "Weekly";
            result.Add(dt);
            dt = new PassTypeDto();
            dt.Id = 5;
            dt.Type = "Monthly";
            result.Add(dt);
            return result;
        }
        public async Task<List<ResetPeriodDto>> GetResetPeriodList()
        {
            List<ResetPeriodDto> result = new List<ResetPeriodDto>();
            ResetPeriodDto dt = new ResetPeriodDto();
            dt.Id = 1;
            dt.Type = "Daily";
            result.Add(dt);
            dt = new ResetPeriodDto();
            dt.Id = 2;
            dt.Type = "Weekly";
            result.Add(dt);
            dt = new ResetPeriodDto();
            dt.Id = 3;
            dt.Type = "Monthly";
            result.Add(dt);
            return result;
        }
        public async Task<List<ExchangeResetPeriodDto>> ExchangeResetPeriodList()
        {
            List<ExchangeResetPeriodDto> result = new List<ExchangeResetPeriodDto>();
            ExchangeResetPeriodDto dt = new ExchangeResetPeriodDto();
            dt.Id = 1;
            dt.Type = "Daily";
            result.Add(dt);
            dt = new ExchangeResetPeriodDto();
            dt.Id = 2;
            dt.Type = "Weekly";
            result.Add(dt);
            dt = new ExchangeResetPeriodDto();
            dt.Id = 3;
            dt.Type = "Monthly";
            result.Add(dt);
            dt = new ExchangeResetPeriodDto();
            dt.Id = 4;
            dt.Type = "No reset";
            result.Add(dt);
            return result;
        }
        public async Task<List<WeekDayDto>> GetWeekDaysList()
        {
            List<WeekDayDto> result = new List<WeekDayDto>();
            var i = 0;
            foreach (var item in Enum.GetNames(typeof(DayOfWeek)))
            {
                i += 1;
                result.Add(new WeekDayDto() { DayName = item, Id = i });
            }
            return result;
        }
        #endregion
    }
}
