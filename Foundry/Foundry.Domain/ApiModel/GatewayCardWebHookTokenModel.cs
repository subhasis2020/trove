using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class GatewayCardWebhookNLogModel {
        public GatewayCardWebHookTokenModel GatewayCardWebHookTokenModel { get; set; }
        public GatewayRequestResponseLogModel GatewayRequestResponseLogModel { get; set; }
    }
    public class GatewayCardWebHookTokenModel
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
        public bool IsCardToSave { get; set; }
        public string ipgFirstTransactionId { get; set; }
        public string nickName { get; set; }
        public  long Bin { get; set; }
        
        public  bool? IsCardValid { get; set; }
        public string schemetransactionID { get; set; }


    }

    public class GatewayRequestResponseLogModel
    {
        public int id { get; set; }
        public int debitUserId { get; set; }
        public int creditUserId { get; set; }
        public string WebhookResponse { get; set; }
        public DateTime? webhookReceivedDate { get; set; }
        public string Nonce { get; set; }
        public string ClientToken { get; set; }
    }

    public partial class FiservPaymentTransactionLogModel
    {
        public int id { get; set; }
        public int debitUserId { get; set; }
        public int creditUserId { get; set; }
        public DateTime? FiservRequestDate { get; set; }
        public DateTime? FiservResponseDate { get; set; }
        public string FiservRequestContent { get; set; }
        public string FiservResponseContent { get; set; }
    }
}
