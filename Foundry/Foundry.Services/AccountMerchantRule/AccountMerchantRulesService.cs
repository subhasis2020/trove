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

namespace Foundry.Services
{
    public class AccountMerchantRulesService : FoundryRepositoryBase<AccountMerchantRules>, IAccountMerchantRulesService
    {
        private readonly IAccountMerchantRulesDetailService _accMerchantRuleDetail;
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;
        public AccountMerchantRulesService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, ILoggerManager logger,
            IMapper mapper, IAccountMerchantRulesDetailService accMerchantRuleDetail)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _accMerchantRuleDetail = accMerchantRuleDetail;
        }
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<AccountMerchantRuleDto> GetAccountMerchantRule(int programId, string businessTypeId, string accountTypeId, int accountId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetMerchantForAccountMerchantRuleQuery, new { OrganisationType = Constants.OrganasationType.Merchant, ProgramId = programId, BusinessType = businessTypeId, AccountTypeId = accountTypeId, ProgramAccountId = accountId }).Result)
                {
                    try
                    {
                        var obj = new
                        {
                            ProgramId = programId,
                            BusinessTypeId = businessTypeId,
                            AccountTypeId = accountTypeId
                        };
                        var merchantInfo = new AccountMerchantRuleDto();
                        var businessType = multi.Read<BusinessTypeDto>().ToList();
                        var merchants = multi.Read<AccountMerchantRuleMerchantDto>().ToList();
                        var accmerchantsrules = multi.Read<AccountMerchantRuleAndDetailDto>().ToList();
                        merchantInfo.BusinessTypes = businessType;
                        merchantInfo.Merchants = merchants;
                        merchantInfo.AccountMerchantRuleAndDetail = accmerchantsrules;
                        return merchantInfo;
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

        public async Task<string> AddEditAccountMerchantRules(AccountMerchantRuleViewModel model)
        {
            try
            {
                if (model.selectedMerchant != null && model.selectedMerchant.Count > 0)
                {
                    var accMerchantRule = await GetDataAsync(new { model.programAccountId });
                    foreach (var at in model.accountTypeId.Split(','))
                    {
                        foreach (var item in model.selectedMerchant.Where(x => x.IsSelected == false))
                        {
                            var id = await RemoveAsync(new { merchantId = item.Id, model.programAccountId, accountTypeId = Convert.ToInt32(at) });
                            await _accMerchantRuleDetail.DeleteEntityAsync(new { accountMerchantRuleId = id });
                        }
                        foreach (var item in model.selectedMerchant.Where(x => x.IsSelected == true))
                        {
                            var obj = await GetDataByIdAsync(new { merchantId = item.Id, model.programAccountId, accountTypeId = Convert.ToInt32(at) });

                            if (obj == null)
                                obj = new AccountMerchantRules();
                            obj.isActive = true;
                            obj.isDeleted = false;
                            obj.merchantId = Convert.ToInt32(item.Id);
                            obj.programAccountID = model.programAccountId;
                            obj.accountTypeId = Convert.ToInt32(at);
                            var id = await InsertOrUpdateAsync(obj, new { obj.id });
                            if (id > 0)
                            {
                                var objaccMerchantRuleDetail = await _accMerchantRuleDetail.GetDataAsync(new { accountMerchantRuleId = id });
                                if (objaccMerchantRuleDetail.Count() == 0)
                                {
                                    foreach (var mp in model.accountMerchantRuleAndDetails)
                                    {
                                        var objmp = new AccountMerchantRulesDetail();
                                        objmp.maxPassUsage = mp.maxPassUsage;
                                        objmp.maxPassValue = mp.maxPassValue;
                                        objmp.mealPeriod = mp.mealPeriodId;
                                        objmp.minPassValue = mp.minPassValue;
                                        objmp.transactionLimit = mp.transactionLimit;
                                        objmp.accountMerchantRuleId = id;
                                        objmp.id = mp.id;
                                        await _accMerchantRuleDetail.InsertOrUpdateAsync(objmp, new { objmp.id });
                                    }
                                }
                            }

                        }
                    }
                    return "1";
                }
                return "0";
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> EditAccountMerchantRuleDetails(List<AccountMerchantRuleAndDetailViewModel> model)
        {
            try
            {
                if (model.Count > 0)
                {
                    foreach (var mp in model)
                    {
                        var objmp = await _accMerchantRuleDetail.GetDataByIdAsync(new { mp.id, mealPeriod = mp.mealPeriodId });
                        if (objmp != null)
                        {
                            objmp.maxPassUsage = mp.maxPassUsage;
                            objmp.maxPassValue = mp.maxPassValue;
                            objmp.mealPeriod = mp.mealPeriodId;
                            objmp.minPassValue = mp.minPassValue;
                            objmp.transactionLimit = mp.transactionLimit;
                            objmp.id = mp.id;
                            await _accMerchantRuleDetail.InsertOrUpdateAsync(objmp, new { objmp.id });
                        }
                    }
                    return "1";
                }
                return "0";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
