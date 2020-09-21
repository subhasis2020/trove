using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Web.Models
{
    public class MerchnatTransactionModel
    {
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
    }
}
