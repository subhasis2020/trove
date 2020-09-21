using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class OfferCodeDto
    {
        public int id { get; set; }
        public string offerName { get; set; }
        public string offerIconPath { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public bool isCheckedCodeOffer { get; set; } = false;
    }
}
