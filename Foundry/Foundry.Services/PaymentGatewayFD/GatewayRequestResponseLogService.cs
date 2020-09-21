using Foundry.Domain.DbModel;
using Foundry.LogService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class GatewayRequestResponseLogService : FoundryRepositoryBase<GatewayRequestResponseLog>, IGatewayRequestResponseLogService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private ILoggerManager _logger;


        public GatewayRequestResponseLogService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, ILoggerManager logger)
                   : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _logger = logger;
        }
    }
}
