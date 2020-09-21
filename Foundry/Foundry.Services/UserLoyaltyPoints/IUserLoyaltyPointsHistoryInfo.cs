using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Services
{
  public interface IUserLoyaltyPointsHistoryInfo
    {
        Task<int> AddUserLoyaltyPointsHistory(UserLoyaltyPointsHistoryViewModel model);
        Task<IEnumerable<UserLoyaltyPointsHistoryDto>> GetUserLoyaltyPointsHistory(int id);
        Task<IEnumerable<UserLoyaltyPointsHistoryDto>> GetUserLoyaltyTrackingHistory1(int id);
        Task<IEnumerable<UserLoyaltyPointsHistoryDto>> GetUserLoyaltyTrackingHistory(int id, int pageNumber, int pageSize, string sortColumnName, string sortOrderDirection);
    }
}
