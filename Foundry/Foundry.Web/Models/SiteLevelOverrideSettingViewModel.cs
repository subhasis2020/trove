using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class SiteLevelOverrideSettingViewModel
    {
        public int id { get; set; }
        public int programId { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? siteLevelBitePayRatio { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? siteLevelDcbFlexRatio { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? siteLevelUserStatusVipRatio { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? siteLevelUserStatusRegularRatio { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal? FirstTransactionBonus { get; set; }
    }
}
