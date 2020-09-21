﻿using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class OfferType
    {
        public OfferType()
        {
            Offer = new HashSet<Offer>();
            OfferSubType = new HashSet<OfferSubType>();
        }

        public int Id { get; set; }
        public string OfferType1 { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual ICollection<Offer> Offer { get; set; }
        public virtual ICollection<OfferSubType> OfferSubType { get; set; }
    }
}
