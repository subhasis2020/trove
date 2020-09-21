using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class OfferMerchant
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public int? MerchantId { get; set; }
        public DateTime? OfferLeftDate { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual Organisation Merchant { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual Offer Offer { get; set; }
    }
}
