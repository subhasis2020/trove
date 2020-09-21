using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class AccountMerchantRuleViewModel
    {
        public int programAccountId { get; set; }
        public string accountTypeId { get; set; }
        public List<AccountMerchantRuleMerchantDto> selectedMerchant { get; set; }
        public List<AccountMerchantRuleDetailsDto> accountMerchantRuleDetails { get; set; }
        public List<AccountMerchantRuleAndDetailViewModel> accountMerchantRuleAndDetails { get; set; }
        public int programId { get; set; }
    }
    public class AccountMerchantRuleAndDetailViewModel
    {
        public int id { get; set; }
        public string merchantName { get; set; }
        public int mealPeriodId { get; set; }
        public string mealPeriod { get; set; }
        public decimal maxPassUsage { get; set; }
        public decimal? minPassValue { get; set; }
        public decimal? maxPassValue { get; set; }
        public decimal? transactionLimit { get; set; }
    }
}
