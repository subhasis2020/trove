using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class FiservMainTransactionModel
    {
        public string clientRequestId { get; set; }
        public string apiTraceId { get; set; }
        public string ipgTransactionId { get; set; }
        public string orderId { get; set; }
        public string transactionType { get; set; }
        public string transactionOrigin { get; set; }
        public PaymentMethodFiservTransactionModel paymentMethodDetails { get; set; }
        public string country { get; set; }
        public string terminalId { get; set; }
        public string merchantId { get; set; }
        public int transactionTime { get; set; }
        public ApprovedAmountTransactioModel approvedAmount { get; set; }
        public string transactionStatus { get; set; }
        public string schemeTransactionId { get; set; }
        public ProcessorTransactionModel processor { get; set; }
        //Added explicitly for credit user id and it is not coming from payment transaction response
        public int CreditUserId { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public int ProgramAccountIdSelected { get; set; }
        public string TransactionPaymentGateway { get; set; }

        public int? PlanId { get; set; } = 0;
        public int? ProgramId { get; set; } = 0;
        public int? OrganizationId { get; set; } = 0;
        public int? AccountTypeId { get; set; } = 0;
        public string schemetransactionID { get; set; }

    }

    public class PaymentMethodFiservTransactionModel
    {
        public PaymentFiservCardTransactionModel paymentCard { get; set; }
        public string paymentMethodType { get; set; }
    }
    public class PaymentFiservCardTransactionModel
    {
        public FiservTransactionExpiryDateModel expiryDate { get; set; }
        public string cardFunction { get; set; }
        public string bin { get; set; }
        public string last4 { get; set; }
        public string brand { get; set; }

    }

    public class FiservTransactionExpiryDateModel
    {
        public string month { get; set; }
        public string year { get; set; }
    }

    public class ApprovedAmountTransactioModel
    {
        public decimal total { get; set; }
        public string currency { get; set; }
        public ComponentsTransactionModel components { get; set; }
    }
    public class ComponentsTransactionModel
    {
        public string subtotal { get; set; }
    }
    public class ProcessorTransactionModel
    {
        public string referenceNumber { get; set; }
        public string authorizationCode { get; set; }
        public string responseCode { get; set; }
        public string network { get; set; }
        public string associationResponseCode { get; set; }
        public string responseMessage { get; set; }
        public AVSResponseTransactionModel avsResponse { get; set; }
        public string securityCodeResponse { get; set; }

    }
    public class AVSResponseTransactionModel
    {
        public string streetMatch { get; set; }
        public string postalCodeMatch { get; set; }

    }
}
