using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Web.Models
{
    public class AccountMerchantRuleModel
    {
        public string programAccountId { get; set; }
        public string accountTypeId { get; set; }
        public List<BusinessTypeDto> businessTypes { get; set; }
        public List<int> selectedMerchant { get; set; }
        public List<AccountMerchantRuleMerchantDto> Merchants { get; set; }
        public List<AccountMerchantRuleDetailsDto> accountMerchantRuleDetails { get; set; }
        public List<AccountMerchantRuleAndDetailModel> AccountMerchantRuleAndDetail { get; set; }
        public string programId { get; set; }
        public List<int> selectedBusinessType { get; set; }
    }
    public class AccountMerchantRuleAndDetailModel
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
