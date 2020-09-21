using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class TransactionViewDto
    {
        public int Id { get; set; }
        public string Amount { get; set; }
        public string AccountType { get; set; }
        public string MerchantName { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
