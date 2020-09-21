using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class UserRewardsProgressLinkingService : FoundryRepositoryBase<UserRewardsProgressLinking>, IUserRewardsProgressLinkingService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        public UserRewardsProgressLinkingService(IDatabaseConnectionFactory databaseConnectionFactory)
       : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }
    }
}
