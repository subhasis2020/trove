using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundry.Domain.Dto;

namespace Foundry.Web.Models
{
    public class MerchantBusinessInfoModel
    {
        public string Id { get; set; }
        public List<OrganisationScheduleDto> HoursOfOperation { get; set; }
        public List<OrganisationScheduleDto> HolidayHours { get; set; }
        public MerchantBusinessDto Merchant { get; set; }
        public List<MerchantTerminalDto> MerchantTerminal { get; set; }
        public List<MealPeriodDto> MealPeriod { get; set; }
        public List<WeekDayDto> WeekDays { get; set; }
        public List<DwellTimeDto> DwellTime { get; set; }
        public List<TerminalTypeDto> TerminalType { get; set; }
    }
}
