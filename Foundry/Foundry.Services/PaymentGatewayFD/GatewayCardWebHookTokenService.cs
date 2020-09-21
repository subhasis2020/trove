using Dapper;
using Foundry.Domain;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;
using Foundry.LogService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class GatewayCardWebHookTokenService : FoundryRepositoryBase<GatewayCardWebHookToken>, IGatewayCardWebHookTokenService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private ILoggerManager _logger;


        public GatewayCardWebHookTokenService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, ILoggerManager logger)
                   : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<GatewayCardWebhookTokenDto> GetLatestWebhookToken(int creditUserId, int debitUserId)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        CreditUserId = creditUserId,
                        DebitUserId = debitUserId
                    };
                    return (await sqlConnection.QuerySingleOrDefaultAsync<GatewayCardWebhookTokenDto>(SQLQueryConstants.GetLatestWebToken, obj));
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

        public async Task<GatewayCardWebhookTokenDto> GetLatestWebhookTokenByClientToken(string clientToken)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                         clientToken,
                    };
                    return (await sqlConnection.QuerySingleOrDefaultAsync<GatewayCardWebhookTokenDto>(SQLQueryConstants.GetLatestWebTokenByClientToken, obj));
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

        public async Task<BinDataDto> CheckBinNumberIsValid(long bin)
        {
            using (var sqlConnection = await _databaseConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var obj = new
                    {
                        bin,
                    };
                    return (await sqlConnection.QuerySingleOrDefaultAsync<BinDataDto>(SQLQueryConstants.GetBinNumberIsValid, obj));
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
