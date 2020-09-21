using Foundry.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Text;


namespace Foundry.Domain.ApiModel
{
    public class OrganisationBusinessInformationModel
    {
        public int Id { get; set; }
        public int CloneId { get; set; }
        public List<OrganisationScheduleModel> HoursOfOperation { get; set; }
        public List<OrganisationScheduleModel> HolidayHours { get; set; }
        public OrganisationViewModel Merchant { get; set; }
        public List<OrganisationMerchantTerminalModel> MerchantTerminal { get; set; }
        public List<OrganisationMealPeriodModel> MealPeriod { get; set; }
    }
}
