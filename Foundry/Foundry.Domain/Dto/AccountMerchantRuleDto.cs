using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class AccountMerchantRuleDto
    {
        public IEnumerable<BusinessTypeDto> BusinessTypes { get; set; }
        public IEnumerable<AccountMerchantRuleMerchantDto> Merchants { get; set; }
        public IEnumerable<AccountMerchantRuleAndDetailDto> AccountMerchantRuleAndDetail { get; set; }
    }

    public class AccountMerchantRuleMerchantDto
    {
        public string Id { get; set; }
        public string MerchantName { get; set; }
        public bool IsSelected { get; set; }
    }
    public class AccountMerchantRuleAndDetailDto
    {
        public int id { get; set; }
        public int accountTypeId { get; set; }
        public string merchantName { get; set; }
        public int mealPeriodId { get; set; }
        public string mealPeriod { get; set; }
        public decimal maxPassUsage { get; set; }
        public decimal? minPassValue { get; set; }
        public decimal? maxPassValue { get; set; }
        public decimal? transactionLimit { get; set; }
    }
}
