
using System;

namespace Foundry.Domain.Dto
{
    public class MerchantTransactionDto
    {
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }

    }
}
