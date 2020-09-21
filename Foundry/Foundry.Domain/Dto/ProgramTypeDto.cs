using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Domain.Dto
{
    public class ProgramTypeDto
    {
        public int ProgramTypeId { get; set; }
        public int OrganisationId { get; set; }
    }

    public class ProgramTypeIdDto
    {
        public int ProgramTypeId { get; set; }     
    }

    public class ProgramIdDto
    {
        public int ProgramTypeId { get; set; }
    }
    public class PlanIdDto
    {
        public int PlanId { get; set; }
    }

    public class MercahntIdDto
    {
        public int merchantId { get; set; }
    }
    public class AdminProgramIdDto
    {
        public int programId { get; set; }
    }
}
