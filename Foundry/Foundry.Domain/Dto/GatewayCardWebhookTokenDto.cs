using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{

    public class GatewayCardWebhookTokenDto
    {
        public int id { get; set; }
        public int creditUserId { get; set; }
        public int debitUserId { get; set; }
        public string Token { get; set; }
        public string maskedLastDigitCard { get; set; }
        public string nameOnCard { get; set; }
        public string expiryMonthYear { get; set; }
        public string cardBrand { get; set; }
        public string last4digits { get; set; }
        public DateTime? TokenReceivedDate { get; set; }
        public string Nonce { get; set; }
        public string ClientToken { get; set; }
        public string ipgFirstTransactionId { get; set; }
        public string nickName { get; set; }
        public string schemetransactionID { get; set; }
    }

}
