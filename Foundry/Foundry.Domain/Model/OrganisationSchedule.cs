using System;
using System.Collections.Generic;

namespace Foundry.Domain.Model
{
    public partial class OrganisationSchedule
    {
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public string WorkingDay { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? ClosedTime { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
