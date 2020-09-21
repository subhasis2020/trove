using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Foundry.Domain.ApiModel;
using Foundry.Domain.DbModel;
using Foundry.Domain.Dto;

namespace Foundry.Services
{
    public interface IPromotions : IFoundryRepositoryBase<Promotion>
    {
        Task<int> AddEditPromotion(MerchantRewardViewModel model);
        Task<int> EditPromotionStatus(int id, bool status);
        Task<IEnumerable<PromotionsDto>> GetAllPromotionsOfMerchant(int merchantId);
        Task<PromotionsDto> GetPromotionDetailById(int promotionId);
        Task<List<RewardsDto>> GetAchievedRewardDetailInformation(int userId, int programId, string baseURL);
        Task<List<RewardsDto>> GetAllRewardsInformation(int userId, int programId,string baseURL);
        Task<List<RewardsOnDate>> GetUserRewardsBasedOnCurrentDate();
        Task<List<RewardsOnDate>> GetUserOffersBasedOnCurrentDate();
        Task<List<RewardsOnDate>> GetUsersToAheadCompleteRewards();
        Task<List<RewardsOnDate>> GetUsersCompletedRewardsNotify();
    }
}
