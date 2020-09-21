using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class AccountMerchantRuleDetailsDto
    {
        public int id { get; set; }
        public int accountMerchantRuleId { get; set; }
        public int mealPeriod { get; set; }
        public decimal maxPassUsage { get; set; }
        public decimal? minPassValue { get; set; }
        public decimal? maxPassValue { get; set; }
        public decimal? transactionLimit { get; set; }
    }
}
