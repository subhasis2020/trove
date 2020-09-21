using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class OfferSubType
    {
        public OfferSubType()
        {
            Offer = new HashSet<Offer>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OfferTypeId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? OfferCodeId { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual OfferCode OfferCode { get; set; }
        public virtual OfferType OfferType { get; set; }
        public virtual ICollection<Offer> Offer { get; set; }
    }
}
