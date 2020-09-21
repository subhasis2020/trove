using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Services
{
    public class ReloadRule : FoundryRepositoryBase<ReloadRules>, IReloadRule
    {
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public ReloadRule(IDatabaseConnectionFactory databaseConnectionFactory)
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
