using Foundry.Domain.DbModel;
using System;
using System.Collections.Generic;

namespace Foundry.Domain.Dto
{
    public class OfferDto
    {
        public int OrganisationId { get; set; }
        public int OfferId { get; set; }
        public string OrganisationName { get; set; }
        public int BannnerTypeId { get; set; }
        public string OfferText { get; set; }
        public string OfferValue { get; set; }
        public string OfferImagePath { get; set; }
        public double Distance { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int OfferTypeId { get; set; }
        public int OfferSubTypeId { get; set; }
        public OrganisationScheduleAndHolidayDto organisationScheduleAndHoliday { get; set; }

        /* New Columns Added */
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool IsMapVisible { get; set; }
        public bool IsClosed { get; set; }
        public bool isTrafficChartVisible { get; set; }

    }
}
