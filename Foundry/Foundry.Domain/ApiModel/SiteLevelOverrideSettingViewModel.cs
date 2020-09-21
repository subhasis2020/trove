using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
  public  class SiteLevelOverrideSettingViewModel
    {
        public int id { get; set; }
        public int programId { get; set; }
        public int? siteLevelBitePayRatio { get; set; }
        public int? siteLevelDcbFlexRatio { get; set; }
        public int? siteLevelUserStatusVipRatio { get; set; }
        public int? siteLevelUserStatusRegularRatio { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
    }
}
