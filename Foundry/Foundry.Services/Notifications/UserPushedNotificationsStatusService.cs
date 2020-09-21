using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class UserPushedNotificationsStatusService : FoundryRepositoryBase<UserPushedNotificationsStatus>, IUserPushedNotificationsStatusService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public UserPushedNotificationsStatusService(IDatabaseConnectionFactory databaseConnectionFactory)
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
