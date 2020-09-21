using AutoMapper;
using Dapper;
using Foundry.Domain;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.LogService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class PlanProgramAccountLinkingService : FoundryRepositoryBase<PlanProgramAccountsLinking>, IPlanProgramAccountLinkingService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;
        public PlanProgramAccountLinkingService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, ILoggerManager logger,
            IMapper mapper)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public async Task<string> AddUpdatePlanProgramAccount(List<PlanProgramAccountsLinking> model)
        {
            var deletedContent = await DeleteEntityAsync(new { model.FirstOrDefault().planId });

            await AddAsync(model);
            return Cryptography.EncryptPlainToCipher(model.FirstOrDefault().planId.ToString());
        }

        public async Task<List<ProgramAccountDetailDto>> GetProgramAccountsDetailsByPlanIds(List<int> planIds)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var ppal = await sqlConnection.QueryAsync<ProgramAccountDetailDto>(SQLQueryConstants.GetProgramAccountsDetailsByPlanIds, new { PlanId = planIds });
                    return ppal.ToList();
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
