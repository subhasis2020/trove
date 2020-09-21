using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class OrganisationDetailWithMasterDto
    {
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ContactNumber { get; set; }
        public int PrimaryProgramId { get; set; }
        public string PrimaryProgramName { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string FacebookURL { get; set; }
        public string TwitterURL { get; set; }
        public string SkypeHandle { get; set; }
        public string InstagramHandle { get; set; }
        public int BusinessTypeId { get; set; }
        public bool ShowMap { get; set; }
        public string ImagePath { get; set; }
        public string Location { get; set; }
        public List<ProgramDto> Program { get; set; }
        public List<AccountTypeDto> AccType { get; set; }
        public List<OrganisationProgramDto> OrgProgram { get; set; }
        public List<AccountTypeDto> OrgAccType { get; set; }
        public List<BusinessTypeDto> BusinessType { get; set; }
        public string ImageFileName { get; set; }
        public string Jpos_MerchantId { get; set; }
    }
}
