using Foundry.Domain.DbModel;
using System.Collections.Generic;

namespace Foundry.Domain.Dto
{
    public class OrganisationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Location { get; set; }
        public int OrganisationType { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNumber { get; set; }
        public string LogoPath { get; set; }
        public string ClosingStatus { get; set; }
        public string OpenCloseTime { get; set; }
        public double Distance { get; set; }
        public bool IsFavorite { get; set; }
        public bool? IsMaster { get; set; }
        public OrganisationScheduleAndHolidayDto organisationScheduleAndHoliday { get; set; }
        public int? MaxCapacity { get; set; }
        public string Description { get; set; }
        public string ContactTitle { get; set; }
        /*New Column Added*/
        public string OrganisationSubTitle { get; set; }
        public bool IsClosed { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool IsMapVisible { get; set; }
        public int? dwellTime { get; set; }
        public int MaxSeatCapacityOccupied { get; set; }
        public bool isTrafficChartVisible { get; set; }
        public string JPOS_MerchantId { get; set; }

    }


    public class OrganisationMainDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Location { get; set; }
        public int OrganisationType { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNumber { get; set; }
        public string LogoPath { get; set; }
        public string ClosingStatus { get; set; }
        public string OpenCloseTime { get; set; }
        public double Distance { get; set; }
        public bool IsFavorite { get; set; }
        public bool? IsMaster { get; set; }
        public OrganisationScheduleAndHolidayDto organisationScheduleAndHoliday { get; set; }
        public int? MaxCapacity { get; set; }
        public string Description { get; set; }
        public string ContactTitle { get; set; }
        /*New Column Added*/
        public string OrganisationSubTitle { get; set; }
        public bool IsClosed { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool IsMapVisible { get; set; }
        public int? dwellTime { get; set; }
        public int MaxSeatCapacityOccupied { get; set; }
        public bool? isActive { get; set; }
        public bool? isDeleted { get; set; }

    }

    public class OrganisationDrpDto {
        public int id { get; set; }
        public string name { get; set; }

    }
}
