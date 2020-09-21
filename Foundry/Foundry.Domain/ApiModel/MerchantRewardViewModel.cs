using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Domain.ApiModel
{
    public class MerchantRewardViewModel
    {
        public int id { get; set; }
        public int? offerTypeId { get; set; }
        public int? offerSubTypeId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int? bannerTypeId { get; set; }
        public string bannerDescription { get; set; }
        public DateTime? startDate { get; set; }
        public TimeSpan? startTime { get; set; }
        public DateTime? endDate { get; set; }
        public TimeSpan? endTime { get; set; }
        public string promotionDay { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public int? MerchantId { get; set; }
        public int? noOfVisits { get; set; }
        public decimal? amounts { get; set; }
        public int? businessTypeId { get; set; }
        public string backgroundColor { get; set; }
        public string firstGradiantColor { get; set; }
        public bool IsPromotion { get; set; }
        public bool IsDaily { get; set; }
        public string ImagePath { get; set; }
        public bool IsPublished { get; set; }
    }
}
