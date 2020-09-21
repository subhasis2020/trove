using Foundry.Domain.DbModel;
using System;

namespace Foundry.Services
{
    public class I2CBank2CardTransferService : FoundryRepositoryBase<i2cBank2CardTransfer>, II2CBank2CardTransferService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public I2CBank2CardTransferService(IDatabaseConnectionFactory databaseConnectionFactory)
      : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }
    }
}
