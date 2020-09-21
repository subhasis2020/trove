using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class UserClaimRepository : FoundryRepositoryBase<UserClaim>, IUserClaimRepository
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public UserClaimRepository(IDatabaseConnectionFactory databaseConnectionFactory)
           : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
