using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class ProgramAccountDetailsWithMasterDto
    {
        public int id { get; set; }
        public string accountName { get; set; }
        public int accountTypeId { get; set; }
        public int? programId { get; set; }
        public int? passType { get; set; }
        public decimal? intialBalanceCount { get; set; }
        public int? resetPeriodType { get; set; }
        public int? resetDay { get; set; }
        public TimeSpan? resetTime { get; set; }
        public int? maxPassUsage { get; set; }
        public bool? isPassExchangeEnabled { get; set; }
        public int? exchangePassValue { get; set; }
        public int? exchangeResetPeriodType { get; set; }
        public int? exchangeResetDay { get; set; }
        public TimeSpan? exchangeResetTime { get; set; }
        public bool? isRollOver { get; set; }
        public DateTime? flexEndDate { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
	    public string ProgramAccountId { get; set; }
        public int? maxPassPerWeek { get; set; }
        public int? maxPassPerMonth { get; set; }
        public DateTime? resetDateOfMonth { get; set; }
        public int? flexMaxSpendPerDay { get; set; }
        public int? flexMaxSpendPerWeek { get; set; }
        public int? flexMaxSpendPerMonth { get; set; }
        public DateTime? exchangeResetDateOfMonth { get; set; }
        public decimal? vplMaxBalance { get; set; }
        public decimal? vplMaxAddValueAmount { get; set; }
        public int? vplMaxNumberOfTransaction { get; set; }
        public List<AccountTypeDto> AccountType { get; set; }
        public List<WeekDayDto> WeekDays { get; set; }
        public List<PassTypeDto> lstPassType { get; set; }
        public List<ResetPeriodDto> ResetPeriod { get; set; }
        public List<ExchangeResetPeriodDto> ExchangeResetPeriod { get; set; }
        public List<InitialBalanceDto> InitialBalance { get; set; }
        public string Jpos_ProgramAccountId { get; set; }
        public string Jpos_ProgramAccountEncId { get; set; }
    }
}
