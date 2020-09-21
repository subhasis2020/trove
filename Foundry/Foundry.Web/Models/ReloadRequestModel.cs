using Foundry.Web.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Foundry.Web.Models
{
    public class ReloadRequestModel
    {
        public int ReloadRequestId { get; set; }
        public int ReloadUserId { get; set; }
        [Required(ErrorMessage = "*Reload Amount is required.")]
        public decimal? ReloadAmount { get; set; }
        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string SecurityCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public bool IsAutoReload { get; set; }
        public int ProgramId { get; set; }
        public int AccountTypeId { get; set; }
        public int BenefactorUserId { get; set; }
        public decimal? AutoReloadAmount { get; set; }
        public decimal? CheckDroppedAmount { get; set; }
        public string AccountReloadSrNo { get; set; }
        public bool IsPaymentViaBank { get; set; }
        public string CardToken { get; set; }
        public string clientTokenPG { get; set; }
        public string CCCode { get; set; }
        public string IsCardDetailToSave { get; set; }
        public string ProgramAccountIdSelected { get; set; }
        public bool IsCardSelectionFromDropdown { get; set; }
        public bool IsNewCardTransaction { get; set; }
        public string ipgFirstTransactionId { get; set; }
        public BankAccountModel bankAccount { get; set; }
        public string schemetransactionID { get; set; }
        public int linkedUserId { get; set; }

    }
}
