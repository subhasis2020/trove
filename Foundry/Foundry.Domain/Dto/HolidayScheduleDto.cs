using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class HolidayScheduleDto
    {
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public string HolidayName { get; set; }
        public DateTime HolidayDate { get; set; }
        public bool IsForHolidayNameToShow { get; set; }
    }
}
