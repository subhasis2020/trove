using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class ProgramAccountDto
    {
        public string strId { get; set; }
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
    }
}
