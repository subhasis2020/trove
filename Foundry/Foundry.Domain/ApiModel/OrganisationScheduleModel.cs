using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.ApiModel
{
    public class OrganisationScheduleModel
    {
        public int id { get; set; }
        public int organisationId { get; set; }
        public string workingDay { get; set; }
        public TimeSpan? openTime { get; set; }
        public TimeSpan? closedTime { get; set; }
        public bool? isActive { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public int? modifiedBy { get; set; }
        public DateTime? modifiedDate { get; set; }
        public bool? isDeleted { get; set; }
        public bool? isHoliday { get; set; }
        public DateTime? holidayDate { get; set; }
        public bool IsForHolidayNameToShow { get; set; }
        public string HolidayName { get; set; }
    }
}
