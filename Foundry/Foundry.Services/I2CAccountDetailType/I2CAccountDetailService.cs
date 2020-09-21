using Foundry.Domain.DbModel;
using System;

namespace Foundry.Services
{
    public class I2CAccountDetailService : FoundryRepositoryBase<I2CAccountDetail>, II2CAccountDetailService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public I2CAccountDetailService(IDatabaseConnectionFactory databaseConnectionFactory)
      : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

    }
}
