using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class OfferGroup
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public int GroupId { get; set; }

        public virtual Group Group { get; set; }
        public virtual Offer Offer { get; set; }
    }
}
