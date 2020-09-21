using System;

namespace Foundry.Domain.ApiModel
{
    public class ReloadRequestModel
    {
        public int? ReloadRequestId { get; set; }
        public int ReloadUserId { get; set; }
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
        public decimal CheckDroppedAmount { get; set; }
    }
}
