using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Domain.ApiModel
{
    public class PgGatewayModel
    {
        public string gateway { get; set; }
        public string apiKey { get; set; }
        public string apiSecret { get; set; }
        public string storeId { get; set; }
        public bool zeroDollarAuth { get; set; }

    }

    public class PGResponseTokenization
    {
        public string clientToken { get; set; }
        public string publicKeyBase64 { get; set; }
    }

    public class PayloadPaymentFirst
    {
        public TransactionAmount transactionAmount { get; set; }
        public string requestType { get; set; }
        public string storeId { get; set; }
        public PaymentMenthod paymentMethod { get; set; }
        public StoredCredentials storedCredentials { get; set; }


    }

    public class TransactionAmount
    {
        public decimal total { get; set; }
        public string currency { get; set; }

    }
    public class PaymentMenthod
    {
        public PaymentToken paymentToken { get; set; }
    }

    public class PaymentToken
    {
        public string value { get; set; }
        public string function { get; set; }
        public string securityCode { get; set; }
        public ExpiryDate expiryDate { get; set; }
    }
    public class ExpiryDate
    {
        public string month { get; set; }
        public string year { get; set; }
    }
    public class StoredCredentials
    {
        public string sequence { get; set; }
        public bool scheduled { get; set; }
        public string referencedSchemeTransactionId { get; set; }

    }
    //public class Referencetransactionid
    //{
    //    public string referencetransactionid { get; set; }
    //}
    public class Data
    {
        public string reloadUserId { get; set; }
    }

    public class PayloadSecondaryTransaction {
        public TransactionAmount transactionAmount { get; set; }
        public string requestType { get; set; }
    }
}
