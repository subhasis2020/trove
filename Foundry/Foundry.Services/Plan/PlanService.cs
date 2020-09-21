using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundry.Domain.Dto;
using Foundry.Domain;
using Dapper;
using System.Linq;
using Foundry.LogService;
using Foundry.Domain.ApiModel;
using AutoMapper;
using static Foundry.Domain.Constants;

namespace Foundry.Services
{
    public class PlanService : FoundryRepositoryBase<ProgramPackage>, IPlanService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IPlanProgramAccountLinkingService _planProgramAccount;
        private readonly IPrograms _program;
        private readonly ISharedJPOSService _sharedJPOSService;
        private readonly IProgramAccountService _programAccounts;
        public PlanService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, ILoggerManager logger,
            IMapper mapper, IPlanProgramAccountLinkingService planProgramAccount, IPrograms program, ISharedJPOSService sharedJPOSService,
            IProgramAccountService programAccounts)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _planProgramAccount = planProgramAccount;
            _program = program;
            _sharedJPOSService = sharedJPOSService;
            _programAccounts = programAccounts;
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
        public async Task<IEnumerable<PlanListingDto>> GetPlanListing(int programId)
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
                    var plan = await sqlConnection.QueryAsync<PlanListingDto>(SQLQueryConstants.GetPlanLisitngQuery, obj);
                    return plan.ToList();
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
        public async Task<int> UpdatePlanStatus(int PlanId, bool IsActive)
        {
            try
            {
                var plan = await FindAsync(new { Id = PlanId });
                plan.isActive = IsActive;
                return await UpdateAsync(plan, new { plan.id });
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<int> DeletePlanById(int planId)
        {
            return await RemoveAsync(new { Id = planId });
        }
        public async Task<int> AddEditPlan(PlanViewModel model)
        {
            try
            {
                var chkExist = await FindAsync(new { id = model.id });
                if (chkExist == null)
                    chkExist = new ProgramPackage();
                if (model.clientId != null)
                    chkExist.clientId = model.clientId;
                if (model.createdBy != null)
                    chkExist.createdBy = model.createdBy;
                if (model.createdDate != null)
                    chkExist.createdDate = model.createdDate;
                if (model.description != null)
                    chkExist.description = model.description;
                chkExist.endDate = model.endDate;
                chkExist.endTime = model.endTime;
                if (model.isActive != null)
                    chkExist.isActive = model.isActive;
                if (model.isDeleted != null)
                    chkExist.isDeleted = model.isDeleted;
                if (model.modifiedBy != null)
                    chkExist.modifiedBy = model.modifiedBy;
                if (model.modifiedDate != null)
                    chkExist.modifiedDate = model.modifiedDate;
                if (model.name != null)
                    chkExist.name = model.name;
                chkExist.noOfFlexPoints = model.noOfFlexPoints;
                chkExist.noOfMealPasses = model.noOfMealPasses;
                if (model.planId != null)
                    chkExist.planId = model.planId;
                if (model.programId != 0)
                    chkExist.programId = model.programId;
                chkExist.startDate = model.startDate;
                chkExist.startTime = model.startTime;
                chkExist.id = model.id;
                return await InsertOrUpdateAsync(chkExist, new { id = model.id });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> AddEditPlanDetails(PlanViewModel model, string clientIpAddress)
        {
            try
            {
                var progIssuerId = (await _program.FindAsync(new { id = model.programId }))?.JPOS_IssuerId;
                var accounts = await _programAccounts.GetAccountsDetailsByIds(model.PlanProgramAccount.Select(x => x.programAccountId).ToList());

                var pId = await AddEditPlan(model);
                if (pId > 0)
                {
                    if (model.id == 0)
                    {
                        var id = model.Jpos_PlanId;
                        var planId = "P1000-" + pId.ToString();
                        var oPlanJPOS = new PlanJposDto()
                        {
                            clientId = model.clientId,
                            active = true,
                            description = model.description,
                            endDate = model.endDate.Value,
                            issuer = progIssuerId,
                            startDate = model.startDate.Value,
                            name = model.name,
                            accounts = accounts.Count > 0 ? String.Join(", ", accounts.Select(x => x.accountName).ToList()) : ""
                        };
                        int result = await _sharedJPOSService.PostRespectiveDataJPOS(JPOSAPIURLConstants.Plans, oPlanJPOS, id, clientIpAddress, JPOSAPIConstants.Plans);
                        var chkExist = await FindAsync(new { id = pId });
                        chkExist.planId = planId;
                        chkExist.Jpos_PlanId = Convert.ToString(result);
                        await UpdateAsync(chkExist, new { id = pId });
                    }
                    var planprogramAccount = _mapper.Map<List<PlanProgramAccountsLinking>>(model.PlanProgramAccount);
                    planprogramAccount.ToList().ForEach(x => x.planId = pId);
                    await _planProgramAccount.AddUpdatePlanProgramAccount(planprogramAccount);
                    return Cryptography.EncryptPlainToCipher(pId.ToString());
                }
                return "0";
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<PlanDetailsWithMasterDto> GetPlanInfoWithProgramAccount(int pId, int ppId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                using (var multi = sqlConnection.QueryMultipleAsync(SQLQueryConstants.GetPlanDetailInformationWithMasterQuery, new { PlanId = pId, ProgramId = ppId }).Result)
                {
                    try
                    {
                        var planInfo = new PlanDetailsWithMasterDto();
                        var programAccount = multi.Read<ProgramAccountDto>().ToList();
                        if (pId > 0)
                            planInfo = multi.Read<PlanDetailsWithMasterDto>().FirstOrDefault();
                        var planProgramAccount = multi.Read<PlanProgramAccountLinkingDto>().ToList();
                        planInfo.ProgramAccount = programAccount;
                        planInfo.PlanProgramAccount = planProgramAccount;
                        return planInfo;
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
}
