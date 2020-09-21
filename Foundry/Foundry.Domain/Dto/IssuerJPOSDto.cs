using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class IssuerJPOSDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string programId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string organizationId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool active { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string tz { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime startDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime endDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string programType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string city { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string state { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zip { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string country { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string website { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string contactName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string contactNumber { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string contactEmail { get; set; }
    }

    public class AccountHolderJposDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string realId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string issuer { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool active { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string gender { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string city { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string state { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zip { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string country { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? birthDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string firstName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string lastName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string lastName2 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string phone { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime startDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime endDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string email { get; set; }
    }
    public class MerchantJposDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string merchantId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string subclass { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string parent { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string acquirer { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool active { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string contactName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address2 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string city { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string state { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zip { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string country { get; set; }
    }

    public class TerminalJposDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string terminalId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool active { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string info { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string softVersion { get; set; }
    }

    public class PlanJposDto
    {
        public string clientId { get; set; }
        public string name { get; set; }
        public string issuer { get; set; }
        public bool active { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string description { get; set; }
        public string accounts { get; set; }
    }

    public class AccountsJposDto
    {
        public string name { get; set; }
        public string issuer { get; set; }
        public string layer { get; set; }
        public bool active { get; set; }
        public double initialBalance { get; set; }
        public double dailyLimit { get; set; }
        public double weeklyLimit { get; set; }
        public double monthlyLimit { get; set; }
        public string roleOver { get; set; }
        public string description { get; set; }
    }

    public class jposCardCollection
    {

        public string cardHolder { get; set; }
        public string cardProduct { get; set; }
        public string bin { get; set; }
        public string token { get; set; }

    }



    public class Header
    {

    }

    public class CardAcceptor
    {
        public string MerchantCode { get; set; }
        public string MCC { get; set; }
        public string DeviceType { get; set; }
        public string LocalDateTime { get; set; }

    }

    public class Transaction
    {
        public string TransactionId { get; set; }
        public string MessageType { get; set; }
        public string TransactionType { get; set; }
        public string Date { get; set; }
        public string TransactionAmount { get; set; }
        public string AuthorizationCode { get; set; }
        public string RetrievalReferenceNo { get; set; }
        public string TransactionDescription { get; set; }
        public CardAcceptor CardAcceptor { get; set; }

    }

    public class Card
    {
        public string CardReferenceID { get; set; }
        public string AvailableBalance { get; set; }

    }

    public class TranLogDto
    {
        public Header Header { get; set; }
        public Transaction Transaction { get; set; }
        public Card Card { get; set; }

    }



}
