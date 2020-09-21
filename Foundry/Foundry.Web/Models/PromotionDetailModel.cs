using Foundry.Domain.Dto;
using Foundry.Web.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class PromotionDetailModel
    {
        public int PromotionId { get; set; }
        public int PromotionTypeId { get; set; }
        public string MerchantId { get; set; }
        public IFormFile PostedFileUpload { get; set; }
        public string PromotionImagePath { get; set; }
        [Required(ErrorMessage = "Please enter description.")]
        public string PromotionDescription { get; set; }
        public string PromoDetail { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [RegularExpression(@"\d{1,2}/\d{1,2}/\d{4,4}", ErrorMessage = "Invalid date.")]
        [Required(ErrorMessage = "Please enter start date.")]
        public DateTime? StartDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [RegularExpression(@"\d{1,2}/\d{1,2}/\d{4,4}", ErrorMessage = "Invalid date.")]
        [Required(ErrorMessage = "Please enter end date.")]
        public DateTime? EndDate { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string RepeatDay { get; set; }
        public string BannerTypeId { get; set; }
        [Required(ErrorMessage = "Please enter banner description.")]
        public string BannerDescription { get; set; }
        public List<OfferCodeDto> OfferCodes { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsDaily { get; set; }
        public string encPromId { get; set; }
        public string ImageFileName { get; set; }
    }
}
