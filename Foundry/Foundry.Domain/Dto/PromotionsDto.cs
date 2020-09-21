using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class PromotionsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BannerTypeId { get; set; }
        public int MerchantId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PromotionDay { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string BannerDescription { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDailyPromotion { get; set; }
        public int? RepeatDailyDay { get; set; }
        public string ImagePath { get; set; }
        public string encPromId { get; set; }
        public string ImageFileName { get; set; }
    }

    public class RewardsDto
    {
        public int RewardProgressId { get; set; }
        public int RewardId { get; set; }
        public string RewardTitle { get; set; }
        public string RewardSubTitle { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BusinessTypeId { get; set; }
        public string IconPath { get; set; }
        public int MerchantId { get; set; }
        public float rewardProgressAchieved { get; set; }
        public float TotalRewardPointsToAchieve { get; set; }
        public int OfferSubTypeId { get; set; }
        public string RewardBackColor { get; set; }
        public string MerchantName { get; set; }
        public string LeftAchievementProgressLine { get; set; }
    }

    public class RewardsOnDate
    {
        public int RewardId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BusinessTypeId { get; set; }
        public int OfferSubTypeId { get; set; }
        public int MerchantId { get; set; }
        public int ProgramId { get; set; }
        public int CreatedBy { get; set; }
        public string MerchantName { get; set; }
    }

    public class RewardsSatatusDto
    {
        public string RewardId { get; set; }
        public bool rewardActiveStatus { get; set; }
    }
}
