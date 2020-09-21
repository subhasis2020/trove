using Foundry.Domain.DbModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class JPOSCallLogService : FoundryRepositoryBase<JPOSCallLog>, IJPOSCallLogService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IGeneralSettingService _generalRepository;

        public JPOSCallLogService(IDatabaseConnectionFactory databaseConnectionFactory, IConfiguration configuration, IGeneralSettingService generalRepository) : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            _configuration = configuration;
            _generalRepository = generalRepository;
        }
        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
