using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class MerchantRewardModel
    {
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public string PrimaryOrganisationName { get; set; }
        public string PrimaryProgramName { get; set; }
        [Required(ErrorMessage = "Please enter reward title.")]
        public string RewardTitle { get; set; }
        [Required(ErrorMessage = "Please enter reward sub title.")]
        public string RewardSubTitle { get; set; }
        [Required(ErrorMessage = "Please enter description.")]
        public string Description { get; set; }
        public int OfferTypeId { get; set; }
        public int OfferSubTypeId { get; set; }
        [Required(ErrorMessage = "Please enter the amount.")]
        [RegularExpression(@"^(\d*\.)?\d+$", ErrorMessage = "The amount must be a number.")]
        public decimal? Amount { get; set; }
        [Required(ErrorMessage = "Please enter number of visits.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter valid number of visits.")]
        public int? Visits { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [RegularExpression(@"\d{1,2}/\d{1,2}/\d{4,4}", ErrorMessage = "Invalid date.")]
        public DateTime? StartDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [RegularExpression(@"\d{1,2}/\d{1,2}/\d{4,4}", ErrorMessage = "Invalid date.")]
        public DateTime? EndDate { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string BackGroundColor { get; set; }
        [Required(ErrorMessage = "Please choose an icon to display on coupon.")]
        public int BusinessTypeId { get; set; }
        public string BusinessIconPath { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublished { get; set; }
        public List<BusinessTypeDto> BusinessType { get; set; }
        public List<OfferSubTypeDto> OfferSubType { get; set; }
    }
}
