using Foundry.Domain.DbModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IRoleRepository : IFoundryRepositoryBase<Role>
    {
        Task<IEnumerable<Role>> GetRolesByRoleType(int roleType);
    }
}
