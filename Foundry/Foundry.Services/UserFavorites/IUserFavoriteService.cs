using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Services
{
    public interface IUserFavoriteService : IFoundryRepositoryBase<UserFavorites>
    {
        Task<int> AddUpdateUserFavorite(UserFavoriteModel model);
        Task<List<int>> GetUsersListForFavoriteMerchant(List<int> userIds, int merchantId);
    }
}