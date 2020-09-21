using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
  public  class SiteLevelOverrideSettingApiViewModel
    {
        public int id { get; set; }
        public int programId { get; set; }
       
        public decimal? siteLevelBitePayRatio { get; set; }
     
        public decimal? siteLevelDcbFlexRatio { get; set; }
       
        public decimal? siteLevelUserStatusVipRatio { get; set; }
      
        public decimal? siteLevelUserStatusRegularRatio { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public decimal? FirstTransactionBonus { get; set; }
    }
}
