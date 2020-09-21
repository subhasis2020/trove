using System.Collections.Generic;

namespace Foundry.Domain.ApiModel
{

    public class i2cBankAccountListModel
    {
        public string ResponseCode { get; set; }
        public string ResponseDesc { get; set; }
        public string CustomerId { get; set; }
        public List<Accounts> Accounts { get; set; }
        public string NoOfRecords { get; set; }
        public string NoOfRecordsSpecified { get; set; }
        public string Balance { get; set; }
        public string TransId { get; set; }
        public string Fee { get; set; }
    }

    public class Accounts
    {
        public string AccountSrNo { get; set; }
        public string AccountNumber { get; set; }
        public string AccountTitle { get; set; }
        public string AccountType { get; set; }
        public string AccountTypeSpecified { get; set; }
        public string AccountNickname { get; set; }
        public string ACHType { get; set; }
        public string ACHTypeSpecified { get; set; }
        public string BankName { get; set; }
        public string RoutingNumber { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string FailedRegistrationTries { get; set; }
        public string CreatedAt { get; set; }
        public string RegistrationExpiresAt { get; set; }
        public string EditAllowed { get; set; }
        public string EditAllowedSpecified { get; set; }
        public string DeleteAllowed { get; set; }
        public string DeleteAllowedSpecified { get; set; }
        public string VerificationAllowed { get; set; }
        public string VerificationAllowedSpecified { get; set; }
        public string TrialACHDate { get; set; }
        public string TrialACHReturnDate { get; set; }
        public string TrialACHReturnCode { get; set; }
        public string TrialACHNOCCode { get; set; }
        public string TrialACHNOCData { get; set; }
        public string AccountVerifiedOn { get; set; }
    }
}
