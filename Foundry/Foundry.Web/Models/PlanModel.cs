using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Foundry.Domain.ApiModel;
using Foundry.Domain.Dto;

namespace Foundry.Web.Models
{
    public class PlanModel
    {
        public string id { get; set; }
        [Required(ErrorMessage = "Please enter plan name.")]
        public string name { get; set; }
        public string programId { get; set; }
        public int? noOfMealPasses { get; set; }
        public int? noOfFlexPoints { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [RegularExpression(@"\d{1,2}/\d{1,2}/\d{4,4}", ErrorMessage = "Invalid date.")]
        [Required(ErrorMessage = "Please enter start date of plan.")]
        public DateTime? startDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [RegularExpression(@"\d{1,2}/\d{1,2}/\d{4,4}", ErrorMessage = "Invalid date.")]
        [Required(ErrorMessage = "Please enter end date of plan.")]
        public DateTime? endDate { get; set; }
        [Required(ErrorMessage = "Please enter start time.")]
        public TimeSpan? startTime { get; set; }
        [Required(ErrorMessage = "Please enter end time.")]
        public TimeSpan? endTime { get; set; }
        public string description { get; set; }
        [Required(ErrorMessage = "Please enter client id.")]
        public string clientId { get; set; }
        public string planId { get; set; }
        public bool? isActive { get; set; }
        public string Jpos_PlanId { get; set; }
        public string Jpos_PlanEncId { get; set; }
        public List<ProgramAccountDto> programAccount { get; set; }
        [Required(ErrorMessage = "Please select atleast one account.")]
        public IEnumerable<int> selectedProgramAccount { get; set; }
        public List<PlanProgramAccountModel> planProgramAccount { get; set; }
    }
}
