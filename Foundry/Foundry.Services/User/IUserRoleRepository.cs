using Foundry.Domain.DbModel;
using System.Threading.Tasks;

namespace Foundry.Services
{
    
    public interface IUserRoleRepository : IFoundryRepositoryBase<UserRole>
    {
        Task<int> AddUserRole(UserRole userRorle);
    }
}
