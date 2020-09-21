using Foundry.Domain.DbModel;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IUsersGroup : IFoundryRepositoryBase<UserGroup>
    {
        Task<Group> GetGroupIdByName(string name);
        Task<int> AddUpdateUserGroup(int userId, int groupId);
    }
}
