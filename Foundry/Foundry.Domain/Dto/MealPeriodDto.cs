using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class MealPeriodDto
    {
        public int id { get; set; }
        public string title { get; set; }
        public int? organisationId { get; set; }
        public TimeSpan? openTime { get; set; }
        public TimeSpan? closeTime { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
	    public string days { get; set; }
        public bool? isSelected { get; set; }
        public IEnumerable<string> Selecteddays { get; set; }
    }
}
