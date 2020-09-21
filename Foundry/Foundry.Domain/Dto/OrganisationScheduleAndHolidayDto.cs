using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
   public class OrganisationScheduleAndHolidayDto
    {
        public List<OrganisationScheduleDto> OrganisationSchedule { get; set; }
        public HolidayScheduleDto HolidaySchedule { get; set; }
    }
}
