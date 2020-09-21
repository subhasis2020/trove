using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class FiservPaymentTransactionLogService : FoundryRepositoryBase<FiservPaymentTransactionLog>, IFiservPaymentTransactionLogService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
      

        public FiservPaymentTransactionLogService(IDatabaseConnectionFactory databaseConnectionFactory)
                   : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
           
        }
    }
}
