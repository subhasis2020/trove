using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;

namespace Foundry.Domain.Dto
{
    public class OrganisationScheduleDto
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
        /*New Columns Added*/
        public bool? IsHoliday { get; set; }
        public string HolidayDate { get; set; }
        public bool IsForHolidayNameToShow { get; set; }
        public string HolidayName { get; set; }
        public int S_No { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual User ModifiedByNavigation { get; set; }
        public virtual Organisation Organisation { get; set; }
    }

}
