using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class OrgLoyalityGlobalSettingsDto
    {
        public  int id { get; set; }      
        public  int organisationId { get; set; }    
        public decimal? loyalityThreshhold { get; set; }      
        public decimal? globalReward { get; set; }     
        public decimal? globalRatePoints { get; set; }       
        public decimal? bitePayRatio { get; set; }
        public decimal? dcbFlexRatio { get; set; }    
        public decimal? userStatusVipRatio { get; set; }     
        public decimal? userStatusRegularRatio { get; set; }      
        public  DateTime? createdDate { get; set; }
        public  DateTime? modifiedDate { get; set; }
        public decimal? FirstTransactionBonus { get; set; }
    }
}
