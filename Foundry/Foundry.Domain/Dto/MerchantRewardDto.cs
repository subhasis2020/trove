using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Domain.Dto
{
    public class MerchantRewardDto
    {
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public string PrimaryOrganisationName { get; set; }
        public string PrimaryProgramName { get; set; }
        public string RewardTitle { get; set; }
        public string RewardSubTitle { get; set; }
        public string Description { get; set; }
        public int OfferTypeId { get; set; }
        public int OfferSubTypeId { get; set; }
        public decimal Amount { get; set; }
        public int Visits { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string BackGroundColor { get; set; }
        public int BusinessTypeId { get; set; }
        public bool IsActive { get; set; }
        public string BusinessIconPath { get; set; }
        public bool IsPublished { get; set; }
        public List<BusinessTypeDto> BusinessType { get; set; }
        public List<OfferSubTypeDto> OfferSubType { get; set; }
    }

    public class OfferSubTypeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
