using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class ProgramAccountModel
    {
        public string id { get; set; }
        [Required(ErrorMessage = "Please enter account name.")]
        public string accountName { get; set; }
        [Required(ErrorMessage = "Please select account type.")]
        public int accountTypeId { get; set; }
        public string programId { get; set; }
        [Required(ErrorMessage = "Please select pass type.")]
        public int? passType { get; set; }
        [Required(ErrorMessage = "Please enter initial balance.")]
        [Display(Name = "initial balance")]
        public decimal? intialBalanceCount { get; set; }
        [Required(ErrorMessage = "Please select reset period.")]
        public int? resetPeriodType { get; set; }
        [Required(ErrorMessage = "Please select reset day.")]
        public int? resetDay { get; set; }
        [Required(ErrorMessage = "Please enter reset time.")]
        public TimeSpan? resetTime { get; set; }
        [Required(ErrorMessage = "Please enter max pass per day.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The field max pass per day must be a number.")]
        public int? maxPassUsage { get; set; }
        public bool? isPassExchangeEnabled { get; set; }
        [Required(ErrorMessage = "Please enter how many exchanges.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The field how many exchanges must be a number.")]
        public int? exchangePassValue { get; set; }
        [Required(ErrorMessage = "Please select reset period.")]
        public int? exchangeResetPeriodType { get; set; }
        [Required(ErrorMessage = "Please select reset day.")]
        public int? exchangeResetDay { get; set; }
        [Required(ErrorMessage = "Please enter reset time.")]
        public TimeSpan? exchangeResetTime { get; set; }
        public bool? isRollOver { get; set; }
        [Required(ErrorMessage = "Please enter account name.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [RegularExpression(@"\d{1,2}/\d{1,2}/\d{4,4}", ErrorMessage = "Invalid date.")]
        public DateTime? flexEndDate { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public string ProgramAccountId { get; set; }
        [Required(ErrorMessage = "Please enter max pass per week.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The field max pass per week must be a number.")]
        public int? maxPassPerWeek { get; set; }
        [Required(ErrorMessage = "Please enter max pass per month.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The field max pass per month must be a number.")]
        public int? maxPassPerMonth { get; set; }
        [Required(ErrorMessage = "Please enter reset date of month.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [RegularExpression(@"\d{1,2}/\d{1,2}/\d{4,4}", ErrorMessage = "Invalid date.")]
        public DateTime? resetDateOfMonth { get; set; }
        [Required(ErrorMessage = "Please enter max spend per day.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The field max spend per day must be a number.")]
        public int? flexMaxSpendPerDay { get; set; }
        [Required(ErrorMessage = "Please enter max spend per week.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The field max spend per week must be a number.")]
        public int? flexMaxSpendPerWeek { get; set; }
        [Required(ErrorMessage = "Please enter max spend per month.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The field max spend per month must be a number.")]
        public int? flexMaxSpendPerMonth { get; set; }
        [Required(ErrorMessage = "Please enter reset date of month.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [RegularExpression(@"\d{1,2}/\d{1,2}/\d{4,4}", ErrorMessage = "Invalid date.")]
        public DateTime? exchangeResetDateOfMonth { get; set; }
        [Required(ErrorMessage = "Please enter max balance.")]
        [Display(Name = "max balance")]
        public decimal? vplMaxBalance { get; set; }
        [Required(ErrorMessage = "Please enter max add value amount.")]
        [Display(Name = "max add value amount")]
        public decimal? vplMaxAddValueAmount { get; set; }
        [Required(ErrorMessage = "Please enter max number of add value transaction per day.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The field max number of add value transaction per day must be a number.")]
        public int? vplMaxNumberOfTransaction { get; set; }
        public List<AccountTypeDto> accountType { get; set; }
        public List<PassTypeDto> lstPassType { get; set; }
        public List<ResetPeriodDto> ResetPeriod { get; set; }
        public List<ExchangeResetPeriodDto> ExchangeResetPeriod { get; set; }
        public List<WeekDayDto> WeekDays { get; set; }
        public string Jpos_ProgramAccountId { get; set; }
        public string Jpos_ProgramAccountEncId { get; set; }
    }
}
