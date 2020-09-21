using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
   public class PaymentCardWebhookDto
    {
        public string authCard { get; set; }
        public CardDto card{ get; set; }
        public bool error { get; set; }
        public string gatewayRefId { get; set; }
     //   public bool zeroDollarAuth { get; set; }
     public zeroDollarAuthDto zeroDollarAuth { get; set; }

    }
    public class CardDto
    {
        public string bin { get; set; }
        public string brand { get; set; }
        public ExpiryDto exp { get; set; }
        public string last4 { get; set; }
        public string masked { get; set; }
        public string name { get; set; }
        public string token { get; set; }
    }
    public class ExpiryDto
    {
        public string month { get; set; }
        public string year { get; set; }
    }
    public class zeroDollarAuthDto
    {
      public string cvv2 { get; set; }
        public avsDto avs { get; set; }
    }
    public class avsDto
    {
    public string streetMatch { get; set; }
    public string postalCodeMatch { get; set; }
    }

}
