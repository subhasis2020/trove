using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Foundry.Domain.Constants;

namespace Foundry.Services
{
    public class MerchantTerminals : FoundryRepositoryBase<MerchantTerminal>, IMerchantTerminals
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private readonly ISharedJPOSService _sharedJPOSService;
        private readonly IOrganisation _organisation;
        private readonly IOrganisationProgram _organisationProgram;
        private readonly IPrograms _program;
        public MerchantTerminals(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration,
            ISharedJPOSService sharedJPOSService, IOrganisation organisation, IOrganisationProgram organisationProgram,
            IPrograms program)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _sharedJPOSService = sharedJPOSService;
            _organisation = organisation;
            _organisationProgram = organisationProgram;
            _program = program;
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<int> AddEditMerchantTerminal(OrganisationMerchantTerminalModel model)
        {
            try
            {
                var obj = new MerchantTerminal();
                obj.id = model.id;
                obj.organisationId = model.organisationId;
                obj.terminalId = model.terminalId;
                obj.terminalName = model.terminalName;
                obj.terminalType = model.terminalType;
                return await InsertOrUpdateAsync(obj, new { model.id });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> AddEditMerchantTerminal(List<OrganisationMerchantTerminalModel> model, string clientIpAddress)
        {
            int isSucces = 0;
            try
            {
                var orgid = model.FirstOrDefault().organisationId;
                var org = await _organisation.FindAsync(new { id = orgid });
                var orgPrg = await _organisationProgram.FindAsync(new { organisationId = orgid });
                var prd = await _program.FindAsync(new { id = orgPrg?.programId });
                var result = await GetMultipleDataByConditionAsync(new { organisationId = orgid });
                var DeletionChk = result.ToList().Where(x => !model.Select(m => m.id).ToList().Contains(x.id));
                var JposTerminalURL = JPOSAPIURLConstants.Terminals.Replace("{issuer}", prd?.JPOS_IssuerId).Replace("{merchant}", org?.JPOS_MerchantId);
                if (DeletionChk.ToList().Count > 0)
                {
                    foreach (var itemDelete in DeletionChk)
                    {
                        var jposId = itemDelete.Jpos_TerminalId;
                        TerminalJposDto oTerminalJpos = new TerminalJposDto()
                        {
                            active = false
                        };
                        await _sharedJPOSService.DeleteRespectiveDataJPOS(JposTerminalURL, oTerminalJpos, jposId, clientIpAddress, JPOSAPIConstants.Terminals);
                        await DeleteEntityAsync(new { itemDelete.id });
                    }
                }
                foreach (var item in model)
                {
                    var merchantExistCheck = await FindAsync(new { item.id });
                    var terminalTypeVal = (await _organisation.GetTerminalTypeList()).FirstOrDefault(x => x.Id == item.terminalType.Value).TerminalType;
                    if (merchantExistCheck != null)
                    {
                        merchantExistCheck.id = item.id;
                        merchantExistCheck.Jpos_TerminalId = item.Jpos_TerminalId;
                        merchantExistCheck.organisationId = item.organisationId;
                        merchantExistCheck.terminalId = item.terminalId;
                        merchantExistCheck.terminalName = item.terminalName;
                        merchantExistCheck.terminalType = item.terminalType;
                        await UpdateAsync(merchantExistCheck, new { item.id });
                        var jposId = item.Jpos_TerminalId;
                        TerminalJposDto oTerminalJpos = new TerminalJposDto()
                        {
                            active = true,
                            info = item.terminalName,
                            softVersion = terminalTypeVal,
                            terminalId = item.terminalId
                        };
                        await _sharedJPOSService.PostRespectiveDataJPOS(JposTerminalURL, oTerminalJpos, jposId, clientIpAddress, JPOSAPIConstants.Terminals);
                    }
                    else
                    {
                        TerminalJposDto oTerminalJpos = new TerminalJposDto()
                        {
                            active = true,
                            info = item.terminalName,
                            softVersion = terminalTypeVal,
                            terminalId = item.terminalId
                        };
                        int resultAdd = await _sharedJPOSService.PostRespectiveDataJPOS(JposTerminalURL, oTerminalJpos, null, clientIpAddress, JPOSAPIConstants.Terminals);
                        var obj = new MerchantTerminal();
                        obj.id = item.id;
                        obj.organisationId = item.organisationId;
                        obj.terminalId = item.terminalId;
                        obj.terminalName = item.terminalName;
                        obj.terminalType = item.terminalType;
                        obj.Jpos_TerminalId = resultAdd.ToString();
                        await AddAsync(obj);
                    }
                }
                return isSucces;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
