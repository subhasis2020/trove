using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class OrgnisationWithProgramTypeDto
    {
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public string OrganisationSubTitle { get; set; }
        public List<ProgramTypeIdDto> OrganisationProgramType { get; set; }
        public string FacebookURL { get; set; }
        public string TwitterURL { get; set; }
        public string SkypeHandle { get; set; }
        public string JPOS_MerchantId { get; set; }
    }
}
