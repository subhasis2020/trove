using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class Offer
    {
        public Offer()
        {
            OfferGroup = new HashSet<OfferGroup>();
            OfferMerchant = new HashSet<OfferMerchant>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ProgramId { get; set; }
        public int OfferTypeId { get; set; }
        public int OfferSubTypeId { get; set; }
        public int? BuyQuantity { get; set; }
        public int? GetQuantity { get; set; }
        public int? VisitNumber { get; set; }
        public int? FreeQuantity { get; set; }
        public decimal? DiscountInPercentage { get; set; }
        public decimal? DiscountInCash { get; set; }
        public bool? IsCouponValid { get; set; }
        public string CouponCode { get; set; }
        public DateTime? OfferValidFrom { get; set; }
        public DateTime? OfferValidTill { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? MerchantId { get; set; }
        public string OfferDayName { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual OfferSubType OfferSubType { get; set; }
        public virtual OfferType OfferType { get; set; }
        public virtual Program Program { get; set; }
        public virtual ICollection<OfferGroup> OfferGroup { get; set; }
        public virtual ICollection<OfferMerchant> OfferMerchant { get; set; }
    }
}
