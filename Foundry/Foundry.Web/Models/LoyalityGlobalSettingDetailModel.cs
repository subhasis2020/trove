using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class LoyalityGlobalSettingDetailModel
    {
        public int id { get; set; }
        public int organisationId { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? loyalityThreshhold { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? globalReward { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? globalRatePoints { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? bitePayRatio { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? dcbFlexRatio { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? userStatusVipRatio { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? userStatusRegularRatio { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        [Required(ErrorMessage = "*")]
        public  decimal? FirstTransactionBonus { get; set; }

    }
}
