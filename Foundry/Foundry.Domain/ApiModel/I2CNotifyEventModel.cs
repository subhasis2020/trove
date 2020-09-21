using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class I2CNotifyEventModel
    {
        public Header Header { get; set; }
        public Transaction Transaction { get; set; }
        public Card Card { get; set; }

    }
    public class HealthCheckmModel
    {
        public Header Header { get; set; }

        public long HealthCheckId { get; set; }
    }


    public class Header
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string MessageCreationDateTime { get; set; }

    }

    public class CardAcceptor
    {
        public string AcquirerId { get; set; }
        public string MerchantCode { get; set; }
        public string NameAndLocation { get; set; }
        public string MerchantCity { get; set; }
        public string MerchantState { get; set; }
        public string MerchantZipCode { get; set; }
        public string MCC { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string LocalDateTime { get; set; }

    }

    public class Transaction
    {
        public string NotificationEventId { get; set; }
        public string TransactionId { get; set; }
        public string MessageType { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public CardAcceptor CardAcceptor { get; set; }
        public string TransactionType { get; set; }
        public string Service { get; set; }
        public string TransactionAmount { get; set; }
        public string TransactionCurrency { get; set; }
        public string AuthorizationCode { get; set; }
        public string RetrievalReferenceNo { get; set; }
        public string TransferID { get; set; }
        public string BankAccountNumber { get; set; }
        public string TransactionDescription { get; set; }
        public string ExternalTransReference { get; set; }

    }

    public class Card
    {
        public string CardProgramID { get; set; }
        public string CardReferenceID { get; set; }
        public string PrimaryCardReferenceID { get; set; }
        public string CustomerId { get; set; }
        public string AvailableBalance { get; set; }
        public string CardStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string CellNo { get; set; }

    }

}
