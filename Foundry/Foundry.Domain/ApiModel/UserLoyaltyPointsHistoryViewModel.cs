using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
  public  class UserLoyaltyPointsHistoryViewModel
    {
        public int id { get; set; }
        public int? userId { get; set; }
        public string transactionId { get; set; }
        public decimal? transactionAmount { get; set; }
        public decimal? pointsEarned { get; set; }
        public decimal? totalPoints { get; set; }
        public decimal? rewardAmount { get; set; }
        public decimal? leftOverPoints { get; set; }
        public bool? isThresholdReached { get; set; }
        public DateTime? transactionDate { get; set; }
        public DateTime? createdDate { get; set; }
    }
}
