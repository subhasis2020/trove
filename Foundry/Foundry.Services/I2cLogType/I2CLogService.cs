using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class I2CLogService : FoundryRepositoryBase<I2CLog>, II2CLogService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public I2CLogService(IDatabaseConnectionFactory databaseConnectionFactory)
      : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }
    }
}
