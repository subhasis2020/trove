using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public class RoleRepository : FoundryRepositoryBase<Role>, IRoleRepository
    {

        /*Creating database connection*/
        private readonly IDatabaseConnectionFactory _databaseConnectionFactory;

        public RoleRepository(IDatabaseConnectionFactory databaseConnectionFactory)
            : base(databaseConnectionFactory)
        {
            _databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public new void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<Role>> GetRolesByRoleType(int roleType)
        {
            return await GetDataAsync(new { RoleType = roleType });

        }
    }
}
