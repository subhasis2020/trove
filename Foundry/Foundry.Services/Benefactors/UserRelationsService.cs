using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class UserRelationsService : FoundryRepositoryBase<UserRelations>, IUserRelationsService
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
        private readonly IBenefactorsProgram _benefactorprogram;

        public UserRelationsService(IDatabaseConnectionFactory databaseConnectionFactory
            , IBenefactorsProgram benefactorsProgram)
        : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));

            _benefactorprogram = benefactorsProgram;
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}
